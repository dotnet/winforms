// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using Microsoft.Win32;

namespace System.ComponentModel.Design;

public sealed partial class MultilineStringEditor
{
    private class MultilineStringEditorUI : RichTextBox
    {
        private IWindowsFormsEditorService? _editorService;
        private bool _editing;
        private bool _escapePressed;
        private bool _ctrlEnterPressed;
        private SolidBrush? _watermarkBrush;
        private Size _watermarkSize = Size.Empty;
        private readonly Dictionary<int, Font?> _fallbackFonts;
        private bool _firstTimeResizeToContent = true;

        private readonly StringFormat _watermarkFormat;

        // TextBox needs a little space greater than that actually text content to display the caret.
        private const int CaretPadding = 3;

        internal MultilineStringEditorUI()
        {
            InitializeComponent();
            _watermarkFormat = new StringFormat
            {
                Alignment = StringAlignment.Center,
                LineAlignment = StringAlignment.Center
            };
            _fallbackFonts = new(2);
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
                _watermarkBrush?.Dispose();
                _watermarkBrush = null;
            }

            base.Dispose(disposing);
        }

        protected override object CreateRichEditOleCallback() => new OleCallback(this);

        protected override bool IsInputKey(Keys keyData)
        {
            if ((keyData & Keys.KeyCode) == Keys.Return && Multiline && (keyData & Keys.Alt) == 0)
            {
                return true;
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
            // The RichTextBox does not always invalidate the entire client area unless there is a resize, so if
            // you do a newline before you type enough text to resize the editor the watermark will be
            // ScrollWindowEx'ed down the screen. To prevent this, we do a full Invalidate if the watermark is
            // showing when a key is pressed.
            if (ShouldShowWatermark)
            {
                Invalidate();
            }

            // Ask the editor service to ask my parent to close when the user types "Ctrl+Enter".
            if (e is { Control: true, KeyCode: Keys.Return, Modifiers: Keys.Control })
            {
                _editorService!.CloseDropDown();
                _ctrlEnterPressed = true;
            }
        }

        internal object Value
        {
            get
            {
                Debug.Assert(
                    _editing,
                    "Value is only valid between Begin and EndEdit. (Do not want to keep a reference to a large text buffer.)");
                return Text;
            }
        }

        internal void BeginEdit(IWindowsFormsEditorService editorService, object? value)
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

            // DANGER:  This assumes we will grow to the left. This is true for
            // PropertyGrid (DropDownHolder::OnCurrentControlResize)
            int maxDelta = location.X - workingArea.Left;

            // NOTE:  If we are shrinking, requestedWidth will be negative, so the Min
            // will not bound shrinking by maxDelta. This is intentional.
            int requestedDelta = Math.Min((requestedSize.Width - ClientSize.Width), maxDelta);
            ClientSize = new Size(ClientSize.Width + requestedDelta, MinimumSize.Height);
        }

        private unsafe Size ContentSize
        {
            get
            {
                using var hdc = GetDcScope.ScreenDC;
                using ObjectScope font = new(Font.ToHFONT());
                using SelectObjectScope fontSelection = new(hdc, font);

                RECT rect = default;
                fixed (char* t = Text)
                {
                    PInvoke.DrawText(hdc, t, Text.Length, ref rect, DRAW_TEXT_FORMAT.DT_CALCRECT);
                }

                return new Size(rect.Width + CaretPadding, rect.Height);
            }
        }

        private bool _contentsResizedRaised;

        protected override void OnContentsResized(ContentsResizedEventArgs e)
        {
            _contentsResizedRaised = true;
            ResizeToContent();
            base.OnContentsResized(e);
        }

        protected override void OnTextChanged(EventArgs e)
        {
            // OnContentsResized does not get raised for trailing whitespace. To work around this, we listen for
            // an OnTextChanged that was not preceeded by an OnContentsResized. Changing the box size here is more
            // expensive, however, so we only want to do it when we have to.
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

                // Move caret to the end
                Select(Text.Length, 0);
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
                        Math.Min(Font.Height * 10, workingArea.Height / 3));
                }

                return _minimumSize;
            }
        }

        [AllowNull]
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
            if (value is null)
            {
                return;
            }

            int[] surrogates = StringInfo.ParseCombiningCharacters(value);
            if (surrogates.Length == value.Length)
            {
                return;
            }

            for (int i = 0; i < surrogates.Length; i++)
            {
                int currentSurrogate = surrogates[i];
                if (currentSurrogate < start || currentSurrogate >= start + length)
                {
                    // Only process text in the specified area.
                    continue;
                }

                char low = (char)0x0000;
                if (currentSurrogate + 1 < value.Length)
                {
                    low = value[currentSurrogate + 1];
                }

                if (value[currentSurrogate] < 0xD800 || value[currentSurrogate] > 0xDBFF || low < 0xDC00 || low > 0xDFFF)
                {
                    continue;
                }

                // Plane 0 is the default plane.
                int planeNumber = (value[currentSurrogate] / 0x40) - (0xD800 / 0x40) + 1;
                if (!_fallbackFonts.TryGetValue(planeNumber, out Font? replaceFont))
                {
                    using RegistryKey? regkey = Registry.LocalMachine.OpenSubKey(
                        @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\LanguagePack\SurrogateFallback");

                    if (regkey is not null)
                    {
                        string? fallBackFontName = (string?)regkey.GetValue($"Plane{planeNumber}");
                        if (!string.IsNullOrEmpty(fallBackFontName))
                        {
                            replaceFont = new Font(fallBackFontName, base.Font.Size, base.Font.Style);
                        }

                        _fallbackFonts[planeNumber] = replaceFont;
                    }
                }

                if (replaceFont is not null)
                {
                    int selectionLength = (i == surrogates.Length - 1)
                        ? value.Length - currentSurrogate
                        : surrogates[i + 1] - currentSurrogate;
                    Select(currentSurrogate, selectionLength);
                    SelectionFont = replaceFont;
                }
            }
        }

        [AllowNull]
        public override string Text
        {
            get
            {
                // Override the Text property from RichTextBox so that we can get the window text from this control
                // without doing a StreamOut operation on the control since StreamOut will cause an IME Composition
                // Window to close unexpectedly.

                if (!IsHandleCreated)
                {
                    return string.Empty;
                }

                string windowText = PInvokeCore.GetWindowText(this);
                if (!_ctrlEnterPressed)
                {
                    return windowText;
                }
                else
                {
                    int index = windowText.LastIndexOf("\r\n", StringComparison.Ordinal);
                    Debug.Assert(index != -1, "We should have found a Ctrl+Enter in the string");
                    return windowText.Remove(index, 2);
                }
            }
            set => base.Text = value;
        }

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
                if (_watermarkBrush is null)
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
            switch (m.MsgInternal)
            {
                case PInvokeCore.WM_PAINT:
                    {
                        if (ShouldShowWatermark)
                        {
                            using Graphics g = CreateGraphics();
                            g.DrawString(
                                SR.MultilineStringEditorWatermark,
                                Font,
                                WatermarkBrush,
                                new RectangleF(0.0f, 0.0f, ClientSize.Width, ClientSize.Height),
                                _watermarkFormat);
                        }

                        break;
                    }
            }
        }
    }
}
