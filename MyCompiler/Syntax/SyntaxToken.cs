namespace MyCompiler.Syntax;

public class SyntaxToken : SyntaxNode
{
    public override TokenType Type { get; }

    public SyntaxToken(TokenType type, object? value)
    {
        Type = type;
        Value = value;
    }
    
    public override IEnumerable<SyntaxNode> GetChildren() => Enumerable.Empty<SyntaxNode>();

    public object? Value { get; }
}