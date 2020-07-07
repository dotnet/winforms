// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using static Interop;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Scope for <see cref="PaintEventArgs"/> that prefers to avoid creating <see cref="System.Drawing.Graphics"/> if
    ///  possible to get HDC.
    /// </summary>
    internal readonly ref struct PaintEventHdcScope
    {
        internal Gdi32.HDC HDC { get; }
        private readonly DeviceContextHdcScope _scope;

        public PaintEventHdcScope(PaintEventArgs args)
        {
            if (!args.IsGraphicsCreated)
            {
                HDC = args.HDC;
                _scope = default;
            }
            else
            {
                _scope = new DeviceContextHdcScope(args.Graphics);
                HDC = _scope.HDC;
            }
        }

        public static implicit operator Gdi32.HDC(in PaintEventHdcScope scope) => scope.HDC;

        public void Dispose()
        {
            _scope.Dispose();
        }
    }
}
