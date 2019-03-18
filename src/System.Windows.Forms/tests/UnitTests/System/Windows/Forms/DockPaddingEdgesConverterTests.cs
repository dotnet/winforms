// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class DockPaddingEdgesConverterTests
    {
        [Fact]
        public void DockPaddingEdgesConverter_GetProperties_Invoke_ReturnsExpected()
        {
            var converter = new ScrollableControl.DockPaddingEdgesConverter();
            PropertyDescriptorCollection properties = converter.GetProperties(null);
            Assert.Equal(5, properties.Count);
            Assert.Equal(nameof(ScrollableControl.DockPaddingEdges.All), properties[0].Name);
            Assert.Equal(nameof(ScrollableControl.DockPaddingEdges.Left), properties[1].Name);
            Assert.Equal(nameof(ScrollableControl.DockPaddingEdges.Top), properties[2].Name);
            Assert.Equal(nameof(ScrollableControl.DockPaddingEdges.Right), properties[3].Name);
            Assert.Equal(nameof(ScrollableControl.DockPaddingEdges.Bottom), properties[4].Name);
        }

        [Fact]
        public void DockPaddingEdgesConverter_GetPropertiesSupported_Invoke_ReturnsTrue()
        {
            var converter = new ScrollableControl.DockPaddingEdgesConverter();
            Assert.True(converter.GetPropertiesSupported());
        }
    }
}
