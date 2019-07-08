using System;

namespace Ridics.Authentication.TicketStore.Store
{
    public class CacheTicketStoreConfig
    {
        public TimeSpan SlidingExpiration { get; set; }
    }
}