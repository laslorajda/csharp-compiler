namespace MyCompiler.Syntax;

public class Lexer
{
    private readonly string _text;
    private int _position;
    
    public Lexer(string text)
    {
        _text = text;
    }

    private char Current => _text[_position];

    public SyntaxToken GetNextToken()
    {
        if (_position >= _text.Length)
        {
            return new SyntaxToken(SyntaxKind.EndOfFileToken, null);
        }

        if (char.IsWhiteSpace(Current))
        {
            while (_position < _text.Length && char.IsWhiteSpace(Current))
            {
                _position++;
            }
            
            return new SyntaxToken(SyntaxKind.WhitespaceToken, null);
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
                ? new SyntaxToken(SyntaxKind.NumberToken, result)
                : new SyntaxToken(SyntaxKind.BadResultToken, null);
        }

        switch (Current)
        {
            case '+':
                _position++;
                return new SyntaxToken(SyntaxKind.PlusToken, null);
            case '-':
                _position++;
                return new SyntaxToken(SyntaxKind.MinusToken, null);
            case '*':
                _position++;
                return new SyntaxToken(SyntaxKind.StarToken, null);
            case '/':
                _position++;
                return new SyntaxToken(SyntaxKind.SlashToken, null);
            case '(':
                _position++;
                return new SyntaxToken(SyntaxKind.OpenParenthesis, null);
            case ')':
                _position++;
                return new SyntaxToken(SyntaxKind.CloseParenthesis, null);
            default:
                _position++;
                return new SyntaxToken(SyntaxKind.BadResultToken, null);
        }
    }
}