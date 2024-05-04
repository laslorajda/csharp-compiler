namespace Compiler.CodeAnalysis.Syntax;

public class ParenthesizedExpressionSyntax : ExpressionSyntax
{
    public SyntaxToken Left { get; }
    public ExpressionSyntax Expression { get; }
    public SyntaxToken Right { get; }

    public ParenthesizedExpressionSyntax(SyntaxToken left, ExpressionSyntax expression, SyntaxToken right)
    {
        Left = left;
        Expression = expression;
        Right = right;
    }

    public override SyntaxKind Kind => SyntaxKind.ParenthesizedExpression;
}