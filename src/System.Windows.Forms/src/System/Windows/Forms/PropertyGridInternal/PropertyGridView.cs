// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows.Forms.Design;
using System.Windows.Forms.VisualStyles;
using Microsoft.Win32;
using static Interop;

namespace System.Windows.Forms.PropertyGridInternal
{
    internal sealed partial class PropertyGridView :
        Control,
        IWin32Window,
        IWindowsFormsEditorService,
        IServiceProvider
    {
        private static Point InvalidPoint { get; } = new(int.MinValue, int.MinValue);

        private static readonly TraceSwitch s_gridViewDebugPaint = new("GridViewDebugPaint", "PropertyGridView: Debug property painting");

        // Constants
        private const int EditIndent = 0;
        private const int OutlineIndent = 10;
        private const int OutlineSize = 9;
        private int _outlineSize = OutlineSize;
        private const int OutlineSizeExplorerTreeStyle = 16;
        private int _outlineSizeExplorerTreeStyle = OutlineSizeExplorerTreeStyle;
        private const int PaintWidth = 20;
        private int _paintWidth = PaintWidth;
        private const int PaintIndent = 26;
        private int _paintIndent = PaintIndent;
        private const int MaxListBoxHeight = 200;
        private int _maxListBoxHeight = MaxListBoxHeight;

        private const int RowLabel = 1;
        private const int RowValue = 2;

        internal const short GdiPlusSpace = 2;
        internal const int MaxRecurseExpand = 10;

        private const int DotDotDotIconWidth = 7;
        private const int DotDotDotIconHeight = 8;
        private const int DownArrowIconWidth = 16;
        private const int DownArrowIconHeight = 16;

        private const int Offset2Pixels = 2;
        private int _offset2Units = Offset2Pixels;

        private static Point InvalidPosition { get; } = new(int.MinValue, int.MinValue);

        private Font _boldFont;
        private Color _grayTextColor;

        // For backwards compatibility of default colors
        private bool _grayTextColorModified;

        private GridEntryCollection _allGridEntries;

        // Row information
        public int TotalProperties { get; private set; } = -1;
        private int _visibleRows = -1;
        private int _labelWidth = -1;
        public double _labelRatio = 2;                          // ratio of whole row to label width

        private short _requiredLabelPaintMargin = GdiPlusSpace;

        // current selected row and tooltip.
        private int _selectedRow = -1;
        private GridEntry _selectedGridEntry;
        private int _tipInfo = -1;

        // Editors & controls
        private GridViewTextBox _editTextBox;
        private DropDownButton _dropDownButton;
        private DropDownButton _dialogButton;
        private GridViewListBox _listBox;
        private DropDownHolder _dropDownHolder;
        private Rectangle _lastClientRect = Rectangle.Empty;
        private Control _currentEditor;
        private ScrollBar _scrollBar;
        private GridToolTip _toolTip;
        private GridErrorDialog _errorDialog;

        private Flags _flags = Flags.NeedsRefresh | Flags.IsNewSelection | Flags.NeedUpdateUIBasedOnFont;
        private ErrorState _errorState = ErrorState.None;

        private Point _location = new(1, 1);

        private string _originalTextValue;          // original text, in case of ESC
        private int _cumulativeVerticalWheelDelta;
        private long _rowSelectTime;
        private Point _rowSelectPos = Point.Empty;  // the position that we clicked on a row to test for double clicks
        private Point _lastMouseDown = InvalidPosition;
        private int _lastMouseMove;
        private GridEntry _lastClickedEntry;

        private IServiceProvider _serviceProvider;
        private IHelpService _topHelpService;
        private IHelpService _helpService;

        private readonly EventHandler _valueClick;
        private readonly EventHandler _labelClick;
        private readonly EventHandler _outlineClick;
        private readonly EventHandler _valueDoubleClick;
        private readonly EventHandler _labelDoubleClick;
        private readonly GridEntryRecreateChildrenEventHandler _recreateChildren;

        private int _cachedRowHeight = -1;

        private GridPositionData _positionData;

        public PropertyGridView(IServiceProvider serviceProvider, PropertyGrid propertyGrid)
            : base()
        {
            if (DpiHelper.IsScalingRequired)
            {
                _paintWidth = DpiHelper.LogicalToDeviceUnitsX(PaintWidth);
                _paintIndent = DpiHelper.LogicalToDeviceUnitsX(PaintIndent);
                _outlineSizeExplorerTreeStyle = DpiHelper.LogicalToDeviceUnitsX(OutlineSizeExplorerTreeStyle);
                _outlineSize = DpiHelper.LogicalToDeviceUnitsX(OutlineSize);
                _maxListBoxHeight = DpiHelper.LogicalToDeviceUnitsY(MaxListBoxHeight);
            }

            _valueClick = OnGridEntryValueClick;
            _labelClick = OnGridEntryLabelClick;
            _outlineClick = OnGridEntryOutlineClick;
            _valueDoubleClick = OnGridEntryValueDoubleClick;
            _labelDoubleClick = OnGridEntryLabelDoubleClick;
            _recreateChildren = OnRecreateChildren;

            OwnerGrid = propertyGrid;
            _serviceProvider = serviceProvider;

            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);
            SetStyle(ControlStyles.ResizeRedraw, false);
            SetStyle(ControlStyles.UserMouse, true);

            BackColor = SystemColors.Window;
            ForeColor = SystemColors.WindowText;
            _grayTextColor = SystemColors.GrayText;
            TabStop = true;

            Text = "PropertyGridView";

            CreateUI();
            LayoutWindow(invalidate: true);
        }

        public override Color BackColor
        {
            get => base.BackColor;
            set => base.BackColor = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public bool CanCopy
            => _selectedGridEntry is not null
                && (EditTextBox.Focused || !string.IsNullOrEmpty(_selectedGridEntry.GetPropertyTextValue()));

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public bool CanCut => CanCopy && _selectedGridEntry is not null && _selectedGridEntry.IsTextEditable;

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public bool CanPaste => _selectedGridEntry is not null && _selectedGridEntry.IsTextEditable;

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        public bool CanUndo
        {
            get
            {
                if (!EditTextBox.Visible || !EditTextBox.Focused)
                {
                    return false;
                }

                return User32.SendMessageW(EditTextBox, (User32.WM)User32.EM.CANUNDO) != IntPtr.Zero;
            }
        }

        internal DropDownButton DropDownButton
        {
            get
            {
                if (_dropDownButton is null)
                {
                    OwnerGrid.CheckInCreate();

                    _dropDownButton = new()
                    {
                        UseComboBoxTheme = true
                    };

                    Bitmap bitmap = CreateResizedBitmap("Arrow", DownArrowIconWidth, DownArrowIconHeight);
                    _dropDownButton.Image = bitmap;
                    _dropDownButton.BackColor = SystemColors.Control;
                    _dropDownButton.ForeColor = SystemColors.ControlText;
                    _dropDownButton.Click += OnButtonClick;
                    _dropDownButton.GotFocus += OnDropDownButtonGotFocus;
                    _dropDownButton.LostFocus += OnChildLostFocus;
                    _dropDownButton.TabIndex = 2;

                    CommonEditorSetup(_dropDownButton);
                    _dropDownButton.Size = DpiHelper.IsScalingRequirementMet
                        ? new(SystemInformation.VerticalScrollBarArrowHeightForDpi(_deviceDpi), RowHeight)
                        : new(SystemInformation.VerticalScrollBarArrowHeight, RowHeight);
                }

                return _dropDownButton;
            }
        }

        private Button DialogButton
        {
            get
            {
                if (_dialogButton is null)
                {
                    OwnerGrid.CheckInCreate();

                    _dialogButton = new DropDownButton
                    {
                        BackColor = SystemColors.Control,
                        ForeColor = SystemColors.ControlText,
                        TabIndex = 3,
                        Image = CreateResizedBitmap("dotdotdot", DotDotDotIconWidth, DotDotDotIconHeight)
                    };

                    _dialogButton.Click += OnButtonClick;
                    _dialogButton.KeyDown += OnButtonKeyDown;
                    _dialogButton.GotFocus += OnDropDownButtonGotFocus;
                    _dialogButton.LostFocus += OnChildLostFocus;
                    _dialogButton.Size = DpiHelper.IsScalingRequirementMet
                        ? new Size(SystemInformation.VerticalScrollBarArrowHeightForDpi(_deviceDpi), RowHeight)
                        : new Size(SystemInformation.VerticalScrollBarArrowHeight, RowHeight);

                    CommonEditorSetup(_dialogButton);
                }

                return _dialogButton;
            }
        }

        private static Bitmap GetBitmapFromIcon(string iconName, int iconWidth, int iconHeight)
        {
            Size desiredSize = new(iconWidth, iconHeight);
            Icon icon = new(new Icon(typeof(PropertyGrid), iconName), desiredSize);
            var bitmap = icon.ToBitmap();
            icon.Dispose();

            if (bitmap.Size.Width != iconWidth || bitmap.Size.Height != iconHeight)
            {
                Bitmap scaledBitmap = DpiHelper.CreateResizedBitmap(bitmap, desiredSize);
                if (scaledBitmap is not null)
                {
                    bitmap.Dispose();
                    bitmap = scaledBitmap;
                }
            }

            return bitmap;
        }

        /// <summary>
        ///  The common text box for editing values.
        /// </summary>
        private GridViewTextBox EditTextBox
        {
            get
            {
                if (_editTextBox is null)
                {
                    OwnerGrid.CheckInCreate();

                    _editTextBox = new(this)
                    {
                        BorderStyle = BorderStyle.None,
                        AutoSize = false,
                        TabStop = false,
                        AcceptsReturn = true,
                        BackColor = BackColor,
                        ForeColor = ForeColor
                    };

                    _editTextBox.KeyDown += OnEditKeyDown;
                    _editTextBox.KeyPress += OnEditKeyPress;
                    _editTextBox.GotFocus += OnEditGotFocus;
                    _editTextBox.LostFocus += OnEditLostFocus;
                    _editTextBox.MouseDown += OnEditMouseDown;
                    _editTextBox.TextChanged += OnEditChange;

                    _editTextBox.TabIndex = 1;
                    CommonEditorSetup(_editTextBox);
                }

                return _editTextBox;
            }
        }

        /// <summary>
        ///  Represents the Editor's control accessible object.
        /// </summary>
        internal AccessibleObject EditAccessibleObject => EditTextBox.AccessibilityObject;

        internal GridViewListBox DropDownListBox
        {
            get
            {
                if (_listBox is null)
                {
                    OwnerGrid.CheckInCreate();

                    _listBox = new(this)
                    {
                        DrawMode = DrawMode.OwnerDrawFixed
                    };

                    _listBox.MouseUp += OnListMouseUp;
                    _listBox.DrawItem += OnListDrawItem;
                    _listBox.SelectedIndexChanged += OnListChange;
                    _listBox.KeyDown += OnListKeyDown;
                    _listBox.LostFocus += OnChildLostFocus;
                    _listBox.Visible = true;
                    _listBox.ItemHeight = RowHeight;
                }

                return _listBox;
            }
        }

        /// <summary>
        ///  Represents the DropDownListBox accessible object.
        /// </summary>
        internal AccessibleObject DropDownListBoxAccessibleObject
            => DropDownListBox.Visible ? DropDownListBox.AccessibilityObject : null;

        internal bool DrawValuesRightToLeft
        {
            get
            {
                if (_editTextBox is not null && _editTextBox.IsHandleCreated)
                {
                    int exStyle = unchecked((int)(long)User32.GetWindowLong(_editTextBox, User32.GWL.EXSTYLE));
                    return (exStyle & (int)User32.WS_EX.RTLREADING) != 0;
                }
                else
                {
                    return false;
                }
            }
        }

        internal DropDownHolder DropDownControlHolder => _dropDownHolder;

        internal bool DropDownVisible=> _dropDownHolder is not null && _dropDownHolder.Visible;

        public bool FocusInside => ContainsFocus || DropDownVisible;

        internal Color GrayTextColor
        {
            get
            {
                // If changed from the default, then the set value is returned.
                if (_grayTextColorModified)
                {
                    return _grayTextColor;
                }

                if (ForeColor.ToArgb() == SystemColors.WindowText.ToArgb())
                {
                    return SystemColors.GrayText;
                }

                // Compute the new color by halving the value of the old one.
                int colorRGB = ForeColor.ToArgb();

                int alphaValue = (colorRGB >> 24) & 0xff;
                if (alphaValue != 0)
                {
                    alphaValue /= 2;
                    colorRGB &= 0xFFFFFF;
                    colorRGB |= (int)((alphaValue << 24) & 0xFF000000);
                }
                else
                {
                    colorRGB /= 2;
                }

                return Color.FromArgb(colorRGB);
            }
            set
            {
                _grayTextColor = value;
                _grayTextColorModified = true;
            }
        }

        // This dialog's width is defined by the summary message
        // in the top pane. We don't restrict dialog width in any way.
        // Use caution and check at all DPI scaling factors if adding a new message
        // to be displayed in the top pane.

        private GridErrorDialog ErrorDialog => _errorDialog ??= new GridErrorDialog(OwnerGrid);

        private bool HasEntries => TopLevelGridEntries is not null && TopLevelGridEntries.Count > 0;

        private int InternalLabelWidth
        {
            get
            {
                if (_flags.HasFlag(Flags.NeedUpdateUIBasedOnFont))
                {
                    UpdateUIBasedOnFont(true);
                }

                if (_labelWidth == -1)
                {
                    SetConstants();
                }

                return _labelWidth;
            }
        }

        internal int LabelPaintMargin
        {
            set => _requiredLabelPaintMargin = (short)Math.Max(Math.Max(value, _requiredLabelPaintMargin), GdiPlusSpace);
        }

        /// <summary>
        ///  Returns 'true' if the <see cref="EditTextBox" /> has a change that needs committed.
        /// </summary>
        private bool EditTextBoxNeedsCommit
        {
            get
            {
                string text;

                if (_editTextBox is null || !EditTextBox.Visible)
                {
                    return false;
                }

                text = EditTextBox.Text;

                if ((string.IsNullOrEmpty(text) && string.IsNullOrEmpty(_originalTextValue))
                    || text.Equals(_originalTextValue))
                {
                    return false;
                }

                return true;
            }
        }

        public PropertyGrid OwnerGrid { get; private set; }

        private int RowHeight
        {
            get
            {
                if (_cachedRowHeight == -1)
                {
                    _cachedRowHeight = Font.Height + 2;
                }

                return _cachedRowHeight;
            }
        }

        /// <summary>
        ///  Returns a default location for showing the context menu.  This
        ///  location is the center of the active property label in the grid, and
        ///  is used to position the context menu when the menu is invoked
        ///  via the keyboard.
        /// </summary>
        public Point ContextMenuDefaultLocation
        {
            get
            {
                // Get the rect for the currently selected property name, find the middle.
                Rectangle rect = GetRectangle(_selectedRow, RowLabel);
                Point point = PointToScreen(new(rect.X, rect.Y));
                return new Point(point.X + (rect.Width / 2), point.Y + (rect.Height / 2));
            }
        }

        private ScrollBar ScrollBar
        {
            get
            {
                if (_scrollBar is null)
                {
                    OwnerGrid.CheckInCreate();

                    _scrollBar = new VScrollBar();
                    _scrollBar.Scroll += OnScroll;
                    Controls.Add(_scrollBar);
                }

                return _scrollBar;
            }
        }

        internal GridEntry SelectedGridEntry
        {
            get => _selectedGridEntry;
            set
            {
                if (_allGridEntries is not null)
                {
                    foreach (GridEntry entry in _allGridEntries)
                    {
                        if (entry == value)
                        {
                            SelectGridEntry(value, pageIn: true);
                            return;
                        }
                    }
                }

                GridEntry equivalentEntry = FindEquivalentGridEntry(new GridEntryCollection(new[] { value }, disposeItems: false));

                if (equivalentEntry is not null)
                {
                    SelectGridEntry(equivalentEntry, pageIn: true);
                    return;
                }

                throw new ArgumentException(SR.PropertyGridInvalidGridEntry);
            }
        }

        /// <summary>
        ///  Returns or sets the IServiceProvider the PropertyGridView will use to obtain
        ///  services.  This may be null.
        /// </summary>
        public IServiceProvider ServiceProvider
        {
            get => _serviceProvider;
            set
            {
                if (value != _serviceProvider)
                {
                    _serviceProvider = value;

                    _topHelpService = null;

                    if (_helpService is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }

                    _helpService = null;
                }
            }
        }

        /// <summary>
        ///  Indicates whether or not the control supports UIA Providers via
        ///  IRawElementProviderFragment/IRawElementProviderFragmentRoot interfaces.
        /// </summary>
        internal override bool SupportsUiaProviders => true;

        private int TipColumn
        {
            get => (_tipInfo & unchecked((int)0xFFFF0000)) >> 16;
            set
            {
                // Clear the column
                _tipInfo &= 0xFFFF;

                // Set the row
                _tipInfo |= (value & 0xFFFF) << 16;
            }
        }

        private int TipRow
        {
            get => _tipInfo & 0xFFFF;
            set
            {
                // Clear the row
                _tipInfo &= unchecked((int)0xFFFF0000);

                // Set the row
                _tipInfo |= value & 0xFFFF;
            }
        }

        private GridToolTip ToolTip
        {
            get
            {
                if (_toolTip is null)
                {
                    OwnerGrid.CheckInCreate();

                    _toolTip = new GridToolTip(new Control[] { this, EditTextBox })
                    {
                        ToolTip = string.Empty,
                        Font = Font
                    };
                }

                return _toolTip;
            }
        }

        /// <summary>
        ///  Gets the top level grid entries.
        /// </summary>
        internal GridEntryCollection TopLevelGridEntries { get; private set; }

        internal GridEntryCollection AccessibilityGetGridEntries() => GetAllGridEntries();

        internal Rectangle AccessibilityGetGridEntryBounds(GridEntry gridEntry)
        {
            int row = GetRowFromGridEntry(gridEntry);
            if (row < 0)
            {
                return Rectangle.Empty;
            }

            Rectangle rect = GetRectangle(row, RowValue | RowLabel);

            // Translate rect to screen coordinates
            var pt = new Point(rect.X, rect.Y);
            User32.ClientToScreen(new HandleRef(this, Handle), ref pt);

            Rectangle parent = gridEntry.OwnerGrid.GridViewAccessibleObject.Bounds;

            int propertyGridViewBottom = parent.Bottom - 1; // - 1 is PropertyGridView bottom border

            if (pt.Y > propertyGridViewBottom)
            {
                return Rectangle.Empty;
            }

            if (pt.Y + rect.Height > propertyGridViewBottom)
            {
                rect.Height = propertyGridViewBottom - pt.Y;
            }

            return new Rectangle(pt.X, pt.Y, rect.Width, rect.Height);
        }

        internal int AccessibilityGetGridEntryChildID(GridEntry gridEntry)
        {
            GridEntryCollection entries = GetAllGridEntries();

            if (entries is null)
            {
                return -1;
            }

            // Find the grid entry and return its ID

            for (int index = 0; index < entries.Count; ++index)
            {
                if (entries[index].Equals(gridEntry))
                {
                    return index;
                }
            }

            return -1;
        }

        internal void AccessibilitySelect(GridEntry entry)
        {
            SelectGridEntry(entry, pageIn: true);
            Focus();
        }

        private void AddGridEntryEvents(GridEntryCollection entries, int startIndex, int count)
        {
            if (entries is null)
            {
                return;
            }

            if (count == -1)
            {
                count = entries.Count - startIndex;
            }

            for (int i = startIndex; i < (startIndex + count); i++)
            {
                GridEntry entry = entries[i];
                entry.AddOnValueClick(_valueClick);
                entry.AddOnLabelClick(_labelClick);
                entry.AddOnOutlineClick(_outlineClick);
                entry.AddOnOutlineDoubleClick(_outlineClick);
                entry.AddOnValueDoubleClick(_valueDoubleClick);
                entry.AddOnLabelDoubleClick(_labelDoubleClick);
                entry.AddOnRecreateChildren(_recreateChildren);
            }
        }

        private void AdjustOrigin(Graphics g, Point newOrigin, ref Rectangle r)
        {
            Debug.WriteLineIf(
                s_gridViewDebugPaint.TraceVerbose,
                $"Adjusting paint origin to ({newOrigin.X},{newOrigin.Y})");

            g.ResetTransform();
            g.TranslateTransform(newOrigin.X, newOrigin.Y);
            r.Offset(-newOrigin.X, -newOrigin.Y);
        }

        private void CancelSplitterMove()
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:CancelSplitterMove");
            if (_flags.HasFlag(Flags.IsSplitterMove))
            {
                SetFlag(Flags.IsSplitterMove, false);
                Capture = false;

                if (_selectedRow != -1)
                {
                    SelectRow(_selectedRow);
                }
            }
        }

