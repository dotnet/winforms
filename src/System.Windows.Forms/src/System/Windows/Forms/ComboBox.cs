// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Buffers;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms.Layout;
using static System.Windows.Forms.ComboBox.ObjectCollection;
using static Interop;
using static Interop.User32;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Displays an editing field and a list, allowing the user to select from thelist or to enter new text. Displays
    ///  only the editing field until the user explicitly displays the list.
    /// </summary>
    [DefaultEvent(nameof(SelectedIndexChanged))]
    [DefaultProperty(nameof(Items))]
    [DefaultBindingProperty(nameof(Text))]
    [Designer("System.Windows.Forms.Design.ComboBoxDesigner, " + AssemblyRef.SystemDesign)]
    [SRDescription(nameof(SR.DescriptionComboBox))]
    public partial class ComboBox : ListControl
    {
        private static readonly object EVENT_DROPDOWN = new object();
        private static readonly object EVENT_DRAWITEM = new object();
        private static readonly object EVENT_MEASUREITEM = new object();
        private static readonly object EVENT_SELECTEDINDEXCHANGED = new object();
        private static readonly object EVENT_SELECTIONCHANGECOMMITTED = new object();
        private static readonly object EVENT_SELECTEDITEMCHANGED = new object();
        private static readonly object EVENT_DROPDOWNSTYLE = new object();
        private static readonly object EVENT_TEXTUPDATE = new object();
        private static readonly object EVENT_DROPDOWNCLOSED = new object();

        private static readonly int PropMaxLength = PropertyStore.CreateKey();
        private static readonly int PropItemHeight = PropertyStore.CreateKey();
        private static readonly int PropDropDownWidth = PropertyStore.CreateKey();
        private static readonly int PropDropDownHeight = PropertyStore.CreateKey();
        private static readonly int PropStyle = PropertyStore.CreateKey();
        private static readonly int PropDrawMode = PropertyStore.CreateKey();
        private static readonly int PropMatchingText = PropertyStore.CreateKey();
        private static readonly int PropFlatComboAdapter = PropertyStore.CreateKey();

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

        private ComboBoxChildNativeWindow _childDropDown;
        private ComboBoxChildNativeWindow _childEdit;
        private ComboBoxChildNativeWindow _childListBox;

        private IntPtr _dropDownHandle;
        private ObjectCollection _itemsCollection;
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
        private bool _suppressNextWindosPos;
        private bool _canFireLostFocus;

        // When the user types a letter and drops the dropdown the combobox itself auto-searches the matching item and
        // selects the item in the edit thus changing the windowText. Hence we should Fire the TextChanged event in
        // such a scenario. The string below is used for checking the window Text before and after the dropdown.
        private string _currentText = string.Empty;
        private string _lastTextChangedValue;
        private bool _dropDown;
        private readonly AutoCompleteDropDownFinder _finder = new AutoCompleteDropDownFinder();

        private bool _selectedValueChangedFired;
        private AutoCompleteMode _autoCompleteMode = AutoCompleteMode.None;
        private AutoCompleteSource _autoCompleteSource = AutoCompleteSource.None;

        /// <summary>
        ///  This stores the custom StringCollection required for the autoCompleteSource when its set to CustomSource.
        /// </summary>
        private AutoCompleteStringCollection _autoCompleteCustomSource;
        private StringSource _stringSource;
        private bool _fromHandleCreate;

        private ComboBoxChildListUiaProvider _childListAccessibleObject;
        private ComboBoxChildEditUiaProvider _childEditAccessibleObject;
        private ComboBoxChildTextUiaProvider _childTextAccessibleObject;

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
                //valid values are 0x0 to 0x3
                SourceGenerated.EnumValidator.Validate(value);
                if (DropDownStyle == ComboBoxStyle.DropDownList &&
                    AutoCompleteSource != AutoCompleteSource.ListItems &&
                    value != AutoCompleteMode.None)
                {
                    throw new NotSupportedException(SR.ComboBoxAutoCompleteModeOnlyNoneAllowed);
                }

                if (Application.OleRequired() != System.Threading.ApartmentState.STA)
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

                if (DropDownStyle == ComboBoxStyle.DropDownList &&
                    AutoCompleteMode != AutoCompleteMode.None &&
                    value != AutoCompleteSource.ListItems)
                {
                    throw new NotSupportedException(SR.ComboBoxAutoCompleteSourceOnlyListItemsAllowed);
                }

                if (Application.OleRequired() != System.Threading.ApartmentState.STA)
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
        [Editor("System.Windows.Forms.Design.ListControlStringCollectionEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor))]
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        public AutoCompleteStringCollection AutoCompleteCustomSource
        {
            get
            {
                if (_autoCompleteCustomSource is null)
                {
                    _autoCompleteCustomSource = new AutoCompleteStringCollection();
                    _autoCompleteCustomSource.CollectionChanged += new CollectionChangeEventHandler(OnAutoCompleteCustomSourceChanged);
                }

                return _autoCompleteCustomSource;
            }
            set
            {
                if (_autoCompleteCustomSource != value)
                {
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
                    return SystemColors.Window;
                }
            }
            set => base.BackColor = value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public override Image BackgroundImage
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
        new public event EventHandler BackgroundImageChanged
        {
            add => base.BackgroundImageChanged += value;
            remove => base.BackgroundImageChanged -= value;
        }

        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler BackgroundImageLayoutChanged
        {
            add => base.BackgroundImageLayoutChanged += value;
            remove => base.BackgroundImageLayoutChanged -= value;
        }

        internal ChildAccessibleObject ChildEditAccessibleObject
        {
            get
            {
                if (_childEditAccessibleObject is null)
                {
                    _childEditAccessibleObject = new ComboBoxChildEditUiaProvider(this, _childEdit.Handle);
                }

                return _childEditAccessibleObject;
            }
        }

        internal ChildAccessibleObject ChildListAccessibleObject
        {
            get
            {
                if (_childListAccessibleObject is null)
                {
                    _childListAccessibleObject =
                        new ComboBoxChildListUiaProvider(this, DropDownStyle == ComboBoxStyle.Simple ? _childListBox.Handle : _dropDownHandle);
                }

                return _childListAccessibleObject;
            }
        }

        internal AccessibleObject ChildTextAccessibleObject
        {
            get
            {
                if (_childTextAccessibleObject is null)
                {
                    _childTextAccessibleObject = new ComboBoxChildTextUiaProvider(this);
                }

                return _childTextAccessibleObject;
            }
        }

        /// <summary>
        ///  Returns the parameters needed to create the handle.  Inheriting classes
        ///  can override this to provide extra functionality.  They should not,
        ///  however, forget to call base.CreateParams() first to get the struct
        ///  filled up with the basic info.
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                CreateParams cp = base.CreateParams;
                cp.ClassName = ComCtl32.WindowClasses.WC_COMBOBOX;
                cp.Style |= (int)WS.VSCROLL | (int)CBS.HASSTRINGS | (int)CBS.AUTOHSCROLL;
                cp.ExStyle |= (int)WS_EX.CLIENTEDGE;
                if (!_integralHeight)
                {
                    cp.Style |= (int)CBS.NOINTEGRALHEIGHT;
                }

                switch (DropDownStyle)
                {
                    case ComboBoxStyle.Simple:
                        cp.Style |= (int)CBS.SIMPLE;
                        break;
                    case ComboBoxStyle.DropDown:
                        cp.Style |= (int)CBS.DROPDOWN;
                        // Make sure we put the height back or we won't be able to size the dropdown!
                        cp.Height = PreferredHeight;
                        break;
                    case ComboBoxStyle.DropDownList:
                        cp.Style |= (int)CBS.DROPDOWNLIST;
                        // Comment above...
                        cp.Height = PreferredHeight;
                        break;
                }

                switch (DrawMode)
                {
                    case DrawMode.OwnerDrawFixed:
                        cp.Style |= (int)CBS.OWNERDRAWFIXED;
                        break;
                    case DrawMode.OwnerDrawVariable:
                        cp.Style |= (int)CBS.OWNERDRAWVARIABLE;
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
        public new object DataSource
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
                int drawMode = Properties.GetInteger(PropDrawMode, out bool found);
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
                    //valid values are 0x0 to 0x2.
                    SourceGenerated.EnumValidator.Validate(value);
                    ResetHeightCache();
                    Properties.SetInteger(PropDrawMode, (int)value);
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
                int dropDownWidth = Properties.GetInteger(PropDropDownWidth, out bool found);

                if (found)
                {
                    return dropDownWidth;
                }
                else
                {
                    return Width;
                }
            }

            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidArgument, nameof(DropDownWidth), value));
                }

                if (Properties.GetInteger(PropDropDownWidth) != value)
                {
                    Properties.SetInteger(PropDropDownWidth, value);
                    if (IsHandleCreated)
                    {
                        SendMessageW(this, (WM)CB.SETDROPPEDWIDTH, (IntPtr)value);
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
                int dropDownHeight = Properties.GetInteger(PropDropDownHeight, out bool found);
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
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidArgument, nameof(DropDownHeight), value));
                }

                if (Properties.GetInteger(PropDropDownHeight) != value)
                {
                    Properties.SetInteger(PropDropDownHeight, value);

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
                    return unchecked((int)(long)SendMessageW(this, (WM)CB.GETDROPPEDSTATE)) != 0;
                }

                return false;
            }
            set
            {
                if (!IsHandleCreated)
                {
                    CreateHandle();
                }

                SendMessageW(this, (WM)CB.SHOWDROPDOWN, (IntPtr)(value ? -1 : 0));
            }
        }

        /// <summary>
        ///  Gets or
        ///  sets
        ///  the flat style appearance of the button control.
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
                //valid values are 0x0 to 0x3
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

                IntPtr focus = GetFocus();
                return focus != IntPtr.Zero && ((_childEdit is not null && focus == _childEdit.Handle) || (_childListBox is not null && focus == _childListBox.Handle));
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
                    return SystemColors.WindowText;
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
                    int itemHeight = Properties.GetInteger(PropItemHeight, out bool found);
                    if (found)
                    {
                        return itemHeight;
                    }
                    else
                    {
                        return FontHeight + 2;   //
                    }
                }

                // Note that the above if clause deals with the case when the handle has not yet been created
                Debug.Assert(IsHandleCreated, "Handle should be created at this point");

                int h = unchecked((int)(long)SendMessageW(this, (WM)CB.GETITEMHEIGHT));
                if (h == -1)
                {
                    throw new Win32Exception();
                }

                return h;
            }

            set
            {
                if (value < 1)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidArgument, nameof(ItemHeight), value));
                }

                ResetHeightCache();

                if (Properties.GetInteger(PropItemHeight) != value)
                {
                    Properties.SetInteger(PropItemHeight, value);
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
        [Editor("System.Windows.Forms.Design.ListControlStringCollectionEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor))]
        [MergableProperty(false)]
        public ObjectCollection Items
        {
            get
            {
                if (_itemsCollection is null)
                {
                    _itemsCollection = new ObjectCollection(this);
                }

                return _itemsCollection;
            }
        }

        // Text used to match an item in the list when auto-completion
        // is used in DropDownList style.
        private string MatchingText
        {
            get
            {
                string matchingText = (string)Properties.GetObject(PropMatchingText);
                return matchingText ?? string.Empty;
            }
            set
            {
                if (value is not null || Properties.ContainsObject(PropMatchingText))
                {
                    Properties.SetObject(PropMatchingText, value);
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
                if (value < 1 || value > 100)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidBoundArgument, nameof(MaxDropDownItems), value, 1, 100));
                }

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
                return Properties.GetInteger(PropMaxLength);
            }
            set
            {
                if (value < 0)
                {
                    value = 0;
                }

                if (MaxLength != value)
                {
                    Properties.SetInteger(PropMaxLength, value);
                    if (IsHandleCreated)
                    {
                        SendMessageW(this, (WM)CB.LIMITTEXT, (IntPtr)value);
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
        public new event EventHandler PaddingChanged
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
                    //do preferred height the old broken way for everett apps
                    //we need this for compat reasons because (get this)
                    //  (a) everett preferredheight was always wrong.
                    //  (b) so, when combobox1.Size = actualdefaultsize was called, it would enter setboundscore
                    //  (c) this updated requestedheight
                    //  (d) if the user then changed the combo to simple style, the height did not change.
                    // We simply cannot match this behavior if preferredheight is corrected so that (b) never
                    // occurs.  We simply do not know when Size was set.

                    // So in whidbey, the behavior will be:
                    //  (1) user uses default size = setting dropdownstyle=simple will revert to simple height
                    //  (2) user uses nondefault size = setting dropdownstyle=simple will not change height from this value

                    //In everett
                    //  if the user manually sets Size = (121, 20) in code (usually height gets forced to 21), then he will see Whidey.(1) above
                    //  user usually uses nondefault size and will experience whidbey.(2) above

                    Size textSize = TextRenderer.MeasureText(LayoutUtils.TestString, Font, new Size(short.MaxValue, (int)(FontHeight * 1.25)), TextFormatFlags.SingleLine);
                    _prefHeightCache = (short)(textSize.Height + SystemInformation.BorderSize.Height * 8 + Padding.Size.Height);

                    return _prefHeightCache;
                }
                else
                {
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

        private string[] GetStringsForAutoComplete(IList collection)
        {
            if (collection is AutoCompleteStringCollection)
            {
                string[] strings = new string[AutoCompleteCustomSource.Count];
                for (int i = 0; i < AutoCompleteCustomSource.Count; i++)
                {
                    strings[i] = AutoCompleteCustomSource[i];
                }

                return strings;
            }
            else if (collection is ObjectCollection)
            {
                string[] strings = new string[_itemsCollection.Count];
                for (int i = 0; i < _itemsCollection.Count; i++)
                {
                    strings[i] = GetItemText(_itemsCollection[i]);
                }

                return strings;
            }

            return Array.Empty<string>();
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
                    return unchecked((int)(long)SendMessageW(this, (WM)CB.GETCURSEL));
                }

                return _selectedIndex;
            }
            set
            {
                if (SelectedIndex != value)
                {
                    int itemCount = 0;
                    if (_itemsCollection is not null)
                    {
                        itemCount = _itemsCollection.Count;
                    }

                    if (value < -1 || value >= itemCount)
                    {
                        throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidArgument, nameof(SelectedIndex), value));
                    }

                    if (IsHandleCreated)
                    {
                        SendMessageW(this, (WM)CB.SETCURSEL, (IntPtr)value);
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
        }

        /// <summary>
        ///  The handle to the object that is currently selected in the
        ///  combos list.
        /// </summary>
        [Browsable(false)]
        [Bindable(true)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRDescription(nameof(SR.ComboBoxSelectedItemDescr))]
        public object SelectedItem
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
                    //
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
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        [SRDescription(nameof(SR.ComboBoxSelectedTextDescr))]
        public string SelectedText
        {
            get
            {
                if (DropDownStyle == ComboBoxStyle.DropDownList)
                {
                    return "";
                }

                return Text.Substring(SelectionStart, SelectionLength);
            }
            set
            {
                if (DropDownStyle != ComboBoxStyle.DropDownList)
                {
                    //guard against null string, since otherwise we will throw an
                    //AccessViolation exception, which is bad
                    string str = (value ?? "");
                    CreateControl();
                    if (IsHandleCreated && _childEdit is not null)
                    {
                        SendMessageW(new HandleRef(this, _childEdit.Handle), (WM)EM.REPLACESEL, (IntPtr)(-1), str);
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
                SendMessageW(this, (WM)CB.GETEDITSEL, (IntPtr)(&start), (IntPtr)(&end));
                return end - start;
            }
            set
            {
                // SelectionLength can be negtive...
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
                SendMessageW(this, (WM)CB.GETEDITSEL, (IntPtr)(&value), IntPtr.Zero);
                return value;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidArgument, nameof(SelectionStart), value));
                }

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
                int style = Properties.GetInteger(PropStyle, out bool found);
                if (found)
                {
                    return (ComboBoxStyle)style;
                }

                return ComboBoxStyle.DropDown;
            }
            set
            {
                if (DropDownStyle != value)
                {
                    // verify that 'value' is a valid enum type...
                    //valid values are 0x0 to 0x2
                    SourceGenerated.EnumValidator.Validate(value);

                    if (value == ComboBoxStyle.DropDownList &&
                        AutoCompleteSource != AutoCompleteSource.ListItems &&
                        AutoCompleteMode != AutoCompleteMode.None)
                    {
                        AutoCompleteMode = AutoCompleteMode.None;
                    }

                    // reset preferred height.
                    ResetHeightCache();

                    Properties.SetInteger(PropStyle, (int)value);

                    if (IsHandleCreated)
                    {
                        RecreateHandle();
                    }

                    OnDropDownStyleChanged(EventArgs.Empty);
                }
            }
        }

        [Localizable(true)]
        [Bindable(true)]
        public override string Text
        {
            get
            {
                if (SelectedItem is not null && !BindingFieldEmpty)
                {
                    //preserve everett behavior if "formatting enabled == false" -- just return selecteditem text.
                    if (FormattingEnabled)
                    {
                        string candidate = GetItemText(SelectedItem);
                        if (!string.IsNullOrEmpty(candidate))
                        {
                            if (string.Compare(candidate, base.Text, true, CultureInfo.CurrentCulture) == 0)
                            {
                                return candidate;   //for whidbey, if we only differ by case -- return the candidate;
                            }
                        }
                    }
                    else
                    {
                        return FilterItemOnProperty(SelectedItem).ToString();       //heinous.
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
                object selectedItem = null;

                selectedItem = SelectedItem;

                if (!DesignMode)
                {
                    //

                    if (value is null)
                    {
                        SelectedIndex = -1;
                    }
                    else if (value is not null &&
                        (selectedItem is null || (string.Compare(value, GetItemText(selectedItem), false, CultureInfo.CurrentCulture) != 0)))
                    {
                        int index = FindStringIgnoreCase(value);

                        //we cannot set the index to -1 unless we want to do something unusual and save/restore text
                        //because the native control will erase the text when we change the index to -1
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
            //look for an exact match and then a case insensitive match if that fails.
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

            //don't fire textch if we had set the selectedindex -- because it was already fired if so.
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
        public new event EventHandler DoubleClick
        {
            add => base.DoubleClick += value;
            remove => base.DoubleClick -= value;
        }

        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.drawItemEventDescr))]
        public event DrawItemEventHandler DrawItem
        {
            add => Events.AddHandler(EVENT_DRAWITEM, value);
            remove => Events.RemoveHandler(EVENT_DRAWITEM, value);
        }

        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.ComboBoxOnDropDownDescr))]
        public event EventHandler DropDown
        {
            add => Events.AddHandler(EVENT_DROPDOWN, value);
            remove => Events.RemoveHandler(EVENT_DROPDOWN, value);
        }

        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.measureItemEventDescr))]
        public event MeasureItemEventHandler MeasureItem
        {
            add
            {
                Events.AddHandler(EVENT_MEASUREITEM, value);
                UpdateItemHeight();
            }
            remove
            {
                Events.RemoveHandler(EVENT_MEASUREITEM, value);
                UpdateItemHeight();
            }
        }

        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.selectedIndexChangedEventDescr))]
        public event EventHandler SelectedIndexChanged
        {
            add => Events.AddHandler(EVENT_SELECTEDINDEXCHANGED, value);
            remove => Events.RemoveHandler(EVENT_SELECTEDINDEXCHANGED, value);
        }

        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.selectionChangeCommittedEventDescr))]
        public event EventHandler SelectionChangeCommitted
        {
            add => Events.AddHandler(EVENT_SELECTIONCHANGECOMMITTED, value);
            remove => Events.RemoveHandler(EVENT_SELECTIONCHANGECOMMITTED, value);
        }

        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.ComboBoxDropDownStyleChangedDescr))]
        public event EventHandler DropDownStyleChanged
        {
            add => Events.AddHandler(EVENT_DROPDOWNSTYLE, value);
            remove => Events.RemoveHandler(EVENT_DROPDOWNSTYLE, value);
        }

        /// <summary>
        ///  ComboBox Onpaint.
        /// </summary>
        /// <hideinheritance/>
        [Browsable(false)]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public new event PaintEventHandler Paint
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
        public event EventHandler TextUpdate
        {
            add => Events.AddHandler(EVENT_TEXTUPDATE, value);
            remove => Events.RemoveHandler(EVENT_TEXTUPDATE, value);
        }

        /// <summary>
        ///  This will fire the DropDownClosed Event on the ComboBox. This events fires when the Combobox gets the
        ///  CBN_CLOSEUP notification. This happens when the DropDown closes.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.ComboBoxOnDropDownClosedDescr))]
        public event EventHandler DropDownClosed
        {
            add => Events.AddHandler(EVENT_DROPDOWNCLOSED, value);
            remove => Events.RemoveHandler(EVENT_DROPDOWNCLOSED, value);
        }

        /// <summary>
        ///  Performs the work of adding the specified items to the combobox
        /// </summary>
        [Obsolete("This method has been deprecated.  There is no replacement.  https://go.microsoft.com/fwlink/?linkid=14202")]
        protected virtual void AddItemsCore(object[] value)
        {
            int count = value is null ? 0 : value.Length;
            if (count == 0)
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
            return (_updateCount == 0);
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

            // Get the Combox Rect
            var comboRectMid = new RECT();
            GetWindowRect(this, ref comboRectMid);

            //Get the Edit Rectangle.
            var editRectMid = new RECT();
            GetWindowRect(_childEdit, ref editRectMid);

            //get the delta
            int comboXMid = PARAM.SignedLOWORD(m.LParam) + (editRectMid.left - comboRectMid.left);
            int comboYMid = PARAM.SignedHIWORD(m.LParam) + (editRectMid.top - comboRectMid.top);

            return new Point(comboXMid, comboYMid);
        }

        /// <summary>
        ///  Subclassed window procedure for the edit and list child controls of the
        ///  combo box.
        /// </summary>
        private void ChildWndProc(ref Message m)
        {
            switch ((WM)m.Msg)
            {
                case WM.CHAR:
                    if (DropDownStyle == ComboBoxStyle.Simple && m.HWnd == _childListBox.Handle)
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
                case WM.SYSCHAR:
                    if (DropDownStyle == ComboBoxStyle.Simple && m.HWnd == _childListBox.Handle)
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
                case WM.KEYDOWN:
                case WM.SYSKEYDOWN:
                    if (SystemAutoCompleteEnabled && !ACNativeWindow.AutoCompleteActive)
                    {
                        _finder.FindDropDowns(false);
                    }

                    if (AutoCompleteMode != AutoCompleteMode.None)
                    {
                        char keyChar = unchecked((char)(long)m.WParam);
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

                    if (DropDownStyle == ComboBoxStyle.Simple && m.HWnd == _childListBox.Handle)
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

                case WM.INPUTLANGCHANGE:
                    DefChildWndProc(ref m);
                    break;

                case WM.KEYUP:
                case WM.SYSKEYUP:
                    if (DropDownStyle == ComboBoxStyle.Simple && m.HWnd == _childListBox.Handle)
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
                case WM.KILLFOCUS:
                    // Consider - If we dont' have a childwndproc, then we don't get here, so we don't
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
                case WM.SETFOCUS:

                    // Consider - If we dont' have a childwndproc, then we don't get here, so we don't
                    // set the status. Do we need to? This happens when we have a DropDownList.
                    if (!DesignMode)
                    {
                        ImeContext.SetImeStatus(CachedImeMode, m.HWnd);
                    }

                    if (!HostedInWin32DialogManager)
                    {
                        IContainerControl c = GetContainerControl();
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

                case WM.SETFONT:
                    DefChildWndProc(ref m);
                    if (_childEdit is not null && m.HWnd == _childEdit.Handle)
                    {
                        SendMessageW(new HandleRef(this, _childEdit.Handle), (WM)EM.SETMARGINS, (IntPtr)(EC.LEFTMARGIN | EC.RIGHTMARGIN));
                    }

                    break;
                case WM.LBUTTONDBLCLK:
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

                case WM.MBUTTONDBLCLK:
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

                case WM.RBUTTONDBLCLK:
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

                case WM.LBUTTONDOWN:
                    _mousePressed = true;
                    _mouseEvents = true;

                    // Set the mouse capture as this is the child Wndproc.
                    Capture = true;
                    DefChildWndProc(ref m);

                    // The message gets fired from Combo-box's WndPrc - convert to Combobox coordinates
                    Point Ptl = EditToComboboxMapping(m);

                    OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, Ptl.X, Ptl.Y, 0));
                    break;
                case WM.LBUTTONUP:
                    // Get the mouse location
                    var rect = new RECT();
                    GetWindowRect(this, ref rect);
                    Rectangle clientRect = rect;

                    int x = PARAM.SignedLOWORD(m.LParam);
                    int y = PARAM.SignedHIWORD(m.LParam);
                    Point pt = new Point(x, y);
                    pt = PointToScreen(pt);

                    // Combobox gets a WM_LBUTTONUP for focus change- check MouseEvents
                    if (_mouseEvents && !ValidationCancelled)
                    {
                        _mouseEvents = false;
                        if (_mousePressed)
                        {
                            if (clientRect.Contains(pt))
                            {
                                _mousePressed = false;
                                OnClick(new MouseEventArgs(MouseButtons.Left, 1, PARAM.SignedLOWORD(m.LParam), PARAM.SignedHIWORD(m.LParam), 0));
                                OnMouseClick(new MouseEventArgs(MouseButtons.Left, 1, PARAM.SignedLOWORD(m.LParam), PARAM.SignedHIWORD(m.LParam), 0));
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

                    // The message gets fired from Combo-box's WndPrc - convert to Combobox coordinates
                    pt = EditToComboboxMapping(m);

                    OnMouseUp(new MouseEventArgs(MouseButtons.Left, 1, pt.X, pt.Y, 0));
                    break;
                case WM.MBUTTONDOWN:
                    _mousePressed = true;
                    _mouseEvents = true;

                    // Set the mouse capture as this is the child Wndproc.
                    Capture = true;
                    DefChildWndProc(ref m);

                    // The message gets fired from Combo-box's WndPrc - convert to Combobox coordinates
                    Point P = EditToComboboxMapping(m);

                    OnMouseDown(new MouseEventArgs(MouseButtons.Middle, 1, P.X, P.Y, 0));
                    break;
                case WM.RBUTTONDOWN:
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
                case WM.MBUTTONUP:
                    _mousePressed = false;
                    _mouseEvents = false;

                    // Set the mouse capture as this is the child Wndproc.
                    Capture = false;
                    DefChildWndProc(ref m);
                    OnMouseUp(new MouseEventArgs(MouseButtons.Middle, 1, PARAM.SignedLOWORD(m.LParam), PARAM.SignedHIWORD(m.LParam), 0));
                    break;
                case WM.RBUTTONUP:
                    _mousePressed = false;
                    _mouseEvents = false;

                    DefChildWndProc(ref m);

                    // The message gets fired from Combo-box's WndPrc - convert to Combobox coordinates
                    Point ptRBtnUp = EditToComboboxMapping(m);

                    OnMouseUp(new MouseEventArgs(MouseButtons.Right, 1, ptRBtnUp.X, ptRBtnUp.Y, 0));
                    break;

                case WM.CONTEXTMENU:
                    // Forward context menu messages to the parent control
                    if (ContextMenuStrip is not null)
                    {
                        SendMessageW(this, WM.CONTEXTMENU, m.WParam, m.LParam);
                    }
                    else
                    {
                        DefChildWndProc(ref m);
                    }

                    break;

                case WM.MOUSEMOVE:
                    Point point = EditToComboboxMapping(m);

                    // Call the DefWndProc() so that mousemove messages get to the windows edit control
                    DefChildWndProc(ref m);
                    OnMouseEnterInternal(EventArgs.Empty);
                    OnMouseMove(new MouseEventArgs(MouseButtons, 0, point.X, point.Y, 0));
                    break;

                case WM.SETCURSOR:
                    if (Cursor != DefaultCursor && _childEdit is not null && m.HWnd == _childEdit.Handle && PARAM.LOWORD(m.LParam) == (int)HT.CLIENT)
                    {
                        Cursor.Current = Cursor;
                    }
                    else
                    {
                        DefChildWndProc(ref m);
                    }

                    break;

                case WM.MOUSELEAVE:
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

            if (_childEdit is not null && ChildEditAccessibleObject.Bounds.Contains(PointToScreen(e.Location)))
            {
                ChildEditAccessibleObject.RaiseAutomationEvent(UiaCore.UIA.Text_TextSelectionChangedEventId);
            }
        }

        /// <summary>
        ///  Helper to handle MouseLeave.
        /// </summary>
        private void OnMouseLeaveInternal(EventArgs args)
        {
            var rect = new RECT();
            GetWindowRect(this, ref rect);
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
                NativeWindow childWindow;
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

                //childwindow could be null if the handle was recreated while within a message handler
                // and then whoever recreated the handle allowed the message to continue to be processed
                //we cannot really be sure the new child will properly handle this window message, so we eat it.
                if (childWindow is not null)
                {
                    childWindow.DefWndProc(ref m);
                }
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
                if (_childEdit is not null && _childEdit.Handle != IntPtr.Zero)
                {
                    InvalidateRect(new HandleRef(this, _childEdit.Handle), null, BOOL.FALSE);
                }

                if (_childListBox is not null && _childListBox.Handle != IntPtr.Zero)
                {
                    InvalidateRect(new HandleRef(this, _childListBox.Handle), null, BOOL.FALSE);
                }
            }
        }

        /// <summary>
        ///  Finds the first item in the combo box that starts with the given string.
        ///  The search is not case sensitive.
        /// </summary>
        public int FindString(string s) => FindString(s, startIndex: -1);

        /// <summary>
        ///  Finds the first item after the given index which starts with the given string.
        ///  The search is not case sensitive.
        /// </summary>
        public int FindString(string s, int startIndex)
        {
            return FindStringInternal(s, _itemsCollection, startIndex, exact: false, ignoreCase: true);
        }

        /// <summary>
        ///  Finds the first item in the combo box that matches the given string.
        ///  The strings must match exactly, except for differences in casing.
        /// </summary>
        public int FindStringExact(string s)
        {
            return FindStringExact(s, startIndex: -1, ignoreCase: true);
        }

        /// <summary>
        ///  Finds the first item after the given index that matches the given string.
        ///  The strings must match exactly, except for differences in casing.
        /// </summary>
        public int FindStringExact(string s, int startIndex)
        {
            return FindStringExact(s, startIndex, ignoreCase: true);
        }

        /// <summary>
        ///  Finds the first item after the given index that matches the given string.
        ///  The strings must match exactly, except for differences in casing.
        /// </summary>
        internal int FindStringExact(string s, int startIndex, bool ignoreCase)
        {
            return FindStringInternal(s, _itemsCollection, startIndex, exact: true, ignoreCase);
        }

        // GetPreferredSize and SetBoundsCore call this method to allow controls to self impose
        // constraints on their size.
        internal override Rectangle ApplyBoundsConstraints(int suggestedX, int suggestedY, int proposedWidth, int proposedHeight)
        {
            if (DropDownStyle == ComboBoxStyle.DropDown
                || DropDownStyle == ComboBoxStyle.DropDownList)
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

            if (index < 0 || _itemsCollection is null || index >= _itemsCollection.Count)
            {
                throw new ArgumentOutOfRangeException(nameof(index), index, string.Format(SR.InvalidArgument, nameof(index), index));
            }

            if (IsHandleCreated)
            {
                int h = unchecked((int)(long)SendMessageW(this, (WM)CB.GETITEMHEIGHT, (IntPtr)index));
                if (h == -1)
                {
                    throw new Win32Exception();
                }

                return h;
            }

            return ItemHeight;
        }

        internal IntPtr GetListHandle()
        {
            return DropDownStyle == ComboBoxStyle.Simple ? _childListBox.Handle : _dropDownHandle;
        }

        internal NativeWindow GetListNativeWindow()
        {
            return DropDownStyle == ComboBoxStyle.Simple ? _childListBox : _childDropDown;
        }

        internal int GetListNativeWindowRuntimeIdPart()
        {
            NativeWindow listNativeWindow = GetListNativeWindow();
            return listNativeWindow is not null ? listNativeWindow.GetHashCode() : 0;
        }

        internal override Gdi32.HBRUSH InitializeDCForWmCtlColor(Gdi32.HDC dc, User32.WM msg)
        {
            if (msg == WM.CTLCOLORSTATIC && !ShouldSerializeBackColor())
            {
                // Let the Win32 Edit control handle background colors itself.
                // This is necessary because a disabled edit control will display a different
                // BackColor than when enabled.
                return default;
            }
            else if (msg == WM.CTLCOLORLISTBOX && GetStyle(ControlStyles.UserPaint))
            {
                // Base class returns hollow brush when UserPaint style is set, to avoid flicker in
                // main control. But when returning colors for child dropdown list, return normal ForeColor/BackColor,
                // since hollow brush leaves the list background unpainted.
                Gdi32.SetTextColor(dc, ColorTranslator.ToWin32(ForeColor));
                Gdi32.SetBkColor(dc, ColorTranslator.ToWin32(BackColor));
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
            if (m.Msg == (int)WM.KEYDOWN)
            {
                Debug.Assert((ModifierKeys & Keys.Alt) == 0);
                // Keys.Delete only triggers a WM_KEYDOWN and WM_KEYUP, and no WM_CHAR. That's why it's treated separately.
                if ((Keys)unchecked((int)(long)m.WParam) == Keys.Delete)
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
            else if (m.Msg == (int)WM.CHAR)
            {
                Debug.Assert((ModifierKeys & Keys.Alt) == 0);
                char keyChar = unchecked((char)(long)m.WParam);
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

        // Invalidate the entire control, including child HWNDs and non-client areas
        private unsafe void InvalidateEverything()
        {
            if (!IsHandleCreated)
            {
                return;
            }

            // Control.Invalidate(true) doesn't invalidate the non-client region
            RedrawWindow(
                new HandleRef(this, Handle),
                null,
                IntPtr.Zero,
                RDW.INVALIDATE | RDW.FRAME | RDW.ERASE | RDW.ALLCHILDREN);
        }

        /// <summary>
        ///  Determines if keyData is in input key that the control wants.
        ///  Overridden to return true for RETURN and ESCAPE when the combo box is
        ///  dropped down.
        /// </summary>
        protected override bool IsInputKey(Keys keyData)
        {
            Keys keyCode = keyData & (Keys.KeyCode | Keys.Alt);
            if (keyCode == Keys.Return || keyCode == Keys.Escape)
            {
                if (DroppedDown || _autoCompleteDroppedDown)
                {
                    //old behavior
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
        ///  Adds the given item to the native combo box.  This asserts if the handle hasn't been
        ///  created.
        /// </summary>
        private int NativeAdd(object item)
        {
            Debug.Assert(IsHandleCreated, "Shouldn't be calling Native methods before the handle is created.");
            int insertIndex = unchecked((int)(long)SendMessageW(this, (WM)CB.ADDSTRING, IntPtr.Zero, GetItemText(item)));
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
            string saved = null;
            if (DropDownStyle != ComboBoxStyle.DropDownList)
            {
                saved = WindowText;
            }

            SendMessageW(this, (WM)CB.RESETCONTENT);
            if (saved is not null)
            {
                WindowText = saved;
            }
        }

        /// <summary>
        ///  Get the text stored by the native control for the specified list item.
        /// </summary>
        private unsafe string NativeGetItemText(int index)
        {
            int maxLength = PARAM.ToInt(SendMessageW(this, (WM)CB.GETLBTEXTLEN, (IntPtr)index));
            if (maxLength == LB_ERR)
            {
                return string.Empty;
            }

            char[] text = ArrayPool<char>.Shared.Rent(maxLength + 1);
            string result;
            fixed (char* pText = text)
            {
                int actualLength = PARAM.ToInt(SendMessageW(this, (WM)CB.GETLBTEXT, (IntPtr)index, (IntPtr)pText));
                Debug.Assert(actualLength != LB_ERR, "Should have validated the index above");
                if (actualLength == LB_ERR)
                {
                    return string.Empty;
                }

                result = new string(pText, 0, Math.Min(maxLength, actualLength));
            }

            ArrayPool<char>.Shared.Return(text);
            return result;
        }

        /// <summary>
        ///  Inserts the given item to the native combo box at the index.  This asserts if the handle hasn't been
        ///  created or if the resulting insert index doesn't match the passed in index.
        /// </summary>
        private int NativeInsert(int index, object item)
        {
            Debug.Assert(IsHandleCreated, "Shouldn't be calling Native methods before the handle is created.");
            int insertIndex = unchecked((int)(long)SendMessageW(this, (WM)CB.INSERTSTRING, (IntPtr)index, GetItemText(item)));
            if (insertIndex < 0)
            {
                throw new OutOfMemoryException(SR.ComboBoxItemOverflow);
            }

            Debug.Assert(insertIndex == index, "NativeComboBox inserted at " + insertIndex + " not the requested index of " + index);
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
            //
            if (DropDownStyle == ComboBoxStyle.DropDownList && SelectedIndex == index)
            {
                Invalidate();
            }

            SendMessageW(this, (WM)CB.DELETESTRING, (IntPtr)index);
        }

        internal override void RecreateHandleCore()
        {
            string oldText = WindowText;
            base.RecreateHandleCore();
            if (!string.IsNullOrEmpty(oldText) && string.IsNullOrEmpty(WindowText))
            {
                WindowText = oldText;   //restore the window text
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
        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            if (MaxLength > 0)
            {
                SendMessageW(this, (WM)CB.LIMITTEXT, (IntPtr)MaxLength);
            }

            // Get the handles and wndprocs of the ComboBox's child windows
            //
            Debug.Assert(_childEdit is null, "Child edit window already attached");
            Debug.Assert(_childListBox is null, "Child listbox window already attached");

            bool ok = _childEdit is null && _childListBox is null;

            if (ok && DropDownStyle != ComboBoxStyle.DropDownList)
            {
                IntPtr hwnd = GetWindow(new HandleRef(this, Handle), GW.CHILD);
                if (hwnd != IntPtr.Zero)
                {
                    // if it's a simple dropdown list, the first HWND is the list box.
                    //
                    if (DropDownStyle == ComboBoxStyle.Simple)
                    {
                        _childListBox = new ComboBoxChildNativeWindow(this, ChildWindowType.ListBox);
                        _childListBox.AssignHandle(hwnd);

                        // get the edits hwnd...
                        //
                        hwnd = GetWindow(new HandleRef(this, hwnd), GW.HWNDNEXT);
                    }

                    _childEdit = new ComboBoxChildNativeWindow(this, ChildWindowType.Edit);
                    _childEdit.AssignHandle(hwnd);

                    // set the initial margin for combobox to be zero (this is also done whenever the font is changed).
                    SendMessageW(new HandleRef(this, _childEdit.Handle), (WM)EM.SETMARGINS, (IntPtr)(EC.LEFTMARGIN | EC.RIGHTMARGIN));
                }
            }

            int dropDownWidth = Properties.GetInteger(PropDropDownWidth, out bool found);
            if (found)
            {
                SendMessageW(this, (WM)CB.SETDROPPEDWIDTH, (IntPtr)dropDownWidth);
            }

            found = false;
            int itemHeight = Properties.GetInteger(PropItemHeight, out found);
            if (found)
            {
                // someone has set the item height - update it
                UpdateItemHeight();
            }

            // Resize a simple style combobox on handle creation
            // to respect the requested height.
            //
            if (DropDownStyle == ComboBoxStyle.Simple)
            {
                Height = _requestedHeight;
            }

            //If HandleCreated set the AutoComplete...
            //this function checks if the correct properties are set to enable AutoComplete feature on combobox.
            try
            {
                _fromHandleCreate = true;
                SetAutoComplete(false, false);
            }
            finally
            {
                _fromHandleCreate = false;
            }

            if (_itemsCollection is not null)
            {
                foreach (object item in _itemsCollection)
                {
                    NativeAdd(item);
                }

                // Now udpate the current selection.
                //
                if (_selectedIndex >= 0)
                {
                    SendMessageW(this, (WM)CB.SETCURSEL, (IntPtr)_selectedIndex);
                    UpdateText();
                    _selectedIndex = -1;
                }
            }

            // NOTE: Setting SelectedIndex must be the last thing we do.
        }

        /// <summary>
        ///  We need to un-subclasses everything here.  Inheriting classes should
        ///  not forget to call base.OnHandleDestroyed()
        /// </summary>
        protected override void OnHandleDestroyed(EventArgs e)
        {
            _dropDownHandle = IntPtr.Zero;
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
            ((DrawItemEventHandler)Events[EVENT_DRAWITEM])?.Invoke(this, e);
        }

        /// <summary>
        ///  This is the code that actually fires the dropDown event.  Don't
        ///  forget to call base.onDropDown() to ensure that dropDown events
        ///  are correctly fired at all other times.
        /// </summary>
        protected virtual void OnDropDown(EventArgs e)
        {
            ((EventHandler)Events[EVENT_DROPDOWN])?.Invoke(this, e);

            if (!IsHandleCreated)
            {
                return;
            }

            // Notify collapsed/expanded property change.
            AccessibilityObject.RaiseAutomationPropertyChangedEvent(
                UiaCore.UIA.ExpandCollapseExpandCollapseStatePropertyId,
                UiaCore.ExpandCollapseState.Collapsed,
                UiaCore.ExpandCollapseState.Expanded);

            if (AccessibilityObject is ComboBoxAccessibleObject accessibleObject)
            {
                accessibleObject.SetComboBoxItemFocus();
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

            //return when dropped down already fires commit.
            if (!e.Handled && (e.KeyChar == (char)(int)Keys.Return || e.KeyChar == (char)(int)Keys.Escape)
                && DroppedDown)
            {
                _dropDown = false;
                if (FormattingEnabled)
                {
                    //Set the Text which would Compare the WindowText with the TEXT and change SelectedIndex.
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

            if (_childEdit is not null && ContainsNavigationKeyCode(e.KeyCode))
            {
                ChildEditAccessibleObject.RaiseAutomationEvent(UiaCore.UIA.Text_TextSelectionChangedEventId);
            }
        }

        private bool ContainsNavigationKeyCode(Keys keyCode)
        {
            switch (keyCode)
            {
                case Keys.Home:
                case Keys.End:
                case Keys.Left:
                case Keys.Right:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        ///  This is the code that actually fires the measuereItem event.  Don't
        ///  forget to call base.onMeasureItem() to ensure that measureItem
        ///  events are correctly fired at all other times.
        /// </summary>
        protected virtual void OnMeasureItem(MeasureItemEventArgs e)
        {
            ((MeasureItemEventHandler)Events[EVENT_MEASUREITEM])?.Invoke(this, e);
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
            ((EventHandler)Events[EVENT_SELECTIONCHANGECOMMITTED])?.Invoke(this, e);

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
            ((EventHandler)Events[EVENT_SELECTEDINDEXCHANGED])?.Invoke(this, e);

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

            if (AccessibilityObject is ComboBoxAccessibleObject accessibleObject &&
                (DropDownStyle == ComboBoxStyle.DropDownList || DropDownStyle == ComboBoxStyle.DropDown))
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
                ChildEditAccessibleObject.RaiseAutomationEvent(UiaCore.UIA.Text_TextSelectionChangedEventId);
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
                //read this as "if everett or   (whidbey and selindex is valid)"
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
            ((EventHandler)Events[EVENT_SELECTEDITEMCHANGED])?.Invoke(this, e);
        }

        /// <summary>
        ///  This is the code that actually fires the DropDownStyleChanged event.
        /// </summary>
        protected virtual void OnDropDownStyleChanged(EventArgs e)
        {
            ((EventHandler)Events[EVENT_DROPDOWNSTYLE])?.Invoke(this, e);
        }

        /// <summary>
        ///  This method is called by the parent control when any property
        ///  changes on the parent. This can be overriden by inheriting
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
                //we always will recreate the handle when autocomplete mode is on
                RecreateHandle();
            }

            CommonProperties.xClearPreferredSizeCache(this);
        }

        private void OnAutoCompleteCustomSourceChanged(object sender, CollectionChangeEventArgs e)
        {
            if (AutoCompleteSource == AutoCompleteSource.CustomSource)
            {
                if (AutoCompleteCustomSource.Count == 0)
                {
                    SetAutoComplete(true, true /*recreate handle*/);
                }
                else
                {
                    SetAutoComplete(true, false);
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
                if (AutoCompleteMode != AutoCompleteMode.None &&
                    AutoCompleteSource == AutoCompleteSource.ListItems &&
                    DropDownStyle == ComboBoxStyle.DropDownList)
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

            if (_childEdit is not null)
            {
                ChildEditAccessibleObject.RaiseAutomationEvent(UiaCore.UIA.Text_TextChangedEventId);
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
            //clear the pref height cache
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
        ///  Raises the <see cref='Control.Resize'/> event.
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

            //

            RefreshItems();
        }

        /// <summary>
        ///  This event is fired when the dropdown portion of the combobox is hidden.
        /// </summary>
        protected virtual void OnDropDownClosed(EventArgs e)
        {
            ((EventHandler)Events[EVENT_DROPDOWNCLOSED])?.Invoke(this, e);

            if (!IsHandleCreated)
            {
                return;
            }

            // Need to announce the focus on combo-box with new selected value on drop-down close.
            // If do not do this focus in Level 3 stays on list item of unvisible list.
            // This is necessary for DropDown style as edit should not take focus.
            if (DropDownStyle == ComboBoxStyle.DropDown)
            {
                AccessibilityObject.RaiseAutomationEvent(UiaCore.UIA.AutomationFocusChangedEventId);
            }

            // Notify Collapsed/expanded property change.
            AccessibilityObject.RaiseAutomationPropertyChangedEvent(
                UiaCore.UIA.ExpandCollapseExpandCollapseStatePropertyId,
                UiaCore.ExpandCollapseState.Expanded,
                UiaCore.ExpandCollapseState.Collapsed);

            // Collapsing the DropDown, so reset the flag.
            _dropDownWillBeClosed = false;
        }

        /// <summary>
        ///  This event is fired when the edit portion of a combobox is about to display altered text.
        ///  This event is NOT fired if the TEXT property is programatically changed.
        /// </summary>
        protected virtual void OnTextUpdate(EventArgs e)
        {
            ((EventHandler)Events[EVENT_TEXTUPDATE])?.Invoke(this, e);
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            bool returnedValue = base.ProcessCmdKey(ref msg, keyData);

            if (DropDownStyle != ComboBoxStyle.DropDownList &&
                (keyData == (Keys.Control | Keys.Back) || keyData == (Keys.Control | Keys.Shift | Keys.Back)))
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
            if (AutoCompleteMode != AutoCompleteMode.None &&
                AutoCompleteSource == AutoCompleteSource.ListItems &&
                DropDownStyle == ComboBoxStyle.DropDownList &&
                InterceptAutoCompleteKeystroke(m))
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
            //
            int selectedIndex = SelectedIndex;
            ObjectCollection savedItems = _itemsCollection;

            _itemsCollection = null;

            object[] newItems = null;

            // if we have a dataSource and a DisplayMember, then use it
            // to populate the Items collection
            //
            if (DataManager is not null && DataManager.Count != -1)
            {
                newItems = new object[DataManager.Count];
                for (int i = 0; i < newItems.Length; i++)
                {
                    newItems[i] = DataManager[i];
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
                //
                if (IsHandleCreated)
                {
                    NativeClear();
                }

                // Store the current list of items
                //
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
            Items.SetItemInternal(index, Items[index]);
        }

        /// <summary>
        ///  Release the ChildWindow object by un-subclassing the child edit and
        ///  list controls and freeing the root of the ChildWindow object.
        /// </summary>
        private void ReleaseChildWindow()
        {
            if (_childEdit is not null)
            {
                // We do not use UI Automation provider for child edit, so do not need to release providers.
                _childEdit.ReleaseHandle();
                _childEdit = null;
            }

            if (_childListBox is not null)
            {
                // Need to notify UI Automation that it can safely remove all map entries that refer to the specified window.
                ReleaseUiaProvider(_childListBox.Handle);

                _childListBox.ReleaseHandle();
                _childListBox = null;
            }

            if (_childDropDown is not null)
            {
                // Need to notify UI Automation that it can safely remove all map entries that refer to the specified window.
                ReleaseUiaProvider(_childDropDown.Handle);

                _childDropDown.ReleaseHandle();
                _childDropDown = null;
            }
        }

        internal override void ReleaseUiaProvider(IntPtr handle)
        {
            base.ReleaseUiaProvider(handle);

            var uiaProvider = AccessibilityObject as ComboBoxAccessibleObject;
            uiaProvider?.ResetListItemAccessibleObjects();
        }

        private void ResetAutoCompleteCustomSource()
        {
            AutoCompleteCustomSource = null;
        }

        private void ResetDropDownWidth()
        {
            Properties.RemoveInteger(PropDropDownWidth);
        }

        private void ResetItemHeight()
        {
            Properties.RemoveInteger(PropItemHeight);
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

            if (AutoCompleteMode != AutoCompleteMode.None)
            {
                if (!_fromHandleCreate && recreate && IsHandleCreated)
                {
                    //RecreateHandle to avoid Leak.
                    // notice the use of member variable to avoid re-entrancy
                    AutoCompleteMode backUpMode = AutoCompleteMode;
                    _autoCompleteMode = AutoCompleteMode.None;
                    RecreateHandle();
                    _autoCompleteMode = backUpMode;
                }

                if (AutoCompleteSource == AutoCompleteSource.CustomSource)
                {
                    if (AutoCompleteCustomSource is not null)
                    {
                        if (AutoCompleteCustomSource.Count == 0)
                        {
                            Shlwapi.SHAutoComplete(new HandleRef(this, _childEdit.Handle), Shlwapi.SHACF.AUTOSUGGEST_FORCE_OFF | Shlwapi.SHACF.AUTOAPPEND_FORCE_OFF);
                        }
                        else
                        {
                            if (_stringSource is null)
                            {
                                _stringSource = new StringSource(GetStringsForAutoComplete(AutoCompleteCustomSource));
                                if (!_stringSource.Bind(new HandleRef(this, _childEdit.Handle), (Shell32.AUTOCOMPLETEOPTIONS)AutoCompleteMode))
                                {
                                    throw new ArgumentException(SR.AutoCompleteFailure);
                                }
                            }
                            else
                            {
                                _stringSource.RefreshList(GetStringsForAutoComplete(AutoCompleteCustomSource));
                            }
                        }
                    }
                }
                else if (AutoCompleteSource == AutoCompleteSource.ListItems)
                {
                    if (DropDownStyle != ComboBoxStyle.DropDownList)
                    {
                        if (_itemsCollection is not null)
                        {
                            if (_itemsCollection.Count == 0)
                            {
                                Shlwapi.SHAutoComplete(new HandleRef(this, _childEdit.Handle), Shlwapi.SHACF.AUTOSUGGEST_FORCE_OFF | Shlwapi.SHACF.AUTOAPPEND_FORCE_OFF);
                            }
                            else
                            {
                                if (_stringSource is null)
                                {
                                    _stringSource = new StringSource(GetStringsForAutoComplete(Items));
                                    if (!_stringSource.Bind(new HandleRef(this, _childEdit.Handle), (Shell32.AUTOCOMPLETEOPTIONS)AutoCompleteMode))
                                    {
                                        throw new ArgumentException(SR.AutoCompleteFailureListItems);
                                    }
                                }
                                else
                                {
                                    _stringSource.RefreshList(GetStringsForAutoComplete(Items));
                                }
                            }
                        }
                    }
                    else
                    {
                        // Drop Down List special handling
                        Debug.Assert(DropDownStyle == ComboBoxStyle.DropDownList);
                        Shlwapi.SHAutoComplete(new HandleRef(this, _childEdit.Handle), Shlwapi.SHACF.AUTOSUGGEST_FORCE_OFF | Shlwapi.SHACF.AUTOAPPEND_FORCE_OFF);
                    }
                }
                else
                {
                    Shlwapi.SHACF mode = Shlwapi.SHACF.DEFAULT;
                    if (AutoCompleteMode == AutoCompleteMode.Suggest)
                    {
                        mode |= Shlwapi.SHACF.AUTOSUGGEST_FORCE_ON | Shlwapi.SHACF.AUTOAPPEND_FORCE_OFF;
                    }

                    if (AutoCompleteMode == AutoCompleteMode.Append)
                    {
                        mode |= Shlwapi.SHACF.AUTOAPPEND_FORCE_ON | Shlwapi.SHACF.AUTOSUGGEST_FORCE_OFF;
                    }

                    if (AutoCompleteMode == AutoCompleteMode.SuggestAppend)
                    {
                        mode |= Shlwapi.SHACF.AUTOSUGGEST_FORCE_ON;
                        mode |= Shlwapi.SHACF.AUTOAPPEND_FORCE_ON;
                    }

                    Shlwapi.SHAutoComplete(new HandleRef(this, _childEdit.Handle), (Shlwapi.SHACF)AutoCompleteSource | mode);
                }
            }
            else if (reset)
            {
                Shlwapi.SHAutoComplete(new HandleRef(this, _childEdit.Handle), Shlwapi.SHACF.AUTOSUGGEST_FORCE_OFF | Shlwapi.SHACF.AUTOAPPEND_FORCE_OFF);
            }
        }

        /// <summary>
        ///  Selects the text in the editable portion of the ComboBox at the
        ///  from the given start index to the given end index.
        /// </summary>
        public void Select(int start, int length)
        {
            if (start < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(start), start, string.Format(SR.InvalidArgument, nameof(start), start));
            }

            // the Length can be negative to support Selecting in the "reverse" direction..
            int end = start + length;

            // but end cannot be negative... this means Length is far negative...
            if (end < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length), length, string.Format(SR.InvalidArgument, nameof(length), length));
            }

            SendMessageW(this, (WM)CB.SETEDITSEL, IntPtr.Zero, PARAM.FromLowHigh(start, end));
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
                    SendMessageW(this, (WM)CB.SETCURSEL, (IntPtr)DataManager.Position);
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
            return (Properties.ContainsInteger(PropDropDownWidth));
        }

        /// <summary>
        ///  Indicates whether the itemHeight property should be persisted.
        /// </summary>
        internal bool ShouldSerializeItemHeight()
        {
            return (Properties.ContainsInteger(PropItemHeight));
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
            return s + ", Items.Count: " + ((_itemsCollection is null) ? "0" : _itemsCollection.Count.ToString(CultureInfo.CurrentCulture));
        }

        private void UpdateDropDownHeight()
        {
            if (_dropDownHandle != IntPtr.Zero)
            {
                //Now use the DropDownHeight property instead of calculating the Height...
                int height = DropDownHeight;
                if (height == DefaultDropDownHeight)
                {
                    int itemCount = (_itemsCollection is null) ? 0 : _itemsCollection.Count;
                    int count = Math.Min(Math.Max(itemCount, 1), _maxDropDownItems);
                    height = (ItemHeight * count + 2);
                }

                SetWindowPos(
                    new HandleRef(this, _dropDownHandle),
                    HWND_TOP,
                    0,
                    0,
                    DropDownWidth,
                    height,
                    SWP.NOMOVE | SWP.NOZORDER);
            }
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
                // if we don't create control here we report item heights incorrectly later on.
                CreateControl();
            }

            if (DrawMode == DrawMode.OwnerDrawFixed)
            {
                SendMessageW(this, (WM)CB.SETITEMHEIGHT, (IntPtr)(-1), (IntPtr)ItemHeight);
                SendMessageW(this, (WM)CB.SETITEMHEIGHT, IntPtr.Zero, (IntPtr)ItemHeight);
            }
            else if (DrawMode == DrawMode.OwnerDrawVariable)
            {
                SendMessageW(this, (WM)CB.SETITEMHEIGHT, (IntPtr)(-1), (IntPtr)ItemHeight);
                Graphics graphics = CreateGraphicsInternal();
                for (int i = 0; i < Items.Count; i++)
                {
                    int original = unchecked((int)(long)SendMessageW(this, (WM)CB.GETITEMHEIGHT, (IntPtr)i));
                    MeasureItemEventArgs mievent = new MeasureItemEventArgs(graphics, i, original);
                    OnMeasureItem(mievent);
                    if (mievent.ItemHeight != original)
                    {
                        SendMessageW(this, (WM)CB.SETITEMHEIGHT, (IntPtr)i, (IntPtr)mievent.ItemHeight);
                    }
                }

                graphics.Dispose();
            }
        }

        /// <summary>
        ///  Forces the text to be updated based on the current selection.
        /// </summary>
        private void UpdateText()
        {
            //           Fire text changed for dropdown combos when the selection
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
            string s = null;

            if (SelectedIndex != -1)
            {
                object item = Items[SelectedIndex];
                if (item is not null)
                {
                    s = GetItemText(item);
                }
            }

            Text = s;

            if (DropDownStyle == ComboBoxStyle.DropDown)
            {
                if (_childEdit is not null && _childEdit.Handle != IntPtr.Zero)
                {
                    SendMessageW(new HandleRef(this, _childEdit.Handle), WM.SETTEXT, IntPtr.Zero, s);
                }
            }
        }

        private void WmEraseBkgnd(ref Message m)
        {
            if ((DropDownStyle == ComboBoxStyle.Simple) && ParentInternal is not null)
            {
                RECT rect = default;
                GetClientRect(this, ref rect);
                Gdi32.HDC hdc = (Gdi32.HDC)m.WParam;
                using var hbrush = new Gdi32.CreateBrushScope(ParentInternal?.BackColor ?? SystemColors.Control);
                hdc.FillRectangle(rect, hbrush);
                m.Result = (IntPtr)1;
                return;
            }

            base.WndProc(ref m);
        }

        private void WmParentNotify(ref Message m)
        {
            base.WndProc(ref m);
            if (unchecked((int)(long)m.WParam) == ((int)WM.CREATE | 1000 << 16))
            {
                _dropDownHandle = m.LParam;

                // By some reason WmParentNotify with WM_DESTROY is not called before recreation.
                // So release the old references here.
                if (_childDropDown is not null)
                {
                    // Need to notify UI Automation that it can safely remove all map entries that refer to the specified window.
                    ReleaseUiaProvider(_childDropDown.Handle);

                    _childDropDown.ReleaseHandle();
                }

                _childDropDown = new ComboBoxChildNativeWindow(this, ChildWindowType.DropDownList);
                _childDropDown.AssignHandle(_dropDownHandle);

                // Reset the child list accessible object in case the the DDL is recreated.
                // For instance when dialog window containging the ComboBox is reopened.
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
            switch ((CBN)PARAM.HIWORD(m.WParam))
            {
                case CBN.DBLCLK:
                    break;
                case CBN.EDITUPDATE:
                    OnTextUpdate(EventArgs.Empty);
                    break;
                case CBN.CLOSEUP:

                    OnDropDownClosed(EventArgs.Empty);
                    if (FormattingEnabled && Text != _currentText && _dropDown)
                    {
                        OnTextChanged(EventArgs.Empty);
                    }

                    _dropDown = false;
                    break;
                case CBN.DROPDOWN:
                    _currentText = Text;
                    _dropDown = true;
                    OnDropDown(EventArgs.Empty);
                    UpdateDropDownHeight();

                    break;
                case CBN.EDITCHANGE:
                    OnTextChanged(EventArgs.Empty);
                    break;
                case CBN.SELCHANGE:
                    UpdateText();
                    OnSelectedIndexChanged(EventArgs.Empty);
                    break;
                case CBN.SELENDOK:
                    OnSelectionChangeCommittedInternal(EventArgs.Empty);
                    break;
            }
        }

        private unsafe void WmReflectDrawItem(ref Message m)
        {
            DRAWITEMSTRUCT* dis = (DRAWITEMSTRUCT*)m.LParam;

            using var e = new DrawItemEventArgs(
                dis->hDC,
                Font,
                dis->rcItem,
                dis->itemID,
                dis->itemState,
                ForeColor,
                BackColor);

            OnDrawItem(e);

            m.Result = (IntPtr)1;
        }

        private unsafe void WmReflectMeasureItem(ref Message m)
        {
            MEASUREITEMSTRUCT* mis = (MEASUREITEMSTRUCT*)m.LParam;

            // Determine if message was sent by a combo item or the combo edit field
            int itemID = (int)mis->itemID;
            if (DrawMode == DrawMode.OwnerDrawVariable && itemID >= 0)
            {
                using Graphics graphics = CreateGraphicsInternal();
                var mie = new MeasureItemEventArgs(graphics, itemID, ItemHeight);
                OnMeasureItem(mie);
                mis->itemHeight = unchecked((uint)mie.ItemHeight);
            }
            else
            {
                // Message was sent by the combo edit field
                mis->itemHeight = (uint)ItemHeight;
            }

            m.Result = (IntPtr)1;
        }

        /// <summary>
        ///  The comboboxs window procedure.  Inheritng classes can override this
        ///  to add extra functionality, but should not forget to call
        ///  base.wndProc(m); to ensure the combo continues to function properly.
        /// </summary>
        protected override void WndProc(ref Message m)
        {
            switch ((WM)m.Msg)
            {
                // We don't want to fire the focus events twice -
                // once in the combobox and once in the ChildWndProc.
                case WM.SETFOCUS:
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
                case WM.KILLFOCUS:
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

                        if (!Application.RenderWithVisualStyles && GetStyle(ControlStyles.UserPaint) == false && DropDownStyle == ComboBoxStyle.DropDownList && (FlatStyle == FlatStyle.Flat || FlatStyle == FlatStyle.Popup))
                        {
                            PostMessageW(this, WM.MOUSELEAVE);
                        }
                    }

                    finally
                    {
                        _fireLostFocus = true;
                    }

                    break;
                case WM.CTLCOLOREDIT:
                case WM.CTLCOLORLISTBOX:
                    m.Result = (IntPtr)InitializeDCForWmCtlColor((Gdi32.HDC)m.WParam, (User32.WM)m.Msg);
                    break;
                case WM.ERASEBKGND:
                    WmEraseBkgnd(ref m);
                    break;
                case WM.PARENTNOTIFY:
                    WmParentNotify(ref m);
                    break;
                case WM.REFLECT_COMMAND:
                    WmReflectCommand(ref m);
                    break;
                case WM.REFLECT_DRAWITEM:
                    WmReflectDrawItem(ref m);
                    break;
                case WM.REFLECT_MEASUREITEM:
                    WmReflectMeasureItem(ref m);
                    break;
                case WM.LBUTTONDOWN:
                    _mouseEvents = true;
                    base.WndProc(ref m);
                    break;
                case WM.LBUTTONUP:
                    // Get the mouse location
                    var r = new RECT();
                    GetWindowRect(this, ref r);
                    Rectangle clientRect = r;

                    int x = PARAM.SignedLOWORD(m.LParam);
                    int y = PARAM.SignedHIWORD(m.LParam);
                    Point pt = new Point(x, y);
                    pt = PointToScreen(pt);
                    //mouseEvents is used to keep the check that we get the WM_LBUTTONUP after
                    //WM_LBUTTONDOWN or WM_LBUTTONDBLBCLK
                    // combo box gets a WM_LBUTTONUP for focus change ...
                    //
                    if (_mouseEvents && !ValidationCancelled)
                    {
                        _mouseEvents = false;
                        bool captured = Capture;
                        if (captured && clientRect.Contains(pt))
                        {
                            OnClick(new MouseEventArgs(MouseButtons.Left, 1, PARAM.SignedLOWORD(m.LParam), PARAM.SignedHIWORD(m.LParam), 0));
                            OnMouseClick(new MouseEventArgs(MouseButtons.Left, 1, PARAM.SignedLOWORD(m.LParam), PARAM.SignedHIWORD(m.LParam), 0));
                        }

                        base.WndProc(ref m);
                    }
                    else
                    {
                        Capture = false;
                        DefWndProc(ref m);
                    }

                    break;

                case WM.MOUSELEAVE:
                    DefWndProc(ref m);
                    OnMouseLeaveInternal(EventArgs.Empty);
                    break;

                case WM.PAINT:
                    if (GetStyle(ControlStyles.UserPaint) == false && (FlatStyle == FlatStyle.Flat || FlatStyle == FlatStyle.Popup))
                    {
                        using var dropDownRegion = new Gdi32.RegionScope(FlatComboBoxAdapter._dropDownRect);
                        using var windowRegion = new Gdi32.RegionScope(Bounds);

                        // Stash off the region we have to update (the base is going to clear this off in BeginPaint)
                        bool getRegionSucceeded = GetUpdateRgn(Handle, windowRegion, bErase: BOOL.TRUE) != RegionType.ERROR;

                        Gdi32.CombineRgn(dropDownRegion, windowRegion, dropDownRegion, Gdi32.RGN.DIFF);
                        RECT updateRegionBoundingRect = default;
                        Gdi32.GetRgnBox(windowRegion, ref updateRegionBoundingRect);

                        FlatComboBoxAdapter.ValidateOwnerDrawRegions(this, updateRegionBoundingRect);

                        // Call the base class to do its painting (with a clipped DC).
                        bool useBeginPaint = m.WParam == IntPtr.Zero;
                        using var paintScope = useBeginPaint ? new BeginPaintScope(Handle) : default;

                        Gdi32.HDC dc = useBeginPaint ? paintScope : (Gdi32.HDC)m.WParam;

                        using var savedDcState = new Gdi32.SaveDcScope(dc);

                        if (getRegionSucceeded)
                        {
                            Gdi32.SelectClipRgn(dc, dropDownRegion);
                        }

                        m.WParam = (IntPtr)dc;
                        DefWndProc(ref m);

                        if (getRegionSucceeded)
                        {
                            Gdi32.SelectClipRgn(dc, windowRegion);
                        }

                        using Graphics g = Graphics.FromHdcInternal((IntPtr)dc);
                        FlatComboBoxAdapter.DrawFlatCombo(this, g);

                        return;
                    }

                    base.WndProc(ref m);
                    break;

                case WM.PRINTCLIENT:
                    // all the fancy stuff we do in OnPaint has to happen again in OnPrint.
                    if (!GetStyle(ControlStyles.UserPaint) && (FlatStyle == FlatStyle.Flat || FlatStyle == FlatStyle.Popup))
                    {
                        DefWndProc(ref m);

                        if ((unchecked((PRF)(long)m.LParam) & PRF.CLIENT) == PRF.CLIENT)
                        {
                            if (!GetStyle(ControlStyles.UserPaint) && (FlatStyle == FlatStyle.Flat || FlatStyle == FlatStyle.Popup))
                            {
                                using (Graphics g = Graphics.FromHdcInternal(m.WParam))
                                {
                                    FlatComboBoxAdapter.DrawFlatCombo(this, g);
                                }
                            }

                            return;
                        }
                    }

                    base.WndProc(ref m);
                    return;

                case WM.SETCURSOR:
                    base.WndProc(ref m);
                    break;

                case WM.SETFONT:
                    if (Width == 0)
                    {
                        _suppressNextWindosPos = true;
                    }

                    base.WndProc(ref m);
                    break;

                case WM.WINDOWPOSCHANGED:
                    if (!_suppressNextWindosPos)
                    {
                        base.WndProc(ref m);
                    }

                    _suppressNextWindosPos = false;
                    break;

                case WM.NCDESTROY:
                    base.WndProc(ref m);
                    ReleaseChildWindow();
                    break;

                default:
                    if (m.Msg == (int)User32.RegisteredMessage.WM_MOUSEENTER)
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
                if (!(Properties.GetObject(PropFlatComboAdapter) is FlatComboAdapter comboAdapter) || !comboAdapter.IsValid(this))
                {
                    comboAdapter = CreateFlatComboAdapterInstance();
                    Properties.SetObject(PropFlatComboAdapter, comboAdapter);
                }

                return comboAdapter;
            }
        }

        internal virtual FlatComboAdapter CreateFlatComboAdapterInstance()
            => new FlatComboAdapter(this, smallButton: false);

        /// <summary>
        ///  Represents the ComboBox child native window type.
        /// </summary>
        private enum ChildWindowType
        {
            ListBox,
            Edit,
            DropDownList
        }
    }
}
