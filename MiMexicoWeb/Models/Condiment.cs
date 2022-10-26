using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace MiMexicoWeb.Models
{
    public class Condiment
    {
        [Key]
        public int id { get; set; }
        [Required]
        public string name { get; set; }
    }
}
