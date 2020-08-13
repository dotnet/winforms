// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms
{
    internal static partial class GdiCache
    {
        /// <summary>
        ///  Scope that creates a wrapping <see cref="Drawing.Graphics"/> for a <see cref="ScreenDcCache.ScreenDcScope"/>
        ///  and manages disposal of the <see cref="Drawing.Graphics"/> and the scope.
        /// </summary>
#if DEBUG
        internal class ScreenGraphicsScope : DisposalTracking.Tracker, IDisposable
#else
        internal readonly ref struct ScreenGraphicsScope
#endif
        {
            private readonly ScreenDcCache.ScreenDcScope _dcScope;
            public Graphics Graphics { get; }

            public ScreenGraphicsScope(ref ScreenDcCache.ScreenDcScope scope)
            {
                _dcScope = scope;
                Graphics = scope.HDC.CreateGraphics();
            }

            public static implicit operator Graphics(in ScreenGraphicsScope scope) => scope.Graphics;

            public void Dispose()
            {
                Graphics?.Dispose();
                _dcScope.Dispose();
                DisposalTracking.SuppressFinalize(this!);
            }
        }
    }
}
