﻿using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Http.Headers;

namespace MiMexicoWeb.Models
{
	public class OrderDetails
	{
		public int Id { get; set; }
		[Required]
		public int OrderId { get; set; }
		[ForeignKey("OrderId")]
		[ValidateNever]
		public OrderHeader OrderHeader { get; set; }
		[Required]
		public int ProductId { get; set; }
		[ForeignKey("ProdcutId")]
		[ValidateNever]
		public Item item { get; set; }
		public int Count { get; set; }
		public double Price { get; set; }
	}
}
