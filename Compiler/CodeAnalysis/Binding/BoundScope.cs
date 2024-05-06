using System.Collections.Immutable;

namespace Compiler.CodeAnalysis.Binding;

internal sealed class BoundScope
{
    private BoundScope? Parent { get; }
    private readonly Dictionary<string, VariableSymbol> _variables = new();

    public BoundScope(BoundScope? parent)
    {
        Parent = parent;
    }
    
    public bool TryDeclare(VariableSymbol variable) => _variables.TryAdd(variable.Name, variable);

    public bool TryLookup(string name, out VariableSymbol? variable)
    {
        if (_variables.TryGetValue(name, out variable))
        {
            return true;
        }

        return Parent != null && Parent.TryLookup(name, out variable);
    }

    public ImmutableArray<VariableSymbol> GetDeclaredVariables() => [.._variables.Values];
}