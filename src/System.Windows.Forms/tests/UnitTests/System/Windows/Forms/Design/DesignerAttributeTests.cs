// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;

namespace System.Windows.Forms.Design.Tests
{
    // NB: doesn't require thread affinity
    public class DesignerAttributeTests : IClassFixture<ThreadExceptionFixture>
    {
        private const string AssemblyRef_SystemWinforms = "System.Windows.Forms, Version=" + FXAssembly.Version + ", Culture=neutral, PublicKeyToken=" + AssemblyRef.MicrosoftPublicKey;
        private const string AssemblyRef_SystemWinformsDesign = "System.Windows.Forms.Design, Version=" + FXAssembly.Version + ", Culture=neutral, PublicKeyToken=" + AssemblyRef.MicrosoftPublicKey;
        private readonly ITestOutputHelper _output;

        private static readonly ImmutableHashSet<string> SkipList = ImmutableHashSet.Create(new string[] {
            // https://github.com/dotnet/winforms/issues/2412
            "System.Windows.Forms.Design.ControlBindingsConverter, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",

            // https://github.com/dotnet/winforms/issues/2411
            "System.Windows.Forms.Design.AxDesigner, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
            "System.Windows.Forms.Design.AxHostDesigner, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
            "System.Windows.Forms.Design.BindingNavigatorDesigner, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
            "System.Windows.Forms.Design.BindingSourceDesigner, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
            "System.Windows.Forms.Design.ButtonBaseDesigner, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
            "System.Windows.Forms.Design.ComboBoxDesigner, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
            "System.Windows.Forms.Design.DataGridViewColumnDesigner, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
            "System.Windows.Forms.Design.DataGridViewComboBoxColumnDesigner, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
            "System.Windows.Forms.Design.DataGridViewDesigner, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
            "System.Windows.Forms.Design.DateTimePickerDesigner, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
            "System.Windows.Forms.Design.FlowLayoutPanelDesigner, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
            "System.Windows.Forms.Design.FolderBrowserDialogDesigner, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
            "System.Windows.Forms.Design.GroupBoxDesigner, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
            "System.Windows.Forms.Design.ImageListDesigner, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
            "System.Windows.Forms.Design.LabelDesigner, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
            "System.Windows.Forms.Design.ListBoxDesigner, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
            "System.Windows.Forms.Design.ListViewDesigner, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
            "System.Windows.Forms.Design.MonthCalendarDesigner, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
            "System.Windows.Forms.Design.NotifyIconDesigner, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
            "System.Windows.Forms.Design.PanelDesigner, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
            "System.Windows.Forms.Design.PictureBoxDesigner, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
            "System.Windows.Forms.Design.PrintDialogDesigner, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
            "System.Windows.Forms.Design.PropertyGridDesigner, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
            "System.Windows.Forms.Design.RadioButtonDesigner, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
            "System.Windows.Forms.Design.RichTextBoxDesigner, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
            "System.Windows.Forms.Design.SaveFileDialogDesigner, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
            "System.Windows.Forms.Design.SplitContainerDesigner, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
            "System.Windows.Forms.Design.SplitterDesigner, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
            "System.Windows.Forms.Design.SplitterPanelDesigner, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
            "System.Windows.Forms.Design.StatusBarDesigner, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
            "System.Windows.Forms.Design.TabControlDesigner, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
            "System.Windows.Forms.Design.TabPageDesigner, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
            "System.Windows.Forms.Design.TableLayoutPanelDesigner, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
            "System.Windows.Forms.Design.TextBoxDesigner, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
            "System.Windows.Forms.Design.ToolStripContainerDesigner, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
            "System.Windows.Forms.Design.ToolStripContentPanelDesigner, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
            "System.Windows.Forms.Design.ToolStripPanelDesigner, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
            "System.Windows.Forms.Design.TrackBarDesigner, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
            "System.Windows.Forms.Design.TreeViewDesigner, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
            "System.Windows.Forms.Design.UpDownBaseDesigner, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
            "System.Windows.Forms.Design.UserControlDocumentDesigner, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
            "System.Windows.Forms.Design.WebBrowserDesigner, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",

            // https://github.com/dotnet/winforms/issues/1115
            "System.Windows.Forms.Design.DataGridViewColumnCollectionEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
            "System.Windows.Forms.Design.DataGridViewColumnDataPropertyNameEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
            "System.Windows.Forms.Design.DataGridViewComponentEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
            "System.Windows.Forms.Design.DataMemberFieldEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
            "System.Windows.Forms.Design.DataMemberListEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
            "System.Windows.Forms.Design.StyleCollectionEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
            "System.Windows.Forms.Design.ToolStripCollectionEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
            "System.Windows.Forms.Design.ToolStripImageIndexEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
            "System.Windows.Forms.Design.TreeNodeCollectionEditor, System.Design, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a",
        });

        public DesignerAttributeTests(ITestOutputHelper output)
        {
            _output = output;
        }

        public static IEnumerable<object[]> GetAttributeOfType_TestData(string assembly, Type attributeType)
        {
            foreach (var type in Assembly.Load(assembly).GetTypes())
                foreach (var attribute in type.GetCustomAttributes(attributeType, false))
                    yield return new[] { type, attribute };
        }

        public static IEnumerable<object[]> GetAttributeOfTypeAndProperty_TestData(string assembly, Type attributeType)
        {
            foreach (var type in Assembly.Load(assembly).GetTypes())
            {
                foreach (var attribute in type.GetCustomAttributes(attributeType, false))
                    yield return new[] { type.FullName, attribute };

                foreach (var property in type.GetProperties())
                    foreach (var attribute in property.GetCustomAttributes(attributeType, false))
                        yield return new[] { $"{type.FullName}, property {property.Name}", attribute };
            }
        }

