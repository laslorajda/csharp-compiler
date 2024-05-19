using Compiler.CodeAnalysis;
using Compiler.CodeAnalysis.Syntax;
using Compiler.CodeAnalysis.Text;

namespace MyCompiler;

internal sealed class MyRepl : Repl
{
    private Compilation? _previous;
    private bool _showTree;
    private bool _showProgram;
    private readonly Dictionary<VariableSymbol, object> _variables = new();

    protected override void EvaluateMetaInput(string input)
    {
        switch (input)
        {
            case "#showTree":
                _showTree = !_showTree;
                Console.WriteLine(_showTree ? "Showing parse trees." : "Not showing parse trees");
                return;
            case "#showProgram":
                _showProgram = !_showProgram;
                Console.WriteLine(_showProgram ? "Showing bound tree." : "Not showing bound tree");
                return;
            case "#cls":
                Console.Clear();
                return;
            case "#reset":
                _previous = null;
                break;
            default:
                base.EvaluateMetaInput(input);
                break;
        }
    }

    protected override bool IsCompleteSubmission(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            return true;
        }

        var syntaxTree = SyntaxTree.Parse(text);
        return GetLastToken(syntaxTree.Root.Statement).IsMissing;
    }

    private static SyntaxToken GetLastToken(SyntaxNode node)
    {
        while (true)
        {
            if (node is SyntaxToken token)
            {
                return token;
            }

            node = node.GetChildren().Last();
        }
    }

    protected override void EvaluateSubmission(string text)
    {
        var syntaxTree = SyntaxTree.Parse(text);

        var compilation = _previous == null ? new Compilation(syntaxTree) : _previous.ContinueWith(syntaxTree);
        var result = compilation.Evaluate(_variables);

        if (_showTree)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            syntaxTree.Root.WriteTo(Console.Out);
            Console.ResetColor();
        }

        if (_showProgram)
        {
            compilation.EmiteTree(Console.Out);
        }

        if (!result.Diagnostics.Any())
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(result.Value);
            Console.ResetColor();
            _previous = compilation;
        }
        else
        {
            foreach (var diagnostic in result.Diagnostics)
            {
                var syntaxTreeText = syntaxTree.Text;
                var lineIndex = syntaxTreeText.GetLineIndex(diagnostic.Span.Start);
                var line = syntaxTreeText.Lines[lineIndex];
                var lineNumber = lineIndex + 1;
                var character = diagnostic.Span.Start - line.Start + 1;

                Console.WriteLine();

                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.Write($"({lineNumber}, {character}): ");
                Console.WriteLine(diagnostic);
                Console.ResetColor();

                var prefixSpan = TextSpan.FromBounds(line.Start, diagnostic.Span.Start);
                var suffixSpan = TextSpan.FromBounds(diagnostic.Span.End, line.End);

                var prefix = syntaxTreeText.ToString(prefixSpan);
                var error = syntaxTreeText.ToString(diagnostic.Span);
                var suffix = syntaxTreeText.ToString(suffixSpan);

                Console.Write("    ");
                Console.Write(prefix);

                Console.ForegroundColor = ConsoleColor.DarkRed;
                Console.Write(error);
                Console.ResetColor();

                Console.Write(suffix);

                Console.WriteLine();
            }

            Console.WriteLine();
        }
    }

    protected override void RenderLine(string line)
    {
        var tokens = SyntaxTree.ParseTokens(line);
        foreach (var token in tokens)
        {
            var isKeyword = token.Kind.ToString().EndsWith("Keyword");
            var isNumber = token.Kind == SyntaxKind.NumberToken;
            if (isKeyword)
            {
                Console.ForegroundColor = ConsoleColor.Blue;
            }
            else if (!isNumber)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
            }

            Console.Write(token.Text);
            Console.ResetColor();
        }
    }
}