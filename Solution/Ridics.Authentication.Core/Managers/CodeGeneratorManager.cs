using System.Collections.Generic;
using System.Linq;
using Ridics.Authentication.Core.Models.Enum;
using Ridics.Authentication.Core.Utils.CodeGenerator.ContactConfirm;
using Ridics.Authentication.Core.Utils.CodeGenerator.User;

namespace Ridics.Authentication.Core.Managers
{
    public class CodeGeneratorManager
    {
        private readonly Dictionary<ContactTypeEnumModel, IContactConfirmCodeGenerator> m_confirmCodeGenerators;
        private readonly VerificationCodeGenerator m_verificationCodeGenerator;
        private readonly UsernameGenerator m_usernameGenerator;
        private readonly PasswordGenerator m_passwordGenerator;

        public CodeGeneratorManager(
            IList<IContactConfirmCodeGenerator> confirmCodeGenerators, VerificationCodeGenerator verificationCodeGenerator,
            UsernameGenerator usernameGenerator, PasswordGenerator passwordGenerator
        )
        {
            m_confirmCodeGenerators = confirmCodeGenerators.ToDictionary(x => x.SupportedContactType);
            m_verificationCodeGenerator = verificationCodeGenerator;
            m_usernameGenerator = usernameGenerator;
            m_passwordGenerator = passwordGenerator;
        }

        public string GenerateContactConfirmCode(ContactTypeEnumModel contactType)
        {
            return m_confirmCodeGenerators[contactType].Generate();
        }

        public string GenerateVerificationCode()
        {
            return m_verificationCodeGenerator.Generate();
        }

        public string GenerateUsername()
        {
            return m_usernameGenerator.Generate();
        }

        public string GeneratePassword()
        {
            return m_passwordGenerator.Generate();
        }
    }
}
