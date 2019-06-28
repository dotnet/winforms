// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Text;
using Xunit;

namespace System.Resources.Tests
{
    public class ResXFileRefTests
    {
        [Fact]
        public void ResXFileRef_Constructor()
        {
            var fileName = "SomeFile";
            var typeName = "SomeType";

            var fileRef = new ResXFileRef(fileName, typeName);

            Assert.Equal(fileName, fileRef.FileName);
            Assert.Equal(typeName, fileRef.TypeName);
            Assert.Null(fileRef.TextFileEncoding);
        }

        [Fact]
        public void ResXFileRef_EncodingConstructor()
        {
            var fileName = "SomeFile";
            var typeName = "SomeType";
            Encoding encoding = Encoding.Default;

            var fileRef = new ResXFileRef(fileName, typeName, encoding);

            Assert.Equal(fileName, fileRef.FileName);
            Assert.Equal(typeName, fileRef.TypeName);
            Assert.Equal(encoding, fileRef.TextFileEncoding);
        }
    }
}
