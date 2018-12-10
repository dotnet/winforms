// Licensed to the .NET Foundation under one or more agreements.	
// The .NET Foundation licenses this file to you under the MIT license.	
// See the LICENSE file in the project root for more information.	

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class DataStreamFromComStreamTests
    {
        private Stream SerializeOnStream(object obj)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            var memoryStream = new MemoryStream();
            formatter.Serialize(memoryStream, obj);
            memoryStream.Seek(0, SeekOrigin.Begin);
            return memoryStream;
        }

        private object DeserializeFromStream(Stream stream)
        {
            stream.Seek(0, SeekOrigin.Begin);
            BinaryFormatter formatter = new BinaryFormatter();
            var obj = formatter.Deserialize(stream);
            return obj;
        }

        [Fact]
        private void DataStreamFromComStream_Read()
        {
            var sample = Guid.NewGuid();

            BinaryFormatter formatter = new BinaryFormatter();
            using (var memoryStream = SerializeOnStream(sample))
            {
                var istream = (UnsafeNativeMethods.IStream)new UnsafeNativeMethods.ComStreamFromDataStream(memoryStream);
                Stream stream = new DataStreamFromComStream(istream);
                var target = (Guid)formatter.Deserialize(stream);
                Assert.Equal(target, sample);
            }
        }

        [Fact]
        private void DataStreamFromComStream_Write()
        {
            var sample = Guid.NewGuid();

            BinaryFormatter formatter = new BinaryFormatter();
            using (var memoryStream = new MemoryStream())
            {
                var istream = (UnsafeNativeMethods.IStream)new UnsafeNativeMethods.ComStreamFromDataStream(memoryStream);
                Stream stream = new DataStreamFromComStream(istream);
                formatter.Serialize(stream, sample);
                var target = (Guid)DeserializeFromStream(memoryStream);
                Assert.Equal(target, sample);
            }
        }
    }
}
