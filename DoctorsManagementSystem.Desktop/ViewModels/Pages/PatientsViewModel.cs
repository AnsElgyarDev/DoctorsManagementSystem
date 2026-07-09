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
    private readonly IContentDialogService _contentDialogService;
    private readonly IServiceProvider _serviceProvider;

    public PatientsViewModel(
        IPatientApiClient patientApiClient,
        ILogger<PatientsViewModel> logger,
        IContentDialogService contentDialogService,
        IServiceProvider serviceProvider)
    {
        _patientApiClient = patientApiClient;
        _logger = logger;
        _contentDialogService = contentDialogService;
        _serviceProvider = serviceProvider;
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
    private async Task OpenAddPatientDialogAsync()
    {
        var addPatientViewModel = _serviceProvider.GetRequiredService<AddPatientViewModel>();
        var dialogContent = new AddPatientDialogContent(addPatientViewModel);

        var dialog = new ContentDialog
        {
            Title = "Add New Patient",
            Content = dialogContent,
            CloseButtonText = "Close"
        };

        void OnSaved(Patient createdPatient)
        {
            Patients.Add(createdPatient);
            State = PatientsLoadState.Loaded;
            dialog.Hide();
        }

        void OnCancelled()
        {
            dialog.Hide();
        }

        addPatientViewModel.Saved += OnSaved;
        addPatientViewModel.Cancelled += OnCancelled;

        await _contentDialogService.ShowAsync(dialog, CancellationToken.None);

        addPatientViewModel.Saved -= OnSaved;
        addPatientViewModel.Cancelled -= OnCancelled;
    }
}