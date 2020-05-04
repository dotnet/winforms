using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static Interop;

namespace System.Windows.Forms
{ 
    public partial class Splitter
    {
        [ComVisible(true)]
        internal class SplitterAccessibleObject : ControlAccessibleObject
        {
            internal SplitterAccessibleObject(Splitter owner) : base(owner)
            {
            }

            private Splitter OwningSplitter => Owner as Splitter;

            internal override object GetPropertyValue(UiaCore.UIA propertyID)
            {
                switch (propertyID)
                {
                    case UiaCore.UIA.NamePropertyId:
                        return Name;
                    case UiaCore.UIA.AutomationIdPropertyId:
                        return OwningSplitter.IsHandleCreated ? OwningSplitter.Name : String.Empty;
                    case UiaCore.UIA.ControlTypePropertyId:
                        return UiaCore.UIA.PaneControlTypeId;
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
