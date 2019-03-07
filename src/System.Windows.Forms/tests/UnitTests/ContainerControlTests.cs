// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using System.Drawing;
using WinForms.Common.Tests;

namespace System.Windows.Forms.Tests
{
    public class ContainerControlTests
    {
        /// <summary>
        /// Data for the AutoScaleBaseSize test
        /// </summary>
        public static TheoryData<System.Drawing.Size> AutoScaleBaseSizeData =>
            TestHelper.GetSizeTheoryData();

        [Theory]
        [MemberData(nameof(AutoScaleBaseSizeData))]
        public void ContainerControl_AutoScaleBaseSize(System.Drawing.Size expected)
        {
            var contCont = new ContainerControl();

            if (expected.Width < 0 || expected.Height < 0)
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => contCont.AutoScaleDimensions = expected);
            }
            else
            {
                contCont.AutoScaleDimensions = expected;
                var actual = contCont.AutoScaleDimensions;

                Assert.Equal(expected, actual);
            }

        }

    }
}
