// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {
    using Accessibility;
    using System.Runtime.Serialization.Formatters;
    using System.Threading;
    using System.Runtime.Remoting;
    using System.Runtime.InteropServices;
    using System.Diagnostics;
    using System;
    using System.Runtime.Versioning;
    using System.Globalization;
    using System.Security;
    using System.Security.Permissions;
    using System.Windows.Forms.Layout;
    using System.Windows.Forms.ComponentModel;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Collections;
    using System.Drawing;
    using System.Windows.Forms.Internal;
    using System.Drawing.Design;
    using Microsoft.Win32;
    using System.Reflection;
    using System.Collections.Specialized;
    using System.Text;
    using System.Diagnostics.CodeAnalysis;

    /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Displays an editing field and a list, allowing the user to select from the
    ///       list or to enter new text. Displays only the editing field until the user
    ///       explicitly displays the list.
    ///
    ///    </para>
    /// </devdoc>
    [
    ComVisible(true),
    ClassInterface(ClassInterfaceType.AutoDispatch),
    DefaultEvent(nameof(SelectedIndexChanged)),
    DefaultProperty(nameof(Items)),
    DefaultBindingProperty(nameof(Text)),
    Designer("System.Windows.Forms.Design.ComboBoxDesigner, " + AssemblyRef.SystemDesign),
    SRDescription(nameof(SR.DescriptionComboBox))
    ]
    public class ComboBox : ListControl {

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
        private bool autoCompleteDroppedDown = false;

        private FlatStyle flatStyle = FlatStyle.Standard;
        private int updateCount;

        //Timestamp of the last keystroke. Used for auto-completion
        // in DropDownList style.
        private long autoCompleteTimeStamp;

        private int selectedIndex = -1;  // used when we don't have a handle.
        private bool allowCommit = true;

        // When the style is "simple", the requested height is used
        // for the actual height of the control.
        // When the style is non-simple, the height of the control
        // is determined by the OS.
        private int requestedHeight;

        private ComboBoxChildNativeWindow childEdit;
        private ComboBoxChildNativeWindow childListBox;

        private IntPtr dropDownHandle;
        private ObjectCollection itemsCollection;
        private short prefHeightCache = -1;
        private short maxDropDownItems = 8;
        private bool integralHeight = true;
        private bool mousePressed;
        private bool mouseEvents;
        private bool mouseInEdit;

        private bool sorted;
        private bool fireSetFocus = true;
        private bool fireLostFocus = true;
        private bool mouseOver;
        private bool suppressNextWindosPos;
        private bool canFireLostFocus;

        // When the user types a letter and drops the dropdown...
        // the combobox itself auto-searches the matching item...
        // and selects the item in the edit...
        // thus changing the windowText...
        // hence we should Fire the TextChanged event in such a scenario..
        // The string below is used for checking the window Text before and after the dropdown.
        private string currentText = "";
        private string lastTextChangedValue;
        private bool dropDown;
        private AutoCompleteDropDownFinder finder = new AutoCompleteDropDownFinder();

        private bool selectedValueChangedFired;

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.autoCompleteMode"]/*' />
        /// <devdoc>
        ///     This stores the value for the autocomplete mode which can be either
        ///     None, AutoSuggest, AutoAppend or AutoSuggestAppend.
        /// </devdoc>
        private AutoCompleteMode autoCompleteMode = AutoCompleteMode.None;

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.autoCompleteSource"]/*' />
        /// <devdoc>
        ///     This stores the value for the autoCompleteSource mode which can be one of the values
        ///     from AutoCompleteSource enum.
        /// </devdoc>
        private AutoCompleteSource autoCompleteSource = AutoCompleteSource.None;

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.autoCompleteCustomSource"]/*' />
        /// <devdoc>
        ///     This stores the custom StringCollection required for the autoCompleteSource when its set to CustomSource.
        /// </devdoc>
        private AutoCompleteStringCollection autoCompleteCustomSource;
        private StringSource stringSource;
        private bool fromHandleCreate = false;

        private ComboBoxChildEditUiaProvider childEditAccessibleObject;
        private ComboBoxChildTextUiaProvider childTextAccessibleObject;

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.ComboBox"]/*' />
        /// <devdoc>
        ///     Creates a new ComboBox control.  The default style for the combo is
        ///     a regular DropDown Combo.
        /// </devdoc>
        public ComboBox() {
            SetStyle(ControlStyles.UserPaint |
                     ControlStyles.UseTextForAccessibility |
                     ControlStyles.StandardClick, false);

            requestedHeight = DefaultSimpleStyleHeight;
            
            // this class overrides GetPreferredSizeCore, let Control automatically cache the result
            SetState2(STATE2_USEPREFERREDSIZECACHE, true);  
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.AutoCompleteMode"]/*' />
        /// <devdoc>
        ///     This is the AutoCompleteMode which can be either
        ///     None, AutoSuggest, AutoAppend or AutoSuggestAppend.
        ///     This property in conjunction with AutoCompleteSource enables the AutoComplete feature for ComboBox.
        /// </devdoc>
        [
        DefaultValue(AutoCompleteMode.None),
        SRDescription(nameof(SR.ComboBoxAutoCompleteModeDescr)),
        Browsable(true), EditorBrowsable(EditorBrowsableState.Always)
        ]
        public AutoCompleteMode AutoCompleteMode {
            get {
                return autoCompleteMode;
            }
            set {
                //valid values are 0x0 to 0x3
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)AutoCompleteMode.None, (int)AutoCompleteMode.SuggestAppend)) {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(AutoCompleteMode));
                }
                if (this.DropDownStyle == ComboBoxStyle.DropDownList &&
                    this.AutoCompleteSource != AutoCompleteSource.ListItems &&
                    value != AutoCompleteMode.None) {
                    throw new NotSupportedException(SR.ComboBoxAutoCompleteModeOnlyNoneAllowed);
                }
                if (Application.OleRequired() != System.Threading.ApartmentState.STA) {
                    throw new ThreadStateException(SR.ThreadMustBeSTA);
                }
                bool resetAutoComplete = false;
                if (autoCompleteMode != AutoCompleteMode.None && value == AutoCompleteMode.None) {
                    resetAutoComplete = true;
                }
                autoCompleteMode = value;
                SetAutoComplete(resetAutoComplete, true);
            }
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.AutoCompleteSource"]/*' />
        /// <devdoc>
        ///     This is the AutoCompleteSource which can be one of the
        ///     values from AutoCompleteSource enumeration.
        ///     This property in conjunction with AutoCompleteMode enables the AutoComplete feature for ComboBox.
        /// </devdoc>
        [
        DefaultValue(AutoCompleteSource.None),
        SRDescription(nameof(SR.ComboBoxAutoCompleteSourceDescr)),
        Browsable(true), EditorBrowsable(EditorBrowsableState.Always)
        ]
        public AutoCompleteSource AutoCompleteSource {
            get {
                return autoCompleteSource;
            }
            set {
                // FxCop: Avoid usage of Enum.IsDefined - this looks like an enum that could grow
                if (!ClientUtils.IsEnumValid_NotSequential(value, (int)value,
                                                    (int)AutoCompleteSource.None,
                                                    (int)AutoCompleteSource.AllSystemSources,
                                                    (int)AutoCompleteSource.AllUrl,
                                                    (int)AutoCompleteSource.CustomSource,
                                                    (int)AutoCompleteSource.FileSystem,
                                                    (int)AutoCompleteSource.FileSystemDirectories,
                                                    (int)AutoCompleteSource.HistoryList,
                                                    (int)AutoCompleteSource.ListItems,
                                                    (int)AutoCompleteSource.RecentlyUsedList))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(AutoCompleteSource));
                }

                if (this.DropDownStyle == ComboBoxStyle.DropDownList &&
                    this.AutoCompleteMode != AutoCompleteMode.None &&
                    value != AutoCompleteSource.ListItems) {
                    throw new NotSupportedException(SR.ComboBoxAutoCompleteSourceOnlyListItemsAllowed);
                }
                if (Application.OleRequired() != System.Threading.ApartmentState.STA) {
                    throw new ThreadStateException(SR.ThreadMustBeSTA);
                }

                if (value != AutoCompleteSource.None && 
                    value != AutoCompleteSource.CustomSource && 
                    value != AutoCompleteSource.ListItems)
                {
                    FileIOPermission fiop = new FileIOPermission(PermissionState.Unrestricted);
                    fiop.AllFiles = FileIOPermissionAccess.PathDiscovery;
                    fiop.Demand();
                }

                autoCompleteSource = value;
                SetAutoComplete(false, true);
            }
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.AutoCompleteCustomSource"]/*' />
        /// <devdoc>
        ///     This is the AutoCompleteCustomSource which is custom StringCollection used when the
        ///     AutoCompleteSource is CustomSource.
        /// </devdoc>
        [
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        Localizable(true),
        SRDescription(nameof(SR.ComboBoxAutoCompleteCustomSourceDescr)),
        Editor("System.Windows.Forms.Design.ListControlStringCollectionEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor)),
        Browsable(true), EditorBrowsable(EditorBrowsableState.Always)
        ]
        public AutoCompleteStringCollection AutoCompleteCustomSource {
            get {
                if (autoCompleteCustomSource == null) {
                    autoCompleteCustomSource = new AutoCompleteStringCollection();
                    autoCompleteCustomSource.CollectionChanged += new CollectionChangeEventHandler(this.OnAutoCompleteCustomSourceChanged);
                }
                return autoCompleteCustomSource;
            }
            set {
                if (autoCompleteCustomSource != value) {

                    if (autoCompleteCustomSource != null) {
                        autoCompleteCustomSource.CollectionChanged -= new CollectionChangeEventHandler(this.OnAutoCompleteCustomSourceChanged);
                    }

                    autoCompleteCustomSource = value;

                    if (autoCompleteCustomSource != null) {
                        autoCompleteCustomSource.CollectionChanged += new CollectionChangeEventHandler(this.OnAutoCompleteCustomSourceChanged);
                    }
                    SetAutoComplete(false, true);
                }
            }
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.BackColor"]/*' />
        /// <devdoc>
        ///     The background color of this control. This is an ambient property and
        ///     will always return a non-null value.
        /// </devdoc>
        public override Color BackColor {
            get {
                if (ShouldSerializeBackColor()) {
                    return base.BackColor;
                }
                else {
                    return SystemColors.Window;
                }
            }
            set {
                base.BackColor = value;
            }
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.BackgroundImage"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override Image BackgroundImage {
            get {
                return base.BackgroundImage;
            }
            set {
                base.BackgroundImage = value;
            }
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.BackgroundImageLayout"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override ImageLayout BackgroundImageLayout {
            get {
                return base.BackgroundImageLayout;
            }
            set {
                base.BackgroundImageLayout = value;
            }
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.BackgroundImageChanged"]/*' />
        /// <internalonly/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler BackgroundImageChanged {
            add {
                base.BackgroundImageChanged += value;
            }
            remove {
                base.BackgroundImageChanged -= value;
            }
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.BackgroundImageLayoutChanged"]/*' />
        /// <internalonly/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler BackgroundImageLayoutChanged {
            add {
                base.BackgroundImageLayoutChanged += value;
            }
            remove {
                base.BackgroundImageLayoutChanged -= value;
            }
        }

        internal ChildAccessibleObject ChildEditAccessibleObject {
            get {
                if (childEditAccessibleObject == null) {
                    childEditAccessibleObject = new ComboBoxChildEditUiaProvider(this, childEdit.Handle);
                }

                return childEditAccessibleObject;
            }
        }

        internal AccessibleObject ChildTextAccessibleObject {
            get {
                if (childTextAccessibleObject == null) {
                    childTextAccessibleObject = new ComboBoxChildTextUiaProvider(this);
                }

                return childTextAccessibleObject;
            }
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.CreateParams"]/*' />
        /// <devdoc>
        ///     Returns the parameters needed to create the handle.  Inheriting classes
        ///     can override this to provide extra functionality.  They should not,
        ///     however, forget to call base.CreateParams() first to get the struct
        ///     filled up with the basic info.
        /// </devdoc>
        /// <internalonly/>
        protected override CreateParams CreateParams {
            [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            get {
                CreateParams cp = base.CreateParams;
                cp.ClassName = "COMBOBOX";
                cp.Style |= NativeMethods.WS_VSCROLL | NativeMethods.CBS_HASSTRINGS | NativeMethods.CBS_AUTOHSCROLL;
                cp.ExStyle |= NativeMethods.WS_EX_CLIENTEDGE;
                if (!integralHeight) cp.Style |= NativeMethods.CBS_NOINTEGRALHEIGHT;

                switch (DropDownStyle) {
                    case ComboBoxStyle.Simple:
                        cp.Style |= NativeMethods.CBS_SIMPLE;
                        break;
                    case ComboBoxStyle.DropDown:
                        cp.Style |= NativeMethods.CBS_DROPDOWN;
                        // Make sure we put the height back or we won't be able to size the dropdown!
                        cp.Height = PreferredHeight;
                        break;
                    case ComboBoxStyle.DropDownList:
                        cp.Style |= NativeMethods.CBS_DROPDOWNLIST;
                        // Comment above...
                        cp.Height = PreferredHeight;
                        break;
                }
                switch (DrawMode) {

                    case DrawMode.OwnerDrawFixed:
                        cp.Style |= NativeMethods.CBS_OWNERDRAWFIXED;
                        break;
                    case DrawMode.OwnerDrawVariable:
                        cp.Style |= NativeMethods.CBS_OWNERDRAWVARIABLE;
                        break;
                }

                return cp;
            }
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.DefaultSize"]/*' />
        /// <devdoc>
        ///     Deriving classes can override this to configure a default size for their control.
        ///     This is more efficient than setting the size in the control's constructor.
        /// </devdoc>
        protected override Size DefaultSize {
            get {
                return new Size(121, PreferredHeight);
            }
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.DataSource"]/*' />
        /// <devdoc>
        ///     The ListSource to consume as this ListBox's source of data.
        ///     When set, a user can not modify the Items collection.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatData)),
        DefaultValue(null),
        RefreshProperties(RefreshProperties.Repaint),
        AttributeProvider(typeof(IListSource)),
        SRDescription(nameof(SR.ListControlDataSourceDescr))
        ]
        public new object DataSource {
            get {
                return base.DataSource;
            }
            set {
                base.DataSource = value;
            }
        }


        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.DrawMode"]/*' />
        /// <devdoc>
        ///     Retrieves the value of the DrawMode property.  The DrawMode property
        ///     controls whether the control is drawn by Windows or by the user.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(DrawMode.Normal),
        SRDescription(nameof(SR.ComboBoxDrawModeDescr)),
        RefreshProperties(RefreshProperties.Repaint)
        ]
        public DrawMode DrawMode {
            get {
                bool found;
                int drawMode = Properties.GetInteger(PropDrawMode, out found);
                if (found) {
                    return (DrawMode)drawMode;
                }

                return DrawMode.Normal;
            }
            set {
                if (DrawMode != value) {
                    //valid values are 0x0 to 0x2.
                    if (!ClientUtils.IsEnumValid(value, (int)value, (int)DrawMode.Normal, (int)DrawMode.OwnerDrawVariable)) {
                        throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(DrawMode));
                    }
                    ResetHeightCache();
                    Properties.SetInteger(PropDrawMode, (int)value);
                    RecreateHandle();
                }
            }
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.DropDownWidth"]/*' />
        /// <devdoc>
        ///     Returns the width of the drop down box in a combo box.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        SRDescription(nameof(SR.ComboBoxDropDownWidthDescr))
        ]
        public int DropDownWidth {
            get {
                bool found;
                int dropDownWidth = Properties.GetInteger(PropDropDownWidth, out found);

                if (found) {
                    return dropDownWidth;
                }
                else {
                    return Width;
                }
            }

            set {
                if (value < 1) {
                    throw new ArgumentOutOfRangeException(nameof(DropDownWidth), string.Format(SR.InvalidArgument, "DropDownWidth", (value).ToString(CultureInfo.CurrentCulture)));
                }
                if (Properties.GetInteger(PropDropDownWidth) != value) {
                    Properties.SetInteger(PropDropDownWidth, value);
                    if (IsHandleCreated) {
                        SendMessage(NativeMethods.CB_SETDROPPEDWIDTH, value, 0);
                    }

                }
            }
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.DropDownHeight"]/*' />
        /// <devdoc>
        ///     Sets the Height of the drop down box in a combo box.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        SRDescription(nameof(SR.ComboBoxDropDownHeightDescr)),
        Browsable(true), EditorBrowsable(EditorBrowsableState.Always),
        DefaultValue(106)
        ]
        public int DropDownHeight {
            get {
                bool found;
                int dropDownHeight = Properties.GetInteger(PropDropDownHeight, out found);
                if (found) {
                    return dropDownHeight;
                }
                else {
                    return DefaultDropDownHeight;
                }
            }
            set {
                if (value < 1) {
                    throw new ArgumentOutOfRangeException(nameof(DropDownHeight), string.Format(SR.InvalidArgument, "DropDownHeight", (value).ToString(CultureInfo.CurrentCulture)));
                }
                if (Properties.GetInteger(PropDropDownHeight) != value) {
                    Properties.SetInteger(PropDropDownHeight, value);

                    // The dropDownHeight is not reflected unless the
                    // combobox integralHeight == false..
                    IntegralHeight = false;
                }
            }
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.DroppedDown"]/*' />
        /// <devdoc>
        ///     Indicates whether the DropDown of the combo is  currently dropped down.
        /// </devdoc>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ComboBoxDroppedDownDescr))
        ]
        public bool DroppedDown {
            get {
                if (IsHandleCreated) {
                    return unchecked( (int) (long)SendMessage(NativeMethods.CB_GETDROPPEDSTATE, 0, 0) ) != 0;
                }
                else {
                    return false;
                }
            }

            set {

                if (!IsHandleCreated) {
                    CreateHandle();
                }
                SendMessage(NativeMethods.CB_SHOWDROPDOWN, value ? -1 : 0, 0);
            }
        }

        /// <include file='doc\ButtonBase.uex' path='docs/doc[@for="ButtonBase.FlatStyle"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or
        ///       sets
        ///       the flat style appearance of the button control.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        DefaultValue(FlatStyle.Standard),
        Localizable(true),
        SRDescription(nameof(SR.ComboBoxFlatStyleDescr))
        ]
        public FlatStyle FlatStyle {
            get {
                return flatStyle;
            }
            set {
                //valid values are 0x0 to 0x3
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)FlatStyle.Flat, (int)FlatStyle.System)) {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(FlatStyle));
                }
                flatStyle = value;
                Invalidate();
            }
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.Focused"]/*' />
        /// <devdoc>
        ///     Returns true if this control has focus.
        /// </devdoc>
        public override bool Focused {
            get {
                if (base.Focused) return true;
                IntPtr focus = UnsafeNativeMethods.GetFocus();
                return focus != IntPtr.Zero && ((childEdit != null && focus == childEdit.Handle) || (childListBox != null && focus == childListBox.Handle));
            }
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.ForeColor"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets the foreground color of the control.
        ///    </para>
        /// </devdoc>
        public override Color ForeColor {
            get {
                if (ShouldSerializeForeColor()) {
                    return base.ForeColor;
                }
                else {
                    return SystemColors.WindowText;
                }
            }
            set {
                base.ForeColor = value;
            }
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.IntegralHeight"]/*' />
        /// <devdoc>
        ///     Indicates if the combo should avoid showing partial Items.  If so,
        ///     then only full items will be displayed, and the list portion will be resized
        ///     to prevent partial items from being shown.  Otherwise, they will be
        ///     shown
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(true),
        Localizable(true),
        SRDescription(nameof(SR.ComboBoxIntegralHeightDescr))
        ]
        public bool IntegralHeight {
            get {
                return integralHeight;
            }

            set {
                if (integralHeight != value) {
                    integralHeight = value;
                    RecreateHandle();
                }
            }
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.ItemHeight"]/*' />
        /// <devdoc>
        ///     Returns the height of an item in the combo box. When drawMode is Normal
        ///     or OwnerDrawFixed, all items have the same height. When drawMode is
        ///     OwnerDrawVariable, this method returns the height that will be given
        ///     to new items added to the combo box. To determine the actual height of
        ///     an item, use the GetItemHeight() method with an integer parameter.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        Localizable(true),
        SRDescription(nameof(SR.ComboBoxItemHeightDescr))
        ]
        public int ItemHeight {
            get {

                DrawMode drawMode = DrawMode;
                if (drawMode == DrawMode.OwnerDrawFixed ||
                    drawMode == DrawMode.OwnerDrawVariable ||
                    !IsHandleCreated) {

                    bool found;
                    int itemHeight = Properties.GetInteger(PropItemHeight, out found);
                    if (found) {
                        return itemHeight;
                    }
                    else {
                        return FontHeight + 2;   // 
                    }
                }

                // Note that the above if clause deals with the case when the handle has not yet been created
                Debug.Assert(IsHandleCreated, "Handle should be created at this point");

                int h = unchecked( (int) (long)SendMessage(NativeMethods.CB_GETITEMHEIGHT, 0, 0) );
                if (h == -1) {
                    throw new Win32Exception();
                }
                return h;
            }

            set {
                if (value < 1) {
                    throw new ArgumentOutOfRangeException(nameof(ItemHeight), string.Format(SR.InvalidArgument, "ItemHeight", (value).ToString(CultureInfo.CurrentCulture)));
                }

                ResetHeightCache();

                if (Properties.GetInteger(PropItemHeight) != value) {
                    Properties.SetInteger(PropItemHeight, value);
                    if (DrawMode != DrawMode.Normal) {
                        UpdateItemHeight();
                    }
                }
            }
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.Items"]/*' />
        /// <devdoc>
        ///     Collection of the items contained in this ComboBox.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatData)),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
        Localizable(true),
        SRDescription(nameof(SR.ComboBoxItemsDescr)),
        Editor("System.Windows.Forms.Design.ListControlStringCollectionEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor)),
        MergableProperty(false)
        ]
        public ObjectCollection Items {
            get {
                if (itemsCollection == null) {
                    itemsCollection = new ObjectCollection(this);
                }
                return itemsCollection;
            }
        }

        // Text used to match an item in the list when auto-completion
        // is used in DropDownList style.
        private string MatchingText {
            get {
                string matchingText = (string)Properties.GetObject(PropMatchingText);
                return (matchingText == null) ? string.Empty : matchingText;
            }
            set {
                if (value != null || this.Properties.ContainsObject(PropMatchingText)) {
                    Properties.SetObject(PropMatchingText, value);
                }
            }
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.MaxDropDownItems"]/*' />
        /// <devdoc>
        ///     The maximum number of items to be shown in the dropdown portion
        ///     of the ComboBox.  This number can be between 1 and 100.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(8),
        Localizable(true),
        SRDescription(nameof(SR.ComboBoxMaxDropDownItemsDescr))
        ]
        public int MaxDropDownItems {
            get {
                return maxDropDownItems;
            }
            set {
                if (value < 1 || value > 100) {
                    throw new ArgumentOutOfRangeException(nameof(MaxDropDownItems), string.Format(SR.InvalidBoundArgument, "MaxDropDownItems", (value).ToString(CultureInfo.CurrentCulture), (1).ToString(CultureInfo.CurrentCulture), (100).ToString(CultureInfo.CurrentCulture)));
                }
                maxDropDownItems = (short)value;
            }
        }

        public override Size MaximumSize {
            get { return base.MaximumSize; }
            set {
                base.MaximumSize = new Size(value.Width, 0);
            }
        }

        public override Size MinimumSize {
            get { return base.MinimumSize; }
            set {
                base.MinimumSize = new Size(value.Width, 0);
            }
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.MaxLength"]/*' />
        /// <devdoc>
        ///     The maximum length of the text the user may type into the edit control
        ///     of a combo box.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(0),
        Localizable(true),
        SRDescription(nameof(SR.ComboBoxMaxLengthDescr))
        ]
        public int MaxLength {
            get {
                return Properties.GetInteger(PropMaxLength);
            }
            set {
                if (value < 0) value = 0;
                if (MaxLength != value) {
                    Properties.SetInteger(PropMaxLength, value);
                    if (IsHandleCreated) SendMessage(NativeMethods.CB_LIMITTEXT, value, 0);
                }
            }
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.MouseIsOver"]/*' />
        /// <devdoc>
        ///     If the mouse is over the combobox, draw selection rect.
        /// </devdoc>
        internal bool MouseIsOver {
            get { return mouseOver; }
            set {
                if (mouseOver != value) {
                    mouseOver = value;
                    // Nothing to see here... Just keep on walking...
                    // Turns out that with Theming off, we don't get quite the same messages as with theming on, so
                    // our drawing gets a little messed up. So in case theming is off, force a draw here.
                    if ((!ContainsFocus || !Application.RenderWithVisualStyles) && this.FlatStyle == FlatStyle.Popup) {
                        Invalidate();
                        Update();
                    }
                }
            }
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.Padding"]/*' />
        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)
        ]
        public new Padding Padding {
            get { return base.Padding; }
            set { base.Padding = value; }
        }

        [
        Browsable(false),
        EditorBrowsable(EditorBrowsableState.Never)
        ]
        public new event EventHandler PaddingChanged {
            add { base.PaddingChanged += value; }
            remove { base.PaddingChanged -= value; }
        }
            

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.PreferredHeight"]/*' />
        /// <devdoc>
        ///     ApplySizeConstraints calls into this method when DropDownStyles is DropDown and DropDownList.
        ///     This causes PreferredSize to be bounded by PreferredHeight in these two cases only.
        /// </devdoc>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ComboBoxPreferredHeightDescr))
        ]
        public int PreferredHeight {
            get {
                if (!FormattingEnabled) {

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

                    Size textSize = TextRenderer.MeasureText(LayoutUtils.TestString, this.Font, new Size(Int16.MaxValue, (int)(FontHeight * 1.25)), TextFormatFlags.SingleLine);
                    prefHeightCache = (short)(textSize.Height + SystemInformation.BorderSize.Height * 8 + Padding.Size.Height);

                    return prefHeightCache ;
                }
                else {
                    // Normally we do this sort of calculation in GetPreferredSizeCore which has builtin
                    // caching, but in this case we can not because PreferredHeight is used in ApplySizeConstraints
                    // which is used by GetPreferredSize (infinite loop).
                    if (prefHeightCache < 0) {
                        Size textSize = TextRenderer.MeasureText(LayoutUtils.TestString, this.Font, new Size(Int16.MaxValue, (int)(FontHeight * 1.25)), TextFormatFlags.SingleLine);

                        // For a "simple" style combobox, the preferred height depends on the
                        // number of items in the combobox.
                        if (DropDownStyle == ComboBoxStyle.Simple) {
                            int itemCount = Items.Count + 1;
                            prefHeightCache = (short)(textSize.Height * itemCount + SystemInformation.BorderSize.Height * 16 + Padding.Size.Height);
                        }
                        else {
                            // We do this old school rather than use SizeFromClientSize because CreateParams calls this
                            // method and SizeFromClientSize calls CreateParams (another infinite loop.)
                            prefHeightCache = (short)GetComboHeight();
                        }
                    }
                    return prefHeightCache;
                }
            }
        }


        // ComboBox.PreferredHeight returns incorrect values
        // This is translated from windows implementation.  Since we cannot control the size
        // of the combo box, we need to use the same calculation they do.
        [SuppressMessage("Microsoft.Globalization", "CA1303:DoNotPassLiteralsAsLocalizedParameters")] // "0" is what Windows uses in all languages.
        private int GetComboHeight() {

            int cyCombo = 0;
            // Add on CYEDGE just for some extra space in the edit field/static item.       
            // It's really only for static text items, but we want static & editable
            // controls to be the same height.
            Size textExtent = Size.Empty;

            using (WindowsFont font = WindowsFont.FromFont(this.Font)) {
                // this is the character that Windows uses to determine the extent
                textExtent = WindowsGraphicsCacheManager.MeasurementGraphics.GetTextExtent("0", font);
            }

            int dyEdit = textExtent.Height + SystemInformation.Border3DSize.Height;

            if (DrawMode != DrawMode.Normal) {
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
                for (int i = 0; i < AutoCompleteCustomSource.Count; i++) {
                    strings[i] = AutoCompleteCustomSource[i];
                }
                return strings;
                
            }
            else if (collection is ObjectCollection)
            {
                string[] strings = new string[itemsCollection.Count];
                for (int i = 0; i < itemsCollection.Count; i++) {
                    strings[i] = GetItemText(itemsCollection[i]);
                }
                return strings;
                
            }
            return new string[0];
        }


        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.SelectedIndex"]/*' />
        /// <devdoc>
        ///     The [zero based] index of the currently selected item in the combos list.
        ///     Note If the value of index is -1, then the ComboBox is
        ///     set to have no selection.
        /// </devdoc>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ComboBoxSelectedIndexDescr))
        ]
        public override int SelectedIndex {
            get {
                if (IsHandleCreated) {

                    return unchecked( (int) (long)SendMessage(NativeMethods.CB_GETCURSEL, 0, 0) );
                }
                else {
                    return selectedIndex;
                }
            }
            set {
                if (SelectedIndex != value) {
                    int itemCount = 0;
                    if (itemsCollection != null) {
                        itemCount = itemsCollection.Count;
                    }

                    if (value < -1 || value >= itemCount) {
                        throw new ArgumentOutOfRangeException(nameof(SelectedIndex), string.Format(SR.InvalidArgument, "SelectedIndex", (value).ToString(CultureInfo.CurrentCulture)));
                    }

                    if (IsHandleCreated) {
                        SendMessage(NativeMethods.CB_SETCURSEL, value, 0);

                    }
                    else {
                        selectedIndex = value;
                    }

                    UpdateText();

                    if (IsHandleCreated) {
                        OnTextChanged(EventArgs.Empty);
                    }

                    OnSelectedItemChanged(EventArgs.Empty);
                    OnSelectedIndexChanged(EventArgs.Empty);
                }
            }
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.SelectedItem"]/*' />
        /// <devdoc>
        ///     The handle to the object that is currently selected in the
        ///     combos list.
        /// </devdoc>
        [
        Browsable(false),
        Bindable(true),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ComboBoxSelectedItemDescr))
        ]
        public object SelectedItem {
            get {
                int index = SelectedIndex;
                return (index == -1) ? null : Items[index];
            }
            set {
                int x = -1;

                if (itemsCollection != null) {
                    //
                    if (value != null)
                        x = itemsCollection.IndexOf(value);
                    else
                        SelectedIndex = -1;
                }

                if (x != -1) {
                    SelectedIndex = x;
                }
            }
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.SelectedText"]/*' />
        /// <devdoc>
        ///     The selected text in the edit component of the ComboBox. If the
        ///     ComboBox has ComboBoxStyle.DROPDOWNLIST, the return is an empty
        ///     string ("").
        /// </devdoc>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ComboBoxSelectedTextDescr))
        ]
        public string SelectedText {
            get {
                if (DropDownStyle == ComboBoxStyle.DropDownList) return "";
                return Text.Substring(SelectionStart, SelectionLength);
            }
            set {
                if (DropDownStyle != ComboBoxStyle.DropDownList) {
                    //guard against null string, since otherwise we will throw an
                    //AccessViolation exception, which is bad
                    string str = (value == null ? "" : value);
                    CreateControl();
                    if (IsHandleCreated) {
                        Debug.Assert(childEdit != null);
                        if (childEdit != null) {
                            UnsafeNativeMethods.SendMessage(new HandleRef(this, childEdit.Handle), NativeMethods.EM_REPLACESEL, NativeMethods.InvalidIntPtr, str);
                        }
                    }
                }
            }
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.SelectionLength"]/*' />
        /// <devdoc>
        ///     The length, in characters, of the selection in the editbox.
        /// </devdoc>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ComboBoxSelectionLengthDescr))
        ]
        public int SelectionLength {
            get {
                int[] end = new int[] { 0 };
                int[] start = new int[] { 0 };
                UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.CB_GETEDITSEL, start, end);
                return end[0] - start[0];
            }
            set {
                // SelectionLength can be negtive...
                Select(SelectionStart, value);
            }
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.SelectionStart"]/*' />
        /// <devdoc>
        ///     The [zero-based] index of the first character in the current text selection.
        /// </devdoc>
        [
        Browsable(false),
        DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden),
        SRDescription(nameof(SR.ComboBoxSelectionStartDescr))
        ]
        public int SelectionStart {
            get {
                int[] value = new int[] { 0 };
                UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.CB_GETEDITSEL, value, (int[])null);
                return value[0];
            }
            set {
                if (value < 0) {
                    throw new ArgumentOutOfRangeException(nameof(SelectionStart), string.Format(SR.InvalidArgument, "SelectionStart", value.ToString(CultureInfo.CurrentCulture)));
                }
                Select(value, SelectionLength);
            }
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.Sorted"]/*' />
        /// <devdoc>
        ///     Indicates if the Combos list is sorted or not.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(false),
        SRDescription(nameof(SR.ComboBoxSortedDescr))
        ]
        public bool Sorted {
            get {
                return sorted;
            }
            set {
                if (sorted != value) {
                    if (this.DataSource != null && value) {
                        throw new ArgumentException(SR.ComboBoxSortWithDataSource);
                    }

                    sorted = value;
                    RefreshItems();
                    SelectedIndex = -1;
                }
            }
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.DropDownStyle"]/*' />
        /// <devdoc>
        ///     The type of combo that we are right now.  The value would come
        ///     from the System.Windows.Forms.ComboBoxStyle enumeration.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        DefaultValue(ComboBoxStyle.DropDown),
        SRDescription(nameof(SR.ComboBoxStyleDescr)),
        RefreshPropertiesAttribute(RefreshProperties.Repaint)
        ]
        public ComboBoxStyle DropDownStyle {
            get {
                bool found;
                int style = Properties.GetInteger(PropStyle, out found);
                if (found) {
                    return (ComboBoxStyle)style;
                }

                return ComboBoxStyle.DropDown;
            }
            set {
                if (DropDownStyle != value) {

                    // verify that 'value' is a valid enum type...
                    //valid values are 0x0 to 0x2
                    if (!ClientUtils.IsEnumValid(value, (int)value, (int)ComboBoxStyle.Simple, (int)ComboBoxStyle.DropDownList))
                    {
                        throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(ComboBoxStyle));
                    }

                    if (value == ComboBoxStyle.DropDownList &&
                        this.AutoCompleteSource != AutoCompleteSource.ListItems &&
                        this.AutoCompleteMode != AutoCompleteMode.None) {
                        this.AutoCompleteMode = AutoCompleteMode.None;
                    }

                    // reset preferred height.
                    ResetHeightCache();

                    Properties.SetInteger(PropStyle, (int)value);

                    if (IsHandleCreated) {
                        RecreateHandle();
                    }

                    OnDropDownStyleChanged(EventArgs.Empty);
                }
            }
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.Text"]/*' />
        [
        Localizable(true),
        Bindable(true)
        ]
        public override string Text {
            get {
                if (SelectedItem != null && !BindingFieldEmpty) {

                    //preserve everett behavior if "formatting enabled == false" -- just return selecteditem text.
                    if (FormattingEnabled) {
                        string candidate = GetItemText(SelectedItem);
                        if (!String.IsNullOrEmpty(candidate)) {
                            if (String.Compare(candidate, base.Text, true, CultureInfo.CurrentCulture) == 0) {
                                return candidate;   //for whidbey, if we only differ by case -- return the candidate;
                            }
                        }
                    }
                    else {
                        return FilterItemOnProperty(SelectedItem).ToString();       //heinous.
                    }
                }
                return base.Text;
            }
            set {
                if (DropDownStyle == ComboBoxStyle.DropDownList && !IsHandleCreated && !String.IsNullOrEmpty(value) && FindStringExact(value) == -1)
                    return;

                base.Text = value;
                object selectedItem = null;

                selectedItem = SelectedItem;

                if (!DesignMode) {
                    //

                    if (value == null) {
                        SelectedIndex = -1;
                    }
                    else if (value != null &&
                        (selectedItem == null || (String.Compare(value, GetItemText(selectedItem), false, CultureInfo.CurrentCulture) != 0))) {

                        int index = FindStringIgnoreCase(value);

                        //we cannot set the index to -1 unless we want to do something unusual and save/restore text
                        //because the native control will erase the text when we change the index to -1
                        if (index != -1) {
                            SelectedIndex = index;
                        }
                    }
                }
            }
        }


        private int FindStringIgnoreCase(string value) {
            //look for an exact match and then a case insensitive match if that fails.
            int index = FindStringExact(value, -1, false);

            if (index == -1) {
                index = FindStringExact(value, -1, true);
            }

            return index;
        }

        // Special AutoComplete notification handling
        // If the text changed, this will fire TextChanged
        // If it matches an item in the list, this will fire SIC and SVC
        private void NotifyAutoComplete() {
            NotifyAutoComplete(true);
        }

        // Special AutoComplete notification handling
        // If the text changed, this will fire TextChanged
        // If it matches an item in the list, this will fire SIC and SVC
        private void NotifyAutoComplete(bool setSelectedIndex) {
            string text = this.Text;
            bool textChanged = (text != this.lastTextChangedValue);
            bool selectedIndexSet = false;

            if (setSelectedIndex) {
                // Process return key.  This is sent by the AutoComplete DropDown when a
                // selection is made from the DropDown
                // Check to see if the Text Changed.  If so, at least fire a TextChanged
                int index = FindStringIgnoreCase(text);

                if ((index != -1) && (index != SelectedIndex)) {
                    // We found a match, do the full monty
                    SelectedIndex = index;

                    // Put the cursor at the end
                    SelectionStart = 0;
                    SelectionLength = text.Length;

                    selectedIndexSet = true;
                }
            }

            //don't fire textch if we had set the selectedindex -- because it was already fired if so.
            if (textChanged && !selectedIndexSet) {
                // No match, just fire a TextChagned
                OnTextChanged(EventArgs.Empty);
            }

            // Save the new value
            this.lastTextChangedValue = text;
        }

        internal override bool SupportsUiaProviders {
            get {
                return AccessibilityImprovements.Level3;
            }
        }

        // Returns true if using System AutoComplete
        private bool SystemAutoCompleteEnabled {
            get {
                return ((this.autoCompleteMode != AutoCompleteMode.None) && (this.DropDownStyle != ComboBoxStyle.DropDownList));
            }
        }

        // Prevent this event from being displayed in the Property Grid.
        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.DoubleClick"]/*' />
        /// <internalonly/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler DoubleClick {
            add {
                base.DoubleClick += value;
            }
            remove {
                base.DoubleClick -= value;
            }
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.DrawItem"]/*' />
        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.drawItemEventDescr))]
        public event DrawItemEventHandler DrawItem {
            add {
                Events.AddHandler(EVENT_DRAWITEM, value);
            }
            remove {
                Events.RemoveHandler(EVENT_DRAWITEM, value);
            }
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.DropDown"]/*' />
        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.ComboBoxOnDropDownDescr))]
        public event EventHandler DropDown {
            add {
                Events.AddHandler(EVENT_DROPDOWN, value);
            }
            remove {
                Events.RemoveHandler(EVENT_DROPDOWN, value);
            }
        }


        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.MeasureItem"]/*' />
        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.measureItemEventDescr))]
        public event MeasureItemEventHandler MeasureItem {
            add {
                Events.AddHandler(EVENT_MEASUREITEM, value);
                UpdateItemHeight();
            }
            remove {
                Events.RemoveHandler(EVENT_MEASUREITEM, value);
                UpdateItemHeight();
            }
        }


        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.SelectedIndexChanged"]/*' />
        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.selectedIndexChangedEventDescr))]
        public event EventHandler SelectedIndexChanged {
            add {
                Events.AddHandler(EVENT_SELECTEDINDEXCHANGED, value);
            }
            remove {
                Events.RemoveHandler(EVENT_SELECTEDINDEXCHANGED, value);
            }
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.SelectionChangeCommitted"]/*' />
        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.selectionChangeCommittedEventDescr))]
        public event EventHandler SelectionChangeCommitted {
            add {
                Events.AddHandler(EVENT_SELECTIONCHANGECOMMITTED, value);
            }
            remove {
                Events.RemoveHandler(EVENT_SELECTIONCHANGECOMMITTED, value);
            }
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.DropDownStyleChanged"]/*' />
        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.ComboBoxDropDownStyleChangedDescr))]
        public event EventHandler DropDownStyleChanged {
            add {
                Events.AddHandler(EVENT_DROPDOWNSTYLE, value);
            }
            remove {
                Events.RemoveHandler(EVENT_DROPDOWNSTYLE, value);
            }
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.OnPaint"]/*' />
        /// <devdoc>
        ///     ComboBox Onpaint.
        /// </devdoc>
        /// <internalonly/><hideinheritance/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event PaintEventHandler Paint {
            add {
                base.Paint += value;
            }
            remove {
                base.Paint -= value;
            }
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.TextUpdate"]/*' />
        /// <devdoc>
        ///    <para>This will fire the TextUpdate Event on the ComboBox. This events fires when the Combobox gets the
        ///          CBN_EDITUPDATE notification.
        //     </para>
        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.TextUpdate"]/*' />
        /// </devdoc>
        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.ComboBoxOnTextUpdateDescr))]
        public event EventHandler TextUpdate {
            add {
                Events.AddHandler(EVENT_TEXTUPDATE, value);
            }
            remove {
                Events.RemoveHandler(EVENT_TEXTUPDATE, value);
            }
        }


        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.DropDownClosed"]/*' />
        /// <devdoc>
        ///     <para>This will fire the DropDownClosed Event on the ComboBox. This events fires when the Combobox gets the
        ///          CBN_CLOSEUP notification. This happens when the DropDown closes.
        //      </para>
        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.DropDownClosed"]/*' />
        /// </devdoc>
        [SRCategory(nameof(SR.CatBehavior)), SRDescription(nameof(SR.ComboBoxOnDropDownClosedDescr))]
        public event EventHandler DropDownClosed {
            add {
                Events.AddHandler(EVENT_DROPDOWNCLOSED, value);
            }
            remove {
                Events.RemoveHandler(EVENT_DROPDOWNCLOSED, value);
            }
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.AddItemsCore"]/*' />
        /// <devdoc>
        ///     Performs the work of adding the specified items to the combobox
        /// </devdoc>
        [Obsolete("This method has been deprecated.  There is no replacement.  http://go.microsoft.com/fwlink/?linkid=14202")]
        protected virtual void AddItemsCore(object[] value) {
            int count = value == null ? 0 : value.Length;
            if (count == 0) {
                return;
            }

            BeginUpdate();
            try {
                Items.AddRangeInternal(value);
            }
            finally {
                EndUpdate();
            }
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.BeginUpdate"]/*' />
        /// <devdoc>
        ///     Disables redrawing of the combo box. A call to beginUpdate() must be
        ///     balanced by a following call to endUpdate(). Following a call to
        ///     beginUpdate(), any redrawing caused by operations performed on the
        ///     combo box is deferred until the call to endUpdate().
        /// </devdoc>
        public void BeginUpdate() {
            updateCount++;
            BeginUpdateInternal();
        }

        private void CheckNoDataSource() {
            if (DataSource != null) {
                throw new ArgumentException(SR.DataSourceLocksItems);
            }
        }

        /// <devdoc>
        /// </devdoc>
        protected override AccessibleObject CreateAccessibilityInstance() {
            if (AccessibilityImprovements.Level3) {
                return new ComboBoxUiaProvider(this);
            }
            else if (AccessibilityImprovements.Level1) {
                return new ComboBoxExAccessibleObject(this);
            }
            else {
                return new ComboBoxAccessibleObject(this);
            }
        }

         /// <devdoc>
        /// </devdoc>
        internal bool UpdateNeeded()
        {
            return (updateCount == 0);
        }


        /// <devdoc>
        ///     This procedure takes in the message, converts the Edit handle coordinates into Combo Box Coordinates
        /// </devdoc>
        /// <internalonly/>
        internal Point EditToComboboxMapping(Message m) {
            if (childEdit == null) {
                return new Point(0, 0);
            }
            // Get the Combox Rect ...
            //
            NativeMethods.RECT comboRectMid = new NativeMethods.RECT();
            UnsafeNativeMethods.GetWindowRect(new HandleRef(this, Handle), ref comboRectMid);
            //
            //Get the Edit Rectangle...
            //
            NativeMethods.RECT editRectMid = new NativeMethods.RECT();
            UnsafeNativeMethods.GetWindowRect(new HandleRef(this, childEdit.Handle), ref editRectMid);

            //get the delta
            int comboXMid = NativeMethods.Util.SignedLOWORD(m.LParam) + (editRectMid.left - comboRectMid.left);
            int comboYMid = NativeMethods.Util.SignedHIWORD(m.LParam) + (editRectMid.top - comboRectMid.top);

            return (new Point(comboXMid, comboYMid));

        }


        /// <devdoc>
        ///     Subclassed window procedure for the edit and list child controls of the
        ///     combo box.
        /// </devdoc>
        /// <internalonly/>
        private void ChildWndProc(ref Message m) {

            switch (m.Msg) {
                case NativeMethods.WM_CHAR:
                    if (DropDownStyle == ComboBoxStyle.Simple && m.HWnd == childListBox.Handle) {
                        DefChildWndProc(ref m);
                    }
                    else {
                        if (PreProcessControlMessage(ref m) != PreProcessControlState.MessageProcessed) {
                            if (ProcessKeyMessage(ref m)) {
                                return;
                            }
                            DefChildWndProc(ref m);
                        }
                    }
                    break;
                case NativeMethods.WM_SYSCHAR:
                    if (DropDownStyle == ComboBoxStyle.Simple && m.HWnd == childListBox.Handle) {
                        DefChildWndProc(ref m);
                    }
                    else {
                        if (PreProcessControlMessage(ref m) != PreProcessControlState.MessageProcessed) {
                            if (ProcessKeyEventArgs(ref m)) {
                                return;
                            }
                            DefChildWndProc(ref m);
                        }
                    }
                    break;
                case NativeMethods.WM_KEYDOWN:
                case NativeMethods.WM_SYSKEYDOWN:
                    if (SystemAutoCompleteEnabled && !ACNativeWindow.AutoCompleteActive) {
                        finder.FindDropDowns(false);
                    }

                    if (AutoCompleteMode != AutoCompleteMode.None) {
                        char keyChar = unchecked((char)(long)m.WParam);
                        if (keyChar == (char)(int)Keys.Escape) {
                            this.DroppedDown = false;
                        }
                        else if (keyChar == (char)(int)Keys.Return && this.DroppedDown) {
                            UpdateText();
                            OnSelectionChangeCommittedInternal(EventArgs.Empty);
                            this.DroppedDown = false;
                        }
                    }
                    
                    if (DropDownStyle == ComboBoxStyle.Simple && m.HWnd == childListBox.Handle) {
                        DefChildWndProc(ref m);
                    }
                    else {
                        if (PreProcessControlMessage(ref m) != PreProcessControlState.MessageProcessed) {
                            if (ProcessKeyMessage(ref m)) {
                                return;
                            }
                            DefChildWndProc(ref m);
                        }
                    }
                    break;

                case NativeMethods.WM_INPUTLANGCHANGE:
                    DefChildWndProc( ref m ); 
                    break;

                case NativeMethods.WM_KEYUP:
                case NativeMethods.WM_SYSKEYUP:
                    if (DropDownStyle == ComboBoxStyle.Simple && m.HWnd == childListBox.Handle) {
                        DefChildWndProc(ref m);
                    }
                    else {
                        if (PreProcessControlMessage(ref m) != PreProcessControlState.MessageProcessed) {
                            if (ProcessKeyMessage(ref m)) {
                                return;
                            }
                            DefChildWndProc(ref m);
                        }
                    }
                    if (SystemAutoCompleteEnabled && !ACNativeWindow.AutoCompleteActive) {
                        finder.FindDropDowns();
                    }

                    break;
                case NativeMethods.WM_KILLFOCUS:
                    // Consider - If we dont' have a childwndproc, then we don't get here, so we don't 
                    // update the cache. Do we need to? This happens when we have a DropDownList.
                    if (!DesignMode) {
                        OnImeContextStatusChanged( m.HWnd );
                    }

                    DefChildWndProc(ref m);
                    // We don't want to fire the focus events twice -
                    // once in the combobox and once here.
                    if (fireLostFocus) {
                        this.InvokeLostFocus(this, EventArgs.Empty);
                    }

                    if (FlatStyle == FlatStyle.Popup) {
                        this.Invalidate();
                    }

                    break;
                case NativeMethods.WM_SETFOCUS:

                    // Consider - If we dont' have a childwndproc, then we don't get here, so we don't 
                    // set the status. Do we need to? This happens when we have a DropDownList.
                    if (!DesignMode) {
                        ImeContext.SetImeStatus(CachedImeMode, m.HWnd);
                    }

                    if (!HostedInWin32DialogManager) {
                        IContainerControl c = GetContainerControlInternal();
                        if (c != null) {
                            ContainerControl container = c as ContainerControl;
                            if (container != null) {
                                if (!container.ActivateControlInternal(this, false)) {
                                    return;
                                }
                            }
                        }
                    }

                    DefChildWndProc(ref m);

                    // We don't want to fire the focus events twice -
                    // once in the combobox and once here.
                    if (fireSetFocus) {
                        this.InvokeGotFocus(this, EventArgs.Empty);
                    }

                    if (FlatStyle == FlatStyle.Popup) {
                        this.Invalidate();
                    }
                    break;

                case NativeMethods.WM_SETFONT:
                    DefChildWndProc(ref m);
                    if (childEdit != null && m.HWnd == childEdit.Handle) {
                        UnsafeNativeMethods.SendMessage(new HandleRef(this, childEdit.Handle), NativeMethods.EM_SETMARGINS,
                                                  NativeMethods.EC_LEFTMARGIN | NativeMethods.EC_RIGHTMARGIN, 0);
                    }
                    break;
                case NativeMethods.WM_LBUTTONDBLCLK:
                    //the Listbox gets  WM_LBUTTONDOWN - WM_LBUTTONUP -WM_LBUTTONDBLCLK - WM_LBUTTONUP...
                    //sequence for doubleclick...
                    //Set MouseEvents...
                    mousePressed = true;
                    mouseEvents = true;
                    CaptureInternal = true;
                    //Call the DefWndProc() so that mousemove messages get to the windows edit
                    //
                    DefChildWndProc(ref m);
                    //the up gets fired from "Combo-box's WndPrc --- So Convert these Coordinates to Combobox coordianate...
                    //
                    Point Ptlc = EditToComboboxMapping(m);
                    OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, Ptlc.X, Ptlc.Y, 0));
                    break;

                case NativeMethods.WM_MBUTTONDBLCLK:
                    //the Listbox gets  WM_LBUTTONDOWN - WM_LBUTTONUP -WM_LBUTTONDBLCLK - WM_LBUTTONUP...
                    //sequence for doubleclick...
                    //Set MouseEvents...
                    mousePressed = true;
                    mouseEvents = true;
                    CaptureInternal = true;
                    //Call the DefWndProc() so that mousemove messages get to the windows edit
                    //
                    DefChildWndProc(ref m);
                    //the up gets fired from "Combo-box's WndPrc --- So Convert these Coordinates to Combobox coordianate...
                    //
                    Point Ptmc = EditToComboboxMapping(m);
                    OnMouseDown(new MouseEventArgs(MouseButtons.Middle, 1, Ptmc.X, Ptmc.Y, 0));
                    break;

                case NativeMethods.WM_RBUTTONDBLCLK:
                    //the Listbox gets  WM_LBUTTONDOWN - WM_LBUTTONUP -WM_LBUTTONDBLCLK - WM_LBUTTONUP...
                    //sequence for doubleclick...
                    //Set MouseEvents...
                    mousePressed = true;
                    mouseEvents = true;
                    CaptureInternal = true;
                    //Call the DefWndProc() so that mousemove messages get to the windows edit
                    //
                    DefChildWndProc(ref m);
                    //the up gets fired from "Combo-box's WndPrc --- So Convert these Coordinates to Combobox coordianate...
                    //
                    Point Ptrc = EditToComboboxMapping(m);
                    OnMouseDown(new MouseEventArgs(MouseButtons.Right, 1, Ptrc.X, Ptrc.Y, 0));
                    break;

                case NativeMethods.WM_LBUTTONDOWN:
                    mousePressed = true;
                    mouseEvents = true;
                    //set the mouse capture .. this is the Child Wndproc..
                    //
                    CaptureInternal = true;
                    DefChildWndProc(ref m);
                    //the up gets fired from "Combo-box's WndPrc --- So Convert these Coordinates to Combobox coordianate...
                    //
                    Point Ptl = EditToComboboxMapping(m);

                    OnMouseDown(new MouseEventArgs(MouseButtons.Left, 1, Ptl.X, Ptl.Y, 0));
                    break;
                case NativeMethods.WM_LBUTTONUP:
                    // Get the mouse location
                    //
                    NativeMethods.RECT r = new NativeMethods.RECT();
                    UnsafeNativeMethods.GetWindowRect(new HandleRef(this, Handle), ref r);
                    Rectangle ClientRect = new Rectangle(r.left, r.top, r.right - r.left, r.bottom - r.top);
                    // Get the mouse location
                    //
                    int x = NativeMethods.Util.SignedLOWORD(m.LParam);
                    int y = NativeMethods.Util.SignedHIWORD(m.LParam);
                    Point pt = new Point(x, y);
                    pt = PointToScreen(pt);
                    // combo box gets a WM_LBUTTONUP for focus change ...
                    // So check MouseEvents....
                    if (mouseEvents && !ValidationCancelled) {
                        mouseEvents = false;
                        if (mousePressed) {
                            if (ClientRect.Contains(pt)) {
                                mousePressed = false;
                                OnClick(new MouseEventArgs(MouseButtons.Left, 1, NativeMethods.Util.SignedLOWORD(m.LParam), NativeMethods.Util.SignedHIWORD(m.LParam), 0));
                                OnMouseClick(new MouseEventArgs(MouseButtons.Left, 1, NativeMethods.Util.SignedLOWORD(m.LParam), NativeMethods.Util.SignedHIWORD(m.LParam), 0));
                            }
                            else {
                                mousePressed = false;
                                mouseInEdit = false;
                                OnMouseLeave(EventArgs.Empty);
                            }
                        }
                    }
                    DefChildWndProc(ref m);
                    CaptureInternal = false;

                    //the up gets fired from "Combo-box's WndPrc --- So Convert these Coordinates to Combobox coordianate...
                    //
                    pt = EditToComboboxMapping(m);

                    OnMouseUp(new MouseEventArgs(MouseButtons.Left, 1, pt.X, pt.Y, 0));
                    break;
                case NativeMethods.WM_MBUTTONDOWN:
                    mousePressed = true;
                    mouseEvents = true;
                    //set the mouse capture .. this is the Child Wndproc..
                    //
                    CaptureInternal = true;
                    DefChildWndProc(ref m);
                    //the up gets fired from "Combo-box's WndPrc --- So Convert these Coordinates to Combobox coordianate...
                    //
                    Point P = EditToComboboxMapping(m);

                    OnMouseDown(new MouseEventArgs(MouseButtons.Middle, 1, P.X, P.Y, 0));
                    break;
                case NativeMethods.WM_RBUTTONDOWN:
                    mousePressed = true;
                    mouseEvents = true;

                    //set the mouse capture .. this is the Child Wndproc..
                    // 



                    if (this.ContextMenu != null || this.ContextMenuStrip != null)
                        CaptureInternal = true;
                    DefChildWndProc(ref m);
                    //the up gets fired from "Combo-box's WndPrc --- So Convert these Coordinates to Combobox coordianate...
                    //
                    Point Pt = EditToComboboxMapping(m);

                    OnMouseDown(new MouseEventArgs(MouseButtons.Right, 1, Pt.X, Pt.Y, 0));
                    break;
                case NativeMethods.WM_MBUTTONUP:
                    mousePressed = false;
                    mouseEvents = false;
                    //set the mouse capture .. this is the Child Wndproc..
                    //
                    CaptureInternal = false;
                    DefChildWndProc(ref m);
                    OnMouseUp(new MouseEventArgs(MouseButtons.Middle, 1, NativeMethods.Util.SignedLOWORD(m.LParam), NativeMethods.Util.SignedHIWORD(m.LParam), 0));
                    break;
                case NativeMethods.WM_RBUTTONUP:
                    mousePressed = false;
                    mouseEvents = false;
                    //set the mouse capture .. this is the Child Wndproc..
                    //
                    if (this.ContextMenu != null)
                        CaptureInternal = false;
                    DefChildWndProc(ref m);
                    //the up gets fired from "Combo-box's WndPrc --- So Convert these Coordinates to Combobox coordianate...
                    //
                    Point ptRBtnUp = EditToComboboxMapping(m);

                    OnMouseUp(new MouseEventArgs(MouseButtons.Right, 1, ptRBtnUp.X, ptRBtnUp.Y, 0));
                    break;

                case NativeMethods.WM_CONTEXTMENU:
                    // Forward context menu messages to the parent control
                    if (this.ContextMenu != null || this.ContextMenuStrip != null) {
                        UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.WM_CONTEXTMENU, m.WParam, m.LParam);
                    }
                    else {
                        DefChildWndProc(ref m);
                    }
                    break;

                case NativeMethods.WM_MOUSEMOVE:
                    Point point = EditToComboboxMapping(m);
                    //Call the DefWndProc() so that mousemove messages get to the windows edit
                    //
                    DefChildWndProc(ref m);
                    OnMouseEnterInternal(EventArgs.Empty);
                    OnMouseMove(new MouseEventArgs(MouseButtons, 0, point.X, point.Y, 0));
                    break;

                case NativeMethods.WM_SETCURSOR:
                    if (Cursor != DefaultCursor && childEdit != null && m.HWnd == childEdit.Handle && NativeMethods.Util.LOWORD(m.LParam) == NativeMethods.HTCLIENT) {
                        Cursor.CurrentInternal = Cursor;
                    }
                    else {
                        DefChildWndProc(ref m);
                    }
                    break;

                case NativeMethods.WM_MOUSELEAVE:
                    DefChildWndProc(ref m);
                    OnMouseLeaveInternal(EventArgs.Empty);
                    break;

                default:
                    DefChildWndProc(ref m);
                    break;
            }
        }

        /// <summary>
        /// Helper to handle MouseEnter.
        /// </summary>
        /// <param name="args"></param>
        private void OnMouseEnterInternal(EventArgs args) {
            if (!mouseInEdit) {
                OnMouseEnter(args);
                mouseInEdit = true;
            }
        }

        /// <summary>
        /// Helper to handle mouseleave
        /// </summary>
        /// <param name="args"></param>
        private void OnMouseLeaveInternal(EventArgs args) {
            NativeMethods.RECT rect = new NativeMethods.RECT();
            UnsafeNativeMethods.GetWindowRect(new HandleRef(this, Handle), ref rect);
            Rectangle Rect = new Rectangle(rect.left, rect.top, rect.right - rect.left, rect.bottom - rect.top);
            Point p = MousePosition;
            if (!Rect.Contains(p)) {
                OnMouseLeave(args);
                mouseInEdit = false;
            }
        }

        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        private void DefChildWndProc(ref Message m) {
            if (childEdit != null) {
                NativeWindow childWindow = (m.HWnd == childEdit.Handle) ? childEdit : childListBox;
                //childwindow could be null if the handle was recreated while within a message handler
                // and then whoever recreated the handle allowed the message to continue to be processed
                //we cannot really be sure the new child will properly handle this window message, so we eat it.
                if (childWindow != null) {
                    childWindow.DefWndProc(ref m);
                }
            }
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.Dispose"]/*' />
        protected override void Dispose(bool disposing) {
            if (disposing) {
                if (autoCompleteCustomSource != null) {
                    autoCompleteCustomSource.CollectionChanged -= new CollectionChangeEventHandler(this.OnAutoCompleteCustomSourceChanged);
                }
                if (stringSource != null)
                {
                    stringSource.ReleaseAutoComplete();
                    stringSource = null;
                }
            }
            base.Dispose(disposing);
        }




        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.EndUpdate"]/*' />
        /// <devdoc>
        ///     Reenables redrawing of the combo box. A call to beginUpdate() must be
        ///     balanced by a following call to endUpdate(). Following a call to
        ///     beginUpdate(), any redrawing caused by operations performed on the
        ///     combo box is deferred until the call to endUpdate().
        /// </devdoc>
        public void EndUpdate() {
            updateCount--;
            if (updateCount == 0 && AutoCompleteSource == AutoCompleteSource.ListItems)
            {
                SetAutoComplete(false, false);
            }
            if (EndUpdateInternal()) {
                if (childEdit != null && childEdit.Handle != IntPtr.Zero) {
                    SafeNativeMethods.InvalidateRect(new HandleRef(this, childEdit.Handle), null, false);
                }
                if (childListBox != null && childListBox.Handle != IntPtr.Zero) {
                    SafeNativeMethods.InvalidateRect(new HandleRef(this, childListBox.Handle), null, false);
                }
            }
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.FindString"]/*' />
        /// <devdoc>
        ///     Finds the first item in the combo box that starts with the given string.
        ///     The search is not case sensitive.
        /// </devdoc>
        public int FindString(string s) {
            return FindString(s, -1);
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.FindString1"]/*' />
        /// <devdoc>
        ///     Finds the first item after the given index which starts with the given
        ///     string. The search is not case sensitive.
        /// </devdoc>
        public int FindString(string s, int startIndex) {
            if (s == null) {
                return -1;
            }

            if (itemsCollection == null || itemsCollection.Count == 0) {
                return -1;
            }

            // The last item in the list is still a valid starting point for a search.
            if (startIndex < -1 || startIndex >= itemsCollection.Count) {
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            }

            // Always use the managed FindStringInternal instead of CB_FINDSTRING.
            // The managed version correctly handles Turkish I.
            //
            return FindStringInternal(s, Items, startIndex, false);
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.FindStringExact"]/*' />
        /// <devdoc>
        ///     Finds the first item in the combo box that matches the given string.
        ///     The strings must match exactly, except for differences in casing.
        /// </devdoc>
        public int FindStringExact(string s) {
            return FindStringExact(s, -1, true);
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.FindStringExact1"]/*' />
        /// <devdoc>
        ///     Finds the first item after the given index that matches the given
        ///     string. The strings must match exactly, except for differences in
        ///     casing.
        /// </devdoc>
        public int FindStringExact(string s, int startIndex) {
            return FindStringExact(s, startIndex, true);
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.FindStringExact1"]/*' />
        /// <devdoc>
        ///     Finds the first item after the given index that matches the given
        ///     string. The strings must match exactly, except for differences in
        ///     casing.
        /// </devdoc>
        internal int FindStringExact(string s, int startIndex, bool ignorecase) {
            if (s == null) return -1;

            if (itemsCollection == null || itemsCollection.Count == 0) {
                return -1;
            }

            // The last item in the list is still a valid starting point for a search.
            if (startIndex < -1 || startIndex >= itemsCollection.Count) {
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            }

            // Always use the managed FindStringInternal instead of CB_FINDSTRINGEXACT.
            // The managed version correctly handles Turkish I.
            //
            return FindStringInternal(s, Items, startIndex, true, ignorecase);
        }

        // GetPreferredSize and SetBoundsCore call this method to allow controls to self impose
        // constraints on their size.
        internal override Rectangle ApplyBoundsConstraints(int suggestedX, int suggestedY, int proposedWidth, int proposedHeight) {
            if (DropDownStyle == ComboBoxStyle.DropDown
                || DropDownStyle == ComboBoxStyle.DropDownList) {

                proposedHeight = PreferredHeight;
            }

            return base.ApplyBoundsConstraints(suggestedX,suggestedY, proposedWidth, proposedHeight);
        }

    [SuppressMessage("Microsoft.Portability", "CA1902:AvoidTestingForFloatingPointEquality")]
        protected override void ScaleControl(SizeF factor, BoundsSpecified specified) {
            
            if (factor.Width != 1F && factor.Height != 1F) {
                // we get called on scale before we get a notification that our font has changed.
                // in this case, we need to reset our height cache.
                ResetHeightCache();
            }
            base.ScaleControl(factor, specified);
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.GetItemHeight"]/*' />
        /// <devdoc>
        ///     Returns the height of the given item in an OwnerDrawVariable style
        ///     combo box. This method should not be used for Normal or OwnerDrawFixed
        ///     style combo boxes.
        /// </devdoc>
        public int GetItemHeight(int index) {

            // This function is only relevant for OwnerDrawVariable
            if (DrawMode != DrawMode.OwnerDrawVariable) {
                return ItemHeight;
            }

            if (index < 0 || itemsCollection == null || index >= itemsCollection.Count) {
                throw new ArgumentOutOfRangeException(nameof(index), string.Format(SR.InvalidArgument, "index", (index).ToString(CultureInfo.CurrentCulture)));
            }

            if (IsHandleCreated) {

                int h = unchecked( (int) (long)SendMessage(NativeMethods.CB_GETITEMHEIGHT, index, 0) );
                if (h == -1) {
                    throw new Win32Exception();
                }
                return h;
            }

            return ItemHeight;
        }

        internal override IntPtr InitializeDCForWmCtlColor(IntPtr dc, int msg) {
            if ((msg == NativeMethods.WM_CTLCOLORSTATIC) && !ShouldSerializeBackColor()) {
                // Let the Win32 Edit control handle background colors itself.
                // This is necessary because a disabled edit control will display a different
                // BackColor than when enabled.
                return IntPtr.Zero;
            }
            else if ((msg == NativeMethods.WM_CTLCOLORLISTBOX) && GetStyle(ControlStyles.UserPaint)) {
                // Base class returns hollow brush when UserPaint style is set, to avoid flicker in
                // main control. But when returning colors for child dropdown list, return normal ForeColor/BackColor,
                // since hollow brush leaves the list background unpainted.
                SafeNativeMethods.SetTextColor(new HandleRef(null, dc), ColorTranslator.ToWin32(ForeColor));
                SafeNativeMethods.SetBkColor(new HandleRef(null, dc), ColorTranslator.ToWin32(BackColor));
                return BackColorBrush;
            }
            else {
                return base.InitializeDCForWmCtlColor(dc, msg);
            }
        }

        // Returns true when the key processing needs to be intercepted to allow
        // auto-completion in DropDownList style.
        private bool InterceptAutoCompleteKeystroke(Message m) {
            if (m.Msg == NativeMethods.WM_KEYDOWN) {
                Debug.Assert((ModifierKeys & Keys.Alt) == 0);
                // Keys.Delete only triggers a WM_KEYDOWN and WM_KEYUP, and no WM_CHAR. That's why it's treated separately.
                if ((Keys)unchecked( (int) (long)m.WParam) == Keys.Delete) {
                    // Reset matching text and remove any selection
                    this.MatchingText = "";
                    this.autoCompleteTimeStamp = DateTime.Now.Ticks;
                    if (this.Items.Count > 0) {
                        SelectedIndex = 0;
                    }
                    return false;
                }
            }
            else if (m.Msg == NativeMethods.WM_CHAR) {
                Debug.Assert((ModifierKeys & Keys.Alt) == 0);
                char keyChar = unchecked((char)(long)m.WParam);
                if (keyChar == (char)Keys.Back) {
                    if (DateTime.Now.Ticks - this.autoCompleteTimeStamp > AutoCompleteTimeout ||
                        this.MatchingText.Length <= 1) {
                        // Reset matching text and remove any selection
                        this.MatchingText = "";
                        if (this.Items.Count > 0) {
                            SelectedIndex = 0;
                        }
                    }
                    else {
                        // Remove one character from matching text and rematch
                        this.MatchingText = this.MatchingText.Remove(this.MatchingText.Length - 1);
                        this.SelectedIndex = FindString(this.MatchingText);
                    }
                    this.autoCompleteTimeStamp = DateTime.Now.Ticks;
                    return false;
                }
                else if (keyChar == (char)Keys.Escape) {
                    this.MatchingText = "";
                }

                string newMatchingText;
                if (keyChar != (char)Keys.Escape && keyChar != (char)Keys.Return && !DroppedDown
					&& AutoCompleteMode != AutoCompleteMode.Append) {
                    DroppedDown = true;
                }
                if (DateTime.Now.Ticks - this.autoCompleteTimeStamp > AutoCompleteTimeout) {
                    newMatchingText = new string(keyChar, 1);
                    if (FindString(newMatchingText) != -1) {
                        this.MatchingText = newMatchingText;
                        // Select the found item
                    }
                    this.autoCompleteTimeStamp = DateTime.Now.Ticks;
                    return false;
                }
                else {
                    newMatchingText = this.MatchingText + keyChar;
                    int itemFound = FindString(newMatchingText);
                    if (itemFound != -1) {
                        this.MatchingText = newMatchingText;
                        if (itemFound != this.SelectedIndex) {
                            this.SelectedIndex = itemFound;
                        }
                    }
                    // Do not change the selection
                    this.autoCompleteTimeStamp = DateTime.Now.Ticks;
                    return true;
                }
            }
            return false;
        }

        // Invalidate the entire control, including child HWNDs and non-client areas
        private void InvalidateEverything() {
            SafeNativeMethods.RedrawWindow(new HandleRef(this, Handle),
                                           null, NativeMethods.NullHandleRef,
                                           NativeMethods.RDW_INVALIDATE |
                                           NativeMethods.RDW_FRAME |  // Control.Invalidate(true) doesn't invalidate the non-client region
                                           NativeMethods.RDW_ERASE |
                                           NativeMethods.RDW_ALLCHILDREN);
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.IsInputKey"]/*' />
        /// <devdoc>
        ///     Determines if keyData is in input key that the control wants.
        ///     Overridden to return true for RETURN and ESCAPE when the combo box is
        ///     dropped down.
        /// </devdoc>
        /// <internalonly/>
        protected override bool IsInputKey(Keys keyData) {

            Keys keyCode = keyData & (Keys.KeyCode | Keys.Alt);
            if (keyCode == Keys.Return || keyCode == Keys.Escape) {
                if (this.DroppedDown || autoCompleteDroppedDown) {
                    //old behavior
                    return true;
                }
                else if (SystemAutoCompleteEnabled && ACNativeWindow.AutoCompleteActive) {
                    autoCompleteDroppedDown = true;
                    return true;
                }
            }

            return base.IsInputKey(keyData);
        }

        /// <devdoc>
        ///     Adds the given item to the native combo box.  This asserts if the handle hasn't been
        ///     created.
        /// </devdoc>
        private int NativeAdd(object item) {
            Debug.Assert(IsHandleCreated, "Shouldn't be calling Native methods before the handle is created.");
            int insertIndex = unchecked( (int) (long)SendMessage(NativeMethods.CB_ADDSTRING, 0, GetItemText(item)));
            if (insertIndex < 0) {
                throw new OutOfMemoryException(SR.ComboBoxItemOverflow);
            }
            return insertIndex;
        }

        /// <devdoc>
        ///     Clears the contents of the combo box.
        /// </devdoc>
        private void NativeClear() {
            Debug.Assert(IsHandleCreated, "Shouldn't be calling Native methods before the handle is created.");
            string saved = null;
            if (DropDownStyle != ComboBoxStyle.DropDownList) {
                saved = WindowText;
            }
            SendMessage(NativeMethods.CB_RESETCONTENT, 0, 0);
            if (saved != null) {
                WindowText = saved;
            }
        }

        /// <devdoc>
        ///     Get the text stored by the native control for the specified list item.
        /// </devdoc>
        private string NativeGetItemText(int index) {
            int len = unchecked( (int) (long)SendMessage(NativeMethods.CB_GETLBTEXTLEN, index, 0));
            StringBuilder sb = new StringBuilder(len + 1);
            UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.CB_GETLBTEXT, index, sb);
            return sb.ToString();
        }

        /// <devdoc>
        ///     Inserts the given item to the native combo box at the index.  This asserts if the handle hasn't been
        ///     created or if the resulting insert index doesn't match the passed in index.
        /// </devdoc>
        private int NativeInsert(int index, object item) {
            Debug.Assert(IsHandleCreated, "Shouldn't be calling Native methods before the handle is created.");
            int insertIndex = unchecked( (int) (long)SendMessage(NativeMethods.CB_INSERTSTRING, index, GetItemText(item)));
            if (insertIndex < 0) {
                throw new OutOfMemoryException(SR.ComboBoxItemOverflow);
            }
            Debug.Assert(insertIndex == index, "NativeComboBox inserted at " + insertIndex + " not the requested index of " + index);
            return insertIndex;
        }

        /// <devdoc>
        ///     Removes the native item from the given index.
        /// </devdoc>
        private void NativeRemoveAt(int index) {
            Debug.Assert(IsHandleCreated, "Shouldn't be calling Native methods before the handle is created.");

            // Windows combo does not invalidate the selected region if you remove the
            // currently selected item.  Test for this and invalidate.  Note that because
            // invalidate will lazy-paint we can actually invalidate before we send the
            // delete message.
            //
            if (DropDownStyle == ComboBoxStyle.DropDownList && SelectedIndex == index) {
                Invalidate();
            }

            SendMessage(NativeMethods.CB_DELETESTRING, index, 0);
        }

        internal override void RecreateHandleCore() {
            
            string oldText = WindowText;
            base.RecreateHandleCore();
            if (!String.IsNullOrEmpty(oldText) && String.IsNullOrEmpty(WindowText)) {
                WindowText = oldText;   //restore the window text
            }
        }
        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.CreateHandle"]/*' />
        /// <devdoc>
        ///     Overridden to avoid multiple layouts during handle creation due to combobox size change
        /// </devdoc>
        /// <internalonly/>

        protected override void CreateHandle()
        {
            using (new LayoutTransaction(ParentInternal, this, PropertyNames.Bounds))
            {
                base.CreateHandle();
            }
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.OnHandleCreated"]/*' />
        /// <devdoc>
        ///     Overridden to make sure all the items and styles get set up correctly.
        ///     Inheriting classes should not forget to call
        ///     base.OnHandleCreated()
        /// </devdoc>
        /// <internalonly/>
        protected override void OnHandleCreated(EventArgs e) {

            base.OnHandleCreated(e);

            if (MaxLength > 0) {
                SendMessage(NativeMethods.CB_LIMITTEXT, MaxLength, 0);
            }

            // Get the handles and wndprocs of the ComboBox's child windows
            //
            Debug.Assert(childEdit == null, "Child edit window already attached");
            Debug.Assert(childListBox == null, "Child listbox window already attached");

            bool ok = childEdit == null && childListBox == null;

            if (ok && DropDownStyle != ComboBoxStyle.DropDownList) {
                IntPtr hwnd = UnsafeNativeMethods.GetWindow(new HandleRef(this, Handle), NativeMethods.GW_CHILD);
                if (hwnd != IntPtr.Zero) {

                    // if it's a simple dropdown list, the first HWND is the list box.
                    //                    
                    if (DropDownStyle == ComboBoxStyle.Simple) {
                        childListBox = new ComboBoxChildNativeWindow(this, ChildWindowType.List);
                        childListBox.AssignHandle(hwnd);

                        // get the edits hwnd...
                        //
                        hwnd = UnsafeNativeMethods.GetWindow(new HandleRef(this, hwnd), NativeMethods.GW_HWNDNEXT);
                    }

                    childEdit = new ComboBoxChildNativeWindow(this, ChildWindowType.Edit);
                    childEdit.AssignHandle(hwnd);

                    // set the initial margin for combobox to be zero (this is also done whenever the font is changed).
                    //
                    UnsafeNativeMethods.SendMessage(new HandleRef(this, childEdit.Handle), NativeMethods.EM_SETMARGINS,
                                              NativeMethods.EC_LEFTMARGIN | NativeMethods.EC_RIGHTMARGIN, 0);
                }
            }

            bool found;
            int dropDownWidth = Properties.GetInteger(PropDropDownWidth, out found);
            if (found) {
                SendMessage(NativeMethods.CB_SETDROPPEDWIDTH, dropDownWidth, 0);
            }

            found = false;
            int itemHeight = Properties.GetInteger(PropItemHeight, out found);
            if (found) {
                // someone has set the item height - update it
                UpdateItemHeight();
            }
            // Resize a simple style combobox on handle creation
            // to respect the requested height.
            //
            if (DropDownStyle == ComboBoxStyle.Simple) {
                Height = requestedHeight;
            }

            //If HandleCreated set the AutoComplete...
            //this function checks if the correct properties are set to enable AutoComplete feature on combobox.
            try
            {
                fromHandleCreate = true;
                SetAutoComplete(false, false);
            }
            finally
            {
                fromHandleCreate = false;
            }


            if (itemsCollection != null) {
                foreach (object item in itemsCollection) {
                    NativeAdd(item);
                }

                // Now udpate the current selection.
                //
                if (selectedIndex >= 0) {
                    SendMessage(NativeMethods.CB_SETCURSEL, selectedIndex, 0);
                    UpdateText();
                    selectedIndex = -1;
                }
            }
            // NOTE: Setting SelectedIndex must be the last thing we do.

        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.OnHandleDestroyed"]/*' />
        /// <devdoc>
        ///     We need to un-subclasses everything here.  Inheriting classes should
        ///     not forget to call base.OnHandleDestroyed()
        /// </devdoc>
        /// <internalonly/>
        protected override void OnHandleDestroyed(EventArgs e) {
            dropDownHandle = IntPtr.Zero;
            if (Disposing) {
                itemsCollection = null;
                selectedIndex = -1;
            }
            else {
                selectedIndex = SelectedIndex;
            }
            if (stringSource != null)
            {
                stringSource.ReleaseAutoComplete();
                stringSource = null;
            }
            base.OnHandleDestroyed(e);
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.OnDrawItem"]/*' />
        /// <devdoc>
        ///     This is the code that actually fires the drawItem event.  Don't
        ///     forget to call base.onDrawItem() to ensure that drawItem events
        ///     are correctly fired at all other times.
        /// </devdoc>
        protected virtual void OnDrawItem(DrawItemEventArgs e) {
            DrawItemEventHandler handler = (DrawItemEventHandler)Events[EVENT_DRAWITEM];
            if (handler != null) handler(this, e);
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.OnDropDown"]/*' />
        /// <devdoc>
        ///     This is the code that actually fires the dropDown event.  Don't
        ///     forget to call base.onDropDown() to ensure that dropDown events
        ///     are correctly fired at all other times.
        /// </devdoc>
        protected virtual void OnDropDown(EventArgs e) {
            EventHandler handler = (EventHandler)Events[EVENT_DROPDOWN];
            if (handler != null) handler(this, e);
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.OnKeyDown"]/*' />
        /// <devdoc>
        /// <para>Raises the <see cref='System.Windows.Forms.ComboBox.KeyDown'/> event.</para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void OnKeyDown(KeyEventArgs e) {
            // Do Return/ESC handling
            if (SystemAutoCompleteEnabled) {
                if (e.KeyCode == Keys.Return) {
                    // Set SelectedIndex
                    NotifyAutoComplete(true);
                }
                else if ((e.KeyCode == Keys.Escape) && autoCompleteDroppedDown) {
                    // Fire TextChanged Only
                    NotifyAutoComplete(false);
                }
                autoCompleteDroppedDown = false;
            }

            // Base Handling
            base.OnKeyDown(e);
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.OnKeyPress"]/*' />
        /// <devdoc>
        ///     Key press event handler. Overridden to close up the combo box when the
        ///     user presses RETURN or ESCAPE.
        /// </devdoc>
        /// <internalonly/>
        protected override void OnKeyPress(KeyPressEventArgs e) {

            base.OnKeyPress(e);

            //return when dropped down already fires commit.
            if (!e.Handled && (e.KeyChar == (char)(int)Keys.Return || e.KeyChar == (char)(int)Keys.Escape)
                && DroppedDown) {
                dropDown = false;
                if (FormattingEnabled) {
                    //Set the Text which would Compare the WindowText with the TEXT and change SelectedIndex.
                    Text = WindowText;
                    SelectAll();
                    e.Handled = false;
                }
                else {
                    DroppedDown = false;
                    e.Handled = true;
                }
            }
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.OnMeasureItem"]/*' />
        /// <devdoc>
        ///     This is the code that actually fires the measuereItem event.  Don't
        ///     forget to call base.onMeasureItem() to ensure that measureItem
        ///     events are correctly fired at all other times.
        /// </devdoc>
        protected virtual void OnMeasureItem(MeasureItemEventArgs e) {
            MeasureItemEventHandler handler = (MeasureItemEventHandler)Events[EVENT_MEASUREITEM];
            if (handler != null) handler(this, e);
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.OnMeasureItem"]/*' />
        /// <devdoc>
        ///  If we have the style set to popup show mouse over
        /// </devdoc>
        protected override void OnMouseEnter(EventArgs e) {
            base.OnMouseEnter(e);
            MouseIsOver = true;
        }
        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.OnMeasureItem"]/*' />
        /// <devdoc>
        ///  If we have the style set to popup show mouse over
        /// </devdoc>
        protected override void OnMouseLeave(EventArgs e) {
            base.OnMouseLeave(e);
            MouseIsOver = false;
        }

        /// <devdoc>
        /// This internal helper allows us to call the committed function multiple times without worrying about double firing.
        /// </devdoc>
        private void OnSelectionChangeCommittedInternal(EventArgs e) {
            //There are cases where if we disable the combo while in this event handler, it sends the message again.
            //This is a recursion gaurd to ensure we only send one commit per user action.
            if (allowCommit) {
                try {
                    allowCommit = false;
                    OnSelectionChangeCommitted(e);
                }
                finally {
                    allowCommit = true;
                }
            }
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.OnSelectionChangeCommitted"]/*' />
        /// <devdoc>
        ///     This is the code that actually fires the SelectionChangeCommitted event.
        ///     Don't forget to call base.OnSelectionChangeCommitted() to ensure
        ///     that SelectionChangeCommitted events are correctly fired at all other times.
        /// </devdoc>
        protected virtual void OnSelectionChangeCommitted(EventArgs e) {
            EventHandler handler = (EventHandler)Events[EVENT_SELECTIONCHANGECOMMITTED];
            if (handler != null) handler(this, e);
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.OnSelectedIndexChanged"]/*' />
        /// <devdoc>
        ///     This is the code that actually fires the selectedIndexChanged event.
        ///     Don't forget to call base.onSelectedIndexChanged() to ensure
        ///     that selectedIndexChanged events are correctly fired at all other times.
        /// </devdoc>
        protected override void OnSelectedIndexChanged(EventArgs e) {
            base.OnSelectedIndexChanged(e);
            EventHandler handler = (EventHandler)Events[EVENT_SELECTEDINDEXCHANGED];
            if (handler != null) handler(this, e);

            // set the position in the dataSource, if there is any
            // we will only set the position in the currencyManager if it is different
            // from the SelectedIndex. Setting CurrencyManager::Position (even w/o changing it)
            // calls CurrencyManager::EndCurrentEdit, and that will pull the dataFrom the controls
            // into the backEnd. We do not need to do that.
            //
            // don't change the position if SelectedIndex is -1 because this indicates a selection not from the list.
            if (this.DataManager != null && DataManager.Position != SelectedIndex) {
                //read this as "if everett or   (whidbey and selindex is valid)"
                if (!FormattingEnabled || SelectedIndex != -1) {
                    this.DataManager.Position = this.SelectedIndex;
                }
            }
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.OnSelectedValueChanged"]/*' />
        protected override void OnSelectedValueChanged(EventArgs e) {
            base.OnSelectedValueChanged(e);
            selectedValueChangedFired = true;
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.OnSelectedItemChanged"]/*' />
        /// <devdoc>
        ///     This is the code that actually fires the selectedItemChanged event.
        ///     Don't forget to call base.onSelectedItemChanged() to ensure
        ///     that selectedItemChanged events are correctly fired at all other times.
        /// </devdoc>
        protected virtual void OnSelectedItemChanged(EventArgs e) {
            EventHandler handler = (EventHandler)Events[EVENT_SELECTEDITEMCHANGED];
            if (handler != null) handler(this, e);
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.OnDropDownStyleChanged"]/*' />
        /// <devdoc>
        ///     This is the code that actually fires the DropDownStyleChanged event.
        /// </devdoc>
        protected virtual void OnDropDownStyleChanged(EventArgs e) {
            EventHandler handler = (EventHandler)Events[EVENT_DROPDOWNSTYLE];
            if (handler != null) handler(this, e);
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.OnParentBackColorChanged"]/*' />
        /// <devdoc>
        ///     This method is called by the parent control when any property
        ///     changes on the parent. This can be overriden by inheriting
        ///     classes, however they must call base.OnParentPropertyChanged.
        /// </devdoc>
        protected override void OnParentBackColorChanged(EventArgs e) {
            base.OnParentBackColorChanged(e);
            if (DropDownStyle == ComboBoxStyle.Simple) Invalidate();
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.OnFontChanged"]/*' />
        /// <devdoc>
        ///     Indicates that a critical property, such as color or font has
        ///     changed.
        /// </devdoc>
        /// <internalonly/>
        protected override void OnFontChanged(EventArgs e) {
            base.OnFontChanged(e);
            ResetHeightCache();

            if (this.AutoCompleteMode == AutoCompleteMode.None) {
                UpdateControl(true);
            }
            else {
                //we always will recreate the handle when autocomplete mode is on
                RecreateHandle();
            }
            CommonProperties.xClearPreferredSizeCache(this);
        }


        private void OnAutoCompleteCustomSourceChanged(object sender, CollectionChangeEventArgs e) {
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

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.OnBackColorChanged"]/*' />
        /// <devdoc>
        ///     Indicates that a critical property, such as color or font has
        ///     changed.
        /// </devdoc>
        /// <internalonly/>
        protected override void OnBackColorChanged(EventArgs e) {
            base.OnBackColorChanged(e);
            UpdateControl(false);
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.OnForeColorChanged"]/*' />
        /// <devdoc>
        ///     Indicates that a critical property, such as color or font has
        ///     changed.
        /// </devdoc>
        /// <internalonly/>
        protected override void OnForeColorChanged(EventArgs e) {
            base.OnForeColorChanged(e);
            UpdateControl(false);
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.OnGotFocus"]/*' />
        /// <internalonly/>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void OnGotFocus(EventArgs e) {
            if (!canFireLostFocus) {
                base.OnGotFocus(e);
                canFireLostFocus = true;
            }
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.OnLostFocus"]/*' />
        /// <internalonly/>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void OnLostFocus(EventArgs e) {
            if (canFireLostFocus) {
                if (this.AutoCompleteMode != AutoCompleteMode.None &&
                    this.AutoCompleteSource == AutoCompleteSource.ListItems &&
                    this.DropDownStyle == ComboBoxStyle.DropDownList) {
                    this.MatchingText = "";
                }
                base.OnLostFocus(e);
                canFireLostFocus = false;
            }
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.OnTextChanged"]/*' />
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void OnTextChanged(EventArgs e) {
            if (SystemAutoCompleteEnabled) {
                string text = this.Text;

                // Prevent multiple TextChanges...
                if (text != this.lastTextChangedValue) {
                    // Need to still fire a TextChanged
                    base.OnTextChanged(e);

                    // Save the new value
                    this.lastTextChangedValue = text;
                }
            }
            else {
                // Call the base
                base.OnTextChanged(e);
            }
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.OnValidating"]/*' />
        /// <devdoc>
        /// <para>Raises the <see cref='System.Windows.Forms.ComboBox.Validating'/>
        /// event.</para>
        /// </devdoc>
        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void OnValidating(CancelEventArgs e) {
            if (SystemAutoCompleteEnabled) {
                // Handle AutoComplete notification
                NotifyAutoComplete();
            }

            // Call base
            base.OnValidating(e);
        }


        private void UpdateControl(bool recreate) {
            //clear the pref height cache
            ResetHeightCache();

            if (IsHandleCreated) {
                if (DropDownStyle == ComboBoxStyle.Simple && recreate) {
                    // Control forgets to add a scrollbar.
                    RecreateHandle();
                }
                else {
                    UpdateItemHeight();
                    // Force everything to repaint.
                    InvalidateEverything();
                }
            }
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.OnResize"]/*' />
        /// <devdoc>
        /// <para>Raises the <see cref='System.Windows.Forms.Control.Resize'/> event.</para>
        /// </devdoc>
        protected override void OnResize(EventArgs e) {
            base.OnResize(e);
            if (DropDownStyle == ComboBoxStyle.Simple) {
                // simple style combo boxes have more painting problems than you can shake a stick at
                InvalidateEverything();
            }
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.OnDataSourceChanged"]/*' />
        protected override void OnDataSourceChanged(EventArgs e) {
            if (Sorted) {
                if (DataSource != null && Created) {
                    // we will only throw the exception when the control is already on the form.
                    Debug.Assert(DisplayMember.Equals(String.Empty), "if this list is sorted it means that dataSource was null when Sorted first became true. at that point DisplayMember had to be String.Empty");
                    DataSource = null;
                    throw new InvalidOperationException(SR.ComboBoxDataSourceWithSort);
                }
            }
            if (DataSource == null) {
                BeginUpdate();
                SelectedIndex = -1;
                Items.ClearInternal();
                EndUpdate();
            }
            if (!Sorted && Created)
                base.OnDataSourceChanged(e);
            RefreshItems();
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.OnDisplayMemberChanged"]/*' />
        protected override void OnDisplayMemberChanged(EventArgs e) {
            base.OnDisplayMemberChanged(e);

            // 


            RefreshItems();
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.OnDropDownClosed"]/*' />
        /// <devdoc>
        ///     This event is fired when the dropdown portion of the combobox is hidden.
        /// </devdoc>
        protected virtual void OnDropDownClosed(EventArgs e) {
            EventHandler handler = (EventHandler)Events[EVENT_DROPDOWNCLOSED];
            if (handler != null) handler(this, e);
        }


        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.OnTextUpdate"]/*' />
        /// <devdoc>
        ///     This event is fired when the edit portion of a combobox is about to display altered text.
        ///     This event is NOT fired if the TEXT property is programatically changed.
        /// </devdoc>
        protected virtual void OnTextUpdate(EventArgs e) {
            EventHandler handler = (EventHandler)Events[EVENT_TEXTUPDATE];
            if (handler != null) handler(this, e);
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.ProcessKeyEventArgs"]/*' />
        [
            SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode),
        ]
        protected override bool ProcessKeyEventArgs(ref Message m) {
            if (this.AutoCompleteMode != AutoCompleteMode.None &&
                this.AutoCompleteSource == AutoCompleteSource.ListItems &&
                this.DropDownStyle == ComboBoxStyle.DropDownList &&
                InterceptAutoCompleteKeystroke(m)) {
                return true;
            }
            else {
                return base.ProcessKeyEventArgs(ref m);
            }
        }

        private void ResetHeightCache() {
            prefHeightCache = -1;
        }
        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.RefreshItems"]/*' />
        /// <devdoc>
        /// Reparses the objects, getting new text strings for them.
        /// </devdoc>
        /// <internalonly/>
        protected override void RefreshItems() {

            // Save off the selection and the current collection.
            //
            int selectedIndex = SelectedIndex;
            ObjectCollection savedItems = itemsCollection;

            itemsCollection = null;


            object[] newItems = null;

            // if we have a dataSource and a DisplayMember, then use it
            // to populate the Items collection
            //
            if (this.DataManager != null && this.DataManager.Count != -1) {
                newItems = new object[this.DataManager.Count];
                for (int i = 0; i < newItems.Length; i++) {
                    newItems[i] = this.DataManager[i];
                }
            }
            else if (savedItems != null) {
                newItems = new object[savedItems.Count];
                savedItems.CopyTo(newItems, 0);
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
                if (newItems != null)
                {
                    Items.AddRangeInternal(newItems);
                }
                if (this.DataManager != null)
                {
                    // put the selectedIndex in sync w/ the position in the dataManager
                    this.SelectedIndex = this.DataManager.Position;
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

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.RefreshItem"]/*' />
        /// <devdoc>
        /// Reparses the object at the given index, getting new text string for it.
        /// </devdoc>
        /// <internalonly/>
        protected override void RefreshItem(int index) {
            Items.SetItemInternal(index, Items[index]);
        }

        /// <devdoc>
        ///     Release the ChildWindow object by un-subclassing the child edit and
        ///     list controls and freeing the root of the ChildWindow object.
        /// </devdoc>
        private void ReleaseChildWindow() {

            if (childEdit != null) {

                childEdit.ReleaseHandle();
                childEdit = null;
            }

            if (childListBox != null) {

                childListBox.ReleaseHandle();
                childListBox = null;
            }
        }

        private void ResetAutoCompleteCustomSource() {
            AutoCompleteCustomSource = null;
        }

        private void ResetDropDownWidth() {
            Properties.RemoveInteger(PropDropDownWidth);
        }

        private void ResetItemHeight() {
            Properties.RemoveInteger(PropItemHeight);
        }

        public override void ResetText() {
            base.ResetText();
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.SetAutoComplete"]/*' />
        /// <devdoc>
        ///     Enables the AutoComplete feature for combobox depending on the properties set.
        ///     These properties are namely AutoCompleteMode, AutoCompleteSource and AutoCompleteCustomSource.
        /// </devdoc>
	[SuppressMessage("Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands")]
        private void SetAutoComplete(bool reset, bool recreate) {
            if (!IsHandleCreated || childEdit == null) {
                return;
            }

            if (AutoCompleteMode != AutoCompleteMode.None) {
                if (!fromHandleCreate && recreate && IsHandleCreated)
                {
                    //RecreateHandle to avoid Leak.
                    // notice the use of member variable to avoid re-entrancy
                    AutoCompleteMode backUpMode = this.AutoCompleteMode;
                    autoCompleteMode = AutoCompleteMode.None;
                    RecreateHandle();
                    autoCompleteMode = backUpMode;
                }
                
                if (AutoCompleteSource == AutoCompleteSource.CustomSource) {
                    if (AutoCompleteCustomSource != null) {
                        if (AutoCompleteCustomSource.Count == 0) {
                            int mode = NativeMethods.AUTOSUGGEST_OFF | NativeMethods.AUTOAPPEND_OFF;
                            SafeNativeMethods.SHAutoComplete(new HandleRef(this, childEdit.Handle), mode);
                        }
                        else {

                            if (stringSource == null)
                            {
                                stringSource = new StringSource(GetStringsForAutoComplete(AutoCompleteCustomSource));
                                if (!stringSource.Bind(new HandleRef(this, childEdit.Handle), (int)AutoCompleteMode)) {
                                    throw new ArgumentException(SR.AutoCompleteFailure);
                                }
                            }
                            else
                            {
                                stringSource.RefreshList(GetStringsForAutoComplete(AutoCompleteCustomSource));
                            }
                            
                        }
                    }
                }
                else if (AutoCompleteSource == AutoCompleteSource.ListItems) {
                    if (DropDownStyle != ComboBoxStyle.DropDownList) {
                        if (itemsCollection != null) {
                            if (itemsCollection.Count == 0) {
                                int mode = NativeMethods.AUTOSUGGEST_OFF | NativeMethods.AUTOAPPEND_OFF;
                                SafeNativeMethods.SHAutoComplete(new HandleRef(this, childEdit.Handle), mode);
                            }
                            else {

                                if (stringSource == null)
                                {
                                    stringSource = new StringSource(GetStringsForAutoComplete(Items));
                                    if (!stringSource.Bind(new HandleRef(this, childEdit.Handle), (int)AutoCompleteMode)) {
                                        throw new ArgumentException(SR.AutoCompleteFailureListItems);
                                    }
                                }
                                else
                                {
                                    stringSource.RefreshList(GetStringsForAutoComplete(Items));
                                }
                                
                            }
                        }
                    }
                    else {
                        // Drop Down List special handling
                        Debug.Assert(DropDownStyle == ComboBoxStyle.DropDownList);
                        int mode = NativeMethods.AUTOSUGGEST_OFF | NativeMethods.AUTOAPPEND_OFF;
                        SafeNativeMethods.SHAutoComplete(new HandleRef(this, childEdit.Handle), mode);
                    }
                }
                else {
                    try {
                        int mode = 0;

                        if (AutoCompleteMode == AutoCompleteMode.Suggest) {
                            mode |= NativeMethods.AUTOSUGGEST | NativeMethods.AUTOAPPEND_OFF;
                        }
                        if (AutoCompleteMode == AutoCompleteMode.Append) {
                            mode |= NativeMethods.AUTOAPPEND | NativeMethods.AUTOSUGGEST_OFF;
                        }
                        if (AutoCompleteMode == AutoCompleteMode.SuggestAppend) {
                            mode |= NativeMethods.AUTOSUGGEST;
                            mode |= NativeMethods.AUTOAPPEND;
                        }
                        int ret = SafeNativeMethods.SHAutoComplete(new HandleRef(this, childEdit.Handle), (int)AutoCompleteSource | mode);
                    }
                    catch (SecurityException) {
                        // If we don't have full trust, degrade gracefully. Allow the control to
                        // function without auto-complete. Allow the app to continue running.
                    }
                }
            }
            else if (reset) {
                int mode = NativeMethods.AUTOSUGGEST_OFF | NativeMethods.AUTOAPPEND_OFF;
                SafeNativeMethods.SHAutoComplete(new HandleRef(this, childEdit.Handle), mode);
            }
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.Select"]/*' />
        /// <devdoc>
        ///     Selects the text in the editable portion of the ComboBox at the
        ///     from the given start index to the given end index.
        /// </devdoc>
        public void Select(int start, int length) {
            if (start < 0) {
                throw new ArgumentOutOfRangeException(nameof(start), string.Format(SR.InvalidArgument, "start", start.ToString(CultureInfo.CurrentCulture)));
            }
            // the Length can be negative to support Selecting in the "reverse" direction..
            int end = start + length;

            // but end cannot be negative... this means Length is far negative...
            if (end < 0) {
                throw new ArgumentOutOfRangeException(nameof(length), string.Format(SR.InvalidArgument, "length", length.ToString(CultureInfo.CurrentCulture)));
            }

            SendMessage(NativeMethods.CB_SETEDITSEL, 0, NativeMethods.Util.MAKELPARAM(start, end));
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.SelectAll"]/*' />
        /// <devdoc>
        ///     Selects all the text in the editable portion of the ComboBox.
        /// </devdoc>
        public void SelectAll() {
            Select(0, Int32.MaxValue);
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.SetBoundsCore"]/*' />
        protected override void SetBoundsCore(int x, int y, int width, int height, BoundsSpecified specified) {
            // If we are changing height, store the requested height.
            // Requested height is used if the style is changed to simple.
            // (
            if ((specified & BoundsSpecified.Height) != BoundsSpecified.None) {
                requestedHeight = height;
            }

            base.SetBoundsCore(x, y, width, height, specified);
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.SetItemsCore"]/*' />
        /// <devdoc>
        ///     Performs the work of setting the specified items to the combobox
        /// </devdoc>
        protected override void SetItemsCore(IList value) {
            BeginUpdate();
            Items.ClearInternal();
            Items.AddRangeInternal(value);

            // if the list changed, we want to keep the same selected index
            // CurrencyManager will provide the PositionChanged event
            // it will be provided before changing the list though...
            if (this.DataManager != null) {
                if (this.DataSource is ICurrencyManagerProvider) {
                    this.selectedValueChangedFired = false;
                }

                if (IsHandleCreated) {
                    SendMessage(NativeMethods.CB_SETCURSEL, DataManager.Position, 0);
                }
                else {
                    selectedIndex = DataManager.Position;
                }

                // if set_SelectedIndexChanged did not fire OnSelectedValueChanged
                // then we have to fire it ourselves, cos the list changed anyway
                if (!selectedValueChangedFired) {
                    OnSelectedValueChanged(EventArgs.Empty);
                    selectedValueChangedFired = false;
                }
            }

            EndUpdate();
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.SetItemCore"]/*' />
        protected override void SetItemCore(int index, object value) {
            Items.SetItemInternal(index, value);
        }

        private bool ShouldSerializeAutoCompleteCustomSource() {
            return autoCompleteCustomSource != null && autoCompleteCustomSource.Count > 0;
        }

        internal bool ShouldSerializeDropDownWidth() {
            return (Properties.ContainsInteger(PropDropDownWidth));
        }

        /// <devdoc>
        ///     Indicates whether the itemHeight property should be persisted.
        /// </devdoc>
        internal bool ShouldSerializeItemHeight() {
            return (Properties.ContainsInteger(PropItemHeight));
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.ShouldSerializeText"]/*' />
        /// <devdoc>
        ///     Determines if the Text property needs to be persisted.
        /// </devdoc>
        internal override bool ShouldSerializeText() {
            return SelectedIndex == -1 && base.ShouldSerializeText();
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.ToString"]/*' />
        /// <devdoc>
        ///     Provides some interesting info about this control in String form.
        /// </devdoc>
        /// <internalonly/>
        public override string ToString() {

            string s = base.ToString();
            return s + ", Items.Count: " + ((itemsCollection == null) ? (0).ToString(CultureInfo.CurrentCulture) : itemsCollection.Count.ToString(CultureInfo.CurrentCulture));
        }

        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        private void UpdateDropDownHeight() {
            if (dropDownHandle != IntPtr.Zero) {
                //Now use the DropDownHeight property instead of calculating the Height...
                int height = DropDownHeight;
                if (height == DefaultDropDownHeight) {
                    int itemCount = (itemsCollection == null) ? 0 : itemsCollection.Count;
                    int count = Math.Min(Math.Max(itemCount, 1), maxDropDownItems);
                    height = (ItemHeight * count + 2);
                }
                SafeNativeMethods.SetWindowPos(new HandleRef(this, dropDownHandle), NativeMethods.NullHandleRef, 0, 0, DropDownWidth, height,
                                     NativeMethods.SWP_NOMOVE | NativeMethods.SWP_NOZORDER);
            }
        }

        /// <devdoc>
        ///     Manufactures a MeasureItemEventArgs for each item in the list to simulate
        ///     the combobox requesting the info. This gives the effect of allowing the
        ///     measureitem info to be updated at anytime.
        /// </devdoc>
        /// <internalonly/>
        private void UpdateItemHeight() {
            if (!IsHandleCreated) {
                // if we don't create control here we report item heights incorrectly later on.
                CreateControl();
            }
            if (DrawMode == DrawMode.OwnerDrawFixed) {
                SendMessage(NativeMethods.CB_SETITEMHEIGHT, -1, ItemHeight);
                SendMessage(NativeMethods.CB_SETITEMHEIGHT, 0, ItemHeight);
            }
            else if (DrawMode == DrawMode.OwnerDrawVariable) {
                SendMessage(NativeMethods.CB_SETITEMHEIGHT, -1, ItemHeight);
                Graphics graphics = CreateGraphicsInternal();
                for (int i = 0; i < Items.Count; i++) {
                    int original = unchecked( (int) (long)SendMessage(NativeMethods.CB_GETITEMHEIGHT, i, 0));
                    MeasureItemEventArgs mievent = new MeasureItemEventArgs(graphics, i, original);
                    OnMeasureItem(mievent);
                    if (mievent.ItemHeight != original) {
                        SendMessage(NativeMethods.CB_SETITEMHEIGHT, i, mievent.ItemHeight);
                    }
                }
                graphics.Dispose();
            }
        }

        /// <devdoc>
        ///     Forces the text to be updated based on the current selection.
        /// </devdoc>
        /// <internalonly/>
        private void UpdateText() {
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
            // v1.1 - SelectedIndex == -1 corresponds to Text == null
            string s = null;

            if (SelectedIndex != -1) {
                object item = Items[SelectedIndex];
                if (item != null) {
                    s = GetItemText(item);
                }
            }

            Text = s;

            if (DropDownStyle == ComboBoxStyle.DropDown) {
                if (childEdit != null && childEdit.Handle != IntPtr.Zero) {
                    UnsafeNativeMethods.SendMessage(new HandleRef(this, childEdit.Handle), NativeMethods.WM_SETTEXT, IntPtr.Zero, s);
                }
            }
        }

        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        private void WmEraseBkgnd(ref Message m) {
            if ((DropDownStyle == ComboBoxStyle.Simple) && ParentInternal != null) {
                NativeMethods.RECT rect = new NativeMethods.RECT();
                SafeNativeMethods.GetClientRect(new HandleRef(this, Handle), ref rect);
                Control p = ParentInternal;
                Graphics graphics = Graphics.FromHdcInternal(m.WParam);
                if (p != null) {
                    Brush brush = new SolidBrush(p.BackColor);
                    graphics.FillRectangle(brush, rect.left, rect.top,
                                           rect.right - rect.left, rect.bottom - rect.top);
                    brush.Dispose();
                }
                else {
                    graphics.FillRectangle(SystemBrushes.Control, rect.left, rect.top,
                                           rect.right - rect.left, rect.bottom - rect.top);
                }
                graphics.Dispose();
                m.Result = (IntPtr)1;
                return;
            }
            base.WndProc(ref m);
        }

        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        private void WmParentNotify(ref Message m) {
            base.WndProc(ref m);
            if (unchecked((int)(long)m.WParam) == (NativeMethods.WM_CREATE | 1000 << 16)) {
                dropDownHandle = m.LParam;
            }
        }

        /// <summary>
        /// Text change behavior.
        /// Here are the window messages corresponding to each user event.
        ///
        /// DropDown (free text window):
        ///   Type in Text Window:
        ///             CBN_EDITUPDATE
        ///             CBN_EDITCHANGE
        ///   Down/Up Arrow
        ///             CBN_SELENDOK   (text not changed yet)
        ///             CBN_SELCHANGE
        ///   with text set to beginning of a valid item -- drop and close dropdown (selects that item)
        ///             CBN_DROPDOWN
        ///             CBN_CLOSEUP
        ///   Drop List, up/down arrow to select item, click away
        ///             CBN_DROPDOWN
        ///             CBN_SELCHANGE
        ///             CBN_CLOSEUP
        ///   Drop List, click on item
        ///             CBN_DROPDOWN
        ///             CBN_SELENDOK
        ///             CBN_CLOSEUP
        ///             CBN_SELCHANGE (text changes here via selected item)
        ///
        /// DropDownList (limited text window):
        ///
        ///   Type text and arrow up/down:
        ///             CBN_SELENDOK  (text already changed)
        ///             CBN_SELCHANGE
        ///   Drop List, up/down arrow to select item, click away
        ///             CBN_DROPDOWN
        ///             CBN_SELCHANGE
        ///             CBN_CLOSEUP
        ///   Drop List, click on item
        ///             CBN_DROPDOWN
        ///             CBN_SELENDOK
        ///             CBN_CLOSEUP
        ///             CBN_SELCHANGE
        ///
        /// Simple (listbox visible):
        ///   Type in Text Window:
        ///             CBN_EDITUPDATE
        ///             CBN_EDITCHANGE
        ///   Down/Up Arrow
        ///             CBN_SELENDOK    (text not changed yet)
        ///             CBN_SELCHANGE
        ///   Click on item
        ///             CBN_SELENDOK    (text not changed yet)
        ///             CBN_SELCHANGE
        ///
        ///
        /// What we do is fire textchange events in these messages:
        ///   CBN_SELCHANGE
        ///   CBN_EDITCHANGE
        ///   CBN_CLOSEUP
        ///
        /// and we only actually call the real event if the Text is different than currentText.
        /// currentText is never changed outside this method.
        /// This internal version can be called from anywhere we might suspect text has changed
        /// it's fairly safe to call anywhere.

        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        private void WmReflectCommand(ref Message m) {
            switch (NativeMethods.Util.HIWORD(m.WParam)) {
                case NativeMethods.CBN_DBLCLK:
                    //OnDoubleClick(EventArgs.Empty);
                    break;
                case NativeMethods.CBN_EDITUPDATE:
                    OnTextUpdate(EventArgs.Empty);
                    break;
                case NativeMethods.CBN_CLOSEUP:
                    
                    OnDropDownClosed(EventArgs.Empty);
                    if (FormattingEnabled && Text != currentText && dropDown) {
                        OnTextChanged(EventArgs.Empty);
                    }
                    dropDown = false;
                    break;
                case NativeMethods.CBN_DROPDOWN:
                    currentText = Text;
                    dropDown = true;
                    OnDropDown(EventArgs.Empty);
                    UpdateDropDownHeight();
                    
                    break;
                case NativeMethods.CBN_EDITCHANGE:
                    OnTextChanged(EventArgs.Empty);
                    break;
                case NativeMethods.CBN_SELCHANGE:
                    UpdateText();
                    OnSelectedIndexChanged(EventArgs.Empty);
                    break;
                case NativeMethods.CBN_SELENDOK:
                    OnSelectionChangeCommittedInternal(EventArgs.Empty);
                    break;
            }
        }

        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        private void WmReflectDrawItem(ref Message m) {
            NativeMethods.DRAWITEMSTRUCT dis = (NativeMethods.DRAWITEMSTRUCT)m.GetLParam(typeof(NativeMethods.DRAWITEMSTRUCT));
            IntPtr oldPal = SetUpPalette(dis.hDC, false /*force*/, false /*realize*/);
            try {
                Graphics g = Graphics.FromHdcInternal(dis.hDC);

                try {
                    OnDrawItem(new DrawItemEventArgs(g, Font, Rectangle.FromLTRB(dis.rcItem.left, dis.rcItem.top, dis.rcItem.right, dis.rcItem.bottom),
                                                     dis.itemID, (DrawItemState)dis.itemState, ForeColor, BackColor));
                }
                finally {
                    g.Dispose();
                }
            }
            finally {
                if (oldPal != IntPtr.Zero) {
                    SafeNativeMethods.SelectPalette(new HandleRef(this, dis.hDC), new HandleRef(null, oldPal), 0);
                }
            }
            m.Result = (IntPtr)1;
        }

        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        private void WmReflectMeasureItem(ref Message m) {
            NativeMethods.MEASUREITEMSTRUCT mis = (NativeMethods.MEASUREITEMSTRUCT)m.GetLParam(typeof(NativeMethods.MEASUREITEMSTRUCT));

            // Determine if message was sent by a combo item or the combo edit field
            if (DrawMode == DrawMode.OwnerDrawVariable && mis.itemID >= 0) {
                Graphics graphics = CreateGraphicsInternal();
                MeasureItemEventArgs mie = new MeasureItemEventArgs(graphics, mis.itemID, ItemHeight);
                OnMeasureItem(mie);
                mis.itemHeight = mie.ItemHeight;
                graphics.Dispose();
            }
            else {
                // Message was sent by the combo edit field
                mis.itemHeight = ItemHeight;
            }
            Marshal.StructureToPtr(mis, m.LParam, false);
            m.Result = (IntPtr)1;
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.WndProc"]/*' />
        /// <devdoc>
        ///     The comboboxs window procedure.  Inheritng classes can override this
        ///     to add extra functionality, but should not forget to call
        ///     base.wndProc(m); to ensure the combo continues to function properly.
        /// </devdoc>
        /// <internalonly/>
        [SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.UnmanagedCode)]
        protected override void WndProc(ref Message m) {
            switch (m.Msg) {
                // We don't want to fire the focus events twice -
                // once in the combobox and once in the ChildWndProc.
                case NativeMethods.WM_SETFOCUS:
                    try {
                        fireSetFocus = false;
                        base.WndProc(ref m);
                    }
                        
                    finally {
                        fireSetFocus = true;
                    }
                    break;
                case NativeMethods.WM_KILLFOCUS:
                    try {
                        fireLostFocus = false;
                        base.WndProc(ref m);
                        // Nothing to see here... Just keep on walking...
                        // Turns out that with Theming off, we don't get quite the same messages as with theming on.
                        
                        // With theming on we get a WM_MOUSELEAVE after a WM_KILLFOCUS even if you use the Tab key
                        // to move focus. Our response to WM_MOUSELEAVE causes us to repaint everything correctly.

                        // With theming off, we do not get a WM_MOUSELEAVE after a WM_KILLFOCUS, and since we don't have a childwndproc
                        // when we are a Flat DropDownList, we need to force a repaint. The easiest way to do this is to send a 
                        // WM_MOUSELEAVE to ourselves, since that also sets up the right state. Or... at least the state is the same
                        // as with Theming on.
                        
                        if (!Application.RenderWithVisualStyles && GetStyle(ControlStyles.UserPaint) == false && this.DropDownStyle == ComboBoxStyle.DropDownList && (FlatStyle == FlatStyle.Flat || FlatStyle == FlatStyle.Popup)) {
                            UnsafeNativeMethods.PostMessage(new HandleRef(this, Handle), NativeMethods.WM_MOUSELEAVE, 0, 0);                            
                        }
                    }

                    finally {
                        fireLostFocus = true;
                    }
                    break;
                case NativeMethods.WM_CTLCOLOREDIT:
                case NativeMethods.WM_CTLCOLORLISTBOX:
                    m.Result = InitializeDCForWmCtlColor(m.WParam, m.Msg);
                    break;
                case NativeMethods.WM_ERASEBKGND:
                    WmEraseBkgnd(ref m);
                    break;
                case NativeMethods.WM_PARENTNOTIFY:
                    WmParentNotify(ref m);
                    break;
                case NativeMethods.WM_REFLECT + NativeMethods.WM_COMMAND:
                    WmReflectCommand(ref m);
                    break;
                case NativeMethods.WM_REFLECT + NativeMethods.WM_DRAWITEM:
                    WmReflectDrawItem(ref m);
                    break;
                case NativeMethods.WM_REFLECT + NativeMethods.WM_MEASUREITEM:
                    WmReflectMeasureItem(ref m);
                    break;
                case NativeMethods.WM_LBUTTONDOWN:
                    mouseEvents = true;
                    base.WndProc(ref m);
                    break;
                case NativeMethods.WM_LBUTTONUP:
                    // Get the mouse location
                    //
                    NativeMethods.RECT r = new NativeMethods.RECT();
                    UnsafeNativeMethods.GetWindowRect(new HandleRef(this, Handle), ref r);
                    Rectangle ClientRect = new Rectangle(r.left, r.top, r.right - r.left, r.bottom - r.top);

                    int x = NativeMethods.Util.SignedLOWORD(m.LParam);
                    int y = NativeMethods.Util.SignedHIWORD(m.LParam);
                    Point pt = new Point(x, y);
                    pt = PointToScreen(pt);
                    //mouseEvents is used to keep the check that we get the WM_LBUTTONUP after
                    //WM_LBUTTONDOWN or WM_LBUTTONDBLBCLK
                    // combo box gets a WM_LBUTTONUP for focus change ...
                    //
                    if (mouseEvents && !ValidationCancelled) {
                        mouseEvents = false;
                        bool captured = Capture;
                        if (captured && ClientRect.Contains(pt)) {
                            OnClick(new MouseEventArgs(MouseButtons.Left, 1, NativeMethods.Util.SignedLOWORD(m.LParam), NativeMethods.Util.SignedHIWORD(m.LParam), 0));
                            OnMouseClick(new MouseEventArgs(MouseButtons.Left, 1, NativeMethods.Util.SignedLOWORD(m.LParam), NativeMethods.Util.SignedHIWORD(m.LParam), 0));

                        }
                        base.WndProc(ref m);
                    }
                    else {
                        CaptureInternal = false;
                        DefWndProc(ref m);
                    }
                    break;

                case NativeMethods.WM_MOUSELEAVE:
                    DefWndProc(ref m);
                    OnMouseLeaveInternal(EventArgs.Empty);
                    break;

                case NativeMethods.WM_PAINT:
                    if (GetStyle(ControlStyles.UserPaint) == false && (FlatStyle == FlatStyle.Flat || FlatStyle == FlatStyle.Popup)) {

                        using (WindowsRegion dr = new WindowsRegion(FlatComboBoxAdapter.dropDownRect)) {
                            using (WindowsRegion wr = new WindowsRegion(this.Bounds)) {

                                // Stash off the region we have to update (the base is going to clear this off in BeginPaint)
                                NativeMethods.RegionFlags updateRegionFlags = (NativeMethods.RegionFlags)SafeNativeMethods.GetUpdateRgn(new HandleRef(this, this.Handle), new HandleRef(this, wr.HRegion), true);

                                dr.CombineRegion(wr, dr, RegionCombineMode.DIFF);

                                Rectangle updateRegionBoundingRect = wr.ToRectangle();
                                FlatComboBoxAdapter.ValidateOwnerDrawRegions(this, updateRegionBoundingRect);
                                // Call the base class to do its painting (with a clipped DC).

                                NativeMethods.PAINTSTRUCT ps = new NativeMethods.PAINTSTRUCT();
                                IntPtr dc;
                                bool disposeDc = false;
                                if (m.WParam == IntPtr.Zero) {
                                    dc = UnsafeNativeMethods.BeginPaint(new HandleRef(this, Handle), ref ps);
                                    disposeDc = true;
                                }
                                else {
                                    dc = m.WParam;
                                }

                                using (DeviceContext mDC = DeviceContext.FromHdc(dc)) {
                                    using (WindowsGraphics wg = new WindowsGraphics(mDC)) {
                                        if (updateRegionFlags != NativeMethods.RegionFlags.ERROR) {
                                            wg.DeviceContext.SetClip(dr);
                                        }
                                        m.WParam = dc;
                                        DefWndProc(ref m);
                                        if (updateRegionFlags != NativeMethods.RegionFlags.ERROR) {
                                            wg.DeviceContext.SetClip(wr);
                                        }
                                        using (Graphics g = Graphics.FromHdcInternal(dc)) {
                                            FlatComboBoxAdapter.DrawFlatCombo(this, g);
                                        }
                                    }
                                }

                                if (disposeDc) {
                                    UnsafeNativeMethods.EndPaint(new HandleRef(this, Handle), ref ps);
                                }

                            }
                            return;
                        }
                    }
                    
                    base.WndProc(ref m);
                    break;
                case NativeMethods.WM_PRINTCLIENT: 
                    // all the fancy stuff we do in OnPaint has to happen again in OnPrint.
                    if (GetStyle(ControlStyles.UserPaint) == false && FlatStyle == FlatStyle.Flat || FlatStyle == FlatStyle.Popup) {
                        DefWndProc(ref m);
                        
                        if ((unchecked( (int) (long)m.LParam) & NativeMethods.PRF_CLIENT) == NativeMethods.PRF_CLIENT) { 
                            if (GetStyle(ControlStyles.UserPaint) == false && FlatStyle == FlatStyle.Flat || FlatStyle == FlatStyle.Popup) {
                                using (Graphics g = Graphics.FromHdcInternal(m.WParam)) {
                                    FlatComboBoxAdapter.DrawFlatCombo(this,g);
                                }
                            }
                            return;
                        }
                    }
                    base.WndProc(ref m);
                    return;                    
                case NativeMethods.WM_SETCURSOR:
                    base.WndProc(ref m);
                    break;

                case NativeMethods.WM_SETFONT:
                    //(
                    if (Width == 0) {
                        suppressNextWindosPos = true;
                    }
                    base.WndProc(ref m);
                    break;


                case NativeMethods.WM_WINDOWPOSCHANGED:
                    if (!suppressNextWindosPos) {
                        base.WndProc(ref m);
                    }
                    suppressNextWindosPos = false;
                    break;

                case NativeMethods.WM_NCDESTROY:
                    base.WndProc(ref m);
                    ReleaseChildWindow();
                    break;
                    
                default:
                    if (m.Msg == NativeMethods.WM_MOUSEENTER) {
                        DefWndProc(ref m);
                        OnMouseEnterInternal(EventArgs.Empty);
                        break;
                    }
                    base.WndProc(ref m);
                    break;
            }
        }

        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        private class ComboBoxChildNativeWindow : NativeWindow {
            private ComboBox _owner;
            private InternalAccessibleObject _accessibilityObject;
            private ChildWindowType _childWindowType;

            internal ComboBoxChildNativeWindow(ComboBox comboBox, ChildWindowType childWindowType) {
                _owner = comboBox;
                _childWindowType = childWindowType;
            }

            protected override void WndProc(ref Message m) {
                switch (m.Msg) {
                    case NativeMethods.WM_GETOBJECT:
                        WmGetObject(ref m);
                        return;
                    default:
                        _owner.ChildWndProc(ref m);
                        break;
                }
            }

            private ChildAccessibleObject GetChildAccessibleObject(ChildWindowType childWindowType) {
                if (childWindowType == ChildWindowType.Edit) {
                    return _owner.ChildEditAccessibleObject;
                }

                return new ChildAccessibleObject(_owner, Handle);
            }

            private void WmGetObject(ref Message m) {

                if (AccessibilityImprovements.Level3 && m.LParam == (IntPtr)NativeMethods.UiaRootObjectId &&
                    _childWindowType != ChildWindowType.List) { // Do not use UIA for the list at this moment.
                    AccessibleObject uiaProvider = GetChildAccessibleObject(_childWindowType);

                    // If the requested object identifier is UiaRootObjectId, 
                    // we should return an UI Automation provider using the UiaReturnRawElementProvider function.
                    InternalAccessibleObject internalAccessibleObject;
                    IntSecurity.UnmanagedCode.Assert();
                    try
                    {
                        internalAccessibleObject = new InternalAccessibleObject(uiaProvider);
                    }
                    finally
                    {
                        CodeAccessPermission.RevertAssert();
                    }
                    m.Result = UnsafeNativeMethods.UiaReturnRawElementProvider(
                        new HandleRef(this, Handle),
                        m.WParam,
                        m.LParam,
                        internalAccessibleObject);

                    return;
                }

                // See "How to Handle WM_GETOBJECT" in MSDN
                //
                if (NativeMethods.OBJID_CLIENT == unchecked((int)(long)m.LParam)) {

                    // Get the IAccessible GUID
                    //
                    Guid IID_IAccessible = new Guid(NativeMethods.uuid_IAccessible);

                    // Get an Lresult for the accessibility Object for this control
                    //
                    IntPtr punkAcc;
                    try {
                        AccessibleObject wfAccessibleObject = null;
                        UnsafeNativeMethods.IAccessibleInternal iacc = null;

                        if (_accessibilityObject == null) {
                            IntSecurity.UnmanagedCode.Assert();
                            try {
                                wfAccessibleObject = AccessibilityImprovements.Level3
                                    ? GetChildAccessibleObject(_childWindowType)
                                    : new ChildAccessibleObject(_owner, Handle);
                                _accessibilityObject = new InternalAccessibleObject(wfAccessibleObject);
                            }
                            finally {
                                CodeAccessPermission.RevertAssert();
                            }
                        }
                        iacc = (UnsafeNativeMethods.IAccessibleInternal)_accessibilityObject;

                        // Obtain the Lresult
                        //
                        punkAcc = Marshal.GetIUnknownForObject(iacc);

                        IntSecurity.UnmanagedCode.Assert();
                        try {
                            m.Result = UnsafeNativeMethods.LresultFromObject(ref IID_IAccessible, m.WParam, new HandleRef(this, punkAcc));
                        }
                        finally {
                            CodeAccessPermission.RevertAssert();
                            Marshal.Release(punkAcc);
                        }
                    }
                    catch (Exception e) {
                        throw new InvalidOperationException(SR.RichControlLresult, e);
                    }
                }
                else {  // m.lparam != OBJID_CLIENT, so do default message processing
                    DefWndProc(ref m);
                }
            }

        }

        private sealed class ItemComparer : System.Collections.IComparer {
            private ComboBox comboBox;

            public ItemComparer(ComboBox comboBox) {
                this.comboBox = comboBox;
            }

            public int Compare(object item1, object item2) {
                if (item1 == null) {
                    if (item2 == null)
                        return 0; //both null, then they are equal

                    return -1; //item1 is null, but item2 is valid (greater)
                }
                if (item2 == null)
                    return 1; //item2 is null, so item 1 is greater

                String itemName1 = comboBox.GetItemText(item1);
                String itemName2 = comboBox.GetItemText(item2);

                CompareInfo compInfo = (Application.CurrentCulture).CompareInfo;
                return compInfo.Compare(itemName1, itemName2, CompareOptions.StringSort);
            }
        }

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.ObjectCollection"]/*' />
        [ListBindable(false)]
        public class ObjectCollection : IList {

            private ComboBox owner;
            private ArrayList innerList;
            private IComparer comparer;

            /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.ObjectCollection.ObjectCollection"]/*' />
            public ObjectCollection(ComboBox owner) {
                this.owner = owner;
            }

            private IComparer Comparer {
                get {
                    if (comparer == null) {
                        comparer = new ItemComparer(owner);
                    }
                    return comparer;
                }
            }

            private ArrayList InnerList {
                get {
                    if (innerList == null) {
                        innerList = new ArrayList();
                    }
                    return innerList;
                }
            }

            /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.ObjectCollection.Count"]/*' />
            /// <devdoc>
            ///     Retrieves the number of items.
            /// </devdoc>
            public int Count {
                get {
                    return InnerList.Count;
                }
            }

            /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ObjectCollection.ICollection.SyncRoot"]/*' />
            /// <internalonly/>
            object ICollection.SyncRoot {
                get {
                    return this;
                }
            }

            /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ObjectCollection.ICollection.IsSynchronized"]/*' />
            /// <internalonly/>
            bool ICollection.IsSynchronized {
                get {
                    return false;
                }
            }

            /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ObjectCollection.IList.IsFixedSize"]/*' />
            /// <internalonly/>
            bool IList.IsFixedSize {
                get {
                    return false;
                }
            }

            /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.ObjectCollection.IsReadOnly"]/*' />
            public bool IsReadOnly {
                get {
                    return false;
                }
            }

            /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.ObjectCollection.Add"]/*' />
            /// <devdoc>
            ///     Adds an item to the combo box. For an unsorted combo box, the item is
            ///     added to the end of the existing list of items. For a sorted combo box,
            ///     the item is inserted into the list according to its sorted position.
            ///     The item's toString() method is called to obtain the string that is
            ///     displayed in the combo box.
            ///     A SystemException occurs if there is insufficient space available to
            ///     store the new item.
            /// </devdoc>

            public int Add(object item)
            {
                owner.CheckNoDataSource();
                int index = AddInternal(item);
                if (owner.UpdateNeeded() && owner.AutoCompleteSource == AutoCompleteSource.ListItems)
                {
                    owner.SetAutoComplete(false, false);
                }
                return index;
            }

            
            private int AddInternal(object item) {
                
                if (item == null) {
                    throw new ArgumentNullException(nameof(item));
                }
                int index = -1;
                if (!owner.sorted)
                {
                    InnerList.Add(item);
                }
                else
                {
                    index = InnerList.BinarySearch(item, Comparer);
                    if (index < 0)
                    {
                        index = ~index; // getting the index of the first element that is larger than the search value
                    }

                    Debug.Assert(index>=0 && index <= InnerList.Count, "Wrong index for insert");
                    InnerList.Insert(index, item);
                }
                bool successful = false;

                try {
                    if (owner.sorted) {
                        if (owner.IsHandleCreated) {
                            owner.NativeInsert(index, item);
                        }
                    }
                    else {
                        index = InnerList.Count-1;
                        if (owner.IsHandleCreated) {
                            owner.NativeAdd(item);
                        }
                    }
                    successful = true;
                }
                finally {
                    if (!successful) {
                        InnerList.Remove(item);
                    }
                }

                return index;
            }

            /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ObjectCollection.IList.Add"]/*' />
            /// <internalonly/>
            int IList.Add(object item) {
                return Add(item);
            }

            /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.ObjectCollection.AddRange"]/*' />
            public void AddRange(object[] items) {
                owner.CheckNoDataSource();
                owner.BeginUpdate();
                try
                {
                    AddRangeInternal(items);
                }
                finally
                {
                    owner.EndUpdate();
                }
            }
           
            internal void AddRangeInternal(IList items) {

                if (items == null)
                {
                    throw new ArgumentNullException(nameof(items));
                }
                foreach (object item in items) {
                    // adding items one-by-one for performance (especially for sorted combobox)
                    // we can not rely on ArrayList.Sort since its worst case complexity is n*n
                    // AddInternal is based on BinarySearch and ensures n*log(n) complexity
                    AddInternal(item);
                }
                if (owner.AutoCompleteSource == AutoCompleteSource.ListItems)
                {
                    owner.SetAutoComplete(false, false);
                }
            }

            /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.ObjectCollection.this"]/*' />
            /// <devdoc>
            ///     Retrieves the item with the specified index.
            /// </devdoc>
            [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public virtual object this[int index] {
                get {
                    if (index < 0 || index >= InnerList.Count) {
                        throw new ArgumentOutOfRangeException(nameof(index), string.Format(SR.InvalidArgument, "index", (index).ToString(CultureInfo.CurrentCulture)));
                    }

                    return InnerList[index];
                }
                set {
                    owner.CheckNoDataSource();
                    SetItemInternal(index, value);
                }
            }

            /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.ObjectCollection.Clear"]/*' />
            /// <devdoc>
            ///     Removes all items from the ComboBox.
            /// </devdoc>
            public void Clear() {
                owner.CheckNoDataSource();
                ClearInternal();
            }

            internal void ClearInternal() {

                if (owner.IsHandleCreated) {
                    owner.NativeClear();
                }

                InnerList.Clear();
                owner.selectedIndex = -1;
                if (owner.AutoCompleteSource == AutoCompleteSource.ListItems)
                {
                    owner.SetAutoComplete(false, true /*recreateHandle*/);
                }
            }

            /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.ObjectCollection.Contains"]/*' />
            public bool Contains(object value) {
                return IndexOf(value) != -1;
            }

            /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.ObjectCollection.CopyTo"]/*' />
            /// <devdoc>
            ///     Copies the ComboBox Items collection to a destination array.
            /// </devdoc>
            public void CopyTo(object[] destination, int arrayIndex) {
                InnerList.CopyTo(destination, arrayIndex);
            }

            /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ObjectCollection.ICollection.CopyTo"]/*' />
            /// <internalonly/>
            void ICollection.CopyTo(Array destination, int index) {
                InnerList.CopyTo(destination, index);
            }

            /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.ObjectCollection.GetEnumerator"]/*' />
            /// <devdoc>
            ///     Returns an enumerator for the ComboBox Items collection.
            /// </devdoc>
            public IEnumerator GetEnumerator() {
                return InnerList.GetEnumerator();
            }

            /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.ObjectCollection.IndexOf"]/*' />
            public int IndexOf(object value) {
                if (value == null) {
                    throw new ArgumentNullException(nameof(value));
                }

                return InnerList.IndexOf(value);
            }

            /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.ObjectCollection.Insert"]/*' />
            /// <devdoc>
            ///     Adds an item to the combo box. For an unsorted combo box, the item is
            ///     added to the end of the existing list of items. For a sorted combo box,
            ///     the item is inserted into the list according to its sorted position.
            ///     The item's toString() method is called to obtain the string that is
            ///     displayed in the combo box.
            ///     A SystemException occurs if there is insufficient space available to
            ///     store the new item.
            /// </devdoc>
            public void Insert(int index, object item) {
                owner.CheckNoDataSource();

                if (item == null) {
                    throw new ArgumentNullException(nameof(item));
                }

                if (index < 0 || index > InnerList.Count) {
                    throw new ArgumentOutOfRangeException(nameof(index), string.Format(SR.InvalidArgument, "index", (index).ToString(CultureInfo.CurrentCulture)));
                }

                // If the combo box is sorted, then nust treat this like an add
                // because we are going to twiddle the index anyway.
                //
                if (owner.sorted) {
                    Add(item);
                }
                else {
                    InnerList.Insert(index, item);
                    if (owner.IsHandleCreated) {

                        bool successful = false;

                        try {
                            owner.NativeInsert(index, item);
                            successful = true;
                        }
                        finally {
                            if (successful) {
                                if (owner.AutoCompleteSource == AutoCompleteSource.ListItems)
                                {
                                    owner.SetAutoComplete(false, false);
                                }
                            }
                            else {
                                InnerList.RemoveAt(index);
                            }
                        }
                    }
                }
            }

            /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.ObjectCollection.RemoveAt"]/*' />
            /// <devdoc>
            ///     Removes an item from the ComboBox at the given index.
            /// </devdoc>
            public void RemoveAt(int index) {
                owner.CheckNoDataSource();

                if (index < 0 || index >= InnerList.Count) {
                    throw new ArgumentOutOfRangeException(nameof(index), string.Format(SR.InvalidArgument, "index", (index).ToString(CultureInfo.CurrentCulture)));
                }

                if (owner.IsHandleCreated) {
                    owner.NativeRemoveAt(index);
                }

                InnerList.RemoveAt(index);
                if (!owner.IsHandleCreated && index < owner.selectedIndex) {
                    owner.selectedIndex--;
                }
                if (owner.AutoCompleteSource == AutoCompleteSource.ListItems)
                {
                    owner.SetAutoComplete(false, false);
                }
            }

            /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.ObjectCollection.Remove"]/*' />
            /// <devdoc>
            ///     Removes the given item from the ComboBox, provided that it is
            ///     actually in the list.
            /// </devdoc>
            public void Remove(object value) {

                int index = InnerList.IndexOf(value);

                if (index != -1) {
                    RemoveAt(index);
                }
            }

            internal void SetItemInternal(int index, object value) {
                if (value == null) {
                    throw new ArgumentNullException(nameof(value));
                }

                if (index < 0 || index >= InnerList.Count) {
                    throw new ArgumentOutOfRangeException(nameof(index), string.Format(SR.InvalidArgument, "index", (index).ToString(CultureInfo.CurrentCulture)));
                }

                InnerList[index] = value;

                // If the native control has been created, and the display text of the new list item object
                // is different to the current text in the native list item, recreate the native list item...
                if (owner.IsHandleCreated) {
                    bool selected = (index == owner.SelectedIndex);

                    if (String.Compare(this.owner.GetItemText(value), this.owner.NativeGetItemText(index), true, CultureInfo.CurrentCulture) != 0) {
                        owner.NativeRemoveAt(index);
                        owner.NativeInsert(index, value);
                        if (selected) {
                            owner.SelectedIndex = index;
                            owner.UpdateText();
                        }
                        if (owner.AutoCompleteSource == AutoCompleteSource.ListItems)
                        {
                            owner.SetAutoComplete(false, false);
                        }
                    }
                    else {
                        // NEW - FOR COMPATIBILITY REASONS
                        // Minimum compatibility fix
                        if (selected) {
                            owner.OnSelectedItemChanged(EventArgs.Empty);   //we do this because set_SelectedIndex does this. (for consistency)
                            owner.OnSelectedIndexChanged(EventArgs.Empty);
                        }
                    }
                }
            }

        } // end ObjectCollection

        /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ChildAccessibleObject"]/*' />
        /// <internalonly/>
        [ComVisible(true)]
        public class ChildAccessibleObject : AccessibleObject {

            ComboBox owner;

            /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ChildAccessibleObject.ChildAccessibleObject"]/*' />
            [SecurityPermission(SecurityAction.Demand, Flags = SecurityPermissionFlag.UnmanagedCode)]
            public ChildAccessibleObject(ComboBox owner, IntPtr handle) {
                Debug.Assert(owner != null && owner.Handle != IntPtr.Zero, "ComboBox's handle hasn't been created");

                this.owner = owner;
                UseStdAccessibleObjects(handle);
            }

            /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ChildAccessibleObject.Name"]/*' />
            public override string Name {
                get {
                    return owner.AccessibilityObject.Name;
                }
            }
        }

        /// <devdoc>
        /// </devdoc>
        [System.Runtime.InteropServices.ComVisible(true)]
        internal class ComboBoxAccessibleObject : ControlAccessibleObject {

            private const int COMBOBOX_ACC_ITEM_INDEX = 1;

            public ComboBoxAccessibleObject(Control ownerControl)
                : base(ownerControl) {
            }

            internal override string get_accNameInternal(object childID) {
                this.ValidateChildID(ref childID);

                if (childID != null && ((int)childID) == COMBOBOX_ACC_ITEM_INDEX) {
                    return this.Name;
                }
                else {
                    return base.get_accNameInternal(childID);
                }
            }

            internal override string get_accKeyboardShortcutInternal(object childID) {
                this.ValidateChildID(ref childID);
                if (childID != null && ((int)childID) == COMBOBOX_ACC_ITEM_INDEX) {
                    return this.KeyboardShortcut;
                } else {
                    return base.get_accKeyboardShortcutInternal(childID);
                }
            }
        }

        /// <devdoc>
        /// </devdoc>
        [ComVisible(true)]
        internal class ComboBoxExAccessibleObject : ComboBoxAccessibleObject {

            private ComboBox ownerItem = null;

            private void ComboBoxDefaultAction(bool expand) {
                if (ownerItem.DroppedDown != expand) {
                    ownerItem.DroppedDown = expand;
                }
            }

            public ComboBoxExAccessibleObject(ComboBox ownerControl)
                : base(ownerControl) {
                    ownerItem = ownerControl;
            }

            internal override bool IsIAccessibleExSupported() {
                if (ownerItem != null) {
                    return true;
                }
                return base.IsIAccessibleExSupported();
            }

            internal override bool IsPatternSupported(int patternId) {
                if (patternId == NativeMethods.UIA_ExpandCollapsePatternId) {
                    if (ownerItem.DropDownStyle == ComboBoxStyle.Simple) {
                        return false;
                    }
                    return true;
                }
                else {
                    if (patternId == NativeMethods.UIA_ValuePatternId) {
                        if (ownerItem.DropDownStyle == ComboBoxStyle.DropDownList) {
                            return false;
                        }
                        return true;
                    }
                }
                return base.IsPatternSupported(patternId);
            }

            internal override int[] RuntimeId {
                get {
                    if (ownerItem != null) {
                        // we need to provide a unique ID
                        // others are implementing this in the same manner
                        // first item is static - 0x2a (RuntimeIDFirstItem)
                        // second item can be anything, but here it is a hash

                        var runtimeId = new int[3];
                        runtimeId[0] = RuntimeIDFirstItem;
                        runtimeId[1] = (int)(long)ownerItem.Handle;
                        runtimeId[2] = ownerItem.GetHashCode();
                        return runtimeId;
                    }
                    
                    return base.RuntimeId;
                }
            }

            internal override object GetPropertyValue(int propertyID) {

                if (propertyID == NativeMethods.UIA_NamePropertyId) {
                    return Name;
                }
                else if (propertyID == NativeMethods.UIA_IsExpandCollapsePatternAvailablePropertyId) {
                    return (object)this.IsPatternSupported(NativeMethods.UIA_ExpandCollapsePatternId);
                }
                else if (propertyID == NativeMethods.UIA_IsValuePatternAvailablePropertyId) {
                    return (object)this.IsPatternSupported(NativeMethods.UIA_ValuePatternId);
                }

                return base.GetPropertyValue(propertyID);
            }
            
            internal override void Expand() {
                ComboBoxDefaultAction(true);
            }

            internal override void Collapse() {
                ComboBoxDefaultAction(false);
            }

            internal override UnsafeNativeMethods.ExpandCollapseState ExpandCollapseState {
                get {
                    return ownerItem.DroppedDown == true ? UnsafeNativeMethods.ExpandCollapseState.Expanded : UnsafeNativeMethods.ExpandCollapseState.Collapsed;
                }
            }
        }

        /// <summary>
        /// ComboBox control accessible object for AccessibilityImprovements of Level 3 with UI Automation provider functionality.
        /// This inherits from the base ComboBoxExAccessibleObject and ComboBoxAccessibleObject to have all base functionality.
        /// </summary>
        internal class ComboBoxUiaProvider : ComboBoxExAccessibleObject {

            private ComboBox _owner;

            /// <summary>
            /// Initializes new instance of ComboBoxUiaProvider.
            /// </summary>
            /// <param name="owner">The owning ComboBox control.</param>
            public ComboBoxUiaProvider(ComboBox owner) : base(owner) {
                _owner = owner;
            }

            /// <summary>
            /// Gets the DropDown button accessible object. (UI Automation provider)
            /// </summary>
            public ComboBoxChildDropDownButtonUiaProvider DropDownButtonUiaProvider {
                get {
                    return (new ComboBoxChildDropDownButtonUiaProvider(_owner, _owner.Handle));
                }
            }

            /// <summary>
            /// Returns the element in the specified direction.
            /// </summary>
            /// <param name="direction">Indicates the direction in which to navigate.</param>
            /// <returns>Returns the element in the specified direction.</returns>
            internal override UnsafeNativeMethods.IRawElementProviderFragment FragmentNavigate(UnsafeNativeMethods.NavigateDirection direction) {
                if (direction == UnsafeNativeMethods.NavigateDirection.FirstChild) {
                    return GetChild(0);
                }
                else if (direction == UnsafeNativeMethods.NavigateDirection.LastChild) {
                    var childCount = GetChildCount();
                    if (childCount > 0)
                    {
                        return GetChild(childCount - 1);
                    }
                }

                return base.FragmentNavigate(direction);
            }

            /// <summary>
            /// Gets the accessible child corresponding to the specified index.
            /// </summary>
            /// <param name="index">The child index.</param>
            /// <returns>The accessible child.</returns>
            public override AccessibleObject GetChild(int index) {

                if (_owner.childEdit != null && _owner.childEdit.Handle != IntPtr.Zero) {
                    if (index == 0) {
                        return _owner.ChildEditAccessibleObject;
                    }

                    index--;
                }
                else if (_owner.DropDownStyle == ComboBoxStyle.DropDownList) {
                    if (index == 0) {
                        return _owner.ChildTextAccessibleObject;
                    }

                    index--;
                }

                if (index == 0 && _owner.DropDownStyle != ComboBoxStyle.Simple) {
                    return DropDownButtonUiaProvider;
                }

                return null;
            }

            /// <summary>
            /// Gets the number of children belonging to an accessible object.
            /// </summary>
            /// <returns>The number of children.</returns>
            public override int GetChildCount() {
                int childCount = 0;

                if (_owner.childEdit != null && _owner.childEdit.Handle != IntPtr.Zero) {
                    childCount++;
                }
                else if (_owner.DropDownStyle == ComboBoxStyle.DropDownList) {
                    childCount++; // Text instead of edit for style is DropDownList but not DropDown.
                }

                if (_owner.DropDownStyle != ComboBoxStyle.Simple) {
                    childCount++; // DropDown button.
                }

                return childCount;
            }

            /// <summary>
            /// Gets the accessible property value.
            /// </summary>
            /// <param name="propertyID">The accessible property ID.</param>
            /// <returns>The accessible property value.</returns>
            internal override object GetPropertyValue(int propertyID) {
                if (propertyID == NativeMethods.UIA_ControlTypePropertyId) {
                    return NativeMethods.UIA_ComboBoxControlTypeId;
                }

                return base.GetPropertyValue(propertyID);
            }
        }

        /// <summary>
        /// Represents the ComboBox's child (inner) edit native window control accessible object with UI Automation provider functionality.
        /// </summary>
        internal class ComboBoxChildEditUiaProvider : ChildAccessibleObject {

            private int COMBO_BOX_EDIT_AUTOMATION_ID = 1001;

            private ComboBox _owner;
            private IntPtr _handle;

            /// <summary>
            /// Initializes new instance of ComboBoxChildEditUiaProvider.
            /// </summary>
            /// <param name="owner">The ComboBox owning control.</param>
            /// <param name="childEditControlhandle">The child edit native window handle.</param>
            public ComboBoxChildEditUiaProvider(ComboBox owner, IntPtr childEditControlhandle) : base(owner, childEditControlhandle) {
                _owner = owner;
                _handle = childEditControlhandle;
            }

            /// <summary>
            /// Returns the element in the specified direction.
            /// </summary>
            /// <param name="direction">Indicates the direction in which to navigate.</param>
            /// <returns>Returns the element in the specified direction.</returns>
            internal override UnsafeNativeMethods.IRawElementProviderFragment FragmentNavigate(UnsafeNativeMethods.NavigateDirection direction) {
                switch (direction) {
                    case UnsafeNativeMethods.NavigateDirection.Parent:
                        Debug.WriteLine("Edit parent " + _owner.AccessibilityObject.GetPropertyValue(NativeMethods.UIA_ControlTypePropertyId));
                        return _owner.AccessibilityObject;
                    case UnsafeNativeMethods.NavigateDirection.NextSibling:
                        if (_owner.DropDownStyle == ComboBoxStyle.Simple) {
                            return null;
                        }

                        int comboBoxChildCount = _owner.AccessibilityObject.GetChildCount();
                        if (comboBoxChildCount > 1) { // DropDown button is next;
                            return _owner.AccessibilityObject.GetChild(comboBoxChildCount - 1);
                        }

                        return null;
                    case UnsafeNativeMethods.NavigateDirection.PreviousSibling:
                        var firstComboBoxChild = _owner.AccessibilityObject.GetChild(0);
                        if (RuntimeId != firstComboBoxChild.RuntimeId) {
                            return firstComboBoxChild;
                        }

                        return null;
                    default:
                        return base.FragmentNavigate(direction);
                }
            }

            /// <summary>
            /// Gets the top level element.
            /// </summary>
            internal override UnsafeNativeMethods.IRawElementProviderFragmentRoot FragmentRoot {
                get {
                    return _owner.AccessibilityObject;
                }
            }

            /// <summary>
            /// Gets the accessible property value.
            /// </summary>
            /// <param name="propertyID">The accessible property ID.</param>
            /// <returns>The accessible property value.</returns>
            internal override object GetPropertyValue(int propertyID) {
                switch (propertyID) {
                   case NativeMethods.UIA_RuntimeIdPropertyId:
                        return RuntimeId;
                    case NativeMethods.UIA_BoundingRectanglePropertyId:
                        return Bounds;
                    case NativeMethods.UIA_ControlTypePropertyId:
                        return NativeMethods.UIA_EditControlTypeId;
                    case NativeMethods.UIA_NamePropertyId:
                        return Name;
                    case NativeMethods.UIA_AccessKeyPropertyId:
                        return string.Empty;
                    case NativeMethods.UIA_HasKeyboardFocusPropertyId:
                        return _owner.Focused;
                    case NativeMethods.UIA_IsKeyboardFocusablePropertyId:
                        return (State & AccessibleStates.Focusable) == AccessibleStates.Focusable;
                    case NativeMethods.UIA_IsEnabledPropertyId:
                        return _owner.Enabled;
                    case NativeMethods.UIA_AutomationIdPropertyId:
                        return COMBO_BOX_EDIT_AUTOMATION_ID;
                    case NativeMethods.UIA_HelpTextPropertyId:
                        return Help ?? string.Empty;
                    case NativeMethods.UIA_IsPasswordPropertyId:
                        return false;
                    case NativeMethods.UIA_NativeWindowHandlePropertyId:
                        return _handle;
                    case NativeMethods.UIA_IsOffscreenPropertyId:
                        return false;

                    default:
                        return base.GetPropertyValue(propertyID);
                }
            }

            /// <summary>
            /// Indicates whether the specified pattern is supported.
            /// </summary>
            /// <param name="patternId">The pattern ID.</param>
            /// <returns>True if specified pattern is supported, otherwise false.</returns>
            internal override bool IsPatternSupported(int patternId) {
                if (patternId == NativeMethods.UIA_ValuePatternId ||
                    patternId == NativeMethods.UIA_TextPatternId ||
                    patternId == NativeMethods.UIA_TextPattern2Id) {
                    return true;
                }

                return base.IsPatternSupported(patternId);
            }

            /// <summary>
            /// Gets the accessible name of the object.
            /// </summary>
            public override string Name {
                get {
                    return _owner.AccessibilityObject.Name ?? string.Empty;
                }
            }

            /// <summary>
            /// Gets the runtime ID.
            /// </summary>
            internal override int[] RuntimeId {
                get {
                    var runtimeId = new int[2];
                    runtimeId[0] = RuntimeIDFirstItem;
                    runtimeId[1] = GetHashCode();

                    return runtimeId;
                }
            }
        }

        /// <summary>
        /// Represents the ComboBox's child text (is used instead of inner Edit when style is DropDownList but not DropDown) accessible object.
        /// </summary>
        internal class ComboBoxChildTextUiaProvider : AccessibleObject {

            private const int COMBOBOX_TEXT_ACC_ITEM_INDEX = 1;

            private ComboBox _owner;

            /// <summary>
            /// Initializes new instance of ComboBoxChildTextUiaProvider.
            /// </summary>
            /// <param name="owner">The owning ComboBox control.</param>
            public ComboBoxChildTextUiaProvider(ComboBox owner) {
                _owner = owner;
            }

            /// <summary>
            /// Gets the bounds.
            /// </summary>
            public override Rectangle Bounds {
                get {
                    return _owner.AccessibilityObject.Bounds;
                }
            }

            /// <summary>
            /// Gets the child ID.
            /// </summary>
            /// <returns>The child ID.</returns>
            internal override int GetChildId() {
                return COMBOBOX_TEXT_ACC_ITEM_INDEX;
            }

            /// <summary>
            /// Gets or sets the accessible Name of ComboBox's child text element.
            /// </summary>
            public override string Name {
                get {
                    return _owner.AccessibilityObject.Name ?? string.Empty;
                }
                set {
                    // Do nothing.
                }
            }

            /// <summary>
            /// Returns the element in the specified direction.
            /// </summary>
            /// <param name="direction">Indicates the direction in which to navigate.</param>
            /// <returns>Returns the element in the specified direction.</returns>
            internal override UnsafeNativeMethods.IRawElementProviderFragment FragmentNavigate(UnsafeNativeMethods.NavigateDirection direction) {
                switch (direction) {
                    case UnsafeNativeMethods.NavigateDirection.Parent:
                        return _owner.AccessibilityObject;
                    case UnsafeNativeMethods.NavigateDirection.NextSibling:
                        int comboBoxChildCount = _owner.AccessibilityObject.GetChildCount();
                        if (comboBoxChildCount > 1) { // DropDown button is next;
                            return _owner.AccessibilityObject.GetChild(comboBoxChildCount - 1);
                        }

                        return null;
                    case UnsafeNativeMethods.NavigateDirection.PreviousSibling:
                        var firstComboBoxChild = _owner.AccessibilityObject.GetChild(0);
                        if (RuntimeId != firstComboBoxChild.RuntimeId) {
                            return firstComboBoxChild;
                        }

                        return null;
                    default:
                        return base.FragmentNavigate(direction);
                }
            }

            /// <summary>
            /// Gets the top level element.
            /// </summary>
            internal override UnsafeNativeMethods.IRawElementProviderFragmentRoot FragmentRoot {
                get {
                    return _owner.AccessibilityObject;
                }
            }

            /// <summary>
            /// Gets the accessible property value.
            /// </summary>
            /// <param name="propertyID">The accessible property ID.</param>
            /// <returns>The accessible property value.</returns>
            internal override object GetPropertyValue(int propertyID) {
                switch (propertyID) {
                    case NativeMethods.UIA_RuntimeIdPropertyId:
                        return RuntimeId;
                    case NativeMethods.UIA_BoundingRectanglePropertyId:
                        return Bounds;
                    case NativeMethods.UIA_ControlTypePropertyId:
                        return NativeMethods.UIA_TextControlTypeId;
                    case NativeMethods.UIA_NamePropertyId:
                        return Name;
                    case NativeMethods.UIA_AccessKeyPropertyId:
                        return string.Empty;
                    case NativeMethods.UIA_HasKeyboardFocusPropertyId:
                        return _owner.Focused;
                    case NativeMethods.UIA_IsKeyboardFocusablePropertyId:
                        return (State & AccessibleStates.Focusable) == AccessibleStates.Focusable;
                    case NativeMethods.UIA_IsEnabledPropertyId:
                        return _owner.Enabled;
                    case NativeMethods.UIA_HelpTextPropertyId:
                        return Help ?? string.Empty;
                    case NativeMethods.UIA_IsPasswordPropertyId:
                    case NativeMethods.UIA_IsOffscreenPropertyId:
                        return false;
                    default:
                        return base.GetPropertyValue(propertyID);
                }
            }

            /// <summary>
            /// Gets the runtime ID.
            /// </summary>
            internal override int[] RuntimeId {
                get {
                    var runtimeId = new int[5];
                    runtimeId[0] = RuntimeIDFirstItem;
                    runtimeId[1] = (int)(long)_owner.Handle;
                    runtimeId[2] = _owner.GetHashCode();
                    runtimeId[3] = GetHashCode();
                    runtimeId[4] = GetChildId();

                    return runtimeId;
                }
            }

            /// <summary>
            /// Gets the accessible state.
            /// </summary>
            public override AccessibleStates State {
                get {
                    AccessibleStates state = AccessibleStates.Focusable;
                    if (_owner.Focused) {
                        state |= AccessibleStates.Focused;
                    }

                    return state;
                }
            }
        }

        /// <summary>
        /// Represents the ComboBox child (inner) DropDown button accessible object with UI Automation functionality.
        /// </summary>
        internal class ComboBoxChildDropDownButtonUiaProvider : AccessibleObject {

            private const int COMBOBOX_DROPDOWN_BUTTON_ACC_ITEM_INDEX = 2;
            private ComboBox _owner;

            /// <summary>
            /// Initializes new instance of ComboBoxChildDropDownButtonUiaProvider.
            /// </summary>
            /// <param name="owner">The owning ComboBox control.</param>
            /// <param name="comboBoxControlhandle">The owning ComboBox control's handle.</param>
            public ComboBoxChildDropDownButtonUiaProvider(ComboBox owner, IntPtr comboBoxControlhandle) {
                _owner = owner;
                UseStdAccessibleObjects(comboBoxControlhandle);
            }

            /// <summary>
            /// Gets or sets the accessible Name of ComboBox's child DropDown button. ("Open" or "Close" depending on stat of the DropDown)
            /// </summary>
            public override string Name {
                get {
                    return get_accNameInternal(COMBOBOX_DROPDOWN_BUTTON_ACC_ITEM_INDEX);
                }
                set {
                    var systemIAccessible = GetSystemIAccessibleInternal();
                    systemIAccessible.set_accName(COMBOBOX_DROPDOWN_BUTTON_ACC_ITEM_INDEX, value);
                }
            }

            /// <summary>
            /// Gets the DropDown button bounds.
            /// </summary>
            public override Rectangle Bounds {
                get {
                    int left;
                    int top;
                    int width;
                    int height;
                    var systemIAccessible = GetSystemIAccessibleInternal();
                    systemIAccessible.accLocation(out left, out top, out width, out height, COMBOBOX_DROPDOWN_BUTTON_ACC_ITEM_INDEX);
                    return new Rectangle(left, top, width, height);
                }
            }

            /// <summary>
            /// Gets the DropDown button default action.
            /// </summary>
            public override string DefaultAction {
                get {
                    var systemIAccessible = GetSystemIAccessibleInternal();
                    return systemIAccessible.accDefaultAction[COMBOBOX_DROPDOWN_BUTTON_ACC_ITEM_INDEX];
                }
            }

            /// <summary>
            /// Returns the element in the specified direction.
            /// </summary>
            /// <param name="direction">Indicates the direction in which to navigate.</param>
            /// <returns>Returns the element in the specified direction.</returns>
            internal override UnsafeNativeMethods.IRawElementProviderFragment FragmentNavigate(UnsafeNativeMethods.NavigateDirection direction) {
                if (direction == UnsafeNativeMethods.NavigateDirection.Parent) {
                    return _owner.AccessibilityObject;
                }
                else if (direction == UnsafeNativeMethods.NavigateDirection.PreviousSibling) {
                    int comboBoxChildCount = _owner.AccessibilityObject.GetChildCount();
                    if (comboBoxChildCount > 1) { // Text or edit is previous;
                        return _owner.AccessibilityObject.GetChild(comboBoxChildCount - 1);
                    }

                    return null;
                }

                return base.FragmentNavigate(direction);
            }

            /// <summary>
            /// Gets the top level element.
            /// </summary>
            internal override UnsafeNativeMethods.IRawElementProviderFragmentRoot FragmentRoot {
                get {
                    return _owner.AccessibilityObject;
                }
            }

            /// <summary>
            /// Gets the child accessible object ID.
            /// </summary>
            /// <returns>The child accessible object ID.</returns>
            internal override int GetChildId() {
                return COMBOBOX_DROPDOWN_BUTTON_ACC_ITEM_INDEX;
            }

            /// <summary>
            /// Gets the accessible property value.
            /// </summary>
            /// <param name="propertyID">The accessible property ID.</param>
            /// <returns>The accessible property value.</returns>
            internal override object GetPropertyValue(int propertyID) {
                switch (propertyID) {
                    case NativeMethods.UIA_RuntimeIdPropertyId:
                        return RuntimeId;
                    case NativeMethods.UIA_BoundingRectanglePropertyId:
                        return BoundingRectangle;
                    case NativeMethods.UIA_ControlTypePropertyId:
                        return NativeMethods.UIA_ButtonControlTypeId;
                    case NativeMethods.UIA_NamePropertyId:
                        return Name;
                    case NativeMethods.UIA_AccessKeyPropertyId:
                        return KeyboardShortcut;
                    case NativeMethods.UIA_HasKeyboardFocusPropertyId:
                        return _owner.Focused;
                    case NativeMethods.UIA_IsKeyboardFocusablePropertyId:
                        return (State & AccessibleStates.Focusable) == AccessibleStates.Focusable;
                    case NativeMethods.UIA_IsEnabledPropertyId:
                        return _owner.Enabled;
                    case NativeMethods.UIA_HelpTextPropertyId:
                        return Help ?? string.Empty;
                    case NativeMethods.UIA_IsPasswordPropertyId:
                        return false;
                    case NativeMethods.UIA_IsOffscreenPropertyId:
                        return (State & AccessibleStates.Offscreen) == AccessibleStates.Offscreen;
                    default:
                        return base.GetPropertyValue(propertyID);
                }
            }

            /// <summary>
            /// Gets the help text.
            /// </summary>
            public override string Help {
                get {
                    var systemIAccessible = GetSystemIAccessibleInternal();
                    return systemIAccessible.accHelp[COMBOBOX_DROPDOWN_BUTTON_ACC_ITEM_INDEX];
                }
            }

            /// <summary>
            /// Gets the keyboard shortcut.
            /// </summary>
            public override string KeyboardShortcut {
                get {
                    var systemIAccessible = GetSystemIAccessibleInternal();
                    return systemIAccessible.get_accKeyboardShortcut(COMBOBOX_DROPDOWN_BUTTON_ACC_ITEM_INDEX);
                }
            }

            /// <summary>
            /// Indicates whether specified pattern is supported.
            /// </summary>
            /// <param name="patternId">The pattern ID.</param>
            /// <returns>True if specified </returns>
            internal override bool IsPatternSupported(int patternId) {
                if (patternId == NativeMethods.UIA_LegacyIAccessiblePatternId ||
                    patternId == NativeMethods.UIA_InvokePatternId) {
                    return true;
                }

                return base.IsPatternSupported(patternId);
            }

            /// <summary>
            /// Gets the accessible role.
            /// </summary>
            public override AccessibleRole Role {
                get {
                    var systemIAccessible = GetSystemIAccessibleInternal();
                    return (AccessibleRole)systemIAccessible.get_accRole(COMBOBOX_DROPDOWN_BUTTON_ACC_ITEM_INDEX);
                }
            }

            /// <summary>
            /// Gets the runtime ID.
            /// </summary>
            internal override int[] RuntimeId {
                get {
                    var runtimeId = new int[5];
                    runtimeId[0] = RuntimeIDFirstItem;
                    runtimeId[1] = (int)(long)_owner.Handle;
                    runtimeId[2] = _owner.GetHashCode();

                    // Made up constant from MSAA proxy. When MSAA proxy is used as an accessibility provider,
                    // the similar Runtime ID is returned (for consistency purpose)
                    const int generatedRuntimeId = 61453;
                    runtimeId[3] = generatedRuntimeId;
                    runtimeId[4] = COMBOBOX_DROPDOWN_BUTTON_ACC_ITEM_INDEX;
                    return runtimeId;
                }
            }

            /// <summary>
            /// Gets the accessible state.
            /// </summary>
            public override AccessibleStates State {
                get {
                    var systemIAccessible = GetSystemIAccessibleInternal();
                    return (AccessibleStates)systemIAccessible.get_accState(COMBOBOX_DROPDOWN_BUTTON_ACC_ITEM_INDEX);
                }
            }
        }

        /// <devdoc>
        ///  This subclasses an autocomplete window so that we can determine if control is inside the AC wndproc.
        /// </devdoc>
        private sealed class ACNativeWindow : NativeWindow {
            static internal int inWndProcCnt;
            //this hashtable can contain null for those ACWindows we find, but are sure are not ours.
            static private Hashtable ACWindows = new Hashtable();

            [
                SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")
            ]
            internal ACNativeWindow(IntPtr acHandle) {
                Debug.Assert(!ACWindows.ContainsKey(acHandle));
                this.AssignHandle(acHandle);
                ACWindows.Add(acHandle, this);
                UnsafeNativeMethods.EnumChildWindows(new HandleRef(this, acHandle),
                    new NativeMethods.EnumChildrenCallback(ACNativeWindow.RegisterACWindowRecursive),
                    NativeMethods.NullHandleRef);
            }

            [SuppressMessage("Microsoft.Performance", "CA1806:DoNotIgnoreMethodResults")]
            private static bool RegisterACWindowRecursive(IntPtr handle, IntPtr lparam) {
                if (!ACWindows.ContainsKey(handle)) {
                    ACNativeWindow newAC = new ACNativeWindow(handle);
                }
                return true;
            }

            internal bool Visible {
                get {
                    return SafeNativeMethods.IsWindowVisible(new HandleRef(this, Handle));
                }
            }

            static internal bool AutoCompleteActive {
                get {
                    if (inWndProcCnt>0) {
                        return true;
                    }
                    foreach(object o in ACWindows.Values) {
                        ACNativeWindow window = o as ACNativeWindow;
                        if (window != null && window.Visible) {
                            return true;
                        }
                    }
                    return false;
                }
            }

            protected override void WndProc(ref Message m) {
                inWndProcCnt++;
                try {
                    base.WndProc(ref m);
                }
                finally {
                    inWndProcCnt--;
                }

                if (m.Msg == NativeMethods.WM_NCDESTROY) {
                    Debug.Assert(ACWindows.ContainsKey(this.Handle));
                    ACWindows.Remove(this.Handle);   //so we do not leak ac windows.
                }
            }

            [SuppressMessage("Microsoft.Performance", "CA1806:DoNotIgnoreMethodResults")]
            internal static void RegisterACWindow(IntPtr acHandle, bool subclass) {
                if (subclass && ACWindows.ContainsKey(acHandle)) {
                    if (ACWindows[acHandle] == null) {
                        ACWindows.Remove(acHandle); //if an external handle got destroyed, dont let it stop us.
                    }
                }

                if (!ACWindows.ContainsKey(acHandle)) {
                    if (subclass) {
                        ACNativeWindow newAC = new ACNativeWindow(acHandle);
                    }
                    else {
                        ACWindows.Add(acHandle, null);
                    }
                }
            }

            /// <summary>
            /// This method clears out null entries so we get a clean BEFORE and AFTER snapshot
            /// null entries are ACWindows that belong to someone else.
            /// </summary>
            [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1808:AvoidCallsThatBoxValueTypes")] // Perfectly acceptible when dealing with PropertyDescriptors
            internal static void ClearNullACWindows() {
                ArrayList nulllist = new ArrayList();
                foreach (DictionaryEntry e in ACWindows) {
                    if (e.Value == null) {
                        nulllist.Add(e.Key);
                    }
                }
                foreach (IntPtr handle in nulllist) {
                    ACWindows.Remove(handle);
                }
            }
        }

        /// <devdoc>
        ///  This finds all autcomplete windows that belong to the active thread.
        /// </devdoc>
        private class AutoCompleteDropDownFinder {
            private const int MaxClassName = 256;
            private const string AutoCompleteClassName = "Auto-Suggest Dropdown";
            bool shouldSubClass = false; //nonstatic

            internal void FindDropDowns() {
                FindDropDowns(true);
            }

            internal void FindDropDowns(bool subclass) {
                if (!subclass) {
                    //generating a before snapshot -- lets lose the null handles
                    ACNativeWindow.ClearNullACWindows();
                }
                // Look for a popped up dropdown
                shouldSubClass = subclass;
                UnsafeNativeMethods.EnumThreadWindows(SafeNativeMethods.GetCurrentThreadId(), new NativeMethods.EnumThreadWindowsCallback(this.Callback), new HandleRef(null, IntPtr.Zero));
            }

            private bool Callback(IntPtr hWnd, IntPtr lParam) {
                HandleRef hRef = new HandleRef(null, hWnd);

                // Check class name and see if it's visible
                if (GetClassName(hRef) == AutoCompleteClassName) {
                    ACNativeWindow.RegisterACWindow(hRef.Handle, shouldSubClass);
                }

                return true;
            }

            static string GetClassName(HandleRef hRef) {
                StringBuilder sb = new StringBuilder(MaxClassName);
                UnsafeNativeMethods.GetClassName(hRef, sb, MaxClassName);
                return sb.ToString();
            }
        }
        

        private FlatComboAdapter FlatComboBoxAdapter {
             get { 
                 FlatComboAdapter comboAdapter = Properties.GetObject(PropFlatComboAdapter) as FlatComboAdapter;
                 if (comboAdapter == null || !comboAdapter.IsValid(this)) {
                    comboAdapter = CreateFlatComboAdapterInstance();
                    Properties.SetObject(PropFlatComboAdapter, comboAdapter);
                 }
                 return comboAdapter;
             }
        }
        internal virtual FlatComboAdapter CreateFlatComboAdapterInstance() {
            return new FlatComboAdapter(this,/*smallButton=*/false);
        }

        
        internal class FlatComboAdapter {
             Rectangle outerBorder;        
             Rectangle innerBorder;
             Rectangle innerInnerBorder;
             internal Rectangle dropDownRect;
             Rectangle whiteFillRect;
             Rectangle clientRect;

             RightToLeft origRightToLeft; // The combo box's RTL value when we were created

             private const int WhiteFillRectWidth = 5; // used for making the button look smaller than it is

             private static int OFFSET_2PIXELS = 2;
             protected static int Offset2Pixels = OFFSET_2PIXELS;

            public FlatComboAdapter(ComboBox comboBox, bool smallButton) {
                // adapter is re-created when combobox is resized, see IsValid method, thus we don't need to handle DPI changed explicitly 
                Offset2Pixels = comboBox.LogicalToDeviceUnits(OFFSET_2PIXELS);

                clientRect = comboBox.ClientRectangle;
                int dropDownButtonWidth = SystemInformation.GetHorizontalScrollBarArrowWidthForDpi(comboBox.deviceDpi);
                outerBorder = new Rectangle(clientRect.Location, new Size(clientRect.Width - 1, clientRect.Height - 1));
                innerBorder = new Rectangle(outerBorder.X + 1, outerBorder.Y + 1, outerBorder.Width - dropDownButtonWidth - 2, outerBorder.Height - 2);
                innerInnerBorder = new Rectangle(innerBorder.X + 1, innerBorder.Y + 1, innerBorder.Width - 2, innerBorder.Height - 2);
                dropDownRect = new Rectangle(innerBorder.Right + 1, innerBorder.Y, dropDownButtonWidth, innerBorder.Height + 1);


                // fill in several pixels of the dropdown rect with white so that it looks like the combo button is thinner.
                if (smallButton) {
                    whiteFillRect = dropDownRect;
                    whiteFillRect.Width = WhiteFillRectWidth;
                    dropDownRect.X += WhiteFillRectWidth;
                    dropDownRect.Width -= WhiteFillRectWidth;
                }

                origRightToLeft = comboBox.RightToLeft;


                if (origRightToLeft == RightToLeft.Yes) {
                    innerBorder.X = clientRect.Width - innerBorder.Right;
                    innerInnerBorder.X = clientRect.Width - innerInnerBorder.Right;
                    dropDownRect.X = clientRect.Width - dropDownRect.Right;
                    whiteFillRect.X = clientRect.Width - whiteFillRect.Right + 1;  // since we're filling, we need to move over to the next px.
                }

            }
 
             public bool IsValid(ComboBox combo) {
                return (combo.ClientRectangle == clientRect && combo.RightToLeft == origRightToLeft);                       
             }

             /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.DrawFlatCombo"]/*' />
             /// <devdoc>
             ///     Paints over the edges of the combo box to make it appear flat.
             /// </devdoc>
             public virtual void DrawFlatCombo(ComboBox comboBox, Graphics g) {
                 if (comboBox.DropDownStyle == ComboBoxStyle.Simple){
                     return;
                 }
             
                 Color outerBorderColor = GetOuterBorderColor(comboBox);
                 Color innerBorderColor = GetInnerBorderColor(comboBox);
                 bool rightToLeft = comboBox.RightToLeft == RightToLeft.Yes;
             
                 // draw the drop down
                 DrawFlatComboDropDown(comboBox, g, dropDownRect);
             
                 // when we are disabled there is one line of color that seems to eek through if backcolor is set
                 // so lets erase it.
                 if (!LayoutUtils.IsZeroWidthOrHeight(whiteFillRect)) {
                    // fill in two more pixels with white so it looks smaller.
                    using (Brush b = new SolidBrush(innerBorderColor)) {
                         g.FillRectangle(b, whiteFillRect);
                    }
                 }
                 
                 

                 // Draw the outer border
                 if (outerBorderColor.IsSystemColor) {
                     Pen outerBorderPen = SystemPens.FromSystemColor(outerBorderColor);
                     g.DrawRectangle(outerBorderPen, outerBorder);
                     if (rightToLeft) {
                         g.DrawRectangle(outerBorderPen, new Rectangle(outerBorder.X, outerBorder.Y, dropDownRect.Width + 1, outerBorder.Height));
                     }
                     else {
                         g.DrawRectangle(outerBorderPen, new Rectangle(dropDownRect.X, outerBorder.Y, outerBorder.Right - dropDownRect.X, outerBorder.Height));
                     }             
                 }
                 else {
                     using (Pen outerBorderPen = new Pen(outerBorderColor)) {
                         g.DrawRectangle(outerBorderPen, outerBorder);
                         if (rightToLeft) {
                             g.DrawRectangle(outerBorderPen, new Rectangle(outerBorder.X, outerBorder.Y, dropDownRect.Width + 1, outerBorder.Height));
                         }
                         else {
                             g.DrawRectangle(outerBorderPen, new Rectangle(dropDownRect.X, outerBorder.Y, outerBorder.Right - dropDownRect.X, outerBorder.Height));
                         }
                     }
                 }
             
                 // Draw the inner border
                 if (innerBorderColor.IsSystemColor) {
                     Pen innerBorderPen = SystemPens.FromSystemColor(innerBorderColor);
                     g.DrawRectangle(innerBorderPen, innerBorder);
                     g.DrawRectangle(innerBorderPen, innerInnerBorder);
                 }
                 else {
                     using (Pen innerBorderPen = new Pen(innerBorderColor)) {
                         g.DrawRectangle(innerBorderPen, innerBorder);
                         g.DrawRectangle(innerBorderPen, innerInnerBorder);
                     }
                 }
             
             
                   
                 // Draw a dark border around everything if we're in popup mode
                 if ((!comboBox.Enabled)  ||(comboBox.FlatStyle == FlatStyle.Popup)) {
                     bool focused = comboBox.ContainsFocus || comboBox.MouseIsOver;
                     Color borderPenColor = GetPopupOuterBorderColor(comboBox, focused);
                   
                     using (Pen borderPen = new Pen(borderPenColor)) {
                 
                         Pen innerPen = (comboBox.Enabled) ? borderPen : SystemPens.Control;
                         
                         // around the dropdown
                         if (rightToLeft) {
                             g.DrawRectangle(innerPen, new Rectangle(outerBorder.X, outerBorder.Y, dropDownRect.Width + 1, outerBorder.Height));
                         }
                         else {
                             g.DrawRectangle(innerPen, new Rectangle(dropDownRect.X, outerBorder.Y, outerBorder.Right - dropDownRect.X, outerBorder.Height));
                         }
                         
                         // around the whole combobox.
                         g.DrawRectangle(borderPen, outerBorder);
                     
                     }
                 }
           
             }
             
             /// <include file='doc\ComboBox.uex' path='docs/doc[@for="ComboBox.DrawFlatComboDropDown"]/*' />
             /// <devdoc>
             ///     Paints over the edges of the combo box to make it appear flat.
             /// </devdoc>
             
             protected virtual void DrawFlatComboDropDown(ComboBox comboBox, Graphics g, Rectangle dropDownRect) {
             
                g.FillRectangle(SystemBrushes.Control, dropDownRect);

                Brush brush = (comboBox.Enabled) ? SystemBrushes.ControlText : SystemBrushes.ControlDark;
             
                 Point middle = new Point(dropDownRect.Left + dropDownRect.Width / 2, dropDownRect.Top + dropDownRect.Height / 2);
                 if (origRightToLeft == RightToLeft.Yes) {
                     // if the width is odd - favor pushing it over one pixel left.
                    middle.X -= (dropDownRect.Width % 2);
                 }
                 else {
                     // if the width is odd - favor pushing it over one pixel right.
                     middle.X += (dropDownRect.Width % 2);
                 }
             
                 g.FillPolygon(brush, new Point[] {
                     new Point(middle.X - Offset2Pixels, middle.Y - 1),
                     new Point(middle.X + Offset2Pixels + 1, middle.Y - 1),
                     new Point(middle.X, middle.Y + Offset2Pixels)
                 });
             }
             
             protected virtual Color GetOuterBorderColor(ComboBox comboBox) {
                return (comboBox.Enabled) ? SystemColors.Window : SystemColors.ControlDark;
             }

             protected virtual Color GetPopupOuterBorderColor(ComboBox comboBox, bool focused) {
                if (!comboBox.Enabled) {
                    return SystemColors.ControlDark;
                }
                return (focused) ? SystemColors.ControlDark : SystemColors.Window;
             }
             
             protected virtual Color GetInnerBorderColor(ComboBox comboBox) {
                return (comboBox.Enabled) ? comboBox.BackColor : SystemColors.Control;
             }
             
             // this eliminates flicker by removing the pieces we're going to paint ourselves from 
             // the update region.  Note the UpdateRegionBox is the bounding box of the actual update region.
             // this is just here so we can quickly eliminate rectangles that arent in the update region.
             public void ValidateOwnerDrawRegions(ComboBox comboBox, Rectangle updateRegionBox) {
                 NativeMethods.RECT validRect;
                 if (comboBox != null) { return; }
                 Rectangle topOwnerDrawArea = new Rectangle(0,0,comboBox.Width, innerBorder.Top);
                 Rectangle bottomOwnerDrawArea = new Rectangle(0,innerBorder.Bottom,comboBox.Width, comboBox.Height-innerBorder.Bottom);
                 Rectangle leftOwnerDrawArea = new Rectangle(0,0,innerBorder.Left, comboBox.Height);
                 Rectangle rightOwnerDrawArea = new Rectangle(innerBorder.Right,0,comboBox.Width - innerBorder.Right,comboBox.Height);
 
                 if (topOwnerDrawArea.IntersectsWith(updateRegionBox)) {                            
                     validRect =  new NativeMethods.RECT(topOwnerDrawArea);
                     SafeNativeMethods.ValidateRect(new HandleRef(comboBox, comboBox.Handle), ref validRect);
                 }
 
                 if (bottomOwnerDrawArea.IntersectsWith(updateRegionBox)) {                            
                     validRect =  new NativeMethods.RECT(bottomOwnerDrawArea);
                     SafeNativeMethods.ValidateRect(new HandleRef(comboBox, comboBox.Handle), ref validRect);
                 }
 
                 if (leftOwnerDrawArea.IntersectsWith(updateRegionBox)) {                            
                     validRect =  new NativeMethods.RECT(leftOwnerDrawArea);
                     SafeNativeMethods.ValidateRect(new HandleRef(comboBox, comboBox.Handle), ref validRect);
                 }
 
                 if (rightOwnerDrawArea.IntersectsWith(updateRegionBox)) {                            
                     validRect =  new NativeMethods.RECT(rightOwnerDrawArea);
                     SafeNativeMethods.ValidateRect(new HandleRef(comboBox, comboBox.Handle), ref validRect);
                 }
             
             }
        }

        /// <summary>
        /// Represents the ComboBox child native window type.
        /// </summary>
        private enum ChildWindowType {
            List,
            Edit
        }
    }
}
