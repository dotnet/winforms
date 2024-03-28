// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

namespace System.Drawing.Printing;

/// <summary>
///  Specifies the standard paper sizes.
/// </summary>
public enum PaperKind
{
    /// <summary>
    ///  The paper size is defined by the user.
    /// </summary>
    Custom = 0,

    /// <summary>
    ///  Letter paper (8.5 in. by 11 in.).
    /// </summary>
    Letter = (int)PInvoke.DMPAPER_LETTER,

    /// <summary>
    ///  Legal paper (8.5 in. by 14 in.).
    /// </summary>
    Legal = (int)PInvoke.DMPAPER_LEGAL,

    /// <summary>
    ///  A4 paper (210 mm by 297 mm).
    /// </summary>
    A4 = (int)PInvoke.DMPAPER_A4,

    /// <summary>
    ///  C paper (17 in. by 22 in.).
    /// </summary>
    CSheet = (int)PInvoke.DMPAPER_CSHEET,

    /// <summary>
    ///  D paper (22 in. by 34 in.).
    /// </summary>
    DSheet = (int)PInvoke.DMPAPER_DSHEET,

    /// <summary>
    ///  E paper (34 in. by 44 in.).
    /// </summary>
    ESheet = (int)PInvoke.DMPAPER_ESHEET,

    /// <summary>
    ///  Letter small paper (8.5 in. by 11 in.).
    /// </summary>
    LetterSmall = (int)PInvoke.DMPAPER_LETTERSMALL,

    /// <summary>
    ///  Tabloid paper (11 in. by 17 in.).
    /// </summary>
    Tabloid = (int)PInvoke.DMPAPER_TABLOID,

    /// <summary>
    ///  Ledger paper (17 in. by 11 in.).
    /// </summary>
    Ledger = (int)PInvoke.DMPAPER_LEDGER,

    /// <summary>
    ///  Statement paper (5.5 in. by 8.5 in.).
    /// </summary>
    Statement = (int)PInvoke.DMPAPER_STATEMENT,

    /// <summary>
    ///  Executive paper (7.25 in. by 10.5 in.).
    /// </summary>
    Executive = (int)PInvoke.DMPAPER_EXECUTIVE,

    /// <summary>
    ///  A3 paper (297 mm by 420 mm).
    /// </summary>
    A3 = (int)PInvoke.DMPAPER_A3,

    /// <summary>
    ///  A4 small paper (210 mm by 297 mm).
    /// </summary>
    A4Small = (int)PInvoke.DMPAPER_A4SMALL,

    /// <summary>
    ///  A5 paper (148 mm by 210 mm).
    /// </summary>
    A5 = (int)PInvoke.DMPAPER_A5,

    /// <summary>
    ///  B4 paper (250 mm by 353 mm).
    /// </summary>
    B4 = (int)PInvoke.DMPAPER_B4,

    /// <summary>
    ///  B5 paper (176 mm by 250 mm).
    /// </summary>
    B5 = (int)PInvoke.DMPAPER_B5,

    /// <summary>
    ///  Folio paper (8.5 in. by 13 in.).
    /// </summary>
    Folio = (int)PInvoke.DMPAPER_FOLIO,

    /// <summary>
    ///  Quarto paper (215 mm by 275 mm).
    /// </summary>
    Quarto = (int)PInvoke.DMPAPER_QUARTO,

    /// <summary>
    ///  10-by-14-inch paper.
    /// </summary>
    Standard10x14 = (int)PInvoke.DMPAPER_10X14,

    /// <summary>
    ///  11-by-17-inch paper.
    /// </summary>
    Standard11x17 = (int)PInvoke.DMPAPER_11X17,

    /// <summary>
    ///  Note paper (8.5 in. by 11 in.).
    /// </summary>
    Note = (int)PInvoke.DMPAPER_NOTE,

