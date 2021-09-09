// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Xunit;
using static System.Windows.Forms.TrackBar;

namespace System.Windows.Forms.Tests
{
    public class TrackBar_TrackBarChildAccessibleObjectTests
    {
        [WinFormsFact]
        public void TrackBarChildAccessibleObject_Ctor_OwnerTrackBarCannotBeNull()
        {
            Assert.Throws<ArgumentNullException>(() => new SubObject(null));
        }

        private class SubObject : TrackBarChildAccessibleObject
        {
            public SubObject(TrackBar owningTrackBar) : base(owningTrackBar)
            { }
        }
    }
}
