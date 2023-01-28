// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace System.Windows.Forms.ComponentModel.Com2Interop
{
    internal partial class Com2PropertyDescriptor
    {
        private readonly ref struct ValidityScope
        {
#pragma warning disable IDE0036 // Order modifiers (currently broken, required must come first)
            required public Com2Properties? Properties { get; init; }
#pragma warning restore IDE0036

            [SetsRequiredMembers]
            public ValidityScope(Com2Properties? properties)
            {
                Properties = properties;

                // Should never be null- but we can't easily express this with null annotation as it is a post
                // initialization association.
                Debug.Assert(properties is not null);
                if (Properties is not null)
                {
                    Properties.AlwaysValid = Properties.CheckValidity();
                }
            }

            public void Dispose()
            {
                if (Properties is not null)
                {
                    Properties.AlwaysValid = false;
                }
            }
        }
    }
}
