// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    using System.Runtime.CompilerServices;

    internal static class LocalAppContextSwitches
    {
        internal const string DontSupportReentrantFilterMessageSwitchName = @"Switch.System.Windows.Forms.DontSupportReentrantFilterMessage";
        internal const string EnableVisualStyleValidationSwitchName = @"Switch.System.Windows.Forms.EnableVisualStyleValidation";
        private static int _dontSupportReentrantFilterMessage;
        private static int _enableVisualStyleValidation;

        public static bool DontSupportReentrantFilterMessage
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return LocalAppContext.GetCachedSwitchValue(LocalAppContextSwitches.DontSupportReentrantFilterMessageSwitchName, ref _dontSupportReentrantFilterMessage);
            }
        }

        public static bool EnableVisualStyleValidation
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return LocalAppContext.GetCachedSwitchValue(LocalAppContextSwitches.EnableVisualStyleValidationSwitchName, ref _enableVisualStyleValidation);
            }
        }
    }
}
