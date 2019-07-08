using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Ridics.Authentication.Service.Models.ViewModel.Keys
{
    public class EditableApiAccessKeyHashViewModel : ApiAccessKeyHashViewModel
    {
        [Display(Name = "value")]
        [Required(ErrorMessage = "value-required")]
        public string Value { get; set; }

        public IList<SelectListItem> SelectableAlgorithms { get; set; }
    }
}