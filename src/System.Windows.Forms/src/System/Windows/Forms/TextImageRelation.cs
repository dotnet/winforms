// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    ///  Defined in such a way that you can cast the relation to an AnchorStyle and
    ///  the direction of the AnchorStyle points to where the image goes.
    ///  (e.g., (AnchorStyle)ImageBeforeText -> Left))
    /// </summary>
    public enum TextImageRelation
    {
        Overlay = AnchorStyles.None,
        ImageBeforeText = AnchorStyles.Left,
        TextBeforeImage = AnchorStyles.Right,
        ImageAboveText = AnchorStyles.Top,
        TextAboveImage = AnchorStyles.Bottom
    }
}
