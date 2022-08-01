// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using Microsoft.Win32;
using Xunit;
using Xunit.Abstractions;

namespace System.Windows.Forms.UITests
{
    public class ProfessionalColorTableTests : ControlTestBase
    {
        public ProfessionalColorTableTests(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
        }

        [WinFormsTheory(Skip = "Deadlocks under x86, see: https://github.com/dotnet/winforms/issues/3254")]
        [ActiveIssue("https://github.com/dotnet/winforms/issues/3254")]
        [InlineData(UserPreferenceCategory.Color)]
        [InlineData(UserPreferenceCategory.Accessibility)]
        [InlineData(UserPreferenceCategory.Desktop)]
        [InlineData(UserPreferenceCategory.Icon)]
        [InlineData(UserPreferenceCategory.Mouse)]
        [InlineData(UserPreferenceCategory.Keyboard)]
        [InlineData(UserPreferenceCategory.Menu)]
        [InlineData(UserPreferenceCategory.Power)]
        [InlineData(UserPreferenceCategory.Screensaver)]
        [InlineData(UserPreferenceCategory.Window)]
        public void ProfessionalColorTable_ChangeUserPreferences_GetColor_ReturnsExpected(UserPreferenceCategory category)
        {
            // Simulate a SystemEvents.UserPreferenceChanged event.
            var table = new ProfessionalColorTable();
            Color color = table.ButtonSelectedHighlight;
            SystemEventsHelper.SendMessageOnUserPreferenceChanged(category);
            Assert.Equal(color, table.ButtonSelectedHighlight);
        }
    }
}
