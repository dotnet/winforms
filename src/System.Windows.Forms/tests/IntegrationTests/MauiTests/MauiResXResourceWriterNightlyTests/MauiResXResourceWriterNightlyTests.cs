// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Resources;
using System.Reflection;
using System.Collections;
using System.IO;
using System.Windows.Forms.IntegrationTests.Common;
using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using System.Threading;

namespace System.Windows.Forms.IntegrationTests.MauiTests
{
    public class MauiResXResourceWriterNightlyTests : ReflectBase
    {

        #region Testcase setup
        public MauiResXResourceWriterNightlyTests(string[] args) : base(args)
        {
            this.BringToForeground();
        }

        [STAThread]
        public static void Main(string[] args)
        {
            Thread.CurrentThread.SetCulture("en-US");
            Application.Run(new MauiResXResourceWriterNightlyTests(args));
        }

        protected override void InitTest(TParams p)
        {
            base.InitTest(p);
        }
        #endregion

        private const int maxInt = 50;

        //==========================================
        // Scenarios
        //==========================================
        #region Scenarios
        //[Scenario("Scenario1: null - Does NOT throw ArgumentNullException - edge case we shipped in 1.0")]
        [Scenario(true)]
        public ScenarioResult Scenario1NullContructor(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            try
            {
                ResXResourceWriter writer = new ResXResourceWriter((System.String)null);
                sr.IncCounters(true);
            }
            catch (ArgumentNullException ex)
            {
                ex.ToString();
                sr.IncCounters(false, "FAIL: ArgumentNullException was thrown", p.log);
            }
            return sr;
        }

        //[Scenario("Scenario2: 1. valid node - verify resource was added")]
        [Scenario(true)]
        public ScenarioResult Scenario2ValidNode(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            //SAMEERM BEGIN
            //Bitmap value = (Bitmap)p.ru.GetImage(ImageStyle.Bitmap); //test out equality
            //WFCTestLib.Util.GraphicsTools.Compare function does not take into account the stride of the bitmap. 
            //So if we have a bitmap the width of which is not an even multiple of 8 the Compare function will casue a buffer overflow and 
            //corrupt memory of the. Till we fix the compare function create a bitmap whose width is an even multiple of 8.
            Bitmap value = new Bitmap(64, 64);
            //SAMEERM END

            Image value2 = p.ru.GetImage(); //test the different types of images

            String key = p.ru.GetString(maxInt);
            String key2 = p.ru.GetString(maxInt);

            try
            {
                ResXResourceWriter writer = new ResXResourceWriter("ResxResourceWriter.resx");
                ResXDataNode node = new ResXDataNode(key, value);
                ResXDataNode node2 = new ResXDataNode(key2, value2);

                writer.AddResource(node);
                writer.AddResource(node2);
                writer.Close();

                ResXResourceReader reader = new ResXResourceReader("ResxResourceWriter.resx");
                reader.UseResXDataNodes = true;
                IDictionaryEnumerator it = reader.GetEnumerator();
                it.MoveNext();

                ResXDataNode newNode = it.Value as ResXDataNode;

                AssemblyName[] names = new AssemblyName[1];
                names[0] = new System.Reflection.AssemblyName(Assembly.GetAssembly(typeof(System.Drawing.Bitmap)).FullName);

                System.Drawing.Image result = (System.Drawing.Image)newNode.GetValue(names);
                WFCTestLib.Util.GraphicsTools temp = new GraphicsTools();

                Bitmap tempout;
                sr.IncCounters(temp.Compare((Bitmap)result, value, out tempout), "FAIL: resource was not added correctly", p.log);

                it.MoveNext();
                ResXDataNode newNode2 = it.Value as ResXDataNode;
                result = null;
                result = (System.Drawing.Image)newNode.GetValue(names);

                sr.IncCounters(result != null, "FAIL: writing and reading image failed", p.log);

            }
            catch (Exception ex)
            {
                sr.IncCounters(false, p.log, BugDb.VSWhidbey, 331065, ex.Message);
            }

            return sr;
        }

