using Compiler.CodeAnalysis.Text;

namespace Compiler.CodeAnalysis.Syntax;

public class SyntaxTree
{
    private SyntaxTree(SourceText text)
    {
        var parser = new Parser(text);
        var root = parser.ParseCompliationUnit();
        Diagnostics = parser.Diagnostics;
        
        Text = text;
        Root = root;
    }

    public SourceText Text { get; }
    public CompilationUnitSyntax Root { get; }

    public DiagnosticBag Diagnostics { get; }

    public static SyntaxTree Parse(string text)
    {
        var sourceText = SourceText.From(text);
        return Parse(sourceText);
    }

    private static SyntaxTree Parse(SourceText text)
    {
        return new SyntaxTree(text);
    }

    public static IEnumerable<SyntaxToken> ParseTokens(string text)
    {
        var sourceText = SourceText.From(text);
        return ParseTokens(sourceText);
    }

    private static IEnumerable<SyntaxToken> ParseTokens(SourceText text)
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