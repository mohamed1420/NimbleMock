using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using NimbleMock.Internal;
using NimbleMock.Exceptions;

namespace NimbleMock;

/// <summary>
/// A builder for verifying method call expectations.
/// </summary>
/// <typeparam name="T">The interface type being mocked.</typeparam>
public ref struct VerifyBuilder<T> where T : class
{
    private readonly MockInstance<T> _instance;
    private readonly MethodId _methodId;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal VerifyBuilder(MockInstance<T> instance, MethodId methodId)
    {
        _instance = instance;
        _methodId = methodId;
    }
    
    /// <summary>
    /// Verifies that the method was called exactly once.
    /// </summary>
    /// <exception cref="Exceptions.VerificationException">Thrown when the method was not called exactly once.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Once()
    {
        var count = _instance.GetCallCount(_methodId);
        if (count != 1)
            throw new VerificationException(
                $"Expected 1 call, got {count}");
        _instance.MarkAsVerified(_methodId);
    }
    
    /// <summary>
    /// Verifies that the method was called the specified number of times.
    /// </summary>
    /// <param name="expected">The expected number of calls.</param>
    /// <exception cref="Exceptions.VerificationException">Thrown when the method was not called the expected number of times.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Times(int expected)
    {
        var count = _instance.GetCallCount(_methodId);
        if (count != expected)
            throw new VerificationException(
                $"Expected {expected} calls, got {count}");
        _instance.MarkAsVerified(_methodId);
    }
    
    /// <summary>
    /// Verifies that the method was never called.
    /// </summary>
    /// <exception cref="Exceptions.VerificationException">Thrown when the method was called.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Never()
    {
        var count = _instance.GetCallCount(_methodId);
        if (count != 0)
            throw new VerificationException(
                $"Expected 0 calls, got {count}");
        _instance.MarkAsVerified(_methodId);
    }
    
    /// <summary>
    /// Verifies that the method was called at least the specified number of times.
    /// </summary>
    /// <param name="minimum">The minimum expected number of calls.</param>
    /// <exception cref="Exceptions.VerificationException">Thrown when the method was called fewer times than the minimum.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AtLeast(int minimum)
    {
        var count = _instance.GetCallCount(_methodId);
        if (count < minimum)
            throw new VerificationException(
                $"Expected at least {minimum} calls, got {count}");
        _instance.MarkAsVerified(_methodId);
    }
    
    /// <summary>
    /// Starts verification of method arguments at the specified parameter position.
    /// </summary>
    /// <typeparam name="TArg">The type of the argument to verify.</typeparam>
    /// <param name="position">The zero-based position of the parameter to verify. Defaults to 0.</param>
    /// <returns>An argument verify builder for configuring argument verification.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ArgVerifyBuilder<T, TArg> WithArg<TArg>(int position = 0)
        => new(_instance, _methodId, position);
}

