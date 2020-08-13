// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Xunit;
using Xunit.Sdk;

namespace System.Windows.Forms.Maui.IntegrationTests
{
    /// <summary>
    ///  This is a custom xUnit memberdata attribute which allows us to execute code
    ///  before the data is resolved. It's needed because we want to read scenario names from
    ///  test results while still being able to use memberdata to parameterize the xUnit test,
    ///  but the normal MemberData attribute resolves before anything else (even the constructor)
    ///  is invoked.
    ///
    ///  This code is based on an example at https://tpodolak.com/blog/2017/06/19/xunit-memberdataattribute-generic-type-inheritance/
    /// </summary>
    [DataDiscoverer("Xunit.Sdk.MemberDataDiscoverer", "xunit.core")]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class MauiDataAttribute : MemberDataAttributeBase
    {
        /// <summary>
        ///  The name of the maui test project to get scenarios for
        /// </summary>
        private readonly string _projectName;

        // don't pass anything to the base class because we don't need it
        public MauiDataAttribute(string projectName) : base(null, null)
        {
            _projectName = projectName;

            // This makes it so visual studio doesn't try to populate the test explorer
            // with each maui scenario before the tests are actually executed.
            // This is important because once the data is retrieved before the tests are run,
            // it will never be updated again unless you do a full clean/rebuild.
            // So if you add a scenario to a maui test, for example, the new scenario won't show
            // up in the test explorer.
            DisableDiscoveryEnumeration = true;
        }

        /// <summary>
        ///  This is the method that actually returns the data that will be parameterized into the
        ///  xUnit test method. We actually don't use the base class logic at all,
        ///  we just get the scenarios for the specified project name and return them.
        /// </summary>
        /// <param name="testMethod">MethodInfo of the method being decorated</param>
        /// <returns>The test data</returns>
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            // get the scenarios
            var scenarios = MauiTestHelper.GetScenarios(_projectName);

            // convert the data to the expected format
            return scenarios.Select(x => new object[] { x });
        }

        /// <summary>
        ///  Required to derive from MemberDataAttributeBase
        /// </summary>
        /// <param name="testMethod">MethodInfo of the method being decorated</param>
        /// <param name="item">The item representing a single line of data</param>
        /// <returns>The item as an object array</returns>
        protected override object[] ConvertDataItem(MethodInfo testMethod, object item)
        {
            throw new NotImplementedException();
        }
    }
}
