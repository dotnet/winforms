// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Identifies an element in the dataGridView (base class for TCell, TBand, TRow, TColumn.
    /// </summary>
    public class DataGridViewElement
    {
        private DataGridView? _dataGridView;

        /// <summary>
        ///  Initializes a new instance of the <see cref="DataGridViewElement"/> class.
        /// </summary>
        public DataGridViewElement()
        {
            if (NeedSuppressFinalize(GetType()))
                GC.SuppressFinalize(this);

            State = DataGridViewElementStates.Visible;
        }

        internal DataGridViewElement(DataGridViewElement dgveTemplate)
        {
            // Selected and Displayed states are not inherited
            State = dgveTemplate.State & (DataGridViewElementStates.Frozen | DataGridViewElementStates.ReadOnly | DataGridViewElementStates.Resizable | DataGridViewElementStates.ResizableSet | DataGridViewElementStates.Visible);
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public virtual DataGridViewElementStates State { get; internal set; }

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
        public DataGridView? DataGridView
        {
            get => _dataGridView;
            internal set
            {
                if (_dataGridView != value)
                {
                    _dataGridView = value;
                    OnDataGridViewChanged();
                }
            }
        }

        protected virtual void OnDataGridViewChanged()
        {
        }

        protected void RaiseCellClick(DataGridViewCellEventArgs e)
        {
            _dataGridView?.OnCellClickInternal(e);
        }

        protected void RaiseCellContentClick(DataGridViewCellEventArgs e)
        {
            _dataGridView?.OnCellContentClickInternal(e);
        }

        protected void RaiseCellContentDoubleClick(DataGridViewCellEventArgs e)
        {
            _dataGridView?.OnCellContentDoubleClickInternal(e);
        }

        protected void RaiseCellValueChanged(DataGridViewCellEventArgs e)
        {
            _dataGridView?.OnCellValueChangedInternal(e);
        }

        protected void RaiseDataError(DataGridViewDataErrorEventArgs e)
        {
            _dataGridView?.OnDataErrorInternal(e);
        }

        protected void RaiseMouseWheel(MouseEventArgs e)
        {
            _dataGridView?.OnMouseWheelInternal(e);
        }

        /// <summary>
        /// Determines whether <see cref="GC.SuppressFinalize(object)" /> should be called in the constructor for the given <paramref name="type"/>.<br/>
        /// <see cref="GC.SuppressFinalize(object)" /> must be called on known internal subclasses of <see cref="DataGridViewBand" /> / <see cref="DataGridViewCell" /> which don't need a finalizer.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns><see langword="true"/> if need to call <see cref="GC.SuppressFinalize(object)" />, <see langword="false"/> otherwise.</returns>
        private static bool NeedSuppressFinalize(Type type)
        {
            return type == typeof(DataGridViewBand) || type == typeof(DataGridViewColumn) || type == typeof(DataGridViewButtonColumn) || type == typeof(DataGridViewCheckBoxColumn) ||
                type == typeof(DataGridViewComboBoxColumn) || type == typeof(DataGridViewImageColumn) || type == typeof(DataGridViewLinkColumn) || type == typeof(DataGridViewTextBoxColumn) ||
                type == typeof(DataGridViewRow) || type == typeof(DataGridViewCell) || type == typeof(DataGridViewButtonCell) || type == typeof(DataGridViewCheckBoxCell) ||
                type == typeof(DataGridViewComboBoxCell) || type == typeof(DataGridViewHeaderCell) || type == typeof(DataGridViewColumnHeaderCell) || type == typeof(DataGridViewTopLeftHeaderCell) ||
                type == typeof(DataGridViewRowHeaderCell) || type == typeof(DataGridViewImageCell) || type == typeof(DataGridViewLinkCell) || type == typeof(DataGridViewTextBoxCell);
        }
    }
}
