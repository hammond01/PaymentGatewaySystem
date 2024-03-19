namespace PaymentGateway.Domain.Constants;

public class MessageConstants
{
    public const string InternalServerError = "Internal Server Error";
    public const string PaymentTransactionNotFound = "Payment Transaction Not Found";
    public const string PaymentTransactionCreated = "Payment Transaction Created";
    public const string PaymentTransactionUpdated = "Payment Transaction Updated";
}

public class MessageConstantsWithValue
{
    public static string createSuccess(string serviceName) => $"Create new {serviceName} successfully";
    public static string createFail(string serviceName) => $"Create new {serviceName} fail";
    public static string updateSuccess(string serviceName) => $"{serviceName} has been updated successfully";
    public static string updateFail(string serviceName) => $"{serviceName} has been updated fail";
    public static string getDataSuccess(string serviceName) => $"Get {serviceName} successfully";
    public static string getDataFail(string serviceName) => $"Get {serviceName} fail";
}