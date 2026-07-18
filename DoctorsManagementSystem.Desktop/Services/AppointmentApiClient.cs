using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using DoctorsManagementSystem.Dto;
using DoctorsManagementSystem.Shared.Dtos;
using Microsoft.Extensions.Logging;

namespace DoctorsManagementSystem.Desktop.Services;

public class AppointmentApiClient : IAppointmentApiClient
{
    private const string AppointmentsEndpoint = "appointments";

    private readonly HttpClient _httpClient;
    private readonly ILogger<AppointmentApiClient> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public AppointmentApiClient(IHttpClientFactory httpClientFactory, ILogger<AppointmentApiClient> logger)
    {
        _httpClient = httpClientFactory.CreateClient("DoctorsApi");
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<int> GetTodayAppointmentsCountAsync(CancellationToken cancellationToken = default)
    {
        var endpoint = $"{AppointmentsEndpoint}/today-count";

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

            var result = await response.Content.ReadFromJsonAsync<TodayAppointmentsCountDto>(_jsonOptions, cancellationToken);
            return result?.Count ?? 0;
        }
        catch (ApiException) { throw; }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Could not reach the API at {BaseAddress}.", _httpClient.BaseAddress);
            throw new ApiException("Could not reach the clinic server. Please check that the backend is running and try again.", innerException: ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse the today's-appointments-count response from the API.");
            throw new ApiException("The server returned data in an unexpected format.", innerException: ex);
        }
        catch (TaskCanceledException ex) when (!cancellationToken.IsCancellationRequested)
        {
            _logger.LogError(ex, "The request to the API timed out.");
            throw new ApiException("The request timed out. Please check your connection and try again.", innerException: ex);
        }
    }

    public async Task ScheduleAppointmentAsync(AppointmentDto appointmentDto, CancellationToken cancellationToken = default)
    {
        try
        {
            using var response = await _httpClient.PostAsJsonAsync(AppointmentsEndpoint, appointmentDto, _jsonOptions, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var message = await ExtractErrorMessageAsync(response, cancellationToken);
                _logger.LogError("POST {Endpoint} failed with status {StatusCode}: {Message}",
                    AppointmentsEndpoint, (int)response.StatusCode, message);
                throw new ApiException(message, (int)response.StatusCode);
            }

            // Success body is a plain-text confirmation string, not JSON.
        }
        catch (ApiException) { throw; }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Could not reach the API at {BaseAddress}.", _httpClient.BaseAddress);
            throw new ApiException("Could not reach the clinic server. Please check that the backend is running and try again.", innerException: ex);
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
        }

        return body;
    }

    private record ApiProblemDetails(string? Title, string? Detail);
}