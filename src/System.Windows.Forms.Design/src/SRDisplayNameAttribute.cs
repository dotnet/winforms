// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
namespace System.Windows.Forms.Design
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Event | AttributeTargets.Class |
                    AttributeTargets.Method)]
    internal sealed class SRDisplayNameAttribute : DisplayNameAttribute
    {
        private bool replaced;

        /// <summary>
        ///     Constructs a new sys display name.
        /// </summary>
        /// <param name='displayName'>
        ///     description text.
        public SRDisplayNameAttribute(string displayName) : base(displayName)
        {
        }

        /// <summary>
        ///     Retrieves the description text.
        /// </summary>
        /// <returns>
        ///     description
        /// </returns>
        public override string DisplayName
        {
            get
            {
                if (!replaced)
                {
                    replaced = true;
                    DisplayNameValue = base.DisplayName;
                }

                return base.DisplayName;
            }
        }
    }
}
