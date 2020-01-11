﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel.Design;
using Xunit;

namespace System.Windows.Forms.Design.Editors.Tests.Serialization
{
    public class SerializableAttributeTests : IClassFixture<ThreadExceptionFixture>
    {
        [Fact]
        public void EnsureSerializableAttribute()
        {
            BinarySerialization.EnsureSerializableAttribute(
                typeof(ArrayEditor).Assembly,
                new HashSet<string>
                {
                    // This Assembly does not have any serializable types.
                });
        }
    }
}
