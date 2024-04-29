namespace MyCompiler.Syntax;

public class Evaluator
{
    private readonly ExpressionSyntax _root;

    public Evaluator(ExpressionSyntax root)
    {
        _root = root;
    }

    public int Evaluate()
    {
        return EvaluateExpression(_root);
    }

    private int EvaluateExpression(ExpressionSyntax node)
    {
        while (true)
        {
            switch (node)
            {
                case LiteralExpression n:
                    return (int)n.LiteralToken.Value!;
                case BinaryExpression b:
                {
                    var left = EvaluateExpression(b.Left);
                    var right = EvaluateExpression(b.Right);

                    return b.OperatorToken.Kind switch
                    {
                        SyntaxKind.PlusToken => left + right,
                        SyntaxKind.MinusToken => left - right,
                        SyntaxKind.StarToken => left * right,
                        SyntaxKind.SlashToken => left / right,
                        _ => throw new Exception("Unexpected binary operator")
                    };
                }
                case ParenthesizedExpression p:
                    node = p;
                    continue;
            }

            throw new Exception($"Unexpected node {node.Kind}");
        }
    }
}