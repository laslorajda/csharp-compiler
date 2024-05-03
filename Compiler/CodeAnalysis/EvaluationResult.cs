namespace Compiler.CodeAnalysis;

public sealed class EvaluationResult
{
    public IReadOnlyList<string> Diagnostic { get; }
    public object Value { get; }
    
    public EvaluationResult(IReadOnlyList<string> diagnostic, object value)
    {
        Diagnostic = diagnostic;
        Value = value;
    }
}