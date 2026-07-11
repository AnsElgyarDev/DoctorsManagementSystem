using System.Text.Json.Serialization;

namespace DoctorsManagementSystem.Desktop.Models;

public class DashboardStats
{
    [JsonPropertyName("todayAppointmentsCount")]
    public int TodayAppointmentsCount { get; set; }

    [JsonPropertyName("monthlyRevenue")]
    public decimal MonthlyRevenue { get; set; }

    [JsonPropertyName("upcomingOperationsCount")]
    public int UpcomingOperationsCount { get; set; }

    [JsonPropertyName("latestPatients")]
    public List<LatestPatient> LatestPatients { get; set; } = new();
}

public class LatestPatient
{
    [JsonPropertyName("patientId")]
    public int PatientId { get; set; }

    [JsonPropertyName("patientName")]
    public string PatientName { get; set; } = string.Empty;

    [JsonPropertyName("patientPhone")]
    public string PatientPhone { get; set; } = string.Empty;

    [JsonPropertyName("registeredAt")]
    public DateTime RegisteredAt { get; set; }
}