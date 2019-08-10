// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ScreenTests
    {
        [Fact]
        public void Screen_AllScreens_Get_ReturnsExpected()
        {
            Screen[] screens = Screen.AllScreens;
            Assert.NotEmpty(screens);
            Assert.Same(screens, Screen.AllScreens);

            foreach (Screen screen in screens)
            {
                VerifyScreen(screen);
            }

            Assert.Contains(screens, s => s.Primary);
        }

        [Fact]
        public void Screen_PrimaryScreen_Get_ReturnsExpected()
        {
            Screen screen = Screen.PrimaryScreen;
            Assert.NotNull(screen);
            VerifyScreen(screen);
        }

        public static IEnumerable<object[]> Equals_Screen_TestData()
        {
            var screen = new Screen((IntPtr)1);
            yield return new object[] { screen, screen, true };
            yield return new object[] { screen, new Screen((IntPtr)1), true };
            yield return new object[] { screen, new Screen((IntPtr)2), false };
        }

        public static IEnumerable<object[]> Equals_Object_TestData()
        {
            var screen = new Screen((IntPtr)1);
            yield return new object[] { screen, new object(), false };
            yield return new object[] { screen, null, false };
        }

        [Theory]
        [MemberData(nameof(Equals_Screen_TestData))]
        [MemberData(nameof(Equals_Object_TestData))]
        public void Screen_Equals_Invoke_ReturnsExpected(Screen screen, object obj, bool expected)
        {
            Assert.Equal(expected, screen.Equals(obj));
        }

        [Theory]
        [MemberData(nameof(Equals_Screen_TestData))]
        public void Screen_GetHashCode_Invoke_ReturnsExpected(Screen screen1, Screen screen2, bool expected)
        {
            Assert.Equal(expected, screen1.GetHashCode().Equals(screen2.GetHashCode()));
        }

        public static IEnumerable<object[]> FromControl_TestData()
        {
            yield return new object[] { new Control() };

            var createdControl = new Control();
            Assert.NotEqual(IntPtr.Zero, createdControl.Handle);
            yield return new object[] { createdControl };
        }

        [Theory]
        [MemberData(nameof(FromControl_TestData))]
        public void Screen_FromControl_Invoke_ReturnsExpected(Control control)
        {
            Screen screen = Screen.FromControl(control);
            Assert.NotNull(screen);
            VerifyScreen(screen);
        }

        [Fact]
        public void Screen_FromControl_NullControl_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("control", () => Screen.FromControl(null));
        }

        public static IEnumerable<object[]> FromHandle_TestData()
        {
            yield return new object[] { IntPtr.Zero };
            yield return new object[] { new Control().Handle };
        }

        [Theory]
        [MemberData(nameof(FromHandle_TestData))]
        public void Screen_FromHandle_Invoke_ReturnsExpected(IntPtr handle)
        {
            Screen screen = Screen.FromHandle(handle);
            Assert.NotNull(screen);
            VerifyScreen(screen);
        }

        public static IEnumerable<object[]> FromPoint_TestData()
        {
            yield return new object[] { new Point(-1, -2) };
            yield return new object[] { new Point(0, 0) };
            yield return new object[] { new Point(1, 2) };
            yield return new object[] { new Point(int.MaxValue, int.MaxValue) };
        }

        [Theory]
        [MemberData(nameof(FromPoint_TestData))]
        public void Screen_FromPoint_Invoke_ReturnsExpected(Point point)
        {
            Screen screen = Screen.FromPoint(point);
            Assert.NotNull(screen);
            VerifyScreen(screen);
        }

        public static IEnumerable<object[]> FromRectangle_TestData()
        {
            yield return new object[] { new Rectangle(-1, -2, -3, -4) };
            yield return new object[] { new Rectangle(0, 0, 0, 0) };
            yield return new object[] { new Rectangle(1, 2, 3, 4) };
            yield return new object[] { new Rectangle(int.MaxValue, int.MaxValue, int.MaxValue, int.MaxValue) };
        }

        [Theory]
        [MemberData(nameof(FromRectangle_TestData))]
        public void Screen_FromRectangle_Invoke_ReturnsExpected(Rectangle rectangle)
        {
            Screen screen = Screen.FromRectangle(rectangle);
            Assert.NotNull(screen);
            VerifyScreen(screen);
        }

        [Theory]
        [MemberData(nameof(FromControl_TestData))]
        public void Screen_GetBounds_InvokeControl_ReturnsExpected(Control control)
        {
            Screen screen = Screen.FromControl(control);
            Assert.Equal(screen.Bounds, Screen.GetBounds(control));
        }

        [Theory]
        [MemberData(nameof(FromPoint_TestData))]
        public void Screen_GetBounds_InvokePoint_ReturnsExpected(Point point)
        {
            Screen screen = Screen.FromPoint(point);
            Assert.Equal(screen.Bounds, Screen.GetBounds(point));
        }

        [Theory]
        [MemberData(nameof(FromRectangle_TestData))]
        public void Screen_GetBounds_InvokeRectangle_ReturnsExpected(Rectangle rectangle)
        {
            Screen screen = Screen.FromRectangle(rectangle);
            Assert.Equal(screen.Bounds, Screen.GetBounds(rectangle));
        }

        [Fact]
        public void Screen_GetBounds_NullControl_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("control", () => Screen.GetBounds(null));
        }

        [Theory]
        [MemberData(nameof(FromControl_TestData))]
        public void Screen_GetWorkingArea_InvokeControl_ReturnsExpected(Control control)
        {
            Screen screen = Screen.FromControl(control);
            Assert.Equal(screen.WorkingArea, Screen.GetWorkingArea(control));
        }

        [Theory]
        [MemberData(nameof(FromPoint_TestData))]
        public void Screen_GetWorkingArea_InvokePoint_ReturnsExpected(Point point)
        {
            Screen screen = Screen.FromPoint(point);
            Assert.Equal(screen.WorkingArea, Screen.GetWorkingArea(point));
        }

        [Theory]
        [MemberData(nameof(FromRectangle_TestData))]
        public void Screen_GetWorkingArea_InvokeRectangle_ReturnsExpected(Rectangle rectangle)
        {
            Screen screen = Screen.FromRectangle(rectangle);
            Assert.Equal(screen.WorkingArea, Screen.GetWorkingArea(rectangle));
        }

        [Fact]
        public void Screen_GetWorkingArea_NullControl_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("control", () => Screen.GetWorkingArea(null));
        }

        [Fact]
        public void Screen_ToString_Invoke_ReturnsExpected()
        {
            Screen screen = Screen.PrimaryScreen;
            Assert.Equal($"Screen[Bounds={screen.Bounds} WorkingArea={screen.WorkingArea} Primary=True DeviceName={screen.DeviceName}", screen.ToString());
        }

        private static void VerifyScreen(Screen screen)
        {
            Assert.Contains(screen.BitsPerPixel, new int[] { 1, 2, 4, 8, 16, 24, 32, 48, 64 });
            Assert.True(screen.Bounds.Width != 0);
            Assert.True(screen.Bounds.Height != 0);
            Assert.InRange(screen.DeviceName.Length, 1, 32);
            Assert.Equal(screen.DeviceName, screen.DeviceName.Trim('\0'));
            Assert.InRange(screen.WorkingArea.Width, screen.Bounds.X, screen.Bounds.Width);
            Assert.InRange(screen.WorkingArea.Height, screen.Bounds.Y, screen.Bounds.Height);
        }
    }
}
