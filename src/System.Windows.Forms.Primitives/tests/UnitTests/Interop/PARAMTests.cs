// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static Interop;

namespace System.Windows.Forms.Primitives.Tests.Interop
{
    public class PARAMTests
    {
        [ConditionalFact(typeof(ArchitectureDetection), nameof(ArchitectureDetection.Is32bit))]
        public unsafe void FromLowHigh_x32_Result()
        {
            Assert.Equal(unchecked((int)0x03040102), (int)PARAM.FromLowHigh(0x0102, 0x0304));
            Assert.Equal(unchecked((int)0xF3F4F1F2), (int)PARAM.FromLowHigh(0xF1F2, 0xF3F4));
        }

        [ConditionalFact(typeof(ArchitectureDetection), nameof(ArchitectureDetection.Is64bit))]
        public unsafe void FromLowHigh_x64_Result()
        {
            Assert.Equal(unchecked((long)0x0000000003040102), (long)PARAM.FromLowHigh(0x0102, 0x0304));
            Assert.Equal(unchecked((long)0xFFFFFFFFF3F4F1F2), (long)PARAM.FromLowHigh(0xF1F2, 0xF3F4));
        }

        [ConditionalFact(typeof(ArchitectureDetection), nameof(ArchitectureDetection.Is32bit))]
        public unsafe void FromLowHighUnsigned_x32_Result()
        {
            Assert.Equal(unchecked((int)0x03040102), (int)PARAM.FromLowHighUnsigned(0x0102, 0x0304));
            Assert.Equal(unchecked((int)0xF3F4F1F2), (int)PARAM.FromLowHighUnsigned(0xF1F2, 0xF3F4));
        }

        [ConditionalFact(typeof(ArchitectureDetection), nameof(ArchitectureDetection.Is64bit))]
        public unsafe void FromLowHighUnsigned_x64_Result()
        {
            Assert.Equal(unchecked((long)0x0000000003040102), (long)PARAM.FromLowHighUnsigned(0x0102, 0x0304));
            Assert.Equal(unchecked((long)0x00000000F3F4F1F2), (long)PARAM.FromLowHighUnsigned(0xF1F2, 0xF3F4));
        }

        [ConditionalFact(typeof(ArchitectureDetection), nameof(ArchitectureDetection.Is32bit))]
        public unsafe void LOWORD_x32_Result()
        {
            Assert.Equal(0x0304, PARAM.LOWORD((nint)(int)0x01020304));
            Assert.Equal(0xF3F4, PARAM.LOWORD((nint)unchecked((int)0xF1F2F3F4)));
        }

        [ConditionalFact(typeof(ArchitectureDetection), nameof(ArchitectureDetection.Is64bit))]
        public unsafe void LOWORD_x64_Result()
        {
            Assert.Equal(0x0304, PARAM.LOWORD(unchecked((nint)(long)0x0506070801020304)));
            Assert.Equal(0xF3F4, PARAM.LOWORD(unchecked((nint)unchecked((long)0xF5F6F7F8F1F2F3F4))));
        }

        [ConditionalFact(typeof(ArchitectureDetection), nameof(ArchitectureDetection.Is32bit))]
        public unsafe void HIWORD_x32_Result()
        {
            Assert.Equal(0x0102, PARAM.HIWORD((nint)(int)0x01020304));
            Assert.Equal(0xF1F2, PARAM.HIWORD((nint)unchecked((int)0xF1F2F3F4)));
        }

        [ConditionalFact(typeof(ArchitectureDetection), nameof(ArchitectureDetection.Is64bit))]
        public unsafe void HIWORD_x64_Result()
        {
            Assert.Equal(0x0102, PARAM.HIWORD(unchecked((nint)unchecked((long)0x0506070801020304))));
            Assert.Equal(0xF1F2, PARAM.HIWORD(unchecked((nint)unchecked((long)0xF5F6F7F8F1F2F3F4))));
        }

        [ConditionalFact(typeof(ArchitectureDetection), nameof(ArchitectureDetection.Is32bit))]
        public unsafe void SignedLOWORD_x32_Result()
        {
            Assert.Equal(unchecked((short)0x0304), PARAM.SignedLOWORD((nint)(int)0x01020304));
            Assert.Equal(unchecked((short)0xF3F4), PARAM.SignedLOWORD((nint)unchecked((int)0xF1F2F3F4)));
        }

        [ConditionalFact(typeof(ArchitectureDetection), nameof(ArchitectureDetection.Is64bit))]
        public unsafe void SignedLOWORD_x64_Result()
        {
            Assert.Equal(unchecked((short)0x0304), PARAM.SignedLOWORD(unchecked((nint)(long)0x0506070801020304)));
            Assert.Equal(unchecked((short)0xF3F4), PARAM.SignedLOWORD(unchecked((nint)unchecked((long)0xF5F6F7F8F1F2F3F4))));
        }

        [ConditionalFact(typeof(ArchitectureDetection), nameof(ArchitectureDetection.Is32bit))]
        public unsafe void SignedHIWORD_x32_Result()
        {
            Assert.Equal(unchecked((short)0x0102), PARAM.SignedHIWORD((nint)(int)0x01020304));
            Assert.Equal(unchecked((short)0xF1F2), PARAM.SignedHIWORD((nint)unchecked((int)0xF1F2F3F4)));
        }

        [ConditionalFact(typeof(ArchitectureDetection), nameof(ArchitectureDetection.Is64bit))]
        public unsafe void SignedHIWORD_x64_Result()
        {
            Assert.Equal(unchecked((short)0x0102), PARAM.SignedHIWORD(unchecked((nint)unchecked((long)0x0506070801020304))));
            Assert.Equal(unchecked((short)0xF1F2), PARAM.SignedHIWORD(unchecked((nint)unchecked((long)0xF5F6F7F8F1F2F3F4))));
        }

        [ConditionalFact(typeof(ArchitectureDetection), nameof(ArchitectureDetection.Is32bit))]
        public unsafe void ToInt_x32_Result()
        {
            Assert.Equal(unchecked((int)0x01020304), PARAM.ToInt((nint)(int)0x01020304));
            Assert.Equal(unchecked((int)0xF1F2F3F4), PARAM.ToInt((nint)unchecked((int)0xF1F2F3F4)));
        }

        [ConditionalFact(typeof(ArchitectureDetection), nameof(ArchitectureDetection.Is64bit))]
        public unsafe void ToInt_x64_Result()
        {
            Assert.Equal(unchecked((int)0x01020304), PARAM.ToInt(unchecked((nint)(long)0x0506070801020304)));
            Assert.Equal(unchecked((int)0xF1F2F3F4), PARAM.ToInt(unchecked((nint)(long)0xF5F6F7F8F1F2F3F4)));
        }

        [ConditionalFact(typeof(ArchitectureDetection), nameof(ArchitectureDetection.Is32bit))]
        public unsafe void ToUInt_x32_Result()
        {
            Assert.Equal((uint)0x01020304, PARAM.ToUInt((nint)(int)0x01020304));
            Assert.Equal((uint)0xF1F2F3F4, PARAM.ToUInt((nint)unchecked((int)0xF1F2F3F4)));
        }

        [ConditionalFact(typeof(ArchitectureDetection), nameof(ArchitectureDetection.Is64bit))]
        public unsafe void ToUInt_x64_Result()
        {
            Assert.Equal((uint)0x01020304, PARAM.ToUInt(unchecked((nint)(long)0x0506070801020304)));
            Assert.Equal((uint)0xF1F2F3F4, PARAM.ToUInt(unchecked((nint)(long)0xF5F6F7F8F1F2F3F4)));
        }
    }
}
