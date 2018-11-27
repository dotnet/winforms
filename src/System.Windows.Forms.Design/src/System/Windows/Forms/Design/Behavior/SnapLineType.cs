// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.Design.Behavior
{
    /// <summary>
    ///     Describes a type of SnapLine.  These are used by the SnapLine hueristic to
    ///     determine which like-SnapLines can snap to one another.
    /// </summary>
    public enum SnapLineType
    {
        Top,

        Bottom,

        Left,

        Right,

        Horizontal,

        Vertical,

        Baseline
    }
}
