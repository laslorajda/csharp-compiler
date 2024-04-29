namespace MyCompiler.Syntax;

public abstract class SyntaxNode
{
    public abstract TokenType Type { get; }
    public abstract IEnumerable<SyntaxNode> GetChildren();
}