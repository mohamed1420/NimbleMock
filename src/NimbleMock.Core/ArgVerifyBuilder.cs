using System;
using System.Runtime.CompilerServices;

namespace NimbleMock;

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

