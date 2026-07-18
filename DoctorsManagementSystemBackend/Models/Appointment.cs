using System.Text.Json.Serialization;

namespace DoctorsManagementSystem.model;

public enum AppointmentStatus
{
    Pending,
    Completed,
    Canceled
}

public class Appointment
{
    public int AppointmentId { get; set; }
    public int PatientId { get; set; }
    public DateOnly AppointmentDate { get; set; }
    public TimeOnly AppointmentTime { get; set; }
    public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;
    public string ReasonForVisit { get; set; } = string.Empty;

    [JsonIgnore]
    public required Patient patient { get; set; }
}