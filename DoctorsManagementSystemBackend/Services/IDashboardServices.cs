using DoctorsManagementSystem.Dto;
using DoctorsManagementSystem.Shared.Dtos;

namespace DoctorsManagementSystem.Service;

public interface IDashboardServices
{
    DashboardStatsDto GetDashboardStats();
}