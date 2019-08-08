// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Forms;
using System.Resources;
using System.Text;
using System.Collections;
using System.Windows.Forms.IntegrationTests.Common;
using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using System.Threading;

namespace System.Windows.Forms.IntegrationTests.MauiTests
{
    public class MauiResXFileRefTests : ReflectBase
    {

        #region Testcase setup
        public MauiResXFileRefTests(string[] args) : base(args)
        {
            this.BringToForeground();
        }

        [STAThread]
        public static void Main(string[] args)
        {
            Thread.CurrentThread.SetCulture("en-US");
            Application.Run(new MauiResXFileRefTests(args));
        }

        protected override void InitTest(TParams p)
        {
            base.InitTest(p);
        }

        const int maxNum = 100;
        #endregion

        //"Text.txt", typeof(string).AssemblyQualifiedName)

        //==========================================
        // Scenarios
        //==========================================
        #region Scenarios
        //[Scenario("1. Contructor(null, null, null")]
        [Scenario(true)]
        public ScenarioResult Scenario1(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            try
            {
                ResXFileRef fref = new ResXFileRef(null, null, null);
                sr.IncCounters(false, "FAIL: Contructor did not throw an exception: ", p.log);
            }
            catch (ArgumentNullException ex)
            {
                ex.ToString();
                sr.IncCounters(true);
            }
            return sr;
        }

        //[Scenario("2. Contructor(empty, empty, null")]
        [Scenario(true)]
        public ScenarioResult Scenario2(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            try
            {
                ResXFileRef fref = new ResXFileRef(String.Empty, String.Empty, null);
                sr.IncCounters(fref.FileName.Equals(String.Empty), "FAIL: FileName not Empty", p.log);
                sr.IncCounters(fref.TypeName.Equals(String.Empty), "FAIL: TypeName not Empty", p.log);
                sr.IncCounters(fref.TextFileEncoding == null, "FAIL: TextFileEncoding not null", p.log);
            }
            catch (Exception ex)
            {
                sr.IncCounters(false, "FAIL: Contructor threw an exception: " + ex.Message.ToString(), p.log);
            }
            return sr;
        }

        //[Scenario("3. Contructor(empty, empty, some Encoding")]
        [Scenario(true)]
        public ScenarioResult Scenario3(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            try
            {

                ResXFileRef fref = new ResXFileRef(String.Empty, String.Empty, System.Text.Encoding.ASCII);
                sr.IncCounters(fref.FileName == String.Empty, "FAIL: FileName not null", p.log);
                sr.IncCounters(fref.TypeName == String.Empty, "FAIL: TypeName not null", p.log);
                sr.IncCounters(fref.TextFileEncoding.EncodingName.Equals(System.Text.Encoding.ASCII.EncodingName.ToString()), "FAIL: TextFileEncoding not set", p.log);
            }
            catch (Exception ex)
            {
                sr.IncCounters(false, "FAIL: Contructor threw an exception: " + ex.Message.ToString(), p.log);
            }
            return sr;
        }

        //[Scenario("4. Contructor(null, valid typeName, some Encoding")]
        [Scenario(true)]
        public ScenarioResult Scenario4(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            try
            {

                ResXFileRef fref = new ResXFileRef(null, typeof(string).AssemblyQualifiedName, System.Text.Encoding.Unicode);
                sr.IncCounters(false, "FAIL: Contructor did not throw an exception: ", p.log);
            }
            catch (ArgumentNullException ex)
            {
                ex.ToString();
                sr.IncCounters(true);
            }
            return sr;
        }

        //[Scenario("5. Contructor(valid fileName, valid typeName,  some Encoding")]
        [Scenario(true)]
        public ScenarioResult Scenario5(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            String fileName = "D:\\Temp.txt";
            try
            {

                ResXFileRef fref = new ResXFileRef(fileName, typeof(string).AssemblyQualifiedName, System.Text.Encoding.UTF32);
                sr.IncCounters(fref.FileName == fileName, "FAIL: FileName not set", fileName, fref.FileName, p.log);
                sr.IncCounters(fref.TypeName.Equals(typeof(string).AssemblyQualifiedName), "FAIL: TypeName not set", typeof(string).AssemblyQualifiedName, fref.TypeName.ToString(), p.log);
                sr.IncCounters(fref.TextFileEncoding.EncodingName.Equals(System.Text.Encoding.UTF32.EncodingName.ToString()), "FAIL: TextFileEncoding not set", p.log);
            }
            catch (Exception ex)
            {
                sr.IncCounters(false, "FAIL: Contructor threw an exception: " + ex.Message.ToString(), p.log);
            }
            return sr;
        }

