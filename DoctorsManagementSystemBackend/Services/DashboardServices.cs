using DoctorsManagementSystem.Data;
using DoctorsManagementSystem.Dto;
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
        var totalPatients = _Context.Patients.Count();
        var totalRevenue = _Context.Patients.Sum(p => (decimal?)p.TotalBill) ?? 0m;

        // No CreatedAt/RegisteredAt column exists on Patient yet, so "recent" is
        // approximated by PatientId descending (higher ID = registered later).
        var recentPatients = _Context.Patients
            .OrderByDescending(p => p.PatientId)
            .Take(5)
            .Select(p => new RecentPatientDto
            {
                PatientId = p.PatientId,
                PatientName = p.PatientName,
                PatientPhone = p.PatientPhone,
                TotalBill = p.TotalBill
            })
            .ToList();

        return new DashboardStatsDto
        {
            TotalPatients = totalPatients,
            TotalRevenue = totalRevenue,
            RecentPatients = recentPatients
        };
    }
}