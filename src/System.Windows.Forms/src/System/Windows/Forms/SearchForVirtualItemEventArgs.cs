// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Drawing;

namespace System.Windows.Forms {
    public class SearchForVirtualItemEventArgs : EventArgs {
        private bool isTextSearch;
        private bool isPrefixSearch;
        private bool includeSubItemsInSearch;
        private string text;
        private Point startingPoint;
        private SearchDirectionHint direction;
        private int startIndex;
        private int index = -1;

        public SearchForVirtualItemEventArgs(bool isTextSearch, bool isPrefixSearch, bool includeSubItemsInSearch, string text, Point startingPoint, SearchDirectionHint direction, int startIndex) {
            this.isTextSearch = isTextSearch;
            this.isPrefixSearch = isPrefixSearch;
            this.includeSubItemsInSearch = includeSubItemsInSearch;
            this.text = text;
            this.startingPoint = startingPoint;
            this.direction = direction;
            this.startIndex = startIndex;
        }

        public bool IsTextSearch {
            get  {
                return isTextSearch;
            }
        }

        public bool IncludeSubItemsInSearch {
            get {
                return includeSubItemsInSearch;
            }
        }

        public int Index {
            get  {
                return this.index;
            }
            set
            {
                this.index = value;
            }
        }

        public bool IsPrefixSearch {
            get  {
                return isPrefixSearch;
            }
        }

        public string Text{
            get  {
                return text;
            }
        }

        public Point StartingPoint {
            get  {
                return startingPoint;
            }
        }

        public SearchDirectionHint Direction {
            get  {
                return direction;
            }
        }

        public int StartIndex {
            get  {
                return startIndex;
            }
        }
    }
}
