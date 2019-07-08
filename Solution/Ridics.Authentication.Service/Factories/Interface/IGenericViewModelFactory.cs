using System.Collections.Generic;
using Ridics.Authentication.Service.Models.ViewModel;

namespace Ridics.Authentication.Service.Factories.Interface
{
    public interface IGenericViewModelFactory
    {
        ConfirmDialogViewModel GetConfirmDialogViewmodel(string dialogTitle, string dialogMessage);
        
        ConfirmDialogViewModel GetConfirmDialogViewmodel(string id, string dialogTitle, string dialogMessage);

        ListViewModel<T> GetListViewModel<T>(IList<T> items, string dialogTitle, string dialogMessage, int itemsCount, int itemOnPage);

        ListViewModel<T> GetListViewModel<T>(IList<T> items, string dialogTitle, string dialogMessage, int itemsCount);

        ViewModel<T> GetViewModel<T>(T item, string dialogTitle, string dialogMessage);

        List<SelectableViewModel<T>> GetSelectableViewmodelList<T>(IEnumerable<T> viewModelList);
    }
}