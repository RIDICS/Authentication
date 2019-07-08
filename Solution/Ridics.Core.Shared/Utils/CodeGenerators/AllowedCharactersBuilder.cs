namespace Ridics.Core.Shared.Utils.CodeGenerators
{
    public class AllowedCharactersBuilder
    {
        private readonly string m_numericSecure = @"23456789";
        private readonly string m_numericFull = @"0123456789";
        private readonly string m_upperAlphabetSecure = @"ABCDEFGHJKLMNPRSTUVWXYZ";
        private readonly string m_upperAlphabetFull = @"ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        private readonly string m_lowerAlphabetSecure = @"abcdefghijkmnopqrstuvwxyz";
        private readonly string m_lowerAlphabetFull = @"abcdefghijklmnopqrstuvwxyz";
        private readonly string m_specialChars = @"[]?/<~#`!@$%^&*()+=|:"";',>{}";

        private bool m_allowNumeric;
        private bool m_allowUpperAlphabet;
        private bool m_allowLowerAlphabet;
        private bool m_allowSpecialChars;

        public AllowedCharactersBuilder AddNumeric()
        {
            m_allowNumeric = true;
            return this;
        }

        public AllowedCharactersBuilder AddUpperAlphabet()
        {
            m_allowUpperAlphabet = true;
            return this;
        }

        public AllowedCharactersBuilder AddLowerAlphabet()
        {
            m_allowLowerAlphabet = true;
            return this;
        }

        public AllowedCharactersBuilder AddSpecialChars()
        {
            m_allowSpecialChars = true;
            return this;
        }

        public char[] Build()
        {
            var allowedChars = string.Empty;

            if (m_allowNumeric)
            {
                allowedChars += m_allowUpperAlphabet || m_allowLowerAlphabet
                    ? m_numericSecure
                    : m_numericFull;
            }

            if (m_allowUpperAlphabet)
            {
                allowedChars += m_allowNumeric || m_allowLowerAlphabet
                    ? m_upperAlphabetSecure
                    : m_upperAlphabetFull;
            }

            if (m_allowLowerAlphabet)
            {
                allowedChars += m_allowNumeric || m_allowUpperAlphabet
                    ? m_lowerAlphabetSecure
                    : m_lowerAlphabetFull;
            }

            if (m_allowSpecialChars)
            {
                allowedChars += m_specialChars;
            }

            return allowedChars.ToCharArray();
        }
    }
}