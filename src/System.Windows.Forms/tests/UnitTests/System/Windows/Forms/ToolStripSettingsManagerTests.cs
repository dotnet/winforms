// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Configuration;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ToolStripSettingsManagerTests : IClassFixture<ThreadExceptionFixture>
    {
        [WinFormsFact]
        public void ToolStripSettingsManager_Save_Load_RoundTripExpected()
        {
            using var mainForm = new Form();

            using var toolStrip = new ToolStrip();
            toolStrip.Name = "Child";
            toolStrip.Size = new Drawing.Size(10, 10);
            toolStrip.Visible = false;
            mainForm.Controls.Add(toolStrip);

            var toolStripSettingsManager = new ToolStripSettingsManager(mainForm, "MainForm");

            // Work around for issue https://github.com/dotnet/winforms/issues/5836.
            // Cleaning user.config file if exists that may have written by previously ran tests.
            var configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);
            if (File.Exists(configuration.FilePath))
            {
                File.Delete(configuration.FilePath);
            }

            toolStripSettingsManager.Save();

            toolStrip.Size = new Drawing.Size(5, 5);
            toolStrip.Visible = true;

            toolStripSettingsManager.Load();

            Assert.Equal(new Drawing.Size(10, 10), toolStrip.Size);
            Assert.False(toolStrip.Visible);

            // Cleaning the user.config file created by this test.
            configuration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.PerUserRoamingAndLocal);
            if (File.Exists(configuration.FilePath))
            {
                File.Delete(configuration.FilePath);
            }
        }
    }
}
