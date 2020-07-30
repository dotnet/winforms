// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using System.Windows.Forms.VisualStyles;
using static Interop;

namespace System.ComponentModel.Design
{
    internal sealed class DesignerActionPanel : ContainerControl
    {
        private static readonly object s_eventFormActivated = new object();
        private static readonly object s_eventFormDeactivate = new object();

        private const int EditInputWidth = 150; // The static size of edit controls
        private const int ListBoxMaximumHeight = 200; // The maximum height of a dropdown listbox
        private const int MinimumWidth = 150; // The minimum overall width of the panel
        private const int BottomPadding = 2; // Padding at the bottom of the panel

        private const int LineLeftMargin = 5; // Left padding for all lines
        private const int LineRightMargin = 4; // Right padding for all lines
        private const int LineVerticalPadding = 7; // Vertical padding between lines
        private const int TextBoxTopPadding = 4; // Additional padding for top of textbox lines
        private const int SeparatorHorizontalPadding = 3; // Left and right padding for separator lines
        private const int TextBoxLineCenterMargin = 5; // Padding between the label region and editor region of a textbox line
        private const int TextBoxLineInnerPadding = 1; // Padding within the editor region of a textbox line

        private const int EditorLineSwatchPadding = 1; // Padding for the swatch of an editor line
        private const int EditorLineButtonPadding = 1; // Padding for the button of an editor line
        private const int PanelHeaderVerticalPadding = 3; // Vertical padding within the header of the panel
        private const int PanelHeaderHorizontalPadding = 5; // Horizontal padding within the header of the panel

        private const int TextBoxHeightFixup = 2; // Countereffects the fix for VSWhidbey 359726 - we relied on the broken behavior before
        private CommandID[] _filteredCommandIDs;
        private readonly ToolTip _toolTip;
        private readonly List<Line> _lines;
        private readonly List<int> _lineYPositions;
        private readonly List<int> _lineHeights;

        private readonly Color _gradientLightColor = SystemColors.Control;
        private readonly Color _gradientDarkColor = SystemColors.Control;
        private readonly Color _titleBarColor = SystemColors.ActiveCaption;
        private readonly Color _titleBarUnselectedColor = SystemColors.InactiveCaption;
        private readonly Color _titleBarTextColor = SystemColors.ActiveCaptionText;
        private readonly Color _separatorColor = SystemColors.ControlDark;
        private readonly Color _borderColor = SystemColors.ActiveBorder;
        private readonly Color _linkColor = SystemColors.HotTrack;
        private readonly Color _activeLinkColor = SystemColors.HotTrack;
        private readonly Color _labelForeColor = SystemColors.ControlText;

        private readonly IServiceProvider _serviceProvider;
        private bool _inMethodInvoke;
        private bool _updatingTasks;
        private bool _dropDownActive;

        public DesignerActionPanel(IServiceProvider serviceProvider)
        {
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.Opaque, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, true);
            SetStyle(ControlStyles.UserPaint, true);

            _serviceProvider = serviceProvider;
            _lines = new List<Line>();
            _lineHeights = new List<int>();
            _lineYPositions = new List<int>();
            _toolTip = new ToolTip();
            // Try to get the font from the IUIService, otherwise, use the default
            IUIService uiService = (IUIService)ServiceProvider.GetService(typeof(IUIService));
            if (uiService != null)
            {
                Font = (Font)uiService.Styles["DialogFont"];
                if (uiService.Styles["VsColorPanelGradientDark"] is Color)
                {
                    _gradientDarkColor = (Color)uiService.Styles["VsColorPanelGradientDark"];
                }
                if (uiService.Styles["VsColorPanelGradientLight"] is Color)
                {
                    _gradientLightColor = (Color)uiService.Styles["VsColorPanelGradientLight"];
                }
                if (uiService.Styles["VsColorPanelHyperLink"] is Color)
                {
                    _linkColor = (Color)uiService.Styles["VsColorPanelHyperLink"];
                }
                if (uiService.Styles["VsColorPanelHyperLinkPressed"] is Color)
                {
                    _activeLinkColor = (Color)uiService.Styles["VsColorPanelHyperLinkPressed"];
                }
                if (uiService.Styles["VsColorPanelTitleBar"] is Color)
                {
                    _titleBarColor = (Color)uiService.Styles["VsColorPanelTitleBar"];
                }
                if (uiService.Styles["VsColorPanelTitleBarUnselected"] is Color)
                {
                    _titleBarUnselectedColor = (Color)uiService.Styles["VsColorPanelTitleBarUnselected"];
                }
                if (uiService.Styles["VsColorPanelTitleBarText"] is Color)
                {
                    _titleBarTextColor = (Color)uiService.Styles["VsColorPanelTitleBarText"];
                }
                if (uiService.Styles["VsColorPanelBorder"] is Color)
                {
                    _borderColor = (Color)uiService.Styles["VsColorPanelBorder"];
                }
                if (uiService.Styles["VsColorPanelSeparator"] is Color)
                {
                    _separatorColor = (Color)uiService.Styles["VsColorPanelSeparator"];
                }
                if (uiService.Styles["VsColorPanelText"] is Color)
                {
                    _labelForeColor = (Color)uiService.Styles["VsColorPanelText"];
                }
            }
            MinimumSize = new Size(150, 0);
        }

        public Color ActiveLinkColor
        {
            get => _activeLinkColor;
        }

        public Color BorderColor
        {
            get => _borderColor;
        }

        private bool DropDownActive
        {
            get => _dropDownActive;
        }

        /// <summary>
        ///  Returns the list of commands that should be filtered by the form that hosts this panel. This is done so that these specific commands will not get passed on to VS, and can instead be handled by the panel itself.
        /// </summary>
        public CommandID[] FilteredCommandIDs
        {
            get
            {
                if (_filteredCommandIDs is null)
                {
                    _filteredCommandIDs = new CommandID[] {
                        StandardCommands.Copy,
                        StandardCommands.Cut,
                        StandardCommands.Delete,
                        StandardCommands.F1Help,
                        StandardCommands.Paste,
                        StandardCommands.Redo,
                        StandardCommands.SelectAll,
                        StandardCommands.Undo,
                        MenuCommands.KeyCancel,
                        MenuCommands.KeyReverseCancel,
                        MenuCommands.KeyDefaultAction,
                        MenuCommands.KeyEnd,
                        MenuCommands.KeyHome,
                        MenuCommands.KeyMoveDown,
                        MenuCommands.KeyMoveLeft,
                        MenuCommands.KeyMoveRight,
                        MenuCommands.KeyMoveUp,
                        MenuCommands.KeyNudgeDown,
                        MenuCommands.KeyNudgeHeightDecrease,
                        MenuCommands.KeyNudgeHeightIncrease,
                        MenuCommands.KeyNudgeLeft,
                        MenuCommands.KeyNudgeRight,
                        MenuCommands.KeyNudgeUp,
                        MenuCommands.KeyNudgeWidthDecrease,
                        MenuCommands.KeyNudgeWidthIncrease,
                        MenuCommands.KeySizeHeightDecrease,
                        MenuCommands.KeySizeHeightIncrease,
                        MenuCommands.KeySizeWidthDecrease,
                        MenuCommands.KeySizeWidthIncrease,
                        MenuCommands.KeySelectNext,
                        MenuCommands.KeySelectPrevious,
                        MenuCommands.KeyShiftEnd,
                        MenuCommands.KeyShiftHome,
                    };
                }
                return _filteredCommandIDs;
            }
        }

        /// <summary>
        ///  Gets the Line that currently has input focus.
        /// </summary>
        private Line FocusedLine
        {
            get
            {
                Control activeControl = ActiveControl;
                if (activeControl != null)
                {
                    return activeControl.Tag as Line;
                }
                return null;
            }
        }

        public Color GradientDarkColor
        {
            get => _gradientDarkColor;
        }

        public Color GradientLightColor
        {
            get => _gradientLightColor;
        }

        public bool InMethodInvoke
        {
            get => _inMethodInvoke;
            internal set => _inMethodInvoke = value;
        }

        public Color LinkColor
        {
            get => _linkColor;
        }

        public Color SeparatorColor
        {
            get => _separatorColor;
        }

        private IServiceProvider ServiceProvider
        {
            get => _serviceProvider;
        }

        public Color TitleBarColor
        {
            get => _titleBarColor;
        }

        public Color TitleBarTextColor
        {
            get => _titleBarTextColor;
        }

        public Color TitleBarUnselectedColor
        {
            get => _titleBarUnselectedColor;
        }

        public Color LabelForeColor
        {
            get => _labelForeColor;
        }

        /// <summary>
        ///  Helper event so that Lines can be notified of this event.
        /// </summary>
        private event EventHandler FormActivated
        {
            add => Events.AddHandler(s_eventFormActivated, value);
            remove => Events.RemoveHandler(s_eventFormActivated, value);
        }

        /// <summary>
        ///  Helper event so that Lines can be notified of this event.
        /// </summary>
        private event EventHandler FormDeactivate
        {
            add => Events.AddHandler(s_eventFormDeactivate, value);
            remove => Events.RemoveHandler(s_eventFormDeactivate, value);
        }

        private void AddToCategories(LineInfo lineInfo, ListDictionary categories)
        {
            string categoryName = lineInfo.Item.Category;
            if (categoryName is null)
            {
                categoryName = string.Empty;
            }

            ListDictionary category = (ListDictionary)categories[categoryName];
            if (category is null)
            {
                category = new ListDictionary();
                categories.Add(categoryName, category);
            }

            List<LineInfo> categoryList = (List<LineInfo>)category[lineInfo.List];
            if (categoryList is null)
            {
                categoryList = new List<LineInfo>();
                category.Add(lineInfo.List, categoryList);
            }
            categoryList.Add(lineInfo);
        }

