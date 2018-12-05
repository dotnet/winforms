// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;
    using System.Drawing;
    using System.Diagnostics;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    
    /// <devdoc>
    ///    <para>Identifies an element in the dataGridView (base class for TCell, TBand, TRow, TColumn.</para>
    /// </devdoc>
    public class DataGridViewElement 
    {
        private DataGridViewElementStates state; // enabled frozen readOnly resizable selected visible
        private DataGridView dataGridView;

        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of the <see cref='System.Windows.Forms.DataGridViewElement'/> class.
        ///    </para>
        /// </devdoc>
        public DataGridViewElement()
        {
            this.state = DataGridViewElementStates.Visible;
        }

        internal DataGridViewElement(DataGridViewElement dgveTemplate)
        {
            // Selected and Displayed states are not inherited
            this.state = dgveTemplate.State & (DataGridViewElementStates.Frozen | DataGridViewElementStates.ReadOnly | DataGridViewElementStates.Resizable | DataGridViewElementStates.ResizableSet | DataGridViewElementStates.Visible);
        }

        [
            Browsable(false),
            EditorBrowsable(EditorBrowsableState.Advanced)
        ]
        public virtual DataGridViewElementStates State
        {
            get
            {
                return this.state;
            }
        }

        internal DataGridViewElementStates StateInternal
        {
            set
            {
                this.state = value;
            }
        }

        internal bool StateIncludes(DataGridViewElementStates elementState)
        {
            return (this.State & elementState) == elementState;
        }

        internal bool StateExcludes(DataGridViewElementStates elementState)
        {
            return (this.State & elementState) == 0;
        }

        [
            Browsable(false),
            DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public DataGridView DataGridView
        {
            get
            {
                return this.dataGridView;
            }
        }

        internal DataGridView DataGridViewInternal
        {
            set
            {
                if (this.DataGridView != value)
                {
                    this.dataGridView = value;
                    OnDataGridViewChanged();
                }
            }
        }

        protected virtual void OnDataGridViewChanged()
        {
        }

        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")] // Method raises an event for the grid control
        protected void RaiseCellClick(DataGridViewCellEventArgs e)
        {
            if (this.dataGridView != null)
            {
                this.dataGridView.OnCellClickInternal(e);
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")] // Method raises an event for the grid control
        protected void RaiseCellContentClick(DataGridViewCellEventArgs e)
        {
            if (this.dataGridView != null)
            {
                this.dataGridView.OnCellContentClickInternal(e);
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")] // Method raises an event for the grid control
        protected void RaiseCellContentDoubleClick(DataGridViewCellEventArgs e)
        {
            if (this.dataGridView != null)
            {
                this.dataGridView.OnCellContentDoubleClickInternal(e);
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")] // Method raises an event for the grid control
        protected void RaiseCellValueChanged(DataGridViewCellEventArgs e)
        {
            if (this.dataGridView != null)
            {
                this.dataGridView.OnCellValueChangedInternal(e);
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")] // Method raises an event for the grid control
        protected void RaiseDataError(DataGridViewDataErrorEventArgs e)
        {
            if (this.dataGridView != null)
            {
                this.dataGridView.OnDataErrorInternal(e);
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")] // Method raises an event for the grid control
        protected void RaiseMouseWheel(MouseEventArgs e)
        {
            if (this.dataGridView != null)
            {
                this.dataGridView.OnMouseWheelInternal(e);
            }
        }
    }
}
