//------------------------------------------------------------------------------
// <copyright file="ByteViewer.cs" company="Microsoft">
//     Copyright (c) Microsoft Corporation.  All rights reserved.
// </copyright>                                                                
//------------------------------------------------------------------------------

namespace System.ComponentModel.Design  {
    using System.Design;
    using System.Text;
    using System.Runtime.Serialization.Formatters;
    using System.Runtime.InteropServices;
    using System.Diagnostics;
    using System;    
    using System.ComponentModel;
    using System.Windows.Forms;
    using System.IO;
    using System.Globalization;
    using System.Drawing;
    using Microsoft.Win32;

    /// <include file='doc\ByteViewer.uex' path='docs/doc[@for="ByteViewer"]/*' />
    /// <devdoc>
    ///    <para>Displays byte arrays in
    ///       HEXDUMP, ANSI and Unicode formats.</para>
    /// </devdoc>
    [
    ToolboxItem(false),
    DesignTimeVisible(false)
    ]
    public class ByteViewer : TableLayoutPanel {
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

        private int SCROLLBAR_HEIGHT;
        private int SCROLLBAR_WIDTH;
        

        private const int HEX_DUMP_GAP = 5;

        private const int ADDRESS_START_X = BORDER_GAP + INSET_GAP;
        private const int CLIENT_START_Y = BORDER_GAP + INSET_GAP;
        private const int LINE_START_Y = CLIENT_START_Y + CELL_HEIGHT / 8;
        private const int HEX_START_X = ADDRESS_START_X + ADDRESS_WIDTH;
        private const int DUMP_START_X = HEX_START_X + HEX_WIDTH + HEX_DUMP_GAP;
        private const int SCROLLBAR_START_X = DUMP_START_X + DUMP_WIDTH + HEX_DUMP_GAP;

        private static readonly Font ADDRESS_FONT = new Font("Microsoft Sans Serif", 8.0f);
        private static readonly Font HEXDUMP_FONT = new Font("Courier New", 8.0f);

        private VScrollBar scrollBar;
        private TextBox edit;

        private int columnCount = DEFAULT_COLUMN_COUNT;
        private int rowCount = DEFAULT_ROW_COUNT;

        private byte[] dataBuf;

        private int startLine;
        private int displayLinesCount;
        private int linesCount;

        private DisplayMode displayMode;
        private DisplayMode realDisplayMode;

        /// <include file='doc\ByteViewer.uex' path='docs/doc[@for="ByteViewer.ByteViewer"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of the <see cref='System.ComponentModel.Design.ByteViewer'/> class.
        ///    </para>
        /// </devdoc>
        public ByteViewer()
        : base() {

            this.SuspendLayout();
            this.CellBorderStyle = TableLayoutPanelCellBorderStyle.Inset;            

            this.ColumnCount = 1;
            this.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));

            this.RowCount = 1;
            this.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            
            InitUI();

            this.ResumeLayout();
            
