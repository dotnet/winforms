// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    using System;
  
    /// <devdoc>
    ///    Comprises the information specifying how acceleration should be performed
    ///     on a Windows up-down control when the up/down button is pressed for certain
    ///     amount of time.
    /// </devdoc>
    public class NumericUpDownAcceleration
    {
        private int seconds;      // Ideally we would use UInt32 but it is not CLS-compliant.
        private decimal increment;  // Ideally we would use UInt32 but NumericUpDown uses Decimal values.

        public NumericUpDownAcceleration(int seconds, decimal increment)
        {
            if (seconds < 0)
            {
                throw new ArgumentOutOfRangeException(nameof(seconds), seconds, SR.NumericUpDownLessThanZeroError);
            }

            if (increment < decimal.Zero)
            {
                throw new ArgumentOutOfRangeException(nameof(increment), increment, SR.NumericUpDownLessThanZeroError);
            }

            this.seconds   = seconds;
            this.increment = increment;
        }

        /// <devdoc>
        ///     Determines the amount of time for the UpDown control to wait to set the increment
        ///     step when holding the up/down button.
        /// </devdoc>
        public int Seconds
        {
            get
            {
                return this.seconds;
            }
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(seconds), value, SR.NumericUpDownLessThanZeroError);
                }
                this.seconds = value;
            }
        }

        /// <devdoc>
        ///     Determines the amount to increment by.
        /// </devdoc>
        public decimal Increment {

            get 
            {
                return this.increment;
            }

            set
            {
                if (value < decimal.Zero)
                {
                    throw new ArgumentOutOfRangeException(nameof(increment), value, SR.NumericUpDownLessThanZeroError);
                }
                this.increment = value;
            }
        }
    }

}
