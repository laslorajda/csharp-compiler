﻿namespace Compiler.CodeAnalysis.Binding;

internal abstract class BoundNode
{
    internal abstract BoundNodeKind Kind { get; }
}