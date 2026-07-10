using System.ComponentModel.DataAnnotations;

namespace DoctorsManagementSystem.Dto;

public record PatientUpdateDto
{
    [Required, StringLength(100, MinimumLength = 3)]
    public string PatientName { get; init; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal TotalBill { get; init; }

    [EmailAddress]
    public string? PatientEmail { get; init; }

    [Required, RegularExpression(@"^01[0-2,5]{1}[0-9]{8}$")]
    public string PatientPhone { get; init; } = "0123456789";
}