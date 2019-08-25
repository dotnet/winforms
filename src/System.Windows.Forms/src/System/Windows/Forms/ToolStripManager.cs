// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using static Interop;

namespace System.Windows.Forms
{
    public sealed class ToolStripManager
    {
        // WARNING: ThreadStatic initialization happens only on the first thread at class CTOR time.
        // use InitializeThread mechanism to initialize ThreadStatic members
        [ThreadStatic]
        private static ClientUtils.WeakRefCollection toolStripWeakArrayList;
        [ThreadStatic]
        private static ClientUtils.WeakRefCollection toolStripPanelWeakArrayList;
        [ThreadStatic]
        private static bool initialized;

        private static Font defaultFont;
        private static ConcurrentDictionary<int, Font> defaultFontCache = new ConcurrentDictionary<int, Font>();

        // WARNING: When subscribing to static event handlers - make sure you unhook from them
        // otherwise you can leak USER objects on process shutdown.
        // Consider: use WeakRefCollection
        [ThreadStatic]
        private static Delegate[] staticEventHandlers;
        private const int staticEventDefaultRendererChanged = 0;
        private const int staticEventCount = 1;

        private static readonly object internalSyncObject = new object();

        private static int currentDpi = DpiHelper.DeviceDpi;

        private static void InitalizeThread() {
            if (!initialized) {
                initialized = true;
                currentRendererType = ProfessionalRendererType;
            }
        }
        private ToolStripManager()
        {
        }

        static ToolStripManager()
        {
            SystemEvents.UserPreferenceChanging += new UserPreferenceChangingEventHandler(OnUserPreferenceChanging);
        }

        internal static Font DefaultFont
        {
            get
            {
                Font sysFont = null;

                // We need to cache the default fonts for the different DPIs.
                if (DpiHelper.IsPerMonitorV2Awareness)
                {
                    int dpi = CurrentDpi;

                    Font retFont = null;
                    if (defaultFontCache.TryGetValue(dpi, out retFont) == false || retFont == null)
                    {
                        // default to menu font
                        sysFont = SystemInformation.GetMenuFontForDpi(dpi);
                        if (sysFont != null)
                        {
                            // ensure font is in pixels so it displays properly in the property grid at design time.
                            if (sysFont.Unit != GraphicsUnit.Point)
                            {
                                retFont = ControlPaint.FontInPoints(sysFont);
                                sysFont.Dispose();
                            }
                            else
                            {
                                retFont = sysFont;
                            }
                            defaultFontCache[dpi] = retFont;
                        }
                    }
                    return retFont;
                }
                else
                {
                    Font retFont = defaultFont;  // threadsafe local reference

                    if (retFont == null)
                    {
                        lock (internalSyncObject)
                        {
                            // double check the defaultFont after the lock.
                            retFont = defaultFont;

                            if (retFont == null)
                            {
                                // default to menu font
                                sysFont = SystemFonts.MenuFont;
                                if (sysFont == null)
                                {
                                    // ...or to control font if menu font unavailable
                                    sysFont = Control.DefaultFont;
                                }
                                if (sysFont != null)
                                {
                                    // ensure font is in pixels so it displays properly in the property grid at design time.
                                    if (sysFont.Unit != GraphicsUnit.Point)
                                    {
                                        defaultFont = ControlPaint.FontInPoints(sysFont);
                                        retFont = defaultFont;
                                        sysFont.Dispose();
                                    }
                                    else
                                    {
                                        defaultFont = sysFont;
                                        retFont = defaultFont;
                                    }
                                }
                                return retFont;
                            }
                        }
                    }
                    return retFont;
                }
            }
        }

        internal static int CurrentDpi
        {
            get
            {
                return currentDpi;
            }
            set
            {
                currentDpi = value;
            }
        }

        internal static ClientUtils.WeakRefCollection ToolStrips
        {
            get
            {
                if (toolStripWeakArrayList == null)
                {
                    toolStripWeakArrayList = new ClientUtils.WeakRefCollection();
                }
                return toolStripWeakArrayList;
            }
        }

        ///<summary>Static events only!!!</summary>
        private static void AddEventHandler(int key, Delegate value)
        {
            lock (internalSyncObject)
            {
                if (staticEventHandlers == null)
                {
                    staticEventHandlers = new Delegate[staticEventCount];
                }
                staticEventHandlers[key] = Delegate.Combine(staticEventHandlers[key], value);
            }
        }

        /// <summary>
        ///  Find a toolstrip in the weak ref arraylist, return null if nothing was found
        /// </summary>
        public static ToolStrip FindToolStrip(string toolStripName)
        {
            ToolStrip result = null;
            for (int i = 0; i < ToolStrips.Count; i++)
            {
                // is this the right string comparaison?
                if (ToolStrips[i] != null && string.Equals(((ToolStrip)ToolStrips[i]).Name, toolStripName, StringComparison.Ordinal))
                {
                    result = (ToolStrip)ToolStrips[i];
                    break;
                }
            }
            return result;
        }

        /// <summary>
        ///  Find a toolstrip in the weak ref arraylist, return null if nothing was found
        /// </summary>
        internal static ToolStrip FindToolStrip(Form owningForm, string toolStripName)
        {
            ToolStrip result = null;
            for (int i = 0; i < ToolStrips.Count; i++)
            {
                // is this the right string comparaison?
                if (ToolStrips[i] != null && string.Equals(((ToolStrip)ToolStrips[i]).Name, toolStripName, StringComparison.Ordinal))
                {
                    result = (ToolStrip)ToolStrips[i];
                    if (result.FindForm() == owningForm)
                    {
                        break;
                    }
                }
            }
            return result;
        }

