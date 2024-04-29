namespace MyCompiler.Syntax;

public class SyntaxToken : SyntaxNode
{
    public override SyntaxKind Kind { get; }

    public SyntaxToken(SyntaxKind kind, object? value)
    {
        Kind = kind;
        Value = value;
    }
    
    public override IEnumerable<SyntaxNode> GetChildren() => Enumerable.Empty<SyntaxNode>();

    public object? Value { get; }
}