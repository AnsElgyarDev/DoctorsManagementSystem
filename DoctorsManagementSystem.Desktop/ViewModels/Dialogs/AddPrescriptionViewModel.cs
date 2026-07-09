using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DoctorsManagementSystem.Desktop.Services;
using DoctorsManagementSystem.Dto;
using Microsoft.Extensions.Logging;

namespace DoctorsManagementSystem.Desktop.ViewModels.Dialogs;

public partial class AddPrescriptionViewModel : ObservableObject
{
    private readonly IPatientApiClient _patientApiClient;
    private readonly ILogger<AddPrescriptionViewModel> _logger;
    private int _patientId;

    public AddPrescriptionViewModel(IPatientApiClient patientApiClient, ILogger<AddPrescriptionViewModel> logger)
    {
        _patientApiClient = patientApiClient;
        _logger = logger;
        SessionDate = DateTime.Today;
    }

    /// <summary>Called by PatientDetailsViewModel right after resolving this VM from DI.</summary>
    public void Initialize(int patientId)
    {
        _patientId = patientId;
    }

    [ObservableProperty] private string surgeryName = string.Empty;
    [ObservableProperty] private string surgeryNotes = string.Empty;
    [ObservableProperty] private decimal surgeryBill;
    [ObservableProperty] private DateTime sessionDate;
    [ObservableProperty] private bool isSaving;
    [ObservableProperty] private string errorMessage = string.Empty;

    public event Action? Saved;
    public event Action? Cancelled;

    [RelayCommand]
    private async Task SaveAsync()
    {
        ErrorMessage = string.Empty;

        var dto = new PrescriptionDto
        {
            SurgeryName = SurgeryName,
            SurgeryNotes = SurgeryNotes,
            SurgeryBill = SurgeryBill,
            SessionDate = SessionDate
        };

        var validationContext = new ValidationContext(dto);
        var validationResults = new List<ValidationResult>();

        if (!Validator.TryValidateObject(dto, validationContext, validationResults, validateAllProperties: true))
        {
            ErrorMessage = string.Join(" ", validationResults.Select(r => r.ErrorMessage));
            return;
        }

        IsSaving = true;

        try
        {
            await _patientApiClient.AddPrescriptionAsync(_patientId, dto);
            Saved?.Invoke();
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "Failed to add prescription for patient {PatientId}.", _patientId);
            ErrorMessage = ex.Message;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error adding prescription for patient {PatientId}.", _patientId);
            ErrorMessage = "Something went wrong while saving. Please try again.";
        }
        finally
        {
            IsSaving = false;
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        Cancelled?.Invoke();
    }
}