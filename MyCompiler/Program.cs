using System.Text;
using Compiler.CodeAnalysis;
using Compiler.CodeAnalysis.Syntax;
using Compiler.CodeAnalysis.Text;

var showTree = false;
var showProgram = false;
var variables = new Dictionary<VariableSymbol, object>();
var textBuilder = new StringBuilder();
Compilation? previous = null;

while (true)
{
    Console.ForegroundColor = ConsoleColor.Green;
    
    if (textBuilder.Length == 0)
        Console.Write("» ");
    else
        Console.Write("· ");
    
    Console.ResetColor();

    var input = Console.ReadLine();
    var isBlank = string.IsNullOrWhiteSpace(input);

    if (textBuilder.Length == 0)
    {
        if (isBlank)
        {
            break;
        }

        if (input == "#showTree")
        {
            showTree = !showTree;
            Console.WriteLine(showTree ? "Showing parse trees." : "Not showing parse trees");
            continue;
        }

        if (input == "#showProgram")
        {
            showProgram = !showProgram;
            Console.WriteLine(showTree ? "Showing bound tree." : "Not showing bound tree");
            continue;
        }

        if (input == "#cls")
        {
            Console.Clear();
            continue;
        }

        if (input == "#reset")
        {
            previous = null;
            continue;
        }
    }

    textBuilder.AppendLine(input);
    var text = textBuilder.ToString();

    var syntaxTree = SyntaxTree.Parse(text);

    if (!isBlank && syntaxTree.Diagnostics.Any())
        continue;

    var compilation = previous == null ? new Compilation(syntaxTree) : previous.ContinueWith(syntaxTree);
    var result = compilation.Evaluate(variables);

    if (showTree)
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;
        syntaxTree.Root.WriteTo(Console.Out);
        Console.ResetColor();
    }

    if (showProgram)
    {
        compilation.EmiteTree(Console.Out);
    }

    if (!result.Diagnostics.Any())
    {
        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine(result.Value);
        Console.ResetColor();
        previous = compilation;
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

    textBuilder.Clear();
}