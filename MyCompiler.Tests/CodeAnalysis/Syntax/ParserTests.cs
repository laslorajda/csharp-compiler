using Compiler.CodeAnalysis.Syntax;

namespace MyCompiler.Tests.CodeAnalysis.Syntax;

public class ParserTests
{
    
    [Theory]
    [MemberData(nameof(GetBinaryOperatorPairsData))]
    public void ParserBinaryExpressionHonorsPrecedences(SyntaxKind op1, SyntaxKind op2)
    {
        var op1Precedence = op1.GetBinaryOperatorPrecedence();
        var op2Precedence = op2.GetBinaryOperatorPrecedence();
        var op1Text = SyntaxFacts.GetText(op1);
        var op2Text = SyntaxFacts.GetText(op2);
        var text = $"a {op1Text} b {op2Text} c";
        var expression = ParseExpression(text);
        using var e = new AssertingEnumerator(expression);

        if (op1Precedence >= op2Precedence)
        {     
            //     op2
            //    /   \
            //   op1   c
            //  /   \
            // a     b
            e.AssertNode(SyntaxKind.BinaryExpression);
            e.AssertNode(SyntaxKind.BinaryExpression);
            e.AssertNode(SyntaxKind.NameExpression);
            e.AssertToken(SyntaxKind.IdentifierToken, "a");
            e.AssertToken(op1, op1Text);
            e.AssertNode(SyntaxKind.NameExpression);
            e.AssertToken(SyntaxKind.IdentifierToken, "b");
            e.AssertToken(op2, op2Text);
            e.AssertNode(SyntaxKind.NameExpression);
            e.AssertToken(SyntaxKind.IdentifierToken, "c");
        }
        else
        {
            //   op1
            //  /   \
            // a    op2
            //     /   \
            //    b     c
            e.AssertNode(SyntaxKind.BinaryExpression);
            e.AssertNode(SyntaxKind.NameExpression);
            e.AssertToken(SyntaxKind.IdentifierToken, "a");
            e.AssertToken(op1, op1Text);
            e.AssertNode(SyntaxKind.BinaryExpression);
            e.AssertNode(SyntaxKind.NameExpression);
            e.AssertToken(SyntaxKind.IdentifierToken, "b");
            e.AssertToken(op2, op2Text);
            e.AssertNode(SyntaxKind.NameExpression);
            e.AssertToken(SyntaxKind.IdentifierToken, "c");
        }
        
    }

    private static ExpressionSyntax ParseExpression(string text)
    {
        var expression = SyntaxTree.Parse(text).Root.Expression;
        return expression;
    }

    [Theory]
    [MemberData(nameof(GetUnaryOperatorPairsData))]
    public void ParserUnaryExpressionHonorsPrecedences(SyntaxKind unaryKind, SyntaxKind binaryKind)
    {
        var unaryPrecedence = unaryKind.GetUnaryOperatorPrecedence();
        var binaryPrecedence = binaryKind.GetBinaryOperatorPrecedence();
        var unaryText = SyntaxFacts.GetText(unaryKind);
        var binaryText = SyntaxFacts.GetText(binaryKind);
        var text = $"{unaryText} a {binaryText} b";
        var expression = ParseExpression(text);
        using var e = new AssertingEnumerator(expression);
        
        if (unaryPrecedence >= binaryPrecedence)
        {
            //  binary
            //    /  \
            // unary  b
            //   |
            //   a
            e.AssertNode(SyntaxKind.BinaryExpression);
            e.AssertNode(SyntaxKind.UnaryExpression);
            e.AssertToken(unaryKind, unaryText);
            e.AssertNode(SyntaxKind.NameExpression);
            e.AssertToken(SyntaxKind.IdentifierToken, "a");
            e.AssertToken(binaryKind, binaryText);
            e.AssertNode(SyntaxKind.NameExpression);
            e.AssertToken(SyntaxKind.IdentifierToken, "b");
        }
        else
        {
            // unary
            //   |
            // binary
            //  /   \
            // a     b
            e.AssertNode(SyntaxKind.UnaryExpression);
            e.AssertToken(unaryKind, unaryText);
            e.AssertNode(SyntaxKind.BinaryExpression);
            e.AssertNode(SyntaxKind.NameExpression);
            e.AssertToken(SyntaxKind.IdentifierToken, "a");
            e.AssertToken(binaryKind, binaryText);
            e.AssertNode(SyntaxKind.NameExpression);
            e.AssertToken(SyntaxKind.IdentifierToken, "b");
        }
    }

    public static IEnumerable<object[]> GetBinaryOperatorPairsData()
    {
        foreach (var op1 in SyntaxFacts.GetBinaryOperatorKinds())
        {
            foreach (var op2 in SyntaxFacts.GetBinaryOperatorKinds())
            {
                yield return [op1, op2];
            }
        }   
    } 
    
    public static IEnumerable<object[]> GetUnaryOperatorPairsData()
    {
        foreach (var unary in SyntaxFacts.GetUnaryOperatorKinds())
        {
            foreach (var binary in SyntaxFacts.GetBinaryOperatorKinds())
            {
                yield return [unary, binary];
            }
        }   
    }
}