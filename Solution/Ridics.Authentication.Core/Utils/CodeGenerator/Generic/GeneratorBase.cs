using System.Configuration;
using Ridics.Authentication.Core.Utils.CodeGenerator.Configuration;
using Ridics.Core.Shared.Utils.CodeGenerators;

namespace Ridics.Authentication.Core.Utils.CodeGenerator.Generic
{
    public abstract class GeneratorBase : IGenerator
    {
        private readonly IGenericCodeGenerator m_genericCodeGenerator;
        private readonly CodeGeneratorConfigurationBase m_configuration;
        private char[] m_allowedChars;

        protected GeneratorBase(IGenericCodeGenerator genericCodeGenerator, CodeGeneratorConfigurationBase configuration)
        {
            m_genericCodeGenerator = genericCodeGenerator;
            m_configuration = configuration;
            CheckConfiguration(configuration);
        }

        protected virtual char[] GetAllowedChars(CodeGeneratorConfigurationBase configuration)
        {
            return new AllowedCharactersBuilder().BuildFromConfiguration(configuration.AllowedCharacters);
        }

        protected void CheckConfiguration(CodeGeneratorConfigurationBase configuration)
        {
            var allowedChars = new AllowedCharactersBuilder().BuildFromConfiguration(configuration.AllowedCharacters);

            if (allowedChars == null || allowedChars.Length == 0)
            {
                throw new ConfigurationErrorsException("No allowed chars. Configuration resolves to empty list.");
            }

            if (configuration.CodeLength <= 0)
            {
                throw new ConfigurationErrorsException("Not allowed code length. Code length must be greater then zero.");
            }
        }

        public virtual string Generate()
        {
            if (m_allowedChars == null || m_allowedChars.Length == 0)
            {
                m_allowedChars = GetAllowedChars(m_configuration);
            }

            return m_genericCodeGenerator.GenerateCode(m_configuration.CodeLength, m_allowedChars);
        }
    }
}