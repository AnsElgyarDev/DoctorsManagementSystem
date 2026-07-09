using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DoctorsManagementSystem.Desktop.Services;
using DoctorsManagementSystem.Dto;
using Microsoft.Extensions.Logging;

namespace DoctorsManagementSystem.Desktop.ViewModels.Dialogs;

public partial class AddOperationViewModel : ObservableObject
{
    private readonly IPatientApiClient _patientApiClient;
    private readonly ILogger<AddOperationViewModel> _logger;
    private int _patientId;

    public AddOperationViewModel(IPatientApiClient patientApiClient, ILogger<AddOperationViewModel> logger)
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

    [ObservableProperty] private string operationName = string.Empty;
    [ObservableProperty] private string notes = string.Empty;
    [ObservableProperty] private decimal operationCost;
    [ObservableProperty] private DateTime sessionDate;
    [ObservableProperty] private bool isSaving;
    [ObservableProperty] private string errorMessage = string.Empty;

    public event Action? Saved;
    public event Action? Cancelled;

    [RelayCommand]
    private async Task SaveAsync()
    {
        ErrorMessage = string.Empty;

        var dto = new OperationDto
        {
            OperationName = OperationName,
            Notes = Notes,
            OperationCost = OperationCost,
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
            await _patientApiClient.AddOperationAsync(_patientId, dto);
            Saved?.Invoke();
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "Failed to add operation for patient {PatientId}.", _patientId);
            ErrorMessage = ex.Message;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error adding operation for patient {PatientId}.", _patientId);
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