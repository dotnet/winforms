// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Drawing;
using System.Threading;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Thread safe collection of screen device contexts.
    /// </summary>
    /// <remarks>
    ///  This caching counts on consumers not leaving the HDC in a dirty state. There is a signficant overhead to
    ///  saving and restoring the state which would make this cache much less impactful. If we can't have confidence
    ///  the DC state is being restored the best option may be to simply create a brand new screen DC every time we
    ///  need one. That said, these DCs are safe to use across threads, as the creating thread is left running.
    ///
    ///  Creating a screen DC from scratch and deleting it takes about 11us. Saving and restoring takes about half
    ///  that time. Jumping to the other thread to get a safe DC when we've run out of cached items is roughly a 10us
    ///  extra cost. Renting an existing DC from the cache and returning it is on the order of 20_ns_. If we were
    ///  forced to save state we'd be taking about 9us renting and returning cached DCs- which would take around
    ///  20 rentals to break even with simply creating and tossing away a brand new DC.
    /// </remarks>
    internal sealed partial class ScreenDcCache : IDisposable
    {
        private readonly IntPtr[] _itemsCache;
        private readonly Thread _thread;
        private readonly ThreadWorker _worker;
        private readonly CancellationTokenSource _tokenSource = new CancellationTokenSource();

        /// <summary>
        ///  Create a cache with space for the specified number of HDCs.
        /// </summary>
        public ScreenDcCache(int cacheSpace = 10)
        {
            _worker = new ThreadWorker(_tokenSource.Token);

            // We need a thread that doesn't ever finish to create HDCs on so they'll stay valid for all threads
            // in the process for the life of the process.

            _thread = new Thread(_worker.Start)
            {
                Name = "WinForms Background Worker",
                IsBackground = true
            };
            _thread.Start();

            Debug.Assert(cacheSpace >= 0);

            _itemsCache = new IntPtr[cacheSpace];

            // Create an initial stash of screen dc's
            _worker.QueueAndWaitForCompletion(() =>
            {
                int max = Math.Min(cacheSpace, 5);
                for (int i = 0; i < max; i++)
                {
                    Gdi32.HDC hdc = Gdi32.CreateCompatibleDC(default);
                    _itemsCache[i] = (IntPtr)hdc;
                }
            });
        }

        /// <summary>
        ///  Get a DC from the cache or create one if none are available.
        /// </summary>
        public ScreenDcScope Acquire()
        {
            IntPtr item;

            for (int i = 0; i < _itemsCache.Length; i++)
            {
                item = Interlocked.Exchange(ref _itemsCache[i], IntPtr.Zero);
                if (item != IntPtr.Zero)
                {
                    return new ScreenDcScope(this, (Gdi32.HDC)item);
                }
            }

            // Didn't find anything in the cache, create a new HDC
            return CreateNew();
        }

        private ScreenDcScope CreateNew()
        {
            Gdi32.HDC newDc = default;
            _worker.QueueAndWaitForCompletion(() => newDc = Gdi32.CreateCompatibleDC(default));
            return new ScreenDcScope(this, newDc);
        }

        /// <summary>
        ///  Release an item back to the cache, disposing if no room is available.
        /// </summary>
        private void Release(Gdi32.HDC hdc)
        {
            if (hdc.IsNull)
                throw new ArgumentNullException(nameof(hdc));

            ValidateHdc(hdc);

            IntPtr temp = (IntPtr)hdc;

            for (int i = 0; i < _itemsCache.Length; i++)
            {
                // Flip with the array until we get back an empty slot
                temp = Interlocked.Exchange(ref _itemsCache[i], temp);
                if (temp == IntPtr.Zero)
                {
                    return;
                }
            }

            // Too many to store, delete the last item we swapped.
            Gdi32.DeleteDC((Gdi32.HDC)temp);
        }

        public void Dispose()
        {
            for (int i = 0; i < _itemsCache.Length; i++)
            {
                IntPtr hdc = _itemsCache[i];
                if (hdc != IntPtr.Zero)
                {
                    Gdi32.DeleteDC((Gdi32.HDC)hdc);
                }
            }

            _tokenSource.Cancel();
            _tokenSource.Dispose();
        }

        [Conditional("DEBUG")]
        private void ValidateHdc(Gdi32.HDC hdc)
        {
            // A few sanity checks against the HDC to see if it was left in a dirty state

            Gdi32.HRGN hrgn = Gdi32.CreateRectRgn(0, 0, 0, 0);
            Debug.Assert(Gdi32.GetClipRgn(hdc, hrgn) == 0, "Should not have a clipping region");
            Gdi32.DeleteObject(hrgn);

            Gdi32.GetViewportOrgEx(hdc, out Point point);
            Debug.Assert(point.IsEmpty, "Viewport origin shouldn't be shifted");
            Debug.Assert(Gdi32.GetMapMode(hdc) == Gdi32.MM.TEXT);
        }
    }
}
