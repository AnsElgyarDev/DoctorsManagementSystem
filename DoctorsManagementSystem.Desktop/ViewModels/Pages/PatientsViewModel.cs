using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DoctorsManagementSystem.Desktop.Models;
using DoctorsManagementSystem.Desktop.Services;
using Microsoft.Extensions.Logging;

namespace DoctorsManagementSystem.Desktop.ViewModels.Pages;

public partial class PatientsViewModel : ObservableObject
{
    private readonly IPatientApiClient _patientApiClient;
    private readonly ILogger<PatientsViewModel> _logger;

    public PatientsViewModel(IPatientApiClient patientApiClient, ILogger<PatientsViewModel> logger)
    {
        _patientApiClient = patientApiClient;
        _logger = logger;
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
}