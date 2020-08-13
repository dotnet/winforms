// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Linq;
using Moq;
using Xunit;

namespace System.Windows.Forms.Tests
{
    // NB: doesn't require thread affinity
    public class SpecialFolderEnumConverterTests : IClassFixture<ThreadExceptionFixture>
    {
        [Fact]
        public void SpecialFolderEnumConverter_ConvertFrom_String_Success()
        {
            PropertyDescriptor descriptor = TypeDescriptor.GetProperties(new FolderBrowserDialog()).Find(nameof(FolderBrowserDialog.RootFolder), ignoreCase: false);
            TypeConverter converter = descriptor.Converter;
            Assert.Equal(Environment.SpecialFolder.CommonDocuments, converter.ConvertFrom("CommonDocuments"));
        }

        [Fact]
        public void SpecialFolderEnumConverter_GetStandardValues_Invoke_ReturnsExpected()
        {
            TypeConverter converter = GetNewSpecialFolderEnumConverter();
            var expected = new Environment.SpecialFolder[]
            {
                Environment.SpecialFolder.AdminTools,
                Environment.SpecialFolder.ApplicationData,
                Environment.SpecialFolder.CDBurning,
                Environment.SpecialFolder.CommonAdminTools,
                Environment.SpecialFolder.CommonApplicationData,
                Environment.SpecialFolder.CommonDesktopDirectory,
                Environment.SpecialFolder.CommonDocuments,
                Environment.SpecialFolder.CommonMusic,
                Environment.SpecialFolder.CommonOemLinks,
                Environment.SpecialFolder.CommonPictures,
                Environment.SpecialFolder.CommonProgramFiles,
                Environment.SpecialFolder.CommonProgramFilesX86,
                Environment.SpecialFolder.CommonPrograms,
                Environment.SpecialFolder.CommonStartMenu,
                Environment.SpecialFolder.CommonStartup,
                Environment.SpecialFolder.CommonTemplates,
                Environment.SpecialFolder.CommonVideos,
                Environment.SpecialFolder.Cookies,
                Environment.SpecialFolder.Desktop,
                Environment.SpecialFolder.DesktopDirectory,
                Environment.SpecialFolder.Favorites,
                Environment.SpecialFolder.Fonts,
                Environment.SpecialFolder.History,
                Environment.SpecialFolder.InternetCache,
                Environment.SpecialFolder.LocalApplicationData,
                Environment.SpecialFolder.LocalizedResources,
                Environment.SpecialFolder.MyComputer,
                Environment.SpecialFolder.MyDocuments,
                Environment.SpecialFolder.MyMusic,
                Environment.SpecialFolder.MyPictures,
                Environment.SpecialFolder.MyVideos,
                Environment.SpecialFolder.NetworkShortcuts,
                Environment.SpecialFolder.PrinterShortcuts,
                Environment.SpecialFolder.ProgramFiles,
                Environment.SpecialFolder.ProgramFilesX86,
                Environment.SpecialFolder.Programs,
                Environment.SpecialFolder.Recent,
                Environment.SpecialFolder.Resources,
                Environment.SpecialFolder.SendTo,
                Environment.SpecialFolder.StartMenu,
                Environment.SpecialFolder.Startup,
                Environment.SpecialFolder.System,
                Environment.SpecialFolder.SystemX86,
                Environment.SpecialFolder.Templates,
                Environment.SpecialFolder.UserProfile,
                Environment.SpecialFolder.Windows
            };
            Assert.Equal(expected, converter.GetStandardValues(null).Cast<Environment.SpecialFolder>());

            converter = GetNewSpecialFolderEnumConverter();
            var mockProvider = new Mock<TypeDescriptionProvider>(MockBehavior.Strict);
            mockProvider
                .Setup(p => p.GetReflectionType(typeof(Environment.SpecialFolder), null))
                .Returns(() => typeof(CustomReflectionType));

            TypeDescriptor.AddProvider(mockProvider.Object, typeof(Environment.SpecialFolder));
            Assert.Equal(new Environment.SpecialFolder[] { Environment.SpecialFolder.Personal }, converter.GetStandardValues(null).Cast<Environment.SpecialFolder>());
            TypeDescriptor.RemoveProvider(mockProvider.Object, typeof(Environment.SpecialFolder));
        }

        [Fact]
        public void SpecialFolderEnumConverter_GetStandardValues_NonSpecialFolderType_ReturnsExpected()
        {
            PropertyDescriptor descriptor = TypeDescriptor.GetProperties(new FolderBrowserDialog()).Find(nameof(FolderBrowserDialog.RootFolder), ignoreCase: false);
            TypeConverter converter = (TypeConverter)Activator.CreateInstance(descriptor.Converter.GetType(), new Type[] { typeof(CustomEnum) });
            Assert.Equal(new CustomEnum[] { CustomEnum.Value }, converter.GetStandardValues(null).Cast<CustomEnum>());
        }

        private TypeConverter GetNewSpecialFolderEnumConverter()
        {
            PropertyDescriptor descriptor = TypeDescriptor.GetProperties(new FolderBrowserDialog()).Find(nameof(FolderBrowserDialog.RootFolder), ignoreCase: false);
            return (TypeConverter)Activator.CreateInstance(descriptor.Converter.GetType(), new Type[] { typeof(Environment.SpecialFolder) });
        }

        private class CustomReflectionType
        {
#pragma warning disable CS0649
            public static int Personal;
#pragma warning restore CS0649
        }

        private enum CustomEnum
        {
            Value
        }
    }
}
