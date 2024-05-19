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
    [InlineData("~1", -2)]
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
    [InlineData("10 < 15", true)]
    [InlineData("10 <= 10", true)]
    [InlineData("24 > 12", true)]
    [InlineData("24 >= 24", true)]
    [InlineData("4 < 4", false)]
    [InlineData("4 <= 3", false)]
    [InlineData("24 > 25", false)]
    [InlineData("24 >= 25", false)]
    [InlineData("1 | 2", 3)]
    [InlineData("1 | 0", 1)]
    [InlineData("1 & 2", 0)]
    [InlineData("2 & 3", 2)]
    [InlineData("1 ^ 0", 1)]
    [InlineData("0 ^ 1", 1)]
    [InlineData("1 ^ 1", 0)]
    [InlineData("true", true)]
    [InlineData("false", false)]
    [InlineData("!true", false)]
    [InlineData("!false", true)]
    [InlineData("!!true", true)]
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
    [InlineData("false | false", false)]
    [InlineData("false | true", true)]
    [InlineData("true | false", true)]
    [InlineData("true | true", true)]
    [InlineData("false & false", false)]
    [InlineData("false & true", false)]
    [InlineData("true & false", false)]
    [InlineData("true & true", true)]
    [InlineData("true ^ true", false)]
    [InlineData("false ^ true", true)]
    [InlineData("true ^ false", true)]
    [InlineData("false ^ false", false)]
    [InlineData("{ var a = 0 (a = 5) * a}", 25)]
    [InlineData("{ var a = 0 if a == 0 a = 5 a}", 5)]
    [InlineData("{ var a = 0 if a == 3 a = 5 a}", 0)]
    [InlineData("{ var a = 0 if a == 0 a = 5 else a = 15 a}", 5)]
    [InlineData("{ var a = 0 if a == 3 a = 5 else a = 15 a}", 15)]
    [InlineData("{ var a = 0 var b = 0 while a < 10 { b = a + b a = a + 1 } b}", 45)]
    [InlineData("{ var result = 0 for i = 0 to 10 { result = result + i } result }", 55)]
    [InlineData("{ var a = 10 for i = 0 to (a = a - 1) { } a }", 9)]
    public void  EvaluationTests(string text, object expectedValue)
    {
        var syntaxTree = SyntaxTree.Parse(text);
        var compliation = new Compilation(syntaxTree);
        var variables = new Dictionary<VariableSymbol, object>();
        var result = compliation.Evaluate(variables);

        result.Diagnostics.Should().BeEmpty();
        result.Value.Should().Be(expectedValue);
    }

    [Fact]
    public void EvaluatorVariableDecalarationReportsRedeclaration()
    {
        const string text = """
                            {
                                var x = 10
                                var y = 100
                                {
                                    var x = 10
                                }
                                var [x] = 5
                            }
                            """;

        const string diagnostics = """

                                   Variable 'x' is already declared.
                                   """;

        AssertDiagnostics(text, diagnostics);
    }

    [Fact]
    public void EvaluatorBlockStatementNoInifiteLoop()
    {
        const string text = """
                            {
                            [)][]
                            """;

        const string diagnostics = """

                                   Unexpected token <CloseParenthesisToken>, expected <IdentifierToken>.
                                   Unexpected token <EndOfFileToken>, expected <CloseBraceToken>.

                                   """;
        
        AssertDiagnostics(text, diagnostics);
    }
    [Fact]
    public void EvalutaorNameReportsUndefined()
    {
        const string text = "[x] * 10";

        const string diagnostics = """

                                   Variable 'x' does not exist.
                                   """;

        AssertDiagnostics(text, diagnostics);
    }
    
    [Fact]
    public void EvaluatorAssignedReportsCannotAssign()
    {
        const string text = """
                            {
                                let x = 10
                                x [=] 0
                            }
                            """;

        const string diagnostics = """
                                   
                                   Variable 'x' is read-only and cannot be assigned to.
                                   
                                   """;

        AssertDiagnostics(text, diagnostics);
    }
    
    [Fact]
    public void EvaluatorIfStatementReportsCannotConvert()
    {
        const string text = """
                   {
                       var x = 10
                       if [x]
                           x = 0
                   }
                   """;
        
        const string diagnostics = """
                                   
                                   Cannot convert type 'System.Int32' to 'System.Boolean'.
                                   
                                   """;

        AssertDiagnostics(text, diagnostics);
    }
    
    [Fact]
    public void EvaluatorWhileStatementReportsCannotConvert()
    {
        const string text = """
                            {
                                var x = 10
                                while [10]
                                    x = 0
                            }
                            """;
        
        const string diagnostics = """

                                   Cannot convert type 'System.Int32' to 'System.Boolean'.

                                   """;

        AssertDiagnostics(text, diagnostics);
    }
    
    [Fact]
    public void EvaluatorForStatementReportsCannotConvertLowerBound()
    {
        const string text = """
                            {
                                var result = 0
                                for i = [false] to 10
                                    result = result + i
                            }
                            """;

        const string diagnostics = """

                                   Cannot convert type 'System.Boolean' to 'System.Int32'.

                                   """;

        AssertDiagnostics(text, diagnostics);
    }
    
    [Fact]
    public void EvaluatorForStatementReportsCannotConvertUpperBound()
    {
        const string text = """
                            {
                                var result = 0
                                for i = 0 to [true]
                                    result = result + i
                            }
                            """;

        const string diagnostics = """
                                   
                                   Cannot convert type 'System.Boolean' to 'System.Int32'.
                                   
                                   """;

        AssertDiagnostics(text, diagnostics);
    }

    [Fact]
    public void EvaluatorNameExpressionReportsUndefined()
    {
        const string text = "[x]";

        const string diagnostics = """

                                   Variable 'x' does not exist.

                                   """;

        AssertDiagnostics(text, diagnostics);
    }
    
    [Fact]
    public void EvaluatorNameExpressionReportsNoErrorForInsertedToken()
    {
        const string text = "[]";
        
        const string diagnostics = "Unexpected token <EndOfFileToken>, expected <IdentifierToken>.";

        AssertDiagnostics(text, diagnostics);
    }
    
    [Fact]
    public void EvalatorUnaryReportUndefinedOperator()
    {
        const string text = "[+]true";
        
        const string diagnostics = """
                                   
                                   Unary operator '+' is not defined for type 'System.Boolean'.
                                   
                                   """;

        AssertDiagnostics(text, diagnostics);
    }
    
    [Fact]
    public void EvalatorBinaryReportUndefinedOperator()
    {
        const string text = "true [+] false";

        const string diagnostics = """

                                   Binary operator '+' is not defined for types 'System.Boolean' and 'System.Boolean'.

                                   """;
            
        AssertDiagnostics(text, diagnostics);
    }

    private static void AssertDiagnostics(string text, string diagnosticText)
    {
        var annotatedText = AnnotatedText.Parse(text);
        var syntaxTree = SyntaxTree.Parse(annotatedText.Text);
        var compliation = new Compilation(syntaxTree);
        var result = compliation.Evaluate(new Dictionary<VariableSymbol, object>());
        var expectedDiagnostics = AnnotatedText.UnindentLines(diagnosticText);

        if (annotatedText.Spans.Length != expectedDiagnostics.Count)
        {
            throw new Exception("Must mark as many spans as there are expected diagnostics");
        }

        result.Diagnostics.Length.Should().Be(expectedDiagnostics.Count);

        for (var i = 0; i < expectedDiagnostics.Count; i++)
        {
            var expectedMessage = expectedDiagnostics[i];
            var actualMessage = result.Diagnostics[i].Message;
            var expectedSpan = annotatedText.Spans[i];
            var actualSpan = result.Diagnostics[i].Span;
            
            actualMessage.Should().Be(expectedMessage);
            actualSpan.Should().Be(expectedSpan);
        }
    }
}