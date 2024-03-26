namespace PaymentGateway.Domain.Exceptions.ErrorMessage;

public class LayerErrorMessage
{
    public static string ERROR_AT_INFRASTRUCTURE(string msg) => $"An error occurred at the Infrastructure processing layer: {msg}";
    public static string ERROR_AT_PERSISTENCE(string msg) => $"An error occurred at the Persistence processing layer: {msg}";
    public static string ERROR_AT_APPLICATION(string msg) => $"An error occurred at the Application processing layer: {msg}";
}
