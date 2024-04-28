namespace MyCompiler.Syntax;

public class NumberExpressionSyntax : ExpressionSyntax
{
    public override TokenType Type => TokenType.NumberExpressionToken;

    public readonly SyntaxToken NumberToken;

    public NumberExpressionSyntax(SyntaxToken numberToken)
    {
        NumberToken = numberToken;
    }

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return NumberToken;
    }
}