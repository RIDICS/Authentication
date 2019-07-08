namespace Ridics.Core.Utils.RegularExpression.Latin
{
    public class RegularExpressionUnicodeBasicLatin
    {
        //Uppercase letters
        //[A-Z]
        public const string LettersUpper = @"\u0041-\u005a";

        //Lowercase letters
        //[a-z]
        public const string LettersLower = @"\u0061-\u007a";

        //Numbers
        //[0-9]
        public const string Numbers = @"\u0030-\u0039";
    }
}