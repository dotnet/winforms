// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    /// 
    /// </summary>
    public class TaskDialogHyperlinkClickedEventArgs : EventArgs
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="hyperlink"></param>
        internal TaskDialogHyperlinkClickedEventArgs(string hyperlink)
        {
            Hyperlink = hyperlink;
        }

        /// <summary>
        /// 
        /// </summary>
        public string Hyperlink
        {
            get;
        }
    }
}
