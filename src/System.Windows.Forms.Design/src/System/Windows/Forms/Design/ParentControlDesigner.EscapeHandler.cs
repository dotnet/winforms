// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.Design;

namespace System.Windows.Forms.Design
{
    public partial class ParentControlDesigner
    {
        /// <include file='doc\ParentControlDesigner.uex' path='docs/doc[@for="ParentControlDesigner.EscapeHandler"]/*' />
        /// <devdoc>
        ///      This class overrides the escape command so that we can escape
        ///      out of our private drags.
        /// </devdoc>
        private class EscapeHandler : IMenuStatusHandler
        {
            private ParentControlDesigner designer;

            /// <include file='doc\ParentControlDesigner.uex' path='docs/doc[@for="ParentControlDesigner.EscapeHandler.EscapeHandler"]/*' />
            /// <devdoc>
            ///      Creates a new escape handler.
            /// </devdoc>
            public EscapeHandler(ParentControlDesigner designer)
            {
                this.designer = designer;

            }

            /// <include file='doc\ParentControlDesigner.uex' path='docs/doc[@for="ParentControlDesigner.EscapeHandler.OverrideInvoke"]/*' />
            /// <devdoc>
            ///     CommandSet will check with this handler on each status update
            ///     to see if the handler wants to override the availability of
            ///     this command.
            /// </devdoc>
            public bool OverrideInvoke(MenuCommand cmd)
            {
                if (cmd.CommandID.Equals(MenuCommands.KeyCancel))
                {
                    designer.OnMouseDragEnd(true);
                    return true;
                }

                return false;
            }

            /// <include file='doc\ParentControlDesigner.uex' path='docs/doc[@for="ParentControlDesigner.EscapeHandler.OverrideStatus"]/*' />
            /// <devdoc>
            ///     CommandSet will check with this handler on each status update
            ///     to see if the handler wants to override the availability of
            ///     this command.
            /// </devdoc>
            public bool OverrideStatus(MenuCommand cmd)
            {
                if (cmd.CommandID.Equals(MenuCommands.KeyCancel))
                {
                    cmd.Enabled = true;
                }
                else
                {
                    cmd.Enabled = false;
                }

                return true;
            }
        }
    }
}
