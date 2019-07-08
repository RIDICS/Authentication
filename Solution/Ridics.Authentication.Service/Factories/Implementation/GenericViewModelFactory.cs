using System.Collections.Generic;
using System.Linq;
using Ridics.Authentication.Service.Factories.Interface;
using Ridics.Authentication.Service.Models.ViewModel;

namespace Ridics.Authentication.Service.Factories.Implementation
{
    public class GenericViewModelFactory : IGenericViewModelFactory
    {
        public ConfirmDialogViewModel GetConfirmDialogViewmodel(string id, string dialogTitle, string dialogMessage)
        {
            return new ConfirmDialogViewModel
            {
                Id = id,
                Title = dialogTitle,
                Message = dialogMessage
            };
        }

        public ConfirmDialogViewModel GetConfirmDialogViewmodel(string dialogTitle, string dialogMessage)
        {
            return GetConfirmDialogViewmodel("confirmModal", dialogTitle, dialogMessage);
        }

        public ListViewModel<T> GetListViewModel<T>(IList<T> items, string dialogTitle, string dialogMessage, int itemsCount,
            int itemsOnPage)
        {
            var vm = GetListViewModel(items, dialogTitle, dialogMessage, itemsCount);
            vm.Pagination.ItemsOnPage = itemsOnPage;
            return vm;
        }

        public ListViewModel<T> GetListViewModel<T>(IList<T> items, string dialogTitle, string dialogMessage, int itemsCount)
        {
            var confirmDialog = GetConfirmDialogViewmodel(dialogTitle, dialogMessage);

            var vm = new ListViewModel<T>
            {
                Items = items,
                DeleteConfirmDialog = confirmDialog,
                Pagination = new PaginationViewModel
                {
                    ItemsCount = itemsCount
                }
            };

            return vm;
        }

        public ViewModel<T> GetViewModel<T>(T item, string dialogTitle, string dialogMessage)
        {
            var confirmDialog = GetConfirmDialogViewmodel(dialogTitle, dialogMessage);

            var vm = new ViewModel<T>
            {
                Item = item,
                DeleteConfirmDialog = confirmDialog
            };

            return vm;
        }

        public List<SelectableViewModel<T>> GetSelectableViewmodelList<T>(IEnumerable<T> viewModelList)
        {
            return viewModelList.Select(x => new SelectableViewModel<T> {Item = x}).ToList();
        }
    }
}