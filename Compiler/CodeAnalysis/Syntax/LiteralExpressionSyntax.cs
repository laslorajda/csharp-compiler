namespace Compiler.CodeAnalysis.Syntax;

public class LiteralExpressionSyntax : ExpressionSyntax
{
    public object? Value { get; }
    public override SyntaxKind Kind => SyntaxKind.LiteralExpression;

    private readonly SyntaxToken _literalToken;

    public LiteralExpressionSyntax(SyntaxToken literalToken, object? value)
    {
        Value = value;
        _literalToken = literalToken;
    }

    public LiteralExpressionSyntax(SyntaxToken literalToken) : this(literalToken, literalToken.Value)
    {
    }

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return _literalToken;
    }
}