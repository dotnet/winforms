// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Moq;
using Xunit;

namespace System.Windows.Forms.Metafiles.Tests
{
    public class RepeatValidatorTests
    {
        [Fact]
        public unsafe void RepeatValidator_Validate_complete_should_be_expected()
        {
            Mock<IEmfValidator> emfValidator = new();
            RepeatValidator repeatValidator = new(emfValidator.Object, 2);

            EmfRecord emfRecord = new();

            repeatValidator.Validate(ref emfRecord, state: null!, out bool complete);
            Assert.False(complete);

            // call again - we'll be at zero now
            repeatValidator.Validate(ref emfRecord, state: null!, out complete);
            Assert.True(complete);

            // call again - this will put us at negative count
            repeatValidator.Validate(ref emfRecord, state: null!, out complete);
            Assert.True(complete);
        }
    }
}
