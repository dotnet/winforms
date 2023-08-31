// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms.TestUtilities;

/// <summary>
///  This class contains methods that return a list of parameters for unit tests.
///  These methods have been moved to a static class since we have several test classes that use the same test parameters.
///  Thus, we reduce the amount of copy-paste and we can be sure that all tests use the same data sets.
/// </summary>
public static class TrackBarTestHelper
{
    private const int Maximum = 10;
    private const int Minumim = 0;

    public static IEnumerable<object[]> TrackBarAccessibleObject_ButtonsAreDisplayed_TestData()
    {
        foreach (Orientation orientation in Enum.GetValues(typeof(Orientation)))
        {
            foreach (RightToLeft rightToLeft in new RightToLeft[] { RightToLeft.Yes, RightToLeft.No })
            {
                foreach (bool rightToLeftLayout in new[] { true, false })
                {
                    yield return new object[] { orientation, rightToLeft, rightToLeftLayout, Minumim, Maximum, /*value*/ 5 };
                }
            }
        }
    }

    public static IEnumerable<object[]> TrackBarAccessibleObject_FirstButtonIsDisplayed_TestData()
    {
        foreach (Orientation orientation in Enum.GetValues(typeof(Orientation)))
        {
            foreach (RightToLeft rightToLeft in new RightToLeft[] { RightToLeft.Yes, RightToLeft.No })
            {
                foreach (bool rightToLeftLayout in new[] { true, false })
                {
                    bool isMirrored = rightToLeft == RightToLeft.Yes && rightToLeftLayout;

                    // Depending on orientation and RTL settings, the first button can be shown
                    // or hidden when the trackbar value is at minimum or maximum.
                    int value = orientation == Orientation.Horizontal && (rightToLeft == RightToLeft.No || isMirrored)
                        ? Maximum
                        : Minumim;

                    yield return new object[] { orientation, rightToLeft, rightToLeftLayout, Minumim, Maximum, /*value*/ 5 };
                    yield return new object[] { orientation, rightToLeft, rightToLeftLayout, Minumim, Maximum, value };
                }
            }
        }
    }

    public static IEnumerable<object[]> TrackBarAccessibleObject_FirstButtonIsHidden_TestData()
    {
        foreach (Orientation orientation in Enum.GetValues(typeof(Orientation)))
        {
            foreach (RightToLeft rightToLeft in new RightToLeft[] { RightToLeft.Yes, RightToLeft.No })
            {
                foreach (bool rightToLeftLayout in new[] { true, false })
                {
                    bool isMirrored = rightToLeft == RightToLeft.Yes && rightToLeftLayout;

                    // Depending on orientation and RTL settings, the first button can be shown
                    // or hidden when the trackbar value is at minimum or maximum.
                    int value = orientation == Orientation.Horizontal && (rightToLeft == RightToLeft.No || isMirrored)
                        ? Minumim
                        : Maximum;

                    yield return new object[] { orientation, rightToLeft, rightToLeftLayout, Minumim, Maximum, value };
                }
            }
        }
    }

    public static IEnumerable<object[]> TrackBarAccessibleObject_LastButtonIsDisplayed_TestData()
    {
        foreach (Orientation orientation in Enum.GetValues(typeof(Orientation)))
        {
            foreach (RightToLeft rightToLeft in new RightToLeft[] { RightToLeft.Yes, RightToLeft.No })
            {
                foreach (bool rightToLeftLayout in new[] { true, false })
                {
                    bool isMirrored = rightToLeft == RightToLeft.Yes && rightToLeftLayout;

                    // Depending on orientation and RTL settings, the first button can be shown
                    // or hidden when the trackbar value is at minimum or maximum.
                    int value = orientation == Orientation.Horizontal && (rightToLeft == RightToLeft.No || isMirrored)
                        ? Minumim
                        : Maximum;

                    yield return new object[] { orientation, rightToLeft, rightToLeftLayout, Minumim, Maximum, /*value*/ 5 };
                    yield return new object[] { orientation, rightToLeft, rightToLeftLayout, Minumim, Maximum, value };
                    yield return new object[] { orientation, rightToLeft, rightToLeftLayout, Minumim, /*maximum*/ 0, /*value*/ 0 };
                }
            }
        }
    }

