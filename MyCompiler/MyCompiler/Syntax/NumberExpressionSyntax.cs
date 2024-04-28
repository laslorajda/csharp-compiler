namespace MyCompiler.Syntax;

public class NumberExpressionSyntax : ExpressionSyntax
{
    public override TokenType Type => TokenType.NumberExpressionToken;

    private readonly SyntaxToken _token;

    public NumberExpressionSyntax(SyntaxToken token)
    {
        _token = token;
    }

    public override IEnumerable<SyntaxNode> GetChildren()
    {
        yield return _token;
    }
}