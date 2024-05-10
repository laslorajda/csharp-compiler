﻿using System.Collections.Immutable;
using Compiler.CodeAnalysis.Binding;
using Compiler.CodeAnalysis.Syntax;

namespace Compiler.CodeAnalysis.Lowering;

internal sealed class Lowerer : BoundTreeRewriter
{
    private int _labelCount;

    private Lowerer()
    {
    }

    private LabelSymbol GenerateLabel()
    {
        var name = $"label{_labelCount++}";
        return new LabelSymbol(name);
    }

    public static BoundBlockStatement Lower(BoundStatement statement)
    {
        var lowerer = new Lowerer();
        var result = lowerer.RewriteStatement(statement);
        return Flatten(result);
    }

    private static BoundBlockStatement Flatten(BoundStatement statement)
    {
        var builder = ImmutableArray.CreateBuilder<BoundStatement>();
        var stack = new Stack<BoundStatement>();
        stack.Push(statement);

        while (stack.Count > 0)
        {
            var current = stack.Pop();
            if (current is BoundBlockStatement block)
            {
                foreach (var s in block.Statements.Reverse())
                {
                    stack.Push(s);
                }
            }
            else
            {
                builder.Add(current);
            }
        }

        return new BoundBlockStatement(builder.ToImmutable());
    }

    protected override BoundStatement RewriteForStatement(BoundForStatement node)
    {
        var variableDeclaration = new BoundVariableDeclarationStatement(node.Variable, node.LowerBound);
        var variableExpression = new BoundVariableExpression(node.Variable);
        var upperBoundSymbol = new VariableSymbol("upperBound", true, typeof(int));
        var upperBoundDeclaration = new BoundVariableDeclarationStatement(upperBoundSymbol, node.UpperBound);
        var condition = new BoundBinaryExpression(variableExpression,
            BoundBinaryOperator.Bind(SyntaxKind.LessOrEqualsToken, typeof(int), typeof(int)), new BoundVariableExpression(upperBoundSymbol));
        var increment = new BoundExpressionStatement(
            new BoundAssignmentExpression(node.Variable, new BoundBinaryExpression(
                variableExpression,
                BoundBinaryOperator.Bind(SyntaxKind.PlusToken, typeof(int), typeof(int)),
                new BoundLiteralExpression(1))));


        var whileBody = new BoundBlockStatement([node.Body, increment]);
        var whileSatatement = new BoundWhileStatement(condition, whileBody);

        var result = new BoundBlockStatement([variableDeclaration, upperBoundDeclaration, whileSatatement]);
        return RewriteStatement(result);
    }

    protected override BoundStatement RewriteIfStatement(BoundIfStatement node)
    {
        var endLabel = GenerateLabel();
        var endLabelStatement = new BoundLabelStatement(endLabel);

        if (node.ElseStatement == null)
        {
            var gotoFalseStatement = new BoundConditionalGotoStatement(endLabel, node.Condition, true);
            var result = new BoundBlockStatement([gotoFalseStatement, node.ThenStatement, endLabelStatement]);
            return RewriteStatement(result);
        }

        var elseLabel = GenerateLabel();
        var gotoFalse = new BoundConditionalGotoStatement(elseLabel, node.Condition, true);
        var gotoEndStatement = new BoundGotoStatement(endLabel);
        var elseLabelStatement = new BoundLabelStatement(elseLabel);
        var resultWithElse = new BoundBlockStatement([
            gotoFalse, node.ThenStatement, gotoEndStatement, elseLabelStatement, node.ElseStatement,
            endLabelStatement
        ]);

        return RewriteStatement(resultWithElse);
    }

    protected override BoundStatement RewriteWhileStatement(BoundWhileStatement node)
    {
        var continueLabel = GenerateLabel();
        var checkLabel = GenerateLabel();
        var endLabel = GenerateLabel();

        var gotoCheckStatement = new BoundGotoStatement(checkLabel);
        var continueLabelStatement = new BoundLabelStatement(continueLabel);
        var checkLabelStatement = new BoundLabelStatement(checkLabel);
        var gotoTrue = new BoundConditionalGotoStatement(continueLabel, node.Condition, false);
        var endLabelStatement = new BoundLabelStatement(endLabel);

        var result = new BoundBlockStatement([
            gotoCheckStatement, continueLabelStatement, node.Body, checkLabelStatement, gotoTrue, endLabelStatement
        ]);

        return RewriteStatement(result);
    }
}