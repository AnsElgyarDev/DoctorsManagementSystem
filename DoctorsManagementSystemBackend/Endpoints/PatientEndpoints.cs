using DoctorsManagementSystem.Dto;
using DoctorsManagementSystem.model;
using DoctorsManagementSystem.Service;
using Mapster;
using Microsoft.AspNetCore.Http.HttpResults;

namespace DoctorsManagementSystem.Endpoints;

public static class PatientEndpoints
{
    
    public static void UsePatientEndpoints(this WebApplication app)
    {
        // Get Endpoints        
        app.MapGet("/Patients", Results<NotFound<string>, Ok<IEnumerable<Patient>>>
                  (IPatientServices patientServices) =>
        {
            var Patients = patientServices.GetAllPatients();
            
            if (Patients == null || !Patients.Any())
            {
                return TypedResults.NotFound("There are no patients yet."); 
            }

            return TypedResults.Ok(Patients);
        });

        app.MapGet("/Patients/{PatientId}", Results<NotFound<string>, Ok<Patient>>
                  (IPatientServices patientServices, int PatientId) =>
        {
            var patientToReturn = patientServices.GetPatientsById(PatientId);
            
            if(patientToReturn is null)
                return TypedResults.NotFound("Patient Not Found!");

            return TypedResults.Ok(patientToReturn);
        });

        app.MapGet("/Patients/{PatientId}/Prescriptions", (int PatientId, IPatientServices patientServices) =>
        {
            var PrescriptionsToReturn = patientServices.GetPatientsPrescription(PatientId);

            return TypedResults.Ok(PrescriptionsToReturn);
        });

        // Post Endpoints
        app.MapPost("/Patients", Results<NotFound<string>, Ok<PatientDto>> 
                   (IPatientServices patientServices, PatientDto patientDto) =>
        {
            var PatientToAdd  = patientServices.RegisterPatient(patientDto);
 
            if(PatientToAdd is null)
                return TypedResults.NotFound("There are no Data Sent!");
            
            var resultDto = PatientToAdd.Adapt<PatientDto>();

            return TypedResults.Ok(resultDto);
        });

        app.MapPost("/Patients/{patientId}/Prescriptions", 
                   (int patientId, PrescriptionDto prescriptionDto, IPatientServices patientServices) =>
        {
            var presciptionToAdd = patientServices.AddPatientPrescription(patientId, prescriptionDto);
            
            return TypedResults.Ok(presciptionToAdd);    
        });

        // Delete Endpoints
        app.MapDelete("/Patients/{PatientId}", Results<BadRequest<string>, Ok<string>>
                     (IPatientServices patientServices, int PatientId) =>
        {
            var isSuccess = patientServices.DeletePatient(PatientId);
           
            if(!isSuccess)
                return TypedResults.BadRequest("Not Found!");
           
            return TypedResults.Ok("Deletion Commited Successfully!");
        });

        // Update Endpoints
        app.MapPut("/Patients/{PatientId}", Results<BadRequest<string>, NoContent>
                  (IPatientServices patientServices, PatientDto patientDto, int PatientId) =>
        {
            var isSuccess = patientServices.UpdatePatient(patientDto, PatientId); 
            if(!isSuccess)
                return TypedResults.BadRequest("Patient Not Found!");
            return TypedResults.NoContent();
        });

        app.MapPut("/Patients/{PatientId}/Prescriptions",
                  (int PatientId, int prescriptionId, PrescriptionDto prescriptionDto, IPatientServices patientServices) =>
        {
            var patientprescriptionToUpate = patientServices.UpdatePatientPrescription(PatientId, prescriptionId, prescriptionDto); 
            
            return TypedResults.Ok(patientprescriptionToUpate);
        }); 

    }
} 