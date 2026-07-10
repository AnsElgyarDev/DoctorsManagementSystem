using System.Text.Json.Serialization;

namespace DoctorsManagementSystem.Desktop.Models;

public class DashboardStats
{
    [JsonPropertyName("totalPatients")]
    public int TotalPatients { get; set; }

    [JsonPropertyName("totalRevenue")]
    public decimal TotalRevenue { get; set; }

    [JsonPropertyName("recentPatients")]
    public List<RecentPatient> RecentPatients { get; set; } = new();
}

public class RecentPatient
{
    [JsonPropertyName("patientId")]
    public int PatientId { get; set; }

    [JsonPropertyName("patientName")]
    public string PatientName { get; set; } = string.Empty;

    [JsonPropertyName("patientPhone")]
    public string PatientPhone { get; set; } = string.Empty;

    [JsonPropertyName("totalBill")]
    public decimal TotalBill { get; set; }
}