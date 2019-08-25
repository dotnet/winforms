// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.Design
{
    /// <summary>
    ///  Supports the hosting of several 'pane' windows separated by splitter bars.
    /// </summary>
    internal interface ISplitWindowService
    {
        /// <summary>
        ///  Requests the service to add a window 'pane'.
        /// </summary>
        void AddSplitWindow(Control window);

        /// <summary>
        ///  Requests the service to remove a window 'pane'.
        /// </summary>
        void RemoveSplitWindow(Control window);
    }
}
