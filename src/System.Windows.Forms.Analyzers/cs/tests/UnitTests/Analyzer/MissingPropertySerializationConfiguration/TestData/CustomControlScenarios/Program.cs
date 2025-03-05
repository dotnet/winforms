// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace CSharpControls;

public static class Program
{
    public static void Main(string[] args)
    {
        var control = new ScalableControl();

        control.ScaleFactor = 1.5f;
        control.ScaledSize = new SizeF(100, 100);
        control.ScaledLocation = new PointF(10, 10);
    }
}
