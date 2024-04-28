using FluentAssertions;
using MyCompiler.Syntax;
using MyCompilerTests.Data;

namespace MyCompilerTests;

public class LexerFixture
{
    [Fact]
    public void EmptyStringReturnsEndOfFileToken()
    {
        var lexer = new Lexer(string.Empty);
        var token = lexer.GetNextToken();
        token.Type.Should().Be(TokenType.EndOfFileToken);
    }
    
    [Fact]
    public void StringContainingOnlyWhitespacesShouldReturnWhitespaceToken()
    {
        var lexer = new Lexer("   ");
        var token = lexer.GetNextToken();
        token.Type.Should().Be(TokenType.WhitespaceToken);
    }
    
    [Fact]
    public void StringContainingNumberShouldReturnNumberTokenWithValue()
    {
        var lexer = new Lexer("123");
        var token = lexer.GetNextToken();
        token.Type.Should().Be(TokenType.NumberToken);
        token.Value.Should().Be(123);
    }

    [Fact]
    public void InvalidStringShouldReturnBadResultToken()
    {
        var lexer = new Lexer("asd");
        var token = lexer.GetNextToken();
        token.Type.Should().Be(TokenType.BadResultToken);
    }
    
    [Theory]
    [ClassData(typeof(MathematicalOperatorsData))]
    public void StringContainingMathematicalOperatorsShouldReturnCorrectTokens(string text, TokenType expectedType)
    {
        var lexer = new Lexer(text);
        var token = lexer.GetNextToken();
        token.Type.Should().Be(expectedType);
    }
    
    [Fact]
    public void ParenthesesShouldReturnCorrectTokens()
    {
        var lexer = new Lexer("()");
        var token = lexer.GetNextToken();
        token.Type.Should().Be(TokenType.OpenParenthesisToken);
        
        token = lexer.GetNextToken();
        token.Type.Should().Be(TokenType.CloseParenthesisToken);
    }
}