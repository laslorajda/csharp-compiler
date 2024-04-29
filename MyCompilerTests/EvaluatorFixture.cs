using FluentAssertions;
using MyCompiler.Syntax;

namespace MyCompilerTests;

public class EvaluatorFixture
{
    [Fact]
    public void SingleNumberShouldReturnNumberValue()
    {
        const string text = "123";
        
        var syntaxTree = new Parser(text).Parse();
        var result = new Evaluator(syntaxTree.Root).Evaluate();
        
        result.Should().Be(123);
    }
    
    [Fact]
    public void SingleAdditionShouldReturnAdditionValue()
    {
        const string text = "1 + 2";
        
        var syntaxTree = new Parser(text).Parse();
        var result = new Evaluator(syntaxTree.Root).Evaluate();
        
        result.Should().Be(3);
    }
    
    [Fact]
    public void ComplexExpressionShouldReturnCorrectValue()
    {
        const string text = "1 + 2 * 3";
        
        var syntaxTree = new Parser(text).Parse();
        var result = new Evaluator(syntaxTree.Root).Evaluate();
        
        result.Should().Be(7);
    }
    
    [Fact]
    public void UnaryExpressionShouldReturnCorrectValue()
    {
        const string text = "-1 + 2";
        
        var syntaxTree = new Parser(text).Parse();
        var result = new Evaluator(syntaxTree.Root).Evaluate();
        
        result.Should().Be(1);
    }
    
    [Fact]
    public void DoubleUnaryOperatorShouldReturnCorrectValue()
    {
        const string text = "--1";
        
        var syntaxTree = new Parser(text).Parse();
        var result = new Evaluator(syntaxTree.Root).Evaluate();
        
        result.Should().Be(1);
    }
}