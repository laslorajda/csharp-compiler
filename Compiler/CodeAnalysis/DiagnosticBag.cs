using Compiler.CodeAnalysis.Syntax;
using System.Collections;

namespace Compiler.CodeAnalysis;

public sealed class DiagnosticBag : IEnumerable<Diagnostic>
{
    private readonly List<Diagnostic> _diagnostics = [];
    
    private void Report(TextSpan span, string message)
    {
        var diagnostic = new Diagnostic(span, message);
        _diagnostics.Add(diagnostic);
    }

    public IEnumerator<Diagnostic> GetEnumerator() => _diagnostics.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public void ReportInvalidNumber(TextSpan textSpan, string text, Type type)
    {
        var message = $"The number ${text} isn't a valid ${type}.";
        Report(textSpan, message);
    }

    public void ReportBadCharacter(int position, char character)
    {
        var message = $"Bad character input: '{character}'.";
        Report(new TextSpan(position, 1), message);
    }

    public void AddRange(DiagnosticBag diagnostics)
    {
        _diagnostics.AddRange(diagnostics._diagnostics);
    }

    public void ReportUnexpectedToken(TextSpan span, SyntaxKind actualKind, SyntaxKind expectedKind)
    {
        var message = $"Unexpected token <{actualKind}>, expected <{expectedKind}>.";
        Report(span, message);
    }

    public void ReportUndefinedUnaryOperator(TextSpan span, string operatorText, Type operandType)
    {
        var message = $"Unary operator '{operatorText}' is not defined for type '{operandType}'.";
        Report(span, message);
    }

    public void ReportUndefinedBinaryOperator(TextSpan span, string operatorText, Type leftType, Type rightType)
    {
        var message = $"Binary operator '{operatorText}' is not defined for types '{leftType}' and '{rightType}'.";
        Report(span, message);
    }
}