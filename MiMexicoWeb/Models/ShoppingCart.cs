using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace MiMexicoWeb.Models
{
    public class ShoppingCart
    {

        public int Id { get; set; }
        [Required]
        public int itemId { get; set; }
        [ForeignKey("itemId")]
        [ValidateNever]
        public Item Item { get; set; }
        public int meatId { get; set; }
        public int quantity { get; set; }

        [NotMapped]
        public double Price { get; set; }

        public int shoppingCartID { get; set; }
    }
}
