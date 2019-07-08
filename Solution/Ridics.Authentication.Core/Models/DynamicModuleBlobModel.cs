using System;
using Ridics.Authentication.Core.Models.Enum;

namespace Ridics.Authentication.Core.Models
{
    public class DynamicModuleBlobModel
    {
        public int Id { get; protected set; }

        public DynamicModuleBlobEnumModel Type { get; set; }

        public FileResourceModel File { get; set; }

        public DateTime LastChange { get; set; }

        public string SerializeState()
        {
            return LastChange.ToString("o");
        }
    }
}
