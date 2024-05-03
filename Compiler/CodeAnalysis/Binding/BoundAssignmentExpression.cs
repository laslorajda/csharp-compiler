namespace Compiler.CodeAnalysis.Binding;

internal class BoundAssignmentExpression : BoundExpression
{
    public string Name { get; }
    public BoundExpression Expression { get; }
    internal override BoundNodeKind Kind => BoundNodeKind.AssignmentExpression;
    internal override Type? Type => Expression.Type;
    
    public BoundAssignmentExpression(string name, BoundExpression expression)
    {
        Name = name;
        Expression = expression;
    }
}