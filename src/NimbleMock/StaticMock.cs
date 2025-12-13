using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using NimbleMock.Internal;
using NimbleMock.Exceptions;

namespace NimbleMock;

/// <summary>
/// A verifiable static mock instance that provides access to the wrapped static type and verification methods.
/// </summary>
/// <typeparam name="T">The static/sealed type being mocked.</typeparam>
public sealed class StaticMock<T> where T : class
{
    private readonly StaticWrapper<T> _wrapper;
    
    internal StaticMock(StaticWrapper<T> wrapper)
    {
        _wrapper = wrapper;
    }
    
    /// <summary>
    /// Gets the wrapped static type accessor.
    /// </summary>
    public T Wrapped => throw new InvalidOperationException(
        "Static types cannot be accessed directly. Use the wrapper methods instead.");
    
    /// <summary>
    /// Starts verification for a static member call.
    /// </summary>
    /// <typeparam name="TResult">The return type of the member to verify.</typeparam>
    /// <param name="expr">An expression specifying the static member to verify.</param>
    /// <returns>A verify builder for configuring verification expectations.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public StaticVerifyBuilder<T> Verify<TResult>(Expression<Func<T, TResult>> expr)
        => new(_wrapper, MethodId.From<T>(expr));
}

