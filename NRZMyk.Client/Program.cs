using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using NRZMyk.Client;
using NRZMyk.Services.Configuration;
using NRZMyk.Services.Services;
using AutoMapper;
using BlazorApplicationInsights;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using NRZMyk.Services.Interfaces;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<NRZMyk.Components.App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddHttpClient("NRZMyk.ServerAPI", 
    client => client.BaseAddress = new Uri(builder.HostEnvironment.BaseAddress))
                .AddHttpMessageHandler<BaseAddressAuthorizationMessageHandler>();
builder.Services.AddScoped(sp => sp.GetRequiredService<IHttpClientFactory>()
    .CreateClient("NRZMyk.ServerAPI"));

// Custom services start

builder.Services.AddBlazorApplicationInsights();
builder.Services.AddAutoMapper(typeof(SentinelEntryService).Assembly);
builder.Services.AddTransient<SentinelEntryService, SentinelEntryServiceImpl>();
builder.Services.AddTransient<IAccountService, AccountService>();
builder.Services.AddTransient<IClinicalBreakpointService, ClinicalBreakpointService>();
builder.Services.AddTransient<IMicStepsService, MicStepsService>();

builder.Services.Configure<BreakpointSettings>(options =>
{
    builder.Configuration.Bind(options);
});

builder.Services.AddMsalAuthentication(options =>
{
    builder.Configuration.Bind("AzureAdB2C", options.ProviderOptions.Authentication);
    options.ProviderOptions.DefaultAccessTokenScopes.Add("openid");
    options.ProviderOptions.DefaultAccessTokenScopes.Add("offline_access");
    options.ProviderOptions.DefaultAccessTokenScopes.Add("https://nrcmycosis.onmicrosoft.com/nrzmyk-client/API.Access");
}).AddAccountClaimsPrincipalFactory<RemoteAuthenticationState, RemoteUserAccount, CustomUserFactory>();

// Custom services end

await builder.Build().RunAsync();
