namespace Compiler.CodeAnalysis.Binding;

internal sealed class BoundLabelStatement : BoundStatement
{
    public LabelSymbol Label { get; }
    internal override BoundNodeKind Kind => BoundNodeKind.LabelStatement;

    public BoundLabelStatement(LabelSymbol label)
    {
        Label = label;
    }
}