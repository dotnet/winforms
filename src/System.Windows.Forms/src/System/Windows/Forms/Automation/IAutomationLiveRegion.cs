// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

namespace System.Windows.Forms.Automation
{
    public interface IAutomationLiveRegion
    {
        /// <summary>
        ///  Gets or sets notification characteristics of the live region.
        /// </summary>
        AutomationLiveSetting LiveSetting { get; set; }
    }
}
