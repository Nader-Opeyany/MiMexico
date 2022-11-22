using Microsoft.Build.Framework;
using System.Data;

namespace MiMexicoWeb.Models
{
	public class OrderHeader
	{
		public int Id { get; set; }

		public int OrderHeaderId { get; set; }

		public DataSetDateTime OrderDate { get; set; }

		public double OrderTotal { get; set; }

		public string? OrderStatus { get; set; }

		public string? PaymentStatus { get; set; }

		public DateTime PaymentDate { get; set; }

		//public DateTime PaytmentDueDate { get; set; }

		// traction ID for stripe
		public string? SessionId { get; set; }

		public string? PaymentIntentId { get; set; }

		[Required]
		public string PhoneNumber { get; set; }

		[Required]
		public string Name { get; set; }





	}
}
