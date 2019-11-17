// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using Moq;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class ErrorProviderTests
    {
        [Fact]
        public void ErrorProvider_Ctor_Default()
        {
            var provider = new SubErrorProvider();
            Assert.Equal(250, provider.BlinkRate);
            Assert.Equal(ErrorBlinkStyle.BlinkIfDifferentError, provider.BlinkStyle);
            Assert.True(provider.CanRaiseEvents);
            Assert.Null(provider.Container);
            Assert.Null(provider.ContainerControl);
            Assert.Null(provider.DataMember);
            Assert.Null(provider.DataSource);
            Assert.False(provider.DesignMode);
            Assert.NotNull(provider.Events);
            Assert.Same(provider.Events, provider.Events);
            Assert.NotNull(provider.Icon);
            Assert.Same(provider.Icon, provider.Icon);
            Assert.Null(provider.Site);
            Assert.Null(provider.Tag);
        }

        [Fact]
        public void ErrorProvider_Ctor_ContainerControl()
        {
            var parentControl = new ContainerControl();
            var provider = new SubErrorProvider(parentControl);
            Assert.Equal(250, provider.BlinkRate);
            Assert.Equal(ErrorBlinkStyle.BlinkIfDifferentError, provider.BlinkStyle);
            Assert.True(provider.CanRaiseEvents);
            Assert.Null(provider.Container);
            Assert.Same(parentControl, provider.ContainerControl);
            Assert.Null(provider.DataMember);
            Assert.Null(provider.DataSource);
            Assert.False(provider.DesignMode);
            Assert.NotNull(provider.Events);
            Assert.Same(provider.Events, provider.Events);
            Assert.NotNull(provider.Icon);
            Assert.Same(provider.Icon, provider.Icon);
            Assert.Null(provider.Site);
            Assert.Null(provider.Tag);
        }

        [Fact]
        public void ErrorProvider_Ctor_NullParentControl_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("parentControl", () => new ErrorProvider((ContainerControl)null));
        }

        [Fact]
        public void ErrorProvider_Ctor_IContainer()
        {
            var container = new Container();
            var provider = new SubErrorProvider(container);
            Assert.Equal(250, provider.BlinkRate);
            Assert.Equal(ErrorBlinkStyle.BlinkIfDifferentError, provider.BlinkStyle);
            Assert.True(provider.CanRaiseEvents);
            Assert.Same(container, provider.Container);
            Assert.Null(provider.ContainerControl);
            Assert.Null(provider.DataMember);
            Assert.Null(provider.DataSource);
            Assert.False(provider.DesignMode);
            Assert.NotNull(provider.Events);
            Assert.Same(provider.Events, provider.Events);
            Assert.NotNull(provider.Icon);
            Assert.Same(provider.Icon, provider.Icon);
            Assert.NotNull(provider.Site);
            Assert.Null(provider.Tag);
        }

        [Fact]
        public void ErrorProvider_Ctor_NullContainer_ThrowsArgumentNullException()
        {
            Assert.Throws<ArgumentNullException>("container", () => new ErrorProvider((IContainer)null));
        }

        [Theory]
        [InlineData(0, ErrorBlinkStyle.NeverBlink)]
        [InlineData(1, ErrorBlinkStyle.BlinkIfDifferentError)]
        [InlineData(250, ErrorBlinkStyle.BlinkIfDifferentError)]
        public void ErrorProvider_BlinkRate_Set_GetReturnsExpected(int value, ErrorBlinkStyle expectedBlinkStyle)
        {
            var provider = new ErrorProvider
            {
                BlinkRate = value
            };
            Assert.Equal(value, provider.BlinkRate);
            Assert.Equal(expectedBlinkStyle, provider.BlinkStyle);

            // Set same.
            provider.BlinkRate = value;
            Assert.Equal(value, provider.BlinkRate);
            Assert.Equal(expectedBlinkStyle, provider.BlinkStyle);

            // Set blink style.
            provider.BlinkStyle = ErrorBlinkStyle.BlinkIfDifferentError;
            Assert.Equal(value, provider.BlinkRate);
            Assert.Equal(expectedBlinkStyle, provider.BlinkStyle);
        }

        [Fact]
        public void ErrorProvider_BlinkRate_SetNegative_ThrowsArgumentOutOfRangeException()
        {
            var provider = new ErrorProvider();
            Assert.Throws<ArgumentOutOfRangeException>("value", () => provider.BlinkRate = -1);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(ErrorBlinkStyle))]
        public void ErrorProvider_BlinkStyle_Set_GetReturnsExpected(ErrorBlinkStyle value)
        {
            var provider = new ErrorProvider
            {
                BlinkStyle = value
            };
            Assert.Equal(value, provider.BlinkStyle);
            Assert.Equal(250, provider.BlinkRate);
            
            // Set same.
            provider.BlinkStyle = value;
            Assert.Equal(value, provider.BlinkStyle);
            Assert.Equal(250, provider.BlinkRate);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(ErrorBlinkStyle))]
        public void ErrorProvider_BlinkStyle_SetAlreadyBlink_GetReturnsExpected(ErrorBlinkStyle value)
        {
            var provider = new ErrorProvider
            {
                BlinkStyle = ErrorBlinkStyle.AlwaysBlink
            };

            provider.BlinkStyle = value;
            Assert.Equal(value, provider.BlinkStyle);
            Assert.Equal(250, provider.BlinkRate);
            
            // Set same.
            provider.BlinkStyle = value;
            Assert.Equal(value, provider.BlinkStyle);
            Assert.Equal(250, provider.BlinkRate);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(ErrorBlinkStyle))]
        public void ErrorProvider_BlinkStyle_SetWithZeroBlinkRate_GetReturnsExpected(ErrorBlinkStyle value)
        {
            var provider = new ErrorProvider
            {
                BlinkRate = 0,
                BlinkStyle = value
            };
            Assert.Equal(ErrorBlinkStyle.NeverBlink, provider.BlinkStyle);
            Assert.Equal(0, provider.BlinkRate);
            
            // Set same.
            provider.BlinkStyle = value;
            Assert.Equal(ErrorBlinkStyle.NeverBlink, provider.BlinkStyle);
            Assert.Equal(0, provider.BlinkRate);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(ErrorBlinkStyle))]
        public void ErrorProvider_BlinkStyle_SetInvalidValue_ThrowsInvalidEnumArgumentException(ErrorBlinkStyle value)
        {
            var provider = new ErrorProvider();
            Assert.Throws<InvalidEnumArgumentException>("value", () => provider.BlinkStyle = value);
        }

        public static IEnumerable<object[]> ContainerControl_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new ContainerControl() };
            yield return new object[] { new SubContainerControl() };
        }

        [Theory]
        [MemberData(nameof(ContainerControl_TestData))]
        public void ErrorProvider_ContainerControl_Set_GetReturnsExpected(ContainerControl value)
        {
            var provider = new ErrorProvider
            {
                ContainerControl = value
            };
            Assert.Same(value, provider.ContainerControl);

            // Set same.
            provider.ContainerControl = value;
            Assert.Same(value, provider.ContainerControl);
        }

        [Theory]
        [MemberData(nameof(ContainerControl_TestData))]
        public void ErrorProvider_ContainerControl_SetWithNonNullOldValue_GetReturnsExpected(ContainerControl value)
        {
            var provider = new ErrorProvider
            {
                ContainerControl = new ContainerControl()
            };

            provider.ContainerControl = value;
            Assert.Same(value, provider.ContainerControl);

            // Set same.
            provider.ContainerControl = value;
            Assert.Same(value, provider.ContainerControl);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void ErrorProvider_DataMember_Set_GetReturnsExpected(string value, string expected)
        {
            var provider = new ErrorProvider
            {
                DataMember = value
            };
            Assert.Equal(expected, provider.DataMember);
            
            // Set same.
            provider.DataMember = value;
            Assert.Equal(expected, provider.DataMember);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void ErrorProvider_DataMember_SetWithNonNullOldValue_GetReturnsExpected(string value, string expected)
        {
            var provider = new ErrorProvider
            {
                DataMember = "OldMember"
            };

            provider.DataMember = value;
            Assert.Equal(expected, provider.DataMember);
            
            // Set same.
            provider.DataMember = value;
            Assert.Equal(expected, provider.DataMember);
        }

        public static IEnumerable<object[]> DataMember_SetWithContainerControl_TestData()
        {
            foreach (string value in new string[] { null, string.Empty, "dataMember" })
            {
                yield return new object[] { null, value, value ?? string.Empty };
                yield return new object[] { new ContainerControl(), value, value ?? string.Empty };
                yield return new object[] { new SubContainerControl(), value, value ?? string.Empty };
            }
        }

        [Theory]
        [MemberData(nameof(DataMember_SetWithContainerControl_TestData))]
        public void ErrorProvider_DataMember_SetWithContainerControl_GetReturnsExpected(ContainerControl containerControl, string value, string expected)
        {
            var provider = new ErrorProvider
            {
                ContainerControl = containerControl,
                DataMember = value
            };
            Assert.Equal(expected, provider.DataMember);

            // Set same.
            provider.DataMember = value;
            Assert.Equal(expected, provider.DataMember);
        }

        public static IEnumerable<object[]> DataMember_SetWithValidDataMemberWithContainerControl_TestData()
        {
            foreach (string dataMember in new string[] { nameof(DataClass.Value), nameof(DataClass.ListValue) })
            {
                yield return new object[] { null, dataMember };
                yield return new object[] { new ContainerControl(), dataMember };
                yield return new object[] { new SubContainerControl(), dataMember };
            }
        }

        [Theory]
        [MemberData(nameof(DataMember_SetWithValidDataMemberWithContainerControl_TestData))]
        public void ErrorProvider_DataMember_SetWithValidDataSourceWithContainerControl_ReturnsExpected(ContainerControl containerControl, string dataMember)
        {
            var value = new DataClass();
            var provider = new ErrorProvider
            {
                ContainerControl = containerControl,
                DataSource = value,
                DataMember = dataMember
            };
            Assert.Same(value, provider.DataSource);
            Assert.Same(dataMember, provider.DataMember);
        }

        [Fact]
        public void ErrorProvider_DataMember_ShouldSerializeValue_ReturnsExpected()
        {
            var provider = new ErrorProvider();
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ErrorProvider))[nameof(ErrorProvider.DataMember)];
            Assert.False(property.ShouldSerializeValue(provider));

            provider.DataMember = "dataMember";
            Assert.True(property.ShouldSerializeValue(provider));
        }

        [Fact]
        public void ErrorProvider_DataMember_CanResetValue_ReturnsExpected()
        {
            var provider = new ErrorProvider();
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ErrorProvider))[nameof(ErrorProvider.DataMember)];
            Assert.False(property.CanResetValue(provider));

            provider.DataMember = "dataMember";
            Assert.True(property.CanResetValue(provider));

            property.ResetValue(provider);
            Assert.True(property.CanResetValue(provider));
            Assert.Empty(provider.DataMember);
        }

        public static IEnumerable<object[]> NoBindingContextContainerControl_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new SubContainerControl() };
        }

        [Theory]
        [MemberData(nameof(NoBindingContextContainerControl_TestData))]
        public void ErrorProvider_DataMember_SetWithInvalidDataSourceWithContainerControl_ReturnsExpected(ContainerControl containerControl)
        {
            var value = new DataClass();
            var provider = new ErrorProvider
            {
                ContainerControl = containerControl,
                DataSource = value,
                DataMember = "NoSuchValue"
            };
            Assert.Same(value, provider.DataSource);
            Assert.Equal("NoSuchValue", provider.DataMember);
        }

        [Fact]
        public void ErrorProvider_DataMember_SetWithInvalidDataSourceWithContainerControl_ResetsDataMember()
        {
            var containerControl = new ContainerControl();
            var value = new DataClass();
            var provider = new ErrorProvider
            {
                ContainerControl = containerControl,
                DataSource = value
            };
            Assert.Throws<ArgumentException>(null, () => provider.DataMember = "NoSuchValue");
            Assert.Same(value, provider.DataSource);
            Assert.Equal("NoSuchValue", provider.DataMember);
        }

        public static IEnumerable<object[]> DataSource_Set_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new object() };
            yield return new object[] { new DataClass() };
        }

        [Theory]
        [MemberData(nameof(DataSource_Set_TestData))]
        public void ErrorProvider_DataSource_SetWithNullDataMember_GetReturnsExpected(object value)
        {
            var provider = new ErrorProvider
            {
                DataSource = value
            };
            Assert.Same(value, provider.DataSource);
            Assert.Null(provider.DataMember);

            // Set same.
            provider.DataSource = value;
            Assert.Same(value, provider.DataSource);
            Assert.Null(provider.DataMember);
        }

        [Theory]
        [MemberData(nameof(DataSource_Set_TestData))]
        public void ErrorProvider_DataSource_SetWithEmptyDataMember_GetReturnsExpected(object value)
        {
            var provider = new ErrorProvider
            {
                DataMember = string.Empty,
                DataSource = value
            };
            Assert.Same(value, provider.DataSource);
            Assert.Empty(provider.DataMember);

            // Set same.
            provider.DataSource = value;
            Assert.Same(value, provider.DataSource);
            Assert.Empty(provider.DataMember);
        }

        [Theory]
        [MemberData(nameof(DataSource_Set_TestData))]
        public void ErrorProvider_DataSource_SetWithNonNullOldValue_GetReturnsExpected(object value)
        {
            var provider = new ErrorProvider
            {
                DataSource = new object()
            };

            provider.DataSource = value;
            Assert.Same(value, provider.DataSource);

            // Set same.
            provider.DataSource = value;
            Assert.Same(value, provider.DataSource);
        }

        public static IEnumerable<object[]> DataSource_SetWithContainerControl_TestData()
        {
            foreach (object value in new object[] { null, new object(), new DataClass() })
            {
                yield return new object[] { null, value };
                yield return new object[] { new ContainerControl(), value };
                yield return new object[] { new SubContainerControl(), value };
            }
        }

        [Theory]
        [MemberData(nameof(DataSource_SetWithContainerControl_TestData))]
        public void ErrorProvider_DataSource_SetWithContainerControl_GetReturnsExpected(ContainerControl containerControl, object value)
        {
            var provider = new ErrorProvider
            {
                ContainerControl = containerControl,
                DataSource = value
            };
            Assert.Same(value, provider.DataSource);

            // Set same.
            provider.DataSource = value;
            Assert.Same(value, provider.DataSource);
        }

        public static IEnumerable<object[]> DataSource_SetWithValidDataMemberWithContainerControl_TestData()
        {
            foreach (string dataMember in new string[] { nameof(DataClass.Value), nameof(DataClass.ListValue) })
            {
                yield return new object[] { null, dataMember };
                yield return new object[] { new ContainerControl(), dataMember };
                yield return new object[] { new SubContainerControl(), dataMember };
            }
        }

        [Theory]
        [MemberData(nameof(DataSource_SetWithValidDataMemberWithContainerControl_TestData))]
        public void ErrorProvider_DataSource_SetWithValidDataMemberWithContainerControl_ReturnsExpected(ContainerControl containerControl, string dataMember)
        {
            var value = new DataClass();
            var provider = new ErrorProvider
            {
                ContainerControl = containerControl,
                DataMember = dataMember,
                DataSource = value
            };
            Assert.Same(value, provider.DataSource);
            Assert.Same(dataMember, provider.DataMember);
        }

        [Theory]
        [MemberData(nameof(NoBindingContextContainerControl_TestData))]
        public void ErrorProvider_DataSource_SetWithInvalidDataMemberWithContainerControl_ReturnsExpected(ContainerControl containerControl)
        {
            var value = new DataClass();
            var provider = new ErrorProvider
            {
                ContainerControl = containerControl,
                DataMember = "NoSuchValue",
                DataSource = value
            };
            Assert.Same(value, provider.DataSource);
            Assert.Equal("NoSuchValue", provider.DataMember);
        }

        [Fact]
        public void ErrorProvider_DataSource_SetWithInvalidDataMemberWithContainerControl_ResetsDataMember()
        {
            var containerControl = new ContainerControl();
            var value = new DataClass();
            var provider = new ErrorProvider
            {
                ContainerControl = containerControl,
                DataMember = "NoSuchValue",
                DataSource = value
            };
            Assert.Same(value, provider.DataSource);
            Assert.Empty(provider.DataMember);
        }

        [Fact]
        public void ErrorProvider_DataSource_ShouldSerializeValue_ReturnsExpected()
        {
            var provider = new ErrorProvider();
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ErrorProvider))[nameof(ErrorProvider.DataSource)];
            Assert.False(property.ShouldSerializeValue(provider));

            provider.DataSource = new object();
            Assert.True(property.ShouldSerializeValue(provider));
        }

        [Fact]
        public void ErrorProvider_DataSource_CanResetValue_ReturnsExpected()
        {
            var provider = new ErrorProvider();
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ErrorProvider))[nameof(ErrorProvider.DataSource)];
            Assert.False(property.CanResetValue(provider));

            provider.DataSource = new object();
            Assert.True(property.CanResetValue(provider));

            property.ResetValue(provider);
            Assert.False(property.CanResetValue(provider));
            Assert.Null(provider.DataSource);
        }

        public static IEnumerable<object[]> Icon_Set_TestData()
        {
            yield return new object[] { Icon.FromHandle(new Bitmap(10, 10).GetHicon()) };
        }

        [Theory]
        [MemberData(nameof(Icon_Set_TestData))]
        public void ErrorProvider_Icon_Set_GetReturnsExpected(Icon value)
        {
            var provider = new ErrorProvider
            {
                Icon = value
            };
            Assert.Same(value, provider.Icon);

            // Set same.
            provider.Icon = value;
            Assert.Same(value, provider.Icon);
        }

        [Fact]
        public void ErrorProvider_Icon_ShouldSerializeValue_ReturnsExpected()
        {
            var provider = new ErrorProvider();
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ErrorProvider))[nameof(ErrorProvider.Icon)];
            Assert.False(property.ShouldSerializeValue(provider));

            provider.Icon = Icon.FromHandle(new Bitmap(10, 10).GetHicon());
            Assert.True(property.ShouldSerializeValue(provider));
        }

        [Fact]
        public void ErrorProvider_Icon_CanResetValue_ReturnsExpected()
        {
            var provider = new ErrorProvider();
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(ErrorProvider))[nameof(ErrorProvider.Icon)];
            Assert.False(property.CanResetValue(provider));

            provider.Icon = Icon.FromHandle(new Bitmap(10, 10).GetHicon());
            Assert.True(property.CanResetValue(provider));

            property.ResetValue(provider);
            Assert.False(property.CanResetValue(provider));
            Assert.Null(provider.DataSource);
        }

        [Fact]
        public void ErrorProvider_Icon_Null_ThrowsArgumentNullException()
        {
            var provider = new ErrorProvider();
            Assert.Throws<ArgumentNullException>("value", () => provider.Icon = null);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ErrorProvider_RightToLeft_Set_GetReturnsExpected(bool value)
        {
            var provider = new ErrorProvider
            {
                RightToLeft = value
            };
            Assert.Equal(value, provider.RightToLeft);
            
            // Set same.
            provider.RightToLeft = value;
            Assert.Equal(value, provider.RightToLeft);
            
            // Set different.
            provider.RightToLeft = !value;
            Assert.Equal(!value, provider.RightToLeft);
        }

        [Fact]
        public void ErrorProvider_RightToLeft_SetWithHandler_CallsRightToLeftChanged()
        {
            var provider = new ErrorProvider
            {
                RightToLeft = true
            };
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(provider, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            provider.RightToLeftChanged += handler;
        
            // Set different.
            provider.RightToLeft = false;
            Assert.False(provider.RightToLeft);
            Assert.Equal(1, callCount);
        
            // Set same.
            provider.RightToLeft = false;
            Assert.False(provider.RightToLeft);
            Assert.Equal(1, callCount);
        
            // Set different.
            provider.RightToLeft = true;
            Assert.True(provider.RightToLeft);
            Assert.Equal(2, callCount);
        
            // Remove handler.
            provider.RightToLeftChanged -= handler;
            provider.RightToLeft = false;
            Assert.False(provider.RightToLeft);
            Assert.Equal(2, callCount);
        }

        public static IEnumerable<object[]> Site_Set_TestData()
        {
            yield return new object[] { null };

            var mockNullHostSite = new Mock<ISite>(MockBehavior.Strict);
            mockNullHostSite
                .Setup(s => s.GetService(typeof(IDesignerHost)))
                .Returns(null);
            yield return new object[] { mockNullHostSite.Object };

            var mockInvalidHostSite = new Mock<ISite>(MockBehavior.Strict);
            mockInvalidHostSite
                .Setup(s => s.GetService(typeof(IDesignerHost)))
                .Returns(new object());
            yield return new object[] { mockInvalidHostSite.Object };
        }

        [Theory]
        [MemberData(nameof(Site_Set_TestData))]
        public void ErrorProvider_Site_Set_GetReturnsExpected(ISite value)
        {
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.GetService(typeof(IDesignerHost)))
                .Returns(null);
            var provider = new ErrorProvider
            {
                Site = value
            };
            Assert.Same(value, provider.Site);
            
            // Set same.
            provider.Site = value;
            Assert.Same(value, provider.Site);
        }

        [Theory]
        [MemberData(nameof(Site_Set_TestData))]
        public void ErrorProvider_Site_SetWithNonNullOldValue_GetReturnsExpected(ISite value)
        {
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.GetService(typeof(IDesignerHost)))
                .Returns(null);
            var provider = new ErrorProvider
            {
                Site = mockSite.Object
            };

            provider.Site = value;
            Assert.Same(value, provider.Site);
            
            // Set same.
            provider.Site = value;
            Assert.Same(value, provider.Site);
        }

        public static IEnumerable<object[]> Site_SetWithIDesignerHost_TestData()
        {
            yield return new object[] { null, null };
            yield return new object[] { new Component(), null };

            var containerControl = new ContainerControl();
            yield return new object[] { containerControl, containerControl };
        }

        [Theory]
        [MemberData(nameof(Site_SetWithIDesignerHost_TestData))]
        public void ErrorProvider_Site_SetWithIDesignerHost_SetsContainerControl(IComponent rootComponent, ContainerControl expected)
        {
            var mockDesignerHost = new Mock<IDesignerHost>(MockBehavior.Strict);
            mockDesignerHost
                .Setup(h => h.RootComponent)
                .Returns(rootComponent);
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.GetService(typeof(IDesignerHost)))
                .Returns(mockDesignerHost.Object)
                .Verifiable();
            var provider = new ErrorProvider
            {
                Site = mockSite.Object
            };
            mockSite.Verify(s => s.GetService(typeof(IDesignerHost)), Times.Once());
            mockDesignerHost.Verify(h => h.RootComponent, Times.Once());
            Assert.Same(mockSite.Object, provider.Site);
            Assert.Same(expected, provider.ContainerControl);
            mockSite.Verify(s => s.GetService(typeof(IDesignerHost)), Times.Once());
            mockDesignerHost.Verify(h => h.RootComponent, Times.Once());
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void ErrorProvider_Tag_Set_GetReturnsExpected(object value)
        {
            var provider = new ErrorProvider
            {
                Tag = value
            };
            Assert.Same(value, provider.Tag);
            
            // Set same.
            provider.Tag = value;
            Assert.Same(value, provider.Tag);
        }

        public static IEnumerable<object[]> BindToDataAndErrors_TestData()
        {
            foreach (ContainerControl containerControl in new ContainerControl[] { null, new SubContainerControl() })
            {
                foreach (string dataMember in new string[] { null, string.Empty, "dataMember" })
                {
                    yield return new object[] { containerControl, null, dataMember };
                    yield return new object[] { containerControl, new object(), dataMember };
                    yield return new object[] { containerControl, new DataClass(), dataMember };
                }
            }
        }

        [Theory]
        [MemberData(nameof(BindToDataAndErrors_TestData))]
        public void BindToDataAndErrors_Invoke_SetsDataSourceAndDataMember(ContainerControl containerControl, object newDataSource, string newDataMember)
        {
            var provider = new ErrorProvider
            {
                ContainerControl = containerControl
            };
            provider.BindToDataAndErrors(newDataSource, newDataMember);
            Assert.Same(newDataSource, provider.DataSource);
            Assert.Same(newDataMember, provider.DataMember);

            // Call again.
            provider.BindToDataAndErrors(newDataSource, newDataMember);
            Assert.Same(newDataSource, provider.DataSource);
            Assert.Same(newDataMember, provider.DataMember);
        }
        
        public static IEnumerable<object[]> BindToDataAndErrors_WithBindingContext_TestData()
        {
            foreach (string dataMember in new string[] { null, string.Empty })
            {
                yield return new object[] { null, dataMember };
                yield return new object[] { new object(), dataMember };
                yield return new object[] { new DataClass(), dataMember };
            }

            yield return new object[] { null, nameof(DataClass.Value) };
            yield return new object[] { null, nameof(DataClass.ListValue) };
            yield return new object[] { new DataClass(), nameof(DataClass.Value) };
            yield return new object[] { new DataClass(), nameof(DataClass.ListValue) };
        }

        [Theory]
        [MemberData(nameof(BindToDataAndErrors_WithBindingContext_TestData))]
        public void BindToDataAndErrors_InvokeValidDataMemberWithBindingContext_SetsDataSourceAndDataMember(object newDataSource, string newDataMember)
        {
            var containerControl = new ContainerControl();
            var provider = new ErrorProvider
            {
                ContainerControl = containerControl
            };
            provider.BindToDataAndErrors(newDataSource, newDataMember);
            Assert.Same(newDataSource, provider.DataSource);
            Assert.Same(newDataMember, provider.DataMember);

            // Call again.
            provider.BindToDataAndErrors(newDataSource, newDataMember);
            Assert.Same(newDataSource, provider.DataSource);
            Assert.Same(newDataMember, provider.DataMember);
        }

        [Fact]
        public void BindToDataAndErrors_InvokeInvalidDataMemberWithBindingContext_ThrowsArgumentException()
        {
            var containerControl = new ContainerControl();
            var provider = new ErrorProvider
            {
                ContainerControl = containerControl
            };
            var newDataSource = new DataClass();
            Assert.Throws<ArgumentException>(null, () => provider.BindToDataAndErrors(newDataSource, "NoSuchValue"));
            Assert.Same(newDataSource, provider.DataSource);
            Assert.Equal("NoSuchValue", provider.DataMember);

            // Call again.
            provider.BindToDataAndErrors(newDataSource, "NoSuchValue");
            Assert.Same(newDataSource, provider.DataSource);
            Assert.Equal("NoSuchValue", provider.DataMember);
        }

        public static IEnumerable<object[]> CanExtend_TestData()
        {
            yield return new object[] { null, false };
            yield return new object[] { new object(), false };
            yield return new object[] { new Component(), false };
            yield return new object[] { new Form(), false };
            yield return new object[] { new Control(), true };
        }

        [Theory]
        [MemberData(nameof(CanExtend_TestData))]
        public void ErrorProvider_CanExtend_Invoke_ReturnsExpected(object extendee, bool expected)
        {
            var provider = new ErrorProvider();
            Assert.Equal(expected, provider.CanExtend(extendee));
        }

        [Fact]
        public void ErrorProvider_Clear_InvokeMultipleTimesWithItems_Success()
        {
            var provider = new ErrorProvider();
            var control = new Control();
            provider.SetError(control, "error");
            Assert.Equal("error", provider.GetError(control));

            provider.Clear();
            Assert.Empty(provider.GetError(control));

            provider.Clear();
            Assert.Empty(provider.GetError(control));
        }

        [Fact]
        public void ErrorProvider_Clear_InvokeMultipleTimesWithoutItems_Nop()
        {
            var provider = new ErrorProvider();
            provider.Clear();
            provider.Clear();
        }

        [Fact]
        public void ErrorProvider_Dispose_InvokeWithItems_Clears()
        {
            var provider = new ErrorProvider();
            var control = new Control();
            provider.SetError(control, "error");
            Assert.Equal("error", provider.GetError(control));

            provider.Dispose();
            Assert.NotNull(provider.Icon);
            Assert.Empty(provider.GetError(control));

            provider.Dispose();
            Assert.NotNull(provider.Icon);
            Assert.Empty(provider.GetError(control));
        }

        [Fact]
        public void ErrorProvider_Dispose_InvokeMultipleTimesWithoutItems_Nop()
        {
            var provider = new ErrorProvider();
            provider.Dispose();
            Assert.NotNull(provider.Icon);

            provider.Dispose();
            Assert.NotNull(provider.Icon);
        }

        [Theory]
        [InlineData(true, "")]
        [InlineData(false, "error")]
        public void ErrorProvider_Dispose_InvokeBoolWithItems_ClearsIfDisposing(bool disposing, string expectedError)
        {
            var provider = new SubErrorProvider();
            var control = new Control();
            provider.SetError(control, "error");
            Assert.Equal("error", provider.GetError(control));

            provider.Dispose(disposing);
            Assert.NotNull(provider.Icon);
            Assert.Equal(expectedError, provider.GetError(control));

            provider.Dispose(disposing);
            Assert.NotNull(provider.Icon);
            Assert.Equal(expectedError, provider.GetError(control));
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ErrorProvider_Dispose_InvokeBoolMultipleTimesDefault_Nop(bool disposing)
        {
            var provider = new SubErrorProvider();
            provider.Dispose(disposing);
            Assert.NotNull(provider.Icon);

            provider.Dispose(disposing);
            Assert.NotNull(provider.Icon);
        }

        [Fact]
        public void ErrorProvider_GetError_InvokeWithoutError_ReturnsEmpty()
        {
            var provider = new ErrorProvider();
            var control = new Control();
            Assert.Empty(provider.GetError(control));

            // Call again.
            Assert.Empty(provider.GetError(control));
        }

        [Fact]
        public void ErrorProvider_GetError_NullControl_ThrowsArgumentNullException()
        {
            var provider = new ErrorProvider();
            Assert.Throws<ArgumentNullException>("control", () => provider.GetError(null));
        }

        [Fact]
        public void ErrorProvider_GetIconAlignment_InvokeWithoutError_ReturnsMiddleRight()
        {
            var provider = new ErrorProvider();
            var control = new Control();
            Assert.Equal(ErrorIconAlignment.MiddleRight, provider.GetIconAlignment(control));

            // Call again.
            Assert.Equal(ErrorIconAlignment.MiddleRight, provider.GetIconAlignment(control));
        }

        [Fact]
        public void ErrorProvider_GetIconAlignment_NullControl_ThrowsArgumentNullException()
        {
            var provider = new ErrorProvider();
            Assert.Throws<ArgumentNullException>("control", () => provider.GetIconAlignment(null));
        }

        [Fact]
        public void ErrorProvider_GetIconPadding_InvokeWithoutError_ReturnsZero()
        {
            var provider = new ErrorProvider();
            var control = new Control();
            Assert.Equal(0, provider.GetIconPadding(control));

            // Call again.
            Assert.Equal(0, provider.GetIconPadding(control));
        }

        [Fact]
        public void ErrorProvider_GetIconPadding_NullControl_ThrowsArgumentNullException()
        {
            var provider = new ErrorProvider();
            Assert.Throws<ArgumentNullException>("control", () => provider.GetIconPadding(null));
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void ErrorProvider_OnRightToLeftChanged_Invoke_CallsRightToLeftChanged(EventArgs eventArgs)
        {
            var provider = new SubErrorProvider();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(provider, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
        
            // Call with handler.
            provider.RightToLeftChanged += handler;
            provider.OnRightToLeftChanged(eventArgs);
            Assert.Equal(1, callCount);
        
           // Remove handler.
           provider.RightToLeftChanged -= handler;
           provider.OnRightToLeftChanged(eventArgs);
           Assert.Equal(1, callCount);
        }

        public static IEnumerable<object[]> SetError_TestData()
        {
            foreach (ErrorBlinkStyle blinkStyle in Enum.GetValues(typeof(ErrorBlinkStyle)))
            {
                yield return new object[] { blinkStyle, null, string.Empty };
                yield return new object[] { blinkStyle, string.Empty, string.Empty };
                yield return new object[] { blinkStyle, "value", "value" };
            }
        }

        [Theory]
        [MemberData(nameof(SetError_TestData))]
        public void ErrorProvider_SetError_Invoke_GetErrorReturnsExpected(ErrorBlinkStyle blinkStyle, string value, string expected)
        {
            var provider = new ErrorProvider
            {
                BlinkStyle = blinkStyle
            };
            var control = new Control();

            provider.SetError(control, value);
            Assert.Equal(expected, provider.GetError(control));

            // Call again.
            provider.SetError(control, value);
            Assert.Equal(expected, provider.GetError(control));

            // Set empty.
            provider.SetError(control, string.Empty);
            Assert.Empty(provider.GetError(control));
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void ErrorProvider_SetError_NullControl_ThrowsArgumentNullException(string value)
        {
            var provider = new ErrorProvider();
            Assert.Throws<ArgumentNullException>("control", () => provider.SetError(null, value));
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(ErrorIconAlignment))]
        public void ErrorProvider_SetIconAlignment_Invoke_GetIconAlignmentReturnsExpected(ErrorIconAlignment value)
        {
            var provider = new ErrorProvider();
            var control = new Control();

            provider.SetIconAlignment(control, value);
            Assert.Equal(value, provider.GetIconAlignment(control));

            // Call again.
            provider.SetIconAlignment(control, value);
            Assert.Equal(value, provider.GetIconAlignment(control));
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(ErrorIconAlignment))]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(ErrorIconAlignment))]
        public void ErrorProvider_SetIconAlignment_NullControl_ThrowsArgumentNullException(ErrorIconAlignment value)
        {
            var provider = new ErrorProvider();
            Assert.Throws<ArgumentNullException>("control", () => provider.SetIconAlignment(null, value));
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(ErrorIconAlignment))]
        public void ErrorProvider_SetIconAlignment_InvalidValue_ThrowsInvalidEnumArgumentException(ErrorIconAlignment value)
        {
            var provider = new ErrorProvider();
            var control = new Control();
            Assert.Throws<InvalidEnumArgumentException>("value", () => provider.SetIconAlignment(control, value));
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetIntTheoryData))]
        public void ErrorProvider_SetIconPadding_Invoke_GetIconPaddingReturnsExpected(int value)
        {
            var provider = new ErrorProvider();
            var control = new Control();

            provider.SetIconPadding(control, value);
            Assert.Equal(value, provider.GetIconPadding(control));

            // Call again.
            provider.SetIconPadding(control, value);
            Assert.Equal(value, provider.GetIconPadding(control));
        }

        [Fact]
        public void ErrorProvider_SetIconPadding_NullControl_ThrowsArgumentNullException()
        {
            var provider = new ErrorProvider();
            Assert.Throws<ArgumentNullException>("control", () => provider.SetIconPadding(null, 0));
        }

        public static IEnumerable<object[]> CallEvents_TestData()
        {
            foreach (ErrorBlinkStyle blinkStyle in Enum.GetValues(typeof(ErrorBlinkStyle)))
            {
                foreach (string error in new string[] { null, string.Empty, "error" })
                {
                    yield return new object[] { blinkStyle, new SubControl(), error, error ?? string.Empty };
                    yield return new object[] { blinkStyle, new SubControl { Visible = false }, error, error ?? string.Empty };
                    yield return new object[] { blinkStyle, new SubControl { Parent = new Control() }, error, error ?? string.Empty };
                }
            }
        }

        [Theory]
        [MemberData(nameof(CallEvents_TestData))]
        public void ErrorProvider_Items_CallEvents_Success(ErrorBlinkStyle blinkStyle, SubControl control, string error, string expected)
        {
            bool originalVisible = control.Visible;
            var originalParent = new Control();
            var newParent = new Control();

            using (var provider = new ErrorProvider
            {
                BlinkStyle = blinkStyle
            })
            {
                provider.SetError(control, error);
                Assert.Equal(expected, provider.GetError(control));

                // Call event properties - without handle.
                control.Location = new Point(1, 2);
                Assert.Equal(new Point(1, 2), control.Location);
                
                control.Size = new Size(2, 3);
                Assert.Equal(new Size(2, 3), control.Size);

                control.Visible = !originalVisible;
                Assert.Equal(!originalVisible, control.Visible);

                control.Visible = originalVisible;
                Assert.Equal(originalVisible, control.Visible);

                control.Parent = newParent;
                Assert.Same(newParent, control.Parent);

                control.Parent = null;
                Assert.Null(control.Parent);

                control.Parent = originalParent;
                Assert.Same(originalParent, control.Parent);

                // Call event methods - without handle.
                control.OnHandleCreated(EventArgs.Empty);
                control.OnHandleDestroyed(EventArgs.Empty);
                control.OnLocationChanged(EventArgs.Empty);
                control.OnSizeChanged(EventArgs.Empty);
                control.OnVisibleChanged(EventArgs.Empty);
                control.OnParentChanged(EventArgs.Empty);

                // Call event properties - with handle.
                Assert.NotEqual(IntPtr.Zero, control.Handle);

                control.Location = new Point(2, 3);
                Assert.Equal(new Point(2, 3), control.Location);
                
                control.Size = new Size(4, 5);
                Assert.Equal(new Size(4, 5), control.Size);

                control.Visible = !originalVisible;
                Assert.Equal(!originalVisible, control.Visible);

                control.Visible = originalVisible;
                Assert.Equal(originalVisible, control.Visible);

                control.Parent = newParent;
                Assert.Same(newParent, control.Parent);

                control.Parent = null;
                Assert.Null(control.Parent);

                control.Parent = originalParent;
                Assert.Same(originalParent, control.Parent);

                // Call event methods - with handle.
                control.OnHandleCreated(EventArgs.Empty);
                control.OnHandleDestroyed(EventArgs.Empty);
                control.OnLocationChanged(EventArgs.Empty);
                control.OnSizeChanged(EventArgs.Empty);
                control.OnVisibleChanged(EventArgs.Empty);
                control.OnParentChanged(EventArgs.Empty);

                control.Dispose();
            }
        }

        private class SubErrorProvider : ErrorProvider
        {
            public SubErrorProvider() : base()
            {
            }

            public SubErrorProvider(ContainerControl parentControl) : base(parentControl)
            {
            }

            public SubErrorProvider(IContainer container) : base(container)
            {
            }

            public new bool CanRaiseEvents => base.CanRaiseEvents;

            public new bool DesignMode => base.DesignMode;

            public new EventHandlerList Events => base.Events;

            public new void Dispose(bool disposing) => base.Dispose(disposing);

            public new void OnRightToLeftChanged(EventArgs e) => base.OnRightToLeftChanged(e);
        }

        private class DataClass
        {
            public int Value { get; set; }

            public List<int> ListValue { get; set; }
        }

        private class SubContainerControl : ContainerControl
        {
            public override BindingContext BindingContext => null;
        }

        public class SubControl : Control
        {
            public new void OnHandleCreated(EventArgs e) => base.OnHandleCreated(e);

            public new void OnHandleDestroyed(EventArgs e) => base.OnHandleDestroyed(e);

            public new void OnLocationChanged(EventArgs e) => base.OnLocationChanged(e);

            public new void OnParentChanged(EventArgs e) => base.OnParentChanged(e);

            public new void OnSizeChanged(EventArgs e) => base.OnSizeChanged(e);

            public new void OnVisibleChanged(EventArgs e) => base.OnVisibleChanged(e);
        }
    }
}
