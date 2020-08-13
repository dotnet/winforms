 // Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Drawing;
using Xunit;

namespace System.Windows.Forms.VisualStyles.Tests
{
    // NB: doesn't require thread affinity
    public class VisualStyleInformationTests : IClassFixture<ThreadExceptionFixture>
    {
        [Fact]
        public void VisualStyleInformation_Author_Get_ReturnsExpected()
        {
            string author = VisualStyleInformation.Author;
            Assert.NotNull(author);
            Assert.Equal(author, VisualStyleInformation.Author);
            Assert.DoesNotContain('\0', author);
        }

        [Fact]
        public void VisualStyleInformation_ColorScheme_Get_ReturnsExpected()
        {
            string scheme = VisualStyleInformation.ColorScheme;
            Assert.NotNull(scheme);
            Assert.Equal(scheme, VisualStyleInformation.ColorScheme);
            Assert.DoesNotContain('\0', scheme);
        }

        [Fact]
        public void VisualStyleInformation_ColorScheme_Get_ReturnsDifferent()
        {
            string scheme = VisualStyleInformation.ColorScheme;
            if (scheme != string.Empty)
            {
                Assert.NotEqual(scheme, VisualStyleInformation.Size);
                Assert.NotEqual(scheme, VisualStyleInformation.Version);
            }

            Assert.DoesNotContain('\0', scheme);
        }

        [Fact]
        public void VisualStyleInformation_Company_Get_ReturnsExpected()
        {
            string company = VisualStyleInformation.Company;
            Assert.NotNull(company);
            Assert.Equal(company, VisualStyleInformation.Company);
            Assert.DoesNotContain('\0', company);
        }

        [Fact]
        public void VisualStyleInformation_ControlHighlightHot_Get_ReturnsExpected()
        {
            Color color = VisualStyleInformation.ControlHighlightHot;
            Assert.NotEqual(Color.Empty, color);
            Assert.Equal(color, VisualStyleInformation.ControlHighlightHot);
        }

        [Fact]
        public void VisualStyleInformation_Copyright_Get_ReturnsExpected()
        {
            string copyright = VisualStyleInformation.Copyright;
            Assert.NotNull(copyright);
            Assert.Equal(copyright, VisualStyleInformation.Copyright);
            Assert.DoesNotContain('\0', copyright);
        }

        [Fact]
        public void VisualStyleInformation_Description_Get_ReturnsExpected()
        {
            string description = VisualStyleInformation.Description;
            Assert.NotNull(description);
            Assert.Equal(description, VisualStyleInformation.Description);
            Assert.DoesNotContain('\0', description);
        }

        [Fact]
        public void VisualStyleInformation_DisplayName_Get_ReturnsExpected()
        {
            string displayName = VisualStyleInformation.DisplayName;
            Assert.NotNull(displayName);
            Assert.Equal(displayName, VisualStyleInformation.DisplayName);
            Assert.DoesNotContain('\0', displayName);
        }

        [Fact]
        public void VisualStyleInformation_IsEnabledByUser_Get_ReturnsExpected()
        {
            bool enabled = VisualStyleInformation.IsEnabledByUser;
            Assert.Equal(enabled, VisualStyleInformation.IsEnabledByUser);
        }

        [Fact]
        public void VisualStyleInformation_IsSupportedByOS_Get_ReturnsExpected()
        {
            bool supported = VisualStyleInformation.IsSupportedByOS;
            Assert.True(supported);
            Assert.Equal(supported, VisualStyleInformation.IsSupportedByOS);
        }

        [Fact]
        public void VisualStyleInformation_MinimumColorDepth_Get_ReturnsExpected()
        {
            int depth = VisualStyleInformation.MinimumColorDepth;
            Assert.True(depth >= 0);
            Assert.Equal(depth, VisualStyleInformation.MinimumColorDepth);
        }

        [Fact]
        public void VisualStyleInformation_Size_Get_ReturnsExpected()
        {
            string size = VisualStyleInformation.Size;
            Assert.NotNull(size);
            Assert.Equal(size, VisualStyleInformation.Size);
            Assert.DoesNotContain('\0', size);
        }

        [Fact]
        public void VisualStyleInformation_Size_Get_ReturnsDifferent()
        {
            string size = VisualStyleInformation.Size;
            if (size != string.Empty)
            {
                Assert.NotEqual(size, VisualStyleInformation.ColorScheme);
                Assert.NotEqual(size, VisualStyleInformation.Version);
            }

            Assert.DoesNotContain('\0', size);
        }

        [Fact]
        public void VisualStyleInformation_SupportsFlatMenus_Get_ReturnsExpected()
        {
            bool supported = VisualStyleInformation.SupportsFlatMenus;
            Assert.Equal(supported, VisualStyleInformation.SupportsFlatMenus);
        }

        [Fact]
        public void VisualStyleInformation_TextControlBorder_Get_ReturnsExpected()
        {
            Color color = VisualStyleInformation.TextControlBorder;
            Assert.NotEqual(Color.Empty, color);
            Assert.Equal(color, VisualStyleInformation.TextControlBorder);
        }

        [Fact]
        public void VisualStyleInformation_Url_Get_ReturnsExpected()
        {
            string url = VisualStyleInformation.Url;
            Assert.NotNull(url);
            Assert.Equal(url, VisualStyleInformation.Url);
            Assert.DoesNotContain('\0', url);
        }

        [Fact]
        public void VisualStyleInformation_Version_Get_ReturnsExpected()
        {
            string version = VisualStyleInformation.Version;
            Assert.NotNull(version);
            Assert.Equal(version, VisualStyleInformation.Version);
            Assert.DoesNotContain('\0', version);
        }

        [Fact]
        public void VisualStyleInformation_Version_Get_ReturnsDifferent()
        {
            string version = VisualStyleInformation.Version;
            if (version != string.Empty)
            {
                Assert.NotEqual(version, VisualStyleInformation.ColorScheme);
                Assert.NotEqual(version, VisualStyleInformation.Size);
            }

            Assert.DoesNotContain('\0', version);
        }
    }
}
