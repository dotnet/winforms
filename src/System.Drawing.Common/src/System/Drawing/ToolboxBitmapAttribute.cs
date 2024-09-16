// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Drawing.Imaging;
using System.IO;

namespace System.Drawing;

/// <summary>
///  ToolboxBitmapAttribute defines the images associated with a specified component.
///  The component can offer a small and large image (large is optional).
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class ToolboxBitmapAttribute : Attribute
{
    private Image? _smallImage;
    private Image? _largeImage;

    private static readonly Size s_largeSize = new(32, 32);
    private static readonly Size s_smallSize = new(16, 16);

    public ToolboxBitmapAttribute(string imageFile) : this(GetImageFromFile(imageFile, false), GetImageFromFile(imageFile, true))
    {
    }

    public ToolboxBitmapAttribute(Type t) : this(GetImageFromResource(t, null, false), GetImageFromResource(t, null, true))
    {
    }

    public ToolboxBitmapAttribute(Type t, string name)
        : this(GetImageFromResource(t, name, false), GetImageFromResource(t, name, true))
    {
    }

    private ToolboxBitmapAttribute(Image? smallImage, Image? largeImage)
    {
        _smallImage = smallImage;
        _largeImage = largeImage;
    }

    public override bool Equals([NotNullWhen(true)] object? value)
    {
        if (value == this)
        {
            return true;
        }

        if (value is ToolboxBitmapAttribute attribute)
        {
            return attribute._smallImage == _smallImage && attribute._largeImage == _largeImage;
        }

        return false;
    }

    public override int GetHashCode() => base.GetHashCode();

    public Image? GetImage(object? component) => GetImage(component, true);

    public Image? GetImage(object? component, bool large) =>
        component is not null ? GetImage(component.GetType(), large) : null;

    public Image? GetImage(Type type) => GetImage(type, false);

    public Image? GetImage(Type type, bool large) => GetImage(type, null, large);

    public Image? GetImage(Type type, string? imgName, bool large)
    {
        if ((large && _largeImage is null) || (!large && _smallImage is null))
        {
            Image? image = large ? _largeImage : _smallImage;
            image ??= GetImageFromResource(type, imgName, large);

            // last resort for large images.
            if (large && _largeImage is null && _smallImage is not null)
            {
                image = new Bitmap((Bitmap)_smallImage, s_largeSize.Width, s_largeSize.Height);
            }

            if (image is Bitmap b)
            {
                MakeBackgroundAlphaZero(b);
            }

            if (image is null)
            {
                image = s_defaultComponent.GetImage(type, large);

                // We don't want to hand out the static shared image
                // because otherwise it might get disposed.
                if (image is not null)
                {
                    image = (Image)image.Clone();
                }
            }

            if (large)
            {
                _largeImage = image;
            }
            else
            {
                _smallImage = image;
            }
        }

        Image? toReturn = large ? _largeImage : _smallImage;

        if (Equals(Default))
        {
            _largeImage = null;
            _smallImage = null;
        }

        return toReturn;
    }

    // Helper to get the right icon from the given stream that represents an icon.
    private static Bitmap? GetIconFromStream(Stream? stream, bool large, bool scaled)
    {
        if (stream is null)
        {
            return null;
        }

        Bitmap? bitmap = null;
        try
        {
            using Icon ico = new(stream);
            using Icon sizedIco = new(ico, large ? s_largeSize : s_smallSize);

            bitmap = sizedIco.ToBitmap();
            if (DpiHelper.IsScalingRequired && scaled)
            {
                DpiHelper.ScaleBitmapLogicalToDevice(ref bitmap);
            }
        }
        catch (Exception e) when (!ClientUtils.IsCriticalException(e))
        {
        }

        return bitmap;
    }

    // Just forwards to Image.FromFile eating any non-critical exceptions that may result.
    private static Image? GetImageFromFile(string? imageFile, bool large, bool scaled = true)
    {
        Image? image = null;
        try
        {
            if (imageFile is not null)
            {
                string? ext = Path.GetExtension(imageFile);
                if (ext is not null && string.Equals(ext, ".ico", StringComparison.OrdinalIgnoreCase))
                {
                    // ico files support both large and small, so we respect the large flag here.
                    using FileStream reader = File.OpenRead(imageFile!);
                    image = GetIconFromStream(reader, large, scaled);
                }
                else if (!large)
                {
                    // we only read small from non-ico files.
                    image = Image.FromFile(imageFile!);
                    Bitmap? b = image as Bitmap;
                    if (DpiHelper.IsScalingRequired && scaled)
                    {
                        DpiHelper.ScaleBitmapLogicalToDevice(ref b);
                    }
                }
            }
        }
        catch (Exception e) when (!ClientUtils.IsCriticalException(e))
        {
        }

        return image;
    }

    private static Image? GetBitmapFromResource(Type t, string? bitmapName, bool large, bool scaled)
    {
        if (bitmapName is null)
        {
            return null;
        }

        Image? image = null;
        try
        {
            // Load the image from the manifest resources.
            Stream? stream = BitmapSelector.GetResourceStream(t, bitmapName);
            if (stream is not null)
            {
                Bitmap? bitmap = new Bitmap(stream);
                image = bitmap;
                MakeBackgroundAlphaZero(bitmap);
                if (large)
                {
                    image = new Bitmap(bitmap, s_largeSize.Width, s_largeSize.Height);
                }

                if (DpiHelper.IsScalingRequired && scaled)
                {
                    bitmap = (Bitmap)image;
                    DpiHelper.ScaleBitmapLogicalToDevice(ref bitmap);
                    image = bitmap;
                }
            }
        }
        catch (Exception e) when (!ClientUtils.IsCriticalException(e))
        {
        }

        return image;
    }

    private static Bitmap? GetIconFromResource(Type t, string? bitmapName, bool large, bool scaled) =>
        bitmapName is null ? null : GetIconFromStream(BitmapSelector.GetResourceStream(t, bitmapName), large, scaled);

    public static Image? GetImageFromResource(Type t, string? imageName, bool large) =>
        GetImageFromResource(t, imageName, large, scaled: true);

    internal static Image? GetImageFromResource(Type t, string? imageName, bool large, bool scaled)
    {
        Image? image = null;
        try
        {
            string? name = imageName;
            string? iconName = null;
            string? bmpName = null;
            string? rawBmpName = null;

            // If we didn't get a name, use the class name
            if (name is null)
            {
                name = t.FullName!;
                int indexDot = name.LastIndexOf('.');
                if (indexDot != -1)
                {
                    name = name[(indexDot + 1)..];
                }

                // All bitmap images from winforms runtime are changed to Icons
                // and logical names, now, does not contain any extension.
                rawBmpName = name;
                iconName = $"{name}.ico";
                bmpName = $"{name}.bmp";
            }
            else
            {
                if (string.Equals(Path.GetExtension(imageName), ".ico", StringComparison.CurrentCultureIgnoreCase))
                {
                    iconName = name;
                }
                else if (string.Equals(Path.GetExtension(imageName), ".bmp", StringComparison.CurrentCultureIgnoreCase))
                {
                    bmpName = name;
                }
                else
                {
                    // We don't recognize the name as either bmp or ico. we need to try three things.
                    // 1. the name as a bitmap (back compat)
                    // 2. name+.bmp
                    // 3. name+.ico
                    rawBmpName = name;
                    bmpName = $"{name}.bmp";
                    iconName = $"{name}.ico";
                }
            }

            if (rawBmpName is not null)
            {
                image = GetBitmapFromResource(t, rawBmpName, large, scaled);
            }

            if (image is null && rawBmpName is not null)
            {
                image = GetIconFromResource(t, rawBmpName, large, scaled);
            }

            if (image is null && bmpName is not null)
            {
                image = GetBitmapFromResource(t, bmpName, large, scaled);
            }

            if (image is null && iconName is not null)
            {
                image = GetIconFromResource(t, iconName, large, scaled);
            }
        }
        catch (Exception) { }
        return image;
    }

    private static void MakeBackgroundAlphaZero(Bitmap img)
    {
        // Bitmap derived from Icon is already transparent.
        if (img.RawFormat.Guid == ImageFormat.Icon.Guid)
            return;

        Color bottomLeft = img.GetPixel(0, img.Height - 1);
        img.MakeTransparent();

        Color newBottomLeft = Color.FromArgb(0, bottomLeft);
        img.SetPixel(0, img.Height - 1, newBottomLeft);
    }

    public static readonly ToolboxBitmapAttribute Default = new(null, (Image?)null);

    private static readonly ToolboxBitmapAttribute s_defaultComponent;

#pragma warning disable CA1810 // DummyFunction apparently needs to be invoked prior to the rest of the initialization
    static ToolboxBitmapAttribute()
    {
        Stream? stream = BitmapSelector.GetResourceStream(typeof(ToolboxBitmapAttribute), "DefaultComponent.bmp");
        Debug.Assert(stream is not null, "DefaultComponent.bmp must be present as an embedded resource.");

        Bitmap bitmap = new(stream);
        MakeBackgroundAlphaZero(bitmap);
        s_defaultComponent = new ToolboxBitmapAttribute(bitmap, null);
    }
#pragma warning restore CA1810
}
