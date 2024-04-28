using System.Collections;
using MyCompiler.Syntax;

namespace MyCompilerTests;

public class LexerFixture
{
    [Fact]
    public void EmptyStringReturnsEndOfFileToken()
    {
        var lexer = new Lexer(string.Empty);
        var token = lexer.GetNextToken();
        Assert.Equal(TokenType.EndOfFileToken, token.Type);
    }
    
    [Fact]
    public void StringContainingOnlyWhitespacesShouldReturnWhitespaceToken()
    {
        var lexer = new Lexer("   ");
        var token = lexer.GetNextToken();
        Assert.Equal(TokenType.WhitespaceToken, token.Type);
    }
    
    [Fact]
    public void StringContainingNumberShouldReturnNumberTokenWithValue()
    {
        var lexer = new Lexer("123");
        var token = lexer.GetNextToken();
        Assert.Equal(TokenType.NumberToken, token.Type);
        Assert.Equal(token.Value, 123);
    }

    [Fact]
    public void InvalidStringShouldReturnBadResultToken()
    {
        var lexer = new Lexer("asd");
        var token = lexer.GetNextToken();
        Assert.Equal(TokenType.BadResultToken, token.Type);
    }
    
    [Theory]
    [ClassData(typeof(MathematicalOperatorsData))]
    public void StringContainingMathematicalOperatorsShouldReturnCorrectTokens(string text, TokenType expectedType)
    {
        var lexer = new Lexer(text);
        var token = lexer.GetNextToken();
        Assert.Equal(expectedType, token.Type);
    }
}

public class MathematicalOperatorsData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return ["+", TokenType.PlusToken];
        yield return ["-", TokenType.MinusToken];
        yield return ["*", TokenType.StarToken];
        yield return ["/", TokenType.SlashToken];
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}