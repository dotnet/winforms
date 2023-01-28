// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Drawing;
using Xunit;

namespace System.Resources.Tests
{
    // NB: doesn't require thread affinity
    public class ResxDataNodeTests : IClassFixture<ThreadExceptionFixture>
    {
        [Fact]
        public void ResxDataNode_ResXFileRefConstructor()
        {
            var nodeName = "Node";
            var fileRef = new ResXFileRef(string.Empty, string.Empty);
            var dataNode = new ResXDataNode(nodeName, fileRef);

            Assert.Equal(nodeName, dataNode.Name);
            Assert.Same(fileRef, dataNode.FileRef);
        }

        [Fact]
        public void ResxDataNode_GetValue_ByteArray_FromDataNodeInfo_Framework()
        {
            using Bitmap bitmap = new(10, 10);
            var converter = TypeDescriptor.GetConverter(bitmap);
            ResXDataNode temp = new("test", converter.ConvertTo(bitmap, typeof(byte[])));
            var dataNodeInfo = temp.GetDataNodeInfo();
            ResXDataNode dataNode = new(dataNodeInfo, basePath: null);

            var bitmapBytes = dataNode.GetValue(typeResolver: null);
            Bitmap result = Assert.IsType<Bitmap>(converter.ConvertFrom(bitmapBytes));
            Assert.Equal(bitmap.Size, result.Size);
        }

        [Fact]
        public void ResxDataNode_GetValue_ByteArray_FromDataNodeInfo_Core()
        {
            using Bitmap bitmap = new(10, 10);
            var converter = TypeDescriptor.GetConverter(bitmap);
            ResXDataNode temp = new("test", converter.ConvertTo(bitmap, typeof(byte[])));
            var dataNodeInfo = temp.GetDataNodeInfo();
            dataNodeInfo.TypeName = "System.Byte[], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089";
            ResXDataNode dataNode = new(dataNodeInfo, basePath: null);

            var bitmapBytes = dataNode.GetValue(typeResolver: null);
            var result = Assert.IsType<Bitmap>(converter.ConvertFrom(bitmapBytes));
            Assert.Equal(bitmap.Size, result.Size);
        }

        [Fact]
        public void ResxDataNode_GetValue_Null_FromDataNodeInfo()
        {
            ResXDataNode temp = new("test", value: null);
            var dataNodeInfo = temp.GetDataNodeInfo();
            ResXDataNode dataNode = new(dataNodeInfo, basePath: null);

            var valueNull = dataNode.GetValue(typeResolver: null);
            Assert.Null(valueNull);
        }

        [Fact]
        public void ResxDataNode_GetValue_String_FromDataNodeInfo()
        {
            ResXDataNode temp = new("testName", "test");
            var dataNodeInfo = temp.GetDataNodeInfo();
            ResXDataNode dataNode = new(dataNodeInfo, basePath: null);

            var valueString = dataNode.GetValue(typeResolver: null);
            Assert.Equal("test", valueString);
        }
    }
}
