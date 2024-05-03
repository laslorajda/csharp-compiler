namespace Compiler.CodeAnalysis.Syntax;

public sealed class VariableSyntax
{
    public string Name { get; }
    public Type Type { get; }

    public VariableSyntax(string name, Type type)
    {
        Name = name;
        Type = type;
    }
}