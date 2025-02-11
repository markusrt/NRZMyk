using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using NRZMyk.Client;
using NRZMyk.Services.Services;
using BlazorApplicationInsights;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using NRZMyk.Services.Interfaces;
using System.Globalization;
using NRZMyk.Components;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.RootComponents.Add<Routes>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddHttpClient("NRZMyk.ServerAPI", 
        client =>
        {
            client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress);
            client.Timeout = TimeSpan.FromMinutes(5);
        })
    .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
    .CreateClient("NRZMyk.ServerAPI"));
builder.Services.AddScoped<IHttpClient, LoggingJsonHttpClient>();

// Custom services start

builder.Services.AddBlazorApplicationInsights();
builder.Services.AddAutoMapper(typeof(ISentinelEntryService).Assembly);
builder.Services.AddTransient<ISentinelEntryService, SentinelEntryServiceImpl>();
builder.Services.AddTransient<IAccountService, AccountService>();
builder.Services.AddTransient<IClinicalBreakpointService, ClinicalBreakpointService>();
builder.Services.AddTransient<IMicStepsService, MicStepsService>();
builder.Services.AddTransient<IReminderService, ReminderService>();


builder.Services.AddMsalAuthentication(options =>
{
    builder.Configuration.Bind("AzureAdB2C", options.ProviderOptions.Authentication);
    options.ProviderOptions.DefaultAccessTokenScopes.Add("openid");
    options.ProviderOptions.DefaultAccessTokenScopes.Add("offline_access");
    options.ProviderOptions.DefaultAccessTokenScopes.Add("https://nrcmycosis.onmicrosoft.com/nrzmyk-client/API.Access");
}).AddAccountClaimsPrincipalFactory<RemoteAuthenticationState, RemoteUserAccount, CustomUserFactory>();

// Custom services end
CultureInfo.DefaultThreadCurrentCulture = new CultureInfo("de-DE");
CultureInfo.DefaultThreadCurrentUICulture = new CultureInfo("de-DE");
await builder.Build().RunAsync().ConfigureAwait(true);
