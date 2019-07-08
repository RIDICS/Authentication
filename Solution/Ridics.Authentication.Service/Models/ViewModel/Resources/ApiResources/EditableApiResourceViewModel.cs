using System.Collections.Generic;
using Ridics.Authentication.Service.Models.ViewModel.ClaimTypes;

namespace Ridics.Authentication.Service.Models.ViewModel.Resources.ApiResources
{
    public class EditableApiResourceViewModel
    {
        public ApiResourceViewModel ApiResourceViewModel { get; set; }

        public IList<SelectableViewModel<ClaimTypeViewModel>> SelectableClaimTypes { get; set; }
    }
}