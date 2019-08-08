// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Forms;
using System.Resources;
using System.Reflection;
using System.ComponentModel.Design;
using System.Collections;
using System.Windows.Forms.IntegrationTests.Common;
using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using System.Threading;

namespace System.Windows.Forms.IntegrationTests.MauiTests
{
    public class MauiResXDataNodeTextTests : ReflectBase
    {
        #region Testcase setup
        public MauiResXDataNodeTextTests(string[] args) : base(args)
        {
            this.BringToForeground();
        }

        [STAThread]
        public static void Main(string[] args)
        {
            Thread.CurrentThread.SetCulture("en-US");
            Application.Run(new MauiResXDataNodeTextTests(args));
        }

        protected override void InitTest(TParams p)
        {
            base.InitTest(p);
        }
        //my own ITypeResolutionService
        protected class MyService : ITypeResolutionService
        {
            private AssemblyName[] names;
            private Hashtable cachedAssemblies;

            public MyService()
            {
                System.Reflection.Assembly SampleAssembly = System.Reflection.Assembly.GetAssembly(typeof(System.String));
                this.names = new AssemblyName[1];
                this.names[0] = new System.Reflection.AssemblyName(SampleAssembly.FullName);
            }

            public Assembly GetAssembly(AssemblyName name)
            {

                return GetAssembly(name, true);
            }

            public Assembly GetAssembly(AssemblyName name, bool throwOnError)
            {

                Assembly result = null;
                if (cachedAssemblies != null)
                {
                    result = (Assembly)cachedAssemblies[name];
                }
                else
                {
                    cachedAssemblies = new Hashtable();
                }
                // try to load it first fron the gac
                //result = Assembly.LoadWithPartialName(name.FullName);
                result = Assembly.Load(name.FullName);


                if (result != null)
                {
                    cachedAssemblies[name] = result;
                }
                else
                {
                    for (int i = 0; i < names.Length; i++)
                    {
                        if (name.Equals(names[i]))
                        {
                            try
                            {
                                result = Assembly.LoadFrom(GetPathOfAssembly(name));
                                if (result != null)
                                {
                                    cachedAssemblies.Add(name, result);
                                }
                            }
                            catch (Exception)
                            {
                                if (throwOnError)
                                {
                                    throw;
                                }
                            }
                        }
                    }
                }
                return result;
            }

            public string GetPathOfAssembly(AssemblyName name)
            {
                return name.CodeBase;
            }
            public Type GetType(string name)
            {
                return GetType(name, true);
            }

            public Type GetType(string name, bool throwOnError)
            {
                return GetType(name, throwOnError, false);
            }

            public Type GetType(string name, bool throwOnError, bool ignoreCase)
            {
                Type result = null;

                for (int i = 0; i < names.Length; i++)
                {
                    Assembly a = GetAssembly(names[i], throwOnError);
                    result = a.GetType(name, false, ignoreCase);
                    if (result == null)
                    {
                        int indexOfComma = name.IndexOf(",");
                        if (indexOfComma != -1)
                        {
                            string shortName = name.Substring(0, indexOfComma);
                            result = a.GetType(shortName, false, ignoreCase);
                        }
                    }
                    if (result != null)
                        break;
                }


                if (result == null && throwOnError)
                {

                    throw new ArgumentException("Could not find " + name);
                }
                return result;
            }
            public void ReferenceAssembly(AssemblyName name)
            {
                throw new NotSupportedException("not supported");
            }
        }

        #endregion

        //==========================================
        // Scenarios
        //==========================================
        #region Scenarios
        [Scenario("1) ResXDataNode(null, null) - ArgumentNullException")]
        public ScenarioResult Scenario1(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            try
            {
                ResXDataNode node = new ResXDataNode(null, null);
                sr.IncCounters(false, "FAIL: Argument exception expected", p.log);
            }
            catch (ArgumentException ex)
            {
                ex.ToString();
                sr.IncCounters(true);
            }

            return sr;
        }

