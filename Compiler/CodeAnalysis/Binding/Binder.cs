using Compiler.CodeAnalysis.Syntax;

namespace Compiler.CodeAnalysis.Binding;

internal sealed class Binder
{
    public DiagnosticBag Diagnostics = new();
    
    internal BoundExpression BindExpression(ExpressionSyntax syntax)
    {
        return syntax.Kind switch
        {
            SyntaxKind.LiteralExpression => BindLiteralExpression((LiteralExpressionSyntax) syntax),
            SyntaxKind.UnaryExpression => BindUnaryExpression((UnaryExpressionSyntax) syntax),
            SyntaxKind.BinaryExpression => BindBinaryExpression((BinaryExpressionSyntax) syntax),
            SyntaxKind.ParenthesizedExpression => BindExpression(((ParenthesizedExpressionSyntax)syntax).Expression),
            _ => throw new Exception($"Unexpected syntax {syntax.Kind}")
        };
    }


    private static BoundLiteralExpression BindLiteralExpression(LiteralExpressionSyntax syntax)
    {
        var value = syntax.Value ?? 0;
        return new BoundLiteralExpression(value);
    }
    
    private BoundExpression BindUnaryExpression(UnaryExpressionSyntax syntax)
    {
        var boundOperand = BindExpression(syntax.Operand);
        var boundOperandType = boundOperand.Type!;
        var boundOperatorKind = BoundUnaryOperator.Bind(syntax.OperatorToken.Kind, boundOperandType);
        if (boundOperatorKind == null)
        {
            Diagnostics.ReportUndefinedUnaryOperator(syntax.OperatorToken.Span, syntax.OperatorToken.Text,
                boundOperandType);
            return boundOperand;
        }
        return new BoundUnaryExpression(boundOperatorKind, boundOperand);
    }

    private BoundExpression BindBinaryExpression(BinaryExpressionSyntax syntax)
    {
        var boundLeft = BindExpression(syntax.Left);
        var boundRight = BindExpression(syntax.Right);
        var boundOperatorKind = BoundBinaryOperator.Bind(syntax.OperatorToken.Kind, boundLeft.Type, boundRight.Type);
        if (boundOperatorKind == null)
        {
            Diagnostics.ReportUndefinedBinaryOperator(syntax.OperatorToken.Span, syntax.OperatorToken.Text,
                boundLeft.Type, boundRight.Type);
            return boundLeft;
        }

        return new BoundBinaryExpression(boundLeft, boundOperatorKind, boundRight);
    }
}