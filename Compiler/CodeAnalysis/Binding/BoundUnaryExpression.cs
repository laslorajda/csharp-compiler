namespace Compiler.CodeAnalysis.Binding;

internal sealed class BoundUnaryExpression : BoundExpression
{
    internal readonly BoundUnaryOperator? Operator;
    internal readonly BoundExpression Operand;
    internal override Type? Type => Operator?.ResultType;
    internal override BoundNodeKind Kind => BoundNodeKind.UnaryExpression;

    public BoundUnaryExpression(BoundUnaryOperator? op , BoundExpression operand)
    {
        Operator = op;
        Operand = operand;
    }
}