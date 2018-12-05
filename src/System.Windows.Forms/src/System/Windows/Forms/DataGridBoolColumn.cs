// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    
    using System.Windows.Forms;
    using System.ComponentModel;
    using System.Drawing;
    using Microsoft.Win32;

    /// <include file='doc\DataGridBoolColumn.uex' path='docs/doc[@for="DataGridBoolColumn"]/*' />
    /// <devdoc>
    ///    <para>Specifies a column in
    ///       which each cell contains a check box for representing
    ///       a boolean value.</para>
    /// </devdoc>
    public class DataGridBoolColumn : DataGridColumnStyle {
        private static readonly int idealCheckSize = 14;

        private bool isEditing = false;
        private bool isSelected = false;
        private bool allowNull = true;
        private int  editingRow = -1;
        private object currentValue = Convert.DBNull;
        
        private object trueValue = true;
        private object falseValue = false;
        private object nullValue = Convert.DBNull;

        private static readonly object  EventTrueValue      = new object();
        private static readonly object  EventFalseValue     = new object();
        private static readonly object  EventAllowNull      = new object();
        
        /// <include file='doc\DataGridBoolColumn.uex' path='docs/doc[@for="DataGridBoolColumn.DataGridBoolColumn"]/*' />
        /// <devdoc>
        /// <para>Initializes a new instance of the <see cref='System.Windows.Forms.DataGridBoolColumn'/> class.</para>
        /// </devdoc>
        public DataGridBoolColumn() : base() {}

        /// <include file='doc\DataGridBoolColumn.uex' path='docs/doc[@for="DataGridBoolColumn.DataGridBoolColumn1"]/*' />
        /// <devdoc>
        /// <para>Initializes a new instance of a <see cref='System.Windows.Forms.DataGridBoolColumn'/> with the specified <see cref='System.Data.DataColumn'/>.</para>
        /// </devdoc>
        public DataGridBoolColumn(PropertyDescriptor prop)
            : base(prop) {}

        /// <include file='doc\DataGridBoolColumn.uex' path='docs/doc[@for="DataGridBoolColumn.DataGridBoolColumn2"]/*' />
        public DataGridBoolColumn(PropertyDescriptor prop, bool isDefault)
            : base(prop, isDefault){}

        /// <include file='doc\DataGridBoolColumn.uex' path='docs/doc[@for="DataGridBoolColumn.TrueValue"]/*' />
        /// <devdoc>
        ///    <para>Gets or sets the actual value used when setting the 
        ///       value of the column to <see langword='true'/> .</para>
        /// </devdoc>
        [TypeConverterAttribute(typeof(StringConverter)),
        DefaultValue(true)]
        public object TrueValue {
            get {
                return trueValue;
            }
            set {
                if (!trueValue.Equals(value)) {
                    this.trueValue = value;
                    OnTrueValueChanged(EventArgs.Empty);
                    Invalidate();
                }
            }
        }

        /// <include file='doc\DataGridBoolColumn.uex' path='docs/doc[@for="DataGridBoolColumn.TrueValueChanged"]/*' />
        public event EventHandler TrueValueChanged { 
            add {
                Events.AddHandler(EventTrueValue, value);
            }
            remove {
                Events.RemoveHandler(EventTrueValue, value);
            }
        }
        
        /// <include file='doc\DataGridBoolColumn.uex' path='docs/doc[@for="DataGridBoolColumn.FalseValue"]/*' />
        /// <devdoc>
        ///    <para>Gets or sets the actual value used when setting the value of the column to 
        ///    <see langword='false'/>.</para>
        /// </devdoc>
        [TypeConverterAttribute(typeof(StringConverter)), DefaultValue(false)]
        public object FalseValue {
            get {
                return falseValue;
            }
            set {
                if (!falseValue.Equals(value)) {
                    this.falseValue = value;
                    OnFalseValueChanged(EventArgs.Empty);
                    Invalidate();
                }
            }
        }
        
        /// <include file='doc\DataGridBoolColumn.uex' path='docs/doc[@for="DataGridBoolColumn.FalseValueChanged"]/*' />
        public event EventHandler FalseValueChanged { 
            add {
                Events.AddHandler(EventFalseValue, value);
            }
            remove {
                Events.RemoveHandler(EventFalseValue, value);
            }
        }
        
        /// <include file='doc\DataGridBoolColumn.uex' path='docs/doc[@for="DataGridBoolColumn.NullValue"]/*' />
        /// <devdoc>
        ///    <para>Gets or sets the actual value used when setting the value of the column to 
        ///    <see langword='null'/>.</para>
        /// </devdoc>
        [TypeConverterAttribute(typeof(StringConverter))]
        public object NullValue {
            get {
                return nullValue;
            }
            set {
                if (!nullValue.Equals(value)) {
                    this.nullValue = value;
                    OnFalseValueChanged(EventArgs.Empty);
                    Invalidate();
                }
            }
        }
        
        // =------------------------------------------------------------------
        // =        Methods
        // =------------------------------------------------------------------

        // when the grid is in addNewRow it means that the user did not start typing
        // so there is no data to be pushed back into the backEnd.
        // make isEditing false so that in the Commit call we do not do any work.
        //
        /// <include file='doc\DataGridBoolColumn.uex' path='docs/doc[@for="DataGridBoolColumn.ConcedeFocus"]/*' />
        protected internal override void ConcedeFocus() {
            base.ConcedeFocus();
            this.isSelected = false;
            this.isEditing = false;
        }

        private Rectangle GetCheckBoxBounds(Rectangle bounds, bool alignToRight) {
            if (alignToRight)
                return new Rectangle(bounds.X +((bounds.Width - idealCheckSize) /2),
                                     bounds.Y +((bounds.Height - idealCheckSize) / 2),
                                     bounds.Width < idealCheckSize ? bounds.Width : idealCheckSize,
                                     idealCheckSize);
            else
                return new Rectangle(Math.Max(0,bounds.X +((bounds.Width - idealCheckSize) /2)),
                                     Math.Max(0,bounds.Y +((bounds.Height - idealCheckSize) / 2)),
                                     bounds.Width < idealCheckSize ? bounds.Width : idealCheckSize,
                                     idealCheckSize);
        }

        /// <include file='doc\DataGridBoolColumn.uex' path='docs/doc[@for="DataGridBoolColumn.GetColumnValueAtRow"]/*' />
        /// <devdoc>
        ///    <para>Gets the value at the specified row.</para>
        /// </devdoc>
        protected internal override object GetColumnValueAtRow(CurrencyManager lm, int row) {
            object baseValue = base.GetColumnValueAtRow(lm, row);
            object value = Convert.DBNull;
            if (baseValue.Equals(trueValue)) {
                value = true;
            }
            else if (baseValue.Equals(falseValue)) {
                value = false;
            }
            return value;
        }

        private bool IsReadOnly() {
            bool ret = this.ReadOnly;
            if (this.DataGridTableStyle != null) {
                ret = ret || this.DataGridTableStyle.ReadOnly;
                if (this.DataGridTableStyle.DataGrid != null)
                    ret = ret || this.DataGridTableStyle.DataGrid.ReadOnly;
            }
            return ret;
        }

        /// <include file='doc\DataGridBoolColumn.uex' path='docs/doc[@for="DataGridBoolColumn.SetColumnValueAtRow"]/*' />
        /// <devdoc>
        ///    <para>Sets the value a a specified row.</para>
        /// </devdoc>
        protected internal override void SetColumnValueAtRow(CurrencyManager lm, int row, object value) {
            object baseValue = null;
            if (true.Equals(value)) {
                baseValue = TrueValue;
            }
            else if (false.Equals(value)) {
                baseValue = FalseValue;
            }
            else if (Convert.IsDBNull(value)) {
                baseValue = NullValue;
            }
            currentValue = baseValue;
            base.SetColumnValueAtRow(lm, row, baseValue);
        }
        
        /// <include file='doc\DataGridBoolColumn.uex' path='docs/doc[@for="DataGridBoolColumn.GetPreferredSize"]/*' />
        /// <devdoc>
        ///    <para>Gets the optimum width and height of a cell given
        ///       a specific value to contain.</para>
        /// </devdoc>
        protected internal override Size GetPreferredSize(Graphics g, object value) {
            return new Size(idealCheckSize+2, idealCheckSize+2);
        }

        /// <include file='doc\DataGridBoolColumn.uex' path='docs/doc[@for="DataGridBoolColumn.GetMinimumHeight"]/*' />
        /// <devdoc>
        ///    <para>Gets
        ///       the height of a cell in a column.</para>
        /// </devdoc>
        protected internal override int GetMinimumHeight() {
            return idealCheckSize+2;
        }

        /// <include file='doc\DataGridBoolColumn.uex' path='docs/doc[@for="DataGridBoolColumn.GetPreferredHeight"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the height used when resizing columns.
        ///    </para>
        /// </devdoc>
        protected internal override int GetPreferredHeight(Graphics g, object value)
        {
            return idealCheckSize + 2;
        }

        /// <include file='doc\DataGridBoolColumn.uex' path='docs/doc[@for="DataGridBoolColumn.Abort"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Initiates a request to interrupt an edit procedure.
        ///    </para>
        /// </devdoc>
        protected internal override void Abort(int rowNum) {
            isSelected = false;
            isEditing = false;
            Invalidate();
            return;
        }

        /// <include file='doc\DataGridBoolColumn.uex' path='docs/doc[@for="DataGridBoolColumn.Commit"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Initiates a request to complete an editing procedure.
        ///    </para>
        /// </devdoc>
        protected internal override bool Commit(CurrencyManager dataSource, int rowNum) {
            isSelected = false;
            // always invalidate
            Invalidate();
            if (!isEditing)
                return true;

            SetColumnValueAtRow(dataSource, rowNum, currentValue);
            isEditing = false;
            return true;
        }

        /// <include file='doc\DataGridBoolColumn.uex' path='docs/doc[@for="DataGridBoolColumn.Edit"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Prepares the cell for editing a value.
        ///    </para>
        /// </devdoc>
        protected internal override void Edit(CurrencyManager source,
                                    int rowNum,
                                    Rectangle bounds,
                                    bool readOnly,
                                    string displayText,
                                    bool cellIsVisible)
        {
            // toggle state right now...
            isSelected = true;

            // move the focus away from the previous column and give it to the grid
            //
            DataGrid grid = this.DataGridTableStyle.DataGrid;
            if (!grid.Focused)
                grid.FocusInternal();

            if (!readOnly && !IsReadOnly()) {
                editingRow = rowNum;
                currentValue = GetColumnValueAtRow(source, rowNum);
            }

            base.Invalidate();
        }

        /// <devdoc>
        ///    <para>
        ///       Provides a handler for determining which key was pressed, and whether to
        ///       process it.
        ///    </para>
        /// </devdoc>
        internal override bool KeyPress(int rowNum, Keys keyData) {
            if (isSelected && editingRow == rowNum && !IsReadOnly()) {
                if ((keyData & Keys.KeyCode) == Keys.Space) {
                    ToggleValue();
                    Invalidate();
                    return true;
                }
            }
            return base.KeyPress(rowNum, keyData);
        }

        /// <internalonly/>
        /// <devdoc>
        ///    <para>
        ///       Indicates whether the a mouse down event occurred at the specified row, at
        ///       the specified x and y coordinates.
        ///    </para>
        /// </devdoc>
        internal override bool MouseDown(int rowNum, int x, int y) {
            base.MouseDown(rowNum, x, y);
            if (isSelected && editingRow == rowNum && !IsReadOnly()) {
                ToggleValue();
                Invalidate();
                return true;
            }
            return false;
        }

        private void OnTrueValueChanged(EventArgs e) {
            EventHandler eh = this.Events[EventTrueValue] as EventHandler;
            if (eh != null)
                eh(this, e);
        }
        private void OnFalseValueChanged(EventArgs e) {
            EventHandler eh = this.Events[EventFalseValue] as EventHandler;
            if (eh != null)
                eh(this, e);
        }
        private void OnAllowNullChanged(EventArgs e) {
            EventHandler eh = this.Events[EventAllowNull] as EventHandler;
            if (eh != null)
                eh(this, e);
        }

        /// <include file='doc\DataGridBoolColumn.uex' path='docs/doc[@for="DataGridBoolColumn.Paint"]/*' />
        /// <internalonly/>
        /// <devdoc>
        /// <para>Draws the <see cref='System.Windows.Forms.DataGridBoolColumn'/>
        /// with the given <see cref='System.Drawing.Graphics'/>,
        /// <see cref='System.Drawing.Rectangle'/> and row number.</para>
        /// </devdoc>
        protected internal override void Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum)
        {
            Paint(g,bounds,source, rowNum, false);
        }

        /// <include file='doc\DataGridBoolColumn.uex' path='docs/doc[@for="DataGridBoolColumn.Paint1"]/*' />
        /// <internalonly/>
        /// <devdoc>
        /// <para>Draws the <see cref='System.Windows.Forms.DataGridBoolColumn'/>
        /// with the given <see cref='System.Drawing.Graphics'/>, <see cref='System.Drawing.Rectangle'/>,
        /// row number, and alignment settings. </para>
        /// </devdoc>
        protected internal override void Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum, bool alignToRight) {
            Paint(g,bounds,source, rowNum, this.DataGridTableStyle.BackBrush, this.DataGridTableStyle.ForeBrush, alignToRight);
        }

        /// <include file='doc\DataGridBoolColumn.uex' path='docs/doc[@for="DataGridBoolColumn.Paint2"]/*' />
        /// <devdoc>
        /// <para>Draws the <see cref='System.Windows.Forms.DataGridBoolColumn'/> with the given <see cref='System.Drawing.Graphics'/>, <see cref='System.Drawing.Rectangle'/>,
        ///    row number, <see cref='System.Drawing.Brush'/>, and <see cref='System.Drawing.Color'/>. </para>
        /// </devdoc>
        protected internal override void Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum,
                                     Brush backBrush, Brush foreBrush,
                                     bool alignToRight) {
            object value = (isEditing && editingRow == rowNum) ? currentValue : GetColumnValueAtRow(source, rowNum);
            ButtonState checkedState = ButtonState.Inactive;
            if (!Convert.IsDBNull(value)) {
                checkedState = ((bool)value ? ButtonState.Checked : ButtonState.Normal);
            }

            Rectangle box = GetCheckBoxBounds(bounds, alignToRight);

            Region r = g.Clip;
            g.ExcludeClip(box);

            System.Drawing.Brush selectionBrush = this.DataGridTableStyle.IsDefault ? this.DataGridTableStyle.DataGrid.SelectionBackBrush : this.DataGridTableStyle.SelectionBackBrush;
            if (isSelected && editingRow == rowNum && !IsReadOnly()) {
                g.FillRectangle(selectionBrush, bounds);
            }
            else
                g.FillRectangle(backBrush, bounds);
            g.Clip = r;

            if (checkedState == ButtonState.Inactive) {
                ControlPaint.DrawMixedCheckBox(g, box, ButtonState.Checked);
            } else {
                ControlPaint.DrawCheckBox(g, box, checkedState);
            }

            // if the column is read only we should still show selection
            if (IsReadOnly() && isSelected && source.Position == rowNum) {
                bounds.Inflate(-1,-1);
                System.Drawing.Pen pen = new System.Drawing.Pen(selectionBrush);
                pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dash;
                g.DrawRectangle(pen, bounds);
                pen.Dispose();
                // restore the bounds rectangle
                bounds.Inflate(1,1);
            }
        }

        /// <include file='doc\DataGridBoolColumn.uex' path='docs/doc[@for="DataGridBoolColumn.AllowNull"]/*' />
        /// <devdoc>
        ///    <para>Gets or sets a value indicating whether null values are allowed.</para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(true),
        SRDescription(nameof(SR.DataGridBoolColumnAllowNullValue))
        ]
        public bool AllowNull {
            get {
                return allowNull;
            }
            set {
                if (allowNull != value)
                {
                    allowNull = value;
                    // if we do not allow null, and the gridColumn had
                    // a null in it, discard it
                    if (!value && Convert.IsDBNull(currentValue))
                    {
                        currentValue = false;
                        Invalidate();
                    }
                    OnAllowNullChanged(EventArgs.Empty);
                }
            }
        }

        /// <include file='doc\DataGridBoolColumn.uex' path='docs/doc[@for="DataGridBoolColumn.AllowNullChanged"]/*' />
        public event EventHandler AllowNullChanged { 
            add {
                Events.AddHandler(EventAllowNull, value);
            }
            remove {
                Events.RemoveHandler(EventAllowNull, value);
            }
        }
        
        /// <include file='doc\DataGridBoolColumn.uex' path='docs/doc[@for="DataGridBoolColumn.EnterNullValue"]/*' />
        /// <devdoc>
        /// <para>Enters a <see langword='null'/> into the column.</para>
        /// </devdoc>
        protected internal override void EnterNullValue()
        {
            // do not throw an exception when the column is marked as readOnly or
            // does not allowNull
            if (!this.AllowNull || IsReadOnly())
                return;
            if (currentValue != Convert.DBNull) {
                currentValue = Convert.DBNull;
                Invalidate();
            }
        }

        private void ResetNullValue() {
            NullValue = Convert.DBNull;
        }

        private bool ShouldSerializeNullValue() {
            return nullValue != Convert.DBNull;
        }

        private void ToggleValue() {
         
            if (currentValue is bool && ((bool)currentValue) == false) {
                currentValue = true;
            }
            else {
                if (AllowNull) {
                    if (Convert.IsDBNull(currentValue)) {
                        currentValue = false;
                    }
                    else {
                        currentValue = Convert.DBNull;
                    }
                }
                else {
                    currentValue = false;
                }
            }
            // we started editing
            isEditing = true;
            // tell the dataGrid that things are changing
            // we put Rectangle.Empty cause toggle will invalidate the row anyhow
            this.DataGridTableStyle.DataGrid.ColumnStartedEditing(Rectangle.Empty);
        }
    }
}
