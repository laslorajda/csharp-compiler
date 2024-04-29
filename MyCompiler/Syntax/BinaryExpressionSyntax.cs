namespace MyCompiler.Syntax;

public class BinaryExpressionSyntax : ExpressionSyntax
{
    public readonly ExpressionSyntax Left;
    public readonly SyntaxToken OperatorToken;
    public readonly ExpressionSyntax Right;

    public BinaryExpressionSyntax(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
    {
        Left = left;
        OperatorToken = operatorToken;
        Right = right;
    }

    public override TokenType Type => TokenType.BinaryExpression;

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return Left;
        yield return OperatorToken;
        yield return Right;
    }
}