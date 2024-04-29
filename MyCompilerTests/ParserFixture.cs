using FluentAssertions;
using MyCompiler.Syntax;

namespace MyCompilerTests;

public class ParserFixture
{
    [Fact]
    public void EmptyStringShouldThrowException()
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
        
        result.Root.Kind.Should().Be(SyntaxKind.LiteralExpression);
        var children = result.Root.GetChildren().ToList();
        children.Should().HaveCount(1);
        children[0].Kind.Should().Be(SyntaxKind.NumberToken);
        ((SyntaxToken)children[0]).Value.Should().Be(123);
    }
    
    [Fact]
    public void SingleAdditionShouldReturnAdditionToken()
    {
        const string text = "1 + 2";
        
        var result = new Parser(text).Parse();

        result.Root.Kind.Should().Be(SyntaxKind.BinaryExpression);
        var children = result.Root.GetChildren().ToList();
        children.Should().HaveCount(3);
        children[0].Kind.Should().Be(SyntaxKind.LiteralExpression);
        children[1].Kind.Should().Be(SyntaxKind.PlusToken);
        children[2].Kind.Should().Be(SyntaxKind.LiteralExpression);
    }
    
    [Fact]
    public void SingleSubtractionShouldReturnSubtractionToken()
    {
        const string text = "1 - 2";
        
        var result = new Parser(text).Parse();

        result.Root.Kind.Should().Be(SyntaxKind.BinaryExpression);
        var children = result.Root.GetChildren().ToList();
        children.Should().HaveCount(3);
        children[0].Kind.Should().Be(SyntaxKind.LiteralExpression);
        children[1].Kind.Should().Be(SyntaxKind.MinusToken);
        children[2].Kind.Should().Be(SyntaxKind.LiteralExpression);
    }
    
    [Fact]
    public void MultipleAdditionsShouldReturnAdditionToken()
    {
        const string text = "1 + 2 + 3";
        
        var result = new Parser(text).Parse();

        result.Root.Kind.Should().Be(SyntaxKind.BinaryExpression);
        var children = result.Root.GetChildren().ToList();
        children.Should().HaveCount(3);
        children[0].Kind.Should().Be(SyntaxKind.BinaryExpression);
        children[1].Kind.Should().Be(SyntaxKind.PlusToken);
        children[2].Kind.Should().Be(SyntaxKind.LiteralExpression);
    }
    
    [Fact]
    public void MultiplicationShouldHavePrecedenceOverAddition()
    {
        const string text = "1 + 2 * 3";
        
        var result = new Parser(text).Parse();

        result.Root.Kind.Should().Be(SyntaxKind.BinaryExpression);
        var children = result.Root.GetChildren().ToList();
        children.Should().HaveCount(3);
        children[0].Kind.Should().Be(SyntaxKind.LiteralExpression);
        children[1].Kind.Should().Be(SyntaxKind.PlusToken);
        children[2].Kind.Should().Be(SyntaxKind.BinaryExpression);
    }
    
    [Fact]
    public void ParenthesesShouldHavePrecedenceOverMultiplication()
    {
        const string text = "(1 + 2) * 3";
        
        var result = new Parser(text).Parse();

        result.Root.Kind.Should().Be(SyntaxKind.BinaryExpression);
        var children = result.Root.GetChildren().ToList();
        children.Should().HaveCount(3);
        children[0].Kind.Should().Be(SyntaxKind.ParenthesizedExpression);
        children[1].Kind.Should().Be(SyntaxKind.StarToken);
        children[2].Kind.Should().Be(SyntaxKind.LiteralExpression);
    }
    
    [Fact]
    public void UnaryMinusShouldHavePrecedenceOverMultiplication()
    {
        const string text = "-1 * 3";
        
        var result = new Parser(text).Parse();

        result.Root.Kind.Should().Be(SyntaxKind.BinaryExpression);
        var children = result.Root.GetChildren().ToList();
        children.Should().HaveCount(3);
        children[0].Kind.Should().Be(SyntaxKind.UnaryExpression);
        children[1].Kind.Should().Be(SyntaxKind.StarToken);
        children[2].Kind.Should().Be(SyntaxKind.LiteralExpression);
    }
}