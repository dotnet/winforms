// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace System.Windows.Forms
{
    /// <summary>
    /// Identifies an element in the dataGridView (base class for TCell, TBand, TRow, TColumn.
    /// </summary>
    public class DataGridViewElement 
    {
        private DataGridViewElementStates _state; // enabled frozen readOnly resizable selected visible
        private DataGridView _dataGridView;

        /// <summary>
        /// Initializes a new instance of the <see cref='System.Windows.Forms.DataGridViewElement'/> class.
        /// </summary>
        public DataGridViewElement()
        {
            _state = DataGridViewElementStates.Visible;
        }

        internal DataGridViewElement(DataGridViewElement dgveTemplate)
        {
            // Selected and Displayed states are not inherited
            _state = dgveTemplate.State & (DataGridViewElementStates.Frozen | DataGridViewElementStates.ReadOnly | DataGridViewElementStates.Resizable | DataGridViewElementStates.ResizableSet | DataGridViewElementStates.Visible);
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public virtual DataGridViewElementStates State => _state;

        internal DataGridViewElementStates StateInternal
        {
            set => _state = value;
        }

        internal bool StateIncludes(DataGridViewElementStates elementState)
        {
            return (State & elementState) == elementState;
        }

        internal bool StateExcludes(DataGridViewElementStates elementState)
        {
            return (State & elementState) == 0;
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DataGridView DataGridView => _dataGridView;

        internal DataGridView DataGridViewInternal
        {
            set
            {
                if (DataGridView != value)
                {
                    _dataGridView = value;
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
            _dataGridView?.OnCellClickInternal(e);
        }

        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")] // Method raises an event for the grid control
        protected void RaiseCellContentClick(DataGridViewCellEventArgs e)
        {
            _dataGridView?.OnCellContentClickInternal(e);
        }

        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")] // Method raises an event for the grid control
        protected void RaiseCellContentDoubleClick(DataGridViewCellEventArgs e)
        {
            _dataGridView?.OnCellContentDoubleClickInternal(e);
        }

        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")] // Method raises an event for the grid control
        protected void RaiseCellValueChanged(DataGridViewCellEventArgs e)
        {
            _dataGridView?.OnCellValueChangedInternal(e);
        }

        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")] // Method raises an event for the grid control
        protected void RaiseDataError(DataGridViewDataErrorEventArgs e)
        {
            _dataGridView?.OnDataErrorInternal(e);
        }

        [SuppressMessage("Microsoft.Design", "CA1030:UseEventsWhereAppropriate")] // Method raises an event for the grid control
        protected void RaiseMouseWheel(MouseEventArgs e)
        {
            _dataGridView?.OnMouseWheelInternal(e);
        }
    }
}
