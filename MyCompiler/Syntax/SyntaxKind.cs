namespace MyCompiler.Syntax;

public enum SyntaxKind
{
    BadResultToken = 0,
    EndOfFileToken = 1,
    WhitespaceToken = 2,
    NumberToken = 3,
    PlusToken = 4,
    MinusToken = 5,
    StarToken = 6,
    SlashToken = 7,
    BinaryExpression = 8,
    NumberExpressionToken = 9,
    OpenParenthesisToken = 10,
    CloseParenthesisToken = 11,
    ParenthesizedExpressionToken = 12
}