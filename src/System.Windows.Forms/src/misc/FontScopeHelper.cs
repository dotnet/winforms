// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    ///  FontScopeHelper class, that help scoping the control Font for the code being executed under this scope.
    ///  If control Font was already set, this will be a no-op.
    ///  Work around for https://github.com/dotnet/winforms/issues/4762
    /// </summary>
    internal static class FontScopeHelper
    {
        /// <summary>
        ///  Enters Font scope during which the current control's Font property is set
        /// <paramref name="control"/>
        /// </summary>
        public static IDisposable EnterControlFontScope(Control control)
        {
            return new FontScope(control);
        }

        /// <summary>
        ///  Class that help setting control Font scope
        /// </summary>
        private class FontScope : IDisposable
        {
            private bool fontScopeIsSet;
            private Control scopedControl;

            /// <summary>
            ///  Enters Control Font scope
            /// </summary>
            public FontScope(Control control)
            {
                scopedControl = control ?? throw new ArgumentNullException(nameof(control));
                var local = scopedControl.Properties.GetObject(Control.s_fontProperty);
                if (local is null)
                {
                    scopedControl.Properties.SetObject(Control.s_fontProperty, scopedControl.Font);
                    fontScopeIsSet = true;
                }
            }

            /// <summary>
            /// Reset Font
            /// </summary>
            public void Dispose()
            {
                if (fontScopeIsSet)
                {
                    scopedControl.Properties.SetObject(Control.s_fontProperty, null);
                }
            }
        }
    }
}
