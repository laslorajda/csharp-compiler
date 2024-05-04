namespace Compiler.CodeAnalysis.Syntax;

public class SyntaxTree
{
    public SyntaxTree(ExpressionSyntax root, DiagnosticBag diagnostics)
    {
        Root = root;
        Diagnostics = diagnostics;
    }

    public ExpressionSyntax Root { get; }

    public DiagnosticBag Diagnostics { get; }

    public static SyntaxTree Parse(string text)
    {
        var parser = new Parser(text);
        return parser.Parse();
    }

    public static IEnumerable<SyntaxToken> ParseTokens(string text)
    {
        var lexer = new Lexer(text);
        while (true)
        {
            var token = lexer.Lex();
            if(token.Kind == SyntaxKind.EndOfFileToken)
                break;

            yield return token;
        }
    }
}