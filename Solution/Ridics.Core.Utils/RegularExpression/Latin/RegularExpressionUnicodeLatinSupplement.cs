namespace Ridics.Core.Utils.RegularExpression.Latin
{
    public class RegularExpressionUnicodeLatinSupplement
    {
        //Uppercase letters
        //[A-Z]
        public const string LettersUpper = @"\u00C0-\u00D6\u00D8-\u00DF";

        //Lowercase letters
        //[a-z]
        public const string LettersLower = @"\u00E0-\u00F6\u00F8-\u00FF";
    }
}