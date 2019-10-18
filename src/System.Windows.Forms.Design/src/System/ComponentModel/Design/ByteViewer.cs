// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace System.ComponentModel.Design
{
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

        private static readonly Font ADDRESS_FONT = new Font("Microsoft Sans Serif", 8.0f);
        private static readonly Font HEXDUMP_FONT = new Font("Courier New", 8.0f);

        private int SCROLLBAR_HEIGHT;
        private int SCROLLBAR_WIDTH;
        private VScrollBar _scrollBar;
        private TextBox _edit;
        private readonly int _columnCount = DEFAULT_COLUMN_COUNT;
        private int _rowCount = DEFAULT_ROW_COUNT;
        private byte[] _dataBuf;
        private int _startLine;
        private int _displayLinesCount;
        private int _linesCount;
        private DisplayMode _displayMode;
        private DisplayMode _realDisplayMode;

        /// <summary>
        ///  Initializes a new instance of the <see cref='ByteViewer'/> class.
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

        // Stole this code from  XmlSanner       
        private static int AnalizeByteOrderMark(byte[] buffer, int index)
        {
            int c1 = buffer[index + 0] << 8 | buffer[index + 1];
            int c2 = buffer[index + 2] << 8 | buffer[index + 3];
            int c4, c5;

            //Assign an index (label) value for first two bytes
            c4 = GetEncodingIndex(c1);
            //Assign an index (label) value for 3rd and 4th byte            
            c5 = GetEncodingIndex(c2);

            //Bellow table is to identify Encoding type based on
            //first four bytes, those we have converted in index 
            //values for this look up table
            //values on column are first two bytes and
            //values on rows are 3rd and 4th byte 

            int[,] encodings = {
                   //Unknown 0000 feff fffe efbb  3c00 003c 3f00 003f  3c3f 786d  4c6f  a794 
           /*Unknown*/ {1   ,5   ,1   ,1    ,1   ,1   ,1   ,1   ,1    ,1    ,1    ,1    ,1   },
              /*0000*/ {1   ,1   ,1   ,11   ,1   ,10  ,4   ,1   ,1    ,1    ,1    ,1    ,1   },
              /*feff*/ {2   ,9   ,5   ,2    ,2   ,2   ,2   ,2   ,2    ,2    ,2    ,2    ,2   },
              /*fffe*/ {3   ,7   ,3   ,7    ,3   ,3   ,3   ,3   ,3    ,3    ,3    ,3    ,3   },
              /*efbb*/ {14  ,1   ,1   ,1    ,1   ,1   ,1   ,1   ,1    ,1    ,1    ,1    ,1   },
              /*3c00*/ {1   ,6   ,1   ,1    ,1   ,1   ,1   ,3   ,1    ,1    ,1    ,1    ,1   },
              /*003c*/ {1   ,8   ,1   ,1    ,1   ,1   ,1   ,1   ,2    ,1    ,1    ,1    ,1   },
              /*3f00*/ {1   ,1   ,1   ,1    ,1   ,1   ,1   ,1   ,1    ,1    ,1    ,1    ,1   },
              /*003f*/ {1   ,1   ,1   ,1    ,1   ,1   ,1   ,1   ,1    ,1    ,1    ,1    ,1   },
              /*3c3f*/ {1   ,1   ,1   ,1    ,1   ,1   ,1   ,1   ,1    ,1    ,13   ,1    ,1   },
              /*786d*/ {1   ,1   ,1   ,1    ,1   ,1   ,1   ,1   ,1    ,1    ,1    ,1    ,1   },
              /*4c6f*/ {1   ,1   ,1   ,1    ,1   ,1   ,1   ,1   ,1    ,1    ,1    ,1    ,12  },
              /*a794*/ {1   ,1   ,1   ,1    ,1   ,1   ,1   ,1   ,1    ,1    ,1    ,1    ,1   }
            };

            return encodings[c4, c5];
        }

        /// <summary>
        ///  Calculates an index for a cell in the HEX grid
        /// </summary>
        private int CellToIndex(int column, int row)
        {
            return row * _columnCount + column;
        }

        /// <summary>
        ///  Copies the line from main data buffer to a line buffer
        /// </summary>
        private byte[] ComposeLineBuffer(int startLine, int line)
        {
            byte[] lineBuffer;

            int offset = startLine * _columnCount;
            if (offset + (line + 1) * _columnCount > _dataBuf.Length)
            {
                lineBuffer = new byte[_dataBuf.Length % _columnCount];
            }
            else
            {
                lineBuffer = new byte[_columnCount];
            }

            for (int i = 0; i < lineBuffer.Length; i++)
            {
                lineBuffer[i] = _dataBuf[offset + CellToIndex(i, line)];
            }

            return lineBuffer;
        }

        /// <summary>
        ///  Draws an adress part in the HEXDUMP view
        /// </summary>
        private void DrawAddress(Graphics g, int startLine, int line)
        {
            Font font = ADDRESS_FONT;

            string hexString = ((startLine + line) * _columnCount).ToString("X8", CultureInfo.InvariantCulture);

            using Brush foreground = new SolidBrush(ForeColor);
            g.DrawString(hexString, font, foreground, ADDRESS_START_X, LINE_START_Y + line * CELL_HEIGHT);
        }

        /// <summary>
        ///     Draws the client background and frames
        /// </summary>
        /// <internalonly/>
        private void DrawClient(Graphics g)
        {
            using (Brush brush = new SolidBrush(SystemColors.ControlLightLight))
            {
                g.FillRectangle(brush, new Rectangle(HEX_START_X,
                                                     CLIENT_START_Y,
                                                     HEX_WIDTH + HEX_DUMP_GAP + DUMP_WIDTH + HEX_DUMP_GAP,
                                                     _rowCount * CELL_HEIGHT));
            }

            using (Pen pen = new Pen(SystemColors.ControlDark))
            {
                g.DrawRectangle(pen, new Rectangle(HEX_START_X,
                                                   CLIENT_START_Y,
                                                   HEX_WIDTH + HEX_DUMP_GAP + DUMP_WIDTH + HEX_DUMP_GAP - 1,
                                                   _rowCount * CELL_HEIGHT - 1));
                g.DrawLine(pen, DUMP_START_X - HEX_DUMP_GAP,
                                CLIENT_START_Y,
                                DUMP_START_X - HEX_DUMP_GAP,
                                CLIENT_START_Y + _rowCount * CELL_HEIGHT - 1);
            }
        }

        // Char.IsPrintable is going away because it's a mostly meaningless concept.
        // Copied code here to preserve semantics.  -- BrianGru, 10/3/2000
        private static bool CharIsPrintable(char c)
        {
            UnicodeCategory uc = Char.GetUnicodeCategory(c);
            return (!(uc == UnicodeCategory.Control) || (uc == UnicodeCategory.Format) ||
                    (uc == UnicodeCategory.LineSeparator) || (uc == UnicodeCategory.ParagraphSeparator) ||
                    (uc == UnicodeCategory.OtherNotAssigned));
        }

        /// <summary>
        ///  Draws the "DUMP" part in the HEXDUMP view
        /// </summary>
        private void DrawDump(Graphics g, byte[] lineBuffer, int line)
        {
            char c;
            StringBuilder sb = new StringBuilder(lineBuffer.Length);
            for (int i = 0; i < lineBuffer.Length; i++)
            {
                c = Convert.ToChar(lineBuffer[i]);
                if (CharIsPrintable(c))
                {
                    sb.Append(c);
                }
                else
                {
                    sb.Append('.');
                }
            }

            Font font = HEXDUMP_FONT;

            using Brush foreground = new SolidBrush(ForeColor);
            g.DrawString(sb.ToString(), font, foreground, DUMP_START_X, LINE_START_Y + line * CELL_HEIGHT);
        }

        /// <summary>
        ///     Draws the "HEX" part in the HEXDUMP view
        /// </summary>
        /// <internalonly/>
        private void DrawHex(Graphics g, byte[] lineBuffer, int line)
        {
            Font font = HEXDUMP_FONT;

            StringBuilder result = new StringBuilder(lineBuffer.Length * 3 + 1);
            for (int i = 0; i < lineBuffer.Length; i++)
            {
                result.Append(lineBuffer[i].ToString("X2", CultureInfo.InvariantCulture));
                result.Append(" ");
                if (i == _columnCount / 2 - 1)
                {
                    result.Append(" ");  //add one extra in the middle
                }
            }

            using Brush foreground = new SolidBrush(ForeColor);
            g.DrawString(result.ToString(), font, foreground, HEX_START_X + BORDER_GAP, LINE_START_Y + line * CELL_HEIGHT);

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
            for (int i = 0; i < linesCount; i++)
            {
                byte[] lineBuffer = ComposeLineBuffer(startLine, i);
                DrawAddress(g, startLine, i);
                DrawHex(g, lineBuffer, i);
                DrawDump(g, lineBuffer, i);
            }
        }

        /// <summary>
        ///  Establishes the display mode for the control based on the contents of the buffer.
        ///  This is based on the following algorithm:
        ///  * Count number of zeros, prinables and other characters in the half of the dataBuffer
        ///  * Base on the following table establish the mode:
        ///     - 80% Characters or digits -> ANSI
        ///     - 80% Valid Unicode chars -> Unicode
        ///     - All other cases -> HEXDUMP
        ///  Also for the buffer of size [0..5] it returns the HEXDUMP mode
        /// </summary>
        /// <internalonly/>
        private DisplayMode GetAutoDisplayMode()
        {
            int printablesCount = 0;
            int unicodeCount = 0;
            int size;

            if ((_dataBuf == null) || (_dataBuf.Length >= 0 && (_dataBuf.Length < 8)))
            {
                return DisplayMode.Hexdump;
            }

            switch (AnalizeByteOrderMark(_dataBuf, 0))
            {
                case 2:
                    //_Encoding = Encoding.BigEndianUnicode;
                    return DisplayMode.Hexdump;
                case 3:
                    //_Encoding = Encoding.Unicode;
                    return DisplayMode.Unicode;
                case 4:
                case 5:
                    //_Encoding = Ucs4Encoding.UCS4_Bigendian; 
                    return DisplayMode.Hexdump;
                case 6:
                case 7:
                    //_Encoding = Ucs4Encoding.UCS4_Littleendian; 
                    return DisplayMode.Hexdump;
                case 8:
                case 9:
                    //_Encoding = Ucs4Encoding.UCS4_3412;
                    return DisplayMode.Hexdump;
                case 10:
                case 11:
                    //_Encoding = Ucs4Encoding.UCS4_2143;
                    return DisplayMode.Hexdump;
                case 12:
                    //8 ebcdic
                    return DisplayMode.Hexdump;
                case 13: //9                    
                         //_Encoding = new UTF8Encoding(false);
                    return DisplayMode.Ansi;
                case 14:
                    return DisplayMode.Ansi;
                default:
                    //If ByteOrderMark not detected try
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
                        char c = (char)_dataBuf[i]; //OK we do not care for Unicode now
                        if (Char.IsLetterOrDigit(c) || Char.IsWhiteSpace(c))
                        {
                            printablesCount++;
                        }
                    }
                    for (int i = 0; i < size; i += 2)
                    {
                        char[] unicodeChars = new char[1];
                        Encoding.Unicode.GetChars(_dataBuf, i, 2, unicodeChars, 0);
                        if (CharIsPrintable(unicodeChars[0]))
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
        public virtual byte[] GetBytes()
        {
            return _dataBuf;
        }

        /// <summary>
        ///  Gets the display mode for the control.
        /// </summary>
        public virtual DisplayMode GetDisplayMode()
        {
            return _displayMode;
        }

        // Stole this code from  XmlSanner       
        private static int GetEncodingIndex(int c1)
        {
            switch (c1)
            {
                case 0x0000:
                    return 1;
                case 0xfeff:
                    return 2;
                case 0xfffe:
                    return 3;
                case 0xefbb:
                    return 4;
                case 0x3c00:
                    return 5;
                case 0x003c:
                    return 6;
                case 0x3f00:
                    return 7;
                case 0x003f:
                    return 8;
                case 0x3c3f:
                    return 9;
                case 0x786d:
                    return 10;
                case 0x4c6f:
                    return 11;
                case 0xa794:
                    return 12;
                default:
                    return 0; //unknown
            }
        }

        /// <summary>
        ///  Initializes the ansi string variable that will be assigned to the edit box.
        /// </summary>
        private void InitAnsi()
        {
            int size = _dataBuf.Length;
            char[] text = new char[size + 1];
            size = Interop.Kernel32.MultiByteToWideChar(0, 0, _dataBuf, size, text, size);
            text[size] = (char)0;

            for (int i = 0; i < size; i++)
            {
                if (text[i] == '\0')
                {
                    text[i] = (char)0x0B;
                }
            }

            _edit.Text = new string(text);
        }

        /// <summary>
        ///  Initializes the Unicode string varible that will be assigned to the edit box
        /// </summary>
        private void InitUnicode()
        {
            char[] text = new char[_dataBuf.Length / 2 + 1];
            Encoding.Unicode.GetChars(_dataBuf, 0, _dataBuf.Length, text, 0);
            for (int i = 0; i < text.Length; i++)
                if (text[i] == '\0')
                    text[i] = (char)0x0B;

            text[text.Length - 1] = '\0';
            _edit.Text = new string(text);
        }

        /// <summary>
        ///  Initializes the UI components of a control
        /// </summary>
        private void InitUI()
        {
            SCROLLBAR_HEIGHT = SystemInformation.HorizontalScrollBarHeight;
            SCROLLBAR_WIDTH = SystemInformation.VerticalScrollBarWidth;
            // For backwards compat
            Size = new Size(SCROLLBAR_START_X + SCROLLBAR_WIDTH + BORDER_GAP + INSET_GAP,
                                 2 * (BORDER_GAP + INSET_GAP) + _rowCount * (CELL_HEIGHT));

            _scrollBar = new VScrollBar();
            _scrollBar.ValueChanged += new EventHandler(ScrollChanged);
            _scrollBar.TabStop = true;
            _scrollBar.TabIndex = 0;
            _scrollBar.Dock = DockStyle.Right;
            _scrollBar.Visible = false;

            _edit = new TextBox();
            _edit.AutoSize = false;
            _edit.BorderStyle = BorderStyle.None;
            _edit.Multiline = true;
            _edit.ReadOnly = true;
            _edit.ScrollBars = ScrollBars.Both;
            _edit.AcceptsTab = true;
            _edit.AcceptsReturn = true;
            _edit.Dock = DockStyle.Fill;
            _edit.Margin = Padding.Empty;
            _edit.WordWrap = false;
            _edit.Visible = false;

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
                Size = new Size(SCROLLBAR_START_X + SCROLLBAR_WIDTH + BORDER_GAP + INSET_GAP,
                                2 * (BORDER_GAP + INSET_GAP) + _rowCount * (CELL_HEIGHT));
            }

            if (_scrollBar != null)
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
            if (_dataBuf == null)
            {
                return;
            }

            FileStream currentFile = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
            try
            {
                currentFile.Write(_dataBuf, 0, _dataBuf.Length);
                currentFile.Close();
            }
            catch
            {
                currentFile.Close();
                throw;
            }
        }

        /// <summary>
        ///  Scroll event handler.
        /// </summary>
        protected virtual void ScrollChanged(object source, EventArgs e)
        {
            _startLine = _scrollBar.Value;

            Invalidate();
        }

        /// <summary>
        ///  Sets the byte array to be displayed in the viewer.
        /// </summary>
        public virtual void SetBytes(byte[] bytes)
        {
            if (bytes == null)
            {
                throw new ArgumentNullException("bytes");
            }

            if (_dataBuf != null)
            {
                _dataBuf = null;
            }

            _dataBuf = bytes;
            InitState();
            SetDisplayMode(_displayMode);
        }

        /// <summary>
        ///  Sets the current display mode.
        /// </summary>
        public virtual void SetDisplayMode(DisplayMode mode)
        {
            if (!ClientUtils.IsEnumValid(mode, (int)mode, (int)DisplayMode.Hexdump, (int)DisplayMode.Auto))
            {
                throw new InvalidEnumArgumentException("mode", (int)mode, typeof(DisplayMode));
            }

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
                    if (_linesCount > _rowCount)
                    {
                        if (!_scrollBar.Visible)
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
            FileStream currentFile = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None);
            try
            {
                int length = (int)currentFile.Length;
                byte[] buf = new byte[length + 1];
                currentFile.Read(buf, 0, length);
                SetBytes(buf);
                currentFile.Close();
            }
            catch
            {
                currentFile.Close();
                throw;
            }
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
}
