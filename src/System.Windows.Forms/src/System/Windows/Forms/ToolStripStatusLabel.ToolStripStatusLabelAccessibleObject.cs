﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using static Interop;

namespace System.Windows.Forms
{
    public partial class ToolStripStatusLabel
    {
        internal class ToolStripStatusLabelAccessibleObject : ToolStripLabelAccessibleObject
        {
            private readonly ToolStripStatusLabel ownerItem;

            public ToolStripStatusLabelAccessibleObject(ToolStripStatusLabel ownerItem) : base(ownerItem)
            {
                this.ownerItem = ownerItem;
            }

            /// <summary>
            ///  Raises the LiveRegionChanged UIA event.
            /// </summary>
            /// <returns>True if operation succeeds, False otherwise.</returns>
            public override bool RaiseLiveRegionChanged()
            {
                return RaiseAutomationEvent(UiaCore.UIA.LiveRegionChangedEventId);
            }

            internal override object GetPropertyValue(UiaCore.UIA propertyID)
            {
                switch (propertyID)
                {
                    case UiaCore.UIA.LiveSettingPropertyId:
                        return ownerItem.LiveSetting;
                    case UiaCore.UIA.ControlTypePropertyId:
                        return UiaCore.UIA.TextControlTypeId;
                }

                return base.GetPropertyValue(propertyID);
            }
        }
    }
}
