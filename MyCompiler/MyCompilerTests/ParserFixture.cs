using FluentAssertions;
using MyCompiler.Syntax;

namespace MyCompilerTests;

public class ParserFixture
{
    [Fact]
    public void EmptyStringShouldReturnNothing()
    {
        var text = string.Empty;
        Action a = () => new Parser(text).Parse();
        a.Should().Throw<Exception>();
    }
    
    [Fact]
    public void SingleNumberShouldReturnNumberToken()
    {
        const string text = "123";
        
        var result = new Parser(text).Parse();
        
        result.Root.Type.Should().Be(TokenType.NumberExpressionToken);
        var children = result.Root.GetChildren().ToList();
        children.Should().HaveCount(1);
        children[0].Type.Should().Be(TokenType.NumberToken);
        ((SyntaxToken)children[0]).Value.Should().Be(123);
    }
    
    [Fact]
    public void SingleAdditionShouldReturnAdditionToken()
    {
        const string text = "1 + 2";
        
        var result = new Parser(text).Parse();

        result.Root.Type.Should().Be(TokenType.BinaryExpression);
        var children = result.Root.GetChildren().ToList();
        children.Should().HaveCount(3);
        children[0].Type.Should().Be(TokenType.NumberExpressionToken);
        children[1].Type.Should().Be(TokenType.PlusToken);
        children[2].Type.Should().Be(TokenType.NumberExpressionToken);
    }
    
    [Fact]
    public void SingleSubtractionShouldReturnSubtractionToken()
    {
        const string text = "1 - 2";
        
        var result = new Parser(text).Parse();

        result.Root.Type.Should().Be(TokenType.BinaryExpression);
        var children = result.Root.GetChildren().ToList();
        children.Should().HaveCount(3);
        children[0].Type.Should().Be(TokenType.NumberExpressionToken);
        children[1].Type.Should().Be(TokenType.MinusToken);
        children[2].Type.Should().Be(TokenType.NumberExpressionToken);
    }
    
    [Fact]
    public void MultipleAdditionsShouldReturnAdditionToken()
    {
        const string text = "1 + 2 + 3";
        
        var result = new Parser(text).Parse();

        result.Root.Type.Should().Be(TokenType.BinaryExpression);
        var children = result.Root.GetChildren().ToList();
        children.Should().HaveCount(3);
        children[0].Type.Should().Be(TokenType.BinaryExpression);
        children[1].Type.Should().Be(TokenType.PlusToken);
        children[2].Type.Should().Be(TokenType.NumberExpressionToken);
    }
    
    [Fact]
    public void MultiplicationShouldHavePrecedenceOverAddition()
    {
        const string text = "1 + 2 * 3";
        
        var result = new Parser(text).Parse();

        result.Root.Type.Should().Be(TokenType.BinaryExpression);
        var children = result.Root.GetChildren().ToList();
        children.Should().HaveCount(3);
        children[0].Type.Should().Be(TokenType.NumberExpressionToken);
        children[1].Type.Should().Be(TokenType.PlusToken);
        children[2].Type.Should().Be(TokenType.BinaryExpression);
    }
    
    [Fact]
    public void ParenthesesShouldHavePrecedenceOverMultiplication()
    {
        const string text = "(1 + 2) * 3";
        
        var result = new Parser(text).Parse();

        result.Root.Type.Should().Be(TokenType.BinaryExpression);
        var children = result.Root.GetChildren().ToList();
        children.Should().HaveCount(3);
        children[0].Type.Should().Be(TokenType.ParenthesizedExpressionToken);
        children[1].Type.Should().Be(TokenType.StarToken);
        children[2].Type.Should().Be(TokenType.NumberExpressionToken);
    }
}