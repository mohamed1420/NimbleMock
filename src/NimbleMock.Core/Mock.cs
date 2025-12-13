using System;
using System.Runtime.CompilerServices;

namespace NimbleMock;

public static class Mock
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static MockBuilder<T> Of<T>() where T : class
        => new(new MethodSetup[32]);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static PartialMock<T> Partial<T>() where T : class
        => new(new MethodSetup[8]);
}

