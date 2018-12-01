// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace System.Windows.Forms {
    using System;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using Microsoft.Win32;
    using Hashtable = System.Collections.Hashtable;
    using System.Windows.Forms;
    using System.Windows.Forms.Design;
    using System.Diagnostics;
    using System.Drawing;
    using System.Drawing.Design;
    

    /// <include file='doc\HelpProvider.uex' path='docs/doc[@for="HelpProvider"]/*' />
    /// <devdoc>
    ///    <para>
    ///       Provides pop-up or online Help for controls.
    ///    </para>
    /// </devdoc>
    [
    ProvideProperty("HelpString", typeof(Control)),
    ProvideProperty("HelpKeyword", typeof(Control)),
    ProvideProperty("HelpNavigator", typeof(Control)),
    ProvideProperty("ShowHelp", typeof(Control)),
    ToolboxItemFilter("System.Windows.Forms"),
    SRDescription(nameof(SR.DescriptionHelpProvider))
    ]
    public class HelpProvider : Component, IExtenderProvider {

        private string helpNamespace = null;
        private Hashtable helpStrings = new Hashtable();
        private Hashtable showHelp = new Hashtable();
        private Hashtable boundControls = new Hashtable();
        private Hashtable keywords = new Hashtable();
        private Hashtable navigators = new Hashtable();

        private object userData;

        /// <include file='doc\HelpProvider.uex' path='docs/doc[@for="HelpProvider.HelpProvider"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Initializes a new instance of the <see cref='System.Windows.Forms.HelpProvider'/> class.
        ///    </para>
        /// </devdoc>
        public HelpProvider() {
        }

        /// <include file='doc\HelpProvider.uex' path='docs/doc[@for="HelpProvider.HelpNamespace"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Gets or sets a string indicating the name of the Help
        ///       file associated with this <see cref='System.Windows.Forms.HelpProvider'/> object.
        ///    </para>
        /// </devdoc>
        [
        Localizable(true),
        DefaultValue(null),
        Editor("System.Windows.Forms.Design.HelpNamespaceEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor)),
        SRDescription(nameof(SR.HelpProviderHelpNamespaceDescr))
        ]
        public virtual string HelpNamespace {
            get {
                return helpNamespace;
            }

            set {
                this.helpNamespace = value;
            }
        }

        /// <include file='doc\HelpProvider.uex' path='docs/doc[@for="HelpProvider.Tag"]/*' />
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

        /// <include file='doc\HelpProvider.uex' path='docs/doc[@for="HelpProvider.CanExtend"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Determines if the help provider can offer it's extender properties
        ///       to the specified target object.
        ///    </para>
        /// </devdoc>
        public virtual bool CanExtend(object target) {
            return(target is Control);
        }

        /// <include file='doc\HelpProvider.uex' path='docs/doc[@for="HelpProvider.GetHelpKeyword"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Retrieves the Help Keyword displayed when the
        ///       user invokes Help for the specified control.
        ///    </para>
        /// </devdoc>
        [
        DefaultValue(null),
        Localizable(true),
        SRDescription(nameof(SR.HelpProviderHelpKeywordDescr))
        ]
        public virtual string GetHelpKeyword(Control ctl) {
            return(string)keywords[ctl];
        }

        /// <include file='doc\HelpProvider.uex' path='docs/doc[@for="HelpProvider.GetHelpNavigator"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Retrieves the contents of the pop-up help window for the specified
        ///       control.
        ///    </para>
        /// </devdoc>
        [
        DefaultValue(HelpNavigator.AssociateIndex),
        Localizable(true),
        SRDescription(nameof(SR.HelpProviderNavigatorDescr))
        ]
        public virtual HelpNavigator GetHelpNavigator(Control ctl) {
            object nav = navigators[ctl];
            return (nav == null) ? HelpNavigator.AssociateIndex : (HelpNavigator)nav;
        }

        /// <include file='doc\HelpProvider.uex' path='docs/doc[@for="HelpProvider.GetHelpString"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Retrieves the contents of the pop-up help window for the specified
        ///       control.
        ///    </para>
        /// </devdoc>
        [
        DefaultValue(null),
        Localizable(true),
        SRDescription(nameof(SR.HelpProviderHelpStringDescr))
        ]
        public virtual string GetHelpString(Control ctl) {
            return(string)helpStrings[ctl];
        }

        /// <include file='doc\HelpProvider.uex' path='docs/doc[@for="HelpProvider.GetShowHelp"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Retrieves a value indicating whether Help displays for
        ///       the specified control.
        ///    </para>
        /// </devdoc>
        [
        Localizable(true),
        SRDescription(nameof(SR.HelpProviderShowHelpDescr))
        ]
        public virtual bool GetShowHelp(Control ctl) {
            object b = showHelp[ctl];
            if (b == null) {
                return false;
            }
            else {
                return(Boolean) b;
            }
        }

        /// <include file='doc\HelpProvider.uex' path='docs/doc[@for="HelpProvider.OnControlHelp"]/*' />
        /// <devdoc>
        ///     Handles the help event for any bound controls.
        /// </devdoc>
        /// <internalonly/>
        private void OnControlHelp(object sender, HelpEventArgs hevent) {
            Control ctl = (Control)sender;
            string helpString = GetHelpString(ctl);
            string keyword = GetHelpKeyword(ctl);
            HelpNavigator navigator = GetHelpNavigator(ctl);
            bool show = GetShowHelp(ctl);

            if (!show) {
                return;
            }

            // If the mouse was down, we first try whats this help
            //
            if (Control.MouseButtons != MouseButtons.None && helpString != null) {
                Debug.WriteLineIf(Help.WindowsFormsHelpTrace.TraceVerbose, "HelpProvider:: Mouse down w/ helpstring");

                if (helpString.Length > 0) {
                    Help.ShowPopup(ctl, helpString, hevent.MousePos);
                    hevent.Handled = true;
                }
            }

            // If we have a help file, and help keyword we try F1 help next
            //
            if (!hevent.Handled && helpNamespace != null) {
                Debug.WriteLineIf(Help.WindowsFormsHelpTrace.TraceVerbose, "HelpProvider:: F1 help");
                if (keyword != null) {
                    if (keyword.Length > 0) {
                        Help.ShowHelp(ctl, helpNamespace, navigator, keyword);
                        hevent.Handled = true;
                    }
                }

                if (!hevent.Handled) {
                    Help.ShowHelp(ctl, helpNamespace, navigator);
                    hevent.Handled = true;
                }
            }

            // So at this point we don't have a help keyword, so try to display
            // the whats this help
            //
            if (!hevent.Handled && helpString != null) {
                Debug.WriteLineIf(Help.WindowsFormsHelpTrace.TraceVerbose, "HelpProvider:: back to helpstring");

                if (helpString.Length > 0) {
                    Help.ShowPopup(ctl, helpString, hevent.MousePos);
                    hevent.Handled = true;
                }
            }

            // As a last resort, just popup the contents page of the help file...
            //
            if (!hevent.Handled && helpNamespace != null) {
                Debug.WriteLineIf(Help.WindowsFormsHelpTrace.TraceVerbose, "HelpProvider:: contents");

                Help.ShowHelp(ctl, helpNamespace);
                hevent.Handled = true;
            }
        }
        
        /// <include file='doc\HelpProvider.uex' path='docs/doc[@for="HelpProvider.OnQueryAccessibilityHelp"]/*' />
        /// <devdoc>
        ///     Handles the help event for any bound controls.
        /// </devdoc>
        /// <internalonly/>
        private void OnQueryAccessibilityHelp(object sender, QueryAccessibilityHelpEventArgs e) {
            Control ctl = (Control)sender;
            
            e.HelpString = GetHelpString(ctl);
            e.HelpKeyword = GetHelpKeyword(ctl);
            e.HelpNamespace = HelpNamespace;
        }    

        /// <include file='doc\HelpProvider.uex' path='docs/doc[@for="HelpProvider.SetHelpString"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Specifies
        ///       a Help string associated with a control.
        ///    </para>
        /// </devdoc>
        public virtual void SetHelpString(Control ctl, string helpString) {
            helpStrings[ctl] = helpString;
            if (helpString != null) {
                if (helpString.Length > 0) {
                    SetShowHelp(ctl, true);
                }
            }
            UpdateEventBinding(ctl);
        }

        /// <include file='doc\HelpProvider.uex' path='docs/doc[@for="HelpProvider.SetHelpKeyword"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Specifies the Help keyword to display when
        ///       the user invokes Help for a control.
        ///    </para>
        /// </devdoc>
        public virtual void SetHelpKeyword(Control ctl, string keyword) {
            keywords[ctl] = keyword;
            if (keyword != null) {
                if (keyword.Length > 0) {
                    SetShowHelp(ctl, true);
                }
            }
            UpdateEventBinding(ctl);
        }

        /// <include file='doc\HelpProvider.uex' path='docs/doc[@for="HelpProvider.SetHelpNavigator"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Specifies the Help keyword to display when
        ///       the user invokes Help for a control.
        ///    </para>
        /// </devdoc>
        public virtual void SetHelpNavigator(Control ctl, HelpNavigator navigator) {
            //valid values are 0x80000001 to 0x80000007
            if (!ClientUtils.IsEnumValid(navigator, (int)navigator, (int)HelpNavigator.Topic, (int)HelpNavigator.TopicId)){
                //validate the HelpNavigator enum
                throw new InvalidEnumArgumentException(nameof(navigator), (int)navigator, typeof(HelpNavigator));
            }

            navigators[ctl] = navigator;
            SetShowHelp(ctl, true);
            UpdateEventBinding(ctl);
        }
        /// <include file='doc\HelpProvider.uex' path='docs/doc[@for="HelpProvider.SetShowHelp"]/*' />
        /// <devdoc>
        ///    <para>
        ///       Specifies whether Help is displayed for a given control.
        ///    </para>
        /// </devdoc>
        public virtual void SetShowHelp(Control ctl, bool value) {
            showHelp[ ctl] = value ;
            UpdateEventBinding(ctl);
        }

        /// <include file='doc\HelpProvider.uex' path='docs/doc[@for="HelpProvider.ShouldSerializeShowHelp"]/*' />
        /// <devdoc>
        ///    Used by the designer
        /// </devdoc>
        /// <internalonly/>
        internal virtual bool ShouldSerializeShowHelp(Control ctl) {
            return showHelp.ContainsKey(ctl);
        }

        /// <include file='doc\HelpProvider.uex' path='docs/doc[@for="HelpProvider.ResetShowHelp"]/*' />
        /// <devdoc>
        ///    Used by the designer
        /// </devdoc>
        /// <internalonly/>
        public virtual void ResetShowHelp(Control ctl) {
            showHelp.Remove(ctl);
        }

        /// <include file='doc\HelpProvider.uex' path='docs/doc[@for="HelpProvider.UpdateEventBinding"]/*' />
        /// <devdoc>
        ///     Binds/unbinds event handlers to ctl
        /// </devdoc>
        /// <internalonly/>
        private void UpdateEventBinding(Control ctl) {
            if (GetShowHelp(ctl) && !boundControls.ContainsKey(ctl)) {
                ctl.HelpRequested += new HelpEventHandler(this.OnControlHelp);
                ctl.QueryAccessibilityHelp += new QueryAccessibilityHelpEventHandler(this.OnQueryAccessibilityHelp);
                boundControls[ctl] = ctl;
            }
            else if (!GetShowHelp(ctl) && boundControls.ContainsKey(ctl)) {
                ctl.HelpRequested -= new HelpEventHandler(this.OnControlHelp);
                ctl.QueryAccessibilityHelp -= new QueryAccessibilityHelpEventHandler(this.OnQueryAccessibilityHelp);
                boundControls.Remove(ctl);
            }
        }

        /// <include file='doc\HelpProvider.uex' path='docs/doc[@for="HelpProvider.ToString"]/*' />
        /// <devdoc>
        ///    Returns a string representation for this control.
        /// </devdoc>
        /// <internalonly/>
        public override string ToString() {
            string s = base.ToString();
            return s + ", HelpNamespace: " + HelpNamespace;
        }
    }
}
