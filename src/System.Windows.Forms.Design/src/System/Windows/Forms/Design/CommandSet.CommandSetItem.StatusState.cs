// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Design;

internal partial class CommandSet
{
    protected partial class CommandSetItem
    {
        /// <summary>
        /// This class saves the state for a given command. It keeps track of the results
        /// of the last status handler invocation and what "selection version" that happened on.
        /// </summary>
        private class StatusState
        {
            // these are the command's possible values.
            //
            [Flags]
            private enum StatusFlag
            {
                Enabled = 0x01,
                Visible = 0x02,
                Checked = 0x04,
                Supported = 0x08,
                NeedsUpdate = 0x10
            }

            private StatusFlag _statusFlags = StatusFlag.NeedsUpdate; // our flags.

            // Multiple CommandSetItem instances can share a same status handler within a designer host.
            // We use a simple ref count to make sure the CommandSetItem can be properly removed.
            internal int _refCount;

            /// <summary>
            /// Just what it says...
            /// </summary>
            public int SelectionVersion { get; private set; }

            /// <summary>
            /// Pushes the state stored in this object into the given command item.
            /// </summary>
            internal void ApplyState(CommandSetItem item)
            {
                Debug.Assert(!_statusFlags.HasFlag(StatusFlag.NeedsUpdate), "Updating item when StatusState is not valid.");

                item.Enabled = _statusFlags.HasFlag(StatusFlag.Enabled);
                item.Visible = _statusFlags.HasFlag(StatusFlag.Visible);
                item.Checked = _statusFlags.HasFlag(StatusFlag.Checked);
                item.Supported = _statusFlags.HasFlag(StatusFlag.Supported);
            }

            /// <summary>
            /// Updates this status object with the state from the given item,
            /// and saves the selection version.
            /// </summary>
            internal void SaveState(CommandSetItem item, int version)
            {
                SelectionVersion = version;
                _statusFlags = 0;
                if (item.Enabled)
                {
                    _statusFlags |= StatusFlag.Enabled;
                }

                if (item.Visible)
                {
                    _statusFlags |= StatusFlag.Visible;
                }

                if (item.Checked)
                {
                    _statusFlags |= StatusFlag.Checked;
                }

                if (item.Supported)
                {
                    _statusFlags |= StatusFlag.Supported;
                }
            }
        }
    }
}
