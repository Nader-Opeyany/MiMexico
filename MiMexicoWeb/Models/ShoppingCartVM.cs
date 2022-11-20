using Microsoft.AspNetCore.Mvc.Rendering;

namespace MiMexicoWeb.Models.ViewModel
{
	public class ShoppingCartVM
	{
		public IEnumerable<ShoppingCart> ListCart { get; set; }

		public IEnumerable<Meat> MeatList { get; set; }

		public OrderHeader OrderHeader { get; set; }
	}
}
