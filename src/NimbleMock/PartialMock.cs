using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using NimbleMock.Internal;

namespace NimbleMock;

/// <summary>
/// A builder for configuring partial mock method setups.
/// Only specified methods will be mocked; others will throw NotImplementedException.
/// </summary>
/// <typeparam name="T">The interface type being partially mocked.</typeparam>
public ref struct PartialMock<T> where T : class
{
    private readonly MethodSetup[] _setups;
    private int _count;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal PartialMock(MethodSetup[] buffer)
    {
        _setups = buffer;
        _count = 0;
    }
    
    /// <summary>
    /// Configures a method to be mocked, returning the specified value.
    /// </summary>
    /// <typeparam name="TResult">The return type of the method.</typeparam>
    /// <param name="expr">An expression specifying the method to configure.</param>
    /// <param name="value">The value to return when the method is called.</param>
    /// <returns>The partial mock builder for method chaining.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public PartialMock<T> Only<TResult>(
        Expression<Func<T, TResult>> expr,
        TResult value)
    {
        _setups[_count++] = new MethodSetup(
            MethodId.From<T>(expr),
            value!,
            isPartial: true);
        return this;
    }
    
    /// <summary>
    /// Builds the configured partial mock instance.
    /// </summary>
    /// <returns>A verifiable mock instance that can be used and verified.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public VerifiableMock<T> Build()
    {
        var setups = new MethodSetup[_count];
        Array.Copy(_setups, setups, _count);
        return MockProxy<T>.CreatePartial(setups);
    }
}

