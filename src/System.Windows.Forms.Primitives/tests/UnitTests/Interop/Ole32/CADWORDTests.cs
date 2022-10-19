// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using Xunit;
using static Interop;

namespace System.Windows.Forms.Primitives.Ole32Tests
{
    public class CADWORDTests
    {
        [Fact]
        public void CADWORD_ConvertAndFree_SingleItem()
        {
            Ole32.CADWORD ca = CreateIntVector(2020);

            uint[] values = ca.ConvertAndFree();
            Assert.Equal(1, values.Length);
            Assert.Equal(2020u, values[0]);
        }

        [Fact]
        public void CADWORD_ConvertAndFree_EmptyStruct()
        {
            Ole32.CADWORD ca = default;

            uint[] values = ca.ConvertAndFree();
            Assert.Empty(values);
        }

        private static Ole32.CADWORD CreateIntVector(params uint[] values)
            => CreateIntVector(allocations: null, values);

        private static unsafe Ole32.CADWORD CreateIntVector(IList<IntPtr>? allocations, params uint[] values)
        {
            Ole32.CADWORD ca = new()
            {
                cElems = (uint)values.Length,
                pElems = (uint*)Marshal.AllocCoTaskMem(sizeof(uint) * values.Length)
            };

            allocations?.Add((IntPtr)ca.pElems);

            Span<uint> elements = new(ca.pElems, values.Length);
            for (int i = 0; i < values.Length; i++)
            {
                elements[i] = values[i];
            }

            return ca;
        }
    }
}
