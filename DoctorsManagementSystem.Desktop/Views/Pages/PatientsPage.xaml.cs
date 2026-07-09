using System.Windows.Controls;
using DoctorsManagementSystem.Desktop.ViewModels.Pages;

namespace DoctorsManagementSystem.Desktop.Views.Pages;

public partial class PatientsPage : Page
{
    public PatientsViewModel ViewModel { get; }

    public PatientsPage(PatientsViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();

        Loaded += async (_, _) =>
        {
            if (ViewModel.LoadPatientsCommand.CanExecute(null))
            {
                await ViewModel.LoadPatientsCommand.ExecuteAsync(null);
            }
        };
    }
}