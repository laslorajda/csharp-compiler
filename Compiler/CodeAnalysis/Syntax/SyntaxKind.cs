namespace Compiler.CodeAnalysis.Syntax;

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
    EqualsToken,
    EqualsEqualsToken,
    BangEqualsToken,

    // Expressions
    BinaryExpression,
    LiteralExpression,
    OpenParenthesis,
    CloseParenthesis,
    ParenthesizedExpression,
    UnaryExpression,
    NameExpression,
    AssignmentExpression,

    // Keywords
    FalseKeyword,
    TrueKeyword,
    
    // Nodes
    CompliationUnit
}