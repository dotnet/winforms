using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static Interop;

namespace System.Windows.Forms
{
    public partial class RadioButton
    {
        [ComVisible(true)]
        public class RadioButtonAccessibleObject : ControlAccessibleObject
        {
            public RadioButtonAccessibleObject(RadioButton owner) : base(owner)
            {
            }

            private RadioButton OwningRadioButton => Owner as RadioButton;

            internal override bool IsPatternSupported(UiaCore.UIA patternId)
            {
                if (patternId == UiaCore.UIA.SelectionItemPatternId)
                {
                    return true;
                }

                return base.IsPatternSupported(patternId);
            }

            internal override object GetPropertyValue(UiaCore.UIA propertyID)
            {
                switch (propertyID)
                {
                    case UiaCore.UIA.NamePropertyId:
                        return Name;
                    case UiaCore.UIA.AutomationIdPropertyId:
                        return OwningRadioButton.IsHandleCreated ? OwningRadioButton.Name : String.Empty;
                    case UiaCore.UIA.IsSelectionItemPatternAvailablePropertyId:
                        return IsPatternSupported(UiaCore.UIA.SelectionItemPatternId);
                    case UiaCore.UIA.ControlTypePropertyId:
                        return UiaCore.UIA.RadioButtonControlTypeId;
                    case UiaCore.UIA.IsKeyboardFocusablePropertyId:
                        // This is necessary for compatibility with MSAA proxy:
                        // IsKeyboardFocusable = true regardless the control is enabled/disabled.
                        return true;
                }

                return base.GetPropertyValue(propertyID);
            }

            public override string DefaultAction
            {
                get
                {
                    string defaultAction = Owner.AccessibleDefaultActionDescription;
                    if (defaultAction != null)
                    {
                        return defaultAction;
                    }

                    return SR.AccessibleActionCheck;
                }
            }

            internal override bool IsItemSelected
            {
                get => OwningRadioButton.Checked;
            }

            public override void DoDefaultAction()
            {
                if (OwningRadioButton.IsHandleCreated)
                {
                    OwningRadioButton.PerformClick();
                }
            }
        }
    }
}
