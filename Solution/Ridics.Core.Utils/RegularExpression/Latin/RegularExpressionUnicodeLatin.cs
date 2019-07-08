namespace Ridics.Core.Utils.RegularExpression.Latin
{
    public class RegularExpressionUnicodeLatin
    {
        //Uppercase letters
        public const string LettersUpper =
            RegularExpressionUnicodeBasicLatin.LettersUpper + RegularExpressionUnicodeLatinSupplement.LettersUpper +
            RegularExpressionUnicodeLatinExtendedA.LettersUpper;

        //Lowercase letters
        public const string LettersLower =
            RegularExpressionUnicodeBasicLatin.LettersLower + RegularExpressionUnicodeLatinSupplement.LettersLower +
            RegularExpressionUnicodeLatinExtendedA.LettersLower;

        public const string Numbers = RegularExpressionUnicodeBasicLatin.Numbers;
    }
}