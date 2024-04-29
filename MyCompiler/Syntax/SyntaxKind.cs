namespace MyCompiler.Syntax;

public enum SyntaxKind
{
    // Tokens
    BadResultToken = 0,
    EndOfFileToken = 1,
    WhitespaceToken = 2,
    NumberToken = 3,
    PlusToken = 4,
    MinusToken = 5,
    StarToken = 6,
    SlashToken = 7,
    IdentifierToken = 8, 

    
    // Expressions
    BinaryExpression = 9,
    LiteralExpression = 10,
    OpenParenthesis = 11,
    CloseParenthesis = 12,
    ParenthesizedExpression = 13,
    UnaryExpression = 14,
    
    // Keywords
    FalseKeyword = 15,
    TrueKeyword = 16
}