namespace DoctorsManagementSystem.Desktop.Infrastructure.Dialogs;

public interface IFileDialogService
{
    /// <summary>Shows a native Save-As dialog. Returns the chosen path, or null if the user cancelled.</summary>
    string? ShowSaveFileDialog(string defaultFileName, string filter, string title);
}