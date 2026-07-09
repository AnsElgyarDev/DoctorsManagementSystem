using System.Windows.Controls;
using DoctorsManagementSystem.Desktop.ViewModels.Dialogs;

namespace DoctorsManagementSystem.Desktop.Views.Dialogs;

public partial class AddPatientDialogContent : UserControl
{
    public AddPatientDialogContent(AddPatientViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }
}