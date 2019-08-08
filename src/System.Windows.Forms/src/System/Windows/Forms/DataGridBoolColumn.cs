// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Specifies a column in
    ///  which each cell contains a check box for representing
    ///  a boolean value.
    /// </summary>
    public class DataGridBoolColumn : DataGridColumnStyle
    {
        private static readonly int idealCheckSize = 14;

        private bool isEditing = false;
        private bool isSelected = false;
        private bool allowNull = true;
        private int editingRow = -1;
        private object currentValue = Convert.DBNull;

        private object trueValue = true;
        private object falseValue = false;
        private object nullValue = Convert.DBNull;

        private static readonly object EventTrueValue = new object();
        private static readonly object EventFalseValue = new object();
        private static readonly object EventAllowNull = new object();

        /// <summary>
        ///  Initializes a new instance of the <see cref='DataGridBoolColumn'/> class.
        /// </summary>
        public DataGridBoolColumn() : base() { }

        /// <summary>
        ///  Initializes a new instance of a <see cref='DataGridBoolColumn'/> with the specified <see cref='Data.DataColumn'/>.
        /// </summary>
        public DataGridBoolColumn(PropertyDescriptor prop)
            : base(prop) { }

        public DataGridBoolColumn(PropertyDescriptor prop, bool isDefault)
            : base(prop, isDefault) { }

        /// <summary>
        ///  Gets or sets the actual value used when setting the
        ///  value of the column to <see langword='true'/> .
        /// </summary>
        [TypeConverter(typeof(StringConverter)),
        DefaultValue(true)]
        public object TrueValue
        {
            get
            {
                return trueValue;
            }
            set
            {
                if (!trueValue.Equals(value))
                {
                    trueValue = value;
                    OnTrueValueChanged(EventArgs.Empty);
                    Invalidate();
                }
            }
        }

        public event EventHandler TrueValueChanged
        {
            add => Events.AddHandler(EventTrueValue, value);
            remove => Events.RemoveHandler(EventTrueValue, value);
        }

        /// <summary>
        ///  Gets or sets the actual value used when setting the value of the column to
        ///  <see langword='false'/>.
        /// </summary>
        [TypeConverter(typeof(StringConverter)), DefaultValue(false)]
        public object FalseValue
        {
            get
            {
                return falseValue;
            }
            set
            {
                if (!falseValue.Equals(value))
                {
                    falseValue = value;
                    OnFalseValueChanged(EventArgs.Empty);
                    Invalidate();
                }
            }
        }

        public event EventHandler FalseValueChanged
        {
            add => Events.AddHandler(EventFalseValue, value);
            remove => Events.RemoveHandler(EventFalseValue, value);
        }

        /// <summary>
        ///  Gets or sets the actual value used when setting the value of the column to
        ///  <see langword='null'/>.
        /// </summary>
        [TypeConverter(typeof(StringConverter))]
        public object NullValue
        {
            get
            {
                return nullValue;
            }
            set
            {
                if (!nullValue.Equals(value))
                {
                    nullValue = value;
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
        protected internal override void ConcedeFocus()
        {
            base.ConcedeFocus();
            isSelected = false;
            isEditing = false;
        }

        private Rectangle GetCheckBoxBounds(Rectangle bounds, bool alignToRight)
        {
            if (alignToRight)
            {
                return new Rectangle(bounds.X + ((bounds.Width - idealCheckSize) / 2),
                                     bounds.Y + ((bounds.Height - idealCheckSize) / 2),
                                     bounds.Width < idealCheckSize ? bounds.Width : idealCheckSize,
                                     idealCheckSize);
            }
            else
            {
                return new Rectangle(Math.Max(0, bounds.X + ((bounds.Width - idealCheckSize) / 2)),
                                     Math.Max(0, bounds.Y + ((bounds.Height - idealCheckSize) / 2)),
                                     bounds.Width < idealCheckSize ? bounds.Width : idealCheckSize,
                                     idealCheckSize);
            }
        }

        /// <summary>
        ///  Gets the value at the specified row.
        /// </summary>
        protected internal override object GetColumnValueAtRow(CurrencyManager lm, int row)
        {
            object baseValue = base.GetColumnValueAtRow(lm, row);
            object value = Convert.DBNull;
            if (baseValue.Equals(trueValue))
            {
                value = true;
            }
            else if (baseValue.Equals(falseValue))
            {
                value = false;
            }
            return value;
        }

        private bool IsReadOnly()
        {
            bool ret = ReadOnly;
            if (DataGridTableStyle != null)
            {
                ret = ret || DataGridTableStyle.ReadOnly;
                if (DataGridTableStyle.DataGrid != null)
                {
                    ret = ret || DataGridTableStyle.DataGrid.ReadOnly;
                }
            }
            return ret;
        }

        /// <summary>
        ///  Sets the value a a specified row.
        /// </summary>
        protected internal override void SetColumnValueAtRow(CurrencyManager lm, int row, object value)
        {
            object baseValue = null;
            if (true.Equals(value))
            {
                baseValue = TrueValue;
            }
            else if (false.Equals(value))
            {
                baseValue = FalseValue;
            }
            else if (Convert.IsDBNull(value))
            {
                baseValue = NullValue;
            }
            currentValue = baseValue;
            base.SetColumnValueAtRow(lm, row, baseValue);
        }

        /// <summary>
        ///  Gets the optimum width and height of a cell given
        ///  a specific value to contain.
        /// </summary>
        protected internal override Size GetPreferredSize(Graphics g, object value)
        {
            return new Size(idealCheckSize + 2, idealCheckSize + 2);
        }

        /// <summary>
        ///  Gets
        ///  the height of a cell in a column.
        /// </summary>
        protected internal override int GetMinimumHeight()
        {
            return idealCheckSize + 2;
        }

        /// <summary>
        ///  Gets the height used when resizing columns.
        /// </summary>
        protected internal override int GetPreferredHeight(Graphics g, object value)
        {
            return idealCheckSize + 2;
        }

        /// <summary>
        ///  Initiates a request to interrupt an edit procedure.
        /// </summary>
        protected internal override void Abort(int rowNum)
        {
            isSelected = false;
            isEditing = false;
            Invalidate();
            return;
        }

        /// <summary>
        ///  Initiates a request to complete an editing procedure.
        /// </summary>
        protected internal override bool Commit(CurrencyManager dataSource, int rowNum)
        {
            isSelected = false;
            // always invalidate
            Invalidate();
            if (!isEditing)
            {
                return true;
            }

            SetColumnValueAtRow(dataSource, rowNum, currentValue);
            isEditing = false;
            return true;
        }

        /// <summary>
        ///  Prepares the cell for editing a value.
        /// </summary>
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
            DataGrid grid = DataGridTableStyle.DataGrid;
            if (!grid.Focused)
            {
                grid.Focus();
            }

            if (!readOnly && !IsReadOnly())
            {
                editingRow = rowNum;
                currentValue = GetColumnValueAtRow(source, rowNum);
            }

            base.Invalidate();
        }

        /// <summary>
        ///  Provides a handler for determining which key was pressed, and whether to
        ///  process it.
        /// </summary>
        internal override bool KeyPress(int rowNum, Keys keyData)
        {
            if (isSelected && editingRow == rowNum && !IsReadOnly())
            {
                if ((keyData & Keys.KeyCode) == Keys.Space)
                {
                    ToggleValue();
                    Invalidate();
                    return true;
                }
            }
            return base.KeyPress(rowNum, keyData);
        }

        /// <summary>
        ///  Indicates whether the a mouse down event occurred at the specified row, at
        ///  the specified x and y coordinates.
        /// </summary>
        internal override bool MouseDown(int rowNum, int x, int y)
        {
            base.MouseDown(rowNum, x, y);
            if (isSelected && editingRow == rowNum && !IsReadOnly())
            {
                ToggleValue();
                Invalidate();
                return true;
            }
            return false;
        }

        private void OnTrueValueChanged(EventArgs e)
        {
            if (Events[EventTrueValue] is EventHandler eh)
            {
                eh(this, e);
            }
        }
        private void OnFalseValueChanged(EventArgs e)
        {
            if (Events[EventFalseValue] is EventHandler eh)
            {
                eh(this, e);
            }
        }
        private void OnAllowNullChanged(EventArgs e)
        {
            if (Events[EventAllowNull] is EventHandler eh)
            {
                eh(this, e);
            }
        }

        /// <summary>
        ///  Draws the <see cref='DataGridBoolColumn'/>
        ///  with the given <see cref='Graphics'/>,
        /// <see cref='Rectangle'/> and row number.
        /// </summary>
        protected internal override void Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum)
        {
            Paint(g, bounds, source, rowNum, false);
        }

        /// <summary>
        ///  Draws the <see cref='DataGridBoolColumn'/>
        ///  with the given <see cref='Graphics'/>, <see cref='Rectangle'/>,
        ///  row number, and alignment settings.
        /// </summary>
        protected internal override void Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum, bool alignToRight)
        {
            Paint(g, bounds, source, rowNum, DataGridTableStyle.BackBrush, DataGridTableStyle.ForeBrush, alignToRight);
        }

        /// <summary>
        ///  Draws the <see cref='DataGridBoolColumn'/> with the given <see cref='Graphics'/>, <see cref='Rectangle'/>,
        ///  row number, <see cref='Brush'/>, and <see cref='Color'/>.
        /// </summary>
        protected internal override void Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum,
                                     Brush backBrush, Brush foreBrush,
                                     bool alignToRight)
        {
            object value = (isEditing && editingRow == rowNum) ? currentValue : GetColumnValueAtRow(source, rowNum);
            ButtonState checkedState = ButtonState.Inactive;
            if (!Convert.IsDBNull(value))
            {
                checkedState = ((bool)value ? ButtonState.Checked : ButtonState.Normal);
            }

            Rectangle box = GetCheckBoxBounds(bounds, alignToRight);

            Region r = g.Clip;
            g.ExcludeClip(box);

            Brush selectionBrush = DataGridTableStyle.IsDefault ? DataGridTableStyle.DataGrid.SelectionBackBrush : DataGridTableStyle.SelectionBackBrush;
            if (isSelected && editingRow == rowNum && !IsReadOnly())
            {
                g.FillRectangle(selectionBrush, bounds);
            }
            else
            {
                g.FillRectangle(backBrush, bounds);
            }

            g.Clip = r;

            if (checkedState == ButtonState.Inactive)
            {
                ControlPaint.DrawMixedCheckBox(g, box, ButtonState.Checked);
            }
            else
            {
                ControlPaint.DrawCheckBox(g, box, checkedState);
            }

            // if the column is read only we should still show selection
            if (IsReadOnly() && isSelected && source.Position == rowNum)
            {
                bounds.Inflate(-1, -1);
                Pen pen = new Pen(selectionBrush)
                {
                    DashStyle = System.Drawing.Drawing2D.DashStyle.Dash
                };
                g.DrawRectangle(pen, bounds);
                pen.Dispose();
                // restore the bounds rectangle
                bounds.Inflate(1, 1);
            }
        }

        /// <summary>
        ///  Gets or sets a value indicating whether null values are allowed.
        /// </summary>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(true),
        SRDescription(nameof(SR.DataGridBoolColumnAllowNullValue))
        ]
        public bool AllowNull
        {
            get
            {
                return allowNull;
            }
            set
            {
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

        public event EventHandler AllowNullChanged
        {
            add => Events.AddHandler(EventAllowNull, value);
            remove => Events.RemoveHandler(EventAllowNull, value);
        }

        /// <summary>
        ///  Enters a <see langword='null'/> into the column.
        /// </summary>
        protected internal override void EnterNullValue()
        {
            // do not throw an exception when the column is marked as readOnly or
            // does not allowNull
            if (!AllowNull || IsReadOnly())
            {
                return;
            }

            if (currentValue != Convert.DBNull)
            {
                currentValue = Convert.DBNull;
                Invalidate();
            }
        }

        private void ResetNullValue()
        {
            NullValue = Convert.DBNull;
        }

        private bool ShouldSerializeNullValue()
        {
            return nullValue != Convert.DBNull;
        }

        private void ToggleValue()
        {
            if (currentValue is bool && ((bool)currentValue) == false)
            {
                currentValue = true;
            }
            else
            {
                if (AllowNull)
                {
                    if (Convert.IsDBNull(currentValue))
                    {
                        currentValue = false;
                    }
                    else
                    {
                        currentValue = Convert.DBNull;
                    }
                }
                else
                {
                    currentValue = false;
                }
            }
            // we started editing
            isEditing = true;
            // tell the dataGrid that things are changing
            // we put Rectangle.Empty cause toggle will invalidate the row anyhow
            DataGridTableStyle.DataGrid.ColumnStartedEditing(Rectangle.Empty);
        }
    }
}
