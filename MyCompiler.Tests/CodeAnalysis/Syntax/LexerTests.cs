using Compiler.CodeAnalysis.Syntax;
using FluentAssertions;

namespace MyCompiler.Tests.CodeAnalysis.Syntax;

public class LexerTests
{
    [Fact]
    public void LexerTestsAllTokens()
    {
        var tokenKinds = Enum.GetValues<SyntaxKind>()
            .Where(kind => kind.ToString().EndsWith("Keyword") || kind.ToString().EndsWith("Token"))
            .ToArray();

        var testedTokenKinds = GetTokens().Concat(GetSeparators()).Select(t => t.Kind).ToArray();
        var untestedTokenKinds = new SortedSet<SyntaxKind>(tokenKinds);
        untestedTokenKinds.Remove(SyntaxKind.BadResultToken);
        untestedTokenKinds.Remove(SyntaxKind.EndOfFileToken);
        untestedTokenKinds.ExceptWith(testedTokenKinds);
        
        untestedTokenKinds.Should().BeEmpty();
    }
    
    [Theory]
    [MemberData(nameof(GetTokensData))]
    public void LexerLexesToken(SyntaxKind kind, string text)
    {
        var tokens = SyntaxTree.ParseTokens(text).ToArray();

        tokens.Should().ContainSingle();

        var token = tokens[0];
        token.Kind.Should().Be(kind);
    }

    [Theory]
    [MemberData(nameof(GetTokenPairsData))]
    public void LexerLexesTokenPairs(SyntaxKind t1Kind, string t1Text, SyntaxKind t2Kind, string t2Text)
    {
        var text = t1Text + t2Text;
        var tokens = SyntaxTree.ParseTokens(text).ToArray();

        tokens.Should().HaveCount(2);

        tokens[0].Kind.Should().Be(t1Kind);
        tokens[0].Text.Should().Be(t1Text);
        tokens[1].Kind.Should().Be(t2Kind);
        tokens[1].Text.Should().Be(t2Text);
    }

    [Theory]
    [MemberData(nameof(GetTokenPairsWithSeparatorData))]
    public void LexerLexesTokenPairsWithSeparators(SyntaxKind t1Kind, string t1Text, SyntaxKind separatorKind, string separatorText,
        SyntaxKind t2Kind, string t2Text)
    {
        var text = t1Text + separatorText + t2Text;
        var tokens = SyntaxTree.ParseTokens(text).ToArray();

        tokens.Should().HaveCount(3);
        tokens[0].Kind.Should().Be(t1Kind);
        tokens[0].Text.Should().Be(t1Text);
        tokens[1].Kind.Should().Be(separatorKind);
        tokens[1].Text.Should().Be(separatorText);
        tokens[2].Kind.Should().Be(t2Kind);
        tokens[2].Text.Should().Be(t2Text);
    }

    public static IEnumerable<object[]> GetTokensData()
    {
        foreach (var (kind, text) in GetTokens().Concat(GetSeparators()))
        {
            yield return [kind, text];
        }
    }

    public static IEnumerable<object[]> GetTokenPairsData()
    {
        foreach (var (t1Kind, t1Text, t2Kind, t2Text) in GetTokenPairs())
        {
            yield return [t1Kind, t1Text, t2Kind, t2Text];
        }
    }

    public static IEnumerable<object[]> GetTokenPairsWithSeparatorData()
    {
        foreach (var (t1Kind, t1Text, separatorKind, separatorText, t2Kind, t2Text)  in GetTokenPairsWithSeparator())
        {
            yield return [t1Kind, t1Text, separatorKind, separatorText, t2Kind, t2Text];
        }
    }

    private static IEnumerable<(SyntaxKind Kind, string Text)> GetTokens()
    {
        var fixedTokens = Enum.GetValues<SyntaxKind>().Where(kind => SyntaxFacts.GetText(kind) != string.Empty)
            .Select(kind => (kind, SyntaxFacts.GetText(kind)));

        var dynamicTokens = new[]
        {
            (SyntaxKind.IdentifierToken, "a"),
            (SyntaxKind.IdentifierToken, "abc"),

            (SyntaxKind.NumberToken, "1"),
            (SyntaxKind.NumberToken, "123"),
        };

        return fixedTokens.Concat(dynamicTokens);
    }

