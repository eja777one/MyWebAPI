using System.Data;
using System.Data.Common;
using DotNet.Testcontainers.Builders;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using MyWebAPI.Data;
using Npgsql;
using Respawn;
using Testcontainers.PostgreSql;

namespace MyWebApiTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
            .WithImage("postgres:latest")
            .WithDatabase("MyWebApiTestDb")
            .WithUsername("postgres")
            .WithPassword("123Abc123_")
            .WithWaitStrategy(Wait.ForUnixContainer().UntilCommandIsCompleted("pg_isready"))
            .WithCleanUp(true)
            .Build();

        public HttpClient HttpClient { get; private set; } = null!;

        public MainDbContext Db { get; private set; } = null!;
        private Respawner _respawner = null!;
        private DbConnection _dbConnection = null!;

        public async Task InitializeAsync()
        {
            await _dbContainer.StartAsync();

            Db = Services.CreateScope().ServiceProvider.GetRequiredService<MainDbContext>();
            //_dbConnection = new NpgsqlConnection(_dbContainer.GetConnectionString());
            _dbConnection = Db.Database.GetDbConnection();

            HttpClient = CreateClient();

            await _dbConnection.OpenAsync();
            await InitializeRespawnerAsync();
        }

        public new async Task DisposeAsync()
        {
            await _dbContainer.DisposeAsync();
            await _dbConnection.DisposeAsync();
        }

        public async Task ResetDatabaseAsync()
        {
            await _respawner.ResetAsync(_dbConnection);
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            //Environment.SetEnvironmentVariable("ConnectionStrings:Postgres", _dbContainer.GetConnectionString());

            builder.ConfigureTestServices(services =>
            {
                services.RemoveDbContext<MainDbContext>();
                services.AddDbContext<MainDbContext>(options =>
                {
                    options.UseNpgsql(_dbContainer.GetConnectionString());
                });
                services.EnsureDbCreated<MainDbContext>();
            });
        }

        private async Task InitializeRespawnerAsync()
        {
            _respawner = await Respawner.CreateAsync(_dbConnection, new RespawnerOptions
            {
                DbAdapter = DbAdapter.Postgres,
                SchemasToInclude = ["public"],
            });
        }
    }

    public static class ServiceCollectionExtensions
    {
        public static void RemoveDbContext<T>(this IServiceCollection services) where T : DbContext
        {
            var descriptor = services.SingleOrDefault(x => x.ServiceType == typeof(DbContextOptions<T>));
            if (descriptor != null)
            {
                services.Remove(descriptor);
            }
        }

        public static void EnsureDbCreated<T>(this IServiceCollection services) where T : DbContext
        {
            using var scope = services.BuildServiceProvider().CreateScope();
            var serviceProvider = scope.ServiceProvider;
            var context = serviceProvider.GetRequiredService<T>();
            context.Database.EnsureCreated();
        }
    }
}
