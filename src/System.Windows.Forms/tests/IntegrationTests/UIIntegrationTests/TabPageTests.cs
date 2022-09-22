// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using System.Windows.Forms.TestUtilities;
using Xunit;
using Xunit.Abstractions;

namespace System.Windows.Forms.UITests
{
    public class TabPageTests : ControlTestBase
    {
        public TabPageTests(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
        }

        public static IEnumerable<object[]> BackColor_GetVisualStylesWithParent_TestData()
        {
            yield return new object[] { true, TabAppearance.Buttons, Control.DefaultBackColor };
            yield return new object[] { true, TabAppearance.FlatButtons, Control.DefaultBackColor };
            yield return new object[] { true, TabAppearance.Normal, Color.Transparent };
            yield return new object[] { false, TabAppearance.Buttons, Control.DefaultBackColor };
            yield return new object[] { false, TabAppearance.FlatButtons, Control.DefaultBackColor };
            yield return new object[] { false, TabAppearance.Normal, Control.DefaultBackColor };
        }

        [WinFormsTheory]
        [CommonMemberData(typeof(CommonTestHelper), nameof(CommonTestHelper.GetBoolTheoryData))]
        public void TabPage_BackColor_GetVisualStyles_ReturnsExpected(bool useVisualStyleBackColor)
        {
            using var control = new TabPage
            {
                UseVisualStyleBackColor = useVisualStyleBackColor
            };
            Assert.Equal(Control.DefaultBackColor, control.BackColor);
        }

        [WinFormsTheory]
        [MemberData(nameof(BackColor_GetVisualStylesWithParent_TestData))]
        public void TabPage_BackColor_GetVisualStylesWithParent_ReturnsExpected(bool useVisualStyleBackColor, TabAppearance parentAppearance, Color expected)
        {
            using var parent = new TabControl
            {
                Appearance = parentAppearance
            };
            using var control = new TabPage
            {
                UseVisualStyleBackColor = useVisualStyleBackColor,
                Parent = parent
            };
            Assert.Equal(expected.ToArgb(), control.BackColor.ToArgb());
        }
    }
}