        //[Scenario("Scenario3: 1. node.fileName to non-existing path then read - should get exception")]
        [Scenario(true)]
        public ScenarioResult Scenario3NodeFileNameException(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            String key = p.ru.GetString(maxInt);
            String file = p.ru.GetString(maxInt);
            try
            {
                //create node with non-existing path.
                ResXFileRef fref = new ResXFileRef("D:\\NonExist.txt", "System.String");
                ResXDataNode node = new ResXDataNode(key, fref);
                //write to resx
                ResXResourceWriter writer = new ResXResourceWriter("ResxResourceWriter.resx");
                writer.AddResource(node);
                writer.Close();

                //read
                ResXResourceReader reader = new ResXResourceReader("ResxResourceWriter.resx");
                reader.UseResXDataNodes = true;
                IDictionaryEnumerator it = reader.GetEnumerator();
                it.MoveNext();

                ResXDataNode newNode = (ResXDataNode)it.Value;
                AssemblyName[] names = new AssemblyName[1];
                names[0] = new System.Reflection.AssemblyName(Assembly.GetAssembly(typeof(System.String)).FullName);
                object value = newNode.GetValue(names);

                sr.IncCounters(false, "FAIL: Exception expected", p.log);
            }
            catch (FileNotFoundException ex)
            {
                p.log.WriteLine(ex.Message);
                sr.IncCounters(true);
            }

            return sr;
        }

        //[Scenario("Scenario4: 1. node.DataNode.Info.fileRefFullPath is empty string")]
        [Scenario(true)]
        public ScenarioResult Scenario4DataNodeFileRefFullPathEmpty(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            String key = p.ru.GetString(maxInt);
            String file = p.ru.GetString(maxInt);
            try
            {
                //create node with no path.
                ResXFileRef fref = new ResXFileRef("", "System.String");
                ResXDataNode node = new ResXDataNode(key, fref);
                //write to resx
                ResXResourceWriter writer = new ResXResourceWriter("ResxResourceWriter.resx");
                writer.BasePath = "";
                writer.AddResource(node);
                writer.Close();

                //read
                ResXResourceReader reader = new ResXResourceReader("ResxResourceWriter.resx");
                reader.UseResXDataNodes = true;

                IDictionaryEnumerator it = reader.GetEnumerator();
                it.MoveNext();

                ResXDataNode newNode = (ResXDataNode)it.Value;

                AssemblyName[] names = new AssemblyName[1];
                names[0] = new System.Reflection.AssemblyName(Assembly.GetAssembly(typeof(System.String)).FullName);
                object value = newNode.GetValue(names);

                sr.IncCounters(false, "FAIL: ArgumentException expected", p.log);
            }
            catch (ArgumentException ex)
            {
                p.log.WriteLine(ex.Message);
                sr.IncCounters(true);
            }

            return sr;
        }

        //[Scenario("Scenario5: 1. node.DataNode.Info.fileRefFullPath is non empty string")]
        [Scenario(true)]
        public ScenarioResult Scenario5DataNodeFileRefFullPathEmpty(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            String key = p.ru.GetString(maxInt);
            String file = p.ru.GetString(maxInt);
            try
            {
                //create node with path.
                ResXFileRef fref = new ResXFileRef("ResxResourceWriter.resx", "System.String");
                ResXDataNode node = new ResXDataNode(key, fref);
                //write to resx
                ResXResourceWriter writer = new ResXResourceWriter("ResxResourceWriter.resx");
                writer.BasePath = "D:\\school";
                writer.AddResource(node);
                writer.Close();

                //read
                ResXResourceReader reader = new ResXResourceReader("ResxResourceWriter.resx");
                reader.UseResXDataNodes = true;

                IDictionaryEnumerator it = reader.GetEnumerator();
                it.MoveNext();

                ResXDataNode newNode = (ResXDataNode)it.Value;

                AssemblyName[] names = new AssemblyName[1];
                names[0] = new System.Reflection.AssemblyName(Assembly.GetAssembly(typeof(System.String)).FullName);
                object value = newNode.GetValue(names);

                sr.IncCounters(true);
            }
            catch (Exception ex)
            {
                p.log.WriteLine(ex.Message);
                sr.IncCounters(false, "FAIL: Unexpected exception " + ex.Message.ToString(), p.log);
            }

            return sr;
        }

