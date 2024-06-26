﻿using System.Collections.Immutable;
using Compiler.CodeAnalysis.Binding;
using Compiler.CodeAnalysis.Lowering;
using Compiler.CodeAnalysis.Syntax;

namespace Compiler.CodeAnalysis;

public sealed class Compilation
{
    private BoundGlobalScope? _globalScope;
    public Compilation? Previous { get; }
    public SyntaxTree SyntaxTree { get; }
    public Compilation(SyntaxTree syntaxTree) : this(null, syntaxTree)
    {
    }
    
    private Compilation(Compilation? previous, SyntaxTree syntaxTree)
    {
        Previous = previous;
        SyntaxTree = syntaxTree;
    }

    internal BoundGlobalScope GlobalScope
    {
        get
        {
            if (_globalScope != null)
            {
                return _globalScope;
            }

            var globalScope = Binder.BindGlobalScope(Previous?.GlobalScope, SyntaxTree.Root);
            Interlocked.CompareExchange(ref _globalScope, globalScope, null);

            return _globalScope;
        }
    }
    
    public Compilation ContinueWith(SyntaxTree syntaxTree) => new(this, syntaxTree);

    public EvaluationResult Evaluate(Dictionary<VariableSymbol, object> variables)
    {
        
        var diagnostics = SyntaxTree.Diagnostics.Concat(GlobalScope.Diagnostics).ToImmutableArray();
        if (diagnostics.Length != 0)
        {
            return new EvaluationResult(diagnostics, null!);
        }
        
        var statement = GetStatement();
        var evaluator = new Evaluator(statement, variables);
        var value = evaluator.Evaluate();

        return new EvaluationResult(ImmutableArray<Diagnostic>.Empty, value);
    }

    public void EmiteTree(TextWriter writer)
    {
        var statement = GetStatement();
        statement.WriteTo(writer);
    }

    private BoundBlockStatement GetStatement() => Lowerer.Lower(GlobalScope.Statement);
}