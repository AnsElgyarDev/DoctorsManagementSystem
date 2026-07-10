using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using DoctorsManagementSystem.Desktop.Models;
using Microsoft.Extensions.Logging;

namespace DoctorsManagementSystem.Desktop.Services;

public class DashboardApiClient : IDashboardApiClient
{
    private const string StatsEndpoint = "Dashboard/Stats";

    private readonly HttpClient _httpClient;
    private readonly ILogger<DashboardApiClient> _logger;
    private readonly JsonSerializerOptions _jsonOptions;

    public DashboardApiClient(IHttpClientFactory httpClientFactory, ILogger<DashboardApiClient> logger)
    {
        _httpClient = httpClientFactory.CreateClient("DoctorsApi");
        _logger = logger;
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<DashboardStats> GetDashboardStatsAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            using var response = await _httpClient.GetAsync(StatsEndpoint, cancellationToken);

            if (!response.IsSuccessStatusCode)
            {
                var message = await ExtractErrorMessageAsync(response, cancellationToken);
                _logger.LogError("GET {Endpoint} failed with status {StatusCode}: {Message}",
                    StatsEndpoint, (int)response.StatusCode, message);
                throw new ApiException(message, (int)response.StatusCode);
            }

            var stats = await response.Content.ReadFromJsonAsync<DashboardStats>(_jsonOptions, cancellationToken);

            if (stats is null)
                throw new ApiException("The server did not return dashboard stats.", (int)response.StatusCode);

            return stats;
        }
        catch (ApiException) { throw; }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Could not reach the API at {BaseAddress}.", _httpClient.BaseAddress);
            throw new ApiException("Could not reach the clinic server. Please check that the backend is running and try again.", innerException: ex);
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse the dashboard stats response from the API.");
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
}