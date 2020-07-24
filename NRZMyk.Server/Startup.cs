using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.AzureADB2C.UI;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Logging;
using NRZMyk.Server.Model;
using NRZMyk.Services.Data;
using NRZMyk.Services.Service;

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
            services.AddAuthentication(AzureADB2CDefaults.BearerAuthenticationScheme)
                .AddAzureADB2CBearer(options => Configuration.Bind("AzureAdB2C", options))
                .AddJwtBearer(o =>
                {
                    //Additional config snipped
                    o.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = async ctx =>
                        {
                            //Get the calling app client id that came from the token produced by Azure AD
                            string clientId = ctx.Principal.FindFirstValue("appid");


                        }
                    };
                }); ;
            //services.AddSignIn(Configuration);

            services.AddControllersWithViews();
            services.AddRazorPages();

            services.Configure<JwtBearerOptions>(
                AzureADB2CDefaults.JwtBearerAuthenticationScheme, options =>
                {
                    options.TokenValidationParameters.NameClaimType = "name";
                    options.Events = new JwtBearerEvents
                    {
                        OnTokenValidated = async ctx =>
                        {
                            //Get the calling app client id that came from the token produced by Azure AD
                            string clientId = ctx.Principal.FindFirstValue("appid");
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
                            //var jwtToken = ctx.SecurityToken as JwtSecurityToken;
                            //var graphService = await GraphService.CreateOnBehalfOfUserAsync(jwtToken.RawData, Configuration);
                            //var memberGroups = await graphService.CheckMemberGroupsAsync(roleGroups.Keys);

                            ////var claims = memberGroups.Select(groupGuid => new Claim(ClaimTypes.Role, roleGroups[groupGuid]));
                            ////var appIdentity = new ClaimsIdentity(claims);
                            //ctx.Principal.AddIdentity(appIdentity);


                        }
                    };
                });

            services.AddMvc().AddRazorPagesOptions(options => { options.RootDirectory = "/"; });
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));
            //services.AddRazorPages().AddMicrosoftIdentityUI();
            services.AddServerSideBlazor();
            services.AddSingleton<WeatherForecastService>();

            services.AddApplicationInsightsTelemetry();
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

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
                endpoints.MapControllers();
                endpoints.MapFallbackToFile("index.html");
            });
        }
    }
}
