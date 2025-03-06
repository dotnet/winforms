// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.ComponentModel.Design.Serialization;
using System.Drawing;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using Xunit.Abstractions;

namespace System.Windows.Forms.UITests;

public class DesignBehaviorsTests : ControlTestBase
{
    public DesignBehaviorsTests(ITestOutputHelper testOutputHelper)
        : base(testOutputHelper)
    {
    }

    [WinFormsFact]
    public async Task DesignBehaviorsTests_can_DragDrop_ToolboxItem()
    {
        // Regression test for https://github.com/dotnet/winforms/issues/6919, and it verifies that we can successfully
        // drag and drop a toolbox item from a toolbox on to a designer surface.

        Application.ThreadException += (s, e) =>
        {
            // This will preserve the full stack, which otherwise gets replaced
            throw new InvalidOperationException(e.Exception.Message, e.Exception);
        };

        await RunSingleControlTestAsync<TreeView>(async (form, treeView) =>
        {
            // This will indicate when the designer surface has loaded
            SemaphoreSlim loadSignal = new(0);
            // This will indicate when the drag and drop operation has completed
            SemaphoreSlim dndSignal = new(0);

            form.Size = new(600, 550);
            treeView.Dock = DockStyle.Left;
            treeView.Width = 100;

            // Create a toolbox item and assign it to the node
            // When we drag the node to the design surface, this will go through the toolbox service API
            ToolboxItem toolboxItem = new()
            {
                TypeName = typeof(SampleControl).FullName,
                DisplayName = nameof(SampleControl),
                AssemblyName = typeof(SampleControl).Assembly.GetName()
            };

            TreeNode? node = treeView.Nodes.Add("root");
            node.Tag = toolboxItem;

            // Create the designer surface and all required plumbing
            SampleDesignerLoader designerLoader = new();
            ServiceContainer serviceContainer = new();
            DesignSurface designSurface = new(serviceContainer);

            var designerHost = (IDesignerHost)designSurface.GetService(typeof(IDesignerHost))!;
            serviceContainer.RemoveService(typeof(IToolboxService), false);
            serviceContainer.AddService(typeof(IToolboxService), new SampleToolboxService(designerHost));

            if (designerHost.GetService(typeof(IComponentChangeService)) is IComponentChangeService componentChangeService)
            {
                componentChangeService.ComponentAdded += (s, e) =>
                {
                    if (e.Component is Form form)
                    {
                        form.Text = @"RootForm";
                        form.Size = new Size(400, 400);
                    }
                    else
                    {
                        ((Control)e.Component!).Size = new Size(50, 50);
                    }
                };
            }

            designSurface.Loaded += (s, e) =>
            {
                Control rootView = (Control)designSurface.View;
                rootView.Dock = DockStyle.Fill;
                form.Controls.Add(rootView);
                rootView.BringToFront();

                // Indicate that we have finished loading the designer surface
                loadSignal.Release();
            };

            treeView.ItemDrag += treeView_ItemDrag;

            designSurface.BeginLoad(designerLoader);

            // Wait for the designer surface to load
            await loadSignal.WaitAsync(millisecondsTimeout: 3_000);

            // Initiate the drag and drop and wait for the signal that it finished
            var dndStartCoordinates = treeView.PointToScreen(node.Bounds.Location);
            await InitiateDrangDropAsync(form, dndStartCoordinates, (Control)designSurface.View);
            await dndSignal.WaitAsync(millisecondsTimeout: 3_000);

            return;

            void treeView_ItemDrag(object? sender, ItemDragEventArgs e)
            {
                if (designerHost?.GetService(typeof(IToolboxService)) is not IToolboxService toolboxService ||
                    e.Item is not TreeNode node ||
                    node.Tag is not ToolboxItem toolboxItem)
                {
                    return;
                }

                var dataObject = toolboxService.SerializeToolboxItem(toolboxItem) as DataObject;

                var effects = node.TreeView!.DoDragDrop(dataObject!, DragDropEffects.Copy);
            }

            async Task InitiateDrangDropAsync(Form form, Point startCoordinates, Control rootView)
            {
                var virtualPointStart = ToVirtualPoint(startCoordinates);
                startCoordinates.Offset(110, 50);
                var virtualPointEnd = ToVirtualPoint(startCoordinates);

                await InputSimulator.SendAsync(
                    form,
                    inputSimulator => inputSimulator.Mouse.MoveMouseTo(virtualPointStart.X + 6, virtualPointStart.Y + 6)
                                                          .LeftButtonDown()
                                                          .MoveMouseTo(virtualPointEnd.X, virtualPointEnd.Y)
                                                          .LeftButtonUp());

                dndSignal.Release();
            }
        });
    }

#pragma warning disable CS8603 // Possible null reference return.
    internal class SampleToolboxService : IToolboxService
    {
        private readonly IDesignerHost _designerHost;

