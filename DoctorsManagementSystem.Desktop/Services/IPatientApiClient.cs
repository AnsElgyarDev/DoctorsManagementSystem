using DoctorsManagementSystem.Desktop.Models;

namespace DoctorsManagementSystem.Desktop.Services;

public interface IPatientApiClient
{
    Task<IReadOnlyList<Patient>> GetAllPatientsAsync(CancellationToken cancellationToken = default);
}