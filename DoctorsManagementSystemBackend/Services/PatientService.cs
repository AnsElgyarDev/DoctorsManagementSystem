using DoctorsManagementSystem.Data;
using Mapster;
using DoctorsManagementSystem.Dto;
using DoctorsManagementSystem.model;
using Microsoft.EntityFrameworkCore;

namespace DoctorsManagementSystem.Service;

class PatientServices : IPatientServices
{
    private readonly AppDbContext _Context;
    public PatientServices(AppDbContext context)
    {
        this._Context = context;   
    }

    public PrescriptionDto AddPatientPrescription(int patientId, PrescriptionDto prescriptionDto)
    {
        var patient =  _Context.Patients.
                       Include(patient => patient.prescriptions).
                       FirstOrDefault(patient => patient.PatientId == patientId);

        if (patient is null)
        {
            throw new KeyNotFoundException("There is No Patient with this ID!");        
        }
        
        var prescriptionToAdd = prescriptionDto.Adapt<Prescription>();
        
        patient.prescriptions.Add(prescriptionToAdd);

        _Context.SaveChanges();
        var resultDto = prescriptionToAdd.Adapt<PrescriptionDto>();
        
        return resultDto;
    }

    public bool DeletePatient(int PatientId)
    {
        var PatientToDelete = _Context.Patients.Find(PatientId);
        if(PatientToDelete is null)
            return false;
        
        _Context.Patients.Remove(PatientToDelete);
        _Context.SaveChanges();
        
        return true;
    }

    public IEnumerable<Patient> GetAllPatients() => _Context.Patients;

    public Patient? GetPatientsById(int PatientId)
    {
        var UserToReturn = _Context.Patients.Find(PatientId);
     
        return UserToReturn;
    }

    public IEnumerable<Prescription> GetPatientsPrescription(int PatientId)
    {
        var patient = _Context.Patients
        .Include(patient => patient.prescriptions)
        .FirstOrDefault(patient =>  patient.PatientId == PatientId);
        
        if (patient is null)
        {
            throw new KeyNotFoundException("There is No Patient with this ID!");        
        }
        
        return patient.prescriptions;
    }

    public Patient RegisterPatient(PatientDto patientDto)
{
    if (patientDto.Prescriptions == null || !patientDto.Prescriptions.Any())
    {
        throw new ArgumentException("There Must Be Atleast One Prescription!");
    }

    var patientToAdd = patientDto.Adapt<Patient>();

    patientToAdd.prescriptions = patientDto.Prescriptions.Select(p => new Prescription
    {
        SurgeryName = p.SurgeryName,
        SurgeryNotes = p.SurgeryNotes,
        SurgeryBill = p.SurgeryBill,
        SessionDate = p.SessionDate,
        patient = patientToAdd
    }).ToList();

    _Context.Patients.Add(patientToAdd);
    _Context.SaveChanges(); 

    return patientToAdd;
}

    public bool UpdatePatient(PatientDto patientDto, int PatientId)
    {
        // checking the User Existance in the DB and Then save the changes in the DB
        var USerToUpdate = _Context.Patients.Find(PatientId);
        if(USerToUpdate is null)
            return false;
        
        patientDto.Adapt(USerToUpdate);
        _Context.SaveChanges();
        
        return true;
    }

    public PrescriptionDto UpdatePatientPrescription(int PatientId, int prescriptionId, PrescriptionDto prescriptionDto)
    {
        var patient = _Context.Patients.
                    Include(patient => patient.prescriptions).
                    FirstOrDefault(patient => patient.PatientId == PatientId);

        if(patient is null)
            throw new KeyNotFoundException("There is No Patient with this ID!");        

        var prescriptionToUpdate =  patient.
                                    prescriptions.
                                    FirstOrDefault(Prescription => Prescription.PrescriptionId == prescriptionId);
        
        if(prescriptionToUpdate is null)
            throw new KeyNotFoundException("There is No Prescription with this ID!");        

        prescriptionDto.Adapt(prescriptionToUpdate);

        _Context.SaveChanges();
        
        var updatedDto = prescriptionToUpdate.Adapt<PrescriptionDto>();

        return updatedDto;
    }
}