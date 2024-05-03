namespace Compiler.CodeAnalysis.Syntax;

public class SyntaxTree
{
    public SyntaxTree(ExpressionSyntax root, List<string> diagnostics)
    {
        Root = root;
        Diagnostics = diagnostics;
    }

    public ExpressionSyntax Root { get; }

    public IReadOnlyList<string> Diagnostics { get; }
}