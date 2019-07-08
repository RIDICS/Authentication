using System.Net.Http;

namespace Ridics.Authentication.HttpClient.Contract
{
    public class StreamHydrator<TContract> where TContract : StreamContract
    {
        public virtual void Hydrate(
            TContract responseObject,
            HttpResponseMessage response
        )
        {
        }
    }
}