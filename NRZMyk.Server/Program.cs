using System.Security.Claims;
using Coravel;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Identity.Web;
using Microsoft.OpenApi.Models;
using NRZMyk.Server;
using NRZMyk.Server.Authorization;
using NRZMyk.Server.Invocables;
using NRZMyk.Server.Utils;
using NRZMyk.Services.Configuration;
using NRZMyk.Services.Data;
using NRZMyk.Services.Interfaces;
using NRZMyk.Services.Services;
using NRZMyk.Services.Utils;
using SendGrid;
using SendGrid.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);


builder.Services.Configure<AzureAdB2CSettings>(builder.Configuration);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddMicrosoftIdentityWebApi(builder.Configuration, "AzureAdB2C");
builder.Services.Configure<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme, options =>
{
    options.TokenValidationParameters.NameClaimType = "name";
    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = async ctx =>
        {
            if (ctx.Principal.Identity is ClaimsIdentity identity)
            {
                identity.AddRolesFromExtensionClaim();
                await identity.AddOrganizationClaim(ctx).ConfigureAwait(false);
            }
        }
    };
});
builder.Services.AddAuthorizationBuilder()
    .AddPolicy(Policies.AssignedToOrganization, policy => policy.RequireClaim(NRZMyk.Services.Models.ClaimTypes.Organization));

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.Configure<DatabaseSeedSettings>(builder.Configuration);
builder.Services.Configure<ApplicationSettings>(builder.Configuration);
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddApplicationInsightsTelemetry();

builder.Services.AddAutoMapper(typeof(Program).Assembly, typeof(ISentinelEntryService).Assembly);

builder.Services.AddScoped<IGraphServiceClient, GraphServiceClientWrapper>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped(typeof(IAsyncRepository<>), typeof(EfRepository<>));
builder.Services.AddScoped<ISentinelEntryRepository, SentinelEntryRepository>();
builder.Services.AddScoped<IProtectKeyToOrganizationResolver, ProtectKeyToOrganizationResolver>();
builder.Services.AddScoped<IMicStepsService, MicStepsService>();
builder.Services.AddScoped<IReminderService, ReminderService>();
builder.Services.AddScoped<IEmailNotificationService, EmailNotificationService>();
builder.Services.Configure<SendGridClientOptions>(builder.Configuration.GetSection("SendGrid"));
builder.Services.AddSendGrid(_ => { });

builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo {Title = "My API", Version = "v1"});
    c.EnableAnnotations();
    c.SchemaFilter<CustomSchemaFilters>();
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = @"JWT Authorization header using the Bearer scheme.
                      Enter 'Bearer' [space] and then your token in the text input below.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

builder.Services.AddScheduler();
builder.Services.AddScoped<SentinelEntryReminderEmailJob>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseBlazorFrameworkFiles();
app.UseStaticFiles();
app.MapRazorPages();
app.MapControllers();
app.MapFallbackToFile("index.html");
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "NRZMyk API V1");
});

using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var loggerFactory = services.GetRequiredService<ILoggerFactory>();
    var configuration = services.GetRequiredService<IConfiguration>();
    var seedSettings = configuration.Get<DatabaseSeedSettings>();

    try
    {
        var catalogContext = services.GetRequiredService<ApplicationDbContext>();
        await ApplicationDbContextSeed.SeedAsync(catalogContext, loggerFactory, seedSettings.DatabaseSeed).ConfigureAwait(false);
    }
    catch (Exception ex)
    {
        var logger = loggerFactory.CreateLogger<Program>();
        logger.LogError(ex, "An error occurred seeding the DB.");
    }
}

app.Run();

public partial class Program; // for being able to be referenced from tests