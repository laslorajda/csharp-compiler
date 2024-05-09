﻿using System.Collections.Immutable;
using Compiler.CodeAnalysis.Binding;
using Compiler.CodeAnalysis.Syntax;

namespace Compiler.CodeAnalysis.Lowering;

internal sealed class Lowerer : BoundTreeRewriter
{
    private Lowerer()
    {
    }

    public static BoundStatement Lower(BoundStatement statement)
    {
        var lowerer = new Lowerer();
        return lowerer.RewriteStatement(statement);
    }

    protected override BoundStatement RewriteForStatement(BoundForStatement node)
    {
        var variableDeclaration = new BoundVariableDeclarationStatement(node.Variable, node.LowerBound);
        var variableExpression = new BoundVariableExpression(node.Variable);
        var condition = new BoundBinaryExpression(variableExpression,
            BoundBinaryOperator.Bind(SyntaxKind.LessOrEqualsToken, typeof(int), typeof(int)), node.UpperBound);
        var increment = new BoundExpressionStatement(
            new BoundAssignmentExpression(node.Variable, new BoundBinaryExpression(
                variableExpression,
                BoundBinaryOperator.Bind(SyntaxKind.PlusToken, typeof(int), typeof(int)),
                new BoundLiteralExpression(1))));


        var whileBody = new BoundBlockStatement([node.Body, increment]);
        var whileSatatement = new BoundWhileStatement(condition, whileBody);

        var result = new BoundBlockStatement([variableDeclaration, whileSatatement]);
        return RewriteStatement(result);
    }
}