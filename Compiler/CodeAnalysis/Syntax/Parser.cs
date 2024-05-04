namespace Compiler.CodeAnalysis.Syntax;

public sealed class Parser
{
    private readonly SyntaxToken[] _tokens;
    private int _position;

    private readonly DiagnosticBag _diagnostics = new();

    public Parser(string text)
    {
        var lexer = new Lexer(text);
        IList<SyntaxToken> tokens = [];

        SyntaxToken token;
        do
        {
            token = lexer.Lex();
            if (token.Kind is not SyntaxKind.WhitespaceToken and not SyntaxKind.BadResultToken)
            {
                tokens.Add(token);
            }
        } while (token.Kind != SyntaxKind.EndOfFileToken);

        _tokens = tokens.ToArray();
        _diagnostics.AddRange(lexer.Diagnostics);
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

    private ExpressionSyntax ParseExpression() => ParseAssignmentExpression();

    private ExpressionSyntax ParseAssignmentExpression()
    {
        if (Peek(0).Kind == SyntaxKind.IdentifierToken && Peek(1).Kind == SyntaxKind.EqualsToken)
        {
            var left = NextToken();
            var operatorToken = NextToken();
            var right = ParseAssignmentExpression();
            return new AssignmentExpressionSyntax(left, operatorToken, right);
        }

        return ParseBinaryExpression();
    }

    private ExpressionSyntax ParseBinaryExpression(int parentPrecedence = 0)
    {
        ExpressionSyntax left;
        var unaryOperatorPrecedence = Current.Kind.GetUnaryOperatorPrecedence();

        if (unaryOperatorPrecedence != 0 && unaryOperatorPrecedence >= parentPrecedence)
        {
            var operatorToken = NextToken();
            var operand = ParseBinaryExpression(unaryOperatorPrecedence);
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
            var right = ParseBinaryExpression(precedence);
            left = new BinaryExpressionSyntax(left, operatorToken, right);
        }

        return left;
    }

    private ExpressionSyntax ParsePrimary()
    {
        switch (Current.Kind)
        {
            case SyntaxKind.NumberToken:
                return new LiteralExpressionSyntax(NextToken());
            case SyntaxKind.OpenParenthesis:
            {
                return ParseParenthesizedExpression();
            }
            case SyntaxKind.TrueKeyword or SyntaxKind.FalseKeyword:
            {
                return ParseBooleanLiteralExpression();
            }
            case SyntaxKind.IdentifierToken:
            default:
            {
                return PraseNameExpression();
            }
        }
    }

    private ParenthesizedExpressionSyntax ParseParenthesizedExpression()
    {
        var left = NextToken();
        
        if(left.Kind != SyntaxKind.OpenParenthesis)
        {
            _diagnostics.ReportUnexpectedToken(Current.Span, left.Kind, SyntaxKind.OpenParenthesis);
        }
        
        var expression = ParseExpression();
        var right = NextToken();
        if (right.Kind != SyntaxKind.CloseParenthesis)
        {
            _diagnostics.ReportUnexpectedToken(Current.Span, right.Kind, SyntaxKind.CloseParenthesis);
        }

        return new ParenthesizedExpressionSyntax(left, expression, right);
    }

    private LiteralExpressionSyntax ParseBooleanLiteralExpression()
    {
        var keyWordToken = NextToken();
        var value = keyWordToken.Kind == SyntaxKind.TrueKeyword;
        return new LiteralExpressionSyntax(keyWordToken, value);
    }

    private NameExpressionSyntax PraseNameExpression()
    {
        var identifierToken = NextToken();
        return new NameExpressionSyntax(identifierToken);
    }
}