// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Runtime.InteropServices;
using Xunit;
using static Interop;

namespace System.Windows.Forms.ComponentModel.Com2Interop.Tests
{
    public class Int32CAMarshalerTests
    {
        [Fact]
        public void Int32CAMarshaler_FreesMemory()
        {
            List<IntPtr> allocations = new();
            Ole32.CA ca = CreateIntVector(allocations, 1970, 1999);

            MallocSpy.FreeTracker tracker = new();
            using MallocSpyScope scope = new(tracker);

            Int32CAMarshaler marshaller = new(ca);
            Assert.Equal(2, marshaller.Items.Length);
            Assert.Equal(1970, (int)marshaller.Items[0]);
            Assert.Equal(1999, (int)marshaller.Items[1]);

            foreach (IntPtr allocation in allocations)
            {
                Assert.Contains(allocation, tracker.FreedBlocks);
            }
        }

        [Fact]
        public void Int32CAMarshaler_SingleItem()
        {
            Ole32.CA ca = CreateIntVector(2020);

            MallocSpy.FreeTracker tracker = new();
            using MallocSpyScope scope = new(tracker);

            Int32CAMarshaler marshaller = new(ca);
            Assert.Equal(1, marshaller.Items.Length);
            Assert.Equal(2020, (int)marshaller.Items[0]);
        }

        private static Ole32.CA CreateIntVector(params int[] values)
            => CreateIntVector(null, values);

        private unsafe static Ole32.CA CreateIntVector(IList<IntPtr> allocations, params int[] values)
        {
            Ole32.CA ca = new()
            {
                cElems = (uint)values.Length,
                pElems = (void*)Marshal.AllocCoTaskMem(sizeof(int) * values.Length)
            };

            allocations?.Add((IntPtr)ca.pElems);

            Span<int> elements = new(ca.pElems, values.Length);
            for (int i = 0; i < values.Length; i++)
            {
                elements[i] = values[i];
            }

            return ca;
        }
    }
}
