// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;

namespace System.Windows.Forms {
    /// <include file='doc\SearchForVirtualItemEventArgs.uex' path='docs/doc[@for="SearchForVirtualItemEventArgs"]/*' />
    public class SearchForVirtualItemEventArgs : EventArgs {
        private bool isTextSearch;
        private bool isPrefixSearch;
        private bool includeSubItemsInSearch;
        private string text;
        private Point startingPoint;
        private SearchDirectionHint direction;
        private int startIndex;
        private int index = -1;

        /// <include file='doc\SearchForVirtualItemEventArgs.uex' path='docs/doc[@for="SearchForVirtualItemEventArgs.SearchForVirtualItemEventArgs"]/*' />
        public SearchForVirtualItemEventArgs(bool isTextSearch, bool isPrefixSearch, bool includeSubItemsInSearch, string text, Point startingPoint, SearchDirectionHint direction, int startIndex) {
            this.isTextSearch = isTextSearch;
            this.isPrefixSearch = isPrefixSearch;
            this.includeSubItemsInSearch = includeSubItemsInSearch;
            this.text = text;
            this.startingPoint = startingPoint;
            this.direction = direction;
            this.startIndex = startIndex;
        }

        /// <include file='doc\SearchForVirtualItemEventArgs.uex' path='docs/doc[@for="SearchForVirtualItemEventArgs.IsTextSearch"]/*' />
        public bool IsTextSearch {
            get  {
                return isTextSearch;
            }
        }

        /// <include file='doc\SearchForVirtualItemEventArgs.uex' path='docs/doc[@for="SearchForVirtualItemEventArgs.IncludeSubItemsInSearch"]/*' />
        public bool IncludeSubItemsInSearch {
            get {
                return includeSubItemsInSearch;
            }
        }

        /// <include file='doc\SearchForVirtualItemEventArgs.uex' path='docs/doc[@for="SearchForVirtualItemEventArgs.Index"]/*' />
        public int Index {
            get  {
                return this.index;
            }
            set
            {
                this.index = value;
            }
        }

        /// <include file='doc\SearchForVirtualItemEventArgs.uex' path='docs/doc[@for="SearchForVirtualItemEventArgs.IsPrefixSearch"]/*' />
        public bool IsPrefixSearch {
            get  {
                return isPrefixSearch;
            }
        }

        /// <include file='doc\SearchForVirtualItemEventArgs.uex' path='docs/doc[@for="SearchForVirtualItemEventArgs.Text"]/*' />
        public string Text{
            get  {
                return text;
            }
        }

        /// <include file='doc\SearchForVirtualItemEventArgs.uex' path='docs/doc[@for="SearchForVirtualItemEventArgs.StartingPoint"]/*' />
        public Point StartingPoint {
            get  {
                return startingPoint;
            }
        }

        /// <include file='doc\SearchForVirtualItemEventArgs.uex' path='docs/doc[@for="SearchForVirtualItemEventArgs.Direction"]/*' />
        public SearchDirectionHint Direction {
            get  {
                return direction;
            }
        }

        /// <include file='doc\SearchForVirtualItemEventArgs.uex' path='docs/doc[@for="SearchForVirtualItemEventArgs.StartIndex"]/*' />
        public int StartIndex {
            get  {
                return startIndex;
            }
        }
    }
}
