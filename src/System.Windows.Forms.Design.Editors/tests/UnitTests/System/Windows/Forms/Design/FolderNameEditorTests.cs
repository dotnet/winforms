// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Drawing.Design;
using System.Globalization;
using System.Reflection;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Design.Tests
{
    public class FolderNameEditorTests
    {
        [Fact]
        public void GetEditStyle_Invoke_ReturnsModal()
        {
            var editor = new FolderNameEditor();
            Assert.Equal(UITypeEditorEditStyle.Modal, editor.GetEditStyle(null));
        }

        [Fact]
        public void InitializeDialog_Invoke_Nop()
        {
            var editor = new SubFolderNameEditor();
            editor.InitializeDialog();
        }

        public class FolderBrowserTests : FolderNameEditor
        {
            [Fact]
            public void Ctor_Default()
            {
                var browser = new FolderBrowser();
                Assert.Empty(browser.DirectoryPath);
                Assert.Empty(browser.Description);
                Assert.Equal(FolderBrowserStyles.RestrictToFilesystem, browser.Style);
                Assert.Equal(FolderBrowserFolder.Desktop, browser.StartLocation);
            }

            [Theory]
            [CommonMemberData(nameof(CommonTestHelper.GetStringTheoryData))]
            public void Description_Set_GetReturnsExpected(string value)
            {
                var browser = new FolderBrowser
                {
                    Description = value
                };
                Assert.Equal(value ?? string.Empty, browser.Description);

                // Set same.
                browser.Description = value;
                Assert.Equal(value ?? string.Empty, browser.Description);
            }

            [Theory]
            [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(FolderBrowserFolder))]
            [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(FolderBrowserFolder))]
            protected void StartLocation_Set_GetReturnsExpected(FolderBrowserFolder value)
            {
                var browser = new FolderBrowser
                {
                    StartLocation = value
                };
                Assert.Equal(value, browser.StartLocation);

                // Set same.
                browser.StartLocation = value;
                Assert.Equal(value, browser.StartLocation);
            }

            [Theory]
            [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(FolderBrowserStyles))]
            [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(FolderBrowserStyles))]
            protected void Style_Set_GetReturnsExpected(FolderBrowserStyles value)
            {
                var browser = new FolderBrowser
                {
                    Style = value
                };
                Assert.Equal(value, browser.Style);

                // Set same.
                browser.Style = value;
                Assert.Equal(value, browser.Style);
            }
        }

        private class SubFolderNameEditor : FolderNameEditor
        {
            public void InitializeDialog() => base.InitializeDialog(null);
        }
    }
}
