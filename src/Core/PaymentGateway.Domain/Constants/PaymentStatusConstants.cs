namespace PaymentGateway.Domain.Constants;

public class PaymentStatusConstants
{
    //Trạng thái giao dịch
    public static string Pending = "Đang chờ thanh toán";
    public static string Success = "Thanh toán thành công";
    public static string Suspended = "Tạm ngưng thanh toán";
    public static string Error = "Có lỗi xảy ra trong quá trình thanh toán";

}