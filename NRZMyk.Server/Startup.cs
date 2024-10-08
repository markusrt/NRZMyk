using System;
using System.Collections.Generic;
using System.Security.Claims;
using AutoMapper;
using Coravel;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureADB2C.UI;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Graph;
using Microsoft.IdentityModel.Logging;
using Microsoft.OpenApi.Models;
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
using ClaimTypes = NRZMyk.Services.Models.ClaimTypes;

namespace NRZMyk.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration, IWebHostEnvironment environment)
        {
            Configuration = configuration;
            Environment = environment;
        }

        public IConfiguration Configuration { get; }

        public IWebHostEnvironment Environment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureAzureAdB2C(services);
            ConfigureSwagger(services);

            services.AddAutoMapper(typeof(Startup).Assembly, typeof(ISentinelEntryService).Assembly);

            services.AddControllersWithViews();
                // TODO looks nice in swagger but need to find a way on how to make JsonStringEnumConverter
                //      working correctly in WebAssembly... i.e. how to configure it via DI there
                //.AddJsonOptions(x =>
                //{
                //    x.JsonSerializerOptions.WriteIndented = !Environment.IsProduction();
                //    x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                //});
            services.AddRazorPages();

            services.AddAuthorization(options => options.AddPolicy(Policies.AssignedToOrganization,
                policy => policy.RequireClaim(ClaimTypes.Organization)));

            services.AddMvc().AddRazorPagesOptions(options => { options.RootDirectory = "/"; });
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddApplicationInsightsTelemetry();

            services.AddScoped(typeof(IAsyncRepository<>), typeof(EfRepository<>));
            services.AddScoped<ISentinelEntryRepository, SentinelEntryRepository>();
            services.AddScoped<IProtectKeyToOrganizationResolver, ProtectKeyToOrganizationResolver>();
            services.AddScoped<IMicStepsService, MicStepsService>();
            services.AddScoped<IReminderService, ReminderService>();

            services.AddScoped<IEmailNotificationService, EmailNotificationService>();
            services.Configure<SendGridClientOptions>(Configuration.GetSection("SendGrid"));
            services.AddSendGrid(options => { });

            services.Configure<DatabaseSeedSettings>(Configuration);
            services.Configure<ApplicationSettings>(Configuration);

            services.AddDatabaseDeveloperPageExceptionFilter();

            services.AddScheduler();
            services.AddScoped<SentinelEntryReminderEmailJob>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
        {
            if (env.IsDevelopment())
            {
                IdentityModelEventSource.ShowPII = true;
                app.UseDeveloperExceptionPage();
                app.UseMigrationsEndPoint();
                app.UseWebAssemblyDebugging();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseBlazorFrameworkFiles();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            // Enable middleware to serve generated Swagger as a JSON endpoint.
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "NRZMyk API V1");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
            });

            serviceProvider.UseScheduler(scheduler => scheduler.Schedule<SentinelEntryReminderEmailJob>().DailyAt(7,30));
        }

        private void ConfigureAzureAdB2C(IServiceCollection services)
        {
            services.AddAuthentication(AzureADB2CDefaults.BearerAuthenticationScheme)
                .AddAzureADB2CBearer(options => Configuration.Bind("AzureAdB2C", options));
            services.Configure<JwtBearerOptions>(
                AzureADB2CDefaults.JwtBearerAuthenticationScheme, options =>
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
                }
            );

            services.Configure<AzureAdB2CSettings>(Configuration);
            services.AddScoped<IGraphServiceClient, GraphServiceClientWrapper>();
            services.AddScoped<IUserService, UserService>();
        }


        private static void ConfigureSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
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
                c.AddSecurityRequirement(new OpenApiSecurityRequirement()
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
        }
    }
}
