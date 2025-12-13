using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using NimbleMock.Internal;

namespace NimbleMock;

/// <summary>
/// A builder for configuring mock method setups.
/// </summary>
/// <typeparam name="T">The interface type being mocked.</typeparam>
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
    
    /// <summary>
    /// Configures a method to return the specified value.
    /// </summary>
    /// <typeparam name="TResult">The return type of the method.</typeparam>
    /// <param name="expr">An expression specifying the method to configure.</param>
    /// <param name="value">The value to return when the method is called.</param>
    /// <returns>The mock builder for method chaining.</returns>
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
    
    /// <summary>
    /// Configures an async method returning Task&lt;TResult&gt; to return the specified value.
    /// </summary>
    /// <typeparam name="TResult">The return type of the async method.</typeparam>
    /// <param name="expr">An expression specifying the async method to configure.</param>
    /// <param name="value">The value to return when the method is called.</param>
    /// <returns>The mock builder for method chaining.</returns>
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
    
    /// <summary>
    /// Configures an async method returning ValueTask&lt;TResult&gt; to return the specified value.
    /// </summary>
    /// <typeparam name="TResult">The return type of the async method.</typeparam>
    /// <param name="expr">An expression specifying the async method to configure.</param>
    /// <param name="value">The value to return when the method is called.</param>
    /// <returns>The mock builder for method chaining.</returns>
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
    
    /// <summary>
    /// Configures a method to throw the specified exception when called.
    /// </summary>
    /// <typeparam name="TException">The type of exception to throw.</typeparam>
    /// <param name="expr">An expression specifying the method to configure.</param>
    /// <param name="exception">The exception to throw when the method is called.</param>
    /// <returns>The mock builder for method chaining.</returns>
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
    
    /// <summary>
    /// Builds the configured mock instance.
    /// </summary>
    /// <returns>A verifiable mock instance that can be used and verified.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public VerifiableMock<T> Build()
    {
        var setups = new MethodSetup[_count];
        Array.Copy(_setups, setups, _count);
        return MockProxy<T>.Create(setups);
    }
}

