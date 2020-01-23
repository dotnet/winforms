// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

namespace System.Windows.Forms
{
    // According to our data base, following public class has no usage at runtime.
    // Keeping the skeleton to unblock compile scenarios, if any.
    // And to receive feedback if there are any users  of this class that we do not know
    public sealed class WindowsFormsSection
    {
        internal static WindowsFormsSection GetSection()
        {
            throw new NotImplementedException();
        }

        public WindowsFormsSection()
        {
            throw new NotImplementedException();
        }

        public bool JitDebugging
        {
            get => throw new NotImplementedException();
            set => throw new NotImplementedException();
        }
    }
}