    public static IEnumerable<object[]> TrackBarAccessibleObject_LastButtonIsHidden_TestData()
    {
        foreach (Orientation orientation in Enum.GetValues(typeof(Orientation)))
        {
            foreach (RightToLeft rightToLeft in new RightToLeft[] { RightToLeft.Yes, RightToLeft.No })
            {
                foreach (bool rightToLeftLayout in new[] { true, false })
                {
                    bool isMirrored = rightToLeft == RightToLeft.Yes && rightToLeftLayout;

                    // Depending on orientation and RTL settings, the first button can be shown
                    // or hidden when the trackbar value is at minimum or maximum.
                    int value = orientation == Orientation.Horizontal && (rightToLeft == RightToLeft.No || isMirrored)
                        ? Maximum
                        : Minumim;

                    yield return new object[] { orientation, rightToLeft, rightToLeftLayout, Minumim, Maximum, value };
                }
            }
        }
    }

    public static IEnumerable<object[]> TrackBarAccessibleObject_MinimumEqualsMaximum_TestData()
    {
        foreach (Orientation orientation in Enum.GetValues(typeof(Orientation)))
        {
            foreach (RightToLeft rightToLeft in new RightToLeft[] { RightToLeft.Yes, RightToLeft.No })
            {
                foreach (bool rightToLeftLayout in new[] { true, false })
                {
                    yield return new object[] { orientation, rightToLeft, rightToLeftLayout, 0, 0, 0 };
                }
            }
        }
    }

    public static IEnumerable<object[]> TrackBarAccessibleObject_TestData()
    {
        foreach (Orientation orientation in Enum.GetValues(typeof(Orientation)))
        {
            foreach (RightToLeft rightToLeft in new RightToLeft[] { RightToLeft.Yes, RightToLeft.No })
            {
                foreach (bool rightToLeftLayout in new[] { true, false })
                {
                    foreach (bool createControl in new[] { true, false })
                    {
                        yield return new object[] { orientation, rightToLeft, rightToLeftLayout, createControl, Minumim, Maximum, /*value*/ 5 };
                        yield return new object[] { orientation, rightToLeft, rightToLeftLayout, createControl, Minumim, Maximum, /*value*/ 0 };
                        yield return new object[] { orientation, rightToLeft, rightToLeftLayout, createControl, Minumim, Maximum, /*value*/ 10 };
                        yield return new object[] { orientation, rightToLeft, rightToLeftLayout, createControl, Minumim, /*maximum*/ 0, /*value*/ 0 };
                    }
                }
            }
        }
    }

    public static IEnumerable<object[]> TrackBarAccessibleObject_WithoutCreateControl_TestData()
    {
        foreach (Orientation orientation in Enum.GetValues(typeof(Orientation)))
        {
            foreach (RightToLeft rightToLeft in new RightToLeft[] { RightToLeft.Yes, RightToLeft.No })
            {
                foreach (bool rightToLeftLayout in new[] { true, false })
                {
                    yield return new object[] { orientation, rightToLeft, rightToLeftLayout, Minumim, Maximum, /*value*/ 5 };
                    yield return new object[] { orientation, rightToLeft, rightToLeftLayout, Minumim, Maximum, /*value*/ 0 };
                    yield return new object[] { orientation, rightToLeft, rightToLeftLayout, Minumim, Maximum, /*value*/ 10 };
                    yield return new object[] { orientation, rightToLeft, rightToLeftLayout, Minumim, /*maximum*/ 0, /*value*/ 0 };
                }
            }
        }
    }
}
