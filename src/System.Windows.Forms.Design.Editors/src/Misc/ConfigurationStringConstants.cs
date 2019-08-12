// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Configuration
{
    /// <summary>
    ///  These configuration file values are added in .NET FX 4.7 and above frameworks only.
    ///  Sample usage:
    ///  <System.Windows.Forms.ApplicationConfigurationSection>
    ///  <add key="DpiAwareness" value="PerMonitorV2" />
    ///  <add key="ToolStrip.DisableHighDpiImprovements" value="false" />
    ///  <add key="Form.DisableSinglePassControlScaling" value="false" />
    ///  <add key="CheckedListBox.DisableHighDpiImprovements" value="false" />
    ///  <add key="DisableDpiChangedMessageHandling" value="false" />
    ///  <add key="MonthCalendar.DisableHighDpiImprovements" value="false" />
    ///  <add key="DisableDpiChangedHighDpiImprovements" value="false" />
    ///  </System.Windows.Forms.ApplicationConfigurationSection>
    /// </summary>
    internal static class ConfigurationStringConstants
    {
        internal const string WinformsApplicationConfigurationSectionName = "System.Windows.Forms.ApplicationConfigurationSection";
        internal const string DpiAwarenessKeyName = "DpiAwareness";
        internal const string EnableWindowsFormsHighDpiAutoResizingKeyName = "EnableWindowsFormsHighDpiAutoResizing";
        internal const string ToolStripDisableHighDpiImprovementsKeyName = "ToolStrip.DisableHighDpiImprovements";
        internal const string CheckedListBoxDisableHighDpiImprovementsKeyName = "CheckedListBox.DisableHighDpiImprovements";
        internal const string DataGridViewControlDisableHighDpiImprovements = "DataGridView.DisableHighDpiImprovements";
        internal const string FormDisableSinglePassScalingOfDpiFormsKeyName = "Form.DisableSinglePassScalingOfDpiForms";
        internal const string DisableDpiChangedMessageHandlingKeyName = "DisableDpiChangedMessageHandling";
        internal const string DisableDpiChangedHighDpiImprovementsKeyName = "DisableDpiChangedHighDpiImprovements";
        internal const string AnchorLayoutDisableHighDpiImprovementsKeyName = "AnchorLayout.DisableHighDpiImprovements";
        internal const string MonthCalendarDisableHighDpiImprovementsKeyName = "MonthCalendar.DisableHighDpiImprovements";
    }
}