        //[Scenario("Scenario6: 1. node.DataNode.Info.fileRefFullPath is to another drive")]
        [Scenario(true)]
        public ScenarioResult Scenario6FileRefFullPathDrive(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            String key = p.ru.GetString(maxInt);
            String file = p.ru.GetString(maxInt);
            try
            {
                //create the file on the other drive
                ResXResourceWriter temp = new ResXResourceWriter("C:\\machine\\temp.txt");
                temp.Close();

                //create node with path.
                ResXFileRef fref = new ResXFileRef("C:\\machine\\temp.txt", "System.String");
                ResXDataNode node = new ResXDataNode(key, fref);
                //write to resx
                ResXResourceWriter writer = new ResXResourceWriter("ResxResourceWriter.resx");
                writer.BasePath = "D:\\school";
                writer.AddResource(node);
                writer.Close();

                //read
                ResXResourceReader reader = new ResXResourceReader("ResxResourceWriter.resx");
                reader.UseResXDataNodes = true;

                IDictionaryEnumerator it = reader.GetEnumerator();
                it.MoveNext();

                ResXDataNode newNode = (ResXDataNode)it.Value;

                AssemblyName[] names = new AssemblyName[1];
                names[0] = new System.Reflection.AssemblyName(Assembly.GetAssembly(typeof(System.String)).FullName);
                object value = newNode.GetValue(names);

                sr.IncCounters(true);
                System.IO.File.Delete("C:\\machine\\temp.txt");  //remove the file I created on the C drive
            }
            catch (Exception ex)
            {
                p.log.WriteLine(ex.Message);
                sr.IncCounters(false, "FAIL: Unexpected exception " + ex.Message.ToString(), p.log);
            }

            return sr;
        }

        //[Scenario("Scenario7: 2. Get BasePath")]
        [Scenario(true)]
        public ScenarioResult Scenario7BasePath(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            String path = "D:\\school";
            String key = p.ru.GetString(maxInt);
            String file = "D:\\school\\genstrings.dll";
            try
            {
                ResXFileRef fref = new ResXFileRef(file, "System.String");
                ResXDataNode node = new ResXDataNode(key, fref);

                ResXResourceWriter writer = new ResXResourceWriter("ResxResourceWriter.resx");
                writer.BasePath = path;
                writer.AddResource(node);

                writer.Close();

                //read
                ResXResourceReader reader = new ResXResourceReader("ResxResourceWriter.resx");
                reader.UseResXDataNodes = true;

                IDictionaryEnumerator it = reader.GetEnumerator();
                it.MoveNext();

                ResXDataNode newNode = (ResXDataNode)it.Value;
                ResXFileRef value = newNode.FileRef;

                sr.IncCounters(path, writer.BasePath, p.log);
                sr.IncCounters(!value.FileName.Contains(path), "FAIL: Base Path was not used correctly - base path was included", p.log);

            }
            catch (Exception ex)
            {
                sr.IncCounters(false, ex.Message, p.log);
            }
            return sr;
        }

        //[Scenario("Scenario8: 2. Set BasePath to null")]
        [Scenario(true)]
        public ScenarioResult Scenario8BasePathNull(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            String path = null;
            String key = p.ru.GetString(maxInt);
            String file = "D:\\school\\genstrings.dll";
            try
            {
                ResXFileRef fref = new ResXFileRef(file, "System.String");
                ResXDataNode node = new ResXDataNode(key, fref);

                ResXResourceWriter writer = new ResXResourceWriter("ResxResourceWriter.resx");
                writer.BasePath = path;
                writer.AddResource(node);

                writer.Close();

                //read
                ResXResourceReader reader = new ResXResourceReader("ResxResourceWriter.resx");
                reader.UseResXDataNodes = true;

                IDictionaryEnumerator it = reader.GetEnumerator();
                it.MoveNext();

                ResXDataNode newNode = (ResXDataNode)it.Value;
                ResXFileRef value = newNode.FileRef;

                sr.IncCounters(path, writer.BasePath, p.log);
                sr.IncCounters(value.FileName.Equals(file), "FAIL: Base Path was not used correctly - base path was included", p.log);

            }
            catch (Exception ex)
            {
                sr.IncCounters(false, ex.Message, p.log);
            }
            return sr;
        }

