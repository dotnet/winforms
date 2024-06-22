// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Forms.Layout;
using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;
using static System.Windows.Forms.ComboBox.ObjectCollection;

namespace System.Windows.Forms;

/// <summary>
///  Displays an editing field and a list, allowing the user to select from the list or to enter new text. Displays
///  only the editing field until the user explicitly displays the list.
/// </summary>
[DefaultEvent(nameof(SelectedIndexChanged))]
[DefaultProperty(nameof(Items))]
[DefaultBindingProperty(nameof(Text))]
[Designer($"System.Windows.Forms.Design.ComboBoxDesigner, {AssemblyRef.SystemDesign}")]
[SRDescription(nameof(SR.DescriptionComboBox))]
public partial class ComboBox : ListControl
{
    private static readonly object s_dropDownEvent = new();
    private static readonly object s_drawItemEvent = new();
    private static readonly object s_measureItemEvent = new();
    private static readonly object s_selectedIndexChangedEvent = new();
    private static readonly object s_selectionChangedComittedEvent = new();
    private static readonly object s_selectedItemChangedEvent = new();
    private static readonly object s_dropDownStyleEvent = new();
    private static readonly object s_textUpdateEvent = new();
    private static readonly object s_dropDownClosedEvent = new();

    private static readonly int s_propMaxLength = PropertyStore.CreateKey();
    private static readonly int s_propItemHeight = PropertyStore.CreateKey();
    private static readonly int s_propDropDownWidth = PropertyStore.CreateKey();
    private static readonly int s_propDropDownHeight = PropertyStore.CreateKey();
    private static readonly int s_propStyle = PropertyStore.CreateKey();
    private static readonly int s_propDrawMode = PropertyStore.CreateKey();
    private static readonly int s_propMatchingText = PropertyStore.CreateKey();
    private static readonly int s_propFlatComboAdapter = PropertyStore.CreateKey();

    private const int DefaultSimpleStyleHeight = 150;
    private const int DefaultDropDownHeight = 106;
    private const int AutoCompleteTimeout = 10000000; // 1 second timeout for resetting the MatchingText
    private bool _autoCompleteDroppedDown;

    private FlatStyle _flatStyle = FlatStyle.Standard;
    private int _updateCount;

    // Timestamp of the last keystroke. Used for auto-completion in DropDownList style.
    private long _autoCompleteTimeStamp;

    private int _selectedIndex = -1;  // used when we don't have a handle.
    private bool _allowCommit = true;

    // When the style is "simple", the requested height is used for the actual height of the control. When the
    // style is non-simple, the height of the control is determined by the OS.
    private int _requestedHeight;

    private ComboBoxChildNativeWindow? _childDropDown;
    private ComboBoxChildNativeWindow? _childEdit;
    private ComboBoxChildNativeWindow? _childListBox;

    private HWND _dropDownHandle;
    private ObjectCollection? _itemsCollection;
    private short _prefHeightCache = -1;
    private short _maxDropDownItems = 8;
    private bool _integralHeight = true;
    private bool _mousePressed;
    private bool _mouseEvents;
    private bool _mouseInEdit;

    private bool _sorted;
    private bool _fireSetFocus = true;
    private bool _fireLostFocus = true;
    private bool _mouseOver;
    private bool _suppressNextWindowsPos;
    private bool _canFireLostFocus;

    // When the user types a letter and drops the dropdown the combobox itself auto-searches the matching item and
    // selects the item in the edit thus changing the windowText. Hence we should Fire the TextChanged event in
    // such a scenario. The string below is used for checking the window Text before and after the dropdown.
    private string _currentText = string.Empty;
    private string? _lastTextChangedValue;
    private bool _dropDown;
    private readonly AutoCompleteDropDownFinder _finder = new();

    private bool _selectedValueChangedFired;
    private AutoCompleteMode _autoCompleteMode = AutoCompleteMode.None;
    private AutoCompleteSource _autoCompleteSource = AutoCompleteSource.None;

    /// <summary>
    ///  This stores the custom StringCollection required for the autoCompleteSource when its set to CustomSource.
    /// </summary>
    private AutoCompleteStringCollection? _autoCompleteCustomSource;
    private StringSource? _stringSource;
    private bool _fromHandleCreate;

    private ComboBoxChildListUiaProvider? _childListAccessibleObject;
    private ComboBoxChildEditUiaProvider? _childEditAccessibleObject;
    private ComboBoxChildTextUiaProvider? _childTextAccessibleObject;

    // Indicates whether the dropdown list will be closed after selection (on getting CBN_SELENDOK notification)
    // to prevent focusing on the list item after hiding the list.
    private bool _dropDownWillBeClosed;

    /// <summary>
    ///  Creates a new ComboBox control.  The default style for the combo is
    ///  a regular DropDown Combo.
    /// </summary>
    public ComboBox()
    {
        SetStyle(ControlStyles.UserPaint |
                 ControlStyles.UseTextForAccessibility |
                 ControlStyles.StandardClick, false);

        _requestedHeight = DefaultSimpleStyleHeight;

        // this class overrides GetPreferredSizeCore, let Control automatically cache the result
        SetExtendedState(ExtendedStates.UserPreferredSizeCache, true);
    }

    /// <summary>
    ///  This is the AutoCompleteMode which can be either
    ///  None, AutoSuggest, AutoAppend or AutoSuggestAppend.
    ///  This property in conjunction with AutoCompleteSource enables the AutoComplete feature for ComboBox.
    /// </summary>
    [DefaultValue(AutoCompleteMode.None)]
    [SRDescription(nameof(SR.ComboBoxAutoCompleteModeDescr))]
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    public AutoCompleteMode AutoCompleteMode
    {
        get
        {
            return _autoCompleteMode;
        }
        set
        {
            // valid values are 0x0 to 0x3
            SourceGenerated.EnumValidator.Validate(value);
            if (DropDownStyle == ComboBoxStyle.DropDownList
                && AutoCompleteSource != AutoCompleteSource.ListItems
                && value != AutoCompleteMode.None)
            {
                throw new NotSupportedException(SR.ComboBoxAutoCompleteModeOnlyNoneAllowed);
            }

            if (Application.OleRequired() != ApartmentState.STA)
            {
                throw new ThreadStateException(SR.ThreadMustBeSTA);
            }

            bool resetAutoComplete = false;
            if (_autoCompleteMode != AutoCompleteMode.None && value == AutoCompleteMode.None)
            {
                resetAutoComplete = true;
            }

            _autoCompleteMode = value;
            SetAutoComplete(resetAutoComplete, true);
        }
    }

    /// <summary>
    ///  This is the AutoCompleteSource which can be one of the
    ///  values from AutoCompleteSource enumeration.
    ///  This property in conjunction with AutoCompleteMode enables the AutoComplete feature for ComboBox.
    /// </summary>
    [DefaultValue(AutoCompleteSource.None)]
    [SRDescription(nameof(SR.ComboBoxAutoCompleteSourceDescr))]
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    public AutoCompleteSource AutoCompleteSource
    {
        get
        {
            return _autoCompleteSource;
        }
        set
        {
            SourceGenerated.EnumValidator.Validate(value);

            if (DropDownStyle == ComboBoxStyle.DropDownList
                && AutoCompleteMode != AutoCompleteMode.None
                && value != AutoCompleteSource.ListItems)
            {
                throw new NotSupportedException(SR.ComboBoxAutoCompleteSourceOnlyListItemsAllowed);
            }

            if (Application.OleRequired() != ApartmentState.STA)
            {
                throw new ThreadStateException(SR.ThreadMustBeSTA);
            }

            _autoCompleteSource = value;
            SetAutoComplete(false, true);
        }
    }

    /// <summary>
    ///  This is the AutoCompleteCustomSource which is custom StringCollection used when the
    ///  AutoCompleteSource is CustomSource.
    /// </summary>
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [Localizable(true)]
    [SRDescription(nameof(SR.ComboBoxAutoCompleteCustomSourceDescr))]
    [Editor($"System.Windows.Forms.Design.ListControlStringCollectionEditor, {AssemblyRef.SystemDesign}", typeof(UITypeEditor))]
    [Browsable(true)]
    [AllowNull]
    [EditorBrowsable(EditorBrowsableState.Always)]
    public AutoCompleteStringCollection AutoCompleteCustomSource
    {
        get
        {
            if (_autoCompleteCustomSource is null)
            {
                _autoCompleteCustomSource = [];
                _autoCompleteCustomSource.CollectionChanged += new CollectionChangeEventHandler(OnAutoCompleteCustomSourceChanged);
            }

            return _autoCompleteCustomSource;
        }
        set
        {
            if (_autoCompleteCustomSource == value)
            {
                return;
            }

            if (_autoCompleteCustomSource is not null)
            {
                _autoCompleteCustomSource.CollectionChanged -= new CollectionChangeEventHandler(OnAutoCompleteCustomSourceChanged);
            }

            _autoCompleteCustomSource = value;

            if (_autoCompleteCustomSource is not null)
            {
                _autoCompleteCustomSource.CollectionChanged += new CollectionChangeEventHandler(OnAutoCompleteCustomSourceChanged);
            }

            SetAutoComplete(false, true);
        }
    }

    /// <summary>
    ///  The background color of this control. This is an ambient property and
    ///  will always return a non-null value.
    /// </summary>
    public override Color BackColor
    {
        get
        {
            if (ShouldSerializeBackColor())
            {
                return base.BackColor;
            }
            else
            {
                return Application.SystemColors.Window;
            }
        }
        set => base.BackColor = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override Image? BackgroundImage
    {
        get => base.BackgroundImage;
        set => base.BackgroundImage = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override ImageLayout BackgroundImageLayout
    {
        get => base.BackgroundImageLayout;
        set => base.BackgroundImageLayout = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? BackgroundImageChanged
    {
        add => base.BackgroundImageChanged += value;
        remove => base.BackgroundImageChanged -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? BackgroundImageLayoutChanged
    {
        add => base.BackgroundImageLayoutChanged += value;
        remove => base.BackgroundImageLayoutChanged -= value;
    }

    internal ChildAccessibleObject ChildEditAccessibleObject
    {
        get
        {
            _childEditAccessibleObject ??= new ComboBoxChildEditUiaProvider(this, _childEdit!.HWND);

            return _childEditAccessibleObject;
        }
    }

    internal ChildAccessibleObject ChildListAccessibleObject
    {
        get
        {
            _childListAccessibleObject ??=
                    new ComboBoxChildListUiaProvider(this, DropDownStyle == ComboBoxStyle.Simple ? _childListBox!.HWND : _dropDownHandle);

            return _childListAccessibleObject;
        }
    }

    internal AccessibleObject ChildTextAccessibleObject
    {
        get
        {
            _childTextAccessibleObject ??= new ComboBoxChildTextUiaProvider(this);

            return _childTextAccessibleObject;
        }
    }

    internal void ClearChildEditAccessibleObject() => _childEditAccessibleObject = null;

    internal void ClearChildListAccessibleObject() => _childListAccessibleObject = null;

    protected override CreateParams CreateParams
    {
        get
        {
            CreateParams cp = base.CreateParams;
            cp.ClassName = PInvoke.WC_COMBOBOX;
            cp.Style |= (int)WINDOW_STYLE.WS_VSCROLL | PInvoke.CBS_HASSTRINGS | PInvoke.CBS_AUTOHSCROLL;
            cp.ExStyle |= (int)WINDOW_EX_STYLE.WS_EX_CLIENTEDGE;
            if (!_integralHeight)
            {
                cp.Style |= PInvoke.CBS_NOINTEGRALHEIGHT;
            }

            switch (DropDownStyle)
            {
                case ComboBoxStyle.Simple:
                    cp.Style |= PInvoke.CBS_SIMPLE;
                    break;
                case ComboBoxStyle.DropDown:
                    cp.Style |= PInvoke.CBS_DROPDOWN;
                    // Make sure we put the height back or we won't be able to size the dropdown!
                    cp.Height = PreferredHeight;
                    break;
                case ComboBoxStyle.DropDownList:
                    cp.Style |= PInvoke.CBS_DROPDOWNLIST;
                    // Comment above...
                    cp.Height = PreferredHeight;
                    break;
            }

            switch (DrawMode)
            {
                case DrawMode.OwnerDrawFixed:
                    cp.Style |= PInvoke.CBS_OWNERDRAWFIXED;
                    break;
                case DrawMode.OwnerDrawVariable:
                    cp.Style |= PInvoke.CBS_OWNERDRAWVARIABLE;
                    break;
            }

            return cp;
        }
    }

    /// <summary>
    ///  Deriving classes can override this to configure a default size for their control.
    ///  This is more efficient than setting the size in the control's constructor.
    /// </summary>
    protected override Size DefaultSize
    {
        get
        {
            return new Size(121, PreferredHeight);
        }
    }

    /// <summary>
    ///  The ListSource to consume as this ListBox's source of data.
    ///  When set, a user can not modify the Items collection.
    /// </summary>
    [SRCategory(nameof(SR.CatData))]
    [DefaultValue(null)]
    [RefreshProperties(RefreshProperties.Repaint),
    AttributeProvider(typeof(IListSource))]
    [SRDescription(nameof(SR.ListControlDataSourceDescr))]
    public new object? DataSource
    {
        get => base.DataSource;
        set => base.DataSource = value;
    }

    /// <summary>
    ///  Retrieves the value of the DrawMode property.  The DrawMode property
    ///  controls whether the control is drawn by Windows or by the user.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(DrawMode.Normal)]
    [SRDescription(nameof(SR.ComboBoxDrawModeDescr))]
    [RefreshProperties(RefreshProperties.Repaint)]
    public DrawMode DrawMode
    {
        get
        {
            int drawMode = Properties.GetInteger(s_propDrawMode, out bool found);
            if (found)
            {
                return (DrawMode)drawMode;
            }

            return DrawMode.Normal;
        }
        set
        {
            if (DrawMode != value)
            {
                // valid values are 0x0 to 0x2.
                SourceGenerated.EnumValidator.Validate(value);
                ResetHeightCache();
                Properties.SetInteger(s_propDrawMode, (int)value);
                RecreateHandle();
            }
        }
    }

    /// <summary>
    ///  Returns the width of the drop down box in a combo box.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.ComboBoxDropDownWidthDescr))]
    public int DropDownWidth
    {
        get
        {
            int dropDownWidth = Properties.GetInteger(s_propDropDownWidth, out bool found);
            return found ? dropDownWidth : Width;
        }
        set
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value);

            if (Properties.GetInteger(s_propDropDownWidth) != value)
            {
                Properties.SetInteger(s_propDropDownWidth, value);
                if (IsHandleCreated)
                {
                    PInvoke.SendMessage(this, PInvoke.CB_SETDROPPEDWIDTH, (WPARAM)value);
                }
            }
        }
    }

    /// <summary>
    ///  Sets the Height of the drop down box in a combo box.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.ComboBoxDropDownHeightDescr))]
    [Browsable(true)]
    [EditorBrowsable(EditorBrowsableState.Always)]
    [DefaultValue(106)]
    public int DropDownHeight
    {
        get
        {
            int dropDownHeight = Properties.GetInteger(s_propDropDownHeight, out bool found);
            if (found)
            {
                return dropDownHeight;
            }
            else
            {
                return DefaultDropDownHeight;
            }
        }
        set
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value);

            if (Properties.GetInteger(s_propDropDownHeight) != value)
            {
                Properties.SetInteger(s_propDropDownHeight, value);

                // The dropDownHeight is not reflected unless the
                // combobox integralHeight == false..
                IntegralHeight = false;
            }
        }
    }

