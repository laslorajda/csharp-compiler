﻿using System.Collections.Immutable;
using System.Text;
using Compiler.CodeAnalysis.Text;

namespace MyCompiler.Tests;

internal sealed class AnnotatedText
{
    public string Text { get; }
    public ImmutableArray<TextSpan> Spans { get; }

    public AnnotatedText(string text, ImmutableArray<TextSpan> spans)
    {
        Text = text;
        Spans = spans;
    }

    public static AnnotatedText Parse(string text)
    {
        text = Unindent(text);
        var textBuilder = new StringBuilder();
        var spanBuilder = ImmutableArray.CreateBuilder<TextSpan>();
        var startStack = new Stack<int>();
        var position = 0;
        
        foreach (var character in text)
        {
            switch (character)
            {
                case '[':
                    startStack.Push(position);
                    break;
                case ']' when startStack.Count == 0:
                    throw new ArgumentException("Too many ']' in text", nameof(text));
                case ']':
                {
                    var start = startStack.Pop();
                    var span = TextSpan.FromBounds(start, position);
                    spanBuilder.Add(span);
                    break;
                }
                default:
                    position++;
                    textBuilder.Append(character);
                    break;
            }
        }

        if (startStack.Count != 0)
        {
            throw new ArgumentException("Too few ']' in text", nameof(text));
        }

        return new(textBuilder.ToString(), spanBuilder.ToImmutable());
    }
    
    private static string Unindent(string text)
    {
        var lines = UnindentLines(text);

        return string.Join(Environment.NewLine, lines);
    }

    public static List<string> UnindentLines(string text)
    {
        var lines = new List<string>();
        using var reader = new StringReader(text);
        while (reader.ReadLine() is { } line)
        {
            lines.Add(line);
        }
        
        var minIndentation = int.MaxValue;
        for (var i = 0; i < lines.Count; i++)
        {
            var line = lines[i];
            if (line.Trim().Length == 0)
            {
                lines[i] = string.Empty;
                continue;
            }

            var indentation = line.Length - line.TrimStart().Length;
            minIndentation = Math.Min(minIndentation, indentation);
        }

        for (var i = 0; i < lines.Count; i++)
        {
            if (lines[i].Length == 0)
            {
                continue;
            }

            lines[i] = lines[i][minIndentation..];
        }

        while (lines.Count > 0 && lines[0].Length == 0)
        {
            lines.RemoveAt(0);
        }

        while (lines.Count > 0 && lines[^1].Length ==0)
        {
            lines.RemoveAt(lines.Count - 1);
        }

        return lines;
    }
}