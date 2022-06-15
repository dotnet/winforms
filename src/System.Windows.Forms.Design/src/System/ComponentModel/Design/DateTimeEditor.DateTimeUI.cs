// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace System.ComponentModel.Design
{
    public partial class DateTimeEditor
    {
        /// <summary>
        ///  UI we drop down to pick dates.
        /// </summary>
        private class DateTimeUI : Control
        {
            private readonly MonthCalendar _monthCalendar = new DateTimeMonthCalendar();
            private IWindowsFormsEditorService _editorService;

            public DateTimeUI()
            {
                InitializeComponent();
                Size = _monthCalendar.SingleMonthSize;
                _monthCalendar.Resize += MonthCalResize;
            }

            public object Value { get; private set; }

            public void End()
            {
                _editorService = null;
                Value = null;
            }

            private void MonthCalKeyDown(object sender, KeyEventArgs e)
            {
                switch (e.KeyCode)
                {
                    case Keys.Enter:
                        OnDateSelected(sender, null);
                        break;
                }
            }

            protected override void RescaleConstantsForDpi(int deviceDpiOld, int deviceDpiNew)
            {
                base.RescaleConstantsForDpi(deviceDpiOld, deviceDpiNew);

                // Resizing the editor to fit to the SingleMonth size after Dpi changed.
                Size = _monthCalendar.SingleMonthSize;
            }

            private void InitializeComponent()
            {
                _monthCalendar.DateSelected += OnDateSelected;
                _monthCalendar.KeyDown += MonthCalKeyDown;
                Controls.Add(_monthCalendar);
            }

            private void MonthCalResize(object sender, EventArgs e)
            {
                Size = _monthCalendar.Size;
            }

            private void OnDateSelected(object sender, DateRangeEventArgs e)
            {
                Value = _monthCalendar.SelectionStart;
                _editorService.CloseDropDown();
            }

            protected override void OnGotFocus(EventArgs e)
            {
                base.OnGotFocus(e);
                _monthCalendar.Focus();
            }

            public void Start(IWindowsFormsEditorService editorService, object value)
            {
                _editorService = editorService;
                Value = value;

                if (value is not null)
                {
                    DateTime dateTime = (DateTime)value;
                    _monthCalendar.SetDate((dateTime.Equals(DateTime.MinValue)) ? DateTime.Today : dateTime);
                }
            }

            class DateTimeMonthCalendar : MonthCalendar
            {
                protected override bool IsInputKey(Keys keyData) => keyData switch
                {
                    Keys.Enter => true,
                    _ => base.IsInputKey(keyData),
                };
            }
        }
    }
}
