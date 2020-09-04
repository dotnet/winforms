// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using static Interop;
using static Interop.ComCtl32;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Provides a small pop-up window containing a line of text that describes the purpose of a
    ///  tool or control (usually represented as a graphical object) in a program.
    /// </summary>
    [ProvideProperty(nameof(ToolTip), typeof(Control))]
    [DefaultEvent(nameof(Popup))]
    [ToolboxItemFilter("System.Windows.Forms")]
    [SRDescription(nameof(SR.DescriptionToolTip))]
    public class ToolTip : Component, IExtenderProvider, IHandle
    {
        private const int DefaultDelay = 500;
        private const int ReshowRatio = 5;
        private const int AutoPopRatio = 10;

        private const int BalloonOffsetX = 10;

        private const int LocationIndexTop = 0;
        private const int LocationIndexRight = 1;
        private const int LocationIndexBottom = 2;
        private const int LocationIndexLeft = 3;
        private const int LocationTotal = 4;
        private readonly Hashtable _tools = new Hashtable();
        private readonly int[] _delayTimes = new int[4];
        private bool _auto = true;
        private bool _showAlways;
        private ToolTipNativeWindow _window;
        private Control _topLevelControl;
        private bool active = true;
        private Color _backColor = SystemColors.Info;
        private Color _foreColor = SystemColors.InfoText;
        private bool _isBalloon;
        private bool _isDisposing;
        private string _toolTipTitle = string.Empty;
        private ToolTipIcon _toolTipIcon = ToolTipIcon.None;
        private ToolTipTimer _timer;
        private readonly Hashtable _owners = new Hashtable();
        private bool _stripAmpersands;
        private bool _useAnimation = true;
        private bool _useFading = true;
        private int _originalPopupDelay;

        /// <summary>
        ///  Setting TTM_TRACKPOSITION will cause redundant POP and Draw Messages.
        ///  Hence we guard against this by having this private flag.
        /// </summary>
        private bool _trackPosition;

        private PopupEventHandler _onPopup;
        private DrawToolTipEventHandler _onDraw;

        /// <summary>
        ///  Adding a tool twice breaks the ToolTip, so we need to track which
        ///  tools are created to prevent this.
        /// </summary>
        private readonly Hashtable _created = new Hashtable();

        private bool _cancelled;

        /// <summary>
        ///  Initializes a new instance of the <see cref="ToolTip"/> class, given the container.
        /// </summary>
        public ToolTip(IContainer cont) : this()
        {
            if (cont is null)
            {
                throw new ArgumentNullException(nameof(cont));
            }

            cont.Add(this);
        }

        /// <summary>
        ///  Initializes a new instance of the <see cref="ToolTip"/> class in its default state.
        /// </summary>
        public ToolTip()
        {
            _window = new ToolTipNativeWindow(this);
            _auto = true;
            _delayTimes[(int)TTDT.AUTOMATIC] = DefaultDelay;
            AdjustBaseFromAuto();
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the <see cref="ToolTip"/> control is currently active.
        /// </summary>
        [SRDescription(nameof(SR.ToolTipActiveDescr))]
        [DefaultValue(true)]
        public bool Active
        {
            get => active;
            set
            {
                if (active != value)
                {
                    active = value;

                    // Don't activate the tooltip if we're in the designer.
                    if (!DesignMode && GetHandleCreated())
                    {
                        User32.SendMessageW(this, (User32.WM)TTM.ACTIVATE, PARAM.FromBool(value));
                    }
                }
            }
        }

        internal void HideToolTip(IKeyboardToolTip currentTool)
        {
            IWin32Window ownerWindow = currentTool.GetOwnerWindow();
            if (ownerWindow != null)
            {
                Hide(ownerWindow);
            }
        }

        /// <summary>
        ///  Gets or sets the time (in milliseconds) that passes before the <see cref="ToolTip"/> appears.
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        [SRDescription(nameof(SR.ToolTipAutomaticDelayDescr))]
        [DefaultValue(DefaultDelay)]
        public int AutomaticDelay
        {
            get => _delayTimes[(int)TTDT.AUTOMATIC];
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidLowBoundArgumentEx, nameof(AutomaticDelay), value, 0));
                }

                SetDelayTime((int)TTDT.AUTOMATIC, value);
            }
        }

        internal string GetCaptionForTool(Control tool)
        {
            Debug.Assert(tool != null, "tool should not be null");
            return ((TipInfo)_tools[tool])?.Caption;
        }

        /// <summary>
        ///  Gets or sets the initial delay for the <see cref="ToolTip"/> control.
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        [SRDescription(nameof(SR.ToolTipAutoPopDelayDescr))]
        public int AutoPopDelay
        {
            get => _delayTimes[(int)TTDT.AUTOPOP];
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidLowBoundArgumentEx, nameof(AutoPopDelay), value, 0));
                }

                SetDelayTime(TTDT.AUTOPOP, value);
            }
        }

        /// <summary>
        ///  Gets or sets the BackColor for the <see cref="ToolTip"/> control.
        /// </summary>
        [SRDescription(nameof(SR.ToolTipBackColorDescr))]
        [DefaultValue(typeof(Color), "Info")]
        public Color BackColor
        {
            get => _backColor;
            set
            {
                _backColor = value;
                if (GetHandleCreated())
                {
                    User32.SendMessageW(this, (User32.WM)TTM.SETTIPBKCOLOR, PARAM.FromColor(_backColor));
                }
            }
        }

        /// <summary>
        ///  The CreateParams to create the window.
        /// </summary>
        protected virtual CreateParams CreateParams
        {
            get
            {
                CreateParams cp = new CreateParams();
                if (TopLevelControl != null && !TopLevelControl.IsDisposed)
                {
                    cp.Parent = TopLevelControl.Handle;
                }
                cp.ClassName = WindowClasses.TOOLTIPS_CLASS;
                if (_showAlways)
                {
                    cp.Style = (int)TTS.ALWAYSTIP;
                }
                if (_isBalloon)
                {
                    cp.Style |= (int)TTS.BALLOON;
                }
                if (!_stripAmpersands)
                {
                    cp.Style |= (int)TTS.NOPREFIX;
                }
                if (!_useAnimation)
                {
                    cp.Style |= (int)TTS.NOANIMATE;
                }
                if (!_useFading)
                {
                    cp.Style |= (int)TTS.NOFADE;
                }
                cp.ExStyle = 0;
                cp.Caption = null;

                return cp;
            }
        }

        /// <summary>
        ///  Gets or sets the ForeColor for the <see cref="ToolTip"/> control.
        /// </summary>
        [SRDescription(nameof(SR.ToolTipForeColorDescr))]
        [DefaultValue(typeof(Color), "InfoText")]
        public Color ForeColor
        {
            get => _foreColor;
            set
            {
                if (value.IsEmpty)
                {
                    throw new ArgumentException(string.Format(SR.ToolTipEmptyColor, nameof(ForeColor)), nameof(value));
                }

                _foreColor = value;
                if (GetHandleCreated())
                {
                    User32.SendMessageW(this, (User32.WM)TTM.SETTIPTEXTCOLOR, PARAM.FromColor(_foreColor));
                }
            }
        }

        IntPtr IHandle.Handle => Handle;

        internal IntPtr Handle
        {
            get
            {
                if (!GetHandleCreated())
                {
                    CreateHandle();
                }

                return _window.Handle;
            }
        }

        /// <summary>
        ///  Shows if the keyboard tooltip is currently active.
        /// </summary>
        internal bool IsActivatedByKeyboard { get; set; }

        /// <summary>
        ///  Gets or sets the IsBalloon for the <see cref="ToolTip"/> control.
        /// </summary>
        [SRDescription(nameof(SR.ToolTipIsBalloonDescr))]
        [DefaultValue(false)]
        public bool IsBalloon
        {
            get => _isBalloon;
            set
            {
                if (_isBalloon != value)
                {
                    _isBalloon = value;
                    if (GetHandleCreated())
                    {
                        RecreateHandle();
                    }
                }
            }
        }

        private bool IsWindowActive(IWin32Window window)
        {
            // We want to enter in the if block only if ShowParams does not return SW_SHOWNOACTIVATE.
            // for ToolStripDropDown ShowParams returns SW_SHOWNOACTIVATE, in which case we don't
            // want to check IsWindowActive and hence return true.
            if (window is Control windowControl &&
                (windowControl.ShowParams & (User32.SW)0xF) != User32.SW.SHOWNOACTIVATE)
            {
                IntPtr hWnd = User32.GetActiveWindow();
                IntPtr rootHwnd = User32.GetAncestor(windowControl, User32.GA.ROOT);
                if (hWnd != rootHwnd)
                {
                    TipInfo tt = (TipInfo)_tools[windowControl];
                    if (tt != null && (tt.TipType & TipInfo.Type.SemiAbsolute) != 0)
                    {
                        _tools.Remove(windowControl);
                        DestroyRegion(windowControl);
                    }

                    return false;
                }
            }

            return true;
        }

        /// <summary>
        ///  Gets or sets the initial delay for the <see cref="ToolTip"/> control.
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        [SRDescription(nameof(SR.ToolTipInitialDelayDescr))]
        public int InitialDelay
        {
            get => _delayTimes[(int)TTDT.INITIAL];
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidLowBoundArgumentEx, nameof(InitialDelay), value, 0));
                }

                SetDelayTime(TTDT.INITIAL, value);
            }
        }

        /// <summary>
        ///  Indicates whether the ToolTip will be drawn by the system or the user.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [DefaultValue(false)]
        [SRDescription(nameof(SR.ToolTipOwnerDrawDescr))]
        public bool OwnerDraw { get; set; }

        /// <summary>
        ///  Gets or sets the length of time (in milliseconds) that it takes subsequent ToolTip
        ///  instances to appear as the mouse pointer moves from one ToolTip region to another.
        /// </summary>
        [RefreshProperties(RefreshProperties.All)]
        [SRDescription(nameof(SR.ToolTipReshowDelayDescr))]
        public int ReshowDelay
        {
            get => _delayTimes[(int)TTDT.RESHOW];
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(value), value, string.Format(SR.InvalidLowBoundArgumentEx, nameof(ReshowDelay), value, 0));
                }

                SetDelayTime(TTDT.RESHOW, value);
            }
        }

        /// <summary>
        ///  Gets or sets a value indicating whether the <see cref="ToolTip"/> appears even when its
        ///  parent control is not active.
        /// </summary>
        [DefaultValue(false)]
        [SRDescription(nameof(SR.ToolTipShowAlwaysDescr))]
        public bool ShowAlways
        {
            get => _showAlways;
            set
            {
                if (_showAlways != value)
                {
                    _showAlways = value;
                    if (GetHandleCreated())
                    {
                        RecreateHandle();
                    }
                }
            }
        }

        /// <summary>
        ///  When set to true, any ampersands in the Text property are not displayed.
        /// </summary>
        [SRDescription(nameof(SR.ToolTipStripAmpersandsDescr))]
        [Browsable(true)]
        [DefaultValue(false)]
        public bool StripAmpersands
        {
            get => _stripAmpersands;
            set
            {
                if (_stripAmpersands != value)
                {
                    _stripAmpersands = value;
                    if (GetHandleCreated())
                    {
                        RecreateHandle();
                    }
                }
            }
        }

        [SRCategory(nameof(SR.CatData))]
        [Localizable(false)]
        [Bindable(true)]
        [SRDescription(nameof(SR.ControlTagDescr))]
        [DefaultValue(null)]
        [TypeConverter(typeof(StringConverter))]
        public object Tag { get; set; }

        /// <summary>
        ///  Gets or sets an Icon on the ToolTip.
        /// </summary>
        [DefaultValue(ToolTipIcon.None)]
        [SRDescription(nameof(SR.ToolTipToolTipIconDescr))]
        public ToolTipIcon ToolTipIcon
        {
            get => _toolTipIcon;
            set
            {
                if (_toolTipIcon != value)
                {
                    if (!ClientUtils.IsEnumValid(value, (int)value, (int)ToolTipIcon.None, (int)ToolTipIcon.Error))
                    {
                        throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(ToolTipIcon));
                    }

                    _toolTipIcon = value;
                    if (_toolTipIcon > 0 && GetHandleCreated())
                    {
                        // If the title is null/empty, the icon won't display.
                        string title = !string.IsNullOrEmpty(_toolTipTitle) ? _toolTipTitle : " ";
                        User32.SendMessageW(this, (User32.WM)TTM.SETTITLEW, (IntPtr)_toolTipIcon, title);

                        // Tooltip need to be updated to reflect the changes in the icon because
                        // this operation directly affects the size of the tooltip
                        User32.SendMessageW(this, (User32.WM)TTM.UPDATE);
                    }
                }
            }
        }

        /// <summary>
        ///  Gets or sets the title of the ToolTip.
        /// </summary>
        [DefaultValue("")]
        [SRDescription(nameof(SR.ToolTipTitleDescr))]
        public string ToolTipTitle
        {
            get => _toolTipTitle;
            set
            {
                if (value is null)
                {
                    value = string.Empty;
                }

                if (_toolTipTitle != value)
                {
                    _toolTipTitle = value;
                    if (GetHandleCreated())
                    {
                        User32.SendMessageW(this, (User32.WM)TTM.SETTITLEW, (IntPtr)_toolTipIcon, _toolTipTitle);

                        // Tooltip need to be updated to reflect the changes in the titletext because
                        // this operation directly affects the size of the tooltip
                        User32.SendMessageW(this, (User32.WM)TTM.UPDATE);
                    }
                }
            }
        }

        private Control TopLevelControl
        {
            get
            {
                if (_topLevelControl != null)
                {
                    return _topLevelControl;
                }

                Control baseVar = null;
                Control[] regions = new Control[_tools.Keys.Count];
                _tools.Keys.CopyTo(regions, 0);
                for (int i = 0; i < regions.Length; i++)
                {
                    Control ctl = regions[i];
                    baseVar = ctl.TopLevelControlInternal;
                    if (baseVar != null)
                    {
                        break;
                    }

                    if (ctl.IsActiveX)
                    {
                        baseVar = ctl;
                        break;
                    }

                    // In the designer, baseVar can be null since the Parent is not a TopLevel control
                    if (baseVar is null)
                    {
                        if (ctl != null && ctl.ParentInternal != null)
                        {
                            while (ctl.ParentInternal != null)
                            {
                                ctl = ctl.ParentInternal;
                            }

                            baseVar = ctl;
                            if (baseVar != null)
                            {
                                break;
                            }
                        }
                    }
                }

                _topLevelControl = baseVar;
                if (baseVar != null)
                {
                    baseVar.HandleCreated += new EventHandler(TopLevelCreated);
                    baseVar.HandleDestroyed += new EventHandler(TopLevelDestroyed);
                    if (baseVar.IsHandleCreated)
                    {
                        TopLevelCreated(baseVar, EventArgs.Empty);
                    }

                    baseVar.ParentChanged += new EventHandler(OnTopLevelPropertyChanged);
                }

                return baseVar;
            }
        }

        /// <summary>
        ///  When set to true, animations are used when tooltip is shown or hidden.
        /// </summary>
        [SRDescription(nameof(SR.ToolTipUseAnimationDescr))]
        [Browsable(true)]
        [DefaultValue(true)]
        public bool UseAnimation
        {
            get => _useAnimation;
            set
            {
                if (_useAnimation != value)
                {
                    _useAnimation = value;
                    if (GetHandleCreated())
                    {
                        RecreateHandle();
                    }
                }
            }
        }

        /// <summary>
        ///  When set to true, a fade effect is used when tooltips are shown or hidden.
        /// </summary>
        [SRDescription(nameof(SR.ToolTipUseFadingDescr))]
        [Browsable(true)]
        [DefaultValue(true)]
        public bool UseFading
        {
            get => _useFading;
            set
            {
                if (_useFading != value)
                {
                    _useFading = value;
                    if (GetHandleCreated())
                    {
                        RecreateHandle();
                    }
                }
            }
        }

        /// <summary>
        ///  Fires in OwnerDraw mode when the tooltip needs to be drawn.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.ToolTipDrawEventDescr))]
        public event DrawToolTipEventHandler Draw
        {
            add => _onDraw += value;
            remove => _onDraw -= value;
        }

        /// <summary>
        ///  Fires when the tooltip is just about to be shown.
        /// </summary>
        [SRCategory(nameof(SR.CatBehavior))]
        [SRDescription(nameof(SR.ToolTipPopupEventDescr))]
        public event PopupEventHandler Popup
        {
            add => _onPopup += value;
            remove => _onPopup -= value;
        }

        /// <summary>
        ///  Adjusts the other delay values based on the Automatic value.
        /// </summary>
        private void AdjustBaseFromAuto()
        {
            _delayTimes[(int)TTDT.RESHOW] = _delayTimes[(int)TTDT.AUTOMATIC] / ReshowRatio;
            _delayTimes[(int)TTDT.AUTOPOP] = _delayTimes[(int)TTDT.AUTOMATIC] * AutoPopRatio;
            _delayTimes[(int)TTDT.INITIAL] = _delayTimes[(int)TTDT.AUTOMATIC];
        }

        /// <summary>
        ///  ScreenReader announces ToolTip text for an element
        /// </summary>
        private void AnnounceText(Control tool, string text)
        {
            tool?.AccessibilityObject?.RaiseAutomationNotification(
                Automation.AutomationNotificationKind.ActionCompleted,
                Automation.AutomationNotificationProcessing.All,
                ToolTipTitle + " " + text);
        }

        private void HandleCreated(object sender, EventArgs eventargs)
        {
            // Reset the toplevel control when the owner's handle is recreated.
            ClearTopLevelControlEvents();
            _topLevelControl = null;

            Control control = (Control)sender;
            CreateRegion(control);
            CheckNativeToolTip(control);
            CheckCompositeControls(control);

            KeyboardToolTipStateMachine.Instance.Hook(control, this);
        }

        private void CheckNativeToolTip(Control associatedControl)
        {
            // Wait for the Handle Creation.
            if (!GetHandleCreated())
            {
                return;
            }

            if (associatedControl is TreeView treeView && treeView.ShowNodeToolTips)
            {
                treeView.SetToolTip(this, GetToolTip(associatedControl));
            }

            if (associatedControl is TabControl tabControl && tabControl.ShowToolTips)
            {
                tabControl.SetToolTip(this, GetToolTip(associatedControl));
            }

            if (associatedControl is ListView listView)
            {
                listView.SetToolTip(this, GetToolTip(associatedControl));
            }

            // Label now has its own Tooltip for AutoEllipsis.
            // So this control too falls in special casing.
            // We need to disable the LABEL AutoEllipsis tooltip and show
            // this tooltip always.
            if (associatedControl is Label label)
            {
                label.SetToolTip(this);
            }
        }

        private void CheckCompositeControls(Control associatedControl)
        {
            if (associatedControl is UpDownBase upDownBase)
            {
                upDownBase.SetToolTip(this, GetToolTip(associatedControl));
            }
        }

        private void HandleDestroyed(object sender, EventArgs eventargs)
        {
            Control control = (Control)sender;
            DestroyRegion(control);

            KeyboardToolTipStateMachine.Instance.Unhook(control, this);
        }

        /// <summary>
        ///  Fires the Draw event.
        /// </summary>
        private void OnDraw(DrawToolTipEventArgs e) => _onDraw?.Invoke(this, e);

        /// <summary>
        ///  Fires the Popup event.
        /// </summary>
        private void OnPopup(PopupEventArgs e) => _onPopup?.Invoke(this, e);

        private void TopLevelCreated(object sender, EventArgs eventargs)
        {
            CreateHandle();
            CreateAllRegions();
        }

        private void TopLevelDestroyed(object sender, EventArgs eventargs)
        {
            DestoyAllRegions();
            DestroyHandle();
        }

        /// <summary>
        ///  Returns true if the tooltip can offer an extender property to the specified target component.
        /// </summary>
        public bool CanExtend(object target) => target is Control;

        private void ClearTopLevelControlEvents()
        {
            if (_topLevelControl != null)
            {
                _topLevelControl.ParentChanged -= new EventHandler(OnTopLevelPropertyChanged);
                _topLevelControl.HandleCreated -= new EventHandler(TopLevelCreated);
                _topLevelControl.HandleDestroyed -= new EventHandler(TopLevelDestroyed);
            }
        }

        /// <summary>
        ///  Creates the handle for the control.
        /// </summary>
        private void CreateHandle()
        {
            if (GetHandleCreated())
            {
                return;
            }

            IntPtr userCookie = ThemingScope.Activate(Application.UseVisualStyles);
            try
            {
                var icc = new INITCOMMONCONTROLSEX
                {
                    dwICC = ICC.TAB_CLASSES
                };
                InitCommonControlsEx(ref icc);

                CreateParams cp = CreateParams; // Avoid reentrant call to CreateHandle
                if (GetHandleCreated())
                {
                    return;
                }

                _window.CreateHandle(cp);

                if (SystemInformation.HighContrast)
                {
                    UxTheme.SetWindowTheme(Handle, string.Empty, string.Empty);
                }
            }
            finally
            {
                ThemingScope.Deactivate(userCookie);
            }

            // If in OwnerDraw mode, we don't want the default border.
            if (OwnerDraw)
            {
                int style = unchecked((int)((long)User32.GetWindowLong(this, User32.GWL.STYLE)));
                style &= ~(int)User32.WS.BORDER;
                User32.SetWindowLong(this, User32.GWL.STYLE, (IntPtr)style);
            }

            // Setting the max width has the added benefit of enabling multiline tool tips.
            User32.SendMessageW(this, (User32.WM)TTM.SETMAXTIPWIDTH, IntPtr.Zero, (IntPtr)SystemInformation.MaxWindowTrackSize.Width);

            if (_auto)
            {
                SetDelayTime(TTDT.AUTOMATIC, _delayTimes[(int)TTDT.AUTOMATIC]);
                _delayTimes[(int)TTDT.AUTOPOP] = GetDelayTime(TTDT.AUTOPOP);
                _delayTimes[(int)TTDT.INITIAL] = GetDelayTime(TTDT.INITIAL);
                _delayTimes[(int)TTDT.RESHOW] = GetDelayTime(TTDT.RESHOW);
            }
            else
            {
                for (int i = 1; i < _delayTimes.Length; i++)
                {
                    if (_delayTimes[i] >= 1)
                    {
                        SetDelayTime((TTDT)i, _delayTimes[i]);
                    }
                }
            }

            // Set active status
            User32.SendMessageW(this, (User32.WM)TTM.ACTIVATE, PARAM.FromBool(active));

            if (BackColor != SystemColors.Info)
            {
                User32.SendMessageW(this, (User32.WM)TTM.SETTIPBKCOLOR, PARAM.FromColor(BackColor));
            }
            if (ForeColor != SystemColors.InfoText)
            {
                User32.SendMessageW(this, (User32.WM)TTM.SETTIPTEXTCOLOR, PARAM.FromColor(ForeColor));
            }
            if (_toolTipIcon > 0 || !string.IsNullOrEmpty(_toolTipTitle))
            {
                // If the title is null/empty, the icon won't display.
                string title = !string.IsNullOrEmpty(_toolTipTitle) ? _toolTipTitle : " ";
                User32.SendMessageW(this, (User32.WM)TTM.SETTITLEW, (IntPtr)_toolTipIcon, title);
            }
        }

        private void CreateAllRegions()
        {
            Control[] ctls = new Control[_tools.Keys.Count];
            _tools.Keys.CopyTo(ctls, 0);
            for (int i = 0; i < ctls.Length; i++)
            {
                CreateRegion(ctls[i]);
            }
        }

        private void DestoyAllRegions()
        {
            Control[] ctls = new Control[_tools.Keys.Count];
            _tools.Keys.CopyTo(ctls, 0);
            for (int i = 0; i < ctls.Length; i++)
            {
                // DataGridView manages its own tool tip.
                if (ctls[i] is DataGridView)
                {
                    return;
                }

                DestroyRegion(ctls[i]);
            }
        }

        private void SetToolInfo(Control ctl, string caption)
        {
            IntPtr result = GetTOOLINFO(ctl, caption).SendMessage(this, (User32.WM)TTM.ADDTOOLW);

            if ((ctl is TreeView tv && tv.ShowNodeToolTips)
                || (ctl is ListView lv && lv.ShowItemToolTips))
            {
                return;
            }

            if (result == IntPtr.Zero)
            {
                throw new InvalidOperationException(SR.ToolTipAddFailed);
            }
        }

        private void CreateRegion(Control ctl)
        {
            string caption = GetToolTip(ctl);
            bool handlesCreated = ctl.IsHandleCreated
                                  && TopLevelControl != null
                                  && TopLevelControl.IsHandleCreated;
            if (!_created.ContainsKey(ctl) && !string.IsNullOrEmpty(caption)
                && handlesCreated && !DesignMode)
            {
                // Call the SendMessage through a function.
                SetToolInfo(ctl, caption);
                _created[ctl] = ctl;
            }
            if (ctl.IsHandleCreated && _topLevelControl is null)
            {
                // Remove first to purge any duplicates.
                ctl.MouseMove -= new MouseEventHandler(MouseMove);
                ctl.MouseMove += new MouseEventHandler(MouseMove);
            }
        }

        private void MouseMove(object sender, MouseEventArgs me)
        {
            Control ctl = (Control)sender;
            if (!_created.ContainsKey(ctl) && ctl.IsHandleCreated && TopLevelControl != null)
            {
                CreateRegion(ctl);
            }

            if (_created.ContainsKey(ctl))
            {
                ctl.MouseMove -= new MouseEventHandler(MouseMove);
            }
        }

        /// <summary>
        ///  Destroys the handle for this control.
        ///  Required by Label to destroy the handle for the toolTip added for AutoEllipses.
        /// </summary>
        internal void DestroyHandle()
        {
            if (GetHandleCreated())
            {
                _window.DestroyHandle();
            }
        }

        private void DestroyRegion(Control ctl)
        {
            // When the toplevelControl is a form and is Modal, the Handle of the tooltip is released
            // before we come here. In such a case the tool wont get deleted from the tooltip.
            // So we dont check "Handle" in the handlesCreate but check it only foe Non-Nodal dialogs later
            bool handlesCreated = ctl.IsHandleCreated
                                && _topLevelControl != null
                                && _topLevelControl.IsHandleCreated
                                && !_isDisposing;
            if (!(_topLevelControl is Form topForm) || (topForm != null && !topForm.Modal))
            {
                handlesCreated = handlesCreated && GetHandleCreated();
            }

            if (_created.ContainsKey(ctl) && handlesCreated && !DesignMode)
            {
                new ToolInfoWrapper<Control>(ctl).SendMessage(this, (User32.WM)TTM.DELTOOLW);
                _created.Remove(ctl);
            }
        }

        /// <summary>
        ///  Disposes of the <see cref="ToolTip"/> component.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _isDisposing = true;
                try
                {
                    ClearTopLevelControlEvents();
                    StopTimer();

                    // Always destroy the handle.
                    DestroyHandle();
                    RemoveAll();

                    _window = null;

                    // Unhook the DeactiveEvent. Find the Form for associated Control and hook
                    // up to the Deactivated event to Hide the Shown tooltip
                    if (TopLevelControl is Form baseFrom)
                    {
                        baseFrom.Deactivate -= new EventHandler(BaseFormDeactivate);
                    }
                }
                finally
                {
                    _isDisposing = false;
                }
            }

            base.Dispose(disposing);
        }

        /// <summary>
        ///  Returns the delayTime based on the NativeMethods.TTDT_* values.
        /// </summary>
        internal int GetDelayTime(TTDT type)
        {
            if (!GetHandleCreated())
            {
                return _delayTimes[(int)type];
            }

            return (int)(long)User32.SendMessageW(this, (User32.WM)TTM.GETDELAYTIME, (IntPtr)type);
        }

        internal bool GetHandleCreated() => _window != null && _window.Handle != IntPtr.Zero;

        /// <summary>
        ///  Returns a detailed TOOLINFO_TOOLTIP structure that represents the specified region.
        /// </summary>
        private unsafe ToolInfoWrapper<Control> GetTOOLINFO(Control control, string caption)
        {
            TTF flags = TTF.TRANSPARENT | TTF.SUBCLASS;

            // RightToLeft reading order
            if (TopLevelControl?.RightToLeft == RightToLeft.Yes && !control.IsMirrored)
            {
                // Indicates that the ToolTip text will be displayed in the opposite direction
                // to the text in the parent window.
                flags |= TTF.RTLREADING;
            }

            bool noText = (control is TreeView tv && tv.ShowNodeToolTips)
                || (control is ListView lv && lv.ShowItemToolTips);

            var info = new ToolInfoWrapper<Control>(control, flags, noText ? null : caption);
            if (noText)
                info.Info.lpszText = (char*)(-1);

            return info;
        }

        private ToolInfoWrapper<IWin32WindowAdapter> GetWinTOOLINFO(IWin32Window hWnd)
        {
            TTF flags = TTF.TRANSPARENT | TTF.SUBCLASS;

            // RightToLeft reading order
            if (TopLevelControl?.RightToLeft == RightToLeft.Yes)
            {
                bool isWindowMirrored = ((unchecked((int)(long)User32.GetWindowLong(
                    new HandleRef(this, Control.GetSafeHandle(hWnd)), User32.GWL.STYLE)) & (int)User32.WS_EX.LAYOUTRTL) == (int)User32.WS_EX.LAYOUTRTL);

                // Indicates that the ToolTip text will be displayed in the opposite direction
                // to the text in the parent window.
                if (!isWindowMirrored)
                {
                    flags |= TTF.RTLREADING;
                }
            }

            return new ToolInfoWrapper<IWin32WindowAdapter>(new IWin32WindowAdapter(hWnd), flags);
        }

        /// <summary>
        ///  Retrieves the <see cref="ToolTip"/> text associated with the specified control.
        /// </summary>
        [DefaultValue("")]
        [Localizable(true)]
        [SRDescription(nameof(SR.ToolTipToolTipDescr))]
        [Editor("System.ComponentModel.Design.MultilineStringEditor, " + AssemblyRef.SystemDesign, typeof(Drawing.Design.UITypeEditor))]
        public string GetToolTip(Control control)
        {
            if (control is null)
            {
                return string.Empty;
            }

            TipInfo tt = (TipInfo)_tools[control];
            return tt?.Caption ?? string.Empty;
        }

        /// <summary>
        ///  Returns the HWND of the window that is at the specified point. This handles special
        ///  cases where one Control owns multiple HWNDs (i.e. ComboBox).
        /// </summary>
        private IntPtr GetWindowFromPoint(Point screenCoords, ref bool success)
        {
            Control baseVar = TopLevelControl;

            // Special case ActiveX Controls.
            if (baseVar != null && baseVar.IsActiveX)
            {
                // Find the matching HWnd matching the ScreenCoord and find if the Control has a Tooltip.
                IntPtr hwndControl = User32.WindowFromPoint(screenCoords);
                if (hwndControl != IntPtr.Zero)
                {
                    Control currentControl = Control.FromHandle(hwndControl);
                    if (currentControl != null && _tools != null && _tools.ContainsKey(currentControl))
                    {
                        return hwndControl;
                    }
                }
                return IntPtr.Zero;
            }

            IntPtr baseHwnd = baseVar?.Handle ?? IntPtr.Zero;
            IntPtr hwnd = IntPtr.Zero;
            bool finalMatch = false;
            while (!finalMatch)
            {
                Point pt = screenCoords;
                if (baseVar != null)
                {
                    pt = baseVar.PointToClient(screenCoords);
                }

                IntPtr found = User32.ChildWindowFromPointEx(baseHwnd, pt, User32.CWP.SKIPINVISIBLE);
                if (found == baseHwnd)
                {
                    hwnd = found;
                    finalMatch = true;
                }
                else if (found == IntPtr.Zero)
                {
                    finalMatch = true;
                }
                else
                {
                    baseVar = Control.FromHandle(found);
                    if (baseVar is null)
                    {
                        baseVar = Control.FromChildHandle(found);
                        if (baseVar != null)
                        {
                            hwnd = baseVar.Handle;
                        }

                        finalMatch = true;
                    }
                    else
                    {
                        baseHwnd = baseVar.Handle;
                    }
                }
            }

            if (hwnd != IntPtr.Zero)
            {
                Control ctl = Control.FromHandle(hwnd);
                if (ctl != null)
                {
                    Control current = ctl;
                    while (current != null && current.Visible)
                    {
                        current = current.ParentInternal;
                    }
                    if (current != null)
                    {
                        hwnd = IntPtr.Zero;
                    }

                    success = true;
                }
            }

            return hwnd;
        }

        private void OnTopLevelPropertyChanged(object s, EventArgs e)
        {
            ClearTopLevelControlEvents();
            _topLevelControl = null;

            // We must re-acquire this control.  If the existing top level control's handle
            // was never created, but the new parent has a handle, if we don't re-get
            // the top level control here we won't ever create the tooltip handle.
            _topLevelControl = TopLevelControl;
        }

        private void RecreateHandle()
        {
            if (!DesignMode)
            {
                if (GetHandleCreated())
                {
                    DestroyHandle();
                }

                _created.Clear();
                CreateHandle();
                CreateAllRegions();
            }
        }

        /// <summary>
        ///  Removes all of the tooltips currently associated with the <see cref="ToolTip"/> control.
        /// </summary>
        public void RemoveAll()
        {
            Control[] regions = new Control[_tools.Keys.Count];
            _tools.Keys.CopyTo(regions, 0);
            for (int i = 0; i < regions.Length; i++)
            {
                if (regions[i].IsHandleCreated)
                {
                    DestroyRegion(regions[i]);
                }

                regions[i].HandleCreated -= new EventHandler(HandleCreated);
                regions[i].HandleDestroyed -= new EventHandler(HandleDestroyed);

                KeyboardToolTipStateMachine.Instance.Unhook(regions[i], this);
            }

            _created.Clear();
            _tools.Clear();

            ClearTopLevelControlEvents();
            _topLevelControl = null;

            KeyboardToolTipStateMachine.Instance.ResetStateMachine(this);
        }

        /// <summary>
        ///  Sets the delayTime based on the NativeMethods.TTDT_* values.
        /// </summary>
        private void SetDelayTime(TTDT type, int time)
        {
            _auto = type == TTDT.AUTOMATIC;
            _delayTimes[(int)type] = time;

            if (GetHandleCreated() && time >= 0)
            {
                User32.SendMessageW(this, (User32.WM)TTM.SETDELAYTIME, (IntPtr)type, (IntPtr)time);

                // Update everyone else if automatic is set. we need to do this
                // to preserve value in case of handle recreation.
                if (_auto)
                {
                    _delayTimes[(int)TTDT.AUTOPOP] = GetDelayTime(TTDT.AUTOPOP);
                    _delayTimes[(int)TTDT.INITIAL] = GetDelayTime(TTDT.INITIAL);
                    _delayTimes[(int)TTDT.RESHOW] = GetDelayTime(TTDT.RESHOW);
                }
            }
            else if (_auto)
            {
                AdjustBaseFromAuto();
            }
        }

        /// <summary>
        ///  Associates <see cref="ToolTip"/> text with the specified control.
        /// </summary>
        public void SetToolTip(Control control, string caption)
        {
            TipInfo info = new TipInfo(caption, TipInfo.Type.Auto);
            SetToolTipInternal(control, info);
        }

        /// <summary>
        ///  Associates <see cref="ToolTip"/> text with the specified information
        /// </summary>
        private void SetToolTipInternal(Control control, TipInfo info)
        {
            if (control is null)
            {
                throw new ArgumentNullException(nameof(control));
            }

            bool exists = _tools.ContainsKey(control);
            bool empty = info is null || string.IsNullOrEmpty(info.Caption);
            if (exists && empty)
            {
                _tools.Remove(control);
            }
            else if (!empty)
            {
                _tools[control] = info;
            }

            if (!empty && !exists)
            {
                control.HandleCreated += new EventHandler(HandleCreated);
                control.HandleDestroyed += new EventHandler(HandleDestroyed);

                if (control.IsHandleCreated)
                {
                    HandleCreated(control, EventArgs.Empty);
                }
            }
            else
            {
                bool handlesCreated = control.IsHandleCreated
                                      && TopLevelControl != null
                                      && TopLevelControl.IsHandleCreated;

                if (exists && !empty && handlesCreated && !DesignMode)
                {
                    ToolInfoWrapper<Control> toolInfo = GetTOOLINFO(control, info.Caption);
                    toolInfo.SendMessage(this, (User32.WM)TTM.SETTOOLINFOW);
                    CheckNativeToolTip(control);
                    CheckCompositeControls(control);
                }
                else if (empty && exists && !DesignMode)
                {
                    control.HandleCreated -= new EventHandler(HandleCreated);
                    control.HandleDestroyed -= new EventHandler(HandleDestroyed);

                    if (control.IsHandleCreated)
                    {
                        HandleDestroyed(control, EventArgs.Empty);
                    }

                    _created.Remove(control);
                }
            }
        }

        /// <summary>
        ///  Returns true if the AutomaticDelay property should be persisted.
        /// </summary>
        private bool ShouldSerializeAutomaticDelay() => _auto && AutomaticDelay != DefaultDelay;

        /// <summary>
        ///  Returns true if the AutoPopDelay property should be persisted.
        /// </summary>
        private bool ShouldSerializeAutoPopDelay() => !_auto;

        /// <summary>
        ///  Returns true if the InitialDelay property should be persisted.
        /// </summary>
        private bool ShouldSerializeInitialDelay() => !_auto;

        /// <summary>
        ///  Returns true if the ReshowDelay property should be persisted.
        /// </summary>
        private bool ShouldSerializeReshowDelay() => !_auto;

        /// <summary>
        ///  Shows a tooltip for specified text, window, and hotspot
        /// </summary>
        private void ShowTooltip(string text, IWin32Window window, int duration)
        {
            if (window is null)
            {
                throw new ArgumentNullException(nameof(window));
            }

            if (window is Control associatedControl)
            {
                var r = new RECT();
                User32.GetWindowRect(associatedControl, ref r);

                Cursor currentCursor = Cursor.Current;
                Point cursorLocation = Cursor.Position;
                Point p = cursorLocation;

                Screen screen = Screen.FromPoint(cursorLocation);

                // Place the tool tip on the associated control if its not already there
                if (cursorLocation.X < r.left || cursorLocation.X > r.right ||
                     cursorLocation.Y < r.top || cursorLocation.Y > r.bottom)
                {
                    // Calculate the dimensions of the visible rectangle which
                    // is used to estimate the upper x,y of the tooltip placement
                    RECT visibleRect = new RECT
                    {
                        left = (r.left < screen.WorkingArea.Left) ? screen.WorkingArea.Left : r.left,
                        top = (r.top < screen.WorkingArea.Top) ? screen.WorkingArea.Top : r.top,
                        right = (r.right > screen.WorkingArea.Right) ? screen.WorkingArea.Right : r.right,
                        bottom = (r.bottom > screen.WorkingArea.Bottom) ? screen.WorkingArea.Bottom : r.bottom
                    };

                    p.X = visibleRect.left + (visibleRect.right - visibleRect.left) / 2;
                    p.Y = visibleRect.top + (visibleRect.bottom - visibleRect.top) / 2;
                    associatedControl.PointToClient(p);
                    SetTrackPosition(p.X, p.Y);
                    SetTool(window, text, TipInfo.Type.SemiAbsolute, p);

                    if (duration > 0)
                    {
                        StartTimer(this._window, duration);
                    }
                }
                else
                {
                    TipInfo tt = (TipInfo)_tools[associatedControl];
                    if (tt is null)
                    {
                        tt = new TipInfo(text, TipInfo.Type.SemiAbsolute);
                    }
                    else
                    {
                        tt.TipType |= TipInfo.Type.SemiAbsolute;
                        tt.Caption = text;
                    }

                    tt.Position = p;
                    if (duration > 0)
                    {
                        if (_originalPopupDelay == 0)
                        {
                            _originalPopupDelay = AutoPopDelay;
                        }

                        AutoPopDelay = duration;
                    }

                    SetToolTipInternal(associatedControl, tt);
                }
            }
        }

        /// <summary>
        ///  Associates <see cref="ToolTip"/> with the specified control and displays it.
        /// </summary>
        public void Show(string text, IWin32Window window)
        {
            // Check if the foreground window is the TopLevelWindow
            if (IsWindowActive(window))
            {
                ShowTooltip(text, window, 0);
            }
        }

        /// <summary>
        ///  Associates <see cref="ToolTip"/> with the specified control and displays it for the
        ///  specified duration.
        /// </summary>
        public void Show(string text, IWin32Window window, int duration)
        {
            if (window is null)
            {
                throw new ArgumentNullException(nameof(window));
            }
            if (duration < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(duration), duration, string.Format(SR.InvalidLowBoundArgumentEx, nameof(duration), duration, 0));
            }

            if (IsWindowActive(window))
            {
                ShowTooltip(text, window, duration);
            }
        }

        /// <summary>
        ///  Associates <see cref="ToolTip"/> with the specified control and displays it.
        /// </summary>
        public void Show(string text, IWin32Window window, Point point)
        {
            if (window is null)
            {
                throw new ArgumentNullException(nameof(window));
            }

            if (IsWindowActive(window))
            {
                // Set the ToolTips.
                var r = new RECT();
                User32.GetWindowRect(new HandleRef(window, Control.GetSafeHandle(window)), ref r);
                int pointX = r.left + point.X;
                int pointY = r.top + point.Y;

                SetTrackPosition(pointX, pointY);
                SetTool(window, text, TipInfo.Type.Absolute, new Point(pointX, pointY));
            }
        }

        /// <summary>
        ///  Associates <see cref="ToolTip"/> with the specified control and displays it.
        /// </summary>
        public void Show(string text, IWin32Window window, Point point, int duration)
        {
            if (window is null)
            {
                throw new ArgumentNullException(nameof(window));
            }
            if (duration < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(duration), duration, string.Format(SR.InvalidLowBoundArgumentEx, nameof(duration), duration, 0));
            }

            if (IsWindowActive(window))
            {
                // Set the ToolTips.
                var r = new RECT();
                User32.GetWindowRect(new HandleRef(window, Control.GetSafeHandle(window)), ref r);
                int pointX = r.left + point.X;
                int pointY = r.top + point.Y;
                SetTrackPosition(pointX, pointY);
                SetTool(window, text, TipInfo.Type.Absolute, new Point(pointX, pointY));
                StartTimer(window, duration);
            }
        }

        /// <summary>
        ///  Associates <see cref="ToolTip"/> with the specified control and displays it.
        /// </summary>
        public void Show(string text, IWin32Window window, int x, int y)
        {
            if (window is null)
            {
                throw new ArgumentNullException(nameof(window));
            }

            if (IsWindowActive(window))
            {
                var r = new RECT();
                User32.GetWindowRect(new HandleRef(window, Control.GetSafeHandle(window)), ref r);
                int pointX = r.left + x;
                int pointY = r.top + y;
                SetTrackPosition(pointX, pointY);
                SetTool(window, text, TipInfo.Type.Absolute, new Point(pointX, pointY));
            }
        }

        /// <summary>
        ///  Associates <see cref="ToolTip"/> with the specified control and displays it.
        /// </summary>
        public void Show(string text, IWin32Window window, int x, int y, int duration)
        {
            if (window is null)
            {
                throw new ArgumentNullException(nameof(window));
            }
            if (duration < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(duration), duration, string.Format(SR.InvalidLowBoundArgumentEx, nameof(duration), duration, 0));
            }

            if (IsWindowActive(window))
            {
                var r = new RECT();
                User32.GetWindowRect(new HandleRef(window, Control.GetSafeHandle(window)), ref r);
                int pointX = r.left + x;
                int pointY = r.top + y;
                SetTrackPosition(pointX, pointY);
                SetTool(window, text, TipInfo.Type.Absolute, new Point(pointX, pointY));
                StartTimer(window, duration);
            }
        }

        internal void ShowKeyboardToolTip(string text, IKeyboardToolTip tool, int duration)
        {
            if (tool is null)
            {
                throw new ArgumentNullException(nameof(tool));
            }
            if (duration < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(duration), string.Format(SR.InvalidLowBoundArgumentEx, nameof(duration), (duration).ToString(CultureInfo.CurrentCulture), 0));
            }

            Rectangle toolRectangle = tool.GetNativeScreenRectangle();
            // At first, place the tooltip at the middle of the tool (default location)
            int pointX = (toolRectangle.Left + toolRectangle.Right) / 2;
            int pointY = (toolRectangle.Top + toolRectangle.Bottom) / 2;
            SetTool(tool.GetOwnerWindow(), text, TipInfo.Type.Absolute, new Point(pointX, pointY));

            // Then look for a better ToolTip location
            if (TryGetBubbleSize(tool, toolRectangle, out Size bubbleSize))
            {
                Point optimalPoint = GetOptimalToolTipPosition(tool, toolRectangle, bubbleSize.Width, bubbleSize.Height);

                // The optimal point should be used as a tracking position
                pointX = optimalPoint.X;
                pointY = optimalPoint.Y;

                // Update TipInfo for the tool with optimal position
                TipInfo tipInfo = (_tools[tool] ?? _tools[tool.GetOwnerWindow()]) as TipInfo;
                if (tipInfo != null)
                {
                    tipInfo.Position = new Point(pointX, pointY);
                }

                // Ensure that the tooltip bubble is moved to the optimal position even when a mouse tooltip is being replaced with a keyboard tooltip
                Reposition(optimalPoint, bubbleSize);
            }

            SetTrackPosition(pointX, pointY);
            IsActivatedByKeyboard = true;
            StartTimer(tool.GetOwnerWindow(), duration);
        }

        private bool TryGetBubbleSize(IKeyboardToolTip tool, Rectangle toolRectangle, out Size bubbleSize)
        {
            // Get bubble size to use it for optimal position calculation. Requesting the bubble
            // size will AV if there isn't a current tool window.

            IntPtr result = GetCurrentToolHwnd();
            if (result != IntPtr.Zero)
            {
                var info = new ToolInfoWrapper<IWin32WindowAdapter>(new IWin32WindowAdapter(tool.GetOwnerWindow()));
                result = info.SendMessage(this, (User32.WM)TTM.GETBUBBLESIZE);
            }

            if (result == IntPtr.Zero)
            {
                bubbleSize = Size.Empty;
                return false;
            }

            int width = PARAM.LOWORD(result);
            int height = PARAM.HIWORD(result);
            bubbleSize = new Size(width, height);
            return true;
        }

        private Point GetOptimalToolTipPosition(IKeyboardToolTip tool, Rectangle toolRectangle, int width, int height)
        {
            // Possible tooltip locations are tied to the tool rectangle bounds
            int centeredX = toolRectangle.Left + toolRectangle.Width / 2 - width / 2; // tooltip will be aligned with tool vertically
            int centeredY = toolRectangle.Top + toolRectangle.Height / 2 - height / 2; // tooltip will be aligned with tool horizontally

            Rectangle[] possibleLocations = new Rectangle[LocationTotal];
            possibleLocations[LocationIndexTop] = new Rectangle(centeredX, toolRectangle.Top - height, width, height);
            possibleLocations[LocationIndexRight] = new Rectangle(toolRectangle.Right, centeredY, width, height);
            possibleLocations[LocationIndexBottom] = new Rectangle(centeredX, toolRectangle.Bottom, width, height);
            possibleLocations[LocationIndexLeft] = new Rectangle(toolRectangle.Left - width, centeredY, width, height);

            // Neighboring tools should not be overlapped (ideally) by tooltip
            IList<Rectangle> neighboringToolsRectangles = tool.GetNeighboringToolsRectangles();

            // Weights are used to determine which one of the possible location overlaps least area of the neighboring tools
            long[] locationWeights = new long[LocationTotal];

            // Check if the possible locations intersect with the neighboring tools
            for (int i = 0; i < possibleLocations.Length; i++)
            {
                foreach (Rectangle neighboringToolRectangle in neighboringToolsRectangles)
                {
                    Rectangle intersection = Rectangle.Intersect(possibleLocations[i], neighboringToolRectangle);
                    checked
                    {
                        locationWeights[i] += Math.Abs((long)intersection.Width * intersection.Height); // Intersection is a weight
                    }
                }
            }

            // Calculate clipped area of possible locations i.e. area which is located outside the screen area
            Rectangle screenBounds = SystemInformation.VirtualScreen;
            long[] locationClippedAreas = new long[LocationTotal];
            for (int i = 0; i < possibleLocations.Length; i++)
            {
                Rectangle locationAreaWithinScreen = Rectangle.Intersect(screenBounds, possibleLocations[i]);
                checked
                {
                    locationClippedAreas[i] = (Math.Abs((long)possibleLocations[i].Width) - Math.Abs((long)locationAreaWithinScreen.Width))
                        * (Math.Abs((long)possibleLocations[i].Height) - Math.Abs((long)locationAreaWithinScreen.Height));
                }
            }

            // Calculate area of possible locations within top level control rectangle
            long[] locationWithinTopControlAreas = new long[LocationTotal];
            Rectangle topContainerBounds = ((IKeyboardToolTip)TopLevelControl)?.GetNativeScreenRectangle() ?? Rectangle.Empty;
            if (!topContainerBounds.IsEmpty)
            {
                for (int i = 0; i < possibleLocations.Length; i++)
                {
                    Rectangle locationWithinTopControlRectangle = Rectangle.Intersect(topContainerBounds, possibleLocations[i]);
                    checked
                    {
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
            for (int i = 1; i < possibleLocations.Length; i++)
            {
                if (IsCompetingLocationBetter(leastClippedArea, leastWeight, biggestAreaWithinTopControl, locationIndex,
                    locationClippedAreas[i], locationWeights[i], locationWithinTopControlAreas[i], i,
                    rtlEnabled))
                {
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
            bool rtlEnabled)
        {
            if (competingLocationClippedArea < originalLocationClippedArea)
            {
                // Prefer location with less clipped area
                return true;
            }
            else if (competingLocationWeight < originalLocationWeight)
            {
                // Otherwise prefer location with less weight
                return true;
            }
            else if (competingLocationWeight == originalLocationWeight && competingLocationClippedArea == originalLocationClippedArea)
            {
                // Prefer locations located within top level control
                if (competingLocationAreaWithinTopControl > originalLocationAreaWithinTopControl)
                {
                    return true;
                }
                else if (competingLocationAreaWithinTopControl == originalLocationAreaWithinTopControl)
                {
                    switch (originalIndex)
                    {
                        case LocationIndexTop:
                            // Top location is the least preferred location
                            return true;
                        case LocationIndexBottom:
                            // Right and Left locations are preferred instead of Bottom location
                            if (competingIndex == LocationIndexLeft || competingIndex == LocationIndexRight)
                            {
                                return true;
                            }
                            break;
                        case LocationIndexRight:
                            // When RTL is enabled Left location is preferred
                            if (rtlEnabled && competingIndex == LocationIndexLeft)
                            {
                                return true;
                            }
                            break;
                        case LocationIndexLeft:
                            // When RTL is disabled Right location is preferred
                            if (!rtlEnabled && competingIndex == LocationIndexRight)
                            {
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

        /// <summary>
        ///  Private Function to encapsulate TTM_TRACKPOSITION so that this doesnt fire an extra POP event
        /// </summary>
        private void SetTrackPosition(int pointX, int pointY)
        {
            try
            {
                _trackPosition = true;
                User32.SendMessageW(this, (User32.WM)TTM.TRACKPOSITION, IntPtr.Zero, PARAM.FromLowHigh(pointX, pointY));
            }
            finally
            {
                _trackPosition = false;
            }
        }

        /// <summary>
        ///  Hides <see cref="ToolTip"/> with the specified control.
        /// </summary>
        public void Hide(IWin32Window win)
        {
            if (win is null)
            {
                throw new ArgumentNullException(nameof(win));
            }

            if (_window is null)
            {
                return;
            }

            if (GetHandleCreated())
            {
                var info = new ToolInfoWrapper<IWin32WindowAdapter>(new IWin32WindowAdapter(win));
                info.SendMessage(this, (User32.WM)TTM.TRACKACTIVATE);
                info.SendMessage(this, (User32.WM)TTM.DELTOOLW);
            }
            StopTimer();

            // Check if the passed in IWin32Window is a Control.
            if (!(win is Control tool))
            {
                _owners.Remove(win.Handle);
            }
            else
            {
                if (_tools.ContainsKey(tool))
                {
                    SetToolInfo(tool, GetToolTip(tool));
                }
                else
                {
                    _owners.Remove(win.Handle);
                }

                // Find the Form for associated Control and hook up to the Deactivated event
                // to hide the shown tooltip
                Form baseFrom = tool.FindForm();
                if (baseFrom != null)
                {
                    baseFrom.Deactivate -= new EventHandler(BaseFormDeactivate);
                }
            }

            // Clear off the toplevel control.
            ClearTopLevelControlEvents();
            _topLevelControl = null;
            IsActivatedByKeyboard = false;
        }

        private void BaseFormDeactivate(object sender, EventArgs e)
        {
            HideAllToolTips();
            KeyboardToolTipStateMachine.Instance.NotifyAboutFormDeactivation(this);
        }

        private void HideAllToolTips()
        {
            Control[] ctls = new Control[_owners.Values.Count];
            _owners.Values.CopyTo(ctls, 0);
            for (int i = 0; i < ctls.Length; i++)
            {
                Hide(ctls[i]);
            }
        }

        private void SetTool(IWin32Window win, string text, TipInfo.Type type, Point position)
        {
            Control tool = win as Control;
            if (tool != null && _tools.ContainsKey(tool))
            {
                var toolInfo = new ToolInfoWrapper<Control>(tool);
                if (toolInfo.SendMessage(this, (User32.WM)TTM.GETTOOLINFOW) != IntPtr.Zero)
                {
                    TTF flags = TTF.TRACK;
                    if (type == TipInfo.Type.Absolute || type == TipInfo.Type.SemiAbsolute)
                    {
                        flags |= TTF.ABSOLUTE;
                    }
                    toolInfo.Info.uFlags |= flags;
                    toolInfo.Text = text;
                }

                TipInfo tt = (TipInfo)_tools[tool];
                if (tt is null)
                {
                    tt = new TipInfo(text, type);
                }
                else
                {
                    tt.TipType |= type;
                    tt.Caption = text;
                }
                tt.Position = position;
                _tools[tool] = tt;

                IntPtr result = toolInfo.SendMessage(this, (User32.WM)TTM.SETTOOLINFOW);
                result = toolInfo.SendMessage(this, (User32.WM)TTM.TRACKACTIVATE, BOOL.TRUE);
            }
            else
            {
                Hide(win);

                // Need to do this BEFORE we call GetWinTOOLINFO, since it relies on the tools array to be populated
                // in order to find the toplevelparent.
                TipInfo tt = (TipInfo)_tools[tool];
                if (tt is null)
                {
                    tt = new TipInfo(text, type);
                }
                else
                {
                    tt.TipType |= type;
                    tt.Caption = text;
                }

                tt.Position = position;
                _tools[tool] = tt;

                IntPtr hWnd = Control.GetSafeHandle(win);
                _owners[hWnd] = win;

                var toolInfo = GetWinTOOLINFO(win);
                toolInfo.Info.uFlags |= TTF.TRACK;

                if (type == TipInfo.Type.Absolute || type == TipInfo.Type.SemiAbsolute)
                {
                    toolInfo.Info.uFlags |= TTF.ABSOLUTE;
                }

                toolInfo.Text = text;
                IntPtr result = toolInfo.SendMessage(this, (User32.WM)TTM.ADDTOOLW);
                result = toolInfo.SendMessage(this, (User32.WM)TTM.TRACKACTIVATE, BOOL.TRUE);
            }

            if (tool != null)
            {
                // Lets find the Form for associated Control .
                // and hook up to the Deactivated event to Hide the Shown tooltip
                Form baseFrom = tool.FindForm();
                if (baseFrom != null)
                {
                    baseFrom.Deactivate += new EventHandler(BaseFormDeactivate);
                }
            }
        }

        /// <summary>
        ///  Starts the timer hiding Positioned ToolTips
        /// </summary>
        private void StartTimer(IWin32Window owner, int interval)
        {
            if (_timer is null)
            {
                _timer = new ToolTipTimer(owner);
                // Add the timer handler
                _timer.Tick += new EventHandler(TimerHandler);
            }

            _timer.Interval = interval;
            _timer.Start();
        }

        /// <summary>
        ///  Stops the timer for hiding Positioned ToolTips
        /// </summary>
        protected void StopTimer()
        {
            // Hold a local ref to timer so that a posted message doesn't null this out during
            // disposal.
            ToolTipTimer timerRef = _timer;

            if (timerRef != null)
            {
                timerRef.Stop();
                timerRef.Dispose();
                _timer = null;
            }
        }

        /// <summary>
        ///  Generates updown events when the timer calls this function.
        /// </summary>
        private void TimerHandler(object source, EventArgs args)
        {
            Hide(((ToolTipTimer)source).Host);
        }

        /// <summary>
        ///  Finalizes garbage collection.
        /// </summary>
        ~ToolTip() => DestroyHandle();

        /// <summary>
        ///  Returns a string representation for this control.
        /// </summary>
        public override string ToString()
        {
            string s = base.ToString();
            return $"{s} InitialDelay: {InitialDelay}, ShowAlways: {ShowAlways}";
        }

        private void Reposition(Point tipPosition, Size tipSize)
        {
            Point moveToLocation = tipPosition;
            Screen screen = Screen.FromPoint(moveToLocation);

            // Re-adjust the X position of the tool tip if it bleeds off the screen working area
            if (moveToLocation.X + tipSize.Width > screen.WorkingArea.Right)
            {
                moveToLocation.X = screen.WorkingArea.Right - tipSize.Width;
            }

            // re-adjust the Y position of the tool tip if it bleeds off the screen working area.
            if (moveToLocation.Y + tipSize.Height > screen.WorkingArea.Bottom)
            {
                moveToLocation.Y = screen.WorkingArea.Bottom - tipSize.Height;
            }

            User32.SetWindowPos(
                new HandleRef(this, Handle),
                User32.HWND_TOPMOST,
                moveToLocation.X,
                moveToLocation.Y,
                tipSize.Width,
                tipSize.Height,
                User32.SWP.NOACTIVATE | User32.SWP.NOSIZE | User32.SWP.NOOWNERZORDER);
        }

        private IntPtr GetCurrentToolHwnd()
        {
            var toolInfo = new ToolInfoWrapper<Control>();
            if (toolInfo.SendMessage(this, (User32.WM)TTM.GETCURRENTTOOLW) != IntPtr.Zero)
            {
                return toolInfo.Info.hwnd;
            }
            return IntPtr.Zero;
        }

        private IWin32Window GetCurrentToolWindow()
        {
            IntPtr hwnd = GetCurrentToolHwnd();
            return (IWin32Window)_owners[hwnd] ?? Control.FromHandle(hwnd);
        }

        /// <summary>
        ///  Handles the WM_MOVE message.
        /// </summary>
        private void WmMove()
        {
            IWin32Window window = GetCurrentToolWindow();
            if (window is null)
                return;

            TipInfo tt = (TipInfo)_tools[window];
            if (window is null || tt is null)
            {
                return;
            }

            // Treeview handles its own ToolTips.
            if (window is TreeView treeView)
            {
                if (treeView.ShowNodeToolTips)
                {
                    return;
                }
            }

            // Reposition the tooltip when its about to be shown since the tooltip can go out of screen
            // working area bounds Reposition would check the bounds for us.
            var r = new RECT();
            User32.GetWindowRect(this, ref r);
            if (tt.Position != Point.Empty)
            {
                Reposition(tt.Position, r.Size);
            }
        }

        /// <summary>
        ///  Handles the WM_MOUSEACTIVATE message.
        /// </summary>
        private void WmMouseActivate(ref Message msg)
        {
            IWin32Window window = GetCurrentToolWindow();
            if (window is null)
                return;

            var r = new RECT();
            User32.GetWindowRect(new HandleRef(window, Control.GetSafeHandle(window)), ref r);
            Point cursorLocation = Cursor.Position;

            // Do not activate the mouse if its within the bounds of the
            // the associated tool
            if (cursorLocation.X >= r.left && cursorLocation.X <= r.right &&
                cursorLocation.Y >= r.top && cursorLocation.Y <= r.bottom)
            {
                msg.Result = (IntPtr)User32.MA.NOACTIVATE;
            }
        }

        /// <summary>
        ///  Handles the WM_WINDOWFROMPOINT message.
        /// </summary>
        private void WmWindowFromPoint(ref Message msg)
        {
            var sc = (Point)msg.GetLParam(typeof(Point));
            bool result = false;
            msg.Result = GetWindowFromPoint(sc, ref result);
        }

        /// <summary>
        ///  Handles the TTN_SHOW message.
        /// </summary>
        private void WmShow()
        {
            IWin32Window window = GetCurrentToolWindow();
            if (window is null)
                return;

            // Get the bounds.
            var r = new RECT();
            User32.GetWindowRect(this, ref r);

            Control toolControl = window as Control;

            Size currentTooltipSize = r.Size;
            PopupEventArgs e = new PopupEventArgs(window, toolControl, IsBalloon, currentTooltipSize);
            OnPopup(e);

            if (toolControl is DataGridView dataGridView && dataGridView.CancelToolTipPopup(this))
            {
                // The dataGridView cancelled the tooltip.
                e.Cancel = true;
            }

            if (!e.Cancel)
            {
                AnnounceText(toolControl, GetCaptionForTool(toolControl));
            }

            // We need to re-get the rectangle of the tooltip here because
            // any of the tooltip attributes/properties could have been updated
            // during the popup event; in which case the size of the tooltip is
            // affected. e.ToolTipSize is respected over r.Size
            User32.GetWindowRect(this, ref r);
            currentTooltipSize = (e.ToolTipSize == currentTooltipSize) ? r.Size : e.ToolTipSize;

            if (IsBalloon)
            {
                // Get the text display rectangle
                User32.SendMessageW(this, (User32.WM)TTM.ADJUSTRECT, PARAM.FromBool(true), ref r);
                if (r.Size.Height > currentTooltipSize.Height)
                {
                    currentTooltipSize.Height = r.Size.Height;
                }
            }

            // Set the max possible size of the tooltip to the size we received.
            // This prevents the operating system from drawing incorrect rectangles
            // when determing the correct display rectangle
            // Set the MaxWidth only if user has changed the width.
            if (currentTooltipSize != r.Size)
            {
                Screen screen = Screen.FromPoint(Cursor.Position);
                int maxwidth = (IsBalloon)
                    ? Math.Min(currentTooltipSize.Width - 2 * BalloonOffsetX, screen.WorkingArea.Width)
                    : Math.Min(currentTooltipSize.Width, screen.WorkingArea.Width);
                User32.SendMessageW(this, (User32.WM)TTM.SETMAXTIPWIDTH, IntPtr.Zero, (IntPtr)maxwidth);
            }

            if (e.Cancel)
            {
                _cancelled = true;
                User32.SetWindowPos(
                    new HandleRef(this, Handle),
                    User32.HWND_TOPMOST,
                    flags: User32.SWP.NOACTIVATE | User32.SWP.NOOWNERZORDER);
            }
            else
            {
                _cancelled = false;

                // Only width/height changes are respected, so set top,left to what we got earlier
                User32.SetWindowPos(
                    new HandleRef(this, Handle),
                    User32.HWND_TOPMOST,
                    r.left,
                    r.top,
                    currentTooltipSize.Width,
                    currentTooltipSize.Height,
                    User32.SWP.NOACTIVATE | User32.SWP.NOOWNERZORDER);
            }
        }

        /// <summary>
        ///  Handles the WM_WINDOWPOSCHANGED message.
        ///  We need to Hide the window since the native tooltip actually calls SetWindowPos in its TTN_SHOW even if we cancel showing the
        ///  tooltip. Hence we need to listen to the WindowPosChanged message can hide the window ourselves.
        /// </summary>
        private bool WmWindowPosChanged()
        {
            if (_cancelled)
            {
                User32.ShowWindow(this, User32.SW.HIDE);
                return true;
            }

            return false;
        }

        /// <summary>
        ///  Handles the WM_WINDOWPOSCHANGING message.
        /// </summary>
        private unsafe void WmWindowPosChanging(ref Message m)
        {
            if (_cancelled || _isDisposing)
            {
                return;
            }

            User32.WINDOWPOS* wp = (User32.WINDOWPOS*)m.LParam;

            Cursor currentCursor = Cursor.Current;
            Point cursorPos = Cursor.Position;

            IWin32Window window = GetCurrentToolWindow();
            if (window != null)
            {
                TipInfo tt = null;
                if (window != null)
                {
                    tt = (TipInfo)_tools[window];
                    if (tt is null)
                    {
                        return;
                    }

                    // Treeview handles its own ToolTips.
                    if (window is TreeView treeView)
                    {
                        if (treeView.ShowNodeToolTips)
                        {
                            return;
                        }
                    }
                }

                if (IsBalloon)
                {
                    wp->cx += 2 * BalloonOffsetX;
                    return;
                }

                if ((tt.TipType & TipInfo.Type.Auto) != 0 && _window != null)
                {
                    _window.DefWndProc(ref m);
                    return;
                }

                if (((tt.TipType & TipInfo.Type.SemiAbsolute) != 0) && tt.Position == Point.Empty)
                {
                    Screen screen = Screen.FromPoint(cursorPos);
                    if (currentCursor != null)
                    {
                        wp->x = cursorPos.X;
                        wp->y = cursorPos.Y;
                        if (wp->y + wp->cy + currentCursor.Size.Height - currentCursor.HotSpot.Y > screen.WorkingArea.Bottom)
                        {
                            wp->y = cursorPos.Y - wp->cy;
                        }
                        else
                        {
                            wp->y = cursorPos.Y + currentCursor.Size.Height - currentCursor.HotSpot.Y;
                        }
                    }

                    if (wp->x + wp->cx > screen.WorkingArea.Right)
                    {
                        wp->x = screen.WorkingArea.Right - wp->cx;
                    }
                }
                else if ((tt.TipType & TipInfo.Type.SemiAbsolute) != 0 && tt.Position != Point.Empty)
                {
                    Screen screen = Screen.FromPoint(tt.Position);
                    wp->x = tt.Position.X;
                    if (wp->x + wp->cx > screen.WorkingArea.Right)
                    {
                        wp->x = screen.WorkingArea.Right - wp->cx;
                    }

                    wp->y = tt.Position.Y;
                    if (wp->y + wp->cy > screen.WorkingArea.Bottom)
                    {
                        wp->y = screen.WorkingArea.Bottom - wp->cy;
                    }
                }
            }

            m.Result = IntPtr.Zero;
        }

        /// <summary>
        ///  Called just before the tooltip is hidden
        /// </summary>
        private void WmPop()
        {
            IWin32Window window = GetCurrentToolWindow();
            if (window is null)
                return;

            Control control = window as Control;
            TipInfo tt = (TipInfo)_tools[window];
            if (tt is null)
            {
                return;
            }

            // Must reset the maxwidth to the screen size.
            if ((tt.TipType & TipInfo.Type.Auto) != 0 || (tt.TipType & TipInfo.Type.SemiAbsolute) != 0)
            {
                Screen screen = Screen.FromPoint(Cursor.Position);
                User32.SendMessageW(this, (User32.WM)TTM.SETMAXTIPWIDTH, IntPtr.Zero, (IntPtr)screen.WorkingArea.Width);
            }

            // For non-auto tips (those showned through the show(.) methods, we need to
            // dissassociate them from the tip control.
            if ((tt.TipType & TipInfo.Type.Auto) == 0)
            {
                _tools.Remove(control);
                _owners.Remove(window.Handle);

                control.HandleCreated -= new EventHandler(HandleCreated);
                control.HandleDestroyed -= new EventHandler(HandleDestroyed);
                _created.Remove(control);

                if (_originalPopupDelay != 0)
                {
                    AutoPopDelay = _originalPopupDelay;
                    _originalPopupDelay = 0;
                }
            }
            else
            {
                // Clear all other flags except for the Auto flag to ensure automatic tips can still show
                tt.TipType = TipInfo.Type.Auto;
                tt.Position = Point.Empty;
                _tools[control] = tt;
            }
        }

        /// <summary>
        ///  WNDPROC
        /// </summary>
        private void WndProc(ref Message msg)
        {
            switch (msg.Msg)
            {
                case (int)(User32.WM.REFLECT_NOTIFY):
                    User32.NMHDR nmhdr = (User32.NMHDR)msg.GetLParam(typeof(User32.NMHDR));
                    if (nmhdr.code == (int)TTN.SHOW && !_trackPosition)
                    {
                        WmShow();
                    }
                    else if (nmhdr.code == (int)TTN.POP)
                    {
                        WmPop();
                        _window?.DefWndProc(ref msg);
                    }
                    break;

                case (int)User32.WM.WINDOWPOSCHANGING:
                    WmWindowPosChanging(ref msg);
                    break;

                case (int)User32.WM.WINDOWPOSCHANGED:
                    if (!WmWindowPosChanged() && _window != null)
                    {
                        _window.DefWndProc(ref msg);
                    }
                    break;

                case (int)User32.WM.MOUSEACTIVATE:
                    WmMouseActivate(ref msg);
                    break;

                case (int)User32.WM.MOVE:
                    WmMove();
                    break;

                case (int)TTM.WINDOWFROMPOINT:
                    WmWindowFromPoint(ref msg);
                    break;

                case (int)User32.WM.PRINTCLIENT:
                    goto case (int)User32.WM.PAINT;

                case (int)User32.WM.PAINT:
                    if (OwnerDraw && !_isBalloon && !_trackPosition)
                    {
                        using var paintScope = new User32.BeginPaintScope(Handle);
                        Rectangle bounds = paintScope.PaintStruct.rcPaint;
                        if (bounds == Rectangle.Empty)
                        {
                            return;
                        }

                        using Graphics g = paintScope.HDC.CreateGraphics();

                        IWin32Window window = GetCurrentToolWindow();
                        if (window != null)
                        {
                            Font font;
                            try
                            {
                                font = Font.FromHfont(User32.SendMessageW(this, User32.WM.GETFONT));
                            }
                            catch (ArgumentException)
                            {
                                // If the current default tooltip font is a non-TrueType font, then
                                // Font.FromHfont throws this exception, so fall back to the default control font.
                                font = Control.DefaultFont;
                            }

                            Control control = window as Control ?? Control.FromHandle(window.Handle);
                            OnDraw(new DrawToolTipEventArgs(
                                g, window, control, bounds, GetToolTip(control), BackColor, ForeColor, font));

                            break;
                        }
                    }

                    // If not OwnerDraw, fall through
                    goto default;
                default:
                    _window?.DefWndProc(ref msg);
                    break;
            }
        }

        private class ToolTipNativeWindow : NativeWindow
        {
            private readonly ToolTip _control;

            internal ToolTipNativeWindow(ToolTip control)
            {
                _control = control;
            }

            protected override void WndProc(ref Message m) => _control?.WndProc(ref m);
        }

        private class ToolTipTimer : Timer
        {
            public ToolTipTimer(IWin32Window owner) : base()
            {
                Host = owner;
            }

            public IWin32Window Host { get; }
        }

        private class TipInfo
        {
            [Flags]
            public enum Type
            {
                None = 0x0000,
                Auto = 0x0001,
                Absolute = 0x0002,
                SemiAbsolute = 0x0004
            }

            public Type TipType { get; set; } = Type.Auto;
            private string _caption;
            private readonly string _designerText;
            public Point Position { get; set; }

            public TipInfo(string caption, Type type)
            {
                _caption = caption;
                TipType = type;
                if (type == Type.Auto)
                {
                    _designerText = caption;
                }
            }

            public string Caption
            {
                get => ((TipType & (Type.Absolute | Type.SemiAbsolute)) != 0) ? _caption : _designerText;
                set => _caption = value;
            }
        }
    }
}
