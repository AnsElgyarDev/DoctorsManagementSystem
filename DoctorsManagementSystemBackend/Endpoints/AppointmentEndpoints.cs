using DoctorsManagementSystem.Dto;
using DoctorsManagementSystem.Service;
using DoctorsManagementSystem.Shared.Dtos;
using Microsoft.AspNetCore.Http.HttpResults;

namespace DoctorsManagementSystem.Endpoints;

public static class AppointmentEndpoints
{
    public static void UseAppointmentEndpoints(this WebApplication app)
    {
        app.MapGet("/appointments/today-count", Ok<TodayAppointmentsCountDto>
                  (IAppointmentServices appointmentServices) =>
        {
            var count = appointmentServices.GetTodayAppointmentsCount();
            return TypedResults.Ok(new TodayAppointmentsCountDto { Count = count });
        });

        app.MapPost("/appointments", Results<BadRequest<string>, Ok<string>>
                  (IAppointmentServices appointmentServices, AppointmentDto appointmentDto) =>
        {
            var isSuccess = appointmentServices.ScheduleAppointment(appointmentDto);

            if (!isSuccess)
                return TypedResults.BadRequest("Patient not found. Cannot schedule an appointment for an unknown patient.");

            return TypedResults.Ok("Appointment scheduled successfully!");
        });
    }
}