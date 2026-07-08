using DoctorsManagementSystem.Dto;
using DoctorsManagementSystem.model;

namespace DoctorsManagementSystem.Service;

public interface IPatientServices
{
    IEnumerable<Patient> GetAllPatients();
    Patient? GetPatientsById(int PatientId);
    IEnumerable<Prescription> GetPatientsPrescription(int PatientId);
    Patient RegisterPatient (PatientDto patientDto);
    PrescriptionDto AddPatientPrescription(int patientId, PrescriptionDto prescriptionDto);
    bool DeletePatient(int PatientId);
    bool UpdatePatient(PatientDto patientDto, int PatientId);
    PrescriptionDto UpdatePatientPrescription(int PatientId, int prescriptionId, PrescriptionDto prescriptionDto);
}