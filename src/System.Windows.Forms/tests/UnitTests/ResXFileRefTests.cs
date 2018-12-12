using System.Resources;
using System.Text;
using Xunit;

namespace System.Windows.Forms.Tests
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
            var encoding = Encoding.Default;

            var fileRef = new ResXFileRef(fileName, typeName, encoding);

            Assert.Equal(fileName, fileRef.FileName);
            Assert.Equal(typeName, fileRef.TypeName);
            Assert.Equal(encoding, fileRef.TextFileEncoding);
        }
    }
}