        private static bool CanChangeSelection(ToolStrip start, ToolStrip toolStrip)
        {
            if (toolStrip == null)
            {
                Debug.Fail("passed in bogus toolstrip, why?");
                return false;
            }
            bool canChange = toolStrip.TabStop == false &&
                                 toolStrip.Enabled &&
                                 toolStrip.Visible &&
                                 !toolStrip.IsDisposed &&
                                 !toolStrip.Disposing &&
                                 !toolStrip.IsDropDown &&
                                 IsOnSameWindow(start, toolStrip);

            if (canChange)
            {
                foreach (ToolStripItem item in toolStrip.Items)
                {
                    if (item.CanSelect)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private static bool ChangeSelection(ToolStrip start, ToolStrip toolStrip)
        {
            if (toolStrip == null || start == null)
            {
                Debug.Assert(toolStrip != null, "passed in bogus toolstrip, why?");
                Debug.Assert(start != null, "passed in bogus start, why?");
                return false;
            }
            if (start == toolStrip)
            {
                return false;
            }
            if (ModalMenuFilter.InMenuMode)
            {
                if (ModalMenuFilter.GetActiveToolStrip() == start)
                {
                    ModalMenuFilter.RemoveActiveToolStrip(start);
                    start.NotifySelectionChange(null);
                }
                ModalMenuFilter.SetActiveToolStrip(toolStrip);
            }
            else
            {
                toolStrip.Focus();
            }
            // copy over the hwnd that we want to restore focus to on ESC
            start.SnapFocusChange(toolStrip);

            toolStrip.SelectNextToolStripItem(null, toolStrip.RightToLeft != RightToLeft.Yes);
            return true;
        }

        private static Delegate GetEventHandler(int key)
        {
            lock (internalSyncObject)
            {
                if (staticEventHandlers == null)
                {
                    return null;
                }
                else
                {
                    return (Delegate)staticEventHandlers[key];
                }
            }
        }

        private static bool IsOnSameWindow(Control control1, Control control2)
        {
            return (WindowsFormsUtils.GetRootHWnd(control1).Handle == WindowsFormsUtils.GetRootHWnd(control2).Handle);
        }

        internal static bool IsThreadUsingToolStrips()
        {
            return (toolStripWeakArrayList != null && (toolStripWeakArrayList.Count > 0));
        }

        private static void OnUserPreferenceChanging(object sender, UserPreferenceChangingEventArgs e)
        {
            // using changing here so that the cache will be cleared by the time the ToolStrip
            // hooks onto the changed event.

            // SPI_SETNONCLIENTMETRICS is put up in WM_SETTINGCHANGE if the Menu font changes.
            // this corresponds to UserPreferenceCategory.Window.
            if (e.Category == UserPreferenceCategory.Window) {
                if (DpiHelper.IsPerMonitorV2Awareness) {
                    defaultFontCache.Clear();
                }
                else {
                    lock (internalSyncObject) {
                        defaultFont = null;
                    }
                }
            }
        }

        internal static void NotifyMenuModeChange(bool invalidateText, bool activationChange)
        {
            bool toolStripPruneNeeded = false;

            // If we've toggled the ShowUnderlines value, we'll need to invalidate
            for (int i = 0; i < ToolStrips.Count; i++)
            {
                if (!(ToolStrips[i] is ToolStrip toolStrip))
                {
                    toolStripPruneNeeded = true;
                    continue;
                }
                if (invalidateText)
                {
                    toolStrip.InvalidateTextItems();
                }
                if (activationChange)
                {
                    toolStrip.KeyboardActive = false;
                }
            }
            if (toolStripPruneNeeded)
            {
                PruneToolStripList();
            }

        }

        /// <summary> removes dead entries from the toolstrip weak reference collection. </summary>
        internal static void PruneToolStripList()
        {
            if (toolStripWeakArrayList != null)
            {
                if (toolStripWeakArrayList.Count > 0)
                {
                    for (int i = toolStripWeakArrayList.Count - 1; i >= 0; i--)
                    {
                        if (toolStripWeakArrayList[i] == null)
                        {
                            toolStripWeakArrayList.RemoveAt(i);
                        }
                    }
                }
            }
        }

        /// <summary> static events only!!!</summary>
        private static void RemoveEventHandler(int key, Delegate value)
        {
            lock (internalSyncObject)
            {
                if (staticEventHandlers != null)
                {
                    staticEventHandlers[key] = Delegate.Remove(staticEventHandlers[key], value);
                }
            }
        }

        // this is a special version of SelectNextControl which looks for ToolStrips
        // that are TabStop = false in TabOrder.  This is used from Control+Tab
        // handling to swap focus between ToolStrips.
        internal static bool SelectNextToolStrip(ToolStrip start, bool forward)
        {
            if (start == null || start.ParentInternal == null)
            {
                Debug.Assert(start != null, "why is null passed here?");
                return false;
            }

            ToolStrip wrappedControl = null;
            ToolStrip nextControl = null;

            int startTabIndex = start.TabIndex;
            int index = ToolStrips.IndexOf(start);
            int totalCount = ToolStrips.Count;
            for (int i = 0; i < totalCount; i++)
            {
                index = (forward) ? (index + 1) % totalCount
                                  : (index + totalCount - 1) % totalCount;

                if (!(ToolStrips[index] is ToolStrip toolStrip) ||
                    toolStrip == start)
                {
                    continue;
                }

                int nextControlTabIndex = toolStrip.TabIndex;
                Debug.WriteLineIf(ToolStrip.ControlTabDebug.TraceVerbose, "SELECTNEXTTOOLSTRIP: start: " + startTabIndex.ToString(CultureInfo.CurrentCulture) + " " + start.Name);
                // since CanChangeSelection can iterate through all the items in a toolstrip,
                // defer the checking until we think we've got a viable TabIndex candidate.
                // this brings it to O(n+m) instead of O(n*m) where n is # toolstrips & m is avg number
                // items/toolstrip
                if (forward)
                {
                    if (nextControlTabIndex >= startTabIndex && CanChangeSelection(start, toolStrip))
                    {
                        Debug.WriteLineIf(ToolStrip.ControlTabDebug.TraceVerbose, "FORWARD considering selection " + toolStrip.Name + " " + toolStrip.TabIndex.ToString(CultureInfo.CurrentCulture));
                        if (nextControl == null)
                        {
                            nextControl = toolStrip;
                        }
                        else if (toolStrip.TabIndex < nextControl.TabIndex)
                        {
                            // we want to pick a larger index, but one that's
                            // closest to the start tab index.
                            nextControl = toolStrip;
                        }
                    }
                    else if (((wrappedControl == null) || (toolStrip.TabIndex < wrappedControl.TabIndex))
                              && CanChangeSelection(start, toolStrip))
                    {
                        // we've found a candidate for wrapping (the one with the smallest tab index in the collection)
                        Debug.WriteLineIf(ToolStrip.ControlTabDebug.TraceVerbose, "\tFORWARD new wrap candidate " + toolStrip.Name);
                        wrappedControl = toolStrip;
                    }
                }
                else
                {
                    if (nextControlTabIndex <= startTabIndex && CanChangeSelection(start, toolStrip))
                    {
                        Debug.WriteLineIf(ToolStrip.ControlTabDebug.TraceVerbose, "\tREVERSE selecting " + toolStrip.Name);
                        if (nextControl == null)
                        {
                            nextControl = toolStrip;
                        }
                        else if (toolStrip.TabIndex > nextControl.TabIndex)
                        {
                            // we want to pick a smaller index, but one that's
                            // closest to the start tab index.
                            nextControl = toolStrip;
                        }
                    }
                    else if (((wrappedControl == null) || (toolStrip.TabIndex > wrappedControl.TabIndex))
                               && CanChangeSelection(start, toolStrip))
                    {
                        // we've found a candidate for wrapping (the one with the largest tab index in the collection)
                        Debug.WriteLineIf(ToolStrip.ControlTabDebug.TraceVerbose, "\tREVERSE new wrap candidate " + toolStrip.Name);

                        wrappedControl = toolStrip;
                    }
                    else
                    {
                        Debug.WriteLineIf(ToolStrip.ControlTabDebug.TraceVerbose, "\tREVERSE skipping wrap candidate " + toolStrip.Name + toolStrip.TabIndex.ToString(CultureInfo.CurrentCulture));

                    }

                }
                if (nextControl != null
                    && Math.Abs(nextControl.TabIndex - startTabIndex) <= 1)
                {
                    // if we've found a valid candidate AND it's within 1
                    // then bail, we've found something close enough.
                    break;
                }
            }
            if (nextControl != null)
            {
                Debug.WriteLineIf(ToolStrip.ControlTabDebug.TraceVerbose, "SELECTING " + nextControl.Name);
                return ChangeSelection(start, nextControl);

            }
            else if (wrappedControl != null)
            {
                Debug.WriteLineIf(ToolStrip.ControlTabDebug.TraceVerbose, "WRAPPING " + wrappedControl.Name);

                return ChangeSelection(start, wrappedControl);
            }
            return false;
        }

        ///  ============================================================================
        ///  BEGIN task specific functions.  Since ToolStripManager is used
        ///  for Painting, Merging and Rafting, and who knows what else in the future
        ///  the following properties/methods/events are organized in regions
        ///  alphabetically by task
        ///  ----------------------------------------------------------------------------

        ///
        ///  ToolStripManager Default Renderer
        ///
        #region DefaultRenderer

        ///  These are thread static because we want separate instances
        ///  for each thread.  We dont want to guarantee thread safety
        ///  and dont want to have to take locks in painting code.
        [ThreadStatic]
        private static ToolStripRenderer defaultRenderer;

        // types cached for perf.
        internal static Type SystemRendererType = typeof(ToolStripSystemRenderer);
        internal static Type ProfessionalRendererType = typeof(ToolStripProfessionalRenderer);
        private static bool visualStylesEnabledIfPossible = true;

        [ThreadStatic]
        private static Type currentRendererType;

        private static Type CurrentRendererType
        {
            get
            {
                InitalizeThread();
                return currentRendererType;
            }
            set
            {
                currentRendererType = value;
            }
        }

        private static Type DefaultRendererType
        {
            get
            {
                return ProfessionalRendererType;
            }
        }

        /// <summary> the default renderer for the thread.  When ToolStrip.RenderMode is set to manager - this
        ///  is the property used.
        /// </summary>
        public static ToolStripRenderer Renderer
        {
            get
            {
                if (defaultRenderer == null)
                {
                    defaultRenderer = CreateRenderer(RenderMode);
                }
                return defaultRenderer;
            }
            set
            {
                ///
                if (defaultRenderer != value)
                {
                    CurrentRendererType = (value == null) ? DefaultRendererType : value.GetType();
                    defaultRenderer = value;

                    ((EventHandler)GetEventHandler(staticEventDefaultRendererChanged))?.Invoke(null, EventArgs.Empty);

                }
            }
        }

        // <summary>
        // occurs when toolstripmanager.Renderer property has changed
        //
        // WARNING: When subscribing to static event handlers - make sure you unhook from them
        // otherwise you can leak USER objects on process shutdown.
        // </summary>
        public static event EventHandler RendererChanged
        {
            add => AddEventHandler(staticEventDefaultRendererChanged, value);
            remove => RemoveEventHandler(staticEventDefaultRendererChanged, value);
        }

        /// <summary> returns the default toolstrip RenderMode for the thread </summary>
        public static ToolStripManagerRenderMode RenderMode
        {
            get
            {
                Type currentType = CurrentRendererType;

                if (defaultRenderer != null && !defaultRenderer.IsAutoGenerated)
                {
                    return ToolStripManagerRenderMode.Custom;
                }
                // check the type of the currently set renderer.
                // types are cached as this may be called frequently.
                if (currentType == ProfessionalRendererType)
                {
                    return ToolStripManagerRenderMode.Professional;
                }
                if (currentType == SystemRendererType)
                {
                    return ToolStripManagerRenderMode.System;
                }
                return ToolStripManagerRenderMode.Custom;
            }
            set
            {

                ///
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)ToolStripManagerRenderMode.Custom, (int)ToolStripManagerRenderMode.Professional))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(ToolStripManagerRenderMode));
                }

