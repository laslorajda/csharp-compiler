using FluentAssertions;
using MyCompiler;
using MyCompiler.Binding;
using MyCompiler.Syntax;

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
}