            displayMode = DisplayMode.Hexdump;
            realDisplayMode = DisplayMode.Hexdump;
            DoubleBuffered = true;
            SetStyle(ControlStyles.ResizeRedraw, true);

        }

        //Stole this code from  XmlSanner       
        private static int AnalizeByteOrderMark(byte[] buffer, int index) {
            int c1 = buffer[index + 0] << 8 | buffer[index + 1];
            int c2 = buffer[index + 2] << 8 | buffer[index + 3];
            int c4,c5;

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

            return encodings[c4,c5];        
        }


        /// <include file='doc\ByteViewer.uex' path='docs/doc[@for="ByteViewer.CellToIndex"]/*' />
        /// <devdoc>
        ///     Calculates an index for a cell in the HEX grid
        /// </devdoc>
        /// <internalonly/>
        private int CellToIndex(int column, int row) {
            return row * columnCount + column;
        }

        /// <include file='doc\ByteViewer.uex' path='docs/doc[@for="ByteViewer.ComposeLineBuffer"]/*' />
        /// <devdoc>
        ///     Copies the line from main data buffer to a line buffer
        /// </devdoc>
        /// <internalonly/>
        private byte[] ComposeLineBuffer(int startLine, int line) {
            byte[] lineBuffer;

            int offset = startLine * columnCount;
            if (offset + (line+1) * columnCount > dataBuf.Length)
                lineBuffer = new byte[dataBuf.Length % columnCount];
            else
                lineBuffer = new byte[columnCount];
            for (int i = 0; i < lineBuffer.Length; i++)
                lineBuffer[i] = dataBuf[offset + CellToIndex(i, line)];
            return lineBuffer;
        }

        /// <include file='doc\ByteViewer.uex' path='docs/doc[@for="ByteViewer.DrawAddress"]/*' />
        /// <devdoc>
        ///     Draws an adress part in the HEXDUMP view
        /// </devdoc>
        /// <internalonly/>
        private void DrawAddress(System.Drawing.Graphics g, int startLine, int line) {
            Font font = ADDRESS_FONT;

            string hexString = ((startLine + line) * columnCount).ToString("X8", CultureInfo.InvariantCulture);

            Brush foreground = new SolidBrush(ForeColor);
            try {
                g.DrawString(hexString, font, foreground,
                             ADDRESS_START_X,LINE_START_Y + line * CELL_HEIGHT);
            }

            finally {
                if (foreground != null) {
                    foreground.Dispose();
                }
            }
        }

        /// <include file='doc\ByteViewer.uex' path='docs/doc[@for="ByteViewer.DrawClient"]/*' />
        /// <devdoc>
        ///     Draws the client background and frames
        /// </devdoc>
        /// <internalonly/>
        private void DrawClient(System.Drawing.Graphics g) {

            using (Brush brush = new SolidBrush(SystemColors.ControlLightLight)) {
                g.FillRectangle(brush, new Rectangle(HEX_START_X,CLIENT_START_Y
                                                     ,HEX_WIDTH +
                                                     HEX_DUMP_GAP + DUMP_WIDTH + HEX_DUMP_GAP,
                                                     rowCount * CELL_HEIGHT));
            }

            using (Pen pen = new Pen(SystemColors.ControlDark)) {
                g.DrawRectangle(pen, new Rectangle(HEX_START_X,CLIENT_START_Y
                                                   ,HEX_WIDTH +
                                                   HEX_DUMP_GAP + DUMP_WIDTH + HEX_DUMP_GAP - 1,
                                                   rowCount * CELL_HEIGHT - 1));
                g.DrawLine(pen, DUMP_START_X - HEX_DUMP_GAP, CLIENT_START_Y,
                           DUMP_START_X - HEX_DUMP_GAP, CLIENT_START_Y + rowCount * CELL_HEIGHT - 1);
            }
        }

        // Char.IsPrintable is going away because it's a mostly meaningless concept.
        // Copied code here to preserve semantics.  -- BrianGru, 10/3/2000
        private static bool CharIsPrintable(char c) {
            UnicodeCategory uc = Char.GetUnicodeCategory(c);
            return (!(uc == UnicodeCategory.Control) || (uc == UnicodeCategory.Format) || 
                    (uc == UnicodeCategory.LineSeparator) || (uc == UnicodeCategory.ParagraphSeparator) ||
                    (uc == UnicodeCategory.OtherNotAssigned));
        }

        /// <include file='doc\ByteViewer.uex' path='docs/doc[@for="ByteViewer.DrawDump"]/*' />
        /// <devdoc>
        ///     Draws the "DUMP" part in the HEXDUMP view
        /// </devdoc>
        /// <internalonly/>
        private void DrawDump(System.Drawing.Graphics g, byte[] lineBuffer, int line) {
            char c;
            StringBuilder sb = new StringBuilder(lineBuffer.Length);
            for (int i = 0; i < lineBuffer.Length; i++) {
                c = Convert.ToChar(lineBuffer[i]);
                if (CharIsPrintable(c))
                    sb.Append(c);
                else
                    sb.Append('.');
            }
            Font font = HEXDUMP_FONT;
            Brush foreground = new SolidBrush(ForeColor);
            try {
                g.DrawString(sb.ToString(), font, foreground, DUMP_START_X,  LINE_START_Y + line * CELL_HEIGHT);
            }

            finally {
                if (foreground != null) {
                    foreground.Dispose();
                }
            }
        }

        /// <include file='doc\ByteViewer.uex' path='docs/doc[@for="ByteViewer.DrawHex"]/*' />
        /// <devdoc>
        ///     Draws the "HEX" part in the HEXDUMP view
        /// </devdoc>
        /// <internalonly/>
        private void DrawHex(System.Drawing.Graphics g, byte[] lineBuffer, int line) {
            Font font = HEXDUMP_FONT;

            StringBuilder result = new StringBuilder(lineBuffer.Length * 3 + 1);
            for (int i = 0; i < lineBuffer.Length; i++) {
                result.Append(lineBuffer[i].ToString("X2", CultureInfo.InvariantCulture));
                result.Append(" ");
                if (i == columnCount/2 - 1)
                    result.Append(" ");  //add one extra in the middle
            }
            Brush foreground = new SolidBrush(ForeColor);
            try {
                g.DrawString(result.ToString(), font, foreground, HEX_START_X + BORDER_GAP, LINE_START_Y + line * CELL_HEIGHT);
            }

            finally {
                if (foreground != null) {
                    foreground.Dispose();
                }
            }

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

        /// <include file='doc\ByteViewer.uex' path='docs/doc[@for="ByteViewer.DrawLines"]/*' />
        /// <devdoc>
        ///     Draws the all lines (a line includes address, hex and dump part)
        ///     for the HEXDUMP view
        /// </devdoc>
        /// <internalonly/>
        private void DrawLines(System.Drawing.Graphics g, int startLine, int linesCount) {
            for (int i = 0; i < linesCount; i++) {
                byte[] lineBuffer = ComposeLineBuffer(startLine,i);
                DrawAddress(g, startLine, i);
                DrawHex(g,lineBuffer,i);
                DrawDump(g,lineBuffer,i);
            }
        }

        /// <include file='doc\ByteViewer.uex' path='docs/doc[@for="ByteViewer.GetAutoDisplayMode"]/*' />
        /// <devdoc>
        ///     Establishes the display mode for the control based on the
        ///     contents of the buffer.
        ///     This is based on the following algorithm:
        ///     Count number of zeros, prinables and other characters in the half of the dataBuffer
        ///     Base on the following table establish the mode:
        ///     80% Characters or digits -> ANSI
        ///     80% Valid Unicode chars -> Unicode
        ///     All other cases -> HEXDUMP
        ///     Also for the buffer of size [0..5] it returns the HEXDUMP mode
        /// </devdoc>
        /// <internalonly/>
        private DisplayMode GetAutoDisplayMode() {
            int printablesCount = 0;
            int unicodeCount = 0;
            int size;

            if ((dataBuf == null) || (dataBuf.Length >= 0 && (dataBuf.Length < 8)))
                return DisplayMode.Hexdump;

            switch(AnalizeByteOrderMark(dataBuf, 0)) {                            
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
                if (dataBuf.Length > 1024)
                    size = 512;
                else
                    size = dataBuf.Length / 2;
                
                for (int i = 0; i < size; i++) {
                    char c = (char) dataBuf[i]; //OK we do not care for Unicode now
                    if (Char.IsLetterOrDigit(c) || Char.IsWhiteSpace(c))
                        printablesCount++;
                }
                for (int i = 0; i < size; i+=2) {            
                    char[] unicodeChars = new char[1];
                    Encoding.Unicode.GetChars(dataBuf, i, 2, unicodeChars, 0);
                    if (CharIsPrintable(unicodeChars[0]))
                        unicodeCount++;
                }
    
                if (unicodeCount * 100 / (size/2) > 80)
                    return DisplayMode.Unicode;
    
                if (printablesCount * 100 / size > 80)
                    return DisplayMode.Ansi;
        
                return DisplayMode.Hexdump;                    
            }            
        }

        /// <include file='doc\ByteViewer.uex' path='docs/doc[@for="ByteViewer.GetBytes"]/*' />
        /// <devdoc>
        ///    <para>Gets the bytes in the buffer.</para>
        /// </devdoc>
        public virtual byte[] GetBytes() {
            return dataBuf;
        }

        /// <include file='doc\ByteViewer.uex' path='docs/doc[@for="ByteViewer.GetDisplayMode"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets the display mode for the control.
        ///    </para>
        /// </devdoc>
        public virtual DisplayMode GetDisplayMode() {
            return displayMode;
        }

        //Stole this code from  XmlSanner       
        private static int GetEncodingIndex(int c1) {
            switch(c1) {
                case 0x0000:    return 1; 
                case 0xfeff:    return 2; 
                case 0xfffe:    return 3; 
                case 0xefbb:    return 4; 
                case 0x3c00:    return 5;                     
                case 0x003c:    return 6; 
                case 0x3f00:    return 7; 
                case 0x003f:    return 8; 
                case 0x3c3f:    return 9; 
                case 0x786d:    return 10;                     
                case 0x4c6f:    return 11; 
                case 0xa794:    return 12; 
                default:        return 0; //unknown
            }
        }
                                    
        /// <include file='doc\ByteViewer.uex' path='docs/doc[@for="ByteViewer.InitAnsi"]/*' />
        /// <devdoc>
        ///     Initializes the ansi string variable that will be assigned to the edit box.
        /// </devdoc>
        /// <internalonly/>
        private void InitAnsi() {                        
            int size = dataBuf.Length;
            char[] text = new char[size + 1];
            size = NativeMethods.MultiByteToWideChar(0, 0, dataBuf, size, text, size);
            text[size] = (char)0;

            for (int i = 0; i < size; i++)
                if (text[i] == '\0') text[i] = (char)0x0B;
                
            edit.Text = new string(text);
        }

        /// <include file='doc\ByteViewer.uex' path='docs/doc[@for="ByteViewer.InitUnicode"]/*' />
        /// <devdoc>
        ///     Initializes the Unicode string varible that will be assigned to the edit box
        /// </devdoc>
        /// <internalonly/>
        private void InitUnicode() {
            char[] text = new char[dataBuf.Length/2+1];                            
            Encoding.Unicode.GetChars(dataBuf, 0, dataBuf.Length, text, 0);                                                                     
            for (int i = 0; i < text.Length; i++)
                if (text[i] == '\0') text[i] = (char)0x0B;
            
            text[text.Length - 1] = '\0';
            edit.Text = new string(text);
        }

        /// <include file='doc\ByteViewer.uex' path='docs/doc[@for="ByteViewer.InitUI"]/*' />
        /// <devdoc>
        ///     Initializes the UI components of a control
        /// </devdoc>
        /// <internalonly/>
        private void InitUI() {

            SCROLLBAR_HEIGHT = SystemInformation.HorizontalScrollBarHeight;  
            SCROLLBAR_WIDTH =  SystemInformation.VerticalScrollBarWidth;
            // For backwards compat
            this.Size = new Size(SCROLLBAR_START_X + SCROLLBAR_WIDTH  + BORDER_GAP + INSET_GAP,
                                 2 * (BORDER_GAP + INSET_GAP) + rowCount * (CELL_HEIGHT));
            

            scrollBar = new VScrollBar();
            scrollBar.ValueChanged += new EventHandler(this.ScrollChanged);
            scrollBar.TabStop = true;
            scrollBar.TabIndex = 0;
            scrollBar.Dock = DockStyle.Right;
            scrollBar.Visible = false;

            edit = new TextBox();
            edit.AutoSize = false;
            edit.BorderStyle = BorderStyle.None;
            edit.Multiline = true;
            edit.ReadOnly = true;
            edit.ScrollBars = ScrollBars.Both;
            edit.AcceptsTab = true;
            edit.AcceptsReturn = true;
            edit.Dock = DockStyle.Fill;
            edit.Margin = Padding.Empty;
            edit.WordWrap = false;
            edit.Visible = false;

            Controls.Add(scrollBar, 0, 0);
            Controls.Add(edit, 0 ,0);
        }

        /// <include file='doc\ByteViewer.uex' path='docs/doc[@for="ByteViewer.InitState"]/*' />
        /// <devdoc>
        ///     Initializes some important variables
        /// </devdoc>
        /// <internalonly/>
        private void InitState() {
            // calculate number of lines required (being careful to count 1 for the last partial line)
            linesCount = (dataBuf.Length + columnCount - 1) / columnCount;
            
            startLine = 0;
            if (linesCount > rowCount) {
                displayLinesCount = rowCount;
                scrollBar.Hide();
                scrollBar.Maximum = linesCount - 1;
                scrollBar.LargeChange = rowCount;
                scrollBar.Show();
                scrollBar.Enabled = true;
            }
            else {
                displayLinesCount = linesCount;
                scrollBar.Hide();
                scrollBar.Maximum = rowCount;
                scrollBar.LargeChange = rowCount;
                scrollBar.Show();
                scrollBar.Enabled = false;
            }
            scrollBar.Select();
            Invalidate();
        }
        
        /// <include file='doc\ByteViewer.uex' path='docs/doc[@for="ByteViewer.OnKeyDown"]/*' />
        /// <devdoc>
        ///     KeyDown handler.
        /// </devdoc>
        /// <internalonly/>
        protected override void OnKeyDown(KeyEventArgs e) {
            scrollBar.Select();
        }

        /// <include file='doc\ByteViewer.uex' path='docs/doc[@for="ByteViewer.OnPaint"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    <para>
        ///       Paint handler
        ///       for the control.
        ///    </para>
        /// </devdoc>
        protected override void OnPaint(PaintEventArgs e) {
            base.OnPaint(e);
            Graphics g = e.Graphics;

            switch (realDisplayMode) {
                case DisplayMode.Hexdump:
                    this.SuspendLayout();
                    edit.Hide();
                    scrollBar.Show();
                    this.ResumeLayout();
                    DrawClient(g);
                    DrawLines(g, startLine, displayLinesCount);
                    break;
                case DisplayMode.Ansi:
                    edit.Invalidate();
                    break;
                case DisplayMode.Unicode:
                    edit.Invalidate();
                    break;
            }
        }

        /// <include file='doc\ByteViewer.uex' path='docs/doc[@for="ByteViewer.OnResize"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    <para>
        ///       Resize handler for the control.
        ///    </para>
        /// </devdoc>

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration", MessageId="0#")]        
        protected override void OnLayout(LayoutEventArgs e) {

            // Must call this first since we might return
            base.OnLayout(e);
           
            int rows = (ClientSize.Height - 2 * (BORDER_GAP + INSET_GAP)) / CELL_HEIGHT;
            if (rows >= 0 && rows != rowCount)
                rowCount = rows;
            else 
                return;

            if (Dock == DockStyle.None) {
                // For backwards compatibility
                Size = new Size(SCROLLBAR_START_X + SCROLLBAR_WIDTH + BORDER_GAP + INSET_GAP, 2 * (BORDER_GAP + INSET_GAP) + rowCount * (CELL_HEIGHT));
            }

            if (scrollBar != null) {
                if (linesCount > rowCount) {
                    scrollBar.Hide();
                    scrollBar.Maximum = linesCount - 1;
                    scrollBar.LargeChange = rowCount;
                    scrollBar.Show();
                    scrollBar.Enabled = true;
                    scrollBar.Select();
                }
                else
                    scrollBar.Enabled = false;
            }
            displayLinesCount = (startLine + rowCount < linesCount) ? rowCount : linesCount - startLine;

        }

        /// <include file='doc\ByteViewer.uex' path='docs/doc[@for="ByteViewer.SaveToFile"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Writes the raw data from the data buffer to a file.
        ///    </para>
        /// </devdoc>
        public virtual void SaveToFile(string path) {
            if (dataBuf != null) {
                FileStream currentFile = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
                try {
                    currentFile.Write(dataBuf,0,dataBuf.Length);
                    currentFile.Close();
                }
                catch {
                    currentFile.Close();
                    throw;
                }
            }
        }

        /// <include file='doc\ByteViewer.uex' path='docs/doc[@for="ByteViewer.ScrollChanged"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    <para>
        ///       Scroll event handler.
        ///    </para>
        /// </devdoc>
        protected virtual void ScrollChanged(object source, EventArgs e) {
            startLine = scrollBar.Value;
            
            Invalidate();
        }


        /// <include file='doc\ByteViewer.uex' path='docs/doc[@for="ByteViewer.SetBytes"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Sets the byte array to be displayed in the viewer.
        ///    </para>
        /// </devdoc>
        public virtual void SetBytes(byte[] bytes) {
            if (bytes == null)
                throw new ArgumentNullException("bytes");
                
            if (dataBuf != null)
                dataBuf = null;
            dataBuf = bytes;
            InitState();
            SetDisplayMode(displayMode);
        }

        /// <include file='doc\ByteViewer.uex' path='docs/doc[@for="ByteViewer.SetDisplayMode"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Sets the current display mode.
        ///    </para>
        /// </devdoc>
        public virtual void SetDisplayMode(DisplayMode mode) {
           if (!ClientUtils.IsEnumValid(mode,(int)mode, (int)DisplayMode.Hexdump, (int)DisplayMode.Auto)) 
                throw new InvalidEnumArgumentException("mode", (int)mode, typeof(DisplayMode));
           
            displayMode = mode;

            realDisplayMode = (mode == DisplayMode.Auto) ? GetAutoDisplayMode() : mode;

            
            switch (realDisplayMode) {
                case DisplayMode.Ansi:
                    InitAnsi();
                    this.SuspendLayout();
                    edit.Show();
                    scrollBar.Hide();
                    this.ResumeLayout();
                    Invalidate();
                    break;
                case DisplayMode.Unicode:
                    InitUnicode();
                    this.SuspendLayout();
                    edit.Show();
                    scrollBar.Hide();
                    this.ResumeLayout();
                    Invalidate();
                    break;
                case DisplayMode.Hexdump:
                    this.SuspendLayout();
                    edit.Hide();
                    if (linesCount > rowCount) {
                        if (!scrollBar.Visible) {
                            scrollBar.Show();
                            this.ResumeLayout();
                            scrollBar.Invalidate();
                            scrollBar.Select();
                        }
                        else {
                            this.ResumeLayout();
                        }
                    }
                    else {
                        this.ResumeLayout();
                    }
                    break;
            }
            
        }

        /// <include file='doc\ByteViewer.uex' path='docs/doc[@for="ByteViewer.SetFile"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Sets the file to be displayed in the viewer.
        ///    </para>
        /// </devdoc>
        public virtual void SetFile(string path) {
            FileStream currentFile = new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.None);
            try {
                int length = (int)currentFile.Length;
                byte[] buf = new byte[length+1];
                currentFile.Read(buf,0,length);
                SetBytes(buf);
                currentFile.Close();
            }
            catch {
                currentFile.Close();
                throw;
            }
        }

        /// <include file='doc\ByteViewer.uex' path='docs/doc[@for="ByteViewer.SetStartLine"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Sets the current line for the HEXDUMP view.
        ///    </para>
        /// </devdoc>
        public virtual void SetStartLine(int line) {
            if (line < 0 || line >= linesCount || line > dataBuf.Length / columnCount)
                startLine = 0;
            else
                startLine = line;
        }
    }
}
