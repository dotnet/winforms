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
    public partial class TextBox
    {
        [ComVisible(true)]
        internal class TextBoxAccessibleObject : ControlAccessibleObject
        {
            internal TextBoxAccessibleObject(TextBox owner) : base(owner)
            {
            }

            public override string Name
            {
                get
                {
                    string name = base.Name;
                    if (name == null)
                    {
                        name = String.Empty;
                    }

                    // Otherwise just return the default label string, minus any mnemonics
                    return name;
                }
                set => base.Name = value;
            }

            private TextBox OwningTextBox => Owner as TextBox;
            internal override bool IsPatternSupported(UiaCore.UIA patternId)
            {
                if (patternId == UiaCore.UIA.ValuePatternId)
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
                        return OwningTextBox.IsHandleCreated ? OwningTextBox.Name : String.Empty;
                    case UiaCore.UIA.ControlTypePropertyId:
                        return UiaCore.UIA.EditControlTypeId;
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
