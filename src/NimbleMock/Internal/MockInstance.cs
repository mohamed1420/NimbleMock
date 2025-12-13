using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NimbleMock.Internal;

/// <summary>
/// Internal mock instance that tracks method calls and setups.
/// </summary>
/// <typeparam name="T">The interface type being mocked.</typeparam>
public sealed class MockInstance<T> where T : class
{
    private MethodSetup[] _setups = new MethodSetup[32];
    private int _setupCount;
    private bool _isPartial;
    private readonly Dictionary<MethodId, int> _callCounts = new();
    private readonly Dictionary<MethodId, List<object?[]>> _callArgs = new();
    
    public T Proxy { get; set; } = null!;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Initialize(MethodSetup[] setups, bool isPartial)
    {
        if (setups.Length > _setups.Length)
            Array.Resize(ref _setups, setups.Length);
        
        Array.Copy(setups, _setups, setups.Length);
        _setupCount = setups.Length;
        _isPartial = isPartial;
        _callCounts.Clear();
        _callArgs.Clear();
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetSetup(MethodId id, out MethodSetup setup)
    {
        var span = _setups.AsSpan(0, _setupCount);
        for (int i = 0; i < span.Length; i++)
        {
            if (span[i].Method.Equals(id))
            {
                setup = span[i];
                return true;
            }
        }
        
        setup = default;
        return !_isPartial;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void RecordCall(MethodId id, object?[] args)
    {
        _callCounts.TryGetValue(id, out var count);
        _callCounts[id] = count + 1;
        
        if (!_callArgs.TryGetValue(id, out var argList))
        {
            argList = new List<object?[]>();
            _callArgs[id] = argList;
        }
        argList.Add(args);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetCallCount(MethodId id)
        => _callCounts.TryGetValue(id, out var count) ? count : 0;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public object?[][] GetCallArguments(MethodId id)
        => _callArgs.TryGetValue(id, out var args) 
            ? args.ToArray() 
            : Array.Empty<object?[]>();
    
    public string[] GetUnverifiedCalls()
    {
        var setupIds = new HashSet<MethodId>();
        for (int i = 0; i < _setupCount; i++)
            setupIds.Add(_setups[i].Method);
        
        return _callCounts.Keys
            .Where(id => !setupIds.Contains(id))
            .Select(id => id.ToString())
            .ToArray();
    }
}

