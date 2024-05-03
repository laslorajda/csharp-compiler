using Compiler.CodeAnalysis.Syntax;
using System.Collections;

namespace MyCompilerTests.Data;

public class MathematicalOperatorsData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return ["+", SyntaxKind.PlusToken];
        yield return ["-", SyntaxKind.MinusToken];
        yield return ["*", SyntaxKind.StarToken];
        yield return ["/", SyntaxKind.SlashToken];
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}