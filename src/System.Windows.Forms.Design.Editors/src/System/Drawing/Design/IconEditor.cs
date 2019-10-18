// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace System.Drawing.Design
{
    // NOTE: this class should be almost identical to ImageEditor.  The main exception is PaintValue, which has logic that should probably be copied into ImageEditor.

    /// <summary>
    /// Provides an editor for visually picking an icon.
    /// </summary>
    public class IconEditor : UITypeEditor
    {
        private static List<string> s_iconExtensions = new List<string>() { "ico" };
        private static Type[] s_imageExtenders = Array.Empty<Type>();
        private FileDialog _fileDialog;

        protected static string CreateExtensionsString(string[] extensions, string sep)
        {
            const string StarDot = "*.";

            if (extensions == null || extensions.Length == 0)
            {
                return null;
            }

            string text = string.Empty;
            for (int i = 0; i < extensions.Length; i++)
            {
                text = text + StarDot + extensions[i];
                if (i < extensions.Length - 1)
                {
                    text += sep;
                }
            }

            return text;
        }

        protected static string CreateFilterEntry(IconEditor editor)
        {
            const string Comma = ",";
            const string SemiColon = ";";
            const string LeftParan = "(";
            const string RightParan = ")";
            const string Pipe = "|";

            string desc = editor.GetFileDialogDescription();
            string exts = CreateExtensionsString(editor.GetExtensions(), Comma);
            string extsSemis = CreateExtensionsString(editor.GetExtensions(), SemiColon);
            return desc + LeftParan + exts + RightParan + Pipe + extsSemis;
        }

        /// <summary>
        /// Edits the given object value using the editor style provided by
        /// GetEditorStyle.  A service provider is provided so that any
        /// required editing services can be obtained.
        /// </summary>
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider == null)
            {
                return value;
            }

            var edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            if (edSvc == null)
            {
                return value;
            }

            if (_fileDialog == null)
            {
                _fileDialog = new OpenFileDialog();
                var filter = CreateFilterEntry(this);

                Debug.Assert(s_imageExtenders.Length <= 0, "Why does IconEditor have subclasses if Icon doesn't?");

                _fileDialog.Filter = filter;
            }

            IntPtr hwndFocus = UnsafeNativeMethods.GetFocus();
            try
            {
                if (_fileDialog.ShowDialog() == DialogResult.OK)
                {
                    var file = new FileStream(_fileDialog.FileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                    value = LoadFromStream(file);
                }
            }
            finally
            {
                if (hwndFocus != IntPtr.Zero)
                {
                    UnsafeNativeMethods.SetFocus(new HandleRef(null, hwndFocus));
                }
            }

            return value;
        }

        /// <summary>
        /// Retrieves the editing style of the Edit method.  If the method
        /// is not supported, this will return None.
        /// </summary>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
            => UITypeEditorEditStyle.Modal;

        protected virtual string GetFileDialogDescription()
            => SR.iconFileDescription;

        protected virtual string[] GetExtensions()
            => s_iconExtensions.ToArray();

        /// <summary>
        /// Determines if this editor supports the painting of a representation
        /// of an object's value.
        /// </summary>
        public override bool GetPaintValueSupported(ITypeDescriptorContext context)
            => true;

        protected virtual Icon LoadFromStream(Stream stream)
            => new Icon(stream);

        /// <summary>
        /// Paints a representative value of the given object to the provided
        /// canvas. Painting should be done within the boundaries of the
        /// provided rectangle.
        /// </summary>
        public override void PaintValue(PaintValueEventArgs e)
        {
            if (!(e?.Value is Icon icon))
            {
                return;
            }

            // If icon is smaller than rectangle, just center it unscaled in the rectangle
            Rectangle rectangle = e.Bounds;
            if (icon.Width < rectangle.Width)
            {
                rectangle.X = (rectangle.Width - icon.Width) / 2;
                rectangle.Width = icon.Width;
            }
            if (icon.Height < rectangle.Height)
            {
                rectangle.X = (rectangle.Height - icon.Height) / 2;
                rectangle.Height = icon.Height;
            }

            e.Graphics.DrawIcon(icon, rectangle);
        }
    }
}
