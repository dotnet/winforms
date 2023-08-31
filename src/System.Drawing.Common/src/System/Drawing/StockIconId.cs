// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing;

#if NET8_0_OR_GREATER
/// <summary>
///  Icon identifiers for use with <see cref="SystemIcons.GetStockIcon(StockIconId, StockIconOptions)"/>.
/// </summary>
public enum StockIconId
{
    /// <summary>
    ///  Document (blank page), no associated program.
    /// </summary>
    DocumentNoAssociation = 0,      // SIID_DOCNOASSOC

    /// <summary>
    ///  Document with an associated program.
    /// </summary>
    DocumentWithAssociation = 1,    // SIID_DOCASSOC

    /// <summary>
    ///  Generic application with no custom icon.
    /// </summary>
    Application = 2,                // SIID_APPLICATION

    /// <summary>
    ///  Closed folder.
    /// </summary>
    Folder = 3,                     // SIID_FOLDER

    /// <summary>
    ///  Open folder.
    /// </summary>
    FolderOpen = 4,                 // SIID_FOLDEROPEN

    /// <summary>
    ///  5.25" floppy disk drive.
    /// </summary>
    Drive525 = 5,                   // SIID_DRIVE525

    /// <summary>
    ///  3.5" floppy disk drive.
    /// </summary>
    Drive35 = 6,                    // SIID_DRIVE35

    /// <summary>
    ///  Removable drive.
    /// </summary>
    DriveRemovable = 7,             // SIID_DRIVEREMOVE

    /// <summary>
    ///  Fixed drive.
    /// </summary>
    DriveFixed = 8,                 // SIID_DRIVEFIXED

    /// <summary>
    ///  Network drive.
    /// </summary>
    DriveNet = 9,                   // SIID_DRIVENET

    /// <summary>
    ///  Disabled network drive.
    /// </summary>
    DriveNetDisabled = 10,          // SIID_DRIVENETDISABLED

    /// <summary>
    ///  CD drive.
    /// </summary>
    DriveCD = 11,                   // SIID_DRIVECD

    /// <summary>
    ///  RAM disk drive.
    /// </summary>
    DriveRam = 12,                  // SIID_DRIVERAM

    /// <summary>
    ///  Entire network.
    /// </summary>
    World = 13,                     // SIID_WORLD

    /// <summary>
    ///  A computer on the network.
    /// </summary>
    Server = 15,                    // SIID_SERVER

    /// <summary>
    ///  Printer.
    /// </summary>
    Printer = 16,                   // SIID_PRINTER

    /// <summary>
    ///  My network places.
    /// </summary>
    MyNetwork = 17,                 // SIID_MYNETWORK

    /// <summary>
    ///  Find.
    /// </summary>
    Find = 22,                      // SIID_FIND

    /// <summary>
    ///  Help.
    /// </summary>
    Help = 23,                      // SIID_HELP

    /// <summary>
    ///  Overlay for shared items.
    /// </summary>
    Share = 28,                     // SIID_SHARE

    /// <summary>
    ///  Overlay for shortcuts to items.
    /// </summary>
    Link = 29,                      // SIID_LINK

    /// <summary>
    ///  Overlay for slow items.
    /// </summary>
    SlowFile = 30,                  // SIID_SLOWFILE

    /// <summary>
    ///  Empty recycle bin.
    /// </summary>
    Recycler = 31,                  // SIID_RECYCLER

    /// <summary>
    ///  Full recycle bin.
    /// </summary>
    RecyclerFull = 32,              // SIID_RECYCLERFULL

    /// <summary>
    ///  Audio CD media.
    /// </summary>
    MediaCDAudio = 40,              // SIID_MEDIACDAUDIO

    /// <summary>
    ///  Security lock.
    /// </summary>
    Lock = 47,                      // SIID_LOCK

    /// <summary>
    ///  AutoList.
    /// </summary>
    AutoList = 49,                  // SIID_AUTOLIST

    /// <summary>
    ///  Network printer.
    /// </summary>
    PrinterNet = 50,                // SIID_PRINTERNET

    /// <summary>
    ///  Server share.
    /// </summary>
    ServerShare = 51,               // SIID_SERVERSHARE

    /// <summary>
    ///  Fax printer.
    /// </summary>
    PrinterFax = 52,                // SIID_PRINTERFAX

