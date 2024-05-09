namespace Compiler.CodeAnalysis.Binding;

internal sealed class BoundLiteralExpression : BoundExpression
{
    public object Value { get; }
    internal override Type Type => Value.GetType();
    internal override BoundNodeKind Kind => BoundNodeKind.LiteralExpression;

    public BoundLiteralExpression(object value)
    {
        Value = value;
    }
}