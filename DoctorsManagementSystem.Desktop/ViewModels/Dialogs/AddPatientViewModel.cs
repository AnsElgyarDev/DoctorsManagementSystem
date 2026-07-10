using System.ComponentModel.DataAnnotations;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DoctorsManagementSystem.Desktop.Models;
using DoctorsManagementSystem.Desktop.Services;
using DoctorsManagementSystem.Dto;
using Microsoft.Extensions.Logging;

namespace DoctorsManagementSystem.Desktop.ViewModels.Dialogs;

public partial class AddPatientViewModel : ObservableObject
{
    private readonly IPatientApiClient _patientApiClient;
    private readonly ILogger<AddPatientViewModel> _logger;
    private int? _editingPatientId;

    public AddPatientViewModel(IPatientApiClient patientApiClient, ILogger<AddPatientViewModel> logger)
    {
        _patientApiClient = patientApiClient;
        _logger = logger;
        InitializeForAdd();
    }

    [ObservableProperty] private bool isEditMode;
    [ObservableProperty] private string dialogTitle = "Add New Patient";

    [ObservableProperty] private string patientName = string.Empty;
    [ObservableProperty] private string patientPhone = string.Empty;
    [ObservableProperty] private string? patientEmail;
    [ObservableProperty] private decimal totalBill;

    [ObservableProperty] private string surgeryName = string.Empty;
    [ObservableProperty] private string surgeryNotes = string.Empty;
    [ObservableProperty] private decimal surgeryBill;
    [ObservableProperty] private DateTime sessionDate;

    [ObservableProperty] private bool isSaving;
    [ObservableProperty] private string errorMessage = string.Empty;

    public bool ShowPrescriptionSection => !IsEditMode;
    public string SaveButtonText => IsEditMode ? "Save Changes" : "Save Patient";

    partial void OnIsEditModeChanged(bool value)
    {
        OnPropertyChanged(nameof(ShowPrescriptionSection));
        OnPropertyChanged(nameof(SaveButtonText));
    }

    public void InitializeForAdd()
    {
        _editingPatientId = null;
        IsEditMode = false;
        DialogTitle = "Add New Patient";
        PatientName = string.Empty;
        PatientPhone = string.Empty;
        PatientEmail = null;
        TotalBill = 0;
        SurgeryName = string.Empty;
        SurgeryNotes = string.Empty;
        SurgeryBill = 0;
        SessionDate = DateTime.Today;
        ErrorMessage = string.Empty;
    }

    public void InitializeForEdit(Patient patient)
    {
        _editingPatientId = patient.PatientId;
        IsEditMode = true;
        DialogTitle = $"Edit {patient.PatientName}";
        PatientName = patient.PatientName;
        PatientPhone = patient.PatientPhone;
        PatientEmail = patient.PatientEmail;
        TotalBill = patient.TotalBill;
        ErrorMessage = string.Empty;
    }

    public event Action? Saved;
    public event Action? Cancelled;

    [RelayCommand]
    private async Task SaveAsync()
    {
        ErrorMessage = string.Empty;
        if (IsEditMode) await SaveEditAsync();
        else await SaveNewAsync();
    }

    private async Task SaveNewAsync()
    {
        var dto = new PatientDto
        {
            PatientName = PatientName,
            PatientPhone = PatientPhone,
            PatientEmail = string.IsNullOrWhiteSpace(PatientEmail) ? null : PatientEmail,
            TotalBill = TotalBill,
            Prescriptions = new List<PrescriptionDto>
            {
                new PrescriptionDto { SurgeryName = SurgeryName, SurgeryNotes = SurgeryNotes, SurgeryBill = SurgeryBill, SessionDate = SessionDate }
            }
        };

        if (!TryValidate(dto, out var validationError)) { ErrorMessage = validationError; return; }
        IsSaving = true;
        try { await _patientApiClient.CreatePatientAsync(dto); Saved?.Invoke(); }
        catch (ApiException ex) { ErrorMessage = ex.Message; }
        catch (Exception) { ErrorMessage = "Something went wrong while saving."; }
        finally { IsSaving = false; }
    }

    private async Task SaveEditAsync()
    {
        var dto = new PatientUpdateDto
        {
            PatientName = PatientName,
            PatientPhone = PatientPhone,
            PatientEmail = string.IsNullOrWhiteSpace(PatientEmail) ? null : PatientEmail,
            TotalBill = TotalBill
        };

        if (!TryValidate(dto, out var validationError)) { ErrorMessage = validationError; return; }
        IsSaving = true;
        try 
        { 
            await _patientApiClient.UpdatePatientAsync(_editingPatientId!.Value, dto); 
            Saved?.Invoke(); 
        }
        catch (ApiException ex) { ErrorMessage = ex.Message; }
        catch (Exception) { ErrorMessage = "Something went wrong while updating."; }
        finally { IsSaving = false; }
    }

    private static bool TryValidate(object dto, out string errorMessage)
    {
        var validationContext = new ValidationContext(dto);
        var validationResults = new List<ValidationResult>();
        if (!Validator.TryValidateObject(dto, validationContext, validationResults, true))
        {
            errorMessage = string.Join(" ", validationResults.Select(r => r.ErrorMessage));
            return false;
        }
        errorMessage = string.Empty;
        return true;
    }

    [RelayCommand] private void Cancel() => Cancelled?.Invoke();
}