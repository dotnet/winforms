// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {
    using System.Runtime.Serialization.Formatters;
    using System.Runtime.InteropServices;
    using System.Runtime.Remoting;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Diagnostics;
    using System;
    using System.Drawing;
    using System.Text;
    using System.Drawing.Design;
    using Marshal = System.Runtime.InteropServices.Marshal;
    using System.Windows.Forms;    
    using Microsoft.Win32;
    using System.Globalization;


    /// <include file='doc\ToolBarButton.uex' path='docs/doc[@for="ToolBarButton"]/*' />
    /// <devdoc>
    ///    <para> 
    ///       Represents a Windows toolbar button.</para>
    /// </devdoc>
    [
    Designer("System.Windows.Forms.Design.ToolBarButtonDesigner, " + AssemblyRef.SystemDesign),
    DefaultProperty(nameof(Text)),
    ToolboxItem(false),
    DesignTimeVisible(false),
    ]
    public class ToolBarButton : Component {

        string text;
        string name = null;
        string tooltipText;
        bool enabled = true;
        bool visible = true;
        bool pushed = false;
        bool partialPush = false;
        private int commandId = -1; // the cached command id of the button.
        private ToolBarButtonImageIndexer imageIndexer; 

        ToolBarButtonStyle style = ToolBarButtonStyle.PushButton;

        object userData;

        // These variables below are used by the ToolBar control to help
        // it manage some information about us.

        /// <include file='doc\ToolBarButton.uex' path='docs/doc[@for="ToolBarButton.stringIndex"]/*' />
        /// <devdoc>
        ///     If this button has a string, what it's index is in the ToolBar's
        ///     internal list of strings.  Needs to be package protected.
        /// </devdoc>
        /// <internalonly/>
        internal IntPtr stringIndex = (IntPtr)(-1);

        /// <include file='doc\ToolBarButton.uex' path='docs/doc[@for="ToolBarButton.parent"]/*' />
        /// <devdoc>
        ///     Our parent ToolBar control.
        /// </devdoc>
        /// <internalonly/>
        internal ToolBar parent;

        /// <include file='doc\ToolBarButton.uex' path='docs/doc[@for="ToolBarButton.dropDownMenu"]/*' />
        /// <devdoc>
        ///     For DropDown buttons, we can optionally show a
        ///     context menu when the button is dropped down.
        /// </devdoc>
        internal Menu dropDownMenu = null;

        /// <include file='doc\ToolBarButton.uex' path='docs/doc[@for="ToolBarButton.ToolBarButton"]/*' />
        /// <devdoc>
        /// <para>Initializes a new instance of the <see cref='System.Windows.Forms.ToolBarButton'/> class.</para>
        /// </devdoc>
        public ToolBarButton() {
        }
        
        /// <include file='doc\ToolBarButton.uex' path='docs/doc[@for="ToolBarButton.ToolBarButton1"]/*' />
        public ToolBarButton(string text) : base() {
            this.Text = text;
        }


        
        // We need a special way to defer to the ToolBar's image
        // list for indexing purposes.
        internal class ToolBarButtonImageIndexer : ImageList.Indexer {
            private ToolBarButton owner;
           


           public ToolBarButtonImageIndexer(ToolBarButton button) {
              owner = button;
           }
           
           public override ImageList ImageList {
                get { 
                        if ((owner != null) && (owner.parent != null)) {
                            return owner.parent.ImageList;
                        }
                        return null;
                    }
                set { Debug.Assert(false, "We should never set the image list"); }
            }
        }

        internal ToolBarButtonImageIndexer ImageIndexer {
            get { 
                   if (imageIndexer == null) {
                         imageIndexer = new ToolBarButtonImageIndexer(this);
                   }

                   return imageIndexer; 
            }
        }
 
        /// <include file='doc\ToolBarButton.uex' path='docs/doc[@for="ToolBarButton.DropDownMenu"]/*' />
        /// <devdoc>
        ///    <para> 
        ///       Indicates the menu to be displayed in
        ///       the drop-down toolbar button.</para>
        /// </devdoc>
        [
        DefaultValue(null),
        TypeConverterAttribute(typeof(ReferenceConverter)),
        SRDescription(nameof(SR.ToolBarButtonMenuDescr))
        ]
        public Menu DropDownMenu {
            get {
                return dropDownMenu;
            }

            set {
                //The dropdownmenu must be of type ContextMenu, Main & Items are invalid.
                //
                if (value != null && !(value is ContextMenu)) {
                    throw new ArgumentException(SR.ToolBarButtonInvalidDropDownMenuType);
                }
                dropDownMenu = value;
            }
        }

        /// <include file='doc\ToolBarButton.uex' path='docs/doc[@for="ToolBarButton.Enabled"]/*' />
        /// <devdoc>
        ///    <para>Indicates whether the button is enabled or not.</para>
        /// </devdoc>
        [
        DefaultValue(true),
        Localizable(true),
        SRDescription(nameof(SR.ToolBarButtonEnabledDescr))
        ]
        public bool Enabled {
            get {
                return enabled;
            }

            set {
                if (enabled != value) {

                    enabled = value;

                    if (parent != null && parent.IsHandleCreated) {
                        parent.SendMessage(NativeMethods.TB_ENABLEBUTTON, FindButtonIndex(),
                            enabled ? 1 : 0);
                    }
                }
            }
        }

        /// <include file='doc\ToolBarButton.uex' path='docs/doc[@for="ToolBarButton.ImageIndex"]/*' />
        /// <devdoc>
        ///    <para> Indicates the index
        ///       value of the image assigned to the button.</para>
        /// </devdoc>
        [
        TypeConverterAttribute(typeof(ImageIndexConverter)),
        Editor("System.Windows.Forms.Design.ImageIndexEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor)),
        DefaultValue(-1),
        RefreshProperties(RefreshProperties.Repaint),        
        Localizable(true),
        SRDescription(nameof(SR.ToolBarButtonImageIndexDescr))
        ]
        public int ImageIndex {
            get {
                return ImageIndexer.Index;
            }
            set {
                if (ImageIndexer.Index != value) {
                    if (value < -1)
                        throw new ArgumentOutOfRangeException(nameof(ImageIndex), string.Format(SR.InvalidLowBoundArgumentEx, "ImageIndex", (value).ToString(CultureInfo.CurrentCulture), -1));

                    ImageIndexer.Index = value;
                    UpdateButton(false);
                }
            }
        }

                /// <include file='doc\ToolBarButton.uex' path='docs/doc[@for="ToolBarButton.ImageIndex"]/*' />
        /// <devdoc>
        ///    <para> Indicates the index
        ///       value of the image assigned to the button.</para>
        /// </devdoc>
        [
        TypeConverterAttribute(typeof(ImageKeyConverter)),
        Editor("System.Windows.Forms.Design.ImageIndexEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor)),
        DefaultValue(""),
        Localizable(true),
        RefreshProperties(RefreshProperties.Repaint),
        SRDescription(nameof(SR.ToolBarButtonImageIndexDescr))
        ]
        public string ImageKey {
            get {
                return ImageIndexer.Key;
            }
            set {
                if (ImageIndexer.Key != value) {
                    ImageIndexer.Key = value;
                    UpdateButton(false);
                }
            }
        }
	    /// <include file='doc\ToolBarButton.uex' path='docs/doc[@for="ToolBarButton.Name"]/*' />
        /// <devdoc>
        ///     Name of this control. The designer will set this to the same
        ///     as the programatic Id "(name)" of the control - however this
        ///     property has no bearing on the runtime aspects of this control.
        /// </devdoc>
        [Browsable(false)]
        public string Name {
            get {
                return WindowsFormsUtils.GetComponentName(this, name);
            }
            set {
                if (value == null || value.Length == 0) {
                    name = null;
                }
                else {
                   name = value;
                }
                if(Site!= null) {
                    Site.Name = name;
                }
            }
        }


        /// <include file='doc\ToolBarButton.uex' path='docs/doc[@for="ToolBarButton.Parent"]/*' />
        /// <devdoc>
        ///    <para>Indicates the toolbar control that the toolbar button is assigned to. This property is 
        ///       read-only.</para>
        /// </devdoc>
        [
            Browsable(false),
        ]
        public ToolBar Parent {
            get {
                return parent;
            }
        }

        /// <include file='doc\ToolBarButton.uex' path='docs/doc[@for="ToolBarButton.PartialPush"]/*' />
        /// <devdoc>
        ///    <para> 
        ///       Indicates whether a toggle-style toolbar button
        ///       is partially pushed.</para>
        /// </devdoc>
        [
        DefaultValue(false),
        SRDescription(nameof(SR.ToolBarButtonPartialPushDescr))
        ]
        public bool PartialPush {
            get {
                if (parent == null || !parent.IsHandleCreated)
                    return partialPush;
                else {
                    if ((int)parent.SendMessage(NativeMethods.TB_ISBUTTONINDETERMINATE, FindButtonIndex(), 0) != 0)
                        partialPush = true;
                    else
                        partialPush = false;

                    return partialPush;
                }
            }
            set {
                if (partialPush != value) {
                    partialPush = value;
                    UpdateButton(false);
                }
            }
        }

        /// <include file='doc\ToolBarButton.uex' path='docs/doc[@for="ToolBarButton.Pushed"]/*' />
        /// <devdoc>
        ///    <para>Indicates whether a toggle-style toolbar button is currently in the pushed state.</para>
        /// </devdoc>
        [
        DefaultValue(false),
        SRDescription(nameof(SR.ToolBarButtonPushedDescr))
        ]
        public bool Pushed {
            get {
                if (parent == null || !parent.IsHandleCreated)
                    return pushed;
                else {
                    return GetPushedState();
                }
            }
            set {
                if (value != Pushed) { // Getting property Pushed updates pushed member variable
                    pushed = value;
                    UpdateButton(false, false, false);
                }
            }
        }

        /// <include file='doc\ToolBarButton.uex' path='docs/doc[@for="ToolBarButton.Rectangle"]/*' />
        /// <devdoc>
        ///    <para>Indicates the bounding rectangle for a toolbar button. This property is 
        ///       read-only.</para>
        /// </devdoc>
        public Rectangle Rectangle {
            get {
                if (parent != null) {
                    NativeMethods.RECT rc = new NativeMethods.RECT();
                    UnsafeNativeMethods.SendMessage(new HandleRef(parent, parent.Handle), NativeMethods.TB_GETRECT, FindButtonIndex(), ref rc);
                    return Rectangle.FromLTRB(rc.left, rc.top, rc.right, rc.bottom);
                }
                return Rectangle.Empty;
            }
        }

        /// <include file='doc\ToolBarButton.uex' path='docs/doc[@for="ToolBarButton.Style"]/*' />
        /// <devdoc>
        ///    <para> Indicates the style of the
        ///       toolbar button.</para>
        /// </devdoc>
        [
        DefaultValue(ToolBarButtonStyle.PushButton),
        SRDescription(nameof(SR.ToolBarButtonStyleDescr)),
        RefreshProperties(RefreshProperties.Repaint)
        ]
        public ToolBarButtonStyle Style {
            get {
                return style;
            }
            set {
                //valid values are 0x1 to 0x4
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)ToolBarButtonStyle.PushButton, (int)ToolBarButtonStyle.DropDownButton)){
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(ToolBarButtonStyle));
                }
                if (style == value) return;
                style = value;
                UpdateButton(true);
            }
        }

        /// <include file='doc\ToolBarButton.uex' path='docs/doc[@for="ToolBarButton.Tag"]/*' />
        [
        SRCategory(nameof(SR.CatData)),
        Localizable(false),
        Bindable(true),
        SRDescription(nameof(SR.ControlTagDescr)),
        DefaultValue(null),
        TypeConverter(typeof(StringConverter)),
        ]
        public object Tag {
            get {
                return userData;
            }
            set {
                userData = value;
            }
        }

        /// <include file='doc\ToolBarButton.uex' path='docs/doc[@for="ToolBarButton.Text"]/*' />
        /// <devdoc>
        ///    <para> Indicates the text that is displayed on the toolbar button.</para>
        /// </devdoc>
        [
        Localizable(true),
        DefaultValue(""),
        SRDescription(nameof(SR.ToolBarButtonTextDescr))
        ]
        public string Text {
            get {
                return(text == null) ? "" : text;
            }
            set {
                if (String.IsNullOrEmpty(value)) {
                    value = null;
                }
                
                if ( (value == null && text != null) ||
                     (value != null && (text == null || !text.Equals(value)))) {
                    text = value;
                    // Adding a mnemonic requires a handle recreate.
                    UpdateButton(WindowsFormsUtils.ContainsMnemonic(text), true, true);
                }
            }
        }

         

        /// <include file='doc\ToolBarButton.uex' path='docs/doc[@for="ToolBarButton.ToolTipText"]/*' />
        /// <devdoc>
        ///    <para> 
        ///       Indicates
        ///       the text that appears as a tool tip for a control.</para>
        /// </devdoc>
        [
        Localizable(true),
        DefaultValue(""),
        SRDescription(nameof(SR.ToolBarButtonToolTipTextDescr))
        ]
        public string ToolTipText {
            get {
                return tooltipText == null ? "" : tooltipText;
            }
            set {
                tooltipText = value;
            }
        }

        /// <include file='doc\ToolBarButton.uex' path='docs/doc[@for="ToolBarButton.Visible"]/*' />
        /// <devdoc>
        ///    <para> 
        ///       Indicates whether the toolbar button
        ///       is visible.</para>
        /// </devdoc>
        [
        DefaultValue(true),
        Localizable(true),
        SRDescription(nameof(SR.ToolBarButtonVisibleDescr))
        ]
        public bool Visible {
            get {
                return visible;
            }
            set {
                if (visible != value) {
                    visible = value;
                    UpdateButton(false);
                }
            }
        }

        /// <include file='doc\ToolBarButton.uex' path='docs/doc[@for="ToolBarButton.Width"]/*' />
        /// <devdoc>
        ///     This is somewhat nasty -- by default, the windows ToolBar isn't very
        ///     clever about setting the width of buttons, and has a very primitive
        ///     algorithm that doesn't include for things like drop down arrows, etc.
        ///     We need to do a bunch of work here to get all the widths correct. Ugh.
        /// </devdoc>
        /// <internalonly/>
        internal short Width {
            get {
                Debug.Assert(parent != null, "Parent should be non-null when button width is requested");
                
                int width = 0;
                ToolBarButtonStyle style = Style;

                Size edge = SystemInformation.Border3DSize;
                if (style != ToolBarButtonStyle.Separator) {

                    // COMPAT: this will force handle creation.  
                    // we could use the measurement graphics, but it looks like this has been like this since Everett.
                    using (Graphics g = this.parent.CreateGraphicsInternal()) {

                        Size buttonSize = this.parent.buttonSize;                                                      
                        if (!(buttonSize.IsEmpty)) {
                            width = buttonSize.Width;
                        }
                        else {
                            if (this.parent.ImageList != null || !String.IsNullOrEmpty(Text)) {
                                Size imageSize = this.parent.ImageSize;
                                Size textSize = Size.Ceiling(g.MeasureString(Text, parent.Font));
                                if (this.parent.TextAlign == ToolBarTextAlign.Right) {
                                    if (textSize.Width == 0)
                                        width = imageSize.Width + edge.Width * 4;
                                    else
                                        width = imageSize.Width + textSize.Width + edge.Width * 6;
                                }
                                else {
                                    if (imageSize.Width > textSize.Width)
                                        width = imageSize.Width + edge.Width * 4;
                                    else
                                        width = textSize.Width + edge.Width * 4;
                                }
                                if (style == ToolBarButtonStyle.DropDownButton && this.parent.DropDownArrows) {
                                    width += ToolBar.DDARROW_WIDTH;
                                }
                            }
                            else
                                width = this.parent.ButtonSize.Width;                                                      
                        }
                    }
                }
                else {
                    width = edge.Width * 2;
                }

                return(short)width;
            }

        }

        /// <include file='doc\ToolBarButton.uex' path='docs/doc[@for="ToolBarButton.Dispose"]/*' />
        /// <internalonly/>
        /// <devdoc>
        /// </devdoc>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                if (parent != null) {
                    int index = FindButtonIndex();
                    if (index != -1) {
                        parent.Buttons.RemoveAt(index);
                    }
                }
            }
            base.Dispose(disposing);
        }

        /// <include file='doc\ToolBarButton.uex' path='docs/doc[@for="ToolBarButton.FindButtonIndex"]/*' />
        /// <devdoc>
        ///     Finds out index in the parent.
        /// </devdoc>
        /// <internalonly/>
        private int FindButtonIndex() {
            for (int x = 0; x < parent.Buttons.Count; x++) {
                if (parent.Buttons[x] == this) {
                    return x;
                }
            }
            return -1;
        }

        // This is necessary to get the width of the buttons in the toolbar,
        // including the width of separators, so that we can accurately position the tooltip adjacent
        // to the currently hot button when the user uses keyboard navigation to access the toolbar.
        internal int GetButtonWidth() {

            // Assume that this button is the same width as the parent's ButtonSize's Width
            int buttonWidth = Parent.ButtonSize.Width;
            
            NativeMethods.TBBUTTONINFO button = new NativeMethods.TBBUTTONINFO();
            button.cbSize = Marshal.SizeOf(typeof(NativeMethods.TBBUTTONINFO));
            button.dwMask = NativeMethods.TBIF_SIZE;
            
            int buttonID = (int)UnsafeNativeMethods.SendMessage(new HandleRef(Parent, Parent.Handle), NativeMethods.TB_GETBUTTONINFO, commandId, ref button);
            if (buttonID != -1) {
                buttonWidth = button.cx;
            }
            
            return buttonWidth;
        }
        
        private bool GetPushedState()
        {
            if ((int)parent.SendMessage(NativeMethods.TB_ISBUTTONCHECKED, FindButtonIndex(), 0) != 0) {
                pushed = true;
            }
            else {
                pushed = false;
            }

            return pushed;
        }

        /// <include file='doc\ToolBarButton.uex' path='docs/doc[@for="ToolBarButton.GetTBBUTTON"]/*' />
        /// <devdoc>
        ///     Returns a TBBUTTON object that represents this ToolBarButton.
        /// </devdoc>
        internal NativeMethods.TBBUTTON GetTBBUTTON(int commandId) {

            NativeMethods.TBBUTTON button = new NativeMethods.TBBUTTON();

            button.iBitmap = ImageIndexer.ActualIndex;

            // set up the state of the button
            //
            button.fsState = 0;
            if (enabled) button.fsState |= NativeMethods.TBSTATE_ENABLED;
            if (partialPush && style == ToolBarButtonStyle.ToggleButton) button.fsState |= NativeMethods.TBSTATE_INDETERMINATE;
            if (pushed) button.fsState |= NativeMethods.TBSTATE_CHECKED;
            if (!visible) button.fsState |= NativeMethods.TBSTATE_HIDDEN;

            // set the button style
            //
            switch (style) {
                case ToolBarButtonStyle.PushButton:
                    button.fsStyle = NativeMethods.TBSTYLE_BUTTON;
                    break;
                case ToolBarButtonStyle.ToggleButton:
                    button.fsStyle = NativeMethods.TBSTYLE_CHECK;
                    break;
                case ToolBarButtonStyle.Separator:
                    button.fsStyle = NativeMethods.TBSTYLE_SEP;
                    break;
                case ToolBarButtonStyle.DropDownButton:
                    button.fsStyle = NativeMethods.TBSTYLE_DROPDOWN;
                    break;

            }

            button.dwData = (IntPtr)0;
            button.iString = this.stringIndex;
            this.commandId = commandId;
            button.idCommand = commandId;

            return button;
        }

        /// <include file='doc\ToolBarButton.uex' path='docs/doc[@for="ToolBarButton.GetTBBUTTONINFO"]/*' />
        /// <devdoc>
        ///     Returns a TBBUTTONINFO object that represents this ToolBarButton.
        /// </devdoc>
        internal NativeMethods.TBBUTTONINFO GetTBBUTTONINFO(bool updateText, int newCommandId) {

            NativeMethods.TBBUTTONINFO button = new NativeMethods.TBBUTTONINFO();
            button.cbSize = Marshal.SizeOf(typeof(NativeMethods.TBBUTTONINFO));
            button.dwMask = NativeMethods.TBIF_IMAGE
                            | NativeMethods.TBIF_STATE | NativeMethods.TBIF_STYLE;

            // Comctl on Win98 interprets null strings as empty strings, which forces
            // the button to leave space for text.  The only workaround is to avoid having comctl 
            // update the text.
            if (updateText) {
                button.dwMask |= NativeMethods.TBIF_TEXT;
            }

            button.iImage = ImageIndexer.ActualIndex;

            if (newCommandId != commandId) {
                commandId = newCommandId;
                button.idCommand = newCommandId;
                button.dwMask |= NativeMethods.TBIF_COMMAND;
            }

            // set up the state of the button
            //
            button.fsState = 0;
            if (enabled) button.fsState |= NativeMethods.TBSTATE_ENABLED;
            if (partialPush && style == ToolBarButtonStyle.ToggleButton) button.fsState |= NativeMethods.TBSTATE_INDETERMINATE;
            if (pushed) button.fsState |= NativeMethods.TBSTATE_CHECKED;
            if (!visible) button.fsState |= NativeMethods.TBSTATE_HIDDEN;

            // set the button style
            //
            switch (style) {
                case ToolBarButtonStyle.PushButton:
                    button.fsStyle = NativeMethods.TBSTYLE_BUTTON;
                    break;
                case ToolBarButtonStyle.ToggleButton:
                    button.fsStyle = NativeMethods.TBSTYLE_CHECK;
                    break;
                case ToolBarButtonStyle.Separator:
                    button.fsStyle = NativeMethods.TBSTYLE_SEP;
                    break;
            }


            if (text == null) {
                button.pszText = Marshal.StringToHGlobalAuto("\0\0");
            }
            else {
                string textValue = this.text;
                PrefixAmpersands(ref textValue);
                button.pszText = Marshal.StringToHGlobalAuto(textValue);
            }

            return button;
        }
        
        private void PrefixAmpersands(ref string value) {
            // Due to a comctl32 problem, ampersands underline the next letter in the 
            // text string, but the accelerators don't work.
            // So in this function, we prefix ampersands with another ampersand
            // so that they actually appear as ampersands.
            //
            
            // Sanity check parameter
            //
            if (value == null || value.Length == 0) {
                return;
            }
            
            // If there are no ampersands, we don't need to do anything here
            //
            if (value.IndexOf('&') < 0) {
                return;
            }
            
            // Insert extra ampersands
            //
            StringBuilder newString = new StringBuilder();
            for(int i=0; i < value.Length; ++i) {
                if (value[i] == '&') { 
                    if (i < value.Length - 1 && value[i+1] == '&') {
                        ++i;    // Skip the second ampersand
                    }
                    newString.Append("&&");
                }
                else {
                    newString.Append(value[i]);    
                }
            }
            
            value = newString.ToString();
        }

        /// <include file='doc\ToolBarButton.uex' path='docs/doc[@for="ToolBarButton.ToString"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        public override string ToString() {
            return "ToolBarButton: " + Text + ", Style: " + Style.ToString("G");
        }

        /// <include file='doc\ToolBarButton.uex' path='docs/doc[@for="ToolBarButton.UpdateButton"]/*' />
        /// <devdoc>
        ///     When a button property changes and the parent control is created,
        ///     we need to make sure it gets the new button information.
        ///     If Text was changed, call the next overload.
        /// </devdoc>
        internal void UpdateButton(bool recreate) {
            UpdateButton(recreate, false, true);        
        }

        /// <include file='doc\ToolBarButton.uex' path='docs/doc[@for="ToolBarButton.UpdateButton1"]/*' />
        /// <devdoc>
        ///     When a button property changes and the parent control is created,
        ///     we need to make sure it gets the new button information.
        /// </devdoc>
        private void UpdateButton(bool recreate, bool updateText, bool updatePushedState) {
            // It looks like ToolBarButtons with a DropDownButton tend to
            // lose the DropDownButton very easily - so we need to recreate
            // the button each time it changes just to be sure.
            //                                           
            if (style == ToolBarButtonStyle.DropDownButton && parent != null && parent.DropDownArrows) {
                recreate = true;
            }

            // we just need to get the Pushed state : this asks the Button its states and sets 
            // the private member "pushed" to right value..

            // this member is used in "InternalSetButton" which calls GetTBBUTTONINFO(bool updateText)
            // the GetButtonInfo method uses the "pushed" variable..

            //rather than setting it ourselves .... we asks the button to set it for us..
            if (updatePushedState && parent != null && parent.IsHandleCreated) {
                GetPushedState();
            }
            if (parent != null) {
                int index = FindButtonIndex();
                if (index != -1)
                    parent.InternalSetButton(index, this, recreate, updateText);
            }
        }    
    }
}
