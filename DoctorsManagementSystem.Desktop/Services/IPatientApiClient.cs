using DoctorsManagementSystem.Desktop.Models;
using DoctorsManagementSystem.Dto;

namespace DoctorsManagementSystem.Desktop.Services;

public interface IPatientApiClient
{
    Task<IReadOnlyList<Patient>> GetAllPatientsAsync(CancellationToken cancellationToken = default);
    Task<Patient> CreatePatientAsync(PatientDto patientDto, CancellationToken cancellationToken = default);
    Task<Patient> GetPatientByIdAsync(int patientId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<PrescriptionSummary>> GetPatientPrescriptionsAsync(int patientId, CancellationToken cancellationToken = default);
    Task AddPrescriptionAsync(int patientId, PrescriptionDto prescriptionDto, CancellationToken cancellationToken = default);
}