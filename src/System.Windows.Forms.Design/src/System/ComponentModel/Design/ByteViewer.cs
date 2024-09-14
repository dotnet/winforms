// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Globalization;
using System.Text;
using System.Windows.Forms;

namespace System.ComponentModel.Design;

/// <summary>
///  Displays byte arrays in HEXDUMP, ANSI and Unicode formats.
/// </summary>
[ToolboxItem(false)]
[DesignTimeVisible(false)]
public class ByteViewer : TableLayoutPanel
{
    private const int DEFAULT_COLUMN_COUNT = 16;
    private const int DEFAULT_ROW_COUNT = 25;
    private const int COLUMN_COUNT = 16;
    private const int BORDER_GAP = 2;
    private const int INSET_GAP = 3;
    private const int CELL_HEIGHT = 21;
    private const int CELL_WIDTH = 25;
    private const int CHAR_WIDTH = 8;
    private const int ADDRESS_WIDTH = 69;  // this is ceiling(sizeof("DDDDDDDD").width) + 1
    private const int HEX_WIDTH = CELL_WIDTH * COLUMN_COUNT;
    private const int DUMP_WIDTH = CHAR_WIDTH * COLUMN_COUNT;
    private const int HEX_DUMP_GAP = 5;
    private const int ADDRESS_START_X = BORDER_GAP + INSET_GAP;
    private const int CLIENT_START_Y = BORDER_GAP + INSET_GAP;
    private const int LINE_START_Y = CLIENT_START_Y + CELL_HEIGHT / 8;
    private const int HEX_START_X = ADDRESS_START_X + ADDRESS_WIDTH;
    private const int DUMP_START_X = HEX_START_X + HEX_WIDTH + HEX_DUMP_GAP;
    private const int SCROLLBAR_START_X = DUMP_START_X + DUMP_WIDTH + HEX_DUMP_GAP;

    private static readonly Font s_addressFont = new("Microsoft Sans Serif", 8.0f);
    private static readonly Font s_hexDumpFont = new("Courier New", 8.0f);

    private int _scrollbarHeight;
    private int _scrollbarWidth;
    private VScrollBar _scrollBar;
    private TextBox _edit;
    private readonly int _columnCount = DEFAULT_COLUMN_COUNT;
    private int _rowCount = DEFAULT_ROW_COUNT;
    private byte[] _dataBuf = [];
    private int _startLine;
    private int _displayLinesCount;
    private int _linesCount;
    private DisplayMode _displayMode;
    private DisplayMode _realDisplayMode;

    /// <summary>
    ///  Initializes a new instance of the <see cref="ByteViewer"/> class.
    /// </summary>
    public ByteViewer()
        : base()
    {
        SuspendLayout();
        CellBorderStyle = TableLayoutPanelCellBorderStyle.Inset;

        ColumnCount = 1;
        ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

        RowCount = 1;
        RowStyles.Add(new RowStyle(SizeType.Percent, 100F));

        InitUI();

        ResumeLayout();

        _displayMode = DisplayMode.Hexdump;
        _realDisplayMode = DisplayMode.Hexdump;
        DoubleBuffered = true;
        SetStyle(ControlStyles.ResizeRedraw, true);
    }

