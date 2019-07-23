
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using Moq;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class FlowLayoutSettingsTests
    {
        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(FlowDirection))]
        public void FlowLayoutSettings_FlowDirection_Set_GetReturnsExpected(FlowDirection value)
        {
            var toolStrip = new ToolStrip
            {
                LayoutStyle = ToolStripLayoutStyle.Flow
            };
            FlowLayoutSettings settings = Assert.IsType<FlowLayoutSettings>(toolStrip.LayoutSettings);

            settings.FlowDirection = value;
            Assert.Equal(value, settings.FlowDirection);

            // Set same
            settings.FlowDirection = value;
            Assert.Equal(value, settings.FlowDirection);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(FlowDirection))]
        public void FlowLayoutSettings_FlowDirection_SetInvalidValue_ThrowsInvalidEnumArgumentException(FlowDirection value)
        {
            var toolStrip = new ToolStrip
            {
                LayoutStyle = ToolStripLayoutStyle.Flow
            };
            FlowLayoutSettings settings = Assert.IsType<FlowLayoutSettings>(toolStrip.LayoutSettings);
            Assert.Throws<InvalidEnumArgumentException>("value", () => settings.FlowDirection = value);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void FlowLayoutSettings_WrapContents_Set_GetReturnsExpected(bool value)
        {
            var toolStrip = new ToolStrip
            {
                LayoutStyle = ToolStripLayoutStyle.Flow
            };
            FlowLayoutSettings settings = Assert.IsType<FlowLayoutSettings>(toolStrip.LayoutSettings);

            settings.WrapContents = value;
            Assert.Equal(value, settings.WrapContents);

            // Set same
            settings.WrapContents = value;
            Assert.Equal(value, settings.WrapContents);

            // Set different
            settings.WrapContents = !value;
            Assert.Equal(!value, settings.WrapContents);
        }

        [Fact]
        public void GetFlowBreak_ValidControl_ReturnsExpected()
        {
            var toolStrip = new ToolStrip
            {
                LayoutStyle = ToolStripLayoutStyle.Flow
            };
            FlowLayoutSettings settings = Assert.IsType<FlowLayoutSettings>(toolStrip.LayoutSettings);
            Assert.False(settings.GetFlowBreak(new Control()));
        }

        [Fact]
        public void GetFlowBreak_NullChild_ThrowsArgumentNullException()
        {
            var toolStrip = new ToolStrip
            {
                LayoutStyle = ToolStripLayoutStyle.Flow
            };
            FlowLayoutSettings settings = Assert.IsType<FlowLayoutSettings>(toolStrip.LayoutSettings);
            Assert.Throws<ArgumentNullException>("child", () => settings.GetFlowBreak(null));
        }

        [Fact]
        public void GetFlowBreak_InvalidChild_ThrowsNotSupportedException()
        {
            var toolStrip = new ToolStrip
            {
                LayoutStyle = ToolStripLayoutStyle.Flow
            };
            FlowLayoutSettings settings = Assert.IsType<FlowLayoutSettings>(toolStrip.LayoutSettings);
            Assert.Throws<NotSupportedException>(() => settings.GetFlowBreak(new object()));
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void SetFlowBreak_Invoke_GetFlowBreakReturnsExpected(bool value)
        {
            var toolStrip = new ToolStrip
            {
                LayoutStyle = ToolStripLayoutStyle.Flow
            };
            FlowLayoutSettings settings = Assert.IsType<FlowLayoutSettings>(toolStrip.LayoutSettings);

            var control = new Control();
            settings.SetFlowBreak(control, value);
            Assert.Equal(value, settings.GetFlowBreak(control));

            // Set same.
            settings.SetFlowBreak(control, value);
            Assert.Equal(value, settings.GetFlowBreak(control));

            // Set different.
            settings.SetFlowBreak(control, !value);
            Assert.Equal(!value, settings.GetFlowBreak(control));
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void SetFlowBreak_NullChild_ThrowsArgumentNullException(bool value)
        {
            var toolStrip = new ToolStrip
            {
                LayoutStyle = ToolStripLayoutStyle.Flow
            };
            FlowLayoutSettings settings = Assert.IsType<FlowLayoutSettings>(toolStrip.LayoutSettings);
            Assert.Throws<ArgumentNullException>("child", () => settings.SetFlowBreak(null, value));
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void SetFlowBreak_InvalidChild_ThrowsNotSupportedException(bool value)
        {
            var toolStrip = new ToolStrip
            {
                LayoutStyle = ToolStripLayoutStyle.Flow
            };
            FlowLayoutSettings settings = Assert.IsType<FlowLayoutSettings>(toolStrip.LayoutSettings);
            Assert.Throws<NotSupportedException>(() => settings.SetFlowBreak(new object(), value));
        }

        private class SubToolStrip : ToolStrip
        {
        }
    }
}
