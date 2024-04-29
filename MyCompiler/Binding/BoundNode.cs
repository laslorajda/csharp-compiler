namespace MyCompiler.Binding;

internal abstract class BoundNode
{
    internal abstract BoundNodeKind Kind { get; }
}