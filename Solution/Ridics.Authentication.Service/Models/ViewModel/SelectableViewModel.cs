namespace Ridics.Authentication.Service.Models.ViewModel
{
    public class SelectableViewModel<T>
    {
        public T Item { get; set; }

        public bool IsSelected { get; set; }
    }
}