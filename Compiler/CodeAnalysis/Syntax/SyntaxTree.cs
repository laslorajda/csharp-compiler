using Compiler.CodeAnalysis.Text;

namespace Compiler.CodeAnalysis.Syntax;

public class SyntaxTree
{
    public SyntaxTree(SourceText text, ExpressionSyntax root, DiagnosticBag diagnostics)
    {
        Text = text;
        Root = root;
        Diagnostics = diagnostics;
    }

    public SourceText Text { get; }
    public ExpressionSyntax Root { get; }

    public DiagnosticBag Diagnostics { get; }

    public static SyntaxTree Parse(string text)
    {
        var sourceText = SourceText.From(text);
        return Parse(sourceText);
    }

    public static SyntaxTree Parse(SourceText text)
    {
        var parser = new Parser(text);
        return parser.Parse();
    }

    public static IEnumerable<SyntaxToken> ParseTokens(string text)
    {
        var sourceText = SourceText.From(text);
        return ParseTokens(sourceText);
    }

    public static IEnumerable<SyntaxToken> ParseTokens(SourceText text)
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