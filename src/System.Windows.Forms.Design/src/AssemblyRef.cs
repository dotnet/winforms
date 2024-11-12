// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

internal static class FXAssembly
{
    // NB: this must never-ever change to facilitate type-forwarding from
    // .NET Framework, if those are referenced in .NET project.
    internal const string Version = "4.0.0.0";
}

internal static class AssemblyRef
{
    internal const string MicrosoftPublicKey = "b03f5f7f11d50a3a";
    internal const string EnvDTE = $"EnvDTE, Version=7.0.3300.0, Culture=neutral, PublicKeyToken={MicrosoftPublicKey}";
    internal const string SystemDesign = $"System.Design, Version={FXAssembly.Version}, Culture=neutral, PublicKeyToken={MicrosoftPublicKey}";
}
