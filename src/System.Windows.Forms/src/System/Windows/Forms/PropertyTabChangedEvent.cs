// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;
    using System;
    using System.Drawing;
    using System.ComponentModel;
    using Microsoft.Win32;
    using System.Windows.Forms.Design;

    /// <devdoc>
    ///    <para>[To be supplied.]</para>
    /// </devdoc>
    [System.Runtime.InteropServices.ComVisible(true)]
    public class PropertyTabChangedEventArgs : EventArgs{
        
        private PropertyTab oldTab;
        private PropertyTab newTab;

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public PropertyTabChangedEventArgs(PropertyTab oldTab, PropertyTab newTab) {
            this.oldTab = oldTab;
            this.newTab = newTab;
        }
        
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public PropertyTab OldTab {
            get {
                return oldTab;
            }
        }
        
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public PropertyTab NewTab {
            get {
                return newTab;
            }
        }
    }
}
