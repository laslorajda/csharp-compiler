using Compiler.CodeAnalysis.Text;

namespace Compiler.CodeAnalysis.Syntax;

internal sealed class Lexer
{
    private readonly SourceText _text;
    private int _position;
    private int _start;
    private SyntaxKind _kind;
    private object _value = null!;

    public Lexer(SourceText text)
    {
        _text = text;
    }

    public DiagnosticBag Diagnostics { get; } = new();

    private char Current => Peek(0);

    private char Peek(int offset)
    {
        var index = _position + offset;
        return index >= _text.Length ? '\0' : _text[index];
    }

    public SyntaxToken Lex()
    {
        _start = _position;
        _kind = SyntaxKind.BadResultToken;
        _value = null!;
        
        switch (Current)
        {
            case '\0':
                _kind = SyntaxKind.EndOfFileToken;
                break;
            case '+':
                _kind = SyntaxKind.PlusToken;
                _position++;
                break;
            case '-':
                _kind = SyntaxKind.MinusToken;
                _position++;
                break;
            case '*':
                _kind = SyntaxKind.StarToken;
                _position++;
                break;
            case '/':
                _kind = SyntaxKind.SlashToken;
                _position++;
                break;
            case '(':
                _kind = SyntaxKind.OpenParenthesis;
                _position++;
                break;
            case ')':
                _kind = SyntaxKind.CloseParenthesis;
                _position++;
                break;
            case '!':
                _position++;
                if (Current == '=')
                {
                    _position++;
                    _kind = SyntaxKind.BangEqualsToken;
                    break;
                }

                _kind = SyntaxKind.BangToken;
                break;
            case '&':
                _position++;
                if (Current == '&')
                {
                    _position++;
                    _kind = SyntaxKind.AmpersandAmpersandToken;
                    break;
                }

                goto default;
            case '|':
                _position++;
                if (Current == '|')
                {
                    _kind = SyntaxKind.PipePipeToken;
                    _position++;
                    break;
                }

                goto default;
            case '=':
                _position++;
                if (Current == '=')
                {
                    _kind = SyntaxKind.EqualsEqualsToken;
                    _position++;
                    break;
                }

                _kind = SyntaxKind.EqualsToken;
                break;
            case '0':
            case '1':
            case '2':
            case '3':
            case '4':
            case '5':
            case '6':
            case '7':
            case '8':
            case '9':
                ReadNumberToken();
                break;
            case ' ':
            case '\t':
            case '\n':
            case '\r':
                ReadWhitespace();
                break;
            default:
                if (char.IsLetter(Current))
                {
                    ReadIdentifierOrKeyword();
                }else if (char.IsWhiteSpace(Current))
                {
                    ReadWhitespace();
                }
                else
                {
                    Diagnostics.ReportBadCharacter(_position, Current);
                    return new SyntaxToken(SyntaxKind.BadResultToken, null, _text.ToString(_position - 1, 1),
                        _position++);
                }

                break;
        }

        var length = _position - _start;
        var text = SyntaxFacts.GetText(_kind);
        if (text == string.Empty)
        {
            text = _text.ToString(_start, length);
        }

        return new SyntaxToken(_kind, _value, text, _start);
    }

    private void ReadWhitespace()
    {
        while (_position < _text.Length && char.IsWhiteSpace(Current))
        {
            _position++;
        }

        _kind = SyntaxKind.WhitespaceToken;
    }

    private void ReadNumberToken()
    {
        while (_position < _text.Length && char.IsDigit(Current))
        {
            _position++;
        }

        var length = _position - _start;
        var text = _text.ToString(_start, length);

        if (!int.TryParse(text, out var value))
        {
            Diagnostics.ReportInvalidNumber(new TextSpan(_start, length), text, typeof(int));
        }

        _value = value;
        _kind = SyntaxKind.NumberToken;
    }

    private void ReadIdentifierOrKeyword()
    {
        while (_position < _text.Length && char.IsLetter(Current))
        {
            _position++;
        }
        
        var length = _position - _start;
        var text = _text.ToString(_start, length);
        _kind = SyntaxFacts.GetKeywordKind(text);
    }
}