    /// <summary>
    ///  #9 envelope (3.875 in. by 8.875 in.).
    /// </summary>
    Number9Envelope = (int)PInvoke.DMPAPER_ENV_9,

    /// <summary>
    ///  #10 envelope (4.125 in. by 9.5 in.).
    /// </summary>
    Number10Envelope = (int)PInvoke.DMPAPER_ENV_10,

    /// <summary>
    ///  #11 envelope (4.5 in. by 10.375 in.).
    /// </summary>
    Number11Envelope = (int)PInvoke.DMPAPER_ENV_11,

    /// <summary>
    ///  #12 envelope (4.75 in. by 11 in.).
    /// </summary>
    Number12Envelope = (int)PInvoke.DMPAPER_ENV_12,

    /// <summary>
    ///  #14 envelope (5 in. by 11.5 in.).
    /// </summary>
    Number14Envelope = (int)PInvoke.DMPAPER_ENV_14,

    /// <summary>
    ///  DL envelope (110 mm by 220 mm).
    /// </summary>
    DLEnvelope = (int)PInvoke.DMPAPER_ENV_DL,

    /// <summary>
    ///  C5 envelope (162 mm by 229 mm).
    /// </summary>
    C5Envelope = (int)PInvoke.DMPAPER_ENV_C5,

    /// <summary>
    ///  C3 envelope (324 mm by 458 mm).
    /// </summary>
    C3Envelope = (int)PInvoke.DMPAPER_ENV_C3,

    /// <summary>
    ///  C4 envelope (229 mm by 324 mm).
    /// </summary>
    C4Envelope = (int)PInvoke.DMPAPER_ENV_C4,

    /// <summary>
    ///  C6 envelope (114 mm by 162 mm).
    /// </summary>
    C6Envelope = (int)PInvoke.DMPAPER_ENV_C6,

    /// <summary>
    ///  C65 envelope (114 mm by 229 mm).
    /// </summary>
    C65Envelope = (int)PInvoke.DMPAPER_ENV_C65,

    /// <summary>
    ///  B4 envelope (250 mm by 353 mm).
    /// </summary>
    B4Envelope = (int)PInvoke.DMPAPER_ENV_B4,

    /// <summary>
    ///  B5 envelope (176 mm by 250 mm).
    /// </summary>
    B5Envelope = (int)PInvoke.DMPAPER_ENV_B5,

    /// <summary>
    ///  B6 envelope (176 mm by 125 mm).
    /// </summary>
    B6Envelope = (int)PInvoke.DMPAPER_ENV_B6,

    /// <summary>
    ///  Italy envelope (110 mm by 230 mm).
    /// </summary>
    ItalyEnvelope = (int)PInvoke.DMPAPER_ENV_ITALY,

    /// <summary>
    ///  Monarch envelope (3.875 in. by 7.5 in.).
    /// </summary>
    MonarchEnvelope = (int)PInvoke.DMPAPER_ENV_MONARCH,

    /// <summary>
    ///  6 3/4 envelope (3.625 in. by 6.5 in.).
    /// </summary>
    PersonalEnvelope = (int)PInvoke.DMPAPER_ENV_PERSONAL,

    /// <summary>
    ///  US standard fanfold (14.875 in. by 11 in.).
    /// </summary>
    USStandardFanfold = (int)PInvoke.DMPAPER_FANFOLD_US,

    /// <summary>
    ///  German standard fanfold (8.5 in. by 12 in.).
    /// </summary>
    GermanStandardFanfold = (int)PInvoke.DMPAPER_FANFOLD_STD_GERMAN,

    /// <summary>
    ///  German legal fanfold (8.5 in. by 13 in.).
    /// </summary>
    GermanLegalFanfold = (int)PInvoke.DMPAPER_FANFOLD_LGL_GERMAN,

    /// <summary>
    ///  ISO B4 (250 mm by 353 mm).
    /// </summary>
    IsoB4 = (int)PInvoke.DMPAPER_ISO_B4,

