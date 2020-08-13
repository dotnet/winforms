// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Oleaut32
    {
        public unsafe ref struct VARIANTVector
        {
            public VARIANT[] Variants;

            public VARIANTVector(object[]? values)
            {
                if (values is null)
                {
                    Variants = Array.Empty<VARIANT>();
                    return;
                }

                var variants = new VARIANT[values.Length];
                fixed (VARIANT* pVariants = variants)
                {
                    for (int i = 0; i < values.Length; ++i)
                    {
                        Marshal.GetNativeVariantForObject(values[i], (IntPtr)(&pVariants[i]));
                    }
                }

                Variants = variants;
            }

            public void Dispose()
            {
                foreach (VARIANT variant in Variants)
                {
                    variant.Dispose();
                }

                Variants = Array.Empty<VARIANT>();
            }
        }
    }
}