    // Stole this code from XmlScanner
    private static int AnalyzeByteOrderMark(byte[] buffer, int index)
    {
        int c1 = buffer[index + 0] << 8 | buffer[index + 1];
        int c2 = buffer[index + 2] << 8 | buffer[index + 3];
        int c4, c5;

        // Assign an index (label) value for first two bytes
        c4 = GetEncodingIndex(c1);
        // Assign an index (label) value for 3rd and 4th byte
        c5 = GetEncodingIndex(c2);

        // Bellow table is to identify Encoding type based on
        // first four bytes, those we have converted in index
        // values for this look up table
        // values on column are first two bytes and
        // values on rows are 3rd and 4th byte

#pragma warning disable SA1001 // Commas should be spaced correctly
        int[,] encodings =
        {
               // Unknown 0000 feff fffe efbb  3c00 003c 3f00 003f  3c3f 786d  4c6f  a794
       /*Unknown*/ { 1   ,5   ,1   ,1    ,1   ,1   ,1   ,1   ,1    ,1    ,1    ,1    ,1   },
          /*0000*/ { 1   ,1   ,1   ,11   ,1   ,10  ,4   ,1   ,1    ,1    ,1    ,1    ,1   },
          /*feff*/ { 2   ,9   ,5   ,2    ,2   ,2   ,2   ,2   ,2    ,2    ,2    ,2    ,2   },
          /*fffe*/ { 3   ,7   ,3   ,7    ,3   ,3   ,3   ,3   ,3    ,3    ,3    ,3    ,3   },
          /*efbb*/ { 14  ,1   ,1   ,1    ,1   ,1   ,1   ,1   ,1    ,1    ,1    ,1    ,1   },
          /*3c00*/ { 1   ,6   ,1   ,1    ,1   ,1   ,1   ,3   ,1    ,1    ,1    ,1    ,1   },
          /*003c*/ { 1   ,8   ,1   ,1    ,1   ,1   ,1   ,1   ,2    ,1    ,1    ,1    ,1   },
          /*3f00*/ { 1   ,1   ,1   ,1    ,1   ,1   ,1   ,1   ,1    ,1    ,1    ,1    ,1   },
          /*003f*/ { 1   ,1   ,1   ,1    ,1   ,1   ,1   ,1   ,1    ,1    ,1    ,1    ,1   },
          /*3c3f*/ { 1   ,1   ,1   ,1    ,1   ,1   ,1   ,1   ,1    ,1    ,13   ,1    ,1   },
          /*786d*/ { 1   ,1   ,1   ,1    ,1   ,1   ,1   ,1   ,1    ,1    ,1    ,1    ,1   },
          /*4c6f*/ { 1   ,1   ,1   ,1    ,1   ,1   ,1   ,1   ,1    ,1    ,1    ,1    ,12  },
          /*a794*/ { 1   ,1   ,1   ,1    ,1   ,1   ,1   ,1   ,1    ,1    ,1    ,1    ,1   }
        };
#pragma warning restore SA1001

        return encodings[c4, c5];
    }

    /// <summary>
    ///  Draws an address part in the HEXDUMP view
    /// </summary>
    private void DrawAddress(Graphics g, int startLine, int line)
    {
        Font font = s_addressFont;

        Span<char> hexChars = stackalloc char[8];
        bool success = ((startLine + line) * _columnCount).TryFormat(hexChars, out int charCount, "X8", CultureInfo.InvariantCulture);
        Debug.Assert(success && charCount == 8);

        using SolidBrush foreground = new(ForeColor);
        g.DrawString(hexChars, font, foreground, ADDRESS_START_X, LINE_START_Y + line * CELL_HEIGHT);
    }

    /// <summary>
    ///  Draws the client background and frames
    /// </summary>
    /// <internalonly/>
    private void DrawClient(Graphics g)
    {
        using (Brush brush = new SolidBrush(SystemColors.ControlLightLight))
        {
            g.FillRectangle(
                brush,
                new Rectangle(
                    HEX_START_X,
                    CLIENT_START_Y,
                    HEX_WIDTH + HEX_DUMP_GAP + DUMP_WIDTH + HEX_DUMP_GAP,
                    _rowCount * CELL_HEIGHT));
        }

        using Pen pen = new(SystemColors.ControlDark);
        g.DrawRectangle(
            pen,
            new Rectangle(
                HEX_START_X,
                CLIENT_START_Y,
                HEX_WIDTH + HEX_DUMP_GAP + DUMP_WIDTH + HEX_DUMP_GAP - 1,
                _rowCount * CELL_HEIGHT - 1));

        g.DrawLine(
            pen,
            DUMP_START_X - HEX_DUMP_GAP,
            CLIENT_START_Y,
            DUMP_START_X - HEX_DUMP_GAP,
            CLIENT_START_Y + _rowCount * CELL_HEIGHT - 1);
    }

