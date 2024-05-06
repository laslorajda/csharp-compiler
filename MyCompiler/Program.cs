
using System.Text;
using Compiler.CodeAnalysis;
using Compiler.CodeAnalysis.Syntax;
using Compiler.CodeAnalysis.Text;

var showTree = false;
var variables = new Dictionary<VariableSymbol, object>();
var textBuilder = new StringBuilder();

while (true)
{
    if (textBuilder.Length == 0)
    {
        Console.Write("> ");
    }
    else
    {
        Console.Write("| ");
    }

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

        if (input == "#cls")
        {
            Console.Clear();
            continue;
        }
    }

    textBuilder.AppendLine(input);
    var text = textBuilder.ToString();
    
    var syntaxTree = SyntaxTree.Parse(text);

    if (!isBlank && syntaxTree.Diagnostics.Any())
    {
            continue;
    }
    
    var compilation = new Compilation(syntaxTree);
    var result = compilation.Evaluate(variables);

    if (showTree)
    {
        Console.ForegroundColor = ConsoleColor.DarkGray;                
        syntaxTree.Root.WriteTo(Console.Out);
        Console.ResetColor();
    }

    if (!result.Diagnostics.Any())
    {
        Console.WriteLine(result.Value);
    }
    else
    {
        foreach (var diagnostic in result.Diagnostics)
        {
            var syntaxTreeText = syntaxTree.Text;
            var lineIndex = syntaxTreeText.GetLineIndex(diagnostic.Span.Start);
            var lineNumber = lineIndex + 1;
            var line = syntaxTreeText.Lines[lineIndex];
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