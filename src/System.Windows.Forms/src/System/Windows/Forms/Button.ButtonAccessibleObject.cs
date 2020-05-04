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
    public partial class Button
    {
        [ComVisible(true)]
        internal class ButtonAccessibleObject : ControlAccessibleObject
        {
            internal ButtonAccessibleObject(Button owner) : base(owner)
            {
            }

            private Button OwningButton => Owner as Button;

            internal override object GetPropertyValue(UiaCore.UIA propertyID)
            {
                switch (propertyID)
                {
                    case UiaCore.UIA.NamePropertyId:
                        return Name;
                    case UiaCore.UIA.AutomationIdPropertyId:
                        return OwningButton.IsHandleCreated ? OwningButton.Name : String.Empty;
                    case UiaCore.UIA.ControlTypePropertyId:
                        return UiaCore.UIA.ButtonControlTypeId;
                    case UiaCore.UIA.IsKeyboardFocusablePropertyId:
                        // This is necessary for compatibility with MSAA proxy:
                        // IsKeyboardFocusable = true regardless the control is enabled/disabled.
                        return true;
                }

                return base.GetPropertyValue(propertyID);
            }
        }
    }
}
