// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms.Tests.TestResources;
using Windows.Win32.System.Ole;
using WMPLib;
using Xunit;
using static Interop;

namespace System.Windows.Forms.ComponentModel.Com2Interop.Tests
{
    public class ComNativeDescriptorTests : IClassFixture<ThreadExceptionFixture>
    {
        [StaFact]
        public void ComNativeDescriptor_GetProperties_FromIPictureDisp()
        {
            using Bitmap bitmap = new(10, 20);
            object iPictureDisp = IPictureDisp.CreateObjectFromImage(bitmap);
            Assert.NotNull(iPictureDisp);

            var properties = TypeDescriptor.GetProperties(iPictureDisp);
            Assert.NotNull(properties);
            Assert.Equal(5, properties.Count);

            var handleProperty = properties["Handle"];
            Assert.IsType<Com2PropertyDescriptor>(handleProperty);
            Assert.True(handleProperty.IsReadOnly);
            Assert.Equal("Misc", handleProperty.Category);
            Assert.False(handleProperty.IsLocalizable);
            Assert.Equal("Interop+Oleaut32+IDispatch", handleProperty.ComponentType.FullName);

            // OLE_HANDLE
            Assert.Equal("System.Int32", handleProperty.PropertyType.FullName);

            Assert.Equal(DesignerSerializationVisibility.Visible, handleProperty.SerializationVisibility);
            Assert.False(handleProperty.CanResetValue(iPictureDisp));
            Assert.False(handleProperty.ShouldSerializeValue(iPictureDisp));
            Assert.NotEqual(0, handleProperty.GetValue(iPictureDisp));

            var converter = (Com2ExtendedTypeConverter)handleProperty.Converter;
            Assert.IsAssignableFrom<Int32Converter>(converter.InnerConverter);

            var typeProperty = properties["Type"];
            Assert.NotNull(typeProperty);
            Assert.True(typeProperty.IsReadOnly);
            Assert.Equal("Misc", typeProperty.Category);
            Assert.False(typeProperty.IsLocalizable);
            Assert.Equal("Interop+Oleaut32+IDispatch", typeProperty.ComponentType.FullName);

            Assert.Equal("System.Int16", typeProperty.PropertyType.FullName);
            Assert.Equal(PICTYPE.PICTYPE_BITMAP, (PICTYPE)(short)typeProperty.GetValue(iPictureDisp));
        }

        [StaFact]
        public void ComNativeDescriptor_GetProperties_FromIPicture()
        {
            // While we ask for IPicture, the underlying native class also supports IDispatch, so we get support
            // in the type descriptor.
            using Bitmap bitmap = new(10, 20);
            object iPicture = IPicture.CreateObjectFromImage(bitmap);
            Assert.NotNull(iPicture);

            var properties = TypeDescriptor.GetProperties(iPicture);
            Assert.Equal(PICTYPE.PICTYPE_BITMAP, (PICTYPE)(short)properties["Type"].GetValue(iPicture));
        }

        [StaFact]
        public void ComNativeDescriptor_GetProperties_FromActiveXControl()
        {
            Guid guid = typeof(WindowsMediaPlayerClass).GUID;
            HRESULT hr = Ole32.CoCreateInstance(
                in guid,
                IntPtr.Zero,
                Ole32.CLSCTX.INPROC_SERVER,
                in NativeMethods.ActiveX.IID_IUnknown,
                out object mediaPlayer);

            Assert.Equal(HRESULT.S_OK, hr);

            var properties = TypeDescriptor.GetProperties(mediaPlayer);
            Assert.NotNull(properties);
            Assert.Equal(25, properties.Count);

            var urlProperty = properties["URL"];
            Assert.IsType<Com2PropertyDescriptor>(urlProperty);
            Assert.False(urlProperty.IsReadOnly);
            Assert.Equal("Misc", urlProperty.Category);
            Assert.False(urlProperty.IsLocalizable);
            Assert.Equal("Interop+Oleaut32+IDispatch", urlProperty.ComponentType.FullName);

            Assert.Equal("System.String", urlProperty.PropertyType.FullName);

            Assert.Equal(DesignerSerializationVisibility.Visible, urlProperty.SerializationVisibility);
            Assert.False(urlProperty.CanResetValue(mediaPlayer));
            Assert.False(urlProperty.ShouldSerializeValue(mediaPlayer));
            Assert.Equal(string.Empty, urlProperty.GetValue(mediaPlayer));

            urlProperty.SetValue(mediaPlayer, "Movie.mpg");

            // This will be the fully qualified path name, which is based on the current directory.
            Assert.EndsWith("Movie.mpg", (string)urlProperty.GetValue(mediaPlayer));

            var converter = (Com2ExtendedTypeConverter)urlProperty.Converter;
            Assert.IsAssignableFrom<StringConverter>(converter.InnerConverter);
        }

        [StaFact]
        public void ComNativeDescriptor_GetProperties_FromSimpleVBControl()
        {
            if (RuntimeInformation.ProcessArchitecture != Architecture.X86)
            {
                return;
            }

            // Not much to see with this control, but it does exercise a fair amount of code.
            ComClasses.VisualBasicSimpleControl.CreateInstance(out object vbcontrol).ThrowOnFailure();

            var properties = TypeDescriptor.GetProperties(vbcontrol);
            Assert.Empty(properties);

            var events = TypeDescriptor.GetEvents(vbcontrol);
            Assert.Empty(events);

            var attributes = TypeDescriptor.GetAttributes(vbcontrol);
            Assert.Equal(2, attributes.Count);
            BrowsableAttribute browsable = (BrowsableAttribute)attributes[0];
            Assert.True(browsable.Browsable);
            DesignTimeVisibleAttribute visible = (DesignTimeVisibleAttribute)attributes[1];
            Assert.False(visible.Visible);
        }
    }
}
