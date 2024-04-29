namespace MyCompiler.Binding;

internal sealed class BoundBinaryExpression : BoundExpression
{
    internal readonly BoundBinaryOperator? Operator;
    internal readonly BoundExpression Left;
    internal readonly BoundExpression Right;
    internal override Type? Type => Operator?.ResultType;
    internal override BoundNodeKind Kind => BoundNodeKind.BinaryExpression;

    internal BoundBinaryExpression(BoundExpression left, BoundBinaryOperator? op, BoundExpression right)
    {
        Operator = op;
        Left = left;
        Right = right;
    }
}