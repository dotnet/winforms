// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    public enum ToolBarButtonStyle
    {
        /// <summary>
        ///  A standard, three-dimensional button.
        /// </summary>
        PushButton = 1,

        /// <summary>
        ///  A toggle button that appears sunken when clicked and retains the
        ///  sunken appearance until clicked again.
        /// </summary>
        ToggleButton = 2,

        /// <summary>
        ///  A space or line between toolbar buttons. The appearance depends on
        ///  the value of the <see cref='ToolBar.Appearance'/>
        ///  property.
        /// </summary>
        Separator = 3,

        /// <summary>
        ///  A drop down control that displays a menu or other window when
        ///  clicked.
        /// </summary>
        DropDownButton = 4,
    }
}
