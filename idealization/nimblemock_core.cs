// NimbleMock.Core/NimbleMock.cs
using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NimbleMock;

public static class Mock
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MockBuilder<T> Of<T>() where T : class
        => new(stackalloc MethodSetup[32]);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PartialMock<T> Partial<T>() where T : class
        => new(stackalloc MethodSetup[8]);
}

public ref struct MockBuilder<T> where T : class
{
    private readonly Span<MethodSetup> _setups;
    private int _count;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal MockBuilder(Span<MethodSetup> buffer)
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
            MethodId.From(expr),
            value!);
        return this;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public MockBuilder<T> SetupAsync<TResult>(
        Expression<Func<T, Task<TResult>>> expr,
        TResult value)
    {
        _setups[_count++] = new MethodSetup(
            MethodId.From(expr),
            Task.FromResult(value));
        return this;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public MockBuilder<T> SetupAsync<TResult>(
        Expression<Func<T, ValueTask<TResult>>> expr,
        TResult value)
    {
        _setups[_count++] = new MethodSetup(
            MethodId.From(expr),
            new ValueTask<TResult>(value));
        return this;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public MockBuilder<T> Throws<TException>(
        Expression<Func<T, object>> expr,
        TException exception) where TException : Exception
    {
        _setups[_count++] = new MethodSetup(
            MethodId.From(expr),
            exception);
        return this;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public VerifiableMock<T> Build() 
        => MockProxy<T>.Create(_setups[.._count]);
}

public ref struct PartialMock<T> where T : class
{
    private readonly Span<MethodSetup> _setups;
    private int _count;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal PartialMock(Span<MethodSetup> buffer)
    {
        _setups = buffer;
        _count = 0;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public PartialMock<T> Only<TResult>(
        Expression<Func<T, TResult>> expr,
        TResult value)
    {
        _setups[_count++] = new MethodSetup(
            MethodId.From(expr),
            value!,
            isPartial: true);
        return this;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public VerifiableMock<T> Build() 
        => MockProxy<T>.CreatePartial(_setups[.._count]);
}

// NimbleMock.Core/VerifiableMock.cs
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
        => new(_instance, MethodId.From(expr));
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void VerifyNoOtherCalls()
    {
        var unverified = _instance.GetUnverifiedCalls();
        if (unverified.Length > 0)
            throw new VerificationException(
                $"Unexpected calls: {string.Join(", ", unverified)}");
    }
}

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

public ref struct ArgVerifyBuilder<T, TArg> where T : class
{
    private readonly MockInstance<T> _instance;
    private readonly MethodId _methodId;
    private readonly int _position;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal ArgVerifyBuilder(MockInstance<T> instance, MethodId methodId, int position)
    {
        _instance = instance;
        _methodId = methodId;
        _position = position;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Matching(Func<TArg, bool> predicate)
    {
        var args = _instance.GetCallArguments(_methodId);
        var matched = false;
        
        foreach (var argSet in args)
        {
            if (argSet.Length > _position && 
                argSet[_position] is TArg arg &&
                predicate(arg))
            {
                matched = true;
                break;
            }
        }
        
        if (!matched)
            throw new VerificationException(
                $"No call matched predicate at position {_position}");
    }
}

// NimbleMock.Core/MethodSetup.cs
[StructLayout(LayoutKind.Auto)]
internal readonly struct MethodSetup
{
    public readonly MethodId Method;
    public readonly object? ReturnValue;
    public readonly bool IsPartial;
    public readonly bool IsException;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public MethodSetup(MethodId method, object? value, bool isPartial = false)
    {
        Method = method;
        ReturnValue = value;
        IsPartial = isPartial;
        IsException = value is Exception;
    }
}

[StructLayout(LayoutKind.Sequential)]
internal readonly struct MethodId : IEquatable<MethodId>
{
    private readonly int _hash;
    private readonly string _name;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private MethodId(int hash, string name)
    {
        _hash = hash;
        _name = name;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MethodId From<T>(Expression expr)
    {
        if (expr is not LambdaExpression lambda ||
            lambda.Body is not MethodCallExpression call)
            throw new ArgumentException("Expression must be method call");
        
        var hash = HashCode.Combine(
            typeof(T).GetHashCode(),
            call.Method.GetHashCode());
        
        return new MethodId(hash, call.Method.Name);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(MethodId other) => _hash == other._hash;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() => _hash;
    
    public override string ToString() => _name;
}

// NimbleMock.Core/MockProxy.cs
internal static class MockProxy<T> where T : class
{
    private static readonly ObjectPool<MockInstance<T>> Pool = new();
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static VerifiableMock<T> Create(ReadOnlySpan<MethodSetup> setups)
    {
        var instance = Pool.Rent();
        instance.Initialize(setups, isPartial: false);
        return new VerifiableMock<T>(instance);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static VerifiableMock<T> CreatePartial(ReadOnlySpan<MethodSetup> setups)
    {
        var instance = Pool.Rent();
        instance.Initialize(setups, isPartial: true);
        return new VerifiableMock<T>(instance);
    }
}

// NimbleMock.Core/MockInstance.cs
internal sealed class MockInstance<T> where T : class
{
    private MethodSetup[] _setups = new MethodSetup[32];
    private int _setupCount;
    private bool _isPartial;
    private readonly Dictionary<MethodId, int> _callCounts = new();
    private readonly Dictionary<MethodId, List<object?[]>> _callArgs = new();
    
    public required T Proxy { get; init; }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Initialize(ReadOnlySpan<MethodSetup> setups, bool isPartial)
    {
        if (setups.Length > _setups.Length)
            Array.Resize(ref _setups, setups.Length);
        
        setups.CopyTo(_setups);
        _setupCount = setups.Length;
        _isPartial = isPartial;
        _callCounts.Clear();
        _callArgs.Clear();
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetSetup(MethodId id, out MethodSetup setup)
    {
        var span = _setups.AsSpan(0, _setupCount);
        for (int i = 0; i < span.Length; i++)
        {
            if (span[i].Method.Equals(id))
            {
                setup = span[i];
                return true;
            }
        }
        
        setup = default;
        return !_isPartial;
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void RecordCall(MethodId id, object?[] args)
    {
        _callCounts.TryGetValue(id, out var count);
        _callCounts[id] = count + 1;
        
        if (!_callArgs.TryGetValue(id, out var argList))
        {
            argList = new List<object?[]>();
            _callArgs[id] = argList;
        }
        argList.Add(args);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public int GetCallCount(MethodId id)
        => _callCounts.TryGetValue(id, out var count) ? count : 0;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ReadOnlySpan<object?[]> GetCallArguments(MethodId id)
        => _callArgs.TryGetValue(id, out var args) 
            ? CollectionsMarshal.AsSpan(args) 
            : ReadOnlySpan<object?[]>.Empty;
    
    public string[] GetUnverifiedCalls()
    {
        var setupIds = new HashSet<MethodId>();
        for (int i = 0; i < _setupCount; i++)
            setupIds.Add(_setups[i].Method);
        
        return _callCounts.Keys
            .Where(id => !setupIds.Contains(id))
            .Select(id => id.ToString())
            .ToArray();
    }
}

// NimbleMock.Core/ObjectPool.cs
internal sealed class ObjectPool<T> where T : class, new()
{
    private readonly T?[] _items = new T[Environment.ProcessorCount * 4];
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public T Rent()
    {
        var items = _items;
        for (int i = 0; i < items.Length; i++)
        {
            var item = items[i];
            if (item != null && 
                Interlocked.CompareExchange(ref items[i], null, item) == item)
                return item;
        }
        
        return new T();
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Return(T item)
    {
        var items = _items;
        for (int i = 0; i < items.Length; i++)
        {
            if (Interlocked.CompareExchange(ref items[i], item, null) == null)
                return;
        }
    }
}

// NimbleMock.Core/VerificationException.cs
public sealed class VerificationException : Exception
{
    public VerificationException(string message) : base(message) { }
}

// Usage Examples:

/* 
// Basic verification
var mock = Mock.Of<IUserRepository>()
    .Setup(x => x.GetById(1), new User { Id = 1 })
    .Build();

var user = mock.Object.GetById(1);
mock.Verify(x => x.GetById(1)).Once();

// Argument verification
var serviceMock = Mock.Of<IEmailService>()
    .SetupAsync(x => x.SendAsync(default!), true)
    .Build();

await serviceMock.Object.SendAsync("test@example.com");
serviceMock.Verify(x => x.SendAsync(default!))
    .WithArg<string>()
    .Matching(email => email.Contains("@"));

// Multiple calls verification
mock.Verify(x => x.GetById(1)).Times(3);
mock.Verify(x => x.Delete(2)).Never();

// No other calls verification
mock.VerifyNoOtherCalls();
*/