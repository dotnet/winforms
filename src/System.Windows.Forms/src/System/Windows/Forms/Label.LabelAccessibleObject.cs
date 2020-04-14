using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static Interop;

namespace System.Windows.Forms
{
    public partial class Label
    {
        [ComVisible(true)]
        internal class LabelAccessibleObject : ControlAccessibleObject
        {
            public LabelAccessibleObject(Label owner) : base(owner)
            {
            }

            private Label OwningLabel => Owner as Label;

            internal override object GetPropertyValue(UiaCore.UIA propertyID)
            {
                switch (propertyID)
                {
                    case UiaCore.UIA.NamePropertyId:
                        return Name;
                    case UiaCore.UIA.AutomationIdPropertyId:
                        return OwningLabel.IsHandleCreated ? OwningLabel.Name : String.Empty;
                    case UiaCore.UIA.ControlTypePropertyId:
                        return UiaCore.UIA.TextControlTypeId;
                }

                return base.GetPropertyValue(propertyID);
            }
        }
    }
}
