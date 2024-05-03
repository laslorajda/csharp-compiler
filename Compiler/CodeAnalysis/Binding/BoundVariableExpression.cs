namespace Compiler.CodeAnalysis.Binding;

internal sealed class BoundVariableExpression : BoundExpression
{
    public BoundVariableExpression(string name, Type type)
    {
        Name = name;
        Type = type;
    }

    public string Name { get; }
    internal override Type Type { get; }
    internal override BoundNodeKind Kind => BoundNodeKind.VariableExpression;
}