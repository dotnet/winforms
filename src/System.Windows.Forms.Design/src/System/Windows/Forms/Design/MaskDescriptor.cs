// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Globalization;
using System.Threading;

namespace System.Windows.Forms.Design
{
    /// <summary>
    /// MaskDescriptor abstract class defines the set of methods mask descriptors need to implement for the
    /// MaskedTextBox.Mask UITypeEditor to include as options in the property editor. MaskDescriptor
    /// types are discovered at designed time by querying the ITypeDiscoveryService service provider from
    /// the UITypeEditor object.
    /// </summary>
    public abstract class MaskDescriptor
    {
        /// <summary>
        /// The mask being described.
        /// </summary>
        public abstract string Mask { get; }

        /// <summary>
        /// The friendly name of the mask descriptor.
        /// Used also as the description for the mask.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// A sample text following the mask specification.
        /// </summary>
        public abstract string Sample { get; }

        /// <summary>
        /// A Type representing the type providing validation for this mask.
        /// </summary>
        public abstract Type ValidatingType { get; }

        /// <summary>
        /// The CultureInfo representing the locale the mask is designed for.
        /// </summary>
        public virtual CultureInfo Culture
        {
            get { return Thread.CurrentThread.CurrentCulture; }
        }

        /// <summary>
        /// Determines whether the specified mask descriptor is valid and hence can be added to the canned masks list.
        /// A valid MaskDescriptor must meet the following conditions:
        /// 1. Not null.
        /// 2. Not null or empty mask.
        /// 3. Not null or empty name.
        /// 4. Not null or empty sample.
        /// 5. The sample is correct based on the mask and all required edit characters have been provided (mask completed - not necessarily full).
        /// 6. The sample is valid based on the ValidatingType object (if any).
        /// </summary>
        public static bool IsValidMaskDescriptor(MaskDescriptor maskDescriptor)
        {
            return IsValidMaskDescriptor(maskDescriptor, out string _);
        }

        public static bool IsValidMaskDescriptor(MaskDescriptor maskDescriptor, out string validationErrorDescription)
        {
            validationErrorDescription = string.Empty;

            if (maskDescriptor is null)
            {
                validationErrorDescription = SR.MaskDescriptorNull;
                return false;
            }

            if (string.IsNullOrEmpty(maskDescriptor.Mask) || string.IsNullOrEmpty(maskDescriptor.Name) || string.IsNullOrEmpty(maskDescriptor.Sample))
            {
                validationErrorDescription = SR.MaskDescriptorNullOrEmptyRequiredProperty;
                return false;
            }

            MaskedTextProvider maskedTextProvider = new MaskedTextProvider(maskDescriptor.Mask, maskDescriptor.Culture);
            MaskedTextBox maskedTextBox = new MaskedTextBox(maskedTextProvider);

            maskedTextBox.SkipLiterals = true;
            maskedTextBox.ResetOnPrompt = true;
            maskedTextBox.ResetOnSpace = true;
            maskedTextBox.ValidatingType = maskDescriptor.ValidatingType;
            maskedTextBox.FormatProvider = maskDescriptor.Culture;
            maskedTextBox.Culture = maskDescriptor.Culture;
            maskedTextBox.TypeValidationCompleted += new TypeValidationEventHandler(maskedTextBox1_TypeValidationCompleted);
            maskedTextBox.MaskInputRejected += new MaskInputRejectedEventHandler(maskedTextBox1_MaskInputRejected);

            // Add sample. If it fails we are done.
            maskedTextBox.Text = maskDescriptor.Sample;

            if (maskedTextBox.Tag is null) // Sample was added successfully (MaskInputRejected event handler did not change the maskedTextBox tag).
            {
                if (maskDescriptor.ValidatingType != null)
                {
                    maskedTextBox.ValidateText();
                }
            }

            if (maskedTextBox.Tag != null) // Validation failed.
            {
                validationErrorDescription = maskedTextBox.Tag.ToString();
            }

            return validationErrorDescription.Length == 0;
        }

        private static void maskedTextBox1_MaskInputRejected(object sender, MaskInputRejectedEventArgs e)
        {
            MaskedTextBox maskedTextBox = sender as MaskedTextBox;
            maskedTextBox.Tag = MaskedTextBoxDesigner.GetMaskInputRejectedErrorMessage(e);
        }

        private static void maskedTextBox1_TypeValidationCompleted(object sender, TypeValidationEventArgs e)
        {
            if (e.IsValidInput)
            {
                return;
            }

            MaskedTextBox maskedTextBox = sender as MaskedTextBox;
            maskedTextBox.Tag = e.Message;
        }

        /// <summary>
        /// Determines whether this mask descriptor and the passed object describe the same mask.
        /// True if the following conditions are met in both, this and the passed object:
        /// 1. Mask property is the same.
        /// 2. Validating type is the same
        /// Observe that the Name property is not considered since MaskedTextProvider/Box are not
        /// aware of it.
        /// </summary>
        public override bool Equals(object maskDescriptor)
        {
            MaskDescriptor descriptor = maskDescriptor as MaskDescriptor;

            if (!IsValidMaskDescriptor(descriptor) || !IsValidMaskDescriptor(this))
            {
                return this == maskDescriptor; // shallow comparison.
            }

            return ((Mask == descriptor.Mask) && (ValidatingType == descriptor.ValidatingType));
        }

        public override int GetHashCode()
        {
            string hash = Mask;

            if (ValidatingType != null)
            {
                hash += ValidatingType.ToString();
            }
            return hash.GetHashCode();
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.CurrentCulture, "{0}<Name={1}, Mask={2}, ValidatingType={3}",
                GetType(),
                Name != null ? Name : "null",
                Mask != null ? Mask : "null",
                ValidatingType != null ? ValidatingType.ToString() : "null"
                );
        }
    }
}
