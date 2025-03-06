// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.TestUtilities;

public static class ScrollBarTestHelper
{
    private const int Maximum = 100;
    private const int Minumim = 0;

    public static IEnumerable<object[]> HScrollBarAccessibleObject_FirstPageButtonIsDisplayed_TestData()
    {
        foreach (RightToLeft rightToLeft in new RightToLeft[] { RightToLeft.Yes, RightToLeft.No })
        {
            // Depending on orientation and RTL settings, the first button can be shown
            // or hidden when the trackbar value is at minimum or maximum.
            int value = rightToLeft == RightToLeft.Yes ? Minumim : Maximum;

            yield return new object[] { rightToLeft, Minumim, Maximum, /*value*/ 50 };
            yield return new object[] { rightToLeft, Minumim, Maximum, value };
        }
    }

    public static IEnumerable<object[]> HScrollBarAccessibleObject_FirstPageButtonIsHidden_TestData()
    {
        foreach (RightToLeft rightToLeft in new RightToLeft[] { RightToLeft.Yes, RightToLeft.No })
        {
            // Depending on orientation and RTL settings, the first button can be shown
            // or hidden when the trackbar value is at minimum or maximum.
            int value = rightToLeft == RightToLeft.Yes ? Maximum : Minumim;

            yield return new object[] { rightToLeft, Minumim, Maximum, value };
        }
    }

    public static IEnumerable<object[]> HScrollBarAccessibleObject_LastPageButtonIsDisplayed_TestData()
    {
        foreach (RightToLeft rightToLeft in new RightToLeft[] { RightToLeft.Yes, RightToLeft.No })
        {
            // Depending on orientation and RTL settings, the first button can be shown
            // or hidden when the trackbar value is at minimum or maximum.
            int value = rightToLeft == RightToLeft.Yes ? Maximum : Minumim;

            yield return new object[] { rightToLeft, Minumim, Maximum, /*value*/ 50 };
            yield return new object[] { rightToLeft, Minumim, Maximum, value };
        }
    }

    public static IEnumerable<object[]> HScrollBarAccessibleObject_LastPageButtonIsHidden_TestData()
    {
        foreach (RightToLeft rightToLeft in new RightToLeft[] { RightToLeft.Yes, RightToLeft.No })
        {
            // Depending on orientation and RTL settings, the first button can be shown
            // or hidden when the trackbar value is at minimum or maximum.
            int value = rightToLeft == RightToLeft.Yes ? Minumim : Maximum;

            yield return new object[] { rightToLeft, Minumim, Maximum, value };
        }
    }

    public static IEnumerable<object[]> ScrollBarAccessibleObject_BothButtonAreDisplayed_TestData()
    {
        foreach (RightToLeft rightToLeft in new RightToLeft[] { RightToLeft.Yes, RightToLeft.No })
        {
            yield return new object[] { rightToLeft, Minumim, Maximum, /*value*/ 50 };
        }
    }

    public static IEnumerable<object[]> ScrollBarAccessibleObject_MinimumEqualsMaximum_TestData()
    {
        foreach (RightToLeft rightToLeft in new RightToLeft[] { RightToLeft.Yes, RightToLeft.No })
        {
            yield return new object[] { rightToLeft, /*Minumim*/ 0, /*Maximum*/ 0, /*Value*/ 0 };
        }
    }

    public static IEnumerable<object[]> ScrollBarAccessibleObject_TestData()
    {
        foreach (bool createControl in new bool[] { true, false })
        {
            foreach (RightToLeft rightToLeft in new RightToLeft[] { RightToLeft.Yes, RightToLeft.No })
            {
                yield return new object[] { createControl, rightToLeft, Minumim, Maximum, /*Value*/ 0 };
                yield return new object[] { createControl, rightToLeft, Minumim, Maximum, /*Value*/ 50 };
                yield return new object[] { createControl, rightToLeft, Minumim, Maximum, /*Value*/ 100 };
                yield return new object[] { createControl, rightToLeft, Minumim, /*Maximum*/ 0, /*Value*/ 0 };
            }
        }
    }

    public static IEnumerable<object[]> ScrollBarAccessibleObject_WithoutCreateControl_TestData()
    {
        foreach (RightToLeft rightToLeft in new RightToLeft[] { RightToLeft.Yes, RightToLeft.No })
        {
            yield return new object[] { rightToLeft, Minumim, Maximum, /*Value*/ 0 };
            yield return new object[] { rightToLeft, Minumim, Maximum, /*Value*/ 50 };
            yield return new object[] { rightToLeft, Minumim, Maximum, /*Value*/ 100 };
            yield return new object[] { rightToLeft, Minumim, /*Maximum*/ 0, /*Value*/ 0 };
        }
    }

    public static IEnumerable<object[]> VScrollBarAccessibleObject_FirstPageButtonIsDisplayed_TestData()
    {
        foreach (RightToLeft rightToLeft in new RightToLeft[] { RightToLeft.Yes, RightToLeft.No })
        {
            yield return new object[] { rightToLeft, Minumim, Maximum, /*value*/ 50 };
            yield return new object[] { rightToLeft, Minumim, Maximum, /*value*/ Maximum };
        }
    }

    public static IEnumerable<object[]> VScrollBarAccessibleObject_FirstPageButtonIsHidden_TestData()
    {
        foreach (RightToLeft rightToLeft in new RightToLeft[] { RightToLeft.Yes, RightToLeft.No })
        {
            yield return new object[] { rightToLeft, Minumim, Maximum, /*value*/ Minumim };
        }
    }

    public static IEnumerable<object[]> VScrollBarAccessibleObject_LastPageButtonIsDisplayed_TestData()
    {
        foreach (RightToLeft rightToLeft in new RightToLeft[] { RightToLeft.Yes, RightToLeft.No })
        {
            yield return new object[] { rightToLeft, Minumim, Maximum, /*value*/ 50 };
            yield return new object[] { rightToLeft, Minumim, Maximum, /*value*/ Minumim };
        }
    }

    public static IEnumerable<object[]> VScrollBarAccessibleObject_LastPageButtonIsHidden_TestData()
    {
        foreach (RightToLeft rightToLeft in new RightToLeft[] { RightToLeft.Yes, RightToLeft.No })
        {
            yield return new object[] { rightToLeft, Minumim, Maximum, /*value*/ Maximum };
        }
    }
}
