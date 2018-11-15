﻿using Xunit;
using System.Drawing;

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
        public void AutoScaleBaseSize(System.Drawing.Size expected)
        {
            // arrange
            var contCont = new ContainerControl();

            // act
            if (expected.Width < 0 || expected.Height < 0)
            {
                Assert.Throws<ArgumentOutOfRangeException>(() => contCont.AutoScaleDimensions = expected);
            }
            else
            {
                contCont.AutoScaleDimensions = expected;
                var actual = contCont.AutoScaleDimensions;

                // assert
                Assert.Equal(expected, actual);
            }

        }

    }
}
