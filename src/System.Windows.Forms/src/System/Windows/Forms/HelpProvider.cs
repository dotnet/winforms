// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Design;

namespace System.Windows.Forms
{
    /// <summary>
    ///  Provides pop-up or online Help for controls.
    /// </summary>
    [ProvideProperty("HelpString", typeof(Control))]
    [ProvideProperty("HelpKeyword", typeof(Control))]
    [ProvideProperty("HelpNavigator", typeof(Control))]
    [ProvideProperty("ShowHelp", typeof(Control))]
    [ToolboxItemFilter("System.Windows.Forms")]
    [SRDescription(nameof(SR.DescriptionHelpProvider))]
    public class HelpProvider : Component, IExtenderProvider
    {
        private readonly Hashtable _helpStrings = new Hashtable();
        private readonly Hashtable _showHelp = new Hashtable();
        private readonly Hashtable _boundControls = new Hashtable();
        private readonly Hashtable _keywords = new Hashtable();
        private readonly Hashtable _navigators = new Hashtable();

        /// <summary>
        ///  Initializes a new instance of the <see cref='HelpProvider'/> class.
        /// </summary>
        public HelpProvider()
        {
        }

        /// <summary>
        ///  Gets or sets a string indicating the name of the Help file associated with this
        /// <see cref='HelpProvider'/> object.
        /// </summary>
        [Localizable(true)]
        [DefaultValue(null)]
        [Editor("System.Windows.Forms.Design.HelpNamespaceEditor, " + AssemblyRef.SystemDesign, typeof(UITypeEditor))]
        [SRDescription(nameof(SR.HelpProviderHelpNamespaceDescr))]
        public virtual string HelpNamespace { get; set; }

        [SRCategory(nameof(SR.CatData))]
        [Localizable(false)]
        [Bindable(true)]
        [SRDescription(nameof(SR.ControlTagDescr))]
        [DefaultValue(null)]
        [TypeConverter(typeof(StringConverter))]
        public object Tag { get; set; }

        /// <summary>
        ///  Determines if the help provider can offer it's extender properties to the specified target
        ///  object.
        /// </summary>
        public virtual bool CanExtend(object target) => target is Control;

        /// <summary>
        ///  Retrieves the Help Keyword displayed when the user invokes Help for the specified control.
        /// </summary>
        [DefaultValue(null)]
        [Localizable(true)]
        [SRDescription(nameof(SR.HelpProviderHelpKeywordDescr))]
        public virtual string GetHelpKeyword(Control ctl)
        {
            if (ctl is null)
            {
                throw new ArgumentNullException(nameof(ctl));
            }

            return (string)_keywords[ctl];
        }

        /// <summary>
        ///  Retrieves the contents of the pop-up help window for the specified control.
        /// </summary>
        [DefaultValue(HelpNavigator.AssociateIndex)]
        [Localizable(true)]
        [SRDescription(nameof(SR.HelpProviderNavigatorDescr))]
        public virtual HelpNavigator GetHelpNavigator(Control ctl)
        {
            if (ctl is null)
            {
                throw new ArgumentNullException(nameof(ctl));
            }

            object nav = _navigators[ctl];
            return nav is null ? HelpNavigator.AssociateIndex : (HelpNavigator)nav;
        }

        /// <summary>
        ///  Retrieves the contents of the pop-up help window for the specified control.
        /// </summary>
        [DefaultValue(null)]
        [Localizable(true)]
        [SRDescription(nameof(SR.HelpProviderHelpStringDescr))]
        public virtual string GetHelpString(Control ctl)
        {
            if (ctl is null)
            {
                throw new ArgumentNullException(nameof(ctl));
            }

            return (string)_helpStrings[ctl];
        }

        /// <summary>
        ///  Retrieves a value indicating whether Help displays for the specified control.
        /// </summary>
        [Localizable(true)]
        [SRDescription(nameof(SR.HelpProviderShowHelpDescr))]
        public virtual bool GetShowHelp(Control ctl)
        {
            if (ctl is null)
            {
                throw new ArgumentNullException(nameof(ctl));
            }

            object b = _showHelp[ctl];
            return b is null ? false : (bool)b;
        }

