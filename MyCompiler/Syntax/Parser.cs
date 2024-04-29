namespace MyCompiler.Syntax;

public sealed class Parser
{
    private readonly SyntaxToken[] _tokens;
    private int _position;
    
    public Parser(string text)
    {
        var lexer = new Lexer(text);
        IList<SyntaxToken> tokens = [];

        SyntaxToken token;
        do
        {
            token = lexer.GetNextToken();
            if (token.Kind is not SyntaxKind.WhitespaceToken and not SyntaxKind.BadResultToken)
            {
                tokens.Add(token);
            }
        } while (token.Kind != SyntaxKind.EndOfFileToken);

        _tokens = tokens.ToArray();
    }

    private SyntaxToken Current => Peek(0);

    private SyntaxToken Peek(int offset)
    {
        var index = _position + offset;
        return index >= _tokens.Length ? _tokens.Last() : _tokens[index];
    }
    
    private SyntaxToken NextToken()
    {
        var current = Current;
        _position++;
        return current;
    }
    
    public SyntaxTree Parse()
    {
        var expression = ParseExpression();
        return new SyntaxTree(expression);
    }

    private ExpressionSyntax ParseExpression(int parentPrecedence = 0)
    {
        ExpressionSyntax left;
        var unaryOperatorPrecedence = Current.Kind.GetUnaryOperatorPrecedence();

        if (unaryOperatorPrecedence != 0 && unaryOperatorPrecedence > parentPrecedence)
        {
            var operatorToken = NextToken();
            var operand = ParseExpression(unaryOperatorPrecedence);
            left = new UnaryExpressionSyntax(operatorToken, operand);
        }
        else
        {
            left = ParsePrimary();
        }

        while (true)
        {
            var precedence = Current.Kind.GetBinaryOperatorPrecedence();
            if (precedence == 0 || precedence <= parentPrecedence)
            {
                break;
            }
            
            var operatorToken = NextToken();
            var right = ParseExpression(precedence);
            left = new BinaryExpressionSyntax(left, operatorToken, right);
        }

        return left;
    }

    private ExpressionSyntax ParsePrimary()
    {
        if (Current.Kind == SyntaxKind.OpenParenthesis)
        {
            var left = NextToken();
            var expression = ParseExpression();
            var right = NextToken();
            if (right.Kind != SyntaxKind.CloseParenthesis)
            {
                throw new Exception($"Expected ')' but got '{right.Value}'");
            }

            return new ParenthesizedExpressionSyntax(left, expression, right);
        }
        
        if (Current.Kind != SyntaxKind.NumberToken)
        {
            throw new Exception($"Unexpected token <{Current.Kind}>");
        }

        return new LiteralExpressionSyntax(NextToken());
    }

}