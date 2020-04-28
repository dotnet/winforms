// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;

namespace System.Windows.Forms.Tests
{
    // NB: doesn't require thread affinity
    public class LayoutSettingsTests : IClassFixture<ThreadExceptionFixture>
    {
        [Fact]
        public void LayoutSettings_Ctor_Default()
        {
            var settings = new SubLayoutSettings();
            Assert.Null(settings.LayoutEngine);
        }

        private class SubLayoutSettings : LayoutSettings
        {
            public SubLayoutSettings() : base() { }
        }
    }
}
