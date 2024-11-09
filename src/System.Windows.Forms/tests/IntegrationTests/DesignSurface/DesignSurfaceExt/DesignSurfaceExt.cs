// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.ComponentModel.Design;
using System.Drawing;
using System.ComponentModel;
using System.Windows.Forms;
using System.ComponentModel.Design.Serialization;
using System.Windows.Forms.Design;

namespace DesignSurfaceExt;

public class DesignSurfaceExt : DesignSurface, IDesignSurfaceExt
{
    private const string Name = "DesignSurfaceExt";

    #region IDesignSurfaceExt Members

    public void SwitchTabOrder()
    {
        if (!IsTabOrderMode)
        {
            InvokeTabOrder();
        }
        else
        {
            DisposeTabOrder();
        }
    }

    public void UseSnapLines()
    {
        IServiceContainer serviceProvider = GetService(typeof(IServiceContainer)) as IServiceContainer;
        DesignerOptionService opsService = serviceProvider.GetService(typeof(DesignerOptionService)) as DesignerOptionService;
        if (opsService is not null)
        {
            serviceProvider.RemoveService(typeof(DesignerOptionService));
        }

        DesignerOptionService opsService2 = new DesignerOptionServiceExt4SnapLines();
        serviceProvider.AddService(typeof(DesignerOptionService), opsService2);
    }

    public void UseGrid(Size gridSize)
    {
        IServiceContainer serviceProvider = GetService(typeof(IServiceContainer)) as IServiceContainer;
        DesignerOptionService opsService = serviceProvider.GetService(typeof(DesignerOptionService)) as DesignerOptionService;
        if (opsService is not null)
        {
            serviceProvider.RemoveService(typeof(DesignerOptionService));
        }

        DesignerOptionService opsService2 = new DesignerOptionServiceExt4Grid(gridSize);
        serviceProvider.AddService(typeof(DesignerOptionService), opsService2);
    }

    public void UseGridWithoutSnapping(Size gridSize)
    {
        IServiceContainer serviceProvider = GetService(typeof(IServiceContainer)) as IServiceContainer;
        DesignerOptionService opsService = serviceProvider.GetService(typeof(DesignerOptionService)) as DesignerOptionService;
        if (opsService is not null)
        {
            serviceProvider.RemoveService(typeof(DesignerOptionService));
        }

        DesignerOptionService opsService2 = new DesignerOptionServiceExt4GridWithoutSnapping(gridSize);
        serviceProvider.AddService(typeof(DesignerOptionService), opsService2);
    }

    public void UseNoGuides()
    {
        IServiceContainer serviceProvider = GetService(typeof(IServiceContainer)) as IServiceContainer;
        DesignerOptionService opsService = serviceProvider.GetService(typeof(DesignerOptionService)) as DesignerOptionService;
        if (opsService is not null)
        {
            serviceProvider.RemoveService(typeof(DesignerOptionService));
        }

        DesignerOptionService opsService2 = new DesignerOptionServiceExt4NoGuides();
        serviceProvider.AddService(typeof(DesignerOptionService), opsService2);
    }

    public UndoEngineExt GetUndoEngineExt()
    {
        return _undoEngine;
    }

