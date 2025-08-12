using Microsoft.EntityFrameworkCore;
using Npgsql;
using WarehouseManagement.Data;

namespace WarehouseManagement.Extensions
{
    public static class DatabaseExtensions
    {
        public static void InitDatabase(this WebApplication app)
        {
            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            
            var context = services.GetRequiredService<WarehouseContext>();
            var configuration = services.GetRequiredService<IConfiguration>();
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            
            EnsurePostgreSQLDatabaseExists(connectionString);
            context.Database.Migrate();  
            DbInitializer.Initialize(context);
        }
        
        private static void EnsurePostgreSQLDatabaseExists(string? connectionString)
        {
            var builder = new NpgsqlConnectionStringBuilder(connectionString);
            var databaseName = builder.Database;
            
            builder.Database = "postgres";
            using var connection = new NpgsqlConnection(builder.ToString());
            connection.Open();
            
            using var checkCommand = connection.CreateCommand();
            checkCommand.CommandText = $"SELECT 1 FROM pg_database WHERE datname = '{databaseName}'";
            var exists = checkCommand.ExecuteScalar() != null;
            
            if (!exists)
            {
                using var createCommand = connection.CreateCommand();
                createCommand.CommandText = $"CREATE DATABASE \"{databaseName}\"";
                createCommand.ExecuteNonQuery();
            }
        }
    }
}