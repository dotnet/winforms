// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System;

    /// <devdoc>
    ///    <para>Provides a format-independent mechanism for transferring
    ///       data.</para>
    /// </devdoc>
    [System.Runtime.InteropServices.ComVisible(true)]
    public interface IDataObject {
        /// <devdoc>
        ///    <para>Retrieves the data associated with the specified data 
        ///       format, using autoConvert to determine whether to convert the data to the
        ///       format. </para>
        /// </devdoc>
        object GetData(string format, bool autoConvert);

        /// <devdoc>
        ///    <para>Retrieves the data associated with the specified data format.</para>
        /// </devdoc>
        object GetData(string format);

        /// <devdoc>
        ///    <para>Retrieves the data associated with the specified class 
        ///       type format.</para>
        /// </devdoc>
        object GetData(Type format);

        /// <devdoc>
        ///    <para>Stores the specified data and its associated format in 
        ///       this instance, using autoConvert to specify whether the data can be converted
        ///       to another format.</para>
        /// </devdoc>
        void SetData(string format, bool autoConvert, object data);

        /// <devdoc>
        ///    <para>Stores the specified data and its associated format in this
        ///       instance.</para>
        /// </devdoc>
        void SetData(string format, object data);

        /// <devdoc>
        ///    <para>Stores the specified data and its associated class type in this
        ///       instance.</para>
        /// </devdoc>
        void SetData(Type format, object data);

        /// <devdoc>
        ///    <para>Stores the specified data in this instance, using the class of the
        ///       data for the format.</para>
        /// </devdoc>
        void SetData(object data);

        /// <devdoc>
        ///    <para>Determines whether data stored in this instance is 
        ///       associated with the specified format, using autoConvert to determine whether to
        ///       convert the data to the format.</para>
        /// </devdoc>
        bool GetDataPresent(string format, bool autoConvert);

        /// <devdoc>
        ///    <para>Determines whether data stored in this instance is associated with, or can be
        ///       converted to, the specified format.</para>
        /// </devdoc>
        bool GetDataPresent(string format);

        /// <devdoc>
        ///    <para>Determines whether data stored in this instance is associated with, or can be converted to, the specified format.</para>
        /// </devdoc>
        bool GetDataPresent(Type format);

        /// <devdoc>
        ///    <para>Gets a list of all formats that data stored in this 
        ///       instance is associated with or can be converted to, using autoConvert to determine whether to
        ///       retrieve all formats that the data can be converted to or only native data
        ///       formats.</para>
        /// </devdoc>
        string[] GetFormats(bool autoConvert);

        /// <devdoc>
        ///    <para>Gets a list of all formats that data stored in this instance is associated
        ///       with or can be converted to.</para>
        /// </devdoc>
        string[] GetFormats();
    }
}