        /// <summary>
        ///  Computes the best possible location (in desktop coordinates) to display the panel, given the size of the panel and the position of its anchor
        /// </summary>
        public static Point ComputePreferredDesktopLocation(Rectangle rectangleAnchor, Size sizePanel, out DockStyle edgeToDock)
        {
            Rectangle rectScreen = Screen.FromPoint(rectangleAnchor.Location).WorkingArea;
            // Determine where we can draw the panel to minimize clipping. Start with the most preferred position, i.e. bottom-right of anchor For the purposes of computing the flags below, assume the anchor to be small enough to ignore its size.
            bool fRightOfAnchor = true;
            bool fAlignToScreenLeft = false;

            // if the panel is too wide, try flipping to left or aligning to screen left
            if (rectangleAnchor.Right + sizePanel.Width > rectScreen.Right)
            { // no room at right, try at left of anchor
                fRightOfAnchor = false;
                if (rectangleAnchor.Left - sizePanel.Width < rectScreen.Left)
                { // no room at left, either
                    fAlignToScreenLeft = true;
                }
            }

            bool fBelowAnchor = (fRightOfAnchor ? true : false);
            bool fAlignToScreenTop = false;
            if (fBelowAnchor)
            {
                // if the panel is too tall, try flipping to top or aligning to screen top
                if (rectangleAnchor.Bottom + sizePanel.Height > rectScreen.Bottom)
                { // no room at bottom, try at top of anchor
                    fBelowAnchor = false;
                    if (rectangleAnchor.Top - sizePanel.Height < rectScreen.Top)
                    { // no room at top, either
                        fAlignToScreenTop = true;
                    }
                }
            }
            else
            {
                // if the panel is too tall, try flipping to bottom or aligning to screen top
                if (rectangleAnchor.Top - sizePanel.Height < rectScreen.Top)
                { // no room at top, try at bottom of anchor
                    fBelowAnchor = true;
                    if (rectangleAnchor.Bottom + sizePanel.Height > rectScreen.Bottom)
                    { // no room at bottom, either
                        fAlignToScreenTop = true;
                    }
                }
            }

            // The flags give us a total of nine possible positions - {LeftOfAnchor, RightOfAnchor, AlignToScreenLeft} X {AboveAnchor, BelowAnchor, AlignToScreenTop}
            // Out of these, we rule out one combination (AlignToScreenLeft, AlignToScreenTop) because this does not guarantee the alignment of an anchor edge with that of the panel edge
            if (fAlignToScreenTop)
            {
                fAlignToScreenLeft = false;
            }

            int x = 0, y = 0;
            const int EDGE_SPACE = 0;
            edgeToDock = DockStyle.None;

            // Compute the actual position now, based on the flags above, and taking the anchor size into account.
            if (fAlignToScreenLeft && fBelowAnchor)
            {
                x = rectScreen.Left;
                y = rectangleAnchor.Bottom + EDGE_SPACE;
                edgeToDock = DockStyle.Bottom;
            }
            else if (fAlignToScreenLeft && !fBelowAnchor)
            {
                x = rectScreen.Left;
                y = rectangleAnchor.Top - sizePanel.Height - EDGE_SPACE;
                edgeToDock = DockStyle.Top;
            }
            else if (fRightOfAnchor && fAlignToScreenTop)
            {
                x = rectangleAnchor.Right + EDGE_SPACE;
                y = rectScreen.Top;
                edgeToDock = DockStyle.Right;
            }
            else if (fRightOfAnchor && fBelowAnchor)
            {
                x = rectangleAnchor.Right + EDGE_SPACE;
                y = rectangleAnchor.Top;
                edgeToDock = DockStyle.Right;
            }
            else if (fRightOfAnchor && !fBelowAnchor)
            {
                x = rectangleAnchor.Right + EDGE_SPACE;
                y = rectangleAnchor.Bottom - sizePanel.Height;
                edgeToDock = DockStyle.Right;
            }
            else if (!fRightOfAnchor && fAlignToScreenTop)
            {
                x = rectangleAnchor.Left - sizePanel.Width - EDGE_SPACE;
                y = rectScreen.Top;
                edgeToDock = DockStyle.Left;
            }
            else if (!fRightOfAnchor && fBelowAnchor)
            {
                x = rectangleAnchor.Left - sizePanel.Width - EDGE_SPACE;
                y = rectangleAnchor.Top;
                edgeToDock = DockStyle.Left;
            }
            else if (!fRightOfAnchor && !fBelowAnchor)
            {
                x = rectangleAnchor.Right - sizePanel.Width;
                y = rectangleAnchor.Top - sizePanel.Height - EDGE_SPACE;
                edgeToDock = DockStyle.Top;
            }
            else
            {
                Debug.Assert(false); // should never get here
            }
            return new Point(x, y);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _toolTip.Dispose();
            }
            base.Dispose(disposing);
        }

        private Size DoLayout(Size proposedSize, bool measureOnly)
        {
            // REVIEW: Is this a WinForms bug? This shouldn't be called if we're disposing since no one should care about layout
            if (Disposing || IsDisposed)
            {
                return Size.Empty;
            }

            int panelWidth = MinimumWidth;
            int yPos = 0;
            SuspendLayout();
            try
            {
                // Clear cached calculated information
                _lineYPositions.Clear();
                _lineHeights.Clear();

                // Layout each line
                for (int i = 0; i < _lines.Count; i++)
                {
                    Line line = _lines[i];
                    _lineYPositions.Add(yPos);
                    Size size = line.LayoutControls(yPos, proposedSize.Width, measureOnly);
                    panelWidth = Math.Max(panelWidth, size.Width);
                    _lineHeights.Add(size.Height);
                    yPos += size.Height;
                }
            }
            finally
            {
                ResumeLayout(!measureOnly);
            }
            return new Size(panelWidth, yPos + BottomPadding);
        }

        public override Size GetPreferredSize(Size proposedSize)
        {
            // REVIEW: WinForms calls this inside of PerformLayout() only in DEBUG code.From the comment it looks like it's calling it to verify their own cached preferred size, so we just ignore this call.
            if (proposedSize.IsEmpty)
            {
                return proposedSize;
            }
            return DoLayout(proposedSize, true);
        }

        private static bool IsReadOnlyProperty(PropertyDescriptor pd)
        {
            if (pd.IsReadOnly)
            {
                return true;
            }
            return (pd.ComponentType.GetProperty(pd.Name).GetSetMethod() is null);
        }

        protected override void OnFontChanged(EventArgs e)
        {
            base.OnFontChanged(e);
            UpdateEditXPos();
            // REVIEW: How do we notify Lines that the font has changed?
        }

        private void OnFormActivated(object sender, EventArgs e)
        {
            ((EventHandler)Events[s_eventFormActivated])?.Invoke(sender, e);
        }

        private void OnFormClosing(object sender, CancelEventArgs e)
        {
            if (!e.Cancel && TopLevelControl != null)
            {
                Debug.Assert(TopLevelControl is Form, "DesignerActionPanel must be hosted on a Form.");
                Form form = (Form)TopLevelControl;
                if (form != null)
                {
                    form.Closing -= new CancelEventHandler(OnFormClosing);
                }
            }
        }

        private void OnFormDeactivate(object sender, EventArgs e)
        {
            ((EventHandler)Events[s_eventFormDeactivate])?.Invoke(sender, e);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            if (TopLevelControl is Form form)
            {
                form.Closing += new CancelEventHandler(OnFormClosing);
            }
        }

        protected override void OnLayout(LayoutEventArgs levent)
        {
            if (_updatingTasks)
            {
                return;
            }

            DoLayout(Size, false);
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (_updatingTasks)
            {
                return;
            }

            Rectangle rect = Bounds;
            if (RightToLeft == RightToLeft.Yes)
            {
                using (LinearGradientBrush gradientBrush = new LinearGradientBrush(rect, GradientDarkColor, GradientLightColor, LinearGradientMode.Horizontal))
                {
                    e.Graphics.FillRectangle(gradientBrush, ClientRectangle);
                }
            }
            else
            {
                using (LinearGradientBrush gradientBrush = new LinearGradientBrush(rect, GradientLightColor, GradientDarkColor, LinearGradientMode.Horizontal))
                {
                    e.Graphics.FillRectangle(gradientBrush, ClientRectangle);
                }
            }

            using (Pen borderPen = new Pen(BorderColor))
            {
                e.Graphics.DrawRectangle(borderPen, new Rectangle(0, 0, Width - 1, Height - 1));
            }

            Rectangle originalClip = e.ClipRectangle;
            // Determine the first line index to paint
            int index = 0;
            while ((index < (_lineYPositions.Count - 1)) && (_lineYPositions[index + 1] <= originalClip.Top))
            {
                index++;
            }

            Graphics g = e.Graphics;
            for (int i = index; i < _lineYPositions.Count; i++)
            {
                Line line = _lines[i];
                int yPos = _lineYPositions[i];
                int lineHeight = _lineHeights[i];
                int lineWidth = Width;
                // Set the clip rectangle so the lines can't mess with each other
                g.SetClip(new Rectangle(0, yPos, lineWidth, lineHeight));

                // Normalize the paint coordinates
                g.TranslateTransform(0, yPos);
                line.PaintLine(g, lineWidth, lineHeight);
                g.ResetTransform();
                // Stop if we've painted all the lines in the clip rectangle
                if (yPos + lineHeight > originalClip.Bottom)
                {
                    break;
                }
            }
        }

        protected override void OnRightToLeftChanged(EventArgs e)
        {
            base.OnRightToLeftChanged(e);
            PerformLayout();
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            // TODO: RightToLeft management for left/right arrow keys (from old DesignerActionPanel)
            Line focusedLine = FocusedLine;
            if (focusedLine != null)
            {
                if (focusedLine.ProcessDialogKey(keyData))
                {
                    return true;
                }
            }
            return base.ProcessDialogKey(keyData);
        }

        // we want to loop
        protected override bool ProcessTabKey(bool forward)
        {
            return (SelectNextControl(ActiveControl, forward, true, true, true));
        }

        private void ProcessLists(DesignerActionListCollection lists, ListDictionary categories)
        {
            if (lists is null)
            {
                return;
            }
            foreach (DesignerActionList list in lists)
            {
                if (list != null)
                {
                    IEnumerable items = list.GetSortedActionItems();
                    if (items != null)
                    {
                        foreach (DesignerActionItem item in items)
                        {
                            if (item is null)
                            {
                                continue;
                            }
                            LineInfo lineInfo = ProcessTaskItem(list, item);
                            if (lineInfo is null)
                            {
                                continue;
                            }
                            AddToCategories(lineInfo, categories);
                            // Process lists from related component
                            IComponent relatedComponent = null;
                            if (item is DesignerActionPropertyItem propItem)
                            {
                                relatedComponent = propItem.RelatedComponent;
                            }
                            else
                            {
                                if (item is DesignerActionMethodItem methodItem)
                                {
                                    relatedComponent = methodItem.RelatedComponent;
                                }
                            }
                            if (relatedComponent != null)
                            {
                                IEnumerable<LineInfo> relatedLineInfos = ProcessRelatedTaskItems(relatedComponent);
                                if (relatedLineInfos != null)
                                {
                                    foreach (LineInfo relatedLineInfo in relatedLineInfos)
                                    {
                                        AddToCategories(relatedLineInfo, categories);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private IEnumerable<LineInfo> ProcessRelatedTaskItems(IComponent relatedComponent)
        {
            // Add the related tasks
            Debug.Assert(relatedComponent != null);
            DesignerActionListCollection relatedLists = null;
            DesignerActionService actionService = (DesignerActionService)ServiceProvider.GetService(typeof(DesignerActionService));
            if (actionService != null)
            {
                relatedLists = actionService.GetComponentActions(relatedComponent);
            }
            else
            {
                // Try to use the component's service provider if it exists so that we end up getting the right IDesignerHost.
                IServiceProvider serviceProvider = relatedComponent.Site;
                if (serviceProvider is null)
                {
                    serviceProvider = ServiceProvider;
                }
                IDesignerHost host = (IDesignerHost)serviceProvider.GetService(typeof(IDesignerHost));
                if (host != null)
                {
                    if (host.GetDesigner(relatedComponent) is ComponentDesigner componentDesigner)
                    {
                        relatedLists = componentDesigner.ActionLists;
                    }
                }
            }

            List<LineInfo> lineInfos = new List<LineInfo>();

            if (relatedLists != null)
            {
                foreach (DesignerActionList relatedList in relatedLists)
                {
                    if (relatedList != null)
                    {
                        IEnumerable items = relatedList.GetSortedActionItems();
                        if (items != null)
                        {
                            foreach (DesignerActionItem relatedItem in items)
                            {
                                if (relatedItem != null)
                                {
                                    if (relatedItem.AllowAssociate)
                                    {
                                        LineInfo lineInfo = ProcessTaskItem(relatedList, relatedItem);
                                        if (lineInfo != null)
                                        {
                                            lineInfos.Add(lineInfo);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return lineInfos;
        }

        private LineInfo ProcessTaskItem(DesignerActionList list, DesignerActionItem item)
        {
            Line newLine;
            if (item is DesignerActionMethodItem)
            {
                newLine = new MethodLine(_serviceProvider, this);
            }
            else if (item is DesignerActionPropertyItem pti)
            {
                PropertyDescriptor pd = TypeDescriptor.GetProperties(list)[pti.MemberName];
                if (pd is null)
                {
                    throw new InvalidOperationException(string.Format(SR.DesignerActionPanel_CouldNotFindProperty, pti.MemberName, list.GetType().FullName));
                }

                TypeDescriptorContext context = new TypeDescriptorContext(_serviceProvider, pd, list);
                UITypeEditor editor = (UITypeEditor)pd.GetEditor(typeof(UITypeEditor));
                bool standardValuesSupported = pd.Converter.GetStandardValuesSupported(context);
                if (editor is null)
                {
                    if (pd.PropertyType == typeof(bool))
                    {
                        if (IsReadOnlyProperty(pd))
                        {
                            newLine = new TextBoxPropertyLine(_serviceProvider, this);
                        }
                        else
                        {
                            newLine = new CheckBoxPropertyLine(_serviceProvider, this);
                        }
                    }
                    else if (standardValuesSupported)
                    {
                        newLine = new EditorPropertyLine(_serviceProvider, this);
                    }
                    else
                    {
                        newLine = new TextBoxPropertyLine(_serviceProvider, this);
                    }
                }
                else
                {
                    newLine = new EditorPropertyLine(_serviceProvider, this);
                }
            }
            else if (item is DesignerActionTextItem)
            {
                if (item is DesignerActionHeaderItem)
                {
                    newLine = new HeaderLine(_serviceProvider, this);
                }
                else
                {
                    newLine = new TextLine(_serviceProvider, this);
                }
            }
            else
            {
                // Ignore unknown items
                return null;
            }
            return new LineInfo(list, item, newLine);
        }

        private void SetDropDownActive(bool active)
        {
            _dropDownActive = active;
        }

        private void ShowError(string errorMessage)
        {
            IUIService uiService = (IUIService)ServiceProvider.GetService(typeof(IUIService));
            if (uiService != null)
            {
                uiService.ShowError(errorMessage);
            }
            else
            {
                MessageBoxOptions options = 0;
                if (SR.RTL != "RTL_False")
                {
                    options = (MessageBoxOptions.RightAlign | MessageBoxOptions.RtlReading);
                }
                MessageBox.Show(this, errorMessage, SR.UIServiceHelper_ErrorCaption, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1, options);
            }
        }

        /// <summary>
        ///  Strips out ampersands used for mnemonics so that they don't show up in the rendering.
        ///  - Convert "&amp;&amp;" to "&amp;"
        ///  - Convert "&amp;x" to "x"
        ///  - An ampersand by itself at the end of a string is displayed as-is
        /// </summary>
        private static string StripAmpersands(string s)
        {
            if (string.IsNullOrEmpty(s))
            {
                return string.Empty;
            }
            StringBuilder result = new StringBuilder(s.Length);
            for (int i = 0; i < s.Length; i++)
            {
                if (s[i] == '&')
                {
                    // Skip over the ampersand
                    i++;
                    if (i == s.Length)
                    {
                        // If we're at the last character just add the ampersand and stop
                        result.Append('&');
                        break;
                    }
                }
                result.Append(s[i]);
            }
            return result.ToString();
        }

        private void UpdateEditXPos()
        {
            // Find the correct edit control position
            int editXPos = 0;
            for (int i = 0; i < _lines.Count; i++)
            {
                if (_lines[i] is TextBoxPropertyLine line)
                {
                    editXPos = Math.Max(editXPos, ((TextBoxPropertyLine)line).GetEditRegionXPos());
                }
            }

            // Make all the edit controls line up
            for (int i = 0; i < _lines.Count; i++)
            {
                if (_lines[i] is TextBoxPropertyLine line)
                {
                    line.SetEditRegionXPos(editXPos);
                }
            }
        }

        public void UpdateTasks(DesignerActionListCollection actionLists, DesignerActionListCollection serviceActionLists, string title, string subtitle)
        {
            _updatingTasks = true;
            SuspendLayout();
            try
            {
                AccessibleName = title;
                AccessibleDescription = subtitle;

                // Store the focus state
                string focusId = string.Empty;
                Line focusedLine = FocusedLine;
                if (focusedLine != null)
                {
                    focusId = focusedLine.FocusId;
                }

                // Merge the categories from the lists and create controls for each of the items
                ListDictionary categories = new ListDictionary();
                ProcessLists(actionLists, categories);
                ProcessLists(serviceActionLists, categories);
                // Create a flat list of lines w/ separators
                List<LineInfo> newLines = new List<LineInfo>
                {
                    // Always add a special line for the header
                    new LineInfo(null, new DesignerActionPanelHeaderItem(title, subtitle), new PanelHeaderLine(_serviceProvider, this))
                };
                int categoriesIndex = 0;
                foreach (ListDictionary category in categories.Values)
                {
                    int categoryIndex = 0;
                    foreach (List<LineInfo> categoryList in category.Values)
                    {
                        for (int i = 0; i < categoryList.Count; i++)
                        {
                            newLines.Add(categoryList[i]);
                        }

                        categoryIndex++;
                        // Add a sub-separator
                        if (categoryIndex < category.Count)
                        {
                            newLines.Add(new LineInfo(null, null, new SeparatorLine(_serviceProvider, this, true)));
                        }
                    }

                    categoriesIndex++;
                    // Add a separator
                    if (categoriesIndex < categories.Count)
                    {
                        newLines.Add(new LineInfo(null, null, new SeparatorLine(_serviceProvider, this)));
                    }
                }

                // Now try to update similar lines
                int currentTabIndex = 0;
                for (int i = 0; i < newLines.Count; i++)
                {
                    LineInfo newLineInfo = newLines[i];
                    Line newLine = newLineInfo.Line;
                    // See if we can update an old line
                    bool updated = false;
                    if (i < _lines.Count)
                    {
                        Line oldLine = _lines[i];

                        if (oldLine.GetType() == newLine.GetType())
                        {
                            oldLine.UpdateActionItem(newLineInfo.List, newLineInfo.Item, _toolTip, ref currentTabIndex);
                            updated = true;
                        }
                        else
                        {
                            oldLine.RemoveControls(Controls);
                            _lines.RemoveAt(i);
                        }
                    }

                    if (!updated)
                    {
                        // Add the new controls
                        List<Control> newControlList = newLine.GetControls();
                        Control[] controls = new Control[newControlList.Count];
                        newControlList.CopyTo(controls);
                        Controls.AddRange(controls);

                        newLine.UpdateActionItem(newLineInfo.List, newLineInfo.Item, _toolTip, ref currentTabIndex);
                        _lines.Insert(i, newLine);
                    }
                }

                // Remove any excess lines
                for (int i = _lines.Count - 1; i >= newLines.Count; i--)
                {
                    Line excessLine = _lines[i];
                    excessLine.RemoveControls(Controls);
                    _lines.RemoveAt(i);
                }

                // Restore focus
                if (!string.IsNullOrEmpty(focusId))
                {
                    foreach (Line line in _lines)
                    {
                        if (string.Equals(line.FocusId, focusId, StringComparison.Ordinal))
                        {
                            line.Focus();
                        }
                    }
                }
            }
            finally
            {
                UpdateEditXPos();
                _updatingTasks = false;
                // REVIEW: We should rely on the caller to actually perform layout since it our scenarios, the entire right pane will have to be layed out
                // Actually, we do want to resume layout since invalidation causes an OnPaint, and OnPaint relies on everything being layed out already
                ResumeLayout(true);
            }
            Invalidate();
        }

        private class LineInfo
        {
            public Line Line;
            public DesignerActionItem Item;
            public DesignerActionList List;

            public LineInfo(DesignerActionList list, DesignerActionItem item, Line line)
            {
                Debug.Assert(line != null);
                Line = line;
                Item = item;
                List = list;
            }
        }

        internal sealed class TypeDescriptorContext : ITypeDescriptorContext
        {
            private readonly IServiceProvider _serviceProvider;
            private readonly PropertyDescriptor _propDesc;
            private readonly object _instance;

            public TypeDescriptorContext(IServiceProvider serviceProvider, PropertyDescriptor propDesc, object instance)
            {
                _serviceProvider = serviceProvider;
                _propDesc = propDesc;
                _instance = instance;
            }

            private IComponentChangeService ComponentChangeService
            {
                get => (IComponentChangeService)_serviceProvider.GetService(typeof(IComponentChangeService));
            }

            public IContainer Container
            {
                get => (IContainer)_serviceProvider.GetService(typeof(IContainer));
            }

            public object Instance
            {
                get => _instance;
            }

            public PropertyDescriptor PropertyDescriptor
            {
                get => _propDesc;
            }

            public object GetService(Type serviceType) => _serviceProvider.GetService(serviceType);

            public bool OnComponentChanging()
            {
                if (ComponentChangeService != null)
                {
                    try
                    {
                        ComponentChangeService.OnComponentChanging(_instance, _propDesc);
                    }
                    catch (CheckoutException ce)
                    {
                        if (ce == CheckoutException.Canceled)
                        {
                            return false;
                        }
                        throw;
                    }
                }
                return true;
            }

            public void OnComponentChanged()
            {
                if (ComponentChangeService != null)
                {
                    ComponentChangeService.OnComponentChanged(_instance, _propDesc, null, null);
                }
            }
        }

        private abstract class Line
        {
            private readonly DesignerActionPanel _actionPanel;
            private List<Control> _addedControls;
            private readonly IServiceProvider _serviceProvider;

            public Line(IServiceProvider serviceProvider, DesignerActionPanel actionPanel)
            {
                _serviceProvider = serviceProvider;
                _actionPanel = actionPanel ?? throw new ArgumentNullException(nameof(actionPanel));
            }

            protected DesignerActionPanel ActionPanel
            {
                get => _actionPanel;
            }

            public abstract string FocusId
            {
                get;
            }

            protected IServiceProvider ServiceProvider
            {
                get => _serviceProvider;
            }

            protected abstract void AddControls(List<Control> controls);

            internal List<Control> GetControls()
            {
                _addedControls = new List<Control>();
                AddControls(_addedControls);
                // Tag all the controls with the Line so we know who owns it
                foreach (Control c in _addedControls)
                {
                    c.Tag = this;
                }
                return _addedControls;
            }

            public abstract void Focus();

            public abstract Size LayoutControls(int top, int width, bool measureOnly);

            public virtual void PaintLine(Graphics g, int lineWidth, int lineHeight)
            {
            }

            protected internal virtual bool ProcessDialogKey(Keys keyData) => false;

            internal void RemoveControls(Control.ControlCollection controls)
            {
                for (int i = 0; i < _addedControls.Count; i++)
                {
                    Control c = _addedControls[i];
                    c.Tag = null;
                    controls.Remove(c);
                }
            }

            internal abstract void UpdateActionItem(DesignerActionList actionList, DesignerActionItem actionItem, ToolTip toolTip, ref int currentTabIndex);
        }

        private sealed class DesignerActionPanelHeaderItem : DesignerActionItem
        {
            private readonly string _subtitle;

            public DesignerActionPanelHeaderItem(string title, string subtitle) : base(title, null, null)
            {
                _subtitle = subtitle;
            }

            public string Subtitle
            {
                get => _subtitle;
            }
        }

        private sealed class PanelHeaderLine : Line
        {
            private DesignerActionList _actionList;
            private DesignerActionPanelHeaderItem _panelHeaderItem;
            private Label _titleLabel;
            private Label _subtitleLabel;
            private bool _formActive;

            public PanelHeaderLine(IServiceProvider serviceProvider, DesignerActionPanel actionPanel) : base(serviceProvider, actionPanel)
            {
            }

            public sealed override string FocusId
            {
                get => string.Empty;
            }

            protected override void AddControls(List<Control> controls)
            {
                _titleLabel = new Label
                {
                    BackColor = Color.Transparent,
                    ForeColor = ActionPanel.TitleBarTextColor,
                    TextAlign = Drawing.ContentAlignment.MiddleLeft,
                    UseMnemonic = false
                };

                _subtitleLabel = new Label
                {
                    BackColor = Color.Transparent,
                    ForeColor = ActionPanel.TitleBarTextColor,
                    TextAlign = Drawing.ContentAlignment.MiddleLeft,
                    UseMnemonic = false
                };

                controls.Add(_titleLabel);
                controls.Add(_subtitleLabel);
            }

            public sealed override void Focus()
            {
                Debug.Fail("Should never try to focus a PanelHeaderLine");
            }

            public override Size LayoutControls(int top, int width, bool measureOnly)
            {
                Size titleSize = _titleLabel.GetPreferredSize(new Size(int.MaxValue, int.MaxValue));
                Size subtitleSize = Size.Empty;
                if (!string.IsNullOrEmpty(_panelHeaderItem.Subtitle))
                {
                    subtitleSize = _subtitleLabel.GetPreferredSize(new Size(int.MaxValue, int.MaxValue));
                }

                if (!measureOnly)
                {
                    _titleLabel.Location = new Point(LineLeftMargin, top + PanelHeaderVerticalPadding);
                    _titleLabel.Size = titleSize;
                    _subtitleLabel.Location = new Point(LineLeftMargin, top + PanelHeaderVerticalPadding * 2 + titleSize.Height);
                    _subtitleLabel.Size = subtitleSize;
                }
                int newWidth = Math.Max(titleSize.Width, subtitleSize.Width) + 2 * PanelHeaderHorizontalPadding;
                int newHeight = (subtitleSize.IsEmpty ? (titleSize.Height + 2 * PanelHeaderVerticalPadding) : (titleSize.Height + subtitleSize.Height + 3 * PanelHeaderVerticalPadding));
                return new Size(newWidth + 2, newHeight + 1);
            }

            private void OnFormActivated(object sender, EventArgs e)
            {
                // TODO: Figure out better rect
                _formActive = true;
                ActionPanel.Invalidate();
                //ActionPanel.Invalidate(new Rectangle(EditRegionLocation, EditRegionSize), false);
            }

            private void OnFormDeactivate(object sender, EventArgs e)
            {
                // TODO: Figure out better rect
                _formActive = false;
                ActionPanel.Invalidate();
            }

            private void OnParentControlFontChanged(object sender, EventArgs e)
            {
                if (_titleLabel != null && _subtitleLabel != null)
                {
                    _titleLabel.Font = new Font(ActionPanel.Font, FontStyle.Bold);
                    _subtitleLabel.Font = ActionPanel.Font;
                }
            }

            public override void PaintLine(Graphics g, int lineWidth, int lineHeight)
            {
                Color backColor = (_formActive || ActionPanel.DropDownActive) ? ActionPanel.TitleBarColor : ActionPanel.TitleBarUnselectedColor;
                using (SolidBrush b = new SolidBrush(backColor))
                {
                    g.FillRectangle(b, 1, 1, lineWidth - 2, lineHeight - 1);
                }

                // Paint a line under the title label
                using (Pen p = new Pen(ActionPanel.BorderColor))
                {
                    g.DrawLine(p, 0, lineHeight - 1, lineWidth, lineHeight - 1);
                }
            }

            internal override void UpdateActionItem(DesignerActionList actionList, DesignerActionItem actionItem, ToolTip toolTip, ref int currentTabIndex)
            {
                _actionList = actionList;
                _panelHeaderItem = (DesignerActionPanelHeaderItem)actionItem;
                _titleLabel.Text = _panelHeaderItem.DisplayName;
                _titleLabel.TabIndex = currentTabIndex++;
                _subtitleLabel.Text = _panelHeaderItem.Subtitle;
                _subtitleLabel.TabIndex = currentTabIndex++;
                _subtitleLabel.Visible = (_subtitleLabel.Text.Length != 0);
                // Force the font to update
                OnParentControlFontChanged(null, EventArgs.Empty);
            }
        }

        private sealed class MethodLine : Line
        {
            private DesignerActionList _actionList;
            private DesignerActionMethodItem _methodItem;
            private MethodItemLinkLabel _linkLabel;
            public MethodLine(IServiceProvider serviceProvider, DesignerActionPanel actionPanel) : base(serviceProvider, actionPanel)
            {
            }

            public sealed override string FocusId
            {
                get => "METHOD:" + _actionList.GetType().FullName + "." + _methodItem.MemberName;
            }

            protected override void AddControls(List<Control> controls)
            {
                _linkLabel = new MethodItemLinkLabel
                {
                    ActiveLinkColor = ActionPanel.ActiveLinkColor,
                    AutoSize = false,
                    BackColor = Color.Transparent,
                    LinkBehavior = LinkBehavior.HoverUnderline,
                    LinkColor = ActionPanel.LinkColor,
                    TextAlign = Drawing.ContentAlignment.MiddleLeft,
                    UseMnemonic = false,
                    VisitedLinkColor = ActionPanel.LinkColor
                };
                _linkLabel.LinkClicked += new LinkLabelLinkClickedEventHandler(OnLinkLabelLinkClicked);
                controls.Add(_linkLabel);
            }

            public sealed override void Focus()
            {
                _linkLabel.Focus();
            }

            public override Size LayoutControls(int top, int width, bool measureOnly)
            {
                Size linkLabelSize = _linkLabel.GetPreferredSize(new Size(int.MaxValue, int.MaxValue));
                if (!measureOnly)
                {
                    _linkLabel.Location = new Point(LineLeftMargin, top + LineVerticalPadding / 2);
                    _linkLabel.Size = linkLabelSize;
                }
                return linkLabelSize + new Size(LineLeftMargin + LineRightMargin, LineVerticalPadding);
            }

            private void OnLinkLabelLinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
            {
                Debug.Assert(!ActionPanel.InMethodInvoke, "Nested method invocation");
                ActionPanel.InMethodInvoke = true;
                try
                {
                    _methodItem.Invoke();
                }
                catch (Exception ex)
                {
                    if (ex is TargetInvocationException)
                    {
                        ex = ex.InnerException;
                    }
                    //NOTE: We had code to rethrow if this was one of [NullReferenceException, StackOverflowException, OutOfMemoryException,
                    //ThreadAbortException].  Removing this rethrow.  StackOverflow and ThreadAbort can't be meaningfully caught, and
                    //NullRef and OutOfMemory really shouldn't be caught.  Out of these, OOM is the most correct one to call, but OOM is
                    //thrown by GDI+ for pretty much any problem, so isn't reliable as an actual indicator that you're out of memory.  If
                    //you really are out of memory, it's very likely you'll get another OOM shortly.
                    ActionPanel.ShowError(string.Format(SR.DesignerActionPanel_ErrorInvokingAction, _methodItem.DisplayName, Environment.NewLine + ex.Message));
                }
                finally
                {
                    ActionPanel.InMethodInvoke = false;
                }
            }

            internal override void UpdateActionItem(DesignerActionList actionList, DesignerActionItem actionItem, ToolTip toolTip, ref int currentTabIndex)
            {
                _actionList = actionList;
                _methodItem = (DesignerActionMethodItem)actionItem;
                toolTip.SetToolTip(_linkLabel, _methodItem.Description);
                _linkLabel.Text = StripAmpersands(_methodItem.DisplayName);
                _linkLabel.AccessibleDescription = actionItem.Description;
                _linkLabel.TabIndex = currentTabIndex++;
            }

            private sealed class MethodItemLinkLabel : LinkLabel
            {
                protected override bool ProcessDialogKey(Keys keyData)
                {
                    if ((keyData & Keys.Control) == Keys.Control)
                    {
                        Keys keyCode = keyData & Keys.KeyCode;
                        switch (keyCode)
                        {
                            case Keys.Tab:
                                // We specifically ignore Ctrl+Tab because it prevents the window switcher dialog from showing up in VS. Normally the key combination is only needed when a LinkLabel contains multiple links, but that can't happen inside the DesignerActionPanel.
                                return false;
                        }
                    }
                    return base.ProcessDialogKey(keyData);
                }
            }
        }

        private abstract class PropertyLine : Line
        {
            private DesignerActionList _actionList;
            private DesignerActionPropertyItem _propertyItem;
            private object _value;
            private bool _pushingValue;
            private PropertyDescriptor _propDesc;
            private ITypeDescriptorContext _typeDescriptorContext;

            public PropertyLine(IServiceProvider serviceProvider, DesignerActionPanel actionPanel) : base(serviceProvider, actionPanel)
            {
            }

            public sealed override string FocusId
            {
                get => "PROPERTY:" + _actionList.GetType().FullName + "." + _propertyItem.MemberName;
            }

            protected PropertyDescriptor PropertyDescriptor
            {
                get
                {
                    if (_propDesc is null)
                    {
                        _propDesc = TypeDescriptor.GetProperties(_actionList)[_propertyItem.MemberName];
                    }
                    return _propDesc;
                }
            }

            protected DesignerActionPropertyItem PropertyItem
            {
                get => _propertyItem;
            }

            protected ITypeDescriptorContext TypeDescriptorContext
            {
                get
                {
                    if (_typeDescriptorContext is null)
                    {
                        _typeDescriptorContext = new TypeDescriptorContext(ServiceProvider, PropertyDescriptor, _actionList);
                    }
                    return _typeDescriptorContext;
                }
            }

            protected object Value
            {
                get => _value;
            }

            protected abstract void OnPropertyTaskItemUpdated(ToolTip toolTip, ref int currentTabIndex);

            protected abstract void OnValueChanged();

            protected void SetValue(object newValue)
            {
                if (_pushingValue || ActionPanel.DropDownActive)
                {
                    return;
                }
                _pushingValue = true;
                try
                {
                    // Only push the change if the values are different
                    if (newValue != null)
                    {
                        Type valueType = newValue.GetType();
                        // If it's not assignable, try to convert it
                        if (!PropertyDescriptor.PropertyType.IsAssignableFrom(valueType))
                        {
                            if (PropertyDescriptor.Converter != null)
                            {
                                // If we can't convert it, show an error
                                if (!PropertyDescriptor.Converter.CanConvertFrom(_typeDescriptorContext, valueType))
                                {
                                    ActionPanel.ShowError(string.Format(SR.DesignerActionPanel_CouldNotConvertValue, newValue, _propDesc.PropertyType));
                                    return;
                                }
                                else
                                {
                                    newValue = PropertyDescriptor.Converter.ConvertFrom(_typeDescriptorContext, CultureInfo.CurrentCulture, newValue);
                                }
                            }
                        }
                    }
                    if (!object.Equals(_value, newValue))
                    {
                        PropertyDescriptor.SetValue(_actionList, newValue);
                        // Update the value we're caching
                        _value = PropertyDescriptor.GetValue(_actionList);
                        OnValueChanged();
                    }
                }
                catch (Exception e)
                {
                    if (e is TargetInvocationException)
                    {
                        e = e.InnerException;
                    }
                    ActionPanel.ShowError(string.Format(SR.DesignerActionPanel_ErrorSettingValue, newValue, PropertyDescriptor.Name, e.Message));
                }
                finally
                {
                    _pushingValue = false;
                }
            }

            internal sealed override void UpdateActionItem(DesignerActionList actionList, DesignerActionItem actionItem, ToolTip toolTip, ref int currentTabIndex)
            {
                _actionList = actionList;
                _propertyItem = (DesignerActionPropertyItem)actionItem;
                _propDesc = null;
                _typeDescriptorContext = null;
                _value = PropertyDescriptor.GetValue(actionList);
                OnPropertyTaskItemUpdated(toolTip, ref currentTabIndex);
                _pushingValue = true;
                try
                {
                    OnValueChanged();
                }
                finally
                {
                    _pushingValue = false;
                }
            }
        }

        private sealed class CheckBoxPropertyLine : PropertyLine
        {
            private CheckBox _checkBox;

            public CheckBoxPropertyLine(IServiceProvider serviceProvider, DesignerActionPanel actionPanel) : base(serviceProvider, actionPanel)
            {
            }

            protected override void AddControls(List<Control> controls)
            {
                _checkBox = new CheckBox
                {
                    BackColor = Color.Transparent,
                    CheckAlign = Drawing.ContentAlignment.MiddleLeft
                };
                _checkBox.TextAlign = Drawing.ContentAlignment.MiddleLeft;
                _checkBox.UseMnemonic = false;
                _checkBox.ForeColor = ActionPanel.LabelForeColor;
                controls.Add(_checkBox);
            }

            public sealed override void Focus() => _checkBox.Focus();

            public override Size LayoutControls(int top, int width, bool measureOnly)
            {
                Size checkBoxPreferredSize = _checkBox.GetPreferredSize(new Size(int.MaxValue, int.MaxValue));
                if (!measureOnly)
                {
                    _checkBox.Location = new Point(LineLeftMargin, top + LineVerticalPadding / 2);
                    _checkBox.Size = checkBoxPreferredSize;
                }
                return checkBoxPreferredSize + new Size(LineLeftMargin + LineRightMargin, LineVerticalPadding);
            }

            private void OnCheckBoxCheckedChanged(object sender, EventArgs e)
            {
                SetValue(_checkBox.Checked);
            }

            protected override void OnPropertyTaskItemUpdated(ToolTip toolTip, ref int currentTabIndex)
            {
                _checkBox.Text = StripAmpersands(PropertyItem.DisplayName);
                _checkBox.AccessibleDescription = PropertyItem.Description;
                _checkBox.TabIndex = currentTabIndex++;

                toolTip.SetToolTip(_checkBox, PropertyItem.Description);
            }

            protected override void OnValueChanged()
            {
                _checkBox.Checked = (bool)Value;
            }
        }

        private class TextBoxPropertyLine : PropertyLine
        {
            private TextBox _textBox;
            private EditorLabel _readOnlyTextBoxLabel;
            private Control _editControl;
            private Label _label;
            private int _editXPos;
            private bool _textBoxDirty;
            private Point _editRegionLocation;
            private Point _editRegionRelativeLocation;
            private Size _editRegionSize;

            public TextBoxPropertyLine(IServiceProvider serviceProvider, DesignerActionPanel actionPanel) : base(serviceProvider, actionPanel)
            {
            }

            protected Control EditControl
            {
                get => _editControl;
            }

            protected Point EditRegionLocation
            {
                get => _editRegionLocation;
            }

            protected Point EditRegionRelativeLocation
            {
                get => _editRegionRelativeLocation;
            }

            protected Size EditRegionSize
            {
                get => _editRegionSize;
            }

            protected override void AddControls(List<Control> controls)
            {
                _label = new Label
                {
                    BackColor = Color.Transparent,
                    ForeColor = ActionPanel.LabelForeColor,
                    TextAlign = Drawing.ContentAlignment.MiddleLeft,
                    UseMnemonic = false
                };
                _readOnlyTextBoxLabel = new EditorLabel
                {
                    BackColor = Color.Transparent,
                    ForeColor = SystemColors.WindowText,
                    TabStop = true,
                    TextAlign = Drawing.ContentAlignment.TopLeft,
                    UseMnemonic = false,
                    Visible = false
                };
                _readOnlyTextBoxLabel.MouseClick += new MouseEventHandler(OnReadOnlyTextBoxLabelClick);
                _readOnlyTextBoxLabel.KeyDown += new KeyEventHandler(OnReadOnlyTextBoxLabelKeyDown);

                _textBox = new TextBox
                {
                    BorderStyle = BorderStyle.None,
                    TextAlign = System.Windows.Forms.HorizontalAlignment.Left,
                    Visible = false
                };
                _textBox.KeyDown += new KeyEventHandler(OnTextBoxKeyDown);

                controls.Add(_readOnlyTextBoxLabel);
                controls.Add(_textBox);
                controls.Add(_label);
            }

            public sealed override void Focus()
            {
                _editControl.Focus();
            }

            internal int GetEditRegionXPos()
            {
                if (string.IsNullOrEmpty(_label.Text))
                {
                    return LineLeftMargin;
                }
                return LineLeftMargin + _label.GetPreferredSize(new Size(int.MaxValue, int.MaxValue)).Width + TextBoxLineCenterMargin;
            }

            protected virtual int GetTextBoxLeftPadding(int textBoxHeight) => TextBoxLineInnerPadding;

            protected virtual int GetTextBoxRightPadding(int textBoxHeight) => TextBoxLineInnerPadding;

            public override Size LayoutControls(int top, int width, bool measureOnly)
            {
                // Figure out our minimum width, Compare to proposed width, If we are smaller, widen the textbox to fit the line based on the bonus
                int textBoxPreferredHeight = _textBox.GetPreferredSize(new Size(int.MaxValue, int.MaxValue)).Height;
                textBoxPreferredHeight += TextBoxHeightFixup;
                int height = textBoxPreferredHeight + LineVerticalPadding + TextBoxLineInnerPadding * 2 + 2; // 2 == border size

                int editRegionXPos = Math.Max(_editXPos, GetEditRegionXPos());
                int minimumWidth = editRegionXPos + EditInputWidth + LineRightMargin;
                width = Math.Max(width, minimumWidth);
                int textBoxWidthBonus = width - minimumWidth;

                if (!measureOnly)
                {
                    _editRegionLocation = new Point(editRegionXPos, top + TextBoxTopPadding);
                    _editRegionRelativeLocation = new Point(editRegionXPos, TextBoxTopPadding);
                    _editRegionSize = new Size(EditInputWidth + textBoxWidthBonus, textBoxPreferredHeight + TextBoxLineInnerPadding * 2);

                    _label.Location = new Point(LineLeftMargin, top);
                    int labelPreferredWidth = _label.GetPreferredSize(new Size(int.MaxValue, int.MaxValue)).Width;
                    _label.Size = new Size(labelPreferredWidth, height);
                    int specialPadding = 0;
                    if (_editControl is TextBox)
                    {
                        specialPadding = 2;
                    }
                    _editControl.Location = new Point(_editRegionLocation.X + GetTextBoxLeftPadding(textBoxPreferredHeight) + 1 + specialPadding, _editRegionLocation.Y + TextBoxLineInnerPadding + 1);
                    _editControl.Width = _editRegionSize.Width - GetTextBoxRightPadding(textBoxPreferredHeight) - GetTextBoxLeftPadding(textBoxPreferredHeight) - specialPadding;
                    _editControl.Height = _editRegionSize.Height - TextBoxLineInnerPadding * 2 - 1;
                }
                return new Size(width, height);
            }

            protected virtual bool IsReadOnly() => IsReadOnlyProperty(PropertyDescriptor);

            protected override void OnPropertyTaskItemUpdated(ToolTip toolTip, ref int currentTabIndex)
            {
                _label.Text = StripAmpersands(PropertyItem.DisplayName);
                _label.TabIndex = currentTabIndex++;
                toolTip.SetToolTip(_label, PropertyItem.Description);
                _textBoxDirty = false;

                if (IsReadOnly())
                {
                    _readOnlyTextBoxLabel.Visible = true;
                    _textBox.Visible = false;
                    // REVIEW: Setting Visible to false doesn't seem to work, so position far away
                    _textBox.Location = new Point(int.MaxValue, int.MaxValue);
                    _editControl = _readOnlyTextBoxLabel;
                }
                else
                {
                    _readOnlyTextBoxLabel.Visible = false;
                    // REVIEW: Setting Visible to false doesn't seem to work, so position far away
                    _readOnlyTextBoxLabel.Location = new Point(int.MaxValue, int.MaxValue);
                    _textBox.Visible = true;
                    _editControl = _textBox;
                }
                _editControl.AccessibleDescription = PropertyItem.Description;
                _editControl.AccessibleName = StripAmpersands(PropertyItem.DisplayName);
                _editControl.TabIndex = currentTabIndex++;
                _editControl.BringToFront();
            }

            protected virtual void OnReadOnlyTextBoxLabelClick(object sender, MouseEventArgs e)
            {
                if (e.Button == MouseButtons.Left)
                {
                    Focus();
                }
            }

            private void OnReadOnlyTextBoxLabelEnter(object sender, EventArgs e)
            {
                _readOnlyTextBoxLabel.ForeColor = SystemColors.HighlightText;
                _readOnlyTextBoxLabel.BackColor = SystemColors.Highlight;
            }

            private void OnReadOnlyTextBoxLabelLeave(object sender, EventArgs e)
            {
                _readOnlyTextBoxLabel.ForeColor = SystemColors.WindowText;
                _readOnlyTextBoxLabel.BackColor = SystemColors.Window;
            }

            protected TypeConverter.StandardValuesCollection GetStandardValues()
            {
                TypeConverter converter = PropertyDescriptor.Converter;
                if (converter != null &&
                    converter.GetStandardValuesSupported(TypeDescriptorContext))
                {
                    return converter.GetStandardValues(TypeDescriptorContext);
                }
                return null;
            }

            private void OnEditControlKeyDown(KeyEventArgs e)
            {
                if (e.KeyCode == Keys.Down)
                {
                    e.Handled = true;
                    // Try to find the existing value and then pick the one after it
                    TypeConverter.StandardValuesCollection standardValues = GetStandardValues();
                    if (standardValues != null)
                    {
                        for (int i = 0; i < standardValues.Count; i++)
                        {
                            if (object.Equals(Value, standardValues[i]))
                            {
                                if (i < standardValues.Count - 1)
                                {
                                    SetValue(standardValues[i + 1]);
                                }
                                return;
                            }
                        }
                        // Previous value wasn't found, select the first one by default
                        if (standardValues.Count > 0)
                        {
                            SetValue(standardValues[0]);
                        }
                    }
                    return;
                }

                if (e.KeyCode == Keys.Up)
                {
                    e.Handled = true;
                    // Try to find the existing value and then pick the one before it
                    TypeConverter.StandardValuesCollection standardValues = GetStandardValues();
                    if (standardValues != null)
                    {
                        for (int i = 0; i < standardValues.Count; i++)
                        {
                            if (object.Equals(Value, standardValues[i]))
                            {
                                if (i > 0)
                                {
                                    SetValue(standardValues[i - 1]);
                                }
                                return;
                            }
                        }
                        // Previous value wasn't found, select the first one by default
                        if (standardValues.Count > 0)
                        {
                            SetValue(standardValues[standardValues.Count - 1]);
                        }
                    }
                    return;
                }
            }

            private void OnReadOnlyTextBoxLabelKeyDown(object sender, KeyEventArgs e)
            {
                // Delegate the rest of the processing to a common helper
                OnEditControlKeyDown(e);
            }

            private void OnTextBoxKeyDown(object sender, KeyEventArgs e)
            {
                if (ActionPanel.DropDownActive)
                {
                    return;
                }

                if (e.KeyCode == Keys.Enter)
                {
                    UpdateValue();
                    e.Handled = true;
                    return;
                }
                // Delegate the rest of the processing to a common helper
                OnEditControlKeyDown(e);
            }

            private void OnTextBoxLostFocus(object sender, EventArgs e)
            {
                if (ActionPanel.DropDownActive)
                {
                    return;
                }
                UpdateValue();
            }

            private void OnTextBoxTextChanged(object sender, EventArgs e) => _textBoxDirty = true;

            protected override void OnValueChanged() => _editControl.Text = PropertyDescriptor.Converter.ConvertToString(TypeDescriptorContext, Value);

            public override void PaintLine(Graphics g, int lineWidth, int lineHeight)
            {
                Rectangle editRect = new Rectangle(EditRegionRelativeLocation, EditRegionSize);
                g.FillRectangle(SystemBrushes.Window, editRect);
                g.DrawRectangle(SystemPens.ControlDark, editRect);
            }

            internal void SetEditRegionXPos(int xPos)
            {
                // Ignore the x-position if we have no text. This allows the textbox to span the entire width of the panel.
                if (!string.IsNullOrEmpty(_label.Text))
                {
                    _editXPos = xPos;
                }
                else
                {
                    _editXPos = LineLeftMargin;
                }
            }

            private void UpdateValue()
            {
                if (_textBoxDirty)
                {
                    SetValue(_editControl.Text);
                    _textBoxDirty = false;
                }
            }

            /// <summary>
            ///  Custom label that provides accurate accessibility information and focus abilities.
            /// </summary>
            private sealed class EditorLabel : Label
            {
                public EditorLabel()
                {
                    SetStyle(ControlStyles.Selectable, true);
                }

                protected override AccessibleObject CreateAccessibilityInstance() => new EditorLabelAccessibleObject(this);

                protected override void OnGotFocus(EventArgs e)
                {
                    base.OnGotFocus(e);
                    // Since we are not a standard focusable control, we have to raise our own accessibility events.
                    // objectID = OBJID_WINDOW, childID = CHILDID_SELF - 1 (the -1 is because WinForms always adds 1 to the value) (these consts are defined in winuser.h)
                    AccessibilityNotifyClients(AccessibleEvents.Focus, 0, -1);
                }

                protected override bool IsInputKey(Keys keyData)
                {
                    if (keyData == Keys.Down ||
                        keyData == Keys.Up)
                    {
                        return true;
                    }
                    return base.IsInputKey(keyData);
                }

                private sealed class EditorLabelAccessibleObject : ControlAccessibleObject
                {
                    public EditorLabelAccessibleObject(EditorLabel owner) : base(owner)
                    {
                    }

                    public override string Value
                    {
                        get => Owner.Text;
                    }
                }
            }
        }

        private sealed class EditorPropertyLine : TextBoxPropertyLine, IWindowsFormsEditorService, IServiceProvider
        {
            private EditorButton _button;
            private UITypeEditor _editor;
            private bool _hasSwatch;
            private Image _swatch;
            private FlyoutDialog _dropDownHolder;
            private bool _ignoreNextSelectChange;
            private bool _ignoreDropDownValue;

            public EditorPropertyLine(IServiceProvider serviceProvider, DesignerActionPanel actionPanel) : base(serviceProvider, actionPanel)
            {
            }

            private void ActivateDropDown()
            {
                if (_editor != null)
                {
                    try
                    {
                        object newValue = _editor.EditValue(TypeDescriptorContext, this, Value);
                        SetValue(newValue);
                    }
                    catch (Exception ex)
                    {
                        ActionPanel.ShowError(string.Format(SR.DesignerActionPanel_ErrorActivatingDropDown, ex.Message));
                    }
                }
                else
                {
                    ListBox listBox = new ListBox
                    {
                        BorderStyle = BorderStyle.None,
                        IntegralHeight = false,
                        Font = ActionPanel.Font
                    };

                    listBox.KeyDown += new KeyEventHandler(OnListBoxKeyDown);
                    TypeConverter.StandardValuesCollection standardValues = GetStandardValues();
                    if (standardValues != null)
                    {
                        foreach (object o in standardValues)
                        {
                            string newItem = PropertyDescriptor.Converter.ConvertToString(TypeDescriptorContext, CultureInfo.CurrentCulture, o);
                            listBox.Items.Add(newItem);

                            if ((o != null) && o.Equals(Value))
                            {
                                listBox.SelectedItem = newItem;
                            }
                        }
                    }

                    // All measurement code borrowed from WinForms PropertyGridView.cs
                    int maxWidth = 0;

                    // The listbox draws with GDI, not GDI+.  So, we use a normal DC here.
                    using (var hdc = new User32.GetDcScope(listBox.Handle))
                    {
                        using var hFont = new Gdi32.ObjectScope(listBox.Font.ToHFONT());
                        using var fontSelection = new Gdi32.SelectObjectScope(hdc, hFont);

                        var tm = new Gdi32.TEXTMETRICW();

                        if (listBox.Items.Count > 0)
                        {
                            foreach (string s in listBox.Items)
                            {
                                var textSize = new Size();
                                Gdi32.GetTextExtentPoint32W(hdc, s, s.Length, ref textSize);
                                maxWidth = Math.Max(textSize.Width, maxWidth);
                            }
                        }

                        Gdi32.GetTextMetricsW(hdc, ref tm);

                        // border + padding + scrollbar
                        maxWidth += 2 + tm.tmMaxCharWidth + SystemInformation.VerticalScrollBarWidth;

                        listBox.Height = Math.Max(tm.tmHeight + 2, Math.Min(ListBoxMaximumHeight, listBox.PreferredHeight));
                        listBox.Width = Math.Max(maxWidth, EditRegionSize.Width);
                        _ignoreDropDownValue = false;
                    }

                    try
                    {
                        ShowDropDown(listBox, SystemColors.ControlDark);
                    }
                    finally
                    {
                        listBox.KeyDown -= new KeyEventHandler(OnListBoxKeyDown);
                    }

                    if (!_ignoreDropDownValue)
                    {
                        if (listBox.SelectedItem != null)
                        {
                            SetValue(listBox.SelectedItem);
                        }
                    }
                }
            }

            protected override void AddControls(List<Control> controls)
            {
                base.AddControls(controls);
                _button = new EditorButton();
                controls.Add(_button);
            }

            private void CloseDropDown()
            {
                if (_dropDownHolder != null)
                {
                    _dropDownHolder.Visible = false;
                }
            }

            protected override int GetTextBoxLeftPadding(int textBoxHeight)
            {
                if (_hasSwatch)
                {
                    return base.GetTextBoxLeftPadding(textBoxHeight) + textBoxHeight + 2 * EditorLineSwatchPadding;
                }
                else
                {
                    return base.GetTextBoxLeftPadding(textBoxHeight);
                }
            }

            protected override int GetTextBoxRightPadding(int textBoxHeight) => base.GetTextBoxRightPadding(textBoxHeight) + textBoxHeight + 2 * EditorLineButtonPadding;

            protected override bool IsReadOnly()
            {
                if (base.IsReadOnly())
                {
                    return true;
                }

                // If we can't convert from string, we are readonly because we can't convert the user's input
                bool converterReadOnly = !PropertyDescriptor.Converter.CanConvertFrom(TypeDescriptorContext, typeof(string));
                // If standard values are supported and are exclusive, we are readonly
                bool standardValuesExclusive =
                    PropertyDescriptor.Converter.GetStandardValuesSupported(TypeDescriptorContext) &&
                    PropertyDescriptor.Converter.GetStandardValuesExclusive(TypeDescriptorContext);
                return converterReadOnly || standardValuesExclusive;
            }

            public override Size LayoutControls(int top, int width, bool measureOnly)
            {
                Size size = base.LayoutControls(top, width, measureOnly);
                if (!measureOnly)
                {
                    int buttonHeight = EditRegionSize.Height - EditorLineButtonPadding * 2 - 1;
                    _button.Location = new Point(EditRegionLocation.X + EditRegionSize.Width - buttonHeight - EditorLineButtonPadding, EditRegionLocation.Y + EditorLineButtonPadding + 1);
                    _button.Size = new Size(buttonHeight, buttonHeight);
                }
                return size;
            }

            private void OnButtonClick(object sender, EventArgs e)
            {
                ActivateDropDown();
            }

            private void OnButtonGotFocus(object sender, EventArgs e)
            {
                if (!_button.Ellipsis)
                {
                    Focus();
                }
            }

            private void OnListBoxKeyDown(object sender, KeyEventArgs e)
            {
                // Always respect the enter key and F4
                if (e.KeyData == Keys.Enter)
                {
                    _ignoreNextSelectChange = false;
                    CloseDropDown();
                    e.Handled = true;
                }
                else
                {
                    // Ignore selected index change events when the user is navigating via the keyboard
                    _ignoreNextSelectChange = true;
                }
            }

            private void OnListBoxSelectedIndexChanged(object sender, EventArgs e)
            {
                // If we're ignoring this selected index change, do nothing
                if (_ignoreNextSelectChange)
                {
                    _ignoreNextSelectChange = false;
                }
                else
                {
                    CloseDropDown();
                }
            }

            protected override void OnPropertyTaskItemUpdated(ToolTip toolTip, ref int currentTabIndex)
            {
                _editor = (UITypeEditor)PropertyDescriptor.GetEditor(typeof(UITypeEditor));
                base.OnPropertyTaskItemUpdated(toolTip, ref currentTabIndex);
                if (_editor != null)
                {
                    _button.Ellipsis = (_editor.GetEditStyle(TypeDescriptorContext) == UITypeEditorEditStyle.Modal);
                    _hasSwatch = _editor.GetPaintValueSupported(TypeDescriptorContext);
                }
                else
                {
                    _button.Ellipsis = false;
                }

                if (_button.Ellipsis)
                {
                    EditControl.AccessibleRole = (IsReadOnly() ? AccessibleRole.StaticText : AccessibleRole.Text);
                }
                else
                {
                    EditControl.AccessibleRole = (IsReadOnly() ? AccessibleRole.DropList : AccessibleRole.ComboBox);
                }

                _button.TabStop = _button.Ellipsis;
                _button.TabIndex = currentTabIndex++;
                _button.AccessibleRole = (_button.Ellipsis ? AccessibleRole.PushButton : AccessibleRole.ButtonDropDown);
                _button.AccessibleDescription = EditControl.AccessibleDescription;
                _button.AccessibleName = EditControl.AccessibleName;
            }

            protected override void OnReadOnlyTextBoxLabelClick(object sender, MouseEventArgs e)
            {
                base.OnReadOnlyTextBoxLabelClick(sender, e);
                if (e.Button == MouseButtons.Left)
                {
                    if (ActionPanel.DropDownActive)
                    {
                        _ignoreDropDownValue = true;
                        CloseDropDown();
                    }
                    else
                    {
                        ActivateDropDown();
                    }
                }
            }

            protected override void OnValueChanged()
            {
                base.OnValueChanged();
                _swatch = null;
                if (_hasSwatch)
                {
                    ActionPanel.Invalidate(new Rectangle(EditRegionLocation, EditRegionSize), false);
                }
            }

            public override void PaintLine(Graphics g, int lineWidth, int lineHeight)
            {
                base.PaintLine(g, lineWidth, lineHeight);
                if (_hasSwatch)
                {
                    if (_swatch is null)
                    {
                        int width = EditRegionSize.Height - EditorLineSwatchPadding * 2;
                        int height = width - 1;
                        _swatch = new Bitmap(width, height);
                        Rectangle rect = new Rectangle(1, 1, width - 2, height - 2);
                        using (Graphics swatchGraphics = Graphics.FromImage(_swatch))
                        {
                            _editor.PaintValue(Value, swatchGraphics, rect);
                            swatchGraphics.DrawRectangle(SystemPens.ControlDark, new Rectangle(0, 0, width - 1, height - 1));
                        }
                    }
                    g.DrawImage(_swatch, new Point(EditRegionRelativeLocation.X + 2, EditorLineSwatchPadding + 5));
                }
            }

            protected internal override bool ProcessDialogKey(Keys keyData)
            {
                // Do this here rather than in OnKeyDown because if hierarchy is properly set, VS is going to eat the F4 in PreProcessMessage, preventing it from ever getting to an OnKeyDown on this control. Doing it here also allow to not hook up to multiple events for each button.
                if (!_button.Focused && !_button.Ellipsis)
                {
                    if ((keyData == (Keys.Alt | Keys.Down)) || (keyData == (Keys.Alt | Keys.Up)) || (keyData == Keys.F4))
                    {
                        if (!ActionPanel.DropDownActive)
                        {
                            ActivateDropDown();
                        }
                        else
                        {
                            CloseDropDown();
                        }

                        return true;
                    }
                    // Not passing Alt key event to base class to prevent  closing 'Combobox Tasks window'
                    else if ((keyData & Keys.Alt) == Keys.Alt)
                    {
                        return true;
                    }
                }
                return base.ProcessDialogKey(keyData);
            }

            private void ShowDropDown(Control hostedControl, Color borderColor)
            {
                hostedControl.Width = Math.Max(hostedControl.Width, EditRegionSize.Width - 2);
                _dropDownHolder = new DropDownHolder(hostedControl, ActionPanel, borderColor, ActionPanel.Font, this);
                if (ActionPanel.RightToLeft != RightToLeft.Yes)
                {
                    Rectangle editorBounds = new Rectangle(Point.Empty, EditRegionSize);
                    Size dropDownSize = _dropDownHolder.Size;
                    Point editorLocation = ActionPanel.PointToScreen(EditRegionLocation);
                    Rectangle rectScreen = Screen.FromRectangle(ActionPanel.RectangleToScreen(editorBounds)).WorkingArea;
                    dropDownSize.Width = Math.Max(editorBounds.Width + 1, dropDownSize.Width);
                    editorLocation.X = Math.Min(rectScreen.Right - dropDownSize.Width, // min = right screen edge clip
                        Math.Max(rectScreen.X, editorLocation.X + editorBounds.Right - dropDownSize.Width)); // max = left screen edge clip
                    editorLocation.Y += editorBounds.Y;
                    if (rectScreen.Bottom < (dropDownSize.Height + editorLocation.Y + editorBounds.Height))
                    {
                        editorLocation.Y -= dropDownSize.Height + 1;
                    }
                    else
                    {
                        editorLocation.Y += editorBounds.Height;
                    }
                    _dropDownHolder.Location = editorLocation;
                }
                else
                {
                    _dropDownHolder.RightToLeft = ActionPanel.RightToLeft;
                    Rectangle editorBounds = new Rectangle(Point.Empty, EditRegionSize);
                    Size dropDownSize = _dropDownHolder.Size;
                    Point editorLocation = ActionPanel.PointToScreen(EditRegionLocation);
                    Rectangle rectScreen = Screen.FromRectangle(ActionPanel.RectangleToScreen(editorBounds)).WorkingArea;
                    dropDownSize.Width = Math.Max(editorBounds.Width + 1, dropDownSize.Width);
                    editorLocation.X = Math.Min(rectScreen.Right - dropDownSize.Width, // min = right screen edge clip
                        Math.Max(rectScreen.X, editorLocation.X - editorBounds.Width)); // max = left screen edge clip
                    editorLocation.Y += editorBounds.Y;
                    if (rectScreen.Bottom < (dropDownSize.Height + editorLocation.Y + editorBounds.Height))
                    {
                        editorLocation.Y -= dropDownSize.Height + 1;
                    }
                    else
                    {
                        editorLocation.Y += editorBounds.Height;
                    }
                    _dropDownHolder.Location = editorLocation;
                }

                ActionPanel.InMethodInvoke = true;
                ActionPanel.SetDropDownActive(true);
                try
                {
                    _dropDownHolder.ShowDropDown(_button);
                }
                finally
                {
                    _button.ResetMouseStates();
                    ActionPanel.SetDropDownActive(false);
                    ActionPanel.InMethodInvoke = false;
                }
            }

            #region IWindowsFormsEditorService implementation
            void IWindowsFormsEditorService.CloseDropDown()
            {
                CloseDropDown();
            }

            void IWindowsFormsEditorService.DropDownControl(Control control)
            {
                ShowDropDown(control, ActionPanel.BorderColor);
            }

            DialogResult IWindowsFormsEditorService.ShowDialog(Form dialog)
            {
                IUIService uiService = (IUIService)ServiceProvider.GetService(typeof(IUIService));
                if (uiService != null)
                {
                    return uiService.ShowDialog(dialog);
                }
                return dialog.ShowDialog();
            }
            #endregion

            #region IServiceProvider implementation
            object IServiceProvider.GetService(Type serviceType)
            {
                // Inject this class as the IWindowsFormsEditroService so drop-down custom editors can work
                if (serviceType == typeof(IWindowsFormsEditorService))
                {
                    return this;
                }
                return ServiceProvider.GetService(serviceType);
            }
            #endregion

            private class DropDownHolder : FlyoutDialog
            {
                private readonly EditorPropertyLine _parent;
                public DropDownHolder(Control hostedControl, Control parentControl, Color borderColor, Font font, EditorPropertyLine parent) : base(hostedControl, parentControl, borderColor, font)
                {
                    _parent = parent;
                    _parent.ActionPanel.SetDropDownActive(true);
                }

                protected override void OnClosed(EventArgs e)
                {
                    base.OnClosed(e);
                    _parent.ActionPanel.SetDropDownActive(false);
                }

                protected override bool ProcessDialogKey(Keys keyData)
                {
                    if (keyData == Keys.Escape)
                    {
                        // Indicates that the selection was aborted so we should ignore the value
                        _parent._ignoreDropDownValue = true;
                        Visible = false;
                        return true;
                    }

                    return base.ProcessDialogKey(keyData);
                }
            }

            internal class FlyoutDialog : Form, IHandle
            {
                private readonly Control _hostedControl;
                private readonly Control _parentControl;

                public FlyoutDialog(Control hostedControl, Control parentControl, Color borderColor, Font font)
                {
                    _hostedControl = hostedControl;
                    _parentControl = parentControl;
                    BackColor = SystemColors.Window;
                    ControlBox = false;
                    Font = font;
                    FormBorderStyle = FormBorderStyle.None;
                    MinimizeBox = false;
                    MaximizeBox = false;
                    ShowInTaskbar = false;
                    StartPosition = FormStartPosition.Manual;
                    Text = string.Empty;
                    SuspendLayout();
                    try
                    {
                        Controls.Add(hostedControl);
                        int width = Math.Max(_hostedControl.Width, SystemInformation.MinimumWindowSize.Width);
                        int height = Math.Max(_hostedControl.Height, SystemInformation.MinimizedWindowSize.Height);
                        if (!borderColor.IsEmpty)
                        {
                            DockPadding.All = 1;
                            BackColor = borderColor;
                            width += 2;
                            height += 4;
                        }
                        _hostedControl.Dock = DockStyle.Fill;

                        Width = width;
                        Height = height;
                    }
                    finally
                    {
                        ResumeLayout();
                    }
                }

                protected override CreateParams CreateParams
                {
                    get
                    {
                        CreateParams cp = base.CreateParams;
                        cp.ExStyle |= (int)User32.WS_EX.TOOLWINDOW;
                        cp.Style |= unchecked((int)(User32.WS.POPUP | User32.WS.BORDER));
                        cp.ClassStyle |= (int)User32.CS.SAVEBITS;
                        if (_parentControl != null)
                        {
                            if (!_parentControl.IsDisposed)
                            {
                                cp.Parent = _parentControl.Handle;
                            }
                        }
                        return cp;
                    }
                }

                public virtual void FocusComponent()
                {
                    if (_hostedControl != null && Visible)
                    {
                        _hostedControl.Focus();
                    }
                }
                // Lifted directly from PropertyGridView.DropDownHolder. Less destructive than using ShowDialog().
                public void DoModalLoop()
                {
                    while (Visible)
                    {
                        Application.DoEvents();
                        User32.MsgWaitForMultipleObjectsEx(0, IntPtr.Zero, 250, User32.QS.ALLINPUT, User32.MWMO.INPUTAVAILABLE);
                    }
                }

                /// <summary>
                ///  General purpose method, based on Control.Contains()... Determines whether a given window (specified using native window handle) is a descendant of this control. This catches both contained descendants and 'owned' windows such as modal dialogs. Using window handles rather than Control objects allows it to catch un-managed windows as well.
                /// </summary>
                private bool OwnsWindow(IntPtr hWnd)
                {
                    while (hWnd != IntPtr.Zero)
                    {
                        hWnd = User32.GetWindowLong(hWnd, User32.GWL.HWNDPARENT);
                        if (hWnd == IntPtr.Zero)
                        {
                            return false;
                        }
                        if (hWnd == Handle)
                        {
                            return true;
                        }
                    }
                    return false;
                }

                protected override bool ProcessDialogKey(Keys keyData)
                {
                    if ((keyData == (Keys.Alt | Keys.Down)) ||
                        (keyData == (Keys.Alt | Keys.Up)) ||
                        (keyData == Keys.F4))
                    {
                        // Any of these keys indicates the selection is accepted
                        Visible = false;
                        return true;
                    }
                    return base.ProcessDialogKey(keyData);
                }

                public void ShowDropDown(Control parent)
                {
                    try
                    {
                        User32.SetWindowLong(this, User32.GWL.HWNDPARENT, parent.Handle);

                        // Lifted directly from Form.ShowDialog()...
                        IntPtr hWndCapture = User32.GetCapture();
                        if (hWndCapture != IntPtr.Zero)
                        {
                            User32.SendMessageW(hWndCapture, User32.WM.CANCELMODE, IntPtr.Zero, IntPtr.Zero);
                            User32.ReleaseCapture();
                        }
                        Visible = true; // NOTE: Do this AFTER creating handle and setting parent
                        FocusComponent();
                        DoModalLoop();
                    }
                    finally
                    {
                        User32.SetWindowLong(this, User32.GWL.HWNDPARENT, IntPtr.Zero);

                        // sometimes activation goes to LALA land - if our parent control is still  around, remind it to take focus.
                        if (parent != null && parent.Visible)
                        {
                            parent.Focus();
                        }
                    }
                }

                protected override void WndProc(ref Message m)
                {
                    if (m.Msg == (int)User32.WM.ACTIVATE)
                    {
                        if (Visible && PARAM.LOWORD(m.WParam) == (int)User32.WA.INACTIVE)
                        {
                            if (!OwnsWindow(m.LParam))
                            {
                                Visible = false;
                                if (m.LParam == IntPtr.Zero)
                                { //we 're switching process, also dismiss the parent
                                    Control toplevel = _parentControl.TopLevelControl;
                                    if (toplevel is ToolStripDropDown dropDown)
                                    {
                                        // if it's a toolstrip dropdown let it know that we have a specific close reason.
                                        dropDown.Close();
                                    }
                                    else if (toplevel != null)
                                    {
                                        toplevel.Visible = false;
                                    }
                                }
                                return;
                            }
                        }
                    }
                    base.WndProc(ref m);
                }
            }

            // Class that renders either the ellipsis or dropdown button
            internal sealed class EditorButton : Button
            {
                private bool _mouseOver;
                private bool _mouseDown;
                private bool _ellipsis;

                protected override void OnMouseDown(MouseEventArgs e)
                {
                    base.OnMouseDown(e);

                    if (e.Button == MouseButtons.Left)
                    {
                        _mouseDown = true;
                    }
                }

                protected override void OnMouseEnter(EventArgs e)
                {
                    base.OnMouseEnter(e);
                    _mouseOver = true;
                }

                protected override void OnMouseLeave(EventArgs e)
                {
                    base.OnMouseLeave(e);
                    _mouseOver = false;
                }

                protected override void OnMouseUp(MouseEventArgs e)
                {
                    base.OnMouseUp(e);

                    if (e.Button == MouseButtons.Left)
                    {
                        _mouseDown = false;
                    }
                }

                public bool Ellipsis
                {
                    get => _ellipsis;
                    set => _ellipsis = value;
                }

                protected override void OnPaint(PaintEventArgs e)
                {
                    Graphics g = e.Graphics;
                    if (_ellipsis)
                    {
                        PushButtonState buttonState = PushButtonState.Normal;
                        if (_mouseDown)
                        {
                            buttonState = PushButtonState.Pressed;
                        }
                        else if (_mouseOver)
                        {
                            buttonState = PushButtonState.Hot;
                        }

                        ButtonRenderer.DrawButton(g, new Rectangle(-1, -1, Width + 2, Height + 2), "…", Font, Focused, buttonState);
                    }
                    else
                    {
                        if (ComboBoxRenderer.IsSupported)
                        {
                            ComboBoxState state = ComboBoxState.Normal;
                            if (Enabled)
                            {
                                if (_mouseDown)
                                {
                                    state = ComboBoxState.Pressed;
                                }
                                else if (_mouseOver)
                                {
                                    state = ComboBoxState.Hot;
                                }
                            }
                            else
                            {
                                state = ComboBoxState.Disabled;
                            }
                            ComboBoxRenderer.DrawDropDownButton(g, new Rectangle(0, 0, Width, Height), state);
                        }
                        else
                        {
                            PushButtonState buttonState = PushButtonState.Normal;
                            if (Enabled)
                            {
                                if (_mouseDown)
                                {
                                    buttonState = PushButtonState.Pressed;
                                }
                                else if (_mouseOver)
                                {
                                    buttonState = PushButtonState.Hot;
                                }
                            }
                            else
                            {
                                buttonState = PushButtonState.Disabled;
                            }

                            ButtonRenderer.DrawButton(g, new Rectangle(-1, -1, Width + 2, Height + 2), string.Empty, Font, Focused, buttonState);
                            // Draw the arrow icon
                            try
                            {
                                Icon icon = new Icon(typeof(DesignerActionPanel), "Arrow.ico");
                                try
                                {
                                    Bitmap arrowBitmap = icon.ToBitmap();
                                    // Make sure we draw properly under high contrast by re-mapping the arrow color to the WindowText color
                                    ImageAttributes attrs = new ImageAttributes();
                                    try
                                    {
                                        ColorMap cm = new ColorMap
                                        {
                                            OldColor = Color.Black,
                                            NewColor = SystemColors.WindowText
                                        };
                                        attrs.SetRemapTable(new ColorMap[] { cm }, ColorAdjustType.Bitmap);
                                        int imageWidth = arrowBitmap.Width;
                                        int imageHeight = arrowBitmap.Height;
                                        g.DrawImage(arrowBitmap, new Rectangle((Width - imageWidth + 1) / 2, (Height - imageHeight + 1) / 2, imageWidth, imageHeight),
                                                    0, 0, imageWidth, imageWidth, GraphicsUnit.Pixel, attrs, null, IntPtr.Zero);
                                    }
                                    finally
                                    {
                                        if (attrs != null)
                                        {
                                            attrs.Dispose();
                                        }
                                    }
                                }
                                finally
                                {
                                    if (icon != null)
                                    {
                                        icon.Dispose();
                                    }
                                }
                            }
                            catch
                            {
                            }
                        }
                        if (Focused)
                        {
                            ControlPaint.DrawFocusRectangle(g, new Rectangle(2, 2, Width - 5, Height - 5));
                        }
                    }
                }

                public void ResetMouseStates()
                {
                    _mouseDown = false;
                    _mouseOver = false;
                    Invalidate();
                }
            }
        }

        private class TextLine : Line
        {
            private Label _label;
            private DesignerActionTextItem _textItem;

            public TextLine(IServiceProvider serviceProvider, DesignerActionPanel actionPanel) : base(serviceProvider, actionPanel)
            {
            }

            public sealed override string FocusId
            {
                get => string.Empty;
            }

            protected override void AddControls(List<Control> controls)
            {
                _label = new Label
                {
                    BackColor = Color.Transparent,
                    ForeColor = ActionPanel.LabelForeColor,
                    TextAlign = Drawing.ContentAlignment.MiddleLeft,
                    UseMnemonic = false
                };
                controls.Add(_label);
            }

            public sealed override void Focus()
            {
                Debug.Fail("Should never try to focus a TextLine");
            }

            public override Size LayoutControls(int top, int width, bool measureOnly)
            {
                Size labelSize = _label.GetPreferredSize(new Size(int.MaxValue, int.MaxValue));
                if (!measureOnly)
                {
                    _label.Location = new Point(LineLeftMargin, top + LineVerticalPadding / 2);
                    _label.Size = labelSize;
                }
                return labelSize + new Size(LineLeftMargin + LineRightMargin, LineVerticalPadding);
            }

            private void OnParentControlFontChanged(object sender, EventArgs e)
            {
                if (_label != null && _label.Font != null)
                {
                    _label.Font = GetFont();
                }
            }

            protected virtual Font GetFont()
            {
                return ActionPanel.Font;
            }

            internal override void UpdateActionItem(DesignerActionList actionList, DesignerActionItem actionItem, ToolTip toolTip, ref int currentTabIndex)
            {
                _textItem = (DesignerActionTextItem)actionItem;
                _label.Text = StripAmpersands(_textItem.DisplayName);
                _label.Font = GetFont();
                _label.TabIndex = currentTabIndex++;
                toolTip.SetToolTip(_label, _textItem.Description);
            }
        }

        private sealed class HeaderLine : TextLine
        {
            public HeaderLine(IServiceProvider serviceProvider, DesignerActionPanel actionPanel) : base(serviceProvider, actionPanel)
            {
            }

            protected override Font GetFont() => new Font(ActionPanel.Font, FontStyle.Bold);
        }

        private sealed class SeparatorLine : Line
        {
            private readonly bool _isSubSeparator;
            public SeparatorLine(IServiceProvider serviceProvider, DesignerActionPanel actionPanel) : this(serviceProvider, actionPanel, false)
            {
            }

            public SeparatorLine(IServiceProvider serviceProvider, DesignerActionPanel actionPanel, bool isSubSeparator) : base(serviceProvider, actionPanel)
            {
                _isSubSeparator = isSubSeparator;
            }

            public sealed override string FocusId
            {
                get => string.Empty;
            }

            public bool IsSubSeparator => _isSubSeparator;

            protected override void AddControls(List<Control> controls)
            {
            }

            public sealed override void Focus() => Debug.Fail("Should never try to focus a SeparatorLine");

            public override Size LayoutControls(int top, int width, bool measureOnly) => new Size(MinimumWidth, 1);

            public override void PaintLine(Graphics g, int lineWidth, int lineHeight)
            {
                using (Pen p = new Pen(ActionPanel.SeparatorColor))
                {
                    g.DrawLine(p, SeparatorHorizontalPadding, 0, lineWidth - (SeparatorHorizontalPadding + 1), 0);
                }
            }

            internal override void UpdateActionItem(DesignerActionList actionList, DesignerActionItem actionItem, ToolTip toolTip, ref int currentTabIndex)
            {
            }
        }
    }
}
