// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace System.Windows.Forms.Metafiles
{
    public static class DataHelpers
    {
        public static unsafe Point[] PointArray(params int[] values)
        {
            if (values is null)
                throw new ArgumentNullException(nameof(values));

            if (values.Length % 2 != 0)
                throw new ArgumentOutOfRangeException(nameof(values));

            Point[] points = new Point[values.Length / 2];
            fixed (int* i = values)
            fixed (Point* p = points)
            {
                Unsafe.CopyBlock(p, i, (uint)(sizeof(int) * values.Length));
            }

            return points;
        }

        public static unsafe Point[] PointArray(Matrix3x2 transform, params int[] values)
        {
            Point[] points = PointArray(values);
            for (int i = 0; i < points.Length; i++)
            {
                points[i] = transform.TransformPoint(points[i]);
            }

            return points;
        }
    }
}
