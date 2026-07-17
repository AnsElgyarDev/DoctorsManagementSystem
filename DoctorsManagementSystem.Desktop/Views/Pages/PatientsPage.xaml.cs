using System.Windows;
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

        Loaded += OnPageLoaded;
    }

    private async void OnPageLoaded(object sender, RoutedEventArgs e)
    {
        // IsRunning (from CommunityToolkit's generated IAsyncRelayCommand) plus
        // the ViewModel's own internal guard together stop a duplicate Loaded
        // firing — a known Frame/NavigationView quirk during page transitions —
        // from kicking off a second, overlapping load.
        if (ViewModel.LoadPatientsCommand.IsRunning)
            return;

        if (ViewModel.LoadPatientsCommand.CanExecute(null))
        {
            await ViewModel.LoadPatientsCommand.ExecuteAsync(null);
        }
    }
}