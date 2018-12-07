// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {
    using System.Runtime.Serialization.Formatters;
    using System.Threading;
    using System.Runtime.Remoting;
    using System.Runtime.InteropServices;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Diagnostics;
    using System;
    using System.Windows.Forms;
    using System.Windows.Forms.Design;
    using Hashtable = System.Collections.Hashtable;
    using System.Drawing;
    using Microsoft.Win32;
    using System.Security;
    using System.Security.Permissions;
    using System.Text;
    using System.Drawing.Design;
    using System.Globalization;
    using Collections.Generic;

    /// <include file='doc\ToolTip.uex' path='docs/doc[@for="ToolTip"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Provides a small pop-up window containing a line of text
    ///       that describes the purpose of a tool or control (usually represented as a
    ///       graphical
    ///       object) in a program.
    ///    </para>
    /// </devdoc>
    [
    ProvideProperty(nameof(ToolTip), typeof(Control)),
    DefaultEvent(nameof(Popup)),
    ToolboxItemFilter("System.Windows.Forms"),
    SRDescription(nameof(SR.DescriptionToolTip))
    ]
    public class ToolTip : Component, IExtenderProvider {

        const int DEFAULT_DELAY = 500;
        const int RESHOW_RATIO = 5;
        const int AUTOPOP_RATIO = 10;

        const int XBALLOONOFFSET = 10;
        const int YBALLOONOFFSET = 8;

        private const int TOP_LOCATION_INDEX = 0;
        private const int RIGHT_LOCATION_INDEX = 1;
        private const int BOTTOM_LOCATION_INDEX = 2;
        private const int LEFT_LOCATION_INDEX = 3;
        private const int LOCATION_TOTAL = 4;

        Hashtable           tools = new Hashtable();
        int[]               delayTimes = new int[4];
        bool                auto = true;
        bool                showAlways = false;
        ToolTipNativeWindow window = null;
        Control             topLevelControl = null;
        bool                active = true;
        bool                ownerDraw = false;
        object              userData;
        Color               backColor = SystemColors.Info;
        Color               foreColor = SystemColors.InfoText;
        bool                isBalloon;
        bool                isDisposing;
        string              toolTipTitle = string.Empty;
        ToolTipIcon         toolTipIcon = (ToolTipIcon)0;
        ToolTipTimer        timer;
        Hashtable           owners = new Hashtable();
        bool                stripAmpersands = false;
        bool                useAnimation = true;
        bool                useFading = true;
        int                 originalPopupDelay = 0;
        
        // setting TTM_TRACKPOSITION will cause redundant POP and Draw Messages..
        // Hence we gaurd against this by having this private Flag..
        bool                trackPosition = false;

        PopupEventHandler onPopup;
        DrawToolTipEventHandler onDraw;

        // Adding a tool twice breaks the ToolTip, so we need to track which
        // tools are created to prevent this...
        //
        Hashtable           created = new Hashtable();

        private bool cancelled = false;

        /// <include file='doc\ToolTip.uex' path='docs/doc[@for="ToolTip.ToolTip"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of the <see cref='System.Windows.Forms.ToolTip'/> class, given the container.
        ///    </para>
        /// </devdoc>
        public ToolTip(IContainer cont) : this() {
            if (cont == null) {
                throw new ArgumentNullException(nameof(cont));
            }

            cont.Add(this);
        }

        /// <include file='doc\ToolTip.uex' path='docs/doc[@for="ToolTip.ToolTip1"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of the <see cref='System.Windows.Forms.ToolTip'/> class in its default state.
        ///    </para>
        /// </devdoc>
        public ToolTip() {
            window = new ToolTipNativeWindow(this);
            auto = true;
            delayTimes[NativeMethods.TTDT_AUTOMATIC] = DEFAULT_DELAY;
            AdjustBaseFromAuto();
        }

        /// <include file='doc\ToolTip.uex' path='docs/doc[@for="ToolTip.Active"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets a value indicating whether the <see cref='System.Windows.Forms.ToolTip'/> control is currently active.
        ///    </para>
        /// </devdoc>
        [
        SRDescription(nameof(SR.ToolTipActiveDescr)),
        DefaultValue(true)
        ]
        public bool Active {
            get {
                return active;
            }

            set {
                if (active != value) {
                    active = value;

                    //lets not actually activate the tooltip if we're in the designer (just set the value)
                    if (!DesignMode && GetHandleCreated()) {
                        UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.TTM_ACTIVATE, (value==true)? 1: 0, 0);
                    }
                }
            }
        }

        internal void HideToolTip(IKeyboardToolTip currentTool) {
            this.Hide(currentTool.GetOwnerWindow());
        }

        /// <include file='doc\ToolTip.uex' path='docs/doc[@for="ToolTip.AutomaticDelay"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets
        ///       the time (in milliseconds) that passes before the <see cref='System.Windows.Forms.ToolTip'/> appears.
        ///    </para>
        /// </devdoc>
        [
        RefreshProperties(RefreshProperties.All),
        SRDescription(nameof(SR.ToolTipAutomaticDelayDescr)),
        DefaultValue(DEFAULT_DELAY)
        ]
        public int AutomaticDelay {
            get {
                return delayTimes[NativeMethods.TTDT_AUTOMATIC];
            }

            set {
                if (value < 0) {
                    throw new ArgumentOutOfRangeException(nameof(AutomaticDelay), string.Format(SR.InvalidLowBoundArgumentEx, "AutomaticDelay", (value).ToString(CultureInfo.CurrentCulture), (0).ToString(CultureInfo.CurrentCulture)));
                }
                SetDelayTime(NativeMethods.TTDT_AUTOMATIC, value);
            }
        }

        internal string GetCaptionForTool(Control tool) {
            Debug.Assert(tool != null, "tool should not be null");
            return ((TipInfo)this.tools[tool])?.Caption;
        }

        /// <include file='doc\ToolTip.uex' path='docs/doc[@for="ToolTip.AutoPopDelay"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets the initial delay for the <see cref='System.Windows.Forms.ToolTip'/> control.
        ///    </para>
        /// </devdoc>
        [
        RefreshProperties(RefreshProperties.All),
        SRDescription(nameof(SR.ToolTipAutoPopDelayDescr))
        ]
        public int AutoPopDelay {
            get {
                return delayTimes[NativeMethods.TTDT_AUTOPOP];
            }

            set {
                if (value < 0) {
                    throw new ArgumentOutOfRangeException(nameof(AutoPopDelay), string.Format(SR.InvalidLowBoundArgumentEx, "AutoPopDelay", (value).ToString(CultureInfo.CurrentCulture), (0).ToString(CultureInfo.CurrentCulture)));
                }
                SetDelayTime(NativeMethods.TTDT_AUTOPOP, value);
            }
        }

        /// <include file='doc\ToolTip.uex' path='docs/doc[@for="ToolTip.BackColor"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets the BackColor for the <see cref='System.Windows.Forms.ToolTip'/> control.
        ///    </para>
        /// </devdoc>
        [
        SRDescription(nameof(SR.ToolTipBackColorDescr)),
        DefaultValue(typeof(Color),"Info")
        ]
        public Color BackColor {
            get {
                return backColor;
            }

            set {
                backColor = value;
                if (GetHandleCreated()) {
                    UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.TTM_SETTIPBKCOLOR, ColorTranslator.ToWin32(backColor), 0);
                }
            }
        }

        /// <include file='doc\ToolTip.uex' path='docs/doc[@for="ToolTip.CreateParams"]/*' />
        /// <devdoc>
        ///     The createParams to create the window.
        /// </devdoc>
        /// <internalonly/>
        protected virtual CreateParams CreateParams {
            [
                SecurityPermission(SecurityAction.InheritanceDemand, Flags=SecurityPermissionFlag.UnmanagedCode),
                SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)
            ]
            get {
                CreateParams cp = new CreateParams();
                if (TopLevelControl != null && !TopLevelControl.IsDisposed) {
                    cp.Parent = TopLevelControl.Handle;
                }
                cp.ClassName = NativeMethods.TOOLTIPS_CLASS;
                if (showAlways) {
                    cp.Style = NativeMethods.TTS_ALWAYSTIP;
                }
                if (isBalloon) {
                    cp.Style |= NativeMethods.TTS_BALLOON;
                }
                if (!stripAmpersands) {
                    cp.Style |= NativeMethods.TTS_NOPREFIX;
                }
                if (!useAnimation) {
                    cp.Style |= NativeMethods.TTS_NOANIMATE;
                }
                if (!useFading) {
                    cp.Style |= NativeMethods.TTS_NOFADE;
                }
                cp.ExStyle = 0;
                cp.Caption = null;

                return cp;
            }
        }

        /// <include file='doc\ToolTip.uex' path='docs/doc[@for="ToolTip.ForeColor"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets the ForeColor for the <see cref='System.Windows.Forms.ToolTip'/> control.
        ///    </para>
        /// </devdoc>
        [
        SRDescription(nameof(SR.ToolTipForeColorDescr)),
        DefaultValue(typeof(Color),"InfoText")
        ]
        public Color ForeColor {
            get {
                return foreColor;
            }

            set {
                if (value.IsEmpty) {
                    throw new ArgumentException(string.Format(SR.ToolTipEmptyColor, "ForeColor"));
                }

                foreColor = value;
                if (GetHandleCreated()) {
                    UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.TTM_SETTIPTEXTCOLOR, ColorTranslator.ToWin32(foreColor), 0);
                }

            }
        }

        internal IntPtr Handle {
            get {
                if (!GetHandleCreated()) {
                    CreateHandle();
                }
                return window.Handle;
            }
        }

        /// <devdoc>
        ///    

        private bool HasAllWindowsPermission {
            get {
                try {
                    IntSecurity.AllWindows.Demand();
                    return true;
                }
                catch (SecurityException) {
                }
                
                return false;
            }
        }

        /// <include file='doc\ToolTip.uex' path='docs/doc[@for="ToolTip.IsBalloon"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets the IsBalloon for the <see cref='System.Windows.Forms.ToolTip'/> control.
        ///    </para>
        /// </devdoc>
        [
        SRDescription(nameof(SR.ToolTipIsBalloonDescr)),
        DefaultValue(false)
        ]
        public bool IsBalloon {
            get {
                return isBalloon;
            }

            set {
                if (isBalloon != value) {
                    isBalloon = value;
                    if (GetHandleCreated()) {
                        RecreateHandle();
                    }
                }
                
            }
        }

        // ToolTips should be shown only on active Windows.
        private bool IsWindowActive(IWin32Window window)
        {
            Control windowControl = window as Control;
            // We want to enter in the IF block only if ShowParams does not return SW_SHOWNOACTIVATE.
            // for ToolStripDropDown ShowParams returns SW_SHOWNOACTIVATE, in which case we DONT want to check IsWindowActive and hence return true.
            if (windowControl != null &&
                (windowControl.ShowParams & 0xF) != NativeMethods.SW_SHOWNOACTIVATE)
            {
                IntPtr hWnd = UnsafeNativeMethods.GetActiveWindow();
                IntPtr rootHwnd =UnsafeNativeMethods.GetAncestor(new HandleRef(window, window.Handle), NativeMethods.GA_ROOT);
                if (hWnd != rootHwnd)
                {
                    TipInfo tt = (TipInfo)tools[windowControl];
                    if (tt != null && (tt.TipType & TipInfo.Type.SemiAbsolute) != 0)
                    {
                        tools.Remove(windowControl);
                        DestroyRegion(windowControl);
                    }
                    return false;
                }
            }
            return true;
        }

        /// <include file='doc\ToolTip.uex' path='docs/doc[@for="ToolTip.InitialDelay"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets the initial delay for
        ///       the <see cref='System.Windows.Forms.ToolTip'/>
        ///       control.
        ///    </para>
        /// </devdoc>
        [
        RefreshProperties(RefreshProperties.All),
        SRDescription(nameof(SR.ToolTipInitialDelayDescr))
        ]
        public int InitialDelay {
            get {
                return delayTimes[NativeMethods.TTDT_INITIAL];
            }

            set {
                if (value < 0) {
                    throw new ArgumentOutOfRangeException(nameof(InitialDelay), string.Format(SR.InvalidLowBoundArgumentEx, "InitialDelay", (value).ToString(CultureInfo.CurrentCulture), (0).ToString(CultureInfo.CurrentCulture)));
                }
                SetDelayTime(NativeMethods.TTDT_INITIAL, value);
            }
        }
        
        /// <include file='doc\ToolTip.uex' path='docs/doc[@for="ToolTip.OwnerDraw"]/*' />
        /// <devdoc>
        /// Indicates whether the ToolTip will be drawn by the system or the user.
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatBehavior)),
        DefaultValue(false),
        SRDescription(nameof(SR.ToolTipOwnerDrawDescr)) 
        ]
        public bool OwnerDraw 
        {
            get 
            {
                return ownerDraw;
            }
            [UIPermission(SecurityAction.Demand, Window=UIPermissionWindow.AllWindows)]
            set 
            {
                ownerDraw = value;
            }
        }

        /// <include file='doc\ToolTip.uex' path='docs/doc[@for="ToolTip.ReshowDelay"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets the length of time (in milliseconds) that
        ///       it takes subsequent ToolTip instances to appear as the mouse pointer moves from
        ///       one ToolTip region to
        ///       another.
        ///    </para>
        /// </devdoc>
        [
        RefreshProperties(RefreshProperties.All),
        SRDescription(nameof(SR.ToolTipReshowDelayDescr))
        ]
        public int ReshowDelay {
            get {
                return delayTimes[NativeMethods.TTDT_RESHOW];
            }
            set {
                if (value < 0) {
                    throw new ArgumentOutOfRangeException(nameof(ReshowDelay), string.Format(SR.InvalidLowBoundArgumentEx, "ReshowDelay", (value).ToString(CultureInfo.CurrentCulture), (0).ToString(CultureInfo.CurrentCulture)));
                }
                SetDelayTime(NativeMethods.TTDT_RESHOW, value);
            }
        }

        /// <include file='doc\ToolTip.uex' path='docs/doc[@for="ToolTip.ShowAlways"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets a value indicating whether the <see cref='System.Windows.Forms.ToolTip'/>
        ///       appears even when its parent control is not active.
        ///    </para>
        /// </devdoc>
        [
        DefaultValue(false),
        SRDescription(nameof(SR.ToolTipShowAlwaysDescr))
        ]
        public bool ShowAlways {
            get {
                return showAlways;
            }
            set {
                if (showAlways != value) {
                    showAlways = value;
                    if (GetHandleCreated()) {
                        RecreateHandle();
                    }
                }
            }
        }


        /// <include file='doc\ToolTip.uex' path='docs/doc[@for="ToolTip.StripAmpersands"]/*' />
        /// <devdoc>
        ///    <para>
        ///       When set to true, any ampersands in the Text property are not displayed.
        ///    </para>
        /// </devdoc>
        [
        SRDescription(nameof(SR.ToolTipStripAmpersandsDescr)),
        Browsable(true),
        DefaultValue(false)
        ]
        public bool StripAmpersands {
            get {
                return stripAmpersands;
            }
            set {
                if (stripAmpersands != value) {
                    stripAmpersands = value;
                    if (GetHandleCreated()) {
                        RecreateHandle();
                    }
                }
            }
        }

        /// <include file='doc\ToolTip.uex' path='docs/doc[@for="ToolTip.Tag"]/*' />
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

        /// <include file='doc\ToolTip.uex' path='docs/doc[@for="ToolTip.ToolTipIcons"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets an Icon on the ToolTip.
        ///    </para>
        /// </devdoc>
        [
        DefaultValue(ToolTipIcon.None),
        SRDescription(nameof(SR.ToolTipToolTipIconDescr))
        ]
        public ToolTipIcon ToolTipIcon {
            get {
                return toolTipIcon;
            }
            set {
                if (toolTipIcon != value) {
                    //valid values are 0x0 to 0x3
                    if (!ClientUtils.IsEnumValid(value, (int)value, (int)ToolTipIcon.None, (int)ToolTipIcon.Error))
                    {
                        throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(ToolTipIcon));
                    }
                    toolTipIcon = value;
                    if (toolTipIcon > 0 && GetHandleCreated()) {
                        // If the title is null/empty, the icon won't display.
                        string title = !String.IsNullOrEmpty(toolTipTitle) ? toolTipTitle : " ";
                        UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.TTM_SETTITLE, (int)toolTipIcon, title);

                        // Tooltip need to be updated to reflect the changes in the icon because
                        // this operation directly affects the size of the tooltip
                        UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.TTM_UPDATE, 0, 0);

                    }
                }
            }
        }


        /// <devdoc>
        ///    <para>
        ///       Gets or sets the title of the ToolTip.
        ///    </para>
        /// </devdoc>
        [
        DefaultValue(""), 
        SRDescription(nameof(SR.ToolTipTitleDescr))
        ]
        public string ToolTipTitle
        {
            get
            {
                return toolTipTitle;
            }
            set
            {
                if (value == null)
                {
                    value = string.Empty;
                }

                if (toolTipTitle != value)
                {
                    toolTipTitle = value;
                    if (GetHandleCreated())
                    {
                        UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.TTM_SETTITLE, (int)toolTipIcon, toolTipTitle);

                        // Tooltip need to be updated to reflect the changes in the titletext because
                        // this operation directly affects the size of the tooltip
                        UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.TTM_UPDATE, 0, 0);
                    }
                }
            }
        }

        private Control TopLevelControl {
            get {
                Control baseVar = null;
                if (topLevelControl == null) {
                    Control[] regions = new Control[tools.Keys.Count];
                    tools.Keys.CopyTo(regions, 0);
                    if (regions != null && regions.Length > 0) {
                        for (int i=0; i<regions.Length; i++) {
                            Control ctl = regions[i];
                            
                            baseVar = ctl.TopLevelControlInternal;
                            if (baseVar != null) {
                                break;
                            }

                            if (ctl.IsActiveX) {
                                baseVar = ctl;
                                break;
                            }
                            // In designer, baseVar can be null since the Parent is not a TopLevel control
                            if (baseVar == null)
                            {
                                if (ctl != null && ctl.ParentInternal != null) {
                                    while (ctl.ParentInternal != null) {
                                        ctl = ctl.ParentInternal;
                                    }
                                    baseVar = ctl;
                                    if (baseVar != null) {
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    topLevelControl = baseVar;
                    if (baseVar != null) {
                        baseVar.HandleCreated += new EventHandler(this.TopLevelCreated);
                        baseVar.HandleDestroyed += new EventHandler(this.TopLevelDestroyed);
                        if (baseVar.IsHandleCreated) {
                            TopLevelCreated(baseVar, EventArgs.Empty);
                        }
                        baseVar.ParentChanged += new EventHandler(this.OnTopLevelPropertyChanged);
                    }
                }
                else {
                    baseVar = topLevelControl;
                }
                return baseVar;
            }
        }

        /// <include file='doc\ToolTip.uex' path='docs/doc[@for="ToolTip.UseAnimation"]/*' />
        /// <devdoc>
        ///    <para>
        ///       When set to true, animations are used when tooltip is shown or hidden.
        ///    </para>
        /// </devdoc>
        [
        SRDescription(nameof(SR.ToolTipUseAnimationDescr)),
        Browsable(true),
        DefaultValue(true)
        ]
        public bool UseAnimation {
            get {
                return useAnimation;
            }
            set {
                if (useAnimation != value) {
                    useAnimation = value;
                    if (GetHandleCreated()) {
                        RecreateHandle();
                    }
                }
            }
        }


        /// <include file='doc\ToolTip.uex' path='docs/doc[@for="ToolTip.UseFading"]/*' />
        /// <devdoc>
        ///    <para>
        ///       When set to true, a fade effect is used when tooltips are shown or hidden.
        ///    </para>
        /// </devdoc>
        [
        SRDescription(nameof(SR.ToolTipUseFadingDescr)),
        Browsable(true),
        DefaultValue(true)
        ]
        public bool UseFading {
            get {
                return useFading;
            }
            set {
                if (useFading != value) {
                    useFading = value;
                    if (GetHandleCreated()) {
                        RecreateHandle();
                    }
                }
            }
        }

        /// <include file='doc\ToolTip.uex' path='docs/doc[@for="ToolTip.Draw"]/*' />
        /// <devdoc>
        ///    <para>Fires in OwnerDraw mode when the tooltip needs to be drawn.</para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatBehavior)),SRDescription(nameof(SR.ToolTipDrawEventDescr))]
        public event DrawToolTipEventHandler Draw 
        {
            add 
            {
                onDraw += value;
            }
            remove 
            {
                onDraw -= value;
            }
        }
        
        /// <include file='doc\ToolTip.uex' path='docs/doc[@for="ToolTip.Popup"]/*' />
        /// <devdoc>
        ///    <para>Fires when the tooltip is just about to be shown.</para>
        /// </devdoc>
        [SRCategory(nameof(SR.CatBehavior)),SRDescription(nameof(SR.ToolTipPopupEventDescr))]
        public event PopupEventHandler Popup 
        {
            add 
            {
                onPopup += value;
            }
            remove 
            {
                onPopup -= value;
            }
        }


        /// <include file='doc\ToolTip.uex' path='docs/doc[@for="ToolTip.AdjustBaseFromAuto"]/*' />
        /// <devdoc>
        ///     Adjusts the other delay values based on the Automatic value.
        /// </devdoc>
        /// <internalonly/>
        private void AdjustBaseFromAuto() {
            delayTimes[NativeMethods.TTDT_RESHOW] = delayTimes[NativeMethods.TTDT_AUTOMATIC] / RESHOW_RATIO;
            delayTimes[NativeMethods.TTDT_AUTOPOP] = delayTimes[NativeMethods.TTDT_AUTOMATIC] * AUTOPOP_RATIO;
            delayTimes[NativeMethods.TTDT_INITIAL] = delayTimes[NativeMethods.TTDT_AUTOMATIC];
        }
        
        private void HandleCreated(object sender, EventArgs eventargs) {
            // Reset the toplevel control when the owner's handle is recreated.
            ClearTopLevelControlEvents();
            topLevelControl = null;

            Control control = (Control)sender;
            this.CreateRegion(control);
            this.CheckNativeToolTip(control);
            this.CheckCompositeControls(control);

            if (!AccessibilityImprovements.UseLegacyToolTipDisplay) {
                KeyboardToolTipStateMachine.Instance.Hook(control, this);
            }
        }

        private void CheckNativeToolTip(Control associatedControl) {

            //Wait for the Handle Creation..
            if (!GetHandleCreated()) {
                return;
            }
           
            TreeView treeView = associatedControl as TreeView;
            if (treeView != null) {
                if (treeView.ShowNodeToolTips) {
                    treeView.SetToolTip(this,GetToolTip(associatedControl));
                }
            }
            
            if (associatedControl is ToolBar) {
               ((ToolBar)associatedControl).SetToolTip(this);
            }

            TabControl tabControl = associatedControl as TabControl;
            if (tabControl!= null) {
                if (tabControl.ShowToolTips)
                {
                    tabControl.SetToolTip(this, GetToolTip(associatedControl));
                }
            }
            
            if (associatedControl is ListView) {
               ((ListView)associatedControl).SetToolTip(this, GetToolTip(associatedControl));
            }
            
            if (associatedControl is StatusBar) {
                ((StatusBar)associatedControl).SetToolTip(this);
            }

            // Label now has its own Tooltip for AutoEllipsis...
            // So this control too falls in special casing...
            // We need to disable the LABEL AutoEllipsis tooltip and show 
            // this tooltip always...
            if (associatedControl is Label) {
                ((Label)associatedControl).SetToolTip(this);
            }


        }

        private void CheckCompositeControls(Control associatedControl) {
            if (associatedControl is UpDownBase) {
                ((UpDownBase)associatedControl).SetToolTip(this, GetToolTip(associatedControl));
            }
        }

        private void HandleDestroyed(object sender, EventArgs eventargs) {
            Control control = (Control)sender;
            this.DestroyRegion(control);

            if (!AccessibilityImprovements.UseLegacyToolTipDisplay) {
                KeyboardToolTipStateMachine.Instance.Unhook(control, this);
            }
        }

        /// <include file='doc\ToolTip.uex' path='docs/doc[@for="ToolTip.OnDraw"]/*' />
        /// <devdoc>
        /// Fires the Draw event. 
        /// </devdoc>
        private void OnDraw(DrawToolTipEventArgs e) 
        {
            if(onDraw != null) 
            {
                onDraw(this,e); 
            }
        }


        /// <include file='doc\ToolTip.uex' path='docs/doc[@for="ToolTip.OnPopup"]/*' />
        /// <devdoc>
        /// Fires the Popup event. 
        /// </devdoc>
        private void OnPopup(PopupEventArgs e) 
        {
            if(onPopup != null) 
            {
                onPopup(this,e); 
            }
        }

        private void TopLevelCreated(object sender, EventArgs eventargs) {
            CreateHandle();
            CreateAllRegions();
        }

        private void TopLevelDestroyed(object sender, EventArgs eventargs) {
            DestoyAllRegions();
            DestroyHandle();
        }

        /// <include file='doc\ToolTip.uex' path='docs/doc[@for="ToolTip.CanExtend"]/*' />
        /// <devdoc>
        ///    Returns true if the tooltip can offer an extender property to the
        ///    specified target component.
        /// </devdoc>
        /// <internalonly/>
        public bool CanExtend(object target) {
            if (target is Control &&
                !(target is ToolTip)) {

                return true;
            }
            return false;
        }

        private void ClearTopLevelControlEvents() {

            if (this.topLevelControl != null) {
                this.topLevelControl.ParentChanged -= new EventHandler(this.OnTopLevelPropertyChanged);
                this.topLevelControl.HandleCreated -= new EventHandler(this.TopLevelCreated);
                this.topLevelControl.HandleDestroyed -= new EventHandler(this.TopLevelDestroyed);
            }
        }

        /// <include file='doc\ToolTip.uex' path='docs/doc[@for="ToolTip.CreateHandle"]/*' />
        /// <devdoc>
        ///     Creates the handle for the control.
        /// </devdoc>
        /// <internalonly/>
        private void CreateHandle() {
            if (GetHandleCreated()) {
                return;
            }
            IntPtr userCookie = UnsafeNativeMethods.ThemingScope.Activate();
                        
            try {

                NativeMethods.INITCOMMONCONTROLSEX icc = new NativeMethods.INITCOMMONCONTROLSEX();
                icc.dwICC = NativeMethods.ICC_TAB_CLASSES;
                SafeNativeMethods.InitCommonControlsEx(icc);

                CreateParams cp = CreateParams; // Avoid reentrant call to CreateHandle
                if (GetHandleCreated()) {
                    return;
                }
                window.CreateHandle(cp);
            }
            finally {
                UnsafeNativeMethods.ThemingScope.Deactivate(userCookie);
            }

            // If in ownerDraw mode, we don't want the default border.
            if (ownerDraw) {
                int style = unchecked((int)((long)UnsafeNativeMethods.GetWindowLong(new HandleRef(this, Handle), NativeMethods.GWL_STYLE)));
                style &= ~NativeMethods.WS_BORDER;
                UnsafeNativeMethods.SetWindowLong(new HandleRef(this, Handle), NativeMethods.GWL_STYLE, new HandleRef(null, (IntPtr)style));
            }
           
            // Setting the max width has the added benefit of enabling multiline
            // tool tips!
            //
            UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.TTM_SETMAXTIPWIDTH, 0, SystemInformation.MaxWindowTrackSize.Width);

            Debug.Assert(NativeMethods.TTDT_AUTOMATIC == 0, "TTDT_AUTOMATIC != 0");

            if (auto) {
                SetDelayTime(NativeMethods.TTDT_AUTOMATIC, delayTimes[NativeMethods.TTDT_AUTOMATIC]);
                delayTimes[NativeMethods.TTDT_AUTOPOP] = GetDelayTime(NativeMethods.TTDT_AUTOPOP);
                delayTimes[NativeMethods.TTDT_INITIAL] = GetDelayTime(NativeMethods.TTDT_INITIAL);
                delayTimes[NativeMethods.TTDT_RESHOW] = GetDelayTime(NativeMethods.TTDT_RESHOW);
            }
            else {
                for (int i=1; i < delayTimes.Length; i++) {
                    if (delayTimes[i] >= 1) {
                        SetDelayTime(i, delayTimes[i]);
                    }
                }
            }

            // Set active status
            //
            UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.TTM_ACTIVATE, (active == true) ? 1 : 0, 0);

            if (BackColor != SystemColors.Info) {
                UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.TTM_SETTIPBKCOLOR, ColorTranslator.ToWin32(BackColor), 0);
            }
            if (ForeColor != SystemColors.InfoText) {
                UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.TTM_SETTIPTEXTCOLOR, ColorTranslator.ToWin32(ForeColor), 0);
            }
            if (toolTipIcon > 0 || !String.IsNullOrEmpty(toolTipTitle)) {
                // If the title is null/empty, the icon won't display.
                string title = !String.IsNullOrEmpty(toolTipTitle) ? toolTipTitle : " ";
                UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.TTM_SETTITLE, (int)toolTipIcon, title);
            }
        }

        private void CreateAllRegions() {
            Control[] ctls = new Control[tools.Keys.Count];
            tools.Keys.CopyTo(ctls, 0);
            for (int i=0; i<ctls.Length; i++) {
                // the grid view manages its own tool tip
                if (ctls[i] is DataGridView) {
                    return;
                }

                CreateRegion(ctls[i]);
            }
        }

        private void DestoyAllRegions() {
            Control[] ctls = new Control[tools.Keys.Count];
            tools.Keys.CopyTo(ctls, 0);
            for (int i=0; i<ctls.Length; i++) {
                // the grid view manages its own tool tip
                if (ctls[i] is DataGridView) {
                    return;
                }

                DestroyRegion(ctls[i]);
            }
        }
        
        private void SetToolInfo(Control ctl, string caption) {
            bool allocatedString;
            NativeMethods.TOOLINFO_TOOLTIP tool = GetTOOLINFO(ctl, caption, out allocatedString);
            try {
                int ret = (int)UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.TTM_ADDTOOL, 0, tool);
                if (ctl is TreeView || ctl is ListView) {
                    TreeView tv = ctl as TreeView;
                    if (tv != null && tv.ShowNodeToolTips) {
                        return;
                    }
                    else {
                        ListView lv = ctl as ListView;
                        if (lv != null && lv.ShowItemToolTips) {
                            return;
                        }
                    }
                }
                if (ret == 0) {
                    throw new InvalidOperationException(SR.ToolTipAddFailed);
                }
            } finally {
                if(allocatedString && IntPtr.Zero != tool.lpszText) {
                    Marshal.FreeHGlobal(tool.lpszText);
                }
            }	
        }

        private void CreateRegion(Control ctl) {
            string caption = GetToolTip(ctl);
            bool captionValid = caption != null
                                && caption.Length > 0;
            bool handlesCreated = ctl.IsHandleCreated
                                  && TopLevelControl != null
                                  && TopLevelControl.IsHandleCreated;
            if (!created.ContainsKey(ctl) && captionValid
                && handlesCreated && !DesignMode) {

                //Call the Sendmessage thru a function..
                SetToolInfo(ctl, caption);
                created[ctl] = ctl;
            }
            if (ctl.IsHandleCreated && topLevelControl == null) {
                // Remove first to purge any duplicates...
                //
                ctl.MouseMove -= new MouseEventHandler(this.MouseMove);
                ctl.MouseMove += new MouseEventHandler(this.MouseMove);
            }
        }

        private void MouseMove(object sender, MouseEventArgs me) {
            Control ctl = (Control)sender;

            if (!created.ContainsKey(ctl)
                && ctl.IsHandleCreated
                && TopLevelControl != null) {

                CreateRegion(ctl);
            }

            if (created.ContainsKey(ctl)) {
                ctl.MouseMove -= new MouseEventHandler(this.MouseMove);
            }
        }



        /// <include file='doc\ToolTip.uex' path='docs/doc[@for="ToolTip.DestroyHandle"]/*' />
        /// <devdoc>
        ///     Destroys the handle for this control.
        /// </devdoc>
        /// <internalonly/>
        /// Required by Label to destroy the handle for the toolTip added for AutoEllipses.
        internal void DestroyHandle() {

            if (GetHandleCreated()) {
                window.DestroyHandle();
            }
        }

        private void DestroyRegion(Control ctl) {

            // when the toplevelControl is a form and is Modal, the Handle of the tooltip is releasedbefore we come here.
            // In such a case the tool wont get deleted from the tooltip.
            // So we dont check "Handle" in the handlesCreate but check it only foe Non-Nodal dialogs later
            
            bool handlesCreated = ctl.IsHandleCreated
                                && topLevelControl != null
                                && topLevelControl.IsHandleCreated
                                && !this.isDisposing;

            Form topForm = topLevelControl as Form;
            if (topForm == null || (topForm != null && !topForm.Modal))
            {
                handlesCreated = handlesCreated && GetHandleCreated();
            }
            
            if (created.ContainsKey(ctl)
                && handlesCreated && !DesignMode) {

                UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.TTM_DELTOOL, 0, GetMinTOOLINFO(ctl));
                created.Remove(ctl);
            }
        }

        /// <include file='doc\ToolTip.uex' path='docs/doc[@for="ToolTip.Dispose"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    <para>
        ///       Disposes of the <see cref='System.Windows.Forms.ToolTip'/>
        ///       component.
        ///    </para>
        /// </devdoc>
        protected override void Dispose(bool disposing) {
            
            if (disposing) {
                this.isDisposing = true;
                try {
                    ClearTopLevelControlEvents();
                    StopTimer();

                    // always destroy the handle...
                    //
                    DestroyHandle();
                    RemoveAll();

                    window = null;

                    //Unhook the DeactiveEvent...
                    // Lets find the Form for associated Control ...
                    // and hook up to the Deactivated event to Hide the Shown tooltip
                    Form baseFrom = TopLevelControl as Form;
                    if (baseFrom != null) {
                        baseFrom.Deactivate -= new EventHandler(this.BaseFormDeactivate);
                    }
                }
                finally {
                    this.isDisposing = false;
                }
            }  
            base.Dispose(disposing);
        }

        /// <include file='doc\ToolTip.uex' path='docs/doc[@for="ToolTip.GetDelayTime"]/*' />
        /// <devdoc>
        ///     Returns the delayTime based on the NativeMethods.TTDT_* values.
        /// </devdoc>
        /// <internalonly/>
        internal int GetDelayTime(int type) {
            if (GetHandleCreated()) {
                return (int)UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.TTM_GETDELAYTIME, type, 0);
            }
            else {
                return delayTimes[type];
            }
        }

        // Can't be a property -- there is another method called GetHandleCreated
        internal bool GetHandleCreated() {
            return (window != null ? window.Handle != IntPtr.Zero: false);
        }

        /// <include file='doc\ToolTip.uex' path='docs/doc[@for="ToolTip.GetMinTOOLINFO"]/*' />
        /// <devdoc>
        ///     Returns a new instance of the TOOLINFO_T structure with the minimum
        ///     required data to uniquely identify a region. This is used primarily
        ///     for delete operations. NOTE: This cannot force the creation of a handle.
        /// </devdoc>
        /// <internalonly/>
        private NativeMethods.TOOLINFO_TOOLTIP GetMinTOOLINFO(Control ctl) {
            return this.GetMinToolInfoForHandle(ctl.Handle);
        }

        private NativeMethods.TOOLINFO_TOOLTIP GetMinToolInfoForTool(IWin32Window tool) {
            return this.GetMinToolInfoForHandle(tool.Handle);
        }

        private NativeMethods.TOOLINFO_TOOLTIP GetMinToolInfoForHandle(IntPtr handle) {
            NativeMethods.TOOLINFO_TOOLTIP ti = new NativeMethods.TOOLINFO_TOOLTIP();
            ti.cbSize = Marshal.SizeOf(typeof(NativeMethods.TOOLINFO_TOOLTIP));
            ti.hwnd = handle;
            ti.uFlags |= NativeMethods.TTF_IDISHWND;
            ti.uId = handle;
            return ti;
        }

        /// <include file='doc\ToolTip.uex' path='docs/doc[@for="ToolTip.GetTOOLINFO"]/*' />
        /// <devdoc>
        ///     Returns a detailed TOOLINFO_TOOLTIP structure that represents the specified
        ///     region. NOTE: This may force the creation of a handle.
        ///     If the out parameter allocatedString has been set to true, It is the responsibility of the caller
        ///		to free the string buffer referenced by lpszText (using Marshal.FreeHGlobal).
        /// </devdoc>
        /// <internalonly/>
        private NativeMethods.TOOLINFO_TOOLTIP GetTOOLINFO(Control ctl, string caption, out bool allocatedString) {
            allocatedString = false;
            NativeMethods.TOOLINFO_TOOLTIP ti = GetMinTOOLINFO(ctl);
            ti.cbSize = Marshal.SizeOf(typeof(NativeMethods.TOOLINFO_TOOLTIP));
            ti.uFlags |= NativeMethods.TTF_TRANSPARENT | NativeMethods.TTF_SUBCLASS;

            // RightToLeft reading order
            //
            Control richParent = TopLevelControl;
            if (richParent != null && richParent.RightToLeft == RightToLeft.Yes && !ctl.IsMirrored) {
                //Indicates that the ToolTip text will be displayed in the opposite direction 
                //to the text in the parent window.                 
                ti.uFlags |= NativeMethods.TTF_RTLREADING;
            }
            
            if (ctl is TreeView || ctl is ListView) {
                TreeView tv = ctl as TreeView;
                if (tv != null && tv.ShowNodeToolTips) {
                    ti.lpszText = NativeMethods.InvalidIntPtr;
                }
                else {
                    ListView lv = ctl as ListView;
                    if (lv != null && lv.ShowItemToolTips) {
                        ti.lpszText = NativeMethods.InvalidIntPtr;
                    }
                    else {
                        ti.lpszText = Marshal.StringToHGlobalAuto(caption);
                        allocatedString = true;
                    }
                }
            }
            else {
                ti.lpszText = Marshal.StringToHGlobalAuto(caption);
                allocatedString = true;
            }

            
            return ti;
        }

        private NativeMethods.TOOLINFO_TOOLTIP GetWinTOOLINFO(IntPtr hWnd) {
            NativeMethods.TOOLINFO_TOOLTIP ti = new NativeMethods.TOOLINFO_TOOLTIP();
            ti.cbSize = Marshal.SizeOf(typeof(NativeMethods.TOOLINFO_TOOLTIP));
            ti.hwnd = hWnd;
            ti.uFlags |= NativeMethods.TTF_IDISHWND | NativeMethods.TTF_TRANSPARENT | NativeMethods.TTF_SUBCLASS;
                    
            // RightToLeft reading order
            //
            Control richParent = TopLevelControl;
            if (richParent != null && richParent.RightToLeft == RightToLeft.Yes) {
                bool isWindowMirrored = ((unchecked((int)(long)UnsafeNativeMethods.GetWindowLong(new HandleRef(this, hWnd), NativeMethods.GWL_STYLE)) & NativeMethods.WS_EX_LAYOUTRTL) == NativeMethods.WS_EX_LAYOUTRTL);            
                //Indicates that the ToolTip text will be displayed in the opposite direction 
                //to the text in the parent window.                 
                if (!isWindowMirrored) {
                    ti.uFlags |= NativeMethods.TTF_RTLREADING;
                }
            }
            
            ti.uId = ti.hwnd;
            return ti;
        }

        /// <include file='doc\ToolTip.uex' path='docs/doc[@for="ToolTip.GetToolTip"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Retrieves the <see cref='System.Windows.Forms.ToolTip'/> text associated with the specified control.
        ///    </para>
        /// </devdoc>
        [
        DefaultValue(""),
        Localizable(true),
        SRDescription(nameof(SR.ToolTipToolTipDescr)),
        Editor("System.ComponentModel.Design.MultilineStringEditor, " + AssemblyRef.SystemDesign, typeof(System.Drawing.Design.UITypeEditor))
        ]
        public string GetToolTip(Control control) {
            if (control == null) {
                return String.Empty; 
            }
            TipInfo tt = (TipInfo)tools[control];
            if (tt == null || tt.Caption == null) {
                return "";
            }
            else {
                return tt.Caption;
            }
        }

        /// <include file='doc\ToolTip.uex' path='docs/doc[@for="ToolTip.GetWindowFromPoint"]/*' />
        /// <devdoc>
        ///     Returns the HWND of the window that is at the specified point. This
        ///     handles special cases where one Control owns multiple HWNDs (i.e. ComboBox).
        /// </devdoc>
        /// <internalonly/>
        private IntPtr GetWindowFromPoint(Point screenCoords, ref bool success) {
            Control baseVar = TopLevelControl;
            //Special case the ActiveX Controls.
            if (baseVar != null && baseVar.IsActiveX)
            {
                //find the matching HWnd matching the ScreenCoord and find if the Control has a Tooltip.
                IntPtr hwndControl = UnsafeNativeMethods.WindowFromPoint(screenCoords.X, screenCoords.Y);
                if (hwndControl != IntPtr.Zero)
                {
                    Control currentControl = Control.FromHandleInternal(hwndControl);
                    if (currentControl != null && tools != null && tools.ContainsKey(currentControl))
                    {
                        return hwndControl;
                    }
                }
                return IntPtr.Zero;
            }
            
            IntPtr baseHwnd = IntPtr.Zero;

            if (baseVar != null) {
                baseHwnd = baseVar.Handle;
            }

            IntPtr hwnd = IntPtr.Zero;
            bool finalMatch = false;
            while (!finalMatch) {
                Point pt = screenCoords;
                if (baseVar != null) {
                    pt = baseVar.PointToClientInternal(screenCoords);
                }
                IntPtr found = UnsafeNativeMethods.ChildWindowFromPointEx(new HandleRef(null, baseHwnd), pt.X, pt.Y, NativeMethods.CWP_SKIPINVISIBLE);

                if (found == baseHwnd) {
                    hwnd = found;
                    finalMatch = true;
                }
                else if (found == IntPtr.Zero) {
                    finalMatch = true;
                }
                else {
                    baseVar = Control.FromHandleInternal(found);
                    if (baseVar == null) {
                        baseVar = Control.FromChildHandleInternal(found);
                        if (baseVar != null) {
                            hwnd = baseVar.Handle;
                        }
                        finalMatch = true;
                    }
                    else {
                        baseHwnd = baseVar.Handle;
                    }
                }
            }

            if (hwnd != IntPtr.Zero) {
                Control ctl = Control.FromHandleInternal(hwnd);
                if (ctl != null) {
                    Control current = ctl;
                    while (current != null && current.Visible) {
                        current = current.ParentInternal;
                    }
                    if (current != null) {
                        hwnd = IntPtr.Zero;
                    }
                    success = true;
                }
            }

            return hwnd;
        }

        private void OnTopLevelPropertyChanged(object s, EventArgs e) {
            ClearTopLevelControlEvents();
            this.topLevelControl = null;

            // We must re-aquire this control.  If the existing top level control's handle
            // was never created, but the new parent has a handle, if we don't re-get
            // the top level control here we won't ever create the tooltip handle.
            //
            this.topLevelControl = TopLevelControl;
        }

        /// <include file='doc\ToolTip.uex' path='docs/doc[@for="ToolTip.RecreateHandle"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        private void RecreateHandle() {
            if (!DesignMode) {
                if (GetHandleCreated()) {
                    DestroyHandle();
                }
                created.Clear();
                CreateHandle();
                CreateAllRegions();
            }
        }


        /// <include file='doc\ToolTip.uex' path='docs/doc[@for="ToolTip.RemoveAll"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Removes all of the tooltips currently associated
        ///       with the <see cref='System.Windows.Forms.ToolTip'/> control.
        ///    </para>
        /// </devdoc>
        public void RemoveAll() {
            Control[] regions = new Control[tools.Keys.Count];
            tools.Keys.CopyTo(regions, 0);
            for (int i=0; i<regions.Length; i++) {
                if (regions[i].IsHandleCreated) {
                    DestroyRegion(regions[i]);
                }
                regions[i].HandleCreated -= new EventHandler(this.HandleCreated);
                regions[i].HandleDestroyed -= new EventHandler(this.HandleDestroyed);

                if (!AccessibilityImprovements.UseLegacyToolTipDisplay) {
                    KeyboardToolTipStateMachine.Instance.Unhook(regions[i], this);
                }
            }

            created.Clear();
            tools.Clear();

            ClearTopLevelControlEvents();
            this.topLevelControl = null;

            if (!AccessibilityImprovements.UseLegacyToolTipDisplay) {
                KeyboardToolTipStateMachine.Instance.ResetStateMachine(this);
            }
        }

        /// <include file='doc\ToolTip.uex' path='docs/doc[@for="ToolTip.SetDelayTime"]/*' />
        /// <devdoc>
        ///     Sets the delayTime based on the NativeMethods.TTDT_* values.
        /// </devdoc>
        /// <internalonly/>
        private void SetDelayTime(int type, int time) {
            if (type == NativeMethods.TTDT_AUTOMATIC) {
                auto = true;
            }
            else {
                auto = false;
            }

            delayTimes[type] = time;

            if (GetHandleCreated() && time >= 0) {
                UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.TTM_SETDELAYTIME, type, time);

                // Update everyone else if automatic is set... we need to do this
                // to preserve value in case of handle recreation.
                //
                if (auto) {
                    delayTimes[NativeMethods.TTDT_AUTOPOP] = GetDelayTime(NativeMethods.TTDT_AUTOPOP);
                    delayTimes[NativeMethods.TTDT_INITIAL] = GetDelayTime(NativeMethods.TTDT_INITIAL);
                    delayTimes[NativeMethods.TTDT_RESHOW] = GetDelayTime(NativeMethods.TTDT_RESHOW);
                }
            }
            else if (auto) {
                AdjustBaseFromAuto();
            }
        }

        /// <include file='doc\ToolTip.uex' path='docs/doc[@for="ToolTip.SetToolTip"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Associates <see cref='System.Windows.Forms.ToolTip'/> text with the specified control.
        ///    </para>
        /// </devdoc>
        public void SetToolTip(Control control, string caption) {

            TipInfo info = new TipInfo(caption, TipInfo.Type.Auto);
            SetToolTipInternal(control, info); 

        }

        /// <include file='doc\ToolTip.uex' path='docs/doc[@for="ToolTip.SetToolTipInternal"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Associates <see cref="System.Windows..Forms.ToolTip'/> text with the specified information
        ///    </para>
        /// </devdoc>
        private void SetToolTipInternal(Control control, TipInfo info) {

            // Sanity check the function parameters
            if (control == null) {
                throw new ArgumentNullException(nameof(control));
            }

            bool exists = false;
            bool empty = false;

            if (tools.ContainsKey(control)) {
                exists = true;
            }

            if (info == null || String.IsNullOrEmpty(info.Caption)) {
                empty = true;
            }

            if (exists && empty) {
                tools.Remove(control);
            }
            else if (!empty) {
                tools[control] = info;
            }

            if (!empty && !exists) {
                control.HandleCreated += new EventHandler(this.HandleCreated);
                control.HandleDestroyed += new EventHandler(this.HandleDestroyed);

                if (control.IsHandleCreated) {
                    HandleCreated(control, EventArgs.Empty);
                }
            }
            else {
                bool handlesCreated = control.IsHandleCreated
                                      && TopLevelControl != null
                                      && TopLevelControl.IsHandleCreated;

                if (exists && !empty && handlesCreated && !DesignMode) {
                    bool allocatedString;
                    NativeMethods.TOOLINFO_TOOLTIP toolInfo = GetTOOLINFO(control, info.Caption, out allocatedString);
                    try {
                        UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.TTM_SETTOOLINFO,
                                                        0, toolInfo);
                    } finally {
                        if(allocatedString && IntPtr.Zero != toolInfo.lpszText) {
                            Marshal.FreeHGlobal(toolInfo.lpszText);
                        }
                    }
                    CheckNativeToolTip(control);
                    CheckCompositeControls(control);
                }
                else if (empty && exists && !DesignMode) {

                    control.HandleCreated -= new EventHandler(this.HandleCreated);
                    control.HandleDestroyed -= new EventHandler(this.HandleDestroyed);

                    if (control.IsHandleCreated) {
                        HandleDestroyed(control, EventArgs.Empty);
                    }

                    created.Remove(control);
                }
            }
        }

        /// <include file='doc\ToolTip.uex' path='docs/doc[@for="ToolTip.ShouldSerializeAutomaticDelay"]/*' />
        /// <devdoc>
        ///    Returns true if the AutomaticDelay property should be persisted.
        /// </devdoc>
        /// <internalonly/>
        private bool ShouldSerializeAutomaticDelay() {
            if (auto) {
                if (AutomaticDelay != DEFAULT_DELAY) {
                    return true;
                }
            }
            return false;
        }

        /// <include file='doc\ToolTip.uex' path='docs/doc[@for="ToolTip.ShouldSerializeAutoPopDelay"]/*' />
        /// <devdoc>
        ///    Returns true if the AutoPopDelay property should be persisted.
        /// </devdoc>
        /// <internalonly/>
        private bool ShouldSerializeAutoPopDelay() {
            return !auto;
        }

        /// <include file='doc\ToolTip.uex' path='docs/doc[@for="ToolTip.ShouldSerializeInitialDelay"]/*' />
        /// <devdoc>
        ///    Returns true if the InitialDelay property should be persisted.
        /// </devdoc>
        /// <internalonly/>
        private bool ShouldSerializeInitialDelay() {
            return !auto;
        }

        /// <include file='doc\ToolTip.uex' path='docs/doc[@for="ToolTip.ShouldSerializeReshowDelay"]/*' />
        /// <devdoc>
        ///    Returns true if the ReshowDelay property should be persisted.
        /// </devdoc>
        /// <internalonly/>
        private bool ShouldSerializeReshowDelay() {
            return !auto;
        }


        /// <include file='doc\ToolTip.uex' path='docs/doc[@for="ToolTip.ShowTooltip"]/*' />
        /// <devdoc>
        ///    Shows a tooltip for specified text, window, and hotspot
        /// </devdoc>
        /// <internalonly/>
        private void ShowTooltip(string text, IWin32Window win, int duration) {
            if (win == null) {
                throw new ArgumentNullException(nameof(win));
            }

            Control associatedControl = win as Control;
            if (associatedControl != null) {
                NativeMethods.RECT r = new NativeMethods.RECT();
                UnsafeNativeMethods.GetWindowRect(new HandleRef(associatedControl, associatedControl.Handle), ref r);

                Cursor currentCursor = Cursor.CurrentInternal;
                Point cursorLocation = Cursor.Position;
                Point p = cursorLocation;
                
                Screen screen = Screen.FromPoint(cursorLocation);

                // Place the tool tip on the associated control if its not already there
                if ( cursorLocation.X < r.left || cursorLocation.X > r.right ||
                     cursorLocation.Y < r.top || cursorLocation.Y > r.bottom ) {

                    // calculate the dimensions of the visible rectangle which
                    // is used to estimate the upper x,y of the tooltip placement                  
                    NativeMethods.RECT visibleRect = new NativeMethods.RECT();
                    visibleRect.left = (r.left < screen.WorkingArea.Left) ? screen.WorkingArea.Left:r.left;
                    visibleRect.top = (r.top < screen.WorkingArea.Top) ? screen.WorkingArea.Top:r.top;
                    visibleRect.right = (r.right > screen.WorkingArea.Right) ? screen.WorkingArea.Right:r.right;
                    visibleRect.bottom = (r.bottom > screen.WorkingArea.Bottom) ? screen.WorkingArea.Bottom:r.bottom;
                   
                    p.X = visibleRect.left + (visibleRect.right - visibleRect.left)/2;
                    p.Y = visibleRect.top + (visibleRect.bottom - visibleRect.top)/2;
                    associatedControl.PointToClientInternal(p);
                    SetTrackPosition(p.X, p.Y);
                    SetTool(win, text, TipInfo.Type.SemiAbsolute, p);

                    if (duration > 0) {
                       StartTimer(window, duration);
                    }
                    
                }
                else {

                    TipInfo tt = (TipInfo)tools[associatedControl];
                    if (tt == null) {
                        tt = new TipInfo(text, TipInfo.Type.SemiAbsolute);
                    }
                    else {
                        tt.TipType |= TipInfo.Type.SemiAbsolute;
                        tt.Caption = text;
                    }
                    tt.Position = p;

                    if (duration > 0) {
                       if (this.originalPopupDelay == 0) {
                          this.originalPopupDelay = AutoPopDelay;
                       }
                       AutoPopDelay  = duration;
                    }
                    SetToolTipInternal(associatedControl, tt);
                }
            }        
        }


        /// <include file='doc\ToolTip.uex' path='docs/doc[@for="ToolTip.Show()"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Associates <see cref='System.Windows.Forms.ToolTip'/> with the specified control and displays it.
        ///    </para>
        /// </devdoc>
         public void Show(string text, IWin32Window window) {
            // Check if the foreground window is the TopLevelWindow
            if (HasAllWindowsPermission && IsWindowActive(window)) {
                ShowTooltip(text, window, 0);
            }
            
        }

        /// <include file='doc\ToolTip.uex' path='docs/doc[@for="ToolTip.Show1()"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Associates <see cref='System.Windows.Forms.ToolTip'/> with the specified control 
        ///       and displays it for the specified duration.
        ///    </para>
        /// </devdoc>
        public void Show(string text, IWin32Window window, int duration) {
            if (duration < 0) {
                throw new ArgumentOutOfRangeException(nameof(duration), string.Format(SR.InvalidLowBoundArgumentEx, "duration", (duration).ToString(CultureInfo.CurrentCulture), (0).ToString(CultureInfo.CurrentCulture)));
            }

            if (HasAllWindowsPermission && IsWindowActive(window)) {
                ShowTooltip(text, window, duration);
            }
        }
        
        /// <include file='doc\ToolTip.uex' path='docs/doc[@for="ToolTip.Show2()"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Associates <see cref='System.Windows.Forms.ToolTip'/> with the specified control and displays it.
        ///    </para>
        /// </devdoc>
        public void Show(string text, IWin32Window window, Point point) {
            if (window == null) {
                throw new ArgumentNullException(nameof(window));
            }

            if (HasAllWindowsPermission && IsWindowActive(window)) {
                //Set The ToolTips...
                NativeMethods.RECT r = new NativeMethods.RECT();
                UnsafeNativeMethods.GetWindowRect(new HandleRef(window, Control.GetSafeHandle(window)), ref r);
                int pointX = r.left + point.X;
                int pointY = r.top + point.Y;

                SetTrackPosition(pointX, pointY);
                SetTool(window, text, TipInfo.Type.Absolute, new Point(pointX, pointY));
            }
        }

        /// <include file='doc\ToolTip.uex' path='docs/doc[@for="ToolTip.Show3()"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Associates <see cref='System.Windows.Forms.ToolTip'/> with the specified control and displays it.
        ///    </para>
        /// </devdoc>
        public void Show(string text, IWin32Window window, Point point, int duration) {
            if (window == null) {
                throw new ArgumentNullException(nameof(window));
            }
            if (duration < 0) {
                throw new ArgumentOutOfRangeException(nameof(duration), string.Format(SR.InvalidLowBoundArgumentEx, "duration", (duration).ToString(CultureInfo.CurrentCulture), (0).ToString(CultureInfo.CurrentCulture)));
            }

            if (HasAllWindowsPermission && IsWindowActive(window)) {
                //Set The ToolTips...
                NativeMethods.RECT r = new NativeMethods.RECT();
                UnsafeNativeMethods.GetWindowRect(new HandleRef(window, Control.GetSafeHandle(window)), ref r);
                int pointX = r.left + point.X;
                int pointY = r.top + point.Y;
                SetTrackPosition(pointX, pointY);
                SetTool(window, text, TipInfo.Type.Absolute, new Point(pointX, pointY));
                StartTimer(window, duration);
            }
        }


        
        /// <include file='doc\ToolTip.uex' path='docs/doc[@for="ToolTip.Show4()"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Associates <see cref='System.Windows.Forms.ToolTip'/> with the specified control and displays it.
        ///    </para>
        /// </devdoc>
        public void Show(string text, IWin32Window window, int x, int y) {
            if (window == null) {
                throw new ArgumentNullException(nameof(window));
            }

            if (HasAllWindowsPermission && IsWindowActive(window)) {
                NativeMethods.RECT r = new NativeMethods.RECT();
                UnsafeNativeMethods.GetWindowRect(new HandleRef(window, Control.GetSafeHandle(window)), ref r);
                int pointX = r.left + x;
                int pointY = r.top + y;
                SetTrackPosition(pointX, pointY);
                SetTool(window, text, TipInfo.Type.Absolute, new Point(pointX, pointY));
            }
        }

        /// <include file='doc\ToolTip.uex' path='docs/doc[@for="ToolTip.Show5()"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Associates <see cref='System.Windows.Forms.ToolTip'/> with the specified control and displays it.
        ///    </para>
        /// </devdoc>
        public void Show(string text, IWin32Window window, int x, int y, int duration) {
            if (window == null) {
                throw new ArgumentNullException(nameof(window));
            }
            if (duration < 0) {
                throw new ArgumentOutOfRangeException(nameof(duration), string.Format(SR.InvalidLowBoundArgumentEx, "duration", (duration).ToString(CultureInfo.CurrentCulture), (0).ToString(CultureInfo.CurrentCulture)));
            }

            if (HasAllWindowsPermission && IsWindowActive(window)) {
                NativeMethods.RECT r = new NativeMethods.RECT();
                UnsafeNativeMethods.GetWindowRect(new HandleRef(window, Control.GetSafeHandle(window)), ref r);
                int pointX = r.left + x;
                int pointY = r.top + y;
                SetTrackPosition(pointX, pointY);
                SetTool(window, text, TipInfo.Type.Absolute, new Point(pointX, pointY));
                StartTimer(window, duration);
            }
        }

        internal void ShowKeyboardToolTip(string text, IKeyboardToolTip tool, int duration) {
            if (tool == null) {
                throw new ArgumentNullException(nameof(tool));
            }
            if (duration < 0) {
                throw new ArgumentOutOfRangeException(nameof(duration), string.Format(SR.InvalidLowBoundArgumentEx, nameof(duration), (duration).ToString(CultureInfo.CurrentCulture), (0).ToString(CultureInfo.CurrentCulture)));
            }

            Rectangle toolRectangle = tool.GetNativeScreenRectangle();
            // At first, place the tooltip at the middle of the tool (default location)
            int pointX = (toolRectangle.Left + toolRectangle.Right) / 2;
            int pointY = (toolRectangle.Top + toolRectangle.Bottom) / 2;
            this.SetTool(tool.GetOwnerWindow(), text, TipInfo.Type.Absolute, new Point(pointX, pointY));

            // Then look for a better ToolTip location
            Size bubbleSize;
            if (this.TryGetBubbleSize(tool, toolRectangle, out bubbleSize)) {
                Point optimalPoint = this.GetOptimalToolTipPosition(tool, toolRectangle, bubbleSize.Width, bubbleSize.Height);

                // The optimal point should be used as a tracking position
                pointX = optimalPoint.X;
                pointY = optimalPoint.Y;

                // Update TipInfo for the tool with optimal position
                TipInfo tipInfo = (TipInfo)(tools[tool] ?? tools[tool.GetOwnerWindow()]);
                tipInfo.Position = new Point(pointX, pointY);

                // Ensure that the tooltip bubble is moved to the optimal position even when a mouse tooltip is being replaced with a keyboard tooltip
                this.Reposition(optimalPoint, bubbleSize);
            }

            SetTrackPosition(pointX, pointY);
            StartTimer(tool.GetOwnerWindow(), duration);
        }

        private bool TryGetBubbleSize(IKeyboardToolTip tool, Rectangle toolRectangle, out Size bubbleSize) {
            // Get bubble size to use it for optimal position calculation
            IntPtr bubbleSizeInt = UnsafeNativeMethods.SendMessage(new HandleRef(this, this.Handle), NativeMethods.TTM_GETBUBBLESIZE, 0, this.GetMinToolInfoForTool(tool.GetOwnerWindow()));
            if (bubbleSizeInt.ToInt32() != NativeMethods.S_FALSE) {
                int width = NativeMethods.Util.LOWORD(bubbleSizeInt);
                int height = NativeMethods.Util.HIWORD(bubbleSizeInt);
                bubbleSize = new Size(width, height);
                return true;
            }
            else {
                bubbleSize = Size.Empty;
                return false;
            }
        }

        private Point GetOptimalToolTipPosition(IKeyboardToolTip tool, Rectangle toolRectangle, int width, int height) {
            // Possible tooltip locations are tied to the tool rectangle bounds
            int centeredX = toolRectangle.Left + toolRectangle.Width / 2 - width / 2; // tooltip will be aligned with tool vertically
            int centeredY = toolRectangle.Top + toolRectangle.Height / 2 - height / 2; // tooltip will be aligned with tool horizontally

            Rectangle[] possibleLocations = new Rectangle[LOCATION_TOTAL];
            possibleLocations[TOP_LOCATION_INDEX] = new Rectangle(centeredX, toolRectangle.Top - height, width, height);
            possibleLocations[RIGHT_LOCATION_INDEX] = new Rectangle(toolRectangle.Right, centeredY, width, height);
            possibleLocations[BOTTOM_LOCATION_INDEX] = new Rectangle(centeredX, toolRectangle.Bottom, width, height);
            possibleLocations[LEFT_LOCATION_INDEX] = new Rectangle(toolRectangle.Left - width, centeredY, width, height);


            // Neighboring tools should not be overlapped (ideally) by tooltip
            IList<Rectangle> neighboringToolsRectangles = tool.GetNeighboringToolsRectangles();

            // Weights are used to determine which one of the possible location overlaps least area of the neighboring tools
            long[] locationWeights = new long[LOCATION_TOTAL];

            // Check if the possible locations intersect with the neighboring tools
            for (int i = 0; i < possibleLocations.Length; i++) {
                foreach (Rectangle neighboringToolRectangle in neighboringToolsRectangles) {
                    Rectangle intersection = Rectangle.Intersect(possibleLocations[i], neighboringToolRectangle);
                    checked {
                        locationWeights[i] += Math.Abs((long)intersection.Width * intersection.Height); // Intersection is a weight
                    }
                }
            }

            // Calculate clipped area of possible locations i.e. area which is located outside the screen area
            Rectangle screenBounds = SystemInformation.VirtualScreen;
            long[] locationClippedAreas = new long[LOCATION_TOTAL];
            for (int i = 0; i < possibleLocations.Length; i++) {
                Rectangle locationAreaWithinScreen = Rectangle.Intersect(screenBounds, possibleLocations[i]);
                checked {
                    locationClippedAreas[i] = (Math.Abs((long)possibleLocations[i].Width) - Math.Abs((long)locationAreaWithinScreen.Width))
                        * (Math.Abs((long)possibleLocations[i].Height) - Math.Abs((long)locationAreaWithinScreen.Height));
                }
            }

            // Calculate area of possible locations within top level control rectangle
            long[] locationWithinTopControlAreas = new long[LOCATION_TOTAL];
            Rectangle topContainerBounds = ((IKeyboardToolTip)this.TopLevelControl)?.GetNativeScreenRectangle() ?? Rectangle.Empty;
            if (!topContainerBounds.IsEmpty) {
                for (int i = 0; i < possibleLocations.Length; i++) {
                    Rectangle locationWithinTopControlRectangle = Rectangle.Intersect(topContainerBounds, possibleLocations[i]);
                    checked {
                        locationWithinTopControlAreas[i] = Math.Abs((long)locationWithinTopControlRectangle.Height * locationWithinTopControlRectangle.Width);
                    }
                }
            }

            // Pick optimal location
            long leastWeight = locationWeights[0];
            long leastClippedArea = locationClippedAreas[0];
            long biggestAreaWithinTopControl = locationWithinTopControlAreas[0];
            int locationIndex = 0;
            Rectangle optimalLocation = possibleLocations[0];
            bool rtlEnabled = tool.HasRtlModeEnabled();
            for (int i = 1; i < possibleLocations.Length; i++) {
                if (this.IsCompetingLocationBetter(leastClippedArea, leastWeight, biggestAreaWithinTopControl, locationIndex,
                    locationClippedAreas[i], locationWeights[i], locationWithinTopControlAreas[i], i,
                    rtlEnabled)) {
                    leastWeight = locationWeights[i];
                    leastClippedArea = locationClippedAreas[i];
                    biggestAreaWithinTopControl = locationWithinTopControlAreas[i];
                    locationIndex = i;
                    optimalLocation = possibleLocations[i];
                }
            }

            return new Point(optimalLocation.Left, optimalLocation.Top);
        }

        private bool IsCompetingLocationBetter(long originalLocationClippedArea,
            long originalLocationWeight,
            long originalLocationAreaWithinTopControl,
            int originalIndex,
            long competingLocationClippedArea,
            long competingLocationWeight,
            long competingLocationAreaWithinTopControl,
            int competingIndex,
            bool rtlEnabled) {
            // Prefer location with less clipped area
            if (competingLocationClippedArea < originalLocationClippedArea) {
                return true;
            }
            // Otherwise prefer location with less weight
            else if (competingLocationWeight < originalLocationWeight) {
                return true;
            }
            else if (competingLocationWeight == originalLocationWeight && competingLocationClippedArea == originalLocationClippedArea) {
                // Prefer locations located within top level control
                if (competingLocationAreaWithinTopControl > originalLocationAreaWithinTopControl) {
                    return true;
                }
                else if (competingLocationAreaWithinTopControl == originalLocationAreaWithinTopControl) {
                    switch (originalIndex) {
                        case TOP_LOCATION_INDEX:
                            // Top location is the least preferred location
                            return true;
                        case BOTTOM_LOCATION_INDEX:
                            // Right and Left locations are preferred instead of Bottom location
                            if (competingIndex == LEFT_LOCATION_INDEX || competingIndex == RIGHT_LOCATION_INDEX) {
                                return true;
                            }
                            break;
                        case RIGHT_LOCATION_INDEX:
                            // When RTL is enabled Left location is preferred
                            if (rtlEnabled && competingIndex == LEFT_LOCATION_INDEX) {
                                return true;
                            }
                            break;
                        case LEFT_LOCATION_INDEX:
                            // When RTL is disabled Right location is preferred
                            if (!rtlEnabled && competingIndex == RIGHT_LOCATION_INDEX) {
                                return true;
                            }
                            break;
                        default:
                            throw new NotSupportedException("Unsupported location index value");
                    }
                }
            }

            return false;
        }

        /// <devdoc>
        ///     Private Function to encapsulate TTM_TRACKPOSITION so that this doesnt fire an extra POP event
        /// </devdoc>
        private void SetTrackPosition(int pointX, int pointY)
        {
            try
            {
                trackPosition = true;
                UnsafeNativeMethods.SendMessage(new HandleRef(this, this.Handle), NativeMethods.TTM_TRACKPOSITION, 0, NativeMethods.Util.MAKELONG(pointX, pointY));
            }
            finally
            {
                trackPosition= false;
            }
        }

        /// <include file='doc\ToolTip.uex' path='docs/doc[@for="ToolTip.Hide"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Hides <see cref='System.Windows.Forms.ToolTip'/> with the specified control.
        ///    </para>
        /// </devdoc>
        public void Hide(IWin32Window win) {
            if (win == null) {
                throw new ArgumentNullException(nameof(win));
            }

            if (HasAllWindowsPermission) {
                if (window == null)
                {
                    return;
                }
                
                if (GetHandleCreated())  {                    
                    IntPtr hWnd = Control.GetSafeHandle(win);
                    UnsafeNativeMethods.SendMessage(new HandleRef(this, this.Handle), NativeMethods.TTM_TRACKACTIVATE, 0, GetWinTOOLINFO(hWnd));
                    UnsafeNativeMethods.SendMessage(new HandleRef(this, this.Handle), NativeMethods.TTM_DELTOOL, 0, GetWinTOOLINFO(hWnd));                   
                }
                StopTimer();

                //Check if the passed in IWin32Window is a Control...
                Control tool = win as Control;
                if (tool == null) {
                    owners.Remove(win.Handle);
                }
                else
                {
                    if (tools.ContainsKey(tool)) {
                        SetToolInfo(tool, GetToolTip(tool));
                    }
                    else {
                        owners.Remove(win.Handle);
                    }
                    // Lets find the Form for associated Control ...
                    // and hook up to the Deactivated event to Hide the Shown tooltip
                    Form baseFrom = tool.FindFormInternal();
                    if (baseFrom != null) {
                        baseFrom.Deactivate -= new EventHandler(this.BaseFormDeactivate);
                    }
                }
                
                // Clear off the toplevel control.
                ClearTopLevelControlEvents();
                topLevelControl = null;
            }
        }
        
        private void BaseFormDeactivate(System.Object sender, System.EventArgs e){
            HideAllToolTips();

            if (!AccessibilityImprovements.UseLegacyToolTipDisplay) {
                KeyboardToolTipStateMachine.Instance.NotifyAboutFormDeactivation(this);
            }
        }

        private void HideAllToolTips() {
            Control[] ctls = new Control[owners.Values.Count];
            owners.Values.CopyTo(ctls, 0);
            for (int i=0; i<ctls.Length; i++) {
                Hide(ctls[i]);
            }
        }

        private void SetTool(IWin32Window win, string text, TipInfo.Type type, Point position) {
            Control tool = win as Control;

            if (tool != null && tools.ContainsKey(tool)) {
                bool allocatedString = false;
                NativeMethods.TOOLINFO_TOOLTIP ti = new NativeMethods.TOOLINFO_TOOLTIP();
                try {
                    ti.cbSize = Marshal.SizeOf(typeof(NativeMethods.TOOLINFO_TOOLTIP));
                    ti.hwnd = tool.Handle;
                    ti.uId = tool.Handle;
                    int ret = (int)UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.TTM_GETTOOLINFO, 0, ti);
                    if (ret != 0) {
                        ti.uFlags |= NativeMethods.TTF_TRACK;

                        if (type == TipInfo.Type.Absolute || type == TipInfo.Type.SemiAbsolute) {
                            ti.uFlags |= NativeMethods.TTF_ABSOLUTE;
                        }
                        ti.lpszText = Marshal.StringToHGlobalAuto(text);
                        allocatedString = true;
                    }

                    TipInfo tt = (TipInfo)tools[tool];
                    if (tt == null) {
                        tt = new TipInfo(text, type);
                    }
                    else {
                        tt.TipType |= type;
                        tt.Caption = text;
                    }
                    tt.Position = position;
                    tools[tool] = tt;

                    UnsafeNativeMethods.SendMessage(new HandleRef(this, this.Handle), NativeMethods.TTM_SETTOOLINFO, 0, ti);
                    UnsafeNativeMethods.SendMessage(new HandleRef(this, this.Handle), NativeMethods.TTM_TRACKACTIVATE, 1, ti);
                } finally {
                    if(allocatedString && IntPtr.Zero != ti.lpszText) {
                        Marshal.FreeHGlobal(ti.lpszText);
                    }
                }
            }
            else {
                Hide(win);

                // Need to do this BEFORE we call GetWinTOOLINFO, since it relies on the tools array to be populated
                // in order to find the toplevelparent.
                TipInfo tt = (TipInfo)tools[tool];
                if (tt == null) {
                    tt = new TipInfo(text, type);
                }
                else {
                    tt.TipType |= type;
                    tt.Caption = text;
                }
                tt.Position = position;
                tools[tool] = tt;
                
                IntPtr hWnd = Control.GetSafeHandle(win);
                owners[hWnd] = win;
                NativeMethods.TOOLINFO_TOOLTIP toolInfo = GetWinTOOLINFO(hWnd);
                toolInfo.uFlags |= NativeMethods.TTF_TRACK;

                if (type == TipInfo.Type.Absolute || type == TipInfo.Type.SemiAbsolute) {
                    toolInfo.uFlags |= NativeMethods.TTF_ABSOLUTE;
                }

                try {
                    toolInfo.lpszText = Marshal.StringToHGlobalAuto(text);
                    UnsafeNativeMethods.SendMessage(new HandleRef(this, this.Handle), NativeMethods.TTM_ADDTOOL, 0, toolInfo);
                    UnsafeNativeMethods.SendMessage(new HandleRef(this, this.Handle), NativeMethods.TTM_TRACKACTIVATE, 1, toolInfo);
                } finally {
                    if(IntPtr.Zero != toolInfo.lpszText) {
                        Marshal.FreeHGlobal(toolInfo.lpszText);
                    }
                }
            }
            
            if (tool != null) {

                // Lets find the Form for associated Control ...
                // and hook up to the Deactivated event to Hide the Shown tooltip
                Form baseFrom = tool.FindFormInternal();
                if (baseFrom != null) {
                    baseFrom.Deactivate += new EventHandler(this.BaseFormDeactivate);
                }
            }
            
        }

        /// <include file='doc\ToolTip.uex' path='docs/doc[@for="UpDownBase.ToolTip.StartTimer"]/*' />
        /// <devdoc>
        ///     Starts the timer hiding Positioned ToolTips
        /// </devdoc>
        private void StartTimer(IWin32Window owner, int interval) {

            if (timer == null) {
                timer = new ToolTipTimer(owner);      
                // Add the timer handler
                timer.Tick += new EventHandler(TimerHandler);
            }
            timer.Interval = interval;
            timer.Start();
        }

        /// <include file='doc\ToolTip.uex' path='docs/doc[@for="UpDownBase.UpDownButtons.StopTimer"]/*' />
        /// <devdoc>
        ///     Stops the timer for hiding Positioned ToolTips
        /// </devdoc>
        protected void StopTimer() {
            //Hold a local ref to timer
            //so that a posted message doesn't null this
            //out during disposal.
            ToolTipTimer timerRef = timer;
        
            if (timerRef != null) {
                timerRef.Stop();
                timerRef.Dispose();
                timer = null;
            }
        }

        /// <include file='doc\ToolTip.uex' path='docs/doc[@for="ToolTip.TimerHandler"]/*' />
        /// <devdoc>
        ///     Generates updown events when the timer calls this function.
        /// </devdoc>
        private void TimerHandler(object source, EventArgs args) {
            Hide(((ToolTipTimer)source).Host);
        }

        /// <include file='doc\ToolTip.uex' path='docs/doc[@for="ToolTip.Finalize"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Finalizes garbage collection.
        ///    </para>
        /// </devdoc>
        ~ToolTip() {
            DestroyHandle();
        }

        /// <include file='doc\ToolTip.uex' path='docs/doc[@for="ToolTip.ToString"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    <para>
        ///       Returns a string representation for this control.
        ///    </para>
        /// </devdoc>
        public override string ToString() {

            string s = base.ToString();
            return s + " InitialDelay: " + InitialDelay.ToString(CultureInfo.CurrentCulture) + ", ShowAlways: " + ShowAlways.ToString(CultureInfo.CurrentCulture);
        }

        private void Reposition(Point tipPosition, Size tipSize) {
            Point moveToLocation = tipPosition;
            Screen screen = Screen.FromPoint(moveToLocation);

            // Re-adjust the X position of the tool tip if it bleeds off the screen working area
            if (moveToLocation.X + tipSize.Width > screen.WorkingArea.Right) {
                moveToLocation.X = screen.WorkingArea.Right - tipSize.Width;
            }

            // re-adjust the Y position of the tool tip if it bleeds off the screen working area.
            if (moveToLocation.Y + tipSize.Height> screen.WorkingArea.Bottom) {
                moveToLocation.Y = screen.WorkingArea.Bottom - tipSize.Height;
            } 

            SafeNativeMethods.SetWindowPos(new HandleRef(this, this.Handle),
            NativeMethods.HWND_TOPMOST,
            moveToLocation.X, moveToLocation.Y, tipSize.Width, tipSize.Height,
            NativeMethods.SWP_NOACTIVATE | NativeMethods.SWP_NOSIZE | NativeMethods.SWP_NOOWNERZORDER);
        }

        /// <include file='doc\ToolTip.uex' path='docs/doc[@for="ToolTip.WmMove"]/*' />
        /// <devdoc>
        ///     Handles the WM_MOVE message.
        /// </devdoc>
        /// <internalonly/>
        private void WmMove() {
            NativeMethods.RECT r = new NativeMethods.RECT();
            UnsafeNativeMethods.GetWindowRect(new HandleRef(this, Handle), ref r);
            NativeMethods.TOOLINFO_TOOLTIP ti = new NativeMethods.TOOLINFO_TOOLTIP();
            ti.cbSize = Marshal.SizeOf(typeof(NativeMethods.TOOLINFO_TOOLTIP));
            int ret = (int)UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.TTM_GETCURRENTTOOL, 0, ti);
            if (ret != 0)
            {
                IWin32Window win = (IWin32Window)owners[ti.hwnd];
                if (win == null) {
                    win = (IWin32Window)Control.FromHandleInternal(ti.hwnd);
                }
                
                if (win == null) {
                    return;
                }
                
                TipInfo tt = (TipInfo)tools[win];
                if (win == null || tt==null) {
                   return;
                }

                // Treeview handles its own ToolTips.
                TreeView treeView = win as TreeView;
                if (treeView != null) {
                    if (treeView.ShowNodeToolTips) {
                        return; 
                    }
                }

                // Reposition the tooltip when its about to be shown.. since the tooltip can go out of screen workingarea bounds
                // Reposition would check the bounds for us.
                if (tt.Position != Point.Empty)
                {
                    Reposition(tt.Position, r.Size);
                }
             }
        }


        /// <include file='doc\ToolTip.uex' path='docs/doc[@for="ToolTip.WmMouseActivate"]/*' />
        /// <devdoc>
        ///     Handles the WM_MOUSEACTIVATE message.
        /// </devdoc>
        /// <internalonly/>
        private void WmMouseActivate(ref Message msg) {
            
            NativeMethods.TOOLINFO_TOOLTIP ti = new NativeMethods.TOOLINFO_TOOLTIP();
            ti.cbSize = Marshal.SizeOf(typeof(NativeMethods.TOOLINFO_TOOLTIP));
            int ret = (int)UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.TTM_GETCURRENTTOOL, 0, ti);
            
            if (ret != 0) {

                IWin32Window win = (IWin32Window)owners[ti.hwnd];
                if (win == null) {
                    win = (IWin32Window)Control.FromHandleInternal(ti.hwnd);
                }

                if (win == null) {
                    return;
                }

                NativeMethods.RECT r = new NativeMethods.RECT();
                UnsafeNativeMethods.GetWindowRect(new HandleRef(win, Control.GetSafeHandle(win)), ref r);
                Point cursorLocation = Cursor.Position;

                // Do not activate the mouse if its within the bounds of the
                // the associated tool
                if (cursorLocation.X >= r.left && cursorLocation.X <= r.right &&
                    cursorLocation.Y >= r.top && cursorLocation.Y <= r.bottom) {
                    msg.Result = (IntPtr)NativeMethods.MA_NOACTIVATE;
                }
            }
        }



        /// <include file='doc\ToolTip.uex' path='docs/doc[@for="ToolTip.WmWindowFromPoint"]/*' />
        /// <devdoc>
        ///     Handles the WM_WINDOWFROMPOINT message.
        /// </devdoc>
        /// <internalonly/>
        private void WmWindowFromPoint(ref Message msg) {
            NativeMethods.POINT sc = (NativeMethods.POINT)msg.GetLParam(typeof(NativeMethods.POINT));
            Point screenCoords = new Point(sc.x, sc.y);
            bool result = false;
            msg.Result = GetWindowFromPoint(screenCoords, ref result);
        }



        /// <include file='doc\ToolTip.uex' path='docs/doc[@for="ToolTip.WmShow"]/*' />
        /// <devdoc>
        ///     Handles the TTN_SHOW message.
        /// </devdoc>
        /// <internalonly/>
        private void WmShow() {


            //Get the Bounds....
            NativeMethods.RECT r = new NativeMethods.RECT();
            UnsafeNativeMethods.GetWindowRect(new HandleRef(this, Handle), ref r);

            NativeMethods.TOOLINFO_TOOLTIP ti = new NativeMethods.TOOLINFO_TOOLTIP();
            ti.cbSize = Marshal.SizeOf(typeof(NativeMethods.TOOLINFO_TOOLTIP));
            int ret = (int)UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.TTM_GETCURRENTTOOL, 0, ti);
            
            if (ret != 0) {

                IWin32Window win = (IWin32Window)owners[ti.hwnd];
                if (win == null) {
                    win = (IWin32Window)Control.FromHandleInternal(ti.hwnd);
                }
                
                if (win == null) {
                    return;
                }

                Control toolControl = win as Control;

                Size currentTooltipSize = r.Size;
                PopupEventArgs e = new PopupEventArgs(win, toolControl, IsBalloon, currentTooltipSize); 
                OnPopup(e);

                DataGridView dataGridView = toolControl as DataGridView;
                if (dataGridView != null && dataGridView.CancelToolTipPopup(this))
                {
                    // The dataGridView cancelled the tooltip.
                    e.Cancel = true;
                }

                // We need to re-get the rectangle of the tooltip here because
                // any of the tooltip attributes/properties could have been updated
                // during the popup event; in which case the size of the tooltip is
                // affected. e.ToolTipSize is respected over r.Size
                UnsafeNativeMethods.GetWindowRect(new HandleRef(this, Handle), ref r);
                currentTooltipSize = (e.ToolTipSize == currentTooltipSize) ? r.Size:e.ToolTipSize;


                if (IsBalloon) {
                  // Get the text display rectangle
                   UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.TTM_ADJUSTRECT, 1, ref r);
                   if (r.Size.Height > currentTooltipSize.Height) currentTooltipSize.Height = r.Size.Height;
                }

                // Set the max possible size of the tooltip to the size we received.
                // This prevents the operating system from drawing incorrect rectangles
                // when determing the correct display rectangle
                // Set the MaxWidth only if user has changed the width.
                if (currentTooltipSize != r.Size)
                {
                    Screen screen = Screen.FromPoint(Cursor.Position);
                    int maxwidth = (IsBalloon) ?
                    Math.Min(currentTooltipSize.Width-2*XBALLOONOFFSET, screen.WorkingArea.Width):
                    Math.Min(currentTooltipSize.Width, screen.WorkingArea.Width);
                    UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.TTM_SETMAXTIPWIDTH, 0, maxwidth);
                }

                if (e.Cancel) { 
                    cancelled = true;
                    SafeNativeMethods.SetWindowPos(new HandleRef(this, this.Handle),
                    NativeMethods.HWND_TOPMOST,
                    0, 0, 0, 0,
					NativeMethods.SWP_NOACTIVATE | NativeMethods.SWP_NOOWNERZORDER);

                }
                else {
                    cancelled = false;
                    // Only width/height changes are respected, so set top,left to what we got earlier
                    SafeNativeMethods.SetWindowPos(new HandleRef(this, this.Handle),
                    NativeMethods.HWND_TOPMOST,
                    r.left, r.top, currentTooltipSize.Width, currentTooltipSize.Height,
                    NativeMethods.SWP_NOACTIVATE | NativeMethods.SWP_NOOWNERZORDER);
                }
            }
        }

        /// <include file='doc\ToolTip.uex' path='docs/doc[@for="ToolTip.WmWindowPosChanged"]/*' />
        /// <devdoc>
        ///     Handles the WM_WINDOWPOSCHANGED message.
        ///     We need to Hide the window since the native tooltip actually calls SetWindowPos in its TTN_SHOW even if we cancel showing the
        ///     tooltip : Hence we need to listen to the WindowPosChanged message can hide the window ourselves.
        /// </devdoc>
        /// <internalonly/>
        private bool WmWindowPosChanged() {
            if (cancelled)
            {
               SafeNativeMethods.ShowWindow(new HandleRef(this, Handle), NativeMethods.SW_HIDE);
               return true;
            }
            return false;
        }



        /// <include file='doc\ToolTip.uex' path='docs/doc[@for="ToolTip.WmWindowPosChanging"]/*' />
        /// <devdoc>
        ///     Handles the WM_WINDOWPOSCHANGING message.
        /// </devdoc>
        /// <internalonly/>
        private unsafe void WmWindowPosChanging(ref Message m) {
            if (cancelled || isDisposing)
            {
            	return;
            }

            NativeMethods.WINDOWPOS* wp = (NativeMethods.WINDOWPOS *)m.LParam;
            
            Cursor currentCursor = Cursor.CurrentInternal;
            Point cursorPos = Cursor.Position;

			

            NativeMethods.TOOLINFO_TOOLTIP ti = new NativeMethods.TOOLINFO_TOOLTIP();
            ti.cbSize = Marshal.SizeOf(typeof(NativeMethods.TOOLINFO_TOOLTIP));
            int ret = (int)UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.TTM_GETCURRENTTOOL, 0, ti);
            if (ret != 0) {

                IWin32Window win = (IWin32Window)owners[ti.hwnd];
                if (win == null) {
                    win = (IWin32Window)Control.FromHandleInternal(ti.hwnd);
                }

                if (win == null || !IsWindowActive(win)) {
                    return;
                }

                TipInfo tt = null;
                if (win != null)
                {
                    tt = (TipInfo)tools[win];
                    if (tt == null) {
                       return;
                    }

                    // Treeview handles its own ToolTips.
                    TreeView treeView = win as TreeView;
                    if (treeView != null) {
                        if (treeView.ShowNodeToolTips) {
                            return; 
                        }
                    }
                }

                if (IsBalloon) {
                   wp->cx += 2*XBALLOONOFFSET;
                   return;
                }

                if ( (tt.TipType & TipInfo.Type.Auto) != 0 && window != null )
                {
                    window.DefWndProc(ref m);
                    return;
                }

                if ( ((tt.TipType & TipInfo.Type.SemiAbsolute) != 0) && tt.Position == Point.Empty ) {

                   Screen screen = Screen.FromPoint(cursorPos);
                   if (currentCursor != null)
                   {
                        wp->x = cursorPos.X;
                        // Since HotSpot requires a security demand .. we assert this and revert Assert immediately 
                        try {
                            IntSecurity.ObjectFromWin32Handle.Assert();
                             
                            wp->y = cursorPos.Y;
                            if (wp->y + wp->cy + currentCursor.Size.Height - currentCursor.HotSpot.Y > screen.WorkingArea.Bottom) {
                               wp->y = cursorPos.Y - wp->cy;
                            }
                            else {
                               wp->y = cursorPos.Y + currentCursor.Size.Height - currentCursor.HotSpot.Y;
                            }
                        }
                        finally {
                            CodeAccessPermission.RevertAssert();
                        }
                   }
                   if (wp->x + wp->cx >screen.WorkingArea.Right) {
                      wp->x = screen.WorkingArea.Right - wp->cx;
                   }

                }
                else if ((tt.TipType & TipInfo.Type.SemiAbsolute) != 0 && tt.Position != Point.Empty) {

                   Screen screen = Screen.FromPoint(tt.Position);
                   wp->x = tt.Position.X;
                   if (wp->x + wp->cx >screen.WorkingArea.Right) {
                      wp->x = screen.WorkingArea.Right - wp->cx;
                   }
                   wp->y = tt.Position.Y;
                   
                   if (wp->y + wp->cy > screen.WorkingArea.Bottom) {
                        wp->y = screen.WorkingArea.Bottom - wp->cy;
                   }
                }
            }

            m.Result = IntPtr.Zero; 
        }

        /// <include file='doc\ToolTip.uex' path='docs/doc[@for="ToolTip.WmPop"]/*' />
        /// <devdoc>
        ///     Called just before the tooltip is hidden
        /// </devdoc>
        /// <internalonly/>
        private void WmPop() {

            NativeMethods.TOOLINFO_TOOLTIP ti = new NativeMethods.TOOLINFO_TOOLTIP();
            ti.cbSize = Marshal.SizeOf(typeof(NativeMethods.TOOLINFO_TOOLTIP));
            int ret = (int)UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.TTM_GETCURRENTTOOL, 0, ti);
            if (ret != 0) {

                IWin32Window win = (IWin32Window)owners[ti.hwnd];
                if (win == null) {
                    win = (IWin32Window)Control.FromHandleInternal(ti.hwnd);
                }

                if (win == null) {
                    return;
                }

                Control control = win as Control;
                TipInfo tt = (TipInfo)tools[win];
                if (tt == null) {
                   return;
                }

                // Must reset the maxwidth to the screen size.
                if ((tt.TipType & TipInfo.Type.Auto) != 0 || (tt.TipType & TipInfo.Type.SemiAbsolute) != 0) {
                   Screen screen = Screen.FromPoint(Cursor.Position);
                   UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.TTM_SETMAXTIPWIDTH, 0, screen.WorkingArea.Width);
                }

                // For non-auto tips (those showned through
                // the show(...) methods, we need to dissassociate
                // them from the tip control.
                if ((tt.TipType & TipInfo.Type.Auto) == 0) {

                     tools.Remove(control); 
                     owners.Remove(win.Handle);

                     control.HandleCreated -= new EventHandler(this.HandleCreated);
                     control.HandleDestroyed -= new EventHandler(this.HandleDestroyed);
                     created.Remove(control);

                     if (originalPopupDelay != 0) {
                        AutoPopDelay = originalPopupDelay;
                        originalPopupDelay = 0;
                     }
                }
                else {
                     // Clear all other flags except for the 
                     // Auto flag to ensure automatic tips can still show
                     tt.TipType = TipInfo.Type.Auto;
                     tt.Position = Point.Empty;
                     tools[control] = tt;
                }
            }
        }

        /// <include file='doc\ToolTip.uex' path='docs/doc[@for="ToolTip.WndProc"]/*' />
        /// <devdoc>
        ///     WNDPROC
        /// </devdoc>
        /// <internalonly/>
        private void WndProc(ref Message msg) {


            switch (msg.Msg) {

            case NativeMethods.WM_REFLECT + NativeMethods.WM_NOTIFY:
                 NativeMethods.NMHDR nmhdr = (NativeMethods.NMHDR) msg.GetLParam(typeof(NativeMethods.NMHDR));
                 if (nmhdr.code == NativeMethods.TTN_SHOW && !trackPosition) {
                     WmShow();
                 }
                 else if (nmhdr.code == NativeMethods.TTN_POP) {                    
                    WmPop();
                    if (window != null) {
                        window.DefWndProc(ref msg);
                    }
                 } 
                 break;                
            
            case NativeMethods.WM_WINDOWPOSCHANGING:
                 WmWindowPosChanging(ref msg);
                 break;
			
            case NativeMethods.WM_WINDOWPOSCHANGED:
                 if (!WmWindowPosChanged() && window != null)
                 {
                    window.DefWndProc(ref msg);
                 }
                 break;
					 
            case NativeMethods.WM_MOUSEACTIVATE:
                 WmMouseActivate(ref msg);
                 break;

            case NativeMethods.WM_MOVE:
                 WmMove();
                 break;
            
            case NativeMethods.TTM_WINDOWFROMPOINT:
                WmWindowFromPoint(ref msg);
                break;
   
            case NativeMethods.WM_PRINTCLIENT:
                goto case NativeMethods.WM_PAINT;
                
            case NativeMethods.WM_PAINT:
                if (ownerDraw && !isBalloon && !trackPosition)
                {
                    NativeMethods.PAINTSTRUCT ps = new NativeMethods.PAINTSTRUCT();
                    IntPtr dc = UnsafeNativeMethods.BeginPaint(new HandleRef(this,Handle),ref ps);
                    Graphics g = Graphics.FromHdcInternal(dc);
                    try {
                        Rectangle bounds = new Rectangle(ps.rcPaint_left,ps.rcPaint_top,
                        ps.rcPaint_right - ps.rcPaint_left,
                        ps.rcPaint_bottom - ps.rcPaint_top);
                        if (bounds == Rectangle.Empty ) {
                            return;
                        }
                        NativeMethods.TOOLINFO_TOOLTIP ti = new NativeMethods.TOOLINFO_TOOLTIP();
                        ti.cbSize = Marshal.SizeOf(typeof(NativeMethods.TOOLINFO_TOOLTIP));
                        int ret = unchecked( (int) (long)UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.TTM_GETCURRENTTOOL, 0, ti));
                        if (ret != 0) {
                            IWin32Window win = (IWin32Window)owners[ti.hwnd];
                            Control ac = Control.FromHandleInternal(ti.hwnd);
                            if (win == null) {
                                win = (IWin32Window)ac;
                            }
                            Font font;
                            IntSecurity.ObjectFromWin32Handle.Assert();
                            try {
                                font = Font.FromHfont(UnsafeNativeMethods.SendMessage(new HandleRef(this, Handle), NativeMethods.WM_GETFONT, 0, 0));
                            }
                            catch (ArgumentException) {
                                // If the current default tooltip font is a non-TrueType font, then
                                // Font.FromHfont throws this exception, so fall back to the default control font.
                                font = Control.DefaultFont;
                            }
                            finally {
                                CodeAccessPermission.RevertAssert();
                            }

                            OnDraw(new DrawToolTipEventArgs(g, win, ac, bounds, GetToolTip(ac), 
                                                            BackColor, ForeColor, font));
    
                            break;
                        }
                    }
                    finally
                    {
                        g.Dispose();
                        UnsafeNativeMethods.EndPaint(new HandleRef(this,Handle),ref ps);
                    }
                }

                //If not OwnerDraw, fall through
                goto default;
            default:
                if (window != null) {
                    window.DefWndProc(ref msg);
                }
                break;
            }
        }

        /// <include file='doc\ToolTip.uex' path='docs/doc[@for="ToolTip.ToolTipNativeWindow"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        private class ToolTipNativeWindow : NativeWindow {
            ToolTip control;

            internal ToolTipNativeWindow(ToolTip control) {
                this.control = control;
            }
            
        
        protected override void WndProc(ref Message m) {
            if (control != null) {
            control.WndProc(ref m);
                }
            }
        }

        private class ToolTipTimer : Timer {
            IWin32Window host;

            public ToolTipTimer(IWin32Window owner) : base(){
                this.host = owner;
            }

            public IWin32Window Host {
                get {
                    return host;
                }
            }
        }


        private class TipInfo {

           [Flags]
           public enum Type { 
               None = 0x0000,
               Auto = 0x0001,
               Absolute = 0x0002,
               SemiAbsolute = 0x0004
           };
           
           public  Type     TipType = Type.Auto;
           private string   caption;
           private string   designerText;
           public  Point    Position =  Point.Empty;

           public TipInfo(string caption, Type type) {
               this.caption = caption;
               this.TipType = type;
               if (type == Type.Auto) {
                   this.designerText = caption;
               }
           }

           public string Caption {
               get {
                   return ((this.TipType & (Type.Absolute | Type.SemiAbsolute)) != 0) ? caption:designerText;
               }
               set {
                   this.caption = value;
               }
           }
        }

    }
}
