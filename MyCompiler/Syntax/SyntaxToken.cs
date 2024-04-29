namespace MyCompiler.Syntax;

public class SyntaxToken : SyntaxNode
{
    public override SyntaxKind Kind { get; }

    public SyntaxToken(SyntaxKind kind, object? value, string text)
    {
        Kind = kind;
        Value = value;
        Text = text;
    }
    
    public override IEnumerable<SyntaxNode> GetChildren() => Enumerable.Empty<SyntaxNode>();

    public object? Value { get; }
    public string Text { get; }
}