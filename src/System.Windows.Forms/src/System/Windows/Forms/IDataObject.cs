// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Runtime.InteropServices;

namespace System.Windows.Forms
{
    /// <summary>
    /// Provides a format-independent mechanism for transferring data.
    /// </devdoc>
    [ComVisible(true)]
    public interface IDataObject
    {
        /// <summary>
        /// Retrieves the data associated with the specified data format, using
        /// autoConvert to determine whether to convert the data to the  format.
        /// </devdoc>
        object GetData(string format, bool autoConvert);

        /// <summary>
        ///  Retrieves the data associated with the specified data format.
        /// </devdoc>
        object GetData(string format);

        /// <summary>
        /// Retrieves the data associated with the specified class type format.
        /// </devdoc>
        object GetData(Type format);

        /// <summary>
        /// Stores the specified data and its associated format in  this instance,
        /// using autoConvert to specify whether the data can be converted to
        /// another format.</para>
        /// </devdoc>
        void SetData(string format, bool autoConvert, object data);

        /// <summary>
        /// Stores the specified data and its associated format in this instance.
        /// </devdoc>
        void SetData(string format, object data);

        /// <summary>
        ///  Stores the specified data and its associated class type in this
        /// instance.
        /// </devdoc>
        void SetData(Type format, object data);

        /// <summary>
        /// Stores the specified data in this instance, using the class of the
        /// data for the format.
        /// </devdoc>
        void SetData(object data);

        /// <summary>
        /// Determines whether data stored in this instance is  associated with the
        /// specified format, using autoConvert to determine whether to convert the
        /// data to the format.
        /// </devdoc>
        bool GetDataPresent(string format, bool autoConvert);

        /// <summary>
        /// Determines whether data stored in this instance is associated with, or
        /// can be converted to, the specified format.
        /// </devdoc>
        bool GetDataPresent(string format);

        /// <summary>
        /// Determines whether data stored in this instance is associated with, or
        /// can be converted to, the specified format.
        /// </devdoc>
        bool GetDataPresent(Type format);

        /// <summary>
        /// Gets a list of all formats that data stored in this instance is
        /// associated with or can be converted to, using autoConvert to determine
        /// whether to retrieve all formats that the data can be converted to or'
        /// only native data formats.
        /// </devdoc>
        string[] GetFormats(bool autoConvert);

        /// <summary>
        /// Gets a list of all formats that data stored in this instance is
        /// associated with or can be converted to.
        /// </devdoc>
        string[] GetFormats();
    }
}
