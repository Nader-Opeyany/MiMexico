namespace MiMexicoWeb.Models.ViewModel
{
	public class ShoppingCartVM
	{
		public IEnumerable<ShoppingCart> ListCart { get; set; }

		public double CartTotal { get; set; }
	}
}