    // Char.IsPrintable is going away because it's a mostly meaningless concept.
    // Copied code here to preserve semantics. -- BrianGru, 10/3/2000
    private static bool CharIsPrintable(char c)
    {
        UnicodeCategory uc = char.GetUnicodeCategory(c);
        return uc is not UnicodeCategory.Control
            or UnicodeCategory.Format
            or UnicodeCategory.LineSeparator
            or UnicodeCategory.ParagraphSeparator
            or UnicodeCategory.OtherNotAssigned;
    }

    /// <summary>
    ///  Draws the "DUMP" part in the HEXDUMP view
    /// </summary>
    private void DrawDump(Graphics g, ReadOnlySpan<byte> lineBuffer, int line, Span<char> charsBuffer)
    {
        Debug.Assert(charsBuffer.Length >= lineBuffer.Length);
        Span<char> charsToDraw = charsBuffer[..lineBuffer.Length];
        for (int i = 0; i < lineBuffer.Length; i++)
        {
            char c = Convert.ToChar(lineBuffer[i]);
            charsToDraw[i] = CharIsPrintable(c) ? c : '.';
        }

        Font font = s_hexDumpFont;

        using Brush foreground = new SolidBrush(ForeColor);
        g.DrawString(charsToDraw, font, foreground, DUMP_START_X, LINE_START_Y + line * CELL_HEIGHT);
    }

    /// <summary>
    ///  Draws the "HEX" part in the HEXDUMP view
    /// </summary>
    /// <internalonly/>
    private void DrawHex(Graphics g, ReadOnlySpan<byte> lineBuffer, int line, Span<char> charsBuffer)
    {
        Font font = s_hexDumpFont;

        Debug.Assert(charsBuffer.Length >= lineBuffer.Length * 3 + 1);
        int charsWritten = 0;
        for (int i = 0; i < lineBuffer.Length; i++)
        {
            lineBuffer[i].TryFormat(charsBuffer.Slice(charsWritten, 2), out _, "X2");
            charsWritten += 2;

            if (i == _columnCount / 2 - 1)
            {
                charsBuffer[charsWritten] = ' ';  // Add one extra in the middle.
                charsWritten++;
            }
        }

        ReadOnlySpan<char> result = charsBuffer[..charsWritten];

        using Brush foreground = new SolidBrush(ForeColor);
        g.DrawString(result, font, foreground, HEX_START_X + BORDER_GAP, LINE_START_Y + line * CELL_HEIGHT);

        /* ISSUE a-gregka: If perf problem, could be done this way to eliminate drawing twice on repaint
           The current solution good enough for a dialog box
        int hdc = g.getHandle();
        Windows.SelectObject(hdc, HEXDUMP_FONT.getHandle(g));
        Windows.ExtTextOut(hdc, HEX_START_X, LINE_START_Y - 1 + line * CELL_HEIGHT,
             win.ETO_OPAQUE,
             Utils.createRECT(HEX_START_X, LINE_START_Y -1 + line * CELL_HEIGHT, HEX_WIDTH, CELL_HEIGHT),
             out, columnCount * 3 + 1, null);
        */
    }

    /// <summary>
    ///  Draws the all lines (a line includes address, hex and dump part) for the HEXDUMP view
    /// </summary>
    private void DrawLines(Graphics g, int startLine, int linesCount)
    {
        if (linesCount == 0)
        {
            return;
        }

        int maxLength = _columnCount * 3 + 1;
        using BufferScope<char> charsBuffer = new(stackalloc char[256], maxLength);
        for (int i = 0; i < linesCount; i++)
        {
            ReadOnlySpan<byte> lineBuffer = GetLineBytes(startLine + i);
            DrawAddress(g, startLine, i);
            DrawHex(g, lineBuffer, i, charsBuffer);
            DrawDump(g, lineBuffer, i, charsBuffer);
        }

        ReadOnlySpan<byte> GetLineBytes(int line)
        {
            int offset = line * _columnCount;
            int length = offset + _columnCount > _dataBuf.Length ? _dataBuf.Length % _columnCount : _columnCount;

            return _dataBuf.AsSpan(offset, length);
        }
    }