    /// <summary>
    ///  Networked fax printer.
    /// </summary>
    PrinterFaxNet = 53,             // SIID_PRINTERFAXNET

    /// <summary>
    ///  Print to file.
    /// </summary>
    PrinterFile = 54,               // SIID_PRINTERFILE

    /// <summary>
    ///  Stack.
    /// </summary>
    Stack = 55,                     // SIID_STACK

    /// <summary>
    ///  SVCD media.
    /// </summary>
    MediaSVCD = 56,                 // SIID_MEDIASVCD

    /// <summary>
    ///  Folder containing other items.
    /// </summary>
    StuffedFolder = 57,             // SIID_STUFFEDFOLDER

    /// <summary>
    ///  Unknown drive.
    /// </summary>
    DriveUnknown = 58,              // SIID_DRIVEUNKNOWN

    /// <summary>
    ///  DVD drive.
    /// </summary>
    DriveDVD = 59,                  // SIID_DRIVEDVD

    /// <summary>
    ///  DVD media.
    /// </summary>
    MediaDVD = 60,                  // SIID_MEDIADVD

    /// <summary>
    ///  DVD-RAM media.
    /// </summary>
    MediaDVDRAM = 61,               // SIID_MEDIADVDRAM

    /// <summary>
    ///  DVD-RW media.
    /// </summary>
    MediaDVDRW = 62,                // SIID_MEDIADVDRW

    /// <summary>
    ///  DVD-R media.
    /// </summary>
    MediaDVDR = 63,                 // SIID_MEDIADVDR

    /// <summary>
    ///  DVD-ROM media.
    /// </summary>
    MediaDVDROM = 64,               // SIID_MEDIADVDROM

    /// <summary>
    ///  CD+ (Enhanced CD) media.
    /// </summary>
    MediaCDAudioPlus = 65,          // SIID_MEDIACDAUDIOPLUS

    /// <summary>
    ///  CD-RW media.
    /// </summary>
    MediaCDRW = 66,                 // SIID_MEDIACDRW

    /// <summary>
    ///  CD-R media.
    /// </summary>
    MediaCDR = 67,                  // SIID_MEDIACDR

    /// <summary>
    ///  Burning CD.
    /// </summary>
    MediaCDBurn = 68,               // SIID_MEDIACDBURN

    /// <summary>
    ///  Blank CD media.
    /// </summary>
    MediaBlankCD = 69,              // SIID_MEDIABLANKCD

    /// <summary>
    ///  CD-ROM media.
    /// </summary>
    MediaCDROM = 70,                // SIID_MEDIACDROM

    /// <summary>
    ///  Audio files.
    /// </summary>
    AudioFiles = 71,                // SIID_AUDIOFILES

    /// <summary>
    ///  Image files.
    /// </summary>
    ImageFiles = 72,                // SIID_IMAGEFILES

    /// <summary>
    ///  Video files.
    /// </summary>
    VideoFiles = 73,                // SIID_VIDEOFILES

    /// <summary>
    ///  Mixed files.
    /// </summary>
    MixedFiles = 74,                // SIID_MIXEDFILES

    /// <summary>
    ///  Folder back.
    /// </summary>
    FolderBack = 75,                // SIID_FOLDERBACK

    /// <summary>
    ///  Folder front.
    /// </summary>
    FolderFront = 76,               // SIID_FOLDERFRONT

    /// <summary>
    ///  Security shield. Use for UAC prompts only.
    /// </summary>
    Shield = 77,                    // SIID_SHIELD

    /// <summary>
    ///  Warning.
    /// </summary>
    Warning = 78,                   // SIID_WARNING

    /// <summary>
    ///  Informational.
    /// </summary>
    Info = 79,                      // SIID_INFO

    /// <summary>
    ///  Error.
    /// </summary>
    Error = 80,                     // SIID_ERROR

    /// <summary>
    ///  Key / secure.
    /// </summary>
    Key = 81,                       // SIID_KEY

    /// <summary>
    ///  Software.
    /// </summary>
    Software = 82,                  // SIID_SOFTWARE

    /// <summary>
    ///  Rename.
    /// </summary>
    Rename = 83,                    // SIID_RENAME

    /// <summary>
    ///  Delete.
    /// </summary>
    Delete = 84,                    // SIID_DELETE

    /// <summary>
    ///  Audio DVD media.
    /// </summary>
    MediaAudioDVD = 85,             // SIID_MEDIAAUDIODVD

