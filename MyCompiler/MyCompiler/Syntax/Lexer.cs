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
            return new SyntaxToken(TokenType.EndOfFileToken, null);
        }

        if (char.IsWhiteSpace(Current))
        {
            while (_position < _text.Length && char.IsWhiteSpace(Current))
            {
                _position++;
            }
            
            return new SyntaxToken(TokenType.WhitespaceToken, null);
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
                ? new SyntaxToken(TokenType.NumberToken, result)
                : new SyntaxToken(TokenType.BadResultToken, null);
        }

        return Current switch
        {
            '+' => new SyntaxToken(TokenType.PlusToken, null),
            '-' => new SyntaxToken(TokenType.MinusToken, null),
            '*' => new SyntaxToken(TokenType.StarToken, null),
            '/' => new SyntaxToken(TokenType.SlashToken, null),
            _ => new SyntaxToken(TokenType.BadResultToken, null)
        };
    }
}