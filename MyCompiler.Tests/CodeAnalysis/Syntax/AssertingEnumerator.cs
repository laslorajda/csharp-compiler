using Compiler.CodeAnalysis.Syntax;
using FluentAssertions;

namespace MyCompiler.Tests.CodeAnalysis.Syntax;

internal sealed class AssertingEnumerator : IDisposable
{
    private readonly IEnumerator<SyntaxNode> _enumerator;
    private bool _hasErrors;
    public AssertingEnumerator(SyntaxNode node)
    {
        _enumerator = Flatten(node).GetEnumerator();
    }
    
    public void AssertToken(SyntaxKind kind, string text)
    {
        try
        {
            _enumerator.MoveNext().Should().BeTrue();
            _enumerator.Current.Kind.Should().Be(kind);
            _enumerator.Current.Should().BeOfType<SyntaxToken>();
            var token = (SyntaxToken)_enumerator.Current;

            token.Text.Should().Be(text);
        }
        catch when(MarkFailed())
        {
            throw;
        }
    }

    public void AssertNode(SyntaxKind kind)
    {
        try
        {
            _enumerator.MoveNext().Should().BeTrue();
            _enumerator.Current.Kind.Should().Be(kind);
            _enumerator.Current.Should().NotBeOfType<SyntaxToken>();
        }
        catch when(MarkFailed())
        {
            throw;
        }
    }

    private static IEnumerable<SyntaxNode> Flatten(SyntaxNode node)
    {
        var stack = new Stack<SyntaxNode>();
        stack.Push(node);

        while (stack.Count > 0)
        {
            var n = stack.Pop();
            yield return n;
            
            foreach (var child in n.GetChildren().Reverse())
            {
                stack.Push(child);
            }
        }
    }

    private bool MarkFailed()
    {
        _hasErrors = true;
        return false;
    }

    public void Dispose()
    {
        if (!_hasErrors)
        {
            _enumerator.MoveNext().Should().BeFalse();
        }

        _enumerator.Dispose();
    }
}