        /// <summary>
        ///  Handles the help event for any bound controls.
        /// </summary>
        private void OnControlHelp(object sender, HelpEventArgs hevent)
        {
            Control ctl = (Control)sender;
            string helpString = GetHelpString(ctl);
            string keyword = GetHelpKeyword(ctl);
            HelpNavigator navigator = GetHelpNavigator(ctl);

            if (!GetShowHelp(ctl) || hevent is null)
            {
                return;
            }

            if (Control.MouseButtons != MouseButtons.None && !string.IsNullOrEmpty(helpString))
            {
                Debug.WriteLineIf(Help.WindowsFormsHelpTrace.TraceVerbose, "HelpProvider:: Mouse down w/ helpstring");
                Help.ShowPopup(ctl, helpString, hevent.MousePos);
                hevent.Handled = true;
                return;
            }

            // If we have a help file, and help keyword we try F1 help next
            if (HelpNamespace != null)
            {
                Debug.WriteLineIf(Help.WindowsFormsHelpTrace.TraceVerbose, "HelpProvider:: F1 help");
                if (!string.IsNullOrEmpty(keyword))
                {
                    Help.ShowHelp(ctl, HelpNamespace, navigator, keyword);
                }
                else
                {
                    Help.ShowHelp(ctl, HelpNamespace, navigator);
                }

                hevent.Handled = true;
                return;
            }

            // So at this point we don't have a help keyword, so try to display the whats this help
            if (!string.IsNullOrEmpty(helpString))
            {
                Debug.WriteLineIf(Help.WindowsFormsHelpTrace.TraceVerbose, "HelpProvider:: back to helpstring");
                Help.ShowPopup(ctl, helpString, hevent.MousePos);
                hevent.Handled = true;
            }
        }

        /// <summary>
        ///  Handles the help event for any bound controls.
        /// </summary>
        private void OnQueryAccessibilityHelp(object sender, QueryAccessibilityHelpEventArgs e)
        {
            Control ctl = (Control)sender;
            e.HelpString = GetHelpString(ctl);
            e.HelpKeyword = GetHelpKeyword(ctl);
            e.HelpNamespace = HelpNamespace;
        }

        /// <summary>
        ///  Specifies a Help string associated with a control.
        /// </summary>
        public virtual void SetHelpString(Control ctl, string helpString)
        {
            if (ctl is null)
            {
                throw new ArgumentNullException(nameof(ctl));
            }

            _helpStrings[ctl] = helpString;
            if (!string.IsNullOrEmpty(helpString))
            {
                SetShowHelp(ctl, true);
            }

            UpdateEventBinding(ctl);
        }

        /// <summary>
        ///  Specifies the Help keyword to display when the user invokes Help for a control.
        /// </summary>
        public virtual void SetHelpKeyword(Control ctl, string keyword)
        {
            if (ctl is null)
            {
                throw new ArgumentNullException(nameof(ctl));
            }

            _keywords[ctl] = keyword;
            if (!string.IsNullOrEmpty(keyword))
            {
                SetShowHelp(ctl, true);
            }

            UpdateEventBinding(ctl);
        }

        /// <summary>
        ///  Specifies the Help keyword to display when the user invokes Help for a control.
        /// </summary>
        public virtual void SetHelpNavigator(Control ctl, HelpNavigator navigator)
        {
            if (ctl is null)
            {
                throw new ArgumentNullException(nameof(ctl));
            }
            if (!ClientUtils.IsEnumValid(navigator, (int)navigator, (int)HelpNavigator.Topic, (int)HelpNavigator.TopicId))
            {
                throw new InvalidEnumArgumentException(nameof(navigator), (int)navigator, typeof(HelpNavigator));
            }

            _navigators[ctl] = navigator;
            SetShowHelp(ctl, true);
            UpdateEventBinding(ctl);
        }

        /// <summary>
        ///  Specifies whether Help is displayed for a given control.
        /// </summary>
        public virtual void SetShowHelp(Control ctl, bool value)
        {
            if (ctl is null)
            {
                throw new ArgumentNullException(nameof(ctl));
            }

            _showHelp[ctl] = value;
            UpdateEventBinding(ctl);
        }

        /// <summary>
        ///  Used by the designer
        /// </summary>
        internal bool ShouldSerializeShowHelp(Control ctl)
        {
            if (ctl is null)
            {
                throw new ArgumentNullException(nameof(ctl));
            }

            return _showHelp.ContainsKey(ctl);
        }

        /// <summary>
        ///  Used by the designer
        /// </summary>
        public virtual void ResetShowHelp(Control ctl)
        {
            if (ctl is null)
            {
                throw new ArgumentNullException(nameof(ctl));
            }

            _showHelp.Remove(ctl);
        }

        /// <summary>
        ///  Binds/unbinds event handlers to ctl
        /// </summary>
        private void UpdateEventBinding(Control ctl)
        {
            if (GetShowHelp(ctl) && !_boundControls.ContainsKey(ctl))
            {
                ctl.HelpRequested += new HelpEventHandler(OnControlHelp);
                ctl.QueryAccessibilityHelp += new QueryAccessibilityHelpEventHandler(OnQueryAccessibilityHelp);
                _boundControls[ctl] = ctl;
            }
            else if (!GetShowHelp(ctl) && _boundControls.ContainsKey(ctl))
            {
                ctl.HelpRequested -= new HelpEventHandler(OnControlHelp);
                ctl.QueryAccessibilityHelp -= new QueryAccessibilityHelpEventHandler(OnQueryAccessibilityHelp);
                _boundControls.Remove(ctl);
            }
        }

        /// <summary>
        ///  Returns a string representation for this control.
        /// </summary>
        public override string ToString() => base.ToString() + ", HelpNamespace: " + HelpNamespace;
    }
}
