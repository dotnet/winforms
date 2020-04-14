using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static Interop;

namespace System.Windows.Forms
{
    public partial class GroupBox
    {
        [ComVisible(true)]
        internal class GroupBoxAccessibleObject : ControlAccessibleObject
        {
            internal GroupBoxAccessibleObject(GroupBox owner) : base(owner)
            {
            }

            private GroupBox OwningGroupBox => Owner as GroupBox;

            internal override object GetPropertyValue(UiaCore.UIA propertyID)
            {
                switch (propertyID)
                {
                    case UiaCore.UIA.NamePropertyId:
                        return Name;
                    case UiaCore.UIA.ControlTypePropertyId:
                        return UiaCore.UIA.GroupControlTypeId;
                    case UiaCore.UIA.AutomationIdPropertyId:
                        return OwningGroupBox.IsHandleCreated ? OwningGroupBox.Name : String.Empty;
                    case UiaCore.UIA.IsKeyboardFocusablePropertyId:
                        return true;
                }

                return base.GetPropertyValue(propertyID);
            }
        }
    }
}
