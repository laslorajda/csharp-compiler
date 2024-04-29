namespace MyCompiler.Syntax;

public class Parser
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

    private SyntaxToken Peek(int offset)
    {
        var index = _position + offset;
        return index >= _tokens.Length ? _tokens.Last() : _tokens[index];
    }

    private SyntaxToken Current => Peek(0);
    
    private SyntaxToken NextToken()
    {
        var current = Current;
        _position++;
        return current;
    }
    
    public SyntaxTree Parse()
    {
        var expression = ParseAdditionOrSubtraction();
        return new SyntaxTree(expression);
    }

    private ExpressionSyntax ParseAdditionOrSubtraction()
    {
        var left = ParseMultiplicationOrDivision();
        while (Current.Type is TokenType.PlusToken or TokenType.MinusToken)
        {
            var operatorToken = NextToken();
            var right = ParseMultiplicationOrDivision();
            left = new BinaryExpressionSyntax(left, operatorToken, right);
        }

        return left;
    }

    private ExpressionSyntax ParseMultiplicationOrDivision()
    {
        var left = ParsePrimary();
        while (Current.Type is TokenType.StarToken or TokenType.SlashToken)
        {
            var operatorToken = NextToken();
            var right = ParsePrimary();
            left = new BinaryExpressionSyntax(left, operatorToken, right);
        }

        return left;
    }

    private ExpressionSyntax ParsePrimary()
    {
        if (Current.Type == TokenType.OpenParenthesisToken)
        {
            var left = NextToken();
            var expression = ParseAdditionOrSubtraction();
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
}