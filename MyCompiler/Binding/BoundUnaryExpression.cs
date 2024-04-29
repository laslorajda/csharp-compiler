namespace MyCompiler.Binding;

internal sealed class BoundUnaryExpression : BoundExpression
{
    internal readonly BoundUnaryOperatorKind OperatorKind;
    internal readonly BoundExpression Operand;
    internal override Type Type => Operand.Type;
    internal override BoundNodeKind Kind => BoundNodeKind.UnaryExpression;

    public BoundUnaryExpression(BoundUnaryOperatorKind operatorKind , BoundExpression operand)
    {
        OperatorKind = operatorKind;
        Operand = operand;
    }
}