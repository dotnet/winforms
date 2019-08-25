// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using Moq;
using WinForms.Common.Tests;
using Xunit;

namespace System.Windows.Forms.Tests
{
    public class PictureBoxTests
    {
        private const string PathImageLocation = "bitmaps/nature24bits.jpg";
        private const string UrlImageLocation = "https://github.com/dotnet/corefx-testdata/raw/master/System.Drawing.Common.TestData/bitmaps/nature24bits.jpg";

        [Fact]
        public void PictureBox_Ctor_Default()
        {
            var pictureBox = new SubPictureBox();
            Assert.False(pictureBox.AllowDrop);
            Assert.Equal(AnchorStyles.Top | AnchorStyles.Left, pictureBox.Anchor);
            Assert.False(pictureBox.AutoSize);
            Assert.Equal(Control.DefaultBackColor, pictureBox.BackColor);
            Assert.Null(pictureBox.BackgroundImage);
            Assert.Equal(ImageLayout.Tile, pictureBox.BackgroundImageLayout);
            Assert.Null(pictureBox.BindingContext);
            Assert.Equal(BorderStyle.None, pictureBox.BorderStyle);
            Assert.Equal(50, pictureBox.Bottom);
            Assert.Equal(new Rectangle(0, 0, 100, 50), pictureBox.Bounds);
            Assert.False(pictureBox.CanEnableIme);
            Assert.True(pictureBox.CanRaiseEvents);
            Assert.True(pictureBox.CausesValidation);
            Assert.Equal(new Size(100, 50), pictureBox.ClientSize);
            Assert.Equal(new Rectangle(0, 0, 100, 50), pictureBox.ClientRectangle);
            Assert.Null(pictureBox.Container);
            Assert.False(pictureBox.Created);
            Assert.Same(Cursors.Default, pictureBox.Cursor);
            Assert.Same(Cursors.Default, pictureBox.DefaultCursor);
            Assert.Equal(ImeMode.Disable, pictureBox.DefaultImeMode);
            Assert.Equal(new Padding(3), pictureBox.DefaultMargin);
            Assert.Equal(Size.Empty, pictureBox.DefaultMaximumSize);
            Assert.Equal(Size.Empty, pictureBox.DefaultMinimumSize);
            Assert.Equal(Padding.Empty, pictureBox.DefaultPadding);
            Assert.Equal(new Size(100, 50), pictureBox.DefaultSize);
            Assert.False(pictureBox.DesignMode);
            Assert.Equal(new Rectangle(0, 0, 100, 50), pictureBox.DisplayRectangle);
            Assert.Equal(DockStyle.None, pictureBox.Dock);
            Assert.True(pictureBox.Enabled);
            Assert.NotNull(pictureBox.ErrorImage);
            Assert.Same(pictureBox.ErrorImage, pictureBox.ErrorImage);
            Assert.NotNull(pictureBox.Events);
            Assert.Same(pictureBox.Events, pictureBox.Events);
            Assert.Equal(Control.DefaultFont, pictureBox.Font);
            Assert.Equal(Control.DefaultForeColor, pictureBox.ForeColor);
            Assert.False(pictureBox.HasChildren);
            Assert.Null(pictureBox.Image);
            Assert.Null(pictureBox.ImageLocation);
            Assert.Equal(ImeMode.Disable, pictureBox.ImeMode);
            Assert.Equal(ImeMode.Disable, pictureBox.ImeModeBase);
            Assert.NotNull(pictureBox.InitialImage);
            Assert.Same(pictureBox.InitialImage, pictureBox.InitialImage);
            Assert.Equal(0, pictureBox.Left);
            Assert.Equal(Point.Empty, pictureBox.Location);
            Assert.Equal(new Padding(3), pictureBox.Margin);
            Assert.Equal(Padding.Empty, pictureBox.Padding);
            Assert.Null(pictureBox.Parent);
            Assert.Equal("Microsoft\u00AE .NET", pictureBox.ProductName);
            Assert.Equal(BoundsSpecified.All, pictureBox.RequiredScaling);
            Assert.True(pictureBox.RequiredScalingEnabled);
            Assert.Equal(100, pictureBox.Right);
            Assert.Equal(RightToLeft.No, pictureBox.RightToLeft);
            Assert.Null(pictureBox.Site);
            Assert.Equal(new Size(100, 50), pictureBox.Size);
            Assert.Equal(PictureBoxSizeMode.Normal, pictureBox.SizeMode);
            Assert.Equal(0, pictureBox.TabIndex);
            Assert.False(pictureBox.TabStop);
            Assert.Empty(pictureBox.Text);
            Assert.Equal(0, pictureBox.Top);
            Assert.True(pictureBox.Visible);
            Assert.False(pictureBox.WaitOnLoad);
            Assert.Equal(100, pictureBox.Width);
        }