        public SampleToolboxService(IDesignerHost designerHost)
        {
            _designerHost = designerHost;
        }

        public void AddCreator(ToolboxItemCreatorCallback creator, string format)
        {
            // this method intentionally keep empty
        }

        public void AddCreator(ToolboxItemCreatorCallback creator, string format, IDesignerHost host)
        {
            // this method intentionally keep empty
        }

        public void AddLinkedToolboxItem(ToolboxItem toolboxItem, IDesignerHost host)
        {
            // this method intentionally keep empty
        }

        public void AddLinkedToolboxItem(ToolboxItem toolboxItem, string category, IDesignerHost host)
        {
            // this method intentionally keep empty
        }

        public void AddToolboxItem(ToolboxItem toolboxItem)
        {
            // this method intentionally keep empty
        }

        public void AddToolboxItem(ToolboxItem toolboxItem, string category)
        {
            // this method intentionally keep empty
        }

        public ToolboxItem DeserializeToolboxItem(object serializedObject)
        {
            return DeserializeToolboxItem(serializedObject, _designerHost);
        }

        public ToolboxItem DeserializeToolboxItem(object serializedObject, IDesignerHost? host)
        {
#pragma warning disable WFDEV005 // Type or member is obsolete
            ToolboxItem? item = ((DataObject)serializedObject)?.GetData(typeof(ToolboxItem)) as ToolboxItem;
#pragma warning restore WFDEV005
            return item!;
        }

        public ToolboxItem GetSelectedToolboxItem()
        {
            return GetSelectedToolboxItem(null!);
        }

        public ToolboxItem GetSelectedToolboxItem(IDesignerHost host)
        {
            return null;
        }

        public ToolboxItemCollection GetToolboxItems()
        {
            return null;
        }

        public ToolboxItemCollection GetToolboxItems(IDesignerHost host)
        {
            return null;
        }

        public ToolboxItemCollection GetToolboxItems(string category)
        {
            return null;
        }

        public ToolboxItemCollection GetToolboxItems(string category, IDesignerHost host)
        {
            return null;
        }

        public bool IsSupported(object serializedObject, IDesignerHost host)
        {
            return true;
        }

        public bool IsSupported(object serializedObject, ICollection filterAttributes)
        {
            return true;
        }

        public bool IsToolboxItem(object serializedObject)
        {
            return true;
        }

        public bool IsToolboxItem(object serializedObject, IDesignerHost host)
        {
            return true;
        }

        public void Refresh()
        {
            throw new NotImplementedException();
        }

        public void RemoveCreator(string format)
        {
            // this method intentionally keep empty
        }

        public void RemoveCreator(string format, IDesignerHost host)
        {
            // this method intentionally keep empty
        }

        public void RemoveToolboxItem(ToolboxItem toolboxItem)
        {
            // this method intentionally keep empty
        }

        public void RemoveToolboxItem(ToolboxItem toolboxItem, string category)
        {
            // this method intentionally keep empty
        }

        public void SelectedToolboxItemUsed()
        {
            // this method intentionally keep empty
        }

        public object SerializeToolboxItem(ToolboxItem toolboxItem)
        {
            return new DataObject(toolboxItem);
        }

        public bool SetCursor()
        {
            return false;
        }

        public void SetSelectedToolboxItem(ToolboxItem toolboxItem)
        {
            // this method intentionally keep empty
        }

        public CategoryNameCollection CategoryNames => null;

        public string SelectedCategory
        {
            get => null;
            set { }
        }
    }

    internal class SampleDesignerLoader : BasicDesignerLoader
    {
        protected override void PerformFlush(IDesignerSerializationManager serializationManager)
        {
            throw new NotImplementedException();
        }

        protected override void PerformLoad(IDesignerSerializationManager serializationManager)
        {
            if (LoaderHost is null)
                return;

            ArrayList errors = [];

            LoaderHost.CreateComponent(typeof(Form));

            LoaderHost.EndLoad(nameof(Form), true, errors);
        }
    }

    [Designer(typeof(SampleControlDesigner))]
    public class SampleControl : Control
    {
    }

    internal class SampleControlDesigner : ControlDesigner
    {
        protected override void OnPaintAdornments(PaintEventArgs pe)
        {
            base.OnPaintAdornments(pe);

            pe.Graphics.DrawString($"Design time \n{Control.Site?.Name} !", Control.Font, SystemBrushes.WindowText, new PointF(12, 12));
        }
    }
}
#pragma warning restore CS8603