    /// <summary>
    ///  Establishes the display mode for the control based on the contents of the buffer.
    ///  This is based on the following algorithm:
    ///  * Count number of zeros, printables and other characters in the half of the dataBuffer
    ///  * Base on the following table establish the mode:
    ///  - 80% Characters or digits -> ANSI
    ///  - 80% Valid Unicode chars -> Unicode
    ///  - All other cases -> HEXDUMP
    ///  Also for the buffer of size [0..5] it returns the HEXDUMP mode
    /// </summary>
    /// <internalonly/>
    private DisplayMode GetAutoDisplayMode()
    {
        int printablesCount = 0;
        int unicodeCount = 0;
        int size;

        if (_dataBuf.Length is >= 0 and < 8)
        {
            return DisplayMode.Hexdump;
        }

        switch (AnalyzeByteOrderMark(_dataBuf, 0))
        {
            case 2:
                // _Encoding = Encoding.BigEndianUnicode;
                return DisplayMode.Hexdump;
            case 3:
                // _Encoding = Encoding.Unicode;
                return DisplayMode.Unicode;
            case 4:
            case 5:
                // _Encoding = Ucs4Encoding.UCS4_Bigendian;
                return DisplayMode.Hexdump;
            case 6:
            case 7:
                // _Encoding = Ucs4Encoding.UCS4_Littleendian;
                return DisplayMode.Hexdump;
            case 8:
            case 9:
                // _Encoding = Ucs4Encoding.UCS4_3412;
                return DisplayMode.Hexdump;
            case 10:
            case 11:
                // _Encoding = Ucs4Encoding.UCS4_2143;
                return DisplayMode.Hexdump;
            case 12:
                // 8 ebcdic
                return DisplayMode.Hexdump;
            case 13: // 9
                     // _Encoding = new UTF8Encoding(false);
                return DisplayMode.Ansi;
            case 14:
                return DisplayMode.Ansi;
            default:
                // If ByteOrderMark not detected try
                if (_dataBuf.Length > 1024)
                {
                    size = 512;
                }
                else
                {
                    size = _dataBuf.Length / 2;
                }

                for (int i = 0; i < size; i++)
                {
                    char c = (char)_dataBuf[i]; // OK we do not care for Unicode now
                    if (char.IsLetterOrDigit(c) || char.IsWhiteSpace(c))
                    {
                        printablesCount++;
                    }
                }

                for (int i = 0; i < size; i += 2)
                {
                    char unicodeChar = default;
                    Encoding.Unicode.GetChars(_dataBuf.AsSpan(i, 2), new Span<char>(ref unicodeChar));
                    if (CharIsPrintable(unicodeChar))
                    {
                        unicodeCount++;
                    }
                }

                if (unicodeCount * 100 / (size / 2) > 80)
                {
                    return DisplayMode.Unicode;
                }

                if (printablesCount * 100 / size > 80)
                {
                    return DisplayMode.Ansi;
                }

                return DisplayMode.Hexdump;
        }
    }

    /// <summary>
    ///  Gets the bytes in the buffer.
    /// </summary>
    public virtual byte[] GetBytes() => _dataBuf;

    /// <summary>
    ///  Gets the display mode for the control.
    /// </summary>
    public virtual DisplayMode GetDisplayMode() => _displayMode;

    // Stole this code from XmlScanner
    private static int GetEncodingIndex(int c1) => c1 switch
    {
        0x0000 => 1,
        0xfeff => 2,
        0xfffe => 3,
        0xefbb => 4,
        0x3c00 => 5,
        0x003c => 6,
        0x3f00 => 7,
        0x003f => 8,
        0x3c3f => 9,
        0x786d => 10,
        0x4c6f => 11,
        0xa794 => 12,
        _ => 0, // unknown
    };

