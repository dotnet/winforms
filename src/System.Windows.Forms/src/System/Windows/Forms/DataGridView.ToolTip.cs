// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    public partial class DataGridView
    {
        private class DataGridViewToolTip
        {
            readonly DataGridView dataGridView = null;
            ToolTip toolTip = null;
            private bool toolTipActivated = false;

            public DataGridViewToolTip(DataGridView dataGridView)
            {
                this.dataGridView = dataGridView;
            }

            public bool Activated
            {
                get
                {
                    return toolTipActivated;
                }
            }

            public ToolTip ToolTip
            {
                get
                {
                    return toolTip;
                }
            }

            public void Activate(bool activate)
            {
                if (dataGridView.DesignMode)
                {
                    return;
                }

                // Create the tool tip handle on demand.
                if (activate && toolTip == null)
                {
                    toolTip = new ToolTip
                    {
                        ShowAlways = true,
                        InitialDelay = 0,
                        UseFading = false,
                        UseAnimation = false,
                        AutoPopDelay = 0
                    };
                }

                if (activate)
                {
                    toolTip.Active = true;
                    toolTip.Show(dataGridView.ToolTipPrivate, dataGridView);
                }
                else if (toolTip != null)
                {
                    toolTip.Hide(dataGridView);
                    toolTip.Active = false;
                }

                toolTipActivated = activate;
            }

            public void Dispose()
            {
                if (toolTip != null)
                {
                    toolTip.Dispose();
                    toolTip = null;
                }
            }
        }
    }
}
