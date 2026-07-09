using System.Windows.Controls;
using DoctorsManagementSystem.Desktop.ViewModels.Dialogs;

namespace DoctorsManagementSystem.Desktop.Views.Dialogs;

public partial class AddOperationDialogContent : UserControl
{
    public AddOperationDialogContent(AddOperationViewModel viewModel)
    {
        DataContext = viewModel;
        InitializeComponent();
    }
}