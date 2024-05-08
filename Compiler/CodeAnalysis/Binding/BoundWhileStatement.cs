namespace Compiler.CodeAnalysis.Binding;

internal class BoundWhileStatement : BoundStatement
{
    public BoundExpression Condition { get; }
    public BoundStatement Body { get; }
    internal override BoundNodeKind Kind => BoundNodeKind.WhileStatement;

    public BoundWhileStatement(BoundExpression condition, BoundStatement body)
    {
        Condition = condition;
        Body = body;
    }

}