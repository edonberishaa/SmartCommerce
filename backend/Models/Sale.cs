namespace backend.Models
{
    public class Sale
    {
        public int Id { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public DateTime SaleDate { get; set; }

    }
}
