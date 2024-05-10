namespace Compiler.CodeAnalysis.Binding;

internal sealed class BoundGotoStatement : BoundStatement
{
    public LabelSymbol Label { get; }
    internal override BoundNodeKind Kind => BoundNodeKind.GotoStatement;
    
    public BoundGotoStatement(LabelSymbol label)
    {
        Label = label;
    }
}