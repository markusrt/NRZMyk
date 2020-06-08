using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace NRZMyk.Components.Pages
{
    public class CounterBase : ComponentBase
    {
        [Inject]
        public AuthenticationStateProvider AuthenticationStateProvider { get; set; }

        public int CurrentCount { get; set; }

        public string Groups { get; set; }

        public string Roles { get; set; }

        protected override async Task OnInitializedAsync()
        {
            var state = await AuthenticationStateProvider.GetAuthenticationStateAsync();
            var user = state.User;
            var claims = user.Claims;
            Groups = string.Join(",", claims.Where(c => c.Type == "groups").Select(c => c.Value));
            Roles = string.Join(",", claims.Where(c => c.Type == "http://schemas.microsoft.com/ws/2008/06/identity/claims/role").Select(c => c.Value));
            await base.OnInitializedAsync();
        }

        public void IncrementCount()
        {
            CurrentCount++;
        }
    }
}
