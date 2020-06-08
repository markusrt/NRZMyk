using Microsoft.AspNetCore.Components;

namespace NRZMyk.Components.SharedComponents
{
    public class NavMenuBase : ComponentBase
    {
        private bool _collapseNavMenu = true;

        public string NavMenuCssClass => _collapseNavMenu ? "collapse" : null;

        public void ToggleNavMenu()
        {
            _collapseNavMenu = !_collapseNavMenu;
        }
    }
}
