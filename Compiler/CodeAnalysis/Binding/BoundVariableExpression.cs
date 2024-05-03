using Compiler.CodeAnalysis.Syntax;

namespace Compiler.CodeAnalysis.Binding;

internal sealed class BoundVariableExpression : BoundExpression
{
    public VariableSyntax Variable { get; }
    internal override BoundNodeKind Kind => BoundNodeKind.VariableExpression;
    internal override Type? Type => Variable.Type;
    public BoundVariableExpression(VariableSyntax variable)
    {
        Variable = variable;
    }
    
}