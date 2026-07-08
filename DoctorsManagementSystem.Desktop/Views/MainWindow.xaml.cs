using System;
using DoctorsManagementSystem.Desktop.Infrastructure.Navigation;
using DoctorsManagementSystem.Desktop.ViewModels;
using Wpf.Ui.Controls;

namespace DoctorsManagementSystem.Desktop.Views;

public partial class MainWindow : FluentWindow
{
    public MainWindowViewModel ViewModel { get; }

    public MainWindow(
        MainWindowViewModel viewModel,
        IServiceProvider serviceProvider,
        INavigationService navigationService)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();

        // Lets NavigationView resolve Page instances (and their ViewModels) via DI
        RootNavigationView.SetServiceProvider(serviceProvider);

        // Hands our thin wrapper the live control so ViewModels can navigate
        // without ever referencing Wpf.Ui.Controls.NavigationView directly
        navigationService.Initialize(RootNavigationView);
    }
}