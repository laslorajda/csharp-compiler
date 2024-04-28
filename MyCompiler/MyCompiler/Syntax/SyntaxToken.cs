namespace MyCompiler.Syntax;

public class SyntaxToken
{
    public SyntaxToken(TokenType type, object? value)
    {
        Type = type;
        Value = value;
    }

    public TokenType Type { get; }
    public object? Value { get; }
}