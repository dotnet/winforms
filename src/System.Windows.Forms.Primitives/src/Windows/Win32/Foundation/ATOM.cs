// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace Windows.Win32.Foundation;

/// <summary>
///  Simple wrapper for an ATOM
/// </summary>
internal struct ATOM
{
    // #define MAXINTATOM 0xC000
    // #define MAKEINTATOM(i)  (LPTSTR)((ULONG_PTR)((WORD)(i)))
    // #define INVALID_ATOM ((ATOM)0)

    // Strange uses for window class atoms
    // https://blogs.msdn.microsoft.com/oldnewthing/20080501-00/?p=22503/

    public ushort Value;

    public ATOM(ushort atom) => Value = atom;

    public static ATOM Null { get; } = new(0);

    public readonly bool IsValid => Value != 0;

    public static implicit operator uint(ATOM atom) => atom.Value;
    public static implicit operator ATOM(IntPtr atom) => new((ushort)atom);
}
