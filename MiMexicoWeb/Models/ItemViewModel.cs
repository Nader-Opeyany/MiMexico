
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MiMexicoWeb.Models.ViewModel

{
    public class ItemViewModel
    {
        public Item Item { get; set; }
        [ValidateNever]

        public IEnumerable<SelectListItem> MeatList { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem> CondimentList { get; set; }

  
    }
}