using System;
using System.Windows;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Wpf.Ui.Appearance;
using DoctorsManagementSystem.Desktop.ViewModels;
using DoctorsManagementSystem.Desktop.Views;

namespace DoctorsManagementSystem.Desktop;

public partial class App : Application
{
    private readonly IHost _host;

    public App()
    {
        _host = Host.CreateDefaultBuilder()
            .ConfigureAppConfiguration((_, config) =>
            {
                config.SetBasePath(AppContext.BaseDirectory);
                config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
                ConfigureServices(services, context.Configuration);
            })
            .Build();
    }

    private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
    {
        services.AddSingleton<Wpf.Ui.IContentDialogService, Wpf.Ui.ContentDialogService>();

        services.AddTransient<ViewModels.Dialogs.AddPatientViewModel>();
        
        services.AddSingleton(configuration);

        var apiBaseUrl = configuration["ApiSettings:BaseUrl"]
            ?? throw new InvalidOperationException(
                "ApiSettings:BaseUrl is missing from appsettings.json.");

        services.AddHttpClient("DoctorsApi", client =>
        {
            client.BaseAddress = new Uri(apiBaseUrl);
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });

        // API clients — Singleton is safe here: PatientApiClient holds no mutable
        // per-request state, it only pulls a fresh HttpClient from IHttpClientFactory
        // (which manages connection pooling/lifetime for us) on every call.
        services.AddSingleton<Services.IPatientApiClient, Services.PatientApiClient>();

        // Navigation
        services.AddSingleton<Infrastructure.Navigation.INavigationService,
                            Infrastructure.Navigation.NavigationService>();

        // Shell
        services.AddSingleton<MainWindow>();
        services.AddSingleton<MainWindowViewModel>();

        // Pages + their ViewModels
        services.AddTransient<Views.Pages.DashboardPage>();
        services.AddTransient<ViewModels.Pages.DashboardViewModel>();

        services.AddTransient<Views.Pages.PatientsPage>();
        services.AddTransient<ViewModels.Pages.PatientsViewModel>();
    }


    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        await _host.StartAsync();

        ApplicationThemeManager.Apply(ApplicationTheme.Dark);

        var mainWindow = _host.Services.GetRequiredService<MainWindow>();
        mainWindow.Show();

        // Land on the Dashboard immediately instead of a blank content area
        var navigationService = _host.Services.GetRequiredService<Infrastructure.Navigation.INavigationService>();
        navigationService.Navigate<Views.Pages.DashboardPage>();
    }


    protected override async void OnExit(ExitEventArgs e)
    {
        await _host.StopAsync();
        _host.Dispose();
        base.OnExit(e);
    }
}