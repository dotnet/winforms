// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Globalization;
using Microsoft.Win32;
using static Interop;

namespace System.Windows.Forms
{
    public static partial class ToolStripManager
    {
        // WARNING: ThreadStatic initialization happens only on the first thread at class CTOR time.
        // use InitializeThread mechanism to initialize ThreadStatic members
        [ThreadStatic]
        private static WeakRefCollection t_toolStripWeakArrayList;

        [ThreadStatic]
        private static WeakRefCollection t_toolStripPanelWeakArrayList;

        [ThreadStatic]
        private static bool t_initialized;

        private static Font s_defaultFont;
        private static ConcurrentDictionary<int, Font> s_defaultFontCache = new ConcurrentDictionary<int, Font>();

        // WARNING: When subscribing to static event handlers - make sure you unhook from them
        // otherwise you can leak USER objects on process shutdown.
        // Consider: use WeakRefCollection
        [ThreadStatic]
        private static Delegate[] t_staticEventHandlers;
        private const int StaticEventDefaultRendererChanged = 0;
        private const int StaticEventCount = 1;

        private static readonly object s_internalSyncObject = new object();

        private static void InitializeThread()
        {
            if (!t_initialized)
            {
                t_initialized = true;
                t_currentRendererType = s_professionalRendererType;
            }
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
                    if (s_defaultFontCache.TryGetValue(dpi, out retFont) == false || retFont is null)
                    {
                        // Default to menu font
                        sysFont = SystemInformation.GetMenuFontForDpi(dpi);
                        if (sysFont != null)
                        {
                            // Ensure font is in pixels so it displays properly in the property grid at design time.
                            if (sysFont.Unit != GraphicsUnit.Point)
                            {
                                retFont = ControlPaint.FontInPoints(sysFont);
                                sysFont.Dispose();
                            }
                            else
                            {
                                retFont = sysFont;
                            }
                            s_defaultFontCache[dpi] = retFont;
                        }
                    }
                    return retFont;
                }
                else
                {
                    // Threadsafe local reference
                    Font retFont = s_defaultFont;

                    if (retFont is null)
                    {
                        lock (s_internalSyncObject)
                        {
                            // Double check the defaultFont after the lock.
                            retFont = s_defaultFont;

                            if (retFont is null)
                            {
                                // Default to menu font
                                sysFont = SystemFonts.MenuFont;
                                if (sysFont is null)
                                {
                                    // ...or to control font if menu font unavailable
                                    sysFont = Control.DefaultFont;
                                }
                                if (sysFont != null)
                                {
                                    // Ensure font is in pixels so it displays properly in the property grid at design time.
                                    if (sysFont.Unit != GraphicsUnit.Point)
                                    {
                                        s_defaultFont = ControlPaint.FontInPoints(sysFont);
                                        retFont = s_defaultFont;
                                        sysFont.Dispose();
                                    }
                                    else
                                    {
                                        s_defaultFont = sysFont;
                                        retFont = s_defaultFont;
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

        internal static int CurrentDpi { get; set; } = DpiHelper.DeviceDpi;

        internal static WeakRefCollection ToolStrips
            => t_toolStripWeakArrayList ??= new WeakRefCollection();

        /// <summary>Static events only!!!</summary>
        private static void AddEventHandler(int key, Delegate value)
        {
            lock (s_internalSyncObject)
            {
                t_staticEventHandlers ??= new Delegate[StaticEventCount];
                t_staticEventHandlers[key] = Delegate.Combine(t_staticEventHandlers[key], value);
            }
        }

        /// <summary>
        ///  Find a toolstrip in the weak ref ArrayList, return null if nothing was found
        /// </summary>
        public static ToolStrip FindToolStrip(string toolStripName)
        {
            ToolStrip result = null;
            for (int i = 0; i < ToolStrips.Count; i++)
            {
                if (ToolStrips[i] != null && string.Equals(((ToolStrip)ToolStrips[i]).Name, toolStripName, StringComparison.Ordinal))
                {
                    result = (ToolStrip)ToolStrips[i];
                    break;
                }
            }

            return result;
        }

        /// <summary>
        ///  Find a toolstrip in the weak ref ArrayList, return null if nothing was found
        /// </summary>
        internal static ToolStrip FindToolStrip(Form owningForm, string toolStripName)
        {
            ToolStrip result = null;
            for (int i = 0; i < ToolStrips.Count; i++)
            {
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
            if (toolStrip is null)
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
            if (toolStrip is null || start is null)
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

            // Copy over the hwnd that we want to restore focus to on ESC
            start.SnapFocusChange(toolStrip);

            toolStrip.SelectNextToolStripItem(null, toolStrip.RightToLeft != RightToLeft.Yes);
            return true;
        }

        private static Delegate GetEventHandler(int key)
        {
            lock (s_internalSyncObject)
            {
                if (t_staticEventHandlers is null)
                {
                    return null;
                }

                return (Delegate)t_staticEventHandlers[key];
            }
        }

        private static bool IsOnSameWindow(Control control1, Control control2)
            => User32.GetAncestor(control1, User32.GA.ROOT) == User32.GetAncestor(control2, User32.GA.ROOT);

        internal static bool IsThreadUsingToolStrips()
            => t_toolStripWeakArrayList != null && t_toolStripWeakArrayList.Count > 0;

        private static void OnUserPreferenceChanging(object sender, UserPreferenceChangingEventArgs e)
        {
            // Using changing here so that the cache will be cleared by the time the ToolStrip
            // hooks onto the changed event.

            // SPI_SETNONCLIENTMETRICS is put up in WM_SETTINGCHANGE if the Menu font changes.
            // this corresponds to UserPreferenceCategory.Window.
            if (e.Category != UserPreferenceCategory.Window)
            {
                return;
            }

            if (DpiHelper.IsPerMonitorV2Awareness)
            {
                s_defaultFontCache.Clear();
            }
            else
            {
                lock (s_internalSyncObject)
                {
                    s_defaultFont = null;
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

        /// <summary>
        ///  Removes dead entries from the toolstrip weak reference collection.
        /// </summary>
        internal static void PruneToolStripList()
        {
            if (t_toolStripWeakArrayList is null || t_toolStripWeakArrayList.Count == 0)
            {
                return;
            }

            for (int i = t_toolStripWeakArrayList.Count - 1; i >= 0; i--)
            {
                if (t_toolStripWeakArrayList[i] is null)
                {
                    t_toolStripWeakArrayList.RemoveAt(i);
                }
            }
        }

        private static void RemoveEventHandler(int key, Delegate value)
        {
            lock (s_internalSyncObject)
            {
                if (t_staticEventHandlers != null)
                {
                    t_staticEventHandlers[key] = Delegate.Remove(t_staticEventHandlers[key], value);
                }
            }
        }

        /// <summary>
        ///  This is a special version of SelectNextControl which looks for ToolStrips
        ///  that are TabStop = false in TabOrder. This is used from Control+Tab
        ///  handling to swap focus between ToolStrips.
        /// </summary>
        internal static bool SelectNextToolStrip(ToolStrip start, bool forward)
        {
            if (start is null || start.ParentInternal is null)
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
                Debug.WriteLineIf(ToolStrip.s_controlTabDebug.TraceVerbose, "SELECTNEXTTOOLSTRIP: start: " + startTabIndex.ToString(CultureInfo.CurrentCulture) + " " + start.Name);
                // since CanChangeSelection can iterate through all the items in a toolstrip,
                // defer the checking until we think we've got a viable TabIndex candidate.
                // this brings it to O(n+m) instead of O(n*m) where n is # toolstrips & m is avg number
                // items/toolstrip
                if (forward)
                {
                    if (nextControlTabIndex >= startTabIndex && CanChangeSelection(start, toolStrip))
                    {
                        Debug.WriteLineIf(ToolStrip.s_controlTabDebug.TraceVerbose, "FORWARD considering selection " + toolStrip.Name + " " + toolStrip.TabIndex.ToString(CultureInfo.CurrentCulture));
                        if (nextControl is null)
                        {
                            nextControl = toolStrip;
                        }
                        else if (toolStrip.TabIndex < nextControl.TabIndex)
                        {
                            // We want to pick a larger index, but one that's
                            // closest to the start tab index.
                            nextControl = toolStrip;
                        }
                    }
                    else if (((wrappedControl is null) || (toolStrip.TabIndex < wrappedControl.TabIndex))
                              && CanChangeSelection(start, toolStrip))
                    {
                        // We've found a candidate for wrapping (the one with the smallest tab index in the collection)
                        Debug.WriteLineIf(ToolStrip.s_controlTabDebug.TraceVerbose, "\tFORWARD new wrap candidate " + toolStrip.Name);
                        wrappedControl = toolStrip;
                    }
                }
                else
                {
                    if (nextControlTabIndex <= startTabIndex && CanChangeSelection(start, toolStrip))
                    {
                        Debug.WriteLineIf(ToolStrip.s_controlTabDebug.TraceVerbose, "\tREVERSE selecting " + toolStrip.Name);
                        if (nextControl is null)
                        {
                            nextControl = toolStrip;
                        }
                        else if (toolStrip.TabIndex > nextControl.TabIndex)
                        {
                            // We want to pick a smaller index, but one that's
                            // closest to the start tab index.
                            nextControl = toolStrip;
                        }
                    }
                    else if (((wrappedControl is null) || (toolStrip.TabIndex > wrappedControl.TabIndex))
                               && CanChangeSelection(start, toolStrip))
                    {
                        // We've found a candidate for wrapping (the one with the largest tab index in the collection)
                        Debug.WriteLineIf(ToolStrip.s_controlTabDebug.TraceVerbose, "\tREVERSE new wrap candidate " + toolStrip.Name);

                        wrappedControl = toolStrip;
                    }
                    else
                    {
                        Debug.WriteLineIf(ToolStrip.s_controlTabDebug.TraceVerbose, "\tREVERSE skipping wrap candidate " + toolStrip.Name + toolStrip.TabIndex.ToString(CultureInfo.CurrentCulture));
                    }
                }
                if (nextControl != null
                    && Math.Abs(nextControl.TabIndex - startTabIndex) <= 1)
                {
                    // If we've found a valid candidate and it's within 1
                    // then bail, we've found something close enough.
                    break;
                }
            }
            if (nextControl != null)
            {
                Debug.WriteLineIf(ToolStrip.s_controlTabDebug.TraceVerbose, "SELECTING " + nextControl.Name);
                return ChangeSelection(start, nextControl);
            }
            else if (wrappedControl != null)
            {
                Debug.WriteLineIf(ToolStrip.s_controlTabDebug.TraceVerbose, "WRAPPING " + wrappedControl.Name);

                return ChangeSelection(start, wrappedControl);
            }

            return false;
        }

        /// <remarks>
        ///  This is thread static because we want separate instances for each thread.
        ///  We don't want to guarantee thread safety and don't want to have to take
        ///  locks in painting code.
        /// </remarks>
        [ThreadStatic]
        private static ToolStripRenderer t_defaultRenderer;

        // types cached for perf.
        internal static Type s_systemRendererType = typeof(ToolStripSystemRenderer);
        internal static Type s_professionalRendererType = typeof(ToolStripProfessionalRenderer);
        private static bool s_visualStylesEnabledIfPossible = true;

        [ThreadStatic]
        private static Type t_currentRendererType;

        private static Type CurrentRendererType
        {
            get
            {
                InitializeThread();
                return t_currentRendererType;
            }
            set => t_currentRendererType = value;
        }

        private static Type s_defaultRendererType => s_professionalRendererType;

        /// <summary>
        ///  The default renderer for the thread. When ToolStrip.RenderMode is set
        ///  to manager - this is the property used.
        /// </summary>
        public static ToolStripRenderer Renderer
        {
            get
            {
                if (t_defaultRenderer is null)
                {
                    t_defaultRenderer = CreateRenderer(RenderMode);
                }
                return t_defaultRenderer;
            }
            set
            {
                if (t_defaultRenderer != value)
                {
                    CurrentRendererType = (value is null) ? s_defaultRendererType : value.GetType();
                    t_defaultRenderer = value;

                    ((EventHandler)GetEventHandler(StaticEventDefaultRendererChanged))?.Invoke(null, EventArgs.Empty);
                }
            }
        }

        /// <summary>
        ///  Occurs when toolstripmanager.Renderer property has changed
        ///  Warning: When subscribing to static event handlers - make sure you unhook from them
        ///  otherwise you can leak user objects on process shutdown.
        /// </summary>
        public static event EventHandler RendererChanged
        {
            add => AddEventHandler(StaticEventDefaultRendererChanged, value);
            remove => RemoveEventHandler(StaticEventDefaultRendererChanged, value);
        }

        /// <summary>
        ///  Returns the default toolstrip RenderMode for the thread
        /// </summary>
        public static ToolStripManagerRenderMode RenderMode
        {
            get
            {
                Type currentType = CurrentRendererType;

                if (t_defaultRenderer != null && !t_defaultRenderer.IsAutoGenerated)
                {
                    return ToolStripManagerRenderMode.Custom;
                }
                // check the type of the currently set renderer.
                // types are cached as this may be called frequently.
                if (currentType == s_professionalRendererType)
                {
                    return ToolStripManagerRenderMode.Professional;
                }
                if (currentType == s_systemRendererType)
                {
                    return ToolStripManagerRenderMode.System;
                }

                return ToolStripManagerRenderMode.Custom;
            }
            set
            {
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

        /// <summary>
        ///  An additional layering of control. This lets you pick whether your toolbars
        ///  should use visual style information (theming) to render itself.
        ///  potentially you could want a themed app but an unthemed toolstrip.
        /// </summary>
        public static bool VisualStylesEnabled
        {
            get => s_visualStylesEnabledIfPossible && Application.RenderWithVisualStyles;
            set
            {
                bool oldVis = VisualStylesEnabled;
                s_visualStylesEnabledIfPossible = value;

                if (oldVis != VisualStylesEnabled)
                {
                    ((EventHandler)GetEventHandler(StaticEventDefaultRendererChanged))?.Invoke(null, EventArgs.Empty);
                }
            }
        }

        internal static ToolStripRenderer CreateRenderer(ToolStripManagerRenderMode renderMode)
        {
            switch (renderMode)
            {
                case ToolStripManagerRenderMode.System:
                    return new ToolStripSystemRenderer(isDefault: true);
                case ToolStripManagerRenderMode.Professional:
                    return new ToolStripProfessionalRenderer(isDefault: true);
                case ToolStripManagerRenderMode.Custom:
                default:
                    return new ToolStripSystemRenderer(isDefault: true);
            }
        }

        internal static ToolStripRenderer CreateRenderer(ToolStripRenderMode renderMode)
        {
            switch (renderMode)
            {
                case ToolStripRenderMode.System:
                    return new ToolStripSystemRenderer(isDefault: true);
                case ToolStripRenderMode.Professional:
                    return new ToolStripProfessionalRenderer(isDefault: true);
                case ToolStripRenderMode.Custom:
                default:
                    return new ToolStripSystemRenderer(isDefault: true);
            }
        }

        internal static WeakRefCollection ToolStripPanels
            => t_toolStripPanelWeakArrayList ??= new WeakRefCollection();

        internal static ToolStripPanel ToolStripPanelFromPoint(Control draggedControl, Point screenLocation)
        {
            if (t_toolStripPanelWeakArrayList != null)
            {
                ISupportToolStripPanel draggedItem = draggedControl as ISupportToolStripPanel;
                bool rootWindowCheck = draggedItem.IsCurrentlyDragging;

                for (int i = 0; i < t_toolStripPanelWeakArrayList.Count; i++)
                {
                    if (t_toolStripPanelWeakArrayList[i] is ToolStripPanel toolStripPanel && toolStripPanel.IsHandleCreated && toolStripPanel.Visible &&
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

        /// <summary>
        ///  Loads settings for the given Form using the form type's fullname as settings key.
        /// </summary>
        public static void LoadSettings(Form targetForm)
        {
            if (targetForm is null)
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
            if (targetForm is null)
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
            if (sourceForm is null)
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
            if (sourceForm is null)
            {
                throw new ArgumentNullException(nameof(sourceForm));
            }

            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException(nameof(key));
            }

            var settingsManager = new ToolStripSettingsManager(sourceForm, key);
            settingsManager.Save();
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

        /// <summary>
        ///  Determines if the key combination is valid for a shortcut.
        ///  Must have a modifier key + a regular key.
        /// </summary>
        public static bool IsValidShortcut(Keys shortcut)
        {
            // Should have a key and one or more modifiers.
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
                // Function keys by themselves are valid
                return true;
            }
            else if ((keyCode != Keys.None) && (modifiers != Keys.None))
            {
                switch (keyCode)
                {
                    case Keys.Menu:
                    case Keys.ControlKey:
                    case Keys.ShiftKey:
                        // Shift, control and alt arent valid on their own.
                        return false;
                    default:
                        if (modifiers == Keys.Shift)
                        {
                            // Shift + somekey isnt a valid modifier either
                            return false;
                        }
                        return true;
                }
            }

            // Has to have a valid keycode and valid modifier.
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
                if (ToolStrips[i] is ToolStrip t && t.Shortcuts.Contains(shortcut))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        ///  This function is called for toplevel controls to process shortcuts.
        ///  This function should be called from the topmost container control only.
        /// </summary>
        internal static bool ProcessCmdKey(ref Message m, Keys keyData)
        {
            Debug.WriteLineIf(Control.s_controlKeyboardRouting.TraceVerbose, "ToolStripManager.ProcessCmdKey - processing: [" + keyData.ToString() + "]");
            if (ToolStripManager.IsValidShortcut(keyData))
            {
                // If we're at the toplevel, check the toolstrips for matching shortcuts.
                // Win32 menus are handled in Form.ProcessCmdKey, but we cant guarantee that
                // toolstrips will be hosted in a form. ToolStrips have a hash of shortcuts
                // per container, so this should hopefully be a quick search.
                Debug.WriteLineIf(Control.s_controlKeyboardRouting.TraceVerbose, "ToolStripManager.ProcessCmdKey - IsValidShortcut: [" + keyData.ToString() + "]");

                return ToolStripManager.ProcessShortcut(ref m, keyData);
            }
            if (m.Msg == (int)User32.WM.SYSKEYDOWN)
            {
                Debug.WriteLineIf(Control.s_controlKeyboardRouting.TraceVerbose, "ToolStripManager.ProcessCmdKey - Checking if it's a menu key: [" + keyData.ToString() + "]");
                ToolStripManager.ModalMenuFilter.ProcessMenuKeyDown(ref m);
            }

            return false;
        }

        /// <summary>
        ///  We're halfway to an accellerator table system here.
        ///  Each toolstrip maintains a hash of the current shortcuts its using.
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

                // Start from the focused control and work your way up the parent chain
                do
                {
                    // Check the context menu strip first.
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
                    // The keystroke may applies to one of our parents...
                    // a WM_CONTEXTMENU message bubbles up to the parent control
                    activeControl = activeControlInChain;
                }

                bool retVal = false;
                bool needsPrune = false;

                // Now search the toolstrips
                for (int i = 0; i < ToolStrips.Count; i++)
                {
                    bool isAssociatedContextMenu = false;
                    bool isDoublyAssignedContextMenuStrip = false;

                    if (!(ToolStrips[i] is ToolStrip toolStrip))
                    {
                        // Consider prune tree.
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
                            // We dont want to process someone else's context menu (e.g. button1 and button2 have context menus)
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
                                        // The toplevel context menu is NOT the same as the active control's context menu.
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
                            // Make sure that were processing shortcuts for the correct window.
                            // since the shortcut lookup is faster than this check we've postponed this to the last
                            // possible moment.
                            ToolStrip topMostToolStrip = toolStrip.GetToplevelOwnerToolStrip();
                            if (topMostToolStrip != null && activeControl != null)
                            {
                                IntPtr rootWindowOfToolStrip = User32.GetAncestor(topMostToolStrip, User32.GA.ROOT);
                                IntPtr rootWindowOfControl = User32.GetAncestor(activeControl, User32.GA.ROOT);
                                rootWindowsMatch = rootWindowOfToolStrip == rootWindowOfControl;

                                if (rootWindowsMatch)
                                {
                                    // Double check this is not an MDIContainer type situation...
                                    if (Control.FromHandle(rootWindowOfControl) is Form mainForm && mainForm.IsMdiContainer)
                                    {
                                        Form toolStripForm = topMostToolStrip.FindForm();
                                        if (toolStripForm != mainForm && toolStripForm != null)
                                        {
                                            // wW should only process shortcuts of the ActiveMDIChild or the Main Form.
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

        /// <summary>
        ///  This function handles when Alt is pressed.
        ///  If it finds a menustrip to select, it returns true,
        ///  If it doesnt it returns false.
        ///  If it finds a win32 menu is already associated with the control it bails, returning false.
        /// </summary>
        internal static bool ProcessMenuKey(ref Message m)
        {
            Debug.WriteLineIf(Control.s_controlKeyboardRouting.TraceVerbose, "ToolStripManager.ProcessMenuKey: [" + m.ToString() + "]");
            if (!IsThreadUsingToolStrips())
            {
                return false;
            }

            Debug.WriteLineIf(ToolStrip.s_snapFocusDebug.TraceVerbose, "[ProcessMenuKey] Determining whether we should send focus to MenuStrip");

            Keys keyData = (Keys)(int)m.LParam;

            // Search for our menu to work with
            Control intendedControl = Control.FromHandle(m.HWnd);
            Control toplevelControl = null;

            MenuStrip menuStripToActivate = null;
            if (intendedControl != null)
            {
                // Search for a menustrip to select.
                toplevelControl = intendedControl.TopLevelControlInternal;
                if (toplevelControl != null)
                {
                    IntPtr hMenu = User32.GetMenu(toplevelControl);
                    if (hMenu == IntPtr.Zero)
                    {
                        // Only activate the menu if there's no win32 menu. Win32 menus trump menustrips.
                        menuStripToActivate = GetMainMenuStrip(toplevelControl);
                    }
                    Debug.WriteLineIf(ToolStrip.s_snapFocusDebug.TraceVerbose, string.Format(CultureInfo.CurrentCulture, "[ProcessMenuKey] MenuStripToActivate is: {0}", menuStripToActivate));
                }
            }

            // The data that comes into the LParam is the ASCII code, not the VK_* code.
            // we need to compare against char instead.
            if ((char)keyData == ' ')
            {
                // Dont process system menu
                ModalMenuFilter.MenuKeyToggle = false;
            }
            else if ((char)keyData == '-')
            {
                // Deal with MDI system menu
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
                // This is the same as Control.ModifierKeys - but we save two p/invokes.
                if (User32.GetKeyState((int)Keys.ShiftKey) < 0 && (keyData == Keys.None))
                {
                    // If it's Shift+F10 and we're already InMenuMode, then we
                    // need to cancel this message, otherwise we'll enter the native modal menu loop.
                    Debug.WriteLineIf(ToolStrip.s_snapFocusDebug.TraceVerbose, "[ProcessMenuKey] DETECTED SHIFT+F10" + keyData.ToString());
                    return ToolStripManager.ModalMenuFilter.InMenuMode;
                }
                else
                {
                    if (menuStripToActivate != null && !ModalMenuFilter.MenuKeyToggle)
                    {
                        Debug.WriteLineIf(ToolStrip.s_snapFocusDebug.TraceVerbose, "[ProcessMenuKey] attempting to set focus to menustrip");

                        // If we've alt-tabbed away dont snap/restore focus.
                        IntPtr topmostParentOfMenu = User32.GetAncestor(menuStripToActivate, User32.GA.ROOT);
                        IntPtr foregroundWindow = User32.GetForegroundWindow();

                        if (topmostParentOfMenu == foregroundWindow)
                        {
                            Debug.WriteLineIf(ToolStrip.s_snapFocusDebug.TraceVerbose, "[ProcessMenuKey] ToolStripManager call MenuStrip.OnMenuKey");
                            return menuStripToActivate.OnMenuKey();
                        }
                    }
                    else if (menuStripToActivate != null)
                    {
                        Debug.WriteLineIf(ToolStrip.s_snapFocusDebug.TraceVerbose, "[ProcessMenuKey] Resetting MenuKeyToggle");
                        ModalMenuFilter.MenuKeyToggle = false;
                        return true;
                    }
                }
            }

            return false;
        }

        internal static MenuStrip GetMainMenuStrip(Control control)
        {
            if (control is null)
            {
                Debug.Fail("why are we passing null to GetMainMenuStrip?");
                return null;
            }

            // Look for a particular main menu strip to be set.
            Form mainForm = control.FindForm();
            if (mainForm != null && mainForm.MainMenuStrip != null)
            {
                return mainForm.MainMenuStrip;
            }

            // If not found go through the entire collection.
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
                    if (controlsToLookIn[i] is null)
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
                    if (controlsToLookIn[i] is null)
                    {
                        continue;
                    }

                    if ((controlsToLookIn[i].Controls != null) && controlsToLookIn[i].Controls.Count > 0)
                    {
                        // If it has a valid child collection, append those results to our collection
                        MenuStrip menuStrip = GetFirstMenuStripRecursive(controlsToLookIn[i].Controls);
                        if (menuStrip != null)
                        {
                            return menuStrip;
                        }
                    }
                }
            }
            catch (Exception e) when (!ClientUtils.IsCriticalException(e))
            {
            }

            return null;
        }

        private static ToolStripItem FindMatch(ToolStripItem source, ToolStripItemCollection destinationItems)
        {
            // Based on MergeAction:
            // Append, return the last sibling
            ToolStripItem result = null;
            if (source != null)
            {
                for (int i = 0; i < destinationItems.Count; i++)
                {
                    ToolStripItem candidateItem = destinationItems[i];
                    // Using SafeCompareKeys so we use the same heuristics as keyed collections.
                    if (WindowsFormsUtils.SafeCompareStrings(source.Text, candidateItem.Text, true))
                    {
                        // We found it
                        result = candidateItem;
                        break;
                    }
                }

                if (result is null && source.MergeIndex > -1 && source.MergeIndex < destinationItems.Count)
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
                    if (candidateTS != null && candidateTS.AllowMerge && container == candidateTS.FindForm())
                    {
                        result.Add(candidateTS);
                    }
                }
            }

            // Sort them from more specific to less specific
            result.Sort(new ToolStripCustomIComparer());
            return result;
        }

        private static bool IsSpecialMDIStrip(ToolStrip toolStrip)
            => toolStrip is MdiControlStrip || toolStrip is MdiWindowListStrip;

        /// <summary>
        ///  Merge two toolstrips
        /// </summary>
        public static bool Merge(ToolStrip sourceToolStrip, ToolStrip targetToolStrip)
        {
            if (sourceToolStrip is null)
            {
                throw new ArgumentNullException(nameof(sourceToolStrip));
            }
            if (targetToolStrip is null)
            {
                throw new ArgumentNullException(nameof(targetToolStrip));
            }
            if (targetToolStrip == sourceToolStrip)
            {
                throw new ArgumentException(SR.ToolStripMergeImpossibleIdentical);
            }

            // We only do this if the source and target toolstrips are the same
            bool canMerge = IsSpecialMDIStrip(sourceToolStrip);
            canMerge = (canMerge || (sourceToolStrip.AllowMerge &&
                                      targetToolStrip.AllowMerge &&
                                      (sourceToolStrip.GetType().IsAssignableFrom(targetToolStrip.GetType()) || targetToolStrip.GetType().IsAssignableFrom(sourceToolStrip.GetType()))
                                    )
                        );
            MergeHistory mergeHistory = null;
            if (canMerge)
            {
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
                    if (mergeHistory.MergeHistoryItemsStack.Count > 0)
                    {
                        // Only push this on the stack if we actually did something
                        targetToolStrip.MergeHistoryStack.Push(mergeHistory);
                    }
                }
            }

            return mergeHistory != null && mergeHistory.MergeHistoryItemsStack.Count > 0;
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
                                            // The act of walking through this collection removes items from
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
                                if (source.MergeAction == MergeAction.Replace)
                                {
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
                    break;
            }
            Debug.Unindent();
        }

        /// <summary>
        ///  Merge two toolstrips
        /// </summary>
        public static bool Merge(ToolStrip sourceToolStrip, string targetName)
        {
            if (sourceToolStrip is null)
            {
                throw new ArgumentNullException(nameof(sourceToolStrip));
            }
            if (targetName is null)
            {
                throw new ArgumentNullException(nameof(targetName));
            }

            ToolStrip target = FindToolStrip(targetName);
            if (target is null)
            {
                return false;
            }
            else
            {
                return Merge(sourceToolStrip, target);
            }
        }

        /// <remarks>
        ///  Doesn't do a null check on source - if it's null we unmerge everything
        /// </remarks>
        internal static bool RevertMergeInternal(ToolStrip targetToolStrip, ToolStrip sourceToolStrip, bool revertMDIControls)
        {
            bool result = false;
            if (targetToolStrip is null)
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
                // We have a specific toolstrip to pull out.
                // Make sure the sourceToolStrip is even merged into the targetToolStrip
                foreach (MergeHistory history in targetToolStrip.MergeHistoryStack)
                {
                    foundToolStrip = (history.MergedToolStrip == sourceToolStrip);
                    if (foundToolStrip)
                    {
                        break;
                    }
                }

                // Performance: if we dont have the toolstrip in our merge history, bail.
                if (!foundToolStrip)
                {
                    return false;
                }
            }

            sourceToolStrip?.SuspendLayout();
            targetToolStrip.SuspendLayout();

            try
            {
                Stack<ToolStrip> reApply = new Stack<ToolStrip>();
                foundToolStrip = false;
                while (targetToolStrip.MergeHistoryStack.Count > 0 && !foundToolStrip)
                {
                    // We unmerge something.
                    result = true;
                    MergeHistory history = targetToolStrip.MergeHistoryStack.Pop();
                    if (history.MergedToolStrip == sourceToolStrip)
                    {
                        foundToolStrip = true;
                    }
                    else if (!revertMDIControls && sourceToolStrip is null)
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

                    Debug.Indent();
                    while (history.MergeHistoryItemsStack.Count > 0)
                    {
                        MergeHistoryItem historyItem = history.MergeHistoryItemsStack.Pop();
                        switch (historyItem.MergeAction)
                        {
                            case MergeAction.Remove:
                                historyItem.IndexCollection.Remove(historyItem.TargetItem);
                                // Put it back
                                historyItem.PreviousIndexCollection.Insert(Math.Min(historyItem.PreviousIndex, historyItem.PreviousIndexCollection.Count), historyItem.TargetItem);
                                break;
                            case MergeAction.Insert:
                                historyItem.IndexCollection.Insert(Math.Min(historyItem.Index, historyItem.IndexCollection.Count), historyItem.TargetItem);
                                // No need to put it back, inserting it in a new collection, moved it at the correct location
                                break;
                        }
                    }
                    Debug.Unindent();
                }

                // Re-apply the merges of the toolstrips we had to unmerge first.
                while (reApply.Count > 0)
                {
                    ToolStrip mergeAgain = reApply.Pop();
                    Merge(mergeAgain, targetToolStrip);
                }
            }
            finally
            {
                sourceToolStrip?.ResumeLayout();
                targetToolStrip.ResumeLayout();
            }

            return result;
        }

        /// <summary>
        ///  Unmerge two toolstrips
        /// </summary>
        public static bool RevertMerge(ToolStrip targetToolStrip)
            => RevertMergeInternal(targetToolStrip, null, revertMDIControls: false);

        /// <summary>
        ///  Unmerge two toolstrips
        /// </summary>
        public static bool RevertMerge(ToolStrip targetToolStrip, ToolStrip sourceToolStrip)
        {
            if (sourceToolStrip is null)
            {
                throw new ArgumentNullException(nameof(sourceToolStrip));
            }

            return RevertMergeInternal(targetToolStrip, sourceToolStrip, revertMDIControls: false);
        }

        /// <summary>
        ///  Unmerge two toolstrips
        /// </summary>
        public static bool RevertMerge(string targetName)
        {
            ToolStrip target = FindToolStrip(targetName);
            if (target is null)
            {
                return false;
            }

            return RevertMerge(target);
        }
    }
}
