﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel.Design;

namespace System.Windows.Forms.Design
{
    public partial class ParentControlDesigner
    {
        /// <summary>
        ///  This class overrides the escape command so that we can escape
        ///  out of our private drags.
        /// </summary>
        private class EscapeHandler : IMenuStatusHandler
        {
            private readonly ParentControlDesigner designer;

            /// <summary>
            ///  Creates a new escape handler.
            /// </summary>
            public EscapeHandler(ParentControlDesigner designer)
            {
                this.designer = designer;
            }

            /// <summary>
            ///  CommandSet will check with this handler on each status update
            ///  to see if the handler wants to override the availability of
            ///  this command.
            /// </summary>
            public bool OverrideInvoke(MenuCommand cmd)
            {
                if (cmd.CommandID.Equals(MenuCommands.KeyCancel))
                {
                    designer.OnMouseDragEnd(true);
                    return true;
                }

                return false;
            }

            /// <summary>
            ///  CommandSet will check with this handler on each status update
            ///  to see if the handler wants to override the availability of
            ///  this command.
            /// </summary>
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
