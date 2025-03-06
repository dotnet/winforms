// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Reflection;

namespace System;

internal static class Assemblies
{
    /// <summary>
    ///  The full name of the mscorlib assembly on .NET Framework 4.x.
    /// </summary>
    public const string Mscorlib =
        "mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";

    /// <summary>
    ///  The full name of the System.Drawing assembly on .NET Framework 4.x.
    /// </summary>
    public const string SystemDrawing =
        "System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";

    /// <summary>
    ///  The full name of the System.Drawing.Design assembly on .NET Framework 4.x.
    /// </summary>
    public const string SystemDrawingDesign =
        "System.Drawing.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";

    /// <summary>
    ///  The full name of the System.Windows.Forms assembly on .NET Framework 4.x.
    /// </summary>
    public const string SystemWindowsForms =
        "System.Windows.Forms, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";

    /// <summary>
    ///  The full name of the System.Design assembly on .NET Framework 4.x.
    /// </summary>
    public const string SystemDesign =
        "System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a";

    private static Assembly? s_mscorlibFacadeAssembly;
    internal static Assembly MscorlibAssembly => s_mscorlibFacadeAssembly
        ??= Assembly.Load(Mscorlib);

    internal static Assembly CorelibAssembly { get; } = typeof(string).Assembly;
    internal static string CorelibAssemblyString { get; } = CorelibAssembly.FullName!;
}
