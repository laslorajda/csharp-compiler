namespace Compiler.CodeAnalysis.Binding;

internal class BoundForStatement : BoundStatement
{
    public VariableSymbol Variable { get; }
    public BoundExpression LowerBound { get; }
    public BoundExpression UpperBound { get; }
    public BoundStatement Body { get; }
    internal override BoundNodeKind Kind => BoundNodeKind.ForStatement;

    public BoundForStatement(VariableSymbol variable, BoundExpression lowerBound, BoundExpression upperBound, BoundStatement body)
    {
        Variable = variable;
        LowerBound = lowerBound;
        UpperBound = upperBound;
        Body = body;
    }
}