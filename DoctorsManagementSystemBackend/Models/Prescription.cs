namespace DoctorsManagementSystem.model;

public class Prescription
{
    public int PrescriptionId { get; set; }
    public int PatientId { get; set; }
    public string SurgeryName { get; set; } = string.Empty;
    public string SurgeryNotes { get; set; } = string.Empty;
    public decimal SurgeryBill { get; set; }
    public DateTime SessionDate { get; set; }
    public required Patient patient { get; set; } 
}