    /// <summary>
    ///  Initializes the ansi string variable that will be assigned to the edit box.
    /// </summary>
    private unsafe void InitAnsi()
    {
        using BufferScope<char> charsBuffer = new(stackalloc char[256]);
        int size;
        int bufferSize;
        fixed (byte* pDataBuff = _dataBuf)
        {
            bufferSize = PInvoke.MultiByteToWideChar(PInvoke.CP_ACP, 0, (PCSTR)pDataBuff, _dataBuf.Length, null, 0);
            charsBuffer.EnsureCapacity(bufferSize + 1);
            fixed (char* pText = charsBuffer)
            {
                size = PInvoke.MultiByteToWideChar(PInvoke.CP_ACP, 0, (PCSTR)pDataBuff, bufferSize, pText, bufferSize);
            }
        }

        Span<char> text = charsBuffer.Slice(0, bufferSize + 1);

        text[size] = '\0';
        text[..size].Replace('\0', '\v');

        _edit.Text = new string(text);
    }

    /// <summary>
    ///  Initializes the Unicode string variable that will be assigned to the edit box
    /// </summary>
    private void InitUnicode()
    {
        _edit.Text = string.Create(_dataBuf.Length / sizeof(char) + 1, _dataBuf, static (text, dataBuf) =>
        {
            Encoding.Unicode.GetChars(dataBuf.AsSpan(), text);
            text.Replace('\0', '\v');
            text[^1] = '\0';
        });
    }

    /// <summary>
    ///  Initializes the UI components of a control
    /// </summary>
    [MemberNotNull(nameof(_edit))]
    [MemberNotNull(nameof(_scrollBar))]
    private void InitUI()
    {
        _scrollbarHeight = SystemInformation.HorizontalScrollBarHeight;
        _scrollbarWidth = SystemInformation.VerticalScrollBarWidth;

        // For backwards compatibility
        Size = new Size(
            SCROLLBAR_START_X + _scrollbarWidth + BORDER_GAP + INSET_GAP,
            2 * (BORDER_GAP + INSET_GAP) + _rowCount * (CELL_HEIGHT));

        _scrollBar = new VScrollBar();
        _scrollBar.ValueChanged += ScrollChanged;
        _scrollBar.TabStop = true;
        _scrollBar.TabIndex = 0;
        _scrollBar.Dock = DockStyle.Right;
        _scrollBar.Visible = false;

        _edit = new TextBox
        {
            AutoSize = false,
            BorderStyle = BorderStyle.None,
            Multiline = true,
            ReadOnly = true,
            ScrollBars = ScrollBars.Both,
            AcceptsTab = true,
            AcceptsReturn = true,
            Dock = DockStyle.Fill,
            Margin = Padding.Empty,
            WordWrap = false,
            Visible = false
        };

        Controls.Add(_scrollBar, 0, 0);
        Controls.Add(_edit, 0, 0);
    }

    /// <summary>
    ///  Initializes some important variables
    /// </summary>
    private void InitState()
    {
        // calculate number of lines required (being careful to count 1 for the last partial line)
        _linesCount = (_dataBuf.Length + _columnCount - 1) / _columnCount;

        _startLine = 0;
        if (_linesCount > _rowCount)
        {
            _displayLinesCount = _rowCount;
            _scrollBar.Hide();
            _scrollBar.Maximum = _linesCount - 1;
            _scrollBar.LargeChange = _rowCount;
            _scrollBar.Show();
            _scrollBar.Enabled = true;
        }
        else
        {
            _displayLinesCount = _linesCount;
            _scrollBar.Hide();
            _scrollBar.Maximum = _rowCount;
            _scrollBar.LargeChange = _rowCount;
            _scrollBar.Show();
            _scrollBar.Enabled = false;
        }

        _scrollBar.Select();
        Invalidate();
    }

    /// <summary>
    ///  KeyDown handler.
    /// </summary>
    protected override void OnKeyDown(KeyEventArgs e)
    {
        _scrollBar.Select();
    }

    /// <summary>
    ///  Paint handler for the control.
    /// </summary>
    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        Graphics g = e.Graphics;

