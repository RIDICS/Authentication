using System;
using Ridics.Authentication.Core.Models.Enum;

namespace Ridics.Authentication.Core.Models
{
    public class FileResourceModel
    {
        public int Id { get; set; }

        public Guid Guid { get; set; }

        public FileResourceEnumModel Type { get; set; }

        public string FileExtension { get; set; }
    }
}
