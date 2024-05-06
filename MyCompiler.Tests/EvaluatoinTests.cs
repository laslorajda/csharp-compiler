using Compiler.CodeAnalysis;
using Compiler.CodeAnalysis.Syntax;
using FluentAssertions;

namespace MyCompiler.Tests;

public class EvaluatoinTests
{
    [Theory]
    [InlineData("1", 1)]
    [InlineData("-1", -1)]
    [InlineData("+1", 1)]
    [InlineData("1 + 2", 3)]
    [InlineData("12 - 2", 10)]
    [InlineData("3 * 4", 12)]
    [InlineData("8 / 4", 2)]
    [InlineData("(10)", 10)]
    [InlineData("(10 + 2) * 2", 24)]
    [InlineData("12 == 5", false)]
    [InlineData("12 != 5", true)]
    [InlineData("4 == 4", true)]
    [InlineData("4 != 4", false)]
    [InlineData("true == false", false)]
    [InlineData("true != false", true)]
    [InlineData("false == false", true)]
    [InlineData("false != false", false)]
    [InlineData("true && true", true)]
    [InlineData("true && false", false)]
    [InlineData("false && true", false)]
    [InlineData("false && false", false)]
    [InlineData("true || true", true)]
    [InlineData("true || false", true)]
    [InlineData("false || true", true)]
    [InlineData("false || false", false)]
    [InlineData("true", true)]
    [InlineData("false", false)]
    [InlineData("!true", false)]
    [InlineData("!false", true)]
    [InlineData("!!true", true)]
    [InlineData("{ var a = 0 (a = 5) * a}", 25)]
    public void  EvaluationTests(string text, object expectedValue)
    {
        var syntaxTree = SyntaxTree.Parse(text);
        var compliation = new Compilation(syntaxTree);
        var variables = new Dictionary<VariableSymbol, object>();
        var result = compliation.Evaluate(variables);

        result.Diagnostics.Should().BeEmpty();
        result.Value.Should().Be(expectedValue);
    }
}