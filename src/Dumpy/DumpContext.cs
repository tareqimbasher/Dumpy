using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;

namespace Dumpy;

/// <summary>
/// Tracks traversal state for dumping: current depth and visited references for loop detection.
/// Uses AsyncLocal to scope to the current logical call context.
/// </summary>
public static class DumpContext
{
    private sealed class State
    {
        // ReSharper disable once MemberHidesStaticFromOuterClass
        public int Depth;
        public readonly HashSet<object> Visited = new(HashSetCapacity, ReferenceEqualityComparer.Instance);
    }

    private const int HashSetCapacity = 64;
    private static readonly AsyncLocal<State?> _state = new();

    public static void Reset()
    {
        _state.Value = new State();
    }

    public static void Clear()
    {
        _state.Value = null;
    }

    public static int Depth => _state.Value?.Depth ?? 0;

    public static bool IsActive => _state.Value != null;

    public static void Enter(object? value, Type valueType)
    {
        var state = _state.Value ??= new State();
        state.Depth++;
        if (value != null && !valueType.IsValueType)
        {
            state.Visited.Add(value);
        }
    }

    public static void Exit(object? value, Type valueType)
    {
        var state = _state.Value;
        if (state == null) return;
        if (state.Depth > 0) state.Depth--;
        if (value != null && !valueType.IsValueType)
        {
            state.Visited.Remove(value);
        }
    }

    public static bool IsVisited(object? value, Type valueType)
    {
        var state = _state.Value;
        if (state == null || value == null || valueType.IsValueType) return false;
        return state.Visited.Contains(value);
    }
}

internal sealed class ReferenceEqualityComparer : IEqualityComparer<object?>
{
    public static readonly ReferenceEqualityComparer Instance = new();
    private ReferenceEqualityComparer() { }
    public new bool Equals(object? x, object? y) => ReferenceEquals(x, y);
    
    // Depending on target framework, RuntimeHelpers.GetHashCode might not be annotated
    // with the proper nullability attribute. We'll suppress any warning that might
    // result.
    public int GetHashCode(object? obj) => RuntimeHelpers.GetHashCode(obj!);
}