        //[Scenario("6. Contructor(valid long fileName, valid typeName, some Encoding")]
        [Scenario(true)]
        public ScenarioResult Scenario6(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            String fileName = "D:\\" + p.ru.GetString(int.MaxValue);
            try
            {

                ResXFileRef fref = new ResXFileRef(fileName, typeof(string).AssemblyQualifiedName, System.Text.Encoding.UTF32);
                sr.IncCounters(fref.FileName == fileName, "FAIL: FileName not set", fileName, fref.FileName, p.log);
                sr.IncCounters(fref.TypeName.Equals(typeof(string).AssemblyQualifiedName), "FAIL: TypeName not set", typeof(string).AssemblyQualifiedName, fref.TypeName.ToString(), p.log);
                sr.IncCounters(fref.TextFileEncoding.EncodingName.Equals(System.Text.Encoding.UTF32.EncodingName.ToString()), "FAIL: TextFileEncoding not set", p.log);
            }
            catch (Exception ex)
            {
                sr.IncCounters(false, "FAIL: Contructor threw an exception: " + ex.Message.ToString(), p.log);
            }
            return sr;
        }

        //[Scenario("7. Contructor(empty string fileName,  valid typeName, some Encoding")]
        [Scenario(true)]
        public ScenarioResult Scenario7(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            String fileName = String.Empty;
            try
            {

                ResXFileRef fref = new ResXFileRef(fileName, typeof(string).AssemblyQualifiedName, System.Text.Encoding.UTF32);
                sr.IncCounters(fref.FileName == fileName, "FAIL: FileName not set", fileName, fref.FileName, p.log);
                sr.IncCounters(fref.TypeName.Equals(typeof(string).AssemblyQualifiedName), "FAIL: TypeName not set", typeof(string).AssemblyQualifiedName, fref.TypeName.ToString(), p.log);
                sr.IncCounters(fref.TextFileEncoding.EncodingName.Equals(System.Text.Encoding.UTF32.EncodingName.ToString()), "FAIL: TextFileEncoding not set", p.log);
            }
            catch (Exception ex)
            {
                sr.IncCounters(false, "FAIL: Contructor threw an exception: " + ex.Message.ToString(), p.log);
            }
            return sr;
        }

        //[Scenario("8. Contructor(valid fileName, empty string typeName, some Encoding")]
        [Scenario(true)]
        public ScenarioResult Scenario8(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            String fileName = "D:\\" + p.ru.GetString(p.ru.GetInt());
            try
            {

                ResXFileRef fref = new ResXFileRef(fileName, String.Empty, System.Text.Encoding.UTF32);
                sr.IncCounters(fref.FileName == fileName, "FAIL: FileName not set", fileName, fref.FileName, p.log);
                sr.IncCounters(fref.TypeName.Equals(String.Empty), "FAIL: TypeName not set", String.Empty, fref.TypeName.ToString(), p.log);
                sr.IncCounters(fref.TextFileEncoding.EncodingName.Equals(System.Text.Encoding.UTF32.EncodingName.ToString()), "FAIL: TextFileEncoding not set", p.log);
            }
            catch (Exception ex)
            {
                sr.IncCounters(false, "FAIL: Contructor threw an exception: " + ex.Message.ToString(), p.log);
            }
            return sr;
        }

        //[Scenario("9. Contructor(valid fileName, fully qualified typeName, some Encoding")]
        [Scenario(true)]
        public ScenarioResult Scenario9(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            String fileName = "D:\\" + p.ru.GetString(p.ru.GetInt());
            try
            {

                ResXFileRef fref = new ResXFileRef(fileName, typeof(string).AssemblyQualifiedName, System.Text.Encoding.UTF32);
                sr.IncCounters(fref.FileName == fileName, "FAIL: FileName not set", fileName, fref.FileName, p.log);
                sr.IncCounters(fref.TypeName.Equals(typeof(string).AssemblyQualifiedName), "FAIL: TypeName not set", typeof(string).AssemblyQualifiedName, fref.TypeName.ToString(), p.log);
                sr.IncCounters(fref.TextFileEncoding.EncodingName.Equals(System.Text.Encoding.UTF32.EncodingName.ToString()), "FAIL: TextFileEncoding not set", p.log);
            }
            catch (Exception ex)
            {
                sr.IncCounters(false, "FAIL: Contructor threw an exception: " + ex.Message.ToString(), p.log);
            }
            return sr;
        }

