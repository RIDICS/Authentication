using System.Collections.Generic;
using Ridics.Authentication.Service.Models.ViewModel.Resources.ApiResources;
using Ridics.Authentication.Service.Models.ViewModel.Resources.IdentityResources;

namespace Ridics.Authentication.Service.Models.ViewModel.Clients
{
    public class EditableClientViewModel : ClientViewModel
    {
        public IList<SelectableViewModel<GrantTypeViewModel>> SelectableGrantTypes { get; set; }

        public IList<SelectableViewModel<IdentityResourceViewModel>> SelectableIdentityresources { get; set; }
        
        public IList<SelectableViewModel<ScopeViewModel>> SelectableScopes { get; set; }
    }
}