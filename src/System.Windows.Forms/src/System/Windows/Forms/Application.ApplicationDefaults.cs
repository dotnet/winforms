// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections.Generic;

namespace System.Windows.Forms
{
    public sealed partial class Application
    {
        public class ApplicationDefaults
        {
            internal ApplicationDefaults()
            {
                DefaultProperties = new Dictionary<string, object>();
            }

            internal Dictionary<string, object> DefaultProperties { get; }
        }
    }
}
