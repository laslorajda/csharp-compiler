using Compiler.CodeAnalysis.Syntax;

namespace Compiler.CodeAnalysis.Binding;

internal class BoundAssignmentExpression : BoundExpression
{
    public VariableSyntax Variable { get; }
    public BoundExpression Expression { get; }
    internal override BoundNodeKind Kind => BoundNodeKind.AssignmentExpression;
    internal override Type? Type => Variable.Type;
    
    public BoundAssignmentExpression(VariableSyntax variable, BoundExpression expression)
    {
        Variable = variable;
        Expression = expression;
    }
}