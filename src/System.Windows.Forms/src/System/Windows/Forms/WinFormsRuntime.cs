// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Windows.Forms.Nrbf;
using System.Windows.Forms.Ole;

namespace System.Windows.Forms;

/// <summary>
///  Represents Windows Forms specific runtime services and types.
/// </summary>
internal class WinFormsRuntime : Runtime<DataFormats.Format, WinFormsNrbfSerializer, WinFormsOleServices>
{
    // This class is not intended to be instantiated.
    private WinFormsRuntime() { }
}
