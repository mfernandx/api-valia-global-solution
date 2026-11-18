using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace ValiaApiWeb.Data
{
    public class ValiaDbContextFactory : IDesignTimeDbContextFactory<ValiaDbContext>
    {
        public ValiaDbContext CreateDbContext(string[] args)
        {
            // 读取当前项目目录下的 appsettings.json
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development"}.json", optional: true)
                .Build();

            var connectionString = configuration.GetConnectionString("OracleConnection");

            if (string.IsNullOrWhiteSpace(connectionString))
            {
                throw new InvalidOperationException(
                    "A conexão OracleConnection não foi encontrada no appsettings.json. " +
                    "Verifique se o arquivo existe e contém a chave 'ConnectionStrings:OracleConnection'."
                );
            }

            var optionsBuilder = new DbContextOptionsBuilder<ValiaDbContext>();
            optionsBuilder.UseOracle(
                connectionString,
                oracleOptions => oracleOptions.UseOracleSQLCompatibility(
                    OracleSQLCompatibility.DatabaseVersion21
                )
            );

            return new ValiaDbContext(optionsBuilder.Options);
        }
    }
}