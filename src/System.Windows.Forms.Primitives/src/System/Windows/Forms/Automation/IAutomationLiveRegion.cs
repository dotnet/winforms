// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.Automation;

public interface IAutomationLiveRegion
{
    /// <summary>
    ///  Gets or sets notification characteristics of the live region.
    /// </summary>
    AutomationLiveSetting LiveSetting { get; set; }
}
