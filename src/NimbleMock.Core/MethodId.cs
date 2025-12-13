using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NimbleMock;

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
        
        var hash = unchecked((typeof(T).GetHashCode() * 397) ^ call.Method.GetHashCode());
        
        return new MethodId(hash, call.Method.Name);
    }
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(MethodId other) => _hash == other._hash;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() => _hash;
    
    public override string ToString() => _name;
}

