using Compiler.CodeAnalysis.Syntax;

namespace Compiler.CodeAnalysis.Binding;

internal sealed class Binder
{
    private readonly Dictionary<VariableSyntax, object> _variables;
    public readonly DiagnosticBag Diagnostics = new();

    public Binder(Dictionary<VariableSyntax, object> variables)
    {
        _variables = variables;
    }

    internal BoundExpression BindExpression(ExpressionSyntax syntax)
    {
        return syntax.Kind switch
        {
            SyntaxKind.LiteralExpression => BindLiteralExpression((LiteralExpressionSyntax) syntax),
            SyntaxKind.UnaryExpression => BindUnaryExpression((UnaryExpressionSyntax) syntax),
            SyntaxKind.BinaryExpression => BindBinaryExpression((BinaryExpressionSyntax) syntax),
            SyntaxKind.ParenthesizedExpression => BindExpression(((ParenthesizedExpressionSyntax)syntax).Expression),
            SyntaxKind.NameExpression => BindNameExpression((NameExpressionSyntax) syntax),
            SyntaxKind.AssignmentExpression => BindAssignmentExpression((AssignmentExpressionSyntax) syntax),
            _ => throw new Exception($"Unexpected syntax {syntax.Kind}")
        };
    }

    private static BoundLiteralExpression BindLiteralExpression(LiteralExpressionSyntax syntax)
    {
        var value = syntax.Value ?? 0;
        return new BoundLiteralExpression(value);
    }
    
    private BoundExpression BindUnaryExpression(UnaryExpressionSyntax syntax)
    {
        var boundOperand = BindExpression(syntax.Operand);
        var boundOperandType = boundOperand.Type!;
        var boundOperatorKind = BoundUnaryOperator.Bind(syntax.OperatorToken.Kind, boundOperandType);
        if (boundOperatorKind == null)
        {
            Diagnostics.ReportUndefinedUnaryOperator(syntax.OperatorToken.Span, syntax.OperatorToken.Text,
                boundOperandType);
            return boundOperand;
        }
        return new BoundUnaryExpression(boundOperatorKind, boundOperand);
    }

    private BoundExpression BindBinaryExpression(BinaryExpressionSyntax syntax)
    {
        var boundLeft = BindExpression(syntax.Left);
        var boundRight = BindExpression(syntax.Right);
        var boundOperatorKind = BoundBinaryOperator.Bind(syntax.OperatorToken.Kind, boundLeft.Type, boundRight.Type);
        if (boundOperatorKind == null)
        {
            Diagnostics.ReportUndefinedBinaryOperator(syntax.OperatorToken.Span, syntax.OperatorToken.Text,
                boundLeft.Type, boundRight.Type);
            return boundLeft;
        }

        return new BoundBinaryExpression(boundLeft, boundOperatorKind, boundRight);
    }

    private BoundExpression BindNameExpression(NameExpressionSyntax syntax)
    {
        var name = syntax.IdentifierToken.Text;
        var variable = _variables.Keys.SingleOrDefault(x => x.Name == name);
        
        if (variable != null)
        {
            return new BoundVariableExpression(variable);
        }

        Diagnostics.ReportUndefinedName(syntax.IdentifierToken.Span, name);
        return new BoundLiteralExpression(0);

    }

    private BoundExpression BindAssignmentExpression(AssignmentExpressionSyntax syntax)
    {
        var name = syntax.IdentifierToken.Text;
        var boundExpression = BindExpression(syntax.Expression);
        
        var existingVariable = _variables.Keys.SingleOrDefault(x => x.Name == name);
        if (existingVariable != null)
        {
            _variables.Remove(existingVariable);
        }
        var variable = new VariableSyntax(name, boundExpression.Type!);
        _variables.Add(variable, null!);
        
        return new BoundAssignmentExpression(variable, boundExpression);
    }
}