        //[Scenario("10. Contructor(valid fileName, not fully qualified typeName, some Encoding")]
        [Scenario(true)]
        public ScenarioResult Scenario10(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            String fileName = "D:\\" + p.ru.GetString(p.ru.GetInt());
            try
            {

                ResXFileRef fref = new ResXFileRef(fileName, "Text", System.Text.Encoding.UTF32);
                sr.IncCounters(fref.FileName == fileName, "FAIL: FileName not set", fileName, fref.FileName, p.log);
                sr.IncCounters(fref.TypeName.Equals("Text"), "FAIL: TypeName not set", "Text", fref.TypeName.ToString(), p.log);
                sr.IncCounters(fref.TextFileEncoding.EncodingName.Equals(System.Text.Encoding.UTF32.EncodingName.ToString()), "FAIL: TextFileEncoding not set", p.log);
            }
            catch (Exception ex)
            {
                sr.IncCounters(false, "FAIL: Contructor threw an exception: " + ex.Message.ToString(), p.log);
            }
            return sr;
        }

        //[Scenario("11. Contructor( valid fileName, valid typeName, null")]
        [Scenario(true)]
        public ScenarioResult Scenario11(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            String fileName = "D:\\" + p.ru.GetString(p.ru.GetInt());
            try
            {

                ResXFileRef fref = new ResXFileRef(fileName, typeof(Char).AssemblyQualifiedName, null);
                sr.IncCounters(fref.FileName == fileName, "FAIL: FileName not set", fileName, fref.FileName, p.log);
                sr.IncCounters(fref.TypeName.Equals(typeof(Char).AssemblyQualifiedName), "FAIL: TypeName not set", "Text", fref.TypeName.ToString(), p.log);
                sr.IncCounters(fref.TextFileEncoding == null, "FAIL: TextFileEncoding not null", p.log);
            }
            catch (Exception ex)
            {
                sr.IncCounters(false, "FAIL: Contructor threw an exception: " + ex.Message.ToString(), p.log);
            }
            return sr;
        }

        //[Scenario("12. Contructor(valid fileName, valid typeName, Encoding.default")]
        [Scenario(true)]
        public ScenarioResult Scenario12(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            String fileName = "D:\\" + p.ru.GetString(p.ru.GetInt());
            try
            {

                ResXFileRef fref = new ResXFileRef(fileName, typeof(Char).AssemblyQualifiedName, System.Text.Encoding.Default);
                sr.IncCounters(fref.FileName == fileName, "FAIL: FileName not set", fileName, fref.FileName, p.log);
                sr.IncCounters(fref.TypeName.Equals(typeof(Char).AssemblyQualifiedName), "FAIL: TypeName not set", "Text", fref.TypeName.ToString(), p.log);
                sr.IncCounters(fref.TextFileEncoding.EncodingName.Equals(System.Text.Encoding.Default.EncodingName.ToString()), "FAIL: TextFileEncoding not set", p.log);
            }
            catch (Exception ex)
            {
                sr.IncCounters(false, "FAIL: Contructor threw an exception: " + ex.Message.ToString(), p.log);
            }
            return sr;
        }

        //[Scenario("13. Call ToString when ResXFileRef( null, null, null)")]
        [Scenario(true)]
        public ScenarioResult Scenario13(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            try
            {
                ResXFileRef fref = new ResXFileRef(null, null, null);
                sr.IncCounters(false, "FAIL: Contructor did not throw an exception", p.log);
            }
            catch (ArgumentNullException ex)
            {
                ex.ToString();
                sr.IncCounters(true);
            }
            return sr;
        }

        //[Scenario("14. Call ToString when ResXFileRef( empty, empty, null)")]
        [Scenario(true)]
        public ScenarioResult Scenario14(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            try
            {
                ResXFileRef fref = new ResXFileRef(String.Empty, String.Empty, null);
                sr.IncCounters(fref.ToString() == ";", "FAIL: FileName.ToString", ";", fref.ToString(), p.log);
            }
            catch (Exception ex)
            {
                sr.IncCounters(false, "FAIL: Contructor threw an exception: " + ex.Message.ToString(), p.log);
            }
            return sr;
        }

