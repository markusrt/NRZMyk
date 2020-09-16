using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using NRZMyk.Components;
using NRZMyk.Services.Configuration;
using NRZMyk.Services.Services;

namespace NRZMyk.Client
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);
            builder.RootComponents.Add<App>("app");
            builder.Services.AddHttpClient("NRZMyk.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
                .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

            // Supply HttpClient instances that include access tokens when making requests to the server project
            builder.Services.AddTransient(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("NRZMyk.ServerAPI"));

            builder.Services.AddTransient<SentinelEntryService, SentinelEntryServiceImpl>();
            builder.Services.AddTransient<ClinicalBreakpointService, ClinicalBreakpointServiceImpl>();
            builder.Services.AddTransient<MicStepsService, MicStepsServiceImpl>();

            builder.Services.Configure<BreakpointSettings>(options =>
            {
                builder.Configuration.Bind(options);
            });
            builder.Services.AddMsalAuthentication(options =>
            {
                builder.Configuration.Bind("AzureAdB2C", options.ProviderOptions.Authentication);
                options.ProviderOptions.DefaultAccessTokenScopes.Add("https://nrcmycosis.onmicrosoft.com/e7562337-a4df-40c3-a8a1-7cc33d6bc193/API.Access");
                options.UserOptions.RoleClaim = "extension_Role";
            });

            await builder.Build().RunAsync();
        }
    }
}
