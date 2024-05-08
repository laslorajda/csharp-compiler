namespace Compiler.CodeAnalysis.Syntax;

public sealed class ForStatementSyntax : StatementSyntax
{
    public SyntaxToken ForKeyword { get; }
    public SyntaxToken Identifier { get; }
    public SyntaxToken EqualsToken { get; }
    public ExpressionSyntax LowerBound { get; }
    public SyntaxToken ToKeyword { get; }
    public ExpressionSyntax UpperBound { get; }
    public StatementSyntax Body { get; }
    public override SyntaxKind Kind => SyntaxKind.ForStatement;

    public ForStatementSyntax(SyntaxToken forKeyword, SyntaxToken identifier, SyntaxToken equalsToken,
        ExpressionSyntax lowerBound, SyntaxToken toKeyword, ExpressionSyntax upperBound, StatementSyntax body)
    {
        ForKeyword = forKeyword;
        Identifier = identifier;
        EqualsToken = equalsToken;
        LowerBound = lowerBound;
        ToKeyword = toKeyword;
        UpperBound = upperBound;
        Body = body;
    }
}