// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using WinForms.Common.Tests;

namespace System.Windows.Forms.Tests
{
    public class DpiUnsafeNativeMethodsTests
    {
        

        /*/// <summary>
        /// Data for the TryFindDpiAwarenessContextsEqual test
        /// </summary>
        public static TheoryData TryFindDpiAwarenessContextsEqualData =>
            CommonTestHelper.GetEnumTheoryData<DpiAwarenessContext>();
        

        Theory]
        [MemberData(nameof(TryFindDpiAwarenessContextsEqualData))]
        internal void DpiUnsafeNativeMethods_TryFindDpiAwarenessContextsEqual(DpiAwarenessContext item)
        { 
            Assert.True(DpiUnsafeNativeMethods.TryFindDpiAwarenessContextsEqual(item, item));
        }
    

        [Fact]
        public void DpiUnsafeNativeMethods_AreDpiAwarenessContextsEqualNotForUnspecified()
        {
            Assert.False(DpiUnsafeNativeMethods.AreDpiAwarenessContextsEqual(DpiAwarenessContext.DPI_AWARENESS_CONTEXT_UNSPECIFIED, DpiAwarenessContext.DPI_AWARENESS_CONTEXT_UNSPECIFIED));
        }

        /// <summary>
        /// Data for the TrySetThreadDpiAwarenessContextGetSet test
        /// </summary>
        public static TheoryData TrySetThreadDpiAwarenessContextGetSetData =>
            CommonTestHelper.GetEnumTheoryData<DpiAwarenessContext>();

        [Theory]
        [MemberData(nameof(TrySetThreadDpiAwarenessContextGetSetData))]
        internal void DpiUnsafeNativeMethods_TrySetThreadDpiAwarenessContextGetSet(DpiAwarenessContext item)
        {
            if (item != DpiAwarenessContext.DPI_AWARENESS_CONTEXT_UNSPECIFIED)
            {
                DpiUnsafeNativeMethods.TrySetThreadDpiAwarenessContext(item);

                Assert.True(DpiUnsafeNativeMethods.TryFindDpiAwarenessContextsEqual(item, DpiUnsafeNativeMethods.TryGetThreadDpiAwarenessContext()));
            }
            else
            {
                var ex = Assert.Throws<ArgumentException>(() => DpiUnsafeNativeMethods.TrySetThreadDpiAwarenessContext(DpiAwarenessContext.DPI_AWARENESS_CONTEXT_UNSPECIFIED));
                Assert.Equal(DpiAwarenessContext.DPI_AWARENESS_CONTEXT_UNSPECIFIED.ToString(), ex.ParamName);
            }
        }*/

    }
}
