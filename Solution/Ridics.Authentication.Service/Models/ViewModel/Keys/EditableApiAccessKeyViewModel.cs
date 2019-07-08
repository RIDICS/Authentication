using System.Collections.Generic;

namespace Ridics.Authentication.Service.Models.ViewModel.Keys
{
    public class EditableApiAccessKeyViewModel : ApiAccessKeyViewModel
    {
        public IList<SelectableViewModel<ApiAccessPermissionEnumViewModel>> SelectableApiPermissions { get; set; }

        public EditableApiAccessKeyHashViewModel EditableApiAccessKeyHashViewModel { get; set; }
    }
}