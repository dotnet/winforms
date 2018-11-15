// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Resources;

namespace System
{
    internal static partial class SR
    {   
        public static object GetObject(string name)
        {
            object resourceObject = null;
            try { resourceObject = ResourceManager.GetObject(name); }
            catch (MissingManifestResourceException) { }
            return resourceObject;
        }
    }
}