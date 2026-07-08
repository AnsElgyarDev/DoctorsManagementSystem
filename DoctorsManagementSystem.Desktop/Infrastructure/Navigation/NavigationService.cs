using Wpf.Ui.Controls;

namespace DoctorsManagementSystem.Desktop.Infrastructure.Navigation;

public class NavigationService : INavigationService
{
    private INavigationView? _navigationView;

    public void Initialize(INavigationView navigationView)
    {
        _navigationView = navigationView;
    }

    public bool Navigate(Type pageType)
    {
        if (_navigationView is null)
        {
            throw new InvalidOperationException(
                "NavigationService.Initialize(INavigationView) must be called before navigating. " +
                "This normally happens once, in MainWindow's constructor.");
        }

        return _navigationView.Navigate(pageType);
    }

    public bool Navigate<TPage>() where TPage : class => Navigate(typeof(TPage));

    public bool GoBack() => _navigationView?.GoBack() ?? false;
}