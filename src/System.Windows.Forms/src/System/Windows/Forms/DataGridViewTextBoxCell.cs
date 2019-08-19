// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Drawing;
using System.ComponentModel;
using System.Globalization;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Identifies a cell in the dataGridView.
    /// </summary>
    public class DataGridViewTextBoxCell : DataGridViewCell
    {
        private static readonly int PropTextBoxCellMaxInputLength = PropertyStore.CreateKey();
        private static readonly int PropTextBoxCellEditingTextBox = PropertyStore.CreateKey();

        private const byte DATAGRIDVIEWTEXTBOXCELL_ignoreNextMouseClick = 0x01;
        private const byte DATAGRIDVIEWTEXTBOXCELL_horizontalTextOffsetLeft = 3;
        private const byte DATAGRIDVIEWTEXTBOXCELL_horizontalTextOffsetRight = 4;
        private const byte DATAGRIDVIEWTEXTBOXCELL_horizontalTextMarginLeft = 0;
        private const byte DATAGRIDVIEWTEXTBOXCELL_horizontalTextMarginRight = 0;
        private const byte DATAGRIDVIEWTEXTBOXCELL_verticalTextOffsetTop = 2;
        private const byte DATAGRIDVIEWTEXTBOXCELL_verticalTextOffsetBottom = 1;
        private const byte DATAGRIDVIEWTEXTBOXCELL_verticalTextMarginTopWithWrapping = 1;
        private const byte DATAGRIDVIEWTEXTBOXCELL_verticalTextMarginTopWithoutWrapping = 2;
        private const byte DATAGRIDVIEWTEXTBOXCELL_verticalTextMarginBottom = 1;

        private const int DATAGRIDVIEWTEXTBOXCELL_maxInputLength = 32767;

        private byte flagsState;  // see DATAGRIDVIEWTEXTBOXCELL_ consts above

        private static readonly Type defaultFormattedValueType = typeof(string);
        private static readonly Type defaultValueType = typeof(object);
        private static readonly Type cellType = typeof(DataGridViewTextBoxCell);

        public DataGridViewTextBoxCell()
        {
        }

        /// <summary>
        ///  Creates a new AccessibleObject for this DataGridViewTextBoxCell instance.
        ///  The AccessibleObject instance returned by this method supports ControlType UIA property.
        /// </summary>
        /// <returns>
        ///  AccessibleObject for this DataGridViewTextBoxCell instance.
        /// </returns>
        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new DataGridViewTextBoxCellAccessibleObject(this);
        }

        private DataGridViewTextBoxEditingControl EditingTextBox
        {
            get
            {
                return (DataGridViewTextBoxEditingControl)Properties.GetObject(PropTextBoxCellEditingTextBox);
            }
            set
            {
                if (value != null || Properties.ContainsObject(PropTextBoxCellEditingTextBox))
                {
                    Properties.SetObject(PropTextBoxCellEditingTextBox, value);
                }
            }
        }

        public override Type FormattedValueType
        {
            get
            {
                // we return string for the formatted type
                return defaultFormattedValueType;
            }
        }

        [DefaultValue(DATAGRIDVIEWTEXTBOXCELL_maxInputLength)]
        public virtual int MaxInputLength
        {
            get
            {
                int maxInputLength = Properties.GetInteger(PropTextBoxCellMaxInputLength, out bool found);
                if (found)
                {
                    return maxInputLength;
                }
                return DATAGRIDVIEWTEXTBOXCELL_maxInputLength;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), string.Format(SR.InvalidLowBoundArgumentEx, nameof(MaxInputLength), value, 0));
                }

                Properties.SetInteger(PropTextBoxCellMaxInputLength, value);
                if (OwnsEditingTextBox(RowIndex))
                {
                    EditingTextBox.MaxLength = value;
                }
            }
        }

        public override Type ValueType
        {
            get
            {
                Type valueType = base.ValueType;
                if (valueType != null)
                {
                    return valueType;
                }
                return defaultValueType;
            }
        }

        // Called when the row that owns the editing control gets unshared.
        internal override void CacheEditingControl()
        {
            EditingTextBox = DataGridView.EditingControl as DataGridViewTextBoxEditingControl;
        }

        public override object Clone()
        {
            DataGridViewTextBoxCell dataGridViewCell;
            Type thisType = GetType();
            if (thisType == cellType) //performance improvement
            {
                dataGridViewCell = new DataGridViewTextBoxCell();
            }
            else
            {
                //

                dataGridViewCell = (DataGridViewTextBoxCell)System.Activator.CreateInstance(thisType);
            }
            base.CloneInternal(dataGridViewCell);
            dataGridViewCell.MaxInputLength = MaxInputLength;
            return dataGridViewCell;
        }

        [
            EditorBrowsable(EditorBrowsableState.Advanced)
        ]
        public override void DetachEditingControl()
        {
            DataGridView dgv = DataGridView;
            if (dgv == null || dgv.EditingControl == null)
            {
                throw new InvalidOperationException();
            }

            if (dgv.EditingControl is TextBox textBox)
            {
                textBox.ClearUndo();
            }

            EditingTextBox = null;

            base.DetachEditingControl();
        }

        private Rectangle GetAdjustedEditingControlBounds(Rectangle editingControlBounds, DataGridViewCellStyle cellStyle)
        {
            Debug.Assert(cellStyle.WrapMode != DataGridViewTriState.NotSet);
            Debug.Assert(DataGridView != null);

            int originalWidth = editingControlBounds.Width;
            if (DataGridView.EditingControl is TextBox txtEditingControl)
            {
                switch (cellStyle.Alignment)
                {
                    case DataGridViewContentAlignment.TopLeft:
                    case DataGridViewContentAlignment.MiddleLeft:
                    case DataGridViewContentAlignment.BottomLeft:
                        // Add 3 pixels on the left of the editing control to match non-editing text position
                        if (DataGridView.RightToLeftInternal)
                        {
                            editingControlBounds.X += 1;
                            editingControlBounds.Width = Math.Max(0, editingControlBounds.Width - DATAGRIDVIEWTEXTBOXCELL_horizontalTextOffsetLeft - 2);
                        }
                        else
                        {
                            editingControlBounds.X += DATAGRIDVIEWTEXTBOXCELL_horizontalTextOffsetLeft;
                            editingControlBounds.Width = Math.Max(0, editingControlBounds.Width - DATAGRIDVIEWTEXTBOXCELL_horizontalTextOffsetLeft - 1);
                        }
                        break;

                    case DataGridViewContentAlignment.TopCenter:
                    case DataGridViewContentAlignment.MiddleCenter:
                    case DataGridViewContentAlignment.BottomCenter:
                        editingControlBounds.X += 1;
                        editingControlBounds.Width = Math.Max(0, editingControlBounds.Width - 3);
                        break;

                    case DataGridViewContentAlignment.TopRight:
                    case DataGridViewContentAlignment.MiddleRight:
                    case DataGridViewContentAlignment.BottomRight:
                        // Shorten the editing control by 5 pixels to match non-editing text position
                        if (DataGridView.RightToLeftInternal)
                        {
                            editingControlBounds.X += DATAGRIDVIEWTEXTBOXCELL_horizontalTextOffsetRight - 1;
                            editingControlBounds.Width = Math.Max(0, editingControlBounds.Width - DATAGRIDVIEWTEXTBOXCELL_horizontalTextOffsetRight);
                        }
                        else
                        {
                            editingControlBounds.X += 1;
                            editingControlBounds.Width = Math.Max(0, editingControlBounds.Width - DATAGRIDVIEWTEXTBOXCELL_horizontalTextOffsetRight - 1);
                        }
                        break;
                }

                switch (cellStyle.Alignment)
                {
                    case DataGridViewContentAlignment.TopLeft:
                    case DataGridViewContentAlignment.TopCenter:
                    case DataGridViewContentAlignment.TopRight:
                        editingControlBounds.Y += DATAGRIDVIEWTEXTBOXCELL_verticalTextOffsetTop;
                        editingControlBounds.Height = Math.Max(0, editingControlBounds.Height - DATAGRIDVIEWTEXTBOXCELL_verticalTextOffsetTop);
                        break;

                    case DataGridViewContentAlignment.MiddleLeft:
                    case DataGridViewContentAlignment.MiddleCenter:
                    case DataGridViewContentAlignment.MiddleRight:
                        editingControlBounds.Height++;
                        break;

                    case DataGridViewContentAlignment.BottomLeft:
                    case DataGridViewContentAlignment.BottomCenter:
                    case DataGridViewContentAlignment.BottomRight:
                        editingControlBounds.Height = Math.Max(0, editingControlBounds.Height - DATAGRIDVIEWTEXTBOXCELL_verticalTextOffsetBottom);
                        break;
                }

                int preferredHeight;
                if (cellStyle.WrapMode == DataGridViewTriState.False)
                {
                    preferredHeight = txtEditingControl.PreferredSize.Height;
                }
                else
                {
                    string editedFormattedValue = (string)((IDataGridViewEditingControl)txtEditingControl).GetEditingControlFormattedValue(DataGridViewDataErrorContexts.Formatting);
                    if (string.IsNullOrEmpty(editedFormattedValue))
                    {
                        editedFormattedValue = " ";
                    }
                    TextFormatFlags flags = DataGridViewUtilities.ComputeTextFormatFlagsForCellStyleAlignment(DataGridView.RightToLeftInternal, cellStyle.Alignment, cellStyle.WrapMode);
                    using (Graphics g = WindowsFormsUtils.CreateMeasurementGraphics())
                    {
                        preferredHeight = DataGridViewCell.MeasureTextHeight(g, editedFormattedValue, cellStyle.Font, originalWidth, flags);
                    }
                }
                if (preferredHeight < editingControlBounds.Height)
                {
                    switch (cellStyle.Alignment)
                    {
                        case DataGridViewContentAlignment.TopLeft:
                        case DataGridViewContentAlignment.TopCenter:
                        case DataGridViewContentAlignment.TopRight:
                            // Single pixel move - leave it as is for now
                            break;
                        case DataGridViewContentAlignment.MiddleLeft:
                        case DataGridViewContentAlignment.MiddleCenter:
                        case DataGridViewContentAlignment.MiddleRight:
                            editingControlBounds.Y += (editingControlBounds.Height - preferredHeight) / 2;
                            break;
                        case DataGridViewContentAlignment.BottomLeft:
                        case DataGridViewContentAlignment.BottomCenter:
                        case DataGridViewContentAlignment.BottomRight:
                            editingControlBounds.Y += editingControlBounds.Height - preferredHeight;
                            break;
                    }
                }
            }

            return editingControlBounds;
        }

        protected override Rectangle GetContentBounds(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex)
        {
            if (cellStyle == null)
            {
                throw new ArgumentNullException(nameof(cellStyle));
            }

            if (DataGridView == null || rowIndex < 0 || OwningColumn == null)
            {
                return Rectangle.Empty;
            }

            object value = GetValue(rowIndex);
            object formattedValue = GetFormattedValue(value, rowIndex, ref cellStyle, null, null, DataGridViewDataErrorContexts.Formatting);

            ComputeBorderStyleCellStateAndCellBounds(rowIndex, out DataGridViewAdvancedBorderStyle dgvabsEffective, out DataGridViewElementStates cellState, out Rectangle cellBounds);

            Rectangle textBounds = PaintPrivate(graphics,
                cellBounds,
                cellBounds,
                rowIndex,
                cellState,
                formattedValue,
                null /*errorText*/,                 // textBounds is independent of errorText
                cellStyle,
                dgvabsEffective,
                DataGridViewPaintParts.ContentForeground,
                true /*computeContentBounds*/,
                false /*computeErrorIconBounds*/,
                false /*paint*/);

#if DEBUG
            Rectangle textBoundsDebug = PaintPrivate(graphics,
                cellBounds,
                cellBounds,
                rowIndex,
                cellState,
                formattedValue,
                GetErrorText(rowIndex),
                cellStyle,
                dgvabsEffective,
                DataGridViewPaintParts.ContentForeground,
                true /*computeContentBounds*/,
                false /*computeErrorIconBounds*/,
                false /*paint*/);
            Debug.Assert(textBoundsDebug.Equals(textBounds));
#endif

            return textBounds;
        }

        private protected override string GetDefaultToolTipText()
        {
            if (string.IsNullOrEmpty(Value?.ToString()?.Trim(' ')) || Value is DBNull)
            {
                return SR.DefaultDataGridViewTextBoxCellTollTipText;
            }

            return null;
        }

        protected override Rectangle GetErrorIconBounds(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex)
        {
            if (cellStyle == null)
            {
                throw new ArgumentNullException(nameof(cellStyle));
            }

            if (DataGridView == null ||
                rowIndex < 0 ||
                OwningColumn == null ||
                !DataGridView.ShowCellErrors ||
                string.IsNullOrEmpty(GetErrorText(rowIndex)))
            {
                return Rectangle.Empty;
            }

            ComputeBorderStyleCellStateAndCellBounds(rowIndex, out DataGridViewAdvancedBorderStyle dgvabsEffective, out DataGridViewElementStates cellState, out Rectangle cellBounds);

            Rectangle errorBounds = PaintPrivate(graphics,
                cellBounds,
                cellBounds,
                rowIndex,
                cellState,
                null /*formattedValue*/,            // errorIconBounds is independent of formattedValue
                GetErrorText(rowIndex),
                cellStyle,
                dgvabsEffective,
                DataGridViewPaintParts.ContentForeground,
                false /*computeContentBounds*/,
                true /*computeErrorIconBounds*/,
                false /*paint*/);

#if DEBUG
            object value = GetValue(rowIndex);
            object formattedValue = GetFormattedValue(value, rowIndex, ref cellStyle, null, null, DataGridViewDataErrorContexts.Formatting);

            Rectangle errorBoundsDebug = PaintPrivate(graphics,
                cellBounds,
                cellBounds,
                rowIndex,
                cellState,
                formattedValue,
                GetErrorText(rowIndex),
                cellStyle,
                dgvabsEffective,
                DataGridViewPaintParts.ContentForeground,
                false /*computeContentBounds*/,
                true /*computeErrorIconBounds*/,
                false /*paint*/);
            Debug.Assert(errorBoundsDebug.Equals(errorBounds));
#endif

            return errorBounds;
        }

        protected override Size GetPreferredSize(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex, Size constraintSize)
        {
            if (DataGridView == null)
            {
                return new Size(-1, -1);
            }

            if (cellStyle == null)
            {
                throw new ArgumentNullException(nameof(cellStyle));
            }

            Size preferredSize;
            Rectangle borderWidthsRect = StdBorderWidths;
            int borderAndPaddingWidths = borderWidthsRect.Left + borderWidthsRect.Width + cellStyle.Padding.Horizontal;
            int borderAndPaddingHeights = borderWidthsRect.Top + borderWidthsRect.Height + cellStyle.Padding.Vertical;
            DataGridViewFreeDimension freeDimension = DataGridViewCell.GetFreeDimensionFromConstraint(constraintSize);
            object formattedValue = GetFormattedValue(rowIndex, ref cellStyle, DataGridViewDataErrorContexts.Formatting | DataGridViewDataErrorContexts.PreferredSize);
            string formattedString = formattedValue as string;
            if (string.IsNullOrEmpty(formattedString))
            {
                formattedString = " ";
            }
            TextFormatFlags flags = DataGridViewUtilities.ComputeTextFormatFlagsForCellStyleAlignment(DataGridView.RightToLeftInternal, cellStyle.Alignment, cellStyle.WrapMode);
            if (cellStyle.WrapMode == DataGridViewTriState.True && formattedString.Length > 1)
            {
                switch (freeDimension)
                {
                    case DataGridViewFreeDimension.Width:
                        {
                            preferredSize = new Size(DataGridViewCell.MeasureTextWidth(graphics,
                                                                                       formattedString,
                                                                                       cellStyle.Font,
                                                                                       Math.Max(1, constraintSize.Height - borderAndPaddingHeights - DATAGRIDVIEWTEXTBOXCELL_verticalTextMarginTopWithWrapping - DATAGRIDVIEWTEXTBOXCELL_verticalTextMarginBottom),
                                                                                       flags),
                                                     0);
                            break;
                        }
                    case DataGridViewFreeDimension.Height:
                        {
                            preferredSize = new Size(0,
                                                     DataGridViewCell.MeasureTextHeight(graphics,
                                                                                        formattedString,
                                                                                        cellStyle.Font,
                                                                                        Math.Max(1, constraintSize.Width - borderAndPaddingWidths - DATAGRIDVIEWTEXTBOXCELL_horizontalTextMarginLeft - DATAGRIDVIEWTEXTBOXCELL_horizontalTextMarginRight),
                                                                                        flags));
                            break;
                        }
                    default:
                        {
                            preferredSize = DataGridViewCell.MeasureTextPreferredSize(graphics,
                                                                                      formattedString,
                                                                                      cellStyle.Font,
                                                                                      5.0F,
                                                                                      flags);
                            break;
                        }
                }
            }
            else
            {
                switch (freeDimension)
                {
                    case DataGridViewFreeDimension.Width:
                        {
                            preferredSize = new Size(DataGridViewCell.MeasureTextSize(graphics, formattedString, cellStyle.Font, flags).Width,
                                                     0);
                            break;
                        }
                    case DataGridViewFreeDimension.Height:
                        {
                            preferredSize = new Size(0,
                                                     DataGridViewCell.MeasureTextSize(graphics, formattedString, cellStyle.Font, flags).Height);
                            break;
                        }
                    default:
                        {
                            preferredSize = DataGridViewCell.MeasureTextSize(graphics, formattedString, cellStyle.Font, flags);
                            break;
                        }
                }
            }

            if (freeDimension != DataGridViewFreeDimension.Height)
            {
                preferredSize.Width += DATAGRIDVIEWTEXTBOXCELL_horizontalTextMarginLeft + DATAGRIDVIEWTEXTBOXCELL_horizontalTextMarginRight + borderAndPaddingWidths;
                if (DataGridView.ShowCellErrors)
                {
                    // Making sure that there is enough room for the potential error icon
                    preferredSize.Width = Math.Max(preferredSize.Width, borderAndPaddingWidths + DATAGRIDVIEWCELL_iconMarginWidth * 2 + iconsWidth);
                }
            }
            if (freeDimension != DataGridViewFreeDimension.Width)
            {
                int verticalTextMarginTop = cellStyle.WrapMode == DataGridViewTriState.True ? DATAGRIDVIEWTEXTBOXCELL_verticalTextMarginTopWithWrapping : DATAGRIDVIEWTEXTBOXCELL_verticalTextMarginTopWithoutWrapping;
                preferredSize.Height += verticalTextMarginTop + DATAGRIDVIEWTEXTBOXCELL_verticalTextMarginBottom + borderAndPaddingHeights;
                if (DataGridView.ShowCellErrors)
                {
                    // Making sure that there is enough room for the potential error icon
                    preferredSize.Height = Math.Max(preferredSize.Height, borderAndPaddingHeights + DATAGRIDVIEWCELL_iconMarginHeight * 2 + iconsHeight);
                }
            }
            return preferredSize;
        }

        public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            Debug.Assert(DataGridView != null &&
                         DataGridView.EditingPanel != null &&
                         DataGridView.EditingControl != null);
            Debug.Assert(!ReadOnly);
            base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);
            if (DataGridView.EditingControl is TextBox textBox)
            {
                textBox.BorderStyle = BorderStyle.None;
                textBox.AcceptsReturn = textBox.Multiline = dataGridViewCellStyle.WrapMode == DataGridViewTriState.True;
                textBox.MaxLength = MaxInputLength;
                if (!(initialFormattedValue is string initialFormattedValueStr))
                {
                    textBox.Text = string.Empty;
                }
                else
                {
                    textBox.Text = initialFormattedValueStr;
                }

                EditingTextBox = DataGridView.EditingControl as DataGridViewTextBoxEditingControl;
            }
        }

        public override bool KeyEntersEditMode(KeyEventArgs e)
        {
            if (((char.IsLetterOrDigit((char)e.KeyCode) && !(e.KeyCode >= Keys.F1 && e.KeyCode <= Keys.F24)) ||
                 (e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.Divide) ||
                 (e.KeyCode >= Keys.OemSemicolon && e.KeyCode <= Keys.Oem102) ||
                 (e.KeyCode == Keys.Space && !e.Shift)) &&
                !e.Alt &&
                !e.Control)
            {
                return true;
            }
            return base.KeyEntersEditMode(e);
        }

        protected override void OnEnter(int rowIndex, bool throughMouseClick)
        {
            if (DataGridView == null)
            {
                return;
            }
            if (throughMouseClick)
            {
                flagsState |= (byte)DATAGRIDVIEWTEXTBOXCELL_ignoreNextMouseClick;
            }
        }

        protected override void OnLeave(int rowIndex, bool throughMouseClick)
        {
            if (DataGridView == null)
            {
                return;
            }
            flagsState = (byte)(flagsState & ~DATAGRIDVIEWTEXTBOXCELL_ignoreNextMouseClick);
        }

        protected override void OnMouseClick(DataGridViewCellMouseEventArgs e)
        {
            if (DataGridView == null)
            {
                return;
            }
            Debug.Assert(e.ColumnIndex == ColumnIndex);
            Point ptCurrentCell = DataGridView.CurrentCellAddress;
            if (ptCurrentCell.X == e.ColumnIndex && ptCurrentCell.Y == e.RowIndex && e.Button == MouseButtons.Left)
            {
                if ((flagsState & DATAGRIDVIEWTEXTBOXCELL_ignoreNextMouseClick) != 0x00)
                {
                    flagsState = (byte)(flagsState & ~DATAGRIDVIEWTEXTBOXCELL_ignoreNextMouseClick);
                }
                else if (DataGridView.EditMode != DataGridViewEditMode.EditProgrammatically)
                {
                    DataGridView.BeginEdit(true /*selectAll*/);
                }
            }
        }

        private bool OwnsEditingTextBox(int rowIndex)
        {
            return rowIndex != -1 && EditingTextBox != null && rowIndex == ((IDataGridViewEditingControl)EditingTextBox).EditingControlRowIndex;
        }

        protected override void Paint(Graphics graphics,
            Rectangle clipBounds,
            Rectangle cellBounds,
            int rowIndex,
            DataGridViewElementStates cellState,
            object value,
            object formattedValue,
            string errorText,
            DataGridViewCellStyle cellStyle,
            DataGridViewAdvancedBorderStyle advancedBorderStyle,
            DataGridViewPaintParts paintParts)
        {
            if (cellStyle == null)
            {
                throw new ArgumentNullException(nameof(cellStyle));
            }

            PaintPrivate(graphics,
                clipBounds,
                cellBounds,
                rowIndex,
                cellState,
                formattedValue,
                errorText,
                cellStyle,
                advancedBorderStyle,
                paintParts,
                false /*computeContentBounds*/,
                false /*computeErrorIconBounds*/,
                true /*paint*/);
        }

        // PaintPrivate is used in three places that need to duplicate the paint code:
        // 1. DataGridViewCell::Paint method
        // 2. DataGridViewCell::GetContentBounds
        // 3. DataGridViewCell::GetErrorIconBounds
        //
        // if computeContentBounds is true then PaintPrivate returns the contentBounds
        // else if computeErrorIconBounds is true then PaintPrivate returns the errorIconBounds
        // else it returns Rectangle.Empty;
        private Rectangle PaintPrivate(Graphics graphics,
            Rectangle clipBounds,
            Rectangle cellBounds,
            int rowIndex,
            DataGridViewElementStates cellState,
            object formattedValue,
            string errorText,
            DataGridViewCellStyle cellStyle,
            DataGridViewAdvancedBorderStyle advancedBorderStyle,
            DataGridViewPaintParts paintParts,
            bool computeContentBounds,
            bool computeErrorIconBounds,
            bool paint)
        {
            // Parameter checking.
            // One bit and one bit only should be turned on
            Debug.Assert(paint || computeContentBounds || computeErrorIconBounds);
            Debug.Assert(!paint || !computeContentBounds || !computeErrorIconBounds);
            Debug.Assert(!computeContentBounds || !computeErrorIconBounds || !paint);
            Debug.Assert(!computeErrorIconBounds || !paint || !computeContentBounds);
            Debug.Assert(cellStyle != null);

            // If computeContentBounds == TRUE then resultBounds will be the contentBounds.
            // If computeErrorIconBounds == TRUE then resultBounds will be the error icon bounds.
            // Else resultBounds will be Rectangle.Empty;
            Rectangle resultBounds = Rectangle.Empty;

            if (paint && DataGridViewCell.PaintBorder(paintParts))
            {
                PaintBorder(graphics, clipBounds, cellBounds, cellStyle, advancedBorderStyle);
            }

            Rectangle borderWidths = BorderWidths(advancedBorderStyle);
            Rectangle valBounds = cellBounds;
            valBounds.Offset(borderWidths.X, borderWidths.Y);
            valBounds.Width -= borderWidths.Right;
            valBounds.Height -= borderWidths.Bottom;

            SolidBrush br;

            Point ptCurrentCell = DataGridView.CurrentCellAddress;
            bool cellCurrent = ptCurrentCell.X == ColumnIndex && ptCurrentCell.Y == rowIndex;
            bool cellEdited = cellCurrent && DataGridView.EditingControl != null;
            bool cellSelected = (cellState & DataGridViewElementStates.Selected) != 0;

            if (DataGridViewCell.PaintSelectionBackground(paintParts) && cellSelected && !cellEdited)
            {
                br = DataGridView.GetCachedBrush(cellStyle.SelectionBackColor);
            }
            else
            {
                br = DataGridView.GetCachedBrush(cellStyle.BackColor);
            }

            if (paint && DataGridViewCell.PaintBackground(paintParts) && br.Color.A == 255 && valBounds.Width > 0 && valBounds.Height > 0)
            {
                graphics.FillRectangle(br, valBounds);
            }

            if (cellStyle.Padding != Padding.Empty)
            {
                if (DataGridView.RightToLeftInternal)
                {
                    valBounds.Offset(cellStyle.Padding.Right, cellStyle.Padding.Top);
                }
                else
                {
                    valBounds.Offset(cellStyle.Padding.Left, cellStyle.Padding.Top);
                }
                valBounds.Width -= cellStyle.Padding.Horizontal;
                valBounds.Height -= cellStyle.Padding.Vertical;
            }

            if (paint && cellCurrent && !cellEdited)
            {
                // Draw focus rectangle
                if (DataGridViewCell.PaintFocus(paintParts) &&
                    DataGridView.ShowFocusCues &&
                    DataGridView.Focused &&
                    valBounds.Width > 0 &&
                    valBounds.Height > 0)
                {
                    ControlPaint.DrawFocusRectangle(graphics, valBounds, Color.Empty, br.Color);
                }
            }

            Rectangle errorBounds = valBounds;
            string formattedString = formattedValue as string;

            if (formattedString != null && ((paint && !cellEdited) || computeContentBounds))
            {
                // Font independent margins
                int verticalTextMarginTop = cellStyle.WrapMode == DataGridViewTriState.True ? DATAGRIDVIEWTEXTBOXCELL_verticalTextMarginTopWithWrapping : DATAGRIDVIEWTEXTBOXCELL_verticalTextMarginTopWithoutWrapping;
                valBounds.Offset(DATAGRIDVIEWTEXTBOXCELL_horizontalTextMarginLeft, verticalTextMarginTop);
                valBounds.Width -= DATAGRIDVIEWTEXTBOXCELL_horizontalTextMarginLeft + DATAGRIDVIEWTEXTBOXCELL_horizontalTextMarginRight;
                valBounds.Height -= verticalTextMarginTop + DATAGRIDVIEWTEXTBOXCELL_verticalTextMarginBottom;
                if (valBounds.Width > 0 && valBounds.Height > 0)
                {
                    TextFormatFlags flags = DataGridViewUtilities.ComputeTextFormatFlagsForCellStyleAlignment(DataGridView.RightToLeftInternal, cellStyle.Alignment, cellStyle.WrapMode);
                    if (paint)
                    {
                        if (DataGridViewCell.PaintContentForeground(paintParts))
                        {
                            if ((flags & TextFormatFlags.SingleLine) != 0)
                            {
                                flags |= TextFormatFlags.EndEllipsis;
                            }
                            TextRenderer.DrawText(graphics,
                                formattedString,
                                cellStyle.Font,
                                valBounds,
                                cellSelected ? cellStyle.SelectionForeColor : cellStyle.ForeColor,
                                flags);
                        }
                    }
                    else
                    {
                        resultBounds = DataGridViewUtilities.GetTextBounds(valBounds, formattedString, flags, cellStyle);
                    }
                }
            }
            else if (computeErrorIconBounds)
            {
                if (!string.IsNullOrEmpty(errorText))
                {
                    resultBounds = ComputeErrorIconBounds(errorBounds);
                }
            }
            else
            {
                Debug.Assert(cellEdited || formattedString == null);
                Debug.Assert(paint || computeContentBounds);
            }

            if (DataGridView.ShowCellErrors && paint && DataGridViewCell.PaintErrorIcon(paintParts))
            {
                PaintErrorIcon(graphics, cellStyle, rowIndex, cellBounds, errorBounds, errorText);
            }

            return resultBounds;
        }

        public override void PositionEditingControl(bool setLocation,
                                                    bool setSize,
                                                    Rectangle cellBounds,
                                                    Rectangle cellClip,
                                                    DataGridViewCellStyle cellStyle,
                                                    bool singleVerticalBorderAdded,
                                                    bool singleHorizontalBorderAdded,
                                                    bool isFirstDisplayedColumn,
                                                    bool isFirstDisplayedRow)
        {
            Rectangle editingControlBounds = PositionEditingPanel(cellBounds,
                                                        cellClip,
                                                        cellStyle,
                                                        singleVerticalBorderAdded,
                                                        singleHorizontalBorderAdded,
                                                        isFirstDisplayedColumn,
                                                        isFirstDisplayedRow);
            editingControlBounds = GetAdjustedEditingControlBounds(editingControlBounds, cellStyle);
            DataGridView.EditingControl.Location = new Point(editingControlBounds.X, editingControlBounds.Y);
            DataGridView.EditingControl.Size = new Size(editingControlBounds.Width, editingControlBounds.Height);
        }

        public override string ToString()
        {
            return "DataGridViewTextBoxCell { ColumnIndex=" + ColumnIndex.ToString(CultureInfo.CurrentCulture) + ", RowIndex=" + RowIndex.ToString(CultureInfo.CurrentCulture) + " }";
        }

        protected class DataGridViewTextBoxCellAccessibleObject : DataGridViewCellAccessibleObject
        {
            public DataGridViewTextBoxCellAccessibleObject(DataGridViewCell owner) : base(owner)
            {
            }

            internal override bool IsIAccessibleExSupported() => true;

            internal override object GetPropertyValue(int propertyID)
            {
                if (propertyID == NativeMethods.UIA_ControlTypePropertyId)
                {
                    return NativeMethods.UIA_EditControlTypeId;
                }

                return base.GetPropertyValue(propertyID);
            }
        }
    }
}
