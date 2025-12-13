using System;
using System.Runtime.CompilerServices;
using NimbleMock.Internal;
using NimbleMock.Exceptions;

namespace NimbleMock;

/// <summary>
/// A builder for verifying method call arguments.
/// </summary>
/// <typeparam name="T">The interface type being mocked.</typeparam>
/// <typeparam name="TArg">The type of the argument being verified.</typeparam>
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
    
    /// <summary>
    /// Verifies that at least one call had an argument matching the specified predicate.
    /// </summary>
    /// <param name="predicate">A function that returns true if the argument matches the verification criteria.</param>
    /// <exception cref="Exceptions.VerificationException">Thrown when no call matched the predicate.</exception>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void Matching(Func<TArg, bool> predicate)
    {
        var args = _instance.GetCallArguments(_methodId);
        var matched = false;
        
        foreach (var argSet in (object?[][])args)
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

