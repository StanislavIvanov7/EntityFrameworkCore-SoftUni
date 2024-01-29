namespace FastFood.Core.ViewModels.Orders
{
    public class CreateOrderInputModel
    {
        public string Customer { get; set; }

        public string Item { get; set; }

        public string Employee { get; set; }

        public int Quantity { get; set; }
    }
}
