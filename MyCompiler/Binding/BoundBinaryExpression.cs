namespace MyCompiler.Binding;

internal sealed class BoundBinaryExpression : BoundExpression
{
    internal readonly BoundBinaryOperatorKind OperatorKind;
    internal readonly BoundExpression Left;
    internal readonly BoundExpression Right;
    internal override Type Type => Left.Type;
    internal override BoundNodeKind Kind => BoundNodeKind.BinaryExpression;

    internal BoundBinaryExpression(BoundExpression left, BoundBinaryOperatorKind operatorKind, BoundExpression right)
    {
        OperatorKind = operatorKind;
        Left = left;
        Right = right;
    }
}