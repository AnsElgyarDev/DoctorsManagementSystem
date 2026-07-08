using CommunityToolkit.Mvvm.ComponentModel;

namespace DoctorsManagementSystem.Desktop.ViewModels.Pages;

public partial class PatientsViewModel : ObservableObject
{
    [ObservableProperty]
    private string placeholder = "The real patient list (DataGrid + API calls) arrives in Step 4.";
}