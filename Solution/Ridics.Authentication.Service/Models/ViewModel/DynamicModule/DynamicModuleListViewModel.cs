using System;

namespace Ridics.Authentication.Service.Models.ViewModel.DynamicModule
{
    public class DynamicModuleListViewModel
    {
        public ListViewModel<DynamicModuleViewModel> ListViewModel;
        
        public ConfirmDialogViewModel ApplyChangesConfirmDialog;

        public DateTime LastConfigurationReload;
    }
}