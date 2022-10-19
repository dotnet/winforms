// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

internal static partial class Interop
{
    internal static partial class Ole32
    {
        [DllImport(Libraries.Oleaut32, PreserveSig = false)]
        [return: MarshalAs(UnmanagedType.Interface)]
        private static extern unsafe object OleCreatePictureIndirect(PICTDESC* pictdesc, in Guid refiid, BOOL fOwn);

        [DllImport(Libraries.Oleaut32, EntryPoint = "OleCreatePictureIndirect")]
        private static extern unsafe int OleCreatePictureIndirectRaw(PICTDESC* pictdesc, Guid* refiid, BOOL fOwn, IntPtr* lplpvObj);

        /// <param name="fOwn">
        ///  <see langref="true"/> if the picture object is to destroy its picture when the object is destroyed.
        ///  (The picture handle in the <paramref name="pictdesc"/>.)
        /// </param>
        public static unsafe object OleCreatePictureIndirect(ref PICTDESC pictdesc, in Guid refiid, BOOL fOwn)
        {
            pictdesc.cbSizeofstruct = (uint)sizeof(PICTDESC);
            fixed (PICTDESC* p = &pictdesc)
            {
                return OleCreatePictureIndirect(p, in refiid, fOwn);
            }
        }

        public static unsafe object OleCreatePictureIndirect(Guid* refiid)
        {
            IntPtr lpPicture = IntPtr.Zero;
            int errorCode = OleCreatePictureIndirectRaw(null, refiid, true, &lpPicture);
            if (errorCode < 0)
            {
                Marshal.ThrowExceptionForHR(errorCode);
            }

            return WinFormsComWrappers.Instance
                .GetOrCreateObjectForComInstance(lpPicture, CreateObjectFlags.UniqueInstance);
        }
    }
}
