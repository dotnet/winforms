// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms 
{
    /// <devdoc>
    /// This class contains the information a user needs to paint the ToolTip.
    /// </devdoc>
    public class PopupEventArgs : CancelEventArgs 
    {
        /// <devdoc>
        /// Creates a new PopupEventArgs with the given parameters.
        /// </devdoc>
        public PopupEventArgs(IWin32Window associatedWindow, Control associatedControl, bool isBalloon, Size size) 
        {
            AssociatedWindow = associatedWindow;
            AssociatedControl = associatedControl;
            ToolTipSize = size;
            IsBalloon = isBalloon;
        }
        
        /// <devdoc>
        /// The Associated Window for which the tooltip is being painted. 
        /// </devdoc>
        public IWin32Window AssociatedWindow { get; }

        /// <devdoc>
        /// The control for which the tooltip is being painted. 
        /// </devdoc>
        public Control AssociatedControl { get; }
	
        /// <devdoc>
        /// The rectangle outlining the area in which the painting should be done.
        /// </devdoc>
        public Size ToolTipSize { get; set; }

        /// <devdoc>
        /// Whether the tooltip is Ballooned. 
        /// </devdoc>
        public bool IsBalloon { get; }
    }
}
