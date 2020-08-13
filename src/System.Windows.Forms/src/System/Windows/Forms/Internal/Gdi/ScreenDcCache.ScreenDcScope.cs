// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using static Interop;

namespace System.Windows.Forms
{
    internal sealed partial class ScreenDcCache
    {
        /// <summary>
        ///  Scope to ensure return of the device context back to the cache.
        /// </summary>
#if DEBUG
        internal class ScreenDcScope : DisposalTracking.Tracker, IDisposable
#else
        internal readonly ref struct ScreenDcScope
#endif
        {
            public Gdi32.HDC HDC { get; }
            private readonly ScreenDcCache _cache;

            public ScreenDcScope(ScreenDcCache cache, Gdi32.HDC hdc)
            {
                _cache = cache;
                HDC = hdc;
            }

            public static implicit operator Gdi32.HDC(in ScreenDcScope scope) => scope.HDC;

            public void Dispose()
            {
                _cache.Release(HDC);
                DisposalTracking.SuppressFinalize(this!);
            }
        }
    }
}
