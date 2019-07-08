using System.Collections.Generic;
using Ridics.Authentication.Service.Models.ViewModel.ClaimTypes;

namespace Ridics.Authentication.Service.Models.ViewModel.Resources.IdentityResources
{
    public class EditableIdentityResourceViewModel
    {
        public IdentityResourceViewModel IdentityResourceViewModel { get; set; }

        public IList<SelectableViewModel<ClaimTypeViewModel>> SelectableClaimTypes { get; set; }
    }
}