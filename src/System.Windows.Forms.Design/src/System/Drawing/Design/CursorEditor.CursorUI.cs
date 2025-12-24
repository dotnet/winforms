// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace System.Drawing.Design;

public partial class CursorEditor
{
    /// <summary>
    ///  The user interface for the cursor drop-down. This is just an owner-drawn list box.
    /// </summary>
    private class CursorUI : ListBox
    {
        // Logical (96 DPI) padding values
        private const int LogicalItemPadding = 4; // 2 pixels top + 2 pixels bottom
        private const int LogicalItemMargin = 2; // Margin on each side
        private const int LogicalTextSpacing = 4; // Spacing between cursor and text

        private IWindowsFormsEditorService? _editorService;
        private readonly TypeConverter _cursorConverter;

        public CursorUI()
        {
            Height = ScaleHelper.IsScalingRequired ? ScaleHelper.ScaleToInitialSystemDpi(310) : 310;

            UpdateItemHeight();
            DrawMode = DrawMode.OwnerDrawFixed;
            BorderStyle = BorderStyle.None;

            _cursorConverter = TypeDescriptor.GetConverter(typeof(Cursor));
            Debug.Assert(
                _cursorConverter.GetStandardValuesSupported(),
                $"Converter '{_cursorConverter}' does not support a list of standard values. We cannot provide a drop-down");

            // Fill the list with cursors.
            if (_cursorConverter.GetStandardValuesSupported())
            {
                foreach (object obj in _cursorConverter.GetStandardValues()!)
                {
                    Items.Add(obj);
                }
            }
        }

        public object? Value { get; private set; }

        private void UpdateItemHeight()
        {
            // Get the cursor (small icon) height at the current device DPI
            int cursorHeight = PInvoke.GetCurrentSystemMetrics(SYSTEM_METRICS_INDEX.SM_CYSMICON, (uint)DeviceDpi);
            int scaledPadding = ScaleHelper.ScaleToDpi(LogicalItemPadding, DeviceDpi);

            ItemHeight = Math.Max(scaledPadding + cursorHeight, Font.Height);
        }

        protected override void RescaleConstantsForDpi(int deviceDpiOld, int deviceDpiNew)
        {
            base.RescaleConstantsForDpi(deviceDpiOld, deviceDpiNew);
            UpdateItemHeight();
        }

        public void End()
        {
            _editorService = null;
            Value = null;
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            Value = SelectedItem;
            _editorService!.CloseDropDown();
        }

        protected override void OnDrawItem(DrawItemEventArgs e)
        {
            base.OnDrawItem(e);

            if (e.Index != -1)
            {
                Cursor cursor = (Cursor)Items[e.Index];
                string? text = _cursorConverter.ConvertToString(cursor);
                Font font = e.Font!;
                using var brushText = e.ForeColor.GetCachedSolidBrushScope();
                var cursorWidth = ScaleHelper.ScaleSmallIconToDpi(Icon.FromHandle(cursor.Handle), DeviceDpi).Size.Width;

                int margin = ScaleHelper.ScaleToDpi(LogicalItemMargin, DeviceDpi);
                int padding = ScaleHelper.ScaleToDpi(LogicalItemPadding, DeviceDpi);
                int textSpacing = ScaleHelper.ScaleToDpi(LogicalTextSpacing, DeviceDpi);

                e.DrawBackground();
                e.Graphics.FillRectangle(SystemBrushes.Control, new Rectangle(e.Bounds.X + margin, e.Bounds.Y + margin, cursorWidth, e.Bounds.Height - padding));
                e.Graphics.DrawRectangle(SystemPens.WindowText, new Rectangle(e.Bounds.X + margin, e.Bounds.Y + margin, cursorWidth - 1, e.Bounds.Height - padding - 1));

                cursor.DrawStretched(e.Graphics, new Rectangle(e.Bounds.X + margin, e.Bounds.Y + margin, cursorWidth, e.Bounds.Height - padding));
                e.Graphics.DrawString(text, font, brushText, e.Bounds.X + margin + cursorWidth + textSpacing, e.Bounds.Y + (e.Bounds.Height - font.Height) / 2);
            }
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            if ((keyData & Keys.KeyCode) == Keys.Return && (keyData & (Keys.Alt | Keys.Control)) == 0)
            {
                OnClick(EventArgs.Empty);
                return true;
            }

            return base.ProcessDialogKey(keyData);
        }

        public void Start(IWindowsFormsEditorService editorService, object? value)
        {
            _editorService = editorService;
            Value = value;

            // Select the current cursor
            if (value is not null)
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    if (Items[i] == value)
                    {
                        SelectedIndex = i;
                        break;
                    }
                }
            }
        }
    }
}
