namespace Compiler.CodeAnalysis.Syntax;

public class LiteralExpressionSyntax : ExpressionSyntax
{
    public object? Value { get; }
    public override SyntaxKind Kind => SyntaxKind.LiteralExpression;

    public SyntaxToken LiteralToken { get; }

    public LiteralExpressionSyntax(SyntaxToken literalToken, object? value)
    {
        Value = value;
        LiteralToken = literalToken;
    }

    public LiteralExpressionSyntax(SyntaxToken literalToken) : this(literalToken, literalToken.Value)
    {
    }
}