        [Scenario("2) ResXDataNode(null, object) - ArgumentNullException")]
        public ScenarioResult Scenario2(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            String temp = p.ru.GetString(50);
            try
            {
                ResXDataNode node = new ResXDataNode(null, temp);
                sr.IncCounters(false, "FAIL: Argument exception expected", p.log);
            }
            catch (ArgumentException ex)
            {
                ex.ToString();
                sr.IncCounters(true);
            }

            return sr;
        }

        [Scenario("3) ResXDataNode(name, null) - ArgumentNullException")]
        public ScenarioResult Scenario3(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            String temp = p.ru.GetString(50);
            try
            {
                ResXDataNode node = new ResXDataNode(temp, null);
                sr.IncCounters(false, "FAIL: Argument exception expected", p.log);
            }
            catch (ArgumentException ex)
            {
                ex.ToString();
                sr.IncCounters(true);
            }

            return sr;
        }

        [Scenario("4) ResXDataNode(name, object) - valid: TypeName returns fully qualified type")]
        public ScenarioResult Scenario4(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            String name = p.ru.GetString(5, 100);
            String temp = p.ru.GetString(50);
            try
            {
                ResXDataNode node = new ResXDataNode(name, temp);

                AssemblyName[] names = new AssemblyName[1];
                AssemblyName[] assembly = new System.Reflection.AssemblyName[1];
                Assembly SampleAssembly = System.Reflection.Assembly.GetAssembly(temp.GetType());
                assembly[0] = new System.Reflection.AssemblyName(SampleAssembly.FullName);

                String result = node.GetValueTypeName(names);

                if (!result.Contains(typeof(String).ToString()))
                    sr.IncCounters(false, "FAIL: TypeName did not return fully qualified type", p.log);
                else
                    sr.IncCounters(true);

            }
            catch (ArgumentException ex)
            {
                ex.ToString();
                sr.IncCounters(false);
            }

            return sr;
        }

        [Scenario("5) ResXDataNode(name, object) - invalid: object doesn't support serialization - InvalidOperationException")]
        public ScenarioResult Scenario5(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            String temp = p.ru.GetString(50);
            Button button = new Button();
            button.Text = "Tester";
            try
            {
                ResXDataNode node = new ResXDataNode(temp, button);
                sr.IncCounters(false, "FAIL: InvalidOperationException exception expected", p.log);
            }
            catch (InvalidOperationException ex)
            {
                ex.ToString();
                sr.IncCounters(true);
            }

            return sr;
        }

        [Scenario("6) ResXDataNode(name, object) - object doesn't have type converter ToString")]
        public ScenarioResult Scenario6(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            String temp = p.ru.GetString(50);

            //no string converter, serializable
            System.Drawing.Bitmap image = new System.Drawing.Bitmap(5, 5);

            try
            {
                ResXDataNode node = new ResXDataNode(temp, image);
                //string converter, not serializable
                System.Drawing.Color color = new System.Drawing.Color();

                ResXDataNode node1 = new ResXDataNode(temp, color);
                sr.IncCounters(true);
            }
            catch (InvalidOperationException ex)
            {
                sr.IncCounters(false, "FAIL: Exception occurred: " + ex.Message.ToString(), p.log);
            }

            return sr;
        }

        [Scenario("7) ResXDataNode(name, ResXFileRef) - valid: FileRef is populated")]
        public ScenarioResult Scenario7(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            ResXFileRef fref = new ResXFileRef("MyTestFile.txt", typeof(string).AssemblyQualifiedName);
            String temp = p.ru.GetString(50);
            try
            {
                ResXDataNode node = new ResXDataNode(temp, fref);
                ResXFileRef result = node.FileRef;
                if (result != null)
                {
                    sr.IncCounters(result.FileName == "MyTestFile.txt", "FAIL: FileName not set", p.log);
                    sr.IncCounters(result.TypeName == typeof(string).AssemblyQualifiedName, "FAIL: TypeName not set", p.log);
                }
                else
                    sr.IncCounters(false, "FAIL: ResXFileRef was null", p.log);
            }
            catch (Exception ex)
            {
                sr.IncCounters(false, "FAIL: Exception thrown: " + ex.Message.ToString(), p.log);
            }

            return sr;
        }

