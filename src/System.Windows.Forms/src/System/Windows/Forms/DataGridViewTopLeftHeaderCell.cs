// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Windows.Forms.VisualStyles;
    using System.ComponentModel;
    using System.Windows.Forms.Internal;
    using System.Security.Permissions;

    /// <include file='doc\DataGridViewTopLeftHeaderCell.uex' path='docs/doc[@for="DataGridViewTopLeftHeaderCell"]/*' />
    /// <devdoc>
    ///    <para></para>
    /// </devdoc>
    public class DataGridViewTopLeftHeaderCell : DataGridViewColumnHeaderCell
    {
        private static readonly VisualStyleElement HeaderElement = VisualStyleElement.Header.Item.Normal;

        private const byte DATAGRIDVIEWTOPLEFTHEADERCELL_horizontalTextMarginLeft = 1;
        private const byte DATAGRIDVIEWTOPLEFTHEADERCELL_horizontalTextMarginRight = 2;
        private const byte DATAGRIDVIEWTOPLEFTHEADERCELL_verticalTextMargin = 1;

        /// <include file='doc\DataGridViewTopLeftHeaderCell.uex' path='docs/doc[@for="DataGridViewTopLeftHeaderCell.DataGridViewTopLeftHeaderCell"]/*' />
        public DataGridViewTopLeftHeaderCell()
        {
        }

        /// <include file='doc\DataGridViewTopLeftHeaderCell.uex' path='docs/doc[@for="DataGridViewTopLeftHeaderCell.CreateAccessibilityInstance"]/*' />
        protected override AccessibleObject CreateAccessibilityInstance()
        {
            return new DataGridViewTopLeftHeaderCellAccessibleObject(this);
        }

        /// <include file='doc\DataGridViewTopLeftHeaderCell.uex' path='docs/doc[@for="DataGridViewTopLeftHeaderCell.GetContentBounds"]/*' />
        protected override Rectangle GetContentBounds(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex)
        {
            if (cellStyle == null)
            {
                throw new ArgumentNullException(nameof(cellStyle));
            }

            if (rowIndex != -1)
            {
                throw new ArgumentOutOfRangeException(nameof(rowIndex));
            }

            if (this.DataGridView == null)
            {
                return Rectangle.Empty;
            }

            object value = GetValue(rowIndex);

            // Intentionally not using GetFormattedValue because header cells don't typically perform formatting.
            // the content bounds are computed on demand
            // we mimic a lot of the painting code

            // get the borders
            DataGridViewAdvancedBorderStyle dgvabsEffective;
            DataGridViewElementStates cellState;
            Rectangle cellBounds;

            ComputeBorderStyleCellStateAndCellBounds(rowIndex, out dgvabsEffective, out cellState, out cellBounds);

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

        /// <include file='doc\DataGridViewTopLeftHeaderCell.uex' path='docs/doc[@for="DataGridViewTopLeftHeaderCell.GetErrorIconBounds"]/*' />
        protected override Rectangle GetErrorIconBounds(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex)
        {
            if (rowIndex != -1)
            {
                throw new ArgumentOutOfRangeException(nameof(rowIndex));
            }

            if (this.DataGridView == null)
            {
                return Rectangle.Empty;
            }

            if (cellStyle == null)
            {
                throw new ArgumentNullException(nameof(cellStyle));
            }

            DataGridViewAdvancedBorderStyle dgvabsEffective;
            DataGridViewElementStates cellState;
            Rectangle cellBounds;

            ComputeBorderStyleCellStateAndCellBounds(rowIndex, out dgvabsEffective, out cellState, out cellBounds);

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

        /// <include file='doc\DataGridViewTopLeftHeaderCell.uex' path='docs/doc[@for="DataGridViewTopLeftHeaderCell.GetPreferredSize"]/*' />
        protected override Size GetPreferredSize(Graphics graphics, DataGridViewCellStyle cellStyle, int rowIndex, Size constraintSize)
        {
            if (rowIndex != -1)
            {
                throw new ArgumentOutOfRangeException(nameof(rowIndex));
            }

            if (this.DataGridView == null)
            {
                return new Size(-1, -1);
            }

            if (cellStyle == null)
            {
                throw new ArgumentNullException(nameof(cellStyle));
            }

            Rectangle borderWidthsRect = BorderWidths(this.DataGridView.AdjustedTopLeftHeaderBorderStyle);
            int borderAndPaddingWidths = borderWidthsRect.Left + borderWidthsRect.Width + cellStyle.Padding.Horizontal;
            int borderAndPaddingHeights = borderWidthsRect.Top + borderWidthsRect.Height + cellStyle.Padding.Vertical;
            TextFormatFlags flags = DataGridViewUtilities.ComputeTextFormatFlagsForCellStyleAlignment(this.DataGridView.RightToLeftInternal, cellStyle.Alignment, cellStyle.WrapMode);

            // Intentionally not using GetFormattedValue because header cells don't typically perform formatting.
            object val = GetValue(rowIndex);
            if (!(val is string))
            {
                val = null;
            }
            return DataGridViewUtilities.GetPreferredRowHeaderSize(graphics, 
                                                                   (string) val, 
                                                                   cellStyle, 
                                                                   borderAndPaddingWidths, 
                                                                   borderAndPaddingHeights,
                                                                   this.DataGridView.ShowCellErrors,
                                                                   false /*showGlyph*/,
                                                                   constraintSize, 
                                                                   flags);
        }

        /// <include file='doc\DataGridViewTopLeftHeaderCell.uex' path='docs/doc[@for="DataGridViewTopLeftHeaderCell.Paint"]/*' />
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

            Rectangle valBounds = cellBounds;
            Rectangle borderWidths = BorderWidths(advancedBorderStyle);

            valBounds.Offset(borderWidths.X, borderWidths.Y);
            valBounds.Width -= borderWidths.Right;
            valBounds.Height -= borderWidths.Bottom;

            bool cellSelected = (cellState & DataGridViewElementStates.Selected) != 0;

            if (paint && DataGridViewCell.PaintBackground(paintParts))
            {
                if (this.DataGridView.ApplyVisualStylesToHeaderCells)
                {
                    // XP Theming
                    int state = (int)HeaderItemState.Normal;

                    if (this.ButtonState != ButtonState.Normal)
                    {
                        Debug.Assert(this.ButtonState == ButtonState.Pushed);
                        state = (int)HeaderItemState.Pressed;
                    }
                    else if (this.DataGridView.MouseEnteredCellAddress.Y == rowIndex && this.DataGridView.MouseEnteredCellAddress.X == this.ColumnIndex)
                    {
                        state = (int)HeaderItemState.Hot;
                    }

                    valBounds.Inflate(16, 16);
                    DataGridViewTopLeftHeaderCellRenderer.DrawHeader(graphics, valBounds, state);
                    valBounds.Inflate(-16, -16);
                }
                else
                {
                    SolidBrush br = this.DataGridView.GetCachedBrush((DataGridViewCell.PaintSelectionBackground(paintParts) && cellSelected) ? cellStyle.SelectionBackColor : cellStyle.BackColor);
                    if (br.Color.A == 255)
                    {
                        graphics.FillRectangle(br, valBounds);
                    }
                }
            }

            if (paint && DataGridViewCell.PaintBorder(paintParts))
            {
                PaintBorder(graphics, clipBounds, cellBounds, cellStyle, advancedBorderStyle);
            }

            if (cellStyle.Padding != Padding.Empty)
            {
                if (this.DataGridView.RightToLeftInternal)
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
                !String.IsNullOrEmpty(formattedValueStr) && 
                (paint || computeContentBounds))
            {
                Color textColor;
                if (this.DataGridView.ApplyVisualStylesToHeaderCells)
                {
                    textColor = DataGridViewTopLeftHeaderCellRenderer.VisualStyleRenderer.GetColor(ColorProperty.TextColor);
                }
                else
                {
                    textColor = cellSelected ? cellStyle.SelectionForeColor : cellStyle.ForeColor;
                }
                TextFormatFlags flags = DataGridViewUtilities.ComputeTextFormatFlagsForCellStyleAlignment(this.DataGridView.RightToLeftInternal, cellStyle.Alignment, cellStyle.WrapMode);
                if (paint)
                {
                    if (DataGridViewCell.PaintContentForeground(paintParts))
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
            else if (computeErrorIconBounds && !String.IsNullOrEmpty(errorText))
            {
                resultBounds = ComputeErrorIconBounds(errorBounds);
            }

            if (this.DataGridView.ShowCellErrors && paint && DataGridViewCell.PaintErrorIcon(paintParts))
            {
                PaintErrorIcon(graphics, cellStyle, rowIndex, cellBounds, errorBounds, errorText);
            }

            return resultBounds;
        }

        /// <include file='doc\DataGridViewTopLeftHeaderCell.uex' path='docs/doc[@for="DataGridViewTopLeftHeaderCell.PaintBorder"]/*' />
        protected override void PaintBorder(Graphics graphics,
            Rectangle clipBounds,
            Rectangle bounds,
            DataGridViewCellStyle cellStyle,
            DataGridViewAdvancedBorderStyle advancedBorderStyle)
        {
            if (this.DataGridView == null)
            {
                return;
            }

            base.PaintBorder(graphics, clipBounds, bounds, cellStyle, advancedBorderStyle);

            if (!this.DataGridView.RightToLeftInternal &&
                this.DataGridView.ApplyVisualStylesToHeaderCells)
            {
                if (this.DataGridView.AdvancedColumnHeadersBorderStyle.All == DataGridViewAdvancedCellBorderStyle.Inset)
                {
                    Pen penControlDark = null, penControlLightLight = null;
                    GetContrastedPens(cellStyle.BackColor, ref penControlDark, ref penControlLightLight);
                    graphics.DrawLine(penControlDark, bounds.X, bounds.Y, bounds.X, bounds.Bottom - 1);
                    graphics.DrawLine(penControlDark, bounds.X, bounds.Y, bounds.Right - 1, bounds.Y);
                }
                else if (this.DataGridView.AdvancedColumnHeadersBorderStyle.All == DataGridViewAdvancedCellBorderStyle.Outset)
                {
                    Pen penControlDark = null, penControlLightLight = null;
                    GetContrastedPens(cellStyle.BackColor, ref penControlDark, ref penControlLightLight);
                    graphics.DrawLine(penControlLightLight, bounds.X, bounds.Y, bounds.X, bounds.Bottom - 1);
                    graphics.DrawLine(penControlLightLight, bounds.X, bounds.Y, bounds.Right - 1, bounds.Y);
                }
                else if (this.DataGridView.AdvancedColumnHeadersBorderStyle.All == DataGridViewAdvancedCellBorderStyle.InsetDouble)
                {
                    Pen penControlDark = null, penControlLightLight = null;
                    GetContrastedPens(cellStyle.BackColor, ref penControlDark, ref penControlLightLight);
                    graphics.DrawLine(penControlDark, bounds.X + 1, bounds.Y + 1, bounds.X + 1, bounds.Bottom - 1);
                    graphics.DrawLine(penControlDark, bounds.X + 1, bounds.Y + 1, bounds.Right - 1, bounds.Y + 1);
                }
            }
        }

        /// <include file='doc\DataGridViewTopLeftHeaderCell.uex' path='docs/doc[@for="DataGridViewTopLeftHeaderCell.ToString"]/*' />
        /// <devdoc>
        ///    <para></para>
        /// </devdoc>
        public override string ToString() 
        {
            return "DataGridViewTopLeftHeaderCell";
        }

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
                    if (visualStyleRenderer == null)
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

        /// <include file='doc\DataGridViewTopLeftHeaderCell.uex' path='docs/doc[@for="DataGridViewTopLeftHeaderCellAccessibleObject"]/*' />
        protected class DataGridViewTopLeftHeaderCellAccessibleObject : DataGridViewColumnHeaderCellAccessibleObject
        {
            /// <include file='doc\DataGridViewTopLeftHeaderCell.uex' path='docs/doc[@for="DataGridViewTopLeftHeaderCellAccessibleObject.DataGridViewTopLeftHeaderCellAccessibleObject"]/*' />
            public DataGridViewTopLeftHeaderCellAccessibleObject(DataGridViewTopLeftHeaderCell owner) : base (owner)
            {
            }

            /// <include file='doc\DataGridViewTopLeftHeaderCell.uex' path='docs/doc[@for="DataGridViewTopLeftHeaderCellAccessibleObject.Bounds"]/*' />
            public override Rectangle Bounds
            {
                get
                {
                    Rectangle cellRect = this.Owner.DataGridView.GetCellDisplayRectangle(-1, -1, false /*cutOverflow*/);
                    return this.Owner.DataGridView.RectangleToScreen(cellRect);
                }
            }

            /// <include file='doc\DataGridViewTopLeftHeaderCell.uex' path='docs/doc[@for="DataGridViewTopLeftHeaderCellAccessibleObject.DefaultAction"]/*' />
            public override string DefaultAction
            {
                get
                {
                    if (this.Owner.DataGridView.MultiSelect)
                    {
                        return string.Format(SR.DataGridView_AccTopLeftColumnHeaderCellDefaultAction);
                    }
                    else
                    {
                        return String.Empty;
                    }
                }
            }


            /// <include file='doc\DataGridViewTopLeftHeaderCell.uex' path='docs/doc[@for="DataGridViewTopLeftHeaderCellAccessibleObject.Name"]/*' />
            public override string Name
            {
                get
                {
                    object value = this.Owner.Value;
                    if (value != null && !(value is String))
                    {
                        // The user set the Value on the DataGridViewTopLeftHeaderCell and it did not set it to a string.
                        // Then the name of the DataGridViewTopLeftHeaderAccessibleObject is String.Empty;
                        //
                        return String.Empty;
                    }
                    string strValue = value as string;
                    if (String.IsNullOrEmpty(strValue))
                    {
                        if (this.Owner.DataGridView != null)
                        {
                            if (this.Owner.DataGridView.RightToLeft == RightToLeft.No)
                            {
                                return string.Format(SR.DataGridView_AccTopLeftColumnHeaderCellName);
                            }
                            else
                            {
                                return string.Format(SR.DataGridView_AccTopLeftColumnHeaderCellNameRTL);
                            }
                        }
                        else
                        {
                            return String.Empty;
                        }
                    }
                    else
                    {
                        return String.Empty;
                    }
                }
            }

            /// <include file='doc\DataGridViewTopLeftHeaderCell.uex' path='docs/doc[@for="DataGridViewTopLeftHeaderCellAccessibleObject.Value"]/*' />
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
                    if (this.Owner.DataGridView.AreAllCellsSelected(false /*includeInvisibleCells*/))
                    {
                        resultState |= AccessibleStates.Selected;
                    }

                    return resultState;
                }
            }

            /// <include file='doc\DataGridViewTopLeftHeaderCell.uex' path='docs/doc[@for="DataGridViewTopLeftHeaderCellAccessibleObject.Value"]/*' />
            public override string Value
            {
                [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
                get
                {
                    // We changed DataGridViewTopLeftHeaderCellAccessibleObject::Name to return a string
                    // However, DataGridViewTopLeftHeaderCellAccessibleObject::Value should still return String.Empty.
                    return String.Empty;
                }
            }

            /// <include file='doc\DataGridViewTopLeftHeaderCell.uex' path='docs/doc[@for="DataGridViewTopLeftHeaderCellAccessibleObject.DoDefaultAction"]/*' />
            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            public override void DoDefaultAction()
            {
                this.Owner.DataGridView.SelectAll();
            }

            /// <include file='doc\DataGridViewTopLeftHeaderCell.uex' path='docs/doc[@for="DataGridViewTopLeftHeaderCellAccessibleObject.Navigate"]/*' />
            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            public override AccessibleObject Navigate(AccessibleNavigation navigationDirection)
            {
                Debug.Assert(this.Owner.DataGridView.RowHeadersVisible, "if the row headers are not visible how did you get the top left header cell acc object?");
                switch (navigationDirection)
                {
                    case AccessibleNavigation.Previous:
                        return null;
                    case AccessibleNavigation.Left:
                        if (this.Owner.DataGridView.RightToLeft == RightToLeft.No)
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
                        if (this.Owner.DataGridView.RightToLeft == RightToLeft.No)
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
                if (this.Owner.DataGridView.Columns.GetColumnCount(DataGridViewElementStates.Visible) == 0)
                {
                    return null;
                }

                // return the acc object for the first visible column
                return this.Owner.DataGridView.AccessibilityObject.GetChild(0).GetChild(1);
            }

            /// <include file='doc\DataGridViewTopLeftHeaderCell.uex' path='docs/doc[@for="DataGridViewTopLeftHeaderCellAccessibleObject.Select"]/*' />
            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            public override void Select(AccessibleSelection flags)
            {
                if (this.Owner == null)
                {
                    throw new InvalidOperationException(string.Format(SR.DataGridViewCellAccessibleObject_OwnerNotSet));
                }

                // AccessibleSelection.TakeFocus should focus the grid and then focus the first data grid view data cell
                if ((flags & AccessibleSelection.TakeFocus) == AccessibleSelection.TakeFocus)
                {
                    // Focus the grid
                    this.Owner.DataGridView.FocusInternal();
                    if (this.Owner.DataGridView.Columns.GetColumnCount(DataGridViewElementStates.Visible) > 0 &&
                        this.Owner.DataGridView.Rows.GetRowCount(DataGridViewElementStates.Visible) > 0)
                    {
                        // This means that there are visible rows and columns.
                        // Focus the first data cell.
                        DataGridViewRow row = this.Owner.DataGridView.Rows[this.Owner.DataGridView.Rows.GetFirstRow(DataGridViewElementStates.Visible)];
                        DataGridViewColumn col = this.Owner.DataGridView.Columns.GetFirstColumn(DataGridViewElementStates.Visible);

                        // DataGridView::set_CurrentCell clears the previous selection.
                        // So use SetCurrenCellAddressCore directly.
                        this.Owner.DataGridView.SetCurrentCellAddressCoreInternal(col.Index, row.Index, false /*setAnchorCellAddress*/, true /*validateCurrentCell*/, false /*thoughMouseClick*/);
                    }
                }

                // AddSelection selects the entire grid.
                if ((flags & AccessibleSelection.AddSelection) == AccessibleSelection.AddSelection)
                {
                    if (this.Owner.DataGridView.MultiSelect)
                    {
                        this.Owner.DataGridView.SelectAll();
                    }
                }

                // RemoveSelection clears the selection on the entire grid.
                // But only if AddSelection is not set.
                if ((flags & AccessibleSelection.RemoveSelection) == AccessibleSelection.RemoveSelection &&
                    (flags & AccessibleSelection.AddSelection) == 0)
                {
                    this.Owner.DataGridView.ClearSelection();
                }
            }

            #region IRawElementProviderFragment Implementation

            internal override UnsafeNativeMethods.IRawElementProviderFragment FragmentNavigate(UnsafeNativeMethods.NavigateDirection direction)
            {
                var dataGridView = this.Owner.DataGridView;

                switch (direction)
                {
                    case UnsafeNativeMethods.NavigateDirection.Parent:
                        return dataGridView.AccessibilityObject.GetChild(0);
                    case UnsafeNativeMethods.NavigateDirection.PreviousSibling:
                        return null;
                    case UnsafeNativeMethods.NavigateDirection.NextSibling:
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

            internal override object GetPropertyValue(int propertyId)
            {
                if (AccessibilityImprovements.Level3)
                {
                    switch (propertyId)
                    {
                        case NativeMethods.UIA_NamePropertyId:
                            return this.Name;
                        case NativeMethods.UIA_ControlTypePropertyId:
                            return NativeMethods.UIA_HeaderControlTypeId;
                        case NativeMethods.UIA_IsEnabledPropertyId:
                            return Owner.DataGridView.Enabled;
                        case NativeMethods.UIA_HelpTextPropertyId:
                            return this.Help ?? string.Empty;
                        case NativeMethods.UIA_IsKeyboardFocusablePropertyId:
                        case NativeMethods.UIA_IsPasswordPropertyId:
                        case NativeMethods.UIA_IsOffscreenPropertyId:
                            return false;
                        case NativeMethods.UIA_AccessKeyPropertyId:
                            return string.Empty;
                    }
                }

                return base.GetPropertyValue(propertyId);
            }

            #endregion
        }
    }
}
