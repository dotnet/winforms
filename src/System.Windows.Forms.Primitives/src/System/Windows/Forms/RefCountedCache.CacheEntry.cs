// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

internal abstract partial class RefCountedCache<TObject, TCacheEntryData, TKey>
{
    /// <summary>
    ///  Cache entry that maintains the reference count, entry data, and basic cleanup logic.
    /// </summary>
    [DebuggerDisplay("{DebuggerDisplay}")]
    public abstract class CacheEntry : IDisposable
    {
        private readonly bool _cached;
        private int _refCount;

        public TCacheEntryData Data { get; private set; }

        public CacheEntry(TCacheEntryData data, bool cached)
        {
            Data = data;
            _cached = cached;
        }

        /// <summary>
        ///  Add a reference to this entry.
        /// </summary>
        public void AddRef() => Interlocked.Increment(ref _refCount);

        /// <summary>
        ///  Current reference count for this entry.
        /// </summary>
        public int RefCount => _refCount;

        /// <summary>
        ///  Removes a reference to this entry.
        /// </summary>
        /// <remarks>
        ///  <para>
        ///   This will dispose of the entry when the ref count reaches zero- if the entry isn't actually
        ///   cached <see cref="_cached"/>.
        ///  </para>
        /// </remarks>
        public virtual void RemoveRef()
        {
            int refCount = Interlocked.Decrement(ref _refCount);

            // Did we over dispose??
            Debug.Assert(refCount >= 0);

            if (!_cached && refCount == 0)
            {
                // If this entry wasn't actually cached, we need to clean ourselves up when we're unreferenced.
                // (This happens when there isn't enough room in the cache.)
                Dispose(disposing: true);
            }
        }

        /// <summary>
        ///  Implement this to provide the target object for users.
        /// </summary>
        public abstract TObject Object { get; }

        public Scope CreateScope() => new(this);

        private string DebuggerDisplay => $"Object: {Object} RefCount: {RefCount}";

        /// <summary>
        ///  By default we dispose of <see cref="Data"/> and <see cref="Object"/> if they implement
        ///  <see cref="IDisposable" />. Override to provide custom cleanup logic.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                var disposable = Object as IDisposable;
                disposable?.Dispose();
                disposable = Data as IDisposable;
                disposable?.Dispose();
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
