using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Ridics.Authentication.Service.Models.ViewModel.ClaimTypes
{
    public class ClaimTypeViewModel
    {
        public int Id { get; set; }

        [Display(Name = "name")]
        [Required(ErrorMessage = "name-required")]
        [DataType(DataType.Text)]
        public string Name { get; set; }

        [Display(Name = "type")]
        public ClaimTypeEnumViewModel Type { get; set; }

        [Display(Name = "description")]
        [DataType(DataType.Text)]
        public string Description { get; set; }

        [Display(Name = "type")]
        public int SelectedType { get; set; }

        public IList<ClaimTypeEnumViewModel> AllTypeValues { get; set; }
}
}