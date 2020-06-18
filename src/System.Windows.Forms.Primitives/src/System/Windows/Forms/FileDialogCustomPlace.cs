// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics.CodeAnalysis;
using static Interop;
using static Interop.Shell32;

namespace System.Windows.Forms
{
    /// <remarks>
    ///  Sample Guids
    ///  ComputerFolder: "0AC0837C-BBF8-452A-850D-79D08E667CA7"
    ///  Favorites: "1777F761-68AD-4D8A-87BD-30B759FA33DD"
    ///  Documents: "FDD39AD0-238F-46AF-ADB4-6C85480369C7"
    ///  Profile: "5E6C858F-0E22-4760-9AFE-EA3317B67173"
    /// </remarks>
    public class FileDialogCustomPlace
    {
        private string _path = string.Empty;
        private Guid _knownFolderGuid = Guid.Empty;

        public FileDialogCustomPlace(string? path)
        {
            Path = path;
        }

        public FileDialogCustomPlace(Guid knownFolderGuid)
        {
            KnownFolderGuid = knownFolderGuid;
        }

        [AllowNull]
        public string Path
        {
            get => _path ?? string.Empty;
            set
            {
                _path = value ?? string.Empty;
                _knownFolderGuid = Guid.Empty;
            }
        }

        public Guid KnownFolderGuid
        {
            get => _knownFolderGuid;
            set
            {
                _path = string.Empty;
                _knownFolderGuid = value;
            }
        }

        public override string ToString()
        {
            return $"{base.ToString()} Path: {Path} KnownFolderGuid: {KnownFolderGuid}";
        }

        /// <remarks>
        ///  This can throw in a multitude of ways if the path or Guid doesn't correspond
        ///  to an actual filesystem directory.
        ///  The caller is responsible for handling these situations.
        /// </remarks>
        internal IShellItem? GetNativePath()
        {
            string filePathString;
            if (!string.IsNullOrEmpty(_path))
            {
                filePathString = _path;
            }
            else
            {
                int result = SHGetKnownFolderPath(ref _knownFolderGuid, 0, IntPtr.Zero, out filePathString);
                if (result == 0)
                {
                    return null;
                }
            }

            return GetShellItemForPath(filePathString);
        }
    }
}
