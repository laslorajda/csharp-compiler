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
            if (token.Type is not TokenType.WhitespaceToken and not TokenType.BadResultToken)
            {
                tokens.Add(token);
            }
        } while (token.Type != TokenType.EndOfFileToken);

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
        var left = ParsePrimary();

        while (true)
        {
            var precedence = GetBinaryOperatorPrecedence(Current.Type);
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
        if (Current.Type == TokenType.OpenParenthesisToken)
        {
            var left = NextToken();
            var expression = ParseExpression();
            var right = NextToken();
            if (right.Type != TokenType.CloseParenthesisToken)
            {
                throw new Exception($"Expected ')' but got '{right.Value}'");
            }

            return new ParenthesizedExpressionSyntax(left, expression, right);
        }
        
        if (Current.Type != TokenType.NumberToken)
        {
            throw new Exception($"Unexpected token <{Current.Type}>");
        }

        return new NumberExpressionSyntax(NextToken());
    }

    private static int GetBinaryOperatorPrecedence(TokenType type) =>
        type switch
        {
            TokenType.StarToken or TokenType.SlashToken => 2,
            TokenType.PlusToken or TokenType.MinusToken  => 1,
            _ => 0
        };
}