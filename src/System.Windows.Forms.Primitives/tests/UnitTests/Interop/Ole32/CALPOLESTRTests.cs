// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Runtime.InteropServices;
using Windows.Win32.System.Ole;

namespace System.Windows.Forms.Primitives.Ole32Tests;

public class CALPOLESTRTests
{
    [Fact]
    public void CALPOLESTR_ConvertAndFree_SingleItem()
    {
        CALPOLESTR ca = CreateStringVector("Swizzle");

        string?[] values = ca.ConvertAndFree();
        Assert.Single(values);
        Assert.Equal("Swizzle", values[0]);
    }

    [Fact]
    public void CALPOLESTR_ConvertAndFree_EmptyStruct()
    {
        CALPOLESTR ca = default;

        string?[] values = ca.ConvertAndFree();
        Assert.Empty(values);
    }

    private static CALPOLESTR CreateStringVector(params string[] values)
        => CreateStringVector(allocations: null, values);

    private static unsafe CALPOLESTR CreateStringVector(IList<IntPtr>? allocations, params string[] values)
    {
        CALPOLESTR ca = new()
        {
            cElems = (uint)values.Length,
            pElems = (PWSTR*)Marshal.AllocCoTaskMem(IntPtr.Size * values.Length)
        };

        allocations?.Add((IntPtr)ca.pElems);

        Span<IntPtr> elements = new(ca.pElems, values.Length);
        for (int i = 0; i < values.Length; i++)
        {
            IntPtr nativeCopy = Marshal.StringToCoTaskMemUni(values[i]);
            allocations?.Add(nativeCopy);
            elements[i] = nativeCopy;
        }

        return ca;
    }
}
