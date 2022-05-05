// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Text;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace System.ComponentModel.Design
{
    internal sealed partial class DesignerActionPanel : ContainerControl
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
            if (uiService is not null)
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
                    _filteredCommandIDs = new CommandID[]
                    {
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
                if (activeControl is not null)
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

        private static void AddToCategories(LineInfo lineInfo, ListDictionary categories)
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
            if (!e.Cancel && TopLevelControl is not null)
            {
                Debug.Assert(TopLevelControl is Form, "DesignerActionPanel must be hosted on a Form.");
                Form form = (Form)TopLevelControl;
                if (form is not null)
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
            if (focusedLine is not null)
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
                if (list is not null)
                {
                    IEnumerable items = list.GetSortedActionItems();
                    if (items is not null)
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

                            if (relatedComponent is not null)
                            {
                                IEnumerable<LineInfo> relatedLineInfos = ProcessRelatedTaskItems(relatedComponent);
                                if (relatedLineInfos is not null)
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
            Debug.Assert(relatedComponent is not null);
            DesignerActionListCollection relatedLists = null;
            DesignerActionService actionService = (DesignerActionService)ServiceProvider.GetService(typeof(DesignerActionService));
            if (actionService is not null)
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
                if (host is not null)
                {
                    if (host.GetDesigner(relatedComponent) is ComponentDesigner componentDesigner)
                    {
                        relatedLists = componentDesigner.ActionLists;
                    }
                }
            }

            List<LineInfo> lineInfos = new List<LineInfo>();

            if (relatedLists is not null)
            {
                foreach (DesignerActionList relatedList in relatedLists)
                {
                    if (relatedList is not null)
                    {
                        IEnumerable items = relatedList.GetSortedActionItems();
                        if (items is not null)
                        {
                            foreach (DesignerActionItem relatedItem in items)
                            {
                                if (relatedItem is not null)
                                {
                                    if (relatedItem.AllowAssociate)
                                    {
                                        LineInfo lineInfo = ProcessTaskItem(relatedList, relatedItem);
                                        if (lineInfo is not null)
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
            if (uiService is not null)
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
                    editXPos = Math.Max(editXPos, line.GetEditRegionXPos());
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
                if (focusedLine is not null)
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
    }
}
