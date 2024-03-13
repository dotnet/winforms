// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

internal abstract partial class RefCountedCache<TObject, TCacheEntryData, TKey>
{
    /// <summary>
    ///  Disposable struct that manages reference counting of <see cref="CacheEntry"/>.
    /// </summary>
#if DEBUG
    internal class Scope : DisposalTracking.Tracker, IDisposable
#else
    internal readonly ref struct Scope
#endif
    {
        private readonly TObject _object;
        private CacheEntry Entry { get; }

        /// <summary>
        ///  Constructor to hold an uncached object. Used to wrap something not coming from the cache in a scope
        ///  so it can be abstracted for the end users of a given API.
        ///
        ///  See <see cref="GdiPlusCache.GetSolidBrushScope(Drawing.Color)"/> for an example.
        /// </summary>
        /// <remarks>
        ///  <para>
        ///   Currently we don't dispose the <paramref name="object"/> as we don't need to in our usages. If this
        ///   becomes necessary we can add a bool to track whether or not we should dispose it.
        ///  </para>
        /// </remarks>
        public Scope(TObject @object)
        {
            Entry = default!;
            _object = @object;
        }

        public Scope(CacheEntry entry)
        {
            Debug.Assert(entry is not null);
            _object = default!;
            Entry = entry;
            Entry.AddRef();
        }

        [MaybeNull]
        public TCacheEntryData Data => Entry is null ? default : Entry.Data;
        public TObject Object => this;
        public int RefCount => Entry?.RefCount ?? -1;

        /// <summary>
        ///  Implicit conversion to the "target" type, i.e. <typeparamref name="TObject"/>.
        /// </summary>
        /// <remarks>
        ///  <para>
        ///   This is somewhat dangerous as implicit casting in the using statement will leak the scope. Not doing
        ///   this, however, makes usage with APIs difficult. We track in DEBUG to catch misuse as a mitigation.
        ///  </para>
        /// </remarks>
        public static implicit operator TObject(in Scope scope)
        {
#if DEBUG
            // In DEBUG the scope is a class and we create "default" scopes in some cases.
            if (scope is null)
            {
                return default!;
            }
#endif

            CacheEntry entry = scope.Entry;
            return entry is null ? scope._object : entry.Object;
        }

        public void Dispose()
        {
            Entry?.RemoveRef();
            DisposalTracking.SuppressFinalize(this!);
        }

#if DEBUG
        // Only need to define this constructor when we are a class
        internal Scope()
        {
            Entry = default!;
            _object = default!;
        }
#endif
    }
}
