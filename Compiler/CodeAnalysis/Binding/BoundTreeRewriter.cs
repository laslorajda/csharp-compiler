using System.Collections.Immutable;

namespace Compiler.CodeAnalysis.Binding;

internal abstract class BoundTreeRewriter
{
    public virtual BoundStatement RewriteStatement(BoundStatement node) =>
        node.Kind switch
        {
            BoundNodeKind.BlockStatement => RewriteBlockStatement((BoundBlockStatement)node),
            BoundNodeKind.VariableDeclarationStatement => RewriteVariableDeclarationStatement(
                (BoundVariableDeclarationStatement)node),
            BoundNodeKind.IfStatement => RewriteIfStatement((BoundIfStatement)node),
            BoundNodeKind.WhileStatement => RewriteWhileStatement((BoundWhileStatement)node),
            BoundNodeKind.ForStatement => RewriteForStatement((BoundForStatement)node),
            BoundNodeKind.LabelStatement => RewriteLabelStatement((BoundLabelStatement)node),
            BoundNodeKind.GotoStatement => RewriteGotoStatement((BoundGotoStatement)node),
            BoundNodeKind.ConditionalGotoStatement => RewriteConditionalGotoStatement((BoundConditionalGotoStatement)node),
            BoundNodeKind.ExpressionStatement => RewriteExpressionStatement((BoundExpressionStatement)node),
            _ => throw new Exception("Unexpected node: " + node.Kind)
        };

    public virtual BoundExpression RewriteExpression(BoundExpression node) =>
        node.Kind switch
        {
            BoundNodeKind.LiteralExpression => RewriteLiteralExpression((BoundLiteralExpression) node),
            BoundNodeKind.VariableExpression => RewriteVariableExpression((BoundVariableExpression) node),
            BoundNodeKind.AssignmentExpression => RewriteAssignmentExpression((BoundAssignmentExpression) node),
            BoundNodeKind.UnaryExpression => RewriteUnaryExpression((BoundUnaryExpression) node),
            BoundNodeKind.BinaryExpression => RewriteBinaryExpression((BoundBinaryExpression) node),
            _ => throw new Exception("Unexpected node: " + node.Kind)
        };

    protected virtual BoundStatement RewriteBlockStatement(BoundBlockStatement node)
    {
        var statements = node.Statements.Select(RewriteStatement).ToImmutableArray();
        return statements.SequenceEqual(node.Statements) ? node : new BoundBlockStatement(statements);
    }

    protected virtual BoundStatement RewriteVariableDeclarationStatement(BoundVariableDeclarationStatement node)
    {
        var initializer = RewriteExpression(node.Initializer);
        return initializer == node.Initializer
            ? node
            : new BoundVariableDeclarationStatement(node.Variable, initializer);
    }

    protected virtual BoundStatement RewriteIfStatement(BoundIfStatement node)
    { 
        var condition = RewriteExpression(node.Condition);
        var thenStatement = RewriteStatement(node.ThenStatement);
        var elseStatement = node.ElseStatement == null ? null : RewriteStatement(node.ElseStatement);

        return condition == node.Condition && thenStatement == node.ThenStatement && elseStatement == node.ElseStatement
            ? node
            : new BoundIfStatement(condition, thenStatement, elseStatement);
    }

    protected virtual BoundStatement RewriteWhileStatement(BoundWhileStatement node)
    {
        var condition = RewriteExpression(node.Condition);
        var body = RewriteStatement(node.Body);

        return condition == node.Condition && body == node.Body
            ? node
            : new BoundWhileStatement(condition, body);
    }

    protected virtual BoundStatement RewriteForStatement(BoundForStatement node)
    {
        var lowerBound = RewriteExpression(node.LowerBound);
        var upperBound = RewriteExpression(node.UpperBound);
        var body = RewriteStatement(node.Body);

        return lowerBound == node.LowerBound && upperBound == node.UpperBound && body == node.Body
            ? node
            : new BoundForStatement(node.Variable, lowerBound, upperBound, body); 
    }

    protected virtual BoundStatement RewriteLabelStatement(BoundLabelStatement node) => node;

    protected virtual BoundStatement RewriteGotoStatement(BoundGotoStatement node) => node;

    protected virtual BoundStatement RewriteConditionalGotoStatement(BoundConditionalGotoStatement node)
    {
        var condition = RewriteExpression(node.Condition);
        return condition == node.Condition
            ? node
            : new BoundConditionalGotoStatement(node.Label, condition, node.JumpIfFalse);
    }

    protected virtual BoundStatement RewriteExpressionStatement(BoundExpressionStatement node)
    {
        var expression = RewriteExpression(node.Expression);
        return expression == node.Expression ? node : new BoundExpressionStatement(expression);
    }

    protected virtual BoundExpression RewriteLiteralExpression(BoundLiteralExpression node) => node;

    protected virtual BoundExpression RewriteVariableExpression(BoundVariableExpression node) => node;

    protected virtual BoundExpression RewriteAssignmentExpression(BoundAssignmentExpression node)
    {
        var expression = RewriteExpression(node.Expression);
        return expression == node.Expression ? node : new BoundAssignmentExpression(node.Variable, expression);
    }

    protected virtual BoundExpression RewriteUnaryExpression(BoundUnaryExpression node)
    {
        var operand = RewriteExpression(node.Operand);
        return operand == node.Operand ? node : new BoundUnaryExpression(node.Operator, operand);
    }

    protected virtual BoundExpression RewriteBinaryExpression(BoundBinaryExpression node)
    {
        var left = RewriteExpression(node.Left);
        var right = RewriteExpression(node.Right);
        
        if(left == node.Left && right == node.Right)
        {
            return node;
        }
        return new BoundBinaryExpression(left, node.Operator, right);
    }
}