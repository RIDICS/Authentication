namespace Ridics.Core.Shared.Utils.CodeGenerators
{
    public interface IGenericCodeGenerator
    {
        string GenerateCode(int codeLength, char[] allowedChars);
    }
}