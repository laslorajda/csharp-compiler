namespace MyCompiler.Syntax;

public class ParenthesizedExpressionSyntax : ExpressionSyntax
{
    private readonly SyntaxToken _left;
    private readonly ExpressionSyntax _expression;
    private readonly SyntaxToken _right;

    public ParenthesizedExpressionSyntax(SyntaxToken left, ExpressionSyntax expression, SyntaxToken right)
    {
        _left = left;
        _expression = expression;
        _right = right;
    }

    public override SyntaxKind Kind => SyntaxKind.ParenthesizedExpression;

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return _left;
        yield return _expression;
        yield return _right;
    }
}