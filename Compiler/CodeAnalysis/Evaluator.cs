using Compiler.CodeAnalysis.Binding;

namespace Compiler.CodeAnalysis;

internal class Evaluator
{
    private readonly BoundBlockStatement _root;
    private readonly Dictionary<VariableSymbol, object> _variables;
    private object _lastValue = default!;
    
    public Evaluator(BoundBlockStatement root, Dictionary<VariableSymbol, object> variables)
    {
        _root = root;
        _variables = variables;
    }

    public object Evaluate()
    {
        var labelToIndex = new Dictionary<LabelSymbol, int>();

        for (var i = 0; i < _root.Statements.Length; i++)
        {
            var statement = _root.Statements[i];
            if (statement is BoundLabelStatement l)
            {
                labelToIndex.Add(l.Label, i + 1);
            }
        }

        var index = 0;
        while (index < _root.Statements.Length)
        {
            var statement = _root.Statements[index];
            // ReSharper disable once SwitchStatementHandlesSomeKnownEnumValuesWithDefault
            switch (statement.Kind)
            {
                case BoundNodeKind.VariableDeclarationStatement:
                    EvaludateVariableDeclarationStatement((BoundVariableDeclarationStatement)statement);
                    index++;
                    break;
                case BoundNodeKind.ExpressionStatement:
                    EvaluateExpressionStatement((BoundExpressionStatement)statement);
                    index++;
                    break;
                case BoundNodeKind.ConditionalGotoStatement:
                    var conditionalGoto = (BoundConditionalGotoStatement)statement;
                    var condition = (bool)EvaluateExpression(conditionalGoto.Condition);
                    if (condition && !conditionalGoto.JumpIfFalse || !condition && conditionalGoto.JumpIfFalse)
                    {
                        index = labelToIndex[conditionalGoto.Label];
                    }
                    else
                    {
                        index++;
                    }
                    break;
                case BoundNodeKind.GotoStatement:
                    index = labelToIndex[((BoundGotoStatement)statement).Label];
                    break;
                case BoundNodeKind.LabelStatement:
                    index++;
                    break;
                default:
                    throw new Exception($"Unexpected node {statement.Kind}");
            }
        }

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
            BoundUnaryOperatorKind.OnesComplement => ~(int) operand,
            _ => throw new Exception($"Unexpected unary operator {node.Operator}")
        };
    }

    private object EvaluateBinaryExpression(BoundBinaryExpression b)
    {
        var left = EvaluateExpression(b.Left);
        var right = EvaluateExpression(b.Right);

        return b.Operator?.Kind switch
        {
            BoundBinaryOperatorKind.Addition => (int)left + (int)right,
            BoundBinaryOperatorKind.Subtraction => (int)left - (int)right,
            BoundBinaryOperatorKind.Multiplication => (int)left * (int)right,
            BoundBinaryOperatorKind.Division => (int)left / (int)right,
            BoundBinaryOperatorKind.BitwiseAnd => b.Type == typeof(int)
                ? (int)left & (int)right
                : (bool)left & (bool)right,
            BoundBinaryOperatorKind.BitwiseOr => b.Type == typeof(int)
                ? (int)left | (int)right
                : (bool)left | (bool)right,
            BoundBinaryOperatorKind.BitwiseXor => b.Type == typeof(int)
                ? (int)left ^ (int)right
                : (bool)left ^ (bool)right,
            BoundBinaryOperatorKind.LogicalAnd => (bool)left && (bool)right,
            BoundBinaryOperatorKind.LogicalOr => (bool)left || (bool)right,
            BoundBinaryOperatorKind.Equals => Equals(left, right),
            BoundBinaryOperatorKind.NotEquals => !Equals(left, right),
            BoundBinaryOperatorKind.Less => (int)left < (int)right,
            BoundBinaryOperatorKind.LessOrEquals => (int)left <= (int)right,
            BoundBinaryOperatorKind.Greater => (int)left > (int)right,
            BoundBinaryOperatorKind.GreaterOrEquals => (int)left >= (int)right,
            _ => throw new Exception($"Unexpected binary operator {b.Operator}")
        };
    }
}