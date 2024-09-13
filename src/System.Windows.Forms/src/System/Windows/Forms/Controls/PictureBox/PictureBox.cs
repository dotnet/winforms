// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections.Specialized;
using System.ComponentModel;
using System.Drawing;
using System.Net;
using System.Runtime.InteropServices;
using System.Windows.Forms.Layout;
using System.Windows.Forms.Primitives;

namespace System.Windows.Forms;

/// <summary>
///  Displays an image that can be a graphic from a bitmap, icon, or metafile, as well as from
///  an enhanced metafile, JPEG, or GIF files.
/// </summary>
[DefaultProperty(nameof(Image))]
[DefaultBindingProperty(nameof(Image))]
[Docking(DockingBehavior.Ask)]
[Designer($"System.Windows.Forms.Design.PictureBoxDesigner, {AssemblyRef.SystemDesign}")]
[SRDescription(nameof(SR.DescriptionPictureBox))]
public partial class PictureBox : Control, ISupportInitialize
{
    private static readonly bool s_useWebRequest =
        !AppContext.TryGetSwitch("System.Windows.Forms.PictureBox.UseWebRequest", out bool useWebRequest)
        || useWebRequest;

    private static readonly HttpClient s_httpClient = !LocalAppContextSwitches.ServicePointManagerCheckCrl ? new() :
        new(new HttpClientHandler { CheckCertificateRevocationList = true });

    /// <summary>
    ///  The type of border this control will have.
    /// </summary>
    private BorderStyle _borderStyle = BorderStyle.None;

    /// <summary>
    ///  The image being displayed.
    /// </summary>
    private Image? _image;

    /// <summary>
    ///  Controls how the image is placed within our bounds, or how we are sized to fit said image.
    /// </summary>
    private PictureBoxSizeMode _sizeMode = PictureBoxSizeMode.Normal;

    private Size _savedSize;

    private bool _currentlyAnimating;

    // Instance members for asynchronous behavior
    private AsyncOperation? _currentAsyncLoadOperation;

    private FileStream? _fileStream;
    private string? _imageLocation;
    private Image? _initialImage;
    private Image? _errorImage;
    private int _contentLength;
    private int _totalBytesRead;
    private MemoryStream? _tempDownloadStream;
    private const int ReadBlockSize = 4096;
    private byte[]? _readBuffer;
    private ImageInstallationType _imageInstallationType;
    private SendOrPostCallback? _loadCompletedDelegate;
    private SendOrPostCallback? _loadProgressDelegate;
    private bool _handleValid;
    private readonly Lock _internalSyncObject = new();

    // These default images will be demand loaded.
    private Image? _defaultInitialImage;
    private Image? _defaultErrorImage;

    [ThreadStatic]
    private static Image? t_defaultInitialImageForThread;

    [ThreadStatic]
    private static Image? t_defaultErrorImageForThread;

    private static readonly object s_loadCompletedKey = new();
    private static readonly object s_loadProgressChangedKey = new();

    private const int AsyncOperationInProgressState = 0x00000001;
    private const int CancellationPendingState = 0x00000002;
    private const int UseDefaultInitialImageState = 0x00000004;
    private const int UseDefaultErrorImageState = 0x00000008;
    private const int WaitOnLoadState = 0x00000010;
    private const int NeedToLoadImageLocationState = 0x00000020;
    private const int InInitializationState = 0x00000040;

    // PERF: take all the bools and put them into a state variable
    private BitVector32 _pictureBoxState; // see PICTUREBOXSTATE_ constants above

    /// <summary>
    ///  https://docs.microsoft.com/dotnet/api/system.drawing.image.fromstream#System_Drawing_Image_FromStream_System_IO_Stream_
    ///  if we load an image from a stream, we must keep the stream open for the lifetime of the Image
    /// </summary>
    private StreamReader? _localImageStreamReader;
    private Stream? _uriImageStream;

