using Compiler.CodeAnalysis.Binding;
using Compiler.CodeAnalysis.Syntax;

namespace Compiler.CodeAnalysis;

public sealed class Compilation
{
    public SyntaxTree SyntaxTree { get; }
    public Compilation(SyntaxTree syntaxTree)
    {
        SyntaxTree = syntaxTree;
    }

    public EvaluationResult Evaluate(Dictionary<string, object> variables)
    {
        var binder = new Binder(variables);
        var boundExpression = binder.BindExpression(SyntaxTree.Root);
        
        var diagnostics = SyntaxTree.Diagnostics.Concat(binder.Diagnostics).ToList();
        if (diagnostics.Count != 0)
        {
            return new EvaluationResult(diagnostics, null!);
        }
        
        var evaluator = new Evaluator(boundExpression, variables);
        var value = evaluator.Evaluate();

        return new EvaluationResult(Array.Empty<Diagnostic>(), value);
    }
}