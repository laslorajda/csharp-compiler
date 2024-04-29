namespace MyCompiler.Binding;

internal abstract class BoundExpression : BoundNode
{
    internal abstract Type? Type { get; }
}