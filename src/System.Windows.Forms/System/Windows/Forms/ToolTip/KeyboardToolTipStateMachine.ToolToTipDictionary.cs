// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.CompilerServices;

namespace System.Windows.Forms;

internal sealed partial class KeyboardToolTipStateMachine
{
    private sealed class ToolToTipDictionary
    {
        private readonly ConditionalWeakTable<IKeyboardToolTip, WeakReference<ToolTip?>> _table = [];

        public ToolTip? this[IKeyboardToolTip tool]
        {
            get
            {
                ToolTip? toolTip = null;
                if (_table.TryGetValue(tool, out WeakReference<ToolTip?>? toolTipReference))
                {
                    if (!toolTipReference.TryGetTarget(out toolTip))
                    {
                        // removing dead reference
                        _table.Remove(tool);
                    }
                }

                return toolTip;
            }
            set
            {
                if (_table.TryGetValue(tool, out WeakReference<ToolTip?>? toolTipReference))
                {
                    toolTipReference.SetTarget(value);
                }
                else
                {
                    _table.Add(tool, new WeakReference<ToolTip?>(value));
                }
            }
        }

        public void Remove(IKeyboardToolTip tool, ToolTip toolTip)
        {
            if (_table.TryGetValue(tool, out WeakReference<ToolTip?>? toolTipReference))
            {
                if (toolTipReference.TryGetTarget(out ToolTip? existingToolTip))
                {
                    if (existingToolTip == toolTip)
                    {
                        _table.Remove(tool);
                    }
                }
                else
                {
                    // removing dead reference
                    _table.Remove(tool);
                }
            }
        }
    }
}
