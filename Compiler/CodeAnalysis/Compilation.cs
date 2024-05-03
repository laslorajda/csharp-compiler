﻿using Compiler.CodeAnalysis.Binding;
using Compiler.CodeAnalysis.Syntax;

namespace Compiler.CodeAnalysis;

public sealed class Compilation
{
    public SyntaxTree SyntaxTree { get; }
    public Compilation(SyntaxTree syntaxTree)
    {
        SyntaxTree = syntaxTree;
    }

    public EvaluationResult Evaluate()
    {
        var binder = new Binder();
        var boundExpression = binder.BindExpression(SyntaxTree.Root);
        
        var diagnostics = SyntaxTree.Diagnostics.Concat(binder.Diagnostics);
        
        var evaluator = new Evaluator(boundExpression);
        var value = evaluator.Evaluate();

        return new EvaluationResult(Array.Empty<string>(), value);
    }
}