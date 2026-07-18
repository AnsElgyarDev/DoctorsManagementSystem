namespace DoctorsManagementSystem.Service;

public interface IAppointmentServices
{
    int GetTodayAppointmentsCount();
    bool ScheduleAppointment(Dto.AppointmentDto appointmentDto);
}