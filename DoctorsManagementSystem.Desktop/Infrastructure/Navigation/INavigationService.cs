using Wpf.Ui.Controls;

namespace DoctorsManagementSystem.Desktop.Infrastructure.Navigation;

/// <summary>
/// Decouples ViewModels from the concrete WPF-UI NavigationView control.
/// ViewModels depend on this interface, never on the control itself.
/// </summary>
public interface INavigationService
{
    void Initialize(INavigationView navigationView);
    bool Navigate(Type pageType);
    bool Navigate<TPage>() where TPage : class;
    bool GoBack();
}