﻿using Compiler.CodeAnalysis.Text;

namespace Compiler.CodeAnalysis.Syntax;

public class SyntaxToken : SyntaxNode
{
    public override SyntaxKind Kind { get; }

    public SyntaxToken(SyntaxKind kind, object? value, string text, int position)
    {
        Kind = kind;
        Value = value;
        Text = text;
        Position = position;
    }
    
    public object? Value { get; }
    public string Text { get; }
    public int Position { get; }
    public override TextSpan Span => new(Position, Text.Length);
    public bool IsMissing => string.IsNullOrEmpty(Text);
}