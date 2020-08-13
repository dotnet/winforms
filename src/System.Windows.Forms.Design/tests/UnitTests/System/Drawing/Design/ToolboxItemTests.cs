// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;
using Moq;
using WinForms.Common.Tests;
using Xunit;

namespace System.Drawing.Design.Tests
{
    public class ToolboxItemTests : IClassFixture<ThreadExceptionFixture>
    {
        [Fact]
        public void ToolboxItem_Ctor_Default()
        {
            var item = new ToolboxItem();
            Assert.Null(item.AssemblyName);
            Assert.Null(item.Bitmap);
            Assert.Null(item.Company);
            Assert.Equal(".NET Component", item.ComponentType);
            Assert.Null(item.DependentAssemblies);
            Assert.Null(item.Description);
            Assert.Empty(item.DisplayName);
            Assert.Empty(item.Filter);
            Assert.Same(item.Filter, item.Filter);
            Assert.False(item.IsTransient);
            Assert.False(item.Locked);
            Assert.Null(item.OriginalBitmap);
            Assert.Empty(item.Properties);
            Assert.Same(item.Properties, item.Properties);
            Assert.False(item.Properties.IsFixedSize);
            Assert.False(item.Properties.IsReadOnly);
            Assert.Empty(item.TypeName);
            Assert.Empty(item.Version);
        }

