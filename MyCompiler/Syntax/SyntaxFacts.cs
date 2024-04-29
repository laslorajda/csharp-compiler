namespace MyCompiler.Syntax;

internal static class SyntaxFacts
{
    internal static int GetBinaryOperatorPrecedence(this SyntaxKind kind) =>
        kind switch
        {
            SyntaxKind.StarToken or SyntaxKind.SlashToken => 2,
            SyntaxKind.PlusToken or SyntaxKind.MinusToken  => 1,
            _ => 0
        };
    
    internal static int GetUnaryOperatorPrecedence(this SyntaxKind kind) =>
        kind switch
        {
            SyntaxKind.PlusToken or SyntaxKind.MinusToken => 3,
            _ => 0
        };
}