    private static bool RequiresSeparator(SyntaxKind t1Kind, SyntaxKind t2Kind)
    {
        var t1IsKeyword = t1Kind.ToString().EndsWith("Keyword");
        var t2IsKeyword = t2Kind.ToString().EndsWith("Keyword");

        if (t1Kind == SyntaxKind.IdentifierToken && t2Kind == SyntaxKind.IdentifierToken)
        {
            return true;
        }

        switch (t1IsKeyword)
        {
            case true when t2IsKeyword:
            case true when t2Kind == SyntaxKind.IdentifierToken:
                return true;
        }

        switch (t1Kind)
        {
            case SyntaxKind.IdentifierToken when t2IsKeyword:
            case SyntaxKind.NumberToken when t2Kind == SyntaxKind.NumberToken:
            case SyntaxKind.BangToken when t2Kind is SyntaxKind.EqualsToken or SyntaxKind.EqualsEqualsToken:
            case SyntaxKind.EqualsToken when t2Kind is SyntaxKind.EqualsToken or SyntaxKind.EqualsEqualsToken:
                return true;
            case SyntaxKind.LessToken when t2Kind is SyntaxKind.EqualsToken or SyntaxKind.EqualsEqualsToken:
                return true;
            case SyntaxKind.GreaterToken when t2Kind is SyntaxKind.EqualsToken or SyntaxKind.EqualsEqualsToken:
                return true;
            case SyntaxKind.BadResultToken:
            case SyntaxKind.EndOfFileToken:
            case SyntaxKind.WhitespaceToken:
            case SyntaxKind.PlusToken:
            case SyntaxKind.MinusToken:
            case SyntaxKind.StarToken:
            case SyntaxKind.SlashToken:
            case SyntaxKind.AmpersandAmpersandToken:
            case SyntaxKind.PipePipeToken:
            case SyntaxKind.EqualsEqualsToken:
            case SyntaxKind.BangEqualsToken:
            case SyntaxKind.BinaryExpression:
            case SyntaxKind.LiteralExpression:
            case SyntaxKind.OpenParenthesisToken:
            case SyntaxKind.CloseParenthesisToken:
            case SyntaxKind.ParenthesizedExpression:
            case SyntaxKind.UnaryExpression:
            case SyntaxKind.NameExpression:
            case SyntaxKind.AssignmentExpression:
            case SyntaxKind.FalseKeyword:
            case SyntaxKind.TrueKeyword:
            default:
                return false;
        }
    }

    private static IEnumerable<(SyntaxKind t1Kind, string t1Text, SyntaxKind t2Kind, string t2Text)> GetTokenPairs()
    {
        foreach (var t1 in GetTokens())
        {
            foreach (var t2 in GetTokens())
            {
                if (!RequiresSeparator(t1.Kind, t2.Kind))
                {
                    yield return (t1.Kind, t1.Text, t2.Kind, t2.Text);
                }
            }
        }
    }

    private static IEnumerable<(SyntaxKind Kind, string Text)> GetSeparators()
    {
        return
        [
            (SyntaxKind.WhitespaceToken, " "),
            (SyntaxKind.WhitespaceToken, "  "),
            (SyntaxKind.WhitespaceToken, "\r"),
            (SyntaxKind.WhitespaceToken, "\n"),
            (SyntaxKind.WhitespaceToken, "\r\n"),
        ];
    }

    private static
        IEnumerable<(SyntaxKind t1Kind, string t1Text, SyntaxKind separatorKind, string separatorText, SyntaxKind t2Kind
            , string t2Text)> GetTokenPairsWithSeparator()
    {
        foreach (var t1 in GetTokens())
        {
            foreach (var t2 in GetTokens())
            {
                if (RequiresSeparator(t1.Kind, t2.Kind))
                {
                    foreach (var separator in GetSeparators())
                    {
                        yield return (t1.Kind, t1.Text, separator.Kind, separator.Text, t2.Kind,
                            t2.Text);
                    }
                }
            }
        }
    }
}