using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using NimbleMock.Internal;
using NimbleMock.Exceptions;

namespace NimbleMock;

/// <summary>
/// A builder for verifying static member call expectations.
/// </summary>
/// <typeparam name="T">The static/sealed type being mocked.</typeparam>
public ref struct StaticVerifyBuilder<T> where T : class
{
    private readonly StaticWrapper<T> _wrapper;
    private readonly MethodId _methodId;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal StaticVerifyBuilder(StaticWrapper<T> wrapper, MethodId methodId)
    {
        _wrapper = wrapper;
        _methodId = methodId;
    }
    
    /// <summary>
    /// Verifies that the static member was called exactly once.
    /// </summary>
    /// <exception cref="Exceptions.VerificationException">Thrown when the member was not called exactly once.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Once()
    {
        var count = _wrapper.GetCallCount(_methodId);
        if (count != 1)
            throw new VerificationException(
                $"Expected 1 call, got {count}");
    }
    
    /// <summary>
    /// Verifies that the static member was called the specified number of times.
    /// </summary>
    /// <param name="expected">The expected number of calls.</param>
    /// <exception cref="Exceptions.VerificationException">Thrown when the member was not called the expected number of times.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Times(int expected)
    {
        var count = _wrapper.GetCallCount(_methodId);
        if (count != expected)
            throw new VerificationException(
                $"Expected {expected} calls, got {count}");
    }
    
    /// <summary>
    /// Verifies that the static member was never called.
    /// </summary>
    /// <exception cref="Exceptions.VerificationException">Thrown when the member was called.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Never()
    {
        var count = _wrapper.GetCallCount(_methodId);
        if (count != 0)
            throw new VerificationException(
                $"Expected 0 calls, got {count}");
    }
    
    /// <summary>
    /// Verifies that the static member was called at least the specified number of times.
    /// </summary>
    /// <param name="minimum">The minimum expected number of calls.</param>
    /// <exception cref="Exceptions.VerificationException">Thrown when the member was called fewer times than the minimum.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AtLeast(int minimum)
    {
        var count = _wrapper.GetCallCount(_methodId);
        if (count < minimum)
            throw new VerificationException(
                $"Expected at least {minimum} calls, got {count}");
    }
}

