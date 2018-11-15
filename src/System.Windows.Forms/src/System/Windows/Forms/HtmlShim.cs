// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    
    /// This is essentially a proxy object between the native
    /// html objects and our managed ones.  We want the managed
    /// HtmlDocument, HtmlWindow and HtmlElement to be super-lightweight,
    /// which means that we shouldnt have things that tie up their lifetimes
    /// contained within them.  The "Shim" is essentially the object that
    /// manages events coming out of the HtmlDocument, HtmlElement and HtmlWindow
    /// and serves them back up to the user.

    internal abstract class HtmlShim : IDisposable {

        private EventHandlerList events;
        private int eventCount = 0;
        private Dictionary<EventHandler, HtmlToClrEventProxy> attachedEventList;
        
        
        protected HtmlShim() {
        }
        ~HtmlShim() {
            Dispose(false);
        }

        private EventHandlerList Events {
            get { 
                if (events == null) {
                    events = new EventHandlerList();
                }
                return events;
            }
        }

        /// Support IHtml*3.AttachHandler
        public abstract void AttachEventHandler(string eventName, EventHandler eventHandler);

           
        public void AddHandler(object key, Delegate value) {
            eventCount++;
            Events.AddHandler(key, value);
            OnEventHandlerAdded();
        }

        protected HtmlToClrEventProxy AddEventProxy(string eventName, EventHandler eventHandler) {
            if (attachedEventList == null) {
                attachedEventList = new Dictionary<EventHandler, HtmlToClrEventProxy>();
            }     
            HtmlToClrEventProxy proxy = new HtmlToClrEventProxy(this, eventName, eventHandler);
            attachedEventList[eventHandler] = proxy;
            return proxy;
        }


        public abstract UnsafeNativeMethods.IHTMLWindow2  AssociatedWindow {
            get;
        }
        

        /// create connectionpoint cookie 
        public abstract void ConnectToEvents();

        /// Support IHtml*3.DetachEventHandler
        public abstract void DetachEventHandler(string eventName, EventHandler  eventHandler);
        

        /// disconnect from connectionpoint cookie
        /// inheriting classes should override to disconnect from ConnectionPoint and call base.
        public virtual void DisconnectFromEvents() {

            if (attachedEventList != null) {
                EventHandler[] events = new EventHandler[attachedEventList.Count];
                attachedEventList.Keys.CopyTo(events,0);
            
                foreach (EventHandler eh in events) {
                    HtmlToClrEventProxy proxy = attachedEventList[eh];
                    DetachEventHandler(proxy.EventName, eh);
                }
            }

        }

        /// return the sender for events, usually the HtmlWindow, HtmlElement, HtmlDocument
        protected abstract object GetEventSender();

     
       
        public void Dispose() {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing) {
            if (disposing) {
                DisconnectFromEvents();
                if (this.events != null) {
                    this.events.Dispose();
                    this.events = null;
                }
            }
        }

        public void FireEvent(object key, EventArgs e)
        {
 
            System.Delegate delegateToInvoke = (System.Delegate)Events[key];
        
            if (delegateToInvoke != null) {
                try {
                   delegateToInvoke.DynamicInvoke(GetEventSender(), e);
                }
                catch (Exception ex) {
                    // Note: this check is for the debugger, so we can catch exceptions in the debugger instead of 
                    // throwing a thread exception.
                    if (NativeWindow.WndProcShouldBeDebuggable)
                    {
                        throw;
                    }
                    else {
                        Application.OnThreadException(ex);
                    }
                }
            }
        }
        protected virtual void OnEventHandlerAdded() {
            ConnectToEvents(); 
        }

        protected virtual void OnEventHandlerRemoved() {
            if (eventCount <=0) {
                DisconnectFromEvents();
                eventCount = 0;
            }
        }

        public void RemoveHandler(object key, Delegate value) {
            eventCount--;
            Events.RemoveHandler(key, value);
            OnEventHandlerRemoved();
        }

        protected HtmlToClrEventProxy RemoveEventProxy(EventHandler eventHandler) {
            if (attachedEventList == null) {
              return null;
            }

            if (attachedEventList.ContainsKey(eventHandler)) {  
              HtmlToClrEventProxy proxy = attachedEventList[eventHandler] as HtmlToClrEventProxy;
              attachedEventList.Remove(eventHandler);
              return proxy;
            }
            return null;
        }
    }

}




