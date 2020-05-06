// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Runtime.InteropServices;
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
                => propertyID switch
                {
                    UiaCore.UIA.NamePropertyId
                        => Name,
                    UiaCore.UIA.ControlTypePropertyId
                        => UiaCore.UIA.PaneControlTypeId,
                    UiaCore.UIA.AutomationIdPropertyId
                        => OwningPictureBox.IsHandleCreated ? OwningPictureBox.Name : string.Empty,
                    UiaCore.UIA.IsKeyboardFocusablePropertyId
                        => true,
                    _ => base.GetPropertyValue(propertyID)
                };
        }
    }
}
