using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using NRZMyk.Components.Helpers;
using NRZMyk.Services.Data.Entities;
using NRZMyk.Services.Services;

namespace NRZMyk.Components.Pages
{
    public class IndexBase : BlazorComponent
    {
        [Inject]
        private IAccountService AccountService { get; set; } = default!;

        [Inject]
        private ILogger<IndexBase> Logger { get; set; } = default!;

        internal ICollection<Organization> Organizations { get; set; } = default!;
        
        internal SaveState SaveState { get; set; }

        protected override async Task OnInitializedAsync()
        {
            Logger.LogInformation("Now loading... /Index");
            Organizations = await AccountService.ListOrganizations().ConfigureAwait(true);
            await base.OnInitializedAsync().ConfigureAwait(true);
        }
    }
}