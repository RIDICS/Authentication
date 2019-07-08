using System;

namespace Ridics.Core.Shared.Providers
{
    public interface IDateTimeProvider
    {
        DateTime UtcNow { get; }
    }
}