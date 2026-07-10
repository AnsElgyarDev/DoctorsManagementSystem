using DoctorsManagementSystem.Data;
using DoctorsManagementSystem.Dto;
using DoctorsManagementSystem.model;
using DoctorsManagementSystem.Service;
using Mapster;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

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

        app.MapGet("/Patient/{PatientId}/Operations", Results<NotFound<string>, Ok<ICollection<Prescription>>>
                  (int PatientId, AppDbContext context) =>
        {
            var patient= context.Patients
                                        .Include(patient => patient.prescriptions)
                                        .FirstOrDefault(patient => patient.PatientId == PatientId);
            
            if(patient is null)
                return TypedResults.NotFound("pateint Not Found!");
            
            return TypedResults.Ok(patient.prescriptions);
        });

        app.MapGet("/Patients/{patientId:int}/Operations", async Task<Results<NotFound<string>, Ok<List<OperationDto>>>> 
                  (int patientId, IPatientServices patientServices) =>
        {
            var operations = await patientServices.GetPatientOperationsAsync(patientId);

            if (operations is null)
                return TypedResults.NotFound("Patient not found or has no operations.");

            var result = operations.Adapt<List<OperationDto>>();

            return TypedResults.Ok(result);
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

        app.MapPost("/Patients/{patientId:int}/Operations", async Task<Results<NotFound<string>, BadRequest<string>, Ok>> 
                   (int patientId, OperationDto operationDto, IPatientServices patientServices) =>
        {
            if (operationDto is null)
                return TypedResults.BadRequest("There are no Data Sent!");

            var isAdded = await patientServices.AddOperationAsync(patientId, operationDto);

            if (!isAdded)
                return TypedResults.NotFound("Patient not found to link this operation.");

            return TypedResults.Ok();
        }
        );

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
                (IPatientServices patientServices, PatientUpdateDto patientUpdateDto, int PatientId) =>
        {
            var isSuccess = patientServices.UpdatePatient(patientUpdateDto, PatientId);
            if (!isSuccess) return TypedResults.BadRequest("Patient Not Found!");
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