    /// <summary>
    ///  Creates a new picture with all default properties and no Image. The default PictureBox.SizeMode
    ///  will be PictureBoxSizeMode.NORMAL.
    /// </summary>
    public PictureBox()
    {
        // this class overrides GetPreferredSizeCore, let Control automatically cache the result
        SetExtendedState(ExtendedStates.UserPreferredSizeCache, true);

        _pictureBoxState = new BitVector32(UseDefaultErrorImageState | UseDefaultInitialImageState);

        SetStyle(ControlStyles.Opaque | ControlStyles.Selectable, false);
        SetStyle(ControlStyles.OptimizedDoubleBuffer | ControlStyles.SupportsTransparentBackColor, true);

#pragma warning disable WFO5001 // Type is for evaluation purposes only and is subject to change or removal in future updates. Suppress this diagnostic to proceed.
        SetStyle(ControlStyles.ApplyThemingImplicitly, true);
#pragma warning restore WFO5001

        TabStop = false;
        _savedSize = Size;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override bool AllowDrop
    {
        get => base.AllowDrop;
        set => base.AllowDrop = value;
    }

    /// <summary>
    ///  Indicates the border style for the control.
    /// </summary>
    [DefaultValue(BorderStyle.None)]
    [SRCategory(nameof(SR.CatAppearance))]
    [DispId(PInvokeCore.DISPID_BORDERSTYLE)]
    [SRDescription(nameof(SR.PictureBoxBorderStyleDescr))]
    public BorderStyle BorderStyle
    {
        get => _borderStyle;
        set
        {
            SourceGenerated.EnumValidator.Validate(value);

            if (_borderStyle != value)
            {
                _borderStyle = value;
                RecreateHandle();
                AdjustSize();
            }
        }
    }

    /// <summary>
    ///  Try to build a URI, but if that fails, that means it's a relative path, and we treat it as
    ///  relative to the working directory (which is what GetFullPath uses).
    /// </summary>
    private static Uri CalculateUri(string path)
    {
        try
        {
            return new Uri(path);
        }
        catch (UriFormatException)
        {
            // It's a relative pathname, get its full path as a file.
            path = Path.GetFullPath(path);
            return new Uri(path);
        }
    }

    [SRCategory(nameof(SR.CatAsynchronous))]
    [SRDescription(nameof(SR.PictureBoxCancelAsyncDescr))]
    public void CancelAsync()
    {
        _pictureBoxState[CancellationPendingState] = true;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new bool CausesValidation
    {
        get => base.CausesValidation;
        set => base.CausesValidation = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? CausesValidationChanged
    {
        add => base.CausesValidationChanged += value;
        remove => base.CausesValidationChanged -= value;
    }

    /// <summary>
    ///  Returns the parameters needed to create the handle.
    /// </summary>
    protected override CreateParams CreateParams
    {
        get
        {
            CreateParams cp = base.CreateParams;

            switch (_borderStyle)
            {
                case BorderStyle.Fixed3D:
                    cp.ExStyle |= (int)WINDOW_EX_STYLE.WS_EX_CLIENTEDGE;
                    break;
                case BorderStyle.FixedSingle:
                    cp.Style |= (int)WINDOW_STYLE.WS_BORDER;
                    break;
            }

            return cp;
        }
    }

    protected override ImeMode DefaultImeMode => ImeMode.Disable;

    /// <summary>
    ///  Deriving classes can override this to configure a default size for their control.
    ///  This is more efficient than setting the size in the control's constructor.
    /// </summary>
    protected override Size DefaultSize => new(100, 50);

    [SRCategory(nameof(SR.CatAsynchronous))]
    [Localizable(true)]
    [RefreshProperties(RefreshProperties.All)]
    [SRDescription(nameof(SR.PictureBoxErrorImageDescr))]
    public Image? ErrorImage
    {
        get
        {
            // Strange pictureBoxState[PICTUREBOXSTATE_useDefaultErrorImage] approach used
            // here to avoid statically loading the default bitmaps from resources at
            // runtime when they're never used.
            if (_errorImage is null && _pictureBoxState[UseDefaultErrorImageState])
            {
                if (_defaultErrorImage is null)
                {
                    // Can't share images across threads.
                    t_defaultErrorImageForThread ??= ScaleHelper.GetIconResourceAsDefaultSizeBitmap(typeof(PictureBox), "ImageInError");

                    _defaultErrorImage = t_defaultErrorImageForThread;
                }

                _errorImage = _defaultErrorImage;
            }

            return _errorImage;
        }
        set
        {
            if (ErrorImage != value)
            {
                _pictureBoxState[UseDefaultErrorImageState] = false;
            }

            _errorImage = value;
        }
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override Color ForeColor
    {
        get => base.ForeColor;
        set => base.ForeColor = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? ForeColorChanged
    {
        add => base.ForeColorChanged += value;
        remove => base.ForeColorChanged -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [AllowNull]
    public override Font Font
    {
        get => base.Font;
        set => base.Font = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? FontChanged
    {
        add => base.FontChanged += value;
        remove => base.FontChanged -= value;
    }

    /// <summary>
    ///  Retrieves the Image that the <see cref="PictureBox"/> is currently displaying.
    /// </summary>
    [SRCategory(nameof(SR.CatAppearance))]
    [Localizable(true)]
    [Bindable(true)]
    [SRDescription(nameof(SR.PictureBoxImageDescr))]
    public Image? Image
    {
        get => _image;
        set => InstallNewImage(value, ImageInstallationType.DirectlySpecified);
    }

    // The area occupied by the image
    [SRCategory(nameof(SR.CatAsynchronous))]
    [Localizable(true)]
    [DefaultValue(null)]
    [RefreshProperties(RefreshProperties.All)]
    [SRDescription(nameof(SR.PictureBoxImageLocationDescr))]
    public string? ImageLocation
    {
        get => _imageLocation;
        set
        {
            // Reload even if value hasn't changed, since Image itself may have changed.
            _imageLocation = value;

            _pictureBoxState[NeedToLoadImageLocationState] = !string.IsNullOrEmpty(_imageLocation);

            // Reset main image if it hasn't been directly specified.
            if (string.IsNullOrEmpty(_imageLocation) && _imageInstallationType != ImageInstallationType.DirectlySpecified)
            {
                InstallNewImage(null, ImageInstallationType.DirectlySpecified);
            }

            if (WaitOnLoad && !_pictureBoxState[InInitializationState] && !string.IsNullOrEmpty(_imageLocation))
            {
                // Load immediately, so any error will occur synchronously
                Load();
            }

            Invalidate();
        }
    }

    private Rectangle ImageRectangle => ImageRectangleFromSizeMode(_sizeMode);

    private Rectangle ImageRectangleFromSizeMode(PictureBoxSizeMode mode)
    {
        Rectangle result = LayoutUtils.DeflateRect(ClientRectangle, Padding);
        if (_image is not null)
        {
            switch (mode)
            {
                case PictureBoxSizeMode.Normal:
                case PictureBoxSizeMode.AutoSize:
                    // Use image's size rather than client size.
                    result.Size = _image.Size;
                    break;

                case PictureBoxSizeMode.StretchImage:
                    // Do nothing, was initialized to the available dimensions.
                    break;

                case PictureBoxSizeMode.CenterImage:
                    // Center within the available space.
                    result.X += (result.Width - _image.Width) / 2;
                    result.Y += (result.Height - _image.Height) / 2;
                    result.Size = _image.Size;
                    break;

                case PictureBoxSizeMode.Zoom:
                    Size imageSize = _image.Size;
                    float ratio = Math.Min(ClientRectangle.Width / (float)imageSize.Width, ClientRectangle.Height / (float)imageSize.Height);
                    result.Width = (int)(imageSize.Width * ratio);
                    result.Height = (int)(imageSize.Height * ratio);
                    result.X = (ClientRectangle.Width - result.Width) / 2;
                    result.Y = (ClientRectangle.Height - result.Height) / 2;
                    break;

                default:
                    Debug.Fail($"Unsupported PictureBoxSizeMode value: {mode}");
                    break;
            }
        }

        return result;
    }

    [SRCategory(nameof(SR.CatAsynchronous))]
    [Localizable(true)]
    [RefreshProperties(RefreshProperties.All)]
    [SRDescription(nameof(SR.PictureBoxInitialImageDescr))]
    public Image? InitialImage
    {
        get
        {
            // Strange pictureBoxState[PICTUREBOXSTATE_useDefaultInitialImage] approach
            // used here to avoid statically loading the default bitmaps from resources at
            // runtime when they're never used.
            if (_initialImage is null && _pictureBoxState[UseDefaultInitialImageState])
            {
                if (_defaultInitialImage is null)
                {
                    // Can't share images across threads.
                    t_defaultInitialImageForThread ??= ScaleHelper.GetIconResourceAsDefaultSizeBitmap(typeof(PictureBox), "PictureBox.Loading");
                    _defaultInitialImage = t_defaultInitialImageForThread;
                }

                _initialImage = _defaultInitialImage;
            }

            return _initialImage;
        }
        set
        {
            if (InitialImage != value)
            {
                _pictureBoxState[UseDefaultInitialImageState] = false;
            }

            _initialImage = value;
        }
    }

    private void InstallNewImage(Image? value, ImageInstallationType installationType)
    {
        StopAnimate();
        _image = value;

        LayoutTransaction.DoLayoutIf(AutoSize, this, this, PropertyNames.Image);

        Animate();
        if (installationType != ImageInstallationType.ErrorOrInitial)
        {
            AdjustSize();
        }

        _imageInstallationType = installationType;

        Invalidate();
        CommonProperties.xClearPreferredSizeCache(this);
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new ImeMode ImeMode
    {
        get => base.ImeMode;
        set => base.ImeMode = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? ImeModeChanged
    {
        add => base.ImeModeChanged += value;
        remove => base.ImeModeChanged -= value;
    }

    /// <summary>
    ///  Synchronous load
    /// </summary>
    [SRCategory(nameof(SR.CatAsynchronous))]
    [SRDescription(nameof(SR.PictureBoxLoad0Descr))]
    public void Load()
    {
        if (string.IsNullOrEmpty(_imageLocation))
        {
            throw new InvalidOperationException(SR.PictureBoxNoImageLocation);
        }

        // If the load and install fails, pictureBoxState[PICTUREBOXSTATE_needToLoadImageLocation] will be
        // false to prevent subsequent attempts.
        _pictureBoxState[NeedToLoadImageLocationState] = false;

        try
        {
            DisposeImageStream();
            Uri uri = CalculateUri(_imageLocation);
            if (uri.IsFile)
            {
                _localImageStreamReader = new StreamReader(uri.LocalPath);
                Image img = Image.FromStream(_localImageStreamReader.BaseStream);
                InstallNewImage(img, ImageInstallationType.FromUrl);
            }
            else if (UseWebRequest())
            {
                // Run async operation synchronously to avoid blocking UI thread and potential deadlocks.
                Task.Run(async () =>
                {
                    _uriImageStream = await s_httpClient.GetStreamAsync(uri).ConfigureAwait(false);
                    Image img = Image.FromStream(_uriImageStream);
                    InstallNewImage(img, ImageInstallationType.FromUrl);
                }).GetAwaiter().GetResult();
            }
            else
            {
                throw new NotSupportedException(SR.PictureBoxRemoteLocationNotSupported);
            }
        }
        catch
        {
            if (!DesignMode)
            {
                throw;
            }
            else
            {
                // In design mode, just replace with Error bitmap.
                InstallNewImage(ErrorImage, ImageInstallationType.ErrorOrInitial);
            }
        }
    }

    [SRCategory(nameof(SR.CatAsynchronous))]
    [SRDescription(nameof(SR.PictureBoxLoad1Descr))]
    public void Load(string url)
    {
        ImageLocation = url;
        Load();
    }

    [SRCategory(nameof(SR.CatAsynchronous))]
    [SRDescription(nameof(SR.PictureBoxLoadAsync0Descr))]
    public void LoadAsync()
    {
        if (string.IsNullOrEmpty(_imageLocation))
        {
            throw new InvalidOperationException(SR.PictureBoxNoImageLocation);
        }

        if (_pictureBoxState[AsyncOperationInProgressState])
        {
            // We shouldn't throw here: just return.
            return;
        }

        _pictureBoxState[AsyncOperationInProgressState] = true;

        if ((Image is null || (_imageInstallationType == ImageInstallationType.ErrorOrInitial)) && InitialImage is not null)
        {
            InstallNewImage(InitialImage, ImageInstallationType.ErrorOrInitial);
        }

        _currentAsyncLoadOperation = AsyncOperationManager.CreateOperation(null);

        if (_loadCompletedDelegate is null)
        {
            _loadCompletedDelegate = new SendOrPostCallback(LoadCompletedDelegate);
            _loadProgressDelegate = new SendOrPostCallback(LoadProgressDelegate);
            _readBuffer = new byte[ReadBlockSize];
        }

        _pictureBoxState[NeedToLoadImageLocationState] = false;
        _pictureBoxState[CancellationPendingState] = false;
        _contentLength = -1;
        _tempDownloadStream = new MemoryStream();
        if (UseWebRequest())
        {
            StartLoadViaWebRequest();
        }
        else
        {
            var uri = CalculateUri(_imageLocation);
            if (uri.IsFile)
            {
                LoadFromFileAsync();
            }
            else
            {
                throw new NotSupportedException(SR.PictureBoxRemoteLocationNotSupported);
            }
        }
    }

    private void LoadFromFileAsync()
    {
        try
        {
            _fileStream = File.OpenRead(_imageLocation!);
            _contentLength = (int)_fileStream.Length;
            _totalBytesRead = 0;

            _fileStream.BeginRead(
                _readBuffer!,
                0,
                ReadBlockSize,
                new AsyncCallback(ReadCallBack),
                _fileStream);
        }
        catch (Exception error)
        {
            PostCompleted(error, cancelled: false);
        }
    }

    private void StartLoadViaWebRequest()
    {
#pragma warning disable SYSLIB0014 // Type or member is obsolete
        WebRequest req = WebRequest.Create(CalculateUri(_imageLocation!));
#pragma warning restore SYSLIB0014

        Task.Run(() =>
        {
            // Invoke BeginGetResponse on a threadpool thread, as it has unpredictable latency
            req.BeginGetResponse(new AsyncCallback(GetResponseCallback), req);
        });
    }

    private void PostCompleted(Exception? error, bool cancelled)
    {
        AsyncOperation? temp = _currentAsyncLoadOperation;
        _currentAsyncLoadOperation = null;
        temp?.PostOperationCompleted(_loadCompletedDelegate!, new AsyncCompletedEventArgs(error, cancelled, null));
    }

    private void LoadCompletedDelegate(object? arg)
    {
        AsyncCompletedEventArgs e = (AsyncCompletedEventArgs)arg!;

        Image? img = ErrorImage;
        ImageInstallationType installType = ImageInstallationType.ErrorOrInitial;
        if (!e.Cancelled && e.Error is null)
        {
            // successful completion
            try
            {
                img = Image.FromStream(_tempDownloadStream!);
                installType = ImageInstallationType.FromUrl;
            }
            catch (Exception error)
            {
                e = new AsyncCompletedEventArgs(error, false, null);
            }
        }

        // If cancelled, don't change the image
        if (!e.Cancelled)
        {
            InstallNewImage(img, installType);
        }

        _fileStream?.Dispose();
        _fileStream = null;
        _tempDownloadStream = null;
        _pictureBoxState[CancellationPendingState] = false;
        _pictureBoxState[AsyncOperationInProgressState] = false;
        OnLoadCompleted(e);
    }

    private void LoadProgressDelegate(object? arg) => OnLoadProgressChanged((ProgressChangedEventArgs)arg!);

    private void GetResponseCallback(IAsyncResult result)
    {
        if (_pictureBoxState[CancellationPendingState])
        {
            PostCompleted(error: null, cancelled: true);
            return;
        }

        try
        {
            WebRequest req = (WebRequest)result.AsyncState!;
            WebResponse webResponse = req.EndGetResponse(result);

            _contentLength = (int)webResponse.ContentLength;
            _totalBytesRead = 0;

            Stream responseStream = webResponse.GetResponseStream();

            // Continue on with asynchronous reading.
            responseStream.BeginRead(
                _readBuffer!,
                0,
                ReadBlockSize,
                new AsyncCallback(ReadCallBack),
                responseStream);
        }
        catch (Exception error)
        {
            // Since this is on a non-UI thread, we catch any exceptions and
            // pass them back as data to the UI-thread.
            PostCompleted(error, cancelled: false);
        }
    }

    private void ReadCallBack(IAsyncResult result)
    {
        if (_pictureBoxState[CancellationPendingState])
        {
            PostCompleted(error: null, cancelled: true);
            return;
        }

        Stream responseStream = (Stream)result.AsyncState!;
        try
        {
            int bytesRead = responseStream.EndRead(result);

            if (bytesRead > 0)
            {
                _totalBytesRead += bytesRead;
                _tempDownloadStream!.Write(_readBuffer!, 0, bytesRead);

                responseStream.BeginRead(
                    _readBuffer!,
                    0,
                    ReadBlockSize,
                    new AsyncCallback(ReadCallBack),
                    responseStream);

                // Report progress thus far, but only if we know total length.
                if (_contentLength != -1)
                {
                    int progress = (int)(100 * (_totalBytesRead / ((float)_contentLength)));
                    _currentAsyncLoadOperation?.Post(
                        _loadProgressDelegate!,
                        new ProgressChangedEventArgs(progress, null));
                }
            }
            else
            {
                _tempDownloadStream!.Seek(0, SeekOrigin.Begin);
                _currentAsyncLoadOperation?.Post(
                    _loadProgressDelegate!,
                    new ProgressChangedEventArgs(100, null));

                PostCompleted(error: null, cancelled: false);

                // Do this so any exception that Close() throws will be
                // dealt with ok.
                Stream rs = responseStream;
                responseStream = null!;
                rs.Close();
            }
        }
        catch (Exception error)
        {
            // Since this is on a non-UI thread, we catch any exceptions and
            // pass them back as data to the UI-thread.
            PostCompleted(error, cancelled: false);
            responseStream?.Close();
        }
    }

    [SRCategory(nameof(SR.CatAsynchronous))]
    [SRDescription(nameof(SR.PictureBoxLoadAsync1Descr))]
    public void LoadAsync(string url)
    {
        ImageLocation = url;
        LoadAsync();
    }

    [SRCategory(nameof(SR.CatAsynchronous))]
    [SRDescription(nameof(SR.PictureBoxLoadCompletedDescr))]
    public event AsyncCompletedEventHandler? LoadCompleted
    {
        add => Events.AddHandler(s_loadCompletedKey, value);
        remove => Events.RemoveHandler(s_loadCompletedKey, value);
    }

    [SRCategory(nameof(SR.CatAsynchronous))]
    [SRDescription(nameof(SR.PictureBoxLoadProgressChangedDescr))]
    public event ProgressChangedEventHandler? LoadProgressChanged
    {
        add => Events.AddHandler(s_loadProgressChangedKey, value);
        remove => Events.RemoveHandler(s_loadProgressChangedKey, value);
    }

    private void ResetInitialImage()
    {
        _pictureBoxState[UseDefaultInitialImageState] = true;
        _initialImage = _defaultInitialImage;
    }

    private void ResetErrorImage()
    {
        _pictureBoxState[UseDefaultErrorImageState] = true;
        _errorImage = _defaultErrorImage;
    }

    private void ResetImage()
    {
        InstallNewImage(null, ImageInstallationType.DirectlySpecified);
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public override RightToLeft RightToLeft
    {
        get => base.RightToLeft;
        set => base.RightToLeft = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? RightToLeftChanged
    {
        add => base.RightToLeftChanged += value;
        remove => base.RightToLeftChanged -= value;
    }

    /// <summary>
    ///  Be sure not to re-serialized initial image if it's the default.
    /// </summary>
    private bool ShouldSerializeInitialImage() => !_pictureBoxState[UseDefaultInitialImageState];

    /// <summary>
    ///  Be sure not to re-serialized error image if it's the default.
    /// </summary>
    private bool ShouldSerializeErrorImage() => !_pictureBoxState[UseDefaultErrorImageState];

    /// <summary>
    ///  Be sure not to serialize image if it wasn't directly specified
    ///  through the Image property (e.g., if it's a download, or an initial
    ///  or error image)
    /// </summary>
    private bool ShouldSerializeImage() =>
        (_imageInstallationType == ImageInstallationType.DirectlySpecified) && (Image is not null);

    /// <summary>
    ///  Indicates how the image is displayed.
    /// </summary>
    [DefaultValue(PictureBoxSizeMode.Normal)]
    [SRCategory(nameof(SR.CatBehavior))]
    [Localizable(true)]
    [SRDescription(nameof(SR.PictureBoxSizeModeDescr))]
    [RefreshProperties(RefreshProperties.Repaint)]
    public PictureBoxSizeMode SizeMode
    {
        get => _sizeMode;
        set
        {
            SourceGenerated.EnumValidator.Validate(value);

            if (_sizeMode != value)
            {
                if (value == PictureBoxSizeMode.AutoSize)
                {
                    AutoSize = true;
                    SetStyle(ControlStyles.FixedHeight | ControlStyles.FixedWidth, true);
                }

                if (value != PictureBoxSizeMode.AutoSize)
                {
                    AutoSize = false;
                    SetStyle(ControlStyles.FixedHeight | ControlStyles.FixedWidth, false);
                    _savedSize = Size;
                }

                _sizeMode = value;
                AdjustSize();
                Invalidate();
                OnSizeModeChanged(EventArgs.Empty);
            }
        }
    }

    private static readonly object s_sizeModeChangedEvent = new();

    [SRCategory(nameof(SR.CatPropertyChanged))]
    [SRDescription(nameof(SR.PictureBoxOnSizeModeChangedDescr))]
    public event EventHandler? SizeModeChanged
    {
        add => Events.AddHandler(s_sizeModeChangedEvent, value);
        remove => Events.RemoveHandler(s_sizeModeChangedEvent, value);
    }

    internal override bool SupportsUiaProviders => true;

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new bool TabStop
    {
        get => base.TabStop;
        set => base.TabStop = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? TabStopChanged
    {
        add => base.TabStopChanged += value;
        remove => base.TabStopChanged -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new int TabIndex
    {
        get => base.TabIndex;
        set => base.TabIndex = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? TabIndexChanged
    {
        add => base.TabIndexChanged += value;
        remove => base.TabIndexChanged -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    [Bindable(false)]
    [AllowNull]
    public override string Text
    {
        get => base.Text;
        set => base.Text = value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? TextChanged
    {
        add => base.TextChanged += value;
        remove => base.TextChanged -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? Enter
    {
        add => base.Enter += value;
        remove => base.Enter -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event KeyEventHandler? KeyUp
    {
        add => base.KeyUp += value;
        remove => base.KeyUp -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event KeyEventHandler? KeyDown
    {
        add => base.KeyDown += value;
        remove => base.KeyDown -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event KeyPressEventHandler? KeyPress
    {
        add => base.KeyPress += value;
        remove => base.KeyPress -= value;
    }

    [Browsable(false)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public new event EventHandler? Leave
    {
        add => base.Leave += value;
        remove => base.Leave -= value;
    }

    /// <summary>
    ///  If the PictureBox has the SizeMode property set to AutoSize, this makes sure that the
    ///  picturebox is large enough to hold the image.
    /// </summary>
    private void AdjustSize()
    {
        if (_sizeMode == PictureBoxSizeMode.AutoSize)
        {
            Size = PreferredSize;
        }
        else
        {
            Size = _savedSize;
        }
    }

    private void Animate() => Animate(animate: !DesignMode && Visible && Enabled && ParentInternal is not null);

    private void StopAnimate() => Animate(animate: false);

    private void Animate(bool animate)
    {
        if (animate != _currentlyAnimating)
        {
            if (animate)
            {
                if (_image is not null)
                {
                    ImageAnimator.Animate(_image, OnFrameChanged);
                    _currentlyAnimating = animate;
                }
            }
            else
            {
                if (_image is not null)
                {
                    ImageAnimator.StopAnimate(_image, OnFrameChanged);
                    _currentlyAnimating = animate;
                }
            }
        }
    }

    protected override AccessibleObject CreateAccessibilityInstance()
        => new PictureBoxAccessibleObject(this);

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            StopAnimate();
        }

        DisposeImageStream();
        base.Dispose(disposing);
    }

    private void DisposeImageStream()
    {
        if (_localImageStreamReader is not null)
        {
            _localImageStreamReader.Dispose();
            _localImageStreamReader = null;
        }

        if (_uriImageStream is not null)
        {
            _uriImageStream.Dispose();
            _localImageStreamReader = null;
        }
    }

    /// <summary>
    ///  Overriding this method allows us to get the caching and clamping the proposedSize/output to
    ///  MinimumSize / MaximumSize from GetPreferredSize for free.
    /// </summary>
    internal override Size GetPreferredSizeCore(Size proposedSize)
    {
        if (_image is null)
        {
            return CommonProperties.GetSpecifiedBounds(this).Size;
        }
        else
        {
            Size bordersAndPadding = SizeFromClientSize(Size.Empty) + Padding.Size;
            return _image.Size + bordersAndPadding;
        }
    }

    protected override void OnEnabledChanged(EventArgs e)
    {
        base.OnEnabledChanged(e);
        Animate();
    }

    private void OnFrameChanged(object? o, EventArgs e)
    {
        if (Disposing || IsDisposed)
        {
            return;
        }

        // Handle should be created, before calling the BeginInvoke.
        if (InvokeRequired && IsHandleCreated)
        {
            lock (_internalSyncObject)
            {
                if (_handleValid)
                {
                    BeginInvoke(new EventHandler(OnFrameChanged), o, e);
                }

                return;
            }
        }

        Invalidate();
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected override void OnHandleDestroyed(EventArgs e)
    {
        lock (_internalSyncObject)
        {
            _handleValid = false;
        }

        base.OnHandleDestroyed(e);
    }

    [EditorBrowsable(EditorBrowsableState.Advanced)]
    protected override void OnHandleCreated(EventArgs e)
    {
        lock (_internalSyncObject)
        {
            _handleValid = true;
        }

        base.OnHandleCreated(e);
    }

    protected virtual void OnLoadCompleted(AsyncCompletedEventArgs e) =>
        ((AsyncCompletedEventHandler?)(Events[s_loadCompletedKey]))?.Invoke(this, e);

    protected virtual void OnLoadProgressChanged(ProgressChangedEventArgs e) =>
        ((ProgressChangedEventHandler?)(Events[s_loadProgressChangedKey]))?.Invoke(this, e);

    /// <summary>
    ///  Overridden onPaint to make sure that the image is painted correctly.
    /// </summary>
    protected override void OnPaint(PaintEventArgs pe)
    {
        if (_pictureBoxState[NeedToLoadImageLocationState])
        {
            try
            {
                if (WaitOnLoad)
                {
                    Load();
                }
                else
                {
                    LoadAsync();
                }
            }
            catch (Exception ex) when (!ex.IsCriticalException())
            {
                _image = ErrorImage;
            }
        }

        if (_image is not null && pe is not null)
        {
            Animate();
            ImageAnimator.UpdateFrames(Image);

            // Error and initial image are drawn centered, non-stretched.
            Rectangle drawingRect = _imageInstallationType == ImageInstallationType.ErrorOrInitial
                ? ImageRectangleFromSizeMode(PictureBoxSizeMode.CenterImage)
                : ImageRectangle;

            pe.Graphics.DrawImage(_image, drawingRect);
        }

        // Windows draws the border for us (see CreateParams)
        base.OnPaint(pe!);
    }

    protected override void OnVisibleChanged(EventArgs e)
    {
        base.OnVisibleChanged(e);
        Animate();
    }

    protected override void OnParentChanged(EventArgs e)
    {
        base.OnParentChanged(e);
        Animate();
    }

    /// <summary>
    ///  OnResize override to invalidate entire control in Stetch mode
    /// </summary>
    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        if (_sizeMode == PictureBoxSizeMode.Zoom
            || _sizeMode == PictureBoxSizeMode.StretchImage
            || _sizeMode == PictureBoxSizeMode.CenterImage
            || BackgroundImage is not null)
        {
            Invalidate();
        }

        _savedSize = Size;
    }

    protected virtual void OnSizeModeChanged(EventArgs e)
    {
        if (Events[s_sizeModeChangedEvent] is EventHandler eh)
        {
            eh(this, e);
        }
    }

    /// <summary>
    ///  Returns a string representation for this control.
    /// </summary>
    public override string ToString()
    {
        string s = base.ToString();
        return $"{s}, SizeMode: {_sizeMode:G}";
    }

    [SRCategory(nameof(SR.CatAsynchronous))]
    [Localizable(true)]
    [DefaultValue(false)]
    [SRDescription(nameof(SR.PictureBoxWaitOnLoadDescr))]
    public bool WaitOnLoad
    {
        get => _pictureBoxState[WaitOnLoadState];
        set => _pictureBoxState[WaitOnLoadState] = value;
    }

    void ISupportInitialize.BeginInit()
    {
        _pictureBoxState[InInitializationState] = true;
    }

    void ISupportInitialize.EndInit()
    {
        if (!_pictureBoxState[InInitializationState])
        {
            return;
        }

        // Need to do this in EndInit since there's no guarantee of the
        // order in which ImageLocation and WaitOnLoad will be set.
        if (ImageLocation is not null && ImageLocation.Length != 0 && WaitOnLoad)
        {
            // Load when initialization completes, so any error will occur synchronously
            Load();
        }

        _pictureBoxState[InInitializationState] = false;
    }

    // The Linker is also capable of replacing the value of this method when the application is being trimmed.
    private static bool UseWebRequest() => s_useWebRequest;
}
