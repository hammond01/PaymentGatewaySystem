namespace PaymentGateway.Ultils.ConfigDBConnection.Impl;

public interface IDataAccess
{
    Task<IEnumerable<T>> GetData<T, TP>(string query, TP parameters, string connectionId = "SQL");
    Task<bool> SaveData<TP>(string query, TP parameters, string connectionId = "SQL");
    Task<bool> InsertData<TP>(string table, TP parameters, string connectionId = "SQL");
    Task<bool> UpdateData<TP>(string tableName, string updatedId, TP parameters, string connectionId = "SQL");
    Task<bool> DeleteData<TP>(string tableName, TP parameters, string connectionId = "SQL");
    Task<bool> DeleteDataFromClient(string tableName, string deteledId, string connectionId = "SQL");
    Task<IEnumerable<T>> LoadData<T, TP>(string tableName, TP parameters, string connectionId = "SQL");

}