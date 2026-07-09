using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using DoctorsManagementSystem.Desktop.Models;
using DoctorsManagementSystem.Dto;
using Microsoft.Extensions.Logging;

namespace DoctorsManagementSystem.Desktop.Services;

public class PatientApiClient : IPatientApiClient
{
    private const string PatientsEndpoint = "Patients";

    private readonly HttpClient _httpClient;
    private readonly ILogger<PatientApiClient> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public PatientApiClient(IHttpClientFactory httpClientFactory, ILogger<PatientApiClient> logger)
    {
        _httpClient = httpClientFactory.CreateClient("DoctorsApi");
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<IReadOnlyList<Patient>> GetAllPatientsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            using var response = await _httpClient.GetAsync(PatientsEndpoint, cancellationToken);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogInformation("No patients returned by the API (empty clinic roster).");
                return Array.Empty<Patient>();
            }

            if (!response.IsSuccessStatusCode)
            {
                var message = await ExtractErrorMessageAsync(response, cancellationToken);
                _logger.LogError("GET {Endpoint} failed with status {StatusCode}: {Message}",
                    PatientsEndpoint, (int)response.StatusCode, message);
                throw new ApiException(message, (int)response.StatusCode);
            }

            var patients = await response.Content.ReadFromJsonAsync<List<Patient>>(_jsonOptions, cancellationToken);
            return patients ?? new List<Patient>();
        }
        catch (ApiException) { throw; }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Could not reach the API at {BaseAddress}.", _httpClient.BaseAddress);
            throw new ApiException("Could not reach the clinic server. Please check that the backend is running and try again.", innerException: ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse the patients response from the API.");
            throw new ApiException("The server returned data in an unexpected format.", innerException: ex);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogError(ex, "The request to the API timed out.");
            throw new ApiException("The request timed out. Please check your connection and try again.", innerException: ex);
        }
    }

    public async Task<Patient> CreatePatientAsync(PatientDto patientDto, CancellationToken cancellationToken = default)
    {
        try
        {
            using var response = await _httpClient.PostAsJsonAsync(PatientsEndpoint, patientDto, _jsonOptions, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var message = await ExtractErrorMessageAsync(response, cancellationToken);
                _logger.LogError("POST {Endpoint} failed with status {StatusCode}: {Message}",
                    PatientsEndpoint, (int)response.StatusCode, message);
                throw new ApiException(message, (int)response.StatusCode);
            }

            var createdPatient = await response.Content.ReadFromJsonAsync<Patient>(_jsonOptions, cancellationToken);

            if (createdPatient is null)
                throw new ApiException("The patient was created, but the server did not return the record.", (int)response.StatusCode);

            return createdPatient;
        }
        catch (ApiException) { throw; }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Could not reach the API at {BaseAddress}.", _httpClient.BaseAddress);
            throw new ApiException("Could not reach the clinic server. Please check that the backend is running and try again.", innerException: ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse the create-patient response from the API.");
            throw new ApiException("The server returned data in an unexpected format.", innerException: ex);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogError(ex, "The request to the API timed out.");
            throw new ApiException("The request timed out. Please check your connection and try again.", innerException: ex);
        }
    }

    public async Task<Patient> GetPatientByIdAsync(int patientId, CancellationToken cancellationToken = default)
    {
        var endpoint = $"{PatientsEndpoint}/{patientId}";

        try
        {
            using var response = await _httpClient.GetAsync(endpoint, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var message = await ExtractErrorMessageAsync(response, cancellationToken);
                _logger.LogError("GET {Endpoint} failed with status {StatusCode}: {Message}",
                    endpoint, (int)response.StatusCode, message);
                throw new ApiException(message, (int)response.StatusCode);
            }

            var patient = await response.Content.ReadFromJsonAsync<Patient>(_jsonOptions, cancellationToken);

            if (patient is null)
                throw new ApiException("The server did not return the patient record.", (int)response.StatusCode);

            return patient;
        }
        catch (ApiException) { throw; }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Could not reach the API at {BaseAddress}.", _httpClient.BaseAddress);
            throw new ApiException("Could not reach the clinic server. Please check that the backend is running and try again.", innerException: ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse the patient response from the API.");
            throw new ApiException("The server returned data in an unexpected format.", innerException: ex);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogError(ex, "The request to the API timed out.");
            throw new ApiException("The request timed out. Please check your connection and try again.", innerException: ex);
        }
    }

    public async Task<IReadOnlyList<PrescriptionSummary>> GetPatientPrescriptionsAsync(int patientId, CancellationToken cancellationToken = default)
    {
        var endpoint = $"{PatientsEndpoint}/{patientId}/Prescriptions";

        try
        {
            using var response = await _httpClient.GetAsync(endpoint, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var message = await ExtractErrorMessageAsync(response, cancellationToken);
                _logger.LogError("GET {Endpoint} failed with status {StatusCode}: {Message}",
                    endpoint, (int)response.StatusCode, message);
                throw new ApiException(message, (int)response.StatusCode);
            }

            var prescriptions = await response.Content.ReadFromJsonAsync<List<PrescriptionSummary>>(_jsonOptions, cancellationToken);
            return prescriptions ?? new List<PrescriptionSummary>();
        }
        catch (ApiException) { throw; }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Could not reach the API at {BaseAddress}.", _httpClient.BaseAddress);
            throw new ApiException("Could not reach the clinic server. Please check that the backend is running and try again.", innerException: ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse the prescriptions response from the API.");
            throw new ApiException("The server returned data in an unexpected format.", innerException: ex);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogError(ex, "The request to the API timed out.");
            throw new ApiException("The request timed out. Please check your connection and try again.", innerException: ex);
        }
    }

    public async Task AddPrescriptionAsync(int patientId, PrescriptionDto prescriptionDto, CancellationToken cancellationToken = default)
    {
        var endpoint = $"{PatientsEndpoint}/{patientId}/Prescriptions";

        try
        {
            using var response = await _httpClient.PostAsJsonAsync(endpoint, prescriptionDto, _jsonOptions, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var message = await ExtractErrorMessageAsync(response, cancellationToken);
                _logger.LogError("POST {Endpoint} failed with status {StatusCode}: {Message}",
                    endpoint, (int)response.StatusCode, message);
                throw new ApiException(message, (int)response.StatusCode);
            }
        }
        catch (ApiException) { throw; }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Could not reach the API at {BaseAddress}.", _httpClient.BaseAddress);
            throw new ApiException("Could not reach the clinic server. Please check that the backend is running and try again.", innerException: ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse the add-prescription response from the API.");
            throw new ApiException("The server returned data in an unexpected format.", innerException: ex);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogError(ex, "The request to the API timed out.");
            throw new ApiException("The request timed out. Please check your connection and try again.", innerException: ex);
        }
    }

    private static async Task<string> ExtractErrorMessageAsync(HttpResponseMessage response, CancellationToken cancellationToken)
    {
        var body = await response.Content.ReadAsStringAsync(cancellationToken);

        if (string.IsNullOrWhiteSpace(body))
            return $"The server returned an unexpected error ({(int)response.StatusCode}).";

        try
        {
            var problem = JsonSerializer.Deserialize<ApiProblemDetails>(
                body, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

            if (problem is not null && (!string.IsNullOrWhiteSpace(problem.Detail) || !string.IsNullOrWhiteSpace(problem.Title)))
                return problem.Detail ?? problem.Title!;
        }
        catch (JsonException)
        {
            // Not ProblemDetails JSON — fall through and use the raw text body.
        }

        return body;
    }

    private record ApiProblemDetails(string? Title, string? Detail);

    public async Task<IReadOnlyList<OperationSummary>> GetPatientOperationsAsync(int patientId, CancellationToken cancellationToken = default)
{
    var endpoint = $"{PatientsEndpoint}/{patientId}/Operations";

    try
    {
        using var response = await _httpClient.GetAsync(endpoint, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var message = await ExtractErrorMessageAsync(response, cancellationToken);
            _logger.LogError("GET {Endpoint} failed with status {StatusCode}: {Message}",
                endpoint, (int)response.StatusCode, message);
            throw new ApiException(message, (int)response.StatusCode);
        }

        var operations = await response.Content.ReadFromJsonAsync<List<OperationSummary>>(_jsonOptions, cancellationToken);
        return operations ?? new List<OperationSummary>();
    }
    catch (ApiException) { throw; }
    catch (HttpRequestException ex)
    {
        _logger.LogError(ex, "Could not reach the API at {BaseAddress}.", _httpClient.BaseAddress);
        throw new ApiException("Could not reach the clinic server. Please check that the backend is running and try again.", innerException: ex);
    }
    catch (JsonException ex)
    {
        _logger.LogError(ex, "Failed to parse the operations response from the API.");
        throw new ApiException("The server returned data in an unexpected format.", innerException: ex);
    }
    catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
    {
        _logger.LogError(ex, "The request to the API timed out.");
        throw new ApiException("The request timed out. Please check your connection and try again.", innerException: ex);
    }
}

public async Task AddOperationAsync(int patientId, OperationDto operationDto, CancellationToken cancellationToken = default)
{
    var endpoint = $"{PatientsEndpoint}/{patientId}/Operations";

    try
    {
        using var response = await _httpClient.PostAsJsonAsync(endpoint, operationDto, _jsonOptions, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var message = await ExtractErrorMessageAsync(response, cancellationToken);
            _logger.LogError("POST {Endpoint} failed with status {StatusCode}: {Message}",
                endpoint, (int)response.StatusCode, message);
            throw new ApiException(message, (int)response.StatusCode);
        }
    }
    catch (ApiException) { throw; }
    catch (HttpRequestException ex)
    {
        _logger.LogError(ex, "Could not reach the API at {BaseAddress}.", _httpClient.BaseAddress);
        throw new ApiException("Could not reach the clinic server. Please check that the backend is running and try again.", innerException: ex);
    }
    catch (JsonException ex)
    {
        _logger.LogError(ex, "Failed to parse the add-operation response from the API.");
        throw new ApiException("The server returned data in an unexpected format.", innerException: ex);
    }
    catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
    {
        _logger.LogError(ex, "The request to the API timed out.");
        throw new ApiException("The request timed out. Please check your connection and try again.", innerException: ex);
    }
}
}