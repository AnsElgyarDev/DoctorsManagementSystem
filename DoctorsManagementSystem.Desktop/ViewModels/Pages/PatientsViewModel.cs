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
using Mapster;

namespace DoctorsManagementSystem.Desktop.ViewModels.Pages;

public partial class PatientsViewModel : ObservableObject
{
    private const int PageSize = 15;
    private const int PageButtonWindowSize = 6;

    private readonly IPatientApiClient _patientApiClient;
    private readonly ILogger<PatientsViewModel> _logger;
    private readonly IContentDialogService _contentDialogService;
    private readonly IServiceProvider _serviceProvider;
    private readonly Infrastructure.Navigation.INavigationService _navigationService;

    // Reentrancy guard — prevents two overlapping LoadPatientsAsync executions
    // (e.g. a duplicate Loaded event from a Frame navigation transition, or a
    // fast double-click on Refresh) from mutating the collections concurrently.
    private bool _isLoadInProgress;

    public PatientsViewModel(
        IPatientApiClient patientApiClient,
        ILogger<PatientsViewModel> logger,
        IContentDialogService contentDialogService,
        IServiceProvider serviceProvider,
        Infrastructure.Navigation.INavigationService navigationService)
    {
        _patientApiClient = patientApiClient;
        _logger = logger;
        _contentDialogService = contentDialogService;
        _serviceProvider = serviceProvider;
        _navigationService = navigationService;
    }

    [ObservableProperty]
    private ObservableCollection<Patient> patients = new();

    [ObservableProperty]
    private PatientsLoadState state = PatientsLoadState.Loading;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    [ObservableProperty]
    private int currentPage = 1;

