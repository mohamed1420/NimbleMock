using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace NimbleMock;

public ref struct MockBuilder<T> where T : class
{
    private readonly MethodSetup[] _setups;
    private int _count;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal MockBuilder(MethodSetup[] buffer)
    {
        _setups = buffer;
        _count = 0;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public MockBuilder<T> Setup<TResult>(
        Expression<Func<T, TResult>> expr, 
        TResult value)
    {
        _setups[_count++] = new MethodSetup(
            MethodId.From<T>(expr),
            value!);
        return this;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public MockBuilder<T> SetupAsync<TResult>(
        Expression<Func<T, Task<TResult>>> expr,
        TResult value)
    {
        _setups[_count++] = new MethodSetup(
            MethodId.From<T>(expr),
            Task.FromResult(value));
        return this;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public MockBuilder<T> SetupAsync<TResult>(
        Expression<Func<T, ValueTask<TResult>>> expr,
        TResult value)
    {
        _setups[_count++] = new MethodSetup(
            MethodId.From<T>(expr),
            new ValueTask<TResult>(value));
        return this;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public MockBuilder<T> Throws<TException>(
        Expression<Func<T, object>> expr,
        TException exception) where TException : Exception
    {
        _setups[_count++] = new MethodSetup(
            MethodId.From<T>(expr),
            exception);
        return this;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public VerifiableMock<T> Build()
    {
        var setups = new MethodSetup[_count];
        Array.Copy(_setups, setups, _count);
        return MockProxy<T>.Create(setups);
    }
}

