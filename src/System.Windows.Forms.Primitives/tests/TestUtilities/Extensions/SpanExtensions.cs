// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System
{
    public static class SpanExtensions
    {
        public static T2[] Transform<T1, T2>(this ReadOnlySpan<T1> span, Func<T1, T2> transform)
        {
            T2[] output = new T2[span.Length];
            for (int i = 0; i < span.Length; i++)
            {
                output[i] = transform(span[i]);
            }
            return output;
        }
    }
}
