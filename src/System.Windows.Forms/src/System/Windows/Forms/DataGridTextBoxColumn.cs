// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Hosts a System.Windows.Forms.TextBox control in a cell of a System.Windows.Forms.DataGridColumnStyle for editing strings.
    /// </summary>
    public class DataGridTextBoxColumn : DataGridColumnStyle
    {
        // ui State
        private readonly int xMargin = 2;
        private readonly int yMargin = 1;
        // private int   fontHandle =        0;
        private string format = null;
        private TypeConverter typeConverter;
        private IFormatProvider formatInfo = null;
        private Reflection.MethodInfo parseMethod;

        // hosted control
        private readonly DataGridTextBox edit;

        // editing state
        private string oldValue = null;
        private int editRow = -1;

        /// <summary>
        ///  Initializes a new instance of the System.Windows.Forms.DataGridTextBoxColumn
        ///  class.
        /// </summary>
        public DataGridTextBoxColumn() : this(null, null)
        {
        }

        /// <summary>
        ///  Initializes a new instance of a System.Windows.Forms.DataGridTextBoxColumn with
        ///  a specified System.Data.DataColumn.
        /// </summary>
        public DataGridTextBoxColumn(PropertyDescriptor prop)
        : this(prop, null, false)
        {
        }

        /// <summary>
        ///  Initializes a new instance of a System.Windows.Forms.DataGridTextBoxColumn. with
        ///  the specified System.Data.DataColumn and System.Windows.Forms.ComponentModel.Format.
        /// </summary>
        public DataGridTextBoxColumn(PropertyDescriptor prop, string format) : this(prop, format, false) { }

        public DataGridTextBoxColumn(PropertyDescriptor prop, string format, bool isDefault) : base(prop, isDefault)
        {
            edit = new DataGridTextBox
            {
                BorderStyle = BorderStyle.None,
                Multiline = true,
                AcceptsReturn = true,
                Visible = false
            };
            Format = format;
        }

        public DataGridTextBoxColumn(PropertyDescriptor prop, bool isDefault) : this(prop, null, isDefault) { }

        // =------------------------------------------------------------------
        // =        Properties
        // =------------------------------------------------------------------

        /// <summary>
        ///  Gets the hosted System.Windows.Forms.TextBox control.
        /// </summary>
        [Browsable(false)]
        public virtual TextBox TextBox
        {
            get
            {
                return edit;
            }
        }

        internal override bool KeyPress(int rowNum, Keys keyData)
        {
            if (edit.IsInEditOrNavigateMode)
            {
                return base.KeyPress(rowNum, keyData);
            }

            // if the edit box is editing, then
            // pass this keystroke to the edit box
            //
            return false;
        }

        /// <summary>
        ///  Adds a System.Windows.Forms.TextBox control to the System.Windows.Forms.DataGrid control's System.Windows.Forms.Control.ControlCollection
        ///  .
        /// </summary>
        protected override void SetDataGridInColumn(DataGrid value)
        {
            base.SetDataGridInColumn(value);
            if (edit.ParentInternal != null)
            {
                edit.ParentInternal.Controls.Remove(edit);
            }
            if (value != null)
            {
                value.Controls.Add(edit);
            }

            // we have to tell the edit control about its dataGrid
            edit.SetDataGrid(value);
        }

        /* CUT as part of the new DataGridTableStyleSheet thing
        public override Font Font {
            set {
                base.Font = value;
                Font f = base.Font;
                edit.Font = f;
                // if (f != null) {
                    // fontHandle = f.Handle;
                // }
            }
        }
        */

        /// <summary>
        ///  Gets or sets the System.Windows.Forms.ComponentModel.Format for the System.Windows.Forms.DataGridTextBoxColumn
        ///  .
        /// </summary>
        [
        SRDescription(nameof(SR.FormatControlFormatDescr)),
        DefaultValue(null)
        ]
        public override PropertyDescriptor PropertyDescriptor
        {
            set
            {
                base.PropertyDescriptor = value;
                if (PropertyDescriptor != null)
                {
                    if (PropertyDescriptor.PropertyType != typeof(object))
                    {
                        typeConverter = TypeDescriptor.GetConverter(PropertyDescriptor.PropertyType);
                        parseMethod = PropertyDescriptor.PropertyType.GetMethod("Parse", new Type[] { typeof(string), typeof(IFormatProvider) });
                    }
                }
            }
        }

        // add the corresponding value Editor: rip one from the valueEditor for the DisplayMember in the
        // format object
        [DefaultValue(null), Editor("System.Windows.Forms.Design.DataGridColumnStyleFormatEditor, " + AssemblyRef.SystemDesign, typeof(Drawing.Design.UITypeEditor))]
        public string Format
        {
            get
            {
                return format;
            }
            set
            {
                if (value == null)
                {
                    value = string.Empty;
                }

                if (format == null || !format.Equals(value))
                {
                    format = value;

                    // if the associated typeConverter cannot convert from string,
                    // then we can't modify the column value. hence, make it readOnly
                    //
                    if (format.Length == 0)
                    {
                        if (typeConverter != null && !typeConverter.CanConvertFrom(typeof(string)))
                        {
                            ReadOnly = true;
                        }
                    }

                    Invalidate();
                }
            }
        }

        [Browsable(false), EditorBrowsable(EditorBrowsableState.Advanced)]
        public IFormatProvider FormatInfo
        {
            get
            {
                return formatInfo;
            }
            set
            {
                if (formatInfo == null || !formatInfo.Equals(value))
                {
                    formatInfo = value;
                }
            }
        }

        public override bool ReadOnly
        {
            get
            {
                return base.ReadOnly;
            }
            set
            {
                // if the gridColumn is can't convert the string to
                // the backGround propertyDescriptor, then make the column ReadOnly
                if (!value && (format == null || format.Length == 0))
                {
                    if (typeConverter != null && !typeConverter.CanConvertFrom(typeof(string)))
                    {
                        return;
                    }
                }
                base.ReadOnly = value;
            }
        }

        // =------------------------------------------------------------------
        // =        Methods
        // =------------------------------------------------------------------

        private void DebugOut(string s)
        {
            Debug.WriteLineIf(CompModSwitches.DGEditColumnEditing.TraceVerbose, "DGEditColumnEditing: " + s);
        }

        // will hide the edit control
        /// <summary>
        ///  Informs the column the focus is being conceded.
        /// </summary>
        protected internal override void ConcedeFocus()
        {
            edit.Bounds = Rectangle.Empty;
            // edit.Visible = false;
            // HideEditBox();
        }

        /// <summary>
        ///  Hides the System.Windows.Forms.TextBox
        ///  control and moves the focus to the System.Windows.Forms.DataGrid
        ///  control.
        /// </summary>
        protected void HideEditBox()
        {
            bool wasFocused = edit.Focused;
            edit.Visible = false;

            // it seems that edit.Visible = false will take away the focus from
            // the edit control. And this means that we will not give the focus to the grid
            // If all the columns would have an edit control this would not be bad
            // ( or if the grid is the only control on the form ),
            // but when we have a DataGridBoolColumn then the focus will be taken away
            // by the next control in the form.
            //
            // if (edit.Focused && this.DataGridTableStyle.DataGrid.CanFocus) {

            // when the user deletes the current ( ie active ) column from the
            // grid, the grid should still call EndEdit ( so that the changes that the user made
            // before deleting the column will go to the backEnd)
            // however, in that situation, we are left w/ the editColumn which is not parented.
            // the grid will call Edit to reset the EditColumn
            if (wasFocused && DataGridTableStyle != null && DataGridTableStyle.DataGrid != null && DataGridTableStyle.DataGrid.CanFocus)
            {
                DataGridTableStyle.DataGrid.Focus();
                Debug.Assert(!edit.Focused, "the edit control just conceeded focus to the dataGrid");
            }
        }

        protected internal override void UpdateUI(CurrencyManager source, int rowNum, string displayText)
        {
            edit.Text = GetText(GetColumnValueAtRow(source, rowNum));
            if (!edit.ReadOnly && displayText != null)
            {
                edit.Text = displayText;
            }
        }

        /// <summary>
        ///  Ends an edit operation on the System.Windows.Forms.DataGridColumnStyle
        ///  .
        /// </summary>
        protected void EndEdit()
        {
            edit.IsInEditOrNavigateMode = true;
            DebugOut("Ending Edit");
            Invalidate();
        }

        /// <summary>
        ///  Returns the optimum width and
        ///  height of the cell in a specified row relative
        ///  to the specified value.
        /// </summary>
        protected internal override Size GetPreferredSize(Graphics g, object value)
        {
            Size extents = Size.Ceiling(g.MeasureString(GetText(value), DataGridTableStyle.DataGrid.Font));
            extents.Width += xMargin * 2 + DataGridTableStyle.GridLineWidth;
            extents.Height += yMargin;
            return extents;
        }

        /// <summary>
        ///  Gets the height of a cell in a System.Windows.Forms.DataGridColumnStyle
        ///  .
        /// </summary>
        protected internal override int GetMinimumHeight()
        {
            // why + 3? cause we have to give some way to the edit box.
            return FontHeight + yMargin + 3;
        }

        /// <summary>
        ///  Gets the height to be used in for automatically resizing columns.
        /// </summary>
        protected internal override int GetPreferredHeight(Graphics g, object value)
        {
            int newLineIndex = 0;
            int newLines = 0;
            string valueString = GetText(value);
            while (newLineIndex != -1 && newLineIndex < valueString.Length)
            {
                newLineIndex = valueString.IndexOf("\r\n", newLineIndex + 1);
                newLines++;
            }

            return FontHeight * newLines + yMargin;
        }

        /// <summary>
        ///  Initiates a request to interrupt an edit procedure.
        /// </summary>
        protected internal override void Abort(int rowNum)
        {
            RollBack();
            HideEditBox();
            EndEdit();
        }

        // used for Alt0 functionality
        /// <summary>
        ///  Enters a <see langword='null'/> in the column.
        /// </summary>
        protected internal override void EnterNullValue()
        {
            if (ReadOnly)
            {
                return;
            }

            // if the edit box is not visible, then
            // do not put the edit text in it
            if (!edit.Visible)
            {
                return;
            }

            // if we are editing, then we should be able to enter alt-0 in a cell.
            //
            if (!edit.IsInEditOrNavigateMode)
            {
                return;
            }

            edit.Text = NullText;
            // edit.Visible = true;
            edit.IsInEditOrNavigateMode = false;
            // tell the dataGrid that there is an edit:
            if (DataGridTableStyle != null && DataGridTableStyle.DataGrid != null)
            {
                DataGridTableStyle.DataGrid.ColumnStartedEditing(edit.Bounds);
            }
        }

        /// <summary>
        ///  Inititates a request to complete an editing procedure.
        /// </summary>
        protected internal override bool Commit(CurrencyManager dataSource, int rowNum)
        {
            // always hide the edit box
            // HideEditBox();
            edit.Bounds = Rectangle.Empty;

            if (edit.IsInEditOrNavigateMode)
            {
                return true;
            }

            try
            {
                object value = edit.Text;
                if (NullText.Equals(value))
                {
                    value = Convert.DBNull;
                    edit.Text = NullText;
                }
                else if (format != null && format.Length != 0 && parseMethod != null && FormatInfo != null)
                {
                    // use reflection to get the Parse method on the
                    // type of the propertyDescriptor.
                    value = (object)parseMethod.Invoke(null, new object[] { edit.Text, FormatInfo });
                    if (value is IFormattable)
                    {
                        edit.Text = ((IFormattable)value).ToString(format, formatInfo);
                    }
                    else
                    {
                        edit.Text = value.ToString();
                    }
                }
                else if (typeConverter != null && typeConverter.CanConvertFrom(typeof(string)))
                {
                    value = typeConverter.ConvertFromString(edit.Text);
                    edit.Text = typeConverter.ConvertToString(value);
                }

                SetColumnValueAtRow(dataSource, rowNum, value);
            }
            catch
            {
                // MessageBox.Show("There was an error caught setting field \""
                //                 + this.PropertyDescriptor.Name + "\" to the value \"" + edit.Text + "\"\n"
                //                 + "The value is being rolled back to the original.\n"
                //                 + "The error was a '" + e.Message + "' "  + e.StackTrace
                //                 , "Error commiting changes...", MessageBox.IconError);
                // Debug.WriteLine(e.GetType().Name);
                RollBack();
                return false;
            }
            DebugOut("OnCommit completed without Exception.");
            EndEdit();
            return true;
        }

        /// <summary>
        ///  Prepares a cell for editing.
        /// </summary>
        protected internal override void Edit(CurrencyManager source,
                                    int rowNum,
                                    Rectangle bounds,
                                    bool readOnly,
                                    string displayText,
                                    bool cellIsVisible)
        {
            DebugOut("Begining Edit, rowNum :" + rowNum.ToString(CultureInfo.InvariantCulture));

            Rectangle originalBounds = bounds;

            edit.ReadOnly = readOnly || ReadOnly || DataGridTableStyle.ReadOnly;

            edit.Text = GetText(GetColumnValueAtRow(source, rowNum));
            if (!edit.ReadOnly && displayText != null)
            {
                // tell the grid that we are changing stuff
                DataGridTableStyle.DataGrid.ColumnStartedEditing(bounds);
                // tell the edit control that the user changed it
                edit.IsInEditOrNavigateMode = false;
                edit.Text = displayText;
            }

            if (cellIsVisible)
            {
                bounds.Offset(xMargin, 2 * yMargin);
                bounds.Width -= xMargin;
                bounds.Height -= 2 * yMargin;
                DebugOut("edit bounds: " + bounds.ToString());
                edit.Bounds = bounds;

                edit.Visible = true;

                edit.TextAlign = Alignment;
            }
            else
            {
                edit.Bounds = Rectangle.Empty;
                // edit.Bounds = originalBounds;
                // edit.Visible = false;
            }

            edit.RightToLeft = DataGridTableStyle.DataGrid.RightToLeft;

            edit.Focus();

            editRow = rowNum;

            if (!edit.ReadOnly)
            {
                oldValue = edit.Text;
            }

            // select the text even if the text box is read only
            // because the navigation code in the DataGridTextBox::ProcessKeyMessage
            // uses the SelectedText property
            if (displayText == null)
            {
                edit.SelectAll();
            }
            else
            {
                int end = edit.Text.Length;
                edit.Select(end, 0);
            }

            if (edit.Visible)
            {
                DataGridTableStyle.DataGrid.Invalidate(originalBounds);
            }
        }

        internal override string GetDisplayText(object value)
        {
            return GetText(value);
        }

        private string GetText(object value)
        {
            if (value is DBNull)
            {
                return NullText;
            }
            else if (format != null && format.Length != 0 && (value is IFormattable))
            {
                try
                {
                    return ((IFormattable)value).ToString(format, formatInfo);
                }
                catch
                {
                    //
                }
            }
            else
            {
                // use the typeConverter:
                if (typeConverter != null && typeConverter.CanConvertTo(typeof(string)))
                {
                    return (string)typeConverter.ConvertTo(value, typeof(string));
                }
            }
            return (value != null ? value.ToString() : "");
        }

        /// <summary>
        ///  Paints the a System.Windows.Forms.DataGridColumnStyle with the specified System.Drawing.Graphics,
        ///  System.Drawing.Rectangle, DataView.Rectangle, and row number.
        /// </summary>
        protected internal override void Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum)
        {
            Paint(g, bounds, source, rowNum, false);
        }

        /// <summary>
        ///  Paints a System.Windows.Forms.DataGridColumnStyle with the specified System.Drawing.Graphics, System.Drawing.Rectangle, DataView, row number, and alignment.
        /// </summary>
        protected internal override void Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum, bool alignToRight)
        {
            string text = GetText(GetColumnValueAtRow(source, rowNum));
            PaintText(g, bounds, text, alignToRight);
        }

        /// <summary>
        ///  Paints a System.Windows.Forms.DataGridColumnStyle with the specified System.Drawing.Graphics,
        ///  System.Drawing.Rectangle, DataView.Rectangle, row number, background color,
        ///  and foreground color..
        /// </summary>
        protected internal override void Paint(Graphics g, Rectangle bounds, CurrencyManager source, int rowNum,
                                     Brush backBrush, Brush foreBrush, bool alignToRight)
        {
            string text = GetText(GetColumnValueAtRow(source, rowNum));
            PaintText(g, bounds, text, backBrush, foreBrush, alignToRight);
        }

        /// <summary>
        ///  Draws the text and
        ///  rectangle at the given location with the specified alignment.
        /// </summary>
        protected void PaintText(Graphics g, Rectangle bounds, string text, bool alignToRight)
        {
            PaintText(g, bounds, text, DataGridTableStyle.BackBrush, DataGridTableStyle.ForeBrush, alignToRight);
        }

        /// <summary>
        ///  Draws the text and rectangle at the specified location with the
        ///  specified colors and alignment.
        /// </summary>
        protected void PaintText(Graphics g, Rectangle textBounds, string text, Brush backBrush, Brush foreBrush, bool alignToRight)
        {
            /*
            if (edit.Visible)
                g.BackColor = BackColor;
            */

            Rectangle rect = textBounds;

            StringFormat format = new StringFormat();
            if (alignToRight)
            {
                format.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
            }

            format.Alignment = Alignment == HorizontalAlignment.Left ? StringAlignment.Near : Alignment == HorizontalAlignment.Center ? StringAlignment.Center : StringAlignment.Far;

            // do not wrap the text
            //
            format.FormatFlags |= StringFormatFlags.NoWrap;

            g.FillRectangle(backBrush, rect);
            // by design, painting  leaves a little padding around the rectangle.
            // so do not deflate the rectangle.
            rect.Offset(0, 2 * yMargin);
            rect.Height -= 2 * yMargin;
            g.DrawString(text, DataGridTableStyle.DataGrid.Font, foreBrush, rect, format);
            format.Dispose();
        }

        private void RollBack()
        {
            Debug.Assert(!edit.IsInEditOrNavigateMode, "Must be editing to rollback changes...");
            edit.Text = oldValue;
        }

        protected internal override void ReleaseHostedControl()
        {
            if (edit.ParentInternal != null)
            {
                edit.ParentInternal.Controls.Remove(edit);
            }
        }
    }
}
