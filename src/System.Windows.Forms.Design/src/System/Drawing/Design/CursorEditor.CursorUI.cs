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
        private IWindowsFormsEditorService? _editorService;
        private readonly TypeConverter _cursorConverter;
        private readonly Dictionary<(Cursor cursor, int dpi), int> _cursorWidthCache = new();

        public CursorUI()
        {
            Height = ScaleHelper.IsScalingRequired ? ScaleHelper.ScaleToInitialSystemDpi(310) : 310;

            ItemHeight = Math.Max(4 + Cursors.Default.Size.Height, Font.Height);
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

        public void End()
        {
            _editorService = null;
            Value = null;
            _cursorWidthCache.Clear();
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
                int cursorWidth = GetCursorWidthForDpi(cursor, DeviceDpi);

                e.DrawBackground();
                e.Graphics.FillRectangle(SystemBrushes.Control, new Rectangle(e.Bounds.X + 2, e.Bounds.Y + 2, cursorWidth, e.Bounds.Height - 4));
                e.Graphics.DrawRectangle(SystemPens.WindowText, new Rectangle(e.Bounds.X + 2, e.Bounds.Y + 2, cursorWidth - 1, e.Bounds.Height - 4 - 1));

                cursor.DrawStretched(e.Graphics, new Rectangle(e.Bounds.X + 2, e.Bounds.Y + 2, cursorWidth, e.Bounds.Height - 4));
                e.Graphics.DrawString(text, font, brushText, e.Bounds.X + cursorWidth + 4, e.Bounds.Y + (e.Bounds.Height - font.Height) / 2);
            }
        }

        private int GetCursorWidthForDpi(Cursor cursor, int dpi)
        {
            (Cursor cursor, int dpi) key = (cursor, dpi);
            if (_cursorWidthCache.TryGetValue(key, out int cursorWidth))
            {
                return cursorWidth;
            }

            using Icon wrapper = Icon.FromHandle(cursor.Handle);
            using Icon clone = (Icon)wrapper.Clone();
            using Icon scaled = ScaleHelper.ScaleSmallIconToDpi(clone, dpi, alwaysCreateNew: true);

            cursorWidth = scaled.Size.Width;
            _cursorWidthCache[key] = cursorWidth;
            return cursorWidth;
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
