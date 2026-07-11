using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DoctorsManagementSystem.Desktop.Infrastructure.Navigation;
using DoctorsManagementSystem.Desktop.Models;
using DoctorsManagementSystem.Desktop.Services;
using DoctorsManagementSystem.Desktop.ViewModels.Dialogs;
using DoctorsManagementSystem.Desktop.Views.Dialogs;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Wpf.Ui;
using System.IO;
using Wpf.Ui.Controls;

namespace DoctorsManagementSystem.Desktop.ViewModels.Pages;

public partial class PatientDetailsViewModel : ObservableObject
{
    private readonly IPatientApiClient _patientApiClient;
    private readonly ILogger<PatientDetailsViewModel> _logger;
    private readonly IContentDialogService _contentDialogService;
    private readonly IServiceProvider _serviceProvider;
    private readonly Infrastructure.Navigation.INavigationService _navigationService;
    
    // 1. الحقول الجديدة الخاصة بـ الفايل ديايلوج وتوليد الـ PDF
    private readonly Infrastructure.Dialogs.IFileDialogService _fileDialogService;
    private readonly IPatientPdfExportService _patientPdfExportService;

    // 2. الـ Constructor المحدث بعد حقن الخدمات الجديدة
    public PatientDetailsViewModel(
        IPatientApiClient patientApiClient,
        ILogger<PatientDetailsViewModel> logger,
        IContentDialogService contentDialogService,
        IServiceProvider serviceProvider,
        Infrastructure.Navigation.INavigationService navigationService,
        Infrastructure.Dialogs.IFileDialogService fileDialogService,     
        IPatientPdfExportService patientPdfExportService)                
    {
        _patientApiClient = patientApiClient;
        _logger = logger;
        _contentDialogService = contentDialogService;
        _serviceProvider = serviceProvider;
        _navigationService = navigationService;
        _fileDialogService = fileDialogService;                          
        _patientPdfExportService = patientPdfExportService;               
    }

    private int _patientId;

    [ObservableProperty]
    private Patient? patient;

    [ObservableProperty]
    private ObservableCollection<PrescriptionSummary> prescriptions = new();

    [ObservableProperty]
    private ObservableCollection<OperationSummary> operations = new();

    [ObservableProperty]
    private PatientDetailsLoadState state = PatientDetailsLoadState.Loading;

    [ObservableProperty]
    private string errorMessage = string.Empty;

    // 3. خاصية مراقبة حالة التصدير لتعطيل/تفعيل الزرار في الـ UI
    [ObservableProperty]
    private bool isExporting;

    public void Initialize(int patientId)
    {
        _patientId = patientId;
    }

    [RelayCommand]
    private async Task LoadPatientAsync()
    {
        State = PatientDetailsLoadState.Loading;
        ErrorMessage = string.Empty;

        try
        {
            var patientTask = _patientApiClient.GetPatientByIdAsync(_patientId);
            var prescriptionsTask = _patientApiClient.GetPatientPrescriptionsAsync(_patientId);
            var operationsTask = _patientApiClient.GetPatientOperationsAsync(_patientId);

            await Task.WhenAll(patientTask, prescriptionsTask, operationsTask);

            Patient = patientTask.Result;

            Prescriptions.Clear();
            foreach (var prescription in prescriptionsTask.Result)
            {
                Prescriptions.Add(prescription);
            }

            Operations.Clear();
            foreach (var operation in operationsTask.Result)
            {
                Operations.Add(operation);
            }

            State = PatientDetailsLoadState.Loaded;
        }
        catch (ApiException ex)
        {
            _logger.LogError(ex, "Failed to load patient {PatientId}.", _patientId);
            ErrorMessage = ex.Message;
            State = PatientDetailsLoadState.Error;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error loading patient {PatientId}.", _patientId);
            ErrorMessage = "Something went wrong while loading this patient. Please try again.";
            State = PatientDetailsLoadState.Error;
        }
    }

    [RelayCommand]
    private async Task OpenAddPrescriptionDialogAsync()
    {
        var addPrescriptionViewModel = _serviceProvider.GetRequiredService<AddPrescriptionViewModel>();
        addPrescriptionViewModel.Initialize(_patientId);

        var dialogContent = new AddPrescriptionDialogContent(addPrescriptionViewModel);

        var dialog = new ContentDialog
        {
            Title = "Add New Prescription",
            Content = dialogContent,
            CloseButtonText = "Close"
        };

        async void OnSaved()
        {
            dialog.Hide();
            await LoadPatientAsync();
        }

        void OnCancelled() => dialog.Hide();

        addPrescriptionViewModel.Saved += OnSaved;
        addPrescriptionViewModel.Cancelled += OnCancelled;

        await _contentDialogService.ShowAsync(dialog, CancellationToken.None);

        addPrescriptionViewModel.Saved -= OnSaved;
        addPrescriptionViewModel.Cancelled -= OnCancelled;
    }

    [RelayCommand]
    private async Task OpenAddOperationDialogAsync()
    {
        var addOperationViewModel = _serviceProvider.GetRequiredService<AddOperationViewModel>();
        addOperationViewModel.Initialize(_patientId);

        var dialogContent = new AddOperationDialogContent(addOperationViewModel);

        var dialog = new ContentDialog
        {
            Title = "Add New Medical Operation",
            Content = dialogContent,
            CloseButtonText = "Close"
        };

        async void OnSaved()
        {
            dialog.Hide();
            await LoadPatientAsync();
        }

        void OnCancelled() => dialog.Hide();

        addOperationViewModel.Saved += OnSaved;
        addOperationViewModel.Cancelled += OnCancelled;

        await _contentDialogService.ShowAsync(dialog, CancellationToken.None);

        addOperationViewModel.Saved -= OnSaved;
        addOperationViewModel.Cancelled -= OnCancelled;
    }

    [RelayCommand]
    private void GoBack()
    {
        _navigationService.Navigate<Views.Pages.PatientsPage>();
    }

    // 4. ميثود الأمر (Command) المسؤول عن تشغيل عملية التصدير وحفظ الملف
    [RelayCommand]
    private async Task ExportPdfAsync()
    {
        if (Patient is null)
            return;

        var defaultFileName = $"{Patient.PatientName.Replace(' ', '_')}_MedicalRecord.pdf";
        
        var filePath = _fileDialogService.ShowSaveFileDialog(
            defaultFileName,
            "PDF Document (*.pdf)|*.pdf",
            "Export Patient Record");

        if (string.IsNullOrEmpty(filePath))
            return; 

        IsExporting = true; 

        try
        {
            var pdfBytes = _patientPdfExportService.GeneratePatientReport(
                Patient, Prescriptions.ToList(), Operations.ToList());

            await File.WriteAllBytesAsync(filePath, pdfBytes);

            var confirmDialog = new ContentDialog
            {
                Title = "Export Complete",
                Content = $"The patient record was saved to:\n{filePath}",
                CloseButtonText = "OK"
            };
            await _contentDialogService.ShowAsync(confirmDialog, CancellationToken.None);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to export PDF for patient {PatientId}.", _patientId);

            var errorDialog = new ContentDialog
            {
                Title = "Export Failed",
                Content = "Something went wrong while generating the PDF. Please try again.",
                CloseButtonText = "OK"
            };
            await _contentDialogService.ShowAsync(errorDialog, CancellationToken.None);
        }
        finally
        {
            IsExporting = false; 
        }
    }
}