    public TControl CreateRootComponent<TControl>(Size controlSize)
        where TControl : Control, IComponent
    {
        try
        {
            // - step.1
            // - get the IDesignerHost
            // - if we are not not able to get it
            // - then rollback (return without do nothing)
            IDesignerHost host = GetIDesignerHost();
            if (host is null)
                return null;
            // - check if the root component has already been set
            // - if so then rollback (return without do nothing)
            if (host.RootComponent is not null)
                return null;
            // -
            // -
            // - step.2
            // - create a new root component and initialize it via its designer
            // - if the component has not a designer
            // - then rollback (return without do nothing)
            // - else do the initialization
            BeginLoad(typeof(TControl));
            if (LoadErrors.Count > 0)
                throw new InvalidOperationException($"the BeginLoad() failed! Some error during {typeof(TControl).FullName} loding");
            // -
            // -
            // - step.3
            // - try to modify the Size of the object just created
            IDesignerHost ihost = GetIDesignerHost();
            // - Set the BackColor and the Size
            Control ctrl = null;
            Type hostType = host.RootComponent.GetType();
            if (hostType == typeof(Form))
            {
                ctrl = View as Control;
                ctrl.BackColor = Color.LightGray;
                // - set the Size
                PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(ctrl);
                // - Sets a PropertyDescriptor to the specific property.
                PropertyDescriptor pdS = pdc.Find("Size", false);
                pdS?.SetValue(ihost.RootComponent, controlSize);
            }
            else if (hostType == typeof(UserControl))
            {
                ctrl = View as Control;
                ctrl.BackColor = Color.DarkGray;
                // - set the Size
                PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(ctrl);
                // - Sets a PropertyDescriptor to the specific property.
                PropertyDescriptor pdS = pdc.Find("Size", false);
                pdS?.SetValue(ihost.RootComponent, controlSize);
            }
            else if (hostType == typeof(Component))
            {
                ctrl = View as Control;
                ctrl.BackColor = Color.White;
                // - don't set the Size
            }
            else
            {
                throw new InvalidOperationException($"Undefined Host Type: {hostType}");
            }

            return (TControl)ihost.RootComponent;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"{Name}::CreateRootComponent() - Exception: (see Inner Exception)", ex);
        }
    }

    public TControl CreateControl<TControl>(Size controlSize, Point controlLocation)
        where TControl : Control
    {
        try
        {
            // - step.1
            IComponent newComp = CreateComponent<TControl>(out IDesignerHost host);
            if (newComp is null)
                return null;

            // -
            // -
            // - step.3
            // - try to modify the Size/Location of the object just created
            PropertyDescriptorCollection pdc = TypeDescriptor.GetProperties(newComp);
            // - Sets a PropertyDescriptor to the specific property.
            PropertyDescriptor pdS = pdc.Find("Size", false);
            pdS?.SetValue(newComp, controlSize);
            PropertyDescriptor pdL = pdc.Find("Location", false);
            pdL?.SetValue(newComp, controlLocation);
            // -
            // -
            // - step.4
            // - commit the Creation Operation
            // - adding the control to the DesignSurface's root component
            // - and return the control just created to let further initializations
            ((Control)newComp).Parent = host.RootComponent as Control;

            return (TControl)newComp;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"{Name}::CreateControl() - Exception: (see Inner Exception)", ex);
        }
    }

    public TComponent CreateComponent<TComponent>()
        where TComponent : IComponent
    {
        return CreateComponent<TComponent>(out _);
    }

    private TComponent CreateComponent<TComponent>(out IDesignerHost host)
        where TComponent : IComponent
    {
        try
        {
            // Create a new component and initialize it via its designer

            host = GetIDesignerHost();
            if (host is null
                || host.RootComponent is null
                || host.CreateComponent(typeof(TComponent)) is not IComponent newComp
                || host.GetDesigner(newComp) is not IDesigner designer)
            {
                return default;
            }

            if (designer is IComponentInitializer initializer)
            {
                initializer.InitializeNewComponent(null);
            }

            return (TComponent)newComp;
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"{Name}::{nameof(CreateComponent)} - Exception: (see Inner Exception)", ex);
        }
    }

    public IDesignerHost GetIDesignerHost() => (IDesignerHost)GetService(typeof(IDesignerHost));

    public Control GetView()
    {
        Control ctrl = View as Control;
        ctrl.Dock = DockStyle.Fill;
        return ctrl;
    }

    #endregion

    #region TabOrder

    private TabOrderHooker _tabOrder;
    private bool _tabOrderMode;

    public bool IsTabOrderMode
    {
        get { return _tabOrderMode; }
    }

    public TabOrderHooker TabOrder
    {
        get
        {
            _tabOrder ??= new TabOrderHooker();
            return _tabOrder;
        }
        set { _tabOrder = value; }
    }

    public void InvokeTabOrder()
    {
        TabOrder.HookTabOrder(GetIDesignerHost());
        _tabOrderMode = true;
    }

    public void DisposeTabOrder()
    {
        TabOrder.HookTabOrder(GetIDesignerHost());
        _tabOrderMode = false;
    }
    #endregion

    #region  UndoEngine

    private UndoEngineExt _undoEngine;
    private NameCreationServiceImp _nameCreationService;
    private DesignerSerializationServiceImpl _designerSerializationService;
    private CodeDomComponentSerializationService _codeDomComponentSerializationService;

    #endregion

    // - ctor
    public DesignSurfaceExt()
    {
        InitServices();
    }

    // - The DesignSurface class provides several design-time services automatically.
    // - The DesignSurface class adds all of its services in its constructor.
    // - Most of these services can be overridden by replacing them in the
    // - protected ServiceContainer property.To replace a service, override the constructor,
    // - call base, and make any changes through the protected ServiceContainer property.
    private void InitServices()
    {
        // - each DesignSurface has its own default services
        // - We can leave the default services in their present state,
        // - or we can remove them and replace them with our own.
        // - Now add our own services using IServiceContainer
        // - Note
        // - before loading the root control in the design surface
        // - we must add an instance of naming service to the service container.
        // - otherwise the root component did not have a name and this caused
        // - troubles when we try to use the UndoEngine
        // - 1. NameCreationService
        _nameCreationService = new NameCreationServiceImp();
        ServiceContainer.RemoveService(typeof(INameCreationService), false);
        ServiceContainer.AddService(typeof(INameCreationService), _nameCreationService);

        // - 2. CodeDomComponentSerializationService
        _codeDomComponentSerializationService = new CodeDomComponentSerializationService(ServiceContainer);
        ServiceContainer.RemoveService(typeof(ComponentSerializationService), false);
        ServiceContainer.AddService(typeof(ComponentSerializationService), _codeDomComponentSerializationService);

        // - 3. IDesignerSerializationService
        _designerSerializationService = new DesignerSerializationServiceImpl(ServiceContainer);
        ServiceContainer.RemoveService(typeof(IDesignerSerializationService), false);
        ServiceContainer.AddService(typeof(IDesignerSerializationService), _designerSerializationService);

        // - 4. UndoEngine
        _undoEngine = new UndoEngineExt(ServiceContainer)
        {
            // Disable the UndoEngine
            Enabled = false
        };

        ServiceContainer.RemoveService(typeof(UndoEngine), false);
        ServiceContainer.AddService(typeof(UndoEngine), _undoEngine);

        // - 5. IMenuCommandService
        ServiceContainer.AddService(typeof(IMenuCommandService), new MenuCommandService(this));

        // - 6. ITypeDiscoveryService
        ServiceContainer.AddService(typeof(ITypeDiscoveryService), new TypeDiscoveryService());
    }

    // - do some Edit menu command using the MenuCommandService
    public void DoAction(string command)
    {
        if (string.IsNullOrEmpty(command))
        {
            return;
        }

        IMenuCommandService ims = GetService(typeof(IMenuCommandService)) as IMenuCommandService;

        try
        {
            CommandID commandId = command.ToUpperInvariant() switch
            {
                "CUT" => StandardCommands.Cut,
                "COPY" => StandardCommands.Copy,
                "PASTE" => StandardCommands.Paste,
                "DELETE" => StandardCommands.Delete,
                "INVOKESMARTTAG" => MenuCommands.KeyInvokeSmartTag,
                _ => null,
            };

            if (commandId is not null)
            {
                MenuCommand menuCommand = ims?.FindCommand(commandId);
                if (menuCommand?.Enabled is true)
                {
                    menuCommand.Invoke();
                }
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"{Name}::DoAction() - Exception: error in performing the action: {command}(see Inner Exception)", ex);
        }
    }
}
