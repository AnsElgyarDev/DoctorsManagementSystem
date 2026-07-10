using DoctorsManagementSystem.Desktop.Models;

namespace DoctorsManagementSystem.Desktop.Services;

public interface IDashboardApiClient
{
    Task<DashboardStats> GetDashboardStatsAsync(CancellationToken cancellationToken = default);
}