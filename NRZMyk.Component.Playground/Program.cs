using BlazorApplicationInsights.Interfaces;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using NRZMyk.Components.Playground;
using NRZMyk.Components.Playground.Components;
using NRZMyk.Mocks.MockServices;
using NRZMyk.Services.Interfaces;
using NRZMyk.Services.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddAutoMapper(typeof(Program).Assembly, typeof(ISentinelEntryService).Assembly);
builder.Services.AddRazorPages();
builder.Services.AddRazorComponents().AddInteractiveServerComponents();
builder.Services.AddServerSideBlazor();
builder.Services.AddSingleton<IApplicationInsights, NullApplicationInsights>();
builder.Services.AddSingleton<ISentinelEntryService, MockSentinelEntryServiceImpl>();
builder.Services.AddSingleton<IAccountService, MockAccountService>();
builder.Services.AddSingleton<IClinicalBreakpointService, MockClinicalBreakpointService>();
builder.Services.AddSingleton<IMicStepsService, MicStepsService>();
builder.Services.AddSingleton<IEmailNotificationService, MockEmailNotificationService>();
builder.Services.AddSingleton<IReminderService, ReminderService>();

builder.Services.AddScoped<AuthenticationStateProvider, MockAuthStateProvider>();
builder.Services.AddScoped<SignOutSessionStateManager>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.MapRazorPages();

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
