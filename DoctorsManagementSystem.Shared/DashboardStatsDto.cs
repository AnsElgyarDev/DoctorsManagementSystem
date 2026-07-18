namespace DoctorsManagementSystem.Shared.Dtos;


public record DashboardStatsDto
{
    public decimal MonthlyRevenue { get; init; }
    public int UpcomingOperationsCount { get; init; }
    public List<LatestPatientDto> LatestPatients { get; init; } = new();
}

public record LatestPatientDto
{
    public int PatientId { get; init; }
    public string PatientName { get; init; } = string.Empty;
    public string PatientPhone { get; init; } = string.Empty;
    public DateTime RegisteredAt { get; init; }
}