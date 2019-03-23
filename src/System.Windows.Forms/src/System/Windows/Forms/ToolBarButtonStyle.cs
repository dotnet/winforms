// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    public enum ToolBarButtonStyle
    {
        /// <devdoc>
        /// A standard, three-dimensional button.
        /// </devdoc>
        PushButton = 1,

        /// <devdoc>
        /// A toggle button that appears sunken when clicked and retains the
        /// sunken appearance until clicked again.
        /// </devdoc>
        ToggleButton = 2,

        /// <devdoc>
        /// A space or line between toolbar buttons. The appearance depends on
        /// the value of the <see cref='System.Windows.Forms.ToolBar.Appearance'/>
        /// property.
        /// </devdoc>
        Separator = 3,

        /// <devdoc>
        /// A drop down control that displays a menu or other window when
        /// clicked.
        /// </devdoc>
        DropDownButton = 4,
    }
}
