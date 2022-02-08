// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;
using Xunit;

namespace System
{
    public class ConditionalWinFormsTheoryAttribute : WinFormsTheoryAttribute
    {
        private Architecture _unsupportedArchitecture;
        private bool _isSkipRequired;
        private string _skipReason;

        public override string Skip
        {
            get => base.Skip;
            set
            {
                _skipReason = value;

                if (_isSkipRequired)
                {
                    base.Skip = value;
                }

                // Don't skip a test case if the skip condition is false.
            }
        }

        /// <summary>
        ///  Defines the architecture to skip the test on. This is complementary to <see cref="Skip"/> parameter.
        /// </summary>
        public Architecture UnsupportedArchitecture
        {
            get => _unsupportedArchitecture;
            set
            {
                _unsupportedArchitecture = value;
                _isSkipRequired = RuntimeInformation.ProcessArchitecture == value;
                if (_isSkipRequired && _skipReason is not null)
                {
                    Skip = _skipReason;
                }
            }
        }
    }
}
