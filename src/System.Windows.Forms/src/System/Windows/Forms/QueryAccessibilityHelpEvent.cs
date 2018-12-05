// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.



namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    using System.Drawing;
    using System.ComponentModel;
    using System.Windows.Forms;
    using Microsoft.Win32;

    /// <devdoc>
    ///     The QueryAccessibilityHelpEventArgs is fired when AccessibleObject
    ///     is providing help to accessibility applications.    
    /// </devdoc>
    [System.Runtime.InteropServices.ComVisible(true)]
    public class QueryAccessibilityHelpEventArgs  : EventArgs {
        
        private string helpNamespace;
        private string helpString;
        private string helpKeyword;
        
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public QueryAccessibilityHelpEventArgs() {
        }
        
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public QueryAccessibilityHelpEventArgs(string helpNamespace, string helpString, string helpKeyword) {
            this.helpNamespace = helpNamespace;
            this.helpString = helpString;
            this.helpKeyword = helpKeyword;
        }

        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public string HelpNamespace { 
            get {
                return helpNamespace;
            }
            set {
                helpNamespace = value;
            }
        }
        
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public string HelpString { 
            get {
                return helpString;
            }
            set {
                helpString = value;
            }
        }
        
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        public string HelpKeyword { 
            get {
                return helpKeyword;
            }
            set {
                helpKeyword = value;
            }
        }
    }
}
