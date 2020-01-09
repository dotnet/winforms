// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    ///  Determines the exception mode of NativeWindow's WndProc method. Pass
    ///  a value of this enum into SetUnhandledExceptionMode to control how
    ///  new NativeWindow objects handle exceptions.
    /// </summary>
    public enum UnhandledExceptionMode
    {
        Automatic,
        ThrowException,
        CatchException
    }
}
