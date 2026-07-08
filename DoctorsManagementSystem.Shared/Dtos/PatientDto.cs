using System.ComponentModel.DataAnnotations;
using DoctorsManagementSystem.Dto;

namespace DoctorsManagementSystem.Dto;

public record PatientDto 
{
    [Required(ErrorMessage = "Patient name is required.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Name must be between 3 and 100 characters.")]
    public string PatientName { get; init; } = string.Empty;

    [Range(0, double.MaxValue, ErrorMessage = "Total bill cannot be a negative value.")]
    public decimal TotalBill { get; init; }

    [Required(ErrorMessage = "Prescriptions list is required.")]
    [MinLength(1, ErrorMessage = "There must be at least one prescription.")]
    public List<PrescriptionDto> Prescriptions { get; init; } = new();

    [EmailAddress(ErrorMessage = "Invalid email address format.")]
    public string? PatientEmail { get; init; } = null;

    [Required(ErrorMessage = "Phone number is required.")]
    [RegularExpression(@"^01[0-2,5]{1}[0-9]{8}$", ErrorMessage = "Invalid Egyptian phone number.")]
    public string PatientPhone { get; init; } = "0123456789";
}