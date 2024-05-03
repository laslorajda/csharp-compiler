using Compiler.CodeAnalysis;
using Compiler.CodeAnalysis.Binding;
using Compiler.CodeAnalysis.Syntax;
using FluentAssertions;

namespace MyCompilerTests;

public class EvaluatorFixture
{
    [Fact]
    public void SingleNumberShouldReturnNumberValue()
    {
        const string text = "123";
        
        var syntaxTree = new Parser(text).Parse();
        var binder = new Binder();
        var boundExpression = binder.BindExpression(syntaxTree.Root);
        var result = new Evaluator(boundExpression).Evaluate();

        result.GetType().Should().Be(typeof(int));
        ((int)result).Should().Be(123);
    }
    
    [Fact]
    public void SingleAdditionShouldReturnAdditionValue()
    {
        const string text = "1 + 2";
        
        var syntaxTree = new Parser(text).Parse();
        var binder = new Binder();
        var boundExpression = binder.BindExpression(syntaxTree.Root);
        var result = new Evaluator(boundExpression).Evaluate();
        
        result.GetType().Should().Be(typeof(int));
        ((int)result).Should().Be(3);
    }
    
    [Fact]
    public void ComplexExpressionShouldReturnCorrectValue()
    {
        const string text = "1 + 2 * 3";
        
        var syntaxTree = new Parser(text).Parse();
        var binder = new Binder();
        var boundExpression = binder.BindExpression(syntaxTree.Root);
        var result = new Evaluator(boundExpression).Evaluate();
        
        result.GetType().Should().Be(typeof(int));
        ((int)result).Should().Be(7);
    }
    
    [Fact]
    public void ParentesisShouldReturnCorrectValue()
    {
        const string text = "(1 + 2) * 3";
        
        var syntaxTree = new Parser(text).Parse();
        var binder = new Binder();
        var boundExpression = binder.BindExpression(syntaxTree.Root);
        var result = new Evaluator(boundExpression).Evaluate();
        
        result.GetType().Should().Be(typeof(int));
        ((int)result).Should().Be(9);
    }
    
    [Fact]
    public void UnaryExpressionShouldReturnCorrectValue()
    {
        const string text = "-1 + 2";
        
        var syntaxTree = new Parser(text).Parse();
        var binder = new Binder();
        var boundExpression = binder.BindExpression(syntaxTree.Root);
        var result = new Evaluator(boundExpression).Evaluate();
        
        result.GetType().Should().Be(typeof(int));
        ((int)result).Should().Be(1);
    }
    
    [Fact]
    public void DoubleUnaryOperatorShouldReturnCorrectValue()
    {
        const string text = "--1";
        
        var syntaxTree = new Parser(text).Parse();
        var binder = new Binder();
        var boundExpression = binder.BindExpression(syntaxTree.Root);
        var result = new Evaluator(boundExpression).Evaluate();

        result.GetType().Should().Be(typeof(int));
        ((int)result).Should().Be(1);
    }
    
    [Fact]
    public void TrueShouldReturnTrue()
    {
        const string text = "true";
        
        var syntaxTree = new Parser(text).Parse();
        var binder = new Binder();
        var boundExpression = binder.BindExpression(syntaxTree.Root);
        var result = new Evaluator(boundExpression).Evaluate();

        result.GetType().Should().Be(typeof(bool));
        ((bool)result).Should().Be(true);
    }
    
    [Fact]
    public void FalseShouldReturnFalse()
    {
        const string text = "false";
        
        var syntaxTree = new Parser(text).Parse();
        var binder = new Binder();
        var boundExpression = binder.BindExpression(syntaxTree.Root);
        var result = new Evaluator(boundExpression).Evaluate();
        

        result.GetType().Should().Be(typeof(bool));
        ((bool)result).Should().Be(false);
    }
    
    [Fact]
    public void NotTrueShouldReturnFalse()
    {
        const string text = "!true";
        
        var syntaxTree = new Parser(text).Parse();
        var binder = new Binder();
        var boundExpression = binder.BindExpression(syntaxTree.Root);
        var result = new Evaluator(boundExpression).Evaluate();
        
        result.GetType().Should().Be(typeof(bool));
        ((bool)result).Should().Be(false);
    }
    
    [Fact]
    public void TrueAndFalseShouldReturnFalse()
    {
        const string text = "true && false";
        
        var syntaxTree = new Parser(text).Parse();
        var binder = new Binder();
        var boundExpression = binder.BindExpression(syntaxTree.Root);
        var result = new Evaluator(boundExpression).Evaluate();
        
        result.GetType().Should().Be(typeof(bool));
        ((bool)result).Should().Be(false);
    }
    
    [Fact]
    public void TrueAndTrueShouldReturnTrue()
    {
        const string text = "true && true";
        
        var syntaxTree = new Parser(text).Parse();
        var binder = new Binder();
        var boundExpression = binder.BindExpression(syntaxTree.Root);
        var result = new Evaluator(boundExpression).Evaluate();
        
        result.GetType().Should().Be(typeof(bool));
        ((bool)result).Should().Be(true);
    }
    
    [Fact]
    public void TrueOrFalseShouldReturnTrue()
    {
        const string text = "true || false";
        
        var syntaxTree = new Parser(text).Parse();
        var binder = new Binder();
        var boundExpression = binder.BindExpression(syntaxTree.Root);
        var result = new Evaluator(boundExpression).Evaluate();
        
        result.GetType().Should().Be(typeof(bool));
        ((bool)result).Should().Be(true);
    }
    
    [Fact]
    public void FalseOrFalseShouldReturnFalse()
    {
        const string text = "false || false";
        
        var syntaxTree = new Parser(text).Parse();
        var binder = new Binder();
        var boundExpression = binder.BindExpression(syntaxTree.Root);
        var result = new Evaluator(boundExpression).Evaluate();
        
        result.GetType().Should().Be(typeof(bool));
        ((bool)result).Should().Be(false);
    }
    
    [Theory]
    [InlineData("1 == 1", true)]
    [InlineData("1 == 2",false)]
    [InlineData("true == true", true)]
    [InlineData("true == false", false)]
    [InlineData("1 == 1 && 2 == 3", false)]
    [InlineData("1 == 1 || 2 == 3", true)]
    public void EqualsShouldReturnCorrectValue(string text, bool expected)
    {
        var syntaxTree = new Parser(text).Parse();
        var binder = new Binder();
        var boundExpression = binder.BindExpression(syntaxTree.Root);
        var result = new Evaluator(boundExpression).Evaluate();
        
        result.Should().Be(expected);
    }
}