// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ProfessionalColorsTests : IClassFixture<ThreadExceptionFixture>
    {
        public static IEnumerable<object[]> Properties_TestData()
        {
            Func<T> I<T>(Func<T> t) => t;

            yield return new object[] { I(() => ProfessionalColors.ButtonCheckedGradientBegin) };
            yield return new object[] { I(() => ProfessionalColors.ButtonCheckedGradientEnd) };
            yield return new object[] { I(() => ProfessionalColors.ButtonCheckedGradientMiddle) };
            yield return new object[] { I(() => ProfessionalColors.ButtonCheckedHighlight) };
            yield return new object[] { I(() => ProfessionalColors.ButtonCheckedHighlightBorder) };
            yield return new object[] { I(() => ProfessionalColors.ButtonPressedBorder) };
            yield return new object[] { I(() => ProfessionalColors.ButtonPressedGradientBegin) };
            yield return new object[] { I(() => ProfessionalColors.ButtonPressedGradientEnd) };
            yield return new object[] { I(() => ProfessionalColors.ButtonPressedGradientMiddle) };
            yield return new object[] { I(() => ProfessionalColors.ButtonPressedHighlight) };
            yield return new object[] { I(() => ProfessionalColors.ButtonPressedHighlightBorder) };
            yield return new object[] { I(() => ProfessionalColors.ButtonSelectedBorder) };
            yield return new object[] { I(() => ProfessionalColors.ButtonSelectedGradientBegin) };
            yield return new object[] { I(() => ProfessionalColors.ButtonSelectedGradientEnd) };
            yield return new object[] { I(() => ProfessionalColors.ButtonSelectedGradientMiddle) };
            yield return new object[] { I(() => ProfessionalColors.ButtonSelectedHighlight) };
            yield return new object[] { I(() => ProfessionalColors.ButtonSelectedHighlightBorder) };
            yield return new object[] { I(() => ProfessionalColors.CheckBackground) };
            yield return new object[] { I(() => ProfessionalColors.CheckPressedBackground) };
            yield return new object[] { I(() => ProfessionalColors.CheckSelectedBackground) };
            yield return new object[] { I(() => ProfessionalColors.GripDark) };
            yield return new object[] { I(() => ProfessionalColors.GripLight) };
            yield return new object[] { I(() => ProfessionalColors.ImageMarginGradientBegin) };
            yield return new object[] { I(() => ProfessionalColors.ImageMarginGradientMiddle) };
            yield return new object[] { I(() => ProfessionalColors.ImageMarginRevealedGradientBegin) };
            yield return new object[] { I(() => ProfessionalColors.ImageMarginRevealedGradientEnd) };
            yield return new object[] { I(() => ProfessionalColors.ImageMarginRevealedGradientMiddle) };
            yield return new object[] { I(() => ProfessionalColors.MenuBorder) };
            yield return new object[] { I(() => ProfessionalColors.MenuItemBorder) };
            yield return new object[] { I(() => ProfessionalColors.MenuItemPressedGradientBegin) };
            yield return new object[] { I(() => ProfessionalColors.MenuItemPressedGradientEnd) };
            yield return new object[] { I(() => ProfessionalColors.MenuItemPressedGradientMiddle) };
            yield return new object[] { I(() => ProfessionalColors.MenuItemSelected) };
            yield return new object[] { I(() => ProfessionalColors.MenuItemSelectedGradientBegin) };
            yield return new object[] { I(() => ProfessionalColors.MenuItemSelectedGradientEnd) };
            yield return new object[] { I(() => ProfessionalColors.MenuStripGradientBegin) };
            yield return new object[] { I(() => ProfessionalColors.MenuStripGradientEnd) };
            yield return new object[] { I(() => ProfessionalColors.OverflowButtonGradientBegin) };
            yield return new object[] { I(() => ProfessionalColors.OverflowButtonGradientEnd) };
            yield return new object[] { I(() => ProfessionalColors.OverflowButtonGradientMiddle) };
            yield return new object[] { I(() => ProfessionalColors.RaftingContainerGradientBegin) };
            yield return new object[] { I(() => ProfessionalColors.RaftingContainerGradientEnd) };
            yield return new object[] { I(() => ProfessionalColors.SeparatorDark) };
            yield return new object[] { I(() => ProfessionalColors.SeparatorLight) };
            yield return new object[] { I(() => ProfessionalColors.StatusStripGradientBegin) };
            yield return new object[] { I(() => ProfessionalColors.StatusStripGradientEnd) };
            yield return new object[] { I(() => ProfessionalColors.ToolStripBorder) };
            yield return new object[] { I(() => ProfessionalColors.ToolStripContentPanelGradientBegin) };
            yield return new object[] { I(() => ProfessionalColors.ToolStripContentPanelGradientEnd) };
            yield return new object[] { I(() => ProfessionalColors.ToolStripDropDownBackground) };
            yield return new object[] { I(() => ProfessionalColors.ToolStripGradientBegin) };
            yield return new object[] { I(() => ProfessionalColors.ToolStripGradientEnd) };
            yield return new object[] { I(() => ProfessionalColors.ToolStripGradientMiddle) };
            yield return new object[] { I(() => ProfessionalColors.ToolStripPanelGradientBegin) };
            yield return new object[] { I(() => ProfessionalColors.ToolStripPanelGradientEnd) };
        }

        [WinFormsTheory]
        [MemberData(nameof(Properties_TestData))]
        public void ProfessionalColors_Properties_Get_ReturnsExpected(Func<Color> factory)
        {
            Color result = factory();
            Assert.Equal(result, factory());
        }

        [WinFormsTheory]
        [MemberData(nameof(Properties_TestData))]
        public void ProfessionalColors_Properties_GetToolStripManagerVisualStylesEnabled_ReturnsExpected(Func<Color> factory)
        {
            bool oldValue = ToolStripManager.VisualStylesEnabled;
            try
            {
                ToolStripManager.VisualStylesEnabled = true;

                Color result = factory();
                Assert.Equal(result, factory());
            }
            finally
            {
                ToolStripManager.VisualStylesEnabled = oldValue;
            }
        }

        [WinFormsFact]
        public void ProfessionalColors_ImageMarginGradientEnd_Get_ReturnsExpected()
        {
            Assert.NotEqual(ProfessionalColors.ImageMarginGradientEnd, ProfessionalColors.ImageMarginGradientEnd);
        }
    }
}
