// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;

namespace System.Windows.Forms.Design;

internal partial class FlowLayoutPanelDesigner
{
    private struct ChildInfo
    {
        /// <summary>
        ///  Represents the bounds (including margins) of a child - used for hit testing.
        /// </summary>
        public Rectangle MarginBounds;

        /// <summary>
        ///  Bounds of the control -- used for drawing the IBar.
        /// </summary>
        public Rectangle ControlBounds;

        /// <summary>
        ///  Is this child in the selection collection?
        /// </summary>
        public bool InSelectionCollection;
    }
}
