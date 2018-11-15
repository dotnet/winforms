// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace System.Windows.Forms {
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Diagnostics;
    using System.Threading;
    using System.Runtime.InteropServices;    
    using System.Security;
    using System.Globalization;
    
    /// <devdoc>
    /// Why do we have this class?  Well it seems that RegisterDropTarget
    /// requires an HWND to back it's IDropTargets.  Since some ToolStripItems
    /// do not have HWNDS, this guy's got to figure out who the event was
    /// really supposed to go to and pass it on to it. 
    /// </devdoc>
    internal class ToolStripDropTargetManager : IDropTarget {
	
        private IDropTarget lastDropTarget;
        private ToolStrip owner;

#if DEBUG
        private bool dropTargetIsEntered;
#endif


#if DEBUG        
        internal static readonly TraceSwitch DragDropDebug = new TraceSwitch("DragDropDebug", "Debug ToolStrip DragDrop code");
#else
        internal static readonly TraceSwitch DragDropDebug;
#endif

        /// <devdoc>
        /// Summary of ToolStripDropTargetManager.
        /// </devdoc>
        /// <param name=owner></param>	
        public ToolStripDropTargetManager(ToolStrip owner) {
            this.owner = owner;
            if (owner == null) {
                throw new ArgumentNullException("owner");
            }
        }
    
        /// <devdoc>
        /// Summary of EnsureRegistered.
        /// </devdoc>
        /// <param name=dropTarget></param>	
        public void EnsureRegistered(IDropTarget dropTarget) {
            Debug.WriteLineIf(DragDropDebug.TraceVerbose, "Ensuring drop target registered");  
            SetAcceptDrops(true);	
        }

        /// <devdoc>
        /// Summary of EnsureUnRegistered.
        /// </devdoc>
        /// <param name=dropTarget></param>	
        public void EnsureUnRegistered(IDropTarget dropTarget) {
        
            Debug.WriteLineIf(DragDropDebug.TraceVerbose, "Attempting to unregister droptarget");
            for (int i = 0; i < owner.Items.Count; i++) {
                if (owner.Items[i].AllowDrop) {
                    Debug.WriteLineIf(DragDropDebug.TraceVerbose, "An item still has allowdrop set to true - cant unregister");
                    return; // can't unregister this as a drop target unless everyone is done.			
                }
            }
            if (owner.AllowDrop || owner.AllowItemReorder) {
                Debug.WriteLineIf(DragDropDebug.TraceVerbose, "The ToolStrip has AllowDrop or AllowItemReorder set to true - cant unregister");
                return;  // can't unregister this as a drop target if ToolStrip is still accepting drops
            }
		
            SetAcceptDrops(false);
            owner.DropTargetManager = null;
        }

        /// <devdoc>
        /// Takes a screen point and converts it into an item. May return null.
        /// </devdoc>
        /// <param name=x></param>
        /// <param name=y></param>	
        private ToolStripItem FindItemAtPoint(int x, int y) {
            return owner.GetItemAt(owner.PointToClient(new Point(x,y)));
        }	
        /// <devdoc>
        /// Summary of OnDragEnter.
        /// </devdoc>
        /// <param name=e></param>	
        public void OnDragEnter(DragEventArgs e) {
			Debug.WriteLineIf(DragDropDebug.TraceVerbose, "[DRAG ENTER] ==============");
     
            // If we are supporting Item Reordering 
            // and this is a ToolStripItem - snitch it.
            if (owner.AllowItemReorder && e.Data.GetDataPresent(typeof(ToolStripItem))) {
            	Debug.WriteLineIf(DragDropDebug.TraceVerbose, "ItemReorderTarget taking this...");
                lastDropTarget = owner.ItemReorderDropTarget;
          
            }
            else {
                ToolStripItem item = FindItemAtPoint(e.X, e.Y);

                if ((item != null) && (item.AllowDrop)) { 
                    // the item wants this event
                    Debug.WriteLineIf(DragDropDebug.TraceVerbose, "ToolStripItem taking this: " + item.ToString());
     
                    lastDropTarget = ((IDropTarget)item);				
                }
                else if (owner.AllowDrop) {
                    // the winbar wants this event
                    Debug.WriteLineIf(DragDropDebug.TraceVerbose, "ToolStrip taking this because AllowDrop set to true.");   
                    lastDropTarget = ((IDropTarget)owner);
                    
                }	
                else {
                    // There could be one item that says "AllowDrop == true" which would turn
                    // on this drop target manager.  If we're not over that particular item - then
                    // just null out the last drop target manager.
                    
                    // the other valid reason for being here is that we've done an AllowItemReorder
                    // and we dont have a ToolStripItem contain within the data (say someone drags a link 
                    // from IE over the ToolStrip)

                    Debug.WriteLineIf(DragDropDebug.TraceVerbose, "No one wanted it.");
     
                    lastDropTarget = null;

                }		
            }
            if (lastDropTarget != null) {
                Debug.WriteLineIf(DragDropDebug.TraceVerbose, "Calling OnDragEnter on target...");     
#if DEBUG
                dropTargetIsEntered=true;
#endif
                lastDropTarget.OnDragEnter(e);
            }
        }

        /// <devdoc>
        /// Summary of OnDragOver.
        /// </devdoc>
        /// <param name=e></param>	
        public void OnDragOver(DragEventArgs e) {

	        Debug.WriteLineIf(DragDropDebug.TraceVerbose, "[DRAG OVER] ==============");
     
            IDropTarget newDropTarget = null;

            // If we are supporting Item Reordering 
            // and this is a ToolStripItem - snitch it.
            if (owner.AllowItemReorder && e.Data.GetDataPresent(typeof(ToolStripItem))) {
               	Debug.WriteLineIf(DragDropDebug.TraceVerbose, "ItemReorderTarget taking this...");
                newDropTarget = owner.ItemReorderDropTarget;
            }
            else {
                ToolStripItem item = FindItemAtPoint(e.X, e.Y);

                if ((item != null) && (item.AllowDrop)) { 
                    // the item wants this event
                    Debug.WriteLineIf(DragDropDebug.TraceVerbose, "ToolStripItem taking this: " + item.ToString());
                    newDropTarget = ((IDropTarget)item);				
                }
                else if (owner.AllowDrop) {
                    // the winbar wants this event
                    Debug.WriteLineIf(DragDropDebug.TraceVerbose, "ToolStrip taking this because AllowDrop set to true.");  
                    newDropTarget = ((IDropTarget)owner);
                }	
                else {
					Debug.WriteLineIf(DragDropDebug.TraceVerbose, "No one wanted it.");
                    newDropTarget = null;
                }		
            }

            // if we've switched drop targets - then
            // we need to create drag enter and leave events
            if (newDropTarget != lastDropTarget) {
                Debug.WriteLineIf(DragDropDebug.TraceVerbose, "NEW DROP TARGET!");
                UpdateDropTarget(newDropTarget, e);
            }

            // now call drag over
            if (lastDropTarget != null) {
                Debug.WriteLineIf(DragDropDebug.TraceVerbose, "Calling OnDragOver on target...");     
                lastDropTarget.OnDragOver(e);
            }
				
        }

        /// <devdoc>
        /// Summary of OnDragLeave.
        /// </devdoc>
        /// <param name=e></param>	
        public void OnDragLeave(System.EventArgs e) {
            Debug.WriteLineIf(DragDropDebug.TraceVerbose, "[DRAG LEAVE] ==============");
     
            if (lastDropTarget != null) {
                Debug.WriteLineIf(DragDropDebug.TraceVerbose, "Calling OnDragLeave on current target...");
#if DEBUG
                dropTargetIsEntered=false;
#endif
                lastDropTarget.OnDragLeave(e);

            }
#if DEBUG
            else {
                Debug.Assert(!dropTargetIsEntered, "Why do we have an entered droptarget and NO lastDropTarget?");
            }
#endif
            lastDropTarget = null;
        }

        /// <devdoc>
        /// Summary of OnDragDrop.
        /// </devdoc>
        /// <param name=e></param>	
        public void OnDragDrop(DragEventArgs e) {
            Debug.WriteLineIf(DragDropDebug.TraceVerbose, "[DRAG DROP] ==============");
   
            if (lastDropTarget != null) {
                Debug.WriteLineIf(DragDropDebug.TraceVerbose, "Calling OnDragDrop on current target...");     
          
                lastDropTarget.OnDragDrop(e);
            }
            else {
                Debug.Assert(false, "Why is lastDropTarget null?");
            }
	
            lastDropTarget = null;		
        }


        /// <devdoc>
        ///     Used to actually register the control as a drop target.
        /// </devdoc>
        /// <internalonly/>
        /// <param name=accept></param>	
        private void SetAcceptDrops(bool accept) {
             if (owner.AllowDrop && accept) {
                // if the owner has set AllowDrop to true then demand Clipboard permissions.
                // else its us, and we can assert them.
                IntSecurity.ClipboardRead.Demand();
             }
                
            
             if (accept && owner.IsHandleCreated) {
                 try
                 {
                     if (Application.OleRequired() != System.Threading.ApartmentState.STA)
                     {
                         throw new ThreadStateException(SR.ThreadMustBeSTA);
                     }
                     if (accept)
                     {
                         Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, "Registering as drop target: " + owner.Handle.ToString());
                         // Register
                         int n = UnsafeNativeMethods.RegisterDragDrop(new HandleRef(owner, owner.Handle), (UnsafeNativeMethods.IOleDropTarget)(new DropTarget(this)));

                         Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, "   ret:" + n.ToString(CultureInfo.InvariantCulture));
                         if (n != 0 && n != NativeMethods.DRAGDROP_E_ALREADYREGISTERED)
                         {
                             throw new Win32Exception(n);
                         }
                     }
                     else
                     {
                         IntSecurity.ClipboardRead.Assert();
                         try
                         {
                             Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, "Revoking drop target: " + owner.Handle.ToString());

                             // Revoke
                             int n = UnsafeNativeMethods.RevokeDragDrop(new HandleRef(owner, owner.Handle));
                             Debug.WriteLineIf(CompModSwitches.DragDrop.TraceInfo, "   ret:" + n.ToString(CultureInfo.InvariantCulture));
                             if (n != 0 && n != NativeMethods.DRAGDROP_E_NOTREGISTERED)
                             {
                                 throw new Win32Exception(n);
                             }
                         }
                         finally
                         {
                             CodeAccessPermission.RevertAssert();
                         }
                     }
                 }
                 catch (Exception e)
                 {
                     throw new InvalidOperationException(SR.DragDropRegFailed, e);
                 }
            }
        }

        /// <devdoc>
        /// if we have a new active item, fire drag leave and
        /// enter.  This corresponds to the case where you 
        /// are dragging between items and havent actually
        /// left the ToolStrip's client area.
        /// </devdoc>
        /// <param name=newTarget></param>
        /// <param name=e></param>	
        private void UpdateDropTarget(IDropTarget newTarget, DragEventArgs e) {
		
            if (newTarget != lastDropTarget) {

                // tell the last drag target you've left
                if (lastDropTarget != null) {
                    OnDragLeave(new EventArgs());
                }
                lastDropTarget = newTarget;
                if (newTarget != null) {
                    DragEventArgs dragEnterArgs = new DragEventArgs(e.Data, e.KeyState, e.X, e.Y, e.AllowedEffect, e.Effect);
                    dragEnterArgs.Effect = DragDropEffects.None;

                    // tell the next drag target you've entered
                    OnDragEnter(dragEnterArgs);
                }
            }

        }

        
        
    }

}
