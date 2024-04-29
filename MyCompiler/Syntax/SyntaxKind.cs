namespace MyCompiler.Syntax;

public enum SyntaxKind
{
    // Tokens
    BadResultToken,
    EndOfFileToken,
    WhitespaceToken,
    NumberToken,
    PlusToken,
    MinusToken,
    StarToken,
    SlashToken,
    IdentifierToken,
    BangToken,
    AmpersandAmpersandToken,
    PipePipeToken,

    // Expressions
    BinaryExpression,
    LiteralExpression,
    OpenParenthesis,
    CloseParenthesis,
    ParenthesizedExpression,
    UnaryExpression,

    // Keywords
    FalseKeyword,
    TrueKeyword,
}