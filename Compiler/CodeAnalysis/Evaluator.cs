using Compiler.CodeAnalysis.Binding;
using Compiler.CodeAnalysis.Syntax;

namespace Compiler.CodeAnalysis;

internal class Evaluator
{
    private readonly BoundExpression _root;
    private readonly Dictionary<VariableSymbol, object> _variables;

    public Evaluator(BoundExpression root, Dictionary<VariableSymbol, object> variables)
    {
        _root = root;
        _variables = variables;
    }

    public object Evaluate()
    {
        return EvaluateExpression(_root);
    }

    private object EvaluateExpression(BoundNode node)
    {
        while (true)
        {
            return node switch
            {
                BoundLiteralExpression expression => EvaluateLiteralExpression(expression),
                BoundVariableExpression expression => EvaluateVariableExpression(expression),
                BoundAssignmentExpression expression => EvaluateAssignmentExpression(expression),
                BoundUnaryExpression expression => EvaluateUnaryExpression(expression),
                BoundBinaryExpression expression => EvaluateBinaryExpression(expression),
                _ => throw new Exception($"Unexpected node {node.Kind}")
            };
        }
    }

    private static object EvaluateLiteralExpression(BoundLiteralExpression n)
    {
        return n.Value;
    }

    private object EvaluateVariableExpression(BoundVariableExpression v)
    {
        return _variables[v.Variable];
    }

    private object EvaluateAssignmentExpression(BoundAssignmentExpression a)
    {
        var value = EvaluateExpression(a.Expression);
        _variables[a.Variable] = value;
        return value;
    }

    private object EvaluateUnaryExpression(BoundUnaryExpression u)
    {
        var operand = EvaluateExpression(u.Operand);
        return u.Operator?.Kind switch
        {
            BoundUnaryOperatorKind.Identity => operand,
            BoundUnaryOperatorKind.Negation => -(int) operand,
            BoundUnaryOperatorKind.LogicalNegation => !(bool) operand,
            _ => throw new Exception($"Unexpected unary operator {u.Operator}")
        };
    }

    private object EvaluateBinaryExpression(BoundBinaryExpression b)
    {
        var left = EvaluateExpression(b.Left);
        var right = EvaluateExpression(b.Right);

        return b.Operator?.Kind switch
        {
            BoundBinaryOperatorKind.Addition => (int) left + (int) right,
            BoundBinaryOperatorKind.Subtraction => (int) left - (int) right,
            BoundBinaryOperatorKind.Multiplication => (int) left * (int) right,
            BoundBinaryOperatorKind.Division => (int) left / (int) right,
            BoundBinaryOperatorKind.LogicalAnd => (bool) left && (bool) right,
            BoundBinaryOperatorKind.LogicalOr => (bool) left || (bool) right,
            BoundBinaryOperatorKind.Equals => Equals(left, right),
            BoundBinaryOperatorKind.NotEquals => !Equals(left, right),
            _ => throw new Exception($"Unexpected binary operator {b.Operator}")
        };
    }
}