    /// <summary>
    ///  Indicates whether the DropDown of the combo is  currently dropped down.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.ComboBoxDroppedDownDescr))]
    public bool DroppedDown
    {
        get
        {
            if (IsHandleCreated)
            {
                return (int)PInvoke.SendMessage(this, PInvoke.CB_GETDROPPEDSTATE) != 0;
            }

            return false;
        }
        set
        {
            if (!IsHandleCreated)
            {
                CreateHandle();
            }

            PInvoke.SendMessage(this, PInvoke.CB_SHOWDROPDOWN, (WPARAM)(value ? -1 : 0));
        }
    }

    /// <summary>
    ///  Gets or sets the flat style appearance of the button control.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [DefaultValue(FlatStyle.Standard)]
    [Localizable(true)]
    [SRDescription(nameof(SR.ComboBoxFlatStyleDescr))]
    public FlatStyle FlatStyle
    {
        get
        {
            return _flatStyle;
        }
        set
        {
            // valid values are 0x0 to 0x3
            SourceGenerated.EnumValidator.Validate(value);
            _flatStyle = value;
            Invalidate();
        }
    }

    /// <summary>
    ///  Returns true if this control has focus.
    /// </summary>
    public override bool Focused
    {
        get
        {
            if (base.Focused)
            {
                return true;
            }

            HWND focus = PInvoke.GetFocus();
            return !focus.IsNull
                && ((_childEdit is not null && focus == _childEdit.Handle) || (_childListBox is not null && focus == _childListBox.Handle));
        }
    }

    /// <summary>
    ///  Gets or sets the foreground color of the control.
    /// </summary>
    public override Color ForeColor
    {
        get
        {
            if (ShouldSerializeForeColor())
            {
                return base.ForeColor;
            }
            else
            {
                return Application.SystemColors.WindowText;
            }
        }
        set => base.ForeColor = value;
    }

