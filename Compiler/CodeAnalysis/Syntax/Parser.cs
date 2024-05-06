using System.Collections.Immutable;
using Compiler.CodeAnalysis.Text;

namespace Compiler.CodeAnalysis.Syntax;

public sealed class Parser
{
    private readonly SyntaxToken[] _tokens;
    private int _position;

    public readonly DiagnosticBag Diagnostics = new();

    public Parser(SourceText text)
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
        Diagnostics.AddRange(lexer.Diagnostics);
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
    
    private SyntaxToken MatchToken(SyntaxKind kind)
    {
        if (Current.Kind == kind)
            return NextToken();

        Diagnostics.ReportUnexpectedToken(Current.Span, Current.Kind, kind);
        return new SyntaxToken(kind, Current.Position, string.Empty, -1);
    }
    
    public CompilationUnitSyntax ParseCompliationUnit()
    {
        var statement = ParseStatement();
        var endOfFileToken = MatchToken(SyntaxKind.EndOfFileToken);
        return new CompilationUnitSyntax(statement, endOfFileToken);
    }
    
    private StatementSyntax ParseStatement()
    {
        if (Current.Kind == SyntaxKind.OpenBraceToken)
        {
            return ParseBlockStatement();
        }
        return ParseExpressionStatement();
    }

    private BlockStatementSyntax ParseBlockStatement()
    {
        var statements = ImmutableArray.CreateBuilder<StatementSyntax>();

        var openBraceToken = MatchToken(SyntaxKind.OpenBraceToken);

        while (Current.Kind != SyntaxKind.EndOfFileToken && Current.Kind != SyntaxKind.CloseBraceToken)
        {
            var statement = ParseStatement();
            statements.Add(statement);
        }

        var closeBraceToken = MatchToken(SyntaxKind.CloseBraceToken);
        return new BlockStatementSyntax(openBraceToken, statements.ToImmutable(), closeBraceToken);
    }

    private ExpressionStatementSyntax ParseExpressionStatement()
    {
        var expression = ParseExpression();
        return new ExpressionStatementSyntax(expression);
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
            case SyntaxKind.OpenParenthesisToken:
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
        var left = MatchToken(SyntaxKind.OpenParenthesisToken);
        var expression = ParseExpression();
        var right = MatchToken(SyntaxKind.CloseParenthesisToken);

        return new ParenthesizedExpressionSyntax(left, expression, right);
    }

    private LiteralExpressionSyntax ParseBooleanLiteralExpression()
    {
        var isTrue = Current.Kind == SyntaxKind.TrueKeyword;
        var keywordToken = isTrue ? MatchToken(SyntaxKind.TrueKeyword) : MatchToken(SyntaxKind.FalseKeyword);
        return new LiteralExpressionSyntax(keywordToken, isTrue);
    }

    private NameExpressionSyntax PraseNameExpression()
    {
        var identifierToken = MatchToken(SyntaxKind.IdentifierToken);
        return new NameExpressionSyntax(identifierToken);
    }
}