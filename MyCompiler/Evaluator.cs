using MyCompiler.Binding;
using MyCompiler.Syntax;

namespace MyCompiler;

internal class Evaluator
{
    private readonly BoundExpression _root;

    public Evaluator(BoundExpression root)
    {
        _root = root;
    }

    public int Evaluate()
    {
        return EvaluateExpression(_root);
    }

    private static int EvaluateExpression(BoundExpression node)
    {
        while (true)
        {
            switch (node)
            {
                case BoundLiteralExpression n:
                    return (int)n.Value;
                case BoundUnaryExpression u:
                {
                    var operand = EvaluateExpression(u.Operand);
                    return u.OperatorKind switch
                    {
                        BoundUnaryOperatorKind.Identity => operand,
                        BoundUnaryOperatorKind.Negation => -operand,
                        _ => throw new Exception($"Unexpected unary operator {u.OperatorKind}")
                    };
                }
                case BoundBinaryExpression b:
                {
                    var left = EvaluateExpression(b.Left);
                    var right = EvaluateExpression(b.Right);

                    return b.OperatorKind switch
                    {
                        BoundBinaryOperatorKind.Addition => left + right,
                        BoundBinaryOperatorKind.Subtraction => left - right,
                        BoundBinaryOperatorKind.Multiplication => left * right,
                        BoundBinaryOperatorKind.Division => left / right,
                        _ => throw new Exception($"Unexpected binary operator {b.OperatorKind}")
                    };
                }
            }

            throw new Exception($"Unexpected node {node.Kind}");
        }
    }
}