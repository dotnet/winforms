// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    [SRDescription(nameof(SR.ICurrencyManagerProviderDescr))]
    public interface ICurrencyManagerProvider
    {
        /// <summary>
        ///  Return the main currency manager for this data source.
        /// </summary>
        CurrencyManager CurrencyManager { get; }

        /// <summary>
        ///  Return a related currency manager for specified data member on this data source.
        ///  If data member is null or empty, this method returns the data source's main currency
        ///  manager (ie. this method returns the same value as the CurrencyManager property).
        /// </summary>
        CurrencyManager GetRelatedCurrencyManager(string dataMember);
    }
}
