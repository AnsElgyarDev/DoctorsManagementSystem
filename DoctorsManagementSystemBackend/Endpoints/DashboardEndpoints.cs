using DoctorsManagementSystem.Service;
using DoctorsManagementSystem.Dto;
using Microsoft.AspNetCore.Http.HttpResults;
using DoctorsManagementSystem.Shared.Dtos;

namespace DoctorsManagementSystem.Endpoints;

public static class DashboardEndpoints
{
    public static void UseDashboardEndpoints(this WebApplication app)
    {
        app.MapGet("/Dashboard/Stats", Results<Ok<DashboardStatsDto>, NotFound<string>>
                  (IDashboardServices dashboardServices) =>
        {
            var stats = dashboardServices.GetDashboardStats();
            return TypedResults.Ok(stats);
        });
    }
}