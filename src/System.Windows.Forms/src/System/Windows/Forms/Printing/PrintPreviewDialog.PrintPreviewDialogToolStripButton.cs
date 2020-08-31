// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    public partial class PrintPreviewDialog
    {
        internal class PrintPreviewDialogToolStripButton : ToolStripButton
        {
            /// <summary>
            ///  See <see cref="Control.ProcessDialogKey"/> for more info.
            /// </summary>
            protected internal override bool ProcessDialogKey(Keys keyData)
            {
                if (keyData == Keys.Enter || (SupportsSpaceKey && keyData == Keys.Space))
                {
                    FireEvent(ToolStripItemEventType.Click);
                    return true;
                }

                return false;
            }
        }
    }
}
