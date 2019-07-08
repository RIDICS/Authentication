using System.IO;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Ridics.Authentication.Core.Configuration;
using Ridics.Authentication.Core.Models;
using Ridics.Authentication.Shared;

namespace Ridics.Authentication.Core.Managers
{
    public sealed class LocalFileResourceManager : ManagerBase, IFileResourceManager
    {
        private const string DefaultResourceLocation = "FileResource";

        private readonly string m_fileResourceLocation;
        private readonly PathConfiguration m_pathConfiguration;

        public LocalFileResourceManager(
            PathConfiguration pathConfiguration,
            ILogger logger, ITranslator translator, IMapper mapper, IPaginationConfiguration paginationConfiguration
        ) : this(DefaultResourceLocation, pathConfiguration, logger, translator, mapper, paginationConfiguration)
        {
        }

        public LocalFileResourceManager(
            string fileResourceLocation,
            PathConfiguration pathConfiguration,
            ILogger logger, ITranslator translator, IMapper mapper, IPaginationConfiguration paginationConfiguration
        ) : base(logger, translator, mapper, paginationConfiguration)
        {
            m_fileResourceLocation = fileResourceLocation;
            m_pathConfiguration = pathConfiguration;
        }

        public string ResolveFullPath(FileResourceModel fileResource)
        {
            return string.Format("{0}/{1}.{2}", m_fileResourceLocation, fileResource.Guid, fileResource.FileExtension);
        }

        public string ResolveName(FileResourceModel fileResource)
        {
            return string.Format("{0}.{1}", fileResource.Guid, fileResource.FileExtension);
        }

        public FileStream GetReadStream(FileResourceModel fileResource)
        {
            try
            {
                return File.Open(
                    Path.Combine(
                        m_pathConfiguration.WebRootPath,
                        m_fileResourceLocation,
                        ResolveName(fileResource)
                    ),
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read
                );
            }
            catch (FileNotFoundException)
            {
                return null;
            }
        }

        public FileStream GetWriteStream(FileResourceModel fileResource)
        {
            try
            {
                return File.Open(
                    Path.Combine(
                        m_pathConfiguration.WebRootPath,
                        m_fileResourceLocation,
                        ResolveName(fileResource)
                    ),
                    FileMode.OpenOrCreate,
                    FileAccess.Write,
                    FileShare.Write
                );
            }
            catch (FileNotFoundException)
            {
                return null;
            }
        }
    }
}