        internal GridPositionData CaptureGridPositionData() => new(this);

        private void ClearGridEntryEvents(GridEntryCollection entries, int startIndex, int count)
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:ClearGridEntryEvents");
            if (entries is null)
            {
                return;
            }

            if (count == -1)
            {
                count = entries.Count - startIndex;
            }

            for (int i = startIndex; i < (startIndex + count); i++)
            {
                GridEntry entry = entries[i];
                entry.RemoveOnValueClick(_valueClick);
                entry.RemoveOnLabelClick(_labelClick);
                entry.RemoveOnOutlineClick(_outlineClick);
                entry.RemoveOnOutlineDoubleClick(_outlineClick);
                entry.RemoveOnValueDoubleClick(_valueDoubleClick);
                entry.RemoveOnLabelDoubleClick(_labelDoubleClick);
                entry.RemoveOnRecreateChildren(_recreateChildren);
            }
        }

        public void ClearGridEntries()
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:ClearGridEntries");

            if (!HasEntries)
            {
                return;
            }

            CommonEditorHide();
            TopLevelGridEntries = null;
            ClearGridEntryEvents(_allGridEntries, 0, -1);
            _allGridEntries = null;
            _selectedRow = -1;

            // Don't clear selectedGridEntry because then we can't save where we were on a Refresh()
            _tipInfo = -1;
        }

        /// <summary>
        ///  Closes a previously opened drop down.  This should be called by the
        ///  drop down when the user does something that should close it.
        /// </summary>
        public void CloseDropDown() => CloseDropDownInternal(resetFocus: true);

        private void CloseDropDownInternal(bool resetFocus)
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:CloseDropDown");

            // The activation code in the DropDownHolder can cause this to recurse.

            if (_flags.HasFlag(Flags.DropDownClosing) || _dropDownHolder is null || !_dropDownHolder.Visible)
            {
                return;
            }

            try
            {
                SetFlag(Flags.DropDownClosing, true);

                if (_dropDownHolder.Component == DropDownListBox && _flags.HasFlag(Flags.DropDownCommit))
                {
                    OnListClick(null, null);
                }

                EditTextBox.Filter = false;

                // Disable the drop down holder so it won't steal the focus back.
                _dropDownHolder.SetDropDownControl(null, resizable: false);
                _dropDownHolder.Visible = false;

                // When we disable the dropdown holder focus will be lost, so put it onto one of our children.
                if (resetFocus)
                {
                    if (DialogButton.Visible)
                    {
                        DialogButton.Focus();
                    }
                    else if (DropDownButton.Visible)
                    {
                        DropDownButton.Focus();
                    }
                    else if (EditTextBox.Visible)
                    {
                        EditTextBox.Focus();
                    }
                    else
                    {
                        Focus();
                    }

                    if (_selectedRow != -1)
                    {
                        SelectRow(_selectedRow);
                    }
                }

                if (_selectedRow != -1)
                {
                    GridEntry gridEntry = GetGridEntryFromRow(_selectedRow);
                    if (gridEntry is not null)
                    {
                        gridEntry.AccessibilityObject.RaiseAutomationEvent(UiaCore.UIA.AutomationFocusChangedEventId);
                        gridEntry.AccessibilityObject.RaiseAutomationPropertyChangedEvent(
                            UiaCore.UIA.ExpandCollapseExpandCollapseStatePropertyId,
                            UiaCore.ExpandCollapseState.Expanded,
                            UiaCore.ExpandCollapseState.Collapsed);
                    }
                }
            }
            finally
            {
                SetFlag(Flags.DropDownClosing, false);
            }
        }

        private void CommonEditorHide(bool always = false)
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:CommonEditorHide");

            if (!always && !HasEntries)
            {
                return;
            }

            CloseDropDown();

            bool gotFocus = false;

            if (EditTextBox.Focused || DialogButton.Focused || DropDownButton.Focused)
            {
                if (IsHandleCreated && Visible && Enabled)
                {
                    gotFocus = User32.SetFocus(new HandleRef(this, Handle)) != IntPtr.Zero;
                }
            }

            try
            {
                // We do this because the Focus call above doesn't always stick, so
                // we make the EditTextBox think that it doesn't have focus. This prevents
                // ActiveControl code on the container control from moving focus elsewhere
                // when the dropdown closes.

                EditTextBox.HideFocusState = true;
                if (EditTextBox.Focused && !gotFocus)
                {
                    gotFocus = Focus();
                }

                EditTextBox.Visible = false;

                EditTextBox.SelectionStart = 0;
                EditTextBox.SelectionLength = 0;

                if (DialogButton.Focused && !gotFocus)
                {
                    gotFocus = Focus();
                }

                DialogButton.Visible = false;

                if (DropDownButton.Focused && !gotFocus)
                {
                    gotFocus = Focus();
                }

                DropDownButton.Visible = false;
                _currentEditor = null;
            }
            finally
            {
                EditTextBox.HideFocusState = false;
            }
        }

        private void CommonEditorSetup(Control control)
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:CommonEditorSetup");
            control.Visible = false;
            Controls.Add(control);
        }

        private void CommonEditorUse(Control control, Rectangle targetRectangle)
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:CommonEditorUse");
            Debug.WriteLineIf(s_gridViewDebugPaint.TraceVerbose, "Showing common editors");

            Debug.Assert(control is not null, "Null control passed to CommonEditorUse");

            Rectangle rectCur = control.Bounds;

            // The client rect minus the border line.
            Rectangle clientRect = ClientRectangle;

            clientRect.Inflate(-1, -1);

            try
            {
                targetRectangle = Rectangle.Intersect(clientRect, targetRectangle);

                if (!targetRectangle.IsEmpty)
                {
                    if (!targetRectangle.Equals(rectCur))
                    {
                        control.SetBounds(targetRectangle.X, targetRectangle.Y, targetRectangle.Width, targetRectangle.Height);
                    }

                    control.Visible = true;
                }
            }
            catch
            {
                targetRectangle = Rectangle.Empty;
            }

            if (targetRectangle.IsEmpty)
            {
                control.Visible = false;
            }

            _currentEditor = control;
        }

        private int CountPropsFromOutline(GridEntryCollection entries)
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:CountPropsFromOutLine");
            if (entries is null)
            {
                return 0;
            }

            int cProps = entries.Count;
            for (int i = 0; i < entries.Count; i++)
            {
                if (((GridEntry)entries[i]).InternalExpanded)
                {
                    cProps += CountPropsFromOutline(((GridEntry)entries[i]).Children);
                }
            }

            return cProps;
        }

        /// <summary>
        ///  Constructs the new instance of the accessibility object for this control. Subclasses
        ///  should not call base.CreateAccessibilityObject.
        /// </summary>
        protected override AccessibleObject CreateAccessibilityInstance()
            => new PropertyGridViewAccessibleObject(this, OwnerGrid);

        private Bitmap CreateResizedBitmap(string icon, int width, int height)
        {
            Bitmap bitmap;
            int scaledIconWidth = width;
            int scaledIconHeight = height;
            try
            {
                // Scale for per-monitor DPI.
                if (DpiHelper.IsPerMonitorV2Awareness)
                {
                    scaledIconWidth = LogicalToDeviceUnits(width);
                    scaledIconHeight = LogicalToDeviceUnits(height);
                }
                else if (DpiHelper.IsScalingRequired)
                {
                    // Only primary monitor scaling.
                    scaledIconWidth = DpiHelper.LogicalToDeviceUnitsX(width);
                    scaledIconHeight = DpiHelper.LogicalToDeviceUnitsY(height);
                }

                bitmap = GetBitmapFromIcon(icon, scaledIconWidth, scaledIconHeight);
            }
            catch (Exception e)
            {
                Debug.Fail(e.ToString());
                bitmap = new Bitmap(scaledIconWidth, scaledIconHeight);
            }

            return bitmap;
        }

        private void CreateUI() => UpdateUIBasedOnFont(layoutRequired: false);

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:Dispose");
                _scrollBar?.Dispose();
                _listBox?.Dispose();
                _dropDownHolder?.Dispose();

                _scrollBar = null;
                _listBox = null;
                _dropDownHolder = null;

                OwnerGrid = null;
                TopLevelGridEntries = null;
                _allGridEntries = null;
                _serviceProvider = null;

                _topHelpService = null;

                if (_helpService is not null && _helpService is IDisposable disposable)
                {
                    disposable.Dispose();
                }

                _helpService = null;

                _editTextBox?.Dispose();
                _editTextBox = null;

                _boldFont?.Dispose();
                _boldFont = null;

                _dropDownButton?.Dispose();
                _dropDownButton = null;

                _dialogButton?.Dispose();
                _dialogButton = null;

                _toolTip?.Dispose();
                _toolTip = null;
            }

            base.Dispose(disposing);
        }

        public void DoCopyCommand()
        {
            if (CanCopy)
            {
                if (EditTextBox.Focused)
                {
                    EditTextBox.Copy();
                }
                else if (_selectedGridEntry is not null)
                {
                    Clipboard.SetDataObject(_selectedGridEntry.GetPropertyTextValue());
                }
            }
        }

        public void DoCutCommand()
        {
            if (CanCut)
            {
                DoCopyCommand();
                if (EditTextBox.Visible)
                {
                    EditTextBox.Cut();
                }
            }
        }

        public void DoPasteCommand()
        {
            if (!CanPaste || !EditTextBox.Visible)
            {
                return;
            }

            if (EditTextBox.Focused)
            {
                EditTextBox.Paste();
            }
            else
            {
                IDataObject dataObj = Clipboard.GetDataObject();
                if (dataObj is not null)
                {
                    string data = (string)dataObj.GetData(typeof(string));
                    if (data is not null)
                    {
                        EditTextBox.Focus();
                        EditTextBox.Text = data;
                        SetCommitError(ErrorState.None, capture: true);
                    }
                }
            }
        }

        public void DoUndoCommand()
        {
            if (CanUndo && EditTextBox.Visible)
            {
                User32.SendMessageW(EditTextBox, User32.WM.UNDO);
            }
        }

        private int GetEntryLabelIndent(GridEntry gridEntry) => gridEntry.PropertyLabelIndent + 1;

        private int GetEntryLabelLength(Graphics g, GridEntry gridEntry)
        {
            SizeF sizeF = PropertyGrid.MeasureTextHelper.MeasureText(OwnerGrid, g, gridEntry.PropertyLabel, Font);
            var size = Size.Ceiling(sizeF);
            return _location.X + GetEntryLabelIndent(gridEntry) + size.Width;
        }

        private bool IsEntryLabelLong(Graphics g, GridEntry gridEntry)
        {
            if (gridEntry is null)
            {
                return false;
            }

            int length = GetEntryLabelLength(g, gridEntry);
            return length > _location.X + InternalLabelWidth;
        }

        private void DrawLabel(Graphics g, int row, Rectangle rect, bool selected, bool longLabelrequest, Rectangle clipRect)
        {
            GridEntry gridEntry = GetGridEntryFromRow(row);

            if (gridEntry is null || rect.IsEmpty)
            {
                return;
            }

            Debug.WriteLineIf(s_gridViewDebugPaint.TraceVerbose, $"Drawing label for property {gridEntry.PropertyLabel}");

            var newOrigin = new Point(rect.X, rect.Y);
            clipRect = Rectangle.Intersect(rect, clipRect);

            if (clipRect.IsEmpty)
            {
                return;
            }

            AdjustOrigin(g, newOrigin, ref rect);
            clipRect.Offset(-newOrigin.X, -newOrigin.Y);

            try
            {
                try
                {
                    bool fLongLabel = false;
                    int labelEnd = 0;
                    int labelIndent = GetEntryLabelIndent(gridEntry);

                    if (longLabelrequest)
                    {
                        labelEnd = GetEntryLabelLength(g, gridEntry);
                        fLongLabel = IsEntryLabelLong(g, gridEntry);
                    }

                    gridEntry.PaintLabel(g, rect, clipRect, selected, fLongLabel);
                }
                catch (Exception ex)
                {
                    Debug.Fail(ex.ToString());
                }
            }
            finally
            {
                ResetOrigin(g);
            }
        }

        private void DrawValueEntry(Graphics g, int row, Rectangle clipRect)
        {
            GridEntry gridEntry = GetGridEntryFromRow(row);
            if (gridEntry is null)
            {
                return;
            }

            Debug.WriteLineIf(s_gridViewDebugPaint.TraceVerbose, $"Drawing value for property {gridEntry.PropertyLabel}");

            Rectangle rect = GetRectangle(row, RowValue);
            var newOrigin = new Point(rect.X, rect.Y);
            clipRect = Rectangle.Intersect(clipRect, rect);

            if (clipRect.IsEmpty)
            {
                return;
            }

            AdjustOrigin(g, newOrigin, ref rect);
            clipRect.Offset(-newOrigin.X, -newOrigin.Y);

            try
            {
                try
                {
                    gridEntry.PaintValue(
                        null,
                        g,
                        rect,
                        clipRect,
                        GridEntry.PaintValueFlags.FetchValue
                            | GridEntry.PaintValueFlags.PaintInPlace
                            | GridEntry.PaintValueFlags.CheckShouldSerialize);
                }
                catch (Exception ex)
                {
                    Debug.Fail(ex.ToString());
                }
            }
            finally
            {
                ResetOrigin(g);
            }
        }

        private void F4Selection(bool popupModalDialog)
        {
            GridEntry gridEntry = GetGridEntryFromRow(_selectedRow);
            if (gridEntry is null)
            {
                return;
            }

            // If we are in an errorState, just put the focus back on the Edit.
            if (_errorState != ErrorState.None && EditTextBox.Visible)
            {
                EditTextBox.Focus();
                return;
            }

            if (DropDownButton.Visible)
            {
                PopupDialog(_selectedRow);
            }
            else if (DialogButton.Visible)
            {
                if (popupModalDialog)
                {
                    PopupDialog(_selectedRow);
                }
                else
                {
                    DialogButton.Focus();
                }
            }
            else if (EditTextBox.Visible)
            {
                EditTextBox.Focus();
                EditTextBox.SelectAll();
            }

            return;
        }

        public void DoubleClickRow(int row, bool toggleExpand, int type)
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:DoubleClickRow");
            GridEntry gridEntry = GetGridEntryFromRow(row);
            if (gridEntry is null)
            {
                return;
            }

            Debug.WriteLineIf(s_gridViewDebugPaint.TraceVerbose, $"Property {gridEntry.PropertyLabel} double clicked");

            if (!toggleExpand || type == RowValue)
            {
                try
                {
                    bool action = gridEntry.DoubleClickPropertyValue();
                    if (action)
                    {
                        SelectRow(row);
                        return;
                    }
                }
                catch (Exception ex)
                {
                    SetCommitError(ErrorState.Thrown);
                    ShowInvalidMessage(gridEntry.PropertyLabel, ex);
                    return;
                }
            }

            SelectGridEntry(gridEntry, pageIn: true);

            if (type == RowLabel && toggleExpand && gridEntry.Expandable)
            {
                SetExpand(gridEntry, !gridEntry.InternalExpanded);
                return;
            }

            if (gridEntry.IsValueEditable && gridEntry.Enumerable)
            {
                int index = GetCurrentValueIndex(gridEntry);

                if (index != -1)
                {
                    object[] values = gridEntry.GetPropertyValueList();

                    if (values is null || index >= (values.Length - 1))
                    {
                        index = 0;
                    }
                    else
                    {
                        index++;
                    }

                    CommitValue(values[index]);
                    SelectRow(_selectedRow);
                    Refresh();
                    return;
                }
            }

            if (EditTextBox.Visible)
            {
                EditTextBox.Focus();
                EditTextBox.SelectAll();
                return;
            }
        }

        public Font GetBaseFont() => Font;

        public Font GetBoldFont() => _boldFont ??= new Font(Font, FontStyle.Bold);

        /// <summary>
        ///  Gets the element from point.
        /// </summary>
        /// <param name="x">The point x coordinate.</param>
        /// <param name="y">The point y coordinate.</param>
        /// <returns>The found grid element.</returns>
        internal GridEntry GetElementFromPoint(int x, int y)
        {
            var point = new Point(x, y);
            GridEntryCollection allGridEntries = GetAllGridEntries();
            var targetEntries = new GridEntry[allGridEntries.Count];
            try
            {
                GetGridEntriesFromOutline(allGridEntries, 0, allGridEntries.Count - 1, targetEntries);
            }
            catch (Exception ex)
            {
                Debug.Fail(ex.ToString());
            }

            foreach (GridEntry gridEntry in targetEntries)
            {
                if (gridEntry.AccessibilityObject.Bounds.Contains(point))
                {
                    return gridEntry;
                }
            }

            return null;
        }

        public Color GetLineColor() => OwnerGrid.LineColor;

        public Color GetSelectedItemWithFocusForeColor() => OwnerGrid.SelectedItemWithFocusForeColor;

        public Color GetSelectedItemWithFocusBackColor() => OwnerGrid.SelectedItemWithFocusBackColor;

        public IntPtr GetHostHandle() => Handle;

        public int GetLabelWidth() => InternalLabelWidth;

        internal bool IsExplorerTreeSupported => OwnerGrid.CanShowVisualStyleGlyphs && VisualStyleRenderer.IsSupported;

        public int GetOutlineIconSize() => IsExplorerTreeSupported ? _outlineSizeExplorerTreeStyle : _outlineSize;

        public int GetGridEntryHeight() => RowHeight;

        internal int GetPropertyLocation(string propName, bool getXY, bool rowValue)
        {
            if (_allGridEntries is null || _allGridEntries.Count <= 0)
            {
                return -1;
            }

            for (int i = 0; i < _allGridEntries.Count; i++)
            {
                if (0 == string.Compare(propName, _allGridEntries[i].PropertyLabel, true, CultureInfo.InvariantCulture))
                {
                    if (getXY)
                    {
                        int row = GetRowFromGridEntry(_allGridEntries[i]);

                        if (row < 0 || row >= _visibleRows)
                        {
                            return -1;
                        }
                        else
                        {
                            Rectangle r = GetRectangle(row, rowValue ? RowValue : RowLabel);
                            return PARAM.ToInt(r.X, r.Y);
                        }
                    }
                    else
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        public new object GetService(Type classService)
        {
            if (classService == typeof(IWindowsFormsEditorService))
            {
                return this;
            }

            if (ServiceProvider is not null)
            {
                return _serviceProvider.GetService(classService);
            }

            return null;
        }

        public int GetSplitterWidth() => 1;

        public int GetTotalWidth() => GetLabelWidth() + GetSplitterWidth() + GetValueWidth();

        public int GetValuePaintIndent() => _paintIndent;

        public int GetValuePaintWidth() => _paintWidth;

        public int GetValueStringIndent() => EditIndent;

        public int GetValueWidth() => (int)(InternalLabelWidth * (_labelRatio - 1));

        /// <summary>
        ///  Modally displays the provided control in a drop down.
        /// </summary>
        /// <remarks>
        ///  <para>
        ///   When possible, the current dimensions of the control will be respected. If this is not possible for the
        ///   current screen layout the control may be resized, so it should be implemented using appropriate docking
        ///   and anchoring so it will resize nicely. If the user performs an action that would cause the drop down to
        ///   prematurely disappear the control will be hidden.
        ///  </para>
        /// </remarks>
        public void DropDownControl(Control control)
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:DropDownControl");
            Debug.WriteLineIf(s_gridViewDebugPaint.TraceVerbose, $"DropDownControl(ctl = {control.GetType().Name})");

            _dropDownHolder ??= new(this);
            _dropDownHolder.Visible = false;
            _dropDownHolder.SetDropDownControl(control, _flags.HasFlag(Flags.ResizableDropDown));

            Rectangle rect = GetRectangle(_selectedRow, RowValue);
            Size size = _dropDownHolder.Size;
            Point location = PointToScreen(new Point(0, 0));
            Rectangle rectScreen = Screen.FromControl(EditTextBox).WorkingArea;
            size.Width = Math.Max(rect.Width + 1, size.Width);

            location.X = Math.Min(
                rectScreen.X + rectScreen.Width - size.Width,
                Math.Max(rectScreen.X, location.X + rect.X + rect.Width - size.Width));

            location.Y += rect.Y;
            if (rectScreen.Y + rectScreen.Height < (size.Height + location.Y + EditTextBox.Height))
            {
                location.Y -= size.Height;
                _dropDownHolder.ResizeUp = true;
            }
            else
            {
                location.Y += rect.Height + 1;
                _dropDownHolder.ResizeUp = false;
            }

            // Control is a top level window. Standard way of setting parent on the control is prohibited for top-level controls.
            // It is unknown why this control was created as a top-level control. Windows does not recommend this way of setting parent.
            // We are not touching this for this release. We may revisit it in next release.

            User32.SetWindowLong(_dropDownHolder, User32.GWL.HWNDPARENT, new HandleRef(this, Handle));
            _dropDownHolder.SetBounds(location.X, location.Y, size.Width, size.Height);
            User32.ShowWindow(_dropDownHolder, User32.SW.SHOWNA);
            EditTextBox.Filter = true;
            _dropDownHolder.Visible = true;
            _dropDownHolder.FocusComponent();
            EditTextBox.SelectAll();

            var gridEntry = GetGridEntryFromRow(_selectedRow);
            if (gridEntry is not null)
            {
                gridEntry.AccessibilityObject.RaiseAutomationEvent(UiaCore.UIA.AutomationFocusChangedEventId);
                gridEntry.AccessibilityObject.RaiseAutomationPropertyChangedEvent(
                    UiaCore.UIA.ExpandCollapseExpandCollapseStatePropertyId,
                    UiaCore.ExpandCollapseState.Collapsed,
                    UiaCore.ExpandCollapseState.Expanded);
            }

            try
            {
                DropDownButton.IgnoreMouse = true;
                _dropDownHolder.DoModalLoop();
            }
            finally
            {
                DropDownButton.IgnoreMouse = false;
            }

            if (_selectedRow != -1)
            {
                Focus();
                SelectRow(_selectedRow);
            }
        }

        public void DropDownUpdate()
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "DropDownHolder:DropDownUpdate");
            if (_dropDownHolder is not null && _dropDownHolder.GetUsed())
            {
                int row = _selectedRow;
                GridEntry gridEntry = GetGridEntryFromRow(row);
                EditTextBox.Text = gridEntry.GetPropertyTextValue();
            }
        }

        public bool EnsurePendingChangesCommitted()
        {
            CloseDropDown();
            return CommitEditTextBox();
        }

        private bool FilterEditWndProc(ref Message m)
        {
            // If it's the TAB key, we keep it since we'll give them focus with it.
            if (_dropDownHolder is not null && _dropDownHolder.Visible && m.Msg == (int)User32.WM.KEYDOWN && (int)m.WParam != (int)Keys.Tab)
            {
                Control ctl = _dropDownHolder.Component;
                if (ctl is not null)
                {
                    m.Result = User32.SendMessageW(ctl, (User32.WM)m.Msg, m.WParam, m.LParam);
                    return true;
                }
            }

            return false;
        }

        private bool FilterReadOnlyEditKeyPress(char keyChar)
        {
            GridEntry gridEntry = GetGridEntryFromRow(_selectedRow);
            if (gridEntry.Enumerable && gridEntry.IsValueEditable)
            {
                int index = GetCurrentValueIndex(gridEntry);

                object[] values = gridEntry.GetPropertyValueList();
                string letter = new(new char[] { keyChar });
                for (int i = 0; i < values.Length; i++)
                {
                    object valueCur = values[(i + index + 1) % values.Length];
                    string text = gridEntry.GetPropertyTextValue(valueCur);
                    if (text is not null && text.Length > 0 && string.Compare(text.Substring(0, 1), letter, true, CultureInfo.InvariantCulture) == 0)
                    {
                        CommitValue(valueCur);
                        if (EditTextBox.Focused)
                        {
                            EditTextBox.SelectAll();
                        }

                        return true;
                    }
                }
            }

            return false;
        }

        public bool WillFilterKeyPress(char charPressed)
        {
            if (!EditTextBox.Visible)
            {
                return false;
            }

            Keys modifiers = ModifierKeys;
            if ((modifiers & ~Keys.Shift) != 0)
            {
                return false;
            }

            // Try to activate the Edit.
            // We don't activate for +, -, or * on expandable items because they have special meaning for the tree.

            if (_selectedGridEntry is not null)
            {
                switch (charPressed)
                {
                    case '+':
                    case '-':
                    case '*':
                        return !_selectedGridEntry.Expandable;
                    case unchecked((char)(int)(long)Keys.Tab):
                        return false;
                }
            }

            return true;
        }

        public void FilterKeyPress(char keyChar)
        {
            GridEntry gridEntry = GetGridEntryFromRow(_selectedRow);
            if (gridEntry is null)
            {
                return;
            }

            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:FilterKeyPress()");

            EditTextBox.FilterKeyPress(keyChar);
        }

        private GridEntry FindEquivalentGridEntry(GridEntryCollection gridEntries)
        {
            if (gridEntries is null || gridEntries.Count == 0)
            {
                return null;
            }

            GridEntryCollection allGridEntries = GetAllGridEntries();

            if (allGridEntries is null || allGridEntries.Count == 0)
            {
                return null;
            }

            GridEntry targetEntry = null;
            int row = 0;
            int count = allGridEntries.Count;

            for (int i = 0; i < gridEntries.Count; i++)
            {
                if (gridEntries[i] is null)
                {
                    continue;
                }

                // If we've got one above, and it's expandable, expand it.
                if (targetEntry is not null)
                {
                    // Expand and get the new count.
                    if (!targetEntry.InternalExpanded)
                    {
                        SetExpand(targetEntry, true);
                        allGridEntries = GetAllGridEntries();
                    }

                    count = targetEntry.VisibleChildCount;
                }

                int start = row;
                targetEntry = null;

                // Now, we will only go as many as were expanded.
                for (; row < allGridEntries.Count && ((row - start) <= count); row++)
                {
                    if (gridEntries[i].NonParentEquals(allGridEntries[row]))
                    {
                        targetEntry = allGridEntries[row];
                        row++;
                        break;
                    }
                }

                // Didn't find it.
                if (targetEntry is null)
                {
                    break;
                }
            }

            return targetEntry;
        }

        private Point FindPosition(int x, int y)
        {
            if (RowHeight == -1)
            {
                return InvalidPosition;
            }

            Size size = GetOurSize();

            if (x < 0 || x > size.Width + _location.X)
            {
                return InvalidPosition;
            }

            var pt = new Point(RowLabel, 0);
            if (x > InternalLabelWidth + _location.X)
            {
                pt.X = RowValue;
            }

            pt.Y = (y - _location.Y) / (1 + RowHeight);
            return pt;
        }

        public void Flush()
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView::Flush()");
            if (CommitEditTextBox() && EditTextBox.Focused)
            {
                Focus();
            }
        }

        private GridEntryCollection GetAllGridEntries()
        {
            return GetAllGridEntries(updateCache: false);
        }

        private GridEntryCollection GetAllGridEntries(bool updateCache)
        {
            if (_visibleRows == -1 || TotalProperties == -1 || !HasEntries)
            {
                return null;
            }

            if (_allGridEntries is not null && !updateCache)
            {
                return _allGridEntries;
            }

            var newEntries = new GridEntry[TotalProperties];
            try
            {
                GetGridEntriesFromOutline(TopLevelGridEntries, 0, 0, newEntries);
            }
            catch (Exception ex)
            {
                Debug.Fail(ex.ToString());
            }

            _allGridEntries = new GridEntryCollection(newEntries, disposeItems: false);
            AddGridEntryEvents(_allGridEntries, 0, -1);
            return _allGridEntries;
        }

        private int GetCurrentValueIndex(GridEntry gridEntry)
        {
            if (!gridEntry.Enumerable)
            {
                return -1;
            }

            try
            {
                object[] values = gridEntry.GetPropertyValueList();
                object value = gridEntry.PropertyValue;
                string textValue = gridEntry.TypeConverter.ConvertToString(gridEntry, value);

                if (values is null || values.Length == 0)
                {
                    return -1;
                }

                string itemTextValue;
                int stringMatch = -1;
                int equalsMatch = -1;
                for (int i = 0; i < values.Length; i++)
                {
                    object currentValue = values[i];

                    // Check real values against string values.
                    itemTextValue = gridEntry.TypeConverter.ConvertToString(currentValue);
                    if (value == currentValue || 0 == string.Compare(textValue, itemTextValue, true, CultureInfo.InvariantCulture))
                    {
                        stringMatch = i;
                    }

                    // Now try .equals if they are both non-null
                    if (value is not null && currentValue is not null && currentValue.Equals(value))
                    {
                        equalsMatch = i;
                    }

                    if (stringMatch == equalsMatch && stringMatch != -1)
                    {
                        return stringMatch;
                    }
                }

                if (stringMatch != -1)
                {
                    return stringMatch;
                }

                if (equalsMatch != -1)
                {
                    return equalsMatch;
                }

                return -1;
            }
            catch (Exception e)
            {
                Debug.Fail(e.ToString());
            }

            return -1;
        }

        public int GetDefaultOutlineIndent()
        {
            return OutlineIndent;
        }

        private IHelpService GetHelpService()
        {
            if (_helpService is null && ServiceProvider.TryGetService(out _topHelpService!))
            {
                IHelpService localHelpService = _topHelpService.CreateLocalContext(HelpContextType.ToolWindowSelection);
                if (localHelpService is not null)
                {
                    _helpService = localHelpService;
                }
            }

            return _helpService;
        }

        public int GetScrollOffset()
        {
            if (_scrollBar is null)
            {
                return 0;
            }

            int pos = ScrollBar.Value;
            return pos;
        }

        /// <summary>
        ///  Returns an array of entries specifying the current heirarchy of entries from the given
        ///  <paramref name="gridEntry"/> through its parents to the root.
        /// </summary>
        private GridEntryCollection GetGridEntryHierarchy(GridEntry gridEntry)
        {
            if (gridEntry is null)
            {
                return null;
            }

            int depth = gridEntry.PropertyDepth;
            if (depth > 0)
            {
                var entries = new GridEntry[depth + 1];

                while (gridEntry is not null && depth >= 0)
                {
                    entries[depth] = gridEntry;
                    gridEntry = gridEntry.ParentGridEntry;
                    depth = gridEntry.PropertyDepth;
                }

                return new GridEntryCollection(entries, disposeItems: false);
            }

            return new GridEntryCollection(new GridEntry[] { gridEntry }, disposeItems: false);
        }

        private GridEntry GetGridEntryFromRow(int row) => GetGridEntryFromOffset(row + GetScrollOffset());

        private GridEntry GetGridEntryFromOffset(int offset)
        {
            GridEntryCollection allGridEntries = GetAllGridEntries();
            if (allGridEntries is not null)
            {
                if (offset >= 0 && offset < allGridEntries.Count)
                {
                    return allGridEntries[offset];
                }
            }

            return null;
        }

        private int GetGridEntriesFromOutline(GridEntryCollection entries, int current, int target, GridEntry[] targetEntries)
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:GetGridEntriesFromOutline");
            if (entries is null || entries.Count == 0)
            {
                return current;
            }

            current--; // Want to account for each entry as we find it.

            for (int i = 0; i < entries.Count; i++)
            {
                current++;
                if (current >= target + targetEntries.Length)
                {
                    break;
                }

                GridEntry currentEntry = entries[i];
                if (current >= target)
                {
                    targetEntries[current - target] = currentEntry;
                }

                if (currentEntry.InternalExpanded)
                {
                    GridEntryCollection childEntries = currentEntry.Children;
                    if (childEntries is not null && childEntries.Count > 0)
                    {
                        current = GetGridEntriesFromOutline(childEntries, current + 1, target, targetEntries);
                    }
                }
            }

            return current;
        }

        private Size GetOurSize()
        {
            Size size = ClientSize;
            if (size.Width == 0)
            {
                Size sizeWindow = Size;
                if (sizeWindow.Width > 10)
                {
                    Debug.Fail("We have a bad client width!");
                    size.Width = sizeWindow.Width;
                    size.Height = sizeWindow.Height;
                }
            }

            if (!GetScrollbarHidden())
            {
                Size sizeScroll = ScrollBar.Size;
                size.Width -= sizeScroll.Width;
            }

            size.Width -= 2;
            size.Height -= 2;
            return size;
        }

        public Rectangle GetRectangle(int row, int flRow)
        {
            var rect = new Rectangle(0, 0, 0, 0);
            Size size = GetOurSize();

            rect.X = _location.X;

            bool fLabel = (flRow & RowLabel) != 0;
            bool fValue = (flRow & RowValue) != 0;

            if (fLabel && fValue)
            {
                rect.X = 1;
                rect.Width = size.Width - 1;
            }
            else if (fLabel)
            {
                rect.X = 1;
                rect.Width = InternalLabelWidth - 1;
            }
            else if (fValue)
            {
                rect.X = _location.X + InternalLabelWidth;
                rect.Width = size.Width - InternalLabelWidth;
            }

            rect.Y = row * (RowHeight + 1) + 1 + _location.Y;
            rect.Height = RowHeight;

            return rect;
        }

        internal int GetRowFromGridEntry(GridEntry gridEntry)
        {
            GridEntryCollection allGridEntries = GetAllGridEntries();
            if (gridEntry is null || allGridEntries is null)
            {
                return -1;
            }

            int bestMatch = -1;

            for (int i = 0; i < allGridEntries.Count; i++)
            {
                // Try for an exact match. Semantics of equals are a bit loose here.

                if (gridEntry == allGridEntries[i])
                {
                    return i - GetScrollOffset();
                }
                else if (bestMatch == -1 && gridEntry.Equals(allGridEntries[i]))
                {
                    bestMatch = i - GetScrollOffset();
                }
            }

            if (bestMatch != -1)
            {
                return bestMatch;
            }

            return -1 - GetScrollOffset();
        }

        public bool InPropertySet
        {
            get => _flags.HasFlag(Flags.InPropertySet);
            private set => SetFlag(Flags.InPropertySet, value);
        }

        private bool GetScrollbarHidden() => _scrollBar is null || !ScrollBar.Visible;

        /// <summary>
        ///  Returns a string containing test info about a given GridEntry. Requires an offset into the top-level
        ///  entry collection (ie. nested entries are not accessible). Or specify -1 to get info for the current
        ///  selected entry (which can be any entry, top-level or nested).
        /// </summary>
        public string GetTestingInfo(int entry)
        {
            GridEntry gridEntry = (entry < 0) ? GetGridEntryFromRow(_selectedRow) : GetGridEntryFromOffset(entry);

            return gridEntry is null ? string.Empty : gridEntry.GetTestingInfo();
        }

        public Color GetTextColor() => ForeColor;

        private void LayoutWindow(bool invalidate)
        {
            Rectangle rect = ClientRectangle;
            var sizeWindow = new Size(rect.Width, rect.Height);

            if (_scrollBar is not null)
            {
                Rectangle boundsScroll = ScrollBar.Bounds;
                boundsScroll.X = sizeWindow.Width - boundsScroll.Width - 1;
                boundsScroll.Y = 1;
                boundsScroll.Height = sizeWindow.Height - 2;
                ScrollBar.Bounds = boundsScroll;
            }

            if (invalidate)
            {
                Invalidate();
            }
        }

        internal void InvalidateGridEntryValue(GridEntry ge)
        {
            int row = GetRowFromGridEntry(ge);
            if (row != -1)
            {
                InvalidateRows(row, row, RowValue);
            }
        }

        private void InvalidateRow(int row) => InvalidateRows(row, row, RowValue | RowLabel);

        private void InvalidateRows(int startRow, int endRow) => InvalidateRows(startRow, endRow, RowValue | RowLabel);

        private void InvalidateRows(int startRow, int endRow, int type)
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:InvalidateRows");

            Debug.WriteLineIf(s_gridViewDebugPaint.TraceVerbose, $"Invalidating rows {startRow} through {endRow}");
            Rectangle rect;

            // Invalidate from the start row down.
            if (endRow == -1)
            {
                rect = GetRectangle(startRow, type);
                rect.Height = Size.Height - rect.Y - 1;
                Invalidate(rect);
            }
            else
            {
                for (int i = startRow; i <= endRow; i++)
                {
                    rect = GetRectangle(i, type);
                    Invalidate(rect);
                }
            }
        }

        /// <summary>
        ///  Overridden to handle TAB key.
        /// </summary>
        protected override bool IsInputKey(Keys keyData)
        {
            switch (keyData & Keys.KeyCode)
            {
                case Keys.Escape:
                case Keys.Tab:
                case Keys.F4:
                    return false;

                case Keys.Return:
                    if (EditTextBox.Focused)
                    {
                        return false;
                    }

                    break;
            }

            return base.IsInputKey(keyData);
        }

        private bool IsMyChild(Control control)
        {
            if (control == this || control is null)
            {
                return false;
            }

            Control parent = control.ParentInternal;

            while (parent is not null)
            {
                if (parent == this)
                {
                    return true;
                }

                parent = parent.ParentInternal;
            }

            return false;
        }

        private bool IsScrollValueValid(int newValue)
        {
            // Is this move valid?
            if (newValue == ScrollBar.Value ||
                newValue < 0 ||
                newValue > ScrollBar.Maximum ||
                (newValue + (ScrollBar.LargeChange - 1) >= TotalProperties))
            {
                Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView: move not needed, returning");
                return false;
            }

            return true;
        }

        internal bool IsSiblingControl(Control control1, Control control2)
        {
            Control parent1 = control1.ParentInternal;
            Control parent2 = control2.ParentInternal;

            while (parent2 is not null)
            {
                if (parent1 == parent2)
                {
                    return true;
                }

                parent2 = parent2.ParentInternal;
            }

            return false;
        }

        private void MoveSplitterTo(int xPosition)
        {
            int widthPS = GetOurSize().Width;
            int startPS = _location.X;
            int pos = Math.Max(Math.Min(xPosition, widthPS - 10), GetOutlineIconSize() * 2);

            int oldLabelWidth = InternalLabelWidth;

            _labelRatio = widthPS / (double)(pos - startPS);

            SetConstants();

            if (_selectedRow != -1)
            {
                // Do this to move any editor we have.
                SelectRow(_selectedRow);
            }

            Rectangle r = ClientRectangle;

            // If we're moving to the left, just invalidate the values.
            if (oldLabelWidth > InternalLabelWidth)
            {
                int left = InternalLabelWidth - _requiredLabelPaintMargin;
                Invalidate(new Rectangle(left, 0, Size.Width - left, Size.Height));
            }
            else
            {
                // To the right, just invalidate from where the splitter was.
                r.X = oldLabelWidth - _requiredLabelPaintMargin;
                r.Width -= r.X;
                Invalidate(r);
            }
        }

        /// <summary>
        ///  Shared click handler for the dialog and drop-down button.
        /// </summary>
        private void OnButtonClick(object sender, EventArgs e)
        {
            if (_flags.HasFlag(Flags.BtnLaunchedEditor))
            {
                return;
            }

            if (sender == DialogButton && !CommitEditTextBox())
            {
                return;
            }

            SetCommitError(ErrorState.None);

            try
            {
                CommitEditTextBox();
                SetFlag(Flags.BtnLaunchedEditor, true);
                PopupDialog(_selectedRow);
            }
            finally
            {
                SetFlag(Flags.BtnLaunchedEditor, false);
            }
        }

        private void OnButtonKeyDown(object sender, KeyEventArgs ke) => OnKeyDown(sender, ke);

        private void OnChildLostFocus(object sender, EventArgs e)
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:OnChildLostFocus");
            InvokeLostFocus(this, e);
        }

        private void OnDropDownButtonGotFocus(object sender, EventArgs e)
        {
            if (sender is DropDownButton dropDownButton)
            {
                dropDownButton.AccessibilityObject.SetFocus();
            }
        }

        protected override void OnGotFocus(EventArgs e)
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:OnGotFocus");

            base.OnGotFocus(e);

            if (e is not null && !InPropertySet)
            {
                if (!CommitEditTextBox())
                {
                    EditTextBox.Focus();
                    return;
                }
            }

            if (_selectedGridEntry is not null && GetRowFromGridEntry(_selectedGridEntry) != -1)
            {
                _selectedGridEntry.HasFocus = true;
                SelectGridEntry(_selectedGridEntry, pageIn: false);
            }
            else
            {
                SelectRow(0);
            }

            if (_selectedGridEntry is not null && _selectedGridEntry.GetValueOwner() is not null)
            {
                UpdateHelpAttributes(null, _selectedGridEntry);
            }

            // For empty GridView, draw a focus-indicator rectangle, just inside GridView borders.
            if (TotalProperties <= 0)
            {
                int doubleOffset = 2 * _offset2Units;

                if ((Size.Width > doubleOffset) && (Size.Height > doubleOffset))
                {
                    using Graphics g = CreateGraphicsInternal();
                    ControlPaint.DrawFocusRectangle(g, new Rectangle(_offset2Units, _offset2Units, Size.Width - doubleOffset, Size.Height - doubleOffset));
                }
            }
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);
            SystemEvents.UserPreferenceChanged += OnSysColorChange;
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            SystemEvents.UserPreferenceChanged -= OnSysColorChange;

            // We can leak this if we aren't disposed.
            if (_toolTip is not null && !RecreatingHandle)
            {
                _toolTip.Dispose();
                _toolTip = null;
            }

            base.OnHandleDestroyed(e);
        }

        private void OnListChange(object sender, EventArgs e)
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:OnListChange");
            if (!DropDownListBox.InSetSelectedIndex())
            {
                GridEntry gridEntry = GetGridEntryFromRow(_selectedRow);
                EditTextBox.Text = gridEntry.GetPropertyTextValue(DropDownListBox.SelectedItem);
                EditTextBox.Focus();
                EditTextBox.SelectAll();
            }

            SetFlag(Flags.DropDownCommit, true);
        }

        private void OnListMouseUp(object sender, MouseEventArgs me)
        {
            OnListClick(sender, me);
        }

        private void OnListClick(object sender, EventArgs e)
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:OnListClick");
            _ = GetGridEntryFromRow(_selectedRow);

            if (DropDownListBox.Items.Count == 0)
            {
                CommonEditorHide();
                SetCommitError(ErrorState.None);
                SelectRow(_selectedRow);
                return;
            }
            else
            {
                object value = DropDownListBox.SelectedItem;

                // Don't need the commit because we're committing anyway.
                SetFlag(Flags.DropDownCommit, false);
                if (value is not null && !CommitText((string)value))
                {
                    SetCommitError(ErrorState.None);
                    SelectRow(_selectedRow);
                }
            }
        }

        private void OnListDrawItem(object sender, DrawItemEventArgs e)
        {
            int index = e.Index;

            if (index < 0 || _selectedGridEntry is null)
            {
                return;
            }

            string text = (string)DropDownListBox.Items[e.Index];

            Debug.WriteLineIf(s_gridViewDebugPaint.TraceVerbose, $"Drawing list item, value='{text}'");
            e.DrawBackground();
            e.DrawFocusRectangle();

            Rectangle drawBounds = e.Bounds;
            drawBounds.Y += 1;
            drawBounds.X -= 1;

            GridEntry gridEntry = GetGridEntryFromRow(_selectedRow);

            try
            {
                gridEntry.PaintValue(
                    gridEntry.ConvertTextToValue(text),
                    e.GraphicsInternal,
                    drawBounds,
                    drawBounds,
                    e.State.HasFlag(DrawItemState.Selected) ? GridEntry.PaintValueFlags.DrawSelected : default);
            }
            catch (FormatException ex)
            {
                ShowFormatExceptionMessage(gridEntry.PropertyLabel, ex);
                if (DropDownListBox.IsHandleCreated)
                {
                    DropDownListBox.Visible = false;
                }
            }
        }

        private void OnListKeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Return)
            {
                OnListClick(null, null);
                if (_selectedGridEntry is not null)
                {
                    _selectedGridEntry.OnValueReturnKey();
                }
            }

            OnKeyDown(sender, e);
        }

        protected override void OnLostFocus(EventArgs e)
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:OnLostFocus");
            Debug.WriteLineIf(s_gridViewDebugPaint.TraceVerbose, "PropertyGridView lost focus");

            if (e is not null)
            {
                base.OnLostFocus(e);
            }

            if (FocusInside)
            {
                base.OnLostFocus(e);
                return;
            }

            GridEntry gridEntry = GetGridEntryFromRow(_selectedRow);
            if (gridEntry is not null)
            {
                Debug.WriteLineIf(s_gridViewDebugPaint.TraceVerbose, "removing gridEntry focus");
                gridEntry.HasFocus = false;
                CommonEditorHide();
                InvalidateRow(_selectedRow);
            }

            base.OnLostFocus(e);

            // For empty GridView, clear the focus indicator that was painted in OnGotFocus()
            if (TotalProperties <= 0)
            {
                var clearRect = new Rectangle(1, 1, Size.Width - 2, Size.Height - 2);
                Debug.WriteLineIf(s_gridViewDebugPaint.TraceVerbose, $"Filling empty gridview rect={clearRect}");

                Color color = BackColor;
                if (color.HasTransparency())
                {
                    using Graphics g = CreateGraphicsInternal();
                    using var brush = color.GetCachedSolidBrushScope();
                    g.FillRectangle(brush, clearRect);
                }
                else
                {
                    using var hdc = new User32.GetDcScope(Handle);
                    using var hbrush = new Gdi32.CreateBrushScope(color);
                    hdc.FillRectangle(hbrush, clearRect);
                }
            }
        }

        private void OnEditChange(object sender, EventArgs e)
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:OnEditChange");
            SetCommitError(ErrorState.None, EditTextBox.Focused);

            ToolTip.ToolTip = string.Empty;
            ToolTip.Visible = false;

            if (!EditTextBox.InSetText)
            {
                GridEntry gridEntry = GetGridEntryFromRow(_selectedRow);
                if (gridEntry is not null && gridEntry.IsImmediatelyEditable)
                {
                    CommitEditTextBox();
                }
            }
        }

        private void OnEditGotFocus(object sender, EventArgs e)
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:OnEditGotFocus");

            if (!EditTextBox.Visible)
            {
                Focus();
                return;
            }

            switch (_errorState)
            {
                case ErrorState.MessageBoxUp:
                    return;
                case ErrorState.Thrown:
                    if (EditTextBox.Visible)
                    {
                        EditTextBox.HookMouseDown = true;
                    }

                    break;
                default:
                    if (EditTextBoxNeedsCommit)
                    {
                        SetCommitError(ErrorState.None, capture: true);
                    }

                    break;
            }

            if (_selectedGridEntry is not null && GetRowFromGridEntry(_selectedGridEntry) != -1)
            {
                Debug.WriteLineIf(s_gridViewDebugPaint.TraceVerbose, "adding gridEntry focus");
                _selectedGridEntry.HasFocus = true;
                InvalidateRow(_selectedRow);
                (EditTextBox.AccessibilityObject as ControlAccessibleObject).NotifyClients(AccessibleEvents.Focus);

                EditTextBox.AccessibilityObject.SetFocus();
            }
            else
            {
                SelectRow(0);
            }
        }

        private bool ProcessEnumUpAndDown(GridEntry entry, Keys keyCode, bool closeDropDown = true)
        {
            object value = entry.PropertyValue;
            object[] values = entry.GetPropertyValueList();
            if (values is null)
            {
                return false;
            }

            for (int i = 0; i < values.Length; i++)
            {
                object currentValue = values[i];

                if (value is not null && currentValue is not null && value.GetType() != currentValue.GetType()
                    && entry.TypeConverter.CanConvertTo(entry, value.GetType()))
                {
                    currentValue = entry.TypeConverter.ConvertTo(entry, CultureInfo.CurrentCulture, currentValue, value.GetType());
                }

                bool equal = (value == currentValue) || (value is not null && value.Equals(currentValue));

                if (!equal && value is string @string && currentValue is not null)
                {
                    equal = 0 == string.Compare(@string, currentValue.ToString(), true, CultureInfo.CurrentCulture);
                }

                if (!equal)
                {
                    continue;
                }

                object valueNew;
                if (keyCode == Keys.Up)
                {
                    if (i == 0)
                    {
                        return true;
                    }

                    valueNew = values[i - 1];
                }
                else
                {
                    if (i == values.Length - 1)
                    {
                        return true;
                    }

                    valueNew = values[i + 1];
                }

                CommitValue(entry, valueNew, closeDropDown);
                EditTextBox?.SelectAll();
                return true;
            }

            return false;
        }

        private void OnEditKeyDown(object sender, KeyEventArgs e)
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:OnEditKeyDown");
            if (!e.Alt && (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down))
            {
                GridEntry gridEntry = GetGridEntryFromRow(_selectedRow);
                if (!gridEntry.Enumerable || !gridEntry.IsValueEditable)
                {
                    return;
                }

                e.Handled = true;
                bool processed = ProcessEnumUpAndDown(gridEntry, e.KeyCode);
                if (processed)
                {
                    return;
                }
            }
            else if ((e.KeyCode == Keys.Left || e.KeyCode == Keys.Right)
                && (e.Modifiers & ~Keys.Shift) != 0)
            {
                // Handle non-expand/collapse case of left & right as up & down
                return;
            }

            OnKeyDown(sender, e);
        }

        private void OnEditKeyPress(object sender, KeyPressEventArgs e)
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:OnEditKeyPress");
            GridEntry gridEntry = GetGridEntryFromRow(_selectedRow);
            if (gridEntry is null)
            {
                return;
            }

            if (!gridEntry.IsTextEditable)
            {
                e.Handled = FilterReadOnlyEditKeyPress(e.KeyChar);
            }
        }

        private void OnEditLostFocus(object sender, EventArgs e)
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:OnEditLostFocus");

            // Believe it or not this can actually happen.
            if (EditTextBox.Focused || (_errorState == ErrorState.MessageBoxUp) || (_errorState == ErrorState.Thrown) || InPropertySet)
            {
                return;
            }

            // Check to see if the focus is on the drop down or one of it's children. If so, return.
            if (_dropDownHolder is not null && _dropDownHolder.Visible)
            {
                bool found = false;
                for (IntPtr hwnd = User32.GetForegroundWindow(); hwnd != IntPtr.Zero; hwnd = User32.GetParent(hwnd))
                {
                    if (hwnd == _dropDownHolder.Handle)
                    {
                        found = true;
                    }
                }

                if (found)
                {
                    return;
                }
            }

            if (FocusInside)
            {
                return;
            }

            // If the focus isn't going to a child of the view
            if (!CommitEditTextBox())
            {
                EditTextBox.Focus();
                return;
            }

            // Change our focus state.
            InvokeLostFocus(this, EventArgs.Empty);
        }

        private void OnEditMouseDown(object sender, MouseEventArgs e)
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:OnEditMouseDown");

            if (!FocusInside)
            {
                SelectGridEntry(_selectedGridEntry, pageIn: false);
            }

            if (e.Clicks % 2 == 0)
            {
                DoubleClickRow(_selectedRow, toggleExpand: false, RowValue);
                EditTextBox.SelectAll();
            }

            if (_rowSelectTime == 0)
            {
                return;
            }

            // Check if the click happened within the double click time since the row was selected.
            // This allows the edits to be selected with two clicks instead of 3 (select row, double click).
            long timeStamp = DateTime.Now.Ticks;
            int delta = (int)((timeStamp - _rowSelectTime) / 10000); // make it milliseconds

            if (delta < SystemInformation.DoubleClickTime)
            {
                Point screenPoint = EditTextBox.PointToScreen(new(e.X, e.Y));

                if (Math.Abs(screenPoint.X - _rowSelectPos.X) < SystemInformation.DoubleClickSize.Width &&
                    Math.Abs(screenPoint.Y - _rowSelectPos.Y) < SystemInformation.DoubleClickSize.Height)
                {
                    DoubleClickRow(_selectedRow, toggleExpand: false, RowValue);
                    User32.SendMessageW(EditTextBox, User32.WM.LBUTTONUP, IntPtr.Zero, PARAM.FromLowHigh(e.X, e.Y));
                    EditTextBox.SelectAll();
                }

                _rowSelectPos = Point.Empty;

                _rowSelectTime = 0;
            }
        }

        private bool OnF4(Control sender)
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:OnF4");
            if (ModifierKeys != 0)
            {
                return false;
            }

            if (sender == this || sender == OwnerGrid)
            {
                F4Selection(true);
            }
            else
            {
                UnfocusSelection();
            }

            return true;
        }

        private bool OnEscape(Control sender)
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:OnEscape");
            if ((ModifierKeys & (Keys.Alt | Keys.Control)) != 0)
            {
                return false;
            }

            SetFlag(Flags.DropDownCommit, false);

            if (sender != EditTextBox || !EditTextBox.Focused)
            {
                if (sender != this)
                {
                    CloseDropDown();
                    Focus();
                }

                return false;
            }

            // If we aren't in an error state, just quit.
            if (_errorState == ErrorState.None)
            {
                EditTextBox.Text = _originalTextValue;
                Focus();
                return true;
            }

            if (EditTextBoxNeedsCommit)
            {
                bool success = false;
                EditTextBox.Text = _originalTextValue;
                bool needReset = true;

                if (_selectedGridEntry is not null)
                {
                    string currentTextValue = _selectedGridEntry.GetPropertyTextValue();
                    needReset = _originalTextValue != currentTextValue
                        && !(string.IsNullOrEmpty(_originalTextValue) && string.IsNullOrEmpty(currentTextValue));
                }

                if (needReset)
                {
                    try
                    {
                        success = CommitText(_originalTextValue);
                    }
                    catch
                    {
                    }
                }
                else
                {
                    success = true;
                }

                if (!success)
                {
                    EditTextBox.Focus();
                    EditTextBox.SelectAll();
                    return true;
                }
            }

            SetCommitError(ErrorState.None);
            Focus();
            return true;
        }

        protected override void OnKeyDown(KeyEventArgs e) => OnKeyDown(this, e);

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            GridEntry entry = GetGridEntryFromRow(_selectedRow);
            if (entry is null)
            {
                return;
            }

            e.Handled = true;
            bool controlPressed = e.Control;
            bool shiftPressed = e.Shift;
            bool controlShiftPressed = controlPressed && shiftPressed;
            bool altPressed = e.Alt;
            Keys keyCode = e.KeyCode;
            bool fallingThrough = false;

            // We have to do this here because if we are hosted in a non-windows forms dialog, we never get a chance to
            // peek at the messages, we just get called.
            if (keyCode == Keys.Tab)
            {
                if (ProcessDialogKey(e.KeyData))
                {
                    e.Handled = true;
                    return;
                }
            }

            // Alt-Arrow support.
            if (keyCode == Keys.Down && altPressed && DropDownButton.Visible)
            {
                F4Selection(false);
                return;
            }

            if (keyCode == Keys.Up && altPressed && DropDownButton.Visible && (_dropDownHolder is not null) && _dropDownHolder.Visible)
            {
                UnfocusSelection();
                return;
            }

            if (ToolTip.Visible)
            {
                ToolTip.ToolTip = string.Empty;
            }

            if (controlShiftPressed || sender == this || sender == OwnerGrid)
            {
                switch (keyCode)
                {
                    case Keys.Up:
                    case Keys.Down:
                        int postion = keyCode == Keys.Up ? _selectedRow - 1 : _selectedRow + 1;
                        SelectGridEntry(GetGridEntryFromRow(postion), pageIn: true);
                        SetFlag(Flags.NoDefault, false);
                        return;
                    case Keys.Left:
                        if (controlPressed)
                        {
                            // Move the splitter 3 pixels to the left
                            MoveSplitterTo(InternalLabelWidth - 3);
                            return;
                        }

                        if (entry.InternalExpanded)
                        {
                            SetExpand(entry, false);
                        }
                        else
                        {
                            // Handle non-expand/collapse case of left & right as up & down.
                            SelectGridEntry(GetGridEntryFromRow(_selectedRow - 1), pageIn: true);
                        }

                        return;
                    case Keys.Right:
                        if (controlPressed)
                        {
                            // Move the splitter 3 pixels to the right.
                            MoveSplitterTo(InternalLabelWidth + 3);
                            return;
                        }

                        if (entry.Expandable)
                        {
                            if (entry.InternalExpanded)
                            {
                                SelectGridEntry(entry.Children[0], pageIn: true);
                            }
                            else
                            {
                                SetExpand(entry, true);
                            }
                        }
                        else
                        {
                            // Handle non-expand/collapse case of left & right as up & down.
                            SelectGridEntry(GetGridEntryFromRow(_selectedRow + 1), pageIn: true);
                        }

                        return;
                    case Keys.Return:
                        if (entry.Expandable)
                        {
                            SetExpand(entry, !entry.InternalExpanded);
                        }
                        else
                        {
                            entry.OnValueReturnKey();
                        }

                        return;
                    case Keys.Home:
                    case Keys.End:
                        GridEntryCollection allEntries = GetAllGridEntries();
                        SelectGridEntry(allEntries[keyCode == Keys.Home ? 0 : allEntries.Count - 1], pageIn: true);
                        return;
                    case Keys.Add:
                    case Keys.Oemplus:
                    case Keys.OemMinus:
                    case Keys.Subtract:

                        if (!entry.Expandable)
                        {
                            break;
                        }

                        SetFlag(Flags.IsSpecialKey, true);
                        bool expand = keyCode == Keys.Add || keyCode == Keys.Oemplus;
                        SetExpand(entry, expand);
                        Invalidate();
                        e.Handled = true;
                        return;

                    case Keys.D8:
                        if (shiftPressed)
                        {
                            goto case Keys.Multiply;
                        }

                        break;
                    case Keys.Multiply:
                        SetFlag(Flags.IsSpecialKey, true);
                        RecursivelyExpand(entry, true, true, MaxRecurseExpand);
                        e.Handled = false;
                        return;

                    case Keys.Prior:  // PAGE_UP
                    case Keys.Next:   // PAGE_DOWN

                        bool next = keyCode == Keys.Next;
                        int offset = next ? _visibleRows - 1 : 1 - _visibleRows;

                        int row = _selectedRow;

                        if (controlPressed && !shiftPressed)
                        {
                            return;
                        }

                        if (_selectedRow != -1)
                        {
                            // Actual paging.
                            int start = GetScrollOffset();
                            SetScrollOffset(start + offset);
                            SetConstants();
                            if (GetScrollOffset() != (start + offset))
                            {
                                // We didn't make a full page.
                                if (next)
                                {
                                    row = _visibleRows - 1;
                                }
                                else
                                {
                                    row = 0;
                                }
                            }
                        }

                        SelectRow(row);
                        Refresh();
                        return;

                    // Copy/paste support.

                    case Keys.Insert:
                        if (shiftPressed && !controlPressed && !altPressed)
                        {
                            fallingThrough = true;
                            goto case Keys.V;
                        }

                        goto case Keys.C;
                    case Keys.C:
                        // Copy text in current property.
                        if (controlPressed && !altPressed && !shiftPressed)
                        {
                            DoCopyCommand();
                            return;
                        }

                        break;
                    case Keys.Delete:
                        // Cut text in current property.
                        if (shiftPressed && !controlPressed && !altPressed)
                        {
                            fallingThrough = true;
                            goto case Keys.X;
                        }

                        break;
                    case Keys.X:
                        // Cut text in current property.
                        if (fallingThrough || (controlPressed && !altPressed && !shiftPressed))
                        {
                            Clipboard.SetDataObject(entry.GetPropertyTextValue());
                            CommitText("");
                            return;
                        }

                        break;
                    case Keys.V:
                        // Paste the text.
                        if (fallingThrough || (controlPressed && !altPressed && !shiftPressed))
                        {
                            DoPasteCommand();
                        }

                        break;
                    case Keys.A:
                        if (controlPressed && !altPressed && !shiftPressed && EditTextBox.Visible)
                        {
                            EditTextBox.Focus();
                            EditTextBox.SelectAll();
                        }

                        break;
                }
            }

            if (entry is not null && e.KeyData == (Keys.C | Keys.Alt | Keys.Shift | Keys.Control))
            {
                Clipboard.SetDataObject(entry.GetTestingInfo());
                return;
            }

            if (_selectedGridEntry is not null && _selectedGridEntry.Enumerable &&
                _dropDownHolder is not null && _dropDownHolder.Visible &&
                (keyCode == Keys.Up || keyCode == Keys.Down))
            {
                ProcessEnumUpAndDown(_selectedGridEntry, keyCode, false);
            }

            e.Handled = false;
            return;
        }

        protected override void OnKeyPress(KeyPressEventArgs e)
        {
            if (WillFilterKeyPress(e.KeyChar))
            {
                // Find next property with letter typed.
                FilterKeyPress(e.KeyChar);
            }

            SetFlag(Flags.IsSpecialKey, false);
        }

        protected override void OnMouseDown(MouseEventArgs e)
        {
            // Check for a splitter
            if (e.Button == MouseButtons.Left && SplitterInside(e.X) && TotalProperties != 0)
            {
                if (!CommitEditTextBox())
                {
                    return;
                }

                if (e.Clicks == 2)
                {
                    MoveSplitterTo(Width / 2);
                    return;
                }

                UnfocusSelection();
                SetFlag(Flags.IsSplitterMove, true);
                _tipInfo = -1;
                Capture = true;
                return;
            }

            // Are we on a propentry?
            Point pos = FindPosition(e.X, e.Y);

            if (pos == InvalidPosition)
            {
                return;
            }

            // Notify that prop entry of the click, but normalize it's coords first. We really just need the x, y.
            GridEntry gridEntry = GetGridEntryFromRow(pos.Y);

            if (gridEntry is not null)
            {
                // Get the origin of this pe.
                Rectangle r = GetRectangle(pos.Y, RowLabel);

                _lastMouseDown = new Point(e.X, e.Y);

                // Offset the mouse points & notify the prop entry.
                if (e.Button == MouseButtons.Left)
                {
                    gridEntry.OnMouseClick(e.X - r.X, e.Y - r.Y, e.Clicks, e.Button);
                }
                else
                {
                    SelectGridEntry(gridEntry, pageIn: false);
                }

                _lastMouseDown = InvalidPosition;
                gridEntry.HasFocus = true;
                SetFlag(Flags.NoDefault, false);
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            if (!_flags.HasFlag(Flags.IsSplitterMove))
            {
                Cursor = Cursors.Default;
            }

            base.OnMouseLeave(e);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            int rowMoveCurrent;
            Point point;
            bool onLabel = false;

            if (e is null)
            {
                rowMoveCurrent = -1;
                point = InvalidPosition;
            }
            else
            {
                point = FindPosition(e.X, e.Y);
                if (point == InvalidPosition || (point.X != RowLabel && point.X != RowValue))
                {
                    rowMoveCurrent = -1;
                    ToolTip.ToolTip = string.Empty;
                }
                else
                {
                    rowMoveCurrent = point.Y;
                    onLabel = point.X == RowLabel;
                }
            }

            if (point == InvalidPosition || e is null)
            {
                return;
            }

            if (_flags.HasFlag(Flags.IsSplitterMove))
            {
                MoveSplitterTo(e.X);
            }

            if ((rowMoveCurrent != TipRow || point.X != TipColumn) && !_flags.HasFlag(Flags.IsSplitterMove))
            {
                GridEntry gridItem = GetGridEntryFromRow(rowMoveCurrent);
                string tip = string.Empty;
                _tipInfo = -1;

                if (gridItem is not null)
                {
                    Rectangle itemRect = GetRectangle(point.Y, point.X);
                    if (onLabel && gridItem.GetLabelToolTipLocation(e.X - itemRect.X, e.Y - itemRect.Y) != InvalidPoint)
                    {
                        tip = gridItem.LabelToolTipText;
                        TipRow = rowMoveCurrent;
                        TipColumn = point.X;
                    }
                    else if (!onLabel && gridItem.ValueToolTipLocation != InvalidPoint && !EditTextBox.Focused)
                    {
                        if (!EditTextBoxNeedsCommit)
                        {
                            tip = gridItem.GetPropertyTextValue();
                        }

                        TipRow = rowMoveCurrent;
                        TipColumn = point.X;
                    }
                }

                // Ensure that tooltips don't display when host application is not foreground app.
                // Assume that we don't want to display the tooltips
                IntPtr foregroundWindow = User32.GetForegroundWindow();
                if (User32.IsChild(foregroundWindow, new HandleRef(this, Handle)).IsTrue())
                {
                    // Don't show the tips if a dropdown is showing
                    if (_dropDownHolder is null || _dropDownHolder.Component is null || rowMoveCurrent == _selectedRow)
                    {
                        ToolTip.ToolTip = tip;
                    }
                }
                else
                {
                    ToolTip.ToolTip = string.Empty;
                }
            }

            if (TotalProperties != 0 && (SplitterInside(e.X) || _flags.HasFlag(Flags.IsSplitterMove)))
            {
                Cursor = Cursors.VSplit;
            }
            else
            {
                Cursor = Cursors.Default;
            }

            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseEventArgs e) => CancelSplitterMove();

        protected override void OnMouseWheel(MouseEventArgs e)
        {
            OwnerGrid.OnGridViewMouseWheel(e);

            if (e is HandledMouseEventArgs handledArgs)
            {
                if (handledArgs.Handled)
                {
                    return;
                }

                handledArgs.Handled = true;
            }

            if ((ModifierKeys & (Keys.Shift | Keys.Alt)) != 0 || MouseButtons != MouseButtons.None)
            {
                return; // Do not scroll when Shift or Alt key is down, or when a mouse button is down.
            }

            int wheelScrollLines = SystemInformation.MouseWheelScrollLines;
            if (wheelScrollLines == 0)
            {
                return; // Do not scroll when the user system setting is 0 lines per notch
            }

            Debug.Assert(_cumulativeVerticalWheelDelta > -NativeMethods.WHEEL_DELTA, "cumulativeVerticalWheelDelta is too small");
            Debug.Assert(_cumulativeVerticalWheelDelta < NativeMethods.WHEEL_DELTA, "cumulativeVerticalWheelDelta is too big");

            // Should this only work if the Edit has focus?
            // We use the mouse wheel to change the values in the dropdown if it's an enumerable value.
            if (_selectedGridEntry is not null && _selectedGridEntry.Enumerable && EditTextBox.Focused && _selectedGridEntry.IsValueEditable)
            {
                int index = GetCurrentValueIndex(_selectedGridEntry);
                if (index != -1)
                {
                    int delta = e.Delta > 0 ? -1 : 1;
                    object[] values = _selectedGridEntry.GetPropertyValueList();

                    if (delta > 0 && index >= (values.Length - 1))
                    {
                        index = 0;
                    }
                    else if (delta < 0 && index == 0)
                    {
                        index = values.Length - 1;
                    }
                    else
                    {
                        index += delta;
                    }

                    CommitValue(values[index]);
                    SelectGridEntry(_selectedGridEntry, pageIn: true);
                    EditTextBox.Focus();
                    return;
                }
            }

            int initialOffset = GetScrollOffset();
            _cumulativeVerticalWheelDelta += e.Delta;
            float partialNotches = _cumulativeVerticalWheelDelta / (float)NativeMethods.WHEEL_DELTA;
            int fullNotches = (int)partialNotches;

            if (wheelScrollLines == -1)
            {
                // Equivalent to large change scrolls
                if (fullNotches != 0)
                {
                    int originalOffset = initialOffset;
                    int large = fullNotches * _scrollBar.LargeChange;
                    int newOffset = Math.Max(0, initialOffset - large);
                    newOffset = Math.Min(newOffset, TotalProperties - _visibleRows + 1);

                    initialOffset -= fullNotches * _scrollBar.LargeChange;
                    if (Math.Abs(initialOffset - originalOffset) >= Math.Abs(fullNotches * _scrollBar.LargeChange))
                    {
                        _cumulativeVerticalWheelDelta -= fullNotches * NativeMethods.WHEEL_DELTA;
                    }
                    else
                    {
                        _cumulativeVerticalWheelDelta = 0;
                    }

                    if (!ScrollRows(newOffset))
                    {
                        _cumulativeVerticalWheelDelta = 0;
                        return;
                    }
                }
            }
            else
            {
                // SystemInformation.MouseWheelScrollLines doesn't work under terminal server,
                // it defaults to the notches per scroll.
                int scrollBands = (int)(wheelScrollLines * partialNotches);

                if (scrollBands != 0)
                {
                    if (ToolTip.Visible)
                    {
                        ToolTip.ToolTip = string.Empty;
                    }

                    int newOffset = Math.Max(0, initialOffset - scrollBands);
                    newOffset = Math.Min(newOffset, TotalProperties - _visibleRows + 1);

                    if (scrollBands > 0)
                    {
                        if (_scrollBar.Value <= _scrollBar.Minimum)
                        {
                            _cumulativeVerticalWheelDelta = 0;
                        }
                        else
                        {
                            _cumulativeVerticalWheelDelta -= (int)(scrollBands * (NativeMethods.WHEEL_DELTA / (float)wheelScrollLines));
                        }
                    }
                    else
                    {
                        if (_scrollBar.Value > (_scrollBar.Maximum - _visibleRows + 1))
                        {
                            _cumulativeVerticalWheelDelta = 0;
                        }
                        else
                        {
                            _cumulativeVerticalWheelDelta -= (int)(scrollBands * (NativeMethods.WHEEL_DELTA / (float)wheelScrollLines));
                        }
                    }

                    if (!ScrollRows(newOffset))
                    {
                        _cumulativeVerticalWheelDelta = 0;
                        return;
                    }
                }
                else
                {
                    _cumulativeVerticalWheelDelta = 0;
                }
            }
        }

        protected override void OnMove(EventArgs e) => CloseDropDown();

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:OnPaint");
            Debug.WriteLineIf(s_gridViewDebugPaint.TraceVerbose, $"On paint called.  Rect={e.ClipRectangle}");
            Graphics g = e.Graphics;

            int yPosition = 0;
            int startRow = 0;
            int endRow = _visibleRows - 1;

            Rectangle clipRect = e.ClipRectangle;

            // Give ourselves a little breathing room to account for lines, etc., as well as the entries themselves.
            clipRect.Inflate(0, 2);

            try
            {
                Size sizeWindow = Size;

                // Figure out what rows we're painting.
                Point startPosition = FindPosition(clipRect.X, clipRect.Y);
                Point endPosition = FindPosition(clipRect.X, clipRect.Y + clipRect.Height);
                if (startPosition != InvalidPosition)
                {
                    startRow = Math.Max(0, startPosition.Y);
                }

                if (endPosition != InvalidPosition)
                {
                    endRow = endPosition.Y;
                }

                int visibleCount = Math.Min(TotalProperties - GetScrollOffset(), 1 + _visibleRows);

#if DEBUG
                GridEntry debugEntryStart = GetGridEntryFromRow(startRow);
                GridEntry debugEntryEnd = GetGridEntryFromRow(endRow);
                string startName = debugEntryStart?.PropertyLabel ?? "(null)";
                string endName = debugEntryEnd?.PropertyLabel ?? "(null)";
#endif

                SetFlag(Flags.NeedsRefresh, false);

                Size size = GetOurSize();
                Point location = _location;

                if (GetGridEntryFromRow(visibleCount - 1) is null)
                {
                    visibleCount--;
                }

                // If we actually have some properties, then start drawing the grid.
                if (TotalProperties > 0)
                {
                    // Draw splitter.
                    visibleCount = Math.Min(visibleCount, endRow + 1);

                    Debug.WriteLineIf(s_gridViewDebugPaint.TraceVerbose, "Drawing splitter");
                    using var splitterPen = OwnerGrid.LineColor.GetCachedPenScope(GetSplitterWidth());
                    g.DrawLine(splitterPen, _labelWidth, location.Y, _labelWidth, visibleCount * (RowHeight + 1) + location.Y);

                    // Draw lines.
                    Debug.WriteLineIf(s_gridViewDebugPaint.TraceVerbose, "Drawing lines");
                    using var linePen = g.FindNearestColor(OwnerGrid.LineColor).GetCachedPenScope();

                    int currentRowHeight = 0;
                    int lineEnd = location.X + size.Width;
                    int lineStart = location.X;

                    // Draw values.
                    int totalWidth = GetTotalWidth() + 1;

                    // Draw labels. set clip rect.
                    for (int i = startRow; i < visibleCount; i++)
                    {
                        try
                        {
                            // Draw the line.
                            currentRowHeight = i * (RowHeight + 1) + location.Y;
                            g.DrawLine(linePen, lineStart, currentRowHeight, lineEnd, currentRowHeight);

                            // Draw the value.
                            DrawValueEntry(g, i, clipRect);

                            // Draw the label.
                            Rectangle rect = GetRectangle(i, RowLabel);
                            yPosition = rect.Y + rect.Height;
                            DrawLabel(g, i, rect, i == _selectedRow, false, clipRect);
                            if (i == _selectedRow)
                            {
                                EditTextBox.Invalidate();
                            }
                        }
                        catch
                        {
                            Debug.WriteLineIf(s_gridViewDebugPaint.TraceVerbose,
                                $"Exception thrown during painting property {GetGridEntryFromRow(i).PropertyLabel}");
                        }
                    }

                    // Draw the bottom line
                    currentRowHeight = visibleCount * (RowHeight + 1) + location.Y;
                    g.DrawLine(linePen, lineStart, currentRowHeight, lineEnd, currentRowHeight);
                }

                // Fill anything left.
                if (yPosition < Size.Height)
                {
                    yPosition++;
                    var clearRect = new Rectangle(1, yPosition, Size.Width - 2, Size.Height - yPosition - 1);
                    Debug.WriteLineIf(s_gridViewDebugPaint.TraceVerbose, $"Filling remaining area rect={clearRect}");

                    using var backBrush = BackColor.GetCachedSolidBrushScope();
                    g.FillRectangle(backBrush, clearRect);
                }

                // Draw outside border.
                using var borderPen = OwnerGrid.ViewBorderColor.GetCachedPenScope();
                g.DrawRectangle(borderPen, 0, 0, sizeWindow.Width - 1, sizeWindow.Height - 1);

                _boldFont = null;
            }
            catch
            {
                Debug.Fail("Caught exception in OnPaint");
            }
        }

        private void OnGridEntryLabelDoubleClick(object s, EventArgs e)
        {
            var gridEntry = (GridEntry)s;

            // If we've changed since the click (probably because we moved a row into view), bail.
            if (gridEntry != _lastClickedEntry)
            {
                return;
            }

            int row = GetRowFromGridEntry(gridEntry);
            DoubleClickRow(row, gridEntry.Expandable, RowLabel);
        }

        private void OnGridEntryValueDoubleClick(object s, EventArgs e)
        {
            var gridEntry = (GridEntry)s;

            // If we've changed since the click (probably because we moved a row into view), bail.
            if (gridEntry != _lastClickedEntry)
            {
                return;
            }

            int row = GetRowFromGridEntry(gridEntry);
            DoubleClickRow(row, gridEntry.Expandable, RowValue);
        }

        private void OnGridEntryLabelClick(object sender, EventArgs e)
        {
            _lastClickedEntry = (GridEntry)sender;
            SelectGridEntry(_lastClickedEntry, pageIn: true);
        }

        private void OnGridEntryOutlineClick(object sender, EventArgs e)
        {
            var gridEntry = (GridEntry)sender;
            Debug.Assert(gridEntry.Expandable, "non-expandable IPE firing outline click");

            Cursor oldCursor = Cursor;
            if (!ShouldSerializeCursor())
            {
                oldCursor = null;
            }

            Cursor = Cursors.WaitCursor;

            try
            {
                SetExpand(gridEntry, !gridEntry.InternalExpanded);
                SelectGridEntry(gridEntry, pageIn: false);
            }
            finally
            {
                Cursor = oldCursor;
            }
        }

        private void OnGridEntryValueClick(object sender, EventArgs e)
        {
            _lastClickedEntry = (GridEntry)sender;
            bool setSelectTime = sender != _selectedGridEntry;
            SelectGridEntry(_lastClickedEntry, pageIn: true);
            EditTextBox.Focus();

            if (_lastMouseDown != InvalidPosition)
            {
                // Clear the row select time so we don't interpret this as a double click.
                _rowSelectTime = 0;

                Point editPoint = PointToScreen(_lastMouseDown);
                editPoint = EditTextBox.PointToClient(editPoint);
                User32.SendMessageW(EditTextBox, User32.WM.LBUTTONDOWN, IntPtr.Zero, PARAM.FromLowHigh(editPoint.X, editPoint.Y));
                User32.SendMessageW(EditTextBox, User32.WM.LBUTTONUP, IntPtr.Zero, PARAM.FromLowHigh(editPoint.X, editPoint.Y));
            }

            if (setSelectTime)
            {
                _rowSelectTime = DateTime.Now.Ticks;
                _rowSelectPos = PointToScreen(_lastMouseDown);
            }
            else
            {
                _rowSelectTime = 0;
                _rowSelectPos = Point.Empty;
            }
        }

        protected override void OnFontChanged(EventArgs e)
        {
            _cachedRowHeight = -1;

            if (Disposing || ParentInternal is null || ParentInternal.Disposing)
            {
                return;
            }

            _boldFont = null;    // fontBold is cached based on Font

            ToolTip.Font = Font;
            SetFlag(Flags.NeedUpdateUIBasedOnFont, true);
            UpdateUIBasedOnFont(true);
            base.OnFontChanged(e);

            if (_selectedGridEntry is not null)
            {
                SelectGridEntry(_selectedGridEntry, true);
            }
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            if (Disposing || ParentInternal is null || ParentInternal.Disposing)
            {
                return;
            }

            if (Visible && ParentInternal is not null)
            {
                SetConstants();
                if (_selectedGridEntry is not null)
                {
                    SelectGridEntry(_selectedGridEntry, true);
                }

                if (_toolTip is not null)
                {
                    ToolTip.Font = Font;
                }
            }

            base.OnVisibleChanged(e);
        }

        private void OnRecreateChildren(object s, GridEntryRecreateChildrenEventArgs e)
        {
            var parent = (GridEntry)s;

            if (parent.Expanded)
            {
                var entries = new GridEntry[_allGridEntries.Count];
                _allGridEntries.CopyTo(entries, 0);

                // Find the index of the gridEntry that fired the event in our main list.
                int parentIndex = -1;
                for (int i = 0; i < entries.Length; i++)
                {
                    if (entries[i] == parent)
                    {
                        parentIndex = i;
                        break;
                    }
                }

                Debug.Assert(parentIndex != -1, "parent GridEntry not found in allGridEntries");

                // Clear our existing handlers.
                ClearGridEntryEvents(_allGridEntries, parentIndex + 1, e.OldChildCount);

                // Resize the array if it's changed.
                if (e.OldChildCount != e.NewChildCount)
                {
                    int newArraySize = entries.Length + (e.NewChildCount - e.OldChildCount);
                    var newEntries = new GridEntry[newArraySize];

                    // Copy the existing entries up to the parent.
                    Array.Copy(entries, 0, newEntries, 0, parentIndex + 1);

                    // Copy the entries after the spot we'll be putting the new ones.
                    Array.Copy(
                        entries,
                        parentIndex + e.OldChildCount + 1,
                        newEntries,
                        parentIndex + e.NewChildCount + 1,
                        entries.Length - (parentIndex + e.OldChildCount + 1));

                    entries = newEntries;
                }

                // From that point, replace the children with the new children.
                GridEntryCollection children = parent.Children;
                int childCount = children.Count;

                Debug.Assert(childCount == e.NewChildCount, $"parent reports {childCount} new children, event reports {e.NewChildCount}");

                // Replace the changed items.
                for (int i = 0; i < childCount; i++)
                {
                    entries[parentIndex + i + 1] = children[i];
                }

                // Reset the array, rehook the handlers.
                _allGridEntries.Clear();
                _allGridEntries.AddRange(entries);
                AddGridEntryEvents(_allGridEntries, parentIndex + 1, childCount);
            }

            if (e.OldChildCount != e.NewChildCount)
            {
                TotalProperties = CountPropsFromOutline(TopLevelGridEntries);
                SetConstants();
            }

            Invalidate();
        }

        protected override void OnResize(EventArgs e)
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:OnResize");

            Rectangle newRect = ClientRectangle;
            int yDelta = _lastClientRect == Rectangle.Empty ? 0 : newRect.Height - _lastClientRect.Height;

            // If we are changing widths, update the selected row.

            if (!_lastClientRect.IsEmpty && newRect.Width > _lastClientRect.Width)
            {
                var rectInvalidate = new Rectangle(_lastClientRect.Width - 1, 0, newRect.Width - _lastClientRect.Width + 1, _lastClientRect.Height);
                Invalidate(rectInvalidate);
            }

            if (!_lastClientRect.IsEmpty && yDelta > 0)
            {
                var rectInvalidate = new Rectangle(0, _lastClientRect.Height - 1, _lastClientRect.Width, newRect.Height - _lastClientRect.Height + 1);
                Invalidate(rectInvalidate);
            }

            int scroll = GetScrollOffset();
            SetScrollOffset(0);
            SetConstants();
            SetScrollOffset(scroll);

            if (DpiHelper.IsScalingRequirementMet)
            {
                SetFlag(Flags.NeedUpdateUIBasedOnFont, true);
                UpdateUIBasedOnFont(true);
                base.OnFontChanged(e);
            }

            CommonEditorHide();

            LayoutWindow(false);

            bool selectionVisible = _selectedGridEntry is not null && _selectedRow >= 0 && _selectedRow <= _visibleRows;
            SelectGridEntry(_selectedGridEntry, selectionVisible);
            _lastClientRect = newRect;
        }

        private void OnScroll(object sender, ScrollEventArgs e)
        {
            Debug.WriteLineIf(
                CompModSwitches.DebugGridView.TraceVerbose,
                $"PropertyGridView:OnScroll({ScrollBar.Value} -> {e.NewValue})");

            if (!CommitEditTextBox() || !IsScrollValueValid(e.NewValue))
            {
                // Cancel the move
                e.NewValue = ScrollBar.Value;
                return;
            }

            int oldRow = -1;
            GridEntry oldGridEntry = _selectedGridEntry;
            if (_selectedGridEntry is not null)
            {
                oldRow = GetRowFromGridEntry(oldGridEntry);
                Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "OnScroll: SelectedGridEntry=" + oldGridEntry.PropertyLabel);
            }

            ScrollBar.Value = e.NewValue;
            if (oldGridEntry is not null)
            {
                // We need to zero out the selected row so we don't try to commit again since selectedRow is now bogus.
                _selectedRow = -1;
                SelectGridEntry(oldGridEntry, pageIn: ScrollBar.Value == TotalProperties);
                int newRow = GetRowFromGridEntry(oldGridEntry);
                if (oldRow != newRow)
                {
                    Invalidate();
                }
            }
            else
            {
                Invalidate();
            }
        }

        private void OnSysColorChange(object sender, UserPreferenceChangedEventArgs e)
        {
            if (e.Category == UserPreferenceCategory.Color || e.Category == UserPreferenceCategory.Accessibility)
            {
                SetFlag(Flags.NeedUpdateUIBasedOnFont, true);
            }
        }

        public void PopupDialog(int row)
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:PopupDialog");
            GridEntry gridEntry = GetGridEntryFromRow(row);
            if (gridEntry is null)
            {
                return;
            }

            if (_dropDownHolder is not null && _dropDownHolder.GetUsed())
            {
                CloseDropDown();
                return;
            }

            bool needsDropDownButton = gridEntry.NeedsDropDownButton;
            bool enumerable = gridEntry.Enumerable;
            bool needsCustomEditorButton = gridEntry.NeedsCustomEditorButton;

            if (enumerable && !needsDropDownButton)
            {
                DropDownListBox.Items.Clear();
                _ = gridEntry.PropertyValue;
                object[] rgItems = gridEntry.GetPropertyValueList();
                int maxWidth = 0;

                // The listbox draws with GDI, not GDI+.  So we use a normal DC here.

                using var hdc = new User32.GetDcScope(DropDownListBox.Handle);

                var tm = new Gdi32.TEXTMETRICW();
                int selectionIndex = -1;

                // This creates a copy of the given Font, and as such we need to delete it
                var hFont = (Gdi32.HFONT)Font.ToHfont();
                using (var fontScope = new Gdi32.ObjectScope(hFont))
                {
                    using var fontSelection = new Gdi32.SelectObjectScope(hdc, hFont);

                    selectionIndex = GetCurrentValueIndex(gridEntry);
                    if (rgItems is not null && rgItems.Length > 0)
                    {
                        string value;
                        var textSize = new Size();

                        for (int i = 0; i < rgItems.Length; i++)
                        {
                            value = gridEntry.GetPropertyTextValue(rgItems[i]);
                            DropDownListBox.Items.Add(value);
                            Gdi32.GetTextExtentPoint32W(new HandleRef(DropDownListBox, hdc), value, value.Length, ref textSize);
                            maxWidth = Math.Max(textSize.Width, maxWidth);
                        }
                    }

                    Gdi32.GetTextMetricsW(hdc, ref tm);

                    // border + padding + scrollbar
                    maxWidth += 2 + tm.tmMaxCharWidth + SystemInformation.VerticalScrollBarWidth;
                }

                if (selectionIndex != -1)
                {
                    DropDownListBox.SelectedIndex = selectionIndex;
                }

                SetFlag(Flags.DropDownCommit, false);
                DropDownListBox.Height = Math.Max(tm.tmHeight + 2, Math.Min(_maxListBoxHeight, DropDownListBox.PreferredHeight));
                DropDownListBox.Width = Math.Max(maxWidth, GetRectangle(row, RowValue).Width);
                try
                {
                    bool resizable = DropDownListBox.Items.Count > (DropDownListBox.Height / DropDownListBox.ItemHeight);
                    SetFlag(Flags.ResizableDropDown, resizable);
                    DropDownControl(DropDownListBox);
                }
                finally
                {
                    SetFlag(Flags.ResizableDropDown, false);
                }

                Refresh();
            }
            else if (needsCustomEditorButton || needsDropDownButton)
            {
                try
                {
                    InPropertySet = true;
                    EditTextBox.DisableMouseHook = true;

                    try
                    {
                        SetFlag(Flags.ResizableDropDown, gridEntry.UITypeEditor.IsDropDownResizable);
                        gridEntry.EditPropertyValue(this);
                    }
                    finally
                    {
                        SetFlag(Flags.ResizableDropDown, false);
                    }
                }
                finally
                {
                    InPropertySet = false;
                    EditTextBox.DisableMouseHook = false;
                }

                Refresh();

                if (FocusInside)
                {
                    SelectGridEntry(gridEntry, pageIn: false);
                }
            }
        }

        internal static void PositionTooltip(Control parent, GridToolTip toolTip, Rectangle itemRect)
        {
            toolTip.Visible = false;

            RECT rect = itemRect;

            User32.SendMessageW(toolTip, (User32.WM)ComCtl32.TTM.ADJUSTRECT, (IntPtr)1, ref rect);

            // Now offset it back to screen coords.
            Point location = parent.PointToScreen(new(rect.left, rect.top));

            // Set the position once so it updates it's size with it's real width.
            toolTip.Location = location;

            int overhang = toolTip.Location.X + toolTip.Size.Width - SystemInformation.VirtualScreen.Width;
            if (overhang > 0)
            {
                location.X -= overhang;
                toolTip.Location = location;
            }

            // Tell the control we've repositioned it.
            toolTip.Visible = true;
        }

        protected override bool ProcessDialogKey(Keys keyData)
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:ProcessDialogKey");
            if (HasEntries)
            {
                Keys keyCode = keyData & Keys.KeyCode;
                switch (keyCode)
                {
                    case Keys.F4:
                        if (FocusInside)
                        {
                            return OnF4(this);
                        }

                        break;

                    case Keys.Tab:

                        if (((keyData & Keys.Control) != 0) ||
                            ((keyData & Keys.Alt) != 0))
                        {
                            break;
                        }

                        bool forward = (keyData & Keys.Shift) == 0;

                        Control focusedControl = FromHandle(User32.GetFocus());

                        if (focusedControl is null || !IsMyChild(focusedControl))
                        {
                            if (forward)
                            {
                                TabSelection();
                                focusedControl = FromHandle(User32.GetFocus());

                                // Make sure the value actually took the focus
                                if (IsMyChild(focusedControl))
                                {
                                    return true;
                                }
                                else
                                {
                                    return base.ProcessDialogKey(keyData);
                                }
                            }
                            else
                            {
                                break;
                            }
                        }
                        else
                        {
                            // One of our editors has focus.

                            if (EditTextBox.Focused)
                            {
                                if (forward)
                                {
                                    if (DropDownButton.Visible)
                                    {
                                        DropDownButton.Focus();
                                        return true;
                                    }
                                    else if (DialogButton.Visible)
                                    {
                                        DialogButton.Focus();
                                        return true;
                                    }
                                }
                                else
                                {
                                    SelectGridEntry(GetGridEntryFromRow(_selectedRow), pageIn: false);
                                    return true;
                                }
                            }
                            else if (DialogButton.Focused || DropDownButton.Focused)
                            {
                                if (!forward && EditTextBox.Visible)
                                {
                                    EditTextBox.Focus();
                                    return true;
                                }
                            }
                        }

                        break;
                    case Keys.Up:
                    case Keys.Down:
                    case Keys.Left:
                    case Keys.Right:
                        return false;
                    case Keys.Return:
                        if (DialogButton.Focused || DropDownButton.Focused)
                        {
                            OnButtonClick(DialogButton.Focused ? DialogButton : DropDownButton, EventArgs.Empty);
                            return true;
                        }
                        else if (_selectedGridEntry is not null && _selectedGridEntry.Expandable)
                        {
                            SetExpand(_selectedGridEntry, !_selectedGridEntry.InternalExpanded);
                            return true;
                        }

                        break;
                }
            }

            return base.ProcessDialogKey(keyData);
        }

        private void RecalculateProperties()
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:RecalculateProps");
            int props = CountPropsFromOutline(TopLevelGridEntries);
            if (TotalProperties != props)
            {
                TotalProperties = props;
                ClearGridEntryEvents(_allGridEntries, 0, -1);
                _allGridEntries = null;
            }
        }

        internal void RecursivelyExpand(GridEntry gridEntry, bool initialize, bool expand, int maxExpands)
        {
            if (gridEntry is null || (expand && --maxExpands < 0))
            {
                return;
            }

            SetExpand(gridEntry, expand);

            GridEntryCollection children = gridEntry.Children;
            if (children is not null)
            {
                for (int i = 0; i < children.Count; i++)
                {
                    RecursivelyExpand(children[i], initialize: false, expand, maxExpands);
                }
            }

            if (initialize)
            {
                GridEntry selectedEntry = _selectedGridEntry;
                Refresh();
                SelectGridEntry(selectedEntry, pageIn: false);
                Invalidate();
            }
        }

        public override void Refresh()
        {
            Refresh(fullRefresh: false, startRow: -1, endRow: -1);

            // Resetting gridoutline rect to recalculate before repaint when viewsort property changed.
            // This is necessary especially when user changes sort and move to a secondary monitor with different
            // DPI and change view sort back to original.

            if (TopLevelGridEntries is not null && DpiHelper.IsScalingRequirementMet)
            {
                int outlineRectIconSize = GetOutlineIconSize();
                foreach (GridEntry gridentry in TopLevelGridEntries)
                {
                    if (gridentry.OutlineRect.Height != outlineRectIconSize || gridentry.OutlineRect.Width != outlineRectIconSize)
                    {
                        ResetOutline(gridentry);
                    }
                }
            }

            // Make sure we got everything
            Invalidate();
        }

        public void Refresh(bool fullRefresh) => Refresh(fullRefresh, startRow: -1, endRow: -1);

        private void Refresh(bool fullRefresh, int startRow, int endRow)
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:Refresh");
            Debug.WriteLineIf(
                s_gridViewDebugPaint.TraceVerbose,
                $"Refresh called for rows {startRow} through {endRow}");
            SetFlag(Flags.NeedsRefresh, true);
            GridEntry gridEntry = null;

            if (IsDisposed)
            {
                return;
            }

            bool pageInGridEntry = true;

            if (startRow == -1)
            {
                startRow = 0;
            }

            if (fullRefresh || OwnerGrid.HavePropertyEntriesChanged())
            {
                if (HasEntries && !InPropertySet && !CommitEditTextBox())
                {
                    OnEscape(this);
                }

                int oldLength = TotalProperties;
                object oldObject = TopLevelGridEntries is null
                    || TopLevelGridEntries.Count == 0 ? null : TopLevelGridEntries[0].GetValueOwner();

                // Walk up to the main IPE and refresh it.
                if (fullRefresh)
                {
                    OwnerGrid.RefreshProperties(true);
                }

                if (oldLength > 0 && !_flags.HasFlag(Flags.NoDefault))
                {
                    _positionData = CaptureGridPositionData();
                    CommonEditorHide(true);
                }

                UpdateHelpAttributes(_selectedGridEntry, null);
                _selectedGridEntry = null;
                SetFlag(Flags.IsNewSelection, true);
                TopLevelGridEntries = OwnerGrid.GetCurrentEntries();

                ClearGridEntryEvents(_allGridEntries, 0, -1);
                _allGridEntries = null;
                RecalculateProperties();

                int newLength = TotalProperties;
                if (newLength > 0)
                {
                    if (newLength < oldLength)
                    {
                        SetScrollbarLength();
                        SetScrollOffset(0);
                    }

                    SetConstants();

                    if (_positionData is not null)
                    {
                        gridEntry = _positionData.Restore(this);

                        // Upon restoring the grid entry position, we don't want to page it in.
                        object newObject = TopLevelGridEntries is null || TopLevelGridEntries.Count == 0
                            ? null
                            : TopLevelGridEntries[0].GetValueOwner();
                        pageInGridEntry = (gridEntry is null) || oldLength != newLength || newObject != oldObject;
                    }

                    if (gridEntry is null)
                    {
                        gridEntry = OwnerGrid.GetDefaultGridEntry();
                        SetFlag(Flags.NoDefault, gridEntry is null && TotalProperties > 0);
                    }

                    InvalidateRows(startRow, endRow);
                    if (gridEntry is null)
                    {
                        _selectedRow = 0;
                        _selectedGridEntry = GetGridEntryFromRow(_selectedRow);
                    }
                }
                else if (oldLength == 0)
                {
                    return;
                }
                else
                {
                    SetConstants();
                }

                // Release the old positionData which contains reference to previous selected objects.
                _positionData = null;
                _lastClickedEntry = null;
            }

            if (!HasEntries)
            {
                CommonEditorHide(_selectedRow != -1);
                OwnerGrid.SetStatusBox(null, null);
                SetScrollOffset(0);
                _selectedRow = -1;
                Invalidate();
                return;
            }

            // In case we added or removed properties.

            OwnerGrid.ClearCachedValues();

            InvalidateRows(startRow, endRow);

            if (gridEntry is not null)
            {
                SelectGridEntry(gridEntry, pageInGridEntry);
            }
        }

        public void Reset()
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:Reset");
            GridEntry gridEntry = GetGridEntryFromRow(_selectedRow);
            if (gridEntry is null)
            {
                return;
            }

            gridEntry.ResetPropertyValue();
            SelectRow(_selectedRow);
        }

        private void ResetOrigin(Graphics g) => g.ResetTransform();

        internal void RestoreHierarchyState(ArrayList expandedItems)
        {
            if (expandedItems is null)
            {
                return;
            }

            foreach (GridEntryCollection gec in expandedItems)
            {
                FindEquivalentGridEntry(gec);
            }
        }

        internal ArrayList SaveHierarchyState(GridEntryCollection entries, ArrayList expandedItems = null)
        {
            if (entries is null)
            {
                return new ArrayList();
            }

            if (expandedItems is null)
            {
                expandedItems = new ArrayList();
            }

            for (int i = 0; i < entries.Count; i++)
            {
                if (entries[i].InternalExpanded)
                {
                    GridEntry entry = entries[i];
                    expandedItems.Add(GetGridEntryHierarchy(entry.Children[0]));
                    SaveHierarchyState(entry.Children, expandedItems);
                }
            }

            return expandedItems;
        }

        // Scroll to the new offset
        private bool ScrollRows(int newOffset)
        {
            GridEntry currentEntry = _selectedGridEntry;

            if (!IsScrollValueValid(newOffset) || !CommitEditTextBox())
            {
                return false;
            }

            bool editVisible = EditTextBox.Visible;
            bool dropDownButtonVisible = DropDownButton.Visible;
            bool editButtonVisible = DialogButton.Visible;

            EditTextBox.Visible = false;
            DialogButton.Visible = false;
            DropDownButton.Visible = false;

            SetScrollOffset(newOffset);

            if (currentEntry is not null)
            {
                int currentRow = GetRowFromGridEntry(currentEntry);
                if (currentRow >= 0 && currentRow < _visibleRows - 1)
                {
                    EditTextBox.Visible = editVisible;
                    DialogButton.Visible = editButtonVisible;
                    DropDownButton.Visible = dropDownButtonVisible;
                    SelectGridEntry(currentEntry, pageIn: true);
                }
                else
                {
                    CommonEditorHide();
                }
            }
            else
            {
                CommonEditorHide();
            }

            Invalidate();
            return true;
        }

        internal void SelectGridEntry(GridEntry entry, bool pageIn)
        {
            if (entry is null)
            {
                return;
            }

            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, $"PropertyGridView:SelectGridEntry({entry.PropertyLabel})");

            int row = GetRowFromGridEntry(entry);
            if (row + GetScrollOffset() < 0)
            {
                return;
            }

            int maxRows = (int)Math.Ceiling(((double)GetOurSize().Height) / (1 + RowHeight));

            // Determine whether or not we need to page-in this GridEntry

            if (!pageIn || (row >= 0 && row < (maxRows - 1)))
            {
                // No need to page-in: either fPageIn is false or the row is already in view
                SelectRow(row);
            }
            else
            {
                // Page-in the selected GridEntry
                _selectedRow = -1; // clear the selected row since it's no longer a valid number

                int offset = GetScrollOffset();
                if (row < 0)
                {
                    SetScrollOffset(row + offset);
                    Invalidate();
                    SelectRow(0);
                }
                else
                {
                    // Try to put it one row up from the bottom.
                    int newOffset = row + offset - (maxRows - 2);

                    if (newOffset >= ScrollBar.Minimum && newOffset < ScrollBar.Maximum)
                    {
                        SetScrollOffset(newOffset);
                    }

                    Invalidate();
                    SelectGridEntry(entry, pageIn: false);
                }
            }
        }

        private void SelectRow(int row)
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, $"PropertyGridView:SelectRow({row})");

            if (!_flags.HasFlag(Flags.IsNewSelection))
            {
                if (FocusInside)
                {
                    // If we're in an error state, we want to bail out of this.
                    if (_errorState != ErrorState.None || (row != _selectedRow && !CommitEditTextBox()))
                    {
                        return;
                    }
                }
                else
                {
                    Focus();
                }
            }

            GridEntry gridEntry = GetGridEntryFromRow(row);

            // Update our reset command.
            if (row != _selectedRow)
            {
                UpdateResetCommand(gridEntry);
            }

            if (_flags.HasFlag(Flags.IsNewSelection) && GetGridEntryFromRow(_selectedRow) is null)
            {
                CommonEditorHide();
            }

            UpdateHelpAttributes(_selectedGridEntry, gridEntry);

            // Tell the old selection it's not focused any more.
            if (_selectedGridEntry is not null)
            {
                _selectedGridEntry.HasFocus = false;
            }

            // Selection not visible.
            if (row < 0 || row >= _visibleRows)
            {
                CommonEditorHide();
                _selectedRow = row;
                _selectedGridEntry = gridEntry;
                Refresh();
                return;
            }

            // Leave current selection.
            if (gridEntry is null)
            {
                return;
            }

            bool newRow = false;
            int oldSelectedRow = _selectedRow;
            if (_selectedRow != row || !gridEntry.Equals(_selectedGridEntry))
            {
                CommonEditorHide();
                newRow = true;
            }

            if (!newRow)
            {
                CloseDropDown();
            }

            Rectangle rect = GetRectangle(row, RowValue);
            string s = gridEntry.GetPropertyTextValue();

            // What components are we using?
            bool needsDropDownButton = gridEntry.NeedsDropDownButton | gridEntry.Enumerable;
            bool needsCustomEditorButton = gridEntry.NeedsCustomEditorButton;
            bool customPaint = gridEntry.IsCustomPaint;

            rect.X += 1;
            rect.Width -= 1;

            // We want to allow builders on read-only properties
            if ((needsCustomEditorButton || needsDropDownButton) && !gridEntry.ShouldRenderReadOnly && FocusInside)
            {
                Control button = needsDropDownButton ? DropDownButton : DialogButton;
                Size sizeBtn = DpiHelper.IsScalingRequirementMet
                    ? new Size(SystemInformation.VerticalScrollBarArrowHeightForDpi(_deviceDpi), RowHeight)
                    : new Size(SystemInformation.VerticalScrollBarArrowHeight, RowHeight);

                var rectTarget = new Rectangle(
                    rect.X + rect.Width - sizeBtn.Width, rect.Y,
                    sizeBtn.Width, rect.Height);

                CommonEditorUse(button, rectTarget);
                sizeBtn = button.Size;
                rect.Width -= sizeBtn.Width;
                button.Invalidate();
            }

            // If we're painting the value, size the rect between the button and the painted value
            if (customPaint)
            {
                rect.X += _paintIndent + 1;
                rect.Width -= _paintIndent + 1;
            }
            else
            {
                rect.X += EditIndent + 1; // +1 to compensate for where GDI+ draws it's string relative to the rect.
                rect.Width -= EditIndent + 1;
            }

            if ((_flags.HasFlag(Flags.IsNewSelection) || !EditTextBox.Focused) && s is not null && !s.Equals(EditTextBox.Text))
            {
                EditTextBox.Text = s;
                _originalTextValue = s;
                EditTextBox.SelectionStart = 0;
                EditTextBox.SelectionLength = 0;
            }

            EditTextBox.AccessibleName = gridEntry.Label;

            if (gridEntry.ShouldSerializePropertyValue())
            {
                EditTextBox.Font = GetBoldFont();
            }
            else
            {
                EditTextBox.Font = Font;
            }

            if (_flags.HasFlag(Flags.IsSplitterMove) || !gridEntry.HasValue || !FocusInside)
            {
                EditTextBox.Visible = false;
            }
            else
            {
                rect.Offset(1, 1);
                rect.Height -= 1;
                rect.Width -= 1;
                CommonEditorUse(EditTextBox, rect);
                bool drawReadOnly = gridEntry.ShouldRenderReadOnly;
                EditTextBox.ForeColor = drawReadOnly ? GrayTextColor : ForeColor;
                EditTextBox.BackColor = BackColor;
                EditTextBox.ReadOnly = drawReadOnly || !gridEntry.IsTextEditable;
                EditTextBox.UseSystemPasswordChar = gridEntry.ShouldRenderPassword;
            }

            GridEntry oldSelectedGridEntry = _selectedGridEntry;
            _selectedRow = row;
            _selectedGridEntry = gridEntry;
            OwnerGrid.SetStatusBox(gridEntry.PropertyLabel, gridEntry.PropertyDescription);

            // Tell the new focused item that it now has focus
            if (_selectedGridEntry is not null)
            {
                _selectedGridEntry.HasFocus = FocusInside;
            }

            if (!_flags.HasFlag(Flags.IsNewSelection))
            {
                Focus();
            }

            InvalidateRow(oldSelectedRow);

            InvalidateRow(row);
            if (FocusInside)
            {
                SetFlag(Flags.IsNewSelection, false);
            }

            try
            {
                if (_selectedGridEntry != oldSelectedGridEntry)
                {
                    OwnerGrid.OnSelectedGridItemChanged(oldSelectedGridEntry, _selectedGridEntry);
                }
            }
            catch
            {
            }
        }

        public void SetConstants()
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:SetConstants");
            Size size = GetOurSize();

            _visibleRows = (int)Math.Ceiling(((double)size.Height) / (1 + RowHeight));

            size = GetOurSize();

            if (size.Width >= 0)
            {
                _labelRatio = Math.Max(Math.Min(_labelRatio, 9), 1.1);
                _labelWidth = _location.X + (int)(size.Width / _labelRatio);
            }

            int oldWidth = _labelWidth;

            bool adjustWidth = SetScrollbarLength();
            GridEntryCollection rgipesAll = GetAllGridEntries();
            if (rgipesAll is not null)
            {
                int scroll = GetScrollOffset();
                if ((scroll + _visibleRows) >= rgipesAll.Count)
                {
                    _visibleRows = rgipesAll.Count - scroll;
                }
            }

            if (adjustWidth && size.Width >= 0)
            {
                _labelRatio = GetOurSize().Width / (double)(oldWidth - _location.X);
            }

            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, $"\tsize       :{size}");
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, $"\tlocation   :{_location}");
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, $"\tvisibleRows:{_visibleRows}");
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, $"\tlabelWidth :{_labelWidth}");
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, $"\tlabelRatio :{_labelRatio}");
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, $"\trowHeight  :{RowHeight}");
#if DEBUG
            if (rgipesAll is null)
            {
                Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "\tIPE Count  :(null)");
            }
            else
            {
                Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, $"\tIPE Count  :{rgipesAll.Count}");
            }
