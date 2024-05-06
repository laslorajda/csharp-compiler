using Compiler.CodeAnalysis.Text;
using FluentAssertions;

namespace MyCompiler.Tests.CodeAnalysis.Text;

public class SourceTextTests
{
    [Theory]
    [InlineData(".", 1)]
    [InlineData(".\r\n", 2)]
    [InlineData(".\r\n\r\n", 3)]
    public void SourceTextIncludesLastLine(string text, int expectedLineCount)
    {
        var sourceText = SourceText.From(text);

        sourceText.Lines.Length.Should().Be(expectedLineCount);
    }
}