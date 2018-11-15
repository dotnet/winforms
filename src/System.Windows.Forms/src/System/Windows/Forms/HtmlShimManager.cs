// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {

    using System;
    using System.Collections.Generic;
    using System.Collections;
    using System.Diagnostics;
    
    /// <devdoc>
    ///  HtmlShimManager - this class manages the shims for HtmlWindows, HtmlDocuments, and HtmlElements.
    ///  essentially we need a long-lasting object to call back on events from the web browser, and the 
    ///  manager is the one in charge of making sure this list stays around as long as needed.  
    ///
    ///  When a HtmlWindow unloads we prune our list of corresponding document, window, and element shims.
    ///
    /// </devdoc>
    internal sealed class HtmlShimManager :IDisposable {

            private Dictionary<HtmlWindow, HtmlWindow.HtmlWindowShim> htmlWindowShims;
            private Dictionary<HtmlElement, HtmlElement.HtmlElementShim> htmlElementShims;
            private Dictionary<HtmlDocument, HtmlDocument.HtmlDocumentShim> htmlDocumentShims;

            internal HtmlShimManager() {
            }

            /// <devdoc> AddDocumentShim - adds a HtmlDocumentShim to list of shims to manage 
            ///   Can create a WindowShim as a side effect so it knows when to self prune from the list.
            ///</devdoc>
            public void AddDocumentShim(HtmlDocument doc) {
               HtmlDocument.HtmlDocumentShim shim = null;
               
               if (htmlDocumentShims == null) {
                   htmlDocumentShims = new Dictionary<HtmlDocument,HtmlDocument.HtmlDocumentShim>();
                   shim = new HtmlDocument.HtmlDocumentShim(doc);
                   htmlDocumentShims[doc] = shim;
               }
               else if (!htmlDocumentShims.ContainsKey(doc)) {
                   shim = new HtmlDocument.HtmlDocumentShim(doc);
                   htmlDocumentShims[doc] = shim;
               }
               if (shim != null) {
                   OnShimAdded(shim);
               }
               
           }
    
            /// <devdoc> AddWindowShim - adds a HtmlWindowShim to list of shims to manage 
            ///</devdoc>

            public void AddWindowShim(HtmlWindow window) {
                HtmlWindow.HtmlWindowShim shim = null;
                if (htmlWindowShims == null) {
                    htmlWindowShims = new Dictionary<HtmlWindow,HtmlWindow.HtmlWindowShim>();
                    shim = new HtmlWindow.HtmlWindowShim(window);
                    htmlWindowShims[window] = shim;
                }
                else if (!htmlWindowShims.ContainsKey(window)) {
                    shim = new HtmlWindow.HtmlWindowShim(window);
                    htmlWindowShims[window] = shim;
                }
                if (shim != null) {
                   // strictly not necessary, but here for future use.
                   OnShimAdded(shim);
                }
            }

            /// <devdoc> AddElementShim - adds a HtmlDocumentShim to list of shims to manage 
            ///   Can create a WindowShim as a side effect so it knows when to self prune from the list.
            ///</devdoc>
            public void AddElementShim(HtmlElement element) {
                HtmlElement.HtmlElementShim shim = null;

                if (htmlElementShims == null) {
                    htmlElementShims = new Dictionary<HtmlElement,HtmlElement.HtmlElementShim>();
                    shim = new HtmlElement.HtmlElementShim(element);
                    htmlElementShims[element] = shim;
                }
                else if (!htmlElementShims.ContainsKey(element)) {
                    shim = new HtmlElement.HtmlElementShim(element);
                    htmlElementShims[element] = shim;
                }
                if (shim != null) {
                   OnShimAdded(shim);
                }
               
            }
            
            internal HtmlDocument.HtmlDocumentShim GetDocumentShim(HtmlDocument document) {
                if (htmlDocumentShims == null) {
                    return null;
                }
                if (htmlDocumentShims.ContainsKey(document)) {
                    return htmlDocumentShims[document];
                }
                return null;
            }
            
            internal HtmlElement.HtmlElementShim GetElementShim(HtmlElement element) {
                if (htmlElementShims == null) {
                    return null;
                }
                if (htmlElementShims.ContainsKey(element)) {
                    return htmlElementShims[element];
                }
                return null;
            }

            internal HtmlWindow.HtmlWindowShim GetWindowShim(HtmlWindow window) {
               if (htmlWindowShims == null) {
                    return null;
                }
                if (htmlWindowShims.ContainsKey(window)) {
                    return htmlWindowShims[window];
                }
                return null;
            }

            private void OnShimAdded(HtmlShim addedShim) {
                
                Debug.Assert(addedShim != null, "Why are we calling this with a null shim?");
                if (addedShim != null) {
                    if (!(addedShim is HtmlWindow.HtmlWindowShim)) {
                        // we need to add a window shim here for documents and elements
                        // so we can sync Window.Unload.  The window shim itself will trap
                        // the unload event and call back on us on OnWindowUnloaded.  When
                        // that happens we know we can free all our ptrs to COM.
                        AddWindowShim(new HtmlWindow(this, addedShim.AssociatedWindow));
                    }
                }
            }
            /// <devdoc>
            /// HtmlWindowShim calls back on us when it has unloaded the page.  At this point we need to 
            /// walk through our lists and make sure we've cleaned up
            /// </devdoc>
            internal void OnWindowUnloaded(HtmlWindow unloadedWindow) {
                Debug.Assert(unloadedWindow != null, "Why are we calling this with a null window?");
                if (unloadedWindow != null) {
                    //
                    // prune documents
                    //
                    if (htmlDocumentShims != null) {
                        HtmlDocument.HtmlDocumentShim[] shims = new HtmlDocument.HtmlDocumentShim[htmlDocumentShims.Count];
                        htmlDocumentShims.Values.CopyTo(shims,0);
                        
                        foreach (HtmlDocument.HtmlDocumentShim shim in shims) {
                            if (shim.AssociatedWindow == unloadedWindow.NativeHtmlWindow) {
                                htmlDocumentShims.Remove(shim.Document);
                                shim.Dispose();
                            }
                        }
                    }
                    //
                    // prune elements
                    //
                    if (htmlElementShims != null) {
                        HtmlElement.HtmlElementShim[] shims = new HtmlElement.HtmlElementShim[htmlElementShims.Count];
                        htmlElementShims.Values.CopyTo(shims,0);
                        
                        foreach (HtmlElement.HtmlElementShim shim in shims) {
                            if (shim.AssociatedWindow == unloadedWindow.NativeHtmlWindow) {
                                htmlElementShims.Remove(shim.Element);
                                shim.Dispose();
                            }
                        }
                    }

                    //
                    // prune the particular window from the list.  
                    //
                    if (htmlWindowShims != null) {
                         if (htmlWindowShims.ContainsKey(unloadedWindow)) {
                            HtmlWindow.HtmlWindowShim shim = htmlWindowShims[unloadedWindow];
                            htmlWindowShims.Remove(unloadedWindow);
                            shim.Dispose();
                         }
                    }
                }
            }

            public void Dispose() {
                 Dispose(true);
            }

            private void Dispose(bool disposing) {
                if (disposing) {

                    if (htmlElementShims != null){
                        foreach (HtmlElement.HtmlElementShim shim in htmlElementShims.Values) {
                            shim.Dispose();
                        }
                    }
                    if (htmlDocumentShims != null) {
                        foreach (HtmlDocument.HtmlDocumentShim shim in htmlDocumentShims.Values) {
                            shim.Dispose();
                        }
                    }

                    if (htmlWindowShims != null) {
                        foreach (HtmlWindow.HtmlWindowShim shim in htmlWindowShims.Values) {
                            shim.Dispose();
                        }
                    }
                    htmlWindowShims = null;
                    htmlDocumentShims = null;
                    htmlWindowShims = null;
                    
                    
                }
            }
            ~HtmlShimManager() {
                Dispose(false);
            }
            

            
     }
 }
