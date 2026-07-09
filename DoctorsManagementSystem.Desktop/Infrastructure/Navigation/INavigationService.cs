using Wpf.Ui.Controls;

namespace DoctorsManagementSystem.Desktop.Infrastructure.Navigation;

public interface INavigationService
{
    void Initialize(INavigationView navigationView);
    bool Navigate(Type pageType);
    bool Navigate<TPage>() where TPage : class;
    bool GoBack();

    /// <summary>Stashes a value for the next page to pick up right after navigation.</summary>
    void SetPendingParameter(object? parameter);

    /// <summary>Reads and clears the pending parameter. Returns null if none was set.</summary>
    object? ConsumePendingParameter();
}