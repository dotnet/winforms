// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {
    using System.Runtime.Serialization.Formatters;
    using System.Runtime.Remoting;
    using System.Runtime.InteropServices;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Diagnostics;
    using System;
    using System.Drawing;
    using System.Windows.Forms;
    using Microsoft.Win32;
    using System.Globalization;
    using System.Runtime.Versioning;

    /// <include file='doc\StatusBarPanel.uex' path='docs/doc[@for="StatusBarPanel"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Stores the <see cref='System.Windows.Forms.StatusBar'/>
    ///       control panel's information.
    ///    </para>
    /// </devdoc>
    [
    ToolboxItem(false),
    DesignTimeVisible(false),
    DefaultProperty(nameof(Text))
    ]
    public class StatusBarPanel : Component, ISupportInitialize {

        private const int DEFAULTWIDTH = 100;
        private const int DEFAULTMINWIDTH = 10;
        private const int PANELTEXTINSET = 3;
        private const int PANELGAP = 2;

        private string          text          = "";
        private string          name          = "";
        private string          toolTipText   = "";
        private Icon            icon          = null;

        private HorizontalAlignment        alignment     = HorizontalAlignment.Left;
        private System.Windows.Forms.StatusBarPanelBorderStyle  borderStyle   = System.Windows.Forms.StatusBarPanelBorderStyle.Sunken;
        private StatusBarPanelStyle        style         = StatusBarPanelStyle.Text;

        // these are package scope so the parent can get at them.
        //
        private StatusBar       parent          = null;
        private int             width           = DEFAULTWIDTH;
        private int             right           = 0;
        private int             minWidth        = DEFAULTMINWIDTH;
        private int             index           = 0;
        private StatusBarPanelAutoSize autoSize = StatusBarPanelAutoSize.None;

        private bool initializing = false;

        private object userData;

        /// <include file='doc\StatusBarPanel.uex' path='docs/doc[@for="StatusBarPanel.StatusBarPanel"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Initializes a new default instance of the <see cref='System.Windows.Forms.StatusBarPanel'/> class.
        ///    </para>
        /// </devdoc>
        public StatusBarPanel() {
        }

        /// <include file='doc\StatusBarPanel.uex' path='docs/doc[@for="StatusBarPanel.Alignment"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets the <see cref='System.Windows.Forms.StatusBarPanel.Alignment'/>
        ///       property.
        ///
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        DefaultValue(HorizontalAlignment.Left),
        Localizable(true),
        SRDescription(nameof(SR.StatusBarPanelAlignmentDescr))
        ]
        public HorizontalAlignment Alignment {
            get {
                return alignment;
            }

            set {
                //valid values are 0x0 to 0x2
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)HorizontalAlignment.Left, (int)HorizontalAlignment.Center)){
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(HorizontalAlignment));
                }
                if (alignment != value) {
                    alignment = value;
                    Realize();
                }
            }
        }

        /// <include file='doc\StatusBarPanel.uex' path='docs/doc[@for="StatusBarPanel.AutoSize"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets the <see cref='System.Windows.Forms.StatusBarPanel.AutoSize'/>
        ///       property.
        ///
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        DefaultValue(StatusBarPanelAutoSize.None),
        RefreshProperties(RefreshProperties.All),
        SRDescription(nameof(SR.StatusBarPanelAutoSizeDescr))
        ]
        public StatusBarPanelAutoSize AutoSize {
            get {
                return this.autoSize;
            }

            set {
                //valid values are 0x1 to 0x3
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)StatusBarPanelAutoSize.None, (int)StatusBarPanelAutoSize.Contents)){
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(StatusBarPanelAutoSize));
                }
                if (this.autoSize != value) {
                    this.autoSize = value;
                    UpdateSize();
                }
            }
        }


        /// <include file='doc\StatusBarPanel.uex' path='docs/doc[@for="StatusBarPanel.BorderStyle"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets the <see cref='System.Windows.Forms.StatusBarPanel.BorderStyle'/>
        ///
        ///       property.
        ///
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        DefaultValue(System.Windows.Forms.StatusBarPanelBorderStyle.Sunken),
        DispId(NativeMethods.ActiveX.DISPID_BORDERSTYLE),
        SRDescription(nameof(SR.StatusBarPanelBorderStyleDescr))
        ]
        public StatusBarPanelBorderStyle BorderStyle {
            get {
                return borderStyle;
            }

            set {
                //valid values are 0x1 to 0x3
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)StatusBarPanelBorderStyle.None, (int)StatusBarPanelBorderStyle.Sunken)){
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(StatusBarPanelBorderStyle));
                }
                if (this.borderStyle != value) {
                    this.borderStyle = value;
                    Realize();
                    if (Created)
                        this.parent.Invalidate();
                }
            }
        }

        /// <include file='doc\StatusBarPanel.uex' path='docs/doc[@for="StatusBarPanel.Created"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        internal bool Created {
            get {
                return this.parent != null && this.parent.ArePanelsRealized();
            }
        }

        /// <include file='doc\StatusBarPanel.uex' path='docs/doc[@for="StatusBarPanel.Icon"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets the <see cref='System.Windows.Forms.StatusBarPanel.Icon'/>
        ///       property.
        ///
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        DefaultValue(null),
        Localizable(true),
        SRDescription(nameof(SR.StatusBarPanelIconDescr))
        ]
        public Icon Icon {
            [ResourceExposure(ResourceScope.Machine)]
            get {
                // unfortunately we have no way of getting the icon from the control.
                return this.icon;
            }
            [ResourceExposure(ResourceScope.Machine)]
            [ResourceConsumption(ResourceScope.Machine)]
            set {

                if (value != null && (((Icon)value).Height > SystemInformation.SmallIconSize.Height || ((Icon)value).Width > SystemInformation.SmallIconSize.Width)) {
                    this.icon  = new Icon(value, SystemInformation.SmallIconSize);
                }
                else {
                    this.icon = value;
                }

                if (Created) {
                    IntPtr handle = (this.icon == null) ? IntPtr.Zero : this.icon.Handle;
                    this.parent.SendMessage(NativeMethods.SB_SETICON, (IntPtr)GetIndex(), handle);

                }
                UpdateSize();
                if (Created) {
                    this.parent.Invalidate();
                }
            }
        }

        /// <include file='doc\StatusBarPanel.uex' path='docs/doc[@for="StatusBarPanel.Index"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Expose index internally
        ///    </para>
        /// </devdoc>
        internal int Index
        {
            get
            {
                return index;
            }
            set
            {
                index = value;
            }
        }
        /// <include file='doc\StatusBarPanel.uex' path='docs/doc[@for="StatusBarPanel.MinWidth"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets the minimum width the <see cref='System.Windows.Forms.StatusBarPanel'/> can be within the <see cref='System.Windows.Forms.StatusBar'/>
        ///       control.
        ///
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(DEFAULTMINWIDTH),
        Localizable(true),
        RefreshProperties(RefreshProperties.All),
        SRDescription(nameof(SR.StatusBarPanelMinWidthDescr))
        ]
        public int MinWidth {
            get {
                return this.minWidth;
            }
            set {
                if (value < 0) {
                    throw new ArgumentOutOfRangeException(nameof(MinWidth), string.Format(SR.InvalidLowBoundArgumentEx, "MinWidth", value.ToString(CultureInfo.CurrentCulture), (0).ToString(CultureInfo.CurrentCulture)));
                }

                if (value != this.minWidth) {
                    this.minWidth = value;

                    UpdateSize();
                    if (this.minWidth > this.Width) {
                        Width = value;
                    }
                }
            }
        }

        /// <include file='doc\StatusBarPanel.uex' path='docs/doc[@for="StatusBarPanel.Name"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets the name of the panel.
        ///    </para>
        /// </devdoc>
	[
        SRCategory(nameof(SR.CatAppearance)),
        Localizable(true),
        SRDescription(nameof(SR.StatusBarPanelNameDescr))
        ]
        public string Name {
            get {
                return WindowsFormsUtils.GetComponentName(this, name);
            }
            set {
                name = value;
                if(Site!= null) {
                    Site.Name = name;
                }
            }
        }

        /// <include file='doc\StatusBarPanel.uex' path='docs/doc[@for="StatusBarPanel.Parent"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Represents the <see cref='System.Windows.Forms.StatusBar'/>
        ///       control which hosts the
        ///       panel.
        ///
        ///    </para>
        /// </devdoc>
        [Browsable(false)]
        public StatusBar Parent {
            get {
                return parent;
            }
        }

        /// <include file='doc\StatusBarPanel.uex' path='docs/doc[@for="StatusBarPanel.ParentInternal"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Expose a direct setter for parent internally
        ///    </para>
        /// </devdoc>
        internal StatusBar ParentInternal
        {
            set
            {
                parent = value;
            }
        }

        /// <include file='doc\StatusBarPanel.uex' path='docs/doc[@for="StatusBarPanel.Right"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Expose right internally
        ///    </para>
        /// </devdoc>
        internal int Right
        {
            get
            {
                return right;
            }
            set
            {
                right = value;
            }
        }

        /// <include file='doc\StatusBarPanel.uex' path='docs/doc[@for="StatusBarPanel.Style"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets the style of the panel.
        ///
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        DefaultValue(StatusBarPanelStyle.Text),
        SRDescription(nameof(SR.StatusBarPanelStyleDescr))
        ]
        public StatusBarPanelStyle Style {
            get { return style;}
            set {
                //valid values are 0x1 to 0x2
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)StatusBarPanelStyle.Text, (int)StatusBarPanelStyle.OwnerDraw)){
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(StatusBarPanelStyle));
                }
                if (this.style != value) {
                    this.style = value;
                    Realize();
                    if (Created) {
                        this.parent.Invalidate();
                    }
                }
            }
        }

        /// <include file='doc\StatusBarPanel.uex' path='docs/doc[@for="StatusBarPanel.Tag"]/*' />
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

        /// <include file='doc\StatusBarPanel.uex' path='docs/doc[@for="StatusBarPanel.Text"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets the text of the panel.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        Localizable(true),
        DefaultValue(""),
        SRDescription(nameof(SR.StatusBarPanelTextDescr))
        ]
        public string Text {
            get {
                if (text == null) {
                    return "";
                }
                else {
                    return text;
                }
            }
            set {
                if (value == null) {
                    value = "";
                }

                if (!Text.Equals(value)) {

                    if (value.Length == 0) {
                        this.text = null;
                    }
                    else {
                        this.text = value;
                    }
                    Realize();
                    UpdateSize();
                }
            }
        }

        /// <include file='doc\StatusBarPanel.uex' path='docs/doc[@for="StatusBarPanel.ToolTipText"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets
        ///       or sets the panel's tool tip text.
        ///    </para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        Localizable(true),
        DefaultValue(""),
        SRDescription(nameof(SR.StatusBarPanelToolTipTextDescr))
        ]
        public string ToolTipText {
            get {
                if (this.toolTipText == null) {
                    return "";
                }
                else {
                    return this.toolTipText;
                }
            }
            set {
                if (value == null) {
                    value = "";
                }

                if (!ToolTipText.Equals(value)) {

                    if (value.Length == 0) {
                        this.toolTipText = null;
                    }
                    else {
                        this.toolTipText = value;
                    }

                    if (Created) {
                        parent.UpdateTooltip(this);
                    }
                }
            }
        }

        /// <include file='doc\StatusBarPanel.uex' path='docs/doc[@for="StatusBarPanel.Width"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets the width of the <see cref='System.Windows.Forms.StatusBarPanel'/> within the <see cref='System.Windows.Forms.StatusBar'/>
        ///       control.
        ///
        ///    </para>
        /// </devdoc>
        [
        Localizable(true),
        SRCategory(nameof(SR.CatAppearance)),
        DefaultValue(DEFAULTWIDTH),
        SRDescription(nameof(SR.StatusBarPanelWidthDescr))
        ]
        public int Width {
            get {
                return this.width;
            }
            set {
                if (!initializing && value < this.minWidth)
                    throw new ArgumentOutOfRangeException(nameof(Width), SR.WidthGreaterThanMinWidth);

                this.width = value;
                UpdateSize();
            }
        }

        /// <include file='doc\StatusBarPanel.uex' path='docs/doc[@for="StatusBarPanel.BeginInit"]/*' />
        /// <devdoc>
        ///      Handles tasks required when the control is being initialized.
        /// </devdoc>
        public void BeginInit() {
            initializing = true;
        }

        /// <include file='doc\StatusBarPanel.uex' path='docs/doc[@for="StatusBarPanel.Dispose"]/*' />
        /// <internalonly/>
        /// <devdoc>
        /// </devdoc>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                if (parent != null) {
                    int index = GetIndex();
                    if (index != -1) {
                        parent.Panels.RemoveAt(index);
                    }
                }
            }
            base.Dispose(disposing);
        }

        /// <include file='doc\StatusBarPanel.uex' path='docs/doc[@for="StatusBarPanel.EndInit"]/*' />
        /// <devdoc>
        ///      Called when initialization of the control is complete.
        /// </devdoc>
        public void EndInit() {
            initializing = false;

            if (Width < MinWidth) {
                Width = MinWidth;
            }
        }

        /// <include file='doc\StatusBarPanel.uex' path='docs/doc[@for="StatusBarPanel.GetContentsWidth"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///     Gets the width of the contents of the panel
        /// </devdoc>
        internal int GetContentsWidth(bool newPanel) {
            string text;
            if (newPanel) {
                if (this.text == null)
                    text = "";
                else
                    text = this.text;
            }
            else
                text = Text;

            Graphics g = this.parent.CreateGraphicsInternal();
            Size sz = Size.Ceiling(g.MeasureString(text, parent.Font));
            if (this.icon != null) {
                sz.Width += this.icon.Size.Width + 5;
            }
            g.Dispose();

            int width = sz.Width + SystemInformation.BorderSize.Width*2 + PANELTEXTINSET*2 + PANELGAP;
            return Math.Max(width, minWidth);
        }

        /// <include file='doc\StatusBarPanel.uex' path='docs/doc[@for="StatusBarPanel.GetIndex"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///     Returns the index of the panel by making the parent control search
        ///     for it within its list.
        /// </devdoc>
        private int GetIndex() {
            return index;
        }

        /// <include file='doc\StatusBarPanel.uex' path='docs/doc[@for="StatusBarPanel.Realize"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///     Sets all the properties for this panel.
        /// </devdoc>
        internal void Realize() {
            if (Created) {
                string text;
                string  sendText;
                int     border = 0;

                if (this.text == null) {
                    text = "";
                }
                else {
                    text = this.text;
                }

                HorizontalAlignment align = alignment;
                // Translate the alignment for Rtl apps
                //
                if (parent.RightToLeft == RightToLeft.Yes) {
                    switch (align) {
                        case HorizontalAlignment.Left:
                            align = HorizontalAlignment.Right;
                            break;
                        case HorizontalAlignment.Right:
                            align = HorizontalAlignment.Left;
                            break;
                    }
                }

                switch (align) {
                    case HorizontalAlignment.Center:
                        sendText = "\t" + text;
                        break;
                    case HorizontalAlignment.Right:
                        sendText = "\t\t" + text;
                        break;
                    default:
                        sendText = text;
                        break;
                }
                switch (borderStyle) {
                    case StatusBarPanelBorderStyle.None:
                        border |= NativeMethods.SBT_NOBORDERS;
                        break;
                    case StatusBarPanelBorderStyle.Sunken:
                        break;
                    case StatusBarPanelBorderStyle.Raised:
                        border |= NativeMethods.SBT_POPOUT;
                        break;
                }
                switch (style) {
                    case StatusBarPanelStyle.Text:
                        break;
                    case StatusBarPanelStyle.OwnerDraw:
                        border |= NativeMethods.SBT_OWNERDRAW;
                        break;
                }


                int wparam = GetIndex() | border;
                if (parent.RightToLeft == RightToLeft.Yes) {
                    wparam |= NativeMethods.SBT_RTLREADING;
                }

                int result = (int) UnsafeNativeMethods.SendMessage(new HandleRef(parent, parent.Handle), NativeMethods.SB_SETTEXT, (IntPtr)wparam, sendText);

                if (result == 0)
                    throw new InvalidOperationException(SR.UnableToSetPanelText);

                if (this.icon != null && style != StatusBarPanelStyle.OwnerDraw) {
                    this.parent.SendMessage(NativeMethods.SB_SETICON, (IntPtr)GetIndex(), this.icon.Handle);
                }
                else {
                    this.parent.SendMessage(NativeMethods.SB_SETICON, (IntPtr)GetIndex(), IntPtr.Zero);
                }

                if (style == StatusBarPanelStyle.OwnerDraw) {
                    NativeMethods.RECT rect = new NativeMethods.RECT();
                    result = (int) UnsafeNativeMethods.SendMessage(new HandleRef(parent, parent.Handle), NativeMethods.SB_GETRECT, (IntPtr)GetIndex(), ref rect);

                    if (result != 0) {
                        this.parent.Invalidate(Rectangle.FromLTRB(rect.left, rect.top, rect.right, rect.bottom));
                    }
                }
            }
        }

        private void UpdateSize() {
            if (this.autoSize == StatusBarPanelAutoSize.Contents) {
                ApplyContentSizing();
            }
            else {
                if (Created) {
                    parent.DirtyLayout();
                    parent.PerformLayout();
                }
            }
        }

        private void ApplyContentSizing() {
            if (this.autoSize == StatusBarPanelAutoSize.Contents &&
                parent != null) {
                int newWidth = GetContentsWidth(false);
                if (newWidth != this.Width) {
                    this.Width = newWidth;
                    if (Created) {
                        parent.DirtyLayout();
                        parent.PerformLayout();
                    }
                }
            }
        }

        /// <include file='doc\StatusBarPanel.uex' path='docs/doc[@for="StatusBarPanel.ToString"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Retrieves a string that contains information about the
        ///       panel.
        ///    </para>
        /// </devdoc>
        public override string ToString() {
            return "StatusBarPanel: {" + Text + "}";
        }
    }
}
