// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Always hard bind to System.dll and System.Drawing.dll

using System.Runtime.CompilerServices;

[assembly:DependencyAttribute("System,", LoadHint.Always)]
[assembly:DependencyAttribute("System.Drawing,", LoadHint.Always)]
[assembly:DependencyAttribute("System.Core", LoadHint.Sometimes)]
// This is now trun on by default, use source file NO_RUNTIMECOMPATIBILITY_ATTRIBUTE flag to control this
// [assembly:RuntimeCompatibility(WrapNonExceptionThrows = true)]
[assembly:System.Runtime.CompilerServices.StringFreezingAttribute()]
[assembly:System.Runtime.InteropServices.TypeLibVersion(2, 4)]
 
// Opts into the VS loading icons from the Icon Satellite assembly: System.Windows.Forms.VisualStudio.<version>.0.dll
[assembly:System.Drawing.BitmapSuffixInSatelliteAssemblyAttribute()]

