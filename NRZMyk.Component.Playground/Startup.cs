using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NRZMyk.Mocks.MockServices;
using NRZMyk.Services.Configuration;
using NRZMyk.Services.Services;

namespace NRZMyk.Components.Playground
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

        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<BreakpointSettings>(Configuration);

            services.AddAutoMapper(typeof(Startup).Assembly, typeof(SentinelEntryService).Assembly);
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddSingleton<SentinelEntryService, MockSentinelEntryServiceImpl>();
            services.AddSingleton<ClinicalBreakpointService, MockClinicalBreakpointServiceImpl>();
            services.AddSingleton<MicStepsService, MicStepsServiceImpl>();

            services.AddScoped<AuthenticationStateProvider, MockAuthStateProvider>();
            services.AddScoped<SignOutSessionStateManager>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
            });
        }
    }
}
