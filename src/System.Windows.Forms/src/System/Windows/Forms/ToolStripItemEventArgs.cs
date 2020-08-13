// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

namespace System.Windows.Forms
{
    public class ToolStripItemEventArgs : EventArgs
    {
        public ToolStripItemEventArgs(ToolStripItem item)
        {
            Item = item;
        }

        public ToolStripItem Item { get; }
    }
}
