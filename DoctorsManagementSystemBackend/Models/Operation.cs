using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace DoctorsManagementSystem.model;

public class Operation
{
    public int OperationId { get; set; }
    [Required]
    [StringLength(150)]
    public string OperationName { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public decimal OperationCost { get; set; }   
    public DateTime SessionDate { get; set; }
    public int PatientId { get; set; }
    [JsonIgnore]
    public Patient? Patient { get; set; }
}