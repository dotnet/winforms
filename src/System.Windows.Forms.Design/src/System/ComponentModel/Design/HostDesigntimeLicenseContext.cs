﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.ComponentModel.Design
{
    /// <summary>
    /// This class will provide a license context that the LicenseManager can use to get to the design time services, like ITypeResolutionService.
    /// </summary>
    internal class HostDesigntimeLicenseContext : DesigntimeLicenseContext
    {
        private IServiceProvider provider;

        public HostDesigntimeLicenseContext(IServiceProvider provider)
        {
            this.provider = provider;
        }

        public override object GetService(Type serviceClass)
        {
            return provider.GetService(serviceClass);
        }
    }
}
