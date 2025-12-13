using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace NimbleMock;

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
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public VerifiableMock<T> Build()
    {
        var setups = new MethodSetup[_count];
        Array.Copy(_setups, setups, _count);
        return MockProxy<T>.CreatePartial(setups);
    }
}

