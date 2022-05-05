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
        /// <summary>
        /// These are subclasses of the <see cref="DataGridViewElement"/> for which we don't need to call the finalizer, because it's empty.
        /// See https://github.com/dotnet/winforms/issues/6858.
        /// </summary>
        private static readonly HashSet<Type> s_typesWithEmptyFinalizer = new()
        {
            typeof(DataGridViewBand),
            typeof(DataGridViewColumn),
            typeof(DataGridViewButtonColumn),
            typeof(DataGridViewCheckBoxColumn),
            typeof(DataGridViewComboBoxColumn),
            typeof(DataGridViewImageColumn),
            typeof(DataGridViewLinkColumn),
            typeof(DataGridViewTextBoxColumn),
            typeof(DataGridViewRow),
            typeof(DataGridViewCell),
            typeof(DataGridViewButtonCell),
            typeof(DataGridViewCheckBoxCell),
            typeof(DataGridViewComboBoxCell),
            typeof(DataGridViewHeaderCell),
            typeof(DataGridViewColumnHeaderCell),
            typeof(DataGridViewTopLeftHeaderCell),
            typeof(DataGridViewRowHeaderCell),
            typeof(DataGridViewImageCell),
            typeof(DataGridViewLinkCell),
            typeof(DataGridViewTextBoxCell)
        };

        private DataGridView? _dataGridView;

        /// <summary>
        ///  Initializes a new instance of the <see cref="DataGridViewElement"/> class.
        /// </summary>
        public DataGridViewElement()
        {
            if (s_typesWithEmptyFinalizer.Contains(GetType()))
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
    }
}
