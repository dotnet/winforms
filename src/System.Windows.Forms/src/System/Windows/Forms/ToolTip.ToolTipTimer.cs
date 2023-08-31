// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public partial class ToolTip
{
    private class ToolTipTimer : Timer
    {
        public ToolTipTimer(IWin32Window owner) : base()
        {
            Host = owner;
        }

        public IWin32Window Host { get; }
    }
}