    [ObservableProperty]
    private int totalPages = 1;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(PreviousPageCommand))]
    private bool hasPreviousPage;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(NextPageCommand))]
    private bool hasNextPage;

    [ObservableProperty]
    private ObservableCollection<PageButtonItem> pageButtons = new();

    [RelayCommand]
    private async Task LoadPatientsAsync()
    {
        if (_isLoadInProgress)
            return;

        _isLoadInProgress = true;

        try
        {
            State = PatientsLoadState.Loading;
            ErrorMessage = string.Empty;

            // Bounded, non-recursive: at most 2 attempts. Attempt 0 fetches the
            // requested page; if the server reports CurrentPage is now beyond
            // TotalPages (e.g. after deletions shrank the dataset), attempt 1
            // retries once with the corrected page number. This loop can never
            // run more than twice — there is no recursive self-call anywhere.
            for (var attempt = 0; attempt < 2; attempt++)
            {
                var result = await _patientApiClient.GetAllPatientsAsync(CurrentPage, PageSize);

                TotalPages = Math.Max(result.TotalPages, 1);
                HasPreviousPage = result.HasPreviousPage;
                HasNextPage = result.HasNextPage;

                if (CurrentPage > TotalPages && attempt == 0)
                {
                    CurrentPage = TotalPages;
                    continue;
                }

                Patients.Clear();
                foreach (var patient in result.Items)
                {
                    Patients.Add(patient.Adapt<Patient>());
                }

                RebuildPageButtons();

                State = Patients.Count == 0 ? PatientsLoadState.Empty : PatientsLoadState.Loaded;
                break;
            }
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "Failed to load patients.");
            ErrorMessage = ex.Message;
            State = PatientsLoadState.Error;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while loading patients.");
            ErrorMessage = "Something went wrong while loading patients. Please try again.";
            State = PatientsLoadState.Error;
        }
        finally
        {
            _isLoadInProgress = false;
        }
    }

    private void RebuildPageButtons()
    {
        PageButtons.Clear();

        if (TotalPages <= 7)
        {
            for (var page = 1; page <= TotalPages; page++)
            {
                PageButtons.Add(PageButtonItem.Page(page, page == CurrentPage));
            }
            return;
        }

        var start = Math.Max(1, CurrentPage - (PageButtonWindowSize - 1));
        var end = Math.Min(start + PageButtonWindowSize - 1, TotalPages);

        if (start > 1)
        {
            PageButtons.Add(PageButtonItem.Ellipsis());
        }

        for (var page = start; page <= end; page++)
        {
            PageButtons.Add(PageButtonItem.Page(page, page == CurrentPage));
        }

        if (end < TotalPages)
        {
            PageButtons.Add(PageButtonItem.Ellipsis());
        }
    }

    [RelayCommand(CanExecute = nameof(HasPreviousPage))]
    private async Task PreviousPageAsync()
    {
        if (_isLoadInProgress)
            return;

        CurrentPage--;
        await LoadPatientsAsync();
    }

    [RelayCommand(CanExecute = nameof(HasNextPage))]
    private async Task NextPageAsync()
    {
        if (_isLoadInProgress)
            return;

        CurrentPage++;
        await LoadPatientsAsync();
    }

    [RelayCommand]
    private async Task NavigateToPageAsync(PageButtonItem pageButton)
    {
        if (_isLoadInProgress)
            return;

        if (pageButton is null || pageButton.IsEllipsis || pageButton.PageNumber == CurrentPage)
            return;

        CurrentPage = pageButton.PageNumber;
        await LoadPatientsAsync();
    }

    [RelayCommand]
    private async Task OpenAddPatientDialogAsync()
    {
        var addPatientViewModel = _serviceProvider.GetRequiredService<AddPatientViewModel>();
        addPatientViewModel.InitializeForAdd();

        await ShowPatientFormDialogAsync(addPatientViewModel);
    }

    [RelayCommand]
    private async Task OpenEditPatientDialogAsync(Patient patient)
    {
        var editPatientViewModel = _serviceProvider.GetRequiredService<AddPatientViewModel>();
        editPatientViewModel.InitializeForEdit(patient);

        await ShowPatientFormDialogAsync(editPatientViewModel);
    }

    private async Task ShowPatientFormDialogAsync(AddPatientViewModel formViewModel)
    {
        var dialogContent = new AddPatientDialogContent(formViewModel);

        var dialog = new ContentDialog
        {
            Title = formViewModel.DialogTitle,
            Content = dialogContent,
            CloseButtonText = "Close"
        };

        async void OnSaved()
        {
            dialog.Hide();
            await LoadPatientsAsync();
        }

        void OnCancelled() => dialog.Hide();

        formViewModel.Saved += OnSaved;
        formViewModel.Cancelled += OnCancelled;

        await _contentDialogService.ShowAsync(dialog, CancellationToken.None);

        formViewModel.Saved -= OnSaved;
        formViewModel.Cancelled -= OnCancelled;
    }

    [RelayCommand]
    private async Task DeletePatientAsync(Patient patient)
    {
        var confirmDialog = new ContentDialog
        {
            Title = "Delete Patient",
            Content = $"Are you sure you want to delete {patient.PatientName}? " +
                      "This will permanently remove their record and all associated " +
                      "prescriptions and operations. This cannot be undone.",
            PrimaryButtonText = "Delete",
            CloseButtonText = "Cancel"
        };

        var result = await _contentDialogService.ShowAsync(confirmDialog, CancellationToken.None);

        if (result != ContentDialogResult.Primary)
            return;

        try
        {
            await _patientApiClient.DeletePatientAsync(patient.PatientId);
            await LoadPatientsAsync();
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "Failed to delete patient {PatientId}.", patient.PatientId);
            await ShowErrorDialogAsync(ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error deleting patient {PatientId}.", patient.PatientId);
            await ShowErrorDialogAsync("Something went wrong while deleting this patient. Please try again.");
        }
    }

    private async Task ShowErrorDialogAsync(string message)
    {
        var errorDialog = new ContentDialog
        {
            Title = "Couldn't Delete Patient",
            Content = message,
            CloseButtonText = "OK"
        };

        await _contentDialogService.ShowAsync(errorDialog, CancellationToken.None);
    }

    [RelayCommand]
    private void OpenPatientDetails(Patient patient)
    {
        _navigationService.SetPendingParameter(patient.PatientId);
        _navigationService.Navigate<Views.Pages.PatientDetailsPage>();
    }
}