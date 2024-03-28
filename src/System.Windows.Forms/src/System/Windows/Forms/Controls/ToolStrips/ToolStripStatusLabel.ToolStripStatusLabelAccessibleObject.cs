// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.System.Variant;
using Windows.Win32.UI.Accessibility;

namespace System.Windows.Forms;

public partial class ToolStripStatusLabel
{
    internal class ToolStripStatusLabelAccessibleObject : ToolStripLabelAccessibleObject
    {
        private readonly ToolStripStatusLabel _owningToolStripStatusLabel;

        public ToolStripStatusLabelAccessibleObject(ToolStripStatusLabel ownerItem) : base(ownerItem)
        {
            _owningToolStripStatusLabel = ownerItem;
        }

        /// <summary>
        ///  Raises the LiveRegionChanged UIA event.
        /// </summary>
        /// <returns>True if operation succeeds, False otherwise.</returns>
        public override bool RaiseLiveRegionChanged()
        {
            return RaiseAutomationEvent(UIA_EVENT_ID.UIA_LiveRegionChangedEventId);
        }

        internal override VARIANT GetPropertyValue(UIA_PROPERTY_ID propertyID) =>
            propertyID switch
            {
                UIA_PROPERTY_ID.UIA_LiveSettingPropertyId => (VARIANT)(int)_owningToolStripStatusLabel.LiveSetting,
                _ => base.GetPropertyValue(propertyID)
            };
    }
}
