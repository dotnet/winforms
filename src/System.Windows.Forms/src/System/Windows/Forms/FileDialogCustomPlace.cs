// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Security;
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
                    return string.Empty;
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
                this._path = string.Empty;
                this._knownFolderGuid = value;
            }
        }

        public override string ToString()
        {
            return string.Format(System.Globalization.CultureInfo.CurrentCulture, "{0} Path: {1} KnownFolderGuid: {2}", base.ToString(), this.Path, this.KnownFolderGuid);
        }

        internal FileDialogNative.IShellItem GetNativePath()
        {
            // This can throw in a multitude of ways if the path or Guid doesn't correspond
            // to an actual filesystem directory.
            // The Caller is responsible for handling these situations.
            string filePathString;
            if (!string.IsNullOrEmpty(_path))
            {
                filePathString = _path;
            }
            else
            {
                int result = Interop.Shell32.SHGetKnownFolderPath(ref _knownFolderGuid, 0, IntPtr.Zero, out filePathString);
                if (result == 0)
                {
                    return null;
                }
            }

            return FileDialog.GetShellItemForPath(filePathString);
        }
    }
}