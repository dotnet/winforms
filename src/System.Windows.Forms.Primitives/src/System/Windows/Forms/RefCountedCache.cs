// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Cache that ref counts handed-out objects via "scopes" <see cref="Scope"/>.
    /// </summary>
    /// <typeparam name="TObject">
    ///  The target object the cache represents. If you're caching <see cref="System.Drawing.Pen"/> that would be the
    ///  type you would use here. <see cref="Scope"/> is implicitly convertable to this type.
    /// </typeparam>
    /// <typeparam name="TCacheEntryData">
    ///  The type of data to associate with a cache entry. For a simple cache this can be the same type as
    ///  <typeparamref name="TKey"/>.
    /// </typeparam>
    /// <typeparam name="TKey">
    ///  The type of key used to look up cache entries.
    /// </typeparam>
    internal abstract partial class RefCountedCache<TObject, TCacheEntryData, TKey> : IDisposable
    {
        private readonly SinglyLinkedList<CacheEntry> _list = new SinglyLinkedList<CacheEntry>();

        private readonly int _softLimit;
        private readonly int _hardLimit;

        // Retrieving any node takes at least 300ns (locking, complex matching, etc. can add more time). It can take
        // 5 to 10ns for every step through the linked list with a simple key match. Shifting cost isn't expensive
        // (as SinglyLinkedList is optimized for this). It costs around 30-50ns to move an object to the front of the
        // list. We'll try to move to the front of the list when we think we'll get significant improvements in future
        // accesses that make up for the cost of moving without unduly impacting other frequently accessed items.
        //
        // Doing this also has a "de-aging" effect as we cull from the back of the list when looking for new space.
        private const int MoveToFront = 10;

        public RefCountedCache(int softLimit = 20, int hardLimit = 40)
        {
            Debug.Assert(softLimit > 0 && hardLimit > 0);
            Debug.Assert(softLimit <= hardLimit);

            _softLimit = softLimit;
            _hardLimit = hardLimit;
        }

        /// <summary>
        ///  Override this to create a new <see cref="CacheEntry"/> for the given <paramref name="key"/>.
        /// </summary>
        /// <param name="cached">
        ///  True if the entry is actually kept in the cache. When the cache hits the hard limit entries aren't
        ///  kept in the cache and need to be cleaned up when the ref count drops to zero.
        /// </param>
        protected abstract CacheEntry CreateEntry(TKey key, bool cached);

        /// <summary>
        ///  Return true if the given <paramref name="key"/> matches the given <paramref name="entry"/>.
        /// </summary>
        protected abstract bool IsMatch(TKey key, CacheEntry entry);

        /// <summary>
        ///  Find or create the entry for <paramref name="key"/>.
        /// </summary>
        /// <remarks>
        ///  Override if you want to modify behavior or lock cache access.
        /// </remarks>
        public virtual Scope GetEntry(TKey key)
        {
            // NOTE: Measure carefully when changing logic in this method. Code has been optimized for performance.

            if (key is null)
                throw new ArgumentNullException(nameof(key));

            Scope scope;

            if (Find(key, out Scope foundScope))
            {
                scope = foundScope;
            }
            else
            {
                scope = Add(key);
            }

            return scope;

            [SkipLocalsInit]
            bool Find(TKey key, out Scope scope)
            {
                bool success = false;
                scope = default!;
                int position = MoveToFront;

                var enumerator = _list.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    var node = enumerator.Current;
                    var cacheEntry = node!.Value;

                    if (IsMatch(key, cacheEntry))
                    {
                        scope = new Scope(cacheEntry);
                        if (position < 0)
                        {
                            // Moving to the front as the cost of walking this far in outweighs the cost of moving
                            // the node to the front of the list.
                            enumerator.MoveCurrentToFront();
                        }
                        success = true;
                        break;
                    }

                    position--;
                }

                return success;
            }

            [SkipLocalsInit]
            Scope Add(TKey key)
            {
                Scope scope = default!;

                if (_list.Count >= _softLimit)
                {
                    // Try to free up space
                    Clean();
                }

                if (_list.Count < _hardLimit)
                {
                    // We've got space, add to the cache
                    var data = CreateEntry(key, cached: true);
                    _list.AddFirst(data);
                    scope = new Scope(data);
                }
                else
                {
                    scope = new Scope(CreateEntry(key, cached: false));
                }

                return scope;
            }

            void Clean()
            {
                // If the count is over the soft limit, try to get back under

                int overage = _list.Count - _softLimit;

                if (overage > 0)
                {
                    // Skip to the last part of the list and try to remove what we can

                    int skip = _list.Count - overage;
                    var enumerator = _list.GetEnumerator();
                    int removed = 0;

                    while (enumerator.MoveNext())
                    {
                        skip--;
                        if (skip < 0)
                        {
                            var node = enumerator.Current;
                            if (node!.Value.RefCount == 0)
                            {
                                enumerator.RemoveCurrent();
                                node.Value.Dispose();
                                removed++;
                            }
                        }
                    }

                    // All of the end of the list is in use? Are we leaking ref counts?
                    Debug.Assert(removed != 0 || _softLimit < 20);
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                var enumerator = _list.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    enumerator.Current!.Value.Dispose();
                    enumerator.RemoveCurrent();
                }
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
