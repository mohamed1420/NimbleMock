using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using NimbleMock.Internal;

namespace NimbleMock;

/// <summary>
/// A builder for configuring static/sealed type mocks.
/// </summary>
/// <typeparam name="T">The static/sealed type to mock.</typeparam>
public ref struct StaticMockBuilder<T> where T : class
{
    private readonly MethodSetup[] _setups;
    private int _count;
    private readonly StaticWrapper<T> _wrapper;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal StaticMockBuilder(MethodSetup[] buffer, StaticWrapper<T> wrapper)
    {
        _setups = buffer;
        _count = 0;
        _wrapper = wrapper;
    }
    
    /// <summary>
    /// Configures a static member to return the specified value.
    /// </summary>
    /// <typeparam name="TResult">The return type of the member.</typeparam>
    /// <param name="expr">An expression specifying the static member to configure.</param>
    /// <param name="value">The value to return when the member is accessed.</param>
    /// <returns>The static mock builder for method chaining.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public StaticMockBuilder<T> Returns<TResult>(
        Expression<Func<T, TResult>> expr, 
        TResult value)
    {
        _setups[_count++] = new MethodSetup(
            MethodId.From<T>(expr),
            value!);
        return this;
    }
    
    /// <summary>
    /// Builds the configured static mock instance.
    /// </summary>
    /// <returns>A static mock wrapper that can be used and verified.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public StaticMock<T> Build()
    {
        var setups = new MethodSetup[_count];
        Array.Copy(_setups, setups, _count);
        _wrapper.Initialize(setups);
        return new StaticMock<T>(_wrapper);
    }
}