        switch (_realDisplayMode)
        {
            case DisplayMode.Hexdump:
                SuspendLayout();
                _edit.Hide();
                _scrollBar.Show();
                ResumeLayout();
                DrawClient(g);
                DrawLines(g, _startLine, _displayLinesCount);
                break;

            case DisplayMode.Ansi:
                _edit.Invalidate();
                break;

            case DisplayMode.Unicode:
                _edit.Invalidate();
                break;
        }
    }

    /// <summary>
    ///  Resize handler for the control.
    /// </summary>
    protected override void OnLayout(LayoutEventArgs e)
    {
        // Must call this first since we might return
        base.OnLayout(e);

        int rows = (ClientSize.Height - 2 * (BORDER_GAP + INSET_GAP)) / CELL_HEIGHT;
        if (rows >= 0 && rows != _rowCount)
        {
            _rowCount = rows;
        }
        else
        {
            return;
        }

        if (Dock == DockStyle.None)
        {
            // For backwards compatibility
            Size = new Size(
                SCROLLBAR_START_X + _scrollbarWidth + BORDER_GAP + INSET_GAP,
                2 * (BORDER_GAP + INSET_GAP) + _rowCount * (CELL_HEIGHT));
        }

        if (_scrollBar is not null)
        {
            if (_linesCount > _rowCount)
            {
                _scrollBar.Hide();
                _scrollBar.Maximum = _linesCount - 1;
                _scrollBar.LargeChange = _rowCount;
                _scrollBar.Show();
                _scrollBar.Enabled = true;
                _scrollBar.Select();
            }
            else
            {
                _scrollBar.Enabled = false;
            }
        }

        _displayLinesCount = (_startLine + _rowCount < _linesCount) ? _rowCount : _linesCount - _startLine;
    }

    /// <summary>
    ///  Writes the raw data from the data buffer to a file.
    /// </summary>
    public virtual void SaveToFile(string path)
    {
        using FileStream currentFile = new(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
        currentFile.Write(_dataBuf.AsSpan());
    }

    /// <summary>
    ///  Scroll event handler.
    /// </summary>
    protected virtual void ScrollChanged(object? source, EventArgs e)
    {
        _startLine = _scrollBar.Value;

        Invalidate();
    }

    /// <summary>
    ///  Sets the byte array to be displayed in the viewer.
    /// </summary>
    public virtual void SetBytes(byte[] bytes)
    {
        ArgumentNullException.ThrowIfNull(bytes);

        _dataBuf = bytes;
        InitState();
        SetDisplayMode(_displayMode);
    }

    /// <summary>
    ///  Sets the current display mode.
    /// </summary>
    public virtual void SetDisplayMode(DisplayMode mode)
    {
        SourceGenerated.EnumValidator.Validate(mode, nameof(mode));

        _displayMode = mode;
        _realDisplayMode = (mode == DisplayMode.Auto) ? GetAutoDisplayMode() : mode;

        switch (_realDisplayMode)
        {
            case DisplayMode.Ansi:
                InitAnsi();
                SuspendLayout();
                _edit.Show();
                _scrollBar.Hide();
                ResumeLayout();
                Invalidate();
                break;

            case DisplayMode.Unicode:
                InitUnicode();
                SuspendLayout();
                _edit.Show();
                _scrollBar.Hide();
                ResumeLayout();
                Invalidate();
                break;

            case DisplayMode.Hexdump:
                SuspendLayout();
                _edit.Hide();
                if (_linesCount > _rowCount && !_scrollBar.Visible)
                {
                    _scrollBar.Show();
                    ResumeLayout();
                    _scrollBar.Invalidate();
                    _scrollBar.Select();
                }
                else
                {
                    ResumeLayout();
                }

                break;
        }
    }

    /// <summary>
    ///  Sets the file to be displayed in the viewer.
    /// </summary>
    public virtual void SetFile(string path)
    {
        SetBytes(File.ReadAllBytes(path));
    }

    /// <summary>
    ///  Sets the current line for the HEXDUMP view.
    /// </summary>
    public virtual void SetStartLine(int line)
    {
        if (line < 0 || line >= _linesCount || line > _dataBuf.Length / _columnCount)
        {
            _startLine = 0;
        }
        else
        {
            _startLine = line;
        }
    }
}
