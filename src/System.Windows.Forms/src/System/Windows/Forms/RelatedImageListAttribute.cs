// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms
{
    /// <summary>
    ///  Specifies which imagelist a property relates to. For example ImageListIndex must relate to a
    ///  specific ImageList property
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public sealed class RelatedImageListAttribute : Attribute
    {
        private readonly string relatedImageList = null;

        public RelatedImageListAttribute(string relatedImageList)
        {
            this.relatedImageList = relatedImageList;
        }

        public string RelatedImageList
        {
            get
            {
                return relatedImageList;
            }
        }
    } // end of RelatedImageListAttribute class
} // end of namespace
