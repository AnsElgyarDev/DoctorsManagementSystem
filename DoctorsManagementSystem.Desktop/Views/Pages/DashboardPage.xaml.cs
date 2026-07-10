using System.Windows.Controls;
using DoctorsManagementSystem.Desktop.ViewModels.Pages;

namespace DoctorsManagementSystem.Desktop.Views.Pages;

public partial class DashboardPage : Page
{
    public DashboardViewModel ViewModel { get; }

    public DashboardPage(DashboardViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();

        Loaded += async (_, _) =>
        {
            if (ViewModel.LoadDashboardCommand.CanExecute(null))
            {
                await ViewModel.LoadDashboardCommand.ExecuteAsync(null);
            }
        };
    }
}