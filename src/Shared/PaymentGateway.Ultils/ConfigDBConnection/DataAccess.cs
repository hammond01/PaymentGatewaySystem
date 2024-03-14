using System.Data;
using System.Data.SqlClient;
using Dapper;
using Microsoft.Extensions.Configuration;
using PaymentGateway.Ultils.ConfigDBConnection.Impl;

namespace PaymentGateway.Ultils.ConfigDBConnection;

public class DataAccess : IDataAccess
{
    private readonly IConfiguration _config;

    public DataAccess(IConfiguration config)
    {
        _config = config;
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
        catch
        {
            return false;
        }
    }
}