        public static IEnumerable<object[]> AssemblyName_Set_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new AssemblyName() };
            yield return new object[] { new AssemblyName(typeof(int).Assembly.FullName) };
        }

        [Theory]
        [MemberData(nameof(AssemblyName_Set_TestData))]
        public void ToolboxItem_AssemblyName_Set_GetReturnsExpected(AssemblyName value)
        {
            var item = new ToolboxItem
            {
                AssemblyName = value
            };
            if (value is null)
            {
                Assert.Null(item.AssemblyName);
                Assert.Null(item.Properties["AssemblyName"]);
                Assert.Empty(item.Version);
            }
            else
            {
                Assert.Equal(value.FullName, item.AssemblyName.FullName);
                Assert.NotSame(value, item.AssemblyName);
                Assert.NotSame(item.AssemblyName, item.AssemblyName);
                Assert.Equal(value.FullName, ((AssemblyName)item.Properties["AssemblyName"]).FullName);
                Assert.NotSame(value, item.Properties["AssemblyName"]);
                Assert.NotSame(item.Properties["AssemblyName"], item.Properties["AssemblyName"]);
                Assert.Equal(value.Version?.ToString() ?? string.Empty, item.Version);
            }

            // Set same.
            item.AssemblyName = value;
            if (value is null)
            {
                Assert.Null(item.AssemblyName);
                Assert.Null(item.Properties["AssemblyName"]);
                Assert.Empty(item.Version);
            }
            else
            {
                Assert.Equal(value.FullName, item.AssemblyName.FullName);
                Assert.NotSame(value, item.AssemblyName);
                Assert.NotSame(item.AssemblyName, item.AssemblyName);
                Assert.Equal(value.FullName, ((AssemblyName)item.Properties["AssemblyName"]).FullName);
                Assert.NotSame(value, item.Properties["AssemblyName"]);
                Assert.NotSame(item.Properties["AssemblyName"], item.Properties["AssemblyName"]);
                Assert.Equal(value.Version?.ToString() ?? string.Empty, item.Version);
            }
        }

        [Fact]
        public void ToolboxItem_AssemblyName_SetWithInvalidPropertyType_ThrowsArgumentException()
        {
            var item = new ToolboxItem();
            Assert.Throws<ArgumentException>("value", () => item.Properties.Add("AssemblyName", new object()));
        }

        public static IEnumerable<object[]> Bitmap_Set_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new Bitmap(10, 10) };
        }

        [Theory]
        [MemberData(nameof(Bitmap_Set_TestData))]
        public void ToolboxItem_Bitmap_Set_GetReturnsExpected(Bitmap value)
        {
            var item = new ToolboxItem
            {
                Bitmap = value
            };
            Assert.Same(value, item.Bitmap);
            Assert.Same(value, item.Properties["Bitmap"]);

            // Set same.
            item.Bitmap = value;
            Assert.Same(value, item.Bitmap);
            Assert.Same(value, item.Properties["Bitmap"]);
        }

        [Fact]
        public void ToolboxItem_Bitmap_SetWithInvalidPropertyType_ThrowsArgumentException()
        {
            var item = new ToolboxItem();
            Assert.Throws<ArgumentException>("value", () => item.Properties.Add("Bitmap", new object()));
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void ToolboxItem_Company_Set_GetReturnsExpected(string value, string expected)
        {
            var item = new ToolboxItem
            {
                Company = value
            };
            Assert.Equal(expected, item.Company);
            Assert.Equal(expected, item.Properties["Company"]);

            // Set same.
            item.Company = value;
            Assert.Equal(expected, item.Company);
            Assert.Equal(expected, item.Properties["Company"]);
        }

        [Fact]
        public void ToolboxItem_Company_SetWithInvalidPropertyType_ThrowsArgumentException()
        {
            var item = new ToolboxItem();
            Assert.Throws<ArgumentException>("value", () => item.Properties.Add("Company", new object()));
        }

        public static IEnumerable<object[]> DependentAssemblies_Set_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { Array.Empty<AssemblyName>() };
            yield return new object[] { new AssemblyName[] { null } };
            yield return new object[] { new AssemblyName[] { new AssemblyName() } };
        }

        [Theory]
        [MemberData(nameof(DependentAssemblies_Set_TestData))]
        public void ToolboxItem_DependentAssemblies_Set_GetReturnsExpected(AssemblyName[] value)
        {
            var item = new ToolboxItem
            {
                DependentAssemblies = value
            };
            if (value is null)
            {
                Assert.Null(item.DependentAssemblies);
                Assert.Null(item.Properties["DependentAssemblies"]);
            }
            else
            {
                Assert.Equal(value, item.DependentAssemblies);
                Assert.NotSame(value, item.DependentAssemblies);
                Assert.Equal(value, item.Properties["DependentAssemblies"]);
                Assert.NotSame(value, item.Properties["DependentAssemblies"]);
            }

            // Set same.
            item.DependentAssemblies = value;
            if (value is null)
            {
                Assert.Null(item.DependentAssemblies);
                Assert.Null(item.Properties["DependentAssemblies"]);
            }
            else
            {
                Assert.Equal(value, item.DependentAssemblies);
                Assert.NotSame(value, item.DependentAssemblies);
                Assert.Equal(value, item.Properties["DependentAssemblies"]);
                Assert.NotSame(value, item.Properties["DependentAssemblies"]);
            }
        }

        [Fact]
        public void ToolboxItem_DependentAssemblies_SetWithInvalidPropertyType_GetThrowsArgumentException()
        {
            var item = new ToolboxItem();
            Assert.Throws<ArgumentException>("value", () => item.Properties.Add("DependentAssemblies", new object()));
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void ToolboxItem_Description_Set_GetReturnsExpected(string value, string expected)
        {
            var item = new ToolboxItem
            {
                Description = value
            };
            Assert.Equal(expected, item.Description);
            Assert.Equal(expected, item.Properties["Description"]);

            // Set same.
            item.Description = value;
            Assert.Equal(expected, item.Description);
            Assert.Equal(expected, item.Properties["Description"]);
        }

        [Fact]
        public void ToolboxItem_Description_SetWithInvalidPropertyType_ThrowsArgumentException()
        {
            var item = new ToolboxItem();
            Assert.Throws<ArgumentException>("value", () => item.Properties.Add("Description", new object()));
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void ToolboxItem_DisplayName_Set_GetReturnsExpected(string value, string expected)
        {
            var item = new ToolboxItem
            {
                DisplayName = value
            };
            Assert.Equal(expected, item.DisplayName);
            Assert.Equal(expected, item.Properties["DisplayName"]);

            // Set same.
            item.DisplayName = value;
            Assert.Equal(expected, item.DisplayName);
            Assert.Equal(expected, item.Properties["DisplayName"]);
        }

        [Fact]
        public void ToolboxItem_DisplayName_SetWithInvalidPropertyType_ThrowsArgumentException()
        {
            var item = new ToolboxItem();
            Assert.Throws<ArgumentException>("value", () => item.Properties.Add("DisplayName", new object()));
        }

        public static IEnumerable<object[]> Filter_Set_TestData()
        {
            yield return new object[] { null, Array.Empty<object>() };
            yield return new object[] { Array.Empty<object>(), Array.Empty<object>() };
            yield return new object[] { new object[] { null }, Array.Empty<object>() };
            yield return new object[] { new object[] { new object(), new ToolboxItemFilterAttribute("filterString") }, new object[] { new ToolboxItemFilterAttribute("filterString") } };
        }

        [Theory]
        [MemberData(nameof(Filter_Set_TestData))]
        public void ToolboxItem_Filter_Set_GetReturnsExpected(ICollection value, ICollection expected)
        {
            var item = new ToolboxItem
            {
                Filter = value
            };
            Assert.Equal(expected, item.Filter);
            Assert.Equal(expected, item.Properties["Filter"]);

            // Set same.
            item.Filter = value;
            Assert.Equal(expected, item.Filter);
            Assert.Equal(expected, item.Properties["Filter"]);
        }

        [Fact]
        public void ToolboxItem_Filter_SetWithInvalidPropertyType_GetThrowsArgumentException()
        {
            var item = new ToolboxItem();
            Assert.Throws<ArgumentException>("value", () => item.Properties.Add("Filter", new object()));
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void ToolboxItem_IsTransient_Set_GetReturnsExpected(bool value)
        {
            var item = new ToolboxItem
            {
                IsTransient = value
            };
            Assert.Equal(value, item.IsTransient);
            Assert.Equal(value, item.Properties["IsTransient"]);

            // Set same.
            item.IsTransient = value;
            Assert.Equal(value, item.IsTransient);
            Assert.Equal(value, item.Properties["IsTransient"]);
        }

        [Fact]
        public void ToolboxItem_IsTransient_SetWithNullPropertyType_GetThrowsArgumentNullException()
        {
            var item = new ToolboxItem();
            Assert.Throws<ArgumentNullException>("value", () => item.Properties.Add("IsTransient", null));
        }

        [Fact]
        public void ToolboxItem_IsTransient_SetWithInvalidPropertyType_GetThrowsArgumentException()
        {
            var item = new ToolboxItem();
            Assert.Throws<ArgumentException>("value", () => item.Properties.Add("IsTransient", new object()));
        }

        [Theory]
        [MemberData(nameof(Bitmap_Set_TestData))]
        public void ToolboxItem_OriginalBitmap_Set_GetReturnsExpected(Bitmap value)
        {
            var item = new ToolboxItem
            {
                OriginalBitmap = value
            };
            Assert.Same(value, item.OriginalBitmap);
            Assert.Same(value, item.Properties["OriginalBitmap"]);

            // Set same.
            item.OriginalBitmap = value;
            Assert.Same(value, item.OriginalBitmap);
            Assert.Same(value, item.Properties["OriginalBitmap"]);
        }

        [Fact]
        public void ToolboxItem_OriginalBitmap_SetWithInvalidPropertyType_ThrowsArgumentException()
        {
            var item = new ToolboxItem();
            Assert.Throws<ArgumentException>("value", () => item.Properties.Add("OriginalBitmap", new object()));
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void ToolboxItem_TypeName_Set_GetReturnsExpected(string value, string expected)
        {
            var item = new ToolboxItem
            {
                TypeName = value
            };
            Assert.Equal(expected, item.TypeName);
            Assert.Equal(expected, item.Properties["TypeName"]);

            // Set same.
            item.TypeName = value;
            Assert.Equal(expected, item.TypeName);
            Assert.Equal(expected, item.Properties["TypeName"]);
        }

        [Fact]
        public void ToolboxItem_TypeName_SetWithInvalidPropertyType_ThrowsArgumentException()
        {
            var item = new ToolboxItem();
            Assert.Throws<ArgumentException>("value", () => item.Properties.Add("TypeName", new object()));
        }

        [Fact]
        public void ToolboxItem_CreateComponents_InvokeWithoutHost_ReturnsExpected()
        {
            var item = new ToolboxItem
            {
                AssemblyName = typeof(Component).Assembly.GetName(true),
                TypeName = "System.ComponentModel.Component"
            };
            int creatingCallCount = 0;
            int createdCallCount = 0;
            ToolboxComponentsCreatingEventHandler creatingHandler = (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Null(e.DesignerHost);
                creatingCallCount++;
            };
            ToolboxComponentsCreatedEventHandler createdHandler = (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.IsType<Component>(Assert.Single(e.Components));
                createdCallCount++;
            };
            item.ComponentsCreating += creatingHandler;
            item.ComponentsCreated += createdHandler;

            // With handler.
            Assert.IsType<Component>(Assert.Single(item.CreateComponents()));
            Assert.Equal(1, creatingCallCount);
            Assert.Equal(1, createdCallCount);

            Assert.IsType<Component>(Assert.Single(item.CreateComponents(null)));
            Assert.Equal(2, creatingCallCount);
            Assert.Equal(2, createdCallCount);

            // Remove handler.
            item.ComponentsCreating -= creatingHandler;
            item.ComponentsCreated -= createdHandler;

            Assert.IsType<Component>(Assert.Single(item.CreateComponents(null)));
            Assert.Equal(2, creatingCallCount);
            Assert.Equal(2, createdCallCount);
        }

        public static IEnumerable<object[]> CreateComponents_InvokeWithHostWithNonIComponentInitializerDesigner_TestData()
        {
            var mockDesigner = new Mock<IDesigner>(MockBehavior.Strict);
            mockDesigner.Setup(d => d.Dispose());

            yield return new object[] { new Component(), null };
            yield return new object[] { new Component(), mockDesigner.Object };

            yield return new object[] { null, null };
            yield return new object[] { null, mockDesigner.Object };
        }

        [Theory]
        [MemberData(nameof(CreateComponents_InvokeWithHostWithNonIComponentInitializerDesigner_TestData))]
        public void ToolboxItem_CreateComponents_InvokeWithHostWithNonIComponentInitializerDesigner_ReturnsExpected(Component component, IDesigner designer)
        {
            var item = new ToolboxItem
            {
                TypeName = "typeName"
            };
            var mockDesignerHost = new Mock<IDesignerHost>(MockBehavior.Strict);
            mockDesignerHost
                .Setup(h => h.GetService(typeof(ITypeResolutionService)))
                .Returns(new CustomTypeResolutionService());
            mockDesignerHost
                .Setup(h => h.CreateComponent(typeof(bool)))
                .Returns(component);
            mockDesignerHost
                .Setup(h => h.GetDesigner(component))
                .Returns(designer);

            int creatingCallCount = 0;
            int createdCallCount = 0;
            ToolboxComponentsCreatingEventHandler creatingHandler = (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(mockDesignerHost.Object, e.DesignerHost);
                creatingCallCount++;
            };
            ToolboxComponentsCreatedEventHandler createdHandler = (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(component, Assert.Single(e.Components));
                createdCallCount++;
            };
            item.ComponentsCreating += creatingHandler;
            item.ComponentsCreated += createdHandler;

            // With handler.
            Assert.Same(component, Assert.Single(item.CreateComponents(mockDesignerHost.Object)));
            Assert.Equal(1, creatingCallCount);
            Assert.Equal(1, createdCallCount);

            Assert.Same(component, Assert.Single(item.CreateComponents(mockDesignerHost.Object, null)));
            Assert.Equal(2, creatingCallCount);
            Assert.Equal(2, createdCallCount);

            Assert.Same(component, Assert.Single(item.CreateComponents(mockDesignerHost.Object, new Hashtable())));
            Assert.Equal(3, creatingCallCount);
            Assert.Equal(3, createdCallCount);

            // Remove handler.
            item.ComponentsCreating -= creatingHandler;
            item.ComponentsCreated -= createdHandler;

            Assert.Same(component, Assert.Single(item.CreateComponents(mockDesignerHost.Object)));
            Assert.Equal(3, creatingCallCount);
            Assert.Equal(3, createdCallCount);
        }

        [Fact]
        public void ToolboxItem_CreateComponents_InvokeWithHostWithIComponentInitializerDesigner_ReturnsExpected()
        {
            var component = new Component();
            var item = new ToolboxItem
            {
                TypeName = "typeName"
            };
            var mockDesigner = new Mock<IDesigner>(MockBehavior.Strict);
            mockDesigner.Setup(d => d.Dispose());
            Mock<IComponentInitializer> mockComponentInitializer = mockDesigner.As<IComponentInitializer>();
            mockComponentInitializer
                .Setup(i => i.InitializeNewComponent(null));
            mockComponentInitializer
                .Setup(i => i.InitializeNewComponent(new Hashtable()));

            var mockDesignerHost = new Mock<IDesignerHost>(MockBehavior.Strict);
            mockDesignerHost
                .Setup(h => h.GetService(typeof(ITypeResolutionService)))
                .Returns(new CustomTypeResolutionService());
            mockDesignerHost
                .Setup(h => h.CreateComponent(typeof(bool)))
                .Returns(component);
            mockDesignerHost
                .Setup(h => h.GetDesigner(component))
                .Returns(mockDesigner.Object);

            int creatingCallCount = 0;
            int createdCallCount = 0;
            ToolboxComponentsCreatingEventHandler creatingHandler = (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(mockDesignerHost.Object, e.DesignerHost);
                creatingCallCount++;
            };
            ToolboxComponentsCreatedEventHandler createdHandler = (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(component, Assert.Single(e.Components));
                createdCallCount++;
            };
            item.ComponentsCreating += creatingHandler;
            item.ComponentsCreated += createdHandler;

            // With handler.
            Assert.Same(component, Assert.Single(item.CreateComponents(mockDesignerHost.Object)));
            Assert.Equal(1, creatingCallCount);
            Assert.Equal(1, createdCallCount);

            Assert.Same(component, Assert.Single(item.CreateComponents(mockDesignerHost.Object, null)));
            Assert.Equal(2, creatingCallCount);
            Assert.Equal(2, createdCallCount);

            Assert.Same(component, Assert.Single(item.CreateComponents(mockDesignerHost.Object, new Hashtable())));
            Assert.Equal(3, creatingCallCount);
            Assert.Equal(3, createdCallCount);

            // Remove handler.
            item.ComponentsCreating -= creatingHandler;
            item.ComponentsCreated -= createdHandler;

            Assert.Same(component, Assert.Single(item.CreateComponents(mockDesignerHost.Object)));
            Assert.Equal(3, creatingCallCount);
            Assert.Equal(3, createdCallCount);
        }

        [Fact]
        public void ToolboxItem_CreateComponents_InvokeWithHostWithThrowingIComponentInitializerDesigner_ReturnsExpected()
        {
            var component = new Component();
            var item = new ToolboxItem
            {
                TypeName = "typeName"
            };
            var mockDesigner = new Mock<IDesigner>(MockBehavior.Strict);
            mockDesigner.Setup(d => d.Dispose());
            Mock<IComponentInitializer> mockComponentInitializer = mockDesigner.As<IComponentInitializer>();
            mockComponentInitializer
                .Setup(i => i.InitializeNewComponent(null))
                .Throws(new Exception());
            mockComponentInitializer
                .Setup(i => i.InitializeNewComponent(new Hashtable()))
                .Throws(new Exception());

            var mockDesignerHost = new Mock<IDesignerHost>(MockBehavior.Strict);
            mockDesignerHost
                .Setup(h => h.GetService(typeof(ITypeResolutionService)))
                .Returns(new CustomTypeResolutionService());
            mockDesignerHost
                .Setup(h => h.CreateComponent(typeof(bool)))
                .Returns(component);
            mockDesignerHost
                .Setup(h => h.GetDesigner(component))
                .Returns(mockDesigner.Object);
            mockDesignerHost
                .Setup(h => h.DestroyComponent(component));

            int creatingCallCount = 0;
            int createdCallCount = 0;
            ToolboxComponentsCreatingEventHandler creatingHandler = (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(mockDesignerHost.Object, e.DesignerHost);
                creatingCallCount++;
            };
            ToolboxComponentsCreatedEventHandler createdHandler = (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(component, Assert.Single(e.Components));
                createdCallCount++;
            };
            item.ComponentsCreating += creatingHandler;
            item.ComponentsCreated += createdHandler;

            // With handler.
            Assert.Throws<Exception>(() => item.CreateComponents(mockDesignerHost.Object));
            Assert.Equal(1, creatingCallCount);
            Assert.Equal(0, createdCallCount);

            Assert.Throws<Exception>(() => item.CreateComponents(mockDesignerHost.Object, null));
            Assert.Equal(2, creatingCallCount);
            Assert.Equal(0, createdCallCount);

            Assert.Throws<Exception>(() => item.CreateComponents(mockDesignerHost.Object, new Hashtable()));
            Assert.Equal(3, creatingCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove handler.
            item.ComponentsCreating -= creatingHandler;
            item.ComponentsCreated -= createdHandler;

            Assert.Throws<Exception>(() => item.CreateComponents(mockDesignerHost.Object));
            Assert.Equal(3, creatingCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [Fact]
        public void ToolboxItem_CreateComponents_InvokeWithNullComponentsCoreWithHost_ReturnsExpected()
        {
            var item = new NullComponentsToolboxItem
            {
                TypeName = "typeName"
            };
            var mockDesignerHost = new Mock<IDesignerHost>(MockBehavior.Strict);
            mockDesignerHost
                .Setup(h => h.GetService(typeof(ITypeResolutionService)))
                .Returns(new CustomTypeResolutionService());
            mockDesignerHost
                .Setup(h => h.CreateComponent(typeof(bool)))
                .Returns(new Component());

            int creatingCallCount = 0;
            int createdCallCount = 0;
            ToolboxComponentsCreatingEventHandler creatingHandler = (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(mockDesignerHost.Object, e.DesignerHost);
                creatingCallCount++;
            };
            ToolboxComponentsCreatedEventHandler createdHandler = (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Null(e.Components);
                createdCallCount++;
            };
            item.ComponentsCreating += creatingHandler;
            item.ComponentsCreated += createdHandler;

            // With handler.
            Assert.Null(item.CreateComponents(mockDesignerHost.Object));
            Assert.Equal(1, creatingCallCount);
            Assert.Equal(0, createdCallCount);

            Assert.Null(item.CreateComponents(mockDesignerHost.Object, null));
            Assert.Equal(2, creatingCallCount);
            Assert.Equal(0, createdCallCount);

            Assert.Null(item.CreateComponents(mockDesignerHost.Object, new Hashtable()));
            Assert.Equal(3, creatingCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove handler.
            item.ComponentsCreating -= creatingHandler;
            item.ComponentsCreated -= createdHandler;

            Assert.Null(item.CreateComponents(mockDesignerHost.Object));
            Assert.Equal(3, creatingCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [Theory]
        [InlineData("")]
        [InlineData("NoSuchType")]
        [InlineData("System.Int32")]
        public void ToolboxItem_CreateComponents_InvokeInvalidType_ReturnsEmpty(string typeName)
        {
            var item = new ToolboxItem
            {
                TypeName = typeName
            };
            int creatingCallCount = 0;
            int createdCallCount = 0;
            ToolboxComponentsCreatingEventHandler creatingHandler = (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Null(e.DesignerHost);
                creatingCallCount++;
            };
            ToolboxComponentsCreatedEventHandler createdHandler = (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Null(e.Components);
                createdCallCount++;
            };
            item.ComponentsCreating += creatingHandler;
            item.ComponentsCreated += createdHandler;

            Assert.Empty(item.CreateComponents());
            Assert.Equal(1, creatingCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [Fact]
        public void ToolboxItem_CreateComponentsCore_InvokeWithoutHost_ReturnsExpected()
        {
            var item = new SubToolboxItem
            {
                AssemblyName = typeof(Component).Assembly.GetName(true),
                TypeName = "System.ComponentModel.Component"
            };
            int creatingCallCount = 0;
            int createdCallCount = 0;
            ToolboxComponentsCreatingEventHandler creatingHandler = (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Null(e.DesignerHost);
                creatingCallCount++;
            };
            ToolboxComponentsCreatedEventHandler createdHandler = (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.IsType<Component>(Assert.Single(e.Components));
                createdCallCount++;
            };
            item.ComponentsCreating += creatingHandler;
            item.ComponentsCreated += createdHandler;

            // With handler.
            Assert.IsType<Component>(Assert.Single(item.CreateComponentsCore(null)));
            Assert.Equal(0, creatingCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove handler.
            item.ComponentsCreating -= creatingHandler;
            item.ComponentsCreated -= createdHandler;

            Assert.IsType<Component>(Assert.Single(item.CreateComponentsCore(null)));
            Assert.Equal(0, creatingCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [Theory]
        [MemberData(nameof(CreateComponents_InvokeWithHostWithNonIComponentInitializerDesigner_TestData))]
        public void ToolboxItem_CreateComponentsCore_InvokeWithHostWithNonIComponentInitializerDesigner_ReturnsExpected(Component component, IDesigner designer)
        {
            var item = new SubToolboxItem
            {
                TypeName = "typeName"
            };
            var mockDesignerHost = new Mock<IDesignerHost>(MockBehavior.Strict);
            mockDesignerHost
                .Setup(h => h.GetService(typeof(ITypeResolutionService)))
                .Returns(new CustomTypeResolutionService());
            mockDesignerHost
                .Setup(h => h.CreateComponent(typeof(bool)))
                .Returns(component);
            mockDesignerHost
                .Setup(h => h.GetDesigner(component))
                .Returns(designer);

            int creatingCallCount = 0;
            int createdCallCount = 0;
            ToolboxComponentsCreatingEventHandler creatingHandler = (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(mockDesignerHost.Object, e.DesignerHost);
                creatingCallCount++;
            };
            ToolboxComponentsCreatedEventHandler createdHandler = (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(component, Assert.Single(e.Components));
                createdCallCount++;
            };
            item.ComponentsCreating += creatingHandler;
            item.ComponentsCreated += createdHandler;

            // With handler.
            Assert.Same(component, Assert.Single(item.CreateComponentsCore(mockDesignerHost.Object)));
            Assert.Equal(0, creatingCallCount);
            Assert.Equal(0, createdCallCount);

            Assert.Same(component, Assert.Single(item.CreateComponentsCore(mockDesignerHost.Object, null)));
            Assert.Equal(0, creatingCallCount);
            Assert.Equal(0, createdCallCount);

            Assert.Same(component, Assert.Single(item.CreateComponentsCore(mockDesignerHost.Object, new Hashtable())));
            Assert.Equal(0, creatingCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove handler.
            item.ComponentsCreating -= creatingHandler;
            item.ComponentsCreated -= createdHandler;

            Assert.Same(component, Assert.Single(item.CreateComponents(mockDesignerHost.Object)));
            Assert.Equal(0, creatingCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [Fact]
        public void ToolboxItem_CreateComponentsCore_InvokeWithHostWithIComponentInitializerDesigner_ReturnsExpected()
        {
            var component = new Component();
            var item = new SubToolboxItem
            {
                TypeName = "typeName"
            };
            var mockDesigner = new Mock<IDesigner>(MockBehavior.Strict);
            mockDesigner.Setup(d => d.Dispose());
            Mock<IComponentInitializer> mockComponentInitializer = mockDesigner.As<IComponentInitializer>();
            mockComponentInitializer
                .Setup(i => i.InitializeNewComponent(null));
            mockComponentInitializer
                .Setup(i => i.InitializeNewComponent(new Hashtable()));

            var mockDesignerHost = new Mock<IDesignerHost>(MockBehavior.Strict);
            mockDesignerHost
                .Setup(h => h.GetService(typeof(ITypeResolutionService)))
                .Returns(new CustomTypeResolutionService());
            mockDesignerHost
                .Setup(h => h.CreateComponent(typeof(bool)))
                .Returns(component);
            mockDesignerHost
                .Setup(h => h.GetDesigner(component))
                .Returns(mockDesigner.Object);

            int creatingCallCount = 0;
            int createdCallCount = 0;
            ToolboxComponentsCreatingEventHandler creatingHandler = (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(mockDesignerHost.Object, e.DesignerHost);
                creatingCallCount++;
            };
            ToolboxComponentsCreatedEventHandler createdHandler = (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(component, Assert.Single(e.Components));
                createdCallCount++;
            };
            item.ComponentsCreating += creatingHandler;
            item.ComponentsCreated += createdHandler;

            // With handler.
            Assert.Same(component, Assert.Single(item.CreateComponentsCore(mockDesignerHost.Object)));
            Assert.Equal(0, creatingCallCount);
            Assert.Equal(0, createdCallCount);

            Assert.Same(component, Assert.Single(item.CreateComponentsCore(mockDesignerHost.Object, null)));
            Assert.Equal(0, creatingCallCount);
            Assert.Equal(0, createdCallCount);

            Assert.Same(component, Assert.Single(item.CreateComponentsCore(mockDesignerHost.Object, new Hashtable())));
            Assert.Equal(0, creatingCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove handler.
            item.ComponentsCreating -= creatingHandler;
            item.ComponentsCreated -= createdHandler;

            Assert.Same(component, Assert.Single(item.CreateComponents(mockDesignerHost.Object)));
            Assert.Equal(0, creatingCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [Fact]
        public void ToolboxItem_CreateComponentsCore_InvokeWithHostWithThrowingIComponentInitializerDesigner_ReturnsExpected()
        {
            var component = new Component();
            var item = new SubToolboxItem
            {
                TypeName = "typeName"
            };
            var mockDesigner = new Mock<IDesigner>(MockBehavior.Strict);
            mockDesigner.Setup(d => d.Dispose());
            Mock<IComponentInitializer> mockComponentInitializer = mockDesigner.As<IComponentInitializer>();
            mockComponentInitializer
                .Setup(i => i.InitializeNewComponent(null))
                .Throws(new Exception());
            mockComponentInitializer
                .Setup(i => i.InitializeNewComponent(new Hashtable()))
                .Throws(new Exception());

            var mockDesignerHost = new Mock<IDesignerHost>(MockBehavior.Strict);
            mockDesignerHost
                .Setup(h => h.GetService(typeof(ITypeResolutionService)))
                .Returns(new CustomTypeResolutionService());
            mockDesignerHost
                .Setup(h => h.CreateComponent(typeof(bool)))
                .Returns(component);
            mockDesignerHost
                .Setup(h => h.GetDesigner(component))
                .Returns(mockDesigner.Object);
            mockDesignerHost
                .Setup(h => h.DestroyComponent(component));

            int creatingCallCount = 0;
            int createdCallCount = 0;
            ToolboxComponentsCreatingEventHandler creatingHandler = (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(mockDesignerHost.Object, e.DesignerHost);
                creatingCallCount++;
            };
            ToolboxComponentsCreatedEventHandler createdHandler = (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Same(component, Assert.Single(e.Components));
                createdCallCount++;
            };
            item.ComponentsCreating += creatingHandler;
            item.ComponentsCreated += createdHandler;

            // With handler.
            Assert.Same(component, Assert.Single(item.CreateComponentsCore(mockDesignerHost.Object)));
            Assert.Equal(0, creatingCallCount);
            Assert.Equal(0, createdCallCount);

            Assert.Throws<Exception>(() => item.CreateComponentsCore(mockDesignerHost.Object, null));
            Assert.Equal(0, creatingCallCount);
            Assert.Equal(0, createdCallCount);

            Assert.Throws<Exception>(() => item.CreateComponentsCore(mockDesignerHost.Object, new Hashtable()));
            Assert.Equal(0, creatingCallCount);
            Assert.Equal(0, createdCallCount);

            // Remove handler.
            item.ComponentsCreating -= creatingHandler;
            item.ComponentsCreated -= createdHandler;

            Assert.Same(component, Assert.Single(item.CreateComponentsCore(mockDesignerHost.Object)));
            Assert.Equal(0, creatingCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [Theory]
        [InlineData("")]
        [InlineData("NoSuchType")]
        [InlineData("System.Int32")]
        public void ToolboxItem_CreateComponentsCore_InvokeInvalidType_ReturnsEmpty(string typeName)
        {
            var item = new SubToolboxItem
            {
                TypeName = typeName
            };
            int creatingCallCount = 0;
            int createdCallCount = 0;
            ToolboxComponentsCreatingEventHandler creatingHandler = (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Null(e.DesignerHost);
                creatingCallCount++;
            };
            ToolboxComponentsCreatedEventHandler createdHandler = (sender, e) =>
            {
                Assert.Same(item, sender);
                Assert.Null(e.Components);
                createdCallCount++;
            };
            item.ComponentsCreating += creatingHandler;
            item.ComponentsCreated += createdHandler;

            Assert.Empty(item.CreateComponentsCore(null));
            Assert.Equal(0, creatingCallCount);
            Assert.Equal(0, createdCallCount);
        }

        [Fact]
        public void ToolboxItem_CheckUnlocked_NotLocked_Nop()
        {
            var item = new SubToolboxItem();
            item.CheckUnlocked();
            item.CheckUnlocked();
        }

        [Fact]
        public void ToolboxItem_CheckUnlocked_Locked_ThrowsInvalidOperationException()
        {
            var item = new SubToolboxItem();
            item.Lock();
            Assert.Throws<InvalidOperationException>(() => item.CheckUnlocked());
        }

        public static IEnumerable<object[]> Equals_TestData()
        {
            var item = new ToolboxItem();
            yield return new object[] { item, item, true };
            yield return new object[] { item, new ToolboxItem(), true };
            yield return new object[] { item, new SubToolboxItem(), false };

            yield return new object[]
            {
                item,
                new ToolboxItem
                {
                    Company = "Company",
                    DependentAssemblies = new AssemblyName[] { null },
                    Description = "Description",
                    Filter = new ToolboxItemFilterAttribute[] { new ToolboxItemFilterAttribute("Filter") },
                    IsTransient = true
                },
                true
            };

            yield return new object[]
            {
                new ToolboxItem { TypeName = "TypeName" },
                new ToolboxItem { TypeName = "TypeName" },
                true
            };
            yield return new object[]
            {
                new ToolboxItem { TypeName = "TypeName" },
                new ToolboxItem { TypeName = "typename" },
                false
            };
            yield return new object[]
            {
                new ToolboxItem { TypeName = "TypeName" },
                new ToolboxItem(),
                false
            };
            yield return new object[]
            {
                new ToolboxItem(),
                new ToolboxItem { TypeName = "TypeName" },
                false
            };
            yield return new object[]
            {
                new NoValidationToolboxItem { TypeName = null },
                new NoValidationToolboxItem { TypeName = null },
                true
            };
            yield return new object[]
            {
                new NoValidationToolboxItem { TypeName = null },
                new NoValidationToolboxItem { TypeName = "TypeName" },
                false
            };
            yield return new object[]
            {
                new NoValidationToolboxItem { TypeName = "TypeName" },
                new NoValidationToolboxItem { TypeName = null },
                false
            };

            yield return new object[]
            {
                new ToolboxItem { DisplayName = "DisplayName" },
                new ToolboxItem { DisplayName = "DisplayName" },
                true
            };
            yield return new object[]
            {
                new ToolboxItem { DisplayName = "DisplayName" },
                new ToolboxItem { DisplayName = "displayname" },
                false
            };
            yield return new object[]
            {
                new ToolboxItem { DisplayName = "DisplayName" },
                new ToolboxItem(),
                false
            };
            yield return new object[]
            {
                new ToolboxItem(),
                new ToolboxItem { DisplayName = "DisplayName" },
                false
            };
            yield return new object[]
            {
                new NoValidationToolboxItem { DisplayName = null },
                new NoValidationToolboxItem { DisplayName = null },
                true
            };
            yield return new object[]
            {
                new NoValidationToolboxItem { DisplayName = null },
                new NoValidationToolboxItem { DisplayName = "TypeName" },
                false
            };
            yield return new object[]
            {
                new NoValidationToolboxItem { DisplayName = "TypeName" },
                new NoValidationToolboxItem { DisplayName = null },
                false
            };

            yield return new object[]
            {
                new ToolboxItem { AssemblyName = new AssemblyName("Name") },
                new ToolboxItem { AssemblyName = new AssemblyName("Name") },
                true
            };
            yield return new object[]
            {
                new ToolboxItem { AssemblyName = new AssemblyName("Name") },
                new ToolboxItem { AssemblyName = new AssemblyName("name") },
                false
            };
            yield return new object[]
            {
                new ToolboxItem(),
                new ToolboxItem { AssemblyName = new AssemblyName("Name") },
                false
            };
            yield return new object[]
            {
                new ToolboxItem { AssemblyName = new AssemblyName("Name") },
                new ToolboxItem(),
                false
            };

            yield return new object[] { new ToolboxItem(), new object(), false };
            yield return new object[] { new ToolboxItem(), null, false };
        }

        [Theory]
        [MemberData(nameof(Equals_TestData))]
        public void ToolboxItem_Equals_Invoke_ReturnsExpected(ToolboxItem item, object other, bool expected)
        {
            Assert.Equal(expected, item.Equals(other));
        }

        public static IEnumerable<object[]> FilterPropertyValue_TestData()
        {
            var o = new object();
            yield return new object[] { "AssemblyName", null, null, true };
            yield return new object[] { "AssemblyName", new AssemblyName("Name"), new AssemblyName("Name"), false };
            yield return new object[] { "AssemblyName", o, o, true };
            yield return new object[] { "assemblyName", new AssemblyName("Name"), new AssemblyName("Name"), true };

            yield return new object[] { "DisplayName", null, string.Empty, false };
            yield return new object[] { "DisplayName", "value", "value", true };
            yield return new object[] { "DisplayName", o, o, true };
            yield return new object[] { "displayname", null, null, true };

            yield return new object[] { "TypeName", null, string.Empty, false };
            yield return new object[] { "TypeName", "value", "value", true };
            yield return new object[] { "TypeName", o, o, true };
            yield return new object[] { "typename", null, null, true };

            yield return new object[] { "Filter", null, Array.Empty<ToolboxItemFilterAttribute>(), false };
            yield return new object[] { "Filter", Array.Empty<ToolboxItemFilterAttribute>(), Array.Empty<ToolboxItemFilterAttribute>(), true };
            yield return new object[] { "Filter", o, o, true };
            yield return new object[] { "filter", null, null, true };

            yield return new object[] { "IsTransient", null, false, false };
            yield return new object[] { "IsTransient", true, true, true };
            yield return new object[] { "IsTransient", o, o, true };
            yield return new object[] { "istransient", null, null, true };

            yield return new object[] { "NoSuchProperty", null, null, true };
            yield return new object[] { "NoSuchProperty", "value", "value", true };
            yield return new object[] { "NoSuchProperty", o, o, true };
        }

        [Theory]
        [MemberData(nameof(FilterPropertyValue_TestData))]
        public void ToolboxItem_FilterPropertyValue_Invoke_ReturnsExpected(string propertyName, object value, object expected, bool same)
        {
            var item = new SubToolboxItem();
            object actual = item.FilterPropertyValue(propertyName, value);
            if (expected is AssemblyName expectedName)
            {
                Assert.Equal(expectedName.FullName, Assert.IsType<AssemblyName>(actual).FullName);
            }
            else
            {
                Assert.Equal(expected, actual);
            }
            Assert.Equal(same, object.ReferenceEquals(value, actual));
        }

        public static IEnumerable<object[]> GetHashCode_TestData()
        {
            yield return new object[] { new ToolboxItem() };
            yield return new object[] { new ToolboxItem { TypeName = "TypeName", DisplayName = "DisplayName" } };
            yield return new object[] { new NoValidationToolboxItem { TypeName = null, DisplayName = null } };
        }

        [Theory]
        [MemberData(nameof(GetHashCode_TestData))]
        public void ToolboxItem_GetHashCode_Invoke_ReturnsExpected(ToolboxItem item)
        {
            Assert.Equal(item.GetHashCode(), item.GetHashCode());
        }

        public static IEnumerable<object[]> GetType_TestData()
        {
            var nullServiceDesignerHost = new Mock<IDesignerHost>(MockBehavior.Strict);
            nullServiceDesignerHost
                .Setup(h => h.GetService(typeof(ITypeResolutionService)))
                .Returns(null);
            foreach (object host in new object[] { null, nullServiceDesignerHost.Object })
            {
                yield return new object[] { null, null, "System.Int32", false, typeof(int) };
                yield return new object[] { null, new AssemblyName("NoSuchAssembly"), "System.Int32", false, typeof(int) };
                yield return new object[] { null, new AssemblyName(typeof(int).Assembly.FullName), "System.Int32", false, typeof(int) };
                yield return new object[] { null, new AssemblyName(typeof(ToolboxItem).Assembly.FullName), "System.Int32", false, typeof(int) };
                yield return new object[] { null, new AssemblyName(typeof(int).Assembly.FullName), "System.Drawing.Design.Tests.ToolboxItemTests", false, null };
                yield return new object[] { null, new AssemblyName(typeof(ToolboxItemTests).Assembly.FullName), "System.Drawing.Design.Tests.ToolboxItemTests", false, typeof(ToolboxItemTests) };
                yield return new object[] { null, new AssemblyName(typeof(ToolboxItemTests).Assembly.FullName), "System.Drawing.Design.Tests.toolboxitemtests", false, null };
                yield return new object[] { null, new AssemblyName(typeof(ToolboxItemTests).Assembly.FullName), "NoSuchType", false, null };
                yield return new object[] { null, null, string.Empty, false, null };

                var validNameWithCodeBase = new AssemblyName(typeof(int).Assembly.FullName)
                {
                    CodeBase = "System.Windows.Forms.Design.Tests.dll"
                };
                yield return new object[] { null, validNameWithCodeBase, "System.Drawing.Design.Tests.ToolboxItemTests", false, null };

                var invalidNameWithCodeBase = new AssemblyName("NoSuchAssembly")
                {
                    CodeBase = "System.Windows.Forms.Design.Tests.dll"
                };
                yield return new object[] { null, invalidNameWithCodeBase, "System.Drawing.Design.Tests.ToolboxItemTests", false, typeof(ToolboxItemTests) };

                var invalidNameWithInvalidCodeBase = new AssemblyName("NoSuchAssembly")
                {
                    CodeBase = "AlsoNoSuchAssembly"
                };
                yield return new object[] { null, invalidNameWithInvalidCodeBase, "System.Drawing.Design.Tests.ToolboxItemTests", false, null };

                AssemblyLoadContext.Default.Resolving += (context, name) =>
                {
                    if (name.Name == "ThrowBadImageFormatException")
                    {
                        throw new BadImageFormatException();
                    }
                    else if (name.Name == "ThrowIOException")
                    {
                        throw new IOException();
                    }

                    return null;
                };
                yield return new object[] { null, new AssemblyName("ThrowBadImageFormatException"), "System.Int32", false, typeof(int) };
                yield return new object[] { null, new AssemblyName("ThrowIOException"), "System.Int32", false, typeof(int) };

                var badImageFormatExceptionCodeBase = new AssemblyName("NoSuchAssembly")
                {
                    CodeBase = "ThrowBadImageFormatException"
                };
                yield return new object[] { null, badImageFormatExceptionCodeBase, "System.Int32", false, typeof(int) };

                var ioFormatExceptionCodeBase = new AssemblyName("NoSuchAssembly")
                {
                    CodeBase = "ThrowIOException"
                };
                yield return new object[] { null, ioFormatExceptionCodeBase, "System.Int32", false, typeof(int) };
            }

            var invalidServiceDesignerHost = new Mock<IDesignerHost>(MockBehavior.Strict);
            invalidServiceDesignerHost
                .Setup(h => h.GetService(typeof(ITypeResolutionService)))
                .Returns(new object());
            yield return new object[] { invalidServiceDesignerHost.Object, new AssemblyName(typeof(int).Assembly.FullName), "System.Int32", false, typeof(int) };

            foreach (bool reference in new bool[] { true, false })
            {
                var mockDesignerHost = new Mock<IDesignerHost>(MockBehavior.Strict);
                mockDesignerHost
                    .Setup(h => h.GetService(typeof(ITypeResolutionService)))
                    .Returns(new CustomTypeResolutionService());
                yield return new object[] { mockDesignerHost.Object, null, "typeName", reference, typeof(bool) };
                yield return new object[] { mockDesignerHost.Object, null, string.Empty, false, null };
                yield return new object[] { mockDesignerHost.Object, new AssemblyName(), "typeName", reference, typeof(bool) };
                yield return new object[] { mockDesignerHost.Object, new AssemblyName(typeof(int).Assembly.FullName), "System.Int32", reference, typeof(int) };
                yield return new object[] { mockDesignerHost.Object, new AssemblyName(typeof(int).Assembly.FullName), "System.Drawing.Design.Tests.ToolboxItemTests", reference, typeof(ToolboxItemTests) };
                yield return new object[] { mockDesignerHost.Object, new AssemblyName(typeof(int).Assembly.FullName), "System.Drawing.Design.Tests.toolboxitemtests", reference, null };
                yield return new object[] { mockDesignerHost.Object, new AssemblyName(typeof(int).Assembly.FullName), "NoSuchType", reference, null };
            }
        }

        [Theory]
        [MemberData(nameof(GetType_TestData))]
        public void ToolboxItem_GetType_InvokeWithoutTypeNameAssemblyName_ReturnsExpected(IDesignerHost host, AssemblyName assemblyName, string typeName, bool reference, Type expected)
        {
            if (reference)
            {
                return;
            }

            var item = new ToolboxItem
            {
                AssemblyName = assemblyName,
                TypeName = typeName
            };
            Assert.Equal(expected, item.GetType(host));
        }

        [Theory]
        [MemberData(nameof(GetType_TestData))]
        public void ToolboxItem_GetType_InvokeWithTypeNameAssemblyName_ReturnsExpected(IDesignerHost host, AssemblyName assemblyName, string typeName, bool reference, Type expected)
        {
            var item = new SubToolboxItem();
            Assert.Equal(expected, item.GetType(host, assemblyName, typeName, reference));
        }

        [Fact]
        public void ToolboxItem_GetType_NullTypeName_ThrowsArgumentNullException()
        {
            var item = new SubToolboxItem();
            var mockDesignerHost = new Mock<IDesignerHost>(MockBehavior.Strict);
            mockDesignerHost
                .Setup(h => h.GetService(typeof(ITypeResolutionService)))
                .Returns(null);
            Assert.Throws<ArgumentNullException>("typeName", () => item.GetType(mockDesignerHost.Object, null, null, false));
            Assert.Throws<ArgumentNullException>("typeName", () => item.GetType(null, null, null, false));
        }

        [Fact]
        public void ToolboxItem_GetType_EmptyAssemblyName_ThrowsArgumentException()
        {
            var item = new SubToolboxItem();
            var mockDesignerHost = new Mock<IDesignerHost>(MockBehavior.Strict);
            mockDesignerHost
                .Setup(h => h.GetService(typeof(ITypeResolutionService)))
                .Returns(null);
            Assert.Throws<ArgumentException>(null, () => item.GetType(mockDesignerHost.Object, new AssemblyName(), "typeName", false));
            Assert.Throws<ArgumentException>(null, () => item.GetType(null, new AssemblyName(), "typeName", false));
        }

        public static IEnumerable<object[]> Initialize_TypeWithAttributes_TestData()
        {
            yield return new object[] { typeof(ClassWithValidAttributes), new Size(16, 16) };
            yield return new object[] { typeof(ClassWithStretchedWidthImage), new Size(24, 16) };
            yield return new object[] { typeof(ClassWithStretchedHeightImage), new Size(16, 24) };
            yield return new object[] { typeof(ClassWithInvalidImage), new Size(16, 16) };
        }

        [Theory]
        [MemberData(nameof(Initialize_TypeWithAttributes_TestData))]
        public void ToolboxItem_Initialize_TypeWithAttributes_Success(Type type, Size expectedOriginalBitmapSize)
        {
            using (var bitmap = new Bitmap(10, 10))
            using (var originalBitmap = new Bitmap(10, 10))
            {
                var filter = new ToolboxItemFilterAttribute[] { new ToolboxItemFilterAttribute("Filter") };
                var item = new ToolboxItem
                {
                    AssemblyName = new AssemblyName("AssemblyName"),
                    Bitmap = bitmap,
                    Company = "Company",
                    Description = "Description",
                    DependentAssemblies = new AssemblyName[2],
                    DisplayName = "DisplayName",
                    Filter = filter,
                    OriginalBitmap = originalBitmap
                };
                item.Initialize(type);
                if (expectedOriginalBitmapSize == new Size(10, 10))
                {
                    Assert.NotEqual(bitmap, item.Bitmap);
                    Assert.Same(item.Bitmap, item.Bitmap);
                }
                else
                {
                    Assert.Equal(new Size(16, 16), item.Bitmap.Size);
                }
                Assert.Equal("Microsoft Corporation", item.Company);
                Assert.Equal("Description", item.Description);
                Assert.Equal(type.Assembly.FullName, item.AssemblyName.FullName);
                Assert.Equal(new string[] { type.Assembly.FullName }, item.DependentAssemblies.Select(a => a.FullName));
                Assert.Equal(type.Name, item.DisplayName);
                Assert.Equal(new string[] { type.Name, "Filter", "System.Drawing.Design.Tests.ToolboxItemTests+" + type.Name }, item.Filter.Cast<ToolboxItemFilterAttribute>().Select(a => a.FilterString).OrderBy(f => f));
                Assert.Equal(expectedOriginalBitmapSize, item.OriginalBitmap.Size);
            }
        }

        [Fact]
        public void ToolboxItem_Initialize_ObjectType_Success()
        {
            using (var bitmap = new Bitmap(10, 10))
            using (var originalBitmap = new Bitmap(10, 10))
            {
                var filter = new ToolboxItemFilterAttribute[] { new ToolboxItemFilterAttribute("Filter") };
                var item = new ToolboxItem
                {
                    AssemblyName = new AssemblyName("AssemblyName"),
                    Bitmap = bitmap,
                    Company = "Company",
                    Description = "Description",
                    DependentAssemblies = new AssemblyName[2],
                    DisplayName = "DisplayName",
                    Filter = filter,
                    OriginalBitmap = originalBitmap
                };
                item.Initialize(typeof(object));
                Assert.NotEqual(bitmap, item.Bitmap);
                Assert.Same(item.Bitmap, item.Bitmap);
                Assert.Equal("Microsoft Corporation", item.Company);
                Assert.Empty(item.Description);
                Assert.Equal(typeof(object).Assembly.FullName, item.AssemblyName.FullName);
                Assert.Equal(new string[] { typeof(object).Assembly.FullName }, item.DependentAssemblies.Select(a => a.FullName));
                Assert.Equal("Object", item.DisplayName);
                Assert.Equal(new string[] { "System.Object" }, item.Filter.Cast<ToolboxItemFilterAttribute>().Select(a => a.FilterString));
                Assert.Same(item.OriginalBitmap, item.OriginalBitmap);
            }
        }

        [Fact]
        public void ToolboxItem_Initialize_NullType_Nop()
        {
            using (var bitmap = new Bitmap(10, 10))
            using (var originalBitmap = new Bitmap(10, 10))
            {
                var filter = new ToolboxItemFilterAttribute[] { new ToolboxItemFilterAttribute("Filter") };
                var item = new ToolboxItem
                {
                    AssemblyName = new AssemblyName("AssemblyName"),
                    Bitmap = bitmap,
                    Company = "Company",
                    Description = "Description",
                    DependentAssemblies = new AssemblyName[2],
                    DisplayName = "DisplayName",
                    Filter = filter,
                    OriginalBitmap = originalBitmap
                };
                item.Initialize(null);
                Assert.Equal("AssemblyName", item.AssemblyName.FullName);
                Assert.Same(bitmap, item.Bitmap);
                Assert.Equal("Company", item.Company);
                Assert.Equal("Description", item.Description);
                Assert.Equal(new AssemblyName[2], item.DependentAssemblies);
                Assert.Equal("DisplayName", item.DisplayName);
                Assert.Equal(filter, item.Filter);
                Assert.Same(originalBitmap, item.OriginalBitmap);
            }
        }

        [Theory]
        [InlineData(null)]
        [InlineData(typeof(int))]
        public void ToolboxItem_Initialize_Locked_ThrowsInvalidOperationException(Type type)
        {
            var item = new ToolboxItem();
            item.Lock();
            Assert.Throws<InvalidOperationException>(() => item.Initialize(type));
        }

        [Fact]
        public void ToolboxItem_Lock_Invoke_Success()
        {
            var item = new ToolboxItem();
            item.Lock();
            Assert.True(item.Locked);
            Assert.True(item.Properties.IsFixedSize);
            Assert.True(item.Properties.IsReadOnly);

            // Lock again.
            item.Lock();
            Assert.True(item.Locked);
            Assert.True(item.Properties.IsFixedSize);
            Assert.True(item.Properties.IsReadOnly);
        }

        [Fact]
        public void ToolboxItem_OnComponentsCreated_Invoke_Success()
        {
            var item = new SubToolboxItem();

            // No handler.
            item.OnComponentsCreated(null);

            // Handler.
            int callCount = 0;
            ToolboxComponentsCreatedEventHandler handler = (sender, e) =>
            {
                Assert.Equal(item, sender);
                Assert.Null(e);
                callCount++;
            };

            item.ComponentsCreated += handler;
            item.OnComponentsCreated(null);
            Assert.Equal(1, callCount);

            // Should not call if the handler is removed.
            item.ComponentsCreated -= handler;
            item.OnComponentsCreated(null);
            Assert.Equal(1, callCount);
        }
        [Fact]
        public void ToolboxItem_OnComponentsCreating_Invoke_Success()
        {
            var item = new SubToolboxItem();

            // No handler.
            item.OnComponentsCreating(null);

            // Handler.
            int callCount = 0;
            ToolboxComponentsCreatingEventHandler handler = (sender, e) =>
            {
                Assert.Equal(item, sender);
                Assert.Null(e);
                callCount++;
            };

            item.ComponentsCreating += handler;
            item.OnComponentsCreating(null);
            Assert.Equal(1, callCount);

            // Should not call if the handler is removed.
            item.ComponentsCreating -= handler;
            item.OnComponentsCreating(null);
            Assert.Equal(1, callCount);
        }

        public static IEnumerable<object[]> ToString_TestData()
        {
            yield return new object[] { new ToolboxItem(), string.Empty };
            yield return new object[] { new ToolboxItem { DisplayName = "DisplayName" }, "DisplayName" };
            yield return new object[] { new NoValidationToolboxItem { DisplayName = null }, string.Empty };
        }

        [Theory]
        [MemberData(nameof(ToString_TestData))]
        public void ToolboxItem_ToString_Invoke_ReturnsExpected(ToolboxItem item, string expected)
        {
            Assert.Equal(expected, item.ToString());
        }

        public static IEnumerable<object[]> ValidatePropertyValue_TestData()
        {
            var name = new AssemblyName();
            yield return new object[] { "AssemblyName", null, null };
            yield return new object[] { "AssemblyName", name, name };

            var bitmap = new Bitmap(10, 10);
            yield return new object[] { "Bitmap", null, null };
            yield return new object[] { "Bitmap", bitmap, bitmap };

            var originalBitmap = new Bitmap(10, 10);
            yield return new object[] { "OriginalBitmap", null, null };
            yield return new object[] { "OriginalBitmap", originalBitmap, originalBitmap };

            yield return new object[] { "Company", null, string.Empty };
            yield return new object[] { "Company", "value", "value" };

            yield return new object[] { "Description", null, string.Empty };
            yield return new object[] { "Description", "value", "value" };

            yield return new object[] { "DisplayName", null, string.Empty };
            yield return new object[] { "DisplayName", "value", "value" };

            yield return new object[] { "TypeName", null, string.Empty };
            yield return new object[] { "TypeName", "value", "value" };

            var filter = new ToolboxItemFilterAttribute("filter");
            yield return new object[] { "Filter", null, Array.Empty<ToolboxItemFilterAttribute>() };
            yield return new object[] { "Filter", Array.Empty<ToolboxItemFilterAttribute>(), Array.Empty<ToolboxItemFilterAttribute>() };
            yield return new object[] { "Filter", new object[] { null, "value", filter, filter }, new ToolboxItemFilterAttribute[] { filter, filter } };

            yield return new object[] { "NoSuchProperty", null, null };
            yield return new object[] { "NoSuchProperty", 1, 1 };

            yield return new object[] { "istransient", null, null };
            yield return new object[] { "istransient", 1, 1 };
        }

        [Theory]
        [MemberData(nameof(ValidatePropertyValue_TestData))]
        public void ToolboxItem_ValidatePropertyValue_ValueAllowed_ReturnsExpected(string propertyName, object value, object expected)
        {
            var item = new SubToolboxItem();
            Assert.Equal(expected, item.ValidatePropertyValue(propertyName, value));
        }

        [Theory]
        [InlineData("IsTransient")]
        public void ToolboxItem_ValidatePropertyValue_NullValueDisallowed_ThrowsArgumentNullException(string propertyName)
        {
            var item = new SubToolboxItem();
            Assert.Throws<ArgumentNullException>("value", () => item.ValidatePropertyValue(propertyName, null));
        }

        [Theory]
        [InlineData("AssemblyName")]
        [InlineData("Bitmap")]
        [InlineData("OriginalBitmap")]
        [InlineData("Company")]
        [InlineData("Description")]
        [InlineData("DisplayName")]
        [InlineData("TypeName")]
        [InlineData("Filter")]
        [InlineData("DependentAssemblies")]
        public void ToolboxItem_ValidatePropertyValue_InvalidValue_ThrowsArgumentException(string propertyName)
        {
            var item = new SubToolboxItem();
            Assert.Throws<ArgumentException>("value", () => item.ValidatePropertyValue(propertyName, new object()));
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("propertyName", typeof(int))]
        public void ToolboxItem_ValidatePropertyType_NullDisallowed_ThrowsArgumentNullException(string propertyName, Type expectedType)
        {
            var item = new SubToolboxItem();
            Assert.Throws<ArgumentNullException>("value", () => item.ValidatePropertyType(propertyName, null, expectedType, allowNull: false));
        }

        [Theory]
        [InlineData(null, null)]
        [InlineData("propertyName", typeof(int))]
        public void ToolboxItem_ValidatePropertyType_NullAllowed_Nop(string propertyName, Type expectedType)
        {
            var item = new SubToolboxItem();
            item.ValidatePropertyType(propertyName, null, expectedType, allowNull: true);
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("propertyName", true)]
        public void ToolboxItem_ValidatePropertyType_ValidType_Nop(string propertyName, bool allowNull)
        {
            var item = new SubToolboxItem();
            item.ValidatePropertyType(propertyName, 1, typeof(int), allowNull);
        }

        [Theory]
        [InlineData(null, false)]
        [InlineData("propertyName", true)]
        public void ToolboxItem_ValidatePropertyType_InvalidType_ThrowsArgumentException(string propertyName, bool allowNull)
        {
            var item = new SubToolboxItem();
            Assert.Throws<ArgumentException>("value", () => item.ValidatePropertyType(propertyName, new object(), typeof(int), allowNull));
        }

        private class SubToolboxItem : ToolboxItem
        {
            public new void CheckUnlocked() => base.CheckUnlocked();

            public new IComponent[] CreateComponentsCore(IDesignerHost host)
            {
                return base.CreateComponentsCore(host);
            }

            public new IComponent[] CreateComponentsCore(IDesignerHost host, IDictionary defaultValues)
            {
                return base.CreateComponentsCore(host, defaultValues);
            }

            public new object FilterPropertyValue(string propertyName, object value)
            {
                return base.FilterPropertyValue(propertyName, value);
            }

            public new Type GetType(IDesignerHost host, AssemblyName assemblyName, string typeName, bool reference)
            {
                return base.GetType(host, assemblyName, typeName, reference);
            }

            public new void OnComponentsCreated(ToolboxComponentsCreatedEventArgs args)
            {
                base.OnComponentsCreated(args);
            }

            public new void OnComponentsCreating(ToolboxComponentsCreatingEventArgs args)
            {
                base.OnComponentsCreating(args);
            }

            public new object ValidatePropertyValue(string propertyName, object value)
            {
                return base.ValidatePropertyValue(propertyName, value);
            }

            public new void ValidatePropertyType(string propertyName, object value, Type expectedType, bool allowNull)
            {
                base.ValidatePropertyType(propertyName, value, expectedType, allowNull);
            }
        }

        private class CustomTypeResolutionService : ITypeResolutionService
        {
            public Assembly GetAssemblyResult { get; set; }

            public Assembly GetAssembly(AssemblyName name) => GetAssemblyResult;

            public Assembly GetAssembly(AssemblyName name, bool throwOnError)
            {
                throw new NotImplementedException();
            }

            public string GetPathOfAssembly(AssemblyName name)
            {
                throw new NotImplementedException();
            }

            public Type GetType(string name)
            {
                if (name == "typeName")
                {
                    return typeof(bool);
                }

                return Type.GetType(name);
            }

            public Type GetType(string name, bool throwOnError)
            {
                throw new NotImplementedException();
            }

            public Type GetType(string name, bool throwOnError, bool ignoreCase)
            {
                throw new NotImplementedException();
            }

            public List<AssemblyName> ReferenceAssemblies { get; } = new List<AssemblyName>();

            public void ReferenceAssembly(AssemblyName name)
            {
                ReferenceAssemblies.Add(name);
            }
        }

        private class NoValidationToolboxItem : ToolboxItem
        {
            protected override object FilterPropertyValue(string propertyName, object value)
            {
                // Don't normalize.
                return value;
            }

            protected override object ValidatePropertyValue(string propertyName, object value)
            {
                // Don't normalize.
                return value;
            }
        }

        private class NullComponentsToolboxItem : ToolboxItem
        {
            protected override IComponent[] CreateComponentsCore(IDesignerHost host)
            {
                return null;
            }
        }

        [Description("Description")]
        [ToolboxBitmap(typeof(ToolboxItemTests), "16x16.bmp")]
        [ToolboxItemFilter("System.Drawing.Design.Tests.ToolboxItemTests+ClassWithValidAttributes")]
        [ToolboxItemFilter("ClassWithValidAttributes")]
        [ToolboxItemFilter("Filter")]
        private class ClassWithValidAttributes
        {
        }

        [Description("Description")]
        [ToolboxBitmap(typeof(ToolboxItemTests), "24x16.bmp")]
        [ToolboxItemFilter("System.Drawing.Design.Tests.ToolboxItemTests+ClassWithStretchedWidthImage")]
        [ToolboxItemFilter("ClassWithStretchedWidthImage")]
        [ToolboxItemFilter("Filter")]
        private class ClassWithStretchedWidthImage
        {
        }

        [Description("Description")]
        [ToolboxBitmap(typeof(ToolboxItemTests), "16x24.bmp")]
        [ToolboxItemFilter("System.Drawing.Design.Tests.ToolboxItemTests+ClassWithStretchedHeightImage")]
        [ToolboxItemFilter("ClassWithStretchedHeightImage")]
        [ToolboxItemFilter("Filter")]
        private class ClassWithStretchedHeightImage
        {
        }

        [Description("Description")]
        [ToolboxBitmap("NoSuchImage")]
        [ToolboxItemFilter("System.Drawing.Design.Tests.ToolboxItemTests+ClassWithInvalidImage")]
        [ToolboxItemFilter("ClassWithInvalidImage")]
        [ToolboxItemFilter("Filter")]
        private class ClassWithInvalidImage
        {
        }
    }
}
