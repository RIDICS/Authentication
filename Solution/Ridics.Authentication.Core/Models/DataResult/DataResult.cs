namespace Ridics.Authentication.Core.Models.DataResult
{
    public class DataResult<T>
    {
        public T Result { get; set; }

        public bool HasError => Error != null;

        public bool Succeeded => !HasError;

        public DataResultError Error { get; set; }
    }
}