        //[Scenario("15. Call ToString when ResXFileRef(valid, valid, some Encoding)")]
        [Scenario(true)]
        public ScenarioResult Scenario15(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            String fileName = @"D:\" + p.ru.GetString(100);
            String fixedFileName = checkFileName(fileName);
            String expectedToString = fixedFileName + ";" + typeof(string).AssemblyQualifiedName + ";" + System.Text.Encoding.UTF32.BodyName;

            try
            {
                ResXFileRef fref = new ResXFileRef(fileName, typeof(string).AssemblyQualifiedName, System.Text.Encoding.UTF32);
                sr.IncCounters(expectedToString, fref.ToString(), "FAIL: FileName.ToString", p.log);
            }
            catch (Exception ex)
            {
                sr.IncCounters(false, "FAIL: Contructor threw an exception: " + ex.Message.ToString(), p.log);
            }
            return sr;
        }

        //[Scenario("16. Call ToString when ResXFileRef( empty, *, *)")]
        [Scenario(true)]
        public ScenarioResult Scenario16(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            String fileName = String.Empty;
            String expectedToString = fileName + ";" + typeof(string).AssemblyQualifiedName + ";" + System.Text.Encoding.UTF32.BodyName;
            try
            {
                ResXFileRef fref = new ResXFileRef(fileName, typeof(string).AssemblyQualifiedName, System.Text.Encoding.UTF32);
                sr.IncCounters(fref.ToString().Equals(expectedToString), "FAIL: FileName.ToString", expectedToString, fref.ToString(), p.log);
            }
            catch (Exception ex)
            {
                sr.IncCounters(false, "FAIL: Contructor threw an exception: " + ex.Message.ToString(), p.log);
            }
            return sr;
        }

        //[Scenario("17. Call ToString when ResXFileRef( null, *, *)")]
        [Scenario(true)]
        public ScenarioResult Scenario17(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            String expectedToString = ";" + typeof(string).AssemblyQualifiedName + ";" + System.Text.Encoding.UTF32.BodyName;
            try
            {
                ResXFileRef fref = new ResXFileRef(null, typeof(string).AssemblyQualifiedName, System.Text.Encoding.UTF32);
                sr.IncCounters(false, "FAIL: Contructor did not an exception", p.log);
            }
            catch (ArgumentNullException ex)
            {
                ex.ToString();
                sr.IncCounters(true);
            }
            return sr;
        }

        //[Scenario("18. Call ToString when ResXFileRef( *, empty, *)")]
        [Scenario(true)]
        public ScenarioResult Scenario18(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            String fileName = @"D:\" + p.ru.GetString(maxNum);
            String fixedFileName = checkFileName(fileName);
            String expectedToString = fixedFileName + ";" + String.Empty + ";" + System.Text.Encoding.UTF32.BodyName;
            try
            {
                ResXFileRef fref = new ResXFileRef(fileName, String.Empty, System.Text.Encoding.UTF32);
                sr.IncCounters(expectedToString, fref.ToString(), "FAIL: FileName.ToString", p.log);
            }
            catch (Exception ex)
            {
                sr.IncCounters(false, "FAIL: Contructor threw an exception: " + ex.Message.ToString(), p.log);
            }
            return sr;
        }

        //[Scenario("19. Call ToString when ResXFileRef( *, null, *)")]
        [Scenario(true)]
        public ScenarioResult Scenario19(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            String fileName = @"D:\" + p.ru.GetString(maxNum);
            String expectedToString = fileName + ";" + ";" + System.Text.Encoding.UTF32.BodyName;
            try
            {
                ResXFileRef fref = new ResXFileRef(fileName, null, System.Text.Encoding.UTF32);
                sr.IncCounters(false, "FAIL: Contructor did not throw an exception", p.log);
            }
            catch (Exception ex)
            {
                ex.ToString();
                sr.IncCounters(true);
            }
            return sr;
        }

