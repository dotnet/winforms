// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms.Automation {

    /// <summary>
    /// Provides support for UIA live regions.
    /// The feature is available in applications that are recompiled to target .NET Framework 4.7.3
    /// or opt in into this functionality using compatibility switches.
    /// </summary>
    public interface IAutomationLiveRegion {
        /// <summary>
        /// Gets or sets notification characteristics of the live region.
        /// </summary>
        AutomationLiveSetting LiveSetting { get; set; }
    }
}