namespace Compiler.CodeAnalysis.Syntax;

public class SyntaxTree
{
    public SyntaxTree(ExpressionSyntax root, DiagnosticBag diagnostics)
    {
        Root = root;
        Diagnostics = diagnostics;
    }

    public ExpressionSyntax Root { get; }

    //TODO write tests for this
    public DiagnosticBag Diagnostics { get; }
}