    /// <summary>
    ///  Japanese postcard (100 mm by 148 mm).
    /// </summary>
    JapanesePostcard = (int)PInvoke.DMPAPER_JAPANESE_POSTCARD,

    /// <summary>
    ///  9-by-11-inch paper.
    /// </summary>
    Standard9x11 = (int)PInvoke.DMPAPER_9X11,

    /// <summary>
    ///  10-by-11-inch paper.
    /// </summary>
    Standard10x11 = (int)PInvoke.DMPAPER_10X11,

    /// <summary>
    ///  15-by-11-inch paper.
    /// </summary>
    Standard15x11 = (int)PInvoke.DMPAPER_15X11,

    /// <summary>
    ///  Invite envelope (220 mm by 220 mm).
    /// </summary>
    InviteEnvelope = (int)PInvoke.DMPAPER_ENV_INVITE,

    /// <summary>
    ///  Letter extra paper (9.275 in. by 12 in.).
    ///  This value is specific to the PostScript driver and is used only by Linotronic printers in order to conserve paper.
    /// </summary>
    LetterExtra = (int)PInvoke.DMPAPER_LETTER_EXTRA,

    /// <summary>
    ///  Legal extra paper (9.275 in. by 15 in.).
    ///  This value is specific to the PostScript driver and is used only by Linotronic printers in order to conserve paper.
    /// </summary>
    LegalExtra = (int)PInvoke.DMPAPER_LEGAL_EXTRA,

    /// <summary>
    ///  Tabloid extra paper (11.69 in. by 18 in.).
    ///  This value is specific to the PostScript driver and is used only by Linotronic printers in order to conserve paper.
    /// </summary>
    TabloidExtra = (int)PInvoke.DMPAPER_TABLOID_EXTRA,

    /// <summary>
    ///  A4 extra paper (236 mm by 322 mm).
    ///  This value is specific to the PostScript driver and is used only by Linotronic printers in order to conserve paper.
    /// </summary>
    A4Extra = (int)PInvoke.DMPAPER_A4_EXTRA,

    /// <summary>
    ///  Letter transverse paper (8.275 in. by 11 in.).
    /// </summary>
    LetterTransverse = (int)PInvoke.DMPAPER_LETTER_TRANSVERSE,

    /// <summary>
    ///  A4 transverse paper (210 mm by 297 mm).
    /// </summary>
    A4Transverse = (int)PInvoke.DMPAPER_A4_TRANSVERSE,

    /// <summary>
    ///  Letter extra transverse paper (9.275 in. by 12 in.).
    /// </summary>
    LetterExtraTransverse = (int)PInvoke.DMPAPER_LETTER_EXTRA_TRANSVERSE,

    /// <summary>
    ///  SuperA/SuperA/A4 paper (227 mm by 356 mm).
    /// </summary>
    APlus = (int)PInvoke.DMPAPER_A_PLUS,

    /// <summary>
    ///  SuperB/SuperB/A3 paper (305 mm by 487 mm).
    /// </summary>
    BPlus = (int)PInvoke.DMPAPER_B_PLUS,

    /// <summary>
    ///  Letter plus paper (8.5 in. by 12.69 in.).
    /// </summary>
    LetterPlus = (int)PInvoke.DMPAPER_LETTER_PLUS,

    /// <summary>
    ///  A4 plus paper (210 mm by 330 mm).
    /// </summary>
    A4Plus = (int)PInvoke.DMPAPER_A4_PLUS,

    /// <summary>
    ///  A5 transverse paper (148 mm by 210 mm).
    /// </summary>
    A5Transverse = (int)PInvoke.DMPAPER_A5_TRANSVERSE,

    /// <summary>
    ///  JIS B5 transverse paper (182 mm by 257 mm).
    /// </summary>
    B5Transverse = (int)PInvoke.DMPAPER_B5_TRANSVERSE,

