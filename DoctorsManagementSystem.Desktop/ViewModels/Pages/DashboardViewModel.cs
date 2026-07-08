using CommunityToolkit.Mvvm.ComponentModel;

namespace DoctorsManagementSystem.Desktop.ViewModels.Pages;

public partial class DashboardViewModel : ObservableObject
{
    [ObservableProperty]
    private string greeting = "Welcome to the Dashboard. Patient stats land here in a later step.";
}