    /// <summary>
    ///  Indicates if the combo should avoid showing partial Items.  If so,
    ///  then only full items will be displayed, and the list portion will be resized
    ///  to prevent partial items from being shown.  Otherwise, they will be
    ///  shown
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(true)]
    [Localizable(true)]
    [SRDescription(nameof(SR.ComboBoxIntegralHeightDescr))]
    public bool IntegralHeight
    {
        get
        {
            return _integralHeight;
        }
        set
        {
            if (_integralHeight != value)
            {
                _integralHeight = value;
                RecreateHandle();
            }
        }
    }

    /// <summary>
    ///  Returns the height of an item in the combo box. When drawMode is Normal
    ///  or OwnerDrawFixed, all items have the same height. When drawMode is
    ///  OwnerDrawVariable, this method returns the height that will be given
    ///  to new items added to the combo box. To determine the actual height of
    ///  an item, use the GetItemHeight() method with an integer parameter.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [Localizable(true)]
    [SRDescription(nameof(SR.ComboBoxItemHeightDescr))]
    public int ItemHeight
    {
        get
        {
            DrawMode drawMode = DrawMode;
            if (drawMode == DrawMode.OwnerDrawFixed ||
                drawMode == DrawMode.OwnerDrawVariable ||
                !IsHandleCreated)
            {
                int itemHeight = Properties.GetInteger(s_propItemHeight, out bool found);
                if (found)
                {
                    return itemHeight;
                }
                else
                {
                    return FontHeight + 2;
                }
            }

            // Note that the above if clause deals with the case when the handle has not yet been created
            Debug.Assert(IsHandleCreated, "Handle should be created at this point");

            int h = (int)PInvoke.SendMessage(this, PInvoke.CB_GETITEMHEIGHT);
            if (h == -1)
            {
                throw new Win32Exception();
            }

            return h;
        }
        set
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value);

            ResetHeightCache();

            if (Properties.GetInteger(s_propItemHeight) != value)
            {
                Properties.SetInteger(s_propItemHeight, value);
                if (DrawMode != DrawMode.Normal)
                {
                    UpdateItemHeight();
                }
            }
        }
    }

    /// <summary>
    ///  Collection of the items contained in this ComboBox.
    /// </summary>
    [SRCategory(nameof(SR.CatData))]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
    [Localizable(true)]
    [SRDescription(nameof(SR.ComboBoxItemsDescr))]
    [Editor($"System.Windows.Forms.Design.ListControlStringCollectionEditor, {AssemblyRef.SystemDesign}", typeof(UITypeEditor))]
    [MergableProperty(false)]
    public ObjectCollection Items
    {
        get
        {
            _itemsCollection ??= new ObjectCollection(this);

            return _itemsCollection;
        }
    }

    // Text used to match an item in the list when auto-completion
    // is used in DropDownList style.
    private string MatchingText
    {
        get
        {
            string? matchingText = (string?)Properties.GetObject(s_propMatchingText);
            return matchingText ?? string.Empty;
        }
        set
        {
            if (value is not null || Properties.ContainsObject(s_propMatchingText))
            {
                Properties.SetObject(s_propMatchingText, value);
            }
        }
    }

    /// <summary>
    ///  The maximum number of items to be shown in the dropdown portion
    ///  of the ComboBox.  This number can be between 1 and 100.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(8)]
    [Localizable(true)]
    [SRDescription(nameof(SR.ComboBoxMaxDropDownItemsDescr))]
    public int MaxDropDownItems
    {
        get
        {
            return _maxDropDownItems;
        }
        set
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value);
            ArgumentOutOfRangeException.ThrowIfGreaterThan(value, 100);

            _maxDropDownItems = (short)value;
        }
    }

    public override Size MaximumSize
    {
        get => base.MaximumSize;
        set => base.MaximumSize = new Size(value.Width, 0);
    }

    public override Size MinimumSize
    {
        get => base.MinimumSize;
        set => base.MinimumSize = new Size(value.Width, 0);
    }

    /// <summary>
    ///  The maximum length of the text the user may type into the edit control
    ///  of a combo box.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(0)]
    [Localizable(true)]
    [SRDescription(nameof(SR.ComboBoxMaxLengthDescr))]
    public int MaxLength
    {
        get
        {
            return Properties.GetInteger(s_propMaxLength);
        }
        set
        {
            if (value < 0)
            {
                value = 0;
            }

            if (MaxLength != value)
            {
                Properties.SetInteger(s_propMaxLength, value);
                if (IsHandleCreated)
                {
                    PInvoke.SendMessage(this, PInvoke.CB_LIMITTEXT, (WPARAM)value);
                }
            }
        }
    }

    /// <summary>
    ///  If the mouse is over the combobox, draw selection rect.
    /// </summary>
    internal bool MouseIsOver
    {
        get { return _mouseOver; }
        set
        {
            if (_mouseOver != value)
            {
                _mouseOver = value;
                // Nothing to see here... Just keep on walking...
                // Turns out that with Theming off, we don't get quite the same messages as with theming on, so
                // our drawing gets a little messed up. So in case theming is off, force a draw here.
                if ((!ContainsFocus || !Application.RenderWithVisualStyles) && FlatStyle == FlatStyle.Popup)
                {
                    Invalidate();
                    Update();
                }
            }
        }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public new Padding Padding
    {
        get => base.Padding;
        set => base.Padding = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? PaddingChanged
    {
        add => base.PaddingChanged += value;
        remove => base.PaddingChanged -= value;
    }

    /// <summary>
    ///  ApplySizeConstraints calls into this method when DropDownStyles is DropDown and DropDownList.
    ///  This causes PreferredSize to be bounded by PreferredHeight in these two cases only.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.ComboBoxPreferredHeightDescr))]
    public int PreferredHeight
    {
        get
        {
            if (!FormattingEnabled)
            {
                // do preferred height the old broken way for everett apps
                // we need this for compat reasons because (get this)
                //  (a) everett PreferredHeight was always wrong.
                //  (b) so, when combobox1.Size = actualdefaultsize was called, it would enter setboundscore
                //  (c) this updated requestedheight
                //  (d) if the user then changed the combo to simple style, the height did not change.
                // We simply cannot match this behavior if PreferredHeight is corrected so that (b) never
                // occurs.  We simply do not know when Size was set.

                // So in whidbey, the behavior will be:
                //  (1) user uses default size = setting dropdownstyle=simple will revert to simple height
                //  (2) user uses nondefault size = setting dropdownstyle=simple will not change height from this value

                // In everett
                //  if the user manually sets Size = (121, 20) in code (usually height gets forced to 21), then he will see Whidbey.(1) above
                //  user usually uses nondefault size and will experience whidbey.(2) above

                Size textSize = TextRenderer.MeasureText(LayoutUtils.TestString, Font, new Size(short.MaxValue, (int)(FontHeight * 1.25)), TextFormatFlags.SingleLine);
                _prefHeightCache = (short)(textSize.Height + SystemInformation.BorderSize.Height * 8 + Padding.Size.Height);

                return _prefHeightCache;
            }

            // Normally we do this sort of calculation in GetPreferredSizeCore which has builtin
            // caching, but in this case we can not because PreferredHeight is used in ApplySizeConstraints
            // which is used by GetPreferredSize (infinite loop).
            if (_prefHeightCache < 0)
            {
                Size textSize = TextRenderer.MeasureText(LayoutUtils.TestString, Font, new Size(short.MaxValue, (int)(FontHeight * 1.25)), TextFormatFlags.SingleLine);

                // For a "simple" style combobox, the preferred height depends on the
                // number of items in the combobox.
                if (DropDownStyle == ComboBoxStyle.Simple)
                {
                    int itemCount = Items.Count + 1;
                    _prefHeightCache = (short)(textSize.Height * itemCount + SystemInformation.BorderSize.Height * 16 + Padding.Size.Height);
                }
                else
                {
                    // We do this old school rather than use SizeFromClientSize because CreateParams calls this
                    // method and SizeFromClientSize calls CreateParams (another infinite loop.)
                    _prefHeightCache = (short)GetComboHeight();
                }
            }

            return _prefHeightCache;
        }
    }

    // ComboBox.PreferredHeight returns incorrect values
    // This is translated from windows implementation.  Since we cannot control the size
    // of the combo box, we need to use the same calculation they do.
    private int GetComboHeight()
    {
        int cyCombo = 0;
        // Add on CYEDGE just for some extra space in the edit field/static item.
        // It's really only for static text items, but we want static & editable
        // controls to be the same height.
        Size textExtent = Size.Empty;

        using (var hfont = GdiCache.GetHFONT(Font))
        using (var screen = GdiCache.GetScreenHdc())
        {
            // this is the character that Windows uses to determine the extent
            textExtent = screen.HDC.GetTextExtent("0", hfont);
        }

        int dyEdit = textExtent.Height + SystemInformation.Border3DSize.Height;

        if (DrawMode != DrawMode.Normal)
        {
            // This is an ownerdraw combo.  Have the owner tell us how tall this
            // item is.
            dyEdit = ItemHeight;
        }

        // Set the initial width to be the combo box rect.  Later we will shorten it
        // if there is a dropdown button.
        Size fixedFrameBoderSize = SystemInformation.FixedFrameBorderSize;
        cyCombo = 2 * fixedFrameBoderSize.Height + dyEdit;
        return cyCombo;
    }

    private string[] GetStringsForAutoComplete()
    {
        if (Items is not null)
        {
            string[] strings = new string[Items.Count];
            for (int i = 0; i < Items.Count; i++)
            {
                strings[i] = GetItemText(Items[i])!;
            }

            return strings;
        }

        return [];
    }

    /// <summary>
    ///  The [zero based] index of the currently selected item in the combos list.
    ///  Note If the value of index is -1, then the ComboBox is
    ///  set to have no selection.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.ComboBoxSelectedIndexDescr))]
    public override int SelectedIndex
    {
        get
        {
            if (IsHandleCreated)
            {
                return (int)PInvoke.SendMessage(this, PInvoke.CB_GETCURSEL);
            }

            return _selectedIndex;
        }
        set
        {
            if (SelectedIndex == value)
            {
                return;
            }

            ArgumentOutOfRangeException.ThrowIfLessThan(value, -1);
            ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(value, _itemsCollection?.Count ?? 0);

            if (IsHandleCreated)
            {
                PInvoke.SendMessage(this, PInvoke.CB_SETCURSEL, (WPARAM)value);
            }
            else
            {
                _selectedIndex = value;
            }

            UpdateText();

            if (IsHandleCreated)
            {
                OnTextChanged(EventArgs.Empty);
            }

            OnSelectedItemChanged(EventArgs.Empty);
            OnSelectedIndexChanged(EventArgs.Empty);
        }
    }

    /// <summary>
    ///  The handle to the object that is currently selected in the
    ///  combos list.
    /// </summary>
    [Browsable(false)]
    [Bindable(true)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.ComboBoxSelectedItemDescr))]
    public object? SelectedItem
    {
        get
        {
            int index = SelectedIndex;
            return (index == -1) ? null : Items[index];
        }
        set
        {
            int x = -1;

            if (_itemsCollection is not null)
            {
                if (value is not null)
                {
                    x = _itemsCollection.IndexOf(value);
                }
                else
                {
                    SelectedIndex = -1;
                }
            }

            if (x != -1)
            {
                SelectedIndex = x;
            }
        }
    }

    /// <summary>
    ///  The selected text in the edit component of the ComboBox. If the
    ///  ComboBox has ComboBoxStyle.DROPDOWNLIST, the return is an empty
    ///  string ("").
    /// </summary>
    [Browsable(false)]
    [AllowNull]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.ComboBoxSelectedTextDescr))]
    public string SelectedText
    {
        get
        {
            if (DropDownStyle == ComboBoxStyle.DropDownList)
            {
                return string.Empty;
            }

            return Text.Substring(SelectionStart, SelectionLength);
        }
        set
        {
            if (DropDownStyle != ComboBoxStyle.DropDownList)
            {
                CreateControl();
                if (IsHandleCreated && _childEdit is not null)
                {
                    PInvoke.SendMessage(_childEdit, PInvoke.EM_REPLACESEL, (WPARAM)(-1), value ?? string.Empty);
                }
            }
        }
    }

    /// <summary>
    ///  The length, in characters, of the selection in the editbox.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.ComboBoxSelectionLengthDescr))]
    public unsafe int SelectionLength
    {
        get
        {
            int end = 0;
            int start = 0;
            PInvoke.SendMessage(this, PInvoke.CB_GETEDITSEL, (WPARAM)(&start), (LPARAM)(&end));
            return end - start;
        }
        set
        {
            // SelectionLength can be negative...
            Select(SelectionStart, value);
        }
    }

    /// <summary>
    ///  The [zero-based] index of the first character in the current text selection.
    /// </summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    [SRDescription(nameof(SR.ComboBoxSelectionStartDescr))]
    public unsafe int SelectionStart
    {
        get
        {
            int value = 0;
            PInvoke.SendMessage(this, PInvoke.CB_GETEDITSEL, (WPARAM)(&value));
            return value;
        }
        set
        {
            ArgumentOutOfRangeException.ThrowIfNegative(value);

            Select(value, SelectionLength);
        }
    }

    /// <summary>
    ///  Indicates if the Combos list is sorted or not.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [DefaultValue(false)]
    [SRDescription(nameof(SR.ComboBoxSortedDescr))]
    public bool Sorted
    {
        get
        {
            return _sorted;
        }
        set
        {
            if (_sorted != value)
            {
                if (DataSource is not null && value)
                {
                    throw new ArgumentException(SR.ComboBoxSortWithDataSource);
                }

                _sorted = value;
                RefreshItems();
                SelectedIndex = -1;
            }
        }
    }

    /// <summary>
    ///  The type of combo that we are right now.  The value would come
    ///  from the System.Windows.Forms.ComboBoxStyle enumeration.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [DefaultValue(ComboBoxStyle.DropDown)]
    [SRDescription(nameof(SR.ComboBoxStyleDescr))]
    [RefreshProperties(RefreshProperties.Repaint)]
    public ComboBoxStyle DropDownStyle
    {
        get
        {
            int style = Properties.GetInteger(s_propStyle, out bool found);
            if (found)
            {
                return (ComboBoxStyle)style;
            }

            return ComboBoxStyle.DropDown;
        }
        set
        {
            if (DropDownStyle == value)
            {
                return;
            }

            // verify that 'value' is a valid enum type...
            // valid values are 0x0 to 0x2
            SourceGenerated.EnumValidator.Validate(value);

            if (value == ComboBoxStyle.DropDownList
                && AutoCompleteSource != AutoCompleteSource.ListItems
                && AutoCompleteMode != AutoCompleteMode.None)
            {
                AutoCompleteMode = AutoCompleteMode.None;
            }

            // reset preferred height.
            ResetHeightCache();

            Properties.SetInteger(s_propStyle, (int)value);

            if (IsHandleCreated)
            {
                RecreateHandle();
            }

            OnDropDownStyleChanged(EventArgs.Empty);
        }
    }

    [Localizable(true)]
    [Bindable(true)]
    [AllowNull]
    public override string Text
    {
        get
        {
            if (SelectedItem is not null && !BindingFieldEmpty)
            {
                // preserve everett behavior if "formatting enabled == false" -- just return selecteditem text.
                if (FormattingEnabled)
                {
                    string? candidate = GetItemText(SelectedItem);
                    if (!string.IsNullOrEmpty(candidate))
                    {
                        if (string.Compare(candidate, base.Text, true, CultureInfo.CurrentCulture) == 0)
                        {
                            return candidate;   // for whidbey, if we only differ by case -- return the candidate;
                        }
                    }
                }
                else
                {
                    return FilterItemOnProperty(SelectedItem)!.ToString()!;       // heinous.
                }
            }

            return base.Text;
        }
        set
        {
            if (DropDownStyle == ComboBoxStyle.DropDownList && !IsHandleCreated && !string.IsNullOrEmpty(value) && FindStringExact(value) == -1)
            {
                return;
            }

            base.Text = value;
            object? selectedItem = SelectedItem;

            if (!DesignMode)
            {
                if (value is null)
                {
                    SelectedIndex = -1;
                }
                else if (value is not null
                    && (selectedItem is null
                        || string.Compare(value, GetItemText(selectedItem), ignoreCase: false, CultureInfo.CurrentCulture) != 0))
                {
                    int index = FindStringIgnoreCase(value);

                    // we cannot set the index to -1 unless we want to do something unusual and save/restore text
                    // because the native control will erase the text when we change the index to -1
                    if (index != -1)
                    {
                        SelectedIndex = index;
                    }
                }
            }
        }
    }

    private int FindStringIgnoreCase(string value)
    {
        // look for an exact match and then a case insensitive match if that fails.
        int index = FindStringExact(value, -1, false);

        if (index == -1)
        {
            index = FindStringExact(value, -1, true);
        }

        return index;
    }

    // Special AutoComplete notification handling
    // If the text changed, this will fire TextChanged
    // If it matches an item in the list, this will fire SIC and SVC
    private void NotifyAutoComplete()
    {
        NotifyAutoComplete(true);
    }

    // Special AutoComplete notification handling
    // If the text changed, this will fire TextChanged
    // If it matches an item in the list, this will fire SIC and SVC
    private void NotifyAutoComplete(bool setSelectedIndex)
    {
        string text = Text;
        bool textChanged = (text != _lastTextChangedValue);
        bool selectedIndexSet = false;

        if (setSelectedIndex)
        {
            // Process return key.  This is sent by the AutoComplete DropDown when a
            // selection is made from the DropDown
            // Check to see if the Text Changed.  If so, at least fire a TextChanged
            int index = FindStringIgnoreCase(text);

            if ((index != -1) && (index != SelectedIndex))
            {
                // We found a match, do the full monty
                SelectedIndex = index;

                // Put the cursor at the end
                SelectionStart = 0;
                SelectionLength = text.Length;

                selectedIndexSet = true;
            }
        }

        // don't fire TextChanged if we had set the selectedindex -- because it was already fired if so.
        if (textChanged && !selectedIndexSet)
        {
            // No match, just fire a TextChanged
            OnTextChanged(EventArgs.Empty);
        }

        // Save the new value
        _lastTextChangedValue = text;
    }

    internal override bool SupportsUiaProviders => true;

    // Returns true if using System AutoComplete
    private bool SystemAutoCompleteEnabled
    {
        get
        {
            return ((_autoCompleteMode != AutoCompleteMode.None) && (DropDownStyle != ComboBoxStyle.DropDownList));
        }
    }

    // Prevent this event from being displayed in the Property Grid.
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? DoubleClick
    {
        add => base.DoubleClick += value;
        remove => base.DoubleClick -= value;
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.drawItemEventDescr))]
    public event DrawItemEventHandler? DrawItem
    {
        add => Events.AddHandler(s_drawItemEvent, value);
        remove => Events.RemoveHandler(s_drawItemEvent, value);
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.ComboBoxOnDropDownDescr))]
    public event EventHandler? DropDown
    {
        add => Events.AddHandler(s_dropDownEvent, value);
        remove => Events.RemoveHandler(s_dropDownEvent, value);
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.measureItemEventDescr))]
    public event MeasureItemEventHandler? MeasureItem
    {
        add
        {
            Events.AddHandler(s_measureItemEvent, value);
            UpdateItemHeight();
        }
        remove
        {
            Events.RemoveHandler(s_measureItemEvent, value);
            UpdateItemHeight();
        }
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.selectedIndexChangedEventDescr))]
    public event EventHandler? SelectedIndexChanged
    {
        add => Events.AddHandler(s_selectedIndexChangedEvent, value);
        remove => Events.RemoveHandler(s_selectedIndexChangedEvent, value);
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.selectionChangeCommittedEventDescr))]
    public event EventHandler? SelectionChangeCommitted
    {
        add => Events.AddHandler(s_selectionChangedComittedEvent, value);
        remove => Events.RemoveHandler(s_selectionChangedComittedEvent, value);
    }

    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.ComboBoxDropDownStyleChangedDescr))]
    public event EventHandler? DropDownStyleChanged
    {
        add => Events.AddHandler(s_dropDownStyleEvent, value);
        remove => Events.RemoveHandler(s_dropDownStyleEvent, value);
    }

    /// <summary>
    ///  ComboBox Onpaint.
    /// </summary>
    /// <hideinheritance/>
    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event PaintEventHandler? Paint
    {
        add => base.Paint += value;
        remove => base.Paint -= value;
    }

    /// <summary>
    ///  This will fire the TextUpdate Event on the ComboBox. This events fires when the Combobox gets the
    ///  CBN_EDITUPDATE notification.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.ComboBoxOnTextUpdateDescr))]
    public event EventHandler? TextUpdate
    {
        add => Events.AddHandler(s_textUpdateEvent, value);
        remove => Events.RemoveHandler(s_textUpdateEvent, value);
    }

    /// <summary>
    ///  This will fire the DropDownClosed Event on the ComboBox. This events fires when the Combobox gets the
    ///  CBN_CLOSEUP notification. This happens when the DropDown closes.
    /// </summary>
    [SRCategory(nameof(SR.CatBehavior))]
    [SRDescription(nameof(SR.ComboBoxOnDropDownClosedDescr))]
    public event EventHandler? DropDownClosed
    {
        add => Events.AddHandler(s_dropDownClosedEvent, value);
        remove => Events.RemoveHandler(s_dropDownClosedEvent, value);
    }

    /// <summary>
    ///  Performs the work of adding the specified items to the combobox
    /// </summary>
    [Obsolete("This method has been deprecated.  There is no replacement.  https://go.microsoft.com/fwlink/?linkid=14202")]
    protected virtual void AddItemsCore(object[]? value)
    {
        if (value is null || value.Length == 0)
        {
            return;
        }

        BeginUpdate();
        try
        {
            Items.AddRangeInternal(value);
        }
        finally
        {
            EndUpdate();
        }
    }

    /// <summary>
    ///  Disables redrawing of the combo box. A call to beginUpdate() must be
    ///  balanced by a following call to endUpdate(). Following a call to
    ///  beginUpdate(), any redrawing caused by operations performed on the
    ///  combo box is deferred until the call to endUpdate().
    /// </summary>
    public void BeginUpdate()
    {
        _updateCount++;
        BeginUpdateInternal();
    }

    private void CheckNoDataSource()
    {
        if (DataSource is not null)
        {
            throw new ArgumentException(SR.DataSourceLocksItems);
        }
    }

    protected override AccessibleObject CreateAccessibilityInstance()
    {
        return new ComboBoxAccessibleObject(this);
    }

    internal bool UpdateNeeded()
    {
        return _updateCount == 0;
    }

    /// <summary>
    ///  This procedure takes in the message, converts the Edit handle coordinates into Combo Box Coordinates
    /// </summary>
    internal Point EditToComboboxMapping(Message m)
    {
        if (_childEdit is null)
        {
            return new Point(0, 0);
        }

        // Get the Combobox Rect
        PInvoke.GetWindowRect(this, out var comboRectMid);

        // Get the Edit Rectangle.
        PInvoke.GetWindowRect(_childEdit, out var editRectMid);

        // Get the delta.
        int comboXMid = PARAM.SignedLOWORD(m.LParamInternal) + (editRectMid.left - comboRectMid.left);
        int comboYMid = PARAM.SignedHIWORD(m.LParamInternal) + (editRectMid.top - comboRectMid.top);

        return new Point(comboXMid, comboYMid);
    }

    /// <summary>
    ///  Subclassed window procedure for the edit and list child controls of the
    ///  combo box.
    /// </summary>
    private void ChildWndProc(ref Message m)
    {
        switch (m.MsgInternal)
        {
            case PInvoke.WM_CHAR:
                if (DropDownStyle == ComboBoxStyle.Simple && m.HWnd == _childListBox!.Handle)
                {
                    DefChildWndProc(ref m);
                }
                else
                {
                    if (PreProcessControlMessage(ref m) != PreProcessControlState.MessageProcessed)
                    {
                        if (ProcessKeyMessage(ref m))
                        {
                            return;
                        }

                        DefChildWndProc(ref m);
                    }
                }

                break;
            case PInvoke.WM_SYSCHAR:
                if (DropDownStyle == ComboBoxStyle.Simple && m.HWnd == _childListBox!.Handle)
                {
                    DefChildWndProc(ref m);
                }
                else
                {
                    if (PreProcessControlMessage(ref m) != PreProcessControlState.MessageProcessed)
                    {
                        if (ProcessKeyEventArgs(ref m))
                        {
                            return;
                        }

                        DefChildWndProc(ref m);
                    }
                }

                break;
            case PInvoke.WM_KEYDOWN:
            case PInvoke.WM_SYSKEYDOWN:
                if (SystemAutoCompleteEnabled && !ACNativeWindow.AutoCompleteActive)
                {
                    _finder.FindDropDowns(false);
                }

                if (AutoCompleteMode != AutoCompleteMode.None)
                {
                    char keyChar = (char)(nint)m.WParamInternal;
                    if (keyChar == (char)(int)Keys.Escape)
                    {
                        DroppedDown = false;
                    }
                    else if (keyChar == (char)(int)Keys.Return && DroppedDown)
                    {
                        UpdateText();
                        OnSelectionChangeCommittedInternal(EventArgs.Empty);
                        DroppedDown = false;
                    }
                }

                if (DropDownStyle == ComboBoxStyle.Simple && m.HWnd == _childListBox!.Handle)
                {
                    DefChildWndProc(ref m);
                }
                else
                {
                    if (PreProcessControlMessage(ref m) != PreProcessControlState.MessageProcessed)
                    {
                        if (ProcessKeyMessage(ref m))
                        {
                            return;
                        }

                        DefChildWndProc(ref m);
                    }
                }

                break;

            case PInvoke.WM_INPUTLANGCHANGE:
                DefChildWndProc(ref m);
                break;

            case PInvoke.WM_KEYUP:
            case PInvoke.WM_SYSKEYUP:
                if (DropDownStyle == ComboBoxStyle.Simple && m.HWnd == _childListBox!.Handle)
                {
                    DefChildWndProc(ref m);
                }
                else
                {
                    if (PreProcessControlMessage(ref m) != PreProcessControlState.MessageProcessed)
                    {
                        if (ProcessKeyMessage(ref m))
                        {
                            return;
                        }

                        DefChildWndProc(ref m);
                    }
                }

                if (SystemAutoCompleteEnabled && !ACNativeWindow.AutoCompleteActive)
                {
                    _finder.FindDropDowns();
                }

                break;
            case PInvoke.WM_KILLFOCUS:
                // Consider - If we don't have a childwndproc, then we don't get here, so we don't
                // update the cache. Do we need to? This happens when we have a DropDownList.
                if (!DesignMode)
                {
                    OnImeContextStatusChanged(m.HWnd);
                }

                DefChildWndProc(ref m);
                // We don't want to fire the focus events twice -
                // once in the combobox and once here.
                if (_fireLostFocus)
                {
                    InvokeLostFocus(this, EventArgs.Empty);
                }

                if (FlatStyle == FlatStyle.Popup)
                {
                    Invalidate();
                }

                break;
            case PInvoke.WM_SETFOCUS:

                // Consider - If we don't have a childwndproc, then we don't get here, so we don't
                // set the status. Do we need to? This happens when we have a DropDownList.
                if (!DesignMode)
                {
                    ImeContext.SetImeStatus(CachedImeMode, m.HWnd);
                }

                if (!HostedInWin32DialogManager)
                {
                    IContainerControl? c = GetContainerControl();
                    if (c is not null)
                    {
                        if (c is ContainerControl container)
                        {
                            if (!container.ActivateControl(this, originator: false))
                            {
                                return;
                            }
                        }
                    }
                }

                DefChildWndProc(ref m);

                // We don't want to fire the focus events twice -
                // once in the combobox and once here.
                if (_fireSetFocus)
                {
                    if (!DesignMode && _childEdit is not null && m.HWnd == _childEdit.Handle)
                    {
                        WmImeSetFocus();
                    }

                    InvokeGotFocus(this, EventArgs.Empty);
                }

                if (FlatStyle == FlatStyle.Popup)
                {
                    Invalidate();
                }

                break;

            case PInvoke.WM_SETFONT:
                DefChildWndProc(ref m);
                if (_childEdit is not null && m.HWnd == _childEdit.Handle)
                {
                    PInvoke.SendMessage(
                        _childEdit,
                        PInvoke.EM_SETMARGINS,
                        (WPARAM)(PInvoke.EC_LEFTMARGIN | PInvoke.EC_RIGHTMARGIN));
                }

                break;
            case PInvoke.WM_LBUTTONDBLCLK:
                // The Listbox gets WM_LBUTTONDOWN - WM_LBUTTONUP -WM_LBUTTONDBLCLK - WM_LBUTTONUP
                // sequence for doubleclick.

                // Set MouseEvents
                _mousePressed = true;
                _mouseEvents = true;
                Capture = true;

                // Call the DefWndProc() so that mousemove messages get to the windows edit
                DefChildWndProc(ref m);

                // The message gets fired from Combo-box's WndPrc - convert to Combobox coordinates
                Point Ptlc = EditToComboboxMapping(m);
                OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, Ptlc.X, Ptlc.Y, 0));
                break;

            case PInvoke.WM_MBUTTONDBLCLK:
                // The Listbox gets  WM_LBUTTONDOWN - WM_LBUTTONUP -WM_LBUTTONDBLCLK - WM_LBUTTONUP
                // sequence for doubleclick

                // Set MouseEvents
                _mousePressed = true;
                _mouseEvents = true;
                Capture = true;

                // Call the DefWndProc() so that mousemove messages get to the windows edit
                DefChildWndProc(ref m);

                // The message gets fired from Combo-box's WndPrc - convert to Combobox coordinates
                Point Ptmc = EditToComboboxMapping(m);
                OnMouseDown(new MouseEventArgs(MouseButtons.Middle, 1, Ptmc.X, Ptmc.Y, 0));
                break;

            case PInvoke.WM_RBUTTONDBLCLK:
                // The Listbox gets  WM_LBUTTONDOWN - WM_LBUTTONUP -WM_LBUTTONDBLCLK - WM_LBUTTONUP
                // sequence for doubleclick

                // Set MouseEvents.
                _mousePressed = true;
                _mouseEvents = true;
                Capture = true;

                // Call the DefWndProc() so that mousemove messages get to the windows edit
                DefChildWndProc(ref m);

                // The message gets fired from Combo-box's WndPrc - convert to Combobox coordinates
                Point Ptrc = EditToComboboxMapping(m);
                OnMouseDown(new MouseEventArgs(MouseButtons.Right, 1, Ptrc.X, Ptrc.Y, 0));
                break;

            case PInvoke.WM_LBUTTONDOWN:
                _mousePressed = true;
                _mouseEvents = true;

                // Set the mouse capture as this is the child Wndproc.
                Capture = true;
                DefChildWndProc(ref m);

                // The message gets fired from Combo-box's WndPrc - convert to Combobox coordinates
                Point Ptl = EditToComboboxMapping(m);

                OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, Ptl.X, Ptl.Y, 0));
                break;
            case PInvoke.WM_LBUTTONUP:

                // Combobox gets a WM_LBUTTONUP for focus change- check MouseEvents.
                if (_mouseEvents && !ValidationCancelled)
                {
                    _mouseEvents = false;
                    if (_mousePressed)
                    {
                        PInvoke.GetWindowRect(this, out var rect);
                        Rectangle clientRect = rect;

                        if (clientRect.Contains(PointToScreen(PARAM.ToPoint(m.LParamInternal))))
                        {
                            _mousePressed = false;
                            OnClick(new MouseEventArgs(MouseButtons.Left, 1, PARAM.ToPoint(m.LParamInternal)));
                            OnMouseClick(new MouseEventArgs(MouseButtons.Left, 1, PARAM.ToPoint(m.LParamInternal)));
                        }
                        else
                        {
                            _mousePressed = false;
                            _mouseInEdit = false;
                            OnMouseLeave(EventArgs.Empty);
                        }
                    }
                }

                DefChildWndProc(ref m);
                Capture = false;

                // The message gets fired from Combo-box's WndPrc - convert to Combobox coordinates.
                OnMouseUp(new MouseEventArgs(MouseButtons.Left, 1, EditToComboboxMapping(m)));
                break;
            case PInvoke.WM_MBUTTONDOWN:
                _mousePressed = true;
                _mouseEvents = true;

                // Set the mouse capture as this is the child Wndproc.
                Capture = true;
                DefChildWndProc(ref m);

                // The message gets fired from Combo-box's WndPrc - convert to Combobox coordinates
                Point P = EditToComboboxMapping(m);

                OnMouseDown(new MouseEventArgs(MouseButtons.Middle, 1, P.X, P.Y, 0));
                break;
            case PInvoke.WM_RBUTTONDOWN:
                _mousePressed = true;
                _mouseEvents = true;

                if (ContextMenuStrip is not null)
                {
                    // Set the mouse capture as this is the child Wndproc.
                    Capture = true;
                }

                DefChildWndProc(ref m);

                // The message gets fired from Combo-box's WndPrc - convert to Combobox coordinates
                Point Pt = EditToComboboxMapping(m);

                OnMouseDown(new MouseEventArgs(MouseButtons.Right, 1, Pt.X, Pt.Y, 0));
                break;
            case PInvoke.WM_MBUTTONUP:
                _mousePressed = false;
                _mouseEvents = false;

                // Set the mouse capture as this is the child Wndproc.
                Capture = false;
                DefChildWndProc(ref m);
                OnMouseUp(new MouseEventArgs(MouseButtons.Middle, 1, PARAM.ToPoint(m.LParamInternal)));
                break;
            case PInvoke.WM_RBUTTONUP:
                _mousePressed = false;
                _mouseEvents = false;

                DefChildWndProc(ref m);

                // The message gets fired from Combo-box's WndPrc - convert to Combobox coordinates
                Point ptRBtnUp = EditToComboboxMapping(m);

                OnMouseUp(new MouseEventArgs(MouseButtons.Right, 1, ptRBtnUp.X, ptRBtnUp.Y, 0));
                break;

            case PInvoke.WM_CONTEXTMENU:
                // Forward context menu messages to the parent control
                if (ContextMenuStrip is not null)
                {
                    PInvoke.SendMessage(this, PInvoke.WM_CONTEXTMENU, m.WParamInternal, m.LParamInternal);
                }
                else
                {
                    DefChildWndProc(ref m);
                }

                break;

            case PInvoke.WM_MOUSEMOVE:
                Point point = EditToComboboxMapping(m);

                // Call the DefWndProc() so that mousemove messages get to the windows edit control
                DefChildWndProc(ref m);
                OnMouseEnterInternal(EventArgs.Empty);
                OnMouseMove(new MouseEventArgs(MouseButtons, 0, point.X, point.Y, 0));
                break;

            case PInvoke.WM_SETCURSOR:
                if (Cursor != DefaultCursor && _childEdit is not null
                    && m.HWnd == _childEdit.Handle && PARAM.LOWORD(m.LParamInternal) == (int)PInvoke.HTCLIENT)
                {
                    Cursor.Current = Cursor;
                }
                else
                {
                    DefChildWndProc(ref m);
                }

                break;

            case PInvoke.WM_MOUSELEAVE:
                DefChildWndProc(ref m);
                OnMouseLeaveInternal(EventArgs.Empty);
                break;

            default:
                DefChildWndProc(ref m);
                break;
        }
    }

    /// <summary>
    ///  Helper to handle MouseEnter.
    /// </summary>
    private void OnMouseEnterInternal(EventArgs args)
    {
        if (!_mouseInEdit)
        {
            OnMouseEnter(args);
            _mouseInEdit = true;
        }
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);

        if (IsAccessibilityObjectCreated && _childEdit is not null && ChildEditAccessibleObject.Bounds.Contains(PointToScreen(e.Location)))
        {
            ChildEditAccessibleObject.RaiseAutomationEvent(UIA_EVENT_ID.UIA_Text_TextSelectionChangedEventId);
        }
    }

    /// <summary>
    ///  Helper to handle MouseLeave.
    /// </summary>
    private void OnMouseLeaveInternal(EventArgs args)
    {
        PInvoke.GetWindowRect(this, out var rect);
        Rectangle rectangle = rect;
        Point p = MousePosition;
        if (!rectangle.Contains(p))
        {
            OnMouseLeave(args);
            _mouseInEdit = false;
        }
    }

    private void DefChildWndProc(ref Message m)
    {
        if (_childEdit is not null)
        {
            NativeWindow? childWindow;
            if (m.HWnd == _childEdit.Handle)
            {
                childWindow = _childEdit;
            }
            else if (m.HWnd == _dropDownHandle)
            {
                childWindow = _childDropDown;
            }
            else
            {
                childWindow = _childListBox;
            }

            // childwindow could be null if the handle was recreated while within a message handler
            // and then whoever recreated the handle allowed the message to continue to be processed
            // we cannot really be sure the new child will properly handle this window message, so we eat it.
            childWindow?.DefWndProc(ref m);
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            if (_autoCompleteCustomSource is not null)
            {
                _autoCompleteCustomSource.CollectionChanged -= new CollectionChangeEventHandler(OnAutoCompleteCustomSourceChanged);
            }

            if (_stringSource is not null)
            {
                _stringSource.ReleaseAutoComplete();
                _stringSource = null;
            }
        }

        base.Dispose(disposing);
    }

    /// <summary>
    ///  Reenables redrawing of the combo box. A call to beginUpdate() must be
    ///  balanced by a following call to endUpdate(). Following a call to
    ///  beginUpdate(), any redrawing caused by operations performed on the
    ///  combo box is deferred until the call to endUpdate().
    /// </summary>
    public unsafe void EndUpdate()
    {
        _updateCount--;
        if (_updateCount == 0 && AutoCompleteSource == AutoCompleteSource.ListItems)
        {
            SetAutoComplete(false, false);
        }

        if (EndUpdateInternal())
        {
            if (_childEdit is not null && !_childEdit.HWND.IsNull)
            {
                PInvoke.InvalidateRect(_childEdit, lpRect: null, bErase: false);
            }

            if (_childListBox is not null && !_childListBox.HWND.IsNull)
            {
                PInvoke.InvalidateRect(_childListBox, lpRect: null, bErase: false);
            }
        }
    }

    /// <summary>
    ///  Finds the first item in the combo box that starts with the given string.
    ///  The search is not case sensitive.
    /// </summary>
    public int FindString(string? s) => FindString(s, startIndex: -1);

    /// <summary>
    ///  Finds the first item after the given index which starts with the given string.
    ///  The search is not case sensitive.
    /// </summary>
    public int FindString(string? s, int startIndex)
    {
        return FindStringInternal(s, _itemsCollection, startIndex, exact: false, ignoreCase: true);
    }

    /// <summary>
    ///  Finds the first item in the combo box that matches the given string.
    ///  The strings must match exactly, except for differences in casing.
    /// </summary>
    public int FindStringExact(string? s)
    {
        return FindStringExact(s, startIndex: -1, ignoreCase: true);
    }

    /// <summary>
    ///  Finds the first item after the given index that matches the given string.
    ///  The strings must match exactly, except for differences in casing.
    /// </summary>
    public int FindStringExact(string? s, int startIndex)
    {
        return FindStringExact(s, startIndex, ignoreCase: true);
    }

    /// <summary>
    ///  Finds the first item after the given index that matches the given string.
    ///  The strings must match exactly, except for differences in casing.
    /// </summary>
    internal int FindStringExact(string? s, int startIndex, bool ignoreCase)
    {
        return FindStringInternal(s, _itemsCollection, startIndex, exact: true, ignoreCase);
    }

    // GetPreferredSize and SetBoundsCore call this method to allow controls to self impose
    // constraints on their size.
    internal override Rectangle ApplyBoundsConstraints(int suggestedX, int suggestedY, int proposedWidth, int proposedHeight)
    {
        if (DropDownStyle is ComboBoxStyle.DropDown
            or ComboBoxStyle.DropDownList)
        {
            proposedHeight = PreferredHeight;
        }

        return base.ApplyBoundsConstraints(suggestedX, suggestedY, proposedWidth, proposedHeight);
    }

    protected override void ScaleControl(SizeF factor, BoundsSpecified specified)
    {
        if (factor.Width != 1F && factor.Height != 1F)
        {
            // we get called on scale before we get a notification that our font has changed.
            // in this case, we need to reset our height cache.
            ResetHeightCache();
        }

        base.ScaleControl(factor, specified);
    }

    /// <summary>
    ///  Returns the height of the given item in an OwnerDrawVariable style
    ///  combo box. This method should not be used for Normal or OwnerDrawFixed
    ///  style combo boxes.
    /// </summary>
    public int GetItemHeight(int index)
    {
        // This function is only relevant for OwnerDrawVariable
        if (DrawMode != DrawMode.OwnerDrawVariable)
        {
            return ItemHeight;
        }

        ArgumentOutOfRangeException.ThrowIfNegative(index);
        ArgumentOutOfRangeException.ThrowIfGreaterThanOrEqual(index, _itemsCollection?.Count ?? 0);

        if (IsHandleCreated)
        {
            int h = (int)PInvoke.SendMessage(this, PInvoke.CB_GETITEMHEIGHT, (WPARAM)index);
            if (h == -1)
            {
                throw new Win32Exception();
            }

            return h;
        }

        return ItemHeight;
    }

    internal HandleRef<HWND> GetListHandle()
    {
        return DropDownStyle == ComboBoxStyle.Simple ? new(_childListBox) : new(this, _dropDownHandle);
    }

    internal NativeWindow GetListNativeWindow()
    {
        return DropDownStyle == ComboBoxStyle.Simple ? _childListBox! : _childDropDown!;
    }

    internal int GetListNativeWindowRuntimeIdPart()
    {
        NativeWindow listNativeWindow = GetListNativeWindow();
        return listNativeWindow is not null ? listNativeWindow.GetHashCode() : 0;
    }

    internal override HBRUSH InitializeDCForWmCtlColor(HDC dc, MessageId msg)
    {
        if (msg == PInvoke.WM_CTLCOLORSTATIC && !ShouldSerializeBackColor())
        {
            // Let the Win32 Edit control handle background colors itself.
            // This is necessary because a disabled edit control will display a different
            // BackColor than when enabled.
            return default;
        }
        else if (msg == PInvoke.WM_CTLCOLORLISTBOX && GetStyle(ControlStyles.UserPaint))
        {
            // Base class returns hollow brush when UserPaint style is set, to avoid flicker in
            // main control. But when returning colors for child dropdown list, return normal ForeColor/BackColor,
            // since hollow brush leaves the list background unpainted.
            PInvoke.SetTextColor(dc, (COLORREF)(uint)ColorTranslator.ToWin32(ForeColor));
            PInvoke.SetBkColor(dc, (COLORREF)(uint)ColorTranslator.ToWin32(BackColor));
            return BackColorBrush;
        }
        else
        {
            return base.InitializeDCForWmCtlColor(dc, msg);
        }
    }

    // Returns true when the key processing needs to be intercepted to allow
    // auto-completion in DropDownList style.
    private bool InterceptAutoCompleteKeystroke(Message m)
    {
        if (m.MsgInternal == PInvoke.WM_KEYDOWN)
        {
            Debug.Assert((ModifierKeys & Keys.Alt) == 0);

            // Keys.Delete only triggers a WM_KEYDOWN and WM_KEYUP, and no WM_CHAR. That's why it's treated separately.
            if ((Keys)(int)m.WParamInternal == Keys.Delete)
            {
                // Reset matching text and remove any selection
                MatchingText = string.Empty;
                _autoCompleteTimeStamp = DateTime.Now.Ticks;
                if (Items.Count > 0)
                {
                    SelectedIndex = 0;
                }

                return false;
            }
        }
        else if (m.MsgInternal == PInvoke.WM_CHAR)
        {
            Debug.Assert((ModifierKeys & Keys.Alt) == 0);
            char keyChar = (char)(nuint)m.WParamInternal;
            if (keyChar == (char)Keys.Back)
            {
                if (DateTime.Now.Ticks - _autoCompleteTimeStamp > AutoCompleteTimeout ||
                    MatchingText.Length <= 1)
                {
                    // Reset matching text and remove any selection
                    MatchingText = string.Empty;
                    if (Items.Count > 0)
                    {
                        SelectedIndex = 0;
                    }
                }
                else
                {
                    // Remove one character from matching text and rematch
                    MatchingText = MatchingText.Remove(MatchingText.Length - 1);
                    SelectedIndex = FindString(MatchingText);
                }

                _autoCompleteTimeStamp = DateTime.Now.Ticks;
                return false;
            }
            else if (keyChar == (char)Keys.Escape)
            {
                MatchingText = string.Empty;
            }

            string newMatchingText;
            if (keyChar != (char)Keys.Escape && keyChar != (char)Keys.Return && !DroppedDown
                && AutoCompleteMode != AutoCompleteMode.Append)
            {
                DroppedDown = true;
            }

            if (DateTime.Now.Ticks - _autoCompleteTimeStamp > AutoCompleteTimeout)
            {
                newMatchingText = new string(keyChar, 1);
                if (FindString(newMatchingText) != -1)
                {
                    MatchingText = newMatchingText;
                    // Select the found item
                }

                _autoCompleteTimeStamp = DateTime.Now.Ticks;
                return false;
            }
            else
            {
                newMatchingText = MatchingText + keyChar;
                int itemFound = FindString(newMatchingText);
                if (itemFound != -1)
                {
                    MatchingText = newMatchingText;
                    if (itemFound != SelectedIndex)
                    {
                        SelectedIndex = itemFound;
                    }
                }

                // Do not change the selection
                _autoCompleteTimeStamp = DateTime.Now.Ticks;
                return true;
            }
        }

        return false;
    }

    /// <summary>
    ///  Invalidate the entire control, including child HWNDs and non-client areas.
    /// </summary>
    private unsafe void InvalidateEverything()
    {
        if (!IsHandleCreated)
        {
            return;
        }

        // Control.Invalidate(true) doesn't invalidate the non-client region.
        PInvoke.RedrawWindow(
            this,
            lprcUpdate: null,
            HRGN.Null,
            REDRAW_WINDOW_FLAGS.RDW_INVALIDATE
                | REDRAW_WINDOW_FLAGS.RDW_FRAME
                | REDRAW_WINDOW_FLAGS.RDW_ERASE
                | REDRAW_WINDOW_FLAGS.RDW_ALLCHILDREN);
    }

    /// <summary>
    ///  Determines if keyData is in input key that the control wants.
    ///  Overridden to return true for RETURN and ESCAPE when the combo box is
    ///  dropped down.
    /// </summary>
    protected override bool IsInputKey(Keys keyData)
    {
        Keys keyCode = keyData & (Keys.KeyCode | Keys.Alt);
        if (keyCode is Keys.Return or Keys.Escape)
        {
            if (DroppedDown || _autoCompleteDroppedDown)
            {
                // old behavior
                return true;
            }
            else if (SystemAutoCompleteEnabled && ACNativeWindow.AutoCompleteActive)
            {
                _autoCompleteDroppedDown = true;
                return true;
            }
        }

        return base.IsInputKey(keyData);
    }

    /// <summary>
    ///  Adds the given item to the native combo box.
    /// </summary>
    private int NativeAdd(object item)
    {
        Debug.Assert(IsHandleCreated, "Shouldn't be calling Native methods before the handle is created.");
        int insertIndex = (int)PInvoke.SendMessage(this, PInvoke.CB_ADDSTRING, (WPARAM)0, GetItemText(item));
        if (insertIndex < 0)
        {
            throw new OutOfMemoryException(SR.ComboBoxItemOverflow);
        }

        return insertIndex;
    }

    /// <summary>
    ///  Clears the contents of the combo box.
    /// </summary>
    private void NativeClear()
    {
        Debug.Assert(IsHandleCreated, "Shouldn't be calling Native methods before the handle is created.");
        string? saved = null;
        if (DropDownStyle != ComboBoxStyle.DropDownList)
        {
            saved = WindowText;
        }

        PInvoke.SendMessage(this, PInvoke.CB_RESETCONTENT);
        if (saved is not null)
        {
            WindowText = saved;
        }
    }

    /// <summary>
    ///  Get the text stored by the native control for the specified list item.
    /// </summary>
    [SkipLocalsInit]
    private unsafe string NativeGetItemText(int index)
    {
        int maxLength = (int)PInvoke.SendMessage(this, PInvoke.CB_GETLBTEXTLEN, (WPARAM)index);
        if (maxLength == PInvoke.LB_ERR)
        {
            return string.Empty;
        }

        using BufferScope<char> buffer = new(stackalloc char[128], minimumLength: maxLength);
        fixed (char* b = buffer)
        {
            int actualLength = (int)PInvoke.SendMessage(this, PInvoke.CB_GETLBTEXT, (WPARAM)index, (LPARAM)b);
            Debug.Assert(actualLength != PInvoke.LB_ERR, "Should have validated the index above");
            return actualLength == PInvoke.LB_ERR ? string.Empty : buffer[..Math.Min(maxLength, actualLength)].ToString();
        }
    }

    /// <summary>
    ///  Inserts the given item to the native combo box at the index.  This asserts if the handle hasn't been
    ///  created or if the resulting insert index doesn't match the passed in index.
    /// </summary>
    private int NativeInsert(int index, object item)
    {
        Debug.Assert(IsHandleCreated, "Shouldn't be calling Native methods before the handle is created.");
        int insertIndex = (int)PInvoke.SendMessage(this, PInvoke.CB_INSERTSTRING, (WPARAM)index, GetItemText(item));
        if (insertIndex < 0)
        {
            throw new OutOfMemoryException(SR.ComboBoxItemOverflow);
        }

        Debug.Assert(insertIndex == index, $"NativeComboBox inserted at {insertIndex} not the requested index of {index}");
        return insertIndex;
    }

    /// <summary>
    ///  Removes the native item from the given index.
    /// </summary>
    private void NativeRemoveAt(int index)
    {
        Debug.Assert(IsHandleCreated, "Shouldn't be calling Native methods before the handle is created.");

        // Windows combo does not invalidate the selected region if you remove the
        // currently selected item.  Test for this and invalidate.  Note that because
        // invalidate will lazy-paint we can actually invalidate before we send the
        // delete message.

        if (DropDownStyle == ComboBoxStyle.DropDownList && SelectedIndex == index)
        {
            Invalidate();
        }

        PInvoke.SendMessage(this, PInvoke.CB_DELETESTRING, (WPARAM)index);
    }

    internal override void RecreateHandleCore()
    {
        string oldText = WindowText;
        base.RecreateHandleCore();
        if (!string.IsNullOrEmpty(oldText) && string.IsNullOrEmpty(WindowText))
        {
            WindowText = oldText;   // restore the window text
        }
    }

    /// <summary>
    ///  Overridden to avoid multiple layouts during handle creation due to combobox size change
    /// </summary>
    protected override void CreateHandle()
    {
        using (new LayoutTransaction(ParentInternal, this, PropertyNames.Bounds))
        {
            base.CreateHandle();
        }
    }

    /// <summary>
    ///  Overridden to make sure all the items and styles get set up correctly.
    ///  Inheriting classes should not forget to call
    ///  base.OnHandleCreated()
    /// </summary>
    protected override unsafe void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);

        if (MaxLength > 0)
        {
            PInvoke.SendMessage(this, PInvoke.CB_LIMITTEXT, (WPARAM)MaxLength);
        }

        // Get the handles and WndProcs of the ComboBox's child windows
        Debug.Assert(_childEdit is null, "Child Edit window is already attached.");
        Debug.Assert(_childListBox is null, "Child ListBox window is already attached.");

        bool ok = _childEdit is null && _childListBox is null;

        if (ok && DropDownStyle != ComboBoxStyle.DropDownList)
        {
            HWND hwnd = PInvoke.GetWindow(this, GET_WINDOW_CMD.GW_CHILD);
            if (!hwnd.IsNull)
            {
                // If it's a simple dropdown list, the first HWND is the ListBox.
                if (DropDownStyle == ComboBoxStyle.Simple)
                {
                    _childListBox = new ComboBoxChildNativeWindow(this, ChildWindowType.ListBox);
                    _childListBox.AssignHandle(hwnd);

                    // Get the edits hwnd...
                    hwnd = PInvoke.GetWindow(new HandleRef<HWND>(this, hwnd), GET_WINDOW_CMD.GW_HWNDNEXT);
                }

                _childEdit = new ComboBoxChildNativeWindow(this, ChildWindowType.Edit);
                _childEdit.AssignHandle(hwnd);

                // Set the initial margin for ComboBox to be zero (this is also done whenever the font is changed).
                PInvoke.SendMessage(_childEdit, PInvoke.EM_SETMARGINS, (WPARAM)(PInvoke.EC_LEFTMARGIN | PInvoke.EC_RIGHTMARGIN));
            }
        }

        int dropDownWidth = Properties.GetInteger(s_propDropDownWidth, out bool found);
        if (found)
        {
            PInvoke.SendMessage(this, PInvoke.CB_SETDROPPEDWIDTH, (WPARAM)dropDownWidth);
        }

        _ = Properties.GetInteger(s_propItemHeight, out found);
        if (found)
        {
            // someone has set the item height - update it
            UpdateItemHeight();
        }

        // Resize a simple style ComboBox on handle creation
        // to respect the requested height.
        //
        if (DropDownStyle == ComboBoxStyle.Simple)
        {
            Height = _requestedHeight;
        }

        // If HandleCreated set the AutoComplete...
        // This function checks if the correct properties are set to enable AutoComplete feature on ComboBox.
        try
        {
            _fromHandleCreate = true;
            SetAutoComplete(false, false);
        }
        finally
        {
            _fromHandleCreate = false;
        }

        if (IsDarkModeEnabled)
        {
            // Style the ComboBox Open-Button:
            PInvoke.SetWindowTheme(HWND, "DarkMode_CFD", null);
            COMBOBOXINFO cInfo = default;
            cInfo.cbSize = (uint)sizeof(COMBOBOXINFO);

            // Style the ComboBox drop-down (including its ScrollBar(s)):
            var result = PInvoke.GetComboBoxInfo(HWND, ref cInfo);
            PInvoke.SetWindowTheme(cInfo.hwndList, "DarkMode_Explorer", null);
        }

        if (_itemsCollection is not null)
        {
            foreach (object item in _itemsCollection)
            {
                NativeAdd(item);
            }

            // Now update the current selection.
            if (_selectedIndex >= 0)
            {
                PInvoke.SendMessage(this, PInvoke.CB_SETCURSEL, (WPARAM)_selectedIndex);
                UpdateText();
                _selectedIndex = -1;
            }
        }

        // NOTE: Setting SelectedIndex must be the last thing we do!
    }

    /// <summary>
    ///  We need to un-subclasses everything here.  Inheriting classes should
    ///  not forget to call base.OnHandleDestroyed()
    /// </summary>
    protected override void OnHandleDestroyed(EventArgs e)
    {
        _dropDownHandle = HWND.Null;
        if (Disposing)
        {
            _itemsCollection = null;
            _selectedIndex = -1;
        }
        else
        {
            _selectedIndex = SelectedIndex;
        }

        if (_stringSource is not null)
        {
            _stringSource.ReleaseAutoComplete();
            _stringSource = null;
        }

        base.OnHandleDestroyed(e);
    }

    /// <summary>
    ///  This is the code that actually fires the drawItem event.  Don't
    ///  forget to call base.onDrawItem() to ensure that drawItem events
    ///  are correctly fired at all other times.
    /// </summary>
    protected virtual void OnDrawItem(DrawItemEventArgs e)
    {
        ((DrawItemEventHandler?)Events[s_drawItemEvent])?.Invoke(this, e);
    }

    /// <summary>
    ///  This is the code that actually fires the dropDown event.  Don't
    ///  forget to call base.onDropDown() to ensure that dropDown events
    ///  are correctly fired at all other times.
    /// </summary>
    protected virtual void OnDropDown(EventArgs e)
    {
        ((EventHandler?)Events[s_dropDownEvent])?.Invoke(this, e);

        if (!IsHandleCreated)
        {
            return;
        }

        // Notify collapsed/expanded property change.
        if (IsAccessibilityObjectCreated)
        {
            AccessibilityObject.RaiseAutomationPropertyChangedEvent(
                UIA_PROPERTY_ID.UIA_ExpandCollapseExpandCollapseStatePropertyId,
                (VARIANT)(int)ExpandCollapseState.ExpandCollapseState_Collapsed,
                (VARIANT)(int)ExpandCollapseState.ExpandCollapseState_Expanded);

            if (AccessibilityObject is ComboBoxAccessibleObject accessibleObject)
            {
                accessibleObject.SetComboBoxItemFocus();
            }
        }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected override void OnKeyDown(KeyEventArgs e)
    {
        // Do Return/ESC handling
        if (SystemAutoCompleteEnabled)
        {
            if (e.KeyCode == Keys.Return)
            {
                // Set SelectedIndex
                NotifyAutoComplete(true);
            }
            else if ((e.KeyCode == Keys.Escape) && _autoCompleteDroppedDown)
            {
                // Fire TextChanged Only
                NotifyAutoComplete(false);
            }

            _autoCompleteDroppedDown = false;
        }

        // Base Handling
        base.OnKeyDown(e);
    }

    /// <summary>
    ///  Key press event handler. Overridden to close up the combo box when the
    ///  user presses RETURN or ESCAPE.
    /// </summary>
    protected override void OnKeyPress(KeyPressEventArgs e)
    {
        base.OnKeyPress(e);

        // return when dropped down already fires commit.
        if (!e.Handled && (e.KeyChar == (char)(int)Keys.Return || e.KeyChar == (char)(int)Keys.Escape)
            && DroppedDown)
        {
            _dropDown = false;
            if (FormattingEnabled)
            {
                // Set the Text which would Compare the WindowText with the TEXT and change SelectedIndex.
                Text = WindowText;
                SelectAll();
                e.Handled = false;
            }
            else
            {
                DroppedDown = false;
                e.Handled = true;
            }
        }
    }

    protected override void OnKeyUp(KeyEventArgs e)
    {
        base.OnKeyUp(e);

        if (IsAccessibilityObjectCreated && _childEdit is not null && ContainsNavigationKeyCode(e.KeyCode))
        {
            ChildEditAccessibleObject.RaiseAutomationEvent(UIA_EVENT_ID.UIA_Text_TextSelectionChangedEventId);
        }
    }

    private static bool ContainsNavigationKeyCode(Keys keyCode) => keyCode switch
    {
        Keys.Home or Keys.End or Keys.Left or Keys.Right => true,
        _ => false,
    };

    /// <summary>
    ///  This is the code that actually fires the OnMeasureItem event.  Don't
    ///  forget to call base.onMeasureItem() to ensure that OnMeasureItem
    ///  events are correctly fired at all other times.
    /// </summary>
    protected virtual void OnMeasureItem(MeasureItemEventArgs e)
    {
        ((MeasureItemEventHandler?)Events[s_measureItemEvent])?.Invoke(this, e);
    }

    /// <summary>
    ///  If we have the style set to popup show mouse over
    /// </summary>
    protected override void OnMouseEnter(EventArgs e)
    {
        base.OnMouseEnter(e);
        MouseIsOver = true;
    }

    /// <summary>
    ///  If we have the style set to popup show mouse over
    /// </summary>
    protected override void OnMouseLeave(EventArgs e)
    {
        base.OnMouseLeave(e);
        MouseIsOver = false;
    }

    /// <summary>
    ///  This internal helper allows us to call the committed function multiple times without worrying about double firing.
    /// </summary>
    private void OnSelectionChangeCommittedInternal(EventArgs e)
    {
        // There are cases where if we disable the combo while in this event handler, it sends the message again.
        // This is a recursion guard to ensure we only send one commit per user action.
        if (_allowCommit)
        {
            try
            {
                _allowCommit = false;
                OnSelectionChangeCommitted(e);
            }
            finally
            {
                _allowCommit = true;
            }
        }
    }

    /// <summary>
    ///  This is the code that actually fires the SelectionChangeCommitted event.
    ///  Don't forget to call base.OnSelectionChangeCommitted() to ensure
    ///  that SelectionChangeCommitted events are correctly fired at all other times.
    /// </summary>
    protected virtual void OnSelectionChangeCommitted(EventArgs e)
    {
        ((EventHandler?)Events[s_selectionChangedComittedEvent])?.Invoke(this, e);

        // The user selects a list item or selects an item and then closes the list.
        // It indicates that the user's selection is to be processed but should not
        // be focused after closing the list.
        if (_dropDown)
        {
            _dropDownWillBeClosed = true;
        }
    }

    /// <summary>
    ///  This is the code that actually fires the selectedIndexChanged event.
    ///  Don't forget to call base.onSelectedIndexChanged() to ensure
    ///  that selectedIndexChanged events are correctly fired at all other times.
    /// </summary>
    protected override void OnSelectedIndexChanged(EventArgs e)
    {
        base.OnSelectedIndexChanged(e);
        ((EventHandler?)Events[s_selectedIndexChangedEvent])?.Invoke(this, e);

        if (!IsHandleCreated)
        {
            return;
        }

        if (_dropDownWillBeClosed)
        {
            // This is after-closing selection - do not focus on the list item
            // and reset the state to announce the selections later.
            _dropDownWillBeClosed = false;
            return;
        }

        if (IsAccessibilityObjectCreated)
        {
            if (AccessibilityObject is ComboBoxAccessibleObject accessibleObject
                && (DropDownStyle == ComboBoxStyle.DropDownList || DropDownStyle == ComboBoxStyle.DropDown))
            {
                // Announce DropDown- and DropDownList-styled ComboBox item selection using keyboard
                // in case when Level 3 is enabled and DropDown is not in expanded state. Simple-styled
                // ComboBox selection is announced by TextProvider.
                if (_dropDown)
                {
                    accessibleObject.SetComboBoxItemFocus();
                }

                accessibleObject.SetComboBoxItemSelection();
            }

            if (_childEdit is not null)
            {
                ChildEditAccessibleObject.RaiseAutomationEvent(UIA_EVENT_ID.UIA_Text_TextSelectionChangedEventId);
            }
        }

        // set the position in the dataSource, if there is any
        // we will only set the position in the currencyManager if it is different
        // from the SelectedIndex. Setting CurrencyManager::Position (even w/o changing it)
        // calls CurrencyManager::EndCurrentEdit, and that will pull the dataFrom the controls
        // into the backEnd. We do not need to do that.
        //
        // don't change the position if SelectedIndex is -1 because this indicates a selection not from the list.
        if (DataManager is not null && DataManager.Position != SelectedIndex)
        {
            // read this as "if Everett or (Whidbey and selIndex is valid)"
            if (!FormattingEnabled || SelectedIndex != -1)
            {
                DataManager.Position = SelectedIndex;
            }
        }
    }

    protected override void OnSelectedValueChanged(EventArgs e)
    {
        base.OnSelectedValueChanged(e);
        _selectedValueChangedFired = true;
    }

    /// <summary>
    ///  This is the code that actually fires the selectedItemChanged event.
    ///  Don't forget to call base.onSelectedItemChanged() to ensure
    ///  that selectedItemChanged events are correctly fired at all other times.
    /// </summary>
    protected virtual void OnSelectedItemChanged(EventArgs e)
    {
        ((EventHandler?)Events[s_selectedItemChangedEvent])?.Invoke(this, e);
    }

    /// <summary>
    ///  This is the code that actually fires the DropDownStyleChanged event.
    /// </summary>
    protected virtual void OnDropDownStyleChanged(EventArgs e)
    {
        ((EventHandler?)Events[s_dropDownStyleEvent])?.Invoke(this, e);
    }

    /// <summary>
    ///  This method is called by the parent control when any property
    ///  changes on the parent. This can be overridden by inheriting
    ///  classes, however they must call base.OnParentPropertyChanged.
    /// </summary>
    protected override void OnParentBackColorChanged(EventArgs e)
    {
        base.OnParentBackColorChanged(e);
        if (DropDownStyle == ComboBoxStyle.Simple)
        {
            Invalidate();
        }
    }

    /// <summary>
    ///  Indicates that a critical property, such as color or font has
    ///  changed.
    /// </summary>
    protected override void OnFontChanged(EventArgs e)
    {
        base.OnFontChanged(e);
        ResetHeightCache();

        if (AutoCompleteMode == AutoCompleteMode.None)
        {
            UpdateControl(true);
        }
        else
        {
            // we always will recreate the handle when autocomplete mode is on
            RecreateHandle();
        }

        CommonProperties.xClearPreferredSizeCache(this);
    }

    private void OnAutoCompleteCustomSourceChanged(object? sender, CollectionChangeEventArgs e)
    {
        if (AutoCompleteSource == AutoCompleteSource.CustomSource)
        {
            if (AutoCompleteCustomSource.Count == 0)
            {
                SetAutoComplete(reset: true, recreate: true);
            }
            else
            {
                SetAutoComplete(reset: true, recreate: false);
            }
        }
    }

    /// <summary>
    ///  Indicates that a critical property, such as color or font has
    ///  changed.
    /// </summary>
    protected override void OnBackColorChanged(EventArgs e)
    {
        base.OnBackColorChanged(e);
        UpdateControl(false);
    }

    /// <summary>
    ///  Indicates that a critical property, such as color or font has
    ///  changed.
    /// </summary>
    protected override void OnForeColorChanged(EventArgs e)
    {
        base.OnForeColorChanged(e);
        UpdateControl(false);
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected override void OnGotFocus(EventArgs e)
    {
        if (!_canFireLostFocus)
        {
            base.OnGotFocus(e);
            _canFireLostFocus = true;
        }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected override void OnLostFocus(EventArgs e)
    {
        if (_canFireLostFocus)
        {
            if (AutoCompleteMode != AutoCompleteMode.None
                && AutoCompleteSource == AutoCompleteSource.ListItems
                && DropDownStyle == ComboBoxStyle.DropDownList)
            {
                MatchingText = string.Empty;
            }

            base.OnLostFocus(e);
            _canFireLostFocus = false;
        }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected override void OnTextChanged(EventArgs e)
    {
        if (SystemAutoCompleteEnabled)
        {
            string text = Text;

            // Prevent multiple TextChanges...
            if (text != _lastTextChangedValue)
            {
                // Need to still fire a TextChanged
                base.OnTextChanged(e);

                // Save the new value
                _lastTextChangedValue = text;
            }
        }
        else
        {
            // Call the base
            base.OnTextChanged(e);
        }

        if (IsAccessibilityObjectCreated && _childEdit is not null)
        {
            ChildEditAccessibleObject.RaiseAutomationEvent(UIA_EVENT_ID.UIA_Text_TextChangedEventId);
        }
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected override void OnValidating(CancelEventArgs e)
    {
        if (SystemAutoCompleteEnabled)
        {
            // Handle AutoComplete notification
            NotifyAutoComplete();
        }

        // Call base
        base.OnValidating(e);
    }

    private void UpdateControl(bool recreate)
    {
        // clear the pref height cache
        ResetHeightCache();

        if (IsHandleCreated)
        {
            if (DropDownStyle == ComboBoxStyle.Simple && recreate)
            {
                // Control forgets to add a scrollbar.
                RecreateHandle();
            }
            else
            {
                UpdateItemHeight();
                // Force everything to repaint.
                InvalidateEverything();
            }
        }
    }

    /// <summary>
    ///  Raises the <see cref="Control.Resize"/> event.
    /// </summary>
    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        if (DropDownStyle == ComboBoxStyle.Simple && IsHandleCreated)
        {
            // simple style combo boxes have more painting problems than you can shake a stick at
            InvalidateEverything();
        }
    }

    protected override void OnDataSourceChanged(EventArgs e)
    {
        if (Sorted)
        {
            if (DataSource is not null && Created)
            {
                // we will only throw the exception when the control is already on the form.
                Debug.Assert(DisplayMember.Equals(string.Empty), "if this list is sorted it means that dataSource was null when Sorted first became true. at that point DisplayMember had to be String.Empty");
                DataSource = null;
                throw new InvalidOperationException(SR.ComboBoxDataSourceWithSort);
            }
        }

        if (DataSource is null)
        {
            BeginUpdate();
            SelectedIndex = -1;
            Items.ClearInternal();
            EndUpdate();
        }

        if (!Sorted && Created)
        {
            base.OnDataSourceChanged(e);
        }

        RefreshItems();
    }

    protected override void OnDisplayMemberChanged(EventArgs e)
    {
        base.OnDisplayMemberChanged(e);
        RefreshItems();
    }

    /// <summary>
    ///  This event is fired when the dropdown portion of the combobox is hidden.
    /// </summary>
    protected virtual void OnDropDownClosed(EventArgs e)
    {
        ((EventHandler?)Events[s_dropDownClosedEvent])?.Invoke(this, e);

        if (!IsHandleCreated)
        {
            return;
        }

        if (IsAccessibilityObjectCreated)
        {
            // Need to announce the focus on combo-box with new selected value on drop-down close.
            // If do not do this focus in Level 3 stays on list item of unvisible list.
            // This is necessary for DropDown style as edit should not take focus.
            if (DropDownStyle == ComboBoxStyle.DropDown)
            {
                AccessibilityObject.RaiseAutomationEvent(UIA_EVENT_ID.UIA_AutomationFocusChangedEventId);
            }

            // Some accessibility tools (e.g., NVDA) could recognize ComboBox as IAccessible object
            // and ignore UIA focus change event. For such cases we need to additionaly raise MSAA focus event.
            if (DropDownStyle == ComboBoxStyle.DropDownList)
            {
                // Focus on the ComboBox itself for DropDownList style.
                // childID = CHILDID_SELF - 1 (the -1 will resolve to CHILDID_SELF when we call NotifyWinEvent)
                AccessibilityNotifyClients(AccessibleEvents.Focus, childID: -1);
            }
            else if (_childEdit is not null && _childEdit.Handle != 0)
            {
                // Focus on edit field so that its changes are announced, e.g. when selecting items by arrows.
                PInvoke.NotifyWinEvent(
                    (uint)AccessibleEvents.Focus,
                    _childEdit,
                    (int)OBJECT_IDENTIFIER.OBJID_CLIENT,
                    (int)PInvoke.CHILDID_SELF);
            }

            // Notify Collapsed/expanded property change.
            AccessibilityObject.RaiseAutomationPropertyChangedEvent(
                UIA_PROPERTY_ID.UIA_ExpandCollapseExpandCollapseStatePropertyId,
                (VARIANT)(int)ExpandCollapseState.ExpandCollapseState_Expanded,
                (VARIANT)(int)ExpandCollapseState.ExpandCollapseState_Collapsed);
        }

        // Collapsing the DropDown, so reset the flag.
        _dropDownWillBeClosed = false;
    }

    /// <summary>
    ///  This event is fired when the edit portion of a combobox is about to display altered text.
    ///  This event is NOT fired if the TEXT property is programmatically changed.
    /// </summary>
    protected virtual void OnTextUpdate(EventArgs e)
    {
        ((EventHandler?)Events[s_textUpdateEvent])?.Invoke(this, e);
    }

    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        bool returnedValue = base.ProcessCmdKey(ref msg, keyData);

        if (!returnedValue && keyData == (Keys.Control | Keys.A))
        {
            Select(0, Text.Length);
            SelectedText = Text;
            SelectionStart = 0;
            SelectionLength = Text.Length;

            return true;
        }

        if (DropDownStyle != ComboBoxStyle.DropDownList
            && (keyData == (Keys.Control | Keys.Back) || keyData == (Keys.Control | Keys.Shift | Keys.Back)))
        {
            if (SelectionLength != 0)
            {
                SelectedText = string.Empty;
            }
            else if (SelectionStart != 0)
            {
                int boundaryStart = ClientUtils.GetWordBoundaryStart(Text, SelectionStart);
                int length = SelectionStart - boundaryStart;
                BeginUpdateInternal();
                SelectionStart = boundaryStart;
                SelectionLength = length;
                EndUpdateInternal();
                SelectedText = string.Empty;
            }

            return true;
        }

        return returnedValue;
    }

    protected override bool ProcessKeyEventArgs(ref Message m)
    {
        if (AutoCompleteMode != AutoCompleteMode.None
            && AutoCompleteSource == AutoCompleteSource.ListItems
            && DropDownStyle == ComboBoxStyle.DropDownList
            && InterceptAutoCompleteKeystroke(m))
        {
            return true;
        }
        else
        {
            return base.ProcessKeyEventArgs(ref m);
        }
    }

    private void ResetHeightCache()
    {
        _prefHeightCache = -1;
    }

    /// <summary>
    ///  Reparses the objects, getting new text strings for them.
    /// </summary>
    protected override void RefreshItems()
    {
        // Save off the selection and the current collection.
        int selectedIndex = SelectedIndex;
        ObjectCollection? savedItems = _itemsCollection;

        _itemsCollection = null;

        object[]? newItems = null;

        // if we have a dataSource and a DisplayMember, then use it
        // to populate the Items collection
        if (DataManager is not null && DataManager.Count != -1)
        {
            newItems = new object[DataManager.Count];
            for (int i = 0; i < newItems.Length; i++)
            {
                newItems[i] = DataManager[i]!;
            }
        }
        else if (savedItems is not null)
        {
            newItems = new object[savedItems.Count];
            savedItems.CopyTo(newItems, arrayIndex: 0);
        }

        BeginUpdate();
        try
        {
            // Clear the items.
            if (IsHandleCreated)
            {
                NativeClear();
            }

            // Store the current list of items
            if (newItems is not null)
            {
                Items.AddRangeInternal(newItems);
            }

            if (DataManager is not null)
            {
                // put the selectedIndex in sync w/ the position in the dataManager
                SelectedIndex = DataManager.Position;
            }
            else
            {
                SelectedIndex = selectedIndex;
            }
        }
        finally
        {
            EndUpdate();
        }
    }

    /// <summary>
    ///  Reparses the object at the given index, getting new text string for it.
    /// </summary>
    protected override void RefreshItem(int index)
    {
        Items.SetItemInternal(index, Items[index]!);
    }

    /// <summary>
    ///  Release the ChildWindow object by un-subclassing the child edit and
    ///  list controls and freeing the root of the ChildWindow object.
    /// </summary>
    private void ReleaseChildWindow()
    {
        if (_childEdit is not null)
        {
            _childEdit.ReleaseHandle();
            _childEdit = null;
        }

        if (_childListBox is not null)
        {
            _childListBox.ReleaseHandle();
            _childListBox = null;
        }

        if (_childDropDown is not null)
        {
            _childDropDown.ReleaseHandle();
            _childDropDown = null;
        }
    }

    internal override void ReleaseUiaProvider(HWND handle)
    {
        if (!IsAccessibilityObjectCreated)
        {
            return;
        }

        PInvoke.UiaDisconnectProvider(_childTextAccessibleObject);

        _childTextAccessibleObject = null;

        if (AccessibilityObject is ComboBoxAccessibleObject accessibilityObject)
        {
            accessibilityObject.ResetListItemAccessibleObjects();
            accessibilityObject.ReleaseDropDownButtonUiaProvider();
        }

        base.ReleaseUiaProvider(handle);
    }

    private void ResetAutoCompleteCustomSource()
    {
        AutoCompleteCustomSource = null;
    }

    private void ResetDropDownWidth()
    {
        Properties.RemoveInteger(s_propDropDownWidth);
    }

    private void ResetItemHeight()
    {
        Properties.RemoveInteger(s_propItemHeight);
    }

    public override void ResetText()
    {
        base.ResetText();
    }

    /// <summary>
    ///  Enables the AutoComplete feature for combobox depending on the properties set.
    ///  These properties are namely AutoCompleteMode, AutoCompleteSource and AutoCompleteCustomSource.
    /// </summary>
    private void SetAutoComplete(bool reset, bool recreate)
    {
        if (!IsHandleCreated || _childEdit is null)
        {
            return;
        }

        if (AutoCompleteMode == AutoCompleteMode.None)
        {
            if (reset)
            {
                PInvoke.SHAutoComplete(
                    _childEdit,
                    SHELL_AUTOCOMPLETE_FLAGS.SHACF_AUTOSUGGEST_FORCE_OFF | SHELL_AUTOCOMPLETE_FLAGS.SHACF_AUTOAPPEND_FORCE_OFF);
            }

            return;
        }

        if (!_fromHandleCreate && recreate && IsHandleCreated)
        {
            // RecreateHandle to avoid Leak.
            // notice the use of member variable to avoid re-entrancy
            AutoCompleteMode backUpMode = AutoCompleteMode;
            _autoCompleteMode = AutoCompleteMode.None;
            RecreateHandle();
            _autoCompleteMode = backUpMode;
        }

        if (AutoCompleteSource == AutoCompleteSource.CustomSource)
        {
            if (AutoCompleteCustomSource is null)
            {
                return;
            }

            if (AutoCompleteCustomSource.Count == 0)
            {
                PInvoke.SHAutoComplete(
                    _childEdit,
                    SHELL_AUTOCOMPLETE_FLAGS.SHACF_AUTOSUGGEST_FORCE_OFF | SHELL_AUTOCOMPLETE_FLAGS.SHACF_AUTOAPPEND_FORCE_OFF);

                return;
            }

            if (_stringSource is null)
            {
                _stringSource = new StringSource(AutoCompleteCustomSource.ToArray());
                if (!_stringSource.Bind(_childEdit, (AUTOCOMPLETEOPTIONS)AutoCompleteMode))
                {
                    throw new ArgumentException(SR.AutoCompleteFailure);
                }
            }
            else
            {
                _stringSource.RefreshList(AutoCompleteCustomSource.ToArray());
            }

            return;
        }

        if (AutoCompleteSource == AutoCompleteSource.ListItems)
        {
            if (DropDownStyle == ComboBoxStyle.DropDownList)
            {
                // Drop down list special handling
                Debug.Assert(DropDownStyle == ComboBoxStyle.DropDownList);
                PInvoke.SHAutoComplete(
                    _childEdit,
                    SHELL_AUTOCOMPLETE_FLAGS.SHACF_AUTOSUGGEST_FORCE_OFF | SHELL_AUTOCOMPLETE_FLAGS.SHACF_AUTOAPPEND_FORCE_OFF);

                return;
            }

            if (_itemsCollection is null)
            {
                return;
            }

            if (_itemsCollection.Count == 0)
            {
                PInvoke.SHAutoComplete(
                    _childEdit,
                    SHELL_AUTOCOMPLETE_FLAGS.SHACF_AUTOSUGGEST_FORCE_OFF | SHELL_AUTOCOMPLETE_FLAGS.SHACF_AUTOAPPEND_FORCE_OFF);

                return;
            }

            if (_stringSource is null)
            {
                _stringSource = new StringSource(GetStringsForAutoComplete());
                if (!_stringSource.Bind(_childEdit, (AUTOCOMPLETEOPTIONS)AutoCompleteMode))
                {
                    throw new ArgumentException(SR.AutoCompleteFailureListItems);
                }
            }
            else
            {
                _stringSource.RefreshList(GetStringsForAutoComplete());
            }

            return;
        }

        SHELL_AUTOCOMPLETE_FLAGS mode = SHELL_AUTOCOMPLETE_FLAGS.SHACF_DEFAULT;
        if (AutoCompleteMode == AutoCompleteMode.Suggest)
        {
            mode |= SHELL_AUTOCOMPLETE_FLAGS.SHACF_AUTOSUGGEST_FORCE_ON | SHELL_AUTOCOMPLETE_FLAGS.SHACF_AUTOAPPEND_FORCE_OFF;
        }

        if (AutoCompleteMode == AutoCompleteMode.Append)
        {
            mode |= SHELL_AUTOCOMPLETE_FLAGS.SHACF_AUTOAPPEND_FORCE_ON | SHELL_AUTOCOMPLETE_FLAGS.SHACF_AUTOSUGGEST_FORCE_OFF;
        }

        if (AutoCompleteMode == AutoCompleteMode.SuggestAppend)
        {
            mode |= SHELL_AUTOCOMPLETE_FLAGS.SHACF_AUTOSUGGEST_FORCE_ON;
            mode |= SHELL_AUTOCOMPLETE_FLAGS.SHACF_AUTOAPPEND_FORCE_ON;
        }

        PInvoke.SHAutoComplete(_childEdit.HWND, (SHELL_AUTOCOMPLETE_FLAGS)AutoCompleteSource | mode);

        GC.KeepAlive(this);
    }

    /// <summary>
    ///  Selects the text in the editable portion of the ComboBox at the
    ///  from the given start index to the given end index.
    /// </summary>
    public void Select(int start, int length)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(start);

        // the Length can be negative to support Selecting in the "reverse" direction..
        // but start + length cannot be negative... this means Length is far negative...
        ArgumentOutOfRangeException.ThrowIfLessThan(length, -start);

        int end = start + length;
        PInvoke.SendMessage(this, PInvoke.CB_SETEDITSEL, (WPARAM)0, LPARAM.MAKELPARAM(start, end));
    }

    /// <summary>
    ///  Selects all the text in the editable portion of the ComboBox.
    /// </summary>
    public void SelectAll()
    {
        Select(0, int.MaxValue);
    }

    protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified)
    {
        // If we are changing height, store the requested height.
        // Requested height is used if the style is changed to simple.
        // (
        if ((specified & BoundsSpecified.Height) != BoundsSpecified.None)
        {
            _requestedHeight = height;
        }

        base.SetBoundsCore(x, y, width, height, specified);
    }

    /// <summary>
    ///  Performs the work of setting the specified items to the combobox
    /// </summary>
    protected override void SetItemsCore(IList value)
    {
        BeginUpdate();
        Items.ClearInternal();
        Items.AddRangeInternal(value);

        // if the list changed, we want to keep the same selected index
        // CurrencyManager will provide the PositionChanged event
        // it will be provided before changing the list though...
        if (DataManager is not null)
        {
            if (DataSource is ICurrencyManagerProvider)
            {
                _selectedValueChangedFired = false;
            }

            if (IsHandleCreated)
            {
                PInvoke.SendMessage(this, PInvoke.CB_SETCURSEL, (WPARAM)DataManager.Position);
            }
            else
            {
                _selectedIndex = DataManager.Position;
            }

            // if set_SelectedIndexChanged did not fire OnSelectedValueChanged
            // then we have to fire it ourselves, cos the list changed anyway
            if (!_selectedValueChangedFired)
            {
                OnSelectedValueChanged(EventArgs.Empty);
                _selectedValueChangedFired = false;
            }
        }

        EndUpdate();
    }

    protected override void SetItemCore(int index, object value)
    {
        Items.SetItemInternal(index, value);
    }

    private bool ShouldSerializeAutoCompleteCustomSource()
    {
        return _autoCompleteCustomSource is not null && _autoCompleteCustomSource.Count > 0;
    }

    internal bool ShouldSerializeDropDownWidth()
    {
        return (Properties.ContainsInteger(s_propDropDownWidth));
    }

    /// <summary>
    ///  Indicates whether the itemHeight property should be persisted.
    /// </summary>
    internal bool ShouldSerializeItemHeight()
    {
        return (Properties.ContainsInteger(s_propItemHeight));
    }

    /// <summary>
    ///  Determines if the Text property needs to be persisted.
    /// </summary>
    internal override bool ShouldSerializeText()
    {
        return SelectedIndex == -1 && base.ShouldSerializeText();
    }

    private IReadOnlyList<Entry> Entries => Items.InnerList;

    /// <summary>
    ///  Provides some interesting info about this control in String form.
    /// </summary>
    public override string ToString()
    {
        string s = base.ToString();
        return $"{s}, Items.Count: {_itemsCollection?.Count ?? 0}";
    }

    private void UpdateDropDownHeight()
    {
        if (_dropDownHandle.IsNull)
        {
            return;
        }

        // Now use the DropDownHeight property instead of calculating the Height...
        int height = DropDownHeight;
        if (height == DefaultDropDownHeight)
        {
            int itemCount = (_itemsCollection is null) ? 0 : _itemsCollection.Count;
            int count = Math.Min(Math.Max(itemCount, 1), _maxDropDownItems);
            height = ItemHeight * count + 2;
        }

        PInvoke.SetWindowPos(
            _dropDownHandle,
            HWND.HWND_TOP,
            0,
            0,
            DropDownWidth,
            height,
            SET_WINDOW_POS_FLAGS.SWP_NOMOVE | SET_WINDOW_POS_FLAGS.SWP_NOZORDER);

        GC.KeepAlive(this);
    }

    /// <summary>
    ///  Manufactures a MeasureItemEventArgs for each item in the list to simulate
    ///  the combobox requesting the info. This gives the effect of allowing the
    ///  measureitem info to be updated at anytime.
    /// </summary>
    private void UpdateItemHeight()
    {
        if (!IsHandleCreated)
        {
            // If we don't create control here we report item heights incorrectly later on.
            CreateControl();
        }

        if (DrawMode == DrawMode.OwnerDrawFixed)
        {
            PInvoke.SendMessage(this, PInvoke.CB_SETITEMHEIGHT, (WPARAM)(-1), (LPARAM)ItemHeight);
            PInvoke.SendMessage(this, PInvoke.CB_SETITEMHEIGHT, 0, ItemHeight);
        }
        else if (DrawMode == DrawMode.OwnerDrawVariable)
        {
            PInvoke.SendMessage(this, PInvoke.CB_SETITEMHEIGHT, (WPARAM)(-1), (LPARAM)ItemHeight);
            using Graphics graphics = CreateGraphicsInternal();
            for (int i = 0; i < Items.Count; i++)
            {
                int original = (int)PInvoke.SendMessage(this, PInvoke.CB_GETITEMHEIGHT, (WPARAM)i);
                MeasureItemEventArgs mievent = new(graphics, i, original);
                OnMeasureItem(mievent);
                if (mievent.ItemHeight != original)
                {
                    PInvoke.SendMessage(this, PInvoke.CB_SETITEMHEIGHT, (WPARAM)i, (LPARAM)mievent.ItemHeight);
                }
            }
        }
    }

    /// <summary>
    ///  Forces the text to be updated based on the current selection.
    /// </summary>
    private void UpdateText()
    {
        // Fire text changed for dropdown combos when the selection
        //           changes, since the text really does change.  We've got
        //           to do this asynchronously because the actual edit text
        //           isn't updated until a bit later
        //

        // v1.0 - ComboBox::set_Text compared items w/ "value" and set the SelectedIndex accordingly
        // v1.0 - null values can't correspond to String.Empty
        // v1.0 - SelectedIndex == -1 corresponds to Text == String.Emtpy
        //
        // v1.1 - ComboBox::set_Text compares FilterItemOnProperty(item) w/ "value" and set the SelectedIndex accordingly
        // v1.1 - null values correspond to String.Empty
        // v1.1 - SelectedIndex == -1 corresponds to Text is null
        string? s = null;

        if (SelectedIndex != -1)
        {
            object? item = Items[SelectedIndex];
            if (item is not null)
            {
                s = GetItemText(item);
            }
        }

        Text = s;

        if (DropDownStyle == ComboBoxStyle.DropDown)
        {
            if (_childEdit is not null && !_childEdit.HWND.IsNull)
            {
                PInvoke.SendMessage(_childEdit, PInvoke.WM_SETTEXT, 0, s);
            }
        }
    }

    private void WmEraseBkgnd(ref Message m)
    {
        if ((DropDownStyle == ComboBoxStyle.Simple) && ParentInternal is not null)
        {
            PInvokeCore.GetClientRect(this, out RECT rect);
            HDC hdc = (HDC)m.WParamInternal;
            using var hbrush = new CreateBrushScope(ParentInternal?.BackColor ?? Application.SystemColors.Control);
            hdc.FillRectangle(rect, hbrush);
            m.ResultInternal = (LRESULT)1;
            return;
        }

        base.WndProc(ref m);
    }

    private void WmParentNotify(ref Message m)
    {
        base.WndProc(ref m);
        if ((int)m.WParamInternal == ((int)PInvoke.WM_CREATE | 1000 << 16))
        {
            _dropDownHandle = (HWND)m.LParamInternal;

            // By some reason WmParentNotify with WM_DESTROY is not called before recreation.
            // So release the old references here.
            _childDropDown?.ReleaseHandle();
            _childDropDown = new ComboBoxChildNativeWindow(this, ChildWindowType.DropDownList);
            _childDropDown.AssignHandle(_dropDownHandle);

            // Reset the child list accessible object in case the DDL is recreated.
            // For instance when dialog window containing the ComboBox is reopened.
            _childListAccessibleObject = null;
        }
    }

    /// <summary>
    ///  Text change behavior.
    ///  Here are the window messages corresponding to each user event.
    ///
    ///  DropDown (free text window):
    ///  Type in Text Window:
    ///   CBN_EDITUPDATE
    ///   CBN_EDITCHANGE
    ///  Down/Up Arrow
    ///   CBN_SELENDOK   (text not changed yet)
    ///   CBN_SELCHANGE
    ///  with text set to beginning of a valid item -- drop and close dropdown (selects that item)
    ///   CBN_DROPDOWN
    ///   CBN_CLOSEUP
    ///  Drop List, up/down arrow to select item, click away
    ///   CBN_DROPDOWN
    ///   CBN_SELCHANGE
    ///   CBN_CLOSEUP
    ///  Drop List, click on item
    ///   CBN_DROPDOWN
    ///   CBN_SELENDOK
    ///   CBN_CLOSEUP
    ///   CBN_SELCHANGE (text changes here via selected item)
    ///
    ///  DropDownList (limited text window):
    ///
    ///  Type text and arrow up/down:
    ///   CBN_SELENDOK  (text already changed)
    ///   CBN_SELCHANGE
    ///  Drop List, up/down arrow to select item, click away
    ///   CBN_DROPDOWN
    ///   CBN_SELCHANGE
    ///   CBN_CLOSEUP
    ///  Drop List, click on item
    ///   CBN_DROPDOWN
    ///   CBN_SELENDOK
    ///   CBN_CLOSEUP
    ///   CBN_SELCHANGE
    ///
    ///  Simple (listbox visible):
    ///  Type in Text Window:
    ///   CBN_EDITUPDATE
    ///   CBN_EDITCHANGE
    ///  Down/Up Arrow
    ///   CBN_SELENDOK    (text not changed yet)
    ///   CBN_SELCHANGE
    ///  Click on item
    ///   CBN_SELENDOK    (text not changed yet)
    ///   CBN_SELCHANGE
    ///
    ///
    ///  What we do is fire textchange events in these messages:
    ///  CBN_SELCHANGE
    ///  CBN_EDITCHANGE
    ///  CBN_CLOSEUP
    ///
    ///  and we only actually call the real event if the Text is different than currentText.
    ///  currentText is never changed outside this method.
    ///  This internal version can be called from anywhere we might suspect text has changed
    ///  it's fairly safe to call anywhere.
    /// </summary>
    private void WmReflectCommand(ref Message m)
    {
        switch ((uint)m.WParamInternal.SIGNEDHIWORD)
        {
            case PInvoke.CBN_DBLCLK:
                break;
            case PInvoke.CBN_EDITUPDATE:
                OnTextUpdate(EventArgs.Empty);
                break;
            case PInvoke.CBN_CLOSEUP:

                OnDropDownClosed(EventArgs.Empty);
                if (FormattingEnabled && Text != _currentText && _dropDown)
                {
                    OnTextChanged(EventArgs.Empty);
                }

                _dropDown = false;
                break;
            case PInvoke.CBN_DROPDOWN:
                _currentText = Text;
                _dropDown = true;
                OnDropDown(EventArgs.Empty);
                UpdateDropDownHeight();

                break;
            case PInvoke.CBN_EDITCHANGE:
                OnTextChanged(EventArgs.Empty);
                break;
            case PInvoke.CBN_SELCHANGE:
                UpdateText();
                OnSelectedIndexChanged(EventArgs.Empty);
                break;
            case PInvoke.CBN_SELENDOK:
                OnSelectionChangeCommittedInternal(EventArgs.Empty);
                break;
        }
    }

    private unsafe void WmReflectDrawItem(ref Message m)
    {
        DRAWITEMSTRUCT* dis = (DRAWITEMSTRUCT*)(nint)m.LParamInternal;

        using DrawItemEventArgs e = new(
            dis->hDC,
            Font,
            dis->rcItem,
            dis->itemID,
            dis->itemState,
            ForeColor,
            BackColor);

        OnDrawItem(e);

        m.ResultInternal = (LRESULT)1;
    }

    private unsafe void WmReflectMeasureItem(ref Message m)
    {
        MEASUREITEMSTRUCT* mis = (MEASUREITEMSTRUCT*)(nint)m.LParamInternal;

        // Determine if message was sent by a combo item or the combo edit field
        int itemID = (int)mis->itemID;
        if (DrawMode == DrawMode.OwnerDrawVariable && itemID >= 0)
        {
            using Graphics graphics = CreateGraphicsInternal();
            MeasureItemEventArgs mie = new(graphics, itemID, ItemHeight);
            OnMeasureItem(mie);
            mis->itemHeight = unchecked((uint)mie.ItemHeight);
        }
        else
        {
            // Message was sent by the combo edit field
            mis->itemHeight = (uint)ItemHeight;
        }

        m.ResultInternal = (LRESULT)1;
    }

    /// <summary>
    ///  The Combobox's window procedure.  Inheriting classes can override this
    ///  to add extra functionality, but should not forget to call
    ///  base.wndProc(m); to ensure the combo continues to function properly.
    /// </summary>
    protected override unsafe void WndProc(ref Message m)
    {
        switch (m.MsgInternal)
        {
            // We don't want to fire the focus events twice -
            // once in the combobox and once in the ChildWndProc.
            case PInvoke.WM_SETFOCUS:
                try
                {
                    _fireSetFocus = false;
                    base.WndProc(ref m);
                }
                finally
                {
                    _fireSetFocus = true;
                }

                break;
            case PInvoke.WM_KILLFOCUS:
                try
                {
                    _fireLostFocus = false;
                    base.WndProc(ref m);
                    // Nothing to see here... Just keep on walking...
                    // Turns out that with Theming off, we don't get quite the same messages as with theming on.

                    // With theming on we get a WM_MOUSELEAVE after a WM_KILLFOCUS even if you use the Tab key
                    // to move focus. Our response to WM_MOUSELEAVE causes us to repaint everything correctly.

                    // With theming off, we do not get a WM_MOUSELEAVE after a WM_KILLFOCUS, and since we don't have a childwndproc
                    // when we are a Flat DropDownList, we need to force a repaint. The easiest way to do this is to send a
                    // WM_MOUSELEAVE to ourselves, since that also sets up the right state. Or... at least the state is the same
                    // as with Theming on.

                    if (!Application.RenderWithVisualStyles && !GetStyle(ControlStyles.UserPaint)
                        && DropDownStyle == ComboBoxStyle.DropDownList
                        && (FlatStyle == FlatStyle.Flat || FlatStyle == FlatStyle.Popup))
                    {
                        PInvoke.PostMessage(this, PInvoke.WM_MOUSELEAVE);
                    }
                }
                finally
                {
                    _fireLostFocus = true;
                }

                break;
            case PInvoke.WM_CTLCOLOREDIT:
            case PInvoke.WM_CTLCOLORLISTBOX:
                m.ResultInternal = (LRESULT)(nint)InitializeDCForWmCtlColor((HDC)(nint)m.WParamInternal, m.MsgInternal);
                break;
            case PInvoke.WM_ERASEBKGND:
                WmEraseBkgnd(ref m);
                break;
            case PInvoke.WM_PARENTNOTIFY:
                WmParentNotify(ref m);
                break;
            case MessageId.WM_REFLECT_COMMAND:
                WmReflectCommand(ref m);
                break;
            case MessageId.WM_REFLECT_DRAWITEM:
                WmReflectDrawItem(ref m);
                break;
            case MessageId.WM_REFLECT_MEASUREITEM:
                WmReflectMeasureItem(ref m);
                break;
            case PInvoke.WM_LBUTTONDOWN:
                _mouseEvents = true;
                base.WndProc(ref m);
                break;
            case PInvoke.WM_LBUTTONUP:
                PInvoke.GetWindowRect(this, out var rect);
                Rectangle clientRect = rect;

                Point point = PointToScreen(PARAM.ToPoint(m.LParamInternal));

                // _mouseEvents is used to keep the check that we get the WM_LBUTTONUP after WM_LBUTTONDOWN or
                // WM_LBUTTONDBLBCLK combo box gets a WM_LBUTTONUP for focus change.
                if (_mouseEvents && !ValidationCancelled)
                {
                    _mouseEvents = false;
                    bool captured = Capture;
                    if (captured && clientRect.Contains(point))
                    {
                        OnClick(new MouseEventArgs(MouseButtons.Left, 1, PARAM.ToPoint(m.LParamInternal)));
                        OnMouseClick(new MouseEventArgs(MouseButtons.Left, 1, PARAM.ToPoint(m.LParamInternal)));
                    }

                    base.WndProc(ref m);
                }
                else
                {
                    Capture = false;
                    DefWndProc(ref m);
                }

                break;

            case PInvoke.WM_MOUSELEAVE:
                DefWndProc(ref m);
                OnMouseLeaveInternal(EventArgs.Empty);
                break;

            case PInvoke.WM_PAINT:
                if (!GetStyle(ControlStyles.UserPaint)
                    && (FlatStyle == FlatStyle.Flat || FlatStyle == FlatStyle.Popup)
                    && !(SystemInformation.HighContrast && BackColor == Application.SystemColors.Window))
                {
                    using RegionScope dropDownRegion = new(FlatComboBoxAdapter._dropDownRect);
                    using RegionScope windowRegion = new(Bounds);

                    // Stash off the region we have to update (the base is going to clear this off in BeginPaint)
                    bool getRegionSucceeded = PInvoke.GetUpdateRgn(HWND, windowRegion, bErase: true) != GDI_REGION_TYPE.RGN_ERROR;

                    PInvokeCore.CombineRgn(dropDownRegion, windowRegion, dropDownRegion, RGN_COMBINE_MODE.RGN_DIFF);
                    RECT updateRegionBoundingRect = default;
                    PInvoke.GetRgnBox(windowRegion, &updateRegionBoundingRect);

                    // Call the base class to do its painting (with a clipped DC).
                    bool useBeginPaint = m.WParamInternal == 0u;
                    using var paintScope = useBeginPaint ? new BeginPaintScope(HWND) : default;

                    HDC dc = useBeginPaint ? paintScope! : (HDC)m.WParamInternal;

                    using SaveDcScope savedDcState = new(dc);

                    if (getRegionSucceeded)
                    {
                        PInvokeCore.SelectClipRgn(dc, dropDownRegion);
                    }

                    m.WParamInternal = (WPARAM)dc;
                    DefWndProc(ref m);

                    if (getRegionSucceeded)
                    {
                        PInvokeCore.SelectClipRgn(dc, windowRegion);
                    }

                    using Graphics g = Graphics.FromHdcInternal((IntPtr)dc);
                    FlatComboBoxAdapter.DrawFlatCombo(this, g);

                    return;
                }

                base.WndProc(ref m);
                break;

            case PInvoke.WM_PRINTCLIENT:
                // All the fancy stuff we do in OnPaint has to happen again in OnPrint.
                if (!GetStyle(ControlStyles.UserPaint) && (FlatStyle == FlatStyle.Flat || FlatStyle == FlatStyle.Popup))
                {
                    DefWndProc(ref m);

                    if (((nint)m.LParamInternal & PInvoke.PRF_CLIENT) == PInvoke.PRF_CLIENT)
                    {
                        if (!GetStyle(ControlStyles.UserPaint) && (FlatStyle == FlatStyle.Flat || FlatStyle == FlatStyle.Popup))
                        {
                            using Graphics g = Graphics.FromHdcInternal((HDC)m.WParamInternal);
                            FlatComboBoxAdapter.DrawFlatCombo(this, g);
                        }

                        return;
                    }
                }

                base.WndProc(ref m);
                return;

            case PInvoke.WM_SETCURSOR:
                base.WndProc(ref m);
                break;

            case PInvoke.WM_SETFONT:
                if (Width == 0)
                {
                    _suppressNextWindowsPos = true;
                }

                base.WndProc(ref m);
                break;

            case PInvoke.WM_WINDOWPOSCHANGED:
                if (!_suppressNextWindowsPos)
                {
                    base.WndProc(ref m);
                }

                _suppressNextWindowsPos = false;
                break;

            case PInvoke.WM_NCDESTROY:
                base.WndProc(ref m);
                ReleaseChildWindow();
                break;

            default:
                if (m.MsgInternal == RegisteredMessage.WM_MOUSEENTER)
                {
                    DefWndProc(ref m);
                    OnMouseEnterInternal(EventArgs.Empty);
                    break;
                }

                base.WndProc(ref m);
                break;
        }
    }

    private FlatComboAdapter FlatComboBoxAdapter
    {
        get
        {
            if (!(Properties.GetObject(s_propFlatComboAdapter) is FlatComboAdapter comboAdapter) || !comboAdapter.IsValid(this))
            {
                comboAdapter = CreateFlatComboAdapterInstance();
                Properties.SetObject(s_propFlatComboAdapter, comboAdapter);
            }

            return comboAdapter;
        }
    }

    internal virtual FlatComboAdapter CreateFlatComboAdapterInstance()
        => new(this, smallButton: false);
}