    /// <summary>
    ///  A3 extra paper (322 mm by 445 mm).
    /// </summary>
    A3Extra = (int)PInvoke.DMPAPER_A3_EXTRA,

    /// <summary>
    ///  A5 extra paper (174 mm by 235 mm).
    /// </summary>
    A5Extra = (int)PInvoke.DMPAPER_A5_EXTRA,

    /// <summary>
    ///  ISO B5 extra paper (201 mm by 276 mm).
    /// </summary>
    B5Extra = (int)PInvoke.DMPAPER_B5_EXTRA,

    /// <summary>
    ///  A2 paper (420 mm by 594 mm).
    /// </summary>
    A2 = (int)PInvoke.DMPAPER_A2,

    /// <summary>
    ///  A3 transverse paper (297 mm by 420 mm).
    /// </summary>
    A3Transverse = (int)PInvoke.DMPAPER_A3_TRANSVERSE,

    /// <summary>
    ///  A3 extra transverse paper (322 mm by 445 mm).
    /// </summary>
    A3ExtraTransverse = (int)PInvoke.DMPAPER_A3_EXTRA_TRANSVERSE,

    /// <summary>
    ///  Japanese double postcard (200 mm by 148mm).
    /// </summary>
    JapaneseDoublePostcard = (int)PInvoke.DMPAPER_DBL_JAPANESE_POSTCARD,

    /// <summary>
    ///  A6 paper (105 mm by 148 mm).
    /// </summary>
    A6 = (int)PInvoke.DMPAPER_A6,

    /// <summary>
    ///  Japanese Kaku #2 envelope.
    /// </summary>
    JapaneseEnvelopeKakuNumber2 = (int)PInvoke.DMPAPER_JENV_KAKU2,

    /// <summary>
    ///  Japanese Kaku #3 envelope.
    /// </summary>
    JapaneseEnvelopeKakuNumber3 = (int)PInvoke.DMPAPER_JENV_KAKU3,

    /// <summary>
    ///  Japanese Chou #3 envelope.
    /// </summary>
    JapaneseEnvelopeChouNumber3 = (int)PInvoke.DMPAPER_JENV_CHOU3,

    /// <summary>
    ///  Japanese Chou #4 envelope.
    /// </summary>
    JapaneseEnvelopeChouNumber4 = (int)PInvoke.DMPAPER_JENV_CHOU4,

    /// <summary>
    ///  Letter rotated paper (11 in. by 8.5 in.).
    /// </summary>
    LetterRotated = (int)PInvoke.DMPAPER_LETTER_ROTATED,

    /// <summary>
    ///  A3 rotated paper (420mm by 297 mm).
    /// </summary>
    A3Rotated = (int)PInvoke.DMPAPER_A3_ROTATED,

    /// <summary>
    ///  A4 rotated paper (297 mm by 210 mm).
    /// </summary>
    A4Rotated = (int)PInvoke.DMPAPER_A4_ROTATED,

    /// <summary>
    ///  A5 rotated paper (210 mm by 148 mm).
    /// </summary>
    A5Rotated = (int)PInvoke.DMPAPER_A5_ROTATED,

    /// <summary>
    ///  JIS B4 rotated paper (364 mm by 257 mm).
    /// </summary>
    B4JisRotated = (int)PInvoke.DMPAPER_B4_JIS_ROTATED,

    /// <summary>
    ///  JIS B5 rotated paper (257 mm by 182 mm).
    /// </summary>
    B5JisRotated = (int)PInvoke.DMPAPER_B5_JIS_ROTATED,

    /// <summary>
    ///  Japanese rotated postcard (148 mm by 100 mm).
    /// </summary>
    JapanesePostcardRotated = (int)PInvoke.DMPAPER_JAPANESE_POSTCARD_ROTATED,

    /// <summary>
    ///  Japanese rotated double postcard (148 mm by 200 mm).
    /// </summary>
    JapaneseDoublePostcardRotated = (int)PInvoke.DMPAPER_DBL_JAPANESE_POSTCARD_ROTATED,

