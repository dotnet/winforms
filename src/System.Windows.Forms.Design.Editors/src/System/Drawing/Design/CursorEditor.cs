// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Diagnostics;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace System.Drawing.Design
{
    /// <summary>
    ///  Provides an editor that can perform default file searching for cursor (.cur) files.
    /// </summary>
    [CLSCompliant(false)]
    public class CursorEditor : UITypeEditor
    {
        private CursorUI _cursorUI;

        /// <summary>
        ///  Returns true, indicating that this drop-down control can be resized.
        /// </summary>
        public override bool IsDropDownResizable => true;

        /// <summary>
        ///  Edits the given object value using the editor style provided by GetEditorStyle. A service
        ///  provider is provided so that any required editing services can be obtained.
        /// </summary>
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider == null)
            {
                return value;
            }
            if (!(provider.GetService(typeof(IWindowsFormsEditorService)) is IWindowsFormsEditorService edSvc))
            {
                return value;
            }

            if (_cursorUI == null)
            {
                _cursorUI = new CursorUI(this);
            }

            _cursorUI.Start(edSvc, value);
            edSvc.DropDownControl(_cursorUI);
            value = _cursorUI.Value;
            _cursorUI.End();

            return value;
        }

        /// <summary>
        ///  Retrieves the editing style of the Edit method.
        /// </summary>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }

        /// <summary>
        ///  The user interface for the cursor drop-down. This is just an owner-drawn list box.
        /// </summary>
        private class CursorUI : ListBox
        {
            private object value;
            private IWindowsFormsEditorService edSvc;
            private readonly TypeConverter cursorConverter;
            private UITypeEditor editor;

            public CursorUI(UITypeEditor editor)
            {
                this.editor = editor;

                Height = 310;
                ItemHeight = (int)Math.Max(4 + Cursors.Default.Size.Height, Font.Height);
                DrawMode = DrawMode.OwnerDrawFixed;
                BorderStyle = BorderStyle.None;

                cursorConverter = TypeDescriptor.GetConverter(typeof(Cursor));
                Debug.Assert(cursorConverter.GetStandardValuesSupported(), "Converter '" + cursorConverter.ToString() + "' does not support a list of standard values. We cannot provide a drop-down");

                // Fill the list with cursors.
                //
                if (cursorConverter.GetStandardValuesSupported())
                {
                    foreach (object obj in cursorConverter.GetStandardValues())
                    {
                        Items.Add(obj);
                    }
                }
            }

            public object Value => value;

            public void End()
            {
                edSvc = null;
                value = null;
            }

            protected override void OnClick(EventArgs e)
            {
                base.OnClick(e);
                value = SelectedItem;
                edSvc.CloseDropDown();
            }

            protected override void OnDrawItem(DrawItemEventArgs die)
            {
                base.OnDrawItem(die);

                if (die.Index != -1)
                {
                    Cursor cursor = (Cursor)Items[die.Index];
                    string text = cursorConverter.ConvertToString(cursor);
                    Font font = die.Font;
                    Brush brushText = new SolidBrush(die.ForeColor);

                    die.DrawBackground();
                    die.Graphics.FillRectangle(SystemBrushes.Control, new Rectangle(die.Bounds.X + 2, die.Bounds.Y + 2, 32, die.Bounds.Height - 4));
                    die.Graphics.DrawRectangle(SystemPens.WindowText, new Rectangle(die.Bounds.X + 2, die.Bounds.Y + 2, 32 - 1, die.Bounds.Height - 4 - 1));

                    cursor.DrawStretched(die.Graphics, new Rectangle(die.Bounds.X + 2, die.Bounds.Y + 2, 32, die.Bounds.Height - 4));
                    die.Graphics.DrawString(text, font, brushText, die.Bounds.X + 36, die.Bounds.Y + (die.Bounds.Height - font.Height) / 2);

                    brushText.Dispose();
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

            public void Start(IWindowsFormsEditorService edSvc, object value)
            {
                this.edSvc = edSvc;
                this.value = value;

                // Select the current cursor
                if (value != null)
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
}

