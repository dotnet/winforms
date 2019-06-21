// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Reflection;
using Xunit;
using Xunit.Sdk;

namespace System.Windows.Forms.Maui.IntegrationTests
{
    /// <summary>
    /// This is a custom xUnit memberdata attribute which allows us to execute arbitrary code
    /// before the data is resolved. It's needed because we want to read scenario names from
    /// test results while still being able to use memberdata to parameterize the xUnit test,
    /// but the normal MemberData attribute resolves before anything else (even the constructor)
    /// is invoked.
    /// 
    /// This code is based on an example at https://tpodolak.com/blog/2017/06/19/xunit-memberdataattribute-generic-type-inheritance/
    /// </summary>
    [DataDiscoverer("Xunit.Sdk.MemberDataDiscoverer", "xunit.core")]
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class CustomMemberDataAttribute : MemberDataAttributeBase
    {
        /// <summary>
        /// Constructor, just passes values through to the base class
        /// </summary>
        /// <param name="memberName">The name of the member holding the data</param>
        /// <param name="parameters">Additional params</param>
        public CustomMemberDataAttribute(string memberName, params object[] parameters)
        : base(memberName, parameters)
        {
        }

        /// <summary>
        /// This is the method that actually returns the data that will be parameterized into the
        /// xUnit test method
        /// </summary>
        /// <param name="testMethod">MethodInfo of the method being decorated</param>
        /// <returns>The test data</returns>
        public override IEnumerable<object[]> GetData(MethodInfo testMethod)
        {
            // get the type (class) containing the decorated method
            var type = MemberType ?? testMethod.ReflectedType;

            // run the class constructor, which is static
            System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(type.TypeHandle);

            // return data like normal from the base class
            return base.GetData(testMethod);
        }

        /// <summary>
        /// Required to derive from MemberDataAttributeBase
        /// </summary>
        /// <param name="testMethod">MethodInfo of the method being decorated</param>
        /// <param name="item">The item representing a single line of data</param>
        /// <returns>The item as an object array</returns>
        protected override object[] ConvertDataItem(MethodInfo testMethod, object item)
        {
            if (item == null)
            {
                return null;
            }

            var array = item as object[];
            if (array == null)
            {
                throw new ArgumentException(
                        $"Property {MemberName} on {MemberType ?? testMethod.ReflectedType} yielded an item that is not an object[]");
            }
            return array;
        }
    }
}