        //[Scenario("20. Call ToString when ResXFileRef( *, *, null)")]
        [Scenario(true)]
        public ScenarioResult Scenario20(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            String fileName = @"D:\" + p.ru.GetString(maxNum);
            String fixedFileName = checkFileName(fileName);
            String expectedToString = fixedFileName + ";" + typeof(string).AssemblyQualifiedName;

            try
            {
                ResXFileRef fref = new ResXFileRef(fileName, typeof(string).AssemblyQualifiedName, null);
                sr.IncCounters(expectedToString, fref.ToString(), "FAIL: FileName.ToString", p.log);
            }
            catch (Exception ex)
            {
                sr.IncCounters(false, "FAIL: Contructor threw an exception: " + ex.Message.ToString(), p.log);
            }
            return sr;
        }


        //[Scenario("21. Call ToString when ResXFileRef( *, *, default)")]
        [Scenario(true)]
        public ScenarioResult Scenario21(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            String fileName = @"D:\" + p.ru.GetString(maxNum);
            String fixedFileName = checkFileName(fileName);
            String expectedToString = fixedFileName + ";" + typeof(string).AssemblyQualifiedName + ";" + Encoding.Default.WebName;
            try
            {
                ResXFileRef fref = new ResXFileRef(fileName, typeof(string).AssemblyQualifiedName, Encoding.Default);

                sr.IncCounters(fref.ToString(), expectedToString, "FAIL: FileName.ToString", p.log);
            }
            catch (Exception ex)
            {
                sr.IncCounters(false, "FAIL: Contructor threw an exception: " + ex.Message.ToString(), p.log);
            }
            return sr;
        }

        //[Scenario("22. Verify that Encoding works by providing text file, setting the encoding, verify encoding worked")]
        [Scenario(true)]
        public ScenarioResult Scenario22(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            String path = "%Temp%\\ResXFileRefText.resx";
            path = Environment.ExpandEnvironmentVariables(path);

            ResXResourceWriter writer = new ResXResourceWriter(path);
            ResXFileRef fref = new ResXFileRef("MyTestfile.txt", typeof(string).AssemblyQualifiedName, Encoding.UTF32);

            ResXDataNode node = new ResXDataNode("tester", fref);
            writer.BasePath = "D:\\";
            writer.AddResource(node);
            writer.Close();

            ResXResourceReader reader = new ResXResourceReader(path);
            reader.UseResXDataNodes = true;
            IDictionaryEnumerator id = reader.GetEnumerator();

            id.MoveNext();

            ResXDataNode newNode = id.Value as ResXDataNode;

            sr.IncCounters(newNode.FileRef.TextFileEncoding.BodyName.Equals(Encoding.UTF32.BodyName), "FAIL: Value doesn't contain encoding", p.log);

            return sr;
        }

        private String checkFileName(String fileName)
        {
            if (fileName.IndexOf(";") != -1 || fileName.IndexOf("\"") != -1)
                fileName = "\"" + fileName + "\"";
            return fileName;
        }

        #endregion
    }
}
// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ 1. Contructor(null, null, null
//@ 2. Contructor(empty, empty, null
//@ 3. Contructor(null, null, some Encoding
//@ 4. Contructor(null, valid typeName, some Encoding
//@ 5. Contructor(valid fileName, valid typeName,  some Encoding
//@ 6. Contructor(valid long fileName, valid typeName, some Encoding
//@ 7. Contructor(empty string fileName,  valid typeName, some Encoding
//@ 8. Contructor(valid fileName, empty string typeName, some Encoding
//@ 9. Contructor(valid fileName, fully qualified typeName, some Encoding
//@ 10. Contructor(valid fileName, not fully qualified typeName, some Encoding
//@ 11. Contructor( valid fileName, valid typeName, null
//@ 12. Contructor(valid fileName, valid typeName, Encoding.default
//@ 13. Call ToString when ResXFileRef( null, null, null)
//@ 14. Call ToString when ResXFileRef( empty, empty, null)
//@ 15. Call ToString when ResXFileRef(valid, valid, some Encoding)
//@ 16. Call ToString when ResXFileRef( empty, *, *)
//@ 17. Call ToString when ResXFileRef( null, *, *)
//@ 18. Call ToString when ResXFileRef( *, empty, *)
//@ 19. Call ToString when ResXFileRef( *, null, *)
//@ 20. Call ToString when ResXFileRef( *, *, null)
//@ 21. Call ToString when ResXFileRef( *, *, default)
//@ 22. Verify that Encoding works by providing text file, setting the encoding, verify encoding worked

