using System.Collections.Immutable;
using Compiler.CodeAnalysis.Syntax;

namespace Compiler.CodeAnalysis.Binding;

internal sealed class Binder
{
    
    private BoundScope _scope;
    public readonly DiagnosticBag Diagnostics = new();

    public Binder(BoundScope? parent)
    {
        _scope = new BoundScope(parent);
    }

    public static BoundGlobalScope BindGlobalScope(BoundGlobalScope? previous, CompilationUnitSyntax syntax)
    {
        var parentScope = CreateParentScopes(previous);
        var binder = new Binder(parentScope);
        var expression = binder.BindExpression(syntax.Expression);
        var variables = binder._scope.GetDeclaredVariables();
        var diagnostics = binder.Diagnostics.ToImmutableArray();

        if (previous != null)
        {
            diagnostics = diagnostics.InsertRange(0, previous.Diagnostics);
        }
        
        return new BoundGlobalScope(previous, diagnostics, variables, expression);
    }

    private static BoundScope? CreateParentScopes(BoundGlobalScope? previous)
    {
        var stack = new Stack<BoundGlobalScope>();
        while (previous != null)
        {
            stack.Push(previous);
            previous = previous.Previous;
        }

        BoundScope? current = null;

        while (stack.Count != 0)
        {
            previous = stack.Pop();
            var scope = new BoundScope(current);
            foreach (var variable in previous.Variables)
            {
                scope.TryDeclare(variable);
            }

            current = scope;
        }

        return current;
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
        if (boundOperatorKind != null)
        {
            return new BoundUnaryExpression(boundOperatorKind, boundOperand);
        }

        Diagnostics.ReportUndefinedUnaryOperator(syntax.OperatorToken.Span, syntax.OperatorToken.Text,
            boundOperandType);
        return boundOperand;
    }

    private BoundExpression BindBinaryExpression(BinaryExpressionSyntax syntax)
    {
        var boundLeft = BindExpression(syntax.Left);
        var boundRight = BindExpression(syntax.Right);
        var boundOperatorKind = BoundBinaryOperator.Bind(syntax.OperatorToken.Kind, boundLeft.Type, boundRight.Type);
        if (boundOperatorKind != null)
        {
            return new BoundBinaryExpression(boundLeft, boundOperatorKind, boundRight);
        }

        Diagnostics.ReportUndefinedBinaryOperator(syntax.OperatorToken.Span, syntax.OperatorToken.Text,
            boundLeft.Type, boundRight.Type);
        return boundLeft;

    }

    private BoundExpression BindNameExpression(NameExpressionSyntax syntax)
    {
        var name = syntax.IdentifierToken.Text;
        _scope.TryLookup(name, out var variable);
        
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

        if (!_scope.TryLookup(name, out var variable))
        {
            variable = new VariableSymbol(name, boundExpression.Type!);
            _scope.TryDeclare(variable);
        }

        if (variable!.Type != boundExpression.Type)
        {
            Diagnostics.ReportCannotConvert(syntax.Expression.Span, boundExpression.Type, variable.Type);
            return boundExpression;
        }        
        return new BoundAssignmentExpression(variable, boundExpression);
    }
}