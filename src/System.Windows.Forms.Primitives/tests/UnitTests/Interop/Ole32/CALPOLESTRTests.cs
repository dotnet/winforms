// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using Xunit;
using static Interop;

namespace System.Windows.Forms.Primitives.Ole32Tests
{
    public class CALPOLESTRTests
    {
        [Fact]
        public void CALPOLESTR_ConvertAndFree_SingleItem()
        {
            Ole32.CALPOLESTR ca = CreateStringVector("Swizzle");

            string?[] values = ca.ConvertAndFree();
            Assert.Equal(1, values.Length);
            Assert.Equal("Swizzle", values[0]);
        }

        [Fact]
        public void CALPOLESTR_ConvertAndFree_EmptyStruct()
        {
            Ole32.CALPOLESTR ca = default;

            string?[] values = ca.ConvertAndFree();
            Assert.Empty(values);
        }

        private static Ole32.CALPOLESTR CreateStringVector(params string[] values)
            => CreateStringVector(allocations: null, values);

        private static unsafe Ole32.CALPOLESTR CreateStringVector(IList<IntPtr>? allocations, params string[] values)
        {
            Ole32.CALPOLESTR ca = new()
            {
                cElems = (uint)values.Length,
                pElems = (char**)Marshal.AllocCoTaskMem(IntPtr.Size * values.Length)
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
}
