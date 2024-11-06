// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.Design.Serialization;
using System.Drawing;

namespace System.Private.Windows.Core.TestUtilities;

public static class CommonTestHelper
{
    #region Primitives

    public static TheoryData<string, string, int> GetCtrlBackspaceData()
        => new()
        {
            { "aaa", "", 0 },
            { "---", "", 0 },
            { " aaa", "", 0 },
            { " ---", "", 0 },
            { "aaa---", "", 0 },
            { "---aaa", "---", 0 },
            { "aaa---aaa", "aaa---", 0 },
            { "---aaa---", "---", 0 },
            { "a-a", "a-", 0 },
            { "-a-", "", 0 },
            { "--a-", "--", 0 },
            { "abc", "c", -1 },
            { "a,1-b", "a,b", -1 }
        };

    public static TheoryData<string, string, int> GetCtrlBackspaceRepeatedData()
        => new()
        {
            { "aaa", "", 2 },
            { "---", "", 2 },
            { "aaa---aaa", "", 2 },
            { "---aaa---", "", 2 },
            { "aaa bbb", "", 2 },
            { "aaa bbb ccc", "aaa ", 2 },
            { "aaa --- ccc", "", 2 },
            { "1 2 3 4 5 6 7 8 9 0", "1 ", 9 }
        };

    public static TheoryData<char> GetCharTheoryData()
        => new()
        {
            '\0',
            'a'
        };

    public static TheoryData<IntPtr> GetIntPtrTheoryData()
        => new()
        {
            -1,
            IntPtr.Zero,
            1
        };

    public static TheoryData<Color> GetColorTheoryData()
        => new()
        {
            Color.Red,
            Color.Blue,
            Color.Black
        };

    public static TheoryData<Color> GetColorWithEmptyTheoryData()
        => new()
        {
            Color.Red,
            Color.Empty
        };

    public static TheoryData<Point> GetPointTheoryData() => GetPointTheoryData(TestIncludeType.All);

    public static TheoryData<Point> GetPointTheoryData(TestIncludeType includeType)
    {
        TheoryData<Point> data = new();
        if (!includeType.HasFlag(TestIncludeType.NoPositives))
        {
            data.Add(default);
            data.Add(new Point(10));
            data.Add(new Point(1, 2));
        }

        if (!includeType.HasFlag(TestIncludeType.NoNegatives))
        {
            data.Add(new Point(int.MaxValue, int.MinValue));
            data.Add(new Point(-1, -2));
        }

        return data;
    }

    public static TheoryData<Size> GetSizeTheoryData() => GetSizeTheoryData(TestIncludeType.All);

    public static TheoryData<Size> GetSizeTheoryData(TestIncludeType includeType)
    {
        TheoryData<Size> data = new();
        if (!includeType.HasFlag(TestIncludeType.NoPositives))
        {
            data.Add(default);
            data.Add(new Size(new Point(1, 1)));
            data.Add(new Size(1, 2));
        }

        if (!includeType.HasFlag(TestIncludeType.NoNegatives))
        {
            data.Add(new Size(-1, 1));
            data.Add(new Size(1, -1));
        }

        return data;
    }

    public static TheoryData<Type?, bool> GetConvertFromTheoryData()
        => new()
        {
            { typeof(bool), false },
            { typeof(InstanceDescriptor), true },
            { typeof(int), false },
            { typeof(double), false },
            { null, false }
        };

    public static TheoryData<EventArgs?> GetEventArgsTheoryData()
        => new()
        {
            null,
            new EventArgs()
        };

    #endregion
}
