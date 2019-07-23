// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Collections.Specialized
{
    using System.Collections;

    internal class BackCompatibleStringComparer : IEqualityComparer
    {
        static internal IEqualityComparer Default = new BackCompatibleStringComparer();

        internal BackCompatibleStringComparer()
        {
        }

        //For backcompat
        public static int GetHashCode(string obj)
        {
            unsafe
            {
                fixed (char* src = obj)
                {
                    int hash = 5381;
                    int c;
                    char* szStr = src;

                    while ((c = *szStr) != 0)
                    {
                        hash = ((hash << 5) + hash) ^ c;
                        ++szStr;
                    }
                    return hash;
                }
            }
        }

        bool IEqualityComparer.Equals(object a, object b)
        {
            return Object.Equals(a, b);
        }

        public virtual int GetHashCode(object o)
        {
            if (!(o is string obj))
            {
                return o.GetHashCode();
            }

            return BackCompatibleStringComparer.GetHashCode(obj);
        }
    }
}
