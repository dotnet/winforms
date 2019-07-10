// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using System;
#if NETCORE
using System.Drawing.Design;
using System.Windows.Forms.Design;
#endif

namespace WinformsControlsTest.UserControls
{
    internal class UserControlWithCollectionEditors : UserControl
    {
        private const string Category = "!CollectionEditors";
        private int[] _intArray = Enumerable.Range(1, 10).ToArray();
        private Array _intLegacyArray = Enumerable.Range(1, 10).ToArray();
        private List<int> _intList = Enumerable.Range(1, 10).ToList();

        public UserControlWithCollectionEditors()
        {
            AutoScaleMode = AutoScaleMode.Font;
        }

#if NETCORE
        [Editor(typeof(IntegerCollectionEditor), typeof(UITypeEditor))]
#endif
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Category(Category)]
        public int[] IntArray
        {
            get => _intArray;
            set => _intArray = value;
        }

#if NETCORE
        [Editor(typeof(IntegerCollectionEditor), typeof(UITypeEditor))]
#endif
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Category(Category)]
        public List<int> IntList => _intList;

#if NETCORE
        [Editor(typeof(IntegerCollectionEditor), typeof(UITypeEditor))]
#endif
        [Browsable(true)]
        [EditorBrowsable(EditorBrowsableState.Always)]
        [Category(Category)]
        public Array IntLegacyArray => _intLegacyArray;
    }
}
