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
    private readonly HashSet<MethodId> _verifiedMethods = new();
    
    /// <summary>
    /// Gets or sets the proxy instance for this mock.
    /// </summary>
    public T Proxy { get; set; } = null!;
    
    /// <summary>
    /// Initializes the mock instance with the specified setups.
    /// </summary>
    /// <param name="setups">The method setups to configure.</param>
    /// <param name="isPartial">Whether this is a partial mock.</param>
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
        _verifiedMethods.Clear();
    }
    
    /// <summary>
    /// Attempts to get the setup for the specified method identifier.
    /// </summary>
    /// <param name="id">The method identifier.</param>
    /// <param name="setup">When this method returns, contains the setup if found.</param>
    /// <returns>True if a setup was found; otherwise, false.</returns>
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
    
    /// <summary>
    /// Records a method call with the specified arguments.
    /// </summary>
    /// <param name="id">The method identifier.</param>
    /// <param name="args">The method arguments.</param>
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
    
    /// <summary>
    /// Gets the number of times the specified method was called.
    /// </summary>
    /// <param name="id">The method identifier.</param>
    /// <returns>The number of times the method was called.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetCallCount(MethodId id)
        => _callCounts.TryGetValue(id, out var count) ? count : 0;
    
    /// <summary>
    /// Gets the arguments for all calls to the specified method.
    /// </summary>
    /// <param name="id">The method identifier.</param>
    /// <returns>An array of argument arrays, one for each call.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public object?[][] GetCallArguments(MethodId id)
        => _callArgs.TryGetValue(id, out var args) 
            ? args.ToArray() 
            : Array.Empty<object?[]>();
    
    /// <summary>
    /// Gets or sets a value indicating whether this is a partial mock.
    /// </summary>
    public bool IsPartial => _isPartial;
    
    /// <summary>
    /// Marks a method as verified.
    /// </summary>
    /// <param name="id">The method identifier.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void MarkAsVerified(MethodId id)
    {
        _verifiedMethods.Add(id);
    }
    
    /// <summary>
    /// Gets an array of method names that were called but not verified.
    /// </summary>
    /// <returns>An array of unverified method names.</returns>
    public string[] GetUnverifiedCalls()
    {
        return _callCounts.Keys
            .Where(id => !_verifiedMethods.Contains(id))
            .Select(id => id.ToString())
            .ToArray();
    }
}

