// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using Windows.Win32.System.Com;

namespace Windows.Win32.Foundation
{
    internal readonly unsafe ref struct ComScope<T> where T : unmanaged, IUnknown.Interface
    {
        required public T* Value { get; init; }

        [SetsRequiredMembers]
        public ComScope(T* value) => Value = value;

        public static implicit operator T*(ComScope<T> scope) => scope.Value;

        public bool IsNull => Value is null;

        public void Dispose()
        {
            IUnknown* unknown = (IUnknown*)Value;
            if (unknown is not null)
            {
                unknown->Release();
            }
        }
    }
}
