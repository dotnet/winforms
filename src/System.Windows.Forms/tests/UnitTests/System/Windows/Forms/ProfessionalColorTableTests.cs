// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Drawing;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ProfessionalColorTableTests
    {
        public ProfessionalColorTableTests()
        {
            Application.ThreadException += (sender, e) => throw new Exception(e.Exception.StackTrace.ToString());
        }

        [WinFormsFact]
        public void ProfessionalColorTable_Ctor_Default()
        {
            var table = new ProfessionalColorTable();
            Assert.False(table.UseSystemColors);
        }

        public static IEnumerable<object[]> Properties_TestData()
        {
            // Helper function to avoid casting.
            Func<ProfessionalColorTable, T> I<T>(Func<ProfessionalColorTable, T> t) => t;

            yield return new object[] { I(t => t.ButtonCheckedGradientBegin) };
            yield return new object[] { I(t => t.ButtonCheckedGradientEnd) };
            yield return new object[] { I(t => t.ButtonCheckedGradientMiddle) };
            yield return new object[] { I(t => t.ButtonCheckedHighlight) };
            yield return new object[] { I(t => t.ButtonCheckedHighlightBorder) };
            yield return new object[] { I(t => t.ButtonPressedBorder) };
            yield return new object[] { I(t => t.ButtonPressedGradientBegin) };
            yield return new object[] { I(t => t.ButtonPressedGradientEnd) };
            yield return new object[] { I(t => t.ButtonPressedGradientMiddle) };
            yield return new object[] { I(t => t.ButtonPressedHighlight) };
            yield return new object[] { I(t => t.ButtonPressedHighlightBorder) };
            yield return new object[] { I(t => t.ButtonSelectedBorder) };
            yield return new object[] { I(t => t.ButtonSelectedGradientBegin) };
            yield return new object[] { I(t => t.ButtonSelectedGradientEnd) };
            yield return new object[] { I(t => t.ButtonSelectedGradientMiddle) };
            yield return new object[] { I(t => t.ButtonSelectedHighlight) };
            yield return new object[] { I(t => t.ButtonSelectedHighlightBorder) };
            yield return new object[] { I(t => t.CheckBackground) };
            yield return new object[] { I(t => t.CheckPressedBackground) };
            yield return new object[] { I(t => t.CheckSelectedBackground) };
            yield return new object[] { I(t => t.GripDark) };
            yield return new object[] { I(t => t.GripLight) };
            yield return new object[] { I(t => t.ImageMarginGradientBegin) };
            yield return new object[] { I(t => t.ImageMarginGradientMiddle) };
            yield return new object[] { I(t => t.ImageMarginRevealedGradientBegin) };
            yield return new object[] { I(t => t.ImageMarginRevealedGradientEnd) };
            yield return new object[] { I(t => t.ImageMarginRevealedGradientMiddle) };
            yield return new object[] { I(t => t.MenuBorder) };
            yield return new object[] { I(t => t.MenuItemBorder) };
            yield return new object[] { I(t => t.MenuItemPressedGradientBegin) };
            yield return new object[] { I(t => t.MenuItemPressedGradientEnd) };
            yield return new object[] { I(t => t.MenuItemPressedGradientMiddle) };
            yield return new object[] { I(t => t.MenuItemSelected) };
            yield return new object[] { I(t => t.MenuItemSelectedGradientBegin) };
            yield return new object[] { I(t => t.MenuItemSelectedGradientEnd) };
            yield return new object[] { I(t => t.MenuStripGradientBegin) };
            yield return new object[] { I(t => t.MenuStripGradientEnd) };
            yield return new object[] { I(t => t.OverflowButtonGradientBegin) };
            yield return new object[] { I(t => t.OverflowButtonGradientEnd) };
            yield return new object[] { I(t => t.OverflowButtonGradientMiddle) };
            yield return new object[] { I(t => t.RaftingContainerGradientBegin) };
            yield return new object[] { I(t => t.RaftingContainerGradientEnd) };
            yield return new object[] { I(t => t.SeparatorDark) };
            yield return new object[] { I(t => t.SeparatorLight) };
            yield return new object[] { I(t => t.StatusStripGradientBegin) };
            yield return new object[] { I(t => t.StatusStripGradientEnd) };
            yield return new object[] { I(t => t.ToolStripBorder) };
            yield return new object[] { I(t => t.ToolStripContentPanelGradientBegin) };
            yield return new object[] { I(t => t.ToolStripContentPanelGradientEnd) };
            yield return new object[] { I(t => t.ToolStripDropDownBackground) };
            yield return new object[] { I(t => t.ToolStripGradientBegin) };
            yield return new object[] { I(t => t.ToolStripGradientEnd) };
            yield return new object[] { I(t => t.ToolStripGradientMiddle) };
            yield return new object[] { I(t => t.ToolStripPanelGradientBegin) };
            yield return new object[] { I(t => t.ToolStripPanelGradientEnd) };
        }

        [WinFormsTheory]
        [MemberData(nameof(Properties_TestData))]
        public void ProfessionalColorTable_Properties_Get_ReturnsExpected(Func<ProfessionalColorTable, Color> factory)
        {
            var table = new ProfessionalColorTable();
            Color result = factory(table);
            Assert.Equal(result, factory(table));
        }

        [WinFormsTheory]
        [MemberData(nameof(Properties_TestData))]
        public void ProfessionalColorTable_Properties_GetUseSystemColors_ReturnsExpected(Func<ProfessionalColorTable, Color> factory)
        {
            var table = new ProfessionalColorTable
            {
                UseSystemColors = true
            };
            Color result = factory(table);
            Assert.Equal(result, factory(table));
        }

        [WinFormsTheory]
        [MemberData(nameof(Properties_TestData))]
        public void ProfessionalColorTable_Properties_GetToolStripManagerVisualStylesEnabled_ReturnsExpected(Func<ProfessionalColorTable, Color> factory)
        {
            bool oldValue = ToolStripManager.VisualStylesEnabled;
            try
            {
                ToolStripManager.VisualStylesEnabled = true;

                var table = new ProfessionalColorTable();
                Color result = factory(table);
                Assert.Equal(result, factory(table));
            }
            finally
            {
                ToolStripManager.VisualStylesEnabled = oldValue;
            }
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ProfessionalColorTable_ImageMarginGradientEnd_Get_ReturnsExpected(bool useSystemColors)
        {
            var table = new ProfessionalColorTable
            {
                UseSystemColors = useSystemColors
            };
            Assert.NotEqual(table.ImageMarginGradientEnd, table.ImageMarginGradientEnd);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ProfessionalColorTable_UseSystemColors_Set_GetReturnsExpected(bool value)
        {
            var table = new ProfessionalColorTable
            {
                UseSystemColors = value
            };
            Assert.Equal(value, table.UseSystemColors);
            
            // Set same.
            table.UseSystemColors = value;
            Assert.Equal(value, table.UseSystemColors);
            
            // Set different.
            table.UseSystemColors = !value;
            Assert.Equal(!value, table.UseSystemColors);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ProfessionalColorTable_UseSystemColors_SetWithKnownColor_GetReturnsExpected(bool value)
        {
            var table = new ProfessionalColorTable
            {
                UseSystemColors = !value
            };
            Color result = table.ButtonCheckedGradientBegin;
            Assert.Equal(result, table.ButtonCheckedGradientBegin);

            table.UseSystemColors = value;
            Assert.Equal(value, table.UseSystemColors);
            
            // Set same.
            table.UseSystemColors = value;
            Assert.Equal(value, table.UseSystemColors);
            
            // Set different.
            table.UseSystemColors = !value;
            Assert.Equal(!value, table.UseSystemColors);
        }
    }
}
