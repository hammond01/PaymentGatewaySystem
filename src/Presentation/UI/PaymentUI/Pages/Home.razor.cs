namespace PaymentUI.Pages;

public partial class Home
{
    private readonly PaymentModel paymentModel = new();

    private void HandleSubmit()
    {
        // Handle form submission here
    }

    public class PaymentModel
    {
        public string PaymentType { get; set; }
        public decimal Amount { get; set; }
        public string ProductName { get; set; }
    }
}