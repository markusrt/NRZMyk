using Microsoft.AspNetCore.Components;

namespace NRZMyk.Components.Shared
{
    public class NavMenuBase : ComponentBase
    {
        private bool collapseNavMenu = true;

        public string NavMenuCssClass => collapseNavMenu ? "collapse" : null;

        public void ToggleNavMenu()
        {
            collapseNavMenu = !collapseNavMenu;
        }
    }
}
