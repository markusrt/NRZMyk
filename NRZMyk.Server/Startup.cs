using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureADB2C.UI;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using Microsoft.OpenApi.Models;
using NRZMyk.Server.Model;
using NRZMyk.Services.Configuration;
using NRZMyk.Services.Data;
using NRZMyk.Services.Interfaces;
using NRZMyk.Services.Services;

namespace NRZMyk.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            ConfigureAzureAdB2C(services);
            ConfigureSwagger(services);

            services.AddAutoMapper(typeof(Startup).Assembly);

            services.AddControllersWithViews();
            services.AddRazorPages();

            services.AddMvc().AddRazorPagesOptions(options => { options.RootDirectory = "/"; });
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            services.AddApplicationInsightsTelemetry();

            services.AddSingleton<WeatherForecastService>();
            services.AddScoped(typeof(IAsyncRepository<>), typeof(EfRepository<>));
            services.Configure<CatalogSettings>(Configuration);
        }



        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                IdentityModelEventSource.ShowPII = true;
                app.UseDeveloperExceptionPage();
                app.UseWebAssemblyDebugging();
                app.UseDatabaseErrorPage();
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
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
            });
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
                            //Get the calling app client id that came from the token produced by Azure AD
                            var roleGroups = new Dictionary<string, string>();
                            Configuration.Bind("AuthorizationGroups", roleGroups);
                            var roles = ctx.Principal.Claims.Where(c => c.Type == "extension_Role").ToList();
                            var singleRole = roles.FirstOrDefault()?.Value;
                            if (!string.IsNullOrEmpty(singleRole) && Enum.TryParse<Role>(singleRole, out var role))
                            {
                                var claims = new List<Claim>();
                                if (role.HasFlag(Role.Guest))
                                {
                                    claims.Add(new Claim(ClaimTypes.Role, "User"));
                                }

                                if (role.HasFlag(Role.User))
                                {
                                    claims.Add(new Claim(ClaimTypes.Role, "User"));
                                }

                                if (role.HasFlag(Role.Admin))
                                {
                                    claims.Add(new Claim(ClaimTypes.Role, "Admin"));
                                }

                                if (role.HasFlag(Role.SuperUser))
                                {
                                    claims.Add(new Claim(ClaimTypes.Role, "SuperUser"));
                                }

                                var appIdentity = new ClaimsIdentity(claims);
                                ctx.Principal.AddIdentity(appIdentity);
                            }
                        }
                    };
                }
            );
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
