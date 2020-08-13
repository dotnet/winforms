// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Moq;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Printing;
using System.Runtime.InteropServices;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class PageSetupDialogTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void PageSetupDialog_Ctor_Default()
        {
            using var dialog = new PageSetupDialog();
            Assert.True(dialog.AllowMargins);
            Assert.True(dialog.AllowOrientation);
            Assert.True(dialog.AllowPaper);
            Assert.True(dialog.AllowPrinter);
            Assert.Null(dialog.Container);
            Assert.Null(dialog.Document);
            Assert.False(dialog.EnableMetric);
            Assert.Equal(new Margins(0, 0, 0, 0), dialog.MinMargins);
            Assert.Same(dialog.MinMargins, dialog.MinMargins);
            Assert.Null(dialog.PageSettings);
            Assert.Null(dialog.PrinterSettings);
            Assert.False(dialog.ShowHelp);
            Assert.True(dialog.ShowNetwork);
            Assert.Null(dialog.Site);
            Assert.Null(dialog.Tag);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void PageSetupDialog_AllowMargins_Set_GetReturnsExpected(bool value)
        {
            using var dialog = new PageSetupDialog
            {
                AllowMargins = value
            };
            Assert.Equal(value, dialog.AllowMargins);

            // Set same.
            dialog.AllowMargins = value;
            Assert.Equal(value, dialog.AllowMargins);

            // Set different.
            dialog.AllowMargins = !value;
            Assert.Equal(!value, dialog.AllowMargins);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void PageSetupDialog_AllowOrientation_Set_GetReturnsExpected(bool value)
        {
            using var dialog = new PageSetupDialog
            {
                AllowOrientation = value
            };
            Assert.Equal(value, dialog.AllowOrientation);

            // Set same.
            dialog.AllowOrientation = value;
            Assert.Equal(value, dialog.AllowOrientation);

            // Set different.
            dialog.AllowOrientation = !value;
            Assert.Equal(!value, dialog.AllowOrientation);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void PageSetupDialog_AllowPaper_Set_GetReturnsExpected(bool value)
        {
            using var dialog = new PageSetupDialog
            {
                AllowPaper = value
            };
            Assert.Equal(value, dialog.AllowPaper);

            // Set same.
            dialog.AllowPaper = value;
            Assert.Equal(value, dialog.AllowPaper);

            // Set different.
            dialog.AllowPaper = !value;
            Assert.Equal(!value, dialog.AllowPaper);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void PageSetupDialog_AllowPrinter_Set_GetReturnsExpected(bool value)
        {
            using var dialog = new PageSetupDialog
            {
                AllowPrinter = value
            };
            Assert.Equal(value, dialog.AllowPrinter);

            // Set same.
            dialog.AllowPrinter = value;
            Assert.Equal(value, dialog.AllowPrinter);

            // Set different.
            dialog.AllowPrinter = !value;
            Assert.Equal(!value, dialog.AllowPrinter);
        }

        public static IEnumerable<object[]> Document_Set_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new PrintDocument() };
        }

        [WinFormsTheory]
        [MemberData(nameof(Document_Set_TestData))]
        public void PageSetupDialog_Document_Set_GetReturnsExpected(PrintDocument value)
        {
            using var dialog = new PageSetupDialog
            {
                Document = value
            };
            Assert.Same(value, dialog.Document);
            Assert.Same(value?.DefaultPageSettings, dialog.PageSettings);
            Assert.Same(value?.PrinterSettings, dialog.PrinterSettings);

            // Set same.
            dialog.Document = value;
            Assert.Same(value, dialog.Document);
            Assert.Same(value?.DefaultPageSettings, dialog.PageSettings);
            Assert.Same(value?.PrinterSettings, dialog.PrinterSettings);
        }

        [WinFormsFact]
        public void PageSetupDialog_Document_SetWithNonNullOldValue_GetReturnsExpected()
        {
            using var dialog = new PageSetupDialog
            {
                Document = new PrintDocument()
            };

            using var value = new PrintDocument();
            dialog.Document = value;
            Assert.Same(value, dialog.Document);
            Assert.Same(value.DefaultPageSettings, dialog.PageSettings);
            Assert.Same(value.PrinterSettings, dialog.PrinterSettings);

            // Set same.
            dialog.Document = value;
            Assert.Same(value, dialog.Document);
            Assert.Same(value.DefaultPageSettings, dialog.PageSettings);
            Assert.Same(value.PrinterSettings, dialog.PrinterSettings);
        }

        [WinFormsFact]
        public void PageSetupDialog_Document_SetNullWithNonNullOldValue_GetReturnsExpected()
        {
            using var original = new PrintDocument();
            using var dialog = new PageSetupDialog
            {
                Document = original
            };

            dialog.Document = null;
            Assert.Null(dialog.Document);
            Assert.Same(original.DefaultPageSettings, dialog.PageSettings);
            Assert.Same(original.PrinterSettings, dialog.PrinterSettings);

            // Set same.
            dialog.Document = null;
            Assert.Null(dialog.Document);
            Assert.Same(original.DefaultPageSettings, dialog.PageSettings);
            Assert.Same(original.PrinterSettings, dialog.PrinterSettings);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void PageSetupDialog_EnableMetric_Set_GetReturnsExpected(bool value)
        {
            using var dialog = new PageSetupDialog
            {
                EnableMetric = value
            };
            Assert.Equal(value, dialog.EnableMetric);

            // Set same.
            dialog.EnableMetric = value;
            Assert.Equal(value, dialog.EnableMetric);

            // Set different.
            dialog.EnableMetric = !value;
            Assert.Equal(!value, dialog.EnableMetric);
        }

        public static IEnumerable<object[]> MinMargins_Set_TestData()
        {
            yield return new object[] { null, new Margins(0, 0, 0, 0) };
            yield return new object[] { new Margins(), new Margins() };
            yield return new object[] { new Margins(1, 2, 3, 4), new Margins(1, 2, 3, 4) };
            yield return new object[] { new Margins(0, 0, 0, 0), new Margins(0, 0, 0, 0) };
            yield return new object[] { new Margins(1, 0, 0, 0), new Margins(1, 0, 0, 0) };
            yield return new object[] { new Margins(0, 1, 0, 0), new Margins(0, 1, 0, 0) };
            yield return new object[] { new Margins(0, 0, 1, 0), new Margins(0, 0, 1, 0) };
            yield return new object[] { new Margins(0, 0, 0, 1), new Margins(0, 0, 0, 1) };
        }

        [WinFormsTheory]
        [MemberData(nameof(MinMargins_Set_TestData))]
        public void PageSetupDialog_MinMargins_Set_GetReturnsExpected(Margins value, Margins expected)
        {
            using var dialog = new PageSetupDialog
            {
                MinMargins = value
            };
            Assert.Equal(expected, dialog.MinMargins);

            // Set same.
            dialog.MinMargins = value;
            Assert.Equal(expected, dialog.MinMargins);
        }

        [WinFormsFact]
        public void PageSetupDialog_MinMargins_ResetValue_Success()
        {
            using var dialog = new PageSetupDialog();
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(PageSetupDialog))[nameof(PageSetupDialog.MinMargins)];
            Assert.False(property.CanResetValue(dialog));

            dialog.MinMargins = new Margins(1, 2, 3, 4);
            Assert.True(property.CanResetValue(dialog));
            Assert.Equal(new Margins(1, 2, 3, 4), dialog.MinMargins);

            property.ResetValue(dialog);
            Assert.False(property.CanResetValue(dialog));
            Assert.Equal(new Margins(0, 0, 0, 0), dialog.MinMargins);
        }

        public static IEnumerable<object[]> MinMargins_ShouldSerializeValue_TestData()
        {
            yield return new object[] { null, false };
            yield return new object[] { new Margins(), true };
            yield return new object[] { new Margins(1, 2, 3, 4), true };
            yield return new object[] { new Margins(0, 0, 0, 0), false };
            yield return new object[] { new Margins(1, 0, 0, 0), true };
            yield return new object[] { new Margins(0, 1, 0, 0), true };
            yield return new object[] { new Margins(0, 0, 1, 0), true };
            yield return new object[] { new Margins(0, 0, 0, 10), true };
        }

        [WinFormsTheory]
        [MemberData(nameof(MinMargins_ShouldSerializeValue_TestData))]
        public void PageSetupDialog_MinMargins_ShouldSerializeValue_ReturnsExpected(Margins value, bool expected)
        {
            using var dialog = new PageSetupDialog();
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(PageSetupDialog))[nameof(PageSetupDialog.MinMargins)];
            Assert.False(property.ShouldSerializeValue(dialog));

            dialog.MinMargins = value;
            Assert.Equal(expected, property.ShouldSerializeValue(dialog));
        }

        public static IEnumerable<object[]> PageSettings_Set_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new PageSettings() };
        }

        [WinFormsTheory]
        [MemberData(nameof(PageSettings_Set_TestData))]
        public void PageSetupDialog_PageSettings_Set_GetReturnsExpected(PageSettings value)
        {
            using var dialog = new PageSetupDialog
            {
                PageSettings = value
            };
            Assert.Same(value, dialog.PageSettings);
            Assert.Null(dialog.Document);

            // Set same.
            dialog.PageSettings = value;
            Assert.Same(value, dialog.PageSettings);
            Assert.Null(dialog.Document);
        }

        [WinFormsTheory]
        [MemberData(nameof(PageSettings_Set_TestData))]
        public void PageSetupDialog_PageSettings_SetWithDocument_GetReturnsExpected(PageSettings value)
        {
            using var dialog = new PageSetupDialog
            {
                Document = new PrintDocument(),
                PageSettings = value
            };
            Assert.Same(value, dialog.PageSettings);
            Assert.Null(dialog.Document);

            // Set same.
            dialog.PageSettings = value;
            Assert.Same(value, dialog.PageSettings);
            Assert.Null(dialog.Document);
        }

        public static IEnumerable<object[]> PrinterSettings_Set_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new PrinterSettings() };
        }

        [WinFormsTheory]
        [MemberData(nameof(PrinterSettings_Set_TestData))]
        public void PageSetupDialog_PrinterSettings_Set_GetReturnsExpected(PrinterSettings value)
        {
            using var dialog = new PageSetupDialog
            {
                PrinterSettings = value
            };
            Assert.Same(value, dialog.PrinterSettings);
            Assert.Null(dialog.Document);

            // Set same.
            dialog.PrinterSettings = value;
            Assert.Same(value, dialog.PrinterSettings);
            Assert.Null(dialog.Document);
        }

        [WinFormsTheory]
        [MemberData(nameof(PrinterSettings_Set_TestData))]
        public void PageSetupDialog_PrinterSettings_SetWithDocument_GetReturnsExpected(PrinterSettings value)
        {
            using var dialog = new PageSetupDialog
            {
                Document = new PrintDocument(),
                PrinterSettings = value
            };
            Assert.Same(value, dialog.PrinterSettings);
            Assert.Null(dialog.Document);

            // Set same.
            dialog.PrinterSettings = value;
            Assert.Same(value, dialog.PrinterSettings);
            Assert.Null(dialog.Document);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void PageSetupDialog_ShowHelp_Set_GetReturnsExpected(bool value)
        {
            using var dialog = new PageSetupDialog
            {
                ShowHelp = value
            };
            Assert.Equal(value, dialog.ShowHelp);

            // Set same.
            dialog.ShowHelp = value;
            Assert.Equal(value, dialog.ShowHelp);

            // Set different.
            dialog.ShowHelp = !value;
            Assert.Equal(!value, dialog.ShowHelp);
        }

        [WinFormsTheory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void PageSetupDialog_ShowNetwork_Set_GetReturnsExpected(bool value)
        {
            using var dialog = new PageSetupDialog
            {
                ShowNetwork = value
            };
            Assert.Equal(value, dialog.ShowNetwork);

            // Set same.
            dialog.ShowNetwork = value;
            Assert.Equal(value, dialog.ShowNetwork);

            // Set different.
            dialog.ShowNetwork = !value;
            Assert.Equal(!value, dialog.ShowNetwork);
        }

        [WinFormsFact]
        public void PageSetupDialog_Reset_InvokeDefault_Success()
        {
            using var dialog = new PageSetupDialog();
            dialog.Reset();
            Assert.True(dialog.AllowMargins);
            Assert.True(dialog.AllowOrientation);
            Assert.True(dialog.AllowPaper);
            Assert.True(dialog.AllowPrinter);
            Assert.Null(dialog.Container);
            Assert.Null(dialog.Document);
            Assert.False(dialog.EnableMetric);
            Assert.Equal(new Margins(0, 0, 0, 0), dialog.MinMargins);
            Assert.Same(dialog.MinMargins, dialog.MinMargins);
            Assert.Null(dialog.PageSettings);
            Assert.Null(dialog.PrinterSettings);
            Assert.False(dialog.ShowHelp);
            Assert.True(dialog.ShowNetwork);
            Assert.Null(dialog.Site);
            Assert.Null(dialog.Tag);
        }

        [WinFormsFact]
        public void PageSetupDialog_Reset_InvokeComplex_Success()
        {
            var container = new Container();
            using var dialog = new PageSetupDialog
            {
                AllowMargins = false,
                AllowOrientation = false,
                AllowPaper = false,
                AllowPrinter = false,
                Document = new PrintDocument(),
                MinMargins = new Margins(1, 2, 3, 4),
                ShowHelp = true,
                ShowNetwork = false,
                Tag = "tag"
            };
            container.Add(dialog);

            dialog.Reset();
            Assert.True(dialog.AllowMargins);
            Assert.True(dialog.AllowOrientation);
            Assert.True(dialog.AllowPaper);
            Assert.True(dialog.AllowPrinter);
            Assert.Same(container, dialog.Container);
            Assert.Null(dialog.Document);
            Assert.False(dialog.EnableMetric);
            Assert.Equal(new Margins(0, 0, 0, 0), dialog.MinMargins);
            Assert.Same(dialog.MinMargins, dialog.MinMargins);
            Assert.Null(dialog.PageSettings);
            Assert.Null(dialog.PrinterSettings);
            Assert.False(dialog.ShowHelp);
            Assert.True(dialog.ShowNetwork);
            Assert.NotNull(dialog.Site);
            Assert.Equal("tag", dialog.Tag);
        }

        [WinFormsFact]
        public void PageSetupDialog_ShowDialog_InvokeWithoutDocument_ThrowsArgumentException()
        {
            using var dialog = new PageSetupDialog();
            Assert.Throws<ArgumentException>(null, () => dialog.ShowDialog());
        }
    }
}
