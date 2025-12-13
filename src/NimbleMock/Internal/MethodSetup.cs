using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NimbleMock.Internal;

/// <summary>
/// Internal structure representing a method setup configuration.
/// </summary>
[StructLayout(LayoutKind.Auto)]
public readonly struct MethodSetup
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

