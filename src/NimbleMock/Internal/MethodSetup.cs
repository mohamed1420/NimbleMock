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
    /// <summary>
    /// Gets the method identifier for this setup.
    /// </summary>
    public readonly MethodId Method;
    
    /// <summary>
    /// Gets the return value configured for this setup.
    /// </summary>
    public readonly object? ReturnValue;
    
    /// <summary>
    /// Gets a value indicating whether this setup is for a partial mock.
    /// </summary>
    public readonly bool IsPartial;
    
    /// <summary>
    /// Gets a value indicating whether the return value is an exception.
    /// </summary>
    public readonly bool IsException;
    
    /// <summary>
    /// Initializes a new instance of the MethodSetup structure.
    /// </summary>
    /// <param name="method">The method identifier.</param>
    /// <param name="value">The return value or exception.</param>
    /// <param name="isPartial">Whether this setup is for a partial mock.</param>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public MethodSetup(MethodId method, object? value, bool isPartial = false)
    {
        Method = method;
        ReturnValue = value;
        IsPartial = isPartial;
        IsException = value is Exception;
    }
}

