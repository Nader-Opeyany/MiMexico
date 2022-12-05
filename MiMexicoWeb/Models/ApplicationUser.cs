using Microsoft.AspNetCore.Identity;
using Microsoft.Build.Framework;

namespace MiMexicoWeb.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string Name { get; set; }
        public string? StreedAddress { get; set; }
        public string? City { get; set; }
        public string? Satate { get; set; }
        public string? PostalCode { get; set; }
    }
}
