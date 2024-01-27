// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#if NET9_0_OR_GREATER

using System.Runtime.Versioning;

namespace System.Drawing.Imaging.Effects;

/// <summary>
///  Changes the brightness and contrast of an image.
/// </summary>
[RequiresPreviewFeatures]
public sealed unsafe class BrightnessContrastEffect : Effect
{
    private readonly BrightnessContrastParams _brightnessContrastParams;

    /// <summary>
    ///  Initializes a new instance of the <see cref="BrightnessContrastEffect"/> class.
    /// </summary>
    /// <param name="brightnessLevel">
    ///  Integer in the range -255 through 255 that specifies the brightness level. If the value is 0, the brightness
    ///  remains the same. As the value moves from 0 to 255, the brightness of the image increases. As the value moves
    ///  from 0 to -255, the brightness of the image decreases.
    /// </param>
    /// <param name="contrastLevel">
    ///  Integer in the range -100 through 100 that specifies the contrast level. If the value is 0, the contrast
    ///  remains the same. As the value moves from 0 to 100, the contrast of the image increases. As the value moves
    ///  from 0 to -100, the contrast of the image decreases.
    /// </param>
    public BrightnessContrastEffect(int brightnessLevel, int contrastLevel) : base(PInvoke.BrightnessContrastEffectGuid)
    {
        _brightnessContrastParams = new() { brightnessLevel = brightnessLevel, contrastLevel = contrastLevel };
        SetParameters(ref _brightnessContrastParams);
    }

    /// <summary>
    ///  Integer in the range -255 through 255 that specifies the brightness level. If the value is 0, the brightness
    ///  remains the same. As the value moves from 0 to 255, the brightness of the image increases. As the value moves
    ///  from 0 to -255, the brightness of the image decreases.
    /// </summary>
    public int BrightnessLevel => _brightnessContrastParams.brightnessLevel;

    /// <summary>
    ///  Integer in the range -100 through 100 that specifies the contrast level. If the value is 0, the contrast
    ///  remains the same. As the value moves from 0 to 100, the contrast of the image increases. As the value moves
    ///  from 0 to -100, the contrast of the image decreases.
    /// </summary>
    public int ContrastLevel => _brightnessContrastParams.contrastLevel;
}
#endif
