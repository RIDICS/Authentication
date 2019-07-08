using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Ridics.Authentication.Service.Models.ViewModel.ClaimTypes;

namespace Ridics.Authentication.Service.Models.ViewModel.Users.Claims
{
    public class EditableClaimViewModel : ClaimViewModel
    {
        public int UserId { get; set; }

        [DisplayName("type")]
        [Required(ErrorMessage = "type-required")]
        public int SelectedClaimType { get; set; }

        public IList<ClaimTypeViewModel> ClaimTypes { get; set; }
    }
}