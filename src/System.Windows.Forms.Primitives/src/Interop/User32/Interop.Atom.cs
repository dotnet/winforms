// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;

internal static partial class Interop
{
    /// <summary>
    ///  Simple wrapper for an ATOM
    /// </summary>
    public struct Atom
    {
        // #define MAXINTATOM 0xC000
        // #define MAKEINTATOM(i)  (LPTSTR)((ULONG_PTR)((WORD)(i)))
        // #define INVALID_ATOM ((ATOM)0)

        // Strange uses for window class atoms
        // https://blogs.msdn.microsoft.com/oldnewthing/20080501-00/?p=22503/

        public ushort ATOM;

        public Atom(ushort atom) => ATOM = atom;

        public static Atom Null = new Atom(0);

        public bool IsValid => ATOM != 0;

        public static implicit operator uint(Atom atom) => atom.ATOM;
        public static implicit operator Atom(IntPtr atom) => new Atom((ushort)atom);
    }
}
