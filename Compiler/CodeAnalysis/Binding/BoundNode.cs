using System.Reflection;

namespace Compiler.CodeAnalysis.Binding;

internal abstract class BoundNode
{
    internal abstract BoundNodeKind Kind { get; }

    // Should be source generated
    public IEnumerable<BoundNode> GetChildren()
    {
        var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            if (typeof(BoundNode).IsAssignableFrom(property.PropertyType))
            {
                var child = (BoundNode?)property.GetValue(this);
                if (child != null)
                {
                    yield return child;
                }
            }
            else if (typeof(IEnumerable<BoundNode>).IsAssignableFrom(property.PropertyType))
            {
                var children = (IEnumerable<BoundNode>?)property.GetValue(this);
                if (children == null)
                {
                    continue;
                }

                foreach (var child in children)
                {
                    yield return child;
                }
            }
        }
    }

    private IEnumerable<(string Name, object Value)> GetProperties()
    {
        var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var property in properties)
        {
            if (property.Name is nameof(Kind) or nameof(BoundBinaryExpression.Operator))
            {
                continue;
            }
            
            if(typeof(BoundNode).IsAssignableFrom(property.PropertyType) || typeof(IEnumerable<BoundNode>).IsAssignableFrom(property.PropertyType))
            {
                continue;
            }

            var value = property.GetValue(this) ?? null;
            if (value != null)
            {
                yield return (property.Name, value);
            }
        }
    }

    public void WriteTo(TextWriter writer) => PrettyPrint(writer, this);

    public override string ToString()
    {
        using var writer = new StringWriter();
        WriteTo(writer);
        return writer.ToString();
    }

    private static void PrettyPrint(TextWriter writer, BoundNode node, string indent = "", bool isLast = true)
    {
        var isToConsole = writer == Console.Out;
        var marker = isLast ? "└──" : "├──";

        writer.Write(indent);

        if (isToConsole)
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
        }

        writer.Write(marker);

        if (isToConsole)
        {
            Console.ForegroundColor = GetColor(node);
        }

        var text = GetText(node);
        writer.Write(text);

        if (isToConsole)
        {
            Console.ResetColor();
        }

        WriteProperties(writer, node, isToConsole);

        if (isToConsole)
        {
            Console.ResetColor();
        }

        writer.WriteLine();

        indent += isLast ? "   " : "│  ";

        var lastChild = node.GetChildren().LastOrDefault();

        foreach (var child in node.GetChildren())
            PrettyPrint(writer, child, indent, child == lastChild);
    }

    private static void WriteProperties(TextWriter writer, BoundNode node, bool isToConsole)
    { var isFirstProperty = true;
        foreach (var property in node.GetProperties())
        {
            if (isFirstProperty)
            {
                isFirstProperty = false;
            }
            else
            {
                if (isToConsole)
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                }
                writer.Write(", ");
            }

            writer.Write(" ");
            
            if (isToConsole)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
            }

            writer.Write(property.Name);

            if (isToConsole)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
            }
            writer.Write(" = ");
            
            if (isToConsole)
            {
                Console.ForegroundColor = ConsoleColor.DarkYellow;
            }
            writer.Write(property.Value);
        }
    }

    private static string GetText(BoundNode node) =>
        node switch
        {
            BoundBinaryExpression b => b.Operator?.Kind + "Expression",
            BoundUnaryExpression u => u.Operator?.Kind + "Expression",
            _ => node.Kind.ToString()
        };

    private static ConsoleColor GetColor(BoundNode node) =>
        node switch
        {
            BoundExpression => ConsoleColor.Blue,
            BoundStatement => ConsoleColor.Cyan,
            _ => ConsoleColor.Yellow
        };
}