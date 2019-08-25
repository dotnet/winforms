// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static class FXAssembly
{
    internal const string Version = "4.0.0.0";
}

internal static class AssemblyRef
{
    internal const string MicrosoftPublicKey = "b03f5f7f11d50a3a";
    internal const string SystemDesign = "System.Design, Version=" + FXAssembly.Version + ", Culture=neutral, PublicKeyToken=" + MicrosoftPublicKey;
    internal const string SystemDrawingDesign = "System.Drawing.Design, Version=" + FXAssembly.Version + ", Culture=neutral, PublicKeyToken=" + MicrosoftPublicKey;
    internal const string SystemDrawing = "System.Drawing, Version=" + FXAssembly.Version + ", Culture=neutral, PublicKeyToken=" + MicrosoftPublicKey;
    internal const string SystemWinformsDesign = "System.Windows.Forms.Design, Version=" + FXAssembly.Version + ", Culture=neutral, PublicKeyToken=" + MicrosoftPublicKey;
}
