namespace Compiler.CodeAnalysis.Syntax;

public class Lexer
{
    private readonly string _text;
    private int _position;

    public Lexer(string text)
    {
        _text = text;
    }

    public DiagnosticBag Diagnostics { get; } = new();

    private char Current => Peek(0);

    private char Lookahead => Peek(1);

    private char Peek(int offset)
    {
        var index = _position + offset;
        return index >= _text.Length ? '\0' : _text[index];
    }

    public SyntaxToken GetNextToken()
    {
        if (_position >= _text.Length)
        {
            return new SyntaxToken(SyntaxKind.EndOfFileToken, null, string.Empty, _position);
        }

        var start = _position;
        
        if (char.IsWhiteSpace(Current))
        {
            while (_position < _text.Length && char.IsWhiteSpace(Current))
            {
                _position++;
            }

            return new SyntaxToken(SyntaxKind.WhitespaceToken, null, string.Empty, start);
        }

        if (char.IsDigit(Current))
        {
            while (_position < _text.Length && char.IsDigit(Current))
            {
                _position++;
            }

            var length = _position - start;
            var text = _text.Substring(start, length);

            if (!int.TryParse(text, out var result))
            {
                return new SyntaxToken(SyntaxKind.BadResultToken, null, text, start);
            }

            Diagnostics.ReportInvalidNumber(new TextSpan(start, length), text, typeof(int));
            return new SyntaxToken(SyntaxKind.NumberToken, result, text, start);
        }

        if (char.IsLetter(Current))
        {

            while (_position < _text.Length && char.IsLetter(Current))
            {
                _position++;
            }

            var length = _position - start;
            var text = _text.Substring(start, length);
            var kind = SyntaxFacts.GetKeywordKind(text);
            return new SyntaxToken(kind, null, text, start);
        }

        switch (Current)
        {
            case '+':
                return new SyntaxToken(SyntaxKind.PlusToken, null, "+", _position++);
            case '-':
                return new SyntaxToken(SyntaxKind.MinusToken, null, "-", _position++);
            case '*':
                return new SyntaxToken(SyntaxKind.StarToken, null, "*", _position++);
            case '/':
                return new SyntaxToken(SyntaxKind.SlashToken, null, "/", _position++);
            case '(':
                return new SyntaxToken(SyntaxKind.OpenParenthesis, null, "(", _position++);
            case ')':
                return new SyntaxToken(SyntaxKind.CloseParenthesis, null, ")", _position++);
            case '!':
                if (Lookahead == '=')
                {
                    _position += 2;
                    return new SyntaxToken(SyntaxKind.BangEqualsToken, null, "!=", start);
                }
                return new SyntaxToken(SyntaxKind.BangToken, null, "!", _position++);
            case '&':
                if (Lookahead == '&')
                {
                    _position += 2;
                    return new SyntaxToken(SyntaxKind.AmpersandAmpersandToken, null, "&&", start);
                }

                goto default;
            case '|':
                if (Lookahead == '|')
                {
                    _position += 2;
                    return new SyntaxToken(SyntaxKind.PipePipeToken, null, "||", start);
                }

                goto default;
            case '=':
                if (Lookahead == '=')
                {
                    _position += 2;
                    return new SyntaxToken(SyntaxKind.EqualsEqualsToken, null, "==", start);
                }

                goto default;
            default:
                Diagnostics.ReportBadCharacter(_position, Current);
                return new SyntaxToken(SyntaxKind.BadResultToken, null, string.Empty, _position++);
        }
    }
}