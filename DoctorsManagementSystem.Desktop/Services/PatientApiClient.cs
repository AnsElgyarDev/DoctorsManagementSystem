using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using DoctorsManagementSystem.Desktop.Models;
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

        // PropertyNameCaseInsensitive covers us regardless of whether the
        // backend's JSON casing changes between camelCase and PascalCase.
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

            // The backend returns 404 with a plain-text body when the clinic
            // simply has no patients yet (see PatientEndpoints.cs) — that's a
            // valid empty state, not a failure, so we don't treat it as one.
            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                _logger.LogInformation("No patients returned by the API (empty clinic roster).");
                return Array.Empty<Patient>();
            }

            if (!response.IsSuccessStatusCode)
            {
                var errorBody = await response.Content.ReadAsStringAsync(cancellationToken);
                _logger.LogError(
                    "GET {Endpoint} failed with status {StatusCode}: {Body}",
                    PatientsEndpoint, (int)response.StatusCode, errorBody);

                throw new ApiException(
                    $"The server returned an unexpected error ({(int)response.StatusCode}).",
                    (int)response.StatusCode);
            }

            var patients = await response.Content.ReadFromJsonAsync<List<Patient>>(_jsonOptions, cancellationToken);
            return patients ?? new List<Patient>();
        }
        catch (ApiException)
        {
            throw; // already logged and shaped above — don't re-wrap
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Could not reach the API at {BaseAddress}. Is the backend running?", _httpClient.BaseAddress);
            throw new ApiException(
                "Could not reach the clinic server. Please check that the backend is running and try again.",
                innerException: ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse the patients response from the API.");
            throw new ApiException(
                "The server returned data in an unexpected format.",
                innerException: ex);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogError(ex, "The request to the API timed out.");
            throw new ApiException(
                "The request timed out. Please check your connection and try again.",
                innerException: ex);
        }
    }
}