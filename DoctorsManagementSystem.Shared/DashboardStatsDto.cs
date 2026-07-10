namespace DoctorsManagementSystem.Dto;

public record DashboardStatsDto
{
    public int TotalPatients { get; init; }
    public decimal TotalRevenue { get; init; }
    public List<RecentPatientDto> RecentPatients { get; init; } = new();
}

public record RecentPatientDto
{
    public int PatientId { get; init; }
    public string PatientName { get; init; } = string.Empty;
    public string PatientPhone { get; init; } = string.Empty;
    public decimal TotalBill { get; init; }
}