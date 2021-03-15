// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable enable

using System;
using System.ComponentModel;

namespace WinformsControlsTest
{
    public partial class Dialogs
    {
        private sealed class ExposedClientGuidMetadata
        {
            [Browsable(true)]
            [TypeConverter(typeof(ClientGuidConverter))]
            public Guid? ClientGuid { get; set; }
        }
    }
}
