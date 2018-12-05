// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms 
{

    using System.Diagnostics;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using Microsoft.Win32;

    /// <devdoc>
    ///     This class contains the information a user needs to paint the ToolTip.
    /// </devdoc>
    public class PopupEventArgs : CancelEventArgs 
    {

        private IWin32Window associatedWindow;
        private Size size;
        private Control associatedControl;
        private bool isBalloon;

	        
        /// <devdoc>
        ///     Creates a new PopupEventArgs with the given parameters.
        /// </devdoc>
        public PopupEventArgs(IWin32Window associatedWindow, Control associatedControl, bool isBalloon, Size size) 
        {
            this.associatedWindow = associatedWindow;
            this.size = size;
            this.associatedControl = associatedControl;
            this.isBalloon = isBalloon;
            
        }
        
        /// <devdoc>
        ///     The Associated Window for which the tooltip is being painted. 
        /// </devdoc>
        public IWin32Window AssociatedWindow {
            get {
                return associatedWindow;
            }
        }

        /// <devdoc>
        ///     The control for which the tooltip is being painted. 
        /// </devdoc>
        public Control AssociatedControl {
            get {
                return associatedControl;
            }
            
        }

        /// <devdoc>
        ///     Whether the tooltip is Ballooned. 
        /// </devdoc>
        public bool IsBalloon {
            get {
                return isBalloon;
            }
            
        }
	
        /// <devdoc>
        ///     The rectangle outlining the area in which the painting should be done.
        /// </devdoc>
        public Size ToolTipSize {
            get {
                return size;
            }
            set {
                size = value;
            }
        }
    }
}



