namespace Ridics.Authentication.Service.Models.ViewModel
{
    public class ViewModel<T>
    {
        public T Item { get; set; }

        public ConfirmDialogViewModel DeleteConfirmDialog { get; set; }
    }
}