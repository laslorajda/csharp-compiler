namespace Compiler.CodeAnalysis.Binding;

internal sealed class BoundUnaryExpression : BoundExpression
{
    public BoundUnaryOperator? Operator { get; }
    public BoundExpression Operand { get; }
    internal override Type? Type => Operator?.ResultType;
    internal override BoundNodeKind Kind => BoundNodeKind.UnaryExpression;

    public BoundUnaryExpression(BoundUnaryOperator? op , BoundExpression operand)
    {
        Operator = op;
        Operand = operand;
    }
}