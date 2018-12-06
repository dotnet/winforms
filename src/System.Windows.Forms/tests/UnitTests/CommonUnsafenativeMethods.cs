// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using WinForms.Common.Tests;

namespace System.Windows.Forms.Tests
{
    public class CommonUnsafeNativeMethodsTests
    {
        

        /*/// <summary>
        /// Data for the TryFindDpiAwarenessContextsEqual test
        /// </summary>
        public static TheoryData TryFindDpiAwarenessContextsEqualData =>
            CommonTestHelper.GetEnumTheoryData<DpiAwarenessContext>();
        

        Theory]
        [MemberData(nameof(TryFindDpiAwarenessContextsEqualData))]
        internal void CommonUnsafeNativeMathods_TryFindDpiAwarenessContextsEqual(DpiAwarenessContext item)
        { 
            Assert.True(CommonUnsafeNativeMethods.TryFindDpiAwarenessContextsEqual(item, item));
        }
    

        [Fact]
        public void CommonUnsafeNativeMethods_AreDpiAwarenessContextsEqualNotForUnspecified()
        {
            Assert.False(CommonUnsafeNativeMethods.AreDpiAwarenessContextsEqual(DpiAwarenessContext.DPI_AWARENESS_CONTEXT_UNSPECIFIED, DpiAwarenessContext.DPI_AWARENESS_CONTEXT_UNSPECIFIED));
        }

        /// <summary>
        /// Data for the TrySetThreadDpiAwarenessContextGetSet test
        /// </summary>
        public static TheoryData TrySetThreadDpiAwarenessContextGetSetData =>
            CommonTestHelper.GetEnumTheoryData<DpiAwarenessContext>();

        [Theory]
        [MemberData(nameof(TrySetThreadDpiAwarenessContextGetSetData))]
        internal void CommonUnsafeNativeMethods_TrySetThreadDpiAwarenessContextGetSet(DpiAwarenessContext item)
        {
            if (item != DpiAwarenessContext.DPI_AWARENESS_CONTEXT_UNSPECIFIED)
            {
                CommonUnsafeNativeMethods.TrySetThreadDpiAwarenessContext(item);

                Assert.True(CommonUnsafeNativeMethods.TryFindDpiAwarenessContextsEqual(item, CommonUnsafeNativeMethods.TryGetThreadDpiAwarenessContext()));
            }
            else
            {
                var ex = Assert.Throws<ArgumentException>(() => CommonUnsafeNativeMethods.TrySetThreadDpiAwarenessContext(DpiAwarenessContext.DPI_AWARENESS_CONTEXT_UNSPECIFIED));
                Assert.Equal(DpiAwarenessContext.DPI_AWARENESS_CONTEXT_UNSPECIFIED.ToString(), ex.ParamName);
            }
        }*/

    }
}
