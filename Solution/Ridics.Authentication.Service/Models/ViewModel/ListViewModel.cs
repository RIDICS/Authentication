using System.Collections.Generic;

namespace Ridics.Authentication.Service.Models.ViewModel
{
    public class ListViewModel<T>
    {
        public IList<T> Items { get; set; }

        public ConfirmDialogViewModel DeleteConfirmDialog { get; set; }

        public PaginationViewModel Pagination { get; set; }
    }
}