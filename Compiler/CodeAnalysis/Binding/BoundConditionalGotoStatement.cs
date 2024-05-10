namespace Compiler.CodeAnalysis.Binding;

internal sealed class BoundConditionalGotoStatement : BoundStatement
{
    public LabelSymbol Label { get; }
    public BoundExpression Condition { get; }
    public bool JumpIfFalse { get; }
    internal override BoundNodeKind Kind => BoundNodeKind.ConditionalGotoStatement;

    public BoundConditionalGotoStatement(LabelSymbol label, BoundExpression condition, bool jumpIfFalse)
    {
        Label = label;
        Condition = condition;
        JumpIfFalse = jumpIfFalse;
    }
}