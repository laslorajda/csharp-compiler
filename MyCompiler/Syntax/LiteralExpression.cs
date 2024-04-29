namespace MyCompiler.Syntax;

public class LiteralExpression : ExpressionSyntax
{
    public override SyntaxKind Kind => SyntaxKind.NumberExpressionToken;

    public readonly SyntaxToken LiteralToken;

    public LiteralExpression(SyntaxToken literalToken)
    {
        LiteralToken = literalToken;
    }

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return LiteralToken;
    }
}