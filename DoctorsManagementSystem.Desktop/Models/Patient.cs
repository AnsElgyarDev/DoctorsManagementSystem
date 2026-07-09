using System.Text.Json.Serialization;

namespace DoctorsManagementSystem.Desktop.Models;

/// <summary>
/// Client-side read model matching the JSON actually returned by
/// GET /Patients and GET /Patients/{id}. The backend serializes its EF Core
/// `Patient` entity directly for reads (not PatientDto), so this mirrors
/// that entity's real shape. There is no Gender/Age field in the current
/// backend schema — don't add them here without adding them server-side first.
/// </summary>
public class Patient
{
    [JsonPropertyName("patientId")]
    public int PatientId { get; set; }

    [JsonPropertyName("patientName")]
    public string PatientName { get; set; } = string.Empty;

    [JsonPropertyName("totalBill")]
    public decimal TotalBill { get; set; }

    [JsonPropertyName("patientEmail")]
    public string? PatientEmail { get; set; }

    [JsonPropertyName("patientPhone")]
    public string PatientPhone { get; set; } = string.Empty;

    [JsonPropertyName("prescriptions")]
    public List<PrescriptionSummary> Prescriptions { get; set; } = new();
}

/// <summary>
/// Minimal shape for the nested prescriptions array. GET /Patients does not
/// eager-load this collection server-side, so expect it to arrive empty for
/// now — full prescription data comes from a dedicated endpoint in a later step.
/// </summary>
public class PrescriptionSummary
{
    [JsonPropertyName("prescriptionId")]
    public int PrescriptionId { get; set; }

    [JsonPropertyName("surgeryName")]
    public string SurgeryName { get; set; } = string.Empty;

    [JsonPropertyName("surgeryBill")]
    public decimal SurgeryBill { get; set; }

    [JsonPropertyName("sessionDate")]
    public DateTime SessionDate { get; set; }
}