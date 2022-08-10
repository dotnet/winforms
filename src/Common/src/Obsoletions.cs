// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System;

internal static class Obsoletions
{
    internal const string SharedUrlFormat = "https://aka.ms/winforms-warnings/{0}";

    // Please see docs\project\list-of-diagnostics.md for instructions on the steps required
    // to introduce a new obsoletion, apply it to downlevel builds, claim a diagnostic id,
    // and ensure the "aka.ms/dotnet-warnings/{0}" URL points to documentation for the obsoletion
    // The diagnostic ids reserved for obsoletions are WFDEV### (WFDEV001 - WFDEV999).

    internal const string DomainUpDownAccessibleObjectMessage = $"{nameof(System.Windows.Forms.DomainUpDown.DomainUpDownAccessibleObject)} is no longer used to provide accessible support for {nameof(System.Windows.Forms.DomainUpDown)} controls.";
    internal const string DomainUpDownAccessibleObjectDiagnosticId = "WFDEV002";

    internal const string DomainItemAccessibleObjectMessage = $"{nameof(System.Windows.Forms.DomainUpDown.DomainItemAccessibleObject)} is no longer used to provide accessible support for {nameof(System.Windows.Forms.DomainUpDown)} items.";
    internal const string DomainItemAccessibleObjectDiagnosticId = "WFDEV003";
}
