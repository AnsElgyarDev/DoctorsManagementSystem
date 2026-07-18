using System.ComponentModel.DataAnnotations;

namespace DoctorsManagementSystem.Dto;

public record AppointmentDto
{
    [Required]
    public int PatientId { get; init; }

    [Required]
    public DateOnly AppointmentDate { get; init; }

    [Required]
    public TimeOnly AppointmentTime { get; init; }

    [Required, StringLength(300, MinimumLength = 3)]
    public string ReasonForVisit { get; init; } = string.Empty;
}