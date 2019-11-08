// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Globalization;

namespace System.Windows.Forms.Design
{
    /// <summary>
    /// Designer class for the MaskedTextBox control.
    /// </summary>
    internal class MaskedTextBoxDesigner : TextBoxBaseDesigner
    {
        private DesignerVerbCollection _verbs;
        private DesignerActionListCollection _actions;

        /// <summary>
        /// MaskedTextBox designer action list property. Gets the design-time supported actions on the control.
        /// </summary>
        public override DesignerActionListCollection ActionLists
        {
            get
            {
                if (_actions == null)
                {
                    _actions = new DesignerActionListCollection();
                    _actions.Add(new MaskedTextBoxDesignerActionList(this));
                }

                return _actions;
            }
        }

        /// <summary>
        /// A utility method to get a design time masked text box based on the masked text box being designed.
        /// </summary>
        /// <param name="maskedTextBox">The masked text box.</param>
        /// <returns></returns>
        internal static MaskedTextBox GetDesignMaskedTextBox(MaskedTextBox maskedTextBox)
        {
            MaskedTextBox designMaskedTextBox;
            if (maskedTextBox == null)
            {
                // return a default control.
                designMaskedTextBox = new MaskedTextBox();
            }
            else
            {
                MaskedTextProvider maskedTextProvider = maskedTextBox.MaskedTextProvider;

                if (maskedTextProvider == null)
                {
                    designMaskedTextBox = new MaskedTextBox();
                    designMaskedTextBox.Text = maskedTextBox.Text;
                }
                else
                {
                    designMaskedTextBox = new MaskedTextBox(maskedTextBox.MaskedTextProvider);
                }

                // Clone MTB properties.
                designMaskedTextBox.ValidatingType = maskedTextBox.ValidatingType;
                designMaskedTextBox.BeepOnError = maskedTextBox.BeepOnError;
                designMaskedTextBox.InsertKeyMode = maskedTextBox.InsertKeyMode;
                designMaskedTextBox.RejectInputOnFirstFailure = maskedTextBox.RejectInputOnFirstFailure;
                designMaskedTextBox.CutCopyMaskFormat = maskedTextBox.CutCopyMaskFormat;
                designMaskedTextBox.Culture = maskedTextBox.Culture;
                // designMaskedTextBox.TextMaskFormat = maskedTextBox.TextMaskFormat; - Not relevant since it is to be used programatically only.
            }

            // Some constant properties at design time.
            designMaskedTextBox.UseSystemPasswordChar = false;
            designMaskedTextBox.PasswordChar = '\0';
            designMaskedTextBox.ReadOnly = false;
            designMaskedTextBox.HidePromptOnLeave = false;

            return designMaskedTextBox;
        }

        internal static string GetMaskInputRejectedErrorMessage(MaskInputRejectedEventArgs e)
        {
            string rejectionHint;

            switch (e.RejectionHint)
            {
                case MaskedTextResultHint.AsciiCharacterExpected:
                    rejectionHint = SR.MaskedTextBoxHintAsciiCharacterExpected;
                    break;
                case MaskedTextResultHint.AlphanumericCharacterExpected:
                    rejectionHint = SR.MaskedTextBoxHintAlphanumericCharacterExpected;
                    break;
                case MaskedTextResultHint.DigitExpected:
                    rejectionHint = SR.MaskedTextBoxHintDigitExpected;
                    break;
                case MaskedTextResultHint.LetterExpected:
                    rejectionHint = SR.MaskedTextBoxHintLetterExpected;
                    break;
                case MaskedTextResultHint.SignedDigitExpected:
                    rejectionHint = SR.MaskedTextBoxHintSignedDigitExpected;
                    break;
                case MaskedTextResultHint.PromptCharNotAllowed:
                    rejectionHint = SR.MaskedTextBoxHintPromptCharNotAllowed;
                    break;
                case MaskedTextResultHint.UnavailableEditPosition:
                    rejectionHint = SR.MaskedTextBoxHintUnavailableEditPosition;
                    break;
                case MaskedTextResultHint.NonEditPosition:
                    rejectionHint = SR.MaskedTextBoxHintNonEditPosition;
                    break;
                case MaskedTextResultHint.PositionOutOfRange:
                    rejectionHint = SR.MaskedTextBoxHintPositionOutOfRange;
                    break;
                case MaskedTextResultHint.InvalidInput:
                    rejectionHint = SR.MaskedTextBoxHintInvalidInput;
                    break;
                case MaskedTextResultHint.Unknown:
                default:
                    Debug.Fail("Unknown RejectionHint, defaulting to InvalidInput...");
                    goto case MaskedTextResultHint.InvalidInput;

            }

            return string.Format(CultureInfo.CurrentCulture, SR.MaskedTextBoxTextEditorErrorFormatString, e.Position, rejectionHint);
        }

        /// <summary>
        /// Obsolete ComponentDesigner method which sets component default properties.  Overriden to avoid setting
        /// the Mask improperly.
        /// </summary>
        [Obsolete("This method has been deprecated. Use InitializeNewComponent instead.  http://go.microsoft.com/fwlink/?linkid=14202")]
        public override void OnSetComponentDefaults()
        {
            // do nothing.
        }

