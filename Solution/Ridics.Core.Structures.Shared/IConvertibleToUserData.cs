using System;

namespace Ridics.Core.Structures.Shared
{
    public interface IConvertibleToUserData
    {
        UserAddressingWays Title { get; set; }

        string Prefix { get; set; }

        Guid? MasterUserId { get; set; }

        string FirstName { get; set; }

        string LastName { get; set; }

        string SecondName { get; set; }

        string FullName { get; set; }

        string Suffix { get; set; }
    }
}
