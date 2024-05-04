namespace Compiler.CodeAnalysis;

public sealed class EvaluationResult
{
    public IEnumerable<Diagnostic> Diagnostics { get; }
    public object Value { get; }
    
    public EvaluationResult(IEnumerable<Diagnostic> diagnostics, object value)
    {
        Diagnostics = diagnostics;
        Value = value;
    }
}