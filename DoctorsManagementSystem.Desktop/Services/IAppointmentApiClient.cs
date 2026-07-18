using DoctorsManagementSystem.Dto;
using DoctorsManagementSystem.Shared.Dtos;

namespace DoctorsManagementSystem.Desktop.Services;

public interface IAppointmentApiClient
{
    Task<int> GetTodayAppointmentsCountAsync(CancellationToken cancellationToken = default);
    Task ScheduleAppointmentAsync(AppointmentDto appointmentDto, CancellationToken cancellationToken = default);
}