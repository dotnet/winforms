// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

/// <remarks>
///  <para>
///   Sample Guids
///  </para>
///  <list type="bullet">
///   <item><description>ComputerFolder: "0AC0837C-BBF8-452A-850D-79D08E667CA7"</description></item>
///   <item><description>Favorites: "1777F761-68AD-4D8A-87BD-30B759FA33DD"</description></item>
///   <item><description>Documents: "FDD39AD0-238F-46AF-ADB4-6C85480369C7"</description></item>
///   <item><description>Profile: "5E6C858F-0E22-4760-9AFE-EA3317B67173"</description></item>
///  </list>
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

    internal unsafe IShellItem* GetNativePath()
    {
        string filePathString;
        if (!string.IsNullOrEmpty(_path))
        {
            filePathString = _path;
        }
        else
        {
            fixed (char* path = filePathString)
            fixed (Guid* reference = &_knownFolderGuid)
            {
                int result = PInvoke.SHGetKnownFolderPath(reference, 0, HANDLE.Null, (PWSTR*)path);
                if (result == 0)
                {
                    return null;
                }
            }
        }

        return PInvoke.SHCreateShellItem(filePathString);
    }
}
