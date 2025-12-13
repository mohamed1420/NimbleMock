using System;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NimbleMock.Internal;

/// <summary>
/// Internal structure representing a unique method identifier.
/// </summary>
[StructLayout(LayoutKind.Sequential)]
public readonly struct MethodId : IEquatable<MethodId>
{
    private readonly int _hash;
    private readonly string _name;
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private MethodId(int hash, string name)
    {
        _hash = hash;
        _name = name;
    }
    
    /// <summary>
    /// Creates a MethodId from an expression representing a method call or property access.
    /// </summary>
    /// <typeparam name="T">The interface type containing the method or property.</typeparam>
    /// <param name="expr">The expression representing the method call or property access.</param>
    /// <returns>A MethodId uniquely identifying the method or property.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MethodId From<T>(Expression expr)
    {
        if (expr is not LambdaExpression lambda)
            throw new ArgumentException("Expression must be lambda expression");
        
        string memberName;
        int memberHash;
        
        if (lambda.Body is MethodCallExpression call)
        {
            memberName = call.Method.Name;
            memberHash = call.Method.GetHashCode();
        }
        else if (lambda.Body is MemberExpression member && member.Member != null)
        {
            memberName = member.Member.Name;
            memberHash = member.Member.GetHashCode();
        }
        else
        {
            throw new ArgumentException("Expression must be method call or property access");
        }
        
        var hash = unchecked((typeof(T).GetHashCode() * 397) ^ memberHash);
        
        return new MethodId(hash, memberName);
    }
    
    /// <summary>
    /// Creates a MethodId directly from a type and member name.
    /// Used internally for static/sealed type mocking.
    /// </summary>
    /// <typeparam name="T">The type containing the member.</typeparam>
    /// <param name="memberName">The name of the member.</param>
    /// <returns>A MethodId uniquely identifying the member.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static MethodId FromName<T>(string memberName)
    {
        var hash = unchecked((typeof(T).GetHashCode() * 397) ^ memberName.GetHashCode());
        return new MethodId(hash, memberName);
    }
    
    /// <summary>
    /// Determines whether the specified MethodId is equal to this instance.
    /// </summary>
    /// <param name="other">The MethodId to compare with this instance.</param>
    /// <returns>True if the specified MethodId is equal to this instance; otherwise, false.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool Equals(MethodId other) => _hash == other._hash;
    
    /// <summary>
    /// Returns the hash code for this instance.
    /// </summary>
    /// <returns>A hash code for this instance.</returns>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public override int GetHashCode() => _hash;
    
    /// <summary>
    /// Returns the string representation of this MethodId.
    /// </summary>
    /// <returns>The name of the method.</returns>
    public override string ToString() => _name;
}

