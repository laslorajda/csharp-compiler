using System.Collections;
using MyCompiler.Syntax;

namespace MyCompilerTests.Data;

public class MathematicalOperatorsData : IEnumerable<object[]>
{
    public IEnumerator<object[]> GetEnumerator()
    {
        yield return ["+", TokenType.PlusToken];
        yield return ["-", TokenType.MinusToken];
        yield return ["*", TokenType.StarToken];
        yield return ["/", TokenType.SlashToken];
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}