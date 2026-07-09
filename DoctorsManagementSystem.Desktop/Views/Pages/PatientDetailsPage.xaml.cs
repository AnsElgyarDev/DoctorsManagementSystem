using System.Windows.Controls;
using DoctorsManagementSystem.Desktop.Infrastructure.Navigation;
using DoctorsManagementSystem.Desktop.ViewModels.Pages;

namespace DoctorsManagementSystem.Desktop.Views.Pages;

public partial class PatientDetailsPage : Page
{
    public PatientDetailsViewModel ViewModel { get; }

    public PatientDetailsPage(PatientDetailsViewModel viewModel, INavigationService navigationService)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();

        if (navigationService.ConsumePendingParameter() is int patientId)
        {
            ViewModel.Initialize(patientId);
        }

        Loaded += async (_, _) =>
        {
            if (ViewModel.LoadPatientCommand.CanExecute(null))
            {
                await ViewModel.LoadPatientCommand.ExecuteAsync(null);
            }
        };
    }
}