        public static IEnumerable<object[]> GetAttributeWithType_TestData(string assembly, Type attributeType)
        {
            foreach (var type in Assembly.Load(assembly).GetTypes())
                foreach (var attribute in type.GetCustomAttributes(attributeType, false))
                    yield return new[] { type, attribute };
        }

        public static IEnumerable<object[]> GetAttributeWithProperty_TestData(string assembly, Type attributeType)
        {
            foreach (var type in Assembly.Load(assembly).GetTypes())
                foreach (var property in type.GetProperties())
                    foreach (var attribute in property.GetCustomAttributes(attributeType, false))
                        yield return new[] { property, attribute };
        }

        [Theory]
        [MemberData(nameof(GetAttributeOfType_TestData), AssemblyRef_SystemWinforms, typeof(DesignerAttribute))]
        public void DesignerAttributes_DesignerAttribute_TypeExists(Type annotatedType, DesignerAttribute attribute)
        {
            var type = Type.GetType(attribute.DesignerTypeName, false);
            _output.WriteLine($"{annotatedType.FullName}: {attribute.DesignerTypeName} --> {type?.FullName}");

            if (SkipList.Contains(attribute.DesignerTypeName))
            {
                Assert.Null(type);
                return;
            }

            Assert.NotNull(type);
        }

        [Theory]
        [MemberData(nameof(GetAttributeOfType_TestData), AssemblyRef.SystemDrawing, typeof(DesignerSerializerAttribute))]
        [MemberData(nameof(GetAttributeOfType_TestData), AssemblyRef_SystemWinforms, typeof(DesignerSerializerAttribute))]
        [MemberData(nameof(GetAttributeOfType_TestData), AssemblyRef_SystemWinformsDesign, typeof(DesignerSerializerAttribute))]
        public void DesignerAttributes_DesignerSerializerAttribute_TypeExists(Type annotatedType, DesignerSerializerAttribute attribute)
        {
            var type = Type.GetType(attribute.SerializerTypeName, false);
            _output.WriteLine($"{annotatedType.FullName}: {attribute.SerializerTypeName} --> {type?.FullName}");

            if (SkipList.Contains(attribute.SerializerTypeName))
            {
                Assert.Null(type);
                return;
            }

            Assert.NotNull(type);
        }

        [Theory]
        [MemberData(nameof(GetAttributeWithType_TestData), AssemblyRef_SystemWinforms, typeof(DefaultPropertyAttribute))]
        public void DesignerAttributes_DefaultPropertyAttribute_PropertyExists(Type type, DefaultPropertyAttribute attribute)
        {
            var propertyInfo = type.GetProperty(attribute.Name);
            _output.WriteLine($"{type.FullName}: {attribute.Name} --> {propertyInfo?.Name}");

            Assert.NotNull(propertyInfo);
        }

        [Theory]
        [MemberData(nameof(GetAttributeWithType_TestData), AssemblyRef_SystemWinforms, typeof(DefaultBindingPropertyAttribute))]
        public void DesignerAttributes_DefaultBindingPropertyAttribute_PropertyExists(Type type, DefaultBindingPropertyAttribute attribute)
        {
            var propertyInfo = type.GetProperty(attribute.Name);
            _output.WriteLine($"{type.FullName}: {attribute.Name} --> {propertyInfo?.Name}");

            Assert.NotNull(propertyInfo);
        }

        [Theory]
        [MemberData(nameof(GetAttributeWithType_TestData), AssemblyRef_SystemWinforms, typeof(DefaultEventAttribute))]
        public void DesignerAttributes_DefaultEventAttribute_EventExists(Type type, DefaultEventAttribute attribute)
        {
            var eventInfo = type.GetEvent(attribute.Name);
            _output.WriteLine($"{type.FullName}: {attribute.Name} --> {eventInfo?.Name}");

            Assert.NotNull(eventInfo);
        }

        [Theory]
        [MemberData(nameof(GetAttributeOfTypeAndProperty_TestData), AssemblyRef.SystemDrawing, typeof(TypeConverterAttribute))]
        [MemberData(nameof(GetAttributeOfTypeAndProperty_TestData), AssemblyRef_SystemWinforms, typeof(TypeConverterAttribute))]
        public void DesignerAttributes_TypeConverterAttribute_TypeExists(string subject, TypeConverterAttribute attribute)
        {
            var type = Type.GetType(attribute.ConverterTypeName, false);
            _output.WriteLine($"{subject}: {attribute.ConverterTypeName} --> {type?.Name}");

            if (SkipList.Contains(attribute.ConverterTypeName))
            {
                Assert.Null(type);
                return;
            }

            Assert.NotNull(type);
        }

        [Theory]
        [MemberData(nameof(GetAttributeOfTypeAndProperty_TestData), AssemblyRef.SystemDrawing, typeof(EditorAttribute))]
        [MemberData(nameof(GetAttributeOfTypeAndProperty_TestData), AssemblyRef_SystemWinforms, typeof(EditorAttribute))]
        public void DesignerAttributes_EditorAttribute_TypeExists(string subject, EditorAttribute attribute)
        {
            var type = Type.GetType(attribute.EditorTypeName, false);
            _output.WriteLine($"{subject}: {attribute.EditorTypeName} --> {type?.Name}");

            if (SkipList.Contains(attribute.EditorTypeName))
            {
                Assert.Null(type);
                return;
            }

            Assert.NotNull(type);
        }
    }
}
