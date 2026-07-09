using System;
using DoctorsManagementSystem.Desktop.Infrastructure.Navigation;
using DoctorsManagementSystem.Desktop.ViewModels;
using Wpf.Ui;
using Wpf.Ui.Controls;

namespace DoctorsManagementSystem.Desktop.Views;

public partial class MainWindow : FluentWindow
{
    public MainWindowViewModel ViewModel { get; }

    public MainWindow(
        MainWindowViewModel viewModel,
        IServiceProvider serviceProvider,
        Infrastructure.Navigation.INavigationService navigationService,
        IContentDialogService contentDialogService)
    {
        ViewModel = viewModel;
        DataContext = this;

        InitializeComponent();

        RootNavigationView.SetServiceProvider(serviceProvider);
        navigationService.Initialize(RootNavigationView);

        contentDialogService.SetDialogHost(RootContentDialogPresenter);
    }
}