// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Drawing;
    using Microsoft.Win32;
    using System.Drawing.Imaging;
    using System.Threading;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Windows.Forms.Layout;   
    using System.Security;
    using System.Security.Permissions;
    using System.Runtime.InteropServices;
    using System.ComponentModel.Design.Serialization;
    using System.Windows.Forms.Design; 
    using System.Globalization;
    using System.Windows.Forms.Internal;    
    using System.Runtime.Versioning;

    /// <include file='doc\ToolStripMenuItem.uex' path='docs/doc[@for="ToolStripMenuItem"]/*' />
    /// <devdoc>
    /// ToolStripMenuItem
    /// </devdoc>
    [ToolStripItemDesignerAvailability(ToolStripItemDesignerAvailability.MenuStrip | ToolStripItemDesignerAvailability.ContextMenuStrip)]
    [DesignerSerializer("System.Windows.Forms.Design.ToolStripMenuItemCodeDomSerializer, " + AssemblyRef.SystemDesign, "System.ComponentModel.Design.Serialization.CodeDomSerializer, " + AssemblyRef.SystemDesign)] 
    public class ToolStripMenuItem : ToolStripDropDownItem {

        private static MenuTimer menuTimer                             = new MenuTimer();

        private static readonly int PropShortcutKeys                   = PropertyStore.CreateKey();
        private static readonly int PropCheckState                     = PropertyStore.CreateKey();
        private static readonly int PropMdiForm                        = PropertyStore.CreateKey();

        private bool checkOnClick                                      = false;
        private bool showShortcutKeys                                  = true;
        private ToolStrip lastOwner                                    = null;

        // SUPPORT for mapping NATIVE menu commands to ToolStripMenuItems -----
        // corresponds to wID in MENUITEMINFO structure
        private int     nativeMenuCommandID                               = -1;
        private IntPtr  targetWindowHandle                                = IntPtr.Zero;
        private IntPtr  nativeMenuHandle                                  = IntPtr.Zero;
        
        // Keep checked images shared between menu items, but per thread so we dont have locking issues in GDI+
        [ThreadStatic]
        private static Image indeterminateCheckedImage;
        
        [ThreadStatic]
        private static Image checkedImage;

        private string shortcutKeyDisplayString;
        private string cachedShortcutText;
        private Size cachedShortcutSize = Size.Empty;

        private static readonly Padding defaultPadding = new Padding(4, 0, 4, 0);
        private static readonly Padding defaultDropDownPadding = new Padding(0, 1, 0, 1);
        private static readonly Size checkMarkBitmapSize = new Size(16, 16);
        private Padding scaledDefaultPadding = defaultPadding;
        private Padding scaledDefaultDropDownPadding = defaultDropDownPadding;
        private Size scaledCheckMarkBitmapSize = checkMarkBitmapSize;

        private byte openMouseId = 0;
    
        private static readonly object EventCheckedChanged = new object();
        private static readonly object EventCheckStateChanged = new object();
            
        /// <include file='doc\ToolStripMenuItem.uex' path='docs/doc[@for="ToolStripMenuItem.ToolStripMenuItem"]/*' />
        public ToolStripMenuItem() : base() {
            Initialize(); // all additional work should be done in Initialize
        }
        public ToolStripMenuItem(string text):base(text,null,(EventHandler)null) {
            Initialize();
        }
        public ToolStripMenuItem(Image image):base(null,image,(EventHandler)null) {
            Initialize();
        }
        public ToolStripMenuItem(string text, Image image):base(text,image,(EventHandler)null) {
            Initialize();
        }
        public ToolStripMenuItem(string text, Image image, EventHandler onClick):base(text,image,onClick) {
            Initialize();
        }
        public ToolStripMenuItem(string text, Image image, EventHandler onClick, string name) : base(text,image,onClick, name){
            Initialize();
        }            
        public ToolStripMenuItem(string text, Image image, params ToolStripItem[] dropDownItems):base(text,image,dropDownItems) {
            Initialize();
        }
        public ToolStripMenuItem(string text, Image image, EventHandler onClick, Keys shortcutKeys):base(text,image,onClick) {
            Initialize();
            this.ShortcutKeys = shortcutKeys;
        }
        internal ToolStripMenuItem(Form mdiForm) {
            Initialize();
            Properties.SetObject(PropMdiForm,mdiForm);
        }

        /// <devdoc> this constructor is only used when we're trying to
        ///          mimic a native menu like the system menu.  In that case
        ///          we've got to go ahead and collect the command id and the
        ///          target window to send WM_COMMAND/WM_SYSCOMMAND messages to.
        /// </devdoc>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        internal ToolStripMenuItem(IntPtr hMenu, int nativeMenuCommandId, IWin32Window targetWindow) {
            Initialize();
            this.Overflow                   = ToolStripItemOverflow.Never;
            this.nativeMenuCommandID        = nativeMenuCommandId;
            this.targetWindowHandle         = Control.GetSafeHandle(targetWindow);
            this.nativeMenuHandle           = hMenu;

            // Since fetching the image and the text is an awful lot of work
            // we're going to just cache it and assume the native stuff 
            // doesnt update.
            // we'll only live-update enabled.
            // if this becomes a problem we can override Image and Text properties
            // to live-return the results.

            // fetch image
            this.Image = GetNativeMenuItemImage();
            this.ImageScaling = ToolStripItemImageScaling.None;

            // fetch text
            string text = GetNativeMenuItemTextAndShortcut();

            // the shortcut is tab separated from the item text.
            if (text != null) {
                // separate out the two fields.
                string[] textFields = text.Split('\t');

                if (textFields.Length >= 1){
                    this.Text = textFields[0];
                }

                if (textFields.Length >= 2) {
                    // We dont care about the shortcut here, the OS is going to 
                    // handle it for us by sending a WM_(SYS)COMMAND during TranslateAcellerator
                    // Just display whatever the OS would have.
                    this.ShowShortcutKeys = true;
                    this.ShortcutKeyDisplayString = textFields[1];
                }
            }
        }

        internal override void AutoHide(ToolStripItem otherItemBeingSelected) {     
            if (IsOnDropDown) {
                MenuTimer.Transition(this,otherItemBeingSelected as ToolStripMenuItem);
            }
            else {
                base.AutoHide(otherItemBeingSelected);
            }
        }
        private void ClearShortcutCache() {
            cachedShortcutSize = Size.Empty;
            cachedShortcutText = null;
        }

        /// <include file='doc\ToolStripMenuItem.uex' path='docs/doc[@for="ToolStripMenuItem.CreateDefaultDropDown"]/*' />
        protected override ToolStripDropDown CreateDefaultDropDown() {
            // AutoGenerate a Winbar DropDown - set the property so we hook events
             return new ToolStripDropDownMenu(this, true);
        }

        internal override ToolStripItemInternalLayout CreateInternalLayout() {
              return new ToolStripMenuItemInternalLayout(this);
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override AccessibleObject CreateAccessibilityInstance() {
            return new ToolStripMenuItemAccessibleObject(this);
        }

        private void Initialize() {
            if (DpiHelper.IsScalingRequirementMet) {
                scaledDefaultPadding = DpiHelper.LogicalToDeviceUnits(defaultPadding);
                scaledDefaultDropDownPadding = DpiHelper.LogicalToDeviceUnits(defaultDropDownPadding);
                scaledCheckMarkBitmapSize = DpiHelper.LogicalToDeviceUnits(checkMarkBitmapSize);
            }

            this.Overflow = ToolStripItemOverflow.Never;
            this.MouseDownAndUpMustBeInSameItem = false;
            this.SupportsDisabledHotTracking = true;
        }
    
        /// <include file='doc\ToolStripMenuItem.uex' path='docs/doc[@for="ToolStripMenuItem.DefaultSize"]/*' />
        /// <devdoc>
        /// Deriving classes can override this to configure a default size for their control.
        /// This is more efficient than setting the size in the control's constructor.
        /// </devdoc>
        protected override Size DefaultSize {
            get {
                return new Size(32, 19);
            }
        }


        /// <include file='doc\WinBarMenuItem.uex' path='docs/doc[@for="ToolStripMenuItem.DefaultMargin"]/*' />
        protected internal override Padding DefaultMargin {
            get {
                 return Padding.Empty;
            }
        }
        /// <include file='doc\WinBarMenuItem.uex' path='docs/doc[@for="ToolStripMenuItem.DefaultPadding"]/*' />
        protected override Padding DefaultPadding {
            get {
                if (IsOnDropDown) {
                    return scaledDefaultDropDownPadding;
                }
                else {
                    return scaledDefaultPadding;
                }
            }
        }


        public override bool Enabled {
           get{
                if (nativeMenuCommandID != -1) {
                    // if we're based off a native menu item,
                    // we need to ask it if it's enabled.
                    if (base.Enabled && nativeMenuHandle != IntPtr.Zero && targetWindowHandle != IntPtr.Zero) {
                        return GetNativeMenuItemEnabled();
                    }
                    return false;
                }
                else {
                    return base.Enabled;
                }
           }
           set {
                base.Enabled = value;
           }
        }
      
        /// <include file='doc\ToolStripMenuItem.uex' path='docs/doc[@for="ToolStripMenuItem.Checked"]/*' />
        /// <devdoc>
        /// <para>
        /// Gets or sets a value indicating whether the item is checked.
        /// </para>
        /// </devdoc>
        [
        Bindable(true),
        DefaultValue(false),
        SRCategory(nameof(SR.CatAppearance)),
        RefreshProperties(RefreshProperties.All),
        SRDescription(nameof(SR.CheckBoxCheckedDescr))
        ]
        public bool Checked {
            get {
                return CheckState != CheckState.Unchecked;
            }

            set {
                if (value != Checked) {
                    CheckState = value ? CheckState.Checked : CheckState.Unchecked;
                    InvokePaint();
                }
            }
        }


        /// <devdoc>
        /// Keeps a shared copy of the checked image between all menu items
        /// Fishes out the appropriate one based on CheckState.
        /// </devdoc>
        internal Image CheckedImage {
            [ResourceExposure(ResourceScope.Machine)]
            [ResourceConsumption(ResourceScope.Machine)]
            get {
                CheckState checkedState = CheckState;

                if (checkedState == CheckState.Indeterminate) {
                    if (indeterminateCheckedImage == null) {
                        if (DpiHelper.IsScalingRequirementMet) {
                            indeterminateCheckedImage = GetBitmapFromIcon("IndeterminateChecked.ico", scaledCheckMarkBitmapSize);
                        }
                        else {
                            Bitmap indeterminateCheckedBmp = new Bitmap(typeof(ToolStripMenuItem), "IndeterminateChecked.bmp");
                            if (indeterminateCheckedBmp != null) {
                                // 
                                indeterminateCheckedBmp.MakeTransparent(indeterminateCheckedBmp.GetPixel(1, 1));
                                if (DpiHelper.IsScalingRequired) {
                                    DpiHelper.ScaleBitmapLogicalToDevice(ref indeterminateCheckedBmp);
                                }
                                indeterminateCheckedImage = indeterminateCheckedBmp;
                            }
                        }
                    }
                    return indeterminateCheckedImage;
                }
                else if (checkedState == CheckState.Checked) {
                    if (checkedImage == null) {
                        if (DpiHelper.IsScalingRequirementMet) {
                            checkedImage = GetBitmapFromIcon("Checked.ico", scaledCheckMarkBitmapSize);
                        }
                        else {
                            Bitmap checkedBmp = new Bitmap(typeof(ToolStripMenuItem), "Checked.bmp");
                            if (checkedBmp != null) {
                                // 
                                checkedBmp.MakeTransparent(checkedBmp.GetPixel(1, 1));
                                if (DpiHelper.IsScalingRequired) {
                                    DpiHelper.ScaleBitmapLogicalToDevice(ref checkedBmp);
                                }
                                checkedImage = checkedBmp;
                            }
                        }
                    }
                    return checkedImage;
                }
                return null;

            }
        }

        private static Bitmap GetBitmapFromIcon(string iconName, Size desiredIconSize)
        {
            Bitmap b = null;

            Icon icon = new Icon(typeof(ToolStripMenuItem), iconName);
            if (icon != null)
            {
                Icon desiredIcon = new Icon(icon, desiredIconSize);
                if (desiredIcon != null)
                {
                    try
                    {
                        b = desiredIcon.ToBitmap();

                        if (b != null)
                        {
                            b.MakeTransparent(b.GetPixel(1, 1));
                            if (DpiHelper.IsScalingRequired && (b.Size.Width != desiredIconSize.Width || b.Size.Height != desiredIconSize.Height))
                            {
                                Bitmap scaledBitmap = DpiHelper.CreateResizedBitmap(b, desiredIconSize);
                                if (scaledBitmap != null)
                                {
                                    b.Dispose();
                                    b = scaledBitmap;
                                }
                            }
                        }
                    }
                    finally
                    {
                        icon.Dispose();
                        desiredIcon.Dispose();
                    }
                }
            }

            return b;
        }

         /// <include file='doc\WinBarMenuItem.uex' path='docs/doc[@for="ToolStripMenuItem.CheckOnClick"]/*' />
         [
         DefaultValue(false),
         SRCategory(nameof(SR.CatBehavior)),
         SRDescription(nameof(SR.ToolStripButtonCheckOnClickDescr))
         ]
         public bool CheckOnClick {
             get {
                 return checkOnClick;
             }
             set {
                 checkOnClick = value;
             }
         }

        /// <include file='doc\ToolStripMenuItem.uex' path='docs/doc[@for="ToolStripMenuItem.CheckState"]/*' />
        /// <devdoc>
        /// <para>Gets
        /// or sets a value indicating whether the check box is checked.</para>
        /// </devdoc>
        [
        Bindable(true),
        SRCategory(nameof(SR.CatAppearance)),
        DefaultValue(CheckState.Unchecked),
        RefreshProperties(RefreshProperties.All),
        SRDescription(nameof(SR.CheckBoxCheckStateDescr))
        ]
        public CheckState CheckState {
            get {
                bool found = false;
                object checkState = Properties.GetInteger(PropCheckState, out found);
                return (found) ? (CheckState)checkState : CheckState.Unchecked;
            }

            set {
                //valid values are 0x0 to 0x2
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)CheckState.Unchecked, (int)CheckState.Indeterminate))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(CheckState));
                }
                  
                if (value != CheckState) {
                    Properties.SetInteger(PropCheckState, (int)value);
                    OnCheckedChanged(EventArgs.Empty);
                    OnCheckStateChanged(EventArgs.Empty);               
                }
            }
        }
        
        /// <include file='doc\ToolStripMenuItem.uex' path='docs/doc[@for="ToolStripMenuItem.CheckedChanged"]/*' />
        /// <devdoc>
        /// <para>Occurs when the
        /// value of the <see cref='System.Windows.Forms.CheckBox.Checked'/>
        /// property changes.</para>
        /// </devdoc>
        [SRDescription(nameof(SR.CheckBoxOnCheckedChangedDescr))]
        public event EventHandler CheckedChanged {
            add {
                Events.AddHandler(EventCheckedChanged, value);
            }
            remove {
                Events.RemoveHandler(EventCheckedChanged, value);
            }
        }   
        /// <include file='doc\ToolStripMenuItem.uex' path='docs/doc[@for="ToolStripMenuItem.CheckStateChanged"]/*' />
        /// <devdoc>
        /// <para>Occurs when the
        /// value of the <see cref='System.Windows.Forms.CheckBox.CheckState'/>
        /// property changes.</para>
        /// </devdoc>
        [SRDescription(nameof(SR.CheckBoxOnCheckStateChangedDescr))]
        public event EventHandler CheckStateChanged {
            add {
                Events.AddHandler(EventCheckStateChanged, value);
            }
            remove {
                Events.RemoveHandler(EventCheckStateChanged, value);
            }
        }


        /// <include file='doc\ToolStripMenuItem.uex' path='docs/doc[@for="ToolStripMenuItem.Overflow"]/*' />
        /// <devdoc>
        /// <para>Specifies whether or not the item is glued to the winbar or overflow or
        /// can float between the two.</para>
        /// </devdoc>
        [
        DefaultValue(ToolStripItemOverflow.Never),
        SRDescription(nameof(SR.ToolStripItemOverflowDescr)),
        SRCategory(nameof(SR.CatLayout))
     	]
        public new ToolStripItemOverflow Overflow {
            get { 
                return base.Overflow;
            }
            set { 
                base.Overflow = value;
            }
        }

        /// <include file='doc\ToolStripMenuItem.uex' path='docs/doc[@for="ToolStripMenuItem.ShortcutKeys"]/*' />
        /// <devdoc>
        /// <para>
        /// Gets or sets the shortcut keys associated with the menu
        /// item.
        /// </para>
        /// </devdoc>
        [
        Localizable(true),
        DefaultValue(Keys.None),
        SRDescription(nameof(SR.MenuItemShortCutDescr))
        ]
        public Keys ShortcutKeys {
            get {
                bool found = false;
                object shortcutKeys = Properties.GetInteger(PropShortcutKeys, out found );
                return (found) ? (Keys)shortcutKeys : Keys.None;
            }
            set {
                if ((value != Keys.None) && !ToolStripManager.IsValidShortcut(value)){
                    // prevent use of alt, ctrl, shift modifiers with no key code.
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(Keys));
                }
                Keys originalShortcut = ShortcutKeys;
                if (originalShortcut != value) {
                    ClearShortcutCache();
                    ToolStrip owner = this.Owner;
                    if (owner != null) {
                        // add to the shortcut caching system.
                        if (originalShortcut != Keys.None) {
                            owner.Shortcuts.Remove(originalShortcut);
                        }
                        if (owner.Shortcuts.Contains(value)) {
                            // last one in wins.
                            owner.Shortcuts[value] = this;
                        }
                        else {
                            owner.Shortcuts.Add(value, this);
                        }
                    }
                    Properties.SetInteger(PropShortcutKeys, (int)value);
                    
                    if (ShowShortcutKeys && IsOnDropDown) { 
                        ToolStripDropDownMenu parent = GetCurrentParentDropDown() as ToolStripDropDownMenu;
                        if (parent != null) {
                            LayoutTransaction.DoLayout(this.ParentInternal, this, "ShortcutKeys");
                            parent.AdjustSize();
                        }
                    }
                }
            }
            
        }

        [
        SRDescription(nameof(SR.ToolStripMenuItemShortcutKeyDisplayStringDescr)),
        SRCategory(nameof(SR.CatAppearance)),
        DefaultValue(null),
        Localizable(true)
        ]
        public string ShortcutKeyDisplayString {
            get {
                return shortcutKeyDisplayString;
            }
            set {
                if (shortcutKeyDisplayString != value) {
                    shortcutKeyDisplayString = value;
                    ClearShortcutCache();
                    if (ShowShortcutKeys) {
                        ToolStripDropDown parent = this.ParentInternal as ToolStripDropDown;
                        if (parent != null) {
                            LayoutTransaction.DoLayout(parent, this, "ShortcutKeyDisplayString");
                            parent.AdjustSize();
                        }
                    }
                }
            }
        }
                    
        /// <include file='doc\ToolStripMenuItem.uex' path='docs/doc[@for="ToolStripMenuItem.ShowShortcutKeys"]/*' />
        /// <devdoc>
        /// <para>
        /// Gets or sets a value that indicates whether the shortcut
        /// keys that are assocaited
        /// with the menu item are displayed next to the menu item
        /// caption.
        /// </para>
        /// </devdoc>
        [
        DefaultValue(true),
        Localizable(true),
        SRDescription(nameof(SR.MenuItemShowShortCutDescr))
        ]
        public bool ShowShortcutKeys {
            get {
                return showShortcutKeys;
            }
            set {
                if (value != showShortcutKeys) {
                   ClearShortcutCache();
                   showShortcutKeys = value;
                   ToolStripDropDown parent = this.ParentInternal as ToolStripDropDown;
                   if (parent != null) {
                        LayoutTransaction.DoLayout(parent, this, "ShortcutKeys");
                        parent.AdjustSize();
                       
                    }
                }
            }
        }


        /// <devdoc>
        /// An item is toplevel if it is parented to anything other than a ToolStripDropDownMenu
        /// This implies that a ToolStripMenuItem in an overflow IS a toplevel item
        /// </devdoc>
        internal bool IsTopLevel {
            get {
                return (this.ParentInternal as ToolStripDropDown == null);
            }
        }

        [Browsable(false)]
        public bool IsMdiWindowListEntry {
            get{
                return MdiForm != null;
            }
        }

        internal static MenuTimer MenuTimer {
            get {
                return menuTimer;
            }
        }
        

        /// <devdoc> Tag property for internal use </devdoc>
        internal Form MdiForm {
            get {
                if (Properties.ContainsObject(PropMdiForm)) {
                    return Properties.GetObject(PropMdiForm) as Form;
                }
                return null;
                
            }
        }

        internal ToolStripMenuItem Clone() {

            // dirt simple clone - just properties, no subitems
            
            ToolStripMenuItem menuItem = new ToolStripMenuItem();
            menuItem.Events.AddHandlers(this.Events);

            menuItem.AccessibleName = this.AccessibleName;
            menuItem.AccessibleRole = this.AccessibleRole;
            menuItem.Alignment = this.Alignment;
            menuItem.AllowDrop = this.AllowDrop;
            menuItem.Anchor = this.Anchor;
            menuItem.AutoSize = this.AutoSize;
            menuItem.AutoToolTip = this.AutoToolTip;
            menuItem.BackColor = this.BackColor;
            menuItem.BackgroundImage = this.BackgroundImage;
            menuItem.BackgroundImageLayout = this.BackgroundImageLayout;
            menuItem.Checked = this.Checked;
            menuItem.CheckOnClick = this.CheckOnClick;
            menuItem.CheckState = this.CheckState;
            menuItem.DisplayStyle = this.DisplayStyle;
            menuItem.Dock = this.Dock;
            menuItem.DoubleClickEnabled = this.DoubleClickEnabled;
            menuItem.Enabled = this.Enabled;
            menuItem.Font = this.Font;
            menuItem.ForeColor = this.ForeColor;
            menuItem.Image = this.Image;
            menuItem.ImageAlign = this.ImageAlign;
            menuItem.ImageScaling = this.ImageScaling;
            menuItem.ImageTransparentColor = this.ImageTransparentColor;
            menuItem.Margin = this.Margin;
            menuItem.MergeAction = this.MergeAction;
            menuItem.MergeIndex = this.MergeIndex;
            menuItem.Name = this.Name;
            menuItem.Overflow = this.Overflow;
            menuItem.Padding = this.Padding;
            menuItem.RightToLeft= this.RightToLeft;

            // No settings support for cloned items.
            // menuItem.SaveSettings= this.SaveSettings;
            // menuItem.SettingsKey = this.SettingsKey;

            menuItem.ShortcutKeys = this.ShortcutKeys;
            menuItem.ShowShortcutKeys = this.ShowShortcutKeys;
            menuItem.Tag = this.Tag;
            menuItem.Text = this.Text;
            menuItem.TextAlign = this.TextAlign;
            menuItem.TextDirection = this.TextDirection;
            menuItem.TextImageRelation = this.TextImageRelation;
            menuItem.ToolTipText = this.ToolTipText;

            // cant actually use "Visible" property as that returns whether or not the parent 
            // is visible too.. instead use ParticipatesInLayout as this queries the actual state.
            menuItem.Visible = ((IArrangedElement)this).ParticipatesInLayout;

            if (!AutoSize) {
                menuItem.Size = this.Size;
            }
            return menuItem;
       }

        protected override void Dispose(bool disposing) {
            if (disposing) {
                if (lastOwner != null) {
                    Keys shortcut = this.ShortcutKeys;
                    if (shortcut != Keys.None && lastOwner.Shortcuts.ContainsKey(shortcut)) {
                        lastOwner.Shortcuts.Remove(shortcut);
                    }
                    lastOwner = null;
                    if (MdiForm != null) {
                        Properties.SetObject(PropMdiForm,null);
                    }
                        
                }
            }
            base.Dispose(disposing);
        }

        private bool GetNativeMenuItemEnabled() {
            if (nativeMenuCommandID == -1 || nativeMenuHandle == IntPtr.Zero) {
                Debug.Fail("why were we called to fetch native menu item info with nothing assigned?");
                return false;
            }
            NativeMethods.MENUITEMINFO_T_RW info = new NativeMethods.MENUITEMINFO_T_RW();
            info.cbSize = Marshal.SizeOf(typeof(NativeMethods.MENUITEMINFO_T_RW));
            info.fMask = NativeMethods.MIIM_STATE;
            info.fType = NativeMethods.MIIM_STATE;
            info.wID = nativeMenuCommandID;
            UnsafeNativeMethods.GetMenuItemInfo(new HandleRef(this, nativeMenuHandle), nativeMenuCommandID, /*fByPosition instead of ID=*/ false, info);

            return ((info.fState & NativeMethods.MFS_DISABLED) == 0);
        }

        // returns text and shortcut separated by tab.
        private string GetNativeMenuItemTextAndShortcut() {
            
            if (nativeMenuCommandID == -1 || nativeMenuHandle == IntPtr.Zero) {
                Debug.Fail("why were we called to fetch native menu item info with nothing assigned?");
                return null;
            }
            string text = null;
            
            // fetch the string length
            NativeMethods.MENUITEMINFO_T_RW info = new NativeMethods.MENUITEMINFO_T_RW();
            info.fMask = NativeMethods.MIIM_STRING;
            info.fType = NativeMethods.MIIM_STRING;
            info.wID = nativeMenuCommandID;
            info.dwTypeData = IntPtr.Zero;
            UnsafeNativeMethods.GetMenuItemInfo(new HandleRef(this, nativeMenuHandle), nativeMenuCommandID, /*fByPosition instead of ID=*/  false, info);

            if (info.cch > 0) {
                // fetch the string
                info.cch += 1;  // according to MSDN we need to increment the count we receive by 1.
                info.wID = nativeMenuCommandID;
                IntPtr allocatedStringBuffer = Marshal.AllocCoTaskMem(info.cch * sizeof(char));
                info.dwTypeData = allocatedStringBuffer;
                

                try {
                    UnsafeNativeMethods.GetMenuItemInfo(new HandleRef(this, nativeMenuHandle), nativeMenuCommandID, /*fByPosition instead of ID=*/  false, info);

                    // convert the string into managed data.
                	if (info.dwTypeData != IntPtr.Zero){
                        // we have to use PtrToStringAuto as we can't use Marshal.SizeOf to determine
                        // the size of the struct with a StringBuilder member.
                        text = Marshal.PtrToStringAuto(info.dwTypeData, info.cch);
                    }
                }
                finally {
                   if (allocatedStringBuffer != IntPtr.Zero) {
                        // use our local instead of the info structure member *just* in case windows decides to clobber over it.
                        // we want to be sure to deallocate the memory we know we allocated.
                        Marshal.FreeCoTaskMem(allocatedStringBuffer);
                    }
                }
            }
            return text;
        }

        [ResourceExposure(ResourceScope.Machine)]
        [ResourceConsumption(ResourceScope.Machine | ResourceScope.Process, ResourceScope.Machine | ResourceScope.Process)]
        private Image GetNativeMenuItemImage(){

            if (nativeMenuCommandID == -1 || nativeMenuHandle == IntPtr.Zero) {
                Debug.Fail("why were we called to fetch native menu item info with nothing assigned?");
                return null;
            }
            
            NativeMethods.MENUITEMINFO_T_RW info = new NativeMethods.MENUITEMINFO_T_RW();
            info.fMask = NativeMethods.MIIM_BITMAP;
            info.fType = NativeMethods.MIIM_BITMAP;
            info.wID = nativeMenuCommandID;
            UnsafeNativeMethods.GetMenuItemInfo(new HandleRef(this, nativeMenuHandle), nativeMenuCommandID, /*fByPosition instead of ID=*/ false, info);
      
            if (info.hbmpItem != IntPtr.Zero && info.hbmpItem.ToInt32() > NativeMethods.HBMMENU_POPUP_MINIMIZE) {
                return Bitmap.FromHbitmap(info.hbmpItem);
            }
            else {   
                // its a system defined bitmap
                int buttonToUse = -1;

                switch (info.hbmpItem.ToInt32()) {
                    case NativeMethods.HBMMENU_MBAR_CLOSE:
                    case NativeMethods.HBMMENU_MBAR_CLOSE_D:
                    case NativeMethods.HBMMENU_POPUP_CLOSE:
                        buttonToUse = (int)CaptionButton.Close;
                        break;

                    case NativeMethods.HBMMENU_MBAR_MINIMIZE:
                    case NativeMethods.HBMMENU_MBAR_MINIMIZE_D:
                    case NativeMethods.HBMMENU_POPUP_MINIMIZE:
                        buttonToUse = (int)CaptionButton.Minimize;
                        break;

                    case NativeMethods.HBMMENU_MBAR_RESTORE:
                    case NativeMethods.HBMMENU_POPUP_RESTORE:
                        buttonToUse = (int)CaptionButton.Restore;
                        break;

                    case NativeMethods.HBMMENU_POPUP_MAXIMIZE:
                        buttonToUse = (int)CaptionButton.Maximize;
                        break;

                    case NativeMethods.HBMMENU_SYSTEM:
                        // 
                    case NativeMethods.HBMMENU_CALLBACK:
                        // owner draw not supported
                        default:
                        break;
                }
                if (buttonToUse > -1) {

                    // we've mapped to a system defined bitmap we know how to draw
                    Bitmap image = new Bitmap(16, 16);

                    using (Graphics g = Graphics.FromImage(image)) {
                        ControlPaint.DrawCaptionButton(g, new Rectangle(Point.Empty, image.Size), (CaptionButton)buttonToUse, ButtonState.Flat);
                        g.DrawRectangle(SystemPens.Control, 0, 0, image.Width - 1, image.Height - 1);
                    }

                    image.MakeTransparent(SystemColors.Control);
                    return image;
                }
            }
            return null;
        }

        
        internal Size GetShortcutTextSize() {
             if (!ShowShortcutKeys) {
                return Size.Empty;
             }
             string shortcutString = GetShortcutText();
             if (string.IsNullOrEmpty(shortcutString)) {
                return Size.Empty;
             }
             else if (cachedShortcutSize == Size.Empty) {
                cachedShortcutSize = TextRenderer.MeasureText(shortcutString, Font);
             }
             return cachedShortcutSize;
        }

        internal string GetShortcutText() {
            if (cachedShortcutText == null) {
                cachedShortcutText = ShortcutToText(this.ShortcutKeys, this.ShortcutKeyDisplayString);                 
            }
            return cachedShortcutText;
        }
        
        internal void HandleAutoExpansion() {
            if (Enabled && ParentInternal != null && ParentInternal.MenuAutoExpand && HasDropDownItems) {
                ShowDropDown();
                if (!AccessibilityImprovements.UseLegacyToolTipDisplay) {
                    KeyboardToolTipStateMachine.Instance.NotifyAboutLostFocus(this);
                }
                DropDown.SelectNextToolStripItem(null, /*forward=*/true);
            }
        }

        /// <include file='doc\WinBarMenuItem.uex' path='docs/doc[@for="ToolStripMenuItem.OnClick"]/*' />
        protected override void OnClick(EventArgs e) {
            if (checkOnClick) {
                this.Checked = !this.Checked;
            }
            base.OnClick(e);
            if (nativeMenuCommandID != -1) {
                // fire off the appropriate native handler by posting a message to the window target.
                if ((nativeMenuCommandID & 0xF000) != 0) {
                    // These are system menu items like Minimize, Maximize, Restore, Resize, Move, Close.
                  
                    // use PostMessage instead of SendMessage so that the DefWndProc can appropriately handle
                    // the system message... if we use SendMessage the dismissal of our window
                    // breaks things like the modal sizing loop.
                    UnsafeNativeMethods.PostMessage( new HandleRef(this, targetWindowHandle), NativeMethods.WM_SYSCOMMAND,nativeMenuCommandID, 0);
                }
                else {
                    // These are user added items like ".Net Window..."
                     
                    // be consistent with sending a WM_SYSCOMMAND, use POST not SEND.
                    UnsafeNativeMethods.PostMessage( new HandleRef(this, targetWindowHandle), NativeMethods.WM_COMMAND, nativeMenuCommandID, 0);
                }
                this.Invalidate();
            }
           
        }

        /// <include file='doc\ToolStripMenuItem.uex' path='docs/doc[@for="ToolStripMenuItem.OnCheckedChanged"]/*' />
        /// <devdoc>
        /// <para>Raises the <see cref='System.Windows.Forms.ToolStripMenuItem.CheckedChanged'/>
        /// event.</para>
        /// </devdoc>
        protected virtual void OnCheckedChanged(EventArgs e) {
            EventHandler handler = (EventHandler)Events[EventCheckedChanged];
            if (handler != null) handler(this,e);
        }

        /// <include file='doc\ToolStripMenuItem.uex' path='docs/doc[@for="ToolStripMenuItem.OnCheckStateChanged"]/*' />
        /// <devdoc>
        /// <para>Raises the <see cref='System.Windows.Forms.ToolStripMenuItem.CheckStateChanged'/> event.</para>
        /// </devdoc>
        protected virtual void OnCheckStateChanged(EventArgs e) {     
            AccessibilityNotifyClients(AccessibleEvents.StateChange);
            EventHandler handler = (EventHandler)Events[EventCheckStateChanged];
            if (handler != null) handler(this,e);
        }

        /// <include file='doc\ToolStripMenuItem.uex' path='docs/doc[@for="ToolStripMenuItem.OnDropDownHide"]/*' />
        protected override void OnDropDownHide(EventArgs e) {
        
            Debug.WriteLineIf(ToolStrip.MenuAutoExpandDebug.TraceVerbose, "[ToolStripMenuItem.OnDropDownHide] MenuTimer.Cancel called");
            MenuTimer.Cancel(this);
            base.OnDropDownHide(e);
        }

        protected override void OnDropDownShow(EventArgs e) {
            // if someone has beaten us to the punch by arrowing around
            // cancel the current menu timer.            
            Debug.WriteLineIf(ToolStrip.MenuAutoExpandDebug.TraceVerbose, "[ToolStripMenuItem.OnDropDownShow] MenuTimer.Cancel called");
            MenuTimer.Cancel(this);
            if (ParentInternal != null) {
                ParentInternal.MenuAutoExpand = true;
            }
            base.OnDropDownShow(e);
        }

        protected override void OnFontChanged(EventArgs e) {
            ClearShortcutCache();
            base.OnFontChanged(e);
        }
        /// <devdoc/><internalonly/>
        internal void OnMenuAutoExpand() {
            this.ShowDropDown();
        }

        
        /// <include file='doc\ToolStripMenuItem.uex' path='docs/doc[@for="ToolStripMenuItem.OnMouseDown"]/*' />
        /// <devdoc/>
        protected override void OnMouseDown(MouseEventArgs e) {

            // Opening should happen on mouse down
            // we use a mouse down ID to ensure that the reshow
            
            Debug.WriteLineIf(ToolStrip.MenuAutoExpandDebug.TraceVerbose, "[ToolStripMenuItem.OnMouseDown] MenuTimer.Cancel called");
            MenuTimer.Cancel(this);
            OnMouseButtonStateChange(e, /*isMouseDown=*/true); 
             
        }

        protected override void OnMouseUp(MouseEventArgs e) {
           OnMouseButtonStateChange(e, /*isMouseDown=*/false);
           base.OnMouseUp(e);
        }

        private void OnMouseButtonStateChange(MouseEventArgs e, bool isMouseDown) {
            bool showDropDown = true;
            if (IsOnDropDown) {
              ToolStripDropDown dropDown = GetCurrentParentDropDown() as ToolStripDropDown;

              // Right click support for context menus.
              // used in ToolStripItem to determine whether to fire click OnMouseUp.
              SupportsRightClick = (dropDown.GetFirstDropDown() is ContextMenuStrip);
            }
            else {
              showDropDown = !DropDown.Visible;
              SupportsRightClick = false;
            }

            if (e.Button == MouseButtons.Left || 
              (e.Button == MouseButtons.Right && SupportsRightClick)) {
              
                if (isMouseDown && showDropDown) {
                    // opening should happen on mouse down.  
                    Debug.Assert(ParentInternal != null, "Parent is null here, not going to get accurate ID");
                    openMouseId = (ParentInternal == null) ? (byte)0: ParentInternal.GetMouseId();
                    this.ShowDropDown(/*mousePush =*/true);

                }
                else if (!isMouseDown && !showDropDown) {
                    // closing should happen on mouse up.  ensure it's not the mouse
                    // up for the mouse down we opened with.
                    Debug.Assert(ParentInternal != null, "Parent is null here, not going to get accurate ID");
                    byte closeMouseId = (ParentInternal == null) ? (byte)0: ParentInternal.GetMouseId();
                    int openedMouseID =openMouseId;
                    if (closeMouseId != openedMouseID) {
                        openMouseId = 0;  // reset the mouse id, we should never get this value from toolstrip.
                        ToolStripManager.ModalMenuFilter.CloseActiveDropDown(DropDown, ToolStripDropDownCloseReason.AppClicked);
                        Select();
                    }

                }

            }
        }

            
       
        /// <include file='doc\ToolStripMenuItem.uex' path='docs/doc[@for="ToolStripMenuItem.OnMouseEnter"]/*' />
        /// <devdoc/>
        protected override void OnMouseEnter(EventArgs e) {
           
            Debug.Assert(this.ParentInternal != null, "Why is parent null");
                            
            // If we are in a submenu pop down the submenu.		
            if (this.ParentInternal != null && this.ParentInternal.MenuAutoExpand && Selected) {
                Debug.WriteLineIf(ToolStripItem.MouseDebugging.TraceVerbose, "received mouse enter - calling drop down");

                Debug.WriteLineIf(ToolStrip.MenuAutoExpandDebug.TraceVerbose, "[ToolStripMenuItem.OnMouseEnter] MenuTimer.Cancel / MenuTimer.Start called");
                
                MenuTimer.Cancel(this);
                MenuTimer.Start(this);
                
            }
            base.OnMouseEnter(e);
        }

       
        /// <include file='doc\ToolStripMenuItem.uex' path='docs/doc[@for="ToolStripMenuItem.OnMouseLeave"]/*' />
        /// <devdoc/>
        protected override void OnMouseLeave(EventArgs e) {
              Debug.WriteLineIf(ToolStrip.MenuAutoExpandDebug.TraceVerbose, "[ToolStripMenuItem.OnMouseLeave] MenuTimer.Cancel called");
              MenuTimer.Cancel(this);
              base.OnMouseLeave(e);
        }

        protected override void OnOwnerChanged(EventArgs e) {

            Keys shortcut = this.ShortcutKeys;
            if (shortcut != Keys.None) {
                if (lastOwner != null) {
                    lastOwner.Shortcuts.Remove(shortcut);
                }

                if (Owner != null) {
                   if (Owner.Shortcuts.Contains(shortcut)) {
                        // last one in wins
                        Owner.Shortcuts[shortcut] = this;
                   }
                   else {
                        Owner.Shortcuts.Add(shortcut, this);
                   }
                   lastOwner = Owner;
                }
            }

            base.OnOwnerChanged(e);
        }

        /// <include file='doc\ToolStripMenuItem.uex' path='docs/doc[@for="ToolStripMenuItem.OnPaint"]/*' />
        /// <devdoc/>
        protected override void OnPaint(System.Windows.Forms.PaintEventArgs e) {    

            if (this.Owner != null) {
                ToolStripRenderer renderer = this.Renderer;
                Graphics g = e.Graphics;
                renderer.DrawMenuItemBackground(new ToolStripItemRenderEventArgs(g, this));

                Color textColor = SystemColors.MenuText;
                if (IsForeColorSet) {
                    textColor =  this.ForeColor;
                }
                else if (!this.IsTopLevel || (ToolStripManager.VisualStylesEnabled)) {
                    if (Selected || Pressed) {
                        textColor = SystemColors.HighlightText;
                    }
                    else {
                        textColor =  SystemColors.MenuText;
                    }
                }

                bool rightToLeft = (RightToLeft == RightToLeft.Yes);
                
                ToolStripMenuItemInternalLayout menuItemInternalLayout = this.InternalLayout as ToolStripMenuItemInternalLayout;
                if (menuItemInternalLayout != null && menuItemInternalLayout.UseMenuLayout) {
                    
                    // Support for special DropDownMenu layout
                    #if DEBUG_PAINT
                        g.DrawRectangle(Pens.Green, menuItemInternalLayout.TextRectangle);
                        g.DrawRectangle(Pens.HotPink, menuItemInternalLayout.ImageRectangle);
                        g.DrawRectangle(Pens.Black, menuItemInternalLayout.CheckRectangle);
                        g.DrawRectangle(Pens.Red, menuItemInternalLayout.ArrowRectangle);
                        g.DrawRectangle(Pens.Blue, new Rectangle(Point.Empty, new Size(-1,-1) + this.Size));        
                    #endif
                    if (CheckState != CheckState.Unchecked && menuItemInternalLayout.PaintCheck) {
                        Rectangle checkRectangle = menuItemInternalLayout.CheckRectangle;
                        if (!menuItemInternalLayout.ShowCheckMargin) {
                            checkRectangle =  menuItemInternalLayout.ImageRectangle;
                        }
                        if (checkRectangle.Width != 0) {
                            renderer.DrawItemCheck(new ToolStripItemImageRenderEventArgs(g, this, CheckedImage, checkRectangle));
                        }
                    }
                   
                   
                   if ((DisplayStyle & ToolStripItemDisplayStyle.Text) == ToolStripItemDisplayStyle.Text) { 

                        // render text AND shortcut
                        renderer.DrawItemText(new ToolStripItemTextRenderEventArgs(g, this, this.Text, InternalLayout.TextRectangle, textColor, this.Font, (rightToLeft) ? ContentAlignment.MiddleRight : ContentAlignment.MiddleLeft));
                        bool showShortCut = ShowShortcutKeys;
                        if (!DesignMode){
                            showShortCut = showShortCut && !HasDropDownItems;
                        }
                        
                        if (showShortCut) {
                            renderer.DrawItemText(new ToolStripItemTextRenderEventArgs(g, this, GetShortcutText(), InternalLayout.TextRectangle, textColor, this.Font, (rightToLeft) ? ContentAlignment.MiddleLeft : ContentAlignment.MiddleRight));
                        }
                   }

                    if (HasDropDownItems) {

                        ArrowDirection arrowDir = (rightToLeft) ? ArrowDirection.Left : ArrowDirection.Right;
                        Color arrowColor = (Selected ||Pressed) ? SystemColors.HighlightText : SystemColors.MenuText;
                        arrowColor = (Enabled) ? arrowColor : SystemColors.ControlDark;                   
                        renderer.DrawArrow(new ToolStripArrowRenderEventArgs(g, this, menuItemInternalLayout.ArrowRectangle, arrowColor, arrowDir)); 
                    }

                    if (menuItemInternalLayout.PaintImage && (DisplayStyle & ToolStripItemDisplayStyle.Image) == ToolStripItemDisplayStyle.Image && Image != null) { 
                       renderer.DrawItemImage(new ToolStripItemImageRenderEventArgs(g, this, InternalLayout.ImageRectangle));
                    }
                    
                }
                else {
                                        

                    // Toplevel item support, menu items hosted on a plain winbar dropdown
                    if ((DisplayStyle & ToolStripItemDisplayStyle.Text) == ToolStripItemDisplayStyle.Text) { 
                        renderer.DrawItemText(new ToolStripItemTextRenderEventArgs(g, this, this.Text, InternalLayout.TextRectangle, textColor, this.Font, InternalLayout.TextFormat));
                    }
                    
                    if ((DisplayStyle & ToolStripItemDisplayStyle.Image) == ToolStripItemDisplayStyle.Image && Image != null) { 
                       renderer.DrawItemImage(new ToolStripItemImageRenderEventArgs(g, this, InternalLayout.ImageRectangle));
                    }
                }

               
            } 
            
        }

        /// <include file='doc\ToolStripMenuItem.uex' path='docs/doc[@for="ToolStripMenuItem.ProcessCmdKey"]/*' />
        /// <devdoc>
        /// handle shortcut keys here.
        /// </devdoc>
        [
        SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)
        ]
        protected internal override bool ProcessCmdKey(ref Message m, Keys keyData) {

            if (Enabled && ShortcutKeys == keyData && !HasDropDownItems) {
                FireEvent(ToolStripItemEventType.Click);
                return true;
            }

            // call base here to get ESC, ALT, etc.. handling.
            return base.ProcessCmdKey(ref m, keyData);            
        }


        /// <include file='doc\ToolStripMenuItem.uex' path='docs/doc[@for="ToolStripMenuItem.ProcessMnemonic"]/*' />
        [UIPermission(SecurityAction.LinkDemand, Window=UIPermissionWindow.AllWindows)]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1720:AvoidTypeNamesInParameters")] // 'charCode' matches control.cs
        protected internal override bool ProcessMnemonic(char charCode) {
             // no need to check IsMnemonic, toolstrip.ProcessMnemonic checks this already.
             if (HasDropDownItems) {
				 Select();
                 ShowDropDown();

                 if (!AccessibilityImprovements.UseLegacyToolTipDisplay) {
                    KeyboardToolTipStateMachine.Instance.NotifyAboutLostFocus(this);
                 }

                 DropDown.SelectNextToolStripItem(null, /*forward=*/true);
                 return true;
             }
             
             return base.ProcessMnemonic(charCode);
        }

        /// <include file='doc\WinBarMenuItem.uex' path='docs/doc[@for="ToolStripMenuItem.SetBounds"]/*' />
        /// <devdoc> overridden here so we scooch over when we're in the ToolStripDropDownMenu</devdoc>
        internal protected override void SetBounds(Rectangle rect) {

            ToolStripMenuItemInternalLayout internalLayout = InternalLayout as ToolStripMenuItemInternalLayout;
            if (internalLayout != null && internalLayout.UseMenuLayout) {
               ToolStripDropDownMenu dropDownMenu = Owner as ToolStripDropDownMenu;

               // Scooch over by the padding amount.  The padding is added to 
               // the ToolStripDropDownMenu to keep the non-menu item riffraff
               // aligned to the text rectangle. When flow layout comes through to set our position
               // via IArrangedElement DEFY IT!
               if (dropDownMenu != null) {
                    rect.X -= dropDownMenu.Padding.Left;
                    rect.X = Math.Max(rect.X,0);       
               }
            }
            base.SetBounds(rect); 
        }

        /// <devdoc> this is to support routing to native menu commands </devdoc>
        internal void SetNativeTargetWindow(IWin32Window window) {
            targetWindowHandle = Control.GetSafeHandle(window);
        }
        
        /// <devdoc> this is to support routing to native menu commands </devdoc>
        internal void SetNativeTargetMenu(IntPtr hMenu) {
            nativeMenuHandle = hMenu;
        }
        internal static string ShortcutToText(Keys shortcutKeys, string shortcutKeyDisplayString) {
            if (!string.IsNullOrEmpty(shortcutKeyDisplayString)) {
                return shortcutKeyDisplayString;
            }
            else if (shortcutKeys == Keys.None) {
                return String.Empty;
            }
            else {
                return TypeDescriptor.GetConverter(typeof(Keys)).ConvertToString(shortcutKeys);
            }
        }

        internal override bool IsBeingTabbedTo() {
            if (base.IsBeingTabbedTo()) {
                return true;
            }

            if (ToolStripManager.ModalMenuFilter.InMenuMode) {
                return true;
            }

            return false;
        }

        /// <devdoc>
        /// An implementation of AccessibleChild for use with ToolStripItems        
        /// </devdoc>
        [System.Runtime.InteropServices.ComVisible(true)]        
        internal class ToolStripMenuItemAccessibleObject : ToolStripDropDownItemAccessibleObject {
            private ToolStripMenuItem ownerItem = null;

            public ToolStripMenuItemAccessibleObject(ToolStripMenuItem ownerItem) : base(ownerItem) {
              this.ownerItem = ownerItem;
            }


            public override AccessibleStates State {
                get {
                  if (ownerItem.Enabled ) {
                      AccessibleStates state = base.State;

                      if ((state & AccessibleStates.Pressed) == AccessibleStates.Pressed){
                         // for some reason menu items are never "pressed".
                         state &= ~AccessibleStates.Pressed;
                      }
                      
                      if (ownerItem.Checked) {
                         state |= AccessibleStates.Checked;
                      }
                      return state;
                  }
                  return base.State;
                }
            }
                                                
            internal override object GetPropertyValue(int propertyID) {
                if (propertyID == NativeMethods.UIA_ControlTypePropertyId) {
                    return NativeMethods.UIA_MenuItemControlTypeId;
                }
                else if (AccessibilityImprovements.Level2 && propertyID == NativeMethods.UIA_AcceleratorKeyPropertyId) {
                    return ownerItem.GetShortcutText();
                }
                else {
                    return base.GetPropertyValue(propertyID);
                }
            }
        }
    }    


    internal class MenuTimer {

        private System.Windows.Forms.Timer autoMenuExpandTimer                          = new System.Windows.Forms.Timer();
     
        // consider - weak reference?
        private ToolStripMenuItem currentItem = null;
        private ToolStripMenuItem fromItem = null;
        private bool inTransition = false;
        
        private int quickShow = 1;
        
        private int slowShow; 
        
        public MenuTimer() {
            // MenuShowDelay can be set to 0.  In this case, set to something low so it's inperceptable.
            autoMenuExpandTimer.Tick        += new EventHandler(OnTick);

            // since MenuShowDelay is registry tweakable we've gotta make sure we've got some sort
            // of interval
            slowShow =  Math.Max(quickShow, SystemInformation.MenuShowDelay);
            
        }
        // the current item to autoexpand.
        private ToolStripMenuItem CurrentItem {
            get {
                return currentItem;
            }
            set{
                Debug.WriteLineIf(ToolStrip.MenuAutoExpandDebug.TraceVerbose && currentItem != value, "[MenuTimer.CurrentItem] changed: " + ((value == null) ? "null" : value.ToString()));
                currentItem = value;                
            }
        }
        public bool InTransition {
            get { return inTransition; }
            set { inTransition = value; }
        }
          
        public void Start(ToolStripMenuItem item) {
            if (InTransition) {
                return;
            }
            StartCore(item);
        }

        private void StartCore(ToolStripMenuItem item) {
             if (item != CurrentItem) {
                Cancel(CurrentItem);
            }
            CurrentItem = item;
            if (item != null) {
                CurrentItem = item;
                autoMenuExpandTimer.Interval = item.IsOnDropDown ? slowShow : quickShow;
                autoMenuExpandTimer.Enabled = true;
            }
         }

        public void Transition(ToolStripMenuItem fromItem, ToolStripMenuItem toItem) {
            Debug.WriteLineIf(ToolStrip.MenuAutoExpandDebug.TraceVerbose, "[MenuTimer.Transition] transitioning items " + fromItem.ToString() + " " + toItem.ToString());

            if (toItem == null && InTransition) {
                Cancel();
                // in this case we're likely to have hit an item that's not a menu item
                // or nothing is selected.
                EndTransition(/*forceClose*/ true);
                return;
            }
        
            if (this.fromItem != fromItem) {
              this.fromItem = fromItem;
              CancelCore();
              StartCore(toItem);
            }
            // set up the current item to be the toItem so it will be auto expanded when complete.
            CurrentItem = toItem;
            InTransition = true;
            
        }


        public void Cancel() {
            if (InTransition) {
                return;
            }
            CancelCore();
           
        }
        ///<devdoc> cancels if and only if this item was the one that 
        ///         requested the timer
        ///</devdoc>
        public void Cancel(ToolStripMenuItem item) {
            if (InTransition) {
                return;
            }
            if (item == CurrentItem) {
                CancelCore();
           }
        }

        private void CancelCore() {
            autoMenuExpandTimer.Enabled = false;
            CurrentItem = null;
        }
        private void EndTransition(bool forceClose) {
            ToolStripMenuItem lastSelected = fromItem;
            fromItem = null; // immediately clear BEFORE we call user code.
            if (InTransition) {                
                InTransition = false;
                
                // we should roolup if the current item has changed and is selected.
                bool rollup = forceClose || (CurrentItem != null && CurrentItem != lastSelected && CurrentItem.Selected);
                if (rollup && lastSelected != null && lastSelected.HasDropDownItems) {
                    lastSelected.HideDropDown();                         
                }

            }

        }
        internal void HandleToolStripMouseLeave(ToolStrip toolStrip) {
            if (InTransition && toolStrip == fromItem.ParentInternal) {
                // restore the selection back to CurrentItem.
                // we're about to fall off the edge of the toolstrip, something should be selected
                // at all times while we're InTransition mode - otherwise it looks really funny
                // to have an auto expanded item 
                if (CurrentItem != null) {
                    CurrentItem.Select();
                }
            }
            else {

                // because we've split up selected/pressed, we need to make sure
                // that onmouseleave we make sure there's a selected menu item.
                if (toolStrip.IsDropDown && toolStrip.ActiveDropDowns.Count > 0) {
                    ToolStripDropDown dropDown = toolStrip.ActiveDropDowns[0] as ToolStripDropDown;
                    
                    ToolStripMenuItem menuItem = (dropDown == null) ? null : dropDown.OwnerItem as ToolStripMenuItem;               
                    if (menuItem != null && menuItem.Pressed) {
                        menuItem.Select();
                    }                       
                }
            }
        }

        private void OnTick(object sender, EventArgs e) {
            autoMenuExpandTimer.Enabled = false;
      
            if (CurrentItem == null) {
                return;
            }
            EndTransition(/*forceClose*/false);
            if (CurrentItem != null && !CurrentItem.IsDisposed &&  CurrentItem.Selected && CurrentItem.Enabled && ToolStripManager.ModalMenuFilter.InMenuMode) {
                Debug.WriteLineIf(ToolStrip.MenuAutoExpandDebug.TraceVerbose, "[MenuTimer.OnTick] calling OnMenuAutoExpand");
                CurrentItem.OnMenuAutoExpand();
            }
        }
 

    }

    internal class ToolStripMenuItemInternalLayout : ToolStripItemInternalLayout {
        private ToolStripMenuItem ownerItem;
        
        public ToolStripMenuItemInternalLayout(ToolStripMenuItem ownerItem) : base(ownerItem) {
            this.ownerItem = ownerItem;    
        }

        public bool ShowCheckMargin {
            get{
                ToolStripDropDownMenu menu = ownerItem.Owner as ToolStripDropDownMenu;
                if (menu != null) {
                    return menu.ShowCheckMargin;
                }
                return false;
            }
        }
        public bool ShowImageMargin {
            get{
                ToolStripDropDownMenu menu = ownerItem.Owner as ToolStripDropDownMenu;
                if (menu != null) {
                    return menu.ShowImageMargin;
                }
                return false;
            }
        }

        public bool PaintCheck {
            get {
                return ShowCheckMargin || ShowImageMargin;
            }
        }
        
        public bool PaintImage {
            get {
                return ShowImageMargin;
            }
        }
        public  Rectangle ArrowRectangle {
           get {
               if (UseMenuLayout) {
                   ToolStripDropDownMenu menu = ownerItem.Owner as ToolStripDropDownMenu;
                   if (menu != null) {
                        // since menuItem.Padding isnt taken into consideration, we've got to recalc the centering of
                        // the arrow rect per item
                        Rectangle arrowRect = menu.ArrowRectangle;
                        arrowRect.Y = LayoutUtils.VAlign(arrowRect.Size, ownerItem.ClientBounds, ContentAlignment.MiddleCenter).Y;
                        return arrowRect;
                   }
               }
               return Rectangle.Empty;
           }
        }
        public Rectangle CheckRectangle {
            get {
                 if (UseMenuLayout) {
                     ToolStripDropDownMenu menu = ownerItem.Owner as ToolStripDropDownMenu;
                     if (menu != null) {
                         Rectangle checkRectangle = menu.CheckRectangle;
                         if (ownerItem.CheckedImage != null) {
                             int imageHeight = ownerItem.CheckedImage.Height;
                             // make sure we're vertically centered
                             checkRectangle.Y += (checkRectangle.Height - imageHeight)/2;
                             checkRectangle.Height = imageHeight;
                             return checkRectangle;
                         }
                     }
                 }
                 return Rectangle.Empty;
            }
        }
        public override Rectangle ImageRectangle {
            get {
                if (UseMenuLayout) {
                    ToolStripDropDownMenu menu = ownerItem.Owner as ToolStripDropDownMenu;
                    if (menu != null) {

                        // since menuItem.Padding isnt taken into consideration, we've got to recalc the centering of
                        // the image rect per item
                        Rectangle imageRect = menu.ImageRectangle;
                        if (ownerItem.ImageScaling == ToolStripItemImageScaling.SizeToFit) {
                            imageRect.Size = menu.ImageScalingSize;
                        }
                        else {
                            //If we don't have an image, use the CheckedImage
                            Image image = ownerItem.Image ?? ownerItem.CheckedImage;
                            imageRect.Size = image.Size;
                        }
                        imageRect.Y = LayoutUtils.VAlign(imageRect.Size, ownerItem.ClientBounds, ContentAlignment.MiddleCenter).Y;
                        return imageRect;
                    }
                }
                return base.ImageRectangle;
            }
        }
 
        public override Rectangle TextRectangle {
            get {
                if (UseMenuLayout) {
                    ToolStripDropDownMenu menu = ownerItem.Owner as ToolStripDropDownMenu;
                    if (menu != null) {
                        return menu.TextRectangle;
                    }
                }
                return base.TextRectangle;
            }
        }
        
        public bool UseMenuLayout {
            get {
                return  ownerItem.Owner is ToolStripDropDownMenu;
            }
        }

        public override Size GetPreferredSize(Size constrainingSize) {
            if (UseMenuLayout) {
                ToolStripDropDownMenu menu = ownerItem.Owner as ToolStripDropDownMenu;
                if (menu != null) {
                    return menu.MaxItemSize;
                }
            }
            return base.GetPreferredSize(constrainingSize);
        }
    }
  
 }
