namespace Compiler.CodeAnalysis.Binding;

internal class BoundAssignmentExpression : BoundExpression
{
    public VariableSymbol Variable { get; }
    public BoundExpression Expression { get; }
    internal override BoundNodeKind Kind => BoundNodeKind.AssignmentExpression;
    internal override Type? Type => Variable.Type;
    
    public BoundAssignmentExpression(VariableSymbol variable, BoundExpression expression)
    {
        Variable = variable;
        Expression = expression;
    }
}