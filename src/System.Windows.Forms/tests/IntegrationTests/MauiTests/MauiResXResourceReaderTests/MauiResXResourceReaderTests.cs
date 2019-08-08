// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Windows.Forms;
using System.Resources;
using System.Windows.Forms.IntegrationTests.Common;
using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using System.Threading;

namespace System.Windows.Forms.IntegrationTests.MauiTests
{
    public class MauiResXResourceReaderTests : ReflectBase
    {
        private ResXResourceReader resXReader;

        #region Testcase setup
        public MauiResXResourceReaderTests(string[] args) : base(args)
        {
            this.BringToForeground();
        }

        [STAThread]
        public static void Main(string[] args)
        {
            Thread.CurrentThread.SetCulture("en-US");
            Application.Run(new MauiResXResourceReaderTests(args));
        }

        protected override void InitTest(TParams p)
        {
            base.InitTest(p);
            resXReader = new ResXResourceReader("Resources.resx");
        }
        #endregion

        //==========================================
        // Scenarios
        //==========================================
        #region Scenarios
        //[Scenario(false)]
        [Scenario(true)]
        ScenarioResult TestSetGetBasePath(TParams p, string value)
        {
            resXReader.BasePath = value;
            return new ScenarioResult(value, resXReader.BasePath, "BasePath didn't match set value", p.log);
        }

        //[Scenario("Get BasePath")]
        [Scenario(true)]
        public ScenarioResult GetBasePath(TParams p)
        {
            String basePathString = p.ru.GetString(10);
            return TestSetGetBasePath(p, basePathString);
        }

        //[Scenario("Set BasePath to null")]
        [Scenario(true)]
        public ScenarioResult SetNull(TParams p)
        {
            return TestSetGetBasePath(p, null);
        }

        //[Scenario("Set BasePath to empty string")]
        [Scenario(true)]
        public ScenarioResult SetEmpty(TParams p)
        {
            return TestSetGetBasePath(p, "");
        }

        //[Scenario("Set BasePath to valid path")]
        [Scenario(true)]
        public ScenarioResult SetValidPath(TParams p)
        {
            return TestSetGetBasePath(p, ".");
        }

        //[Scenario("Set BasePath to valid long path")]
        [Scenario(true)]
        public ScenarioResult SetLongValidPath(TParams p)
        {
            return TestSetGetBasePath(p, p.ru.GetString(1000));
        }

        //[Scenario("Set BasePath to valid absolute path")]
        [Scenario(true)]
        public ScenarioResult SetValidAbsolute(TParams p)
        {
            return TestSetGetBasePath(p, "d:\\");
        }

        //[Scenario("Get useResXDataNodes")]
        [Scenario(true)]
        public ScenarioResult GetUseResXDataNodes(TParams p)
        {
            return SetUseResXDataNodes(p);
        }

        //[Scenario("Set useResXDataNodes")]
        [Scenario(true)]
        public ScenarioResult SetUseResXDataNodes(TParams p)
        {
            bool expected = !resXReader.UseResXDataNodes;
            resXReader.UseResXDataNodes = !resXReader.UseResXDataNodes;
            return new ScenarioResult(expected, resXReader.UseResXDataNodes, "UseResXDataNodes didn't match set value", p.log);
        }

        #endregion
    }
}
// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ Get BasePath
//@ Set BasePath to null
//@ Set BasePath to empty string
//@ Set BasePath to valid path
//@ Set BasePath to valid long path
//@ Set BasePath to valid absolute path
//@ Get useResXDataNodes
//@ Set useResXDataNodes
