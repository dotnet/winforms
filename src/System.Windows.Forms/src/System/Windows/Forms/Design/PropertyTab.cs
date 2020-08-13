// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.ComponentModel;
using System.Drawing;

namespace System.Windows.Forms.Design
{
    /// <summary>
    ///  Provides a base class for property tabs.
    /// </summary>
    public abstract class PropertyTab : IExtenderProvider
    {
        private Bitmap _bitmap;
        private bool _checkedBmp;

        ~PropertyTab() => Dispose(false);

        /// <summary>
        ///  Gets or sets a bitmap to display in the property tab.
        /// </summary>
        public virtual Bitmap Bitmap
        {
            get
            {
                if (!_checkedBmp && _bitmap is null)
                {
                    string bmpName = GetType().Name;
                    try
                    {
                        _bitmap = DpiHelper.GetBitmapFromIcon(GetType(), bmpName);
                    }
                    catch (Exception)
                    {
                    }

                    _checkedBmp = true;
                }

                return _bitmap;
            }
        }

        /// <summary>
        ///  Gets or sets the array of components the property tab is associated with.
        /// </summary>
        public virtual object[] Components { get; set; }

        /// <summary>
        ///  Gets or sets the name for the property tab.
        /// </summary>
        public abstract string TabName { get; }

        /// <summary>
        ///  Gets or sets the help keyword that is to be associated with this tab. This
        ///  defaults to the tab name.
        /// </summary>
        public virtual string HelpKeyword => TabName;

        /// <summary>
        ///  Gets a value indicating whether the specified object be can extended.
        /// </summary>
        public virtual bool CanExtend(object extendee) => true;

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_bitmap != null)
                {
                    _bitmap.Dispose();
                    _bitmap = null;
                }
            }
        }

        /// <summary>
        ///  Gets the default property of the specified component.
        /// </summary>
        public virtual PropertyDescriptor GetDefaultProperty(object component)
        {
            return TypeDescriptor.GetDefaultProperty(component);
        }

        /// <summary>
        ///  Gets the properties of the specified component.
        /// </summary>
        public virtual PropertyDescriptorCollection GetProperties(object component)
        {
            return GetProperties(component, null);
        }

        /// <summary>
        ///  Gets the properties of the specified component which match the specified
        ///  attributes.
        /// </summary>
        public abstract PropertyDescriptorCollection GetProperties(object component, Attribute[] attributes);

        /// <summary>
        ///  Gets the properties of the specified component.
        /// </summary>
        public virtual PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object component, Attribute[] attributes)
        {
            return GetProperties(component, attributes);
        }
    }
}
