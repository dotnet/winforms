// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <summary>
    ///  The QueryAccessibilityHelpEventArgs is fired when AccessibleObject is providing help
    ///  to accessibility applications.
    /// </summary>
    [ComVisible(true)]
    public class QueryAccessibilityHelpEventArgs : EventArgs
    {
        public QueryAccessibilityHelpEventArgs()
        {
        }

        public QueryAccessibilityHelpEventArgs(string helpNamespace, string helpString, string helpKeyword)
        {
            HelpNamespace = helpNamespace;
            HelpString = helpString;
            HelpKeyword = helpKeyword;
        }

        public string HelpNamespace { get; set; }

        public string HelpString { get; set; }

        public string HelpKeyword { get; set; }
    }
}
