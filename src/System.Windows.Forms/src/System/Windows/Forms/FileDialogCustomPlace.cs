// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Security;
using System.Security.Permissions;
using System.Text;

namespace System.Windows.Forms
{
//Sample Guids
//          internal const string ComputerFolder = "0AC0837C-BBF8-452A-850D-79D08E667CA7";
//          internal const string Favorites = "1777F761-68AD-4D8A-87BD-30B759FA33DD";
//          internal const string Documents = "FDD39AD0-238F-46AF-ADB4-6C85480369C7";
//          internal const string Profile = "5E6C858F-0E22-4760-9AFE-EA3317B67173";

    public class FileDialogCustomPlace
    {
        private string _path = "";
        private Guid   _knownFolderGuid = Guid.Empty;

        public FileDialogCustomPlace(string path)
        {
            this.Path = path;
        }

        public FileDialogCustomPlace(Guid knownFolderGuid)
        {
            this.KnownFolderGuid = knownFolderGuid; 
        }

        public string Path
        {
            get
            {
                if (string.IsNullOrEmpty(this._path))
                {
                    return String.Empty;
                }
                return this._path;
            }
            set
            {
                this._path = value ?? "";
                this._knownFolderGuid = Guid.Empty;
            }
        }

        public Guid KnownFolderGuid
        {
            get
            {
                return this._knownFolderGuid;
            }
            set
            {
                this._path = String.Empty;
                this._knownFolderGuid = value;
            }
        }

        public override string ToString()
        {
            return string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0} Path: {1} KnownFolderGuid: {2}", base.ToString(), this.Path, this.KnownFolderGuid);
        }

        internal FileDialogNative.IShellItem GetNativePath()
        {
            //This can throw in a multitude of ways if the path or Guid doesn't correspond
            //to an actual filesystem directory.  Caller is responsible for handling these situations.
            string filePathString = "";
            if (!string.IsNullOrEmpty(this._path))
            {
                filePathString = this._path;
            }
            else
            {
                filePathString = GetFolderLocation(this._knownFolderGuid);
            }

            if (string.IsNullOrEmpty(filePathString))
            {
                return null;
            }
            else
            {
                return FileDialog.GetShellItemForPath(filePathString);
            }
        }

        private static string GetFolderLocation(Guid folderGuid)
        {
            //returns a null string if the path can't be found

            //SECURITY: This exposes the filesystem path of the GUID.  The returned value
            // must not be made available to user code.

            if (!UnsafeNativeMethods.IsVista)
            { 
                return null;
            }

            StringBuilder path = new StringBuilder();

            int result = UnsafeNativeMethods.Shell32.SHGetFolderPathEx(ref folderGuid, 0, IntPtr.Zero, path);
            if (NativeMethods.S_OK == result) 
            {
                string ret = path.ToString();
                return ret;
            }
            else
            {
                // 0x80070002 is an explicit FileNotFound error.
                return null;
            }
        }
    }
}