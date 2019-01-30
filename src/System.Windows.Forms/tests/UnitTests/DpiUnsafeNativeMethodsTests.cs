// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using WinForms.Common.Tests;

namespace System.Windows.Forms.Tests
{
    public class DpiUnsafeNativeMethodsTests
    {
        /// <summary>
        /// Data for the TryFindDpiAwarenessContextsEqual test
        /// </summary>
        public static TheoryData TryFindDpiAwarenessContextsEqualData =>
            CommonTestHelper.GetEnumTheoryData<DpiAwarenessContext>();
        

        [Theory]
        [MemberData(nameof(TryFindDpiAwarenessContextsEqualData))]
        internal void DpiUnsafeNativeMethods_TryFindDpiAwarenessContextsEqual(DpiAwarenessContext item)
        { 
            Assert.True(DpiUnsafeNativeMethods.TryFindDpiAwarenessContextsEqual(item, item));
        }

        [Theory]
        [MemberData(nameof(TryFindDpiAwarenessContextsEqualData))]
        internal void DpiUnsafeNativeMethods_TryFindDpiAwarenessContextsEqual_CompareToNull(DpiAwarenessContext item)
        { 
            Assert.False(DpiUnsafeNativeMethods.TryFindDpiAwarenessContextsEqual(item, null));
            Assert.False(DpiUnsafeNativeMethods.TryFindDpiAwarenessContextsEqual(null, item));
        }

        [Fact]
        internal void DpiUnsafeNativeMethods_TryFindDpiAwarenessContextsEqual_BothNull()
        { 
            Assert.True(DpiUnsafeNativeMethods.TryFindDpiAwarenessContextsEqual(null, null));
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
            DpiUnsafeNativeMethods.TrySetThreadDpiAwarenessContext(item);

            Assert.True(DpiUnsafeNativeMethods.TryFindDpiAwarenessContextsEqual(item, DpiUnsafeNativeMethods.TryGetThreadDpiAwarenessContext()));           
        }

        [Fact]
        internal void DPiUnsafeNativeMethods_TrySetThreadDpiAwarenessContext_Null()
        {
            Assert.Throws<ArgumentNullException>(() => DpiUnsafeNativeMethods.TrySetThreadDpiAwarenessContext(null));
        }

    }
}
