﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.Serialization;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms
{
    /// <summary>
    ///  This is a wrapper class to expose interesting properties of TableLayout
    /// </summary>
    [TypeConverter(typeof(TableLayoutSettingsTypeConverter))]
    [Serializable]  // This class participates in resx serialization.
    public sealed partial class TableLayoutSettings : LayoutSettings, ISerializable
    {
        private static readonly int[] borderStyleToOffset = {
            /*None = */ 0,
            /*Single = */ 1,
            /*Inset = */ 2,
            /*InsetDouble = */ 3,
            /*Outset = */ 2,
            /*OutsetDouble = */ 3,
            /*OutsetPartial = */ 3
        };
        private TableLayoutPanelCellBorderStyle _borderStyle;
        private TableLayoutSettingsStub _stub;

        // used by TableLayoutSettingsTypeConverter
        internal TableLayoutSettings() : base(null)
        {
            _stub = new TableLayoutSettingsStub();
        }

        internal TableLayoutSettings(IArrangedElement owner) : base(owner) { }

        internal TableLayoutSettings(SerializationInfo serializationInfo, StreamingContext context) : this()
        {
            TypeConverter converter = TypeDescriptor.GetConverter(this);
            string stringVal = serializationInfo.GetString("SerializedString");

            if (!string.IsNullOrEmpty(stringVal))
            {
                if (converter.ConvertFromInvariantString(stringVal) is TableLayoutSettings tls)
                {
                    ApplySettings(tls);
                }
            }
        }

        public override LayoutEngine LayoutEngine
        {
            get { return TableLayout.Instance; }
        }

        private TableLayout TableLayout
        {
            get { return (TableLayout)LayoutEngine; }
        }

        /// <summary> internal as this is a TableLayoutPanel feature only </summary>
        [DefaultValue(TableLayoutPanelCellBorderStyle.None)]
        [SRCategory(nameof(SR.CatAppearance))]
        [SRDescription(nameof(SR.TableLayoutPanelCellBorderStyleDescr))]
        internal TableLayoutPanelCellBorderStyle CellBorderStyle
        {
            get { return _borderStyle; }
            set
            {
                //valid values are 0x0 to 0x6
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)TableLayoutPanelCellBorderStyle.None, (int)TableLayoutPanelCellBorderStyle.OutsetPartial))
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidArgument, nameof(CellBorderStyle), value));
                }
                _borderStyle = value;
                //set the CellBorderWidth according to the current CellBorderStyle.
                TableLayout.ContainerInfo containerInfo = TableLayout.GetContainerInfo(Owner);
                containerInfo.CellBorderWidth = borderStyleToOffset[(int)value];
                LayoutTransaction.DoLayout(Owner, Owner, PropertyNames.CellBorderStyle);
                Debug.Assert(CellBorderStyle == value, "CellBorderStyle should be the same as we set");
            }
        }

        [DefaultValue(0)]
        internal int CellBorderWidth
        {
            get { return TableLayout.GetContainerInfo(Owner).CellBorderWidth; }
        }

        /// <summary>
        ///  This sets the maximum number of columns allowed on this table instead of allocating
        ///  actual spaces for these columns. So it is OK to set ColumnCount to Int32.MaxValue without
        ///  causing out of memory exception
        /// </summary>
        [SRDescription(nameof(SR.GridPanelColumnsDescr))]
        [SRCategory(nameof(SR.CatLayout))]
        [DefaultValue(0)]
        public int ColumnCount
        {
            get
            {
                TableLayout.ContainerInfo containerInfo = TableLayout.GetContainerInfo(Owner);
                return containerInfo.MaxColumns;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidLowBoundArgumentEx, nameof(ColumnCount), value, 0));
                }

                TableLayout.ContainerInfo containerInfo = TableLayout.GetContainerInfo(Owner);
                containerInfo.MaxColumns = value;
                LayoutTransaction.DoLayout(Owner, Owner, PropertyNames.Columns);
                Debug.Assert(ColumnCount == value, "the max columns should equal to the value we set it to");
            }
        }

        /// <summary>
        ///  This sets the maximum number of rows allowed on this table instead of allocating
        ///  actual spaces for these rows. So it is OK to set RowCount to Int32.MaxValue without
        ///  causing out of memory exception
        /// </summary>
        [SRDescription(nameof(SR.GridPanelRowsDescr))]
        [SRCategory(nameof(SR.CatLayout))]
        [DefaultValue(0)]
        public int RowCount
        {
            get
            {
                TableLayout.ContainerInfo containerInfo = TableLayout.GetContainerInfo(Owner);
                return containerInfo.MaxRows;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidLowBoundArgumentEx, nameof(RowCount), value, 0));
                }

                TableLayout.ContainerInfo containerInfo = TableLayout.GetContainerInfo(Owner);
                containerInfo.MaxRows = value;
                LayoutTransaction.DoLayout(Owner, Owner, PropertyNames.Rows);
                Debug.Assert(RowCount == value, "the max rows should equal to the value we set it to");
            }
        }

        [SRDescription(nameof(SR.GridPanelRowStylesDescr))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [SRCategory(nameof(SR.CatLayout))]
        public TableLayoutRowStyleCollection RowStyles
        {
            get
            {
                if (IsStub)
                {
                    return _stub.RowStyles;
                }
                else
                {
                    TableLayout.ContainerInfo containerInfo = TableLayout.GetContainerInfo(Owner);
                    return containerInfo.RowStyles;
                }
            }
        }

        [SRDescription(nameof(SR.GridPanelColumnStylesDescr))]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        [SRCategory(nameof(SR.CatLayout))]
        public TableLayoutColumnStyleCollection ColumnStyles
        {
            get
            {
                if (IsStub)
                {
                    return _stub.ColumnStyles;
                }
                else
                {
                    TableLayout.ContainerInfo containerInfo = TableLayout.GetContainerInfo(Owner);
                    return containerInfo.ColumnStyles;
                }
            }
        }

        /// <summary>
        ///  Specifies if a TableLayoutPanel will gain additional rows or columns once its existing cells
        ///  become full.  If the value is 'FixedSize' then the TableLayoutPanel will throw an exception
        ///  when the TableLayoutPanel is over-filled.
        /// </summary>
        [SRDescription(nameof(SR.TableLayoutPanelGrowStyleDescr))]
        [SRCategory(nameof(SR.CatLayout))]
        [DefaultValue(TableLayoutPanelGrowStyle.AddRows)]
        public TableLayoutPanelGrowStyle GrowStyle
        {
            get
            {
                return TableLayout.GetContainerInfo(Owner).GrowStyle;
            }

            set
            {
                //valid values are 0x0 to 0x2
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)TableLayoutPanelGrowStyle.FixedSize, (int)TableLayoutPanelGrowStyle.AddColumns))
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidArgument, nameof(GrowStyle), value));
                }

                TableLayout.ContainerInfo containerInfo = TableLayout.GetContainerInfo(Owner);
                if (containerInfo.GrowStyle != value)
                {
                    containerInfo.GrowStyle = value;
                    LayoutTransaction.DoLayout(Owner, Owner, PropertyNames.GrowStyle);
                }
            }
        }

        internal bool IsStub
        {
            get
            {
                if (_stub != null)
                {
                    return true;
                }
                return false;
            }
        }

        internal void ApplySettings(TableLayoutSettings settings)
        {
            if (settings.IsStub)
            {
                if (!IsStub)
                {
                    // we're the real-live thing here, gotta walk through and touch controls
                    settings._stub.ApplySettings(this);
                }
                else
                {
                    // we're just copying another stub into us, just replace the member
                    _stub = settings._stub;
                }
            }
        }

        #region Extended Properties

        public int GetColumnSpan(object control)
        {
            if (control == null)
            {
                throw new ArgumentNullException(nameof(control));
            }

            if (IsStub)
            {
                return _stub.GetColumnSpan(control);
            }
            else
            {
                IArrangedElement element = LayoutEngine.CastToArrangedElement(control);
                return TableLayout.GetLayoutInfo(element).ColumnSpan;
            }
        }

        public void SetColumnSpan(object control, int value)
        {
            if (control == null)
            {
                throw new ArgumentNullException(nameof(control));
            }
            if (value < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidArgument, nameof(value), value));
            }

            if (IsStub)
            {
                _stub.SetColumnSpan(control, value);
            }
            else
            {
                IArrangedElement element = LayoutEngine.CastToArrangedElement(control);
                if (element.Container != null)
                {
                    TableLayout.ClearCachedAssignments(TableLayout.GetContainerInfo(element.Container));
                }
                TableLayout.GetLayoutInfo(element).ColumnSpan = value;
                LayoutTransaction.DoLayout(element.Container, element, PropertyNames.ColumnSpan);
                Debug.Assert(GetColumnSpan(element) == value, "column span should equal to the value we set");
            }
        }

        public int GetRowSpan(object control)
        {
            if (control == null)
            {
                throw new ArgumentNullException(nameof(control));
            }

            if (IsStub)
            {
                return _stub.GetRowSpan(control);
            }
            else
            {
                IArrangedElement element = LayoutEngine.CastToArrangedElement(control);
                return TableLayout.GetLayoutInfo(element).RowSpan;
            }
        }

        public void SetRowSpan(object control, int value)
        {
            if (control == null)
            {
                throw new ArgumentNullException(nameof(control));
            }
            if (value < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidArgument, nameof(value), value));
            }

            if (IsStub)
            {
                _stub.SetRowSpan(control, value);
            }
            else
            {
                IArrangedElement element = LayoutEngine.CastToArrangedElement(control);
                if (element.Container != null)
                {
                    TableLayout.ClearCachedAssignments(TableLayout.GetContainerInfo(element.Container));
                }
                TableLayout.GetLayoutInfo(element).RowSpan = value;
                LayoutTransaction.DoLayout(element.Container, element, PropertyNames.RowSpan);
                Debug.Assert(GetRowSpan(element) == value, "row span should equal to the value we set");
            }
        }

        /// <summary>
        ///  Get the row position of the element
        /// </summary>
        [SRDescription(nameof(SR.GridPanelRowDescr))]
        [SRCategory(nameof(SR.CatLayout))]
        [DefaultValue(-1)]
        public int GetRow(object control)
        {
            if (control == null)
            {
                throw new ArgumentNullException(nameof(control));
            }

            if (IsStub)
            {
                return _stub.GetRow(control);
            }
            else
            {
                IArrangedElement element = LayoutEngine.CastToArrangedElement(control);
                TableLayout.LayoutInfo layoutInfo = TableLayout.GetLayoutInfo(element);
                return layoutInfo.RowPosition;
            }
        }

        /// <summary>
        ///  Set the row position of the element
        ///  If we set the row position to -1, it will automatically switch the control from
        ///  absolutely positioned to non-absolutely positioned
        /// </summary>
        public void SetRow(object control, int row)
        {
            if (control == null)
            {
                throw new ArgumentNullException(nameof(control));
            }
            if (row < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(row), row, string.Format(SR.InvalidArgument, nameof(row), row));
            }

            SetCellPosition(control, row, -1, rowSpecified: true, colSpecified: false);
        }

        /// <summary>
        ///  Get the column position of the element
        /// </summary>
        [SRDescription(nameof(SR.TableLayoutSettingsGetCellPositionDescr))]
        [SRCategory(nameof(SR.CatLayout))]
        [DefaultValue(-1)]
        public TableLayoutPanelCellPosition GetCellPosition(object control)
        {
            if (control == null)
            {
                throw new ArgumentNullException(nameof(control));
            }

            return new TableLayoutPanelCellPosition(GetColumn(control), GetRow(control));
        }

        /// <summary>
        ///  Set the column position of the element
        /// </summary>
        [SRDescription(nameof(SR.TableLayoutSettingsSetCellPositionDescr))]
        [SRCategory(nameof(SR.CatLayout))]
        [DefaultValue(-1)]
        public void SetCellPosition(object control, TableLayoutPanelCellPosition cellPosition)
        {
            if (control == null)
            {
                throw new ArgumentNullException(nameof(control));
            }

            SetCellPosition(control, cellPosition.Row, cellPosition.Column, rowSpecified: true, colSpecified: true);
        }

        /// <summary>
        ///  Get the column position of the element
        /// </summary>
        [SRDescription(nameof(SR.GridPanelColumnDescr))]
        [SRCategory(nameof(SR.CatLayout))]
        [DefaultValue(-1)]
        public int GetColumn(object control)
        {
            if (control == null)
            {
                throw new ArgumentNullException(nameof(control));
            }

            if (IsStub)
            {
                return _stub.GetColumn(control);
            }
            else
            {
                IArrangedElement element = LayoutEngine.CastToArrangedElement(control);
                TableLayout.LayoutInfo layoutInfo = TableLayout.GetLayoutInfo(element);
                return layoutInfo.ColumnPosition;
            }
        }

        /// <summary>
        ///  Set the column position of the element
        ///  If we set the column position to -1, it will automatically switch the control from
        ///  absolutely positioned to non-absolutely positioned
        /// </summary>
        public void SetColumn(object control, int column)
        {
            if (control == null)
            {
                throw new ArgumentNullException(nameof(control));
            }
            if (column < -1)
            {
                throw new ArgumentOutOfRangeException(nameof(column), column, string.Format(SR.InvalidArgument, nameof(column), column));
            }

            if (IsStub)
            {
                _stub.SetColumn(control, column);
            }
            else
            {
                SetCellPosition(control, -1, column, rowSpecified: false, colSpecified: true);
            }
        }

        private void SetCellPosition(object control, int row, int column, bool rowSpecified, bool colSpecified)
        {
            if (IsStub)
            {
                if (colSpecified)
                {
                    _stub.SetColumn(control, column);
                }
                if (rowSpecified)
                {
                    _stub.SetRow(control, row);
                }
            }
            else
            {
                IArrangedElement element = LayoutEngine.CastToArrangedElement(control);
                if (element.Container != null)
                {
                    TableLayout.ClearCachedAssignments(TableLayout.GetContainerInfo(element.Container));
                }
                TableLayout.LayoutInfo layoutInfo = TableLayout.GetLayoutInfo(element);
                if (colSpecified)
                {
                    layoutInfo.ColumnPosition = column;
                }
                if (rowSpecified)
                {
                    layoutInfo.RowPosition = row;
                }
                LayoutTransaction.DoLayout(element.Container, element, PropertyNames.TableIndex);
                Debug.Assert(!colSpecified || GetColumn(element) == column, "column position shoule equal to what we set");
                Debug.Assert(!rowSpecified || GetRow(element) == row, "row position shoule equal to what we set");
            }
        }

        /// <summary>
        ///  Get the element which covers the specified row and column. return null if we can't find one
        /// </summary>
        internal IArrangedElement GetControlFromPosition(int column, int row)
        {
            return TableLayout.GetControlFromPosition(Owner, column, row);
        }

        internal TableLayoutPanelCellPosition GetPositionFromControl(IArrangedElement element)
        {
            return TableLayout.GetPositionFromControl(Owner, element);
        }

        #endregion

        void ISerializable.GetObjectData(SerializationInfo si, StreamingContext context)
        {
            TypeConverter converter = TypeDescriptor.GetConverter(this);
            string stringVal = converter.ConvertToInvariantString(this);

            if (!string.IsNullOrEmpty(stringVal))
            {
                si.AddValue("SerializedString", stringVal);
            }
        }

        internal List<ControlInformation> GetControlsInformation()
        {
            if (IsStub)
            {
                return _stub.GetControlsInformation();
            }
            else
            {
                List<ControlInformation> controlsInfo = new List<ControlInformation>(Owner.Children.Count);

                foreach (IArrangedElement element in Owner.Children)
                {
                    if (element is Control c)
                    {
                        ControlInformation controlInfo = new ControlInformation();

                        // We need to go through the PropertyDescriptor for the Name property
                        // since it is shadowed.
                        PropertyDescriptor prop = TypeDescriptor.GetProperties(c)["Name"];
                        if (prop != null && prop.PropertyType == typeof(string))
                        {
                            controlInfo.Name = prop.GetValue(c);
                        }

                        controlInfo.Row = GetRow(c);
                        controlInfo.RowSpan = GetRowSpan(c);
                        controlInfo.Column = GetColumn(c);
                        controlInfo.ColumnSpan = GetColumnSpan(c);
                        controlsInfo.Add(controlInfo);
                    }
                }
                return controlsInfo;
            }
        }
    }
}
