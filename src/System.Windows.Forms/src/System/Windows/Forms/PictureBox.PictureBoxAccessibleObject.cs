using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static Interop;

namespace System.Windows.Forms
{
    public partial class PictureBox
    {
        [ComVisible(true)]
        internal class PictureBoxAccessibleObject : ControlAccessibleObject
        {
            internal PictureBoxAccessibleObject(PictureBox owner) : base(owner)
            {
            }

            private PictureBox OwningPictureBox => Owner as PictureBox;

            internal override object GetPropertyValue(UiaCore.UIA propertyID)
            {
                switch (propertyID)
                {
                    case UiaCore.UIA.NamePropertyId:
                        return Name;
                    case UiaCore.UIA.ControlTypePropertyId:
                        return UiaCore.UIA.PaneControlTypeId;
                    case UiaCore.UIA.AutomationIdPropertyId:
                        return OwningPictureBox.IsHandleCreated ? OwningPictureBox.Name : String.Empty;
                    case UiaCore.UIA.IsKeyboardFocusablePropertyId:
                        return true;
                }

                return base.GetPropertyValue(propertyID);
            }
        }
    }
}
