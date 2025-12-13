using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace NimbleMock;

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
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Once()
    {
        var count = _instance.GetCallCount(_methodId);
        if (count != 1)
            throw new VerificationException(
                $"Expected 1 call, got {count}");
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Times(int expected)
    {
        var count = _instance.GetCallCount(_methodId);
        if (count != expected)
            throw new VerificationException(
                $"Expected {expected} calls, got {count}");
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Never()
    {
        var count = _instance.GetCallCount(_methodId);
        if (count != 0)
            throw new VerificationException(
                $"Expected 0 calls, got {count}");
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void AtLeast(int minimum)
    {
        var count = _instance.GetCallCount(_methodId);
        if (count < minimum)
            throw new VerificationException(
                $"Expected at least {minimum} calls, got {count}");
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ArgVerifyBuilder<T, TArg> WithArg<TArg>(int position = 0)
        => new(_instance, _methodId, position);
}

