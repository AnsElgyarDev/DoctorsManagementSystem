using System.ComponentModel.DataAnnotations;

namespace DoctorsManagementSystem.Dto;

public record PrescriptionDto
{
    [Required(ErrorMessage = "Surgery name is required.")]
    [StringLength(150, ErrorMessage = "Surgery name cannot exceed 150 characters.")]
    public string SurgeryName { get; init; } = string.Empty;

    public string SurgeryNotes { get; init; } = string.Empty; 

    [Range(0, double.MaxValue, ErrorMessage = "Surgery bill cannot be a negative value.")]
    public decimal SurgeryBill { get; init; }

    [Required(ErrorMessage = "Session date is required.")]
    public DateTime SessionDate { get; init; }
}