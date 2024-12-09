using System;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using FluentAssertions.Common;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NRZMyk.Server;
using NRZMyk.Services.Configuration;
using NRZMyk.Services.Data;
using Testcontainers.MsSql;

namespace Api.Integration.Tests
{
    public static class TestcontainerDbClientFactory
    {
        private static readonly WebApplicationFactory<Program> _factory;
        public static IServiceProvider ServiceProvider => _factory.Services;

        private static readonly MsSqlContainer _dbContainer = new MsSqlBuilder()
            .WithImage("mcr.microsoft.com/mssql/server:2022-latest")
            .WithPassword("Strong_password_123!")
            .Build();

        static TestcontainerDbClientFactory()
        {
            _dbContainer.StartAsync().Wait();

            _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddAuthentication(TestAuthHandler.AuthenticationScheme)
                        .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(
                            TestAuthHandler.AuthenticationScheme, _ => { });

                    var descriptor = services.SingleOrDefault(
                        d => d.ServiceType ==
                             typeof(DbContextOptions<ApplicationDbContext>));
                    if (descriptor != null)
                    {
                        services.Remove(descriptor);
                    }

                    services.AddDbContext<ApplicationDbContext>(options =>
                    {
                        options.UseSqlServer(_dbContainer.GetConnectionString());
                    });
                });
            });

            using (var scope = _factory.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var loggerFactory = services.GetRequiredService<ILoggerFactory>();
                var configuration = services.GetRequiredService<IConfiguration>();
                var seedSettings = configuration.Get<DatabaseSeedSettings>();

                try
                {
                    var catalogContext = services.GetRequiredService<ApplicationDbContext>();
                    ApplicationDbContextSeed.SeedAsync(catalogContext, loggerFactory, seedSettings.DatabaseSeed).Wait();
                }
                catch (Exception ex)
                {
                    var logger = loggerFactory.CreateLogger<Program>();
                    logger.LogError(ex, "An error occurred seeding the DB.");
                }
            }

            AppDomain.CurrentDomain.ProcessExit += Dispose;
        }

        private static void Dispose(object? sender, EventArgs e) {
            var disposeContainerTask = _dbContainer.DisposeAsync().AsTask();
            disposeContainerTask.Wait();
            _factory.Dispose();
        }

        public static HttpClient CreateClient()
        {
            var client = _factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                AllowAutoRedirect = false,
            });
            
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(TestAuthHandler.AuthenticationScheme);
            return client;
        }
    }
}