        //[Scenario("Scenario9: 2. Set BasePath to empty string")]
        [Scenario(true)]
        public ScenarioResult Scenario9BasePathEmpty(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            String path = String.Empty;
            String key = p.ru.GetString(maxInt);
            String file = "D:\\school\\genstrings.dll";
            try
            {
                ResXFileRef fref = new ResXFileRef(file, "System.String");
                ResXDataNode node = new ResXDataNode(key, fref);

                ResXResourceWriter writer = new ResXResourceWriter("ResxResourceWriter.resx");
                writer.BasePath = path;
                writer.AddResource(node);

                writer.Close();

                //read
                ResXResourceReader reader = new ResXResourceReader("ResxResourceWriter.resx");
                reader.UseResXDataNodes = true;

                IDictionaryEnumerator it = reader.GetEnumerator();
                it.MoveNext();

                ResXDataNode newNode = (ResXDataNode)it.Value;
                ResXFileRef value = newNode.FileRef;

                sr.IncCounters(path, writer.BasePath, p.log);
                sr.IncCounters(value.FileName.Equals(file), "FAIL: Base Path was not used correctly - base path was included", p.log);

            }
            catch (Exception ex)
            {
                sr.IncCounters(false, ex.Message, p.log);
            }
            return sr;
        }

        //[Scenario("Scenario10: 2. Set BasePath to valid path but no file - file is created - TBD")]
        [Scenario(true)]
        public ScenarioResult Scenario10BasePathNoFile(TParams p)
        {

            ScenarioResult sr = new ScenarioResult();
            String path = "D:\\Program Files";
            String key = p.ru.GetString(maxInt);
            String file = "D:\\Program Files\\genstrings.dll";
            try
            {
                ResXFileRef fref = new ResXFileRef(file, "System.String");
                ResXDataNode node = new ResXDataNode(key, fref);

                ResXResourceWriter writer = new ResXResourceWriter("ResxResourceWriter.resx");
                writer.BasePath = path;
                writer.AddResource(node);

                writer.Close();

                //read
                ResXResourceReader reader = new ResXResourceReader("ResxResourceWriter.resx");
                reader.UseResXDataNodes = true;

                IDictionaryEnumerator it = reader.GetEnumerator();
                it.MoveNext();

                ResXDataNode newNode = (ResXDataNode)it.Value;
                ResXFileRef value = newNode.FileRef;

                sr.IncCounters(path, writer.BasePath, p.log);
                sr.IncCounters(!value.FileName.Contains(file), "FAIL: Base Path was not used correctly - base path was included", p.log);

            }
            catch (Exception ex)
            {
                sr.IncCounters(false, ex.Message, p.log);
            }
            return sr;
        }

        //[Scenario("Scenario11: 2. Set BasePath to valid long path")]
        [Scenario(true)]
        public ScenarioResult Scenario11BasePathLong(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            System.IO.Directory.CreateDirectory("D:\\school\\ResXResourceWriterNightlyDir\\AnotherDir\\ForManyTests");
            FileStream fs = System.IO.File.Create("D:\\school\\ResXResourceWriterNightlyDir\\AnotherDir\\ForManyTests\\Sample.txt");

            String path = "D:\\school\\ResXResourceWriterNightlyDir\\AnotherDir\\ForManyTests";
            String key = p.ru.GetString(maxInt);
            String file = "D:\\school\\ResXResourceWriterNightlyDir\\AnotherDir\\ForManyTests\\Sample.txt";
            try
            {
                ResXFileRef fref = new ResXFileRef(file, "System.String");
                ResXDataNode node = new ResXDataNode(key, fref);

                ResXResourceWriter writer = new ResXResourceWriter("ResxResourceWriter.resx");
                writer.BasePath = path;
                writer.AddResource(node);

                writer.Close();

                //read
                ResXResourceReader reader = new ResXResourceReader("ResxResourceWriter.resx");
                reader.UseResXDataNodes = true;

                IDictionaryEnumerator it = reader.GetEnumerator();
                it.MoveNext();

                ResXDataNode newNode = (ResXDataNode)it.Value;
                ResXFileRef value = newNode.FileRef;

                sr.IncCounters(path, writer.BasePath, p.log);
                sr.IncCounters(!value.FileName.Contains(path), "FAIL: Base Path was not used correctly - base path was included", p.log);
                reader.Close();
            }
            catch (Exception ex)
            {
                sr.IncCounters(false, ex.Message, p.log);
            }
            fs.Close();
            System.IO.File.Delete(file);
            System.IO.Directory.Delete("D:\\school\\ResXResourceWriterNightlyDir\\AnotherDir\\ForManyTests");
            System.IO.Directory.Delete("D:\\school\\ResXResourceWriterNightlyDir\\AnotherDir\\");
            System.IO.Directory.Delete("D:\\school\\ResXResourceWriterNightlyDir\\");

            return sr;
        }

