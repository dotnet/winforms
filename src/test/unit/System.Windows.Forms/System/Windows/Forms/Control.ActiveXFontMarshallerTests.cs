// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing;
using System.Runtime.InteropServices;
using Windows.Win32.System.Ole;

namespace System.Windows.Forms.Tests;

public unsafe class Control_ActiveXFontMarshalerTests
{
    private static readonly ICustomMarshaler s_marshaler =
        (ICustomMarshaler)Activator.CreateInstance(
            typeof(Control).Assembly.GetType("System.Windows.Forms.Control+ActiveXFontMarshaler")!,
            nonPublic: true)!;

    [StaFact(Skip = "Flaky test being investigated. see: https://github.com/dotnet/winforms/issues/8608")]
    public void ActiveXFontMarshaler_RoundTripManagedFont()
    {
        using Font font = new("Arial", 11.0f, GraphicsUnit.Point);
        nint native = s_marshaler.MarshalManagedToNative(font);
        Assert.NotEqual(0, native);
        using ComScope<IFont> iFont = new((IFont*)native);
        using BSTR name = iFont.Value->Name;
        Assert.Equal(font.Name, name.ToString());
        Assert.Equal(11.25f, (float)iFont.Value->Size);

        using Font outFont = (Font)s_marshaler.MarshalNativeToManaged(native);
        Assert.Equal(font.Name, outFont.Name);
        Assert.Equal(11.25f, outFont.SizeInPoints);
    }
}
