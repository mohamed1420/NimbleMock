using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace NimbleMock;

public sealed class VerifiableMock<T> where T : class
{
    private readonly MockInstance<T> _instance;
    
    internal VerifiableMock(MockInstance<T> instance)
    {
        _instance = instance;
        Object = instance.Proxy;
    }
    
    public T Object { get; }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public VerifyBuilder<T> Verify<TResult>(Expression<Func<T, TResult>> expr)
        => new(_instance, MethodId.From<T>(expr));
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void VerifyNoOtherCalls()
    {
        var unverified = _instance.GetUnverifiedCalls();
        if (unverified.Length > 0)
            throw new VerificationException(
                $"Unexpected calls: {string.Join(", ", unverified)}");
    }
}

