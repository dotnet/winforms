// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Diagnostics;
using System.Drawing;
using System.Windows.Forms.VisualStyles;
using static Interop;

namespace System.Windows.Forms
{
    public class DataGridViewTopLeftHeaderCell : DataGridViewColumnHeaderCell
    {
        private static readonly VisualStyleElement HeaderElement = VisualStyleElement.Header.Item.Normal;

        private const byte DATAGRIDVIEWTOPLEFTHEADERCELL_horizontalTextMarginLeft = 1;
        private const byte DATAGRIDVIEWTOPLEFTHEADERCELL_horizontalTextMarginRight = 2;
        private const byte DATAGRIDVIEWTOPLEFTHEADERCELL_verticalTextMargin = 1;

        public DataGridViewTopLeftHeaderCell()
        {
        }

        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new DataGridViewTopLeftHeaderCellAccessibleObject(this);
        }

        protected override Rectangle GetContentBounds(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex)
        {
            if (cellStyle is null)
            {
                throw new ArgumentNullException(nameof(cellStyle));
            }

            if (rowIndex != -1)
            {
                throw new ArgumentOutOfRangeException(nameof(rowIndex));
            }

            if (DataGridView is null)
            {
                return Rectangle.Empty;
            }

            object value = GetValue(rowIndex);

            // Intentionally not using GetFormattedValue because header cells don't typically perform formatting.
            // the content bounds are computed on demand
            // we mimic a lot of the painting code

            // get the borders

            ComputeBorderStyleCellStateAndCellBounds(rowIndex, out DataGridViewAdvancedBorderStyle dgvabsEffective, out DataGridViewElementStates cellState, out Rectangle cellBounds);

            Rectangle contentBounds = PaintPrivate(graphics,
                cellBounds,
                cellBounds,
                rowIndex,
                cellState,
                value,
                null /*errorText*/,                 // contentBounds is independent of errorText
                cellStyle,
                dgvabsEffective,
                DataGridViewPaintParts.ContentForeground,
                true /*computeContentBounds*/,
                false /*computeErrorIconBounds*/,
                false /*paint*/);

#if DEBUG
            Rectangle contentBoundsDebug = PaintPrivate(graphics,
                cellBounds,
                cellBounds,
                rowIndex,
                cellState,
                value,
                GetErrorText(rowIndex),
                cellStyle,
                dgvabsEffective,
                DataGridViewPaintParts.ContentForeground,
                true /*computeContentBounds*/,
                false /*computeErrorIconBounds*/,
                false /*paint*/);
            Debug.Assert(contentBoundsDebug.Equals(contentBounds));
#endif

            return contentBounds;
        }

