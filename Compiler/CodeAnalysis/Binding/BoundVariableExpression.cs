namespace Compiler.CodeAnalysis.Binding;

internal sealed class BoundVariableExpression : BoundExpression
{
    public VariableSymbol Variable { get; }
    internal override BoundNodeKind Kind => BoundNodeKind.VariableExpression;
    internal override Type? Type => Variable.Type;
    public BoundVariableExpression(VariableSymbol variable)
    {
        Variable = variable;
    }
    
}