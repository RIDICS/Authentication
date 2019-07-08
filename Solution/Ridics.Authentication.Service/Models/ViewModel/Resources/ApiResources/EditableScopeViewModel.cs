using System.Collections.Generic;
using Ridics.Authentication.Service.Models.ViewModel.ClaimTypes;

namespace Ridics.Authentication.Service.Models.ViewModel.Resources.ApiResources
{
    public class EditableScopeViewModel : ScopeViewModel
    {
        public int ApiResourceId { get; set; }

        public IList<SelectableViewModel<ClaimTypeViewModel>> SelectableClaimTypes { get; set; }
    }
}