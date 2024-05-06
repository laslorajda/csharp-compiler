using Compiler.CodeAnalysis.Binding;
using Compiler.CodeAnalysis.Syntax;

namespace Compiler.CodeAnalysis;

internal class Evaluator
{
    private readonly BoundStatement _root;
    private readonly Dictionary<VariableSymbol, object> _variables;
    private object _lastValue = default!;
    
    public Evaluator(BoundStatement root, Dictionary<VariableSymbol, object> variables)
    {
        _root = root;
        _variables = variables;
    }

    public object Evaluate()
    {
        EvaluateStatement(_root);
        return _lastValue;
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

    private void EvaluateStatement(BoundStatement statement)
    {
        switch (statement.Kind)
        {
            case BoundNodeKind.BlockStatement:
                EvaluateBlockStatement((BoundBlockStatement)statement);
                break;
            case BoundNodeKind.VariableDeclarationStatement:
                EvaludateVariableDeclarationStatement((BoundVariableDeclarationStatement)statement);
                break;
            case BoundNodeKind.ExpressionStatement:
                EvaluateExpressionStatement((BoundExpressionStatement)statement);
                break;
            default:
                throw new Exception($"Unexpected statement {statement.Kind}");
        }
    }

    private void EvaluateBlockStatement(BoundBlockStatement node)
    {
        foreach (var statement in node.Statements)
        {
            EvaluateStatement(statement);
        }
    }

    private void EvaludateVariableDeclarationStatement(BoundVariableDeclarationStatement node)
    {
        var value = EvaluateExpression(node.Initializer);
        _variables[node.Variable] = value;
        _lastValue = value;
    }

    private void EvaluateExpressionStatement(BoundExpressionStatement node) =>
        _lastValue = EvaluateExpression(node.Expression);

    private static object EvaluateLiteralExpression(BoundLiteralExpression node)
    {
        return node.Value;
    }

    private object EvaluateVariableExpression(BoundVariableExpression node)
    {
        return _variables[node.Variable];
    }

    private object EvaluateAssignmentExpression(BoundAssignmentExpression node)
    {
        var value = EvaluateExpression(node.Expression);
        _variables[node.Variable] = value;
        return value;
    }

    private object EvaluateUnaryExpression(BoundUnaryExpression node)
    {
        var operand = EvaluateExpression(node.Operand);
        return node.Operator?.Kind switch
        {
            BoundUnaryOperatorKind.Identity => operand,
            BoundUnaryOperatorKind.Negation => -(int) operand,
            BoundUnaryOperatorKind.LogicalNegation => !(bool) operand,
            _ => throw new Exception($"Unexpected unary operator {node.Operator}")
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