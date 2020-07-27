// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Saves and restores <see cref="Graphics.Clip"/>.
    /// </summary>
    internal readonly ref struct GraphicsClipScope
    {
        private readonly Region _originalClip;
        private readonly Graphics _graphics;
        public GraphicsClipScope(Graphics graphics)
        {
            _originalClip = graphics.Clip;
            _graphics = graphics;
        }
        public void Dispose()
        {
            _graphics.Clip = _originalClip;

            // The clip we got back is a copy and it gets copied again on the way in.
            _originalClip.Dispose();
        }
    }
}