#endif
        }

        private void SetCommitError(ErrorState error)
        {
            SetCommitError(error, error == ErrorState.Thrown);
        }

        private void SetCommitError(ErrorState error, bool capture)
        {
#if DEBUG
            if (CompModSwitches.DebugGridView.TraceVerbose)
            {
                string err = "UNKNOWN!";
                switch (error)
                {
                    case ErrorState.None:
                        err = "ERROR_NONE";
                        break;
                    case ErrorState.Thrown:
                        err = "ERROR_THROWN";
                        break;
                    case ErrorState.MessageBoxUp:
                        err = "ERROR_MSGBOX_UP";
                        break;
                }

                Debug.WriteLine($"PropertyGridView:SetCommitError(error={err}, capture={capture})");
            }
#endif
            _errorState = error;
            if (error != ErrorState.None)
            {
                CancelSplitterMove();
            }

            EditTextBox.HookMouseDown = capture;
        }

        internal void SetExpand(GridEntry entry, bool value)
        {
            if (entry is null || !entry.Expandable)
            {
                return;
            }

            int row = GetRowFromGridEntry(entry);
            int currentRow = _selectedRow;

            // If the currently selected row is below us, we need to commit now or the offsets will be wrong.
            if (_selectedRow != -1 && row < _selectedRow && EditTextBox.Visible)
            {
                // This will cause the commit.
                Focus();
            }

            int offset = GetScrollOffset();
            int items = TotalProperties;

            entry.InternalExpanded = value;

            var oldExpandedState = value ? UiaCore.ExpandCollapseState.Collapsed : UiaCore.ExpandCollapseState.Expanded;
            var newExpandedState = value ? UiaCore.ExpandCollapseState.Expanded : UiaCore.ExpandCollapseState.Collapsed;
            _selectedGridEntry?.AccessibilityObject?.RaiseAutomationPropertyChangedEvent(
                UiaCore.UIA.ExpandCollapseExpandCollapseStatePropertyId,
                oldExpandedState,
                newExpandedState);

            RecalculateProperties();
            GridEntry selectedEntry = _selectedGridEntry;
            if (!value)
            {
                for (GridEntry currentEntry = selectedEntry; currentEntry is not null; currentEntry = currentEntry.ParentGridEntry)
                {
                    if (currentEntry.Equals(entry))
                    {
                        selectedEntry = entry;
                    }
                }
            }

            row = GetRowFromGridEntry(entry);

            SetConstants();

            int newItems = TotalProperties - items;

            if (value && newItems > 0 && newItems < _visibleRows && (row + newItems) >= _visibleRows && newItems < currentRow)
            {
                // Scroll to show the newly opened items.
                SetScrollOffset(TotalProperties - items + offset);
            }

            Invalidate();

            SelectGridEntry(selectedEntry, pageIn: false);

            int scroll = GetScrollOffset();
            SetScrollOffset(0);
            SetConstants();
            SetScrollOffset(scroll);
        }

        private void SetFlag(Flags flag, bool value)
        {
            if (value)
            {
                _flags |= flag;
            }
            else
            {
                _flags &= ~flag;
            }
        }

        public void SetScrollOffset(int offset)
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, $"PropertyGridView:SetScrollOffset({offset})");
            int newPosition = Math.Max(0, Math.Min(TotalProperties - _visibleRows + 1, offset));
            int oldPosition = ScrollBar.Value;
            if (newPosition != oldPosition && IsScrollValueValid(newPosition) && _visibleRows > 0)
            {
                ScrollBar.Value = newPosition;
                Invalidate();
                _selectedRow = GetRowFromGridEntry(_selectedGridEntry);
            }
        }

        /// <summary>
        ///  Commits any needed changes in the <see cref="EditTextBox" />. Returns false if there is a change that
        ///  could not be committed.
        /// </summary>
        internal bool CommitEditTextBox()
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:Commit()");

            if (_errorState == ErrorState.MessageBoxUp)
            {
                Debug.WriteLineIf(
                    CompModSwitches.DebugGridView.TraceVerbose,
                    "PropertyGridView:Commit() returning false because an error has been thrown or we are in a property set");
                return false;
            }

            if (!EditTextBoxNeedsCommit)
            {
                SetCommitError(ErrorState.None);
                Debug.WriteLineIf(
                    CompModSwitches.DebugGridView.TraceVerbose,
                    "PropertyGridView:Commit() returning true because no change has been made");
                return true;
            }

            if (InPropertySet)
            {
                return false;
            }

            if (GetGridEntryFromRow(_selectedRow) is null)
            {
                return true;
            }

            bool success = false;
            try
            {
                success = CommitText(EditTextBox.Text);
            }
            finally
            {
                if (!success)
                {
                    EditTextBox.Focus();
                    EditTextBox.SelectAll();
                }
                else
                {
                    SetCommitError(ErrorState.None);
                }
            }

            return success;
        }

        private bool CommitValue(object value)
        {
            GridEntry currentEntry = _selectedGridEntry;

            if (_selectedGridEntry is null && _selectedRow != -1)
            {
                currentEntry = GetGridEntryFromRow(_selectedRow);
            }

            if (currentEntry is null)
            {
                Debug.Fail("Committing with no selected row!");
                return true;
            }

            return CommitValue(currentEntry, value);
        }

        internal bool CommitValue(GridEntry entry, object value, bool closeDropDown = true)
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, $"PropertyGridView:CommitValue({value?.ToString() ?? "null"})");

            int propCount = entry.ChildCount;
            bool capture = EditTextBox.HookMouseDown;
            object originalValue = null;

            try
            {
                originalValue = entry.PropertyValue;
            }
            catch
            {
                // If the getter is failing, we still want to let the set happen.
            }

            try
            {
                try
                {
                    InPropertySet = true;

                    // If this propentry is enumerable, then once a value is selected from the editor,
                    // we'll want to close the drop down (like true/false).  Otherwise, if we're
                    // working with Anchor for ex., then we should be able to select different values
                    // from the editor, without having it close every time.
                    if (entry is not null && entry.Enumerable && closeDropDown)
                    {
                        CloseDropDown();
                    }

                    try
                    {
                        EditTextBox.DisableMouseHook = true;
                        entry.PropertyValue = value;
                    }
                    finally
                    {
                        EditTextBox.DisableMouseHook = false;
                        EditTextBox.HookMouseDown = capture;
                    }
                }
                catch (Exception ex)
                {
                    SetCommitError(ErrorState.Thrown);
                    ShowInvalidMessage(entry.PropertyLabel, ex);
                    return false;
                }
            }
            finally
            {
                InPropertySet = false;
            }

            SetCommitError(ErrorState.None);

            string text = entry.GetPropertyTextValue();
            if (!string.Equals(text, EditTextBox.Text))
            {
                EditTextBox.Text = text;
                EditTextBox.SelectionStart = 0;
                EditTextBox.SelectionLength = 0;
            }

            _originalTextValue = text;

            // Update our reset command.
            UpdateResetCommand(entry);

            if (entry.ChildCount != propCount)
            {
                ClearGridEntryEvents(_allGridEntries, 0, -1);
                _allGridEntries = null;
                SelectGridEntry(entry, pageIn: true);
            }

            if (entry.Disposed)
            {
                bool editfocused = _editTextBox is not null && _editTextBox.Focused;

                // Reselect the row to find the replacement.
                SelectGridEntry(entry, pageIn: true);
                entry = _selectedGridEntry;

                if (editfocused && _editTextBox is not null)
                {
                    _editTextBox.Focus();
                }
            }

            OwnerGrid.OnPropertyValueSet(entry, originalValue);

            return true;
        }

        private bool CommitText(string text)
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, $"PropertyGridView:CommitValue({text ?? "null"})");

            object value;

            GridEntry currentEntry = _selectedGridEntry;

            if (_selectedGridEntry is null && _selectedRow != -1)
            {
                currentEntry = GetGridEntryFromRow(_selectedRow);
            }

            if (currentEntry is null)
            {
                Debug.Fail("Committing with no selected row!");
                return true;
            }

            try
            {
                value = currentEntry.ConvertTextToValue(text);
            }
            catch (Exception ex)
            {
                SetCommitError(ErrorState.Thrown);
                ShowInvalidMessage(currentEntry.PropertyLabel, ex);
                return false;
            }

            SetCommitError(ErrorState.None);

            return CommitValue(value);
        }

        internal void ReverseFocus()
        {
            if (_selectedGridEntry is null)
            {
                Focus();
            }
            else
            {
                SelectGridEntry(_selectedGridEntry, pageIn: true);

                if (DialogButton.Visible)
                {
                    DialogButton.Focus();
                }
                else if (DropDownButton.Visible)
                {
                    DropDownButton.Focus();
                }
                else if (EditTextBox.Visible)
                {
                    EditTextBox.SelectAll();
                    EditTextBox.Focus();
                }
            }
        }

        private bool SetScrollbarLength()
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:SetScrollBarLength");

            if (TotalProperties == -1)
            {
                return false;
            }

            if (TotalProperties < _visibleRows)
            {
                SetScrollOffset(0);
            }
            else if (GetScrollOffset() > TotalProperties)
            {
                SetScrollOffset(TotalProperties + 1 - _visibleRows);
            }

            bool hidden = !ScrollBar.Visible;
            if (_visibleRows > 0)
            {
                ScrollBar.LargeChange = _visibleRows - 1;
            }

            bool scrollBarChanged = false;

            ScrollBar.Maximum = Math.Max(0, TotalProperties - 1);
            if (hidden != (TotalProperties < _visibleRows))
            {
                scrollBarChanged = true;
                ScrollBar.Visible = hidden;
                Size size = GetOurSize();
                if (_labelWidth != -1 && size.Width > 0)
                {
                    if (_labelWidth > _location.X + size.Width)
                    {
                        _labelWidth = _location.X + (int)(size.Width / _labelRatio);
                    }
                    else
                    {
                        _labelRatio = GetOurSize().Width / (double)(_labelWidth - _location.X);
                    }
                }

                Invalidate();
            }

            return scrollBarChanged;
        }

        /// <summary>
        ///  Shows the given dialog, and returns its dialog result.  You should always
        ///  use this method rather than showing the dialog directly, as this will
        ///  properly position the dialog and provide it a dialog owner.
        /// </summary>
        public DialogResult ShowDialog(Form dialog)
        {
            // Try to shift down if sitting right on top of existing owner.
            if (dialog.StartPosition == FormStartPosition.CenterScreen)
            {
                Control topControl = this;
                if (topControl is not null)
                {
                    while (topControl.ParentInternal is not null)
                    {
                        topControl = topControl.ParentInternal;
                    }

                    if (topControl.Size.Equals(dialog.Size))
                    {
                        dialog.StartPosition = FormStartPosition.Manual;
                        Point location = topControl.Location;
                        location.Offset(25, 25);
                        dialog.Location = location;
                    }
                }
            }

            IntPtr priorFocus = User32.GetFocus();

            DialogResult result;
            if (TryGetService(out IUIService uiService))
            {
                result = uiService.ShowDialog(dialog);
            }
            else
            {
                result = dialog.ShowDialog(this);
            }

            if (priorFocus != IntPtr.Zero)
            {
                User32.SetFocus(priorFocus);
            }

            return result;
        }

        private void ShowFormatExceptionMessage(string propertyName, Exception ex)
        {
            propertyName ??= "(unknown)";

            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, $"PropertyGridView:ShowFormatExceptionMessage(prop={propertyName})");

            // We have to uninstall our hook so the user can push the button!
            bool hooked = EditTextBox.HookMouseDown;
            EditTextBox.DisableMouseHook = true;
            SetCommitError(ErrorState.MessageBoxUp, false);

            // Before invoking the error dialog, flush all mouse messages in the message queue.
            // Otherwise the click that triggered the error will still be in the queue, and will get eaten by the dialog,
            // potentially causing an accidental button click. Problem occurs because we trap clicks using a system hook,
            // which usually discards the message by returning 1 to GetMessage(). But this won't occur until after the
            // error dialog gets closed, which is much too late.
            var mouseMessage = new User32.MSG();
            while (User32.PeekMessageW(ref mouseMessage,
                IntPtr.Zero,
                User32.WM.MOUSEFIRST,
                User32.WM.MOUSELAST,
                User32.PM.REMOVE).IsTrue())
            {
                // No-op.
            }

            if (ex is Reflection.TargetInvocationException)
            {
                ex = ex.InnerException;
            }

            // Try to find an exception message to display
            string exMessage = ex.Message;

            bool revert;

            while (exMessage is null || exMessage.Length == 0)
            {
                ex = ex.InnerException;
                if (ex is null)
                {
                    break;
                }

                exMessage = ex.Message;
            }

            ErrorDialog.Message = SR.PBRSFormatExceptionMessage;
            ErrorDialog.Text = SR.PBRSErrorTitle;
            ErrorDialog.Details = exMessage;

            if (TryGetService(out IUIService uiService))
            {
                revert = DialogResult.Cancel == uiService.ShowDialog(ErrorDialog);
            }
            else
            {
                revert = DialogResult.Cancel == ShowDialog(ErrorDialog);
            }

            EditTextBox.DisableMouseHook = false;

            if (hooked)
            {
                SelectGridEntry(_selectedGridEntry, pageIn: true);
            }

            SetCommitError(ErrorState.Thrown, hooked);

            if (revert)
            {
                OnEscape(EditTextBox);
            }
        }

        internal void ShowInvalidMessage(string propertyName, Exception ex)
        {
            propertyName ??= "(unknown)";

            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, $"PropertyGridView:ShowInvalidMessage(prop={propertyName})");

            // We have to uninstall our hook so the user can push the button.
            bool hooked = EditTextBox.HookMouseDown;
            EditTextBox.DisableMouseHook = true;
            SetCommitError(ErrorState.MessageBoxUp, capture: false);

            // Before invoking the error dialog, flush all mouse messages in the message queue.
            //
            // Otherwise the click that triggered the error will still be in the queue, and will get eaten by the dialog,
            // potentially causing an accidental button click. Problem occurs because we trap clicks using a system hook,
            // which usually discards the message by returning 1 to GetMessage(). But this won't occur until after the
            // error dialog gets closed, which is much too late.
            var mouseMsg = new User32.MSG();
            while (User32.PeekMessageW(ref mouseMsg, IntPtr.Zero, User32.WM.MOUSEFIRST, User32.WM.MOUSELAST, User32.PM.REMOVE).IsTrue())
            {
                // No-op.
            }

            if (ex is Reflection.TargetInvocationException)
            {
                ex = ex.InnerException;
            }

            // Try to find an exception message to display.
            string message = ex.Message;

            bool revert;

            while (message is null || message.Length == 0)
            {
                ex = ex.InnerException;
                if (ex is null)
                {
                    break;
                }

                message = ex.Message;
            }

            ErrorDialog.Message = SR.PBRSErrorInvalidPropertyValue;
            ErrorDialog.Text = SR.PBRSErrorTitle;
            ErrorDialog.Details = message;

            if (TryGetService(out IUIService uiService))
            {
                revert = DialogResult.Cancel == uiService.ShowDialog(ErrorDialog);
            }
            else
            {
                revert = DialogResult.Cancel == ShowDialog(ErrorDialog);
            }

            EditTextBox.DisableMouseHook = false;

            if (hooked)
            {
                SelectGridEntry(_selectedGridEntry, pageIn: true);
            }

            SetCommitError(ErrorState.Thrown, capture: hooked);

            if (revert)
            {
                OnEscape(EditTextBox);
            }
        }

        private bool SplitterInside(int x) => Math.Abs(x - InternalLabelWidth) < 4;

        private void TabSelection()
        {
            GridEntry gridEntry = GetGridEntryFromRow(_selectedRow);
            if (gridEntry is null)
            {
                return;
            }

            if (EditTextBox.Visible)
            {
                EditTextBox.Focus();
                EditTextBox.SelectAll();
            }
            else if (_dropDownHolder is not null && _dropDownHolder.Visible)
            {
                _dropDownHolder.FocusComponent();
                return;
            }
            else if (_currentEditor is not null)
            {
                _currentEditor.Focus();
            }

            return;
        }

        internal void RemoveSelectedEntryHelpAttributes()
        {
            UpdateHelpAttributes(_selectedGridEntry, null);
        }

        private void UpdateHelpAttributes(GridEntry oldEntry, GridEntry newEntry)
        {
            // Update the help context with the current property.
            IHelpService helpService = GetHelpService();
            if (helpService is null || oldEntry == newEntry)
            {
                return;
            }

            GridEntry temp = oldEntry;
            if (oldEntry is not null && !oldEntry.Disposed)
            {
                while (temp is not null)
                {
                    helpService.RemoveContextAttribute("Keyword", temp.HelpKeyword);
                    temp = temp.ParentGridEntry;
                }
            }

            if (newEntry is not null)
            {
                temp = newEntry;

                UpdateHelpAttributes(helpService, temp, true);
            }
        }

        private void UpdateHelpAttributes(IHelpService helpService, GridEntry entry, bool addAsF1)
        {
            if (entry is null)
            {
                return;
            }

            UpdateHelpAttributes(helpService, entry.ParentGridEntry, false);
            string helpKeyword = entry.HelpKeyword;
            if (helpKeyword is not null)
            {
                helpService.AddContextAttribute("Keyword", helpKeyword, addAsF1 ? HelpKeywordType.F1Keyword : HelpKeywordType.GeneralKeyword);
            }
        }

        private void UpdateUIBasedOnFont(bool layoutRequired)
        {
            if (!IsHandleCreated || !_flags.HasFlag(Flags.NeedUpdateUIBasedOnFont))
            {
                return;
            }

            try
            {
                if (_listBox is not null)
                {
                    DropDownListBox.ItemHeight = RowHeight + 2;
                }

                if (_dropDownButton is not null)
                {
                    bool isScalingRequirementMet = DpiHelper.IsScalingRequirementMet;
                    if (isScalingRequirementMet)
                    {
                        _dropDownButton.Size = new(SystemInformation.VerticalScrollBarArrowHeightForDpi(_deviceDpi), RowHeight);
                    }
                    else
                    {
                        _dropDownButton.Size = new(SystemInformation.VerticalScrollBarArrowHeight, RowHeight);
                    }

                    if (_dialogButton is not null)
                    {
                        DialogButton.Size = DropDownButton.Size;
                        if (isScalingRequirementMet)
                        {
                            _dialogButton.Image = CreateResizedBitmap("dotdotdot", DotDotDotIconWidth, DotDotDotIconHeight);
                        }
                    }

                    if (isScalingRequirementMet)
                    {
                        _dropDownButton.Image = CreateResizedBitmap("Arrow", DownArrowIconWidth, DownArrowIconHeight);
                    }
                }

                if (layoutRequired)
                {
                    LayoutWindow(invalidate: true);
                }
            }
            finally
            {
                SetFlag(Flags.NeedUpdateUIBasedOnFont, false);
            }
        }

        private bool UnfocusSelection()
        {
            Debug.WriteLineIf(CompModSwitches.DebugGridView.TraceVerbose, "PropertyGridView:UnfocusSelection()");
            GridEntry gridEntry = GetGridEntryFromRow(_selectedRow);
            if (gridEntry is null)
            {
                return true;
            }

            bool commit = CommitEditTextBox();

            if (commit && FocusInside)
            {
                Focus();
            }

            return commit;
        }

        private void UpdateResetCommand(GridEntry gridEntry)
        {
            if (TotalProperties > 0 && TryGetService(out IMenuCommandService menuCommandService))
            {
                MenuCommand reset = menuCommandService.FindCommand(PropertyGridCommands.Reset);
                if (reset is not null)
                {
                    reset.Enabled = gridEntry is not null && gridEntry.CanResetPropertyValue();
                }
            }
        }

        internal bool WantsTab(bool forward)
        {
            if (forward)
            {
                if (Focused)
                {
                    // We want a tab if the grid has focus and we have a button or an Edit.
                    if (DropDownButton.Visible || DialogButton.Visible || EditTextBox.Visible)
                    {
                        return true;
                    }
                }
                else if (EditTextBox.Focused && (DropDownButton.Visible || DialogButton.Visible))
                {
                    // If the Edit has focus, and we have a button, we want the tab as well.
                    return true;
                }

                return OwnerGrid.WantsTab(forward);
            }
            else
            {
                if (EditTextBox.Focused || DropDownButton.Focused || DialogButton.Focused)
                {
                    return true;
                }

                return OwnerGrid.WantsTab(forward);
            }
        }

        private unsafe bool WmNotify(ref Message m)
        {
            if (m.LParam != IntPtr.Zero)
            {
                var nmhdr = (User32.NMHDR*)m.LParam;

                if (nmhdr->hwndFrom == ToolTip.Handle)
                {
                    switch ((ComCtl32.TTN)nmhdr->code)
                    {
                        case ComCtl32.TTN.POP:
                            break;
                        case ComCtl32.TTN.SHOW:
                            // We want to move the tooltip over where our text would be.
                            Point mouseLoc = Cursor.Position;
                            mouseLoc = PointToClient(mouseLoc);
                            mouseLoc = FindPosition(mouseLoc.X, mouseLoc.Y);

                            if (mouseLoc == InvalidPosition)
                            {
                                break;
                            }

                            GridEntry curEntry = GetGridEntryFromRow(mouseLoc.Y);

                            if (curEntry is null)
                            {
                                break;
                            }

                            // Get the proper rectangle.
                            Rectangle itemRect = GetRectangle(mouseLoc.Y, mouseLoc.X);
                            Point tipPt = Point.Empty;

                            // If we need a tooltip, move the tooltip control to that point.
                            if (mouseLoc.X == RowLabel)
                            {
                                tipPt = curEntry.GetLabelToolTipLocation(mouseLoc.X - itemRect.X, mouseLoc.Y - itemRect.Y);
                            }
                            else if (mouseLoc.X == RowValue)
                            {
                                tipPt = curEntry.ValueToolTipLocation;
                            }
                            else
                            {
                                break;
                            }

                            if (tipPt != InvalidPoint)
                            {
                                itemRect.Offset(tipPt);
                                PositionTooltip(this, ToolTip, itemRect);
                                m.Result = (IntPtr)1;
                                return true;
                            }

                            break;
                    }
                }
            }

            return false;
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case (int)User32.WM.SYSCOLORCHANGE:
                    Invalidate();
                    break;

                // If we get focus in the error state, make sure we push it back to the
                // Edit or bad bad things can happen with our state.
                case (int)User32.WM.SETFOCUS:
                    if (!InPropertySet && EditTextBox.Visible && (_errorState != ErrorState.None || !CommitEditTextBox()))
                    {
                        base.WndProc(ref m);
                        EditTextBox.Focus();
                        return;
                    }

                    break;

                case (int)User32.WM.IME_STARTCOMPOSITION:
                    EditTextBox.Focus();
                    EditTextBox.Clear();
                    User32.PostMessageW(EditTextBox, User32.WM.IME_STARTCOMPOSITION);
                    return;

                case (int)User32.WM.IME_COMPOSITION:
                    EditTextBox.Focus();
                    User32.PostMessageW(EditTextBox, User32.WM.IME_COMPOSITION, m.WParam, m.LParam);
                    return;

                case (int)User32.WM.GETDLGCODE:

                    int flags = (int)(User32.DLGC.WANTCHARS | User32.DLGC.WANTARROWS);

                    if (_selectedGridEntry is not null)
                    {
                        if ((ModifierKeys & Keys.Shift) == 0)
                        {
                            // If we're going backwards, we don't want the tab.
                            // Otherwise we only want it if we have an edit.
                            if (_editTextBox.Visible)
                            {
                                flags |= (int)User32.DLGC.WANTTAB;
                            }
                        }
                    }

                    m.Result = (IntPtr)flags;
                    return;

                case (int)User32.WM.MOUSEMOVE:

                    // Check if it's the same position, of so eat the message.
                    if (unchecked((int)(long)m.LParam) == _lastMouseMove)
                    {
                        return;
                    }

                    _lastMouseMove = unchecked((int)(long)m.LParam);
                    break;

                case (int)User32.WM.NOTIFY:
                    if (WmNotify(ref m))
                    {
                        return;
                    }

                    break;
                case AutomationMessages.PGM_GETSELECTEDROW:
                    m.Result = (IntPtr)GetRowFromGridEntry(_selectedGridEntry);
                    return;
                case AutomationMessages.PGM_GETVISIBLEROWCOUNT:
                    m.Result = (IntPtr)Math.Min(_visibleRows, TotalProperties);
                    return;
            }

            base.WndProc(ref m);
        }

        /// <summary>
        ///  Rescale constants for the DPI change
        /// </summary>
        protected override void RescaleConstantsForDpi(int deviceDpiOld, int deviceDpiNew)
        {
            base.RescaleConstantsForDpi(deviceDpiOld, deviceDpiNew);
            RescaleConstants();
        }

        /// <summary>
        ///  Rescale constants on this object.
        /// </summary>
        private void RescaleConstants()
        {
            if (DpiHelper.IsScalingRequirementMet)
            {
                _cachedRowHeight = -1;
                _paintWidth = LogicalToDeviceUnits(PaintWidth);
                _paintIndent = LogicalToDeviceUnits(PaintIndent);
                _outlineSizeExplorerTreeStyle = LogicalToDeviceUnits(OutlineSizeExplorerTreeStyle);
                _outlineSize = LogicalToDeviceUnits(OutlineSize);
                _maxListBoxHeight = LogicalToDeviceUnits(MaxListBoxHeight);
                _offset2Units = LogicalToDeviceUnits(Offset2Pixels);
                if (TopLevelGridEntries is not null)
                {
                    foreach (GridEntry t in TopLevelGridEntries)
                    {
                        ResetOutline(t);
                    }
                }
            }
        }

        /// <summary>
        ///  Recursively reset outline rect for grid entries (both visible and invisible)
        /// </summary>
        private void ResetOutline(GridEntry entry)
        {
            entry.OutlineRect = Rectangle.Empty;
            if (entry.ChildCount > 0)
            {
                foreach (GridEntry ent in entry.Children)
                {
                    ResetOutline(ent);
                }
            }

            return;
        }
    }
}
