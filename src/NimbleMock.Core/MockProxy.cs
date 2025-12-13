using System;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace NimbleMock;

internal static partial class MockProxy<T> where T : class
{
    private static readonly ObjectPool<MockInstance<T>> Pool = new();
    
    // Factory function that can be set by source generator
    // If null, will use reflection fallback
    // This field is initialized by source generator in static constructor
    #pragma warning disable CS0649
    private static Func<MockInstance<T>, T>? _factory;
    #pragma warning restore CS0649
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static T CreateProxy(MockInstance<T> instance)
    {
        // Try to use source-generated factory
        if (_factory != null)
            return _factory(instance);
        
        // Fallback to reflection if source generator hasn't generated the factory
        return CreateProxyReflection(instance);
    }
    
    private static T CreateProxyReflection(MockInstance<T> instance)
    {
        var typeName = $"MockProxy_{typeof(T).Name}";
        var assembly = typeof(T).Assembly;
        var proxyType = assembly.GetType(typeName) ?? 
                       AppDomain.CurrentDomain.GetAssemblies()
                           .SelectMany(a => a.GetTypes())
                           .FirstOrDefault(t => t.Name == typeName);
        
        if (proxyType == null)
        {
            throw new InvalidOperationException(
                $"Mock proxy for {typeof(T).Name} not found. Ensure NimbleMock.SourceGenerator is referenced and the type is mocked.");
        }
        
        var constructor = proxyType.GetConstructor(new[] { typeof(MockInstance<T>) });
        if (constructor == null)
        {
            throw new InvalidOperationException(
                $"Mock proxy {typeName} does not have a constructor accepting MockInstance<{typeof(T).Name}>.");
        }
        
        return (T)constructor.Invoke(new object[] { instance });
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static VerifiableMock<T> Create(MethodSetup[] setups)
    {
        var instance = Pool.Rent();
        instance.Initialize(setups, isPartial: false);
        instance.Proxy = CreateProxy(instance);
        return new VerifiableMock<T>(instance);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static VerifiableMock<T> CreatePartial(MethodSetup[] setups)
    {
        var instance = Pool.Rent();
        instance.Initialize(setups, isPartial: true);
        instance.Proxy = CreateProxy(instance);
        return new VerifiableMock<T>(instance);
    }
}

