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
                case NumberExpressionSyntax n:
                    return (int)n.NumberToken.Value!;
                case BinaryExpressionSyntax b:
                {
                    var left = EvaluateExpression(b.Left);
                    var right = EvaluateExpression(b.Right);

                    return b.OperatorToken.Type switch
                    {
                        TokenType.PlusToken => left + right,
                        TokenType.MinusToken => left - right,
                        TokenType.StarToken => left * right,
                        TokenType.SlashToken => left / right,
                        _ => throw new Exception("Unexpected binary operator")
                    };
                }
                case ParenthesizedExpressionSyntax p:
                    node = p;
                    continue;
            }

            throw new Exception($"Unexpected node {node.Type}");
        }
    }
}