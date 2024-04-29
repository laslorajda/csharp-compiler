namespace MyCompiler.Binding;

internal sealed class BoundLiteralExpression : BoundExpression
{
    public readonly object Value;
    internal override Type Type => Value.GetType();
    internal override BoundNodeKind Kind => BoundNodeKind.LiteralExpression;

    public BoundLiteralExpression(object value)
    {
        Value = value;
    }
}