namespace MyCompiler.Syntax;

public class Lexer
{
    private readonly string _text;
    private int _position;
    
    public Lexer(string text)
    {
        _text = text;
    }

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
            return new SyntaxToken(SyntaxKind.EndOfFileToken, null, string.Empty);
        }

        if (char.IsWhiteSpace(Current))
        {
            while (_position < _text.Length && char.IsWhiteSpace(Current))
            {
                _position++;
            }
            
            return new SyntaxToken(SyntaxKind.WhitespaceToken, null, string.Empty);
        }
        
        if(char.IsDigit(Current))
        {
            var start = _position;
            while (_position < _text.Length && char.IsDigit(Current))
            {
                _position++;
            }
            var length = _position - start;
            var text = _text.Substring(start, length);

            return int.TryParse(text, out var result)
                ? new SyntaxToken(SyntaxKind.NumberToken, result, text)
                : new SyntaxToken(SyntaxKind.BadResultToken, null, text);
        }

        if (char.IsLetter(Current))
        {
            var start = _position;

            while (_position < _text.Length && char.IsLetter(Current))
            {
                _position++;
            }
            
            var length = _position - start;
            var text = _text.Substring(start, length);
            var kind = SyntaxFacts.GetKeywordKind(text);
            return new SyntaxToken(kind, null, text);
        }

        switch (Current)
        {
            case '+':
                _position++;
                return new SyntaxToken(SyntaxKind.PlusToken, null, "+");
            case '-':
                _position++;
                return new SyntaxToken(SyntaxKind.MinusToken, null, "-");
            case '*':
                _position++;
                return new SyntaxToken(SyntaxKind.StarToken, null, "*");
            case '/':
                _position++;
                return new SyntaxToken(SyntaxKind.SlashToken, null, "/");
            case '(':
                _position++;
                return new SyntaxToken(SyntaxKind.OpenParenthesis, null, "(");
            case ')':
                _position++;
                return new SyntaxToken(SyntaxKind.CloseParenthesis, null, ")");
            case '!':
                _position++;
                return new SyntaxToken(SyntaxKind.BangToken, null, "!");
            case '&':
                if (Lookahead == '&')
                {
                    _position += 2;
                    return new SyntaxToken(SyntaxKind.AmpersandAmpersandToken, null, "&&");
                }

                goto default;
            case '|':
                if (Lookahead == '|')
                {
                    _position += 2;
                    return new SyntaxToken(SyntaxKind.PipePipeToken, null, "||");
                }

                goto default;
            default:
                _position++;
                return new SyntaxToken(SyntaxKind.BadResultToken, null, string.Empty);
        }
    }
}