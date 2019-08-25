// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using WinForms.Common.Tests;
using Moq;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ToolTipTests
    {
        [Fact]
        public void ToolTip_Ctor_Default()
        {
            var toolTip = new SubToolTip();
            Assert.True(toolTip.Active);
            Assert.Equal(500, toolTip.AutomaticDelay);
            Assert.Equal(5000, toolTip.AutoPopDelay);
            Assert.Equal(SystemColors.Info, toolTip.BackColor);
            Assert.True(toolTip.CanRaiseEvents);
            Assert.Null(toolTip.Container);
            Assert.False(toolTip.DesignMode);
            Assert.NotNull(toolTip.Events);
            Assert.Same(toolTip.Events, toolTip.Events);
            Assert.Equal(SystemColors.InfoText, toolTip.ForeColor);
            Assert.Equal(500, toolTip.InitialDelay);
            Assert.False(toolTip.IsBalloon);
            Assert.False(toolTip.OwnerDraw);
            Assert.Equal(100, toolTip.ReshowDelay);
            Assert.False(toolTip.ShowAlways);
            Assert.Null(toolTip.Site);
            Assert.False(toolTip.StripAmpersands);
            Assert.Null(toolTip.Tag);
            Assert.Equal(ToolTipIcon.None, toolTip.ToolTipIcon);
            Assert.Empty(toolTip.ToolTipTitle);
            Assert.True(toolTip.UseAnimation);
            Assert.True(toolTip.UseFading);
        }

        [Fact]
        public void Ctor_IContainer_TestData()
        {
            var container = new Container();
            var toolTip = new SubToolTip(container);
            Assert.True(toolTip.Active);
            Assert.Equal(500, toolTip.AutomaticDelay);
            Assert.Equal(5000, toolTip.AutoPopDelay);
            Assert.Equal(SystemColors.Info, toolTip.BackColor);
            Assert.True(toolTip.CanRaiseEvents);
            Assert.Same(container, toolTip.Container);
            Assert.False(toolTip.DesignMode);
            Assert.NotNull(toolTip.Events);
            Assert.Same(toolTip.Events, toolTip.Events);
            Assert.Equal(SystemColors.InfoText, toolTip.ForeColor);
            Assert.Equal(500, toolTip.InitialDelay);
            Assert.False(toolTip.IsBalloon);
            Assert.False(toolTip.OwnerDraw);
            Assert.Equal(100, toolTip.ReshowDelay);
            Assert.False(toolTip.ShowAlways);
            Assert.NotNull(toolTip.Site);
            Assert.False(toolTip.StripAmpersands);
            Assert.Null(toolTip.Tag);
            Assert.Equal(ToolTipIcon.None, toolTip.ToolTipIcon);
            Assert.Empty(toolTip.ToolTipTitle);
            Assert.True(toolTip.UseAnimation);
            Assert.True(toolTip.UseFading);
        }

        [Fact]
        public void Ctor_NullCont_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("cont", () => new ToolTip(null));
        }

        [Fact]
        public void CreateParams_GetDefault_ReturnsExpected()
        {
            var toolTip = new SubToolTip();
            CreateParams createParams = toolTip.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Equal("tooltips_class32", createParams.ClassName);
            Assert.Equal(0, createParams.ClassStyle);
            Assert.Equal(0, createParams.ExStyle);
            Assert.Equal(0, createParams.Height);
            Assert.Equal(IntPtr.Zero, createParams.Parent);
            Assert.Null(createParams.Param);
            Assert.Equal(0x2, createParams.Style);
            Assert.Equal(0, createParams.Width);
            Assert.Equal(0, createParams.X);
            Assert.Equal(0, createParams.Y);
            Assert.NotSame(createParams, toolTip.CreateParams);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ToolTip_Active_Set_GetReturnsExpected(bool value)
        {
            var toolTip = new ToolTip
            {
                Active = value
            };
            Assert.Equal(value, toolTip.Active);
            
            // Set same
            toolTip.Active = value;
            Assert.Equal(value, toolTip.Active);
            
            // Set different
            toolTip.Active = !value;
            Assert.Equal(!value, toolTip.Active);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ToolTip_Active_SetDesignMode_GetReturnsExpected(bool value)
        {
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.DesignMode)
                .Returns(true);
            var toolTip = new ToolTip
            {
                Site = mockSite.Object,
                Active = value
            };
            Assert.Equal(value, toolTip.Active);
            
            // Set same
            toolTip.Active = value;
            Assert.Equal(value, toolTip.Active);
            
            // Set different
            toolTip.Active = !value;
            Assert.Equal(!value, toolTip.Active);
        }

        [Theory]
        [InlineData(0, 0, 0)]
        [InlineData(1, 10, 0)]
        [InlineData(2, 20, 0)]
        [InlineData(100, 1000, 20)]
        [InlineData(500, 5000, 100)]
        [InlineData(5000, 50000, 1000)]
        public void ToolTip_AutomaticDelay_Set_GetReturnsExpected(int value, int expectedAutoPopDelay, int expectedReshowDelay)
        {
            var toolTip = new ToolTip
            {
                AutomaticDelay = value
            };
            Assert.Equal(value, toolTip.AutomaticDelay);
            Assert.Equal(expectedAutoPopDelay, toolTip.AutoPopDelay);
            Assert.Equal(value, toolTip.InitialDelay);
            Assert.Equal(expectedReshowDelay, toolTip.ReshowDelay);
            
            // Set same
            toolTip.AutomaticDelay = value;
            Assert.Equal(value, toolTip.AutomaticDelay);
            Assert.Equal(expectedAutoPopDelay, toolTip.AutoPopDelay);
            Assert.Equal(value, toolTip.InitialDelay);
            Assert.Equal(expectedReshowDelay, toolTip.ReshowDelay);
        }

        [Fact]
        public void ToolTip_AutomaticDelay_ShouldSerialize_ReturnsExpected()
        {
            var toolTip = new ToolTip();
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ToolTip))[nameof(ToolTip.AutomaticDelay)];
            Assert.False(property.ShouldSerializeValue(toolTip));

            toolTip.AutomaticDelay = toolTip.AutomaticDelay;
            Assert.False(property.ShouldSerializeValue(toolTip));

            toolTip.AutomaticDelay = 0;
            Assert.True(property.ShouldSerializeValue(toolTip));
        }

        [Fact]
        public void ToolTip_AutomaticDelay_SetNegativeValue_ThrowsArgumentOutOfRangeException()
        {
            var toolTip = new ToolTip();
            Assert.Throws<ArgumentOutOfRangeException>("value", () => toolTip.AutomaticDelay = -1);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(100)]
        [InlineData(500)]
        [InlineData(5000)]
        public void ToolTip_AutoPopDelay_Set_GetReturnsExpected(int value)
        {
            var toolTip = new ToolTip
            {
                AutoPopDelay = value
            };
            Assert.Equal(500, toolTip.AutomaticDelay);
            Assert.Equal(value, toolTip.AutoPopDelay);
            Assert.Equal(500, toolTip.InitialDelay);
            Assert.Equal(100, toolTip.ReshowDelay);
            
            // Set same
            toolTip.AutoPopDelay = value;
            Assert.Equal(500, toolTip.AutomaticDelay);
            Assert.Equal(value, toolTip.AutoPopDelay);
            Assert.Equal(500, toolTip.InitialDelay);
            Assert.Equal(100, toolTip.ReshowDelay);
        }

        [Fact]
        public void ToolTip_AutoPopDelay_ShouldSerialize_ReturnsExpected()
        {
            var toolTip = new ToolTip();
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ToolTip))[nameof(ToolTip.AutoPopDelay)];
            Assert.False(property.ShouldSerializeValue(toolTip));

            toolTip.AutoPopDelay = toolTip.AutoPopDelay;
            Assert.True(property.ShouldSerializeValue(toolTip));

            toolTip.AutoPopDelay = 0;
            Assert.True(property.ShouldSerializeValue(toolTip));
        }

        [Fact]
        public void ToolTip_AutoPopDelay_SetNegativeValue_ThrowsArgumentOutOfRangeException()
        {
            var toolTip = new ToolTip();
            Assert.Throws<ArgumentOutOfRangeException>("value", () => toolTip.AutoPopDelay = -1);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetColorWithEmptyTheoryData))]
        public void ToolTip_BackColor_Set_GetReturnsExpected(Color value)
        {
            var toolTip = new ToolTip
            {
                BackColor = value
            };
            Assert.Equal(value, toolTip.BackColor);

            // Set same.
            toolTip.BackColor = value;
            Assert.Equal(value, toolTip.BackColor);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetColorTheoryData))]
        public void ToolTip_ForeColor_Set_GetReturnsExpected(Color value)
        {
            var toolTip = new ToolTip
            {
                ForeColor = value
            };
            Assert.Equal(value, toolTip.ForeColor);

            // Set same.
            toolTip.ForeColor = value;
            Assert.Equal(value, toolTip.ForeColor);
        }
        
        [Fact]
        public void ToolTip_ForeColor_SetEmpty_ThrowsArgumentException()
        {
            var toolTip = new ToolTip();
            Assert.Throws<ArgumentException>("value", () => toolTip.ForeColor = Color.Empty);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(100)]
        [InlineData(500)]
        [InlineData(5000)]
        public void ToolTip_InitialDelay_Set_GetReturnsExpected(int value)
        {
            var toolTip = new ToolTip
            {
                InitialDelay = value
            };
            Assert.Equal(500, toolTip.AutomaticDelay);
            Assert.Equal(5000, toolTip.AutoPopDelay);
            Assert.Equal(value, toolTip.InitialDelay);
            Assert.Equal(100, toolTip.ReshowDelay);
            
            // Set same
            toolTip.InitialDelay = value;
            Assert.Equal(500, toolTip.AutomaticDelay);
            Assert.Equal(5000, toolTip.AutoPopDelay);
            Assert.Equal(value, toolTip.InitialDelay);
            Assert.Equal(100, toolTip.ReshowDelay);
        }

        [Fact]
        public void ToolTip_InitialDelay_ShouldSerialize_ReturnsExpected()
        {
            var toolTip = new ToolTip();
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ToolTip))[nameof(ToolTip.InitialDelay)];
            Assert.False(property.ShouldSerializeValue(toolTip));

            toolTip.InitialDelay = toolTip.InitialDelay;
            Assert.True(property.ShouldSerializeValue(toolTip));

            toolTip.InitialDelay = 0;
            Assert.True(property.ShouldSerializeValue(toolTip));
        }

        [Fact]
        public void ToolTip_InitialDelay_SetNegativeValue_ThrowsArgumentOutOfRangeException()
        {
            var toolTip = new ToolTip();
            Assert.Throws<ArgumentOutOfRangeException>("value", () => toolTip.InitialDelay = -1);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ToolTip_IsBalloon_Set_GetReturnsExpected(bool value)
        {
            var toolTip = new ToolTip
            {
                IsBalloon = value
            };
            Assert.Equal(value, toolTip.IsBalloon);
            
            // Set same
            toolTip.IsBalloon = value;
            Assert.Equal(value, toolTip.IsBalloon);
            
            // Set different
            toolTip.IsBalloon = !value;
            Assert.Equal(!value, toolTip.IsBalloon);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ToolTip_OwnerDraw_Set_GetReturnsExpected(bool value)
        {
            var toolTip = new ToolTip
            {
                OwnerDraw = value
            };
            Assert.Equal(value, toolTip.OwnerDraw);
            
            // Set same
            toolTip.OwnerDraw = value;
            Assert.Equal(value, toolTip.OwnerDraw);
            
            // Set different
            toolTip.OwnerDraw = !value;
            Assert.Equal(!value, toolTip.OwnerDraw);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(100)]
        [InlineData(500)]
        [InlineData(5000)]
        public void ToolTip_ReshowDelay_Set_GetReturnsExpected(int value)
        {
            var toolTip = new ToolTip
            {
                ReshowDelay = value
            };
            Assert.Equal(500, toolTip.AutomaticDelay);
            Assert.Equal(5000, toolTip.AutoPopDelay);
            Assert.Equal(500, toolTip.InitialDelay);
            Assert.Equal(value, toolTip.ReshowDelay);
            
            // Set same
            toolTip.ReshowDelay = value;
            Assert.Equal(500, toolTip.AutomaticDelay);
            Assert.Equal(5000, toolTip.AutoPopDelay);
            Assert.Equal(500, toolTip.InitialDelay);
            Assert.Equal(value, toolTip.ReshowDelay);
        }

        [Fact]
        public void ToolTip_ReshowDelay_ShouldSerialize_ReturnsExpected()
        {
            var toolTip = new ToolTip();
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ToolTip))[nameof(ToolTip.ReshowDelay)];
            Assert.False(property.ShouldSerializeValue(toolTip));

            toolTip.ReshowDelay = toolTip.ReshowDelay;
            Assert.True(property.ShouldSerializeValue(toolTip));

            toolTip.ReshowDelay = 0;
            Assert.True(property.ShouldSerializeValue(toolTip));
        }

        [Fact]
        public void ToolTip_ReshowDelay_SetNegativeValue_ThrowsArgumentOutOfRangeException()
        {
            var toolTip = new ToolTip();
            Assert.Throws<ArgumentOutOfRangeException>("value", () => toolTip.ReshowDelay = -1);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ToolTip_ShowAlways_Set_GetReturnsExpected(bool value)
        {
            var toolTip = new ToolTip
            {
                ShowAlways = value
            };
            Assert.Equal(value, toolTip.ShowAlways);
            
            // Set same
            toolTip.ShowAlways = value;
            Assert.Equal(value, toolTip.ShowAlways);
            
            // Set different
            toolTip.ShowAlways = !value;
            Assert.Equal(!value, toolTip.ShowAlways);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ToolTip_StripAmpersands_Set_GetReturnsExpected(bool value)
        {
            var toolTip = new ToolTip
            {
                StripAmpersands = value
            };
            Assert.Equal(value, toolTip.StripAmpersands);
            
            // Set same
            toolTip.StripAmpersands = value;
            Assert.Equal(value, toolTip.StripAmpersands);
            
            // Set different
            toolTip.StripAmpersands = !value;
            Assert.Equal(!value, toolTip.StripAmpersands);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void ToolTip_Tag_Set_GetReturnsExpected(object value)
        {
            var toolTip = new ToolTip
            {
                Tag = value
            };
            Assert.Same(value, toolTip.Tag);
            
            // Set same
            toolTip.Tag = value;
            Assert.Same(value, toolTip.Tag);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(ToolTipIcon))]
        public void ToolTip_ToolTipIcon_Set_GetReturnsExpected(ToolTipIcon value)
        {
            var toolTip = new ToolTip
            {
                ToolTipIcon = value
            };
            Assert.Equal(value, toolTip.ToolTipIcon);
            
            // Set same
            toolTip.ToolTipIcon = value;
            Assert.Equal(value, toolTip.ToolTipIcon);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(ToolTipIcon))]
        public void ToolTip_ToolTipIcon_SetInvalidValue_ThrowsInvalidEnumArgumentException(ToolTipIcon value)
        {
            var toolTip = new ToolTip();
            Assert.Throws<InvalidEnumArgumentException>("value", () => toolTip.ToolTipIcon = value);
        }
        
        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void ToolTip_ToolTipTitle_Set_GetReturnsExpected(string value, string expected)
        {
            var toolTip = new ToolTip
            {
                ToolTipTitle = value
            };
            Assert.Equal(expected, toolTip.ToolTipTitle);
            
            // Set same
            toolTip.ToolTipTitle = value;
            Assert.Equal(expected, toolTip.ToolTipTitle);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ToolTip_UseAnimation_Set_GetReturnsExpected(bool value)
        {
            var toolTip = new ToolTip
            {
                UseAnimation = value
            };
            Assert.Equal(value, toolTip.UseAnimation);
            
            // Set same
            toolTip.UseAnimation = value;
            Assert.Equal(value, toolTip.UseAnimation);
            
            // Set different
            toolTip.UseAnimation = !value;
            Assert.Equal(!value, toolTip.UseAnimation);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ToolTip_UseFading_Set_GetReturnsExpected(bool value)
        {
            var toolTip = new ToolTip
            {
                UseFading = value
            };
            Assert.Equal(value, toolTip.UseFading);
            
            // Set same
            toolTip.UseFading = value;
            Assert.Equal(value, toolTip.UseFading);
            
            // Set different
            toolTip.UseFading = !value;
            Assert.Equal(!value, toolTip.UseFading);
        }

        public static IEnumerable<object[]> CanExtend_TestData()
        {
            yield return new object[] { null, false };
            yield return new object[] { new object(), false };
            yield return new object[] { new ToolTip(), false };
            yield return new object[] { new Control(), true };
        }

        [Theory]
        [MemberData(nameof(CanExtend_TestData))]
        public void ToolTip_CanExtend_Invoke_ReurnsExpected(object target, bool expected)
        {
            var toolTip = new ToolTip();
            Assert.Equal(expected, toolTip.CanExtend(target));
        }

        public static IEnumerable<object[]> GetToolTip_NoSuchControl_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new Control() };
        }

        [Theory]
        [MemberData(nameof(GetToolTip_NoSuchControl_TestData))]
        public void ToolTip_GetToolTip_NoSuchControl_ReturnsEmpty(Control control)
        {
            var toolTip = new ToolTip();
            Assert.Empty(toolTip.GetToolTip(control));
        }

        [Fact]
        public void ToolTip_RemoveAll_InvokeWithTools_GetToolTipReturnsEmpty()
        {
            var control = new Control();
            var toolTip = new ToolTip();
            toolTip.SetToolTip(control, "caption");
            toolTip.RemoveAll();
            Assert.Empty(toolTip.GetToolTip(control));

            toolTip.RemoveAll();
            Assert.Empty(toolTip.GetToolTip(control));
        }

        [Fact]
        public void ToolTip_RemoveAll_InvokeWithoutTools_Nop()
        {
            var toolTip = new ToolTip();
            toolTip.RemoveAll();
            toolTip.RemoveAll();
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void ToolTip_SetToolTip_Invoke_GetToolTipReturnsExpected(string caption, string expected)
        {
            var toolTip = new ToolTip();
            var control = new Control();
            toolTip.SetToolTip(control, caption);
            Assert.Equal(expected, toolTip.GetToolTip(control));

            // Set same.
            toolTip.SetToolTip(control, caption);
            Assert.Equal(expected, toolTip.GetToolTip(control));
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void ToolTip_SetToolTip_InvokeDesignMode_GetToolTipReturnsExpected(string caption, string expected)
        {
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.DesignMode)
                .Returns(true);
            var toolTip = new ToolTip
            {
                Site = mockSite.Object
            };
            var control = new Control();
            toolTip.SetToolTip(control, caption);
            Assert.Equal(expected, toolTip.GetToolTip(control));

            // Set same.
            toolTip.SetToolTip(control, caption);
            Assert.Equal(expected, toolTip.GetToolTip(control));
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void ToolTip_SetToolTip_NullControl_ThrowsArgumentNullException(string caption)
        {
            var toolTip = new ToolTip();
            Assert.Throws<ArgumentNullException>("control", () => toolTip.SetToolTip(null, caption));
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void ToolTip_Show_InvokeStringIWin32WindowControlWindow_Nop(string text)
        {
            var toolTip = new ToolTip();
            toolTip.Show(text, new Control());
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void ToolTip_Show_InvokeStringIWin32WindowNonControlWindow_Nop(string text)
        {
            var toolTip = new ToolTip();
            var mockWindow = new Mock<IWin32Window>(MockBehavior.Strict);
            toolTip.Show(text, mockWindow.Object);
        }

        public static IEnumerable<object[]> Show_StringIWin32WindowInt_TestData()
        {
            foreach (int duration in new int[] { 0, 10 })
            {
                yield return new object[] { null, duration };
                yield return new object[] { string.Empty, duration };
                yield return new object[] { "text", duration };
            }
        }

        [Theory]
        [MemberData(nameof(Show_StringIWin32WindowInt_TestData))]
        public void ToolTip_Show_InvokeStringIWin32WindowIntControlWindow_Nop(string text, int duration)
        {
            var toolTip = new ToolTip();
            toolTip.Show(text, new Control(), duration);
        }

        [Theory]
        [MemberData(nameof(Show_StringIWin32WindowInt_TestData))]
        public void ToolTip_Show_InvokeStringIWin32WindowIntNonControlWindow_Nop(string text, int duration)
        {
            var toolTip = new ToolTip();
            var mockWindow = new Mock<IWin32Window>(MockBehavior.Strict);
            toolTip.Show(text, mockWindow.Object, duration);
        }

        [Fact]
        public void ToolTip_Show_NullWindow_ThrowsArgumentNullException()
        {
            var toolTip = new ToolTip();
            Assert.Throws<ArgumentNullException>("window", () => toolTip.Show("text", null));
            Assert.Throws<ArgumentNullException>("window", () => toolTip.Show("text", null, 1));
        }

        [Fact]
        public void ToolTip_Show_NegativeDuration_ThrowsArgumentOutOfRangeException()
        {
            var toolTip = new ToolTip();
            var mockWindow = new Mock<IWin32Window>(MockBehavior.Strict);
            Assert.Throws<ArgumentOutOfRangeException>("duration", () => toolTip.Show("text", mockWindow.Object, -1));
        }

        [Fact]
        public void ToolTip_ToString_Invoke_ReturnsExpected()
        {
            var toolTip = new ToolTip();
            Assert.Equal("System.Windows.Forms.ToolTip InitialDelay: 500, ShowAlways: False", toolTip.ToString());
        }

        private class SubToolTip : ToolTip
        {
            public SubToolTip() : base()
            {
            }

            public SubToolTip(IContainer cont) : base(cont)
            {
            }

            public new bool CanRaiseEvents => base.CanRaiseEvents;

            public new CreateParams CreateParams => base.CreateParams;

            public new bool DesignMode => base.DesignMode;

            public new EventHandlerList Events => base.Events;
        }
    }
}
