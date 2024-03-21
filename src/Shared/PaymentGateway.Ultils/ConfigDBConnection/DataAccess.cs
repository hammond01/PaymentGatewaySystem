using Dapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PaymentGateway.Ultils.ConfigDBConnection.Impl;
using Serilog;
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

    public async Task<bool> InsertData<TP>(string tableName, TP parameters, string connectionId = "SQL")
    {
        try
        {
            using IDbConnection connection = new SqlConnection(_config.GetConnectionString(connectionId));
            var columns = connection.Query<string>(
                @"SELECT COLUMN_NAME
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_NAME = @TableName AND COLUMN_NAME != 'Id'",
                new { TableName = tableName });
            var dynamicParameters = new DynamicParameters();

            var enumerable = columns.ToList();
            foreach (var column in enumerable)
            {
                var propertyValue = parameters!.GetType().GetProperty(column)?.GetValue(parameters, null);
                if (propertyValue != null) dynamicParameters.Add(column, propertyValue);
            }

            var insertQuery = $"INSERT INTO {tableName} ({string.Join(", ",
                dynamicParameters.ParameterNames.Select(column => $"{column}"))}) " +
                              $"VALUES (@{string.Join(", @",
                                  dynamicParameters.ParameterNames.Select(column => $"{column}"))})";
            await connection.ExecuteAsync(insertQuery, parameters);
            return true;
        }
        catch
        {
            Log.Error($"Insert data to table {tableName} error.");
            throw;
        }
    }

    public async Task<bool> UpdateData<TP>(string tableName, string updatedId, TP parameters, string connectionId = "SQL")
    {
        try
        {
            using IDbConnection connection = new SqlConnection(_config.GetConnectionString(connectionId));
            var primaryKeyColumnName = await GetPrimaryKey(tableName, connectionId);

            var columns = await connection.QueryAsync<string>(
                @$"SELECT COLUMN_NAME
                        FROM INFORMATION_SCHEMA.COLUMNS
                        WHERE TABLE_NAME = @TableName AND COLUMN_NAME != 'Id' AND COLUMN_NAME != '{primaryKeyColumnName}'",
                new { TableName = tableName });

            var dynamicParameters = new DynamicParameters();

            var enumerable = columns.AsList();
            foreach (var column in enumerable)
            {
                var propertyValue = parameters!.GetType().GetProperty(column)?.GetValue(parameters, null);
                if (propertyValue != null) dynamicParameters.Add(column, propertyValue);
            }

            Console.WriteLine(parameters);
            var updateColumns = string.Join(", ",
                dynamicParameters.ParameterNames.Select(column => $"{column} = @{column}"));
            var updateQuery = $"UPDATE {tableName} SET {updateColumns} WHERE {primaryKeyColumnName} = '{updatedId}'";

            await connection.ExecuteAsync(updateQuery, dynamicParameters);
            return true;
        }
        catch
        {
            Log.Error($"Update data from table {tableName} error.");
            throw;
        }
    }

    public async Task<bool> DeleteData<TP>(string tableName, TP parameters, string connectionId = "SQL")
    {
        try
        {
            using IDbConnection connection = new SqlConnection(_config.GetConnectionString(connectionId));
            var primaryKeyColumnName = await GetPrimaryKey(tableName, connectionId);

            var deleteQuery = $"DELETE FROM {tableName} WHERE {primaryKeyColumnName} = @{primaryKeyColumnName}";
            await connection.ExecuteAsync(deleteQuery, parameters);
            return true;
        }
        catch
        {
            Log.Error($"Delete data from table {tableName} error.");
            throw;
        }
    }

    //Delete on the interface still saves data in the database, so I created a new method to update the IsActive column to 0
    public async Task<bool> DeleteDataFromClient<TP>(string tableName, TP parameters, string connectionId = "SQL")
    {
        try
        {
            using IDbConnection connection = new SqlConnection(_config.GetConnectionString(connectionId));
            var primaryKeyColumnName = await GetPrimaryKey(tableName, connectionId);

            var deleteQuery =
                $"UPDATE {tableName} SET Deleted = 1 WHERE {primaryKeyColumnName} = @{primaryKeyColumnName}";
            await connection.ExecuteAsync(deleteQuery, parameters);
            return true;
        }
        catch
        {
            Log.Error($"Delete data from table {tableName} error.");
            throw;
        }
    }

    private async Task<string> GetPrimaryKey(string tableName, string connectionString = "SQL")
    {
        try
        {
            using IDbConnection connection = new SqlConnection(_config.GetConnectionString(connectionString));
            var query = @$"SELECT COLUMN_NAME
                            FROM INFORMATION_SCHEMA.KEY_COLUMN_USAGE
                            WHERE OBJECTPROPERTY(OBJECT_ID(CONSTRAINT_SCHEMA + '.' + CONSTRAINT_NAME), 'IsPrimaryKey') = 1
                            AND TABLE_NAME = '{tableName}';";
            var data = await connection.QuerySingleAsync<string>(query, new { });
            return data!;
        }
        catch
        {
            Log.Error($"Get primary key from table {tableName} error.");
            throw;
        }
    }
}