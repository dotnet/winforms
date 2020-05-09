// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    public sealed partial class ImageList
    {
        [Flags]
        private enum OriginalOptions
        {
            Default = 0x00,
            ImageStrip = 0x01,
            CustomTransparentColor = 0x02,
            OwnsImage = 0x04
        }
    }
}
