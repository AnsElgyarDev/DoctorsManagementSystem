using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DoctorsManagementSystem.Desktop.Models;
using DoctorsManagementSystem.Desktop.Services;
using DoctorsManagementSystem.Desktop.ViewModels.Dialogs;
using DoctorsManagementSystem.Desktop.Views.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace DoctorsManagementSystem.Desktop.ViewModels.Pages;

public partial class DashboardViewModel : ObservableObject
{
    private readonly IDashboardApiClient _dashboardApiClient;
    private readonly ILogger<DashboardViewModel> _logger;
    private readonly IContentDialogService _contentDialogService;
    private readonly IServiceProvider _serviceProvider;

    public DashboardViewModel(
        IDashboardApiClient dashboardApiClient,
        ILogger<DashboardViewModel> logger,
        IContentDialogService contentDialogService,
        IServiceProvider serviceProvider)
    {
        _dashboardApiClient = dashboardApiClient;
        _logger = logger;
        _contentDialogService = contentDialogService;
        _serviceProvider = serviceProvider;
    }

    [ObservableProperty] private int todayAppointmentsCount;
    [ObservableProperty] private decimal monthlyRevenue;
    [ObservableProperty] private int upcomingOperationsCount;
    [ObservableProperty] private ObservableCollection<LatestPatient> latestPatients = new();
    [ObservableProperty] private DashboardLoadState state = DashboardLoadState.Loading;
    [ObservableProperty] private string errorMessage = string.Empty;

    [RelayCommand]
    private async Task LoadDashboardAsync()
    {
        State = DashboardLoadState.Loading;
        ErrorMessage = string.Empty;

        try
        {
            var stats = await _dashboardApiClient.GetDashboardStatsAsync();

            TodayAppointmentsCount = stats.TodayAppointmentsCount;
            MonthlyRevenue = stats.MonthlyRevenue;
            UpcomingOperationsCount = stats.UpcomingOperationsCount;

            LatestPatients.Clear();
            foreach (var patient in stats.LatestPatients)
            {
                LatestPatients.Add(patient);
            }

            State = DashboardLoadState.Loaded;
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "Failed to load dashboard stats.");
            ErrorMessage = ex.Message;
            State = DashboardLoadState.Error;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error loading dashboard stats.");
            ErrorMessage = "Something went wrong while loading the dashboard. Please try again.";
            State = DashboardLoadState.Error;
        }
    }

    [RelayCommand]
    private async Task OpenAddPatientDialogAsync()
    {
        var addPatientViewModel = _serviceProvider.GetRequiredService<AddPatientViewModel>();
        addPatientViewModel.InitializeForAdd();

        var dialogContent = new AddPatientDialogContent(addPatientViewModel);

        var dialog = new ContentDialog
        {
            Title = addPatientViewModel.DialogTitle,
            Content = dialogContent,
            CloseButtonText = "Close"
        };

        async void OnSaved()
        {
            dialog.Hide();
            await LoadDashboardAsync();
        }

        void OnCancelled() => dialog.Hide();

        addPatientViewModel.Saved += OnSaved;
        addPatientViewModel.Cancelled += OnCancelled;

        await _contentDialogService.ShowAsync(dialog, CancellationToken.None);

        addPatientViewModel.Saved -= OnSaved;
        addPatientViewModel.Cancelled -= OnCancelled;
    }
}