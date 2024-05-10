namespace Compiler.CodeAnalysis.Binding;

internal sealed class BoundBinaryExpression : BoundExpression
{
    public BoundBinaryOperator? Operator { get; }
    public BoundExpression Left { get; }
    public BoundExpression Right { get; }
    internal override Type? Type => Operator?.ResultType;
    internal override BoundNodeKind Kind => BoundNodeKind.BinaryExpression;

    internal BoundBinaryExpression(BoundExpression left, BoundBinaryOperator? op, BoundExpression right)
    {
        Operator = op;
        Left = left;
        Right = right;
    }
}