using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DoctorsManagementSystem.Desktop.Infrastructure.Navigation;
using DoctorsManagementSystem.Desktop.Models;
using DoctorsManagementSystem.Desktop.Services;
using DoctorsManagementSystem.Desktop.ViewModels.Dialogs;
using DoctorsManagementSystem.Desktop.Views.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace DoctorsManagementSystem.Desktop.ViewModels.Pages;

public partial class PatientDetailsViewModel : ObservableObject
{
    private readonly IPatientApiClient _patientApiClient;
    private readonly ILogger<PatientDetailsViewModel> _logger;
    private readonly IContentDialogService _contentDialogService;
    private readonly IServiceProvider _serviceProvider;
    private readonly Infrastructure.Navigation.INavigationService _navigationService;

    public PatientDetailsViewModel(
        IPatientApiClient patientApiClient,
        ILogger<PatientDetailsViewModel> logger,
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

    private int _patientId;

    [ObservableProperty]
    private Patient? patient;

    [ObservableProperty]
    private ObservableCollection<PrescriptionSummary> prescriptions = new();

    [ObservableProperty]
    private PatientDetailsLoadState state = PatientDetailsLoadState.Loading;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    /// <summary>Called by PatientDetailsPage right after construction, before loading starts.</summary>
    public void Initialize(int patientId)
    {
        _patientId = patientId;
    }

    [RelayCommand]
    private async Task LoadPatientAsync()
    {
        State = PatientDetailsLoadState.Loading;
        ErrorMessage = string.Empty;

        try
        {
            var patientTask = _patientApiClient.GetPatientByIdAsync(_patientId);
            var prescriptionsTask = _patientApiClient.GetPatientPrescriptionsAsync(_patientId);

            await Task.WhenAll(patientTask, prescriptionsTask);

            Patient = patientTask.Result;

            Prescriptions.Clear();
            foreach (var prescription in prescriptionsTask.Result)
            {
                Prescriptions.Add(prescription);
            }

            State = PatientDetailsLoadState.Loaded;
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "Failed to load patient {PatientId}.", _patientId);
            ErrorMessage = ex.Message;
            State = PatientDetailsLoadState.Error;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error loading patient {PatientId}.", _patientId);
            ErrorMessage = "Something went wrong while loading this patient. Please try again.";
            State = PatientDetailsLoadState.Error;
        }
    }

    [RelayCommand]
    private async Task OpenAddPrescriptionDialogAsync()
    {
        var addPrescriptionViewModel = _serviceProvider.GetRequiredService<AddPrescriptionViewModel>();
        addPrescriptionViewModel.Initialize(_patientId);

        var dialogContent = new AddPrescriptionDialogContent(addPrescriptionViewModel);

        var dialog = new ContentDialog
        {
            Title = "Add New Prescription",
            Content = dialogContent,
            CloseButtonText = "Close"
        };

        async void OnSaved()
        {
            dialog.Hide();
            await LoadPatientAsync(); // refresh the prescription list from the server
        }

        void OnCancelled()
        {
            dialog.Hide();
        }

        addPrescriptionViewModel.Saved += OnSaved;
        addPrescriptionViewModel.Cancelled += OnCancelled;

        await _contentDialogService.ShowAsync(dialog, CancellationToken.None);

        addPrescriptionViewModel.Saved -= OnSaved;
        addPrescriptionViewModel.Cancelled -= OnCancelled;
    }

    [RelayCommand]
    private void GoBack()
    {
        _navigationService.Navigate<Views.Pages.PatientsPage>();
    }
}