        /// <summary>
        ///  Event handler for the set mask verb.
        /// </summary>
        private void OnVerbSetMask(object sender, EventArgs e)
        {
            MaskedTextBoxDesignerActionList actionList = new MaskedTextBoxDesignerActionList(this);
            actionList.SetMask();
        }

        /// <summary>
        ///   Allows a designer to filter the set of properties
        ///   the component it is designing will expose through the
        ///   TypeDescriptor object.  This method is called
        ///   immediately before its corresponding "Post" method.
        ///   If you are overriding this method you should call
        ///   the base implementation before you perform your own
        ///   filtering.
        /// </summary>
        protected override void PreFilterProperties(IDictionary properties)
        {
            base.PreFilterProperties(properties);

            PropertyDescriptor prop;

            string[] shadowProps = new string[]
            {
                "Text",
                "PasswordChar"
            };

            Attribute[] empty = new Attribute[0];

            for (int i = 0; i < shadowProps.Length; i++)
            {
                prop = (PropertyDescriptor)properties[shadowProps[i]];
                if (prop != null)
                {
                    properties[shadowProps[i]] = TypeDescriptor.CreateProperty(typeof(MaskedTextBoxDesigner), prop, empty);
                }
            }
        }

        /// <summary>
        ///  Retrieves a set of rules concerning the movement capabilities of a component.
        ///  This should be one or more flags from the SelectionRules class.  If no designer
        ///  provides rules for a component, the component will not get any UI services.
        /// </summary>
        public override SelectionRules SelectionRules
        {
            get
            {
                SelectionRules rules = base.SelectionRules;
                rules &= ~(SelectionRules.TopSizeable | SelectionRules.BottomSizeable); // Height is fixed.
                return rules;
            }
        }

        /// <summary>
        ///  Shadows the PasswordChar.  UseSystemPasswordChar overrides PasswordChar so independent on the value
        ///  of PasswordChar it will return the systemp password char.  However, the value of PasswordChar is 
        ///  cached so if UseSystemPasswordChar is reset at design time the PasswordChar value can be restored.
        ///  So in the case both properties are set, we need to serialize the real PasswordChar value as well.
        /// </summary>
        private char PasswordChar
        {
            get
            {
                MaskedTextBox maskedTextBox = Control as MaskedTextBox;
                Debug.Assert(maskedTextBox != null, "Designed control is not a MaskedTextBox.");

                if (maskedTextBox.UseSystemPasswordChar)
                {
                    maskedTextBox.UseSystemPasswordChar = false;
                    char pwdChar = maskedTextBox.PasswordChar;
                    maskedTextBox.UseSystemPasswordChar = true;

                    return pwdChar;
                }
                else
                {
                    return maskedTextBox.PasswordChar;
                }
            }
            set
            {
                MaskedTextBox maskedTextBox = Control as MaskedTextBox;
                Debug.Assert(maskedTextBox != null, "Designed control is not a MaskedTextBox.");

                maskedTextBox.PasswordChar = value;
            }
        }

        /// <summary>
        ///  Shadow the Text property to do two things:
        ///     1. Always show the text without prompt or literals.
        ///     2. The text from the UITypeEditor is assigned escaping literals, prompt and spaces, this is to allow for partial inputs.
        ///     Observe that if the MTB is hooked to a PropertyBrowser at design time, shadowing of the property won't work unless the
        ///     application is a well written control designer (implements corresponding interfaces).
        /// </summary>
        private string Text
        {
            get
            {
                // Return text w/o literals or prompt.
                MaskedTextBox maskedTextBox = Control as MaskedTextBox;
                Debug.Assert(maskedTextBox != null, "Designed control is not a MaskedTextBox.");

                // Text w/o prompt or literals.
                if (string.IsNullOrEmpty(maskedTextBox.Mask))
                {
                    return maskedTextBox.Text;
                }
                return maskedTextBox.MaskedTextProvider.ToString(false, false);
            }
            set
            {
                MaskedTextBox maskedTextBox = Control as MaskedTextBox;
                Debug.Assert(maskedTextBox != null, "Designed control is not a MaskedTextBox.");

                if (string.IsNullOrEmpty(maskedTextBox.Mask))
                {
                    maskedTextBox.Text = value;
                }
                else
                {
                    bool ResetOnSpace = maskedTextBox.ResetOnSpace;
                    bool ResetOnPrompt = maskedTextBox.ResetOnPrompt;
                    bool SkipLiterals = maskedTextBox.SkipLiterals;

                    maskedTextBox.ResetOnSpace = true;
                    maskedTextBox.ResetOnPrompt = true;
                    maskedTextBox.SkipLiterals = true;

                    // Value is expected to contain literals and prompt.
                    maskedTextBox.Text = value;

                    maskedTextBox.ResetOnSpace = ResetOnSpace;
                    maskedTextBox.ResetOnPrompt = ResetOnPrompt;
                    maskedTextBox.SkipLiterals = SkipLiterals;
                }
            }
        }

        /// <summary>
        ///  MaskedTextBox designer verb collection property.  Gets the design-time supported verbs of the control.
        /// </summary>
        public override DesignerVerbCollection Verbs
        {
            get
            {
                if (_verbs == null)
                {
                    _verbs = new DesignerVerbCollection();
                    _verbs.Add(new DesignerVerb(SR.MaskedTextBoxDesignerVerbsSetMaskDesc, new EventHandler(OnVerbSetMask)));
                }

                return _verbs;
            }
        }
    }
}
