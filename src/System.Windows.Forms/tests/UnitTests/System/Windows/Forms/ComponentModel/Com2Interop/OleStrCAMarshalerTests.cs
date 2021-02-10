// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Runtime.InteropServices;
using Xunit;
using static Interop;

namespace System.Windows.Forms.ComponentModel.Com2Interop.Tests
{
    public class OleStrCAMarshalerTests
    {
        [Fact]
        public void OleStrCAMarshaler_FreesMemory()
        {
            List<IntPtr> allocations = new();
            Ole32.CA ca = CreateStringVector(allocations, "Sweet", "Potato");

            MallocSpy.FreeTracker tracker = new();
            using MallocSpyScope scope = new(tracker);

            OleStrCAMarshaler marshaller = new(ca);
            Assert.Equal(2, marshaller.Items.Length);
            Assert.Equal("Sweet", (string)marshaller.Items[0]);
            Assert.Equal("Potato", (string)marshaller.Items[1]);

            foreach (IntPtr allocation in allocations)
            {
                Assert.Contains(allocation, tracker.FreedBlocks);
            }
        }

        [Fact]
        public void OleStrCAMarshaler_SingleItem()
        {
            Ole32.CA ca = CreateStringVector("Swizzle");

            MallocSpy.FreeTracker tracker = new();
            using MallocSpyScope scope = new(tracker);

            OleStrCAMarshaler marshaller = new(ca);
            Assert.Equal(1, marshaller.Items.Length);
            Assert.Equal("Swizzle", (string)marshaller.Items[0]);
        }

        private static Ole32.CA CreateStringVector(params string[] values)
            => CreateStringVector(null, values);

        private unsafe static Ole32.CA CreateStringVector(IList<IntPtr> allocations, params string[] values)
        {
            Ole32.CA ca = new()
            {
                cElems = (uint)values.Length,
                pElems = (void*)Marshal.AllocCoTaskMem(IntPtr.Size * values.Length)
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
