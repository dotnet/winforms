// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Reflection;
using Xunit;
using Xunit.Abstractions;

namespace System.Windows.Forms.Design.Tests
{
    public class DesignerAttributeTests
    {
        private const string AssemblyRef_SystemWinforms = "System.Windows.Forms, Version=" + FXAssembly.Version + ", Culture=neutral, PublicKeyToken=" + AssemblyRef.MicrosoftPublicKey;
        private const string AssemblyRef_SystemWinformsDesign = "System.Windows.Forms.Design, Version=" + FXAssembly.Version + ", Culture=neutral, PublicKeyToken=" + AssemblyRef.MicrosoftPublicKey;
        private readonly ITestOutputHelper _output;

        public DesignerAttributeTests(ITestOutputHelper output)
        {
            _output = output;
        }

        public static IEnumerable<object[]> GetAttributeOfType_TestData(string assembly, Type attributeType)
        {
            foreach (var type in Assembly.Load(assembly).GetTypes())
                foreach (var attribute in type.GetCustomAttributes(attributeType, false))
                    yield return new[] { attribute };
        }

        public static IEnumerable<object[]> GetAttributeOfTypeAndProperty_TestData(string assembly, Type attributeType)
        {
            foreach (var type in Assembly.Load(assembly).GetTypes())
            {
                foreach (var attribute in type.GetCustomAttributes(attributeType, false))
                    yield return new[] { attribute };

                foreach (var property in type.GetProperties())
                    foreach (var attribute in property.GetCustomAttributes(attributeType, false))
                        yield return new[] { attribute };
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
        public void DesignerAttributes_DesignerAttribute_TypeExists(DesignerAttribute attribute)
        {
            var type = Type.GetType(attribute.DesignerTypeName, false);
            _output.WriteLine($"{attribute.DesignerTypeName} --> {type?.FullName}");

            Assert.NotNull(type);
        }

        [Theory]
        [MemberData(nameof(GetAttributeOfType_TestData), AssemblyRef.SystemDrawing, typeof(DesignerSerializerAttribute))]
        [MemberData(nameof(GetAttributeOfType_TestData), AssemblyRef_SystemWinforms, typeof(DesignerSerializerAttribute))]
        [MemberData(nameof(GetAttributeOfType_TestData), AssemblyRef_SystemWinformsDesign, typeof(DesignerSerializerAttribute))]
        public void DesignerAttributes_DesignerSerializerAttribute_TypeExists(DesignerSerializerAttribute attribute)
        {
            var type = Type.GetType(attribute.SerializerTypeName, false);
            _output.WriteLine($"{attribute.SerializerTypeName} --> {type?.FullName}");

            Assert.NotNull(type);
        }

        [Theory]
        [MemberData(nameof(GetAttributeWithType_TestData), AssemblyRef_SystemWinforms, typeof(DefaultPropertyAttribute))]
        public void DesignerAttributes_DefaultPropertyAttribute_PropertyExists(Type type, DefaultPropertyAttribute attribute)
        {
            var propertyInfo = type.GetProperty(attribute.Name);
            _output.WriteLine($"{attribute.Name} --> {propertyInfo?.Name}");

            Assert.NotNull(propertyInfo);
        }

        [Theory]
        [MemberData(nameof(GetAttributeWithType_TestData), AssemblyRef_SystemWinforms, typeof(DefaultBindingPropertyAttribute))]
        public void DesignerAttributes_DefaultBindingPropertyAttribute_PropertyExists(Type type, DefaultBindingPropertyAttribute attribute)
        {
            var propertyInfo = type.GetProperty(attribute.Name);
            _output.WriteLine($"{attribute.Name} --> {propertyInfo?.Name}");

            Assert.NotNull(propertyInfo);
        }

        [Theory]
        [MemberData(nameof(GetAttributeWithType_TestData), AssemblyRef_SystemWinforms, typeof(DefaultEventAttribute))]
        public void DesignerAttributes_DefaultEventAttribute_EventExists(Type type, DefaultEventAttribute attribute)
        {
            var eventInfo = type.GetEvent(attribute.Name);
            _output.WriteLine($"{attribute.Name} --> {eventInfo?.Name}");

            Assert.NotNull(eventInfo);
        }

        [Theory]
        [MemberData(nameof(GetAttributeOfTypeAndProperty_TestData), AssemblyRef.SystemDrawing, typeof(TypeConverterAttribute))]
        [MemberData(nameof(GetAttributeOfTypeAndProperty_TestData), AssemblyRef_SystemWinforms, typeof(TypeConverterAttribute))]
        public void DesignerAttributes_TypeConverterAttribute_TypeExists(TypeConverterAttribute attribute)
        {
            var type = Type.GetType(attribute.ConverterTypeName, false);
            _output.WriteLine($"{attribute.ConverterTypeName} --> {type?.Name}");

            Assert.NotNull(type);
        }

        [Theory]
        [MemberData(nameof(GetAttributeOfTypeAndProperty_TestData), AssemblyRef.SystemDrawing, typeof(EditorAttribute))]
        [MemberData(nameof(GetAttributeOfTypeAndProperty_TestData), AssemblyRef_SystemWinforms, typeof(EditorAttribute))]
        public void DesignerAttributes_EditorAttribute_TypeExists(EditorAttribute attribute)
        {
            var type = Type.GetType(attribute.EditorTypeName, false);
            _output.WriteLine($"{attribute.EditorTypeName} --> {type?.Name}");

            Assert.NotNull(type);
        }
    }
}
