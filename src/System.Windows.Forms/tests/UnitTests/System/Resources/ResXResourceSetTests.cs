// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Linq;
using System.Text;
using Xunit;

namespace System.Resources.Tests
{
    // NB: doesn't require thread affinity
	// Unit Tests for System.Resources.ResXResourceSet
    public class ResXResourceSetTests : IClassFixture<ThreadExceptionFixture>
    {
        #region Resource names and contents

        private const string c_SomeMissingTest1_Name = "SomeMissingTest1";

        private const string c_MetaDataTest1_Name = "SomeMetadataTest1";
        private const string c_MetaDataTest1_Text = "Some text for MetaData 1 node";

        private const string c_MetaDataTest2_Name = "SomeMetadataTest2";
        private const string c_MetaDataTest2_Text = "Some text for MetaData 2 node";

        private const string c_DataTest1_Name = "SomeDataTest1";
        private const string c_DataTest1_Text = "Some text for Data 1 node";

        private const string c_DataTest2_Name = "SomeDataTest2";
        private const string c_DataTest2_Text = "Some text for Data 2 node";

        private const string c_IconError_Name = "Error";
        private const string c_IconError_Location = @"TestResources\Files\Error.ico";

        private const string c_BitmapErrorControl_Name = "ErrorControl";
        private const string c_BitmapErrorControl_Location = @"TestResources\Files\ErrorControl.bmp";

        private const string c_AsciiText_Name = "text.ansi";
        private const string c_AsciiText_Text = "Text";

        private const string c_Utf8Text_Name = "text.utf8";
        private const string c_Utf8Text_Text = "Привет";

        #endregion

        [Fact]
        public void ResXResourceSet_FileConstructor_SWF1_0()
        {
            ResXResourceSet_TestFile("Test_SWF1_0.resx");
        }

        [Fact]
        public void ResXResourceSet_StreamConstructor_SWF1_0()
        {
            ResXResourceSet_TestStream("Test_SWF1_0.resx");
        }

        [Fact]
        public void ResXResourceSet_FileConstructor_SWF2_0()
        {
            ResXResourceSet_TestFile("Test_SWF2_0.resx");
        }

        [Fact]
        public void ResXResourceSet_StreamConstructor_SWF2_0()
        {
            ResXResourceSet_TestStream("Test_SWF2_0.resx");
        }

        [Fact]
        public void ResXResourceSet_FileConstructor_SWF4_0()
        {
            ResXResourceSet_TestFile("Test_SWF4_0.resx");
        }

        [Fact]
        public void ResXResourceSet_StreamConstructor_SWF4_0()
        {
            ResXResourceSet_TestStream("Test_SWF4_0.resx");
        }

        [Fact]
        public void ResXResourceSet_FileConstructor_SWF42_42()
        {
            ResXResourceSet_TestFile("Test_SWF42_42.resx");
        }

        [Fact]
        public void ResXResourceSet_StreamConstructor_SWF42_42()
        {
            ResXResourceSet_TestStream("Test_SWF42_42.resx");
        }

        private void ResXResourceSet_TestFile(string fileName)
        {
            Assert.True(System.IO.File.Exists(fileName), $@"RESX file ""{fileName}"" not found, make sure it's in the root folder of the unit test project");

            System.Resources.ResXResourceSet resxFile = new System.Resources.ResXResourceSet(fileName);

            Assert.NotNull(resxFile);

            ResXResourceSet_TestResX(resxFile);
        }

        private void ResXResourceSet_TestStream(string fileName)
        {
            Assert.True(System.IO.File.Exists(fileName), $@"RESX file ""{fileName}"" not found, make sure it's in the root folder of the unit test project");

            using(FileStream fs = new FileStream(fileName, FileMode.Open))
            {
                System.Resources.ResXResourceSet resxFile = new System.Resources.ResXResourceSet(fs);

                Assert.NotNull(resxFile);

                ResXResourceSet_TestResX(resxFile);
            }
        }

