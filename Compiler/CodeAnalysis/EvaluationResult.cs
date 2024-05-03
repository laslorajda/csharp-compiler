namespace Compiler.CodeAnalysis;

public sealed class EvaluationResult
{
    public IEnumerable<Diagnostic> Diagnostic { get; }
    public object Value { get; }
    
    public EvaluationResult(IEnumerable<Diagnostic> diagnostic, object value)
    {
        Diagnostic = diagnostic;
        Value = value;
    }
}