                switch (value)
                {
                    case ToolStripManagerRenderMode.System:
                    case ToolStripManagerRenderMode.Professional:
                        Renderer = CreateRenderer(value);
                        break;
                    case ToolStripManagerRenderMode.Custom:
                        throw new NotSupportedException(SR.ToolStripRenderModeUseRendererPropertyInstead);
                }
            }
        }

        /// <summary> an additional layering of control.  this lets you pick whether your toolbars
        ///  should use visual style information (theming) to render itself.
        ///  potentially you could want a themed app but an unthemed toolstrip. (e.g. Whidbey VS).
        /// </summary>
        public static bool VisualStylesEnabled
        {
            get
            {
                return visualStylesEnabledIfPossible && Application.RenderWithVisualStyles;
            }
            set
            {
                bool oldVis = VisualStylesEnabled;
                visualStylesEnabledIfPossible = value;

                if (oldVis != VisualStylesEnabled)
                {
                    ((EventHandler)GetEventHandler(staticEventDefaultRendererChanged))?.Invoke(null, EventArgs.Empty);
                }
            }
        }

        internal static ToolStripRenderer CreateRenderer(ToolStripManagerRenderMode renderMode)
        {
            switch (renderMode)
            {
                case ToolStripManagerRenderMode.System:
                    return new ToolStripSystemRenderer(/*isAutoGenerated=*/true);
                case ToolStripManagerRenderMode.Professional:
                    return new ToolStripProfessionalRenderer(/*isAutoGenerated=*/true);
                case ToolStripManagerRenderMode.Custom:
                default:
                    return new ToolStripSystemRenderer(/*isAutoGenerated=*/true);
            }
        }
        internal static ToolStripRenderer CreateRenderer(ToolStripRenderMode renderMode)
        {
            switch (renderMode)
            {
                case ToolStripRenderMode.System:
                    return new ToolStripSystemRenderer(/*isAutoGenerated=*/true);
                case ToolStripRenderMode.Professional:
                    return new ToolStripProfessionalRenderer(/*isAutoGenerated=*/true);
                case ToolStripRenderMode.Custom:
                default:
                    return new ToolStripSystemRenderer(/*isAutoGenerated=*/true);
            }
        }

        #endregion DefaultRenderer

        #region ToolStripPanel

        internal static ClientUtils.WeakRefCollection ToolStripPanels
        {
            get
            {
                if (toolStripPanelWeakArrayList == null)
                {
                    toolStripPanelWeakArrayList = new ClientUtils.WeakRefCollection();
                }
                return toolStripPanelWeakArrayList;
            }
        }

        internal static ToolStripPanel ToolStripPanelFromPoint(Control draggedControl, Point screenLocation)
        {
            if (toolStripPanelWeakArrayList != null)
            {
                ISupportToolStripPanel draggedItem = draggedControl as ISupportToolStripPanel;
                bool rootWindowCheck = draggedItem.IsCurrentlyDragging;

                for (int i = 0; i < toolStripPanelWeakArrayList.Count; i++)
                {
                    if (toolStripPanelWeakArrayList[i] is ToolStripPanel toolStripPanel && toolStripPanel.IsHandleCreated && toolStripPanel.Visible &&
                        toolStripPanel.DragBounds.Contains(toolStripPanel.PointToClient(screenLocation)))
                    {
                        // Ensure that we cant drag off one window to another.
                        if (rootWindowCheck)
                        {
                            if (IsOnSameWindow(draggedControl, toolStripPanel))
                            {
                                return toolStripPanel;
                            }
                        }
                        else
                        {
                            return toolStripPanel;
                        }
                    }
                }
            }
            return null;
        }

        #endregion

        #region ToolStripSettings

        /// <summary>
        ///  Loads settings for the given Form using the form type's fullname as settings key.
        /// </summary>
        public static void LoadSettings(Form targetForm)
        {
            if (targetForm == null)
            {
                throw new ArgumentNullException(nameof(targetForm));
            }

            LoadSettings(targetForm, targetForm.GetType().FullName);
        }

        /// <summary>
        ///  Loads settings for the given Form with the given settings key.
        /// </summary>
        public static void LoadSettings(Form targetForm, string key)
        {
            if (targetForm == null)
            {
                throw new ArgumentNullException(nameof(targetForm));
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            ToolStripSettingsManager settingsManager = new ToolStripSettingsManager(targetForm, key);

            settingsManager.Load();
        }

        /// <summary>
        ///  Saves settings for the given form using the form type's fullname as settings key.
        /// </summary>
        public static void SaveSettings(Form sourceForm)
        {
            if (sourceForm == null)
            {
                throw new ArgumentNullException(nameof(sourceForm));
            }

            SaveSettings(sourceForm, sourceForm.GetType().FullName);
        }

        /// <summary>
        ///  Saves settings for the given form with the given settings key.
        /// </summary>
        public static void SaveSettings(Form sourceForm, string key)
        {
            if (sourceForm == null)
            {
                throw new ArgumentNullException(nameof(sourceForm));
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            ToolStripSettingsManager settingsManager = new ToolStripSettingsManager(sourceForm, key);
            ;

            settingsManager.Save();
        }

        #endregion

        ///
        ///  ToolStripManager ALT key PreProcessing
        ///
        #region MenuKeyAndShortcutProcessing

        ///  ModalMenuFilter
        ///  - this installs a message filter when a dropdown becomes active.
        ///  - the message filter
        ///  a. eats WM_MOUSEMOVEs so that the window that's underneath
        ///  doesnt get highlight processing/tooltips
        ///  b. detects mouse clicks.  if the click is outside the dropdown, it
        ///  dismisses it.
        ///  c. detects when the active window has changed.  If the active window
        ///  is unexpected, it dismisses all dropdowns.
        ///  d. detects keyboard messages, and redirects them to the active dropdown.
        ///
        ///  - There should be 1 Message Filter per thread and it should be uninstalled once
        ///  the last dropdown has gone away
        ///  This is not part of ToolStripManager because it's DropDown specific and
        ///  we dont want to publicly expose this message filter.
        internal class ModalMenuFilter : IMessageModifyAndFilter
        {
            private HandleRef _activeHwnd = NativeMethods.NullHandleRef; // the window that was active when we showed the dropdown
            private HandleRef _lastActiveWindow = NativeMethods.NullHandleRef;         // the window that was last known to be active
            private List<ToolStrip> _inputFilterQueue;
            private bool _inMenuMode = false;
            private bool _caretHidden = false;
            private bool _showUnderlines = false;
            private bool menuKeyToggle = false;
            private bool _suspendMenuMode = false;
            private HostedWindowsFormsMessageHook messageHook;
            private Timer _ensureMessageProcessingTimer = null;
            private const int MESSAGE_PROCESSING_INTERVAL = 500;

            private ToolStrip _toplevelToolStrip = null;

            private readonly WeakReference<IKeyboardToolTip> lastFocusedTool = new WeakReference<IKeyboardToolTip>(null);

#if DEBUG
            bool _justEnteredMenuMode = false;
#endif
            [ThreadStatic]
            private static ModalMenuFilter _instance;

            internal static ModalMenuFilter Instance
            {
                get
                {
                    if (_instance == null)
                    {
                        _instance = new ModalMenuFilter();
                    }
                    return _instance;
                }
            }

            private ModalMenuFilter()
            {
            }

            ///  this is the HWnd that was active when we popped the first dropdown.
            internal static HandleRef ActiveHwnd
            {
                get { return Instance.ActiveHwndInternal; }
            }

            // returns whether or not we should show focus cues for mnemonics.
            public bool ShowUnderlines
            {
                get
                {
                    return _showUnderlines;
                }
                set
                {
                    if (_showUnderlines != value)
                    {
                        _showUnderlines = value;
                        ToolStripManager.NotifyMenuModeChange(/*textStyleChanged*/true, /*activationChanged*/false);
                    }
                }
            }

            private HandleRef ActiveHwndInternal
            {
                get
                {
                    return _activeHwnd;
                }
                set
                {
                    if (_activeHwnd.Handle != value.Handle)
                    {
                        Control control = null;

                        // unsubscribe from handle recreate.
                        if (_activeHwnd.Handle != IntPtr.Zero)
                        {
                            control = Control.FromHandle(_activeHwnd.Handle);
                            if (control != null)
                            {
                                control.HandleCreated -= new EventHandler(OnActiveHwndHandleCreated);
                            }
                        }

                        _activeHwnd = value;

                        // make sure we watch out for handle recreates.
                        control = Control.FromHandle(_activeHwnd.Handle);
                        if (control != null)
                        {
                            control.HandleCreated += new EventHandler(OnActiveHwndHandleCreated);
                        }
                    }

                }
            }

            // returns whether or not someone has called EnterMenuMode.
            internal static bool InMenuMode
            {
                get { return Instance._inMenuMode; }
            }

            internal static bool MenuKeyToggle
            {
                get
                {
                    return Instance.menuKeyToggle;
                }
                set
                {
                    if (Instance.menuKeyToggle != value)
                    {
                        Instance.menuKeyToggle = value;

                    }
                }
            }

            ///  This is used in scenarios where windows forms
            ///  does not own the message pump, but needs access
            ///  to the message queue.
            private HostedWindowsFormsMessageHook MessageHook
            {
                get
                {
                    if (messageHook == null)
                    {
                        messageHook = new HostedWindowsFormsMessageHook();
                    }
                    return messageHook;
                }
            }

            // ToolStrip analog to WM_ENTERMENULOOP
            private void EnterMenuModeCore()
            {
                Debug.Assert(!InMenuMode, "How did we get here if we're already in menu mode?");

                if (!InMenuMode)
                {
                    Debug.WriteLineIf(ToolStrip.SnapFocusDebug.TraceVerbose, "___________Entering MenuMode....");
#if DEBUG
                    _justEnteredMenuMode = true;
#endif
                    IntPtr hwndActive = UnsafeNativeMethods.GetActiveWindow();
                    if (hwndActive != IntPtr.Zero)
                    {
                        ActiveHwndInternal = new HandleRef(this, hwndActive);
                    }

                    // PERF,

                    Application.ThreadContext.FromCurrent().AddMessageFilter(this);
                    Application.ThreadContext.FromCurrent().TrackInput(true);

                    if (!Application.ThreadContext.FromCurrent().GetMessageLoop(true))
                    {
                        // message filter isnt going to help as we dont own the message pump
                        // switch over to a MessageHook
                        MessageHook.HookMessages = true;
                    }
                    _inMenuMode = true;

                    NotifyLastLastFocusedToolAboutFocusLoss();

                    // fire timer messages to force our filter to get evaluated.
                    ProcessMessages(true);
                }

            }

            internal void NotifyLastLastFocusedToolAboutFocusLoss()
            {
                IKeyboardToolTip lastFocusedTool = KeyboardToolTipStateMachine.Instance.LastFocusedTool;
                if (lastFocusedTool != null)
                {
                    this.lastFocusedTool.SetTarget(lastFocusedTool);
                    KeyboardToolTipStateMachine.Instance.NotifyAboutLostFocus(lastFocusedTool);
                }
            }

            internal static void ExitMenuMode()
            {
                Instance.ExitMenuModeCore();
            }

            // ToolStrip analog to WM_EXITMENULOOP
            private void ExitMenuModeCore()
            {

                // ensure we've cleaned up the timer.
                ProcessMessages(false);

                if (InMenuMode)
                {
                    try
                    {
                        Debug.WriteLineIf(ToolStrip.SnapFocusDebug.TraceVerbose, "___________Exiting MenuMode....");

                        if (messageHook != null)
                        {
                            // message filter isnt going to help as we dont own the message pump
                            // switch over to a MessageHook
                            messageHook.HookMessages = false;
                        }
                        // PERF,

                        Application.ThreadContext.FromCurrent().RemoveMessageFilter(this);
                        Application.ThreadContext.FromCurrent().TrackInput(false);

#if DEBUG
                        _justEnteredMenuMode = false;
#endif
                        if (ActiveHwnd.Handle != IntPtr.Zero)
                        {
                            // unsubscribe from handle creates
                            Control control = Control.FromHandle(ActiveHwnd.Handle);
                            if (control != null)
                            {
                                control.HandleCreated -= new EventHandler(OnActiveHwndHandleCreated);
                            }
                            ActiveHwndInternal = NativeMethods.NullHandleRef;
                        }
                        if (_inputFilterQueue != null)
                        {
                            _inputFilterQueue.Clear();
                        }
                        if (_caretHidden)
                        {
                            _caretHidden = false;
                            SafeNativeMethods.ShowCaret(NativeMethods.NullHandleRef);
                        }

                        if (lastFocusedTool.TryGetTarget(out IKeyboardToolTip tool) && tool != null)
                        {
                            KeyboardToolTipStateMachine.Instance.NotifyAboutGotFocus(tool);
                        }
                    }
                    finally
                    {
                        _inMenuMode = false;

                        // skip the setter here so we only iterate through the toolstrips once.
                        bool textStyleChanged = _showUnderlines;
                        _showUnderlines = false;
                        ToolStripManager.NotifyMenuModeChange(/*textStyleChanged*/textStyleChanged, /*activationChanged*/true);

                    }

                }
            }

            internal static ToolStrip GetActiveToolStrip()
            {
                return Instance.GetActiveToolStripInternal();
            }

            internal ToolStrip GetActiveToolStripInternal()
            {
                if (_inputFilterQueue != null && _inputFilterQueue.Count > 0)
                {
                    return _inputFilterQueue[_inputFilterQueue.Count - 1];
                }
                return null;
            }

            // return the toolstrip that is at the root.
            private ToolStrip GetCurrentToplevelToolStrip()
            {
                if (_toplevelToolStrip == null)
                {
                    ToolStrip activeToolStrip = GetActiveToolStripInternal();
                    if (activeToolStrip != null)
                    {
                        _toplevelToolStrip = activeToolStrip.GetToplevelOwnerToolStrip();
                    }
                }
                return _toplevelToolStrip;
            }

            private void OnActiveHwndHandleCreated(object sender, EventArgs e)
            {
                Control topLevel = sender as Control;
                ActiveHwndInternal = new HandleRef(this, topLevel.Handle);
            }
            internal static void ProcessMenuKeyDown(ref Message m)
            {
                Keys keyData = (Keys)(int)m.WParam;

                if (Control.FromHandle(m.HWnd) is ToolStrip toolStrip && !toolStrip.IsDropDown)
                {
                    return;
                }

                // handle the case where the ALT key has been pressed down while a dropdown
                // was open.  We need to clear off the MenuKeyToggle so the next ALT will activate
                // the menu.

                if (ToolStripManager.IsMenuKey(keyData))
                {
                    if (!InMenuMode && MenuKeyToggle)
                    {
                        MenuKeyToggle = false;
                    }
                    else if (!MenuKeyToggle)
                    {
                        ModalMenuFilter.Instance.ShowUnderlines = true;
                    }
                }

            }

            internal static void CloseActiveDropDown(ToolStripDropDown activeToolStripDropDown, ToolStripDropDownCloseReason reason)
            {

                activeToolStripDropDown.SetCloseReason(reason);
                activeToolStripDropDown.Visible = false;

                // there's no more dropdowns left in the chain
                if (GetActiveToolStrip() == null)
                {
                    Debug.WriteLineIf(ToolStrip.SnapFocusDebug.TraceVerbose, "[ModalMenuFilter.CloseActiveDropDown] Calling exit because there are no more dropdowns left to activate.");
                    ExitMenuMode();

                    // make sure we roll selection off  the toplevel toolstrip.
                    if (activeToolStripDropDown.OwnerItem != null)
                    {
                        activeToolStripDropDown.OwnerItem.Unselect();
                    }
                }

            }

            // fire a timer event to ensure we have a message in the queue every 500ms
            private void ProcessMessages(bool process)
            {
                if (process)
                {
                    if (_ensureMessageProcessingTimer == null)
                    {
                        _ensureMessageProcessingTimer = new Timer();
                    }
                    _ensureMessageProcessingTimer.Interval = MESSAGE_PROCESSING_INTERVAL;
                    _ensureMessageProcessingTimer.Enabled = true;
                }
                else if (_ensureMessageProcessingTimer != null)
                {
                    _ensureMessageProcessingTimer.Enabled = false;
                    _ensureMessageProcessingTimer.Dispose();
                    _ensureMessageProcessingTimer = null;
                }
            }

            private void ProcessMouseButtonPressed(IntPtr hwndMouseMessageIsFrom, int x, int y)
            {
                Debug.WriteLineIf(ToolStrip.SnapFocusDebug.TraceVerbose, "[ModalMenuFilter.ProcessMouseButtonPressed] Found a mouse down.");

                int countDropDowns = _inputFilterQueue.Count;
                for (int i = 0; i < countDropDowns; i++)
                {
                    ToolStrip activeToolStrip = GetActiveToolStripInternal();

                    if (activeToolStrip != null)
                    {
                        var pt = new Point(x, y);
                        UnsafeNativeMethods.MapWindowPoints(new HandleRef(activeToolStrip, hwndMouseMessageIsFrom), new HandleRef(activeToolStrip, activeToolStrip.Handle), ref pt, 1);
                        if (!activeToolStrip.ClientRectangle.Contains(pt.X, pt.Y))
                        {
                            if (activeToolStrip is ToolStripDropDown activeToolStripDropDown)
                            {

                                if (!(activeToolStripDropDown.OwnerToolStrip != null
                                    && activeToolStripDropDown.OwnerToolStrip.Handle == hwndMouseMessageIsFrom
                                    && activeToolStripDropDown.OwnerDropDownItem != null
                                     && activeToolStripDropDown.OwnerDropDownItem.DropDownButtonArea.Contains(x, y)))
                                {
                                    // the owner item should handle closing the dropdown
                                    // this allows code such as if (DropDown.Visible) { Hide, Show } etc.
                                    CloseActiveDropDown(activeToolStripDropDown, ToolStripDropDownCloseReason.AppClicked);
                                }
                            }
                            else
                            {
                                // make sure we clear the selection.
                                activeToolStrip.NotifySelectionChange(/*selectedItem=*/null);
                                // we're a toplevel toolstrip and we've clicked somewhere else.
                                // Exit menu mode
                                Debug.WriteLineIf(ToolStrip.SnapFocusDebug.TraceVerbose, "[ModalMenuFilter.ProcessMouseButtonPressed] Calling exit because we're a toplevel toolstrip and we've clicked somewhere else.");
                                ExitMenuModeCore();
                            }
                        }
                        else
                        {
                            // we've found a dropdown that intersects with the mouse message
                            break;
                        }
                    }
                    else
                    {
                        Debug.WriteLineIf(ToolStrip.SnapFocusDebug.TraceVerbose, "[ModalMenuFilter.ProcessMouseButtonPressed] active toolstrip is null.");
                        break;
                    }
                }

            }
            private bool ProcessActivationChange()
            {
                int countDropDowns = _inputFilterQueue.Count;
                for (int i = 0; i < countDropDowns; i++)
                {
                    if (GetActiveToolStripInternal() is ToolStripDropDown activeDropDown && activeDropDown.AutoClose)
                    {
                        activeDropDown.Visible = false;
                    }
                }
                // if (_inputFilterQueue.Count == 0) {
                ExitMenuModeCore();
                return true;
                //}
                //return false;

            }

            internal static void SetActiveToolStrip(ToolStrip toolStrip, bool menuKeyPressed)
            {
                if (!InMenuMode && menuKeyPressed)
                {
                    Instance.ShowUnderlines = true;
                }

                Instance.SetActiveToolStripCore(toolStrip);
            }

            internal static void SetActiveToolStrip(ToolStrip toolStrip)
            {
                Instance.SetActiveToolStripCore(toolStrip);
            }

            private void SetActiveToolStripCore(ToolStrip toolStrip)
            {

                if (toolStrip == null)
                {
                    return;
                }
                if (toolStrip.IsDropDown)
                {
                    // for something that never closes, dont use menu mode.
                    ToolStripDropDown dropDown = toolStrip as ToolStripDropDown;

                    if (dropDown.AutoClose == false)
                    {
                        // store off the current active hwnd
                        IntPtr hwndActive = UnsafeNativeMethods.GetActiveWindow();
                        if (hwndActive != IntPtr.Zero)
                        {
                            ActiveHwndInternal = new HandleRef(this, hwndActive);
                        }
                        // dont actually enter menu mode...
                        return;
                    }
                }
                toolStrip.KeyboardActive = true;

                if (_inputFilterQueue == null)
                {
                    // use list because we want to be able to remove at any point
                    _inputFilterQueue = new List<ToolStrip>();
                }
                else
                {
                    ToolStrip currentActiveToolStrip = GetActiveToolStripInternal();

                    // toolstrip dropdowns push/pull their activation based on visibility.
                    // we have to account for the toolstrips that arent dropdowns
                    if (currentActiveToolStrip != null)
                    {
                        if (!currentActiveToolStrip.IsDropDown)
                        {
                            _inputFilterQueue.Remove(currentActiveToolStrip);
                        }
                        else if ((toolStrip.IsDropDown)
                                  && (ToolStripDropDown.GetFirstDropDown(toolStrip)
                                  != ToolStripDropDown.GetFirstDropDown(currentActiveToolStrip)))
                        {

                            Debug.WriteLineIf(ToolStrip.SnapFocusDebug.TraceVerbose, "[ModalMenuFilter.SetActiveToolStripCore] Detected a new dropdown not in this chain opened, Dismissing everything in the old chain. ");
                            _inputFilterQueue.Remove(currentActiveToolStrip);

                            ToolStripDropDown currentActiveToolStripDropDown = currentActiveToolStrip as ToolStripDropDown;
                            currentActiveToolStripDropDown.DismissAll();
                        }
                    }
                }

                // reset the toplevel toolstrip
                _toplevelToolStrip = null;

                if (!_inputFilterQueue.Contains(toolStrip))
                {
                    _inputFilterQueue.Add(toolStrip);
                }

                if (!InMenuMode && _inputFilterQueue.Count > 0)
                {
                    Debug.WriteLineIf(ToolStrip.SnapFocusDebug.TraceVerbose, "[ModalMenuFilter.SetActiveToolStripCore] Setting " + WindowsFormsUtils.GetControlInformation(toolStrip.Handle) + " active.");

                    EnterMenuModeCore();
                }
                // hide the caret if we're showing a toolstrip dropdown
                if (!_caretHidden && toolStrip.IsDropDown && InMenuMode)
                {
                    _caretHidden = true;
                    SafeNativeMethods.HideCaret(NativeMethods.NullHandleRef);
                }

            }

            internal static void SuspendMenuMode()
            {
                Debug.WriteLineIf(ToolStrip.SnapFocusDebug.TraceVerbose, "[ModalMenuFilter] SuspendMenuMode");

                Instance._suspendMenuMode = true;
            }

            internal static void ResumeMenuMode()
            {
                Debug.WriteLineIf(ToolStrip.SnapFocusDebug.TraceVerbose, "[ModalMenuFilter] ResumeMenuMode");
                Instance._suspendMenuMode = false;
            }
            internal static void RemoveActiveToolStrip(ToolStrip toolStrip)
            {
                Instance.RemoveActiveToolStripCore(toolStrip);
            }

            private void RemoveActiveToolStripCore(ToolStrip toolStrip)
            {
                // precautionary - remove the active toplevel toolstrip.
                _toplevelToolStrip = null;

                if (_inputFilterQueue != null)
                {
                    _inputFilterQueue.Remove(toolStrip);
                }
            }

            private static bool IsChildOrSameWindow(HandleRef hwndParent, HandleRef hwndChild)
            {
                if (hwndParent.Handle == hwndChild.Handle)
                {
                    return true;
                }
                if (UnsafeNativeMethods.IsChild(hwndParent, hwndChild))
                {
                    return true;
                }
                return false;
            }

            private static bool IsKeyOrMouseMessage(Message m)
            {
                bool filterMessage = false;

                if (m.Msg >= WindowMessages.WM_MOUSEFIRST && m.Msg <= WindowMessages.WM_MOUSELAST)
                {
                    filterMessage = true;
                }
                else if (m.Msg >= WindowMessages.WM_NCLBUTTONDOWN && m.Msg <= WindowMessages.WM_NCMBUTTONDBLCLK)
                {
                    filterMessage = true;
                }
                else if (m.Msg >= WindowMessages.WM_KEYFIRST && m.Msg <= WindowMessages.WM_KEYLAST)
                {
                    filterMessage = true;
                }
                return filterMessage;
            }

            public bool PreFilterMessage(ref Message m)
            {
#if DEBUG
                Debug.WriteLineIf(ToolStrip.SnapFocusDebug.TraceVerbose && _justEnteredMenuMode, "[ModalMenuFilter.PreFilterMessage] MenuMode MessageFilter installed and working.");
                _justEnteredMenuMode = false;
#endif

                if (_suspendMenuMode)
                {
                    return false;
                }
                ToolStrip activeToolStrip = GetActiveToolStrip();
                if (activeToolStrip == null)
                {
                    return false;
                }
                if (activeToolStrip.IsDisposed)
                {
                    RemoveActiveToolStripCore(activeToolStrip);
                    return false;
                }
                HandleRef hwndActiveToolStrip = new HandleRef(activeToolStrip, activeToolStrip.Handle);
                HandleRef hwndCurrentActiveWindow = new HandleRef(null, UnsafeNativeMethods.GetActiveWindow());

                // if the active window has changed...
                if (hwndCurrentActiveWindow.Handle != _lastActiveWindow.Handle)
                {
                    // if another window has gotten activation - we should dismiss.
                    if (hwndCurrentActiveWindow.Handle == IntPtr.Zero)
                    {
                        // we dont know what it was cause it's on another thread or doesnt exist
                        Debug.WriteLineIf(ToolStrip.SnapFocusDebug.TraceVerbose, "[ModalMenuFilter.PreFilterMessage] Dismissing because: " + WindowsFormsUtils.GetControlInformation(hwndCurrentActiveWindow.Handle) + " has gotten activation. ");
                        ProcessActivationChange();
                    }
                    else if (!(Control.FromChildHandle(hwndCurrentActiveWindow.Handle) is ToolStripDropDown)   // its NOT a dropdown
                        && !IsChildOrSameWindow(hwndCurrentActiveWindow, hwndActiveToolStrip)    // and NOT a child of the active toolstrip
                        && !IsChildOrSameWindow(hwndCurrentActiveWindow, ActiveHwnd))
                    {          // and NOT a child of the active hwnd
                        Debug.WriteLineIf(ToolStrip.SnapFocusDebug.TraceVerbose, "[ModalMenuFilter.PreFilterMessage] Calling ProcessActivationChange because: " + WindowsFormsUtils.GetControlInformation(hwndCurrentActiveWindow.Handle) + " has gotten activation. ");
                        ProcessActivationChange();
                    }
                }

                // store this off so we dont have to do activation processing next time
                _lastActiveWindow = hwndCurrentActiveWindow;

                // PERF: skip over things like PAINT...
                if (!IsKeyOrMouseMessage(m))
                {
                    return false;
                }

                DpiAwarenessContext context = CommonUnsafeNativeMethods.GetDpiAwarenessContextForWindow(m.HWnd);

                using (DpiHelper.EnterDpiAwarenessScope(context))
                {
                    switch (m.Msg)
                    {
                        case WindowMessages.WM_MOUSEMOVE:
                        case WindowMessages.WM_NCMOUSEMOVE:
                            // Mouse move messages should be eaten if they arent for a dropdown.
                            // this prevents things like ToolTips and mouse over highlights from
                            // being processed.
                            Control control = Control.FromChildHandle(m.HWnd);
                            if (control == null || !(control.TopLevelControlInternal is ToolStripDropDown))
                            {
                                // double check it's not a child control of the active toolstrip.
                                if (!IsChildOrSameWindow(hwndActiveToolStrip, new HandleRef(null, m.HWnd)))
                                {

                                    // it is NOT a child of the current active toolstrip.

                                    ToolStrip toplevelToolStrip = GetCurrentToplevelToolStrip();
                                    if (toplevelToolStrip != null
                                        && (IsChildOrSameWindow(new HandleRef(toplevelToolStrip, toplevelToolStrip.Handle),
                                                               new HandleRef(null, m.HWnd))))
                                    {
                                        // DONT EAT mouse message.
                                        // The mouse message is from an HWND that is part of the toplevel toolstrip - let the mosue move through so
                                        // when you have something like the file menu open and mouse over the edit menu
                                        // the file menu will dismiss.

                                        return false;
                                    }
                                    else if (!IsChildOrSameWindow(ActiveHwnd, new HandleRef(null, m.HWnd)))
                                    {
                                        // DONT EAT mouse message.
                                        // the mouse message is from another toplevel HWND.
                                        return false;
                                    }
                                    // EAT mouse message
                                    // the HWND is
                                    //      not part of the active toolstrip
                                    //      not the toplevel toolstrip (e.g. MenuStrip).
                                    //      not parented to the toplevel toolstrip (e.g a combo box on a menu strip).
                                    return true;
                                }
                            }
                            break;
                        case WindowMessages.WM_LBUTTONDOWN:
                        case WindowMessages.WM_RBUTTONDOWN:
                        case WindowMessages.WM_MBUTTONDOWN:
                            //
                            // When a mouse button is pressed, we should determine if it is within the client coordinates
                            // of the active dropdown.  If not, we should dismiss it.
                            //
                            ProcessMouseButtonPressed(m.HWnd,
                                /*x=*/NativeMethods.Util.SignedLOWORD(m.LParam),
                                /*y=*/NativeMethods.Util.SignedHIWORD(m.LParam));

                            break;
                        case WindowMessages.WM_NCLBUTTONDOWN:
                        case WindowMessages.WM_NCRBUTTONDOWN:
                        case WindowMessages.WM_NCMBUTTONDOWN:
                            //
                            // When a mouse button is pressed, we should determine if it is within the client coordinates
                            // of the active dropdown.  If not, we should dismiss it.
                            //
                            ProcessMouseButtonPressed(/*nc messages are in screen coords*/IntPtr.Zero,
                                /*x=*/NativeMethods.Util.SignedLOWORD(m.LParam),
                                /*y=*/NativeMethods.Util.SignedHIWORD(m.LParam));
                            break;

                        case WindowMessages.WM_KEYDOWN:
                        case WindowMessages.WM_KEYUP:
                        case WindowMessages.WM_CHAR:
                        case WindowMessages.WM_DEADCHAR:
                        case WindowMessages.WM_SYSKEYDOWN:
                        case WindowMessages.WM_SYSKEYUP:
                        case WindowMessages.WM_SYSCHAR:
                        case WindowMessages.WM_SYSDEADCHAR:

                            if (!activeToolStrip.ContainsFocus)
                            {
                                Debug.WriteLineIf(ToolStrip.SnapFocusDebug.TraceVerbose, "[ModalMenuFilter.PreFilterMessage] MODIFYING Keyboard message " + m.ToString());

                                // route all keyboard messages to the active dropdown.
                                m.HWnd = activeToolStrip.Handle;
                            }
                            else
                            {
                                Debug.WriteLineIf(ToolStrip.SnapFocusDebug.TraceVerbose, "[ModalMenuFilter.PreFilterMessage] got Keyboard message " + m.ToString());
                            }
                            break;

                    }
                }

                return false;
            }

            private class HostedWindowsFormsMessageHook
            {
                private IntPtr messageHookHandle = IntPtr.Zero;
                private bool isHooked = false;
                private NativeMethods.HookProc hookProc;

                public HostedWindowsFormsMessageHook()
                {

#if DEBUG
                    try
                    {
                        callingStack = Environment.StackTrace;
                    }
                    catch (Security.SecurityException)
                    {
                    }
#endif
                }

#if DEBUG
                readonly string callingStack;
                ~HostedWindowsFormsMessageHook()
                {
                    Debug.Assert(messageHookHandle == IntPtr.Zero, "Finalizing an active mouse hook.  This will crash the process.  Calling stack: " + callingStack);
                }
#endif

                public bool HookMessages
                {
                    get
                    {
                        return messageHookHandle != IntPtr.Zero;
                    }
                    set
                    {
                        if (value)
                        {
                            InstallMessageHook();
                        }
                        else
                        {
                            UninstallMessageHook();
                        }
                    }
                }

                private void InstallMessageHook()
                {
                    lock (this)
                    {
                        if (messageHookHandle != IntPtr.Zero)
                        {
                            return;
                        }

                        hookProc = new NativeMethods.HookProc(MessageHookProc);

                        messageHookHandle = UnsafeNativeMethods.SetWindowsHookEx(NativeMethods.WH_GETMESSAGE,
                                                                   hookProc,
                                                                   new HandleRef(null, IntPtr.Zero),
                                                                   SafeNativeMethods.GetCurrentThreadId());

                        if (messageHookHandle != IntPtr.Zero)
                        {
                            isHooked = true;
                        }
                        Debug.Assert(messageHookHandle != IntPtr.Zero, "Failed to install mouse hook");
                    }
                }

                private unsafe IntPtr MessageHookProc(int nCode, IntPtr wparam, IntPtr lparam)
                {
                    if (nCode == NativeMethods.HC_ACTION)
                    {
                        if (isHooked && (int)wparam == NativeMethods.PM_REMOVE /*only process GetMessage, not PeekMessage*/)
                        {
                            // only process messages we've pulled off the queue
                            NativeMethods.MSG* msg = (NativeMethods.MSG*)lparam;
                            if (msg != null)
                            {
                                //Debug.WriteLine("Got " + Message.Create(msg->hwnd, msg->message, wparam, lparam).ToString());
                                // call pretranslate on the message - this should execute
                                // the message filters and preprocess message.
                                if (Application.ThreadContext.FromCurrent().PreTranslateMessage(ref *msg))
                                {
                                    msg->message = WindowMessages.WM_NULL;
                                }
                            }
                        }
                    }

                    return UnsafeNativeMethods.CallNextHookEx(new HandleRef(this, messageHookHandle), nCode, wparam, lparam);
                }

                private void UninstallMessageHook()
                {
                    lock (this)
                    {
                        if (messageHookHandle != IntPtr.Zero)
                        {
                            UnsafeNativeMethods.UnhookWindowsHookEx(new HandleRef(this, messageHookHandle));
                            hookProc = null;
                            messageHookHandle = IntPtr.Zero;
                            isHooked = false;
                        }
                    }
                }

            }

        }

        internal static bool ShowMenuFocusCues
        {
            get
            {
                if (!DisplayInformation.MenuAccessKeysUnderlined)
                {
                    return ModalMenuFilter.Instance.ShowUnderlines;
                }
                return true;
            }
        }

        /// <summary> determines if the key combination is valid for a shortcut.
        ///  must have a modifier key + a regular key.
        /// </summary>
        public static bool IsValidShortcut(Keys shortcut)
        {
            // should have a key and one or more modifiers.

            Keys keyCode = (Keys)(shortcut & Keys.KeyCode);
            Keys modifiers = (Keys)(shortcut & Keys.Modifiers);

            if (shortcut == Keys.None)
            {
                return false;
            }
            else if ((keyCode == Keys.Delete) || (keyCode == Keys.Insert))
            {
                return true;
            }
            else if (((int)keyCode >= (int)Keys.F1) && ((int)keyCode <= (int)Keys.F24))
            {
                // function keys by themselves are valid
                return true;
            }
            else if ((keyCode != Keys.None) && (modifiers != Keys.None))
            {
                switch (keyCode)
                {
                    case Keys.Menu:
                    case Keys.ControlKey:
                    case Keys.ShiftKey:
                        // shift, control and alt arent valid on their own.
                        return false;
                    default:
                        if (modifiers == Keys.Shift)
                        {
                            // shift + somekey isnt a valid modifier either
                            return false;
                        }
                        return true;
                }
            }
            // has to have a valid keycode and valid modifier.
            return false;
        }

        internal static bool IsMenuKey(Keys keyData)
        {
            Keys keyCode = keyData & Keys.KeyCode;
            return (Keys.Menu == keyCode || Keys.F10 == keyCode);
        }

        public static bool IsShortcutDefined(Keys shortcut)
        {
            for (int i = 0; i < ToolStrips.Count; i++)
            {
                if ((ToolStrips[i] is ToolStrip t) && t.Shortcuts.Contains(shortcut))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary> this function is called for toplevel controls to process shortcuts.
        ///  this function should be called from the topmost container control only.
        /// </summary>
        internal static bool ProcessCmdKey(ref Message m, Keys keyData)
        {
            Debug.WriteLineIf(Control.s_controlKeyboardRouting.TraceVerbose, "ToolStripManager.ProcessCmdKey - processing: [" + keyData.ToString() + "]");
            if (ToolStripManager.IsValidShortcut(keyData))
            {
                // if we're at the toplevel, check the toolstrips for matching shortcuts.
                // Win32 menus are handled in Form.ProcessCmdKey, but we cant guarantee that
                // toolstrips will be hosted in a form.  ToolStrips have a hash of shortcuts
                // per container, so this should hopefully be a quick search.
                Debug.WriteLineIf(Control.s_controlKeyboardRouting.TraceVerbose, "ToolStripManager.ProcessCmdKey - IsValidShortcut: [" + keyData.ToString() + "]");

                return ToolStripManager.ProcessShortcut(ref m, keyData);
            }
            if (m.Msg == WindowMessages.WM_SYSKEYDOWN)
            {
                Debug.WriteLineIf(Control.s_controlKeyboardRouting.TraceVerbose, "ToolStripManager.ProcessCmdKey - Checking if it's a menu key: [" + keyData.ToString() + "]");
                ToolStripManager.ModalMenuFilter.ProcessMenuKeyDown(ref m);
            }

            return false;
        }

        /// <summary> we're halfway to an accellerator table system here.
        ///  each toolstrip maintains a hash of the current shortcuts its using.
        ///  this way the search only takes O(number of toolstrips in the thread)
        ///  ToolStripMenuItem pushes itself into this table as the owner is set or the shortcut changes.
        /// </summary>
        internal static bool ProcessShortcut(ref Message m, Keys shortcut)
        {
            if (!IsThreadUsingToolStrips())
            {
                return false;
            }
            Control activeControl = Control.FromChildHandle(m.HWnd);
            Control activeControlInChain = activeControl;

            if (activeControlInChain != null && IsValidShortcut(shortcut))
            {
                Debug.WriteLineIf(Control.s_controlKeyboardRouting.TraceVerbose, "ToolStripManager.ProcessShortcut - processing: [" + shortcut.ToString() + "]");

                // start from the focused control and work your way up the parent chain
                do
                {
                    //  check the context menu strip first.
                    if (activeControlInChain.ContextMenuStrip != null)
                    {
                        if (activeControlInChain.ContextMenuStrip.Shortcuts.ContainsKey(shortcut))
                        {
                            ToolStripMenuItem item = activeControlInChain.ContextMenuStrip.Shortcuts[shortcut] as ToolStripMenuItem;
                            if (item.ProcessCmdKey(ref m, shortcut))
                            {
                                Debug.WriteLineIf(Control.s_controlKeyboardRouting.TraceVerbose, "ToolStripManager.ProcessShortcut - found item on context menu: [" + item.ToString() + "]");
                                return true;
                            }
                        }
                    }
                    activeControlInChain = activeControlInChain.ParentInternal;
                } while (activeControlInChain != null);

                if (activeControlInChain != null)
                {
                    // the keystroke may applies to one of our parents...
                    // a WM_CONTEXTMENU message bubbles up to the parent control
                    activeControl = activeControlInChain;
                }

                bool retVal = false;
                bool needsPrune = false;

                // now search the toolstrips
                for (int i = 0; i < ToolStrips.Count; i++)
                {
                    bool isAssociatedContextMenu = false;
                    bool isDoublyAssignedContextMenuStrip = false;

                    if (!(ToolStrips[i] is ToolStrip toolStrip))
                    {
                        // consider prune tree...
                        needsPrune = true;
                        continue;
                    }
                    else if (activeControl != null && toolStrip == activeControl.ContextMenuStrip)
                    {
                        continue;
                    }
                    else if (toolStrip.Shortcuts.ContainsKey(shortcut))
                    {

                        if (toolStrip.IsDropDown)
                        {
                            // we dont want to process someone else's context menu (e.g. button1 and button2 have context menus)
                            // button2's context menu should not be processed if button1 is the one we're processing.

                            ToolStripDropDown dropDown = toolStrip as ToolStripDropDown;

                            // If a context menu is re-used between the main menu and the
                            // and some other control's context menu, we should go ahead and evaluate it.

                            if (dropDown.GetFirstDropDown() is ContextMenuStrip toplevelContextMenu)
                            {
                                isDoublyAssignedContextMenuStrip = toplevelContextMenu.IsAssignedToDropDownItem;
                                if (!isDoublyAssignedContextMenuStrip)
                                {
                                    if (toplevelContextMenu != activeControl.ContextMenuStrip)
                                    {
                                        // the toplevel context menu is NOT the same as the active control's context menu.
                                        continue;
                                    }
                                    else
                                    {
                                        isAssociatedContextMenu = true;
                                    }
                                }
                            }
                            // else it's not a child of a context menu
                        }

                        bool rootWindowsMatch = false;

                        if (!isAssociatedContextMenu)
                        {
                            // make sure that were processing shortcuts for the correct window.
                            // since the shortcut lookup is faster than this check we've postponed this to the last
                            // possible moment.
                            ToolStrip topMostToolStrip = toolStrip.GetToplevelOwnerToolStrip();
                            if (topMostToolStrip != null && activeControl != null)
                            {
                                HandleRef rootWindowOfToolStrip = WindowsFormsUtils.GetRootHWnd(topMostToolStrip);
                                HandleRef rootWindowOfControl = WindowsFormsUtils.GetRootHWnd(activeControl);
                                rootWindowsMatch = (rootWindowOfToolStrip.Handle == rootWindowOfControl.Handle);

                                if (rootWindowsMatch)
                                {
                                    // Double check this is not an MDIContainer type situation...
                                    if (Control.FromHandle(rootWindowOfControl.Handle) is Form mainForm && mainForm.IsMdiContainer)
                                    {
                                        Form toolStripForm = topMostToolStrip.FindForm();
                                        if (toolStripForm != mainForm && toolStripForm != null)
                                        {
                                            // we should only process shortcuts of the ActiveMDIChild or the Main Form.
                                            rootWindowsMatch = (toolStripForm == mainForm.ActiveMdiChildInternal);
                                        }
                                    }

                                }
                            }
                        }
                        if (isAssociatedContextMenu || rootWindowsMatch || isDoublyAssignedContextMenuStrip)
                        {
                            if (toolStrip.Shortcuts[shortcut] is ToolStripMenuItem item)
                            {
                                if (item.ProcessCmdKey(ref m, shortcut))
                                {
                                    Debug.WriteLineIf(Control.s_controlKeyboardRouting.TraceVerbose, "ToolStripManager.ProcessShortcut - found item on toolstrip: [" + item.ToString() + "]");
                                    retVal = true;
                                    break;
                                }
                            }
                        }
                    }
                }
                if (needsPrune)
                {
                    PruneToolStripList();
                }
                return retVal;

            }
            return false;

        }

        /// <summary> this function handles when Alt is pressed.
        ///  if it finds a menustrip to select, it returns true,
        ///  if it doesnt it returns false.
        ///  if it finds a win32 menu is already associated with the control it bails, returning false.
        /// </summary>
        internal static bool ProcessMenuKey(ref Message m)
        {
            Debug.WriteLineIf(Control.s_controlKeyboardRouting.TraceVerbose, "ToolStripManager.ProcessMenuKey: [" + m.ToString() + "]");
            if (!IsThreadUsingToolStrips())
            {
                return false;
            }
            // recievedMenuKeyUp = true;

            Debug.WriteLineIf(ToolStrip.SnapFocusDebug.TraceVerbose, "[ProcessMenuKey] Determining whether we should send focus to MenuStrip");

            Keys keyData = (Keys)(int)m.LParam;

            // search for our menu to work with
            Control intendedControl = Control.FromHandle(m.HWnd);
            Control toplevelControl = null;

            MenuStrip menuStripToActivate = null;
            if (intendedControl != null)
            {
                // search for a menustrip to select.
                toplevelControl = intendedControl.TopLevelControlInternal;
                if (toplevelControl != null)
                {
                    IntPtr hMenu = UnsafeNativeMethods.GetMenu(new HandleRef(toplevelControl, toplevelControl.Handle));
                    if (hMenu == IntPtr.Zero)
                    {
                        // only activate the menu if there's no win32 menu.  Win32 menus trump menustrips.
                        menuStripToActivate = GetMainMenuStrip(toplevelControl);
                    }
                    Debug.WriteLineIf(ToolStrip.SnapFocusDebug.TraceVerbose, string.Format(CultureInfo.CurrentCulture, "[ProcessMenuKey] MenuStripToActivate is: {0}", menuStripToActivate));

                }
            }
            // the data that comes into the LParam is the ASCII code, not the VK_* code.
            // we need to compare against char instead.
            if ((char)keyData == ' ')
            { // dont process system menu
                ModalMenuFilter.MenuKeyToggle = false;
            }
            else if ((char)keyData == '-')
            {
                // deal with MDI system menu
                if (toplevelControl is Form mdiChild && mdiChild.IsMdiChild)
                {
                    if (mdiChild.WindowState == FormWindowState.Maximized)
                    {
                        ModalMenuFilter.MenuKeyToggle = false;
                    }
                }
            }
            else
            {
                // this is the same as Control.ModifierKeys - but we save two p/invokes.
                if (UnsafeNativeMethods.GetKeyState((int)Keys.ShiftKey) < 0 && (keyData == Keys.None))
                {
                    // If it's Shift+F10 and we're already InMenuMode, then we
                    // need to cancel this message, otherwise we'll enter the native modal menu loop.
                    Debug.WriteLineIf(ToolStrip.SnapFocusDebug.TraceVerbose, "[ProcessMenuKey] DETECTED SHIFT+F10" + keyData.ToString());
                    return ToolStripManager.ModalMenuFilter.InMenuMode;
                }
                else
                {
                    if (menuStripToActivate != null && !ModalMenuFilter.MenuKeyToggle)
                    {
                        Debug.WriteLineIf(ToolStrip.SnapFocusDebug.TraceVerbose, "[ProcessMenuKey] attempting to set focus to menustrip");

                        // if we've alt-tabbed away dont snap/restore focus.
                        HandleRef topmostParentOfMenu = WindowsFormsUtils.GetRootHWnd(menuStripToActivate);
                        IntPtr foregroundWindow = UnsafeNativeMethods.GetForegroundWindow();

                        if (topmostParentOfMenu.Handle == foregroundWindow)
                        {
                            Debug.WriteLineIf(ToolStrip.SnapFocusDebug.TraceVerbose, "[ProcessMenuKey] ToolStripManager call MenuStrip.OnMenuKey");
                            return menuStripToActivate.OnMenuKey();
                        }
                    }
                    else if (menuStripToActivate != null)
                    {
                        Debug.WriteLineIf(ToolStrip.SnapFocusDebug.TraceVerbose, "[ProcessMenuKey] Resetting MenuKeyToggle");
                        ModalMenuFilter.MenuKeyToggle = false;
                        return true;
                    }
                }

            }
            return false;
        }

        internal static MenuStrip GetMainMenuStrip(Control control)
        {
            if (control == null)
            {
                Debug.Fail("why are we passing null to GetMainMenuStrip?");
                return null;
            }

            // look for a particular main menu strip to be set.
            Form mainForm = control.FindForm();
            if (mainForm != null && mainForm.MainMenuStrip != null)
            {
                return mainForm.MainMenuStrip;
            }

            // if not found go through the entire collection.
            return GetFirstMenuStripRecursive(control.Controls);
        }

        private static MenuStrip GetFirstMenuStripRecursive(Control.ControlCollection controlsToLookIn)
        {
            try
            {
                // Perform breadth first search - as it's likely people will want controls belonging
                // to the same parent close to each other.

                for (int i = 0; i < controlsToLookIn.Count; i++)
                {
                    if (controlsToLookIn[i] == null)
                    {
                        continue;
                    }
                    if (controlsToLookIn[i] is MenuStrip)
                    {
                        return controlsToLookIn[i] as MenuStrip;
                    }
                }

                // Recursive search for controls in child collections.

                for (int i = 0; i < controlsToLookIn.Count; i++)
                {
                    if (controlsToLookIn[i] == null)
                    {
                        continue;
                    }

                    if ((controlsToLookIn[i].Controls != null) && controlsToLookIn[i].Controls.Count > 0)
                    {
                        // if it has a valid child collecion, append those results to our collection
                        MenuStrip menuStrip = GetFirstMenuStripRecursive(controlsToLookIn[i].Controls);
                        if (menuStrip != null)
                        {
                            return menuStrip;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                // Make sure we deal with non-critical failures gracefully.
                if (ClientUtils.IsCriticalException(e))
                {
                    throw;
                }
            }
            return null;
        }

        #endregion MenuKeyAndShortcutProcessing

        ///
        ///  ToolStripManager MenuMerging functions
        ///
        #region MenuMerging

        private static ToolStripItem FindMatch(ToolStripItem source, ToolStripItemCollection destinationItems)
        {
            // based on MergeAction:
            // Append, return the last sibling
            ToolStripItem result = null;
            if (source != null)
            {
                for (int i = 0; i < destinationItems.Count; i++)
                {
                    ToolStripItem candidateItem = destinationItems[i];
                    // using SafeCompareKeys so we use the same heuristics as keyed collections.
                    if (WindowsFormsUtils.SafeCompareStrings(source.Text, candidateItem.Text, true))
                    {
                        result = candidateItem;
                        break; // we found it
                    }
                }

                if (result == null && source.MergeIndex > -1 && source.MergeIndex < destinationItems.Count)
                {
                    result = destinationItems[source.MergeIndex];
                }
            }
            return result;
        }

        internal static ArrayList FindMergeableToolStrips(ContainerControl container)
        {
            ArrayList result = new ArrayList();
            if (container != null)
            {
                for (int i = 0; i < ToolStrips.Count; i++)
                {
                    ToolStrip candidateTS = (ToolStrip)ToolStrips[i];
                    //if(candidateTS != null) {
                    //    Debug.WriteLine("candidate TS: " + candidateTS.Name + " | " + candidateTS.AllowMerge + " | " + (candidateTS.Parent == null ?  "null" : candidateTS.Parent.Name) +" | " + container.Name);
                    //}
                    //Debug.WriteLine(candidateTS == null ? "null" : "not null");
                    if (candidateTS != null && candidateTS.AllowMerge && container == candidateTS.FindForm())
                    {
                        //Debug.WriteLine("adding");
                        result.Add(candidateTS);
                    }
                }
            }
            result.Sort(new ToolStripCustomIComparer()); //we sort them from more specific to less specific
            return result;
        }

        private static bool IsSpecialMDIStrip(ToolStrip toolStrip)
        {
            return (toolStrip is MdiControlStrip || toolStrip is MdiWindowListStrip);
        }

        /// <summary>
        ///  merge two toolstrips
        /// </summary>
        public static bool Merge(ToolStrip sourceToolStrip, ToolStrip targetToolStrip)
        {
            // check arguments
            if (sourceToolStrip == null)
            {
                throw new ArgumentNullException(nameof(sourceToolStrip));
            }
            if (targetToolStrip == null)
            {
                throw new ArgumentNullException(nameof(targetToolStrip));
            }
            if (targetToolStrip == sourceToolStrip)
            {
                throw new ArgumentException(SR.ToolStripMergeImpossibleIdentical);
            }

            // we only do this if the source and target toolstrips are the same
            bool canMerge = IsSpecialMDIStrip(sourceToolStrip);
            canMerge = (canMerge || (sourceToolStrip.AllowMerge &&
                                      targetToolStrip.AllowMerge &&
                                      (sourceToolStrip.GetType().IsAssignableFrom(targetToolStrip.GetType()) || targetToolStrip.GetType().IsAssignableFrom(sourceToolStrip.GetType()))
                                    )
                        );
            MergeHistory mergeHistory = null;
            if (canMerge)
            {
                //Debug.WriteLine("Begin merge between src: " + sourceToolStrip.Name + " and target: " + targetToolStrip.Name);
                Debug.Indent();
                mergeHistory = new MergeHistory(sourceToolStrip);

                int originalCount = sourceToolStrip.Items.Count;

                if (originalCount > 0)
                {
                    sourceToolStrip.SuspendLayout();
                    targetToolStrip.SuspendLayout();
                    try
                    {
                        int lastCount = originalCount;

                        // 2. do the actual merging logic
                        for (int i = 0, itemToLookAt = 0; i < originalCount; i++)
                        {
                            ToolStripItem item = sourceToolStrip.Items[itemToLookAt];
                            //Debug.WriteLine("doing the recursive merge for item " + item.Text);
                            MergeRecursive(item, targetToolStrip.Items, mergeHistory.MergeHistoryItemsStack);

                            int numberOfItemsMerged = lastCount - sourceToolStrip.Items.Count;
                            itemToLookAt = (numberOfItemsMerged > 0) ? itemToLookAt : itemToLookAt + 1;
                            lastCount = sourceToolStrip.Items.Count;
                        }
                    }
                    finally
                    {
                        Debug.Unindent();
                        sourceToolStrip.ResumeLayout();
                        targetToolStrip.ResumeLayout();
                    }
                    //Debug.WriteLine("pusing mergehistory for toolstrip " + sourceToolStrip.Name + " in target toolstrip MergeHistoryStack property");
                    if (mergeHistory.MergeHistoryItemsStack.Count > 0)
                    {
                        // only push this on the stack if we actually did something
                        targetToolStrip.MergeHistoryStack.Push(mergeHistory);
                    }
                }
            }
            bool result = false;
            if (mergeHistory != null && mergeHistory.MergeHistoryItemsStack.Count > 0)
            {
                result = true; // we did merge something
            }
            return result;
        }

        private static void MergeRecursive(ToolStripItem source, ToolStripItemCollection destinationItems, Stack<MergeHistoryItem> history)
        {
            Debug.Indent();
            MergeHistoryItem maction;
            switch (source.MergeAction)
            {
                case MergeAction.MatchOnly:
                case MergeAction.Replace:
                case MergeAction.Remove:
                    ToolStripItem item = FindMatch(source, destinationItems);
                    if (item != null)
                    {
                        switch (source.MergeAction)
                        {
                            case MergeAction.MatchOnly:
                                if (item is ToolStripDropDownItem tsddownDest && source is ToolStripDropDownItem tsddownSrc && tsddownSrc.DropDownItems.Count != 0)
                                {

                                    int originalCount = tsddownSrc.DropDownItems.Count;

                                    if (originalCount > 0)
                                    {
                                        int lastCount = originalCount;
                                        tsddownSrc.DropDown.SuspendLayout();

                                        try
                                        {
                                            // the act of walking through this collection removes items from
                                            // the dropdown.
                                            for (int i = 0, itemToLookAt = 0; i < originalCount; i++)
                                            {

                                                MergeRecursive(tsddownSrc.DropDownItems[itemToLookAt], tsddownDest.DropDownItems, history);

                                                int numberOfItemsMerged = lastCount - tsddownSrc.DropDownItems.Count;
                                                itemToLookAt = (numberOfItemsMerged > 0) ? itemToLookAt : itemToLookAt + 1;
                                                lastCount = tsddownSrc.DropDownItems.Count;
                                            }
                                        }
                                        finally
                                        {
                                            tsddownSrc.DropDown.ResumeLayout();
                                        }
                                    }
                                }
                                break;
                            case MergeAction.Replace:
                            case MergeAction.Remove:
                                //Debug.WriteLine("remove");
                                maction = new MergeHistoryItem(MergeAction.Insert)
                                {
                                    TargetItem = item
                                };
                                int indexOfDestinationItem = destinationItems.IndexOf(item);
                                destinationItems.RemoveAt(indexOfDestinationItem);
                                maction.Index = indexOfDestinationItem;
                                maction.IndexCollection = destinationItems;
                                maction.TargetItem = item;
                                history.Push(maction);
                                //Debug.WriteLine(maction.ToString());
                                if (source.MergeAction == MergeAction.Replace)
                                {
                                    //Debug.WriteLine("replace");
                                    //ToolStripItem clonedItem = source.Clone();
                                    maction = new MergeHistoryItem(MergeAction.Remove)
                                    {
                                        PreviousIndexCollection = source.Owner.Items
                                    };
                                    maction.PreviousIndex = maction.PreviousIndexCollection.IndexOf(source);
                                    maction.TargetItem = source;
                                    destinationItems.Insert(indexOfDestinationItem, source);
                                    maction.Index = indexOfDestinationItem;
                                    maction.IndexCollection = destinationItems;
                                    history.Push(maction);
                                    //Debug.WriteLine(maction.ToString());
                                }
                                break;
                        }
                    }
                    break;
                case MergeAction.Insert:
                    if (source.MergeIndex > -1)
                    {
                        maction = new MergeHistoryItem(MergeAction.Remove)
                        {
                            PreviousIndexCollection = source.Owner.Items
                        };
                        maction.PreviousIndex = maction.PreviousIndexCollection.IndexOf(source);
                        maction.TargetItem = source;
                        int insertIndex = Math.Min(destinationItems.Count, source.MergeIndex);
                        destinationItems.Insert(insertIndex, source);
                        maction.IndexCollection = destinationItems;
                        maction.Index = insertIndex;
                        history.Push(maction);
                        //Debug.WriteLine(maction.ToString());
                    }
                    break;
                case MergeAction.Append:
                    maction = new MergeHistoryItem(MergeAction.Remove)
                    {
                        PreviousIndexCollection = source.Owner.Items
                    };
                    maction.PreviousIndex = maction.PreviousIndexCollection.IndexOf(source);
                    maction.TargetItem = source;
                    int index = destinationItems.Add(source);
                    maction.Index = index;
                    maction.IndexCollection = destinationItems;
                    history.Push(maction);
                    //Debug.WriteLine(maction.ToString());
                    break;
            }
            Debug.Unindent();
        }

        /// <summary>
        ///  merge two toolstrips
        /// </summary>
        public static bool Merge(ToolStrip sourceToolStrip, string targetName)
        {
            if (sourceToolStrip == null)
            {
                throw new ArgumentNullException(nameof(sourceToolStrip));
            }
            if (targetName == null)
            {
                throw new ArgumentNullException(nameof(targetName));
            }

            ToolStrip target = FindToolStrip(targetName);
            if (target == null)
            {
                return false;
            }
            else
            {
                return Merge(sourceToolStrip, target);
            }
        }

        /// <summary>
        ///  doesn't do a null check on source... if it's null we unmerge everything
        /// </summary>
        internal static bool RevertMergeInternal(ToolStrip targetToolStrip, ToolStrip sourceToolStrip, bool revertMDIControls)
        {
            bool result = false;
            if (targetToolStrip == null)
            {
                throw new ArgumentNullException(nameof(targetToolStrip));
            }
            if (targetToolStrip == sourceToolStrip)
            {
                throw new ArgumentException(SR.ToolStripMergeImpossibleIdentical);
            }
            bool foundToolStrip = false;

            if (sourceToolStrip != null)
            {
                // we have a specific toolstrip to pull out.

                // make sure the sourceToolStrip is even merged into the targetToolStrip
                foreach (MergeHistory history in targetToolStrip.MergeHistoryStack)
                {
                    foundToolStrip = (history.MergedToolStrip == sourceToolStrip);
                    if (foundToolStrip)
                    {
                        break;
                    }
                }

                // PERF: if we dont have the toolstrip in our merge history, bail.
                if (!foundToolStrip)
                {
                    //Debug.WriteLine("source toolstrip not contained within target " + history.MergedToolStrip.Name);
                    return false;
                }
            }

            if (sourceToolStrip != null)
            {
                sourceToolStrip.SuspendLayout();
            }
            targetToolStrip.SuspendLayout();

            try
            {
                //Debug.WriteLine("Reverting merge, playing back history for all merged toolstrip ");
                Stack<ToolStrip> reApply = new Stack<ToolStrip>();
                foundToolStrip = false;
                while (targetToolStrip.MergeHistoryStack.Count > 0 && !foundToolStrip)
                {
                    result = true; // we unmerge something...
                    MergeHistory history = targetToolStrip.MergeHistoryStack.Pop();
                    if (history.MergedToolStrip == sourceToolStrip)
                    {
                        foundToolStrip = true;
                    }
                    else if (!revertMDIControls && sourceToolStrip == null)
                    {
                        // Calling ToolStripManager.RevertMerge should not pull out MDIControlStrip && MDIWindowListStrip.
                        if (IsSpecialMDIStrip(history.MergedToolStrip))
                        {
                            reApply.Push(history.MergedToolStrip);
                        }
                    }
                    else
                    {
                        reApply.Push(history.MergedToolStrip);
                    }
                    //Debug.WriteLine("unmerging " + history.MergedToolStrip.Name);
                    Debug.Indent();
                    while (history.MergeHistoryItemsStack.Count > 0)
                    {
                        MergeHistoryItem historyItem = history.MergeHistoryItemsStack.Pop();
                        switch (historyItem.MergeAction)
                        {
                            case MergeAction.Remove:
                                historyItem.IndexCollection.Remove(historyItem.TargetItem);
                                // put it back
                                historyItem.PreviousIndexCollection.Insert(Math.Min(historyItem.PreviousIndex, historyItem.PreviousIndexCollection.Count), historyItem.TargetItem);
                                break;
                            case MergeAction.Insert:
                                historyItem.IndexCollection.Insert(Math.Min(historyItem.Index, historyItem.IndexCollection.Count), historyItem.TargetItem);
                                // no need to put it back, inserting it in a new collection, moved it at the correct location
                                break;
                        }
                    }
                    Debug.Unindent();
                }

                // re-apply the merges of the toolstrips we had to unmerge first.
                while (reApply.Count > 0)
                {
                    ToolStrip mergeAgain = reApply.Pop();
                    Merge(mergeAgain, targetToolStrip);
                }
            }
            finally
            {
                if (sourceToolStrip != null)
                {
                    sourceToolStrip.ResumeLayout();
                }
                targetToolStrip.ResumeLayout();
            }

            return result;
            //ToolStripMergeNode.SynchronizeFromToolStripMergeNode(targetToolStrip.Items, targetToolStrip.MergeItems);
        }

        /// <summary>
        ///  unmerge two toolstrips
        /// </summary>
        public static bool RevertMerge(ToolStrip targetToolStrip)
        {
            return RevertMergeInternal(targetToolStrip, null, /*revertMDIControls*/false);
        }

        /// <summary>
        ///  unmerge two toolstrips
        /// </summary>
        public static bool RevertMerge(ToolStrip targetToolStrip, ToolStrip sourceToolStrip)
        {
            if (sourceToolStrip == null)
            {
                throw new ArgumentNullException(nameof(sourceToolStrip));
            }
            return RevertMergeInternal(targetToolStrip, sourceToolStrip, /*revertMDIControls*/false);
        }

        /// <summary>
        ///  unmerge two toolstrips
        /// </summary>
        public static bool RevertMerge(string targetName)
        {
            ToolStrip target = FindToolStrip(targetName);
            if (target == null)
            {
                return false;
            }
            else
            {
                return RevertMerge(target);
            }
        }

        #endregion MenuMerging

    }

    internal class ToolStripCustomIComparer : IComparer
    {
        int IComparer.Compare(object x, object y)
        {
            if (x.GetType() == y.GetType())
            {
                return 0; // same type
            }
            if (x.GetType().IsAssignableFrom(y.GetType()))
            {
                return 1;
            }
            if (y.GetType().IsAssignableFrom(x.GetType()))
            {
                return -1;
            }
            return 0; // not the same type, not in each other inheritance chain
        }
    }

    internal class MergeHistory
    {
        private Stack<MergeHistoryItem> mergeHistoryItemsStack;
        private readonly ToolStrip mergedToolStrip;

        public MergeHistory(ToolStrip mergedToolStrip)
        {
            this.mergedToolStrip = mergedToolStrip;
        }
        public Stack<MergeHistoryItem> MergeHistoryItemsStack
        {
            get
            {
                if (mergeHistoryItemsStack == null)
                {
                    mergeHistoryItemsStack = new Stack<MergeHistoryItem>();
                }
                return mergeHistoryItemsStack;
            }
        }
        public ToolStrip MergedToolStrip
        {
            get
            {
                return mergedToolStrip;
            }
        }
    }

    internal class MergeHistoryItem
    {
        private readonly MergeAction mergeAction;
        private ToolStripItem targetItem;
        private int index = -1;
        private int previousIndex = -1;
        private ToolStripItemCollection previousIndexCollection;
        private ToolStripItemCollection indexCollection;

        public MergeHistoryItem(MergeAction mergeAction)
        {
            this.mergeAction = mergeAction;
        }
        public MergeAction MergeAction
        {
            get
            {
                return mergeAction;
            }
        }

        public ToolStripItem TargetItem
        {
            get
            {
                return targetItem;
            }
            set
            {
                targetItem = value;
            }
        }

        public int Index
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

        public int PreviousIndex
        {
            get
            {
                return previousIndex;
            }
            set
            {
                previousIndex = value;
            }
        }

        public ToolStripItemCollection PreviousIndexCollection
        {
            get
            {
                return previousIndexCollection;
            }
            set
            {
                previousIndexCollection = value;
            }
        }

        public ToolStripItemCollection IndexCollection
        {
            get
            {
                return indexCollection;
            }
            set
            {
                indexCollection = value;
            }
        }

        public override string ToString()
        {
            return "MergeAction: " + mergeAction.ToString() + " | TargetItem: " + (TargetItem == null ? "null" : TargetItem.Text) + " Index: " + index.ToString(CultureInfo.CurrentCulture);
        }
    }
}
