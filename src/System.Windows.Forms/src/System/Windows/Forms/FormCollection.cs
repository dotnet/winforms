// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#nullable disable

using System.Collections;

namespace System.Windows.Forms
{
    /// <summary>
    ///  This is a read only collection of Forms exposed as a static property of the
    ///  Application class. This is used to store all the currently loaded forms in an app.
    /// </summary>
    public class FormCollection : ReadOnlyCollectionBase
    {
        internal static object CollectionSyncRoot = new object();

        /// <summary>
        ///  Gets a form specified by name, if present, else returns null. If there are multiple
        ///  forms with matching names, the first form found is returned.
        /// </summary>
        public virtual Form this[string name]
        {
            get
            {
                if (name != null)
                {
                    lock (CollectionSyncRoot)
                    {
                        foreach (Form form in InnerList)
                        {
                            if (string.Equals(form.Name, name, StringComparison.OrdinalIgnoreCase))
                            {
                                return form;
                            }
                        }
                    }
                }
                return null;
            }
        }

        /// <summary>
        ///  Gets a form specified by index.
        /// </summary>
        public virtual Form this[int index]
        {
            get
            {
                Form f = null;

                lock (CollectionSyncRoot)
                {
                    f = (Form)InnerList[index];
                }
                return f;
            }
        }

        /// <summary>
        ///  Used internally to add a Form to the FormCollection
        /// </summary>
        internal void Add(Form form)
        {
            lock (CollectionSyncRoot)
            {
                InnerList.Add(form);
            }
        }

        /// <summary>
        ///  Used internally to check if a Form is in the FormCollection
        /// </summary>
        internal bool Contains(Form form)
        {
            bool inCollection = false;
            lock (CollectionSyncRoot)
            {
                inCollection = InnerList.Contains(form);
            }
            return inCollection;
        }

        /// <summary>
        ///  Used internally to add a Form to the FormCollection
        /// </summary>
        internal void Remove(Form form)
        {
            lock (CollectionSyncRoot)
            {
                InnerList.Remove(form);
            }
        }
    }
}
