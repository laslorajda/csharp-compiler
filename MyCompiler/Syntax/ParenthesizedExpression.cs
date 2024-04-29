namespace MyCompiler.Syntax;

public class ParenthesizedExpression : ExpressionSyntax
{
    private readonly SyntaxToken _left;
    private readonly ExpressionSyntax _expression;
    private readonly SyntaxToken _right;

    public ParenthesizedExpression(SyntaxToken left, ExpressionSyntax expression, SyntaxToken right)
    {
        _left = left;
        _expression = expression;
        _right = right;
    }

    public override SyntaxKind Kind => SyntaxKind.ParenthesizedExpressionToken;

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return _left;
        yield return _expression;
        yield return _right;
    }
}