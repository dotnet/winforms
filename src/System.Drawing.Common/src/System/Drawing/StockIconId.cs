// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using Windows.Win32.UI.Shell;

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
    DocumentNoAssociation = SHSTOCKICONID.SIID_DOCNOASSOC,

    /// <summary>
    ///  Document with an associated program.
    /// </summary>
    DocumentWithAssociation = SHSTOCKICONID.SIID_DOCASSOC,

    /// <summary>
    ///  Generic application with no custom icon.
    /// </summary>
    Application = SHSTOCKICONID.SIID_APPLICATION,

    /// <summary>
    ///  Closed folder.
    /// </summary>
    Folder = SHSTOCKICONID.SIID_FOLDER,

    /// <summary>
    ///  Open folder.
    /// </summary>
    FolderOpen = SHSTOCKICONID.SIID_FOLDEROPEN,

    /// <summary>
    ///  5.25" floppy disk drive.
    /// </summary>
    Drive525 = SHSTOCKICONID.SIID_DRIVE525,

    /// <summary>
    ///  3.5" floppy disk drive.
    /// </summary>
    Drive35 = SHSTOCKICONID.SIID_DRIVE35,

    /// <summary>
    ///  Removable drive.
    /// </summary>
    DriveRemovable = SHSTOCKICONID.SIID_DRIVEREMOVE,

    /// <summary>
    ///  Fixed drive.
    /// </summary>
    DriveFixed = SHSTOCKICONID.SIID_DRIVEFIXED,

    /// <summary>
    ///  Network drive.
    /// </summary>
    DriveNet = SHSTOCKICONID.SIID_DRIVENET,

    /// <summary>
    ///  Disabled network drive.
    /// </summary>
    DriveNetDisabled = SHSTOCKICONID.SIID_DRIVENETDISABLED,

    /// <summary>
    ///  CD drive.
    /// </summary>
    DriveCD = SHSTOCKICONID.SIID_DRIVECD,

    /// <summary>
    ///  RAM disk drive.
    /// </summary>
    DriveRam = SHSTOCKICONID.SIID_DRIVERAM,

    /// <summary>
    ///  Entire network.
    /// </summary>
    World = SHSTOCKICONID.SIID_WORLD,

    /// <summary>
    ///  A computer on the network.
    /// </summary>
    Server = SHSTOCKICONID.SIID_SERVER,

    /// <summary>
    ///  Printer.
    /// </summary>
    Printer = SHSTOCKICONID.SIID_PRINTER,

    /// <summary>
    ///  My network places.
    /// </summary>
    MyNetwork = SHSTOCKICONID.SIID_MYNETWORK,

    /// <summary>
    ///  Find.
    /// </summary>
    Find = SHSTOCKICONID.SIID_FIND,

    /// <summary>
    ///  Help.
    /// </summary>
    Help = SHSTOCKICONID.SIID_HELP,

    /// <summary>
    ///  Overlay for shared items.
    /// </summary>
    Share = SHSTOCKICONID.SIID_SHARE,

    /// <summary>
    ///  Overlay for shortcuts to items.
    /// </summary>
    Link = SHSTOCKICONID.SIID_LINK,

    /// <summary>
    ///  Overlay for slow items.
    /// </summary>
    SlowFile = SHSTOCKICONID.SIID_SLOWFILE,

    /// <summary>
    ///  Empty recycle bin.
    /// </summary>
    Recycler = SHSTOCKICONID.SIID_RECYCLER,

    /// <summary>
    ///  Full recycle bin.
    /// </summary>
    RecyclerFull = SHSTOCKICONID.SIID_RECYCLERFULL,

    /// <summary>
    ///  Audio CD media.
    /// </summary>
    MediaCDAudio = SHSTOCKICONID.SIID_MEDIACDAUDIO,

    /// <summary>
    ///  Security lock.
    /// </summary>
    Lock = SHSTOCKICONID.SIID_LOCK,

    /// <summary>
    ///  AutoList.
    /// </summary>
    AutoList = SHSTOCKICONID.SIID_AUTOLIST,

    /// <summary>
    ///  Network printer.
    /// </summary>
    PrinterNet = SHSTOCKICONID.SIID_PRINTERNET,

    /// <summary>
    ///  Server share.
    /// </summary>
    ServerShare = SHSTOCKICONID.SIID_SERVERSHARE,

    /// <summary>
    ///  Fax printer.
    /// </summary>
    PrinterFax = SHSTOCKICONID.SIID_PRINTERFAX,

    /// <summary>
    ///  Networked fax printer.
    /// </summary>
    PrinterFaxNet = SHSTOCKICONID.SIID_PRINTERFAXNET,

    /// <summary>
    ///  Print to file.
    /// </summary>
    PrinterFile = SHSTOCKICONID.SIID_PRINTERFILE,

    /// <summary>
    ///  Stack.
    /// </summary>
    Stack = SHSTOCKICONID.SIID_STACK,

    /// <summary>
    ///  SVCD media.
    /// </summary>
    MediaSVCD = SHSTOCKICONID.SIID_MEDIASVCD,

    /// <summary>
    ///  Folder containing other items.
    /// </summary>
    StuffedFolder = SHSTOCKICONID.SIID_STUFFEDFOLDER,

    /// <summary>
    ///  Unknown drive.
    /// </summary>
    DriveUnknown = SHSTOCKICONID.SIID_DRIVEUNKNOWN,

    /// <summary>
    ///  DVD drive.
    /// </summary>
    DriveDVD = SHSTOCKICONID.SIID_DRIVEDVD,

    /// <summary>
    ///  DVD media.
    /// </summary>
    MediaDVD = SHSTOCKICONID.SIID_MEDIADVD,

    /// <summary>
    ///  DVD-RAM media.
    /// </summary>
    MediaDVDRAM = SHSTOCKICONID.SIID_MEDIADVDRAM,

    /// <summary>
    ///  DVD-RW media.
    /// </summary>
    MediaDVDRW = SHSTOCKICONID.SIID_MEDIADVDRW,

    /// <summary>
    ///  DVD-R media.
    /// </summary>
    MediaDVDR = SHSTOCKICONID.SIID_MEDIADVDR,

    /// <summary>
    ///  DVD-ROM media.
    /// </summary>
    MediaDVDROM = SHSTOCKICONID.SIID_MEDIADVDROM,

    /// <summary>
    ///  CD+ (Enhanced CD) media.
    /// </summary>
    MediaCDAudioPlus = SHSTOCKICONID.SIID_MEDIACDAUDIOPLUS,

    /// <summary>
    ///  CD-RW media.
    /// </summary>
    MediaCDRW = SHSTOCKICONID.SIID_MEDIACDRW,

    /// <summary>
    ///  CD-R media.
    /// </summary>
    MediaCDR = SHSTOCKICONID.SIID_MEDIACDR,

    /// <summary>
    ///  Burning CD.
    /// </summary>
    MediaCDBurn = SHSTOCKICONID.SIID_MEDIACDBURN,

    /// <summary>
    ///  Blank CD media.
    /// </summary>
    MediaBlankCD = SHSTOCKICONID.SIID_MEDIABLANKCD,

    /// <summary>
    ///  CD-ROM media.
    /// </summary>
    MediaCDROM = SHSTOCKICONID.SIID_MEDIACDROM,

    /// <summary>
    ///  Audio files.
    /// </summary>
    AudioFiles = SHSTOCKICONID.SIID_AUDIOFILES,

    /// <summary>
    ///  Image files.
    /// </summary>
    ImageFiles = SHSTOCKICONID.SIID_IMAGEFILES,

    /// <summary>
    ///  Video files.
    /// </summary>
    VideoFiles = SHSTOCKICONID.SIID_VIDEOFILES,

    /// <summary>
    ///  Mixed files.
    /// </summary>
    MixedFiles = SHSTOCKICONID.SIID_MIXEDFILES,

    /// <summary>
    ///  Folder back.
    /// </summary>
    FolderBack = SHSTOCKICONID.SIID_FOLDERBACK,

    /// <summary>
    ///  Folder front.
    /// </summary>
    FolderFront = SHSTOCKICONID.SIID_FOLDERFRONT,

    /// <summary>
    ///  Security shield. Use for UAC prompts only.
    /// </summary>
    Shield = SHSTOCKICONID.SIID_SHIELD,

    /// <summary>
    ///  Warning.
    /// </summary>
    Warning = SHSTOCKICONID.SIID_WARNING,

    /// <summary>
    ///  Informational.
    /// </summary>
    Info = SHSTOCKICONID.SIID_INFO,

    /// <summary>
    ///  Error.
    /// </summary>
    Error = SHSTOCKICONID.SIID_ERROR,

    /// <summary>
    ///  Key / secure.
    /// </summary>
    Key = SHSTOCKICONID.SIID_KEY,

    /// <summary>
    ///  Software.
    /// </summary>
    Software = SHSTOCKICONID.SIID_SOFTWARE,

    /// <summary>
    ///  Rename.
    /// </summary>
    Rename = SHSTOCKICONID.SIID_RENAME,

    /// <summary>
    ///  Delete.
    /// </summary>
    Delete = SHSTOCKICONID.SIID_DELETE,

    /// <summary>
    ///  Audio DVD media.
    /// </summary>
    MediaAudioDVD = SHSTOCKICONID.SIID_MEDIAAUDIODVD,

    /// <summary>
    ///  Movie DVD media.
    /// </summary>
    MediaMovieDVD = SHSTOCKICONID.SIID_MEDIAMOVIEDVD,

    /// <summary>
    ///  Enhanced CD media.
    /// </summary>
    MediaEnhancedCD = SHSTOCKICONID.SIID_MEDIAENHANCEDCD,

    /// <summary>
    ///  Enhanced DVD media.
    /// </summary>
    MediaEnhancedDVD = SHSTOCKICONID.SIID_MEDIAENHANCEDDVD,

    /// <summary>
    ///  HD-DVD media.
    /// </summary>
    MediaHDDVD = SHSTOCKICONID.SIID_MEDIAHDDVD,

    /// <summary>
    ///  BluRay media.
    /// </summary>
    MediaBluRay = SHSTOCKICONID.SIID_MEDIABLURAY,

    /// <summary>
    ///  VCD media.
    /// </summary>
    MediaVCD = SHSTOCKICONID.SIID_MEDIAVCD,

    /// <summary>
    ///  DVD+R media.
    /// </summary>
    MediaDVDPlusR = SHSTOCKICONID.SIID_MEDIADVDPLUSR,

    /// <summary>
    ///  DVD+RW media.
    /// </summary>
    MediaDVDPlusRW = SHSTOCKICONID.SIID_MEDIADVDPLUSRW,

    /// <summary>
    ///  Desktop computer.
    /// </summary>
    DesktopPC = SHSTOCKICONID.SIID_DESKTOPPC,

    /// <summary>
    ///  Mobile computer.
    /// </summary>
    MobilePC = SHSTOCKICONID.SIID_MOBILEPC,

    /// <summary>
    ///  Users.
    /// </summary>
    Users = SHSTOCKICONID.SIID_USERS,

    /// <summary>
    ///  Smart media.
    /// </summary>
    MediaSmartMedia = SHSTOCKICONID.SIID_MEDIASMARTMEDIA,

    /// <summary>
    ///  Compact Flash.
    /// </summary>
    MediaCompactFlash = SHSTOCKICONID.SIID_MEDIACOMPACTFLASH,

    /// <summary>
    ///  Cell phone.
    /// </summary>
    DeviceCellPhone = SHSTOCKICONID.SIID_DEVICECELLPHONE,

    /// <summary>
    ///  Camera.
    /// </summary>
    DeviceCamera = SHSTOCKICONID.SIID_DEVICECAMERA,

    /// <summary>
    ///  Video camera.
    /// </summary>
    DeviceVideoCamera = SHSTOCKICONID.SIID_DEVICEVIDEOCAMERA,

    /// <summary>
    ///  Audio player.
    /// </summary>
    DeviceAudioPlayer = SHSTOCKICONID.SIID_DEVICEAUDIOPLAYER,

    /// <summary>
    ///  Connect to network.
    /// </summary>
    NetworkConnect = SHSTOCKICONID.SIID_NETWORKCONNECT,

    /// <summary>
    ///  Internet.
    /// </summary>
    Internet = SHSTOCKICONID.SIID_INTERNET,

    /// <summary>
    ///  ZIP file.
    /// </summary>
    ZipFile = SHSTOCKICONID.SIID_ZIPFILE,

    /// <summary>
    ///  Settings.
    /// </summary>
    Settings = SHSTOCKICONID.SIID_SETTINGS,

    /// <summary>
    ///  HD-DVD drive.
    /// </summary>
    DriveHDDVD = SHSTOCKICONID.SIID_DRIVEHDDVD,

    /// <summary>
    ///  BluRay drive.
    /// </summary>
    DriveBD = SHSTOCKICONID.SIID_DRIVEBD,

    /// <summary>
    ///  HD-DVD-ROM media.
    /// </summary>
    MediaHDDVDROM = SHSTOCKICONID.SIID_MEDIAHDDVDROM,

    /// <summary>
    ///  HD-DVD-R media.
    /// </summary>
    MediaHDDVDR = SHSTOCKICONID.SIID_MEDIAHDDVDR,

    /// <summary>
    ///  HD-DVD-RAM media.
    /// </summary>
    MediaHDDVDRAM = SHSTOCKICONID.SIID_MEDIAHDDVDRAM,

    /// <summary>
    ///  BluRay-ROM media.
    /// </summary>
    MediaBDROM = SHSTOCKICONID.SIID_MEDIABDROM,

    /// <summary>
    ///  BluRay-R media.
    /// </summary>
    MediaBDR = SHSTOCKICONID.SIID_MEDIABDR,

    /// <summary>
    ///  BluRay-RE media.
    /// </summary>
    MediaBDRE = SHSTOCKICONID.SIID_MEDIABDRE,

    /// <summary>
    ///  Clustered disk.
    /// </summary>
    ClusteredDrive = SHSTOCKICONID.SIID_CLUSTEREDDRIVE
}
#endif
