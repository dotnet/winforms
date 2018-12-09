// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    using System.Runtime.Serialization.Formatters;
    using System.Runtime.InteropServices;
    using System.Runtime.Versioning;
    using System.Diagnostics;
    using System.Diagnostics.CodeAnalysis;

    using System;
    using System.IO;
    using System.Security.Permissions;
    using System.Drawing;
    using System.Net;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Threading;
    using System.Windows.Forms.Layout;
    using Microsoft.Win32;

    /// <include file='doc\PictureBox.uex' path='docs/doc[@for="PictureBox"]/*' />
    /// <devdoc>
    ///    <para> Displays an image that can be a graphic from a bitmap, 
    ///       icon, or metafile, as well as from
    ///       an enhanced metafile, JPEG, or GIF files.</para>
    /// </devdoc>
    [
    ComVisible(true),
    ClassInterface(ClassInterfaceType.AutoDispatch),
    DefaultProperty(nameof(Image)),
    DefaultBindingProperty(nameof(Image)),
    Docking(DockingBehavior.Ask),
    Designer("System.Windows.Forms.Design.PictureBoxDesigner, " + AssemblyRef.SystemDesign),
    SRDescription(nameof(SR.DescriptionPictureBox))
    ]
    public class PictureBox : Control, ISupportInitialize {

        /// <include file='doc\PictureBox.uex' path='docs/doc[@for="PictureBox.borderStyle"]/*' />
        /// <devdoc>
        ///     The type of border this control will have.
        /// </devdoc>
        private BorderStyle borderStyle = System.Windows.Forms.BorderStyle.None;

        /// <include file='doc\PictureBox.uex' path='docs/doc[@for="PictureBox.image"]/*' />
        /// <devdoc>
        ///     The image being displayed.
        /// </devdoc>
        private Image image;

        /// <include file='doc\PictureBox.uex' path='docs/doc[@for="PictureBox.sizeMode"]/*' />
        /// <devdoc>
        ///     Controls how the image is placed within our bounds, or how we are
        ///     sized to fit said image.
        /// </devdoc>
        private PictureBoxSizeMode sizeMode = PictureBoxSizeMode.Normal;
        private Size savedSize;

        bool currentlyAnimating;

        // Instance members for asynchronous behavior
        private AsyncOperation     currentAsyncLoadOperation = null;
        private string             imageLocation;
        private Image              initialImage;
        private Image              errorImage;
        private int                contentLength;
        private int                totalBytesRead;
        private MemoryStream       tempDownloadStream;
        private const int          readBlockSize = 4096;    
        private byte[]             readBuffer = null;
        private ImageInstallationType imageInstallationType;
        private SendOrPostCallback loadCompletedDelegate = null;
        private SendOrPostCallback loadProgressDelegate = null;
        private bool               handleValid = false;
        private object             internalSyncObject = new object();

        // These default images will be demand loaded.
        private Image              defaultInitialImage = null;
        private Image              defaultErrorImage = null;

        [ ThreadStatic ]
        private static Image       defaultInitialImageForThread = null;
        
        [ ThreadStatic ]
        private static Image       defaultErrorImageForThread = null;


        private static readonly object defaultInitialImageKey = new object();
        private static readonly object defaultErrorImageKey = new object();
        private static readonly object loadCompletedKey = new object();
        private static readonly object loadProgressChangedKey = new object();
        

        private const int   PICTUREBOXSTATE_asyncOperationInProgress    = 0x00000001;
        private const int   PICTUREBOXSTATE_cancellationPending         = 0x00000002;
        private const int   PICTUREBOXSTATE_useDefaultInitialImage      = 0x00000004;
        private const int   PICTUREBOXSTATE_useDefaultErrorImage        = 0x00000008;
        private const int   PICTUREBOXSTATE_waitOnLoad                  = 0x00000010;
        private const int   PICTUREBOXSTATE_needToLoadImageLocation     = 0x00000020;
        private const int   PICTUREBOXSTATE_inInitialization            = 0x00000040;

        // PERF: take all the bools and put them into a state variable
        private System.Collections.Specialized.BitVector32      pictureBoxState; // see PICTUREBOXSTATE_ consts above

        // http://msdn.microsoft.com/en-us/library/93z9ee4x(v=VS.100).aspx
        // if we load an image from a stream, 
        // we must keep the stream open for the lifetime of the Image
        StreamReader localImageStreamReader = null;
        Stream uriImageStream = null;
 
        /// <include file='doc\PictureBox.uex' path='docs/doc[@for="PictureBox.PictureBox"]/*' />
        /// <devdoc>
        ///    <para>Creates a new picture with all default properties and no 
        ///       Image. The default PictureBox.SizeMode will be PictureBoxSizeMode.NORMAL.
        ///    </para>
        /// </devdoc>
        public PictureBox() {
            // this class overrides GetPreferredSizeCore, let Control automatically cache the result
            SetState2(STATE2_USEPREFERREDSIZECACHE, true);  

            pictureBoxState = new System.Collections.Specialized.BitVector32(PICTUREBOXSTATE_useDefaultErrorImage |
                                                                             PICTUREBOXSTATE_useDefaultInitialImage);

            SetStyle(ControlStyles.Opaque |ControlStyles.Selectable , false);
            SetStyle(ControlStyles.OptimizedDoubleBuffer|ControlStyles.SupportsTransparentBackColor, true);


            TabStop = false;
            savedSize = Size;
        }
        
        /// <include file='doc\PictureBox.uex' path='docs/doc[@for="PictureBox.AllowDrop"]/*' />
        /// <internalonly/><hideinheritance/>
        /// <devdoc>
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override bool AllowDrop {
            get {
                return base.AllowDrop;
            }
            set {
                base.AllowDrop = value;
            }
        }

        /// <include file='doc\PictureBox.uex' path='docs/doc[@for="PictureBox.BorderStyle"]/*' />
        /// <devdoc>
        ///    <para> Indicates the
        ///       border style for the control.</para>
        /// </devdoc>
        [
        DefaultValue(BorderStyle.None),
        SRCategory(nameof(SR.CatAppearance)),
        DispId(NativeMethods.ActiveX.DISPID_BORDERSTYLE),
        SRDescription(nameof(SR.PictureBoxBorderStyleDescr))
        ]
        public BorderStyle BorderStyle {
            get {
                return borderStyle;
            }

            set {
                //valid values are 0x0 to 0x2
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)BorderStyle.None, (int)BorderStyle.Fixed3D)){
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(BorderStyle));
                }

                if (borderStyle != value) {
                    borderStyle = value;
                    RecreateHandle();
                    AdjustSize();
                }
            }
        }

        // Try to build a URI, but if that fails, that means it's a relative
        // path, and we treat it as relative to the working directory (which is
        // what GetFullPath uses).
        private Uri CalculateUri(string path)
        {
            Uri uri;
            try
            {
                uri = new Uri(path);
            }
            catch (UriFormatException)
            {
                // It's a relative pathname, get its full path as a file. 
                path = Path.GetFullPath(path);
                uri = new Uri(path);
            }
            return uri;
        }

        /// <include file='doc\PictureBox.uex' path='docs/doc[@for="PictureBox.CancelAsync"]/*' />
        [
        SRCategory(nameof(SR.CatAsynchronous)),
        SRDescription(nameof(SR.PictureBoxCancelAsyncDescr))
        ]
        public void CancelAsync()
        {
            pictureBoxState[PICTUREBOXSTATE_cancellationPending] = true;
        }

        /// <include file='doc\PictureBox.uex' path='docs/doc[@for="PictureBox.CausesValidation"]/*' />
        /// <internalonly/>
        /// <devdoc/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new bool CausesValidation {
            get {
                return base.CausesValidation;
            }
            set {
                base.CausesValidation = value;
            }
        }
        
        /// <include file='doc\PictureBox.uex' path='docs/doc[@for="PictureBox.CausesValidationChanged"]/*' />
        /// <internalonly/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler CausesValidationChanged {
            add {
                base.CausesValidationChanged += value;
            }
            remove {
                base.CausesValidationChanged -= value;
            }
        }

        /// <include file='doc\PictureBox.uex' path='docs/doc[@for="PictureBox.CreateParams"]/*' />
        /// <internalonly/>
        /// <devdoc>
        ///    <para>Returns the parameters needed to create the handle. Inheriting classes
        ///       can override this to provide extra functionality. They should not,
        ///       however, forget to call base.getCreateParams() first to get the struct
        ///       filled up with the basic info.</para>
        /// </devdoc>
        protected override CreateParams CreateParams {
            [SecurityPermission(SecurityAction.LinkDemand, Flags=SecurityPermissionFlag.UnmanagedCode)]
            get {
                CreateParams cp = base.CreateParams;

                switch (borderStyle) {
                    case BorderStyle.Fixed3D:
                        cp.ExStyle |= NativeMethods.WS_EX_CLIENTEDGE;
                        break;
                    case BorderStyle.FixedSingle:
                        cp.Style |= NativeMethods.WS_BORDER;
                        break;
                }

                return cp;
            }
        }
        
        /// <include file='doc\PictureBox.uex' path='docs/doc[@for="PictureBox.DefaultImeMode"]/*' />
        protected override ImeMode DefaultImeMode {
            get {
                return ImeMode.Disable;
            }
        }

        /// <include file='doc\PictureBox.uex' path='docs/doc[@for="PictureBox.DefaultSize"]/*' />
        /// <devdoc>
        ///     Deriving classes can override this to configure a default size for their control.
        ///     This is more efficient than setting the size in the control's constructor.
        /// </devdoc>
        protected override Size DefaultSize {
            get {
                return new Size(100, 50);
            }
        }

        /// <include file='doc\PictureBox.uex' path='docs/doc[@for="PictureBox.ErrorImage"]/*' />
        [
        SRCategory(nameof(SR.CatAsynchronous)),
        Localizable(true),
        RefreshProperties(RefreshProperties.All),
        SRDescription(nameof(SR.PictureBoxErrorImageDescr))
        ]
        public Image ErrorImage {
            [ResourceExposure(ResourceScope.Machine)]
            [ResourceConsumption(ResourceScope.Machine)]
            get {
                // Strange pictureBoxState[PICTUREBOXSTATE_useDefaultErrorImage] approach used
                // here to avoid statically loading the default bitmaps from resources at
                // runtime when they're never used.

                if (errorImage == null && pictureBoxState[PICTUREBOXSTATE_useDefaultErrorImage])
                {
                    if (defaultErrorImage == null)
                    {
                        // Can't share images across threads.
                        if (defaultErrorImageForThread == null)
                        {
                            defaultErrorImageForThread =
                                new Bitmap(typeof(PictureBox),
                                           "ImageInError.bmp");
                        }
                        defaultErrorImage = defaultErrorImageForThread;
                    }
                    errorImage = defaultErrorImage;
                }
                return errorImage;
            }
            set {
                if (ErrorImage != value)
                {
                    pictureBoxState[PICTUREBOXSTATE_useDefaultErrorImage] = false;

                }
                errorImage = value;
            }
        }

        /// <include file='doc\PictureBox.uex' path='docs/doc[@for="PictureBox.ForeColor"]/*' />
        /// <internalonly/><hideinheritance/>
        /// <devdoc>
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override Color ForeColor {
            get {
                return base.ForeColor;
            }
            set {
                base.ForeColor = value;
            }
        }

        /// <include file='doc\PictureBox.uex' path='docs/doc[@for="PictureBox.ForeColorChanged"]/*' />
        /// <internalonly/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler ForeColorChanged {
            add {
                base.ForeColorChanged += value;
            }
            remove {
                base.ForeColorChanged -= value;
            }
        }

        /// <include file='doc\PictureBox.uex' path='docs/doc[@for="PictureBox.Font"]/*' />
        /// <internalonly/><hideinheritance/>
        /// <devdoc>
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override Font Font {
            get {
                return base.Font;
            }
            set {
                base.Font = value;
            }
        }

        /// <include file='doc\PictureBox.uex' path='docs/doc[@for="PictureBox.FontChanged"]/*' />
        /// <internalonly/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler FontChanged {
            add {
                base.FontChanged += value;
            }
            remove {
                base.FontChanged -= value;
            }
        }

        /// <include file='doc\PictureBox.uex' path='docs/doc[@for="PictureBox.Image"]/*' />
        /// <devdoc>
        /// <para>Retrieves the Image that the <see cref='System.Windows.Forms.PictureBox'/> is currently displaying.</para>
        /// </devdoc>
        [
        SRCategory(nameof(SR.CatAppearance)),
        Localizable(true),
        Bindable(true),
        SRDescription(nameof(SR.PictureBoxImageDescr))
        ]
        public Image Image {
            [ResourceExposure(ResourceScope.Machine)]
            get {
                return image;
            }
            set {
                InstallNewImage(value, ImageInstallationType.DirectlySpecified);
            }
        }

        // The area occupied by the image
        /// <include file='doc\PictureBox.uex' path='docs/doc[@for="PictureBox.ImageLocation"]/*' />
        [
         SRCategory(nameof(SR.CatAsynchronous)),
         Localizable(true),
         DefaultValue(null),
         RefreshProperties(RefreshProperties.All),
         SRDescription(nameof(SR.PictureBoxImageLocationDescr))
        ]
        public string ImageLocation 
        {
            get 
            {
                return imageLocation;
            }
            set 
            {
                // Reload even if value hasn't changed, since Image itself may
                // have changed.
                imageLocation = value;

                pictureBoxState[PICTUREBOXSTATE_needToLoadImageLocation] = !string.IsNullOrEmpty(imageLocation);
                
                // Reset main image if it hasn't been directly specified. 
                if (string.IsNullOrEmpty(imageLocation) &&
                    imageInstallationType != ImageInstallationType.DirectlySpecified)
                {
                    InstallNewImage(null, ImageInstallationType.DirectlySpecified);
                }

                if (WaitOnLoad && !pictureBoxState[PICTUREBOXSTATE_inInitialization] && !string.IsNullOrEmpty(imageLocation))
                {
                    // Load immediately, so any error will occur synchronously
                    Load();
                }
                
                Invalidate();
            }
        }

        private Rectangle ImageRectangle {
            get {
                return ImageRectangleFromSizeMode(sizeMode);
            }
        }
        
        private Rectangle ImageRectangleFromSizeMode(PictureBoxSizeMode mode)
        {
            Rectangle result = LayoutUtils.DeflateRect(ClientRectangle, Padding);

            if (image != null) {
                switch (mode) {
                  case PictureBoxSizeMode.Normal:
                  case PictureBoxSizeMode.AutoSize:
                      // Use image's size rather than client size.
                      result.Size = image.Size;
                      break;

                  case PictureBoxSizeMode.StretchImage:
                      // Do nothing, was initialized to the available dimensions.
                      break;

                  case PictureBoxSizeMode.CenterImage:
                      // Center within the available space.
                      result.X += (result.Width - image.Width) / 2;
                      result.Y += (result.Height - image.Height) / 2;
                      result.Size = image.Size;
                      break;
                  
                  case PictureBoxSizeMode.Zoom:
                    Size imageSize = image.Size;
                    float ratio = Math.Min((float)ClientRectangle.Width / (float)imageSize.Width, (float)ClientRectangle.Height / (float)imageSize.Height);
                    result.Width = (int)(imageSize.Width * ratio);
                    result.Height = (int) (imageSize.Height * ratio);
                    result.X = (ClientRectangle.Width - result.Width) /2;
                    result.Y = (ClientRectangle.Height - result.Height) /2;
                    break;

                  default:
                      Debug.Fail("Unsupported PictureBoxSizeMode value: " + mode);
                      break;
                }
            }

            return result;
        }
        
        /// <include file='doc\PictureBox.uex' path='docs/doc[@for="PictureBox.InitialImage"]/*' />
        [
        SRCategory(nameof(SR.CatAsynchronous)),
        Localizable(true),
        RefreshProperties(RefreshProperties.All),
        SRDescription(nameof(SR.PictureBoxInitialImageDescr))
        ]
        public Image InitialImage {
            [ResourceExposure(ResourceScope.Machine)]
            [ResourceConsumption(ResourceScope.Machine)]
            get {
                // Strange pictureBoxState[PICTUREBOXSTATE_useDefaultInitialImage] approach
                // used here to avoid statically loading the default bitmaps from resources at
                // runtime when they're never used.
                
                if (initialImage == null && pictureBoxState[PICTUREBOXSTATE_useDefaultInitialImage])
                {
                    if (defaultInitialImage == null)
                    {
                        // Can't share images across threads.
                        if (defaultInitialImageForThread == null)
                        {
                            defaultInitialImageForThread =
                                new Bitmap(typeof(PictureBox),
                                           "PictureBox.Loading.bmp");
                        }
                        defaultInitialImage = defaultInitialImageForThread;
                    }
                    initialImage = defaultInitialImage;
                }
                return initialImage;
            }
            set {
                if (InitialImage != value)
                {
                    pictureBoxState[PICTUREBOXSTATE_useDefaultInitialImage] = false;
                }
                initialImage = value;
            }
        }
        
        private void InstallNewImage(Image value,
                                     ImageInstallationType installationType)
        {
            StopAnimate();
            this.image = value;
            
            LayoutTransaction.DoLayoutIf(AutoSize, this, this, PropertyNames.Image); 
            
            Animate();
            if (installationType != ImageInstallationType.ErrorOrInitial)
            {
                AdjustSize();
            }
            this.imageInstallationType = installationType;
            
            Invalidate();
            CommonProperties.xClearPreferredSizeCache(this);
        }

        /// <include file='doc\PictureBox.uex' path='docs/doc[@for="PictureBox.ImeMode"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public ImeMode ImeMode {
            get {
                return base.ImeMode;
            }
            set {
                base.ImeMode = value;
            }
        }

        /// <include file='doc\PictureBox.uex' path='docs/doc[@for="PictureBox.ImeModeChanged"]/*' />
        /// <internalonly/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler ImeModeChanged {
            add {
                base.ImeModeChanged += value;
            }
            remove {
                base.ImeModeChanged -= value;
            }
        }

        /// <include file='doc\PictureBox.uex' path='docs/doc[@for="PictureBox.Load"]/*' />
        // Synchronous load
        [
        SRCategory(nameof(SR.CatAsynchronous)),
        SRDescription(nameof(SR.PictureBoxLoad0Descr))
        ]
        public void Load()
        {
            if (imageLocation == null || imageLocation.Length == 0)
            {
                throw new
                    InvalidOperationException(SR.PictureBoxNoImageLocation);
            }

            // If the load and install fails, pictureBoxState[PICTUREBOXSTATE_needToLoadImageLocation] will be
            // false to prevent subsequent attempts.
            pictureBoxState[PICTUREBOXSTATE_needToLoadImageLocation] = false;

            Image img;
            ImageInstallationType installType = ImageInstallationType.FromUrl;
            try
            {
                DisposeImageStream();

                Uri uri = CalculateUri(imageLocation);
                if (uri.IsFile)
                {
                    localImageStreamReader = new StreamReader(uri.LocalPath);
                    img = Image.FromStream(localImageStreamReader.BaseStream);
                }
                else
                {
                    using (WebClient wc = new WebClient())
                    {
                        uriImageStream = wc.OpenRead(uri.ToString());
                        img = Image.FromStream(uriImageStream);
                    }
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
                    img = ErrorImage;
                    installType = ImageInstallationType.ErrorOrInitial;
                }
            }

            InstallNewImage(img, installType);
        }

        /// <include file='doc\PictureBox.uex' path='docs/doc[@for="PictureBox.Load2"]/*' />
        [
        SRCategory(nameof(SR.CatAsynchronous)),
        SRDescription(nameof(SR.PictureBoxLoad1Descr)),
        SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings") // PM review done
        ]
        public void Load(String url)
        {
            this.ImageLocation = url;
            this.Load();
        }

        /// <include file='doc\PictureBox.uex' path='docs/doc[@for="PictureBox.LoadAsync"]/*' />
        [
        SRCategory(nameof(SR.CatAsynchronous)),
        SRDescription(nameof(SR.PictureBoxLoadAsync0Descr))
        ]
        public void LoadAsync()
        {
            if (imageLocation == null || imageLocation.Length == 0)
            {
                throw new
                    InvalidOperationException(SR.PictureBoxNoImageLocation);
            }

            if (pictureBoxState[PICTUREBOXSTATE_asyncOperationInProgress])
            {
                //We shouldn't throw here: just return.
                return;
            }

            pictureBoxState[PICTUREBOXSTATE_asyncOperationInProgress] = true;
            
            if ((Image == null ||
                 (imageInstallationType == ImageInstallationType.ErrorOrInitial))
                && InitialImage != null)
            {
                InstallNewImage(InitialImage, ImageInstallationType.ErrorOrInitial);
            }

            currentAsyncLoadOperation = AsyncOperationManager.CreateOperation(null);
            
            if (loadCompletedDelegate == null)
            {
                loadCompletedDelegate = new SendOrPostCallback(LoadCompletedDelegate);
                loadProgressDelegate = new SendOrPostCallback(LoadProgressDelegate);
                readBuffer = new byte[readBlockSize];
            }
            
            pictureBoxState[PICTUREBOXSTATE_needToLoadImageLocation] = false;
            pictureBoxState[PICTUREBOXSTATE_cancellationPending] = false;
            contentLength = -1;
            tempDownloadStream = new MemoryStream();
                        
            WebRequest req = WebRequest.Create(CalculateUri(imageLocation));

            // Invoke BeginGetResponse on a threadpool thread, as it has
            // unpredictable latency, since, on first call, it may load in the
            // configuration system (this is NCL 
            (new WaitCallback(BeginGetResponseDelegate)).BeginInvoke(req, null, null);
        }

        // Solely for calling BeginGetResponse itself asynchronously.
        private void BeginGetResponseDelegate(object arg)
        {
            WebRequest req = (WebRequest)arg;
            req.BeginGetResponse(new AsyncCallback(GetResponseCallback), req);
        }

        private void PostCompleted(Exception error, bool cancelled)
        {
            AsyncOperation temp = currentAsyncLoadOperation;
            currentAsyncLoadOperation = null;
            if (temp != null)
            {
                temp.PostOperationCompleted(
                    loadCompletedDelegate,
                    new AsyncCompletedEventArgs(error, cancelled, null));
            }
        }

        private void LoadCompletedDelegate(object arg) 
        {
            AsyncCompletedEventArgs e = (AsyncCompletedEventArgs)arg;

            Image                 img         = ErrorImage;
            ImageInstallationType installType = ImageInstallationType.ErrorOrInitial;

            if (!e.Cancelled && e.Error == null)
            {
                // successful completion
                try
                {
                    img = Image.FromStream(tempDownloadStream);
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
            
            tempDownloadStream = null;
            pictureBoxState[PICTUREBOXSTATE_cancellationPending] = false;
            pictureBoxState[PICTUREBOXSTATE_asyncOperationInProgress] = false;
            OnLoadCompleted(e);
        }

        private void LoadProgressDelegate(object arg)
        {
            OnLoadProgressChanged((ProgressChangedEventArgs)arg);
        }
        
        private void GetResponseCallback(IAsyncResult result) 
        {
            if (pictureBoxState[PICTUREBOXSTATE_cancellationPending])
            {
                PostCompleted(null, true);
            }
            else
            {
                try
                {
                    WebRequest req = (WebRequest)result.AsyncState;
                    WebResponse webResponse = req.EndGetResponse(result);

                    contentLength = (int)webResponse.ContentLength;
                    totalBytesRead = 0;

                    Stream responseStream = webResponse.GetResponseStream();

                    // Continue on with asynchronous reading.
                    responseStream.BeginRead(
                        readBuffer,
                        0,
                        readBlockSize,
                        new AsyncCallback(ReadCallBack),
                        responseStream);

                }
                catch (Exception error)
                {
                    // Since this is on a non-UI thread, we catch any exceptions and
                    // pass them back as data to the UI-thread.
                    PostCompleted(error, false);
                }
            }
        }

        private void ReadCallBack(IAsyncResult result) 
        {
            if (pictureBoxState[PICTUREBOXSTATE_cancellationPending])
            {
                PostCompleted(null, true);
            }
            else
            {
                Stream responseStream = (Stream)result.AsyncState;
                try
                {
                    int bytesRead = responseStream.EndRead(result);

                    if (bytesRead > 0)
                    {
                        totalBytesRead += bytesRead;
                        tempDownloadStream.Write(readBuffer, 0, bytesRead);

                        responseStream.BeginRead(
                            readBuffer,
                            0,
                            readBlockSize,
                            new AsyncCallback(ReadCallBack),
                            responseStream);

                        // Report progress thus far, but only if we know total length.
                        if (contentLength != -1)
                        {
                            int progress = (int)(100 * (((float)totalBytesRead) / ((float)contentLength)));
                            if (currentAsyncLoadOperation != null)
                            {
                                currentAsyncLoadOperation.Post(loadProgressDelegate,
                                       new ProgressChangedEventArgs(progress, null));
                            }
                        }
                    }
                    else
                    {
                        tempDownloadStream.Seek(0, SeekOrigin.Begin);
                        if (currentAsyncLoadOperation != null)
                        {
                            currentAsyncLoadOperation.Post(loadProgressDelegate,
                                       new ProgressChangedEventArgs(100, null));
                        }
                        PostCompleted(null, false);

                        // Do this so any exception that Close() throws will be
                        // dealt with ok.
                        Stream rs = responseStream;
                        responseStream = null;
                        rs.Close();
                    }
                }
                catch (Exception error)
                {
                    // Since this is on a non-UI thread, we catch any exceptions and
                    // pass them back as data to the UI-thread.
                    PostCompleted(error, false);

                    if (responseStream != null)
                    {
                        responseStream.Close();
                    }
                }
            }
        }
        
        
        /// <include file='doc\PictureBox.uex' path='docs/doc[@for="PictureBox.LoadAsync2"]/*' />
        [
        SRCategory(nameof(SR.CatAsynchronous)),
        SRDescription(nameof(SR.PictureBoxLoadAsync1Descr)),
        SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings") // PM review done
        ]
        public void LoadAsync(String url)
        {
            this.ImageLocation = url;
            this.LoadAsync();
        }

        /// <include file='doc\PictureBox.uex' path='docs/doc[@for="PictureBox.LoadCompleted"]/*' />
        [
        SRCategory(nameof(SR.CatAsynchronous)),
        SRDescription(nameof(SR.PictureBoxLoadCompletedDescr))
        ]
        public event AsyncCompletedEventHandler LoadCompleted
        {
            add
            {
                this.Events.AddHandler(loadCompletedKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(loadCompletedKey, value);
            }
        }

        /// <include file='doc\PictureBox.uex' path='docs/doc[@for="PictureBox.LoadProgressChanged"]/*' />
        [
        SRCategory(nameof(SR.CatAsynchronous)),
        SRDescription(nameof(SR.PictureBoxLoadProgressChangedDescr))
        ]
        public event ProgressChangedEventHandler LoadProgressChanged
        {
            add
            {
                this.Events.AddHandler(loadProgressChangedKey, value);
            }
            remove
            {
                this.Events.RemoveHandler(loadProgressChangedKey, value);
            }
        }
        
        private void ResetInitialImage()
        {
            pictureBoxState[PICTUREBOXSTATE_useDefaultInitialImage] = true;
            initialImage = defaultInitialImage;
        }
        
        private void ResetErrorImage()
        {
            pictureBoxState[PICTUREBOXSTATE_useDefaultErrorImage] = true;
            errorImage = defaultErrorImage;
        }
        
        private void ResetImage()
        {
            InstallNewImage(null, ImageInstallationType.DirectlySpecified);
        }

        /// <include file='doc\PictureBox.uex' path='docs/doc[@for="PictureBox.RightToLeft"]/*' />
        /// <internalonly/><hideinheritance/>
        /// <devdoc>
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public override RightToLeft RightToLeft {
            get {
                return base.RightToLeft;
            }
            set {
                base.RightToLeft = value;
            }
        }

        /// <include file='doc\PictureBox.uex' path='docs/doc[@for="PictureBox.RightToLeftChanged"]/*' />
        /// <internalonly/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler RightToLeftChanged {
            add {
                base.RightToLeftChanged += value;
            }
            remove {
                base.RightToLeftChanged -= value;
            }
        }

        // Be sure not to re-serialized initial image if it's the default. 
        private bool ShouldSerializeInitialImage()
        {
            return !pictureBoxState[PICTUREBOXSTATE_useDefaultInitialImage];
        }
        
        // Be sure not to re-serialized error image if it's the default. 
        private bool ShouldSerializeErrorImage()
        {
            return !pictureBoxState[PICTUREBOXSTATE_useDefaultErrorImage];
        }

        // Be sure not to serialize image if it wasn't directly specified
        // through the Image property (e.g., if it's a download, or an initial
        // or error image)
        private bool ShouldSerializeImage()
        {
            return (imageInstallationType == ImageInstallationType.DirectlySpecified)
                && (Image != null);
        }
        
        /// <include file='doc\PictureBox.uex' path='docs/doc[@for="PictureBox.SizeMode"]/*' />
        /// <devdoc>
        ///    <para>Indicates how the image is displayed.</para>
        /// </devdoc>
        [
        DefaultValue(PictureBoxSizeMode.Normal),
        SRCategory(nameof(SR.CatBehavior)),
        Localizable(true),
        SRDescription(nameof(SR.PictureBoxSizeModeDescr)),
        RefreshProperties(RefreshProperties.Repaint)        
        ]
        public PictureBoxSizeMode SizeMode {
            get {
                return sizeMode;
            }
            set {
                //valid values are 0x0 to 0x4
                if (!ClientUtils.IsEnumValid(value, (int)value, (int)PictureBoxSizeMode.Normal, (int)PictureBoxSizeMode.Zoom))
                {
                    throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(PictureBoxSizeMode));
                }
                if (this.sizeMode != value) {
                    if (value == PictureBoxSizeMode.AutoSize) {
                        this.AutoSize = true;
                        SetStyle(ControlStyles.FixedHeight | ControlStyles.FixedWidth, true);
                    }
                    if (value != PictureBoxSizeMode.AutoSize) {
                        this.AutoSize = false;
                        SetStyle(ControlStyles.FixedHeight | ControlStyles.FixedWidth, false);
                        savedSize = Size;
                    }
                    sizeMode = value;
                    AdjustSize();
                    Invalidate();
                    OnSizeModeChanged(EventArgs.Empty);
                }
            }
        }
        
        private static readonly object EVENT_SIZEMODECHANGED = new object();

        /// <include file='doc\PictureBox.uex' path='docs/doc[@for="PictureBox.SizeModeChanged"]/*' />
        [SRCategory(nameof(SR.CatPropertyChanged)), SRDescription(nameof(SR.PictureBoxOnSizeModeChangedDescr))]
        public event EventHandler SizeModeChanged {
            add {
                Events.AddHandler(EVENT_SIZEMODECHANGED, value);
            }

            remove {
                Events.RemoveHandler(EVENT_SIZEMODECHANGED, value);
            }
        }

        /// <include file='doc\PictureBox.uex' path='docs/doc[@for="PictureBox.TabStop"]/*' />
        /// <internalonly/><hideinheritance/>
        /// <devdoc>
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public bool TabStop {
            get {
                return base.TabStop;
            }
            set {
                base.TabStop = value;
            }
        }

        /// <include file='doc\PictureBox.uex' path='docs/doc[@for="PictureBox.TabStopChanged"]/*' />
        /// <internalonly/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler TabStopChanged {
            add {
                base.TabStopChanged += value;
            }
            remove {
                base.TabStopChanged -= value;
            }
        }

        /// <include file='doc\PictureBox.uex' path='docs/doc[@for="PictureBox.TabIndex"]/*' />
        /// <internalonly/><hideinheritance/>
        /// <devdoc>
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public int TabIndex {
            get {
                return base.TabIndex;
            }
            set {
                base.TabIndex = value;
            }
        }     

        /// <include file='doc\PictureBox.uex' path='docs/doc[@for="PictureBox.TabIndexChanged"]/*' />
        /// <internalonly/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler TabIndexChanged {
            add {
                base.TabIndexChanged += value;
            }
            remove {
                base.TabIndexChanged -= value;
            }
        }

        /// <include file='doc\PictureBox.uex' path='docs/doc[@for="PictureBox.Text"]/*' />
        /// <internalonly/><hideinheritance/>        
        /// <devdoc>
        /// </devdoc>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never), Bindable(false)]        
        public override string Text {
            get {
                return base.Text;
            }
            set {
                base.Text = value;
            }
        }
        
        /// <include file='doc\PictureBox.uex' path='docs/doc[@for="PictureBox.TextChanged"]/*' />
        /// <internalonly/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        new public event EventHandler TextChanged {
            add {
                base.TextChanged += value;
            }
            remove {
                base.TextChanged -= value;
            }
        }
        
        /// <include file='doc\PictureBox.uex' path='docs/doc[@for="PictureBox.Enter"]/*' />
        /// <internalonly/><hideinheritance/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler Enter {
            add {
                base.Enter += value;
            }
            remove {
                base.Enter -= value;
            }
        }

        /// <include file='doc\PictureBox.uex' path='docs/doc[@for="PictureBox.KeyUp"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event KeyEventHandler KeyUp {
            add {
                base.KeyUp += value;
            }
            remove {
                base.KeyUp -= value;
            }
        }

        /// <include file='doc\PictureBox.uex' path='docs/doc[@for="PictureBox.KeyDown"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event KeyEventHandler KeyDown {
            add {
                base.KeyDown += value;
            }
            remove {
                base.KeyDown -= value;
            }
        }

        /// <include file='doc\PictureBox.uex' path='docs/doc[@for="PictureBox.KeyPress"]/*' />
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event KeyPressEventHandler KeyPress {
            add {
                base.KeyPress += value;
            }
            remove {
                base.KeyPress -= value;
            }
        }

        /// <include file='doc\PictureBox.uex' path='docs/doc[@for="PictureBox.Leave"]/*' />
        /// <internalonly/><hideinheritance/>
        [Browsable(false), EditorBrowsable(EditorBrowsableState.Never)]
        public new event EventHandler Leave {
            add {
                base.Leave += value;
            }
            remove {
                base.Leave -= value;
            }
        }

        /// <include file='doc\PictureBox.uex' path='docs/doc[@for="PictureBox.AdjustSize"]/*' />
        /// <devdoc>
        ///     If the PictureBox has the SizeMode property set to AutoSize, this makes
        ///     sure that the picturebox is large enough to hold the image.
        /// </devdoc>
        /// <internalonly/>
        private void AdjustSize() {
            if (sizeMode == PictureBoxSizeMode.AutoSize) {
                Size = PreferredSize;
            } 
            else {
                Size = savedSize;
            }
        }

        private void Animate() {
            Animate(!DesignMode && Visible && Enabled && ParentInternal != null);
        }

        private void StopAnimate() {
            Animate(false);
        }

        private void Animate(bool animate) {
            if (animate != this.currentlyAnimating) {
                if (animate) {
                    if (this.image != null) {
                        ImageAnimator.Animate(this.image, new EventHandler(this.OnFrameChanged));
                        this.currentlyAnimating = animate;
                    }
                }
                else {
                    if (this.image != null) {
                        ImageAnimator.StopAnimate(this.image, new EventHandler(this.OnFrameChanged));
                        this.currentlyAnimating = animate;
                    }
                }
            }
        }

        /// <include file='doc\PictureBox.uex' path='docs/doc[@for="PictureBox.Dispose"]/*' />
        /// <devdoc>
        /// </devdoc>
        /// <internalonly/>
        protected override void Dispose(bool disposing) {
            if (disposing) {
                StopAnimate();
            }

            DisposeImageStream();

            base.Dispose(disposing);
        }

        private void DisposeImageStream() {
            if (localImageStreamReader != null) {
                localImageStreamReader.Dispose();
                localImageStreamReader = null;
            }
            if (uriImageStream != null) {
                uriImageStream.Dispose();
                localImageStreamReader = null;
            }
        }

        // Overriding this method allows us to get the caching and clamping the proposedSize/output to
        // MinimumSize / MaximumSize from GetPreferredSize for free.
        internal override Size GetPreferredSizeCore(Size proposedSize) {
            if (image == null) {
                return CommonProperties.GetSpecifiedBounds(this).Size;
            }
            else {
                Size bordersAndPadding = SizeFromClientSize(Size.Empty) + Padding.Size;
                return image.Size + bordersAndPadding;
            }
        }
        
        /// <include file='doc\PictureBox.uex' path='docs/doc[@for="PictureBox.OnEnabledChanged"]/*' />
        protected override void OnEnabledChanged(EventArgs e) {
            base.OnEnabledChanged(e);
            Animate();
        }
        
        private void OnFrameChanged(object o, EventArgs e) {
            if (Disposing || IsDisposed) {
                return;
            }
            // Handle should be created, before calling the BeginInvoke.
            if (InvokeRequired && IsHandleCreated) {
                lock (internalSyncObject) {
                    if (handleValid) {
                        BeginInvoke(new EventHandler(this.OnFrameChanged), o, e);
                    }
                    return;
                }
            }
            
            Invalidate();  
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void OnHandleDestroyed(EventArgs e) {
            lock (internalSyncObject) {
                handleValid = false;
            }
            base.OnHandleDestroyed(e);
        }

        [EditorBrowsable(EditorBrowsableState.Advanced)]
        protected override void OnHandleCreated(EventArgs e) {
            lock (internalSyncObject) {
                handleValid = true;
            }
            base.OnHandleCreated(e);
        }

        /// <include file='doc\PictureBox.uex' path='docs/doc[@for="PictureBox.OnLoadCompleted"]/*' />
        protected virtual void OnLoadCompleted(AsyncCompletedEventArgs e)
        {
            AsyncCompletedEventHandler handler = (AsyncCompletedEventHandler)(Events[loadCompletedKey]);
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <include file='doc\PictureBox.uex' path='docs/doc[@for="PictureBox.OnLoadProgressChanged"]/*' />
        protected virtual void OnLoadProgressChanged(ProgressChangedEventArgs e)
        {
            ProgressChangedEventHandler handler = (ProgressChangedEventHandler)(Events[loadProgressChangedKey]);
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <include file='doc\PictureBox.uex' path='docs/doc[@for="PictureBox.OnPaint"]/*' />
        /// <devdoc>
        ///     Overridden onPaint to make sure that the image is painted correctly.
        /// </devdoc>
        /// <internalonly/>
        protected override void OnPaint(PaintEventArgs pe) {

            if (pictureBoxState[PICTUREBOXSTATE_needToLoadImageLocation])
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
                catch (Exception ex)
                {   //Dont throw but paint error image LoadAsync fails....

                    if (ClientUtils.IsCriticalException(ex))
                    {
                        throw;
                    }
                    image = ErrorImage;
                }
            }
            
            if (image != null) {
                Animate();
                ImageAnimator.UpdateFrames(this.Image);
                
                // Error and initial image are drawn centered, non-stretched.
                Rectangle drawingRect =
                    (imageInstallationType == ImageInstallationType.ErrorOrInitial)
                    ? ImageRectangleFromSizeMode(PictureBoxSizeMode.CenterImage)
                    : ImageRectangle;
                
                pe.Graphics.DrawImage(image, drawingRect);
                
            }

            // Windows draws the border for us (see CreateParams)
            base.OnPaint(pe); // raise Paint event
        }


        /// <include file='doc\PictureBox.uex' path='docs/doc[@for="PictureBox.OnVisibleChanged"]/*' />
        protected override void OnVisibleChanged(EventArgs e) {
            base.OnVisibleChanged(e);
            Animate();
        }

        /// <include file='doc\PictureBox.uex' path='docs/doc[@for="PictureBox.OnParentChanged"]/*' />
        protected override void OnParentChanged(EventArgs e) {
            base.OnParentChanged(e);
            Animate();
        }

        /// <include file='doc\PictureBox.uex' path='docs/doc[@for="PictureBox.OnResize"]/*' />
        /// <devdoc>
        ///     OnResize override to invalidate entire control in Stetch mode
        /// </devdoc>
        /// <internalonly/>
        protected override void OnResize(EventArgs e) {
            base.OnResize(e);
            if (sizeMode == PictureBoxSizeMode.Zoom || sizeMode == PictureBoxSizeMode.StretchImage || sizeMode == PictureBoxSizeMode.CenterImage || BackgroundImage != null) {
                Invalidate();
            }
            savedSize = Size;
        }

        /// <include file='doc\PictureBox.uex' path='docs/doc[@for="PictureBox.OnSizeModeChanged"]/*' />
        protected virtual void OnSizeModeChanged(EventArgs e) {
            EventHandler eh = Events[EVENT_SIZEMODECHANGED] as EventHandler;
            if (eh != null) {
                eh(this, e);
            }
        }

        /// <include file='doc\PictureBox.uex' path='docs/doc[@for="PictureBox.ToString"]/*' />
        /// <devdoc>
        ///     Returns a string representation for this control.
        /// </devdoc>
        /// <internalonly/>
        public override string ToString() {

            string s = base.ToString();
            return s + ", SizeMode: " + sizeMode.ToString("G");
        }

        /// <include file='doc\PictureBox.uex' path='docs/doc[@for="PictureBox.WaitOnLoad"]/*' />
        [
        SRCategory(nameof(SR.CatAsynchronous)),
        Localizable(true),
        DefaultValue(false),
        SRDescription(nameof(SR.PictureBoxWaitOnLoadDescr))
        ]
        public bool WaitOnLoad {
            get {
                return pictureBoxState[PICTUREBOXSTATE_waitOnLoad];
            }
            set {
                pictureBoxState[PICTUREBOXSTATE_waitOnLoad] = value;
            }
        }

        private enum ImageInstallationType
        {
            DirectlySpecified,
            ErrorOrInitial,
            FromUrl
        }

        /// <include file='doc\PictureBox.uex' path='docs/doc[@for="PictureBox.ISupportInitialize.BeginInit"]/*' />
        /// <internalonly/>
        void ISupportInitialize.BeginInit()
        {
            pictureBoxState[PICTUREBOXSTATE_inInitialization] = true;
        }

        /// <include file='doc\PictureBox.uex' path='docs/doc[@for="PictureBox.ISupportInitialize.EndInit"]/*' />
        /// <internalonly/>
        void ISupportInitialize.EndInit()
        {
            Debug.Assert(pictureBoxState[PICTUREBOXSTATE_inInitialization]);

            // Need to do this in EndInit since there's no guarantee of the
            // order in which ImageLocation and WaitOnLoad will be set.
            if (ImageLocation != null && ImageLocation.Length != 0 && WaitOnLoad)
            {
                // Load when initialization completes, so any error will occur synchronously
                Load();
            }
            
            pictureBoxState[PICTUREBOXSTATE_inInitialization] = false;
        }
    }
}


