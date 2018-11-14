// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {
    using Accessibility;
    using Microsoft.Win32;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.ComponentModel.Design.Serialization;
    using System.Configuration.Assemblies;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Globalization;
    using System.Security.Permissions;
    using System.Security;
    using System.IO;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Runtime.InteropServices.ComTypes;
    using System.Runtime.Remoting;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Formatters;
    using System.Runtime.Serialization.Formatters.Binary;
    using System.Text;
    using System.Threading;
    using System.Windows.Forms.Design;
    using System.Windows.Forms.Internal;
    using Encoding = System.Text.Encoding;
    using System.Drawing.Imaging;
    using System.Windows.Forms.Layout;

    /// <devdoc>
    ///     Control's IME feature.
    /// </devdoc>
    public partial class Control :
    Component,
    UnsafeNativeMethods.IOleControl,
    UnsafeNativeMethods.IOleObject,
    UnsafeNativeMethods.IOleInPlaceObject,
    UnsafeNativeMethods.IOleInPlaceActiveObject,
    UnsafeNativeMethods.IOleWindow,
    UnsafeNativeMethods.IViewObject,
    UnsafeNativeMethods.IViewObject2,
    UnsafeNativeMethods.IPersist,
    UnsafeNativeMethods.IPersistStreamInit,
    UnsafeNativeMethods.IPersistPropertyBag,
    UnsafeNativeMethods.IPersistStorage,
    UnsafeNativeMethods.IQuickActivate,
    ISupportOleDropSource,
    IDropTarget,
    ISynchronizeInvoke,
    IWin32Window,
    IArrangedElement,
    IBindableComponent {

        /// <devdoc>
        ///     Constants starting/ending the WM_CHAR messages to ignore count.  See ImeWmCharsToIgnore property.
        /// </devdoc>
        private const int ImeCharsToIgnoreDisabled = -1;
        private const int ImeCharsToIgnoreEnabled = 0;

        /// <devdoc>
        ///     The ImeMode value for controls with ImeMode = ImeMode.NoControl.  See PropagatingImeMode property.
        /// </devdoc>
        private static ImeMode propagatingImeMode = ImeMode.Inherit; // Inherit means uninitialized.

        /// <devdoc>
        ///     This flag prevents resetting ImeMode value of the focused control.  See IgnoreWmImeNotify property.
        /// </devdoc>
        private static bool ignoreWmImeNotify;

        /// <devdoc>
        ///     This flag works around an Issue with the Chinese IME sending IMENotify messages prior to WmInputLangChange
        ///     which would cause this code to use OnHalf as the default mode overriding .ImeMode property. See WmImeNotify
        /// </devdoc>
        private static bool lastLanguageChinese = false;

        /// <devdoc>
        ///     The ImeMode in the property store.
        /// </devdoc>
        internal ImeMode CachedImeMode {
            get {
                Debug.WriteLineIf( CompModSwitches.ImeMode.Level >= TraceLevel.Info, "Inside get_CachedImeMode(), this = " + this );
                Debug.Indent();

                // Get the ImeMode from the property store
                //
                bool found;
                ImeMode cachedImeMode = (ImeMode) Properties.GetInteger( PropImeMode, out found );
                if( !found ) {
                    cachedImeMode = DefaultImeMode;
                    Debug.WriteLineIf( CompModSwitches.ImeMode.Level >= TraceLevel.Verbose, "using DefaultImeMode == " + cachedImeMode );
                }
                else {
                    Debug.WriteLineIf( CompModSwitches.ImeMode.Level >= TraceLevel.Verbose, "using property store ImeMode == " + cachedImeMode );
                }

                // If inherited, get the mode from this control's parent
                //
                if( cachedImeMode == ImeMode.Inherit ) {
                    Control parent = ParentInternal;
                    if( parent != null ) {
                        cachedImeMode = parent.CachedImeMode;
                        Debug.WriteLineIf( CompModSwitches.ImeMode.Level >= TraceLevel.Verbose, "inherited from parent = " + parent.GetType() );
                    }
                    else {
                        cachedImeMode = ImeMode.NoControl;
                    }
                }

                Debug.WriteLineIf( CompModSwitches.ImeMode.Level >= TraceLevel.Verbose, "returning CachedImeMode == " + cachedImeMode );
                Debug.Unindent();

                return cachedImeMode;
            }

            set {
                // When the control is in restricted mode (!CanEnableIme) the CachedImeMode should be changed only programatically,
                //           calls generated by user interaction should be wrapped with a check for CanEnableIme.

                Debug.WriteLineIf( CompModSwitches.ImeMode.Level >= TraceLevel.Info, "Inside set_CachedImeMode(), this = " + this );
                Debug.Indent();

                Debug.WriteLineIf( CompModSwitches.ImeMode.Level >= TraceLevel.Warning, "Setting cached Ime == " + value );
                Properties.SetInteger( PropImeMode, (int) value );

                Debug.Unindent();
            }
        }

        /// <devdoc>
        ///     Specifies whether the ImeMode property value can be changed to an active value.
        ///     Added to support Password & ReadOnly (and maybe other) properties, which when set, should force disabling
        ///     the IME if using one.
        /// </devdoc>
        protected virtual bool CanEnableIme {
            get {
                // Note: If overriding this property make sure to add the Debug tracing code and call this method (base.CanEnableIme).

                Debug.Indent();
                Debug.WriteLineIf( CompModSwitches.ImeMode.Level >= TraceLevel.Info, string.Format(CultureInfo.CurrentCulture, "Inside get_CanEnableIme(), value = {0}, this = {1}", ImeSupported, this ) );
                Debug.Unindent();

                return ImeSupported;
            }
        }

        /// <devdoc>
        ///     Gets the current IME context mode.  If no IME associated, ImeMode.Inherit is returned.
        /// </devdoc>
        internal ImeMode CurrentImeContextMode {
            get {
                Debug.WriteLineIf( CompModSwitches.ImeMode.Level >= TraceLevel.Info, "Inside get_CurrentImeContextMode(), this = " + this );

                if( this.IsHandleCreated ) {
                    return ImeContext.GetImeMode( this.Handle );
                }
                else {
                    // window is not yet created hence no IME associated yet. 
                    return ImeMode.Inherit;
                }
            }
        }

        /// <devdoc>
        /// </devdoc>
        protected virtual ImeMode DefaultImeMode {
            get { return ImeMode.Inherit; }
        }

        /// <devdoc>
        ///     Flag used to avoid re-entrancy during WM_IME_NOTFIY message processing - see WmImeNotify().
        ///     Also to avoid raising the ImeModeChanged event more than once during the process of changing the ImeMode.
        /// </devdoc>
        internal int DisableImeModeChangedCount {
            get {
                Debug.WriteLineIf( CompModSwitches.ImeMode.Level >= TraceLevel.Info, "Inside get_DisableImeModeChangedCount()" );
                Debug.Indent();

                bool dummy;
                int val = (int) Properties.GetInteger( PropDisableImeModeChangedCount, out dummy );

                Debug.Assert( val >= 0, "Counter underflow." );
                Debug.WriteLineIf( CompModSwitches.ImeMode.Level >= TraceLevel.Info, "Value: " + val );
                Debug.Unindent();

                return val;
            }
            set {
                Debug.WriteLineIf( CompModSwitches.ImeMode.Level >= TraceLevel.Info, "Inside set_DisableImeModeChangedCount(): " + value );
                Properties.SetInteger(PropDisableImeModeChangedCount, value);
            }
        }

        /// <devdoc>
        ///     Flag used to prevent setting ImeMode in focused control when losing focus and hosted in a non-Form shell.
        ///     See WmImeKillFocus() for more info.
        /// </devdoc>
        private static bool IgnoreWmImeNotify {
            get {
                Debug.WriteLineIf(CompModSwitches.ImeMode.Level >= TraceLevel.Info, "Inside get_IgnoreWmImeNotify()");
                Debug.Indent();

                bool val = Control.ignoreWmImeNotify;

                Debug.WriteLineIf(CompModSwitches.ImeMode.Level >= TraceLevel.Info, "Value: " + val);
                Debug.Unindent();

                return val;
            }
            set {
                Debug.WriteLineIf(CompModSwitches.ImeMode.Level >= TraceLevel.Info, "Inside set_IgnoreWmImeNotify(): " + value);
                Control.ignoreWmImeNotify = value;
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ImeMode"]/*' />
        /// <devdoc>
        ///     Specifies a value that determines the IME (Input Method Editor) status of the
        ///     object when that object is selected.
        /// </devdoc>
        [
        SRCategory( nameof(SR.CatBehavior)),
        Localizable( true ),
        AmbientValue( ImeMode.Inherit ),
        SRDescription( nameof(SR.ControlIMEModeDescr))
        ]
        public ImeMode ImeMode {
            get {
                ImeMode imeMode = ImeModeBase;

                if (imeMode == ImeMode.OnHalf) // This is for compatibility.  See QFE#4448.
                {
                    imeMode = ImeMode.On;
                }

                return imeMode;
            }
            set {
                ImeModeBase = value;
            }
        }

        /// <devdoc>
        ///     Internal version of ImeMode property.  This is provided for controls that override CanEnableIme and that
        ///     return ImeMode.Disable for the ImeMode property when CanEnableIme is false - See TextBoxBase controls.
        /// </devdoc>
        protected virtual ImeMode ImeModeBase {
            get {
                Debug.WriteLineIf( CompModSwitches.ImeMode.Level >= TraceLevel.Info, "Inside get_ImeModeBase(), this = " + this );
                Debug.Indent(); 
                
                ImeMode imeMode = CachedImeMode;

                Debug.WriteLineIf( CompModSwitches.ImeMode.Level >= TraceLevel.Info, "Value = " + imeMode );
                Debug.Unindent();

                return imeMode;
            }
            set {
                Debug.WriteLineIf( CompModSwitches.ImeMode.Level >= TraceLevel.Info, string.Format( CultureInfo.CurrentCulture, "Inside set_ImeModeBase({0}), this = {1}", value, this ) );
                Debug.Indent(); 
                
                //valid values are -1 to 0xb
                if( !ClientUtils.IsEnumValid( value, (int) value, (int) ImeMode.Inherit, (int) ImeMode.OnHalf ) ) {
                    throw new InvalidEnumArgumentException( "ImeMode", (int) value, typeof( ImeMode ) );
                }

                ImeMode oldImeMode = CachedImeMode;
                CachedImeMode = value;

                if( oldImeMode != value ) {
                    // Cache current value to determine whether we need to raise the ImeModeChanged.
                    Control ctl = null;

                    if( !DesignMode && ImeModeConversion.InputLanguageTable != ImeModeConversion.UnsupportedTable ) {
                        // Set the context to the new value if control is focused.
                        if( Focused ) {
                            ctl = this;
                        }
                        else if( ContainsFocus ) {
                            ctl = FromChildHandleInternal( UnsafeNativeMethods.GetFocus() );
                        }

                        if( ctl != null && ctl.CanEnableIme ) {
                            // Block ImeModeChanged since we are checking for it below.
                            DisableImeModeChangedCount++;

                            try {
                                ctl.UpdateImeContextMode();
                            }
                            finally {
                                DisableImeModeChangedCount--;
                            }
                        }
                    }

                    VerifyImeModeChanged( oldImeMode, CachedImeMode );
                }
                
                ImeContext.TraceImeStatus( this );
                Debug.Unindent();
            }
        }

        /// <devdoc>
        ///     Determines whether the Control supports IME handling by default.
        /// </devdoc>
        private bool ImeSupported {
            get {
                return DefaultImeMode != ImeMode.Disable;
            }
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.ImeModeChanged"]/*' />
        /// <devdoc>
        ///    <para>[To be supplied.]</para>
        /// </devdoc>
        [WinCategory( "Behavior" ), SRDescription( nameof(SR.ControlOnImeModeChangedDescr) )]
        public event EventHandler ImeModeChanged {
            add {
                Events.AddHandler( EventImeModeChanged, value );
            }
            remove {
                Events.RemoveHandler( EventImeModeChanged, value );
            }
        }

        /// <devdoc>
        ///     Returns the current number of WM_CHAR messages to ignore after processing corresponding WM_IME_CHAR msgs.
        /// </devdoc>
        internal int ImeWmCharsToIgnore {
            // The IME sends WM_IME_CHAR messages for each character in the composition string, and then
            // after all messages are sent, corresponding WM_CHAR messages are also sent. (in non-unicode 
            // windows two WM_CHAR messages are sent per char in the IME).  We need to keep a counter 
            // not to process each character twice or more.
            // Marshal.SystemDefaultCharSize represents the default character size on the system; the default 
            // is 2 for Unicode systems and 1 for ANSI systems.  This is how it is implemented in Control.
            get {
                return Properties.GetInteger( PropImeWmCharsToIgnore );
            }
            set {
                Debug.WriteLineIf( CompModSwitches.ImeMode.Level >= TraceLevel.Info, string.Format( CultureInfo.CurrentCulture, "Inside set_ImeWmCharToIgnore(value={0}), this = {1}", value, this ) );
                Debug.Indent();

                // WM_CHAR is not send after WM_IME_CHAR when the composition has been closed by either, changing the conversion mode or
                // dissasociating the IME (for instance when loosing focus and conversion is forced to complete).
                if( ImeWmCharsToIgnore != ImeCharsToIgnoreDisabled ) {
                    Properties.SetInteger( PropImeWmCharsToIgnore, value );
                }

                Debug.WriteLineIf( CompModSwitches.ImeMode.Level >= TraceLevel.Verbose, "ImeWmCharsToIgnore on leaving setter: " + ImeWmCharsToIgnore );
                Debug.Unindent();
            }
        }

        /// <devdoc>
        ///     Gets the last value CanEnableIme property when it was last checked for ensuring IME context restriction mode.
        ///     This is used by controls that implement some sort of IME restriction mode (like TextBox on Password/ReadOnly mode).
        ///     See the VerifyImeRestrictedModeChanged() method.
        /// </devdoc>
        private bool LastCanEnableIme {
            get {
                Debug.WriteLineIf( CompModSwitches.ImeMode.Level >= TraceLevel.Info, "Inside get_LastCanEnableIme()" );
                Debug.Indent();

                bool valueFound;
                int val = (int) Properties.GetInteger( PropLastCanEnableIme, out valueFound );

                if( valueFound ) {
                    valueFound = val == 1;
                }
                else {
                    valueFound = true;
                }

                Debug.WriteLineIf( CompModSwitches.ImeMode.Level >= TraceLevel.Info, "Value: " + valueFound );
                Debug.Unindent();

                return valueFound;
            }
            set {
                Debug.WriteLineIf( CompModSwitches.ImeMode.Level >= TraceLevel.Info, "Inside set_LastCanEnableIme(): " + value );
                Properties.SetInteger( PropLastCanEnableIme, value ? 1 : 0 );
            }
        }

        /// <devdoc>
        ///     Represents the internal ImeMode value for controls with ImeMode = ImeMode.NoControl.  This property is changed
        ///     only by user interaction and is required to set the IME context appropriately while keeping the ImeMode property
        ///     unchanged.
        /// </devdoc>
        protected static ImeMode PropagatingImeMode {
            get {
                Debug.WriteLineIf( CompModSwitches.ImeMode.Level >= TraceLevel.Info, "Inside get_PropagatingImeMode()" );
                Debug.Indent();

                if( Control.propagatingImeMode == ImeMode.Inherit ) {
                    // Initialize the propagating IME mode to the value the IME associated to the focused window currently has,
                    // this enables propagating the IME mode from/to unmanaged applications hosting winforms controls.
                    Debug.WriteLineIf( CompModSwitches.ImeMode.Level >= TraceLevel.Verbose, "Initializing PropagatingImeMode" );

                    ImeMode imeMode = ImeMode.Inherit;
                    IntPtr focusHandle = UnsafeNativeMethods.GetFocus();

                    if( focusHandle != IntPtr.Zero ) {
                        imeMode = ImeContext.GetImeMode(focusHandle);

                        // If focused control is disabled we won't be able to get the app ime context mode, try the top window.
                        // this is the case of a disabled winforms control hosted in a non-Form shell.
                        if( imeMode == ImeMode.Disable ) {
                            focusHandle = UnsafeNativeMethods.GetAncestor(new HandleRef(null, focusHandle), NativeMethods.GA_ROOT);

                            if( focusHandle != IntPtr.Zero ) {
                                imeMode = ImeContext.GetImeMode(focusHandle);
                            }
                        }
                    }

                    // If IME is disabled the PropagatingImeMode will not be initialized, see property setter below.
                    PropagatingImeMode = imeMode;
                }

                Debug.WriteLineIf( CompModSwitches.ImeMode.Level >= TraceLevel.Verbose, "Value: " + Control.propagatingImeMode );
                Debug.Unindent();

                return Control.propagatingImeMode;
            }
            private set {
                Debug.WriteLineIf( CompModSwitches.ImeMode.Level >= TraceLevel.Info, "Inside set_PropagatingImeMode()" );
                Debug.Indent();

                if (Control.propagatingImeMode != value) {
                    switch( value ) {
                        case ImeMode.NoControl:
                        case ImeMode.Disable:
                            // Cannot set propagating ImeMode to one of these values.
                            Debug.WriteLineIf(CompModSwitches.ImeMode.Level >= TraceLevel.Verbose, "Cannot change PropagatingImeMode to " + value);
                            return;

                        default:
                            Debug.WriteLineIf( CompModSwitches.ImeMode.Level >= TraceLevel.Warning, string.Format( CultureInfo.CurrentCulture, "Setting PropagatingImeMode: Current value = {0}, New value = {1}", propagatingImeMode, value ) );
                            Control.propagatingImeMode = value;
                        break;
                    }
                }

                Debug.Unindent();
            }
        }

        /// <devdoc>
        ///     Sets the IME context to the appropriate ImeMode according to the control's ImeMode state.
        ///     This method is commonly used when attaching the IME to the control's window.
        /// </devdoc>
        internal void UpdateImeContextMode() {
            ImeMode[] inputLanguageTable = ImeModeConversion.InputLanguageTable;
            if (!DesignMode && (inputLanguageTable != ImeModeConversion.UnsupportedTable) && Focused) {
                // Note: CHN IME won't send WM_IME_NOTIFY msg when getting associated, setting the IME context mode
                // forces the message to be sent as a side effect.

                Debug.WriteLineIf( CompModSwitches.ImeMode.Level >= TraceLevel.Info, "Inside UpdateImeContextMode(), this = " + this );
                Debug.Indent();

                // If the value is not supported by the Ime, it will be mapped to a corresponding one, we need
                // to update the cached ImeMode to the actual value.

                ImeMode newImeContextMode = ImeMode.Disable;
                ImeMode currentImeMode = CachedImeMode;

                if( ImeSupported && CanEnableIme ) {
                    newImeContextMode = currentImeMode == ImeMode.NoControl ? PropagatingImeMode : currentImeMode;
                }

                // If PropagatingImeMode has not been initialized it will return ImeMode.Inherit above, need to check newImeContextMode for this.
                if (CurrentImeContextMode != newImeContextMode && newImeContextMode != ImeMode.Inherit) {
                    // If the context changes the window will receive one or more WM_IME_NOTIFY messages and as part of its
                    // processing it will raise the ImeModeChanged event if needed.  We need to prevent the event from been 
                    // raised here from here.
                    DisableImeModeChangedCount++;

                    // Setting IME status to Disable will first close the IME and then disable it.  For CHN IME, the first action will 
                    // update the PropagatingImeMode to ImeMode.Close which is incorrect.  We need to save the PropagatingImeMode in 
                    // this case and restore it after the context has been changed.
                    // Also this call here is very important since it will initialize the PropagatingImeMode if not already initialized
                    // before setting the IME context to the control's ImeMode value which could be different from the propagating value.
                    ImeMode savedPropagatingImeMode = PropagatingImeMode;

                    try {
                        ImeContext.SetImeStatus( newImeContextMode, this.Handle );
                    }
                    finally {
                        DisableImeModeChangedCount--;

                        if (newImeContextMode == ImeMode.Disable && inputLanguageTable == ImeModeConversion.ChineseTable) {
                            // Restore saved propagating mode.
                            PropagatingImeMode = savedPropagatingImeMode;
                        }
                    }

                    // Get mapped value from the context.
                    if( currentImeMode == ImeMode.NoControl ) {
                        if( CanEnableIme ) {
                            PropagatingImeMode = CurrentImeContextMode;
                        }
                    }
                    else {
                        if( CanEnableIme ) {
                            CachedImeMode = CurrentImeContextMode;
                        }

                        // Need to raise the ImeModeChanged event?
                        VerifyImeModeChanged( newImeContextMode, CachedImeMode );
                    }
                }

                ImeContext.TraceImeStatus( this );
                Debug.Unindent();
            }
        }

        /// <devdoc>
        ///     Checks if specified ImeMode values are different and raise the event if true.
        /// </devdoc>
        private void VerifyImeModeChanged( ImeMode oldMode, ImeMode newMode ) {
            if( ImeSupported && (DisableImeModeChangedCount == 0) && (newMode != ImeMode.NoControl) ) {
                Debug.WriteLineIf( CompModSwitches.ImeMode.Level >= TraceLevel.Info, string.Format( CultureInfo.CurrentCulture, "Inside VerifyImeModeChanged(oldMode={0}, newMode={1}), this = {2}", oldMode, newMode, this ) );
                Debug.Indent();

                if( oldMode != newMode ) {
                    OnImeModeChanged( EventArgs.Empty );
                }

                Debug.Unindent();
            }
        }

        /// <devdoc>
        ///     Verifies whether the IME context mode is correct based on the control's Ime restriction mode (CanEnableIme) 
        ///     and updates the IME context if needed.
        /// </devdoc>
        internal void VerifyImeRestrictedModeChanged() {
            Debug.Assert( ImeSupported, "This method should not be called from controls that don't support IME input." );

            Debug.WriteLineIf( CompModSwitches.ImeMode.Level >= TraceLevel.Info, "Inside VerifyImeRestrictedModeChanged(), this = " + this );
            Debug.Indent();

            bool currentCanEnableIme = this.CanEnableIme;

            if( LastCanEnableIme != currentCanEnableIme ) {
                if( Focused ) {
                    // Disable ImeModeChanged from the following call since we'll raise it here if needed.
                    DisableImeModeChangedCount++;
                    try {
                        UpdateImeContextMode();
                    }
                    finally {
                        DisableImeModeChangedCount--;
                    }
                }

                // Assume for a moment the control is getting restricted;
                ImeMode oldImeMode = CachedImeMode;
                ImeMode newImeMode = ImeMode.Disable;

                if( currentCanEnableIme ) {
                    // Control is actually getting unrestricted, swap values.
                    newImeMode = oldImeMode;
                    oldImeMode = ImeMode.Disable;
                }

                // Do we need to raise the ImeModeChanged event?
                VerifyImeModeChanged( oldImeMode, newImeMode );

                // Finally update the saved CanEnableIme value.
                LastCanEnableIme = currentCanEnableIme;
            }

            Debug.Unindent();
        }

        /// <devdoc>
        ///     Update internal ImeMode properties (PropagatingImeMode/CachedImeMode) with actual IME context mode if needed.
        ///     This method can be used with a child control when the IME mode is more relevant to it than to the control itself,
        ///     for instance ComboBox and its native ListBox/Edit controls.
        /// </devdoc>
        internal void OnImeContextStatusChanged( IntPtr handle ) {
            Debug.WriteLineIf( CompModSwitches.ImeMode.Level >= TraceLevel.Info, "Inside OnImeContextStatusChanged(), this = " + this );
            Debug.Indent();

            Debug.Assert( ImeSupported, "WARNING: Attempting to update ImeMode properties on IME-Unaware control!" );
            Debug.Assert( !DesignMode, "Shouldn't be updating cached ime mode at design-time" );

            ImeMode fromContext = ImeContext.GetImeMode( handle );

            if( fromContext != ImeMode.Inherit ) {
                ImeMode oldImeMode = CachedImeMode;

                if( CanEnableIme ) { // Cache or Propagating ImeMode should not be updated by interaction when the control is in restricted mode.
                    if( oldImeMode != ImeMode.NoControl ) {
                            CachedImeMode = fromContext; // This could end up in the same value due to ImeMode language mapping.

                            // ImeMode may be changing by user interaction.
                            VerifyImeModeChanged( oldImeMode, CachedImeMode );
                    }
                    else {
                        PropagatingImeMode = fromContext;
                    }
                }
            }

            Debug.Unindent();
        }

        /// <include file='doc\Control.uex' path='docs/doc[@for="Control.OnImeModeChanged"]/*' />
        /// <devdoc>
        /// <para>Raises the <see cref='System.Windows.Forms.Control.OnImeModeChanged'/>
        /// event.</para>
        /// </devdoc>
        protected virtual void OnImeModeChanged( EventArgs e ) {
            Debug.Assert( ImeSupported, "ImeModeChanged should not be raised on an Ime-Unaware control." );
            Debug.WriteLineIf( CompModSwitches.ImeMode.Level >= TraceLevel.Info, "Inside OnImeModeChanged(), this = " + this );
            EventHandler handler = (EventHandler) Events[EventImeModeChanged];
            if( handler != null ) handler( this, e );
        }

        /// <devdoc>
        ///     Resets the Ime mode.
        /// </devdoc>
        [EditorBrowsable( EditorBrowsableState.Never )]
        public void ResetImeMode() {
            Debug.WriteLineIf( CompModSwitches.ImeMode.Level >= TraceLevel.Info, "Inside ResetImeMode(), this = " + this );
            ImeMode = DefaultImeMode;
        }

        /// <devdoc>
        ///     Returns true if the ImeMode should be persisted in code gen.
        /// </devdoc>
        [EditorBrowsable( EditorBrowsableState.Never )]
        internal virtual bool ShouldSerializeImeMode() {
            // This method is for designer support.  If the ImeMode has not been changed or it is the same as the
            // default value it should not be serialized.
            bool found;
            int imeMode = Properties.GetInteger( PropImeMode, out found );

            return ( found && imeMode != (int) DefaultImeMode );
        }

        /// <devdoc>
        ///     Handles the WM_INPUTLANGCHANGE message
        /// </devdoc>
        /// <internalonly/>
        private void WmInputLangChange( ref Message m ) {
            Debug.WriteLineIf( CompModSwitches.ImeMode.Level >= TraceLevel.Info, "Inside WmInputLangChange(), this = " + this );
            Debug.Indent();

            // Make sure the IME context is associated with the correct (mapped) mode.
            UpdateImeContextMode();

            // If detaching IME (setting to English) reset propagating IME mode so when reattaching the IME is set to direct input again.
            if( ImeModeConversion.InputLanguageTable == ImeModeConversion.UnsupportedTable ) {
                PropagatingImeMode = ImeMode.Off;
            }

            if( ImeModeConversion.InputLanguageTable == ImeModeConversion.ChineseTable ) {
                IgnoreWmImeNotify = false;
            }

            Form form = FindFormInternal();

            if( form != null ) {
                InputLanguageChangedEventArgs e = InputLanguage.CreateInputLanguageChangedEventArgs( m );
                Debug.WriteLineIf( CompModSwitches.ImeMode.Level >= TraceLevel.Info, "Culture=" + e.Culture );
                form.PerformOnInputLanguageChanged( e );
            }

            DefWndProc( ref m );

            ImeContext.TraceImeStatus( this );
            Debug.Unindent();
        }


        /// <devdoc>
        ///     Handles the WM_INPUTLANGCHANGEREQUEST message
        /// </devdoc>
        /// <internalonly/>
        private void WmInputLangChangeRequest( ref Message m ) {
            Debug.WriteLineIf( CompModSwitches.ImeMode.Level >= TraceLevel.Info, "Inside WmInputLangChangeRequest(), this=" + this );
            Debug.Indent();

            InputLanguageChangingEventArgs e = InputLanguage.CreateInputLanguageChangingEventArgs( m );
            Form form = FindFormInternal();

            if( form != null ) {
                Debug.WriteLineIf( CompModSwitches.ImeMode.Level >= TraceLevel.Info, "Culture=" + e.Culture );
                form.PerformOnInputLanguageChanging( e );
            }

            if( !e.Cancel ) {
                DefWndProc( ref m );
            }
            else {
                m.Result = IntPtr.Zero;
            }

            Debug.Unindent();
        }

        /// <devdoc>
        ///     Handles the WM_IME_CHAR message
        /// </devdoc>
        private void WmImeChar( ref Message m ) {
            if( ProcessKeyEventArgs( ref m ) ) {
                return;
            }
            DefWndProc( ref m );
        }

        /// <devdoc>
        ///     Handles the WM_IME_ENDCOMPOSITION message
        /// </devdoc>
        private void WmImeEndComposition( ref Message m ) {
            Debug.WriteLineIf( CompModSwitches.ImeMode.Level >= TraceLevel.Info, "Inside WmImeEndComposition() - Disabling ImeWmCharToIgnore, this=" + this );
            this.ImeWmCharsToIgnore = ImeCharsToIgnoreDisabled;
            DefWndProc( ref m );
        }

        /// <devdoc>
        ///     Handles the WM_IME_NOTIFY message
        /// </devdoc>
        private void WmImeNotify( ref Message m ) {

            ImeMode[] inputLanguageTable = ImeModeConversion.InputLanguageTable;

            // During a change to the Chinese language with Focus already set, the Chinese IME will send several WmImeNotify messages 
            // before ever sending a WmInputLangChange event. Also, the IME will report an IME input context during this time that we 
            // interpret as On = 'OnHalf'. The combination of these causes us to update the default Cached ImeMode to OnHalf, overriding 
            // the control's ImeMode property -- unwanted behavior. We workaround this by skipping our mode synchronization during these
            // IMENotify messages until we get a WmInputLangChange event.
            //
            // If this is the first time here after conversion to chinese language, wait for WmInputLanguageChange 
            // before listening to WmImeNotifys.
            if ((inputLanguageTable == ImeModeConversion.ChineseTable) && !lastLanguageChinese) IgnoreWmImeNotify = true;
            lastLanguageChinese = (inputLanguageTable == ImeModeConversion.ChineseTable);

            if( ImeSupported && inputLanguageTable != ImeModeConversion.UnsupportedTable && !IgnoreWmImeNotify) {
                int wparam = (int) m.WParam;

                // The WM_IME_NOTIFY message is not consistent across the different IMEs, particularly the notification type
                // we care about (IMN_SETCONVERSIONMODE & IMN_SETOPENSTATUS).
                // The IMN_SETOPENSTATUS command is sent when the open status of the input context is updated. 
                // The IMN_SETCONVERSIONMODE command is sent when the conversion mode of the input context is updated.
                // - The Korean IME sents both msg notifications when changing the conversion mode (From/To Hangul/Alpha).
                // - The Chinese IMEs sends the IMN_SETCONVERSIONMODE when changing mode (On/Close, Full Shape/Half Shape) 
                //   and IMN_SETOPENSTATUS when getting disabled/enabled or closing/opening as well, but it does not send any
                //   WM_IME_NOTIFY when associating an IME to the app for the first time; setting the IME mode to direct input
                //   during WM_INPUTLANGCHANGED forces the IMN_SETOPENSTATUS message to be sent.
                // - The Japanese IME sends IMN_SETCONVERSIONMODE when changing from Off to one of the active modes (Katakana..) 
                //   and IMN_SETOPENSTATUS when changing beteween the active modes or when enabling/disabling the IME.
                // In any case we update the cache. 
                // Warning: 
                // Attempting to change the IME mode from here will cause re-entrancy - WM_IME_NOTIFY is resent.
                // We guard against re-entrancy since the ImeModeChanged event can be raised and any changes from the handler could 
                // lead to another WM_IME_NOTIFY loop.

                if( wparam == NativeMethods.IMN_SETCONVERSIONMODE || wparam == NativeMethods.IMN_SETOPENSTATUS ) {
                    Debug.WriteLineIf( CompModSwitches.ImeMode.Level >= TraceLevel.Info, string.Format( CultureInfo.CurrentCulture, "Inside WmImeNotify(m.wparam=[{0}]), this={1}", m.WParam, this ) );
                    Debug.Indent();

                    // Synchronize internal properties with the IME context mode.
                    OnImeContextStatusChanged( this.Handle );

                    Debug.Unindent();
                }
            }

            DefWndProc( ref m );
        }

        /// <devdoc>
        ///     Handles the WM_SETFOCUS message for IME related stuff.
        /// </devdoc>
        internal void WmImeSetFocus() {
            if (ImeModeConversion.InputLanguageTable != ImeModeConversion.UnsupportedTable) {
                Debug.WriteLineIf(CompModSwitches.ImeMode.Level >= TraceLevel.Info, "Inside WmImeSetFocus(), this=" + this);
                Debug.Indent(); 
            
                // Make sure the IME context is set to the correct value.
                // Consider - Perf improvement: ContainerControl controls should update the IME context only when they don't contain
                //            a focusable control since it will be updated by that control.
                UpdateImeContextMode();

                Debug.Unindent();
            }
        }

        /// <devdoc>
        ///     Handles the WM_IME_STARTCOMPOSITION message
        /// </devdoc>
        private void WmImeStartComposition( ref Message m ) {
            Debug.WriteLineIf( CompModSwitches.ImeMode.Level >= TraceLevel.Info, "Inside WmImeStartComposition() - Enabling ImeWmCharToIgnore, this=" + this );

            // Need to call the property store directly because the WmImeCharsToIgnore property is locked when ImeCharsToIgnoreDisabled.
            Properties.SetInteger( PropImeWmCharsToIgnore, ImeCharsToIgnoreEnabled );
            DefWndProc( ref m );
        }

        /// <devdoc>
        ///     Handles the WM_KILLFOCUS message
        /// </devdoc>
        /// <internalonly/>
        private void WmImeKillFocus() {
            Debug.WriteLineIf(CompModSwitches.ImeMode.Level >= TraceLevel.Info, "Inside WmImeKillFocus(), this=" + this);
            Debug.Indent();
            
            Control topMostWinformsParent = TopMostParent;
            Form appForm = topMostWinformsParent as Form;

            if( (appForm == null || appForm.Modal) && !topMostWinformsParent.ContainsFocus ) { 
                // This means the winforms component container is not a WinForms host and it is no longer focused.
                // Or it is not the main app host. 

                Debug.WriteLineIf(CompModSwitches.ImeMode.Level >= TraceLevel.Info, "Unfocused TopMostParent = " + topMostWinformsParent);

                // We need to reset the PropagatingImeMode to force reinitialization when the winforms component gets focused again; 
                // this enables inheritting the propagating mode from an unmanaged application hosting a winforms component.
                // But before leaving the winforms container we need to set the IME to the propagating IME mode since the focused control
                // may not support IME which would leave the IME disabled.
                // See the PropagatingImeMode property

                // Note: We need to check the static field here directly to avoid initialization of the property.
                if (Control.propagatingImeMode != ImeMode.Inherit) {
                    // Setting the ime context of the top window will generate a WM_IME_NOTIFY on the focused control which will
                    // update its ImeMode, we need to prevent this temporarily.
                    IgnoreWmImeNotify = true;

                    try {
                        Debug.WriteLineIf(CompModSwitches.ImeMode.Level >= TraceLevel.Verbose, "Setting IME context to PropagatingImeMode (leaving Winforms container). this = " + this);
                        ImeContext.SetImeStatus(PropagatingImeMode, topMostWinformsParent.Handle);

                        Debug.WriteLineIf(CompModSwitches.ImeMode.Level >= TraceLevel.Verbose, "Resetting PropagatingImeMode. this = " + this);
                        PropagatingImeMode = ImeMode.Inherit;
                    }
                    finally {
                        IgnoreWmImeNotify = false;
                    }
                }
            }


            Debug.Unindent();
        }
    } // end class Control


    ///////////////////////////////////////////////////////// ImeContext class /////////////////////////////////////////////////////////

    /// <devdoc>
    ///     Represents the native IME context.
    /// </devdoc>
    public static class ImeContext {
        /// <devdoc>
        ///     The IME context handle obtained when first associating an IME.
        /// </devdoc>
        private static IntPtr originalImeContext;

        /// <devdoc>
        ///     Disable the IME
        /// </devdoc>
        public static void Disable( IntPtr handle ) {
            Debug.WriteLineIf( CompModSwitches.ImeMode.Level >= TraceLevel.Info, "Inside ImeContext.Disable(" + handle + ")" );
            Debug.Indent();

            if( ImeModeConversion.InputLanguageTable != ImeModeConversion.UnsupportedTable ) {
                // Close the IME if necessary
                //
                if( ImeContext.IsOpen( handle ) ) {
                    ImeContext.SetOpenStatus( false, handle );
                }

                // Disable the IME by disassociating the context from the window.
                //
                Debug.WriteLineIf( CompModSwitches.ImeMode.Level >= TraceLevel.Verbose, "ImmAssociateContext(" + handle + ", null)" );
                IntPtr oldContext = UnsafeNativeMethods.ImmAssociateContext( new HandleRef( null, handle ), NativeMethods.NullHandleRef );
                if( oldContext != IntPtr.Zero ) {
                    originalImeContext = oldContext;
                }
            }

            ImeContext.TraceImeStatus( handle );
            Debug.Unindent();
        }

        /// <devdoc>
        ///     Enable the IME
        /// </devdoc>
        public static void Enable( IntPtr handle ) {
            Debug.WriteLineIf( CompModSwitches.ImeMode.Level >= TraceLevel.Info, "Inside ImeContext.Enable(" + handle + ")" );
            Debug.Indent();

            if( ImeModeConversion.InputLanguageTable != ImeModeConversion.UnsupportedTable ) {
                Debug.WriteLineIf( CompModSwitches.ImeMode.Level >= TraceLevel.Verbose, "ImmGetContext(" + handle + ")" );
                IntPtr inputContext = UnsafeNativeMethods.ImmGetContext( new HandleRef( null, handle ) );
                Debug.WriteLineIf( CompModSwitches.ImeMode.Level >= TraceLevel.Verbose, "context = " + inputContext );

                // Enable IME by associating the IME context to the window.
                if( inputContext == IntPtr.Zero ) {
                    if( originalImeContext == IntPtr.Zero ) {
                        Debug.WriteLineIf( CompModSwitches.ImeMode.Level >= TraceLevel.Verbose, "ImmCreateContext()" );
                        inputContext = UnsafeNativeMethods.ImmCreateContext();
                        if( inputContext != IntPtr.Zero ) {
                            Debug.WriteLineIf( CompModSwitches.ImeMode.Level >= TraceLevel.Verbose, "ImmAssociateContext(" + handle + ", " + inputContext + ")" );
                            UnsafeNativeMethods.ImmAssociateContext( new HandleRef( null, handle ), new HandleRef( null, inputContext ) );
                        }
                    }
                    else {
                        Debug.WriteLineIf( CompModSwitches.ImeMode.Level >= TraceLevel.Verbose, "ImmAssociateContext()" );
                        UnsafeNativeMethods.ImmAssociateContext( new HandleRef( null, handle ), new HandleRef( null, originalImeContext ) );
                    }
                }
                else {
                    Debug.WriteLineIf( CompModSwitches.ImeMode.Level >= TraceLevel.Verbose, "ImmReleaseContext(" + handle + ", " + inputContext + ")" );
                    UnsafeNativeMethods.ImmReleaseContext( new HandleRef( null, handle ), new HandleRef( null, inputContext ) );
                }

                // Make sure the IME is opened.
                if( !ImeContext.IsOpen( handle ) ) {
                    ImeContext.SetOpenStatus( true, handle );
                }
            }

            ImeContext.TraceImeStatus( handle );
            Debug.Unindent();
        }

        /// <devdoc>
        ///     Gets the ImeMode that corresponds to ImeMode.Disable based on the current input language ImeMode table.
        /// </devdoc>
        public static ImeMode GetImeMode( IntPtr handle ) {
            Debug.WriteLineIf( CompModSwitches.ImeMode.Level >= TraceLevel.Info, "Insise ImeContext.GetImeMode(" + handle + ")" );
            Debug.Indent();

            IntPtr inputContext = IntPtr.Zero;
            ImeMode retval = ImeMode.NoControl;

            // Get the right table for the current keyboard layout
            //
            ImeMode[] countryTable = ImeModeConversion.InputLanguageTable;
            if( countryTable == ImeModeConversion.UnsupportedTable ) {
                // No IME associated with current culture. 
                retval = ImeMode.Inherit;
                goto cleanup;
            }

            Debug.WriteLineIf( CompModSwitches.ImeMode.Level >= TraceLevel.Verbose, "ImmGetContext(" + handle + ")" );
            inputContext = UnsafeNativeMethods.ImmGetContext( new HandleRef( null, handle ) );
            Debug.WriteLineIf( CompModSwitches.ImeMode.Level >= TraceLevel.Verbose, "context = " + inputContext );

            if( inputContext == IntPtr.Zero ) {
                // No IME context attached - The Ime has been disabled.
                retval = ImeMode.Disable;
                goto cleanup;
            }

            if( !ImeContext.IsOpen( handle ) ) {
                // There's an IME associated with the window but is closed - the input is taken from the keyboard as is (English).
                retval = countryTable[ImeModeConversion.ImeClosed];
                goto cleanup;
            }

            // Determine the IME mode from the conversion status
            //

            int conversion = 0;     // Combination of conversion mode values
            int sentence = 0;       // Sentence mode value

            Debug.WriteLineIf( CompModSwitches.ImeMode.Level >= TraceLevel.Verbose, "ImmGetConversionStatus(" + inputContext + ", conversion, sentence)" );
            UnsafeNativeMethods.ImmGetConversionStatus( new HandleRef( null, inputContext ), ref conversion, ref sentence );

            Debug.Assert( countryTable != null, "countryTable is null" );

            if( ( conversion & NativeMethods.IME_CMODE_NATIVE ) != 0 ) {
                if( ( conversion & NativeMethods.IME_CMODE_KATAKANA ) != 0 ) {
                    retval = ( ( conversion & NativeMethods.IME_CMODE_FULLSHAPE ) != 0 )
                                ? countryTable[ImeModeConversion.ImeNativeFullKatakana]
                                : countryTable[ImeModeConversion.ImeNativeHalfKatakana];
                    goto cleanup;
                }
                else { // ! Katakana
                    retval = ( ( conversion & NativeMethods.IME_CMODE_FULLSHAPE ) != 0 )
                                ? countryTable[ImeModeConversion.ImeNativeFullHiragana]
                                : countryTable[ImeModeConversion.ImeNativeHalfHiragana];
                    goto cleanup;
                }
            }
            else { // ! IME_CMODE_NATIVE
                retval = ( ( conversion & NativeMethods.IME_CMODE_FULLSHAPE ) != 0 )
                            ? countryTable[ImeModeConversion.ImeAlphaFull]
                            : countryTable[ImeModeConversion.ImeAlphaHalf];
            }

        cleanup:
            if( inputContext != IntPtr.Zero ) {
                Debug.WriteLineIf( CompModSwitches.ImeMode.Level >= TraceLevel.Verbose, "ImmReleaseContext(" + handle + ", " + inputContext + ")" );
                UnsafeNativeMethods.ImmReleaseContext( new HandleRef( null, handle ), new HandleRef( null, inputContext ) );
            }

            ImeContext.TraceImeStatus( handle );

            Debug.Unindent();
            return retval;
        }

        /// <devdoc>
        ///     Get the actual IME status - This method is for debugging purposes only.
        /// </devdoc>
        [Conditional( "DEBUG" )]
        internal static void TraceImeStatus( Control ctl ) {
#if DEBUG
            if ( ctl.IsHandleCreated ){
                TraceImeStatus( ctl.Handle );
            }
#endif
        }

        [Conditional( "DEBUG" )]
        private static void TraceImeStatus( IntPtr handle ) {
#if DEBUG
            if (CompModSwitches.ImeMode.Level >= TraceLevel.Info) {
                string status = "?";
                IntPtr inputContext = IntPtr.Zero;
                ImeMode[] countryTable = ImeModeConversion.InputLanguageTable;

                if (countryTable == ImeModeConversion.UnsupportedTable) {
                    status = "IME not supported in current language.";
                    goto cleanup;
                }

                inputContext = UnsafeNativeMethods.ImmGetContext(new HandleRef(null, handle));

                if (inputContext == IntPtr.Zero) {
                    status = string.Format(CultureInfo.CurrentCulture, "No ime context for handle=[{0}]", handle);
                    goto cleanup;
                }

                if (!UnsafeNativeMethods.ImmGetOpenStatus(new HandleRef(null, inputContext))) {
                    status = string.Format(CultureInfo.CurrentCulture, "Ime closed for handle=[{0}]", handle);
                    goto cleanup;
                }

                int conversion = 0;     // Combination of conversion mode values
                int sentence = 0;       // Sentence mode value

                UnsafeNativeMethods.ImmGetConversionStatus(new HandleRef(null, inputContext), ref conversion, ref sentence);
                ImeMode retval;

                if ((conversion & NativeMethods.IME_CMODE_NATIVE) != 0) {
                    if ((conversion & NativeMethods.IME_CMODE_KATAKANA) != 0) {
                        retval = ((conversion & NativeMethods.IME_CMODE_FULLSHAPE) != 0)
                                    ? countryTable[ImeModeConversion.ImeNativeFullKatakana]
                                    : countryTable[ImeModeConversion.ImeNativeHalfKatakana];
                    }
                    else { // ! Katakana
                        retval = ((conversion & NativeMethods.IME_CMODE_FULLSHAPE) != 0)
                                    ? countryTable[ImeModeConversion.ImeNativeFullHiragana]
                                    : countryTable[ImeModeConversion.ImeNativeHalfHiragana];
                    }
                }
                else { // ! IME_CMODE_NATIVE
                    retval = ((conversion & NativeMethods.IME_CMODE_FULLSHAPE) != 0)
                                ? countryTable[ImeModeConversion.ImeAlphaFull]
                                : countryTable[ImeModeConversion.ImeAlphaHalf];
                }

                status = string.Format(CultureInfo.CurrentCulture, "Ime conversion status mode for handle=[{0}]: {1}", handle, retval);

            cleanup:
                if (inputContext != IntPtr.Zero) {
                    UnsafeNativeMethods.ImmReleaseContext(new HandleRef(null, handle), new HandleRef(null, inputContext));
                }

                Debug.WriteLine(status);
            }
#endif
        }

        /// <devdoc>
        ///     Returns true if the IME is currently open
        /// </devdoc>
        public static bool IsOpen( IntPtr handle ) {
            Debug.WriteLineIf( CompModSwitches.ImeMode.Level >= TraceLevel.Info, "Inside ImeContext.IsOpen(" + handle + ")" );
            Debug.Indent();

            Debug.WriteLineIf( CompModSwitches.ImeMode.Level >= TraceLevel.Verbose, "ImmGetContext(" + handle + ")" );
            IntPtr inputContext = UnsafeNativeMethods.ImmGetContext( new HandleRef( null, handle ) );
            Debug.WriteLineIf( CompModSwitches.ImeMode.Level >= TraceLevel.Verbose, "context = " + inputContext );

            bool retval = false;

            if( inputContext != IntPtr.Zero ) {
                Debug.WriteLineIf( CompModSwitches.ImeMode.Level >= TraceLevel.Verbose, "ImmGetOpenStatus(" + inputContext + ")" );
                retval = UnsafeNativeMethods.ImmGetOpenStatus( new HandleRef( null, inputContext ) );
                Debug.WriteLineIf( CompModSwitches.ImeMode.Level >= TraceLevel.Verbose, "ImmReleaseContext(" + handle + ", " + inputContext + ")" );
                UnsafeNativeMethods.ImmReleaseContext( new HandleRef( null, handle ), new HandleRef( null, inputContext ) );
            }

            Debug.WriteLineIf( CompModSwitches.ImeMode.Level >= TraceLevel.Verbose, "    IsOpen = " + retval );
            Debug.Unindent();

            return retval;
        }

        /// <devdoc>
        ///     Sets the actual IME context value.
        /// </devdoc>
        public static void SetImeStatus( ImeMode imeMode, IntPtr handle ) {
            Debug.WriteLineIf( CompModSwitches.ImeMode.Level >= TraceLevel.Info, string.Format( CultureInfo.CurrentCulture, "Inside ImeContext.SetImeStatus(ImeMode=[{0}, handle=[{1}])", imeMode, handle ) );
            Debug.Indent();

            Debug.Assert( imeMode != ImeMode.Inherit, "ImeMode.Inherit is an invalid argument to ImeContext.SetImeStatus" );

            if( imeMode == ImeMode.Inherit || imeMode == ImeMode.NoControl ) {
                goto cleanup;     // No action required
            }

            ImeMode[] inputLanguageTable = ImeModeConversion.InputLanguageTable;

            if (inputLanguageTable == ImeModeConversion.UnsupportedTable) {
                goto cleanup;     // We only support Japanese, Korean and Chinese IME.
            }

            int conversion = 0;     // Combination of conversion mode values
            int sentence = 0;       // Sentence mode value

            Debug.WriteLineIf( CompModSwitches.ImeMode.Level >= TraceLevel.Warning, "\tSetting IME context to " + imeMode );

            if( imeMode == ImeMode.Disable ) {
                ImeContext.Disable( handle );
            }
            else {
                // This will make sure the IME is opened.
                ImeContext.Enable( handle );
            }

            switch( imeMode ) {
                case ImeMode.NoControl:
                case ImeMode.Disable:
                    break;     // No action required

                case ImeMode.On:
                    // IME active mode (CHN = On, JPN = Hiragana, KOR = Hangul).
                    // Setting ImeMode to Hiragana (or any other active value) will force the IME to get to an active value
                    // independent on the language.
                    imeMode = ImeMode.Hiragana;
                    goto default;

                case ImeMode.Off:
                    // IME direct input (CHN = Off, JPN = Off, KOR = Alpha).
                    if (inputLanguageTable == ImeModeConversion.JapaneseTable) {
                        // Japanese IME interprets Close as Off.
                        goto case ImeMode.Close;
                    }

                    // CHN: to differentiate between Close and Off we set the ImeMode to Alpha.
                    imeMode = ImeMode.Alpha;
                    goto default;

                case ImeMode.Close:
                    if (inputLanguageTable == ImeModeConversion.KoreanTable) {
                        // Korean IME has no idea what Close means.
                        imeMode = ImeMode.Alpha;
                        goto default;
                    }

                    ImeContext.SetOpenStatus( false, handle );
                    break;

                default:
                    if( ImeModeConversion.ImeModeConversionBits.ContainsKey( imeMode ) ) {

                        // Update the conversion status
                        //
                        ImeModeConversion conversionEntry = (ImeModeConversion) ImeModeConversion.ImeModeConversionBits[imeMode];

                        Debug.WriteLineIf( CompModSwitches.ImeMode.Level >= TraceLevel.Verbose, "ImmGetContext(" + handle + ")" );
                        IntPtr inputContext = UnsafeNativeMethods.ImmGetContext( new HandleRef( null, handle ) );
                        Debug.WriteLineIf( CompModSwitches.ImeMode.Level >= TraceLevel.Verbose, "context = " + inputContext );
                        Debug.WriteLineIf( CompModSwitches.ImeMode.Level >= TraceLevel.Verbose, "ImmGetConversionStatus(" + inputContext + ", conversion, sentence)" );
                        UnsafeNativeMethods.ImmGetConversionStatus( new HandleRef( null, inputContext ), ref conversion, ref sentence );

                        conversion |= conversionEntry.setBits;
                        conversion &= ~conversionEntry.clearBits;

                        Debug.WriteLineIf( CompModSwitches.ImeMode.Level >= TraceLevel.Verbose, "ImmSetConversionStatus(" + inputContext + ", conversion, sentence)" );
                        bool retval = UnsafeNativeMethods.ImmSetConversionStatus( new HandleRef( null, inputContext ), conversion, sentence );
                        Debug.WriteLineIf( CompModSwitches.ImeMode.Level >= TraceLevel.Verbose, "ImmReleaseContext(" + handle + ", " + inputContext + ")" );
                        UnsafeNativeMethods.ImmReleaseContext( new HandleRef( null, handle ), new HandleRef( null, inputContext ) );
                    }
                    break;
            }

        cleanup:
            ImeContext.TraceImeStatus( handle );
            Debug.Unindent();
        }

        /// <devdoc>
        ///     Opens or closes the IME context.
        /// </devdoc>
        public static void SetOpenStatus( bool open, IntPtr handle ) {
            Debug.WriteLineIf( CompModSwitches.ImeMode.Level >= TraceLevel.Info, string.Format( CultureInfo.CurrentCulture, "Inside SetOpenStatus(open=[{0}], handle=[{1}])", open, handle ) );
            Debug.Indent();

            if( ImeModeConversion.InputLanguageTable != ImeModeConversion.UnsupportedTable ) {
                Debug.WriteLineIf( CompModSwitches.ImeMode.Level >= TraceLevel.Verbose, "ImmGetContext(" + handle + ")" );
                IntPtr inputContext = UnsafeNativeMethods.ImmGetContext( new HandleRef( null, handle ) );
                Debug.WriteLineIf( CompModSwitches.ImeMode.Level >= TraceLevel.Verbose, "context = " + inputContext );

                if( inputContext != IntPtr.Zero ) {
                    Debug.WriteLineIf( CompModSwitches.ImeMode.Level >= TraceLevel.Verbose, "ImmSetOpenStatus(" + inputContext + ", " + open + ")" );
                    bool succeeded = UnsafeNativeMethods.ImmSetOpenStatus( new HandleRef( null, inputContext ), open );
                    Debug.Assert( succeeded, "Could not set the IME open status." );

                    if( succeeded ) {
                        Debug.WriteLineIf( CompModSwitches.ImeMode.Level >= TraceLevel.Verbose, "ImmReleaseContext(" + handle + ", " + inputContext + ")" );
                        succeeded = UnsafeNativeMethods.ImmReleaseContext( new HandleRef( null, handle ), new HandleRef( null, inputContext ) );
                        Debug.Assert( succeeded, "Could not release IME context." );
                    }
                }
            }

            ImeContext.TraceImeStatus( handle );
            Debug.Unindent();
        }
    }// end ImeContext class


    ///////////////////////////////////////////////////////// ImeModeConversion structure /////////////////////////////////////////////////////////

    /// <devdoc>
    ///     Helper class that provides information about IME convertion mode.  Convertion mode refers to how IME interprets input like
    ///     ALPHANUMERIC or HIRAGANA and depending on its value the IME enables/disables the IME convertion window appropriately.
    /// </devdoc>
    [SuppressMessage("Microsoft.Performance", "CA1815:OverrideEqualsAndOperatorEqualsOnValueTypes")] // this class has no public instance memebers.
    public struct ImeModeConversion
    {
        private static Dictionary<ImeMode, ImeModeConversion> imeModeConversionBits;

        internal int setBits;
        internal int clearBits;

        // Tables of conversions from IME context bits to IME mode
        //
        //internal const int ImeNotAvailable = 0;
        internal const int ImeDisabled = 1;
        internal const int ImeDirectInput = 2;
        internal const int ImeClosed = 3;
        internal const int ImeNativeInput = 4;
        internal const int ImeNativeFullHiragana = 4; // Index of Native Input Mode.
        internal const int ImeNativeHalfHiragana = 5;
        internal const int ImeNativeFullKatakana = 6;
        internal const int ImeNativeHalfKatakana = 7;
        internal const int ImeAlphaFull = 8;
        internal const int ImeAlphaHalf = 9;

        /// <devdoc>
        ///     Supported input language ImeMode tables.
        ///		WARNING: Do not try to map 'active' IME modes from one table to another since they can have a different
        ///				 meaning depending on the language; for instance ImeMode.Off means 'disable' or 'alpha' to Chinese
        ///				 but to Japanese it is 'alpha' and to Korean it has no meaning.
        /// </devdoc>
        private static ImeMode[] japaneseTable = {
            ImeMode.Inherit,
            ImeMode.Disable,
            ImeMode.Off,
            ImeMode.Off,
            ImeMode.Hiragana,
            ImeMode.Hiragana,
            ImeMode.Katakana,
            ImeMode.KatakanaHalf,
            ImeMode.AlphaFull,
            ImeMode.Alpha
        };

        private static ImeMode[] koreanTable = {
            ImeMode.Inherit,
            ImeMode.Disable,
            ImeMode.Alpha,
            ImeMode.Alpha,
            ImeMode.HangulFull,
            ImeMode.Hangul,
            ImeMode.HangulFull,
            ImeMode.Hangul,
            ImeMode.AlphaFull,
            ImeMode.Alpha
        };

        private static ImeMode[] chineseTable = {
            ImeMode.Inherit,
            ImeMode.Disable,
            ImeMode.Off,
            ImeMode.Close,
            ImeMode.On,
            ImeMode.OnHalf,
            ImeMode.On,
            ImeMode.OnHalf,
            ImeMode.Off,
            ImeMode.Off 
        };

        private static ImeMode[] unsupportedTable = {
        };


        internal static ImeMode[] ChineseTable {
            get {
                return chineseTable;
            }
        }

        internal static ImeMode[] JapaneseTable {
            get {
                return japaneseTable;
            }
        }

        internal static ImeMode[] KoreanTable {
            get {
                return koreanTable;
            }
        }

        internal static ImeMode[] UnsupportedTable {
            get {
                return unsupportedTable;
            }
        }

        /// <devdoc>
        ///     Gets the ImeMode table of the current input language.  
        ///     Although this property is per-thread based we cannot cache it and share it among controls running in the same thread
        ///     for two main reasons: we still have some controls that don't handle IME properly (TabControl, ComboBox, TreeView...)
        ///     and would render it invalid and since the IME API is not public third party controls would not have a way to update
        ///     the cached value.
        /// </devdoc>
        internal static ImeMode[] InputLanguageTable {
            get {
                Debug.WriteLineIf(CompModSwitches.ImeMode.Level >= TraceLevel.Info, "Inside get_InputLanguageTable(), Input Language = " + InputLanguage.CurrentInputLanguage.Culture.DisplayName + ", Thread = " + System.Threading.Thread.CurrentThread.ManagedThreadId);
                InputLanguage inputLanguage = InputLanguage.CurrentInputLanguage;

                int lcid = (int)((long)inputLanguage.Handle & (long)0xFFFF);

                switch (lcid) {
                    case 0x0404:    
                    case 0x0804:    
                    case 0x0c04:    
                    case 0x1004:    
                    case 0x1404:    
                        return chineseTable;

                    case 0x0412:    // Korean
                    case 0x0812:    // Korean (Johab)
                        return koreanTable;

                    case 0x0411:    // Japanese
                        return japaneseTable;

                    default:
                        return unsupportedTable;
                }
            }
        }

        /// <devdoc>
        ///     Dictionary of ImeMode and corresponding convertion flags.
        /// </devdoc>
        public static Dictionary<ImeMode, ImeModeConversion> ImeModeConversionBits {
            get {
                if( imeModeConversionBits == null ) {

                    // Create ImeModeConversionBits dictionary
                    imeModeConversionBits = new Dictionary<ImeMode, ImeModeConversion>( 7 );
                    ImeModeConversion conversion;

                    // Hiragana, On
                    //
                    conversion.setBits = NativeMethods.IME_CMODE_FULLSHAPE | NativeMethods.IME_CMODE_NATIVE;
                    conversion.clearBits = NativeMethods.IME_CMODE_KATAKANA;
                    imeModeConversionBits.Add( ImeMode.Hiragana, conversion );

                    // Katakana
                    //
                    conversion.setBits = NativeMethods.IME_CMODE_FULLSHAPE | NativeMethods.IME_CMODE_KATAKANA | NativeMethods.IME_CMODE_NATIVE;
                    conversion.clearBits = 0;
                    imeModeConversionBits.Add( ImeMode.Katakana, conversion );

                    // KatakanaHalf
                    //
                    conversion.setBits = NativeMethods.IME_CMODE_KATAKANA | NativeMethods.IME_CMODE_NATIVE;
                    conversion.clearBits = NativeMethods.IME_CMODE_FULLSHAPE;
                    imeModeConversionBits.Add( ImeMode.KatakanaHalf, conversion );

                    // AlphaFull
                    //
                    conversion.setBits = NativeMethods.IME_CMODE_FULLSHAPE;
                    conversion.clearBits = NativeMethods.IME_CMODE_KATAKANA | NativeMethods.IME_CMODE_NATIVE;
                    imeModeConversionBits.Add( ImeMode.AlphaFull, conversion );

                    // Alpha
                    //
                    conversion.setBits = 0;
                    conversion.clearBits = NativeMethods.IME_CMODE_FULLSHAPE | NativeMethods.IME_CMODE_KATAKANA | NativeMethods.IME_CMODE_NATIVE;
                    imeModeConversionBits.Add( ImeMode.Alpha, conversion );

                    // HangulFull
                    //
                    conversion.setBits = NativeMethods.IME_CMODE_FULLSHAPE | NativeMethods.IME_CMODE_NATIVE;
                    conversion.clearBits = 0;
                    imeModeConversionBits.Add( ImeMode.HangulFull, conversion );

                    // Hangul
                    //
                    conversion.setBits = NativeMethods.IME_CMODE_NATIVE;
                    conversion.clearBits = NativeMethods.IME_CMODE_FULLSHAPE;
                    imeModeConversionBits.Add( ImeMode.Hangul, conversion );

                    // OnHalf
                    //
                    conversion.setBits = NativeMethods.IME_CMODE_NATIVE;
                    conversion.clearBits = NativeMethods.IME_CMODE_KATAKANA | NativeMethods.IME_CMODE_FULLSHAPE;
                    imeModeConversionBits.Add(ImeMode.OnHalf, conversion);
                }

                return imeModeConversionBits;
            }
        }

        public static bool IsCurrentConversionTableSupported{
            get {
                return InputLanguageTable != UnsupportedTable;
            }
        }
    }// end ImeModeConversion struct.
} // end namespace System.Windows.Forms
