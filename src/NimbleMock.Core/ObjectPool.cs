using System;
using System.Runtime.CompilerServices;
using System.Threading;

namespace NimbleMock;

internal sealed class ObjectPool<T> where T : class
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
        
        return Activator.CreateInstance<T>();
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

