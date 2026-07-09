using Wpf.Ui.Controls;

namespace DoctorsManagementSystem.Desktop.Infrastructure.Navigation;

public class NavigationService : INavigationService
{
    private INavigationView? _navigationView;
    private object? _pendingParameter;

    public void Initialize(INavigationView navigationView)
    {
        _navigationView = navigationView;
    }

    public bool Navigate(Type pageType)
    {
        if (_navigationView is null)
        {
            throw new InvalidOperationException(
                "NavigationService.Initialize(INavigationView) must be called before navigating.");
        }

        return _navigationView.Navigate(pageType);
    }

    public bool Navigate<TPage>() where TPage : class => Navigate(typeof(TPage));

    public bool GoBack() => _navigationView?.GoBack() ?? false;

    public void SetPendingParameter(object? parameter) => _pendingParameter = parameter;

    public object? ConsumePendingParameter()
    {
        var value = _pendingParameter;
        _pendingParameter = null;
        return value;
    }
}