using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using NimbleMock.Internal;
using NimbleMock.Exceptions;

namespace NimbleMock;

/// <summary>
/// A verifiable mock instance that provides access to the mocked object and verification methods.
/// </summary>
/// <typeparam name="T">The interface type being mocked.</typeparam>
public sealed class VerifiableMock<T> where T : class
{
    private readonly MockInstance<T> _instance;
    
    internal VerifiableMock(MockInstance<T> instance)
    {
        _instance = instance;
        Object = instance.Proxy;
    }
    
    /// <summary>
    /// Gets the mocked object instance.
    /// </summary>
    public T Object { get; }
    
    /// <summary>
    /// Starts verification for a method call.
    /// </summary>
    /// <typeparam name="TResult">The return type of the method to verify.</typeparam>
    /// <param name="expr">An expression specifying the method to verify.</param>
    /// <returns>A verify builder for configuring verification expectations.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public VerifyBuilder<T> Verify<TResult>(Expression<Func<T, TResult>> expr)
        => new(_instance, MethodId.From<T>(expr));
    
    /// <summary>
    /// Verifies that no methods other than those explicitly verified were called.
    /// Throws <see cref="Exceptions.VerificationException"/> if any unverified calls were made.
    /// </summary>
    /// <exception cref="Exceptions.VerificationException">Thrown when unverified calls were made.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void VerifyNoOtherCalls()
    {
        var unverified = _instance.GetUnverifiedCalls();
        if (unverified.Length > 0)
            throw new VerificationException(
                $"Unexpected calls: {string.Join(", ", unverified)}");
    }
}

