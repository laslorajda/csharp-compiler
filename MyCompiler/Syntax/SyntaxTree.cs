namespace MyCompiler.Syntax;

public class SyntaxTree
{
    public SyntaxTree(ExpressionSyntax root)
    {
        Root = root;
    }

    public ExpressionSyntax Root { get; }
}