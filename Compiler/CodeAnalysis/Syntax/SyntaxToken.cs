namespace Compiler.CodeAnalysis.Syntax;

public class SyntaxToken : SyntaxNode
{
    public override SyntaxKind Kind { get; }

    public SyntaxToken(SyntaxKind kind, object? value, string text, int position)
    {
        Kind = kind;
        Value = value;
        Text = text;
        Position = position;
    }
    
    public override IEnumerable<SyntaxNode> GetChildren() => Enumerable.Empty<SyntaxNode>();

    public object? Value { get; }
    public string Text { get; }
    public int Position { get; }
    public TextSpan Span => new TextSpan(Position, Text.Length);
}