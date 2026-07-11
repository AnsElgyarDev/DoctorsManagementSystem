using DoctorsManagementSystem.Data;
using DoctorsManagementSystem.Shared.Dtos;
using Microsoft.EntityFrameworkCore;

namespace DoctorsManagementSystem.Service;

class DashboardServices : IDashboardServices
{
    private readonly AppDbContext _Context;

    public DashboardServices(AppDbContext context)
    {
        _Context = context;
    }

    public DashboardStatsDto GetDashboardStats()
    {
        var today = DateTime.Today;
        var monthStart = new DateTime(today.Year, today.Month, 1);
        var monthEnd = monthStart.AddMonths(1);

        var todayAppointmentsCount = _Context.Prescriptions
            .Count(p => p.SessionDate.Date == today);

        var upcomingOperationsCount = _Context.Operations
            .Count(o => o.SessionDate.Date >= today);

        var prescriptionRevenue = _Context.Prescriptions
            .Where(p => p.SessionDate >= monthStart && p.SessionDate < monthEnd)
            .Sum(p => (decimal?)p.SurgeryBill) ?? 0m;

        var operationRevenue = _Context.Operations
            .Where(o => o.SessionDate >= monthStart && o.SessionDate < monthEnd)
            .Sum(o => (decimal?)o.OperationCost) ?? 0m;

        var monthlyRevenue = prescriptionRevenue + operationRevenue;

        var latestPatients = _Context.Patients
            .OrderByDescending(p => p.RegisteredAt)
            .Take(5)
            .Select(p => new LatestPatientDto
            {
                PatientId = p.PatientId,
                PatientName = p.PatientName,
                PatientPhone = p.PatientPhone,
                RegisteredAt = p.RegisteredAt
            })
            .ToList();

        return new DashboardStatsDto
        {
            TodayAppointmentsCount = todayAppointmentsCount,
            MonthlyRevenue = monthlyRevenue,
            UpcomingOperationsCount = upcomingOperationsCount,
            LatestPatients = latestPatients
        };
    }
}