        [Theory]
        [InlineData(BorderStyle.None, 0x56000000, 0)]
        [InlineData(BorderStyle.FixedSingle, 0x56800000, 0)]
        [InlineData(BorderStyle.Fixed3D, 0x56000000, 0x200)]
        public void PictureBox_CreateParams_Get_ReturnsExpected(BorderStyle borderStyle, int expectedStyle, int expectedExStyle)
        {
            var control = new SubPictureBox
            {
                BorderStyle = borderStyle
            };
            CreateParams createParams = control.CreateParams;
            Assert.Null(createParams.Caption);
            Assert.Null(createParams.ClassName);
            Assert.Equal(0x8, createParams.ClassStyle);
            Assert.Equal(expectedExStyle, createParams.ExStyle);
            Assert.Equal(50, createParams.Height);
            Assert.Equal(IntPtr.Zero, createParams.Parent);
            Assert.Null(createParams.Param);
            Assert.Equal(expectedStyle, createParams.Style);
            Assert.Equal(100, createParams.Width);
            Assert.Equal(0, createParams.X);
            Assert.Equal(0, createParams.Y);
            Assert.Same(createParams, control.CreateParams);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(BorderStyle))]
        public void PictureBox_BorderStyle_Set_GetReturnsExpected(BorderStyle value)
        {
            var pictureBox = new PictureBox
            {
                BorderStyle = value
            };
            Assert.Equal(value, pictureBox.BorderStyle);

            // Set same.
            pictureBox.BorderStyle = value;
            Assert.Equal(value, pictureBox.BorderStyle);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(BorderStyle))]
        public void PictureBox_BorderStyle_SetWithHandle_GetReturnsExpected(BorderStyle value)
        {
            var pictureBox = new PictureBox();
            Assert.NotEqual(IntPtr.Zero, pictureBox.Handle);

            pictureBox.BorderStyle = value;
            Assert.Equal(value, pictureBox.BorderStyle);

            // Set same.
            pictureBox.BorderStyle = value;
            Assert.Equal(value, pictureBox.BorderStyle);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(BorderStyle))]
        public void PictureBox_BorderStyle_SetInvalid_ThrowsInvalidEnumArgumentException(BorderStyle value)
        {
            var pictureBox = new PictureBox();
            Assert.Throws<InvalidEnumArgumentException>("value", () => pictureBox.BorderStyle = value);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void PictureBox_CausesValidation_Set_GetReturnsExpected(bool value)
        {
            var control = new PictureBox
            {
                CausesValidation = value
            };
            Assert.Equal(value, control.CausesValidation);

            // Set same
            control.CausesValidation = value;
            Assert.Equal(value, control.CausesValidation);

            // Set different
            control.CausesValidation = !value;
            Assert.Equal(!value, control.CausesValidation);
        }

        [Fact]
        public void PictureBox_CausesValidation_SetWithHandler_CallsCausesValidationChanged()
        {
            var control = new PictureBox
            {
                CausesValidation = true
            };
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.CausesValidationChanged += handler;

            // Set different.
            control.CausesValidation = false;
            Assert.False(control.CausesValidation);
            Assert.Equal(1, callCount);

            // Set same.
            control.CausesValidation = false;
            Assert.False(control.CausesValidation);
            Assert.Equal(1, callCount);

            // Set different.
            control.CausesValidation = true;
            Assert.True(control.CausesValidation);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.CausesValidationChanged -= handler;
            control.CausesValidation = false;
            Assert.False(control.CausesValidation);
            Assert.Equal(2, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void PictureBox_Enabled_Set_GetReturnsExpected(bool value)
        {
            var control = new Control
            {
                Enabled = value
            };
            Assert.Equal(value, control.Enabled);

            // Set same.
            control.Enabled = value;
            Assert.Equal(value, control.Enabled);

            // Set different.
            control.Enabled = value;
            Assert.Equal(value, control.Enabled);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void PictureBox_Enabled_SetWithHandle_GetReturnsExpected(bool value)
        {
            var control = new Control();
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            control.Enabled = value;
            Assert.Equal(value, control.Enabled);

            // Set same.
            control.Enabled = value;
            Assert.Equal(value, control.Enabled);

            // Set different.
            control.Enabled = value;
            Assert.Equal(value, control.Enabled);
        }

        [Fact]
        public void PictureBox_Enabled_SetWithHandler_CallsEnabledChanged()
        {
            var control = new Control
            {
                Enabled = true
            };
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.EnabledChanged += handler;

            // Set different.
            control.Enabled = false;
            Assert.False(control.Enabled);
            Assert.Equal(1, callCount);

            // Set same.
            control.Enabled = false;
            Assert.False(control.Enabled);
            Assert.Equal(1, callCount);

            // Set different.
            control.Enabled = true;
            Assert.True(control.Enabled);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.EnabledChanged -= handler;
            control.Enabled = false;
            Assert.False(control.Enabled);
            Assert.Equal(2, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetImageTheoryData))]
        public void PictureBox_ErrorImage_Set_GetReturnsExpected(Image value)
        {
            var pictureBox = new PictureBox
            {
                ErrorImage = value
            };
            Assert.Same(value, pictureBox.ErrorImage);
            Assert.Null(pictureBox.Image);

            // Set same.
            pictureBox.ErrorImage = value;
            Assert.Same(value, pictureBox.ErrorImage);
            Assert.Null(pictureBox.Image);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetImageTheoryData))]
        public void PictureBox_ErrorImage_SetWithNonNullOldValue_GetReturnsExpected(Image value)
        {
            var pictureBox = new PictureBox
            {
                ErrorImage = new Bitmap(10, 10)
            };

            pictureBox.ErrorImage = value;
            Assert.Same(value, pictureBox.ErrorImage);
            Assert.Null(pictureBox.Image);

            // Set same.
            pictureBox.ErrorImage = value;
            Assert.Same(value, pictureBox.ErrorImage);
            Assert.Null(pictureBox.Image);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetImageTheoryData))]
        public void PictureBox_ErrorImage_SetWithHandle_GetReturnsExpected(Image value)
        {
            var pictureBox = new PictureBox();
            Assert.NotEqual(IntPtr.Zero, pictureBox.Handle);

            pictureBox.ErrorImage = value;
            Assert.Same(value, pictureBox.ErrorImage);
            Assert.Null(pictureBox.Image);

            // Set same.
            pictureBox.ErrorImage = value;
            Assert.Same(value, pictureBox.ErrorImage);
            Assert.Null(pictureBox.Image);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetImageTheoryData))]
        public void PictureBox_ErrorImage_SetWithNonNullOldValueWithHandle_GetReturnsExpected(Image value)
        {
            var pictureBox = new PictureBox
            {
                ErrorImage = new Bitmap(10, 10)
            };
            Assert.NotEqual(IntPtr.Zero, pictureBox.Handle);

            pictureBox.ErrorImage = value;
            Assert.Same(value, pictureBox.ErrorImage);
            Assert.Null(pictureBox.Image);

            // Set same.
            pictureBox.ErrorImage = value;
            Assert.Same(value, pictureBox.ErrorImage);
            Assert.Null(pictureBox.Image);
        }

        [Fact]
        public void PictureBox_ErrorImage_ResetValue_Success()
        {
            var image = new Bitmap(10, 10);
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(PictureBox))[nameof(PictureBox.ErrorImage)];
            var pictureBox = new PictureBox();
            Assert.False(property.CanResetValue(pictureBox));

            pictureBox.ErrorImage = image;
            Assert.True(property.CanResetValue(pictureBox));

            property.ResetValue(pictureBox);
            Assert.NotSame(image, pictureBox.ErrorImage);
            Assert.NotNull(image);
            Assert.False(property.CanResetValue(pictureBox));
        }

        [Fact]
        public void PictureBox_ErrorImage_ShouldSerializeValue_Success()
        {
            var image = new Bitmap(10, 10);
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(PictureBox))[nameof(PictureBox.ErrorImage)];
            var pictureBox = new PictureBox();
            Assert.False(property.ShouldSerializeValue(pictureBox));

            pictureBox.ErrorImage = image;
            Assert.True(property.ShouldSerializeValue(pictureBox));

            property.ResetValue(pictureBox);
            Assert.NotSame(image, pictureBox.ErrorImage);
            Assert.NotNull(image);
            Assert.False(property.ShouldSerializeValue(pictureBox));
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetForeColorTheoryData))]
        public void PictureBox_ForeColor_Set_GetReturnsExpected(Color value, Color expected)
        {
            var control = new PictureBox
            {
                ForeColor = value
            };
            Assert.Equal(expected, control.ForeColor);

            // Set same.
            control.ForeColor = value;
            Assert.Equal(expected, control.ForeColor);
        }

        [Fact]
        public void PictureBox_ForeColor_SetWithHandler_CallsForeColorChanged()
        {
            var control = new PictureBox();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.ForeColorChanged += handler;

            // Set different.
            control.ForeColor = Color.Red;
            Assert.Equal(Color.Red, control.ForeColor);
            Assert.Equal(1, callCount);

            // Set same.
            control.ForeColor = Color.Red;
            Assert.Equal(Color.Red, control.ForeColor);
            Assert.Equal(1, callCount);

            // Set different.
            control.ForeColor = Color.Empty;
            Assert.Equal(Control.DefaultForeColor, control.ForeColor);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.ForeColorChanged -= handler;
            control.ForeColor = Color.Red;
            Assert.Equal(Color.Red, control.ForeColor);
            Assert.Equal(2, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetFontTheoryData))]
        public void PictureBox_Font_Set_GetReturnsExpected(Font value)
        {
            var control = new PictureBox
            {
                Font = value
            };
            Assert.Equal(value ?? Control.DefaultFont, control.Font);

            // Set same.
            control.Font = value;
            Assert.Equal(value ?? Control.DefaultFont, control.Font);
        }

        [Fact]
        public void PictureBox_Font_SetWithHandler_CallsFontChanged()
        {
            var control = new PictureBox();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.FontChanged += handler;

            // Set different.
            Font font1 = new Font("Arial", 8.25f);
            control.Font = font1;
            Assert.Same(font1, control.Font);
            Assert.Equal(1, callCount);

            // Set same.
            control.Font = font1;
            Assert.Same(font1, control.Font);
            Assert.Equal(1, callCount);

            // Set different.
            Font font2 = SystemFonts.DialogFont;
            control.Font = font2;
            Assert.Same(font2, control.Font);
            Assert.Equal(2, callCount);

            // Set null.
            control.Font = null;
            Assert.Same(Control.DefaultFont, control.Font);
            Assert.Equal(3, callCount);

            // Remove handler.
            control.FontChanged -= handler;
            control.Font = font1;
            Assert.Same(font1, control.Font);
            Assert.Equal(3, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetImageTheoryData))]
        public void PictureBox_Image_Set_GetReturnsExpected(Image value)
        {
            var pictureBox = new PictureBox
            {
                Image = value
            };
            Assert.Same(value, pictureBox.Image);

            // Set same.
            pictureBox.Image = value;
            Assert.Same(value, pictureBox.Image);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetImageTheoryData))]
        public void PictureBox_Image_SetWithNonNullOldValue_GetReturnsExpected(Image value)
        {
            var pictureBox = new PictureBox
            {
                Image = new Bitmap(10, 10)
            };

            pictureBox.Image = value;
            Assert.Same(value, pictureBox.Image);

            // Set same.
            pictureBox.Image = value;
            Assert.Same(value, pictureBox.Image);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetImageTheoryData))]
        public void PictureBox_Image_SetWithHandle_GetReturnsExpected(Image value)
        {
            var pictureBox = new PictureBox();
            Assert.NotEqual(IntPtr.Zero, pictureBox.Handle);

            pictureBox.Image = value;
            Assert.Same(value, pictureBox.Image);

            // Set same.
            pictureBox.Image = value;
            Assert.Same(value, pictureBox.Image);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetImageTheoryData))]
        public void PictureBox_Image_SetWithNonNullOldValueWithHandle_GetReturnsExpected(Image value)
        {
            var pictureBox = new PictureBox
            {
                Image = new Bitmap(10, 10)
            };
            Assert.NotEqual(IntPtr.Zero, pictureBox.Handle);

            pictureBox.Image = value;
            Assert.Same(value, pictureBox.Image);

            // Set same.
            pictureBox.Image = value;
            Assert.Same(value, pictureBox.Image);
        }

        [Fact]
        public void PictureBox_Image_ResetValue_Success()
        {
            var image = new Bitmap(10, 10);
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(PictureBox))[nameof(PictureBox.Image)];
            var pictureBox = new PictureBox();
            Assert.False(property.CanResetValue(pictureBox));

            pictureBox.Image = image;
            Assert.True(property.CanResetValue(pictureBox));

            property.ResetValue(pictureBox);
            Assert.NotSame(image, pictureBox.Image);
            Assert.NotNull(image);
            Assert.False(property.CanResetValue(pictureBox));
        }

        [Fact]
        public void PictureBox_Image_ShouldSerializeValue_Success()
        {
            var image = new Bitmap(10, 10);
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(PictureBox))[nameof(PictureBox.Image)];
            var pictureBox = new PictureBox();
            Assert.False(property.ShouldSerializeValue(pictureBox));

            pictureBox.Image = image;
            Assert.True(property.ShouldSerializeValue(pictureBox));

            property.ResetValue(pictureBox);
            Assert.NotSame(image, pictureBox.Image);
            Assert.NotNull(image);
            Assert.False(property.ShouldSerializeValue(pictureBox));
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void PictureBox_ImageLocation_Set_GetReturnsExpected(string value)
        {
            var pictureBox = new PictureBox
            {
                ImageLocation = value
            };
            Assert.Equal(value, pictureBox.ImageLocation);
            Assert.Null(pictureBox.Image);

            // Set same.
            pictureBox.ImageLocation = value;
            Assert.Equal(value, pictureBox.ImageLocation);
            Assert.Null(pictureBox.Image);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        [InlineData("bitmaps/nature24bits.jpg")]
        [InlineData("  ")]
        public void PictureBox_ImageLocation_SetWithImage_GetReturnsExpected(string value)
        {
            var image = new Bitmap(10, 10);
            var pictureBox = new PictureBox
            {
                Image = image,
                ImageLocation = value
            };
            Assert.Equal(value, pictureBox.ImageLocation);
            Assert.Same(image, pictureBox.Image);

            // Set same.
            pictureBox.ImageLocation = value;
            Assert.Equal(value, pictureBox.ImageLocation);
            Assert.Same(image, pictureBox.Image);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        public void PictureBox_ImageLocation_SetWithHandle_GetReturnsExpected(string value)
        {
            var pictureBox = new PictureBox();
            Assert.NotEqual(IntPtr.Zero, pictureBox.Handle);

            pictureBox.ImageLocation = value;
            Assert.Equal(value, pictureBox.ImageLocation);
            Assert.Null(pictureBox.Image);

            // Set same.
            pictureBox.ImageLocation = value;
            Assert.Equal(value, pictureBox.ImageLocation);
            Assert.Null(pictureBox.Image);
        }

        [Fact]
        public void PictureBox_ImageLocation_Set_CallsInvalidated()
        {
            var pictureBox = new PictureBox
            {
                ImageLocation = null
            };
            int invalidatedCallCount = 0;
            pictureBox.Invalidated += (sender, e) => invalidatedCallCount++;
            Assert.NotEqual(IntPtr.Zero, pictureBox.Handle);

            // Set different.
            pictureBox.ImageLocation = "Location";
            Assert.Equal("Location", pictureBox.ImageLocation);
            Assert.Equal(1, invalidatedCallCount);

            // Set same.
            pictureBox.ImageLocation = "Location";
            Assert.Equal("Location", pictureBox.ImageLocation);
            Assert.Equal(2, invalidatedCallCount);

            // Set null.
            pictureBox.ImageLocation = null;
            Assert.Null(pictureBox.ImageLocation);
            Assert.Equal(3, invalidatedCallCount);

            // Set empty.
            pictureBox.ImageLocation = string.Empty;
            Assert.Same(string.Empty, pictureBox.ImageLocation);
            Assert.Equal(4, invalidatedCallCount);
        }

        [Fact]
        public void PictureBox_ImageLocation_SetValidWithWaitOnLoadTrueLocal_GetReturnsExpected()
        {
            var pictureBox = new PictureBox
            {
                WaitOnLoad = true
            };

            pictureBox.ImageLocation = PathImageLocation;
            Assert.Equal(new Size(110, 100), pictureBox.Image.Size);
            Assert.Equal(PathImageLocation, pictureBox.ImageLocation);

            // Set same.
            pictureBox.ImageLocation = PathImageLocation;
            Assert.Equal(new Size(110, 100), pictureBox.Image.Size);
            Assert.Equal(PathImageLocation, pictureBox.ImageLocation);
        }

        [Fact]
        public void PictureBox_ImageLocation_SetValidWithWaitOnLoadTrueUri_GetReturnsExpected()
        {
            try
            {
                var pictureBox = new PictureBox
                {
                    WaitOnLoad = true
                };

                pictureBox.Load(UrlImageLocation);
                Assert.Equal(new Size(110, 100), pictureBox.Image.Size);
                Assert.Same(UrlImageLocation, pictureBox.ImageLocation);

                // Set same.
                pictureBox.Load(UrlImageLocation);
                Assert.Equal(new Size(110, 100), pictureBox.Image.Size);
                Assert.Same(UrlImageLocation, pictureBox.ImageLocation);
            }
            catch
            {
                // Swallow network errors.
            }
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetNullOrEmptyStringTheoryData))]
        public void PictureBox_ImageLocation_SetNullOrEmptyWithWaitOnLoadTrue_GetReturnsExpected(string value)
        {
            var pictureBox = new PictureBox
            {
                WaitOnLoad = true
            };

            pictureBox.ImageLocation = value;
            Assert.Null(pictureBox.Image);
            Assert.Equal(value, pictureBox.ImageLocation);

            // Set same.
            pictureBox.ImageLocation = value;
            Assert.Null(pictureBox.Image);
            Assert.Equal(value, pictureBox.ImageLocation);
        }

        [Theory]
        [InlineData("NoSuchValue")]
        [InlineData("  ")]
        public void PictureBox_ImageLocation_SetInvalidWithWaitOnLoadTrue_ThrowsFileNotFoundException(string value)
        {
            var pictureBox = new PictureBox
            {
                WaitOnLoad = true
            };

            Assert.ThrowsAny<Exception>(() => pictureBox.ImageLocation = value);
            Assert.Null(pictureBox.Image);
            Assert.Equal(value, pictureBox.ImageLocation);

            // Set same.
            Assert.ThrowsAny<Exception>(() => pictureBox.ImageLocation = value);
            Assert.Null(pictureBox.Image);
            Assert.Equal(value, pictureBox.ImageLocation);
        }

        public static IEnumerable<object[]> ImageLocation_SetInvalidWithWaitOnLoadTrueDesignMode_TestData()
        {
            Image image1 = new Bitmap(10, 10);
            Image image2 = new Bitmap(10, 10);
            
            foreach (string value in new string[] { " ", "NoSuchImage" })
            {
                yield return new object[] { null, null, value };
                yield return new object[] { image1, image2, value };
                yield return new object[] { image1, null, value };
                yield return new object[] { null, image2, value };
            }
        }

        [Theory]
        [MemberData(nameof(ImageLocation_SetInvalidWithWaitOnLoadTrueDesignMode_TestData))]
        public void PictureBox_ImageLocation_SetInvalidWithWaitOnLoadTrueDesignMode_GetReturnsExpected(Image initialImage, Image errorImage, string value)
        {
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(new AmbientProperties());
            mockSite
                .Setup(s => s.DesignMode)
                .Returns(true);
            var pictureBox = new PictureBox
            {
                InitialImage = initialImage,
                ErrorImage = errorImage,
                WaitOnLoad = true,
                Site = mockSite.Object
            };

            pictureBox.ImageLocation = value;
            Assert.Same(pictureBox.ErrorImage, pictureBox.Image);
            Assert.Equal(value, pictureBox.ImageLocation);

            // Set same.
            pictureBox.ImageLocation = value;
            Assert.Same(pictureBox.ErrorImage, pictureBox.Image);
            Assert.Equal(value, pictureBox.ImageLocation);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetNullOrEmptyStringTheoryData))]
        public void PictureBox_ImageLocation_SetNullOrEmptyWithError_ResetsImage(string value)
        {
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(new AmbientProperties());
            mockSite
                .Setup(s => s.DesignMode)
                .Returns(true);
            var pictureBox = new PictureBox
            {
                WaitOnLoad = true,
                Site = mockSite.Object
            };
            pictureBox.ImageLocation = "Invalid";
            Assert.Equal("Invalid", pictureBox.ImageLocation);
            Assert.Same(pictureBox.ErrorImage, pictureBox.Image);

            pictureBox.ImageLocation = value;
            Assert.Equal(value, pictureBox.ImageLocation);
            Assert.Null(pictureBox.Image);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        [InlineData("  ")]
        [InlineData("bitmaps/nature24bits.jpg")]
        public void PictureBox_ImageLocation_SetInitializing_GetReturnsExpected(string value)
        {
            var pictureBox = new PictureBox
            {
                WaitOnLoad = true
            };
            ((ISupportInitialize)pictureBox).BeginInit();

            pictureBox.ImageLocation = value;
            Assert.Null(pictureBox.Image);
            Assert.Equal(value, pictureBox.ImageLocation);

            // Set same.
            pictureBox.ImageLocation = value;
            Assert.Null(pictureBox.Image);
            Assert.Equal(value, pictureBox.ImageLocation);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryData), typeof(ImageLayout))]
        public void PictureBox_ImeMode_Set_GetReturnsExpected(ImeMode value)
        {
            var control = new PictureBox
            {
                ImeMode = value
            };
            Assert.Equal(value, control.ImeMode);

            // Set same.
            control.ImeMode = value;
            Assert.Equal(value, control.ImeMode);
        }

        [Fact]
        public void PictureBox_ImeMode_SetWithHandler_DoesNotCallImeModeChanged()
        {
            var control = new PictureBox();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.ImeModeChanged += handler;

            // Set different.
            control.ImeMode = ImeMode.On;
            Assert.Equal(ImeMode.On, control.ImeMode);
            Assert.Equal(0, callCount);

            // Set same.
            control.ImeMode = ImeMode.On;
            Assert.Equal(ImeMode.On, control.ImeMode);
            Assert.Equal(0, callCount);

            // Set different.
            control.ImeMode = ImeMode.Off;
            Assert.Equal(ImeMode.Off, control.ImeMode);
            Assert.Equal(0, callCount);

            // Remove handler.
            control.ImeModeChanged -= handler;
            control.ImeMode = ImeMode.Off;
            Assert.Equal(ImeMode.Off, control.ImeMode);
            Assert.Equal(0, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(ImeMode))]
        public void PictureBox_ImeMode_SetInvalid_ThrowsInvalidEnumArgumentException(ImeMode value)
        {
            var control = new PictureBox();
            Assert.Throws<InvalidEnumArgumentException>("value", () => control.ImeMode = value);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetImageTheoryData))]
        public void PictureBox_InitialImage_Set_GetReturnsExpected(Image value)
        {
            var pictureBox = new PictureBox
            {
                InitialImage = value
            };
            Assert.Same(value, pictureBox.InitialImage);
            Assert.Null(pictureBox.Image);

            // Set same.
            pictureBox.InitialImage = value;
            Assert.Same(value, pictureBox.InitialImage);
            Assert.Null(pictureBox.Image);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetImageTheoryData))]
        public void PictureBox_InitialImage_SetWithNonNullOldValue_GetReturnsExpected(Image value)
        {
            var pictureBox = new PictureBox
            {
                InitialImage = new Bitmap(10, 10)
            };

            pictureBox.InitialImage = value;
            Assert.Same(value, pictureBox.InitialImage);
            Assert.Null(pictureBox.Image);

            // Set same.
            pictureBox.InitialImage = value;
            Assert.Same(value, pictureBox.InitialImage);
            Assert.Null(pictureBox.Image);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetImageTheoryData))]
        public void PictureBox_InitialImage_SetWithHandle_GetReturnsExpected(Image value)
        {
            var pictureBox = new PictureBox();
            Assert.NotEqual(IntPtr.Zero, pictureBox.Handle);

            pictureBox.InitialImage = value;
            Assert.Same(value, pictureBox.InitialImage);
            Assert.Null(pictureBox.Image);

            // Set same.
            pictureBox.InitialImage = value;
            Assert.Same(value, pictureBox.InitialImage);
            Assert.Null(pictureBox.Image);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetImageTheoryData))]
        public void PictureBox_InitialImage_SetWithNonNullOldValueWithHandle_GetReturnsExpected(Image value)
        {
            var pictureBox = new PictureBox
            {
                InitialImage = new Bitmap(10, 10)
            };
            Assert.NotEqual(IntPtr.Zero, pictureBox.Handle);

            pictureBox.InitialImage = value;
            Assert.Same(value, pictureBox.InitialImage);
            Assert.Null(pictureBox.Image);

            // Set same.
            pictureBox.InitialImage = value;
            Assert.Same(value, pictureBox.InitialImage);
            Assert.Null(pictureBox.Image);
        }

        [Fact]
        public void PictureBox_InitialImage_ResetValue_Success()
        {
            var image = new Bitmap(10, 10);
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(PictureBox))[nameof(PictureBox.InitialImage)];
            var pictureBox = new PictureBox();
            Assert.False(property.CanResetValue(pictureBox));

            pictureBox.InitialImage = image;
            Assert.True(property.CanResetValue(pictureBox));

            property.ResetValue(pictureBox);
            Assert.NotSame(image, pictureBox.InitialImage);
            Assert.NotNull(image);
            Assert.False(property.CanResetValue(pictureBox));
        }

        [Fact]
        public void PictureBox_InitialImage_ShouldSerializeValue_Success()
        {
            var image = new Bitmap(10, 10);
            PropertyDescriptor property = TypeDescriptor.GetProperties(typeof(PictureBox))[nameof(PictureBox.InitialImage)];
            var pictureBox = new PictureBox();
            Assert.False(property.ShouldSerializeValue(pictureBox));

            pictureBox.InitialImage = image;
            Assert.True(property.ShouldSerializeValue(pictureBox));

            property.ResetValue(pictureBox);
            Assert.NotSame(image, pictureBox.InitialImage);
            Assert.NotNull(image);
            Assert.False(property.ShouldSerializeValue(pictureBox));
        }

        public static IEnumerable<object[]> Parent_Set_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new Control() };
        }

        [Theory]
        [MemberData(nameof(Parent_Set_TestData))]
        public void PictureBox_Parent_Set_GetReturnsExpected(Control value)
        {
            var control = new PictureBox
            {
                Parent = value
            };
            Assert.Same(value, control.Parent);

            // Set same.
            control.Parent = value;
            Assert.Same(value, control.Parent);
        }

        [Theory]
        [MemberData(nameof(Parent_Set_TestData))]
        public void PictureBox_Parent_SetWithNonNullOldParent_GetReturnsExpected(Control value)
        {
            var oldParent = new Control();
            var control = new PictureBox
            {
                Parent = oldParent
            };
            
            control.Parent = value;
            Assert.Same(value, control.Parent);
            Assert.Empty(oldParent.Controls);

            // Set same.
            control.Parent = value;
            Assert.Same(value, control.Parent);
            Assert.Empty(oldParent.Controls);
        }

        [Fact]
        public void PictureBox_Parent_SetNonNull_AddsToControls()
        {
            var parent = new Control();
            var control = new PictureBox
            {
                Parent = parent
            };
            Assert.Same(parent, control.Parent);
            Assert.Same(control, Assert.Single(parent.Controls));

            // Set same.
            control.Parent = parent;
            Assert.Same(parent, control.Parent);
            Assert.Same(control, Assert.Single(parent.Controls));
        }

        [Fact]
        public void PictureBox_Parent_SetWithHandler_CallsParentChanged()
        {
            var parent = new Control();
            var control = new PictureBox();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.ParentChanged += handler;

            // Set different.
            control.Parent = parent;
            Assert.Same(parent, control.Parent);
            Assert.Equal(1, callCount);

            // Set same.
            control.Parent = parent;
            Assert.Same(parent, control.Parent);
            Assert.Equal(1, callCount);

            // Set null.
            control.Parent = null;
            Assert.Null(control.Parent);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.ParentChanged -= handler;
            control.Parent = parent;
            Assert.Same(parent, control.Parent);
            Assert.Equal(2, callCount);
        }

        [Fact]
        public void PictureBox_Parent_SetSame_ThrowsArgumentException()
        {
            var control = new PictureBox();
            Assert.Throws<ArgumentException>(null, () => control.Parent = control);
            Assert.Null(control.Parent);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetRightToLeftTheoryData))]
        public void PictureBox_RightToLeft_Set_GetReturnsExpected(RightToLeft value, RightToLeft expected)
        {
            var control = new PictureBox
            {
                RightToLeft = value
            };
            Assert.Equal(expected, control.RightToLeft);

            // Set same.
            control.RightToLeft = value;
            Assert.Equal(expected, control.RightToLeft);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetRightToLeftTheoryData))]
        public void PictureBox_RightToLeft_SetAsParent_GetReturnsExpected(RightToLeft value, RightToLeft expected)
        {
            var parent = new Control
            {
                RightToLeft = value
            };
            var control = new PictureBox
            {
                Parent = parent
            };
            Assert.Equal(expected, control.RightToLeft);

            // Set same.
            control.RightToLeft = value;
            Assert.Equal(expected, control.RightToLeft);
        }

        [Fact]
        public void PictureBox_RightToLeft_SetWithHandler_CallsRightToLeftChanged()
        {
            var control = new PictureBox();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.RightToLeftChanged += handler;

            // Set different.
            control.RightToLeft = RightToLeft.Yes;
            Assert.Equal(RightToLeft.Yes, control.RightToLeft);
            Assert.Equal(1, callCount);

            // Set same.
            control.RightToLeft = RightToLeft.Yes;
            Assert.Equal(RightToLeft.Yes, control.RightToLeft);
            Assert.Equal(1, callCount);

            // Set different.
            control.RightToLeft = RightToLeft.Inherit;
            Assert.Equal(RightToLeft.No, control.RightToLeft);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.RightToLeftChanged -= handler;
            control.RightToLeft = RightToLeft.Yes;
            Assert.Equal(RightToLeft.Yes, control.RightToLeft);
            Assert.Equal(2, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(RightToLeft))]
        public void PictureBox_RightToLeft_SetInvalid_ThrowsInvalidEnumArgumentException(RightToLeft value)
        {
            var control = new PictureBox();
            Assert.Throws<InvalidEnumArgumentException>("value", () => control.RightToLeft = value);
        }

        public static IEnumerable<object[]> SizeMode_Set_TestData()
        {
            yield return new object[] { PictureBoxSizeMode.Normal, false };
            yield return new object[] { PictureBoxSizeMode.StretchImage, false };
            yield return new object[] { PictureBoxSizeMode.AutoSize, true };
            yield return new object[] { PictureBoxSizeMode.CenterImage, false };
            yield return new object[] { PictureBoxSizeMode.Zoom, false };
        }

        [Theory]
        [MemberData(nameof(SizeMode_Set_TestData))]
        public void PictureBox_SizeMode_Set_GetReturnsExpected(PictureBoxSizeMode value, bool expectedAutoSize)
        {
            var pictureBox = new SubPictureBox
            {
                SizeMode = value
            };
            Assert.Equal(value, pictureBox.SizeMode);
            Assert.Equal(expectedAutoSize, pictureBox.AutoSize);
            Assert.Equal(expectedAutoSize, pictureBox.GetStyle(ControlStyles.FixedHeight | ControlStyles.FixedWidth));

            // Set same.
            pictureBox.SizeMode = value;
            Assert.Equal(value, pictureBox.SizeMode);
            Assert.Equal(expectedAutoSize, pictureBox.AutoSize);
            Assert.Equal(expectedAutoSize, pictureBox.GetStyle(ControlStyles.FixedHeight | ControlStyles.FixedWidth));
        }

        [Theory]
        [MemberData(nameof(SizeMode_Set_TestData))]
        public void PictureBox_SizeMode_SetWithHandle_GetReturnsExpected(PictureBoxSizeMode value, bool expectedAutoSize)
        {
            var pictureBox = new SubPictureBox();
            Assert.NotEqual(IntPtr.Zero, pictureBox.Handle);

            pictureBox.SizeMode = value;
            Assert.Equal(value, pictureBox.SizeMode);
            Assert.Equal(expectedAutoSize, pictureBox.AutoSize);
            Assert.Equal(expectedAutoSize, pictureBox.GetStyle(ControlStyles.FixedHeight | ControlStyles.FixedWidth));

            // Set same.
            pictureBox.SizeMode = value;
            Assert.Equal(value, pictureBox.SizeMode);
            Assert.Equal(expectedAutoSize, pictureBox.AutoSize);
            Assert.Equal(expectedAutoSize, pictureBox.GetStyle(ControlStyles.FixedHeight | ControlStyles.FixedWidth));
        }

        [Fact]
        public void PictureBox_SizeMode_Set_CallsInvalidated()
        {
            var pictureBox = new PictureBox
            {
                SizeMode = PictureBoxSizeMode.Normal
            };
            int invalidatedCallCount = 0;
            pictureBox.Invalidated += (sender, e) => invalidatedCallCount++;
            Assert.NotEqual(IntPtr.Zero, pictureBox.Handle);

            // Set different.
            pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            Assert.Equal(PictureBoxSizeMode.StretchImage, pictureBox.SizeMode);
            Assert.Equal(1, invalidatedCallCount);

            // Set same.
            pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            Assert.Equal(PictureBoxSizeMode.StretchImage, pictureBox.SizeMode);
            Assert.Equal(1, invalidatedCallCount);
        }

        [Fact]
        public void PictureBox_SizeMode_SetWithHandler_CallsSizeModeChanged()
        {
            var pictureBox = new PictureBox
            {
                SizeMode = PictureBoxSizeMode.Normal
            };
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(pictureBox, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            pictureBox.SizeModeChanged += handler;

            // Set different.
            pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            Assert.Equal(PictureBoxSizeMode.StretchImage, pictureBox.SizeMode);
            Assert.Equal(1, callCount);

            // Set same.
            pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            Assert.Equal(PictureBoxSizeMode.StretchImage, pictureBox.SizeMode);
            Assert.Equal(1, callCount);

            // Set different.
            pictureBox.SizeMode = PictureBoxSizeMode.Normal;
            Assert.Equal(PictureBoxSizeMode.Normal, pictureBox.SizeMode);
            Assert.Equal(2, callCount);

            // Remove handler.
            pictureBox.SizeModeChanged -= handler;
            pictureBox.SizeMode = PictureBoxSizeMode.StretchImage;
            Assert.Equal(PictureBoxSizeMode.StretchImage, pictureBox.SizeMode);
            Assert.Equal(2, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEnumTypeTheoryDataInvalid), typeof(PictureBoxSizeMode))]
        public void PictureBox_SizeMode_SetInvalid_ThrowsInvalidEnumArgumentException(PictureBoxSizeMode value)
        {
            var pictureBox = new PictureBox();
            Assert.Throws<InvalidEnumArgumentException>("value", () => pictureBox.SizeMode = value);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(1)]
        [InlineData(2)]
        public void PictureBox_TabIndex_Set_GetReturnsExpected(int value)
        {
            var control = new PictureBox
            {
                TabIndex = value
            };
            Assert.Equal(value, control.TabIndex);

            // Set same.
            control.TabIndex = value;
            Assert.Equal(value, control.TabIndex);
        }

        [Fact]
        public void PictureBox_TabIndex_SetWithHandler_CallsTabIndexChanged()
        {
            var control = new PictureBox
            {
                TabIndex = 0
            };
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.TabIndexChanged += handler;

            // Set different.
            control.TabIndex = 1;
            Assert.Equal(1, control.TabIndex);
            Assert.Equal(1, callCount);

            // Set same.
            control.TabIndex = 1;
            Assert.Equal(1, control.TabIndex);
            Assert.Equal(1, callCount);

            // Set different.
            control.TabIndex = 2;
            Assert.Equal(2, control.TabIndex);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.TabIndexChanged -= handler;
            control.TabIndex = 1;
            Assert.Equal(1, control.TabIndex);
            Assert.Equal(2, callCount);
        }

        [Fact]
        public void PictureBox_TabIndex_SetNegative_CallsArgumentOutOfRangeException()
        {
            var control = new PictureBox();
            Assert.Throws<ArgumentOutOfRangeException>("value", () => control.TabIndex = -1);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void PictureBox_TabStop_Set_GetReturnsExpected(bool value)
        {
            var control = new PictureBox
            {
                TabStop = value
            };
            Assert.Equal(value, control.TabStop);

            // Set same.
            control.TabStop = value;
            Assert.Equal(value, control.TabStop);

            // Set different.
            control.TabStop = value;
            Assert.Equal(value, control.TabStop);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void PictureBox_TabStop_SetWithHandle_GetReturnsExpected(bool value)
        {
            var control = new PictureBox();
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            control.TabStop = value;
            Assert.Equal(value, control.TabStop);

            // Set same.
            control.TabStop = value;
            Assert.Equal(value, control.TabStop);

            // Set different.
            control.TabStop = value;
            Assert.Equal(value, control.TabStop);
        }

        [Fact]
        public void PictureBox_TabStop_SetWithHandler_CallsTabStopChanged()
        {
            var control = new PictureBox
            {
                TabStop = true
            };
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.TabStopChanged += handler;

            // Set different.
            control.TabStop = false;
            Assert.False(control.TabStop);
            Assert.Equal(1, callCount);

            // Set same.
            control.TabStop = false;
            Assert.False(control.TabStop);
            Assert.Equal(1, callCount);

            // Set different.
            control.TabStop = true;
            Assert.True(control.TabStop);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.TabStopChanged -= handler;
            control.TabStop = false;
            Assert.False(control.TabStop);
            Assert.Equal(2, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void PictureBox_Text_Set_GetReturnsExpected(string value, string expected)
        {
            var control = new PictureBox
            {
                Text = value
            };
            Assert.Equal(expected, control.Text);

            // Set same.
            control.Text = value;
            Assert.Equal(expected, control.Text);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringNormalizedTheoryData))]
        public void PictureBox_Text_SetWithHandle_GetReturnsExpected(string value, string expected)
        {
            var control = new PictureBox();
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            control.Text = value;
            Assert.Equal(expected, control.Text);

            // Set same.
            control.Text = value;
            Assert.Equal(expected, control.Text);
        }

        [Fact]
        public void PictureBox_Text_SetWithHandler_CallsTextChanged()
        {
            var control = new PictureBox();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Equal(EventArgs.Empty, e);
                callCount++;
            };
            control.TextChanged += handler;

            // Set different.
            control.Text = "text";
            Assert.Same("text", control.Text);
            Assert.Equal(1, callCount);

            // Set same.
            control.Text = "text";
            Assert.Same("text", control.Text);
            Assert.Equal(1, callCount);

            // Set different.
            control.Text = null;
            Assert.Empty(control.Text);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.TextChanged -= handler;
            control.Text = "text";
            Assert.Same("text", control.Text);
            Assert.Equal(2, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void PictureBox_Visible_Set_GetReturnsExpected(bool value)
        {
            var control = new Control
            {
                Visible = value
            };
            Assert.Equal(value, control.Visible);

            // Set same.
            control.Visible = value;
            Assert.Equal(value, control.Visible);

            // Set different.
            control.Visible = value;
            Assert.Equal(value, control.Visible);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void PictureBox_Visible_SetWithHandle_GetReturnsExpected(bool value)
        {
            var control = new Control();
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            control.Visible = value;
            Assert.Equal(value, control.Visible);

            // Set same.
            control.Visible = value;
            Assert.Equal(value, control.Visible);

            // Set different.
            control.Visible = value;
            Assert.Equal(value, control.Visible);
        }

        [Fact]
        public void PictureBox_Visible_SetWithHandler_CallsVisibleChanged()
        {
            var control = new Control
            {
                Visible = true
            };
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            control.VisibleChanged += handler;

            // Set different.
            control.Visible = false;
            Assert.False(control.Visible);
            Assert.Equal(1, callCount);

            // Set same.
            control.Visible = false;
            Assert.False(control.Visible);
            Assert.Equal(1, callCount);

            // Set different.
            control.Visible = true;
            Assert.True(control.Visible);
            Assert.Equal(2, callCount);

            // Remove handler.
            control.VisibleChanged -= handler;
            control.Visible = false;
            Assert.False(control.Visible);
            Assert.Equal(2, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetBoolTheoryData))]
        public void PictureBox_WaitOnLoad_Set_GetReturnsExpected(bool value)
        {
            var pictureBox = new PictureBox
            {
                WaitOnLoad = value
            };
            Assert.Equal(value, pictureBox.WaitOnLoad);

            // Set same
            pictureBox.WaitOnLoad = value;
            Assert.Equal(value, pictureBox.WaitOnLoad);

            // Set different
            pictureBox.WaitOnLoad = !value;
            Assert.Equal(!value, pictureBox.WaitOnLoad);
        }

        [Fact]
        public void PictureBox_BeginInit_InvokeMultipleTimes_Success()
        {
            var pictureBox = new PictureBox();
            ISupportInitialize supportInitialize = pictureBox;
            supportInitialize.BeginInit();
            supportInitialize.BeginInit();
        }

        [Fact]
        public void PictureBox_BeginInit_EndInitValidImageWaitOnLoadTrue_SetsImage()
        {
            var pictureBox = new PictureBox
            {
                WaitOnLoad = true
            };
            ISupportInitialize supportInitialize = pictureBox;
            supportInitialize.BeginInit();

            pictureBox.ImageLocation = PathImageLocation;
            Assert.Same(PathImageLocation, pictureBox.ImageLocation);
            Assert.Null(pictureBox.Image);

            supportInitialize.EndInit();
            Assert.Same(PathImageLocation, pictureBox.ImageLocation);
            Assert.Equal(new Size(110, 100), pictureBox.Image.Size);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetNullOrEmptyStringTheoryData))]
        public void PictureBox_BeginInit_EndInitNullOrEmptyImageWaitOnLoadTrue_DoesNotSetImage(string imageLocation)
        {
            var pictureBox = new PictureBox
            {
                WaitOnLoad = true
            };
            ISupportInitialize supportInitialize = pictureBox;
            supportInitialize.BeginInit();

            pictureBox.ImageLocation = imageLocation;
            Assert.Same(imageLocation, pictureBox.ImageLocation);
            Assert.Null(pictureBox.Image);

            supportInitialize.EndInit();
            Assert.Same(imageLocation, pictureBox.ImageLocation);
            Assert.Null(pictureBox.Image);
        }

        [Fact]
        public void PictureBox_BeginInit_EndInitWaitOnLoadFalse_DoesNotSetImage()
        {
            var pictureBox = new PictureBox
            {
                WaitOnLoad = false
            };
            ISupportInitialize supportInitialize = pictureBox;
            supportInitialize.BeginInit();

            pictureBox.ImageLocation = PathImageLocation;
            Assert.Same(PathImageLocation, pictureBox.ImageLocation);
            Assert.Null(pictureBox.Image);

            supportInitialize.EndInit();
            Assert.Same(PathImageLocation, pictureBox.ImageLocation);
            Assert.Null(pictureBox.Image);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        [InlineData("bitmaps/nature24bits.jpg")]
        [InlineData("  ")]
        public void PictureBox_EndInit_InvokeMultipleTimes_Nop(string imageLocation)
        {
            var pictureBox = new PictureBox
            {
                ImageLocation = imageLocation
            };
            Assert.Equal(imageLocation, pictureBox.ImageLocation);
            Assert.Null(pictureBox.Image);

            ISupportInitialize supportInitialize = pictureBox;
            supportInitialize.EndInit();
            Assert.Equal(imageLocation, pictureBox.ImageLocation);
            Assert.Null(pictureBox.Image);

            supportInitialize.EndInit();
            Assert.Equal(imageLocation, pictureBox.ImageLocation);
            Assert.Null(pictureBox.Image);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetStringWithNullTheoryData))]
        [InlineData("bitmaps/nature24bits.jpg")]
        [InlineData("  ")]
        public void PictureBox_EndInit_InvokeMultipleTimesWaitOnLoad_DoesNotLoadImage(string imageLocation)
        {
            var pictureBox = new PictureBox
            {
                ImageLocation = imageLocation 
            };
            Assert.Equal(imageLocation, pictureBox.ImageLocation);
            Assert.Null(pictureBox.Image);

            pictureBox.WaitOnLoad = true;
            Assert.Equal(imageLocation, pictureBox.ImageLocation);
            Assert.Null(pictureBox.Image);

            ISupportInitialize supportInitialize = pictureBox;
            supportInitialize.EndInit();
            Assert.Equal(imageLocation, pictureBox.ImageLocation);
            Assert.Null(pictureBox.Image);

            supportInitialize.EndInit();
            Assert.Equal(imageLocation, pictureBox.ImageLocation);
            Assert.Null(pictureBox.Image);
        }

        [Fact]
        public void PictureBox_CancelAsync_InvokeWithoutStarting_Nop()
        {
            var pictureBox = new PictureBox();
            pictureBox.CancelAsync();
            pictureBox.CancelAsync();
        }

        [Fact]
        public void PictureBox_Dispose_Success()
        {
            var pictureBox = new PictureBox();
            pictureBox.Dispose();
            pictureBox.Dispose();
        }

        [Theory]
        [InlineData("NoSuchImage")]
        [InlineData("  ")]
        public void PictureBox_Load_InvalidUrl_ThrowsException(string value)
        {
            var pictureBox = new PictureBox();

            Assert.ThrowsAny<Exception>(() => pictureBox.Load(value));
            Assert.Equal(value, pictureBox.ImageLocation);
            Assert.Null(pictureBox.Image);

            // Call again.
            Assert.ThrowsAny<Exception>(() => pictureBox.Load(value));
            Assert.Equal(value, pictureBox.ImageLocation);
            Assert.Null(pictureBox.Image);
        }

        [Theory]
        [InlineData("NoSuchImage")]
        [InlineData("  ")]
        public void PictureBox_Load_InvalidUrlWithImage_ThrowsException(string value)
        {
            var image = new Bitmap(10, 10);
            var pictureBox = new PictureBox
            {
                Image = image
            };

            Assert.ThrowsAny<Exception>(() => pictureBox.Load(value));
            Assert.Equal(value, pictureBox.ImageLocation);
            Assert.Same(image, pictureBox.Image);

            // Call again.
            Assert.ThrowsAny<Exception>(() => pictureBox.Load(value));
            Assert.Equal(value, pictureBox.ImageLocation);
            Assert.Same(image, pictureBox.Image);
        }

        [Fact]
        public void PictureBox_Load_Url_CallsInvalidated()
        {
            var pictureBox = new PictureBox();
            int invalidatedCallCount = 0;
            pictureBox.Invalidated += (sender, e) => invalidatedCallCount++;
            Assert.NotEqual(IntPtr.Zero, pictureBox.Handle);

            // Call different.
            Assert.ThrowsAny<Exception>(() => pictureBox.Load("Location"));
            Assert.Equal("Location", pictureBox.ImageLocation);
            Assert.Null(pictureBox.Image);
            Assert.Equal(1, invalidatedCallCount);

            // Call again.
            Assert.ThrowsAny<Exception>(() => pictureBox.Load("Location"));
            Assert.Equal("Location", pictureBox.ImageLocation);
            Assert.Null(pictureBox.Image);
            Assert.Equal(2, invalidatedCallCount);

            // Call null.
            Assert.Throws<InvalidOperationException>(() => pictureBox.Load(null));
            Assert.Null(pictureBox.ImageLocation);
            Assert.Null(pictureBox.Image);
            Assert.Equal(3, invalidatedCallCount);

            // Call empty.
            Assert.Throws<InvalidOperationException>(() => pictureBox.Load(string.Empty));
            Assert.Same(string.Empty, pictureBox.ImageLocation);
            Assert.Null(pictureBox.Image);
            Assert.Equal(4, invalidatedCallCount);

            // Call valid.
            pictureBox.Load(PathImageLocation);
            Assert.Equal(PathImageLocation, pictureBox.ImageLocation);
            Assert.Equal(new Size(110, 100), pictureBox.Image.Size);
            Assert.Equal(6, invalidatedCallCount);
        }

        [Fact]
        public void PictureBox_Load_UrlValidWithWaitOnLoadTrueLocal_GetReturnsExpected()
        {
            var pictureBox = new PictureBox
            {
                WaitOnLoad = true
            };

            pictureBox.Load(PathImageLocation);
            Assert.Same(PathImageLocation, pictureBox.ImageLocation);
            Assert.Equal(new Size(110, 100), pictureBox.Image.Size);

            // Call again.
            pictureBox.Load(PathImageLocation);
            Assert.Same(PathImageLocation, pictureBox.ImageLocation);
            Assert.Equal(new Size(110, 100), pictureBox.Image.Size);
        }

        [Fact]
        public void PictureBox_Load_UrlValidWithWaitOnLoadTrueUri_GetReturnsExpected()
        {
            try
            {
                var pictureBox = new PictureBox
                {
                    WaitOnLoad = true
                };

                pictureBox.ImageLocation = UrlImageLocation;
                Assert.Same(UrlImageLocation, pictureBox.ImageLocation);
                Assert.Equal(new Size(110, 100), pictureBox.Image.Size);

                // Call again.
                pictureBox.ImageLocation = UrlImageLocation;
                Assert.Same(UrlImageLocation, pictureBox.ImageLocation);
                Assert.Equal(new Size(110, 100), pictureBox.Image.Size);
            }
            catch
            {
                // Swallow network errors.
            }
        }

        [Theory]
        [MemberData(nameof(ImageLocation_SetInvalidWithWaitOnLoadTrueDesignMode_TestData))]
        public void PictureBox_Load_UrlInvalidWithWaitOnLoadTrueDesignMode_GetReturnsExpected(Image initialImage, Image errorImage, string value)
        {
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(new AmbientProperties());
            mockSite
                .Setup(s => s.DesignMode)
                .Returns(true);
            var pictureBox = new PictureBox
            {
                InitialImage = initialImage,
                ErrorImage = errorImage,
                WaitOnLoad = true,
                Site = mockSite.Object
            };

            pictureBox.Load(value);
            Assert.Same(pictureBox.ErrorImage, pictureBox.Image);
            Assert.Equal(value, pictureBox.ImageLocation);

            // Set same.
            pictureBox.Load(value);
            Assert.Same(pictureBox.ErrorImage, pictureBox.Image);
            Assert.Equal(value, pictureBox.ImageLocation);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetNullOrEmptyStringTheoryData))]
        public void PictureBox_Load_UrlNullOrEmptyWithError_ResetsImage(string value)
        {
            var mockSite = new Mock<ISite>(MockBehavior.Strict);
            mockSite
                .Setup(s => s.GetService(typeof(AmbientProperties)))
                .Returns(new AmbientProperties());
            mockSite
                .Setup(s => s.DesignMode)
                .Returns(true);
            var pictureBox = new PictureBox
            {
                WaitOnLoad = true,
                Site = mockSite.Object
            };
            pictureBox.ImageLocation = "Invalid";
            Assert.Equal("Invalid", pictureBox.ImageLocation);
            Assert.Same(pictureBox.ErrorImage, pictureBox.Image);

            pictureBox.ImageLocation = value;
            Assert.Equal(value, pictureBox.ImageLocation);
            Assert.Null(pictureBox.Image);
        }

        [Fact]
        public void PictureBox_Load_UrlInitializing_GetReturnsExpected()
        {
            var pictureBox = new PictureBox
            {
                WaitOnLoad = true
            };
            ((ISupportInitialize)pictureBox).BeginInit();

            pictureBox.Load(PathImageLocation);
            Assert.Equal(PathImageLocation, pictureBox.ImageLocation);
            Assert.Equal(new Size(110, 100), pictureBox.Image.Size);

            // Call again.
            pictureBox.Load(PathImageLocation);
            Assert.Equal(PathImageLocation, pictureBox.ImageLocation);
            Assert.Equal(new Size(110, 100), pictureBox.Image.Size);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetNullOrEmptyStringTheoryData))]
        public void PictureBox_Load_InvokeWithoutImageLocation_ThrowsInvalidOperationException(string imageLocation)
        {
            var pictureBox = new PictureBox
            {
                ImageLocation = imageLocation
            };
            Assert.Throws<InvalidOperationException>(() => pictureBox.Load());
            Assert.Throws<InvalidOperationException>(() => pictureBox.Load(imageLocation));
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetNullOrEmptyStringTheoryData))]
        public void PictureBox_LoadAsync_InvokeWithoutImageLocation_ThrowsInvalidOperationException(string imageLocation)
        {
            var pictureBox = new PictureBox
            {
                ImageLocation = imageLocation
            };
            Assert.Throws<InvalidOperationException>(() => pictureBox.LoadAsync());
            Assert.Throws<InvalidOperationException>(() => pictureBox.LoadAsync(imageLocation));
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void PictureBox_OnEnabledChanged_Invoke_CallsEnabledChanged(EventArgs eventArgs)
        {
            var control = new SubPictureBox();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.EnabledChanged += handler;
            control.OnEnabledChanged(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.EnabledChanged -= handler;
            control.OnEnabledChanged(eventArgs);
            Assert.Equal(1, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void PictureBox_OnEnter_Invoke_CallsEnter(EventArgs eventArgs)
        {
            var control = new SubPictureBox();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.Enter += handler;
            control.OnEnter(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.Enter -= handler;
            control.OnEnter(eventArgs);
            Assert.Equal(1, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void PictureBox_OnHandleCreated_Invoke_CallsHandleCreated(EventArgs eventArgs)
        {
            var control = new SubPictureBox();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.HandleCreated += handler;
            control.OnHandleCreated(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.HandleCreated -= handler;
            control.OnHandleCreated(eventArgs);
            Assert.Equal(1, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void PictureBox_OnHandleDestroyed_Invoke_CallsHandleDestroyed(EventArgs eventArgs)
        {
            var control = new SubPictureBox();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.HandleDestroyed += handler;
            control.OnHandleDestroyed(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.HandleDestroyed -= handler;
            control.OnHandleDestroyed(eventArgs);
            Assert.Equal(1, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetKeyEventArgsTheoryData))]
        public void PictureBox_OnKeyDown_Invoke_CallsKeyDown(KeyEventArgs eventArgs)
        {
            var control = new SubPictureBox();
            int callCount = 0;
            KeyEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.KeyDown += handler;
            control.OnKeyDown(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.KeyDown -= handler;
            control.OnKeyDown(eventArgs);
            Assert.Equal(1, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetKeyPressEventArgsTheoryData))]
        public void PictureBox_OnKeyPress_Invoke_CallsKeyPress(KeyPressEventArgs eventArgs)
        {
            var control = new SubPictureBox();
            int callCount = 0;
            KeyPressEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.KeyPress += handler;
            control.OnKeyPress(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.KeyPress -= handler;
            control.OnKeyPress(eventArgs);
            Assert.Equal(1, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetKeyEventArgsTheoryData))]
        public void PictureBox_OnKeyUp_Invoke_CallsKeyUp(KeyEventArgs eventArgs)
        {
            var control = new SubPictureBox();
            int callCount = 0;
            KeyEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.KeyUp += handler;
            control.OnKeyUp(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.KeyUp -= handler;
            control.OnKeyUp(eventArgs);
            Assert.Equal(1, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void PictureBox_OnLeave_Invoke_CallsLeave(EventArgs eventArgs)
        {
            var control = new SubPictureBox();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.Leave += handler;
            control.OnLeave(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.Leave -= handler;
            control.OnLeave(eventArgs);
            Assert.Equal(1, callCount);
        }

        public static IEnumerable<object[]> OnLoadCompleted_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new AsyncCompletedEventArgs(null, false, null) };
        }

        [Theory]
        [MemberData(nameof(OnLoadCompleted_TestData))]
        public void PictureBox_OnLoadCompleted_Invoke_CallsLoadCompleted(AsyncCompletedEventArgs eventArgs)
        {
            var control = new SubPictureBox();
            int callCount = 0;
            AsyncCompletedEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.LoadCompleted += handler;
            control.OnLoadCompleted(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.LoadCompleted -= handler;
            control.OnLoadCompleted(eventArgs);
            Assert.Equal(1, callCount);
        }

        public static IEnumerable<object[]> OnLoadProgressChanged_TestData()
        {
            yield return new object[] { null };
            yield return new object[] { new ProgressChangedEventArgs(0, null) };
        }

        [Theory]
        [MemberData(nameof(OnLoadProgressChanged_TestData))]
        public void PictureBox_OnLoadProgressChanged_Invoke_CallsLoadProgressChanged(ProgressChangedEventArgs eventArgs)
        {
            var control = new SubPictureBox();
            int callCount = 0;
            ProgressChangedEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.LoadProgressChanged += handler;
            control.OnLoadProgressChanged(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.LoadProgressChanged -= handler;
            control.OnLoadProgressChanged(eventArgs);
            Assert.Equal(1, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetPaintEventArgsTheoryData))]
        public void PictureBox_OnPaint_Invoke_CallsPaint(PaintEventArgs eventArgs)
        {
            var control = new SubPictureBox();
            int callCount = 0;
            PaintEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.Paint += handler;
            control.OnPaint(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.Paint -= handler;
            control.OnPaint(eventArgs);
            Assert.Equal(1, callCount);
        }

        public static IEnumerable<object[]> OnPaint_Image_TestData()
        {
            foreach (object[] testData in CommonTestHelper.GetPaintEventArgsTheoryData())
            {
                foreach (PictureBoxSizeMode sizeMode in Enum.GetValues(typeof(PictureBoxSizeMode)))
                {
                    yield return new object[] { sizeMode, testData[0] };
                }
            }
        }

        [Theory]
        [MemberData(nameof(OnPaint_Image_TestData))]
        public void PictureBox_OnPaint_InvokeWithValidImage_CallsLoad(PictureBoxSizeMode sizeMode, PaintEventArgs eventArgs)
        {
            var image = new Bitmap(10, 10);
            var pictureBox = new SubPictureBox
            {
                SizeMode = sizeMode,
                Image = image
            };
            Assert.Same(image, pictureBox.Image);
            
            pictureBox.OnPaint(eventArgs);
            Assert.Same(image, pictureBox.Image);
        }

        [Theory]
        [MemberData(nameof(OnPaint_Image_TestData))]
        public void PictureBox_OnPaint_InvokeWithValidImageLocationWaitOnLoadTrue_CallsLoad(PictureBoxSizeMode sizeMode, PaintEventArgs eventArgs)
        {
            var pictureBox = new SubPictureBox
            {
                SizeMode = sizeMode,
                ImageLocation = PathImageLocation
            };
            Assert.Null(pictureBox.Image);
            Assert.Equal(PathImageLocation, pictureBox.ImageLocation);
            
            pictureBox.WaitOnLoad = true;
            pictureBox.OnPaint(eventArgs);
            Assert.Equal(new Size(110, 100), pictureBox.Image.Size);
            Assert.Equal(PathImageLocation, pictureBox.ImageLocation);
        }

        public static IEnumerable<object[]> OnPaint_NullOrEmptyImageLocation_TestData()
        {
            foreach (object[] testData in CommonTestHelper.GetPaintEventArgsTheoryData())
            {
                yield return new object[] { string.Empty, testData[0] };
                yield return new object[] { null, testData[0] };
            }
        }

        [Theory]
        [MemberData(nameof(OnPaint_NullOrEmptyImageLocation_TestData))]
        public void PictureBox_OnPaint_InvokeWithNullOrEmptyImageLocationWaitOnLoadTrue_CallsLoad(string imageLocation, PaintEventArgs eventArgs)
        {
            var pictureBox = new SubPictureBox
            {
                ImageLocation = imageLocation
            };
            Assert.Same(imageLocation, pictureBox.ImageLocation);
            Assert.Null(pictureBox.Image);
            
            pictureBox.WaitOnLoad = true;
            pictureBox.OnPaint(eventArgs);
            Assert.Same(imageLocation, pictureBox.ImageLocation);
            Assert.Null(pictureBox.Image);
        }

        public static IEnumerable<object[]> OnPaint_InvalidImageLocation_TestData()
        {
            foreach (object[] testData in CommonTestHelper.GetPaintEventArgsTheoryData())
            {
                foreach (PictureBoxSizeMode sizeMode in Enum.GetValues(typeof(PictureBoxSizeMode)))
                {
                    yield return new object[] { sizeMode, "NoSuchImage", testData[0] };
                    yield return new object[] { sizeMode, "  ", testData[0] };
                }
            }
        }

        [Theory]
        [MemberData(nameof(OnPaint_InvalidImageLocation_TestData))]
        public void PictureBox_OnPaint_InvokeWithInvalidImageLocationWaitOnLoadTrue_CallsLoad(PictureBoxSizeMode sizeMode, string imageLocation, PaintEventArgs eventArgs)
        {
            var pictureBox = new SubPictureBox
            {
                SizeMode = sizeMode,
                ImageLocation = imageLocation
            };
            Assert.Same(imageLocation, pictureBox.ImageLocation);
            Assert.Null(pictureBox.Image);
            
            pictureBox.WaitOnLoad = true;
            pictureBox.OnPaint(eventArgs);
            Assert.Same(imageLocation, pictureBox.ImageLocation);
            Assert.Same(pictureBox.ErrorImage, pictureBox.Image);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetPaintEventArgsTheoryData))]
        public void PictureBox_OnPaint_InvokeWithImage_CallsPaint(PaintEventArgs eventArgs)
        {
            var control = new SubPictureBox
            {
                Image = new Bitmap(10, 10)
            };
            int callCount = 0;
            PaintEventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.Paint += handler;
            control.OnPaint(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.Paint -= handler;
            control.OnPaint(eventArgs);
            Assert.Equal(1, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void PictureBox_OnParentChanged_Invoke_CallsParentChanged(EventArgs eventArgs)
        {
            var control = new SubPictureBox();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.ParentChanged += handler;
            control.OnParentChanged(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.ParentChanged -= handler;
            control.OnParentChanged(eventArgs);
            Assert.Equal(1, callCount);
        }


        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void PictureBox_OnResize_Invoke_CallsResize(EventArgs eventArgs)
        {
            var control = new SubPictureBox();
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;

            // Call with handler.
            control.Resize += handler;
            control.OnResize(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, invalidatedCallCount);

            // Remove handler.
            control.Resize -= handler;
            control.OnResize(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(0, invalidatedCallCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void PictureBox_OnResize_InvokeWithResizeRedraw_CallsResizeAndInvalidate(EventArgs eventArgs)
        {
            var control = new SubPictureBox();
            control.SetStyle(ControlStyles.ResizeRedraw, true);
            Assert.NotEqual(IntPtr.Zero, control.Handle);
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };
            int invalidatedCallCount = 0;
            InvalidateEventHandler invalidatedHandler = (sender, e) => invalidatedCallCount++;

            // Call with handler.
            control.Resize += handler;
            control.Invalidated += invalidatedHandler;
            control.OnResize(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(1, invalidatedCallCount);

            // Remove handler.
            control.Resize -= handler;
            control.Invalidated -= invalidatedHandler;
            control.OnResize(eventArgs);
            Assert.Equal(1, callCount);
            Assert.Equal(1, invalidatedCallCount);
        }

        [Theory]
        [InlineData(PictureBoxSizeMode.Normal, 0)]
        [InlineData(PictureBoxSizeMode.StretchImage, 1)]
        [InlineData(PictureBoxSizeMode.AutoSize, 0)]
        [InlineData(PictureBoxSizeMode.CenterImage, 1)]
        [InlineData(PictureBoxSizeMode.Zoom, 1)]
        public void PictureBox_OnResize_Invoke_CallsInvalidate(PictureBoxSizeMode sizeMode, int expectedInvalidatedCallCount)
        {
            var control = new SubPictureBox
            {
                SizeMode = sizeMode
            };
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(EventArgs.Empty, e);
                callCount++;
            };
            int invalidatedCallCount = 0;
            control.Invalidated += (sender, e) => invalidatedCallCount++;
            Assert.NotEqual(IntPtr.Zero, control.Handle);

            // Call with handler.
            control.Resize += handler;
            control.OnResize(EventArgs.Empty);
            Assert.Equal(1, callCount);
            Assert.Equal(expectedInvalidatedCallCount, invalidatedCallCount);

            // Remove handler.
            control.Resize -= handler;
            control.OnResize(EventArgs.Empty);
            Assert.Equal(expectedInvalidatedCallCount * 2, invalidatedCallCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void PictureBox_OnSizeModeChanged_Invoke_CallsSizeModeChanged(EventArgs eventArgs)
        {
            var control = new SubPictureBox();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.SizeModeChanged += handler;
            control.OnSizeModeChanged(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.SizeModeChanged -= handler;
            control.OnSizeModeChanged(eventArgs);
            Assert.Equal(1, callCount);
        }

        [Theory]
        [CommonMemberData(nameof(CommonTestHelper.GetEventArgsTheoryData))]
        public void PictureBox_OnVisibleChanged_Invoke_CallsVisibleChanged(EventArgs eventArgs)
        {
            var control = new SubPictureBox();
            int callCount = 0;
            EventHandler handler = (sender, e) =>
            {
                Assert.Same(control, sender);
                Assert.Same(eventArgs, e);
                callCount++;
            };

            // Call with handler.
            control.VisibleChanged += handler;
            control.OnVisibleChanged(eventArgs);
            Assert.Equal(1, callCount);

            // Remove handler.
            control.VisibleChanged -= handler;
            control.OnVisibleChanged(eventArgs);
            Assert.Equal(1, callCount);
        }

        [Fact]
        public void PictureBox_ToString_Invoke_ReturnsExpected()
        {
            var control = new PictureBox();
            Assert.Equal("System.Windows.Forms.PictureBox, SizeMode: Normal", control.ToString());
        }

        private class SubPictureBox : PictureBox
        {
            public new bool CanEnableIme => base.CanEnableIme;

            public new bool CanRaiseEvents => base.CanRaiseEvents;

            public new CreateParams CreateParams => base.CreateParams;

            public new Cursor DefaultCursor => base.DefaultCursor;

            public new ImeMode DefaultImeMode => base.DefaultImeMode;

            public new Padding DefaultMargin => base.DefaultMargin;

            public new Size DefaultMaximumSize => base.DefaultMaximumSize;

            public new Size DefaultMinimumSize => base.DefaultMinimumSize;

            public new Padding DefaultPadding => base.DefaultPadding;

            public new Size DefaultSize => base.DefaultSize;

            public new bool DesignMode => base.DesignMode;

            public new EventHandlerList Events => base.Events;

            public new ImeMode ImeModeBase
            {
                get => base.ImeModeBase;
                set => base.ImeModeBase = value;
            }

            public new bool GetStyle(ControlStyles flag) => base.GetStyle(flag);

            public new void OnEnabledChanged(EventArgs e) => base.OnEnabledChanged(e);

            public new void OnEnter(EventArgs e) => base.OnEnter(e);

            public new void OnHandleCreated(EventArgs e) => base.OnHandleCreated(e);

            public new void OnHandleDestroyed(EventArgs e) => base.OnHandleDestroyed(e);

            public new void OnKeyDown(KeyEventArgs e) => base.OnKeyDown(e);

            public new void OnKeyPress(KeyPressEventArgs e) => base.OnKeyPress(e);

            public new void OnKeyUp(KeyEventArgs e) => base.OnKeyUp(e);

            public new void OnLeave(EventArgs e) => base.OnLeave(e);

            public new void OnLoadCompleted(AsyncCompletedEventArgs e) => base.OnLoadCompleted(e);

            public new void OnLoadProgressChanged(ProgressChangedEventArgs e) => base.OnLoadProgressChanged(e);

            public new void OnPaint(PaintEventArgs e) => base.OnPaint(e);

            public new void OnParentChanged(EventArgs e) => base.OnParentChanged(e);

            public new void OnResize(EventArgs e) => base.OnResize(e);

            public new void OnSizeModeChanged(EventArgs e) => base.OnSizeModeChanged(e);

            public new void OnVisibleChanged(EventArgs e) => base.OnVisibleChanged(e);

            public new void SetStyle(ControlStyles flag, bool value) => base.SetStyle(flag, value);
        }
    }
}
