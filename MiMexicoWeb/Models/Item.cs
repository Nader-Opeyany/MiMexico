using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MiMexicoWeb.Models
{
    public class Item
    {
        public int id { get; set; }
        [Required]
        public string name { get; set; }
        public string description { get; set; }

        [Required]
        [Range(1, 100)]
        public double price { get; set; }

        public int meatId { get; set; }

        [ForeignKey("meatId")]
        [ValidateNever]

        public Meat Meat { get; set; }

        public int condimentId { get; set; }

        [ForeignKey("condimentId")]
        [ValidateNever]

        public Condiment Condiment { get; set; }

        public string ImageUrl { get; set; }
    }
}
