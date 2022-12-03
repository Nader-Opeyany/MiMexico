namespace MiMexicoWeb.Models
{
    public class OrderViewModel
    {
        public OrderHeader OrderHeader { get; set; }

        public IEnumerable<OrderDetails> OrderDetails { get; set; }
    }
}
