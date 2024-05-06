namespace Compiler.CodeAnalysis;

public sealed class VariableSymbol
{
    public string Name { get; }
    public bool IsReadOnly { get; }
    public Type Type { get; }

    public VariableSymbol(string name, bool isReadOnly, Type type)
    {
        Name = name;
        IsReadOnly = isReadOnly;
        Type = type;
    }
}