        [Scenario("8) Get Name")]
        public ScenarioResult Scenario8(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            String temp = p.ru.GetString(50);
            String button = "button";
            try
            {
                ResXDataNode node = new ResXDataNode(temp, button);

                sr.IncCounters(node.Name == temp, "FAIL: Name returned wrong value", temp, node.Name.ToString(), p.log);
            }
            catch (Exception ex)
            {
                ex.ToString();
                sr.IncCounters(false, "FAIL: Unknown failure", p.log);
            }

            return sr;
        }

        [Scenario("9) Set Name to empty string - ArgumentException")]
        public ScenarioResult Scenario9(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            String temp = p.ru.GetString(50);
            String button = "button";
            try
            {
                ResXDataNode node = new ResXDataNode(temp, button);
                try
                {
                    node.Name = null;
                    sr.IncCounters(false, "FAIL: Name set to null should have thrown exception", p.log);
                }
                catch (ArgumentException e)
                {
                    e.ToString();
                    excludedMethods.ToString();
                    sr.IncCounters(true);
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
                sr.IncCounters(false, "FAIL: Unknown failure", p.log);
            }

            return sr;
        }

        [Scenario("10) Set Name to valid Resx formated string")]
        public ScenarioResult Scenario10(TParams p)
        {
            String name = "<data name=\"string1\"><value>hello</value></data>";

            ScenarioResult sr = new ScenarioResult();
            String button = "button";
            try
            {
                ResXDataNode node = new ResXDataNode(name, button);

                sr.IncCounters(node.Name == name, "FAIL: Name returned wrong value", name, node.Name.ToString(), p.log);
            }
            catch (Exception ex)
            {
                ex.ToString();
                sr.IncCounters(false, "FAIL: Unknown failure", p.log);
            }
            return sr;
        }

        [Scenario("11) Set Name to a valid Resx formated long string")]
        public ScenarioResult Scenario11(TParams p)
        {
            String name = "<data name=\"cutToolStripMenuItem.Image\" type=\"System.Drawing.Bitmap, System.Drawing\" mimetype=\"application/x-microsoft.net.object.bytearray.base64\"><value>iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAFRJREFUOE9jYKAh+I9mNjqfoNWDwwCYK0A0yV4A+RHZAIJ+xqYAZjNZtsMMpERzA9QFYJocANMIomGYKHPQNSC7gKBrsCkeeANA/iYrDIgKMMoUAQAqwjZX15oapQAAAABJRU5ErkJggg==</value><comment /></data>";

            ScenarioResult sr = new ScenarioResult();
            String button = "button";
            try
            {
                ResXDataNode node = new ResXDataNode(name, button);

                sr.IncCounters(node.Name == name, "FAIL: Name returned wrong value", name, node.Name.ToString(), p.log);
            }
            catch (Exception ex)
            {
                ex.ToString();
                sr.IncCounters(false, "FAIL: Unknown failure", p.log);
            }
            return sr;
        }

        [Scenario("13) Get Comment")]
        public ScenarioResult Scenario13(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            String temp = p.ru.GetString(50);
            String button = "button";
            try
            {
                ResXDataNode node = new ResXDataNode(temp, button);
                sr.IncCounters(node.Comment == String.Empty, p.log, BugDb.VSWhidbey, 185768, "FAIL: Comment was not String.Empty");
            }
            catch (Exception ex)
            {
                ex.ToString();
                sr.IncCounters(false, "FAIL: Unknown failure", p.log);
            }

            return sr;
        }

        [Scenario("14) Set Comment to null")]
        public ScenarioResult Scenario14(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            String temp = p.ru.GetString(50);
            String button = "button";
            try
            {
                ResXDataNode node = new ResXDataNode(temp, button);
                node.Comment = null;
                //it always returns Empty
                sr.IncCounters(String.Empty, node.Comment, "FAIL: Comment was not empty", p.log);
            }
            catch (Exception ex)
            {
                ex.ToString();
                sr.IncCounters(false, "FAIL: Unknown failure", p.log);
            }

            return sr;
        }

        [Scenario("15) Set Comment to empty string")]
        public ScenarioResult Scenario15(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            String temp = p.ru.GetString(50);
            String button = "button";
            try
            {
                ResXDataNode node = new ResXDataNode(temp, button);
                node.Comment = String.Empty;
                sr.IncCounters(node.Comment == String.Empty, "FAIL: Comment was not String.Empty", p.log);
            }
            catch (Exception ex)
            {
                ex.ToString();
                sr.IncCounters(false, "FAIL: Unknown failure", p.log);
            }

            return sr;
        }

        [Scenario("16) Set Comment to valid random string")]
        public ScenarioResult Scenario16(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            String temp = p.ru.GetString(50);
            String button = "button";
            String comment = p.ru.GetString(40);
            try
            {
                ResXDataNode node = new ResXDataNode(temp, button);
                node.Comment = comment;
                sr.IncCounters(node.Comment == comment, "FAIL: Comment was not set", comment, node.Comment, p.log);
            }
            catch (Exception ex)
            {
                ex.ToString();
                sr.IncCounters(false, "FAIL: Unknown failure", p.log);
            }

            return sr;
        }

        [Scenario("17) Set Comment to valid random long string - XML formatted string")]
        public ScenarioResult Scenario17(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            String temp = p.ru.GetString(50);
            String button = "button";
            String comment = p.ru.GetString(100);
            try
            {
                ResXDataNode node = new ResXDataNode(temp, button);
                node.Comment = comment;
                sr.IncCounters(node.Comment == comment, "FAIL: Comment was not set", comment, node.Comment, p.log);
            }
            catch (Exception ex)
            {
                ex.ToString();
                sr.IncCounters(false, "FAIL: Unknown failure", p.log);
            }

            return sr;
        }

        [Scenario("18) Set Comment to valid random long string - XML mal-formatted string")]
        public ScenarioResult Scenario18(TParams p)
        {

            ScenarioResult sr = new ScenarioResult();
            String temp = p.ru.GetString(50);
            String button = "button";
            String comment = "<data name=\"cutToolStripMenuItem.Image\" type=\"System.Drawing.Bitmap, System.Drawing\" mimetype=\"application/x-microsoft.net.object.bytearray.base64\"><value>iVBORw0KGgoAAAANSUhEUgAAABAAAAAQCAYAAAAf8/9hAAAAAXNSR0IArs4c6QAAAARnQU1BAACxjwv8YQUAAAAgY0hSTQAAeiYAAICEAAD6AAAAgOgAAHUwAADqYAAAOpgAABdwnLpRPAAAAFRJREFUOE9jYKAh+I9mNjqfoNWDwwCYK0A0yV4A+RHZAIJ+xqYAZjNZtsMMpERzA9QFYJocANMIomGYKHPQNSC7gKBrsCkeeANA/iYrDIgKMMoUAQAqwjZX15oapQAAAABJRU5ErkJggg==</value><comment /></data>";
            try
            {
                ResXDataNode node = new ResXDataNode(temp, button);
                node.Comment = comment;
                sr.IncCounters(node.Comment == comment, "FAIL: Comment was not set", comment, node.Comment, p.log);
            }
            catch (Exception ex)
            {
                ex.ToString();
                sr.IncCounters(false, "FAIL: Unknown failure", p.log);
            }

            return sr;
        }

        [Scenario("19) GetTypeName - fully qualified type name for the resource")]
        public ScenarioResult Scenario19(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            ResXFileRef fref = new ResXFileRef("MyTestFile.txt", typeof(string).AssemblyQualifiedName);
            String temp = p.ru.GetString(50);
            try
            {
                ResXDataNode node = new ResXDataNode(temp, fref);

                System.Reflection.AssemblyName[] assembly = new System.Reflection.AssemblyName[1];
                System.Reflection.Assembly SampleAssembly = System.Reflection.Assembly.GetAssembly(temp.GetType());
                assembly[0] = new System.Reflection.AssemblyName(SampleAssembly.FullName);


                String result = node.GetValueTypeName(assembly);
                sr.IncCounters(result.Equals(typeof(string).AssemblyQualifiedName), "FAIL: GetTypeName failed", p.log);

            }
            catch (Exception ex)
            {
                sr.IncCounters(false, "FAIL: Exception thrown: " + ex.Message.ToString(), p.log);
            }

            return sr;
        }

        [Scenario("20) GetTypeName - able to return String.Empty - Add resource with Values = null")]
        public ScenarioResult Scenario20(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            ResXResourceWriter writer = new ResXResourceWriter("MyTestFile.resx");
            String temp = p.ru.GetString(50);
            Object nullValue = null;

            //add a resource with value null
            writer.AddResource(temp, nullValue);
            writer.Close();

            try
            {
                ResXResourceReader reader = new ResXResourceReader("MyTestFile.resx");
                reader.UseResXDataNodes = true;

                IDictionaryEnumerator iterator = reader.GetEnumerator();
                iterator.MoveNext();

                ResXDataNode node = (ResXDataNode)iterator.Value;
                //only pass the assembly for string and not MyClass
                System.Reflection.AssemblyName[] assembly = new System.Reflection.AssemblyName[1];
                System.Reflection.Assembly SampleAssembly = System.Reflection.Assembly.GetAssembly(temp.GetType());
                assembly[0] = new System.Reflection.AssemblyName(SampleAssembly.FullName);

                //result should be empty since it couldn't resolve the type
                String result = node.GetValueTypeName(assembly);

                sr.IncCounters(result.Contains("System.Object"), "FAIL: GetTypeName failed", p.log);

            }
            catch (Exception ex)
            {
                sr.IncCounters(false, "FAIL: Exception thrown: " + ex.Message.ToString(), p.log);
            }

            return sr;
        }

        [Scenario("21) GetTypeName(valid ITypeResolutionService")]
        public ScenarioResult Scenario21(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            String temp = p.ru.GetString(50);
            ResXFileRef fref = new ResXFileRef(temp, temp.GetType().ToString());
            try
            {
                ResXDataNode node = new ResXDataNode(temp, fref);

                MyService iservice = new MyService();

                String result = node.GetValueTypeName(iservice);
                sr.IncCounters(result.Contains("String"), "FAIL: GetTypeName failed", p.log);

            }
            catch (Exception ex)
            {
                sr.IncCounters(false, "FAIL: Exception thrown: " + ex.Message.ToString(), p.log);
            }

            return sr;
        }

        [Scenario("22) GetTypeName(empty AssemblyNames")]
        public ScenarioResult Scenario22(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            String temp = p.ru.GetString(50);
            String value = p.ru.GetString(40);

            ResXFileRef fref = new ResXFileRef(temp, value);
            try
            {
                ResXDataNode node = new ResXDataNode(temp, fref);

                System.Reflection.AssemblyName[] assembly = new System.Reflection.AssemblyName[1];
                System.Reflection.Assembly SampleAssembly = System.Reflection.Assembly.GetAssembly(temp.GetType());
                assembly[0] = new System.Reflection.AssemblyName();

                String result = node.GetValueTypeName(assembly);
                sr.IncCounters(false, "FAIL: Expected exception", p.log);

            }
            catch (ArgumentException ex)
            {
                ex.ToString();
                sr.IncCounters(true);
            }

            return sr;
        }

        [Scenario("23) GetTypeName(valid AssemblyNames")]
        public ScenarioResult Scenario23(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            ResXFileRef fref = new ResXFileRef("MyTestFile.txt", typeof(string).AssemblyQualifiedName);
            String temp = p.ru.GetString(50);
            try
            {
                ResXDataNode node = new ResXDataNode(temp, fref);

                System.Reflection.AssemblyName[] assembly = new System.Reflection.AssemblyName[2];
                System.Reflection.Assembly SampleAssembly = System.Reflection.Assembly.GetAssembly(temp.GetType());

                assembly[0] = new System.Reflection.AssemblyName(SampleAssembly.FullName);


                String result = node.GetValueTypeName(assembly);
                sr.IncCounters(result.Equals(typeof(string).AssemblyQualifiedName), "FAIL: GetTypeName failed", p.log);

            }
            catch (Exception ex)
            {
                sr.IncCounters(false, "FAIL: Exception thrown: " + ex.Message.ToString(), p.log);
            }

            return sr;
        }

        [Scenario("24) GetTypeName where resolution service could not resolve the type - name should be returned as is")]
        public ScenarioResult Scenario24(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            ResXFileRef fref = new ResXFileRef("MyTestFile.txt", typeof(System.IO.File).AssemblyQualifiedName);
            String temp = p.ru.GetString(50);
            try
            {
                ResXDataNode node = new ResXDataNode(temp, fref);

                System.Reflection.AssemblyName[] assembly = new System.Reflection.AssemblyName[1];
                System.Reflection.Assembly SampleAssembly = System.Reflection.Assembly.GetAssembly(temp.GetType());

                assembly[0] = new System.Reflection.AssemblyName(SampleAssembly.FullName);

                String result = node.GetValueTypeName(assembly);
                sr.IncCounters(result, typeof(System.IO.File).AssemblyQualifiedName, "FAIL: GetTypeName failed", p.log);

            }
            catch (Exception ex)
            {
                sr.IncCounters(false, "FAIL: Exception thrown: " + ex.Message.ToString(), p.log);
            }

            return sr;
        }

        [Scenario("25) GetValue(ITypeResolutionService/AssemblyName) - ResXDataNode initialized using object - return a fully qualified type")]
        public ScenarioResult Scenario25(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            String temp = p.ru.GetString(50);
            String value = p.ru.GetString(50);
            try
            {
                ResXDataNode node = new ResXDataNode(temp, value);

                MyService iservice = new MyService();

                Object result = node.GetValue(iservice);
                Type resultType = result.GetType();
                sr.IncCounters(resultType, typeof(System.String), "FAIL: GetTypeName did not return the fully qualified type", p.log);

            }
            catch (Exception ex)
            {
                sr.IncCounters(false, "FAIL: Exception thrown: " + ex.Message.ToString(), p.log);
            }

            return sr;
        }

        [Scenario("26) GetValue(ITypeResolutionService/AssemblyName) - ResXDateNode initialized using ResXFileRef: return resource value")]
        public ScenarioResult Scenario26(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            ResXResourceWriter writer = new ResXResourceWriter("MyTestFile.resx");
            writer.Close();

            ResXFileRef fref = new ResXFileRef("MyTestFile.resx", typeof(System.String).AssemblyQualifiedName);
            try
            {
                ResXDataNode node = new ResXDataNode("temp", fref);

                System.Reflection.AssemblyName[] assembly = new System.Reflection.AssemblyName[1];
                System.Reflection.Assembly SampleAssembly = System.Reflection.Assembly.GetAssembly(fref.GetType());

                assembly[0] = new System.Reflection.AssemblyName(SampleAssembly.FullName);

                Object result = node.GetValue(assembly);
                String stringVersion = result as String;
                sr.IncCounters(stringVersion.Contains("xml"), "FAIL: GetValue did not return the resx file", p.log);
            }
            catch (Exception ex)
            {
                sr.IncCounters(false, "FAIL: Exception thrown: " + ex.Message.ToString(), p.log);
            }

            return sr;
        }


        [Scenario("30) Get FileRef from ResXDataNode initialized using object")]
        public ScenarioResult Scenario30(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            String temp = p.ru.GetString(50);
            String looloo = "lalalalala";

            try
            {
                ResXDataNode node = new ResXDataNode(temp, looloo);
                ResXFileRef result = node.FileRef;
                if (result != null)
                {
                    sr.IncCounters(false, "FAIL: ResXFileRef was not null", p.log);
                }
                else
                    sr.IncCounters(true);
            }
            catch (Exception ex)
            {
                sr.IncCounters(false, "FAIL: Exception thrown: " + ex.Message.ToString(), p.log);
            }

            return sr;
        }

        [Scenario("31) Get FileRef from ResXDataNode initialized using FileRef")]
        public ScenarioResult Scenario31(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            String temp = p.ru.GetString(50);

            ResXResourceWriter writer = new ResXResourceWriter("MyTestFile.resx");
            writer.Close();

            ResXFileRef fref = new ResXFileRef("MyTestFile.resx", typeof(System.String).AssemblyQualifiedName);
            try
            {
                ResXDataNode node = new ResXDataNode(temp, fref);
                ResXFileRef result = node.FileRef;
                if (result == null)
                {
                    sr.IncCounters(false, "FAIL: ResXFileRef was null", p.log);
                }
                else
                    sr.IncCounters(true);
            }
            catch (Exception ex)
            {
                sr.IncCounters(false, "FAIL: Exception thrown: " + ex.Message.ToString(), p.log);
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
//@ 1) ResXDataNode(null, null) - ArgumentNullException

//@ 2) ResXDataNode(null, object) - ArgumentNullException

//@ 3) ResXDataNode(name, null) - ArgumentNullException

//@ 4) ResXDataNode(name, object) - valid: TypeName returns fully qualified type

//@ 5) ResXDataNode(name, object) - invalid: object doesn't support serialization - InvalidOperationException

//@ 6) ResXDataNode(name, object) - invalid: object doesn't have type converter ToString - InvalidOperationException

//@ 7) ResXDataNode(name, ResXFileRef) - valid: FileRef is populated

//@ 8) Get Name

//@ 9) Set Name to empty string - ArgumentException

//@ 10) Set Name to valid Resx formated string

//@ 11) Set Name to a valid Resx formated long string

//@ 13) Get Comment

//@ 14) Set Comment to null

//@ 15) Set Comment to empty string

//@ 16) Set Comment to valid random string

//@ 17) Set Comment to valid random long string - XML formatted string

//@ 18) Set Comment to valid random long string - XML mal-formatted string

//@ 19) GetTypeName - fully qualified type name for the resource

//@ 20) GetTypeName - able to return String.Empty - Add resource with Values = null

//@ 21) GetTypeName(valid ITypeResolutionService

//@ 22) GetTypeName(empty AssemblyNames

//@ 23) GetTypeName(valid AssemblyNames

//@ 24) GetTypeName where resolution service could not resolve the type - name should be returned as is

//@ 25) GetValue(ITypeResolutionService/AssemblyName) - ResXDataNode initialized using object - will not return a fully qualified type

//@ 26) GetValue(ITypeResolutionService/AssemblyName) - ResXDateNode initialized using ResXFileRef: return resource value

//@ 30) Get FileRef from ResXDataNode initialized using object

//@ 31) Get FileRef from ResXDataNode initialized using  ResXFileRef


