using DoctorsManagementSystem.model;
using DoctorsManagementSystem.Dto;

namespace DoctorsManagementSystem.Service;

public interface IPatientServices
{
    PagedResult<Patient> GetAllPatients(int pageNumber, int pageSize);
    Patient? GetPatientsById(int PatientId);
    IEnumerable<Prescription> GetPatientsPrescription(int PatientId);
    Patient RegisterPatient (PatientDto patientDto);
    PrescriptionDto AddPatientPrescription(int patientId, PrescriptionDto prescriptionDto);
    bool DeletePatient(int PatientId);
    bool UpdatePatient(PatientUpdateDto patientUpdateDto, int PatientId);
    PrescriptionDto UpdatePatientPrescription(int PatientId, int prescriptionId, PrescriptionDto prescriptionDto);
    Task<List<Operation>?> GetPatientOperationsAsync(int patientId);
    Task<bool> AddOperationAsync(int patientId, OperationDto operationDto);
}