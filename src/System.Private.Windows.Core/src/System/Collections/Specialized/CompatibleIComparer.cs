// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Collections.Specialized;

internal class BackCompatibleStringComparer : IEqualityComparer<string>
{
    internal static IEqualityComparer<string> Default { get; } = new BackCompatibleStringComparer();

    internal BackCompatibleStringComparer()
    {
    }

    // For backcompat
    public int GetHashCode([DisallowNull] string obj)
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

    public bool Equals(string? x, string? y)
    {
        return object.Equals(x, y);
    }

    public virtual int GetHashCode(object o)
    {
        if (o is not string obj)
        {
            return o.GetHashCode();
        }

        return GetHashCode(obj);
    }
}
