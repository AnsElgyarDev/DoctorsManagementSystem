using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DoctorsManagementSystem.Desktop.Models;
using DoctorsManagementSystem.Desktop.Services;
using DoctorsManagementSystem.Desktop.ViewModels.Dialogs;
using DoctorsManagementSystem.Desktop.Views.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace DoctorsManagementSystem.Desktop.ViewModels.Pages;

public partial class PatientsViewModel : ObservableObject
{
    private readonly IPatientApiClient _patientApiClient;
    private readonly ILogger<PatientsViewModel> _logger;
    private readonly Infrastructure.Navigation.INavigationService _navigationService;
    private readonly IContentDialogService _contentDialogService;
    private readonly IServiceProvider _serviceProvider;

public PatientsViewModel(
    IPatientApiClient patientApiClient,
    ILogger<PatientsViewModel> logger,
    IContentDialogService contentDialogService,
    IServiceProvider serviceProvider,
    Infrastructure.Navigation.INavigationService navigationService)
    {
        _patientApiClient = patientApiClient;
        _logger = logger;
        _contentDialogService = contentDialogService;
        _serviceProvider = serviceProvider;
        _navigationService = navigationService; 
    }
    [ObservableProperty]
    private ObservableCollection<Patient> patients = new();

    [ObservableProperty]
    private PatientsLoadState state = PatientsLoadState.Loading;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    [RelayCommand]
    private async Task LoadPatientsAsync()
    {
        State = PatientsLoadState.Loading;
        ErrorMessage = string.Empty;

        try
        {
            var result = await _patientApiClient.GetAllPatientsAsync();

            Patients.Clear();
            foreach (var patient in result)
            {
                Patients.Add(patient);
            }

            State = Patients.Count == 0 ? PatientsLoadState.Empty : PatientsLoadState.Loaded;
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "Failed to load patients.");
            ErrorMessage = ex.Message;
            State = PatientsLoadState.Error;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while loading patients.");
            ErrorMessage = "Something went wrong while loading patients. Please try again.";
            State = PatientsLoadState.Error;
        }
    }
    [RelayCommand]
private async Task DeletePatientAsync(Patient patient)
{
    var confirmDialog = new ContentDialog
    {
        Title = "Delete Patient",
        Content = $"Are you sure you want to delete {patient.PatientName}? This action cannot be undone.",
        PrimaryButtonText = "Delete",
        CloseButtonText = "Cancel"
    };

    var result = await _contentDialogService.ShowAsync(confirmDialog, CancellationToken.None);

    if (result != ContentDialogResult.Primary)
        return; 

    try
    {
        await _patientApiClient.DeletePatientAsync(patient.PatientId);

        Patients.Remove(patient);

        if (Patients.Count == 0)
            State = PatientsLoadState.Empty;
    }
    catch (ApiException ex)
    {
        _logger.LogError(ex, "Failed to delete patient {PatientId}.", patient.PatientId);
        await ShowErrorDialogAsync(ex.Message);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Unexpected error deleting patient {PatientId}.", patient.PatientId);
        await ShowErrorDialogAsync("Something went wrong while deleting this patient. Please try again.");
    }
}

private async Task ShowErrorDialogAsync(string message)
{
    var errorDialog = new ContentDialog
    {
        Title = "Couldn't Delete Patient",
        Content = message,
        CloseButtonText = "OK"
    };
    await _contentDialogService.ShowAsync(errorDialog, CancellationToken.None);
}

[RelayCommand]
private async Task OpenAddPatientDialogAsync()
{
    var addPatientViewModel = _serviceProvider.GetRequiredService<AddPatientViewModel>();
    addPatientViewModel.InitializeForAdd();
    
    await ShowPatientFormDialogAsync(addPatientViewModel);
}

[RelayCommand]
private async Task OpenEditPatientDialogAsync(Patient patient)
{
    var editPatientViewModel = _serviceProvider.GetRequiredService<AddPatientViewModel>();
    editPatientViewModel.InitializeForEdit(patient);
    
    await ShowPatientFormDialogAsync(editPatientViewModel);
}

private async Task ShowPatientFormDialogAsync(AddPatientViewModel formViewModel)
{
    var dialogContent = new AddPatientDialogContent(formViewModel);
    var dialog = new ContentDialog { Title = formViewModel.DialogTitle, Content = dialogContent, CloseButtonText = "Close" };

    async void OnSaved() { dialog.Hide(); await LoadPatientsAsync(); }
    
    void OnCancelled() => dialog.Hide();

    formViewModel.Saved += OnSaved;
    formViewModel.Cancelled += OnCancelled;
    
    await _contentDialogService.ShowAsync(dialog, CancellationToken.None);

    formViewModel.Saved -= OnSaved;
    formViewModel.Cancelled -= OnCancelled;
}

    [RelayCommand]
    private void OpenPatientDetails(Patient patient)
    {
        _navigationService.SetPendingParameter(patient.PatientId);
        _navigationService.Navigate<Views.Pages.PatientDetailsPage>();
    }
}