        protected override Rectangle GetErrorIconBounds(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex)
        {
            if (rowIndex != -1)
            {
                throw new ArgumentOutOfRangeException(nameof(rowIndex));
            }

            if (DataGridView is null)
            {
                return Rectangle.Empty;
            }

            if (cellStyle is null)
            {
                throw new ArgumentNullException(nameof(cellStyle));
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

            Rectangle errorBoundsDebug = PaintPrivate(graphics,
                cellBounds,
                cellBounds,
                rowIndex,
                cellState,
                value,
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
            if (rowIndex != -1)
            {
                throw new ArgumentOutOfRangeException(nameof(rowIndex));
            }

            if (DataGridView is null)
            {
                return new Size(-1, -1);
            }

            if (cellStyle is null)
            {
                throw new ArgumentNullException(nameof(cellStyle));
            }

            Rectangle borderWidthsRect = BorderWidths(DataGridView.AdjustedTopLeftHeaderBorderStyle);
            int borderAndPaddingWidths = borderWidthsRect.Left + borderWidthsRect.Width + cellStyle.Padding.Horizontal;
            int borderAndPaddingHeights = borderWidthsRect.Top + borderWidthsRect.Height + cellStyle.Padding.Vertical;
            TextFormatFlags flags = DataGridViewUtilities.ComputeTextFormatFlagsForCellStyleAlignment(DataGridView.RightToLeftInternal, cellStyle.Alignment, cellStyle.WrapMode);

            // Intentionally not using GetFormattedValue because header cells don't typically perform formatting.
            object val = GetValue(rowIndex);
            if (!(val is string))
            {
                val = null;
            }
            return DataGridViewUtilities.GetPreferredRowHeaderSize(graphics,
                                                                   (string)val,
                                                                   cellStyle,
                                                                   borderAndPaddingWidths,
                                                                   borderAndPaddingHeights,
                                                                   DataGridView.ShowCellErrors,
                                                                   false /*showGlyph*/,
                                                                   constraintSize,
                                                                   flags);
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
            if (cellStyle is null)
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

            Rectangle valBounds = cellBounds;
            Rectangle borderWidths = BorderWidths(advancedBorderStyle);

            valBounds.Offset(borderWidths.X, borderWidths.Y);
            valBounds.Width -= borderWidths.Right;
            valBounds.Height -= borderWidths.Bottom;

            bool cellSelected = (cellState & DataGridViewElementStates.Selected) != 0;

            if (paint && PaintBackground(paintParts))
            {
                if (DataGridView.ApplyVisualStylesToHeaderCells)
                {
                    // Theming
                    int state = (int)HeaderItemState.Normal;

                    if (ButtonState != ButtonState.Normal)
                    {
                        Debug.Assert(ButtonState == ButtonState.Pushed);
                        state = (int)HeaderItemState.Pressed;
                    }
                    else if (DataGridView.MouseEnteredCellAddress.Y == rowIndex && DataGridView.MouseEnteredCellAddress.X == ColumnIndex)
                    {
                        state = (int)HeaderItemState.Hot;
                    }

                    valBounds.Inflate(16, 16);
                    DataGridViewTopLeftHeaderCellRenderer.DrawHeader(graphics, valBounds, state);
                    valBounds.Inflate(-16, -16);
                }
                else
                {
                    Color brushColor = PaintSelectionBackground(paintParts) && cellSelected
                        ? cellStyle.SelectionBackColor
                        : cellStyle.BackColor;

                    if (!brushColor.HasTransparency())
                    {
                        using var brush = brushColor.GetCachedSolidBrushScope();
                        graphics.FillRectangle(brush, valBounds);
                    }
                }
            }

            if (paint && PaintBorder(paintParts))
            {
                PaintBorder(graphics, clipBounds, cellBounds, cellStyle, advancedBorderStyle);
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

            Rectangle errorBounds = valBounds;
            string formattedValueStr = formattedValue as string;

            // Font independent margins
            valBounds.Offset(DATAGRIDVIEWTOPLEFTHEADERCELL_horizontalTextMarginLeft, DATAGRIDVIEWTOPLEFTHEADERCELL_verticalTextMargin);
            valBounds.Width -= DATAGRIDVIEWTOPLEFTHEADERCELL_horizontalTextMarginLeft + DATAGRIDVIEWTOPLEFTHEADERCELL_horizontalTextMarginRight;
            valBounds.Height -= 2 * DATAGRIDVIEWTOPLEFTHEADERCELL_verticalTextMargin;
            if (valBounds.Width > 0 &&
                valBounds.Height > 0 &&
                !string.IsNullOrEmpty(formattedValueStr) &&
                (paint || computeContentBounds))
            {
                Color textColor;
                if (DataGridView.ApplyVisualStylesToHeaderCells)
                {
                    textColor = DataGridViewTopLeftHeaderCellRenderer.VisualStyleRenderer.GetColor(ColorProperty.TextColor);
                }
                else
                {
                    textColor = cellSelected ? cellStyle.SelectionForeColor : cellStyle.ForeColor;
                }
                TextFormatFlags flags = DataGridViewUtilities.ComputeTextFormatFlagsForCellStyleAlignment(DataGridView.RightToLeftInternal, cellStyle.Alignment, cellStyle.WrapMode);
                if (paint)
                {
                    if (PaintContentForeground(paintParts))
                    {
                        if ((flags & TextFormatFlags.SingleLine) != 0)
                        {
                            flags |= TextFormatFlags.EndEllipsis;
                        }
                        TextRenderer.DrawText(graphics,
                                              formattedValueStr,
                                              cellStyle.Font,
                                              valBounds,
                                              textColor,
                                              flags);
                    }
                }
                else
                {
                    Debug.Assert(computeContentBounds);
                    resultBounds = DataGridViewUtilities.GetTextBounds(valBounds, formattedValueStr, flags, cellStyle);
                }
            }
            else if (computeErrorIconBounds && !string.IsNullOrEmpty(errorText))
            {
                resultBounds = ComputeErrorIconBounds(errorBounds);
            }

            if (DataGridView.ShowCellErrors && paint && PaintErrorIcon(paintParts))
            {
                PaintErrorIcon(graphics, cellStyle, rowIndex, cellBounds, errorBounds, errorText);
            }

            return resultBounds;
        }

        protected override void PaintBorder(Graphics graphics,
            Rectangle clipBounds,
            Rectangle bounds,
            DataGridViewCellStyle cellStyle,
            DataGridViewAdvancedBorderStyle advancedBorderStyle)
        {
            if (DataGridView is null)
            {
                return;
            }

            base.PaintBorder(graphics, clipBounds, bounds, cellStyle, advancedBorderStyle);

            if (!DataGridView.RightToLeftInternal &&
                DataGridView.ApplyVisualStylesToHeaderCells)
            {
                (Color darkColor, Color lightColor) = GetContrastedColors(cellStyle.BackColor);

                if (DataGridView.AdvancedColumnHeadersBorderStyle.All == DataGridViewAdvancedCellBorderStyle.Inset)
                {
                    using var penControlDark = darkColor.GetCachedPenScope();
                    graphics.DrawLine(penControlDark, bounds.X, bounds.Y, bounds.X, bounds.Bottom - 1);
                    graphics.DrawLine(penControlDark, bounds.X, bounds.Y, bounds.Right - 1, bounds.Y);
                }
                else if (DataGridView.AdvancedColumnHeadersBorderStyle.All == DataGridViewAdvancedCellBorderStyle.Outset)
                {
                    using var penControlLightLight = lightColor.GetCachedPenScope();
                    graphics.DrawLine(penControlLightLight, bounds.X, bounds.Y, bounds.X, bounds.Bottom - 1);
                    graphics.DrawLine(penControlLightLight, bounds.X, bounds.Y, bounds.Right - 1, bounds.Y);
                }
                else if (DataGridView.AdvancedColumnHeadersBorderStyle.All == DataGridViewAdvancedCellBorderStyle.InsetDouble)
                {
                    using var penControlDark = darkColor.GetCachedPenScope();
                    graphics.DrawLine(penControlDark, bounds.X + 1, bounds.Y + 1, bounds.X + 1, bounds.Bottom - 1);
                    graphics.DrawLine(penControlDark, bounds.X + 1, bounds.Y + 1, bounds.Right - 1, bounds.Y + 1);
                }
            }
        }

        public override string ToString() => "DataGridViewTopLeftHeaderCell";

        private class DataGridViewTopLeftHeaderCellRenderer
        {
            private static VisualStyleRenderer visualStyleRenderer;

            private DataGridViewTopLeftHeaderCellRenderer()
            {
            }

            public static VisualStyleRenderer VisualStyleRenderer
            {
                get
                {
                    if (visualStyleRenderer is null)
                    {
                        visualStyleRenderer = new VisualStyleRenderer(HeaderElement);
                    }

                    return visualStyleRenderer;
                }
            }

            public static void DrawHeader(Graphics g, Rectangle bounds, int headerState)
            {
                VisualStyleRenderer.SetParameters(HeaderElement.ClassName, HeaderElement.Part, headerState);
                VisualStyleRenderer.DrawBackground(g, bounds, Rectangle.Truncate(g.ClipBounds));
            }
        }

        protected class DataGridViewTopLeftHeaderCellAccessibleObject : DataGridViewColumnHeaderCellAccessibleObject
        {
            public DataGridViewTopLeftHeaderCellAccessibleObject(DataGridViewTopLeftHeaderCell owner) : base(owner)
            {
            }

            public override Rectangle Bounds
            {
                get
                {
                    if (!Owner.DataGridView.IsHandleCreated)
                    {
                        return Rectangle.Empty;
                    }

                    Rectangle cellRect = Owner.DataGridView.GetCellDisplayRectangle(-1, -1, false /*cutOverflow*/);
                    return Owner.DataGridView.RectangleToScreen(cellRect);
                }
            }

            public override string DefaultAction
            {
                get
                {
                    if (Owner.DataGridView.MultiSelect)
                    {
                        return SR.DataGridView_AccTopLeftColumnHeaderCellDefaultAction;
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
            }

            public override string Name
            {
                get
                {
                    object value = Owner.Value;
                    if (value != null && !(value is string))
                    {
                        // The user set the Value on the DataGridViewTopLeftHeaderCell and it did not set it to a string.
                        // Then the name of the DataGridViewTopLeftHeaderAccessibleObject is String.Empty;
                        //
                        return string.Empty;
                    }
                    string strValue = value as string;
                    if (string.IsNullOrEmpty(strValue))
                    {
                        if (Owner.DataGridView != null)
                        {
                            if (Owner.DataGridView.RightToLeft == RightToLeft.No)
                            {
                                return SR.DataGridView_AccTopLeftColumnHeaderCellName;
                            }
                            else
                            {
                                return SR.DataGridView_AccTopLeftColumnHeaderCellNameRTL;
                            }
                        }
                        else
                        {
                            return string.Empty;
                        }
                    }
                    else
                    {
                        return string.Empty;
                    }
                }
            }

            public override AccessibleStates State
            {
                get
                {
                    AccessibleStates resultState = AccessibleStates.Selectable;

                    // get the Offscreen state from the base method.
                    AccessibleStates state = base.State;
                    if ((state & AccessibleStates.Offscreen) == AccessibleStates.Offscreen)
                    {
                        resultState |= AccessibleStates.Offscreen;
                    }

                    // If all the cells are selected, then the top left header cell accessible object is considered to be selected as well.
                    if (Owner.DataGridView.AreAllCellsSelected(false /*includeInvisibleCells*/))
                    {
                        resultState |= AccessibleStates.Selected;
                    }

                    return resultState;
                }
            }

            public override string Value
            {
                get
                {
                    // We changed DataGridViewTopLeftHeaderCellAccessibleObject::Name to return a string
                    // However, DataGridViewTopLeftHeaderCellAccessibleObject::Value should still return String.Empty.
                    return string.Empty;
                }
            }

            public override void DoDefaultAction()
            {
                if (Owner?.DataGridView?.IsHandleCreated is true)
                {
                    Owner.DataGridView.SelectAll();
                }
            }

            public override AccessibleObject Navigate(AccessibleNavigation navigationDirection)
            {
                Debug.Assert(Owner.DataGridView.RowHeadersVisible, "if the row headers are not visible how did you get the top left header cell acc object?");
                switch (navigationDirection)
                {
                    case AccessibleNavigation.Previous:
                        return null;
                    case AccessibleNavigation.Left:
                        if (Owner.DataGridView.RightToLeft == RightToLeft.No)
                        {
                            return null;
                        }
                        else
                        {
                            return NavigateForward();
                        }
                    case AccessibleNavigation.Next:
                        return NavigateForward();
                    case AccessibleNavigation.Right:
                        if (Owner.DataGridView.RightToLeft == RightToLeft.No)
                        {
                            return NavigateForward();
                        }
                        else
                        {
                            return null;
                        }
                    default:
                        return null;
                }
            }

            private AccessibleObject NavigateForward()
            {
                if (Owner.DataGridView.Columns.GetColumnCount(DataGridViewElementStates.Visible) == 0)
                {
                    return null;
                }

                // return the acc object for the first visible column
                return Owner.DataGridView.AccessibilityObject.GetChild(0).GetChild(1);
            }

            public override void Select(AccessibleSelection flags)
            {
                if (Owner is null)
                {
                    throw new InvalidOperationException(SR.DataGridViewCellAccessibleObject_OwnerNotSet);
                }

                if (Owner.DataGridView?.IsHandleCreated != true)
                {
                    return;
                }

                // AccessibleSelection.TakeFocus should focus the grid and then focus the first data grid view data cell
                if ((flags & AccessibleSelection.TakeFocus) == AccessibleSelection.TakeFocus)
                {
                    // Focus the grid
                    Owner.DataGridView.Focus();
                    if (Owner.DataGridView.Columns.GetColumnCount(DataGridViewElementStates.Visible) > 0 &&
                        Owner.DataGridView.Rows.GetRowCount(DataGridViewElementStates.Visible) > 0)
                    {
                        // This means that there are visible rows and columns.
                        // Focus the first data cell.
                        DataGridViewRow row = Owner.DataGridView.Rows[Owner.DataGridView.Rows.GetFirstRow(DataGridViewElementStates.Visible)];
                        DataGridViewColumn col = Owner.DataGridView.Columns.GetFirstColumn(DataGridViewElementStates.Visible);

                        // DataGridView::set_CurrentCell clears the previous selection.
                        // So use SetCurrenCellAddressCore directly.
                        Owner.DataGridView.SetCurrentCellAddressCoreInternal(col.Index, row.Index, false /*setAnchorCellAddress*/, true /*validateCurrentCell*/, false /*thoughMouseClick*/);
                    }
                }

                // AddSelection selects the entire grid.
                if ((flags & AccessibleSelection.AddSelection) == AccessibleSelection.AddSelection)
                {
                    if (Owner.DataGridView.MultiSelect)
                    {
                        Owner.DataGridView.SelectAll();
                    }
                }

                // RemoveSelection clears the selection on the entire grid.
                // But only if AddSelection is not set.
                if ((flags & AccessibleSelection.RemoveSelection) == AccessibleSelection.RemoveSelection &&
                    (flags & AccessibleSelection.AddSelection) == 0)
                {
                    Owner.DataGridView.ClearSelection();
                }
            }

            #region IRawElementProviderFragment Implementation

            internal override UiaCore.IRawElementProviderFragment FragmentNavigate(UiaCore.NavigateDirection direction)
            {
                DataGridView dataGridView = Owner.DataGridView;

                switch (direction)
                {
                    case UiaCore.NavigateDirection.Parent:
                        return dataGridView.AccessibilityObject.GetChild(0);
                    case UiaCore.NavigateDirection.PreviousSibling:
                        return null;
                    case UiaCore.NavigateDirection.NextSibling:
                        if (dataGridView.Columns.GetColumnCount(DataGridViewElementStates.Visible) == 0)
                        {
                            return null;
                        }

                        return NavigateForward();
                    default:
                        return null;
                }
            }

            #endregion

            #region IRawElementProviderSimple Implementation

            internal override object GetPropertyValue(UiaCore.UIA propertyId)
            {
                switch (propertyId)
                {
                    case UiaCore.UIA.NamePropertyId:
                        return Name;
                    case UiaCore.UIA.ControlTypePropertyId:
                        return UiaCore.UIA.HeaderControlTypeId;
                    case UiaCore.UIA.IsEnabledPropertyId:
                        return Owner.DataGridView.Enabled;
                    case UiaCore.UIA.HelpTextPropertyId:
                        return Help ?? string.Empty;
                    case UiaCore.UIA.IsKeyboardFocusablePropertyId:
                    case UiaCore.UIA.IsPasswordPropertyId:
                    case UiaCore.UIA.IsOffscreenPropertyId:
                        return false;
                    case UiaCore.UIA.AccessKeyPropertyId:
                        return string.Empty;
                }

                return base.GetPropertyValue(propertyId);
            }

            #endregion
        }
    }
}
