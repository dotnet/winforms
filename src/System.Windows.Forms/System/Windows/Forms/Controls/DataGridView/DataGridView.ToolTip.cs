// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

public partial class DataGridView
{
    private class DataGridViewToolTip
    {
        private readonly DataGridView _dataGridView;

        public DataGridViewToolTip(DataGridView dataGridView)
        {
            _dataGridView = dataGridView;
        }

        public bool Activated { get; private set; }

        public ToolTip? ToolTip { get; private set; }

        public void Activate(bool activate)
        {
            if (_dataGridView.DesignMode || !_dataGridView.IsHandleCreated)
            {
                return;
            }

            if (activate)
            {
                // Create the tool tip handle on demand.
                ToolTip ??= new ToolTip
                {
                    ShowAlways = true,
                    InitialDelay = 0,
                    UseFading = false,
                    UseAnimation = false,
                    AutoPopDelay = 0
                };

                ToolTip.Active = true;
                ToolTip.Show(_dataGridView.ToolTipPrivate, _dataGridView);
            }
            else if (ToolTip is not null)
            {
                ToolTip.Hide(_dataGridView);
                ToolTip.Active = false;
            }

            Activated = activate;
        }

        public void Dispose()
        {
            if (ToolTip is not null)
            {
                ToolTip.Dispose();
                ToolTip = null;
            }
        }
    }
}
