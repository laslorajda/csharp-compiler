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
    OpenParenthesisToken,
    CloseParenthesisToken,
    OpenBraceToken,
    CloseBraceToken,

    // Expressions
    BinaryExpression,
    LiteralExpression,
    ParenthesizedExpression,
    UnaryExpression,
    NameExpression,
    AssignmentExpression,
    
    //Statements
    BlockStatement,
    ExpressionStatement,

    // Keywords
    FalseKeyword,
    TrueKeyword,
    
    // Nodes
    CompliationUnit
}