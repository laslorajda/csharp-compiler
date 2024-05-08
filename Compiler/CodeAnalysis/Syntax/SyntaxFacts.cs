namespace Compiler.CodeAnalysis.Syntax;

public static class SyntaxFacts
{
    internal static int GetBinaryOperatorPrecedence(this SyntaxKind kind) =>
        kind switch
        {
            SyntaxKind.StarToken or SyntaxKind.SlashToken => 5,
            SyntaxKind.PlusToken or SyntaxKind.MinusToken => 4,
            SyntaxKind.EqualsEqualsToken or SyntaxKind.BangEqualsToken or SyntaxKind.LessToken
                or SyntaxKind.LessOrEqualsToken or SyntaxKind.GreaterToken or SyntaxKind.GreaterOrEqualsToken => 3,
            SyntaxKind.AmpersandAmpersandToken => 2,
            SyntaxKind.PipePipeToken => 1,
            _ => 0
        };
    
    internal static int GetUnaryOperatorPrecedence(this SyntaxKind kind) =>
        kind switch
        {
            SyntaxKind.PlusToken or SyntaxKind.MinusToken or SyntaxKind.BangToken => 6,
            _ => 0
        };

    public static SyntaxKind GetKeywordKind(string text) =>
        text switch
        {
            "true" => SyntaxKind.TrueKeyword,
            "false" => SyntaxKind.FalseKeyword,
            "let" => SyntaxKind.LetKeyword,
            "var" => SyntaxKind.VarKeyword,
            "if" => SyntaxKind.IfKeyword,
            "else" => SyntaxKind.ElseKeyword,
            "while" => SyntaxKind.WhileKeyword,
            _ => SyntaxKind.IdentifierToken
        };

    public static string GetText(SyntaxKind kind) =>
        kind switch
        {
            SyntaxKind.PlusToken => "+",
            SyntaxKind.MinusToken => "-",
            SyntaxKind.StarToken => "*",
            SyntaxKind.SlashToken => "/",
            SyntaxKind.BangToken => "!",
            SyntaxKind.EqualsToken => "=",
            SyntaxKind.LessToken => "<",
            SyntaxKind.LessOrEqualsToken => "<=",
            SyntaxKind.GreaterToken => ">",
            SyntaxKind.GreaterOrEqualsToken => ">=",
            SyntaxKind.AmpersandAmpersandToken => "&&",
            SyntaxKind.PipePipeToken => "||",
            SyntaxKind.EqualsEqualsToken => "==",
            SyntaxKind.BangEqualsToken => "!=",
            SyntaxKind.OpenParenthesisToken => "(",
            SyntaxKind.CloseParenthesisToken => ")",
            SyntaxKind.OpenBraceToken => "{",
            SyntaxKind.CloseBraceToken => "}",
            SyntaxKind.FalseKeyword => "false",
            SyntaxKind.TrueKeyword => "true",
            SyntaxKind.LetKeyword => "let",
            SyntaxKind.VarKeyword => "var",
            SyntaxKind.IfKeyword => "if",
            SyntaxKind.ElseKeyword => "else",
            SyntaxKind.WhileKeyword => "while",
            _ => string.Empty
        };
    
    public static IEnumerable<SyntaxKind> GetUnaryOperatorKinds() =>
        Enum.GetValues<SyntaxKind>().Where(kind => GetUnaryOperatorPrecedence(kind) > 0);
    
    public static IEnumerable<SyntaxKind> GetBinaryOperatorKinds() =>
        Enum.GetValues<SyntaxKind>().Where(kind => GetBinaryOperatorPrecedence(kind) > 0);
}