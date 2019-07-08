using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Ridics.Authentication.DataEntities.Entities
{
    public class DynamicModuleEntity : IEquatable<DynamicModuleEntity>
    {
        public virtual int Id { get; protected set; }

        public virtual Guid ModuleGuid { get; set; }

        public virtual string Name { get; set; }

        public virtual Version ConfigurationVersion { get; set; }

        private string m_configuration;

        public virtual string Configuration
        {
            get => m_configuration;
            set
            {
                m_configuration = value;
                m_configurationChecksum = null;
            }
        }

        private string m_configurationChecksum;

        public virtual string ConfigurationChecksum
        {
            get => m_configurationChecksum ?? ComputeChecksum();
            protected set => m_configurationChecksum = value;
        }

        public virtual ISet<DynamicModuleBlobEntity> DynamicModuleBlobs { get; set; }

        private string ComputeChecksum()
        {
            var stringBuilder = new StringBuilder();

            stringBuilder.Append(ModuleGuid);
            stringBuilder.Append(Name);
            stringBuilder.Append(ConfigurationVersion);
            stringBuilder.Append(Configuration ?? "");

            foreach (var dynamicModuleBlob in DynamicModuleBlobs ?? new HashSet<DynamicModuleBlobEntity>())
            {
                stringBuilder.Append(
                    dynamicModuleBlob.SerializeState()
                );
            }

            using (var hash = SHA256.Create())
            {
                return string.Concat(
                    hash.ComputeHash(Encoding.UTF8.GetBytes(
                        stringBuilder.ToString()
                    )).Select(item => item.ToString("x2"))
                );
            }
        }

        public virtual bool Equals(DynamicModuleEntity other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Id == other.Id;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != GetType()) return false;
            return Equals((DynamicModuleEntity) obj);
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}