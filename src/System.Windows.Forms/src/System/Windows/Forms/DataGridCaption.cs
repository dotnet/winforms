// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System.Diagnostics;

    using System;
    
    using System.Windows.Forms;
    using System.ComponentModel;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.Runtime.Versioning;
    using Microsoft.Win32;
    using System.Globalization;

    /// <devdoc>
    ///    <para>
    ///       Represents a caption in the DataGrid control.
    ///    </para>
    /// </devdoc>
    internal class DataGridCaption {
    
        internal EventHandlerList events;

        private const int xOffset = 3;
        private const int yOffset = 1;
        private const int textPadding = 2;
        private const int buttonToText = 4;
        private static ColorMap[] colorMap = new ColorMap[] {new ColorMap()};


        private static readonly Point minimumBounds = new Point(50, 30);

        private DataGrid dataGrid = null;
        private bool backButtonVisible = false;
        private bool downButtonVisible = false;

        private SolidBrush backBrush             = DefaultBackBrush;
        private SolidBrush foreBrush         = DefaultForeBrush;
        private Pen        textBorderPen     = DefaultTextBorderPen;

        private string text = "";
        private bool   textBorderVisible = false;
        private Font   textFont = null;

        // use the datagridFont when the textFont is not set
        // we cache this font ( cause we have to make it bold every time we paint the caption )
        //
        private Font   dataGridFont = null;

        private bool backActive = false;
        private bool downActive = false;
        private bool backPressed = false;
        private bool downPressed = false;

        // if the downButton should point down or not
        private bool downButtonDown = false;

        private static Bitmap leftButtonBitmap;
        private static Bitmap leftButtonBitmap_bidi;
        private static Bitmap magnifyingGlassBitmap;

        private Rectangle backButtonRect = new Rectangle();
        private Rectangle downButtonRect = new Rectangle();
        private Rectangle textRect       = new Rectangle();

        private CaptionLocation lastMouseLocation = CaptionLocation.Nowhere;

        private EventEntry eventList;
        private static readonly object EVENT_BACKWARDCLICKED = new object();
        private static readonly object EVENT_DOWNCLICKED = new object();
        private static readonly object EVENT_CAPTIONCLICKED = new object();

        internal DataGridCaption(DataGrid dataGrid) {
            this.dataGrid = dataGrid;
            this.downButtonVisible = dataGrid.ParentRowsVisible;
            colorMap[0].OldColor = Color.White;
            colorMap[0].NewColor = this.ForeColor;
            OnGridFontChanged();
        }

        internal void OnGridFontChanged() {
            if (dataGridFont == null || !dataGridFont.Equals(dataGrid.Font)) {
                try {
                    this.dataGridFont = new Font(dataGrid.Font, FontStyle.Bold);
                }
                catch {
                }
            }
        }

        // =------------------------------------------------------------------
        // =        Properties
        // =------------------------------------------------------------------

        internal bool BackButtonActive {
            get {
                return backActive;
            }
            set {
                if (backActive != value) {
                    backActive = value;
                    InvalidateCaptionRect(backButtonRect);
                }
            }
        }

        internal bool DownButtonActive {
            get {
                return downActive;
            }
            set {
                if (downActive != value) {
                    downActive = value;
                    InvalidateCaptionRect(downButtonRect);
                }
            }
        }

        internal static SolidBrush DefaultBackBrush {
            get {
                return (SolidBrush)SystemBrushes.ActiveCaption;
            }
        }
        internal static Pen DefaultTextBorderPen {
            get {
                return new Pen(SystemColors.ActiveCaptionText);
            }
        }
        internal static SolidBrush DefaultForeBrush {
            get {
                return (SolidBrush)SystemBrushes.ActiveCaptionText;
            }
        }
        internal Color BackColor {
            get {
                return backBrush.Color;
            }
            set {
                if (!backBrush.Color.Equals(value)) {
                    if (value.IsEmpty)
                        throw new ArgumentException(string.Format(SR.DataGridEmptyColor, "Caption BackColor"));
                    backBrush = new SolidBrush(value);
                    Invalidate();
                }
            }
        }
        
        internal EventHandlerList Events {
            get {
                if (events == null) {
                    events = new EventHandlerList();
                }
                return events;
            }
        }


        internal Font Font {
            get {
                // use the dataGridFont only if the user 
                // did not set the CaptionFont
                //
                if (textFont == null)
                    return this.dataGridFont;
                else
                    return textFont;
            }
            set {
                if (textFont == null || !textFont.Equals(value)) {
                    textFont = value;
                    // this property gets called in the constructor before dataGrid has a caption
                    // and we don't need this special-handling then...
                    if (dataGrid.Caption != null) {
                        dataGrid.RecalculateFonts();
                        dataGrid.PerformLayout();
                        dataGrid.Invalidate(); // smaller invalidate rect?
                    }
                }
            }
        }

        internal bool ShouldSerializeFont() {
            return textFont != null && !textFont.Equals(this.dataGridFont);
        }

        internal bool ShouldSerializeBackColor() {
            return !backBrush.Equals(DefaultBackBrush);
        }
        
        internal void ResetBackColor() {
            if (ShouldSerializeBackColor()) {
                backBrush = DefaultBackBrush;
                Invalidate();
            }
        }

        internal void ResetForeColor() {
            if (ShouldSerializeForeColor()) {
                foreBrush = DefaultForeBrush;
                Invalidate();
            }
        }

        internal bool ShouldSerializeForeColor() {
            return !foreBrush.Equals(DefaultForeBrush);
        }

        internal void ResetFont() {
            textFont = null;
            Invalidate();
        }

        internal string Text {
            get {
                return text;
            }
            set {
                if (value == null)
                    text = "";
                else
                    text = value;
                Invalidate();
            }
        }

        internal bool TextBorderVisible {
            get {
                return textBorderVisible;
            }
            set {
                textBorderVisible = value;
                Invalidate();
            }
        }

        internal Color ForeColor {
            get {
                return foreBrush.Color;
            }
            set {
                if (value.IsEmpty)
                    throw new ArgumentException(string.Format(SR.DataGridEmptyColor, "Caption ForeColor"));
                foreBrush = new SolidBrush(value);
                colorMap[0].NewColor = this.ForeColor;
                Invalidate();
            }
        }

        internal Point MinimumBounds {
            get {
                return minimumBounds;
            }
        }

        internal bool BackButtonVisible {
            get {
                return backButtonVisible;
            }
            set {
                if (backButtonVisible != value) {
                    backButtonVisible = value;
                    Invalidate();
                }
            }
        }

        internal bool DownButtonVisible {
            get {
                return downButtonVisible;
            }
            set {
                if (downButtonVisible != value) {
                    downButtonVisible = value;
                    Invalidate();
                }
            }
        }

        // =------------------------------------------------------------------
        // =        Methods
        // =------------------------------------------------------------------

        protected virtual void AddEventHandler(object key, Delegate handler) {
            // Locking 'this' here is ok since this is an internal class.
            lock(this) {
                if (handler == null) return;
                for (EventEntry e = eventList; e != null; e = e.next) {
                    if (e.key == key) {
                        e.handler = Delegate.Combine(e.handler, handler);
                        return;
                    }
                }
                eventList = new EventEntry(eventList, key, handler);
            }
        }

        /// <devdoc>
        ///     Adds a listener for the BackwardClicked event.
        /// </devdoc>
        internal event EventHandler BackwardClicked {
            add {
                Events.AddHandler(EVENT_BACKWARDCLICKED, value);
            }
            remove {
                Events.RemoveHandler(EVENT_BACKWARDCLICKED, value);
            }
        }


        /// <devdoc>
        ///     Adds a listener for the CaptionClicked event.
        /// </devdoc>
        internal event EventHandler CaptionClicked {
            add {
                Events.AddHandler(EVENT_CAPTIONCLICKED, value);
            }
            remove {
                Events.RemoveHandler(EVENT_CAPTIONCLICKED, value);
            }
        }

        internal event EventHandler DownClicked {
            add {
                Events.AddHandler(EVENT_DOWNCLICKED, value);
            }
            remove {
                Events.RemoveHandler(EVENT_DOWNCLICKED, value);
            }
        }

        private void Invalidate() {
            if (dataGrid != null)
                dataGrid.InvalidateCaption();
        }

        private void InvalidateCaptionRect(Rectangle r) {
            if (dataGrid != null)
                dataGrid.InvalidateCaptionRect(r);
        }

        private void InvalidateLocation(CaptionLocation loc) {
            Rectangle r;
            switch (loc) {
                case CaptionLocation.BackButton:
                    r = backButtonRect;
                    r.Inflate(1,1);
                    InvalidateCaptionRect(r);
                    break;
                case CaptionLocation.DownButton:
                    r = downButtonRect;
                    r.Inflate(1,1);
                    InvalidateCaptionRect(r);
                    break;
            }
        }

        protected void OnBackwardClicked(EventArgs e) {
            if (backActive) {
                EventHandler handler = (EventHandler)Events[EVENT_BACKWARDCLICKED];
                if (handler != null) handler(this,e);
            }
        }

        protected void OnCaptionClicked(EventArgs e) {
            EventHandler handler = (EventHandler)Events[EVENT_CAPTIONCLICKED];
            if (handler != null) handler(this,e);
        }

        protected void OnDownClicked(EventArgs e) {
            if (downActive && downButtonVisible) {        
                EventHandler handler = (EventHandler)Events[EVENT_DOWNCLICKED];
                if (handler != null) handler(this,e);
            }
        }

        [ResourceExposure(ResourceScope.Machine)]
        [ResourceConsumption(ResourceScope.Machine)]
        private Bitmap GetBitmap(string bitmapName) {
            Bitmap b = null;
            try {
                b = new Bitmap(typeof(DataGridCaption), bitmapName);
                b.MakeTransparent();
            }
            catch (Exception e) {
                Debug.Fail("Failed to load bitmap: " + bitmapName, e.ToString());
            }
            return b;
        }

        [ResourceExposure(ResourceScope.Machine)]
        [ResourceConsumption(ResourceScope.Machine)]
        private Bitmap GetBackButtonBmp(bool alignRight) {
            if (alignRight) {
                if (leftButtonBitmap_bidi == null)
                    leftButtonBitmap_bidi = GetBitmap("DataGridCaption.backarrow_bidi.bmp");
                return leftButtonBitmap_bidi;
            } else {
                if (leftButtonBitmap == null)
                    leftButtonBitmap = GetBitmap("DataGridCaption.backarrow.bmp");
                return leftButtonBitmap;
            }
        }

        [ResourceExposure(ResourceScope.Machine)]
        [ResourceConsumption(ResourceScope.Machine)]
        private Bitmap GetDetailsBmp() {
            if (magnifyingGlassBitmap == null)
                magnifyingGlassBitmap = GetBitmap("DataGridCaption.Details.bmp");
            return magnifyingGlassBitmap;
        }

        protected virtual Delegate GetEventHandler(object key) {
            // Locking 'this' here is ok since this is an internal class.
            lock(this) {
                for (EventEntry e = eventList; e != null; e = e.next) {
                    if (e.key == key) return e.handler;
                }
                return null;
            }
        }

        internal Rectangle GetBackButtonRect(Rectangle bounds, bool alignRight, int downButtonWidth) {
            Bitmap backButtonBmp = GetBackButtonBmp(false);
            Size backButtonSize;
            lock (backButtonBmp) {
                backButtonSize = backButtonBmp.Size;
            }
            return new Rectangle( bounds.Right - xOffset * 4 - downButtonWidth - backButtonSize.Width,
                                  bounds.Y + yOffset + textPadding,
                                  backButtonSize.Width,
                                  backButtonSize.Height);
        }

        internal int GetDetailsButtonWidth() {
            int width = 0;
            Bitmap detailsBmp = GetDetailsBmp();
            lock (detailsBmp) {
                width = detailsBmp.Size.Width;
            }
            return width;
        }

        internal Rectangle GetDetailsButtonRect(Rectangle bounds, bool alignRight) {
            Size downButtonSize;
            Bitmap detailsBmp = GetDetailsBmp();
            lock (detailsBmp) {
                downButtonSize = detailsBmp.Size;
            }
            int downButtonWidth = downButtonSize.Width;
            return new Rectangle( bounds.Right - xOffset * 2 - downButtonWidth,
                                  bounds.Y + yOffset + textPadding,
                                  downButtonWidth,
                                  downButtonSize.Height);
        }

        /// <devdoc>
        ///      Called by the dataGrid when it needs the caption
        ///      to repaint.
        /// </devdoc>
        internal void Paint(Graphics g, Rectangle bounds, bool alignRight) {
            Size textSize = new Size((int) g.MeasureString(text, this.Font).Width + 2, this.Font.Height + 2);

            downButtonRect = GetDetailsButtonRect(bounds, alignRight);
            int downButtonWidth = GetDetailsButtonWidth();
            backButtonRect = GetBackButtonRect(bounds, alignRight, downButtonWidth);

            int backButtonArea = backButtonVisible ? backButtonRect.Width + xOffset + buttonToText : 0;
            int downButtonArea = downButtonVisible && !dataGrid.ParentRowsIsEmpty() ? downButtonWidth + xOffset + buttonToText : 0;

            int textWidthLeft = bounds.Width - xOffset - backButtonArea - downButtonArea;


            textRect = new Rectangle(
                                    bounds.X,
                                    bounds.Y + yOffset,
                                    Math.Min(textWidthLeft, 2 * textPadding + textSize.Width),
                                    2 * textPadding + textSize.Height);

            // align the caption text box, downButton, and backButton
            // if the RigthToLeft property is set to true
            if (alignRight) {
                textRect.X = bounds.Right - textRect.Width;
                backButtonRect.X = bounds.X + xOffset * 4 + downButtonWidth;
                downButtonRect.X = bounds.X + xOffset * 2;
            }

            Debug.WriteLineIf(CompModSwitches.DGCaptionPaint.TraceVerbose, "text size = " + textSize.ToString());
            Debug.WriteLineIf(CompModSwitches.DGCaptionPaint.TraceVerbose, "downButtonWidth = " + downButtonWidth.ToString(CultureInfo.InvariantCulture));
            Debug.WriteLineIf(CompModSwitches.DGCaptionPaint.TraceVerbose, "textWidthLeft = " + textWidthLeft.ToString(CultureInfo.InvariantCulture));
            Debug.WriteLineIf(CompModSwitches.DGCaptionPaint.TraceVerbose, "backButtonRect " + backButtonRect.ToString());
            Debug.WriteLineIf(CompModSwitches.DGCaptionPaint.TraceVerbose, "textRect " + textRect.ToString());
            Debug.WriteLineIf(CompModSwitches.DGCaptionPaint.TraceVerbose, "downButtonRect " + downButtonRect.ToString());

            // we should use the code that is commented out
            // with today's code, there are pixels on the backButtonRect and the downButtonRect
            // that are getting painted twice
            //
            g.FillRectangle(backBrush, bounds);

            if (backButtonVisible) {
                PaintBackButton(g, backButtonRect, alignRight);
                if (backActive) {
                    if (lastMouseLocation == CaptionLocation.BackButton) {
                        backButtonRect.Inflate(1,1);
                        ControlPaint.DrawBorder3D(g, backButtonRect,
                                                  backPressed ? Border3DStyle.SunkenInner : Border3DStyle.RaisedInner);
                    }
                }
            }
            PaintText(g, textRect, alignRight);

            if (downButtonVisible && !dataGrid.ParentRowsIsEmpty()) {
                PaintDownButton(g, downButtonRect);
                // the rules have changed, yet again.
                // now: if we show the parent rows and the mouse is 
                // not on top of this icon, then let the icon be depressed.
                // if the mouse is pressed over the icon, then show the icon pressed
                // if the mouse is over the icon and not pressed, then show the icon SunkenInner;
                //
                if (lastMouseLocation == CaptionLocation.DownButton)
                {
                    downButtonRect.Inflate(1,1);
                    ControlPaint.DrawBorder3D(g, downButtonRect,
                                              downPressed ? Border3DStyle.SunkenInner : Border3DStyle.RaisedInner);
                }
            }
        }

        private void PaintIcon(Graphics g, Rectangle bounds, Bitmap b) {
            ImageAttributes attr = new ImageAttributes();
            attr.SetRemapTable(colorMap, ColorAdjustType.Bitmap);
            g.DrawImage(b, bounds, 0, 0, bounds.Width, bounds.Height,GraphicsUnit.Pixel, attr);
            attr.Dispose();
        }

        private void PaintBackButton(Graphics g, Rectangle bounds, bool alignRight) {
            Bitmap backButtonBmp = GetBackButtonBmp(alignRight);
            lock (backButtonBmp) {
                PaintIcon(g, bounds, backButtonBmp);
            }
        }

        private void PaintDownButton(Graphics g, Rectangle bounds) {
            Bitmap detailsBmp = GetDetailsBmp();
            lock (detailsBmp) {
                PaintIcon(g, bounds, detailsBmp);
            }
        }

        private void PaintText(Graphics g, Rectangle bounds, bool alignToRight) {
            Rectangle textBounds = bounds;

            if (textBounds.Width <= 0 || textBounds.Height <= 0)
                return;

            if (textBorderVisible) {
                g.DrawRectangle(this.textBorderPen, textBounds.X, textBounds.Y, textBounds.Width - 1, textBounds.Height - 1);
                textBounds.Inflate(-1,-1);
            }

            if (textPadding > 0) {
                Rectangle border = textBounds;
                border.Height = textPadding;
                g.FillRectangle(this.backBrush, border);

                border.Y = textBounds.Bottom - textPadding;
                g.FillRectangle(this.backBrush, border);

                border = new Rectangle(textBounds.X, textBounds.Y + textPadding,
                                       textPadding, textBounds.Height - 2*textPadding);
                g.FillRectangle(this.backBrush, border);

                border.X = textBounds.Right - textPadding;
                g.FillRectangle(this.backBrush, border);
                textBounds.Inflate(-textPadding, -textPadding);
            }

            g.FillRectangle(this.backBrush, textBounds);

            // Brush foreBrush = new SolidBrush(dataGrid.CaptionForeColor);
            StringFormat format = new StringFormat();
            if (alignToRight) {
                format.FormatFlags |= StringFormatFlags.DirectionRightToLeft;
                format.Alignment = StringAlignment.Far;
            }
            g.DrawString(text, this.Font, foreBrush, textBounds, format);
            format.Dispose();
            // foreBrush.Dispose();
        }

        private CaptionLocation FindLocation(int x, int y) {
            if (!backButtonRect.IsEmpty) {
                if (backButtonRect.Contains(x,y))
                    return CaptionLocation.BackButton;
            }
            if (!downButtonRect.IsEmpty) {
                if (downButtonRect.Contains(x,y))
                    return CaptionLocation.DownButton;
            }
            if (!textRect.IsEmpty) {
                if (textRect.Contains(x,y))
                    return CaptionLocation.Text;
            }
            return CaptionLocation.Nowhere;
        }

        private bool DownButtonDown {
            get {
                return downButtonDown;
            }
            set {
                if (downButtonDown != value)
                {
                    downButtonDown = value;
                    InvalidateLocation(CaptionLocation.DownButton);
                }
            }
        }

        internal bool GetDownButtonDirection() {
            return DownButtonDown;
        }

        /// <devdoc>
        ///      Called by the dataGrid when the mouse is pressed
        ///      inside the caption.
        /// </devdoc>
        internal void MouseDown(int x, int y) {
            CaptionLocation loc = FindLocation(x, y);
            switch (loc) {
                case CaptionLocation.BackButton:
                    backPressed = true;
                    InvalidateLocation(loc);
                    break;
                case CaptionLocation.DownButton:
                    downPressed = true;
                    InvalidateLocation(loc);
                    break;
                case CaptionLocation.Text:
                    OnCaptionClicked(EventArgs.Empty);
                    break;
            }
        }

        /// <devdoc>
        ///      Called by the dataGrid when the mouse is released
        ///      inside the caption.
        /// </devdoc>
        internal void MouseUp(int x, int y) {
            CaptionLocation loc = FindLocation(x, y);
            switch (loc) {
                case CaptionLocation.DownButton:
                    if (downPressed == true) {
                        downPressed = false;
                        OnDownClicked(EventArgs.Empty);
                    }
                    break;
                case CaptionLocation.BackButton:
                    if (backPressed == true) {
                        backPressed = false;
                        OnBackwardClicked(EventArgs.Empty);
                    }
                    break;
            }
        }

        /// <devdoc>
        ///      Called by the dataGrid when the mouse leaves
        ///      the caption area.
        /// </devdoc>
        internal void MouseLeft() {
            CaptionLocation oldLoc = lastMouseLocation;
            lastMouseLocation = CaptionLocation.Nowhere;
            InvalidateLocation(oldLoc);
        }

        /// <devdoc>
        ///      Called by the dataGrid when the mouse is
        ///      inside the caption.
        /// </devdoc>
        internal void MouseOver(int x, int y) {
            CaptionLocation newLoc = FindLocation(x, y);

            InvalidateLocation(lastMouseLocation);
            InvalidateLocation(newLoc);
            lastMouseLocation = newLoc;
        }

        protected virtual void RaiseEvent(object key, EventArgs e) {
            Delegate handler = GetEventHandler(key);
            if (handler != null)((EventHandler)handler)(this, e);
        }

        protected virtual void RemoveEventHandler(object key, Delegate handler) {
            // Locking 'this' here is ok since this is an internal class.
            lock(this) {
                if (handler == null) return;
                for (EventEntry e = eventList, prev = null; e != null; prev = e, e = e.next) {
                    if (e.key == key) {
                        e.handler = Delegate.Remove(e.handler, handler);
                        if (e.handler == null) {
                            if (prev == null) {
                                eventList = e.next;
                            }
                            else {
                                prev.next = e.next;
                            }
                        }
                        return;
                    }
                }
            }
        }

        protected virtual void RemoveEventHandlers() {
            eventList = null;
        }

        internal void SetDownButtonDirection(bool pointDown)
        {
            DownButtonDown = pointDown;
        }

        /// <devdoc>
        ///      Toggles the direction the "Down Button" is pointing.
        /// </devdoc>
        internal bool ToggleDownButtonDirection() {
            DownButtonDown = !DownButtonDown;
            return DownButtonDown;
        }
        internal enum CaptionLocation {
            Nowhere,
            BackButton,
            DownButton,
            Text
        }

        private sealed class EventEntry {
            internal EventEntry next;
            internal object key;
            internal Delegate handler;

            internal EventEntry(EventEntry next, object key, Delegate handler) {
                this.next = next;
                this.key = key;
                this.handler = handler;
            }
        }

    }
}

