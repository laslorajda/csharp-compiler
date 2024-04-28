namespace MyCompiler.Syntax;

public class BinaryExpressionSyntax : ExpressionSyntax
{
    private readonly ExpressionSyntax _left;
    private readonly SyntaxToken _operatorToken;
    private readonly ExpressionSyntax _right;

    public BinaryExpressionSyntax(ExpressionSyntax left, SyntaxToken operatorToken, ExpressionSyntax right)
    {
        _left = left;
        _operatorToken = operatorToken;
        _right = right;
    }

    public override TokenType Type => TokenType.BinaryExpression;

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return _left;
        yield return _operatorToken;
        yield return _right;
    }
}