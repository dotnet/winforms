// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms
{
    /// <summary>
    ///  This class contains the information a user needs to paint the ToolTip.
    /// </summary>
    public class PopupEventArgs : CancelEventArgs
    {
        /// <summary>
        ///  Creates a new PopupEventArgs with the given parameters.
        /// </summary>
        public PopupEventArgs(IWin32Window associatedWindow, Control associatedControl, bool isBalloon, Size size)
        {
            AssociatedWindow = associatedWindow;
            AssociatedControl = associatedControl;
            ToolTipSize = size;
            IsBalloon = isBalloon;
        }

        /// <summary>
        ///  The Associated Window for which the tooltip is being painted.
        /// </summary>
        public IWin32Window AssociatedWindow { get; }

        /// <summary>
        ///  The control for which the tooltip is being painted.
        /// </summary>
        public Control AssociatedControl { get; }

        /// <summary>
        ///  The rectangle outlining the area in which the painting should be done.
        /// </summary>
        public Size ToolTipSize { get; set; }

        /// <summary>
        ///  Whether the tooltip is Ballooned.
        /// </summary>
        public bool IsBalloon { get; }
    }
}
