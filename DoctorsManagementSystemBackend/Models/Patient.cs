using DoctorsManagementSystem.Dto;

namespace DoctorsManagementSystem.model;

public class Patient
{
    public int PatientId { get; set; }
    public string PatientName { get; set; } = "John Doe";
    public decimal TotalBill { get; set; }
    public string PatientEmail { get; set; } = "anselgyar012@gmail.com";
    public string PatientPhone { get; set; } = "0123456789";
    public ICollection<Prescription> prescriptions { get; set; } = new List<Prescription>();
}