namespace MyCompiler.Syntax;

public class ParenthesizedExpressionSyntax : ExpressionSyntax
{
    public readonly SyntaxToken Left;
    public readonly ExpressionSyntax Expression;
    public readonly SyntaxToken Right;

    public ParenthesizedExpressionSyntax(SyntaxToken left, ExpressionSyntax expression, SyntaxToken right)
    {
        Left = left;
        Expression = expression;
        Right = right;
    }

    public override SyntaxKind Kind => SyntaxKind.ParenthesizedExpression;

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Left;
        yield return Expression;
        yield return Right;
    }
}