        private void ResXResourceSet_TestResX(System.Resources.ResXResourceSet resxFile)
        {
            #region Test for missing string

            string strResX_NotFound_Normal = resxFile.GetString(c_SomeMissingTest1_Name);
            Assert.Null(strResX_NotFound_Normal);

            string strResX_NotFound_IgnoreCase = resxFile.GetString(c_SomeMissingTest1_Name, true);
            Assert.Null(strResX_NotFound_IgnoreCase);

            string strResX_NotFound_NotIgnoreCase = resxFile.GetString(c_SomeMissingTest1_Name, false);
            Assert.Null(strResX_NotFound_NotIgnoreCase);

            #endregion

            #region Test for meta data 1 string

            string strResX_MetaData1_Found_Normal = resxFile.GetString(c_MetaDataTest1_Name);
            Assert.NotNull(strResX_MetaData1_Found_Normal);
            Assert.Equal(c_MetaDataTest1_Text, strResX_MetaData1_Found_Normal);

            string strResX_MetaData1_Missing_Normal = resxFile.GetString(c_MetaDataTest1_Name.ToLower());
            Assert.Null(strResX_MetaData1_Missing_Normal);

            string strResX_MetaData1_Found_IgnoreCase = resxFile.GetString(c_MetaDataTest1_Name.ToLower(), true);
            Assert.NotNull(strResX_MetaData1_Found_IgnoreCase);
            Assert.Equal(c_MetaDataTest1_Text, strResX_MetaData1_Found_IgnoreCase);

            string strResX_MetaData1_Found_NotIgnoreCase = resxFile.GetString(c_MetaDataTest1_Name, false);
            Assert.NotNull(strResX_MetaData1_Found_NotIgnoreCase);
            Assert.Equal(c_MetaDataTest1_Text, strResX_MetaData1_Found_NotIgnoreCase);

            string strResX_MetaData1_Missing_NotIgnoreCase = resxFile.GetString(c_MetaDataTest1_Name.ToLower(), false);
            Assert.Null(strResX_MetaData1_Missing_NotIgnoreCase);

            object objResX_MetaData1_Found_Normal = resxFile.GetObject(c_MetaDataTest1_Name);
            Assert.NotNull(objResX_MetaData1_Found_Normal);
            Assert.IsType<string>(objResX_MetaData1_Found_Normal);
            Assert.Equal(c_MetaDataTest1_Text, objResX_MetaData1_Found_Normal);

            object objResX_MetaData1_Missing_Normal = resxFile.GetObject(c_MetaDataTest1_Name.ToLower());
            Assert.Null(objResX_MetaData1_Missing_Normal);

            object objResX_MetaData1_Found_IgnoreCase = resxFile.GetObject(c_MetaDataTest1_Name.ToLower(), true);
            Assert.NotNull(objResX_MetaData1_Found_IgnoreCase);
            Assert.IsType<string>(objResX_MetaData1_Found_IgnoreCase);
            Assert.Equal(c_MetaDataTest1_Text, objResX_MetaData1_Found_IgnoreCase);

            object objResX_MetaData1_Found_NotIgnoreCase = resxFile.GetObject(c_MetaDataTest1_Name, false);
            Assert.NotNull(objResX_MetaData1_Found_NotIgnoreCase);
            Assert.IsType<string>(objResX_MetaData1_Found_NotIgnoreCase);
            Assert.Equal(c_MetaDataTest1_Text, objResX_MetaData1_Found_NotIgnoreCase);

            object objResX_MetaData1_Missing_NotIgnoreCase = resxFile.GetObject(c_MetaDataTest1_Name.ToLower(), false);
            Assert.Null(objResX_MetaData1_Missing_NotIgnoreCase);

            #endregion

            #region Test for meta data 2 string

            string strResX_MetaData2_Found_Normal = resxFile.GetString(c_MetaDataTest2_Name);
            Assert.NotNull(strResX_MetaData2_Found_Normal);
            Assert.Equal(c_MetaDataTest2_Text, strResX_MetaData2_Found_Normal);

            string strResX_MetaData2_Missing_Normal = resxFile.GetString(c_MetaDataTest2_Name.ToLower());
            Assert.Null(strResX_MetaData2_Missing_Normal);

            string strResX_MetaData2_Found_IgnoreCase = resxFile.GetString(c_MetaDataTest2_Name.ToLower(), true);
            Assert.NotNull(strResX_MetaData2_Found_IgnoreCase);
            Assert.Equal(c_MetaDataTest2_Text, strResX_MetaData2_Found_IgnoreCase);

            string strResX_MetaData2_Found_NotIgnoreCase = resxFile.GetString(c_MetaDataTest2_Name, false);
            Assert.NotNull(strResX_MetaData2_Found_NotIgnoreCase);
            Assert.Equal(c_MetaDataTest2_Text, strResX_MetaData2_Found_NotIgnoreCase);

            string strResX_MetaData2_Missing_NotIgnoreCase = resxFile.GetString(c_MetaDataTest2_Name.ToLower(), false);
            Assert.Null(strResX_MetaData2_Missing_NotIgnoreCase);

            object objResX_MetaData2_Found_Normal = resxFile.GetObject(c_MetaDataTest2_Name);
            Assert.NotNull(objResX_MetaData2_Found_Normal);
            Assert.IsType<string>(objResX_MetaData2_Found_Normal);
            Assert.Equal(c_MetaDataTest2_Text, objResX_MetaData2_Found_Normal);

            object objResX_MetaData2_Missing_Normal = resxFile.GetObject(c_MetaDataTest2_Name.ToLower());
            Assert.Null(objResX_MetaData2_Missing_Normal);

            object objResX_MetaData2_Found_IgnoreCase = resxFile.GetObject(c_MetaDataTest2_Name.ToLower(), true);
            Assert.NotNull(objResX_MetaData2_Found_IgnoreCase);
            Assert.IsType<string>(objResX_MetaData2_Found_IgnoreCase);
            Assert.Equal(c_MetaDataTest2_Text, objResX_MetaData2_Found_IgnoreCase);

            object objResX_MetaData2_Found_NotIgnoreCase = resxFile.GetObject(c_MetaDataTest2_Name, false);
            Assert.NotNull(objResX_MetaData2_Found_NotIgnoreCase);
            Assert.IsType<string>(objResX_MetaData2_Found_NotIgnoreCase);
            Assert.Equal(c_MetaDataTest2_Text, objResX_MetaData2_Found_NotIgnoreCase);

            object objResX_MetaData2_Missing_NotIgnoreCase = resxFile.GetObject(c_MetaDataTest2_Name.ToLower(), false);
            Assert.Null(objResX_MetaData2_Missing_NotIgnoreCase);

            #endregion

            #region Test for data 1 string

            string strResX_Data1_Found_Normal = resxFile.GetString(c_DataTest1_Name);
            Assert.NotNull(strResX_Data1_Found_Normal);
            Assert.Equal(c_DataTest1_Text, strResX_Data1_Found_Normal);

            string strResX_Data1_Missing_Normal = resxFile.GetString(c_DataTest1_Name.ToLower());
            Assert.Null(strResX_Data1_Missing_Normal);

            string strResX_Data1_Found_IgnoreCase = resxFile.GetString(c_DataTest1_Name.ToLower(), true);
            Assert.NotNull(strResX_Data1_Found_IgnoreCase);
            Assert.Equal(c_DataTest1_Text, strResX_Data1_Found_IgnoreCase);

            string strResX_Data1_Found_NotIgnoreCase = resxFile.GetString(c_DataTest1_Name, false);
            Assert.NotNull(strResX_Data1_Found_NotIgnoreCase);
            Assert.Equal(c_DataTest1_Text, strResX_Data1_Found_NotIgnoreCase);

            string strResX_Data1_Missing_NotIgnoreCase = resxFile.GetString(c_DataTest1_Name.ToLower(), false);
            Assert.Null(strResX_Data1_Missing_NotIgnoreCase);

            object objResX_Data1_Found_Normal = resxFile.GetObject(c_DataTest1_Name);
            Assert.NotNull(objResX_Data1_Found_Normal);
            Assert.IsType<string>(objResX_Data1_Found_Normal);
            Assert.Equal(c_DataTest1_Text, objResX_Data1_Found_Normal);

            object objResX_Data1_Missing_Normal = resxFile.GetObject(c_DataTest1_Name.ToLower());
            Assert.Null(objResX_Data1_Missing_Normal);

            object objResX_Data1_Found_IgnoreCase = resxFile.GetObject(c_DataTest1_Name.ToLower(), true);
            Assert.NotNull(objResX_Data1_Found_IgnoreCase);
            Assert.IsType<string>(objResX_Data1_Found_IgnoreCase);
            Assert.Equal(c_DataTest1_Text, objResX_Data1_Found_IgnoreCase);

            object objResX_Data1_Found_NotIgnoreCase = resxFile.GetObject(c_DataTest1_Name, false);
            Assert.NotNull(objResX_Data1_Found_NotIgnoreCase);
            Assert.IsType<string>(objResX_Data1_Found_NotIgnoreCase);
            Assert.Equal(c_DataTest1_Text, objResX_Data1_Found_NotIgnoreCase);

            object objResX_Data1_Missing_NotIgnoreCase = resxFile.GetObject(c_DataTest1_Name.ToLower(), false);
            Assert.Null(objResX_Data1_Missing_NotIgnoreCase);

            #endregion

            #region Test for data 2 string

            string strResX_Data2_Found_Normal = resxFile.GetString(c_DataTest2_Name);
            Assert.NotNull(strResX_Data2_Found_Normal);
            Assert.Equal(c_DataTest2_Text, strResX_Data2_Found_Normal);

            string strResX_Data2_Missing_Normal = resxFile.GetString(c_DataTest2_Name.ToLower());
            Assert.Null(strResX_Data2_Missing_Normal);

            string strResX_Data2_Found_IgnoreCase = resxFile.GetString(c_DataTest2_Name.ToLower(), true);
            Assert.NotNull(strResX_Data2_Found_IgnoreCase);
            Assert.Equal(c_DataTest2_Text, strResX_Data2_Found_IgnoreCase);

            string strResX_Data2_Found_NotIgnoreCase = resxFile.GetString(c_DataTest2_Name, false);
            Assert.NotNull(strResX_Data2_Found_NotIgnoreCase);
            Assert.Equal(c_DataTest2_Text, strResX_Data2_Found_NotIgnoreCase);

            string strResX_Data2_Missing_NotIgnoreCase = resxFile.GetString(c_DataTest2_Name.ToLower(), false);
            Assert.Null(strResX_Data2_Missing_NotIgnoreCase);

            object objResX_Data2_Found_Normal = resxFile.GetObject(c_DataTest2_Name);
            Assert.NotNull(objResX_Data2_Found_Normal);
            Assert.IsType<string>(objResX_Data2_Found_Normal);
            Assert.Equal(c_DataTest2_Text, objResX_Data2_Found_Normal);

            object objResX_Data2_Missing_Normal = resxFile.GetObject(c_DataTest2_Name.ToLower());
            Assert.Null(objResX_Data2_Missing_Normal);

            object objResX_Data2_Found_IgnoreCase = resxFile.GetObject(c_DataTest2_Name.ToLower(), true);
            Assert.NotNull(objResX_Data2_Found_IgnoreCase);
            Assert.IsType<string>(objResX_Data2_Found_IgnoreCase);
            Assert.Equal(c_DataTest2_Text, objResX_Data2_Found_IgnoreCase);

            object objResX_Data2_Found_NotIgnoreCase = resxFile.GetObject(c_DataTest2_Name, false);
            Assert.NotNull(objResX_Data2_Found_NotIgnoreCase);
            Assert.IsType<string>(objResX_Data2_Found_NotIgnoreCase);
            Assert.Equal(c_DataTest2_Text, objResX_Data2_Found_NotIgnoreCase);

            object objResX_Data2_Missing_NotIgnoreCase = resxFile.GetObject(c_DataTest2_Name.ToLower(), false);
            Assert.Null(objResX_Data2_Missing_NotIgnoreCase);

            #endregion

            #region Test for Error.ico Icon file

            Assert.Throws<System.InvalidOperationException>(() => resxFile.GetString(c_IconError_Name));
            Assert.Throws<System.InvalidOperationException>(() => resxFile.GetString(c_IconError_Name.ToLower(), true));

            string strResX_IconData_Missing_Normal = resxFile.GetString(c_IconError_Name.ToLower());
            Assert.Null(strResX_IconData_Missing_Normal);

            string strResX_IconData_Missing_NotIgnoreCase = resxFile.GetString(c_IconError_Name.ToLower(), false);
            Assert.Null(strResX_IconData_Missing_NotIgnoreCase);

            object objResX_IconData_Found_Normal = resxFile.GetObject(c_IconError_Name);
            Assert.NotNull(objResX_IconData_Found_Normal);
            Assert.IsType<System.Drawing.Icon>(objResX_IconData_Found_Normal);
            System.Drawing.Icon icnResX_IconData_Found_Normal = objResX_IconData_Found_Normal as System.Drawing.Icon;
            Assert.NotNull(icnResX_IconData_Found_Normal);

            byte[] arrIconBytesExpected = File.ReadAllBytes(c_IconError_Location);
            byte[] arrIconBytesActual;

            using (MemoryStream msIconActual = new MemoryStream())
            {
                icnResX_IconData_Found_Normal.Save(msIconActual);
                arrIconBytesActual = msIconActual.ToArray();
                msIconActual.Close();
            }

            Assert.True(arrIconBytesExpected.SequenceEqual(arrIconBytesActual), $@"Content of icon file ""{c_IconError_Location}"" not equal to content of icon file referenced in resource file");

            #endregion

            #region Test for ErrorControl.bmp Bitmap file

            Assert.Throws<System.InvalidOperationException>(() => resxFile.GetString(c_BitmapErrorControl_Name));
            Assert.Throws<System.InvalidOperationException>(() => resxFile.GetString(c_BitmapErrorControl_Name.ToLower(), true));

            string strResX_BitmapData_Missing_Normal = resxFile.GetString(c_BitmapErrorControl_Name.ToLower());
            Assert.Null(strResX_BitmapData_Missing_Normal);

            string strResX_BitmapData_Missing_NotIgnoreCase = resxFile.GetString(c_BitmapErrorControl_Name.ToLower(), false);
            Assert.Null(strResX_BitmapData_Missing_NotIgnoreCase);

            object objResX_BitmapData_Found_Normal = resxFile.GetObject(c_BitmapErrorControl_Name);
            Assert.NotNull(objResX_BitmapData_Found_Normal);
            Assert.IsType<System.Drawing.Bitmap>(objResX_BitmapData_Found_Normal);
            System.Drawing.Bitmap bmpResX_BitmapData_Found_Normal = objResX_BitmapData_Found_Normal as System.Drawing.Bitmap;
            Assert.NotNull(bmpResX_BitmapData_Found_Normal);

            System.Drawing.ImageConverter icBitmap = new System.Drawing.ImageConverter();
            System.Drawing.Bitmap bmpExpected = new Drawing.Bitmap(c_BitmapErrorControl_Location);

            byte[] arrBitmapBytesExpected = (byte[])icBitmap.ConvertTo(bmpExpected, typeof(byte[]));
            byte[] arrBitmapBytesActual = (byte[])icBitmap.ConvertTo(bmpResX_BitmapData_Found_Normal, typeof(byte[]));

            Assert.True(arrBitmapBytesExpected.SequenceEqual(arrBitmapBytesActual), $@"Content of bitmap file ""{c_BitmapErrorControl_Location}"" not equal to content of bitmap file referenced in resource file");

            #endregion

            #region Test for text.ansi.txt Text file

            string strResX_AsciiText_Missing_Normal = resxFile.GetString(c_AsciiText_Name.ToUpper());
            Assert.Null(strResX_AsciiText_Missing_Normal);

            string strResX_AsciiText_Missing_NotIgnoreCase = resxFile.GetString(c_AsciiText_Name.ToUpper(), false);
            Assert.Null(strResX_AsciiText_Missing_NotIgnoreCase);

            string strResX_AsciiText_Found_Normal = resxFile.GetString(c_AsciiText_Name);
            Assert.NotNull(strResX_AsciiText_Found_Normal);
            Assert.Equal(c_AsciiText_Text, strResX_AsciiText_Found_Normal);

            string strResX_AsciiText_Found_IgnoreCase = resxFile.GetString(c_AsciiText_Name.ToUpper(), true);
            Assert.NotNull(strResX_AsciiText_Found_IgnoreCase);
            Assert.Equal(c_AsciiText_Text, strResX_AsciiText_Found_IgnoreCase);

            object objResX_AsciiText_Missing_Normal = resxFile.GetObject(c_AsciiText_Name.ToUpper());
            Assert.Null(objResX_AsciiText_Missing_Normal);

            object objResX_AsciiText_Missing_NotIgnoreCase = resxFile.GetObject(c_AsciiText_Name.ToUpper(), false);
            Assert.Null(objResX_AsciiText_Missing_NotIgnoreCase);

            object objResX_AsciiText_Found_Normal = resxFile.GetObject(c_AsciiText_Name);
            Assert.NotNull(objResX_AsciiText_Found_Normal);
            Assert.IsType<string>(objResX_AsciiText_Found_Normal);
            Assert.Equal(c_AsciiText_Text, objResX_AsciiText_Found_Normal);

            object objResX_AsciiText_Found_IgnoreCase = resxFile.GetObject(c_AsciiText_Name.ToUpper(), true);
            Assert.NotNull(objResX_AsciiText_Found_IgnoreCase);
            Assert.IsType<string>(objResX_AsciiText_Found_IgnoreCase);
            Assert.Equal(c_AsciiText_Text, objResX_AsciiText_Found_IgnoreCase);

            #endregion

            #region Test for text.utf8.txt Text file

            string strResX_Utf8Text_Missing_Normal = resxFile.GetString(c_Utf8Text_Name.ToUpper());
            Assert.Null(strResX_Utf8Text_Missing_Normal);

            string strResX_Utf8Text_Missing_NotIgnoreCase = resxFile.GetString(c_Utf8Text_Name.ToUpper(), false);
            Assert.Null(strResX_Utf8Text_Missing_NotIgnoreCase);

            string strResX_Utf8Text_Found_Normal = resxFile.GetString(c_Utf8Text_Name);
            Assert.NotNull(strResX_Utf8Text_Found_Normal);
            Assert.Equal(c_Utf8Text_Text, strResX_Utf8Text_Found_Normal);

            string strResX_Utf8Text_Found_IgnoreCase = resxFile.GetString(c_Utf8Text_Name.ToUpper(), true);
            Assert.NotNull(strResX_Utf8Text_Found_IgnoreCase);
            Assert.Equal(c_Utf8Text_Text, strResX_Utf8Text_Found_IgnoreCase);

            object objResX_Utf8Text_Missing_Normal = resxFile.GetObject(c_Utf8Text_Name.ToUpper());
            Assert.Null(objResX_Utf8Text_Missing_Normal);

            object objResX_Utf8Text_Missing_NotIgnoreCase = resxFile.GetObject(c_Utf8Text_Name.ToUpper(), false);
            Assert.Null(objResX_Utf8Text_Missing_NotIgnoreCase);

            object objResX_Utf8Text_Found_Normal = resxFile.GetObject(c_Utf8Text_Name);
            Assert.NotNull(objResX_Utf8Text_Found_Normal);
            Assert.IsType<string>(objResX_Utf8Text_Found_Normal);
            Assert.Equal(c_Utf8Text_Text, objResX_Utf8Text_Found_Normal);

            object objResX_Utf8Text_Found_IgnoreCase = resxFile.GetObject(c_Utf8Text_Name.ToUpper(), true);
            Assert.NotNull(objResX_Utf8Text_Found_IgnoreCase);
            Assert.IsType<string>(objResX_Utf8Text_Found_IgnoreCase);
            Assert.Equal(c_Utf8Text_Text, objResX_Utf8Text_Found_IgnoreCase);

            #endregion

            #region Test for Reader/Writer types

            Type tpReaderExpected = typeof(ResXResourceReader);
            Type tpReaderActual = resxFile.GetDefaultReader();
            Assert.Equal(tpReaderExpected, tpReaderActual);

            Type tpWriterExpected = typeof(ResXResourceWriter);
            Type tpWriterActual = resxFile.GetDefaultWriter();
            Assert.Equal(tpWriterExpected, tpWriterActual);

            #endregion
        }
    }
}