    /// <summary>
    ///  Movied DVD media.
    /// </summary>
    MediaMovieDVD = 86,             // SIID_MEDIAMOVIEDVD

    /// <summary>
    ///  Enhanced CD media.
    /// </summary>
    MediaEnhancedCD = 87,           // SIID_MEDIAENHANCEDCD

    /// <summary>
    ///  Enhanced DVD media.
    /// </summary>
    MediaEnhancedDVD = 88,          // SIID_MEDIAENHANCEDDVD

    /// <summary>
    ///  HD-DVD media.
    /// </summary>
    MediaHDDVD = 89,                // SIID_MEDIAHDDVD

    /// <summary>
    ///  BluRay media.
    /// </summary>
    MediaBluRay = 90,               // SIID_MEDIABLURAY

    /// <summary>
    ///  VCD media.
    /// </summary>
    MediaVCD = 91,                  // SIID_MEDIAVCD

    /// <summary>
    ///  DVD+R media.
    /// </summary>
    MediaDVDPlusR = 92,             // SIID_MEDIADVDPLUSR

    /// <summary>
    ///  DVD+RW media.
    /// </summary>
    MediaDVDPlusRW = 93,            // SIID_MEDIADVDPLUSRW

    /// <summary>
    ///  Desktop computer.
    /// </summary>
    DesktopPC = 94,                 // SIID_DESKTOPPC

    /// <summary>
    ///  Mobile computer.
    /// </summary>
    MobilePC = 95,                  // SIID_MOBILEPC

    /// <summary>
    ///  Users.
    /// </summary>
    Users = 96,                     // SIID_USERS

    /// <summary>
    ///  Smart media.
    /// </summary>
    MediaSmartMedia = 97,           // SIID_MEDIASMARTMEDIA

    /// <summary>
    ///  Compact Flash.
    /// </summary>
    MediaCompactFlash = 98,         // SIID_MEDIACOMPACTFLASH

    /// <summary>
    ///  Cell phone.
    /// </summary>
    DeviceCellPhone = 99,           // SIID_DEVICECELLPHONE

    /// <summary>
    ///  Camera.
    /// </summary>
    DeviceCamera = 100,             // SIID_DEVICECAMERA

    /// <summary>
    ///  Video camera.
    /// </summary>
    DeviceVideoCamera = 101,        // SIID_DEVICEVIDEOCAMERA

    /// <summary>
    ///  Audio player.
    /// </summary>
    DeviceAudioPlayer = 102,        // SIID_DEVICEAUDIOPLAYER

    /// <summary>
    ///  Connect to network.
    /// </summary>
    NetworkConnect = 103,           // SIID_NETWORKCONNECT

    /// <summary>
    ///  Internet.
    /// </summary>
    Internet = 104,                 // SIID_INTERNET

    /// <summary>
    ///  ZIP file.
    /// </summary>
    ZipFile = 105,                  // SIID_ZIPFILE

    /// <summary>
    ///  Settings.
    /// </summary>
    Settings = 106,                 // SIID_SETTINGS

    /// <summary>
    ///  HD-DVD drive.
    /// </summary>
    DriveHDDVD = 132,               // SIID_DRIVEHDDVD

    /// <summary>
    ///  BluRay drive.
    /// </summary>
    DriveBD = 133,                  // SIID_DRIVEBD

    /// <summary>
    ///  HD-DVD-ROM media.
    /// </summary>
    MediaHDDVDROM = 134,            // SIID_MEDIAHDDVDROM

    /// <summary>
    ///  HD-DVD-R media.
    /// </summary>
    MediaHDDVDR = 135,              // SIID_MEDIAHDDVDR

    /// <summary>
    ///  HD-DVD-RAM media.
    /// </summary>
    MediaHDDVDRAM = 136,            // SIID_MEDIAHDDVDRAM

    /// <summary>
    ///  BluRay-ROM media.
    /// </summary>
    MediaBDROM = 137,               // SIID_MEDIABDROM

    /// <summary>
    ///  BluRay-R media.
    /// </summary>
    MediaBDR = 138,                 // SIID_MEDIABDR

    /// <summary>
    ///  BluRay-RE media.
    /// </summary>
    MediaBDRE = 139,                // SIID_MEDIABDRE

    /// <summary>
    ///  Clustered disk.
    /// </summary>
    ClusteredDrive = 140            // SIID_CLUSTEREDDRIVE
}
#endif
