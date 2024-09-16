// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.Design;

namespace System.Windows.Forms.Design;

internal partial class CommandSet
{
    /// <internalonly/>
    /// <summary>
    ///  The immediate command set item is used for commands that cannot be cached. Commands
    ///  such as Paste that get outside stimulus cannot be cached by our menu system, so
    ///  they get an ImmediateCommandSetItem instead of a CommandSetItem.
    /// </summary>
    protected class ImmediateCommandSetItem : CommandSetItem
    {
        /// <summary>
        ///  Creates a new ImmediateCommandSetItem.
        /// </summary>
        public ImmediateCommandSetItem(CommandSet commandSet, EventHandler statusHandler, EventHandler invokeHandler, CommandID id, IUIService? uiService)
            : base(commandSet, statusHandler, invokeHandler, id, uiService)
        {
        }

        /// <summary>
        ///  Overrides OleStatus in MenuCommand to invoke our status handler first.
        /// </summary>
        public override int OleStatus
        {
            get
            {
                UpdateStatus();
                return base.OleStatus;
            }
        }
    }
}
