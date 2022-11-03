using System.ComponentModel.DataAnnotations;

namespace MiMexicoWeb.Models
{
    public class Meat
    {
        [Key]
        public int id { get; set; }
        [Required]
        public string name { get; set; }
    }
}
