// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Microsoft.Win32;

using IComDataObject = System.Runtime.InteropServices.ComTypes.IDataObject;
using static Interop;

namespace System.ComponentModel.Design
{
    /// <summary>
    ///  An editor for editing strings that supports multiple lines of text and is resizable.
    /// </summary>
    public sealed class MultilineStringEditor : UITypeEditor
    {
        private MultilineStringEditorUI _editorUI;

        /// <summary>
        ///  Edits the given value, returning the editing results.
        /// </summary>
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (provider != null)
            {
                if (provider.GetService(typeof(IWindowsFormsEditorService)) is IWindowsFormsEditorService edSvc)
                {
                    if (_editorUI == null)
                    {
                        _editorUI = new MultilineStringEditorUI();
                    }

                    _editorUI.BeginEdit(edSvc, value);
                    edSvc.DropDownControl(_editorUI);
                    object newValue = _editorUI.Value;
                    if (_editorUI.EndEdit())
                    {
                        value = newValue;
                    }
                }
            }

            return value;
        }

        /// <summary>
        ///  The MultilineStringEditor is a drop down editor, so this returns UITypeEditorEditStyle.DropDown.
        /// </summary>
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }

        /// <summary>
        ///  Returns false; no extra painting is performed.
        /// </summary>
        public override bool GetPaintValueSupported(ITypeDescriptorContext context)
        {
            return false;
        }

        private class MultilineStringEditorUI : RichTextBox
        {
            private IWindowsFormsEditorService _editorService;
            private bool _editing = false;
            private bool _escapePressed; // Initialized in BeginEdit
            private bool _ctrlEnterPressed;
            SolidBrush _watermarkBrush;
            private readonly Hashtable _fallbackFonts;
            private bool _firstTimeResizeToContent = true;

            private readonly StringFormat _watermarkFormat;

            // TextBox needs a little space greater than that actualy text content to display the carent.
            private const int _caretPadding = 3;

            internal MultilineStringEditorUI()
            {
                InitializeComponent();
                _watermarkFormat = new StringFormat
                {
                    Alignment = StringAlignment.Center,
                    LineAlignment = StringAlignment.Center
                };
                _fallbackFonts = new Hashtable(2);
            }

            private void InitializeComponent()
            {
                RichTextShortcutsEnabled = false;
                WordWrap = false;
                BorderStyle = BorderStyle.None;
                Multiline = true;
                ScrollBars = RichTextBoxScrollBars.Both;
                DetectUrls = false;
            }

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    if (_watermarkBrush != null)
                    {
                        _watermarkBrush.Dispose();
                        _watermarkBrush = null;
                    }
                }
                base.Dispose(disposing);
            }

            protected override object CreateRichEditOleCallback()
            {
                return new OleCallback(this);
            }

            protected override bool IsInputKey(Keys keyData)
            {
                if ((keyData & Keys.KeyCode) == Keys.Return)
                {
                    if (Multiline && (keyData & Keys.Alt) == 0)
                    {
                        return true;
                    }
                }
                return base.IsInputKey(keyData);
            }

            protected override bool ProcessDialogKey(Keys keyData)
            {
                if ((keyData & (Keys.Shift | Keys.Alt)) == 0)
                {
                    switch (keyData & Keys.KeyCode)
                    {
                        case Keys.Escape:
                            if ((keyData & Keys.Control) == 0)
                            {
                                // Returned by EndEdit to signal that we should disregard changes.
                                _escapePressed = true;
                            }
                            break;
                    }
                }
                return base.ProcessDialogKey(keyData);
            }

            protected override void OnKeyDown(KeyEventArgs e)
            {
                // The RichTextBox does not always invalidate the entire client area unless there is a resize, so if you do a newline before you type enough text to resize the editor the watermark will be ScrollWindowEx'ed down the screen.  To prevent this, we do a full Invalidate if the watermark is showing when a key is pressed.
                if (ShouldShowWatermark)
                {
                    Invalidate();
                }

                // Ask the editor service to ask my parent to close when the user types "Ctrl+Enter".
                if (e.Control && e.KeyCode == Keys.Return && e.Modifiers == Keys.Control)
                {
                    _editorService.CloseDropDown();
                    _ctrlEnterPressed = true;
                }
            }

            internal object Value
            {
                get
                {
                    Debug.Assert(_editing, "Value is only valid between Begin and EndEdit. (Do not want to keep a reference to a large text buffer.)");
                    return Text;
                }
            }

            internal void BeginEdit(IWindowsFormsEditorService editorService, object value)
            {
                _editing = true;
                _editorService = editorService;
                _minimumSize = Size.Empty;
                _watermarkSize = Size.Empty;
                _escapePressed = false;
                _ctrlEnterPressed = false;
                Text = value as string;
            }

            internal bool EndEdit()
            {
                _editing = false;
                _editorService = null;
                _ctrlEnterPressed = false;
                Text = null;
                return !_escapePressed; // If user pressed Esc, return false so we disregard changes.
            }

            private void ResizeToContent()
            {
                if (_firstTimeResizeToContent)
                {
                    _firstTimeResizeToContent = false;
                }
                else if (!Visible)
                {
                    return;
                }

                Size requestedSize = ContentSize;
                // AdjustWindowRectEx() does not take the WS_VSCROLL or WS_HSCROLL styles into account.
                requestedSize.Width += SystemInformation.VerticalScrollBarWidth;

                // Ensure we do not shrink smaller than our minimum size
                requestedSize.Width = Math.Max(requestedSize.Width, MinimumSize.Width);

                Rectangle workingArea = Screen.GetWorkingArea(this);
                Point location = PointToScreen(Location);
                // DANGER:  This assumes we will grow to the left.  This is true for propertygrid (DropDownHolder::OnCurrentControlResize)
                int maxDelta = location.X - workingArea.Left;
                // NOTE:  If we are shrinking, requestedWidth will be negative, so the Min will not bound shrinking by maxDelta.  This is intentional.
                int requestedDelta = Math.Min((requestedSize.Width - ClientSize.Width), maxDelta);
                ClientSize = new Size(ClientSize.Width + requestedDelta, MinimumSize.Height);
            }

            private Size ContentSize
            {
                get
                {
                    RECT rect = new RECT();
                    using ScreenDC hdc = ScreenDC.Create();
                    IntPtr hRtbFont = Font.ToHfont();
                    IntPtr hOldFont = Gdi32.SelectObject(hdc, hRtbFont);

                    try
                    {
                        User32.DrawTextW(hdc, Text, Text.Length, ref rect, User32.TextFormatFlags.DT_CALCRECT);
                    }
                    finally
                    {
                        Gdi32.DeleteObject(hRtbFont);
                        Gdi32.SelectObject(hdc, hOldFont);
                    }
                    return new Size(rect.right - rect.left + _caretPadding, rect.bottom - rect.top);
                }
            }

            private bool _contentsResizedRaised = false;

            protected override void OnContentsResized(ContentsResizedEventArgs e)
            {
                _contentsResizedRaised = true;
                ResizeToContent();
                base.OnContentsResized(e);
            }

            protected override void OnTextChanged(EventArgs e)
            {
                // OnContentsResized does not get raised for trailing whitespace.  To work around this, we listen for an OnTextChanged that was not preceeded by an OnContentsResized. Changing the box size here is more expensive, however, so we only want to do it when we have to.
                if (!_contentsResizedRaised)
                {
                    ResizeToContent();
                }
                _contentsResizedRaised = false;
                base.OnTextChanged(e);
            }

            protected override void OnVisibleChanged(EventArgs e)
            {
                if (Visible)
                {
                    ProcessSurrogateFonts(0, Text.Length);
                    Select(Text.Length, 0); // move caret to the end
                }
                ResizeToContent();
                base.OnVisibleChanged(e);
            }

            private Size _minimumSize = Size.Empty;

            public override Size MinimumSize
            {
                get
                {
                    if (_minimumSize == Size.Empty)
                    {
                        Rectangle workingArea = Screen.GetWorkingArea(this);
                        _minimumSize = new Size(
                            (int)Math.Min(Math.Ceiling(WatermarkSize.Width * 1.75), workingArea.Width / 3),
                            (int)Math.Min(Font.Height * 10, workingArea.Height / 3));
                    }
                    return _minimumSize;
                }
            }

            public override Font Font
            {
                get => base.Font;
                set
                {
                    return;
                }
            }

            public void ProcessSurrogateFonts(int start, int length)
            {
                string value = Text;
                if (value == null)
                {
                    return;
                }

                int[] surrogates = StringInfo.ParseCombiningCharacters(value);
                if (surrogates.Length != value.Length)
                {
                    for (int i = 0; i < surrogates.Length; i++)
                    {
                        if (surrogates[i] >= start && surrogates[i] < start + length)
                        { // only process text in the specified area
                            char low = (char)0x0000;
                            if (surrogates[i] + 1 < value.Length)
                            {
                                low = value[surrogates[i] + 1];
                            }
                            if (value[surrogates[i]] >= 0xD800 && value[surrogates[i]] <= 0xDBFF)
                            {
                                if (low >= 0xDC00 && low <= 0xDFFF)
                                {
                                    int planeNumber = (value[surrogates[i]] / 0x40) - (0xD800 / 0x40) + 1; //plane 0 is the default plane
                                    Font replaceFont = _fallbackFonts[planeNumber] as Font;

                                    if (replaceFont == null)
                                    {
                                        using RegistryKey regkey = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\LanguagePack\SurrogateFallback");
                                        if (regkey != null)
                                        {
                                            string fallBackFontName = (string)regkey.GetValue("Plane" + planeNumber);
                                            if (!string.IsNullOrEmpty(fallBackFontName))
                                            {
                                                replaceFont = new Font(fallBackFontName, base.Font.Size, base.Font.Style);
                                            }
                                            _fallbackFonts[planeNumber] = replaceFont;
                                        }
                                    }
                                    if (replaceFont != null)
                                    {
                                        int selectionLength = (i == surrogates.Length - 1) ? value.Length - surrogates[i] : surrogates[i + 1] - surrogates[i];
                                        base.Select(surrogates[i], selectionLength);
                                        SelectionFont = replaceFont;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // Override the Text property from RichTextBox so that we can get the window text from this control without doing a StreamOut operation on the control since StreamOut will cause an IME Composition Window to close unexpectedly.
            public override string Text
            {
                get
                {
                    if (IsHandleCreated)
                    {
                        string windowText = User32.GetWindowText(new HandleRef(this, Handle));
                        if (!_ctrlEnterPressed)
                        {
                            return windowText;
                        }
                        else
                        {
                            int index = windowText.LastIndexOf("\r\n");
                            Debug.Assert(index != -1, "We should have found a Ctrl+Enter in the string");
                            return windowText.Remove(index, 2);
                        }
                    }
                    else
                    {
                        return "";
                    }
                }
                set
                {
                    base.Text = value;
                }
            }

            #region Watermark
            private Size _watermarkSize = Size.Empty;
            private Size WatermarkSize
            {
                get
                {
                    if (_watermarkSize == Size.Empty)
                    {
                        SizeF size;
                        // See how much space we should reserve for watermark
                        using (Graphics g = CreateGraphics())
                        {
                            size = g.MeasureString(SR.MultilineStringEditorWatermark, Font);
                        }
                        _watermarkSize = new Size((int)Math.Ceiling(size.Width), (int)Math.Ceiling(size.Height));
                    }
                    return _watermarkSize;
                }
            }

            private bool ShouldShowWatermark
            {
                get
                {
                    // Do not show watermark if we already have text
                    if (Text.Length != 0)
                    {
                        return false;
                    }
                    return WatermarkSize.Width < ClientSize.Width;
                }
            }

            private Brush WatermarkBrush
            {
                get
                {
                    if (_watermarkBrush == null)
                    {
                        Color cw = SystemColors.Window;
                        Color ct = SystemColors.WindowText;
                        Color c = Color.FromArgb((short)(ct.R * 0.3 + cw.R * 0.7), (short)(ct.G * 0.3 + cw.G * 0.7), (short)(ct.B * 0.3 + cw.B * 0.7));
                        _watermarkBrush = new SolidBrush(c);
                    }
                    return _watermarkBrush;
                }
            }

            protected override void WndProc(ref Message m)
            {
                base.WndProc(ref m);
                switch (m.Msg)
                {
                    case WindowMessages.WM_PAINT:
                        {
                            if (ShouldShowWatermark)
                            {
                                using Graphics g = CreateGraphics();
                                g.DrawString(SR.MultilineStringEditorWatermark, Font, WatermarkBrush, new RectangleF(0.0f, 0.0f, ClientSize.Width, ClientSize.Height), _watermarkFormat);
                            }
                            break;
                        }
                }
            }
            #endregion
        }

        // I used the visual basic 6 RichText (REOleCB.CPP) as a guide for this
        private class OleCallback : Richedit.IRichEditOleCallback
        {
            private readonly RichTextBox _owner;
            readonly bool _unrestricted = false;
            static TraceSwitch _richTextDbg;

            static TraceSwitch RichTextDbg
            {
                get
                {
                    _richTextDbg ??= new TraceSwitch("RichTextDbg", "Debug info about RichTextBox");
                    return _richTextDbg;
                }
            }

            internal OleCallback(RichTextBox owner)
            {
                _owner = owner;
            }

            public HRESULT GetNewStorage(out Ole32.IStorage storage)
            {
                Debug.WriteLineIf(RichTextDbg.TraceVerbose, "IRichTextBoxOleCallback::GetNewStorage");
    
                Ole32.ILockBytes pLockBytes = Ole32.CreateILockBytesOnHGlobal(IntPtr.Zero, true);
                Debug.Assert(pLockBytes != null, "pLockBytes is NULL!");

                storage = Ole32.StgCreateDocfileOnILockBytes(
                    pLockBytes,
                    Ole32.STGM.STGM_SHARE_EXCLUSIVE | Ole32.STGM.STGM_CREATE | Ole32.STGM.STGM_READWRITE,
                    0);
                Debug.Assert(storage != null, "storage is NULL!");

                return HRESULT.S_OK;
            }

            public HRESULT GetInPlaceContext(IntPtr lplpFrame, IntPtr lplpDoc, IntPtr lpFrameInfo)
            {
                Debug.WriteLineIf(RichTextDbg.TraceVerbose, "IRichTextBoxOleCallback::GetInPlaceContext");
                return HRESULT.E_NOTIMPL;
            }

            public HRESULT ShowContainerUI(BOOL fShow)
            {
                Debug.WriteLineIf(RichTextDbg.TraceVerbose, "IRichTextBoxOleCallback::ShowContainerUI");
                return HRESULT.S_OK;
            }

            public HRESULT QueryInsertObject(ref Guid lpclsid, IntPtr lpstg, int cp)
            {
                Debug.WriteLineIf(RichTextDbg.TraceVerbose, "IRichTextBoxOleCallback::QueryInsertObject(" + lpclsid.ToString() + ")");
                if (_unrestricted)
                {
                    return HRESULT.S_OK;
                }
                else
                {
                    Guid realClsid = new Guid();
                    int hr = UnsafeNativeMethods.ReadClassStg(new HandleRef(null, lpstg), ref realClsid);
                    Debug.WriteLineIf(RichTextDbg.TraceVerbose, "real clsid:" + realClsid.ToString() + " (hr=" + hr.ToString("X", CultureInfo.InvariantCulture) + ")");

                    if (!NativeMethods.Succeeded(hr))
                    {
                        return HRESULT.S_FALSE;
                    }

                    if (realClsid == Guid.Empty)
                    {
                        realClsid = lpclsid;
                    }

                    switch (realClsid.ToString().ToUpper(CultureInfo.InvariantCulture))
                    {
                        case "00000315-0000-0000-C000-000000000046": // Metafile
                        case "00000316-0000-0000-C000-000000000046": // DIB
                        case "00000319-0000-0000-C000-000000000046": // EMF
                        case "0003000A-0000-0000-C000-000000000046": //BMP
                            return HRESULT.S_OK;
                        default:
                            Debug.WriteLineIf(RichTextDbg.TraceVerbose, "   denying '" + lpclsid.ToString() + "' from being inserted due to security restrictions");
                            return HRESULT.S_FALSE;
                    }
                }
            }

            public HRESULT DeleteObject(IntPtr lpoleobj)
            {
                Debug.WriteLineIf(RichTextDbg.TraceVerbose, "IRichTextBoxOleCallback::DeleteObject");
                return HRESULT.S_OK;
            }

            public HRESULT QueryAcceptData(IComDataObject lpdataobj, IntPtr lpcfFormat, uint reco, BOOL fReally, IntPtr hMetaPict)
            {
                Debug.WriteLineIf(RichTextDbg.TraceVerbose, "IRichTextBoxOleCallback::QueryAcceptData(reco=" + reco + ")");
                if (reco == NativeMethods.RECO_PASTE)
                {
                    DataObject dataObj = new DataObject(lpdataobj);
                    if (dataObj != null &&
                        (dataObj.GetDataPresent(DataFormats.Text) || dataObj.GetDataPresent(DataFormats.UnicodeText)))
                    {
                        return HRESULT.S_OK;
                    }

                    return HRESULT.E_FAIL;
                }
                
                return HRESULT.E_NOTIMPL;
            }

            public HRESULT ContextSensitiveHelp(BOOL fEnterMode)
            {
                Debug.WriteLineIf(RichTextDbg.TraceVerbose, "IRichTextBoxOleCallback::ContextSensitiveHelp");
                return HRESULT.E_NOTIMPL;
            }

            public HRESULT GetClipboardData(ref Richedit.CHARRANGE lpchrg, uint reco, IntPtr lplpdataobj)
            {
                Debug.WriteLineIf(RichTextDbg.TraceVerbose, "IRichTextBoxOleCallback::GetClipboardData");
                return HRESULT.E_NOTIMPL;
            }

            public HRESULT GetDragDropEffect(BOOL fDrag, int grfKeyState, ref int pdwEffect)
            {
                pdwEffect = (int)DragDropEffects.None;
                return HRESULT.S_OK;
            }

            public HRESULT GetContextMenu(short seltype, IntPtr lpoleobj, ref Richedit.CHARRANGE lpchrg, out IntPtr hmenu)
            {
                TextBox tb = new TextBox
                {
                    Visible = true
                };
                ContextMenu cm = tb.ContextMenu;
                if (cm == null || _owner.ShortcutsEnabled == false)
                {
                    hmenu = IntPtr.Zero;
                }
                else
                {
                    hmenu = cm.Handle;
                }

                return HRESULT.S_OK;
            }
        }
    }
}
