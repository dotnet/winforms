// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

internal static partial class Interop
{
    internal static partial class User32
    {
        public static class RegisteredMessage
        {
            private static uint s_wmMouseEnterMessage;
            private static uint s_wmUnSubclass;

            public static WM WM_MOUSEENTER
            {
                get
                {
                    if (s_wmMouseEnterMessage == 0)
                    {
                        s_wmMouseEnterMessage = (uint)RegisterWindowMessageW("WinFormsMouseEnter");
                    }

                    return (WM)s_wmMouseEnterMessage;
                }
            }

            public static WM WM_UIUNSUBCLASS
            {
                get
                {
                    if (s_wmUnSubclass == 0)
                    {
                        s_wmUnSubclass = (uint)RegisterWindowMessageW("WinFormsUnSubclass");
                    }

                    return (WM)s_wmUnSubclass;
                }
            }
        }
    }
}
