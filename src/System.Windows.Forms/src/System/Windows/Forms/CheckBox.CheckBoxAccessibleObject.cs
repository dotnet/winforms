using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static System.Windows.Forms.Control;
using static Interop;

namespace System.Windows.Forms
{
    public partial class CheckBox
    {
        [ComVisible(true)]
        internal class CheckBoxAccessibleObject : ControlAccessibleObject
        {
            internal CheckBoxAccessibleObject(CheckBox owner) : base(owner)
            {
            }

            private CheckBox OwningCheckBox => Owner as CheckBox;

            internal override bool IsPatternSupported(UiaCore.UIA patternId)
            {
                if (patternId == UiaCore.UIA.TogglePatternId)
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
                        return OwningCheckBox.IsHandleCreated ? OwningCheckBox.Name : String.Empty;
                    case UiaCore.UIA.IsTogglePatternAvailablePropertyId:
                        return IsPatternSupported(UiaCore.UIA.TogglePatternId);
                    case UiaCore.UIA.ControlTypePropertyId:
                        return UiaCore.UIA.CheckBoxControlTypeId;
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

                    if (OwningCheckBox.Checked)
                    {
                        return SR.AccessibleActionUncheck;
                    }
                    else
                    {
                        return SR.AccessibleActionCheck;
                    }
                }
            }

            internal override UiaCore.ToggleState ToggleState
            {
                get
                {
                    return OwningCheckBox.Checked ? UiaCore.ToggleState.On : UiaCore.ToggleState.Off;
                }
            }
        }
    }
}
