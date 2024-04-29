namespace MyCompiler.Syntax;

public class BinaryExpression : ExpressionSyntax
{
    public readonly ExpressionSyntax Left;
    public readonly SyntaxToken OperatorToken;
    public readonly ExpressionSyntax Right;

    public BinaryExpression(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
    {
        Left = left;
        OperatorToken = operatorToken;
        Right = right;
    }

    public override SyntaxKind Kind => SyntaxKind.BinaryExpression;

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Left;
        yield return OperatorToken;
        yield return Right;
    }
}