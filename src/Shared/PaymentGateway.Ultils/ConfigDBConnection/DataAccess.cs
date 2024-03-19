using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PaymentGateway.Ultils.ConfigDBConnection.Impl;
using System.Data;
using System.Data.SqlClient;

namespace PaymentGateway.Ultils.ConfigDBConnection;

public class DataAccess : IDataAccess
{
    private readonly IConfiguration _config;
    private readonly ILogger<DataAccess> _logger;

    public DataAccess(IConfiguration config, ILogger<DataAccess> logger)
    {
        _config = config;
        _logger = logger;
    }

    // ReSharper disable once InconsistentNaming
    public async Task<IEnumerable<T>> GetData<T, P>(string query, P parameters, string connectionId = "SQL")
    {
        using IDbConnection connection = new SqlConnection(_config.GetConnectionString(connectionId));
        return await connection.QueryAsync<T>(query, parameters);
    }

    public async Task<bool> SaveData<P>(string query, P parameters, string connectionId = "SQL")
    {
        try
        {
            using IDbConnection connection = new SqlConnection(_config.GetConnectionString(connectionId));
            await connection.ExecuteAsync(query, parameters);
            return true;
        }
        catch (Exception e)
        {
            _logger.LogError($"Save database error: {e.Message}");
            return false;
        }
    }
}