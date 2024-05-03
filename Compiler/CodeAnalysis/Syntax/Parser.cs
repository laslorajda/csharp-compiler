namespace Compiler.CodeAnalysis.Syntax;

public sealed class Parser
{
    private readonly SyntaxToken[] _tokens;
    private int _position;
    private readonly List<string> _diagnostics = [];
    
    public IEnumerable<string> Diagnostic => _diagnostics;
    
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
        _diagnostics.AddRange(lexer.Diagnostic);
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
        return new SyntaxTree(expression, _diagnostics);
    }

    private ExpressionSyntax ParseExpression(int parentPrecedence = 0)
    {
        ExpressionSyntax left;
        var unaryOperatorPrecedence = Current.Kind.GetUnaryOperatorPrecedence();

        if (unaryOperatorPrecedence != 0 && unaryOperatorPrecedence >= parentPrecedence)
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
        switch (Current.Kind)
        {
            case SyntaxKind.OpenParenthesis:
            {
                var left = NextToken();
                var expression = ParseExpression();
                var right = NextToken();
                if (right.Kind != SyntaxKind.CloseParenthesis)
                {
                    _diagnostics.Add($"ERROR: Expected ')' but got '{right.Value}'");
                }

                return new ParenthesizedExpressionSyntax(left, expression, right);
            }
            case SyntaxKind.TrueKeyword or SyntaxKind.FalseKeyword:
            {
                var keyWordToken = NextToken();
                var value = keyWordToken.Kind == SyntaxKind.TrueKeyword;
                return new LiteralExpressionSyntax(keyWordToken, value);
            }
        }

        if (Current.Kind != SyntaxKind.NumberToken)
        {
            _diagnostics.Add($"ERROR: Unexpected token <{Current.Kind}>, expected <NumberToken>");
        }

        return new LiteralExpressionSyntax(NextToken());
    }

}