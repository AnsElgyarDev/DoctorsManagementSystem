using DoctorsManagementSystem.Desktop.Models;

namespace DoctorsManagementSystem.Desktop.Services;

public interface IPatientPdfExportService
{
    byte[] GeneratePatientReport(
        Patient patient,
        IReadOnlyList<PrescriptionSummary> prescriptions,
        IReadOnlyList<OperationSummary> operations);
}