        //[Scenario("Scenario12: 2. Set BasePath to valid relative path")]
        [Scenario(true)]
        public ScenarioResult Scenario12BasePathValid(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            String path = "\\school";
            String key = p.ru.GetString(maxInt);
            String file = "\\school\\genstrings.dll";
            try
            {
                ResXFileRef fref = new ResXFileRef(file, "System.String");
                ResXDataNode node = new ResXDataNode(key, fref);

                ResXResourceWriter writer = new ResXResourceWriter("ResxResourceWriter.resx");
                writer.BasePath = path;
                writer.AddResource(node);

                writer.Close();

                //read
                ResXResourceReader reader = new ResXResourceReader("ResxResourceWriter.resx");
                reader.UseResXDataNodes = true;

                IDictionaryEnumerator it = reader.GetEnumerator();
                it.MoveNext();

                ResXDataNode newNode = (ResXDataNode)it.Value;
                ResXFileRef value = newNode.FileRef;

                sr.IncCounters(path, writer.BasePath, p.log);
                sr.IncCounters(!value.FileName.Contains(path), "FAIL: Base Path was not used correctly - base path was included", p.log);
            }
            catch (Exception ex)
            {
                sr.IncCounters(false, ex.Message, p.log);
            }

            return sr;
        }

        //[Scenario("Scenario13: 2. Set BasePath to a different path")]
        [Scenario(true)]
        public ScenarioResult Scenario13BasePathDiff(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            String path = "C:\\school";
            String key = p.ru.GetString(maxInt);
            String file = "D:\\school\\genstrings.dll";
            try
            {
                ResXFileRef fref = new ResXFileRef(file, "System.String");
                ResXDataNode node = new ResXDataNode(key, fref);

                ResXResourceWriter writer = new ResXResourceWriter("ResxResourceWriter.resx");
                writer.BasePath = path;
                writer.AddResource(node);

                writer.Close();

                //read
                ResXResourceReader reader = new ResXResourceReader("ResxResourceWriter.resx");
                reader.UseResXDataNodes = true;

                IDictionaryEnumerator it = reader.GetEnumerator();
                it.MoveNext();

                ResXDataNode newNode = (ResXDataNode)it.Value;
                ResXFileRef value = newNode.FileRef;

                sr.IncCounters(path, writer.BasePath, p.log);
                sr.IncCounters(value.FileName.Equals(file), "FAIL: Base Path was not used correctly - base path was excluded", p.log);

            }
            catch (Exception ex)
            {
                sr.IncCounters(false, ex.Message, p.log);
            }
            return sr;
        }
        #endregion
    }
}
// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ Scenario1: null - Does NOT throw ArgumentNullException - edge case we shipped in 1.0
//@ Scenario2: 1. valid node - verify resource was added
//@ Scenario3: 1. node.fileName to non-existing path then read - should get exception
//@ Scenario4: 1. node.DataNode.Info.fileRefFullPath is empty string
//@ Scenario5: 1. node.DataNode.Info.fileRefFullPath is non empty string
//@ Scenario6: 1. node.DataNode.Info.fileRefFullPath is to another drive
//@ Scenario7: 2. Get BasePath
//@ Scenario8: 2. Set BasePath to null
//@ Scenario9: 2. Set BasePath to empty string
//@ Scenario10: 2. Set BasePath to valid path but no file - file is created - TBD
//@ Scenario11: 2. Set BasePath to valid long path
//@ Scenario12: 2. Set BasePath to valid relative path
//@ Scenario13: 2. Set BasePath to a different path
