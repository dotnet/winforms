// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Windows.Forms;

internal static class AutomationMessages
{
    internal const int PGM_GETBUTTONCOUNT = (int)PInvoke.WM_USER + 0x50;
    internal const int PGM_GETBUTTONSTATE = (int)PInvoke.WM_USER + 0x52;
    internal const int PGM_SETBUTTONSTATE = (int)PInvoke.WM_USER + 0x51;
    internal const int PGM_GETBUTTONTEXT = (int)PInvoke.WM_USER + 0x53;
    internal const int PGM_GETBUTTONTOOLTIPTEXT = (int)PInvoke.WM_USER + 0x54;
    internal const int PGM_GETROWCOORDS = (int)PInvoke.WM_USER + 0x55;
    internal const int PGM_GETVISIBLEROWCOUNT = (int)PInvoke.WM_USER + 0x56;
    internal const int PGM_GETSELECTEDROW = (int)PInvoke.WM_USER + 0x57;
    internal const int PGM_SETSELECTEDTAB = (int)PInvoke.WM_USER + 0x58; // DO NOT CHANGE THIS : VC uses it!
    internal const int PGM_GETTESTINGINFO = (int)PInvoke.WM_USER + 0x59;

    /// <summary>
    ///  Writes the specified text into a temporary file of the form %TEMP%\"Maui.[file id].log", where
    ///  'file id' is a unique id that is return by this method.
    ///  This is to support MAUI interaction with the PropertyGrid control and MAUI should remove the
    ///  file after used.
    /// </summary>
    public static IntPtr WriteAutomationText(string? text)
    {
        IntPtr fileId = IntPtr.Zero;
        string? fullFileName = GenerateLogFileName(ref fileId);

        if (fullFileName is not null)
        {
            try
            {
                FileStream fs = new(fullFileName, FileMode.Create, FileAccess.Write);
                StreamWriter sw = new(fs);
                sw.WriteLine(text);
                sw.Dispose();
                fs.Dispose();
            }
            catch
            {
                fileId = IntPtr.Zero;
            }
        }

        return fileId;
    }

    /// <summary>
    ///  Writes the contents of a test file as text. This file needs to have the following naming convention:
    ///  %TEMP%\"Maui.[file id].log", where 'file id' is a unique id sent to this window.
    ///  This is to support MAUI interaction with the PropertyGrid control and MAUI should create/delete this file.
    /// </summary>
    public static string? ReadAutomationText(IntPtr fileId)
    {
        Debug.Assert(fileId != IntPtr.Zero, "Invalid file Id");

        string? text = null;

        if (fileId != IntPtr.Zero)
        {
            string? fullFileName = GenerateLogFileName(ref fileId);
            Debug.Assert(File.Exists(fullFileName), "Automation log file does not exist");

            if (File.Exists(fullFileName))
            {
                try
                {
                    FileStream fs = new(fullFileName, FileMode.Open, FileAccess.Read);
                    StreamReader sr = new(fs);
                    text = sr.ReadToEnd();
                    sr.Dispose();
                    fs.Dispose();
                }
                catch
                {
                    text = null;
                }
            }
        }

        return text;
    }

    /// <summary>
    ///  Generate log file from id.
    /// </summary>
    private static string? GenerateLogFileName(ref IntPtr fileId)
    {
        string? fullFileName = null;

        string? filePath = Environment.GetEnvironmentVariable("TEMP");
        Debug.Assert(filePath is not null, "Could not get value of the TEMP environment variable");

        if (filePath is not null)
        {
            if (fileId == IntPtr.Zero) // Create id
            {
                Random rnd = new(DateTime.Now.Millisecond);
                fileId = new IntPtr(rnd.Next());
            }

            fullFileName = $"{filePath}\\Maui{fileId}.log";
        }

        return fullFileName;
    }
}
