using Microsoft.Extensions.Configuration;
using Npgsql;
using System;
using System.Data;

namespace PAN.API.Infrastructure.Dapper;

public class DapperContext
{
    private readonly string _connectionString;

    public DapperContext(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("Default");

        // ✅ DEBUG
        Console.WriteLine("Connection String: " + _connectionString);

        // ✅ SAFETY (prevents silent failure)
        if (string.IsNullOrEmpty(_connectionString))
        {
            throw new Exception("Connection string is NULL. Check appsettings.json");
        }
    }

    public IDbConnection CreateConnection()
        => new NpgsqlConnection(_connectionString);
}