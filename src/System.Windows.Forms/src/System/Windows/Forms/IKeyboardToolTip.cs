// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {
    using System.Collections.Generic;
    using System.Drawing;

    internal interface IKeyboardToolTip  {
        /// <summary>
        /// Returns true if a keyboard ToolTip can be shown for the tool or its descendants at this moment.
        /// </summary>
        bool CanShowToolTipsNow();

        /// <summary>
        /// Returns the tool's native screen rectangle.
        /// </summary>
        Rectangle GetNativeScreenRectangle();

        /// <summary>
        /// Returns a list of neighboring tools' native screen rectangles.
        /// They are used to minimize the intersection between them and a keyboard ToolTip.
        /// </summary>
        IList<Rectangle> GetNeighboringToolsRectangles();

        /// <summary>
        /// Returns true if the tool is being hovered by a mouse pointer.
        /// </summary>
        bool IsHoveredWithMouse();

        /// <summary>
        /// Returns true if the tool has the Right-To-Left mode enabled
        /// </summary>
        bool HasRtlModeEnabled();

        /// <summary>
        /// Returns true if a keyboard ToolTip is allowed for this tool.
        /// </summary>
        bool AllowsToolTip();

        /// <summary>
        /// Returns the tool owner's native window.
        /// The tool can return its own native window if it exists.
        /// </summary>
        IWin32Window GetOwnerWindow();

        /// <summary>
        /// Notifies this tool that it was hooked to a keyboard ToolTip
        /// </summary>
        void OnHooked(ToolTip toolTip);

        /// <summary>
        /// Notifies this tool that it was unhooked from a keyboard ToolTip
        /// </summary>
        void OnUnhooked(ToolTip toolTip);

        /// <summary>
        /// Returns a caption set for this tool by the provided ToolTip
        /// </summary>
        string GetCaptionForTool(ToolTip toolTip);

        /// <summary>
        /// Returns false if this tool's own keyboard tooltip is not expected to be shown (e.g. it is just a container for other tools)
        /// </summary>
        bool ShowsOwnToolTip();

        /// <summary>
        /// Returns true when the tool is being tabbed to
        /// </summary>
        bool IsBeingTabbedTo();

        /// <summary>
        /// Returns false if the tool disables keyboard tooltips for it's children
        /// </summary>
        bool AllowsChildrenToShowToolTips();
    }
}