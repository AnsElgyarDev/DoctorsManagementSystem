namespace DoctorsManagementSystem.Desktop.ViewModels.Pages;

/// <summary>
/// Drives which single panel is visible on PatientsPage at any given moment.
/// </summary>
public enum PatientsLoadState
{
    Loading,
    Error,
    Empty,
    Loaded
}