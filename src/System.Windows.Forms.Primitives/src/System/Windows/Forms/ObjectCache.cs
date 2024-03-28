// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <summary>
///  Light weight multithreaded fixed size cache class.
/// </summary>
internal abstract class ObjectCache<T> : IDisposable where T : class
{
    private readonly T?[] _itemsCache;

    /// <summary>
    ///  Create a cache with space for the specified number of items.
    /// </summary>
    public ObjectCache(int cacheSpace)
    {
        if (cacheSpace < 1)
            cacheSpace = Environment.Is64BitProcess ? 64 : 32;
        _itemsCache = new T[cacheSpace];
    }

    /// <summary>
    ///  Get an item from the cache or create one if none are available.
    /// </summary>
    protected T Acquire()
    {
        T? item;

        for (int i = 0; i < _itemsCache.Length; i++)
        {
            item = Interlocked.Exchange(ref _itemsCache[i], null);
            if (item is not null && Accept(item))
            {
                return item;
            }
        }

        return Create();
    }

    /// <summary>
    ///  Release an item back to the cache, disposing if no room is available.
    /// </summary>
    protected void Release(T item)
    {
        T? temp = item;

        for (int i = 0; i < _itemsCache.Length; i++)
        {
            temp = Interlocked.Exchange(ref _itemsCache[i], temp);
            if (temp is null)
                return;
        }

        (temp as IDisposable)?.Dispose();
    }

    protected abstract bool Accept(T item);
    protected abstract T Create();

    public void Dispose() => Dispose(disposing: true);

    protected virtual void Dispose(bool disposing)
    {
        if (disposing)
        {
            for (int i = 0; i < _itemsCache.Length; i++)
            {
                (_itemsCache[i] as IDisposable)?.Dispose();
                _itemsCache[i] = null;
            }
        }
    }
}
