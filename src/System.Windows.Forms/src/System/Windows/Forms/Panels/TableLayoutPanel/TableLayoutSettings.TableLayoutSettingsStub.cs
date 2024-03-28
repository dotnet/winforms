// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Windows.Forms.Layout;

namespace System.Windows.Forms;

public sealed partial class TableLayoutSettings
{
    private class TableLayoutSettingsStub
    {
        private static ControlInformation s_defaultControlInfo = new(null, -1, -1, 1, 1);
        private TableLayoutColumnStyleCollection? _columnStyles;
        private TableLayoutRowStyleCollection? _rowStyles;
        private Dictionary<object, ControlInformation>? _controlsInfo;

        public TableLayoutSettingsStub()
        {
        }

        /// <summary>
        ///  Applies settings from the stub into a full-fledged TableLayoutSettings.
        ///
        ///  NOTE: this is a one-time only operation - there is data loss to the stub
        ///  as a result of calling this function. We hand as much over to the other settings
        ///  so we don't have to reallocate anything
        /// </summary>
        internal void ApplySettings(TableLayoutSettings settings)
        {
            // apply row,column,rowspan,colspan
            TableLayout.ContainerInfo containerInfo = TableLayout.GetContainerInfo(settings.Owner!);
            if (containerInfo.Container is Control appliedControl && _controlsInfo is not null)
            {
                // we store the control names, look up the controls
                // in the appliedControl's control collection and apply the row,column settings.
                foreach (object controlName in _controlsInfo.Keys)
                {
                    ControlInformation controlInfo = _controlsInfo[controlName];

                    // Look for the control in our table, we have to go through
                    // PropertyDescriptor rather than just going using appliedControl.Controls[controlName]
                    // because the Name property is shadowed at design time
                    foreach (Control tableControl in appliedControl.Controls)
                    {
                        if (tableControl is not null)
                        {
                            string? name = null;
                            PropertyDescriptor? prop = TypeDescriptor.GetProperties(tableControl)["Name"];
                            if (prop is not null && prop.PropertyType == typeof(string))
                            {
                                name = prop.GetValue(tableControl) as string;
                            }

                            if (WindowsFormsUtils.SafeCompareStrings(name, controlName as string, ignoreCase: false))
                            {
                                settings.SetRow(tableControl, controlInfo.Row);
                                settings.SetColumn(tableControl, controlInfo.Column);
                                settings.SetRowSpan(tableControl, controlInfo.RowSpan);
                                settings.SetColumnSpan(tableControl, controlInfo.ColumnSpan);
                                break;
                            }
                        }
                    }
                }
            }

            // Assign over the row and column styles
            containerInfo.RowStyles = _rowStyles;
            containerInfo.ColumnStyles = _columnStyles;

            // Since we've given over the styles to the other guy, null out.
            _columnStyles = null;
            _rowStyles = null;
        }

        public TableLayoutColumnStyleCollection ColumnStyles => _columnStyles ??= [];

        public TableLayoutRowStyleCollection RowStyles => _rowStyles ??= [];

        internal List<ControlInformation> GetControlsInformation()
        {
            if (_controlsInfo is null)
            {
                return [];
            }

            List<ControlInformation> listOfControlInfo = new(_controlsInfo.Count);
            foreach (object name in _controlsInfo.Keys)
            {
                ControlInformation ci = _controlsInfo[name];
                ci.Name = name;
                listOfControlInfo.Add(ci);
            }

            return listOfControlInfo;
        }

        private ControlInformation GetControlInformation(object controlName)
        {
            if (_controlsInfo is null)
            {
                return s_defaultControlInfo;
            }

            return _controlsInfo.GetValueOrDefault(controlName, s_defaultControlInfo);
        }

        public int GetColumn(object controlName) => GetControlInformation(controlName).Column;

        public int GetColumnSpan(object controlName) => GetControlInformation(controlName).ColumnSpan;

        public int GetRow(object controlName) => GetControlInformation(controlName).Row;

        public int GetRowSpan(object controlName) => GetControlInformation(controlName).RowSpan;

        private void SetControlInformation(object controlName, ControlInformation info)
        {
            _controlsInfo ??= [];
            _controlsInfo[controlName] = info;
        }

        public void SetColumn(object controlName, int column)
        {
            if (GetColumn(controlName) != column)
            {
                ControlInformation info = GetControlInformation(controlName);
                info.Column = column;
                SetControlInformation(controlName, info);
            }
        }

        public void SetColumnSpan(object controlName, int value)
        {
            if (GetColumnSpan(controlName) != value)
            {
                ControlInformation info = GetControlInformation(controlName);
                info.ColumnSpan = value;
                SetControlInformation(controlName, info);
            }
        }

        public void SetRow(object controlName, int row)
        {
            if (GetRow(controlName) != row)
            {
                ControlInformation info = GetControlInformation(controlName);
                info.Row = row;
                SetControlInformation(controlName, info);
            }
        }

        public void SetRowSpan(object controlName, int value)
        {
            if (GetRowSpan(controlName) != value)
            {
                ControlInformation info = GetControlInformation(controlName);
                info.RowSpan = value;
                SetControlInformation(controlName, info);
            }
        }
    }
}
