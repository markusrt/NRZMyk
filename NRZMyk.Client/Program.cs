using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using AutoMapper;
using BlazorApplicationInsights;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Authentication.WebAssembly.Msal.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NRZMyk.Components;
using NRZMyk.Services.Configuration;
using NRZMyk.Services.Interfaces;
using NRZMyk.Services.Services;

namespace NRZMyk.Client
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebAssemblyHostBuilder.CreateDefault(args);

            RedirectToBaseUrlOnCriticalException(builder);

            builder.Services.AddBlazorApplicationInsights();
            builder.RootComponents.Add<App>("app");
            builder.Services.AddHttpClient("NRZMyk.ServerAPI", client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
                .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();

            builder.Services.AddAutoMapper(typeof(SentinelEntryService).Assembly);

            // Supply HttpClient instances that include access tokens when making requests to the server project
            builder.Services.AddTransient(sp => sp.GetRequiredService<IHttpClientFactory>().CreateClient("NRZMyk.ServerAPI"));

            builder.Services.AddTransient<SentinelEntryService, SentinelEntryServiceImpl>();
            builder.Services.AddTransient<IAccountService, AccountService>();
            builder.Services.AddTransient<ClinicalBreakpointService, ClinicalBreakpointServiceImpl>();
            builder.Services.AddTransient<IMicStepsService, MicStepsService>();

            builder.Services.Configure<BreakpointSettings>(options =>
            {
                builder.Configuration.Bind(options);
            });

            builder.Services.AddMsalAuthentication(options =>
            {
                builder.Configuration.Bind("AzureAdB2C", options.ProviderOptions.Authentication);
                options.ProviderOptions.DefaultAccessTokenScopes.Add("https://nrcmycosis.onmicrosoft.com/e7562337-a4df-40c3-a8a1-7cc33d6bc193/API.Access");
            }).AddAccountClaimsPrincipalFactory<RemoteAuthenticationState, RemoteUserAccount,
                CustomUserFactory>();

            await builder.Build().RunAsync();
        }

        private static void RedirectToBaseUrlOnCriticalException(WebAssemblyHostBuilder builder)
        {
            //TODO check after blazor .net 5.0 release if the issue with password reset (see #35) is eventually adressed
            //     This would allow to remove this workaround

            var navigationManager = builder.Services.Single(
                s => s.ServiceType == typeof(NavigationManager)).ImplementationInstance as NavigationManager;
            var customLoggerProvider = new ReloadOnCriticalErrorLogProvider(navigationManager, "Unexpected error in authentication");
            builder.Logging.AddProvider(customLoggerProvider);
        }
    }
}
