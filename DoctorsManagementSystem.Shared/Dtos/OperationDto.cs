using System.ComponentModel.DataAnnotations;

namespace DoctorsManagementSystem.Dto;

public record OperationDto
{
    [Required, StringLength(150, MinimumLength = 3)]
    public string OperationName { get; init; } = string.Empty;

    public string Notes { get; init; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal OperationCost { get; init; }

    [Required]
    public DateTime SessionDate { get; init; }
}