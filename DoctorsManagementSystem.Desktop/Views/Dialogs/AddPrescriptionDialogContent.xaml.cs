using System.Windows.Controls;
using DoctorsManagementSystem.Desktop.ViewModels.Dialogs;

namespace DoctorsManagementSystem.Desktop.Views.Dialogs;

public partial class AddPrescriptionDialogContent : UserControl
{
    public AddPrescriptionDialogContent(AddPrescriptionViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }
}