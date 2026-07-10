using DoctorsManagementSystem.Dto;

namespace DoctorsManagementSystem.Service;

public interface IDashboardServices
{
    DashboardStatsDto GetDashboardStats();
}