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
                return new SyntaxToken(SyntaxKind.PlusToken, null, string.Empty);
            case '-':
                _position++;
                return new SyntaxToken(SyntaxKind.MinusToken, null, string.Empty);
            case '*':
                _position++;
                return new SyntaxToken(SyntaxKind.StarToken, null, string.Empty);
            case '/':
                _position++;
                return new SyntaxToken(SyntaxKind.SlashToken, null, string.Empty);
            case '(':
                _position++;
                return new SyntaxToken(SyntaxKind.OpenParenthesis, null, string.Empty);
            case ')':
                _position++;
                return new SyntaxToken(SyntaxKind.CloseParenthesis, null, string.Empty);
            default:
                _position++;
                return new SyntaxToken(SyntaxKind.BadResultToken, null, string.Empty);
        }
    }
}