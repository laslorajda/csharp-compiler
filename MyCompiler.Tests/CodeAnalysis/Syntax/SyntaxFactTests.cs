using Compiler.CodeAnalysis.Syntax;
using FluentAssertions;

namespace MyCompiler.Tests.CodeAnalysis.Syntax;

public class SyntaxFactTests
{
    [Theory]
    [MemberData(nameof(GetSyntaxKinds))]
    public void SyntaxFactGetTextRoundTrips(SyntaxKind kind)
    {
        var text = SyntaxFacts.GetText(kind);
        if (string.IsNullOrEmpty(text)) 
        {
            return;
        }

        var tokens = SyntaxTree.ParseTokens(text).ToArray();
        
        tokens.Should().HaveCount(1);

        var token = tokens[0];
        
        token.Kind.Should().Be(kind);
        token.Text.Should().Be(text);
    }
    
    public static IEnumerable<object[]> GetSyntaxKinds()
    {
        var kinds = Enum.GetValues<SyntaxKind>();
        foreach (var kind in kinds)
        {
            yield return [kind];
        }
    }
}