    /// <summary>
    ///  A6 rotated paper (148 mm by 105 mm).
    /// </summary>
    A6Rotated = (int)PInvoke.DMPAPER_A6_ROTATED,

    /// <summary>
    ///  Japanese rotated Kaku #2 envelope.
    /// </summary>
    JapaneseEnvelopeKakuNumber2Rotated = (int)PInvoke.DMPAPER_JENV_KAKU2_ROTATED,

    /// <summary>
    ///  Japanese rotated Kaku #3 envelope.
    /// </summary>
    JapaneseEnvelopeKakuNumber3Rotated = (int)PInvoke.DMPAPER_JENV_KAKU3_ROTATED,

    /// <summary>
    ///  Japanese rotated Chou #3 envelope.
    /// </summary>
    JapaneseEnvelopeChouNumber3Rotated = (int)PInvoke.DMPAPER_JENV_CHOU3_ROTATED,

    /// <summary>
    ///  Japanese rotated Chou #4 envelope.
    /// </summary>
    JapaneseEnvelopeChouNumber4Rotated = (int)PInvoke.DMPAPER_JENV_CHOU4_ROTATED,

    /// <summary>
    ///  JIS B6 paper (128 mm by 182 mm).
    /// </summary>
    B6Jis = (int)PInvoke.DMPAPER_B6_JIS,

    /// <summary>
    ///  JIS B6 rotated paper (182 mm by 128 mm).
    /// </summary>
    B6JisRotated = (int)PInvoke.DMPAPER_B6_JIS_ROTATED,

    /// <summary>
    ///  12-by-11-inch paper.
    /// </summary>
    Standard12x11 = (int)PInvoke.DMPAPER_12X11,

    /// <summary>
    ///  Japanese You #4 envelope.
    /// </summary>
    JapaneseEnvelopeYouNumber4 = (int)PInvoke.DMPAPER_JENV_YOU4,

    /// <summary>
    ///  Japanese You #4 rotated envelope.
    /// </summary>
    JapaneseEnvelopeYouNumber4Rotated = (int)PInvoke.DMPAPER_JENV_YOU4_ROTATED,

    /// <summary>
    ///  PRC 16K paper (146 mm by 215 mm).
    /// </summary>
    Prc16K = (int)PInvoke.DMPAPER_P16K,

    /// <summary>
    ///  PRC 32K paper (97 mm by 151 mm).
    /// </summary>
    Prc32K = (int)PInvoke.DMPAPER_P32K,

    /// <summary>
    ///  PRC 32K big paper (97 mm by 151 mm).
    /// </summary>
    Prc32KBig = (int)PInvoke.DMPAPER_P32KBIG,

    /// <summary>
    ///  PRC #1 envelope (102 mm by 165 mm).
    /// </summary>
    PrcEnvelopeNumber1 = (int)PInvoke.DMPAPER_PENV_1,

    /// <summary>
    ///  PRC #2 envelope (102 mm by 176 mm).
    /// </summary>
    PrcEnvelopeNumber2 = (int)PInvoke.DMPAPER_PENV_2,

    /// <summary>
    ///  PRC #3 envelope (125 mm by 176 mm).
    /// </summary>
    PrcEnvelopeNumber3 = (int)PInvoke.DMPAPER_PENV_3,

    /// <summary>
    ///  PRC #4 envelope (110 mm by 208 mm).
    /// </summary>
    PrcEnvelopeNumber4 = (int)PInvoke.DMPAPER_PENV_4,

    /// <summary>
    ///  PRC #5 envelope (110 mm by 220 mm).
    /// </summary>
    PrcEnvelopeNumber5 = (int)PInvoke.DMPAPER_PENV_5,

    /// <summary>
    ///  PRC #6 envelope (120 mm by 230 mm).
    /// </summary>
    PrcEnvelopeNumber6 = (int)PInvoke.DMPAPER_PENV_6,

    /// <summary>
    ///  PRC #7 envelope (160 mm by 230 mm).
    /// </summary>
    PrcEnvelopeNumber7 = (int)PInvoke.DMPAPER_PENV_7,

    /// <summary>
    ///  PRC #8 envelope (120 mm by 309 mm).
    /// </summary>
    PrcEnvelopeNumber8 = (int)PInvoke.DMPAPER_PENV_8,

    /// <summary>
    ///  PRC #9 envelope (229 mm by 324 mm).
    /// </summary>
    PrcEnvelopeNumber9 = (int)PInvoke.DMPAPER_PENV_9,

    /// <summary>
    ///  PRC #10 envelope (324 mm by 458 mm).
    /// </summary>
    PrcEnvelopeNumber10 = (int)PInvoke.DMPAPER_PENV_10,

    /// <summary>
    ///  PRC 16K rotated paper (146 mm by 215 mm).
    /// </summary>
    Prc16KRotated = (int)PInvoke.DMPAPER_P16K_ROTATED,

    /// <summary>
    ///  PRC 32K rotated paper (97 mm by 151 mm).
    /// </summary>
    Prc32KRotated = (int)PInvoke.DMPAPER_P32K_ROTATED,

    /// <summary>
    ///  PRC 32K big rotated paper (97 mm by 151 mm).
    /// </summary>
    Prc32KBigRotated = (int)PInvoke.DMPAPER_P32KBIG_ROTATED,

    /// <summary>
    ///  PRC #1 rotated envelope (165 mm by 102 mm).
    /// </summary>
    PrcEnvelopeNumber1Rotated = (int)PInvoke.DMPAPER_PENV_1_ROTATED,

    /// <summary>
    ///  PRC #2 rotated envelope (176 mm by 102 mm).
    /// </summary>
    PrcEnvelopeNumber2Rotated = (int)PInvoke.DMPAPER_PENV_2_ROTATED,

    /// <summary>
    ///  PRC #3 rotated envelope (176 mm by 125 mm).
    /// </summary>
    PrcEnvelopeNumber3Rotated = (int)PInvoke.DMPAPER_PENV_3_ROTATED,

    /// <summary>
    ///  PRC #4 rotated envelope (208 mm by 110 mm).
    /// </summary>
    PrcEnvelopeNumber4Rotated = (int)PInvoke.DMPAPER_PENV_4_ROTATED,

    /// <summary>
    ///  PRC #5 rotated envelope (220 mm by 110 mm).
    /// </summary>
    PrcEnvelopeNumber5Rotated = (int)PInvoke.DMPAPER_PENV_5_ROTATED,

    /// <summary>
    ///  PRC #6 rotated envelope (230 mm by 120 mm).
    /// </summary>
    PrcEnvelopeNumber6Rotated = (int)PInvoke.DMPAPER_PENV_6_ROTATED,

    /// <summary>
    ///  PRC #7 rotated envelope (230 mm by 160 mm).
    /// </summary>
    PrcEnvelopeNumber7Rotated = (int)PInvoke.DMPAPER_PENV_7_ROTATED,

    /// <summary>
    ///  PRC #8 rotated envelope (309 mm by 120 mm).
    /// </summary>
    PrcEnvelopeNumber8Rotated = (int)PInvoke.DMPAPER_PENV_8_ROTATED,

    /// <summary>
    ///  PRC #9 rotated envelope (324 mm by 229 mm).
    /// </summary>
    PrcEnvelopeNumber9Rotated = (int)PInvoke.DMPAPER_PENV_9_ROTATED,

    /// <summary>
    ///  PRC #10 rotated envelope (458 mm by 324 mm).
    /// </summary>
    PrcEnvelopeNumber10Rotated = (int)PInvoke.DMPAPER_PENV_10_ROTATED,
}
