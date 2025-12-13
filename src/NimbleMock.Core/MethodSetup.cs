using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace NimbleMock;

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

