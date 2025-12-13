using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using NimbleMock.Internal;

namespace NimbleMock;

/// <summary>
/// Internal wrapper for static/sealed types that allows mocking.
/// </summary>
/// <typeparam name="T">The static/sealed type being wrapped.</typeparam>
internal sealed class StaticWrapper<T> where T : class
{
    private MethodSetup[] _setups = new MethodSetup[32];
    private int _setupCount;
    private readonly Dictionary<MethodId, int> _callCounts = new();
    private readonly Dictionary<MethodId, List<object?[]>> _callArgs = new();
    private readonly Dictionary<string, PropertyInfo> _staticProperties = new();
    private readonly Dictionary<string, MethodInfo> _staticMethods = new();
    
    public StaticWrapper()
    {
        var type = typeof(T);
        
        foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Static))
        {
            _staticProperties[prop.Name] = prop;
        }
        
        foreach (var method in type.GetMethods(BindingFlags.Public | BindingFlags.Static))
        {
            if (!method.IsSpecialName)
            {
                _staticMethods[method.Name] = method;
            }
        }
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Initialize(MethodSetup[] setups)
    {
        if (setups.Length > _setups.Length)
            Array.Resize(ref _setups, setups.Length);
        
        Array.Copy(setups, _setups, setups.Length);
        _setupCount = setups.Length;
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
        return false;
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
    
    public TResult GetPropertyValue<TResult>(string propertyName)
    {
        if (!_staticProperties.TryGetValue(propertyName, out var prop))
            throw new InvalidOperationException($"Static property {propertyName} not found on {typeof(T).Name}");
        
        var methodId = CreateMethodId(propertyName);
        
        RecordCall(methodId, Array.Empty<object?>());
        
        if (TryGetSetup(methodId, out var setup))
        {
            if (setup.IsException)
                throw (Exception)setup.ReturnValue!;
            
            return (TResult)setup.ReturnValue!;
        }
        
        return (TResult)prop.GetValue(null)!;
    }
    
    public TResult InvokeMethod<TResult>(string methodName, object?[] args)
    {
        if (!_staticMethods.TryGetValue(methodName, out var method))
            throw new InvalidOperationException($"Static method {methodName} not found on {typeof(T).Name}");
        
        var methodId = CreateMethodId(methodName);
        
        RecordCall(methodId, args);
        
        if (TryGetSetup(methodId, out var setup))
        {
            if (setup.IsException)
                throw (Exception)setup.ReturnValue!;
            
            return (TResult)setup.ReturnValue!;
        }
        
        return (TResult)method.Invoke(null, args)!;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static MethodId CreateMethodId(string memberName)
    {
        return MethodId.FromName<T>(memberName);
    }
}

