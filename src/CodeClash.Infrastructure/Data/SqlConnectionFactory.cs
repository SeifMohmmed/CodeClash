using System.Data;
using CodeClash.Application.Abstractions.Data;
using Npgsql;

namespace CodeClash.Infrastructure.Data;
/// <summary>
/// Factory responsible for creating raw SQL database connections.
/// Used mainly for Dapper or read-only queries (CQRS - Query side).
/// </summary>
internal sealed class SqlConnectionFactory : ISqlConnectionFactory
{
    private readonly string _connectionString;

    /// <summary>
    /// Initializes the factory with the database connection string.
    /// </summary>
    public SqlConnectionFactory(string connectionString)
    {
        _connectionString = connectionString;
    }

    /// <summary>
    /// Creates and opens a new database connection.
    /// Caller is responsible for disposing the connection.
    /// </summary>
    public IDbConnection CreateConnection()
    {
        // Create PostgreSQL connection
        var connection = new NpgsqlConnection(_connectionString);

        // Open connection immediately
        // (Important for Dapper usage)
        connection.Open();

        return connection;
    }
}
