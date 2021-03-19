// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Runtime.CompilerServices;

namespace System.Windows.Forms
{
    internal sealed partial class KeyboardToolTipStateMachine
    {
        private sealed class ToolToTipDictionary
        {
            private readonly ConditionalWeakTable<IKeyboardToolTip, WeakReference<ToolTip>> table = new ConditionalWeakTable<IKeyboardToolTip, WeakReference<ToolTip>>();

            public ToolTip this[IKeyboardToolTip tool]
            {
                get
                {
                    ToolTip toolTip = null;
                    if (table.TryGetValue(tool, out WeakReference<ToolTip> toolTipReference))
                    {
                        if (!toolTipReference.TryGetTarget(out toolTip))
                        {
                            // removing dead reference
                            table.Remove(tool);
                        }
                    }

                    return toolTip;
                }
                set
                {
                    if (table.TryGetValue(tool, out WeakReference<ToolTip> toolTipReference))
                    {
                        toolTipReference.SetTarget(value);
                    }
                    else
                    {
                        table.Add(tool, new WeakReference<ToolTip>(value));
                    }
                }
            }

            public void Remove(IKeyboardToolTip tool, ToolTip toolTip)
            {
                if (table.TryGetValue(tool, out WeakReference<ToolTip> toolTipReference))
                {
                    if (toolTipReference.TryGetTarget(out ToolTip existingToolTip))
                    {
                        if (existingToolTip == toolTip)
                        {
                            table.Remove(tool);
                        }
                    }
                    else
                    {
                        // removing dead reference
                        table.Remove(tool);
                    }
                }
            }
        }
    }
}
