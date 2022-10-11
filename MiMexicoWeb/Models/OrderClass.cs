namespace MiMexicoWeb.Models
{
    //testing
    public class OrderClass
    {
        public int orderId { get; set; }

        public String customerFirstName { get; set; }

        public String customerLastName { get; set; }

        public String[] order { get; set; } = new string[50];
        
        public String specialInstructions { get; set; }
        
        public float price { get; set; }

        public bool completed { get; set; }

    }
}
