using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using DoctorsManagementSystem.Desktop.Views.Pages;
using Wpf.Ui.Controls;

namespace DoctorsManagementSystem.Desktop.ViewModels;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private string title = "Doctor's Clinic Management System";

    public ObservableCollection<object> MenuItems { get; } = new()
    {
        new NavigationViewItem
        {
            Content = "Dashboard",
            Icon = new SymbolIcon { Symbol = SymbolRegular.Home24 },
            TargetPageType = typeof(DashboardPage)
        },
        new NavigationViewItem
        {
            Content = "Patients",
            Icon = new SymbolIcon { Symbol = SymbolRegular.Person24 },
            TargetPageType = typeof(PatientsPage)
        }
    };

    public ObservableCollection<object> FooterMenuItems { get; } = new();
}