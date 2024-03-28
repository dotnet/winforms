// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows.Forms.IntegrationTests.Common;
using System.Xml.Serialization;
using static Interop.User32;

namespace System.Windows.Forms.Maui.IntegrationTests
{
    /// <summary>
    ///  This class is used to run maui tests (exe) and gather the results.
    ///  Maui tests are executables that run one or more scenarios, and the results are
    ///  stored in a results.log. This class handles running the exe, deserializing the log
    ///  into a TestCase object, and returning that object to the caller.
    ///
    /// </summary>
    public class MauiTestRunner
    {
        /// <summary>
        ///  Run a Maui test at the specified path and return the results
        /// </summary>
        /// <param name="path">The path to execute</param>
        /// <returns>The test results</returns>
        public TestResult RunTest(string path)
        {
            // run the maui test exe, making sure to set the cwd of the process
            // (so the results.log gets generated in the right place)
            var process = TestHelpers.StartProcess(path, true);
            SetForegroundWindow(process.Handle);
            process.WaitForExit();

            // deserialize the results.log into a TestResult instance
            var logDir = Path.GetDirectoryName(path);
            var logPath = Path.Combine(logDir, "results.log");
            using (StreamReader reader = new(logPath))
            {
                XmlSerializer serializer = new(typeof(TestResult));
                if (!(serializer.Deserialize(reader) is TestResult testResult))
                    throw new InvalidOperationException("Maui results log can't be parsed into a TestResult, file is probably malformed");

                return testResult;
            }
        }
    }

    #region Serialization Classes

    // The following are used to deserialize test result xml to C# classes
    /// <remarks/>
    [Serializable()]
    [XmlRoot("Testcase")]
    public class TestResult
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        [XmlElement()]
        public ScenarioGroup ScenarioGroup { get; set; }

        [XmlElement()]
        public FinalResults FinalResults { get; set; }
    }

    [Serializable()]
    public class ScenarioGroup
    {
        [XmlAttribute("name")]
        public string Name { get; set; }

        /// <remarks/>
        [XmlElement("Scenario")]
        public Scenario[] Scenarios { get; set; }

        [XmlElement()]
        public ClassResults ClassResults { get; set; }
    }

    [Serializable()]
    public class Scenario
    {
        private string _name;

        [XmlAttribute("method")]
        public string Method { get; set; }

        // strip the parens from the name
        [XmlAttribute("name")]
        public string Name
        {
            get => _name;
            set
            {
                if (value.EndsWith("()"))
                {
                    _name = value.Remove(value.Length - 2);
                }
            }
        }

        [XmlElement()]
        public Result Result { get; set; }

        /// <remarks/>
        [XmlText()]
        public string[] Text { get; set; }
    }

    [Serializable()]
    public class Result
    {
        [XmlAttribute("type")]
        public string Type { get; set; }
    }

    [Serializable()]
    public partial class ClassResults
    {
        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlAttribute("total")]
        public int Total { get; set; }

        [XmlAttribute("fail")]
        public int Fail { get; set; }
    }

    [Serializable()]
    public class FinalResults
    {
        [XmlAttribute("type")]
        public string Type { get; set; }

        [XmlAttribute("total")]
        public int Total { get; set; }

        [XmlAttribute("fail")]
        public int Fail { get; set; }
    }

    #endregion Serialization Classes
}
