// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Drawing;
using static Interop;

namespace System.Windows.Forms
{
    internal static class PaintEventExtensions
    {
        /// <summary>
        ///  Convert the <paramref name="deviceContext"/> into a <see cref="Graphics"/> object if possible.
        /// </summary>
        /// <param name="create">
        ///  Will create the <see cref="Graphics"/> if possible and it is not already created.
        /// </param>
        internal static Graphics? TryGetGraphics(this IDeviceContext deviceContext, bool create = false)
            => deviceContext switch
            {
                Graphics graphics => graphics,
                IGraphicsHdcProvider provider => provider.GetGraphics(create),
                _ => AssertNoGraphics(create)
            };

        private static Graphics? AssertNoGraphics(bool create)
        {
            Debug.Assert(!create, "Couldn't get Graphics");
            return null;
        }
    }
}
