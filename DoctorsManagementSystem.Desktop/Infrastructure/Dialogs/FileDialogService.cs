using Microsoft.Win32;

namespace DoctorsManagementSystem.Desktop.Infrastructure.Dialogs;

public class FileDialogService : IFileDialogService
{
    public string? ShowSaveFileDialog(string defaultFileName, string filter, string title)
    {
        var dialog = new SaveFileDialog
        {
            FileName = defaultFileName,
            Filter = filter,
            Title = title,
            AddExtension = true
        };

        return dialog.ShowDialog() == true ? dialog.FileName : null;
    }
}