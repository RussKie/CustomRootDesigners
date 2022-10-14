// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// See the LICENSE file in the project root for more information.
// -------------------------------------------------------------------

using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using CustomControl.Protocol.Notifications;
using Microsoft.DotNet.DesignTools.Designers;
using Microsoft.DotNet.DesignTools.Services;

namespace CustomControl.Designers;

public partial class TestComponentDocumentDesigner : ComponentDesigner, IRootDesigner, ITypeDescriptorFilterService
{
    private IRemoteToolboxService? _lazyToolboxService;
    private ITypeDescriptorFilterService? _delegateFilterService;
    private InheritanceService? _inheritanceService;
    private DesignerFrame? _frame;

    // Our set of menu commands
    private ComponentCommandSet? _commandSet;

    // Member field of custom type RootDesignerView, a control that 
    // will be shown in the Forms designer view. This member is 
    // cached to reduce processing needed to recreate the 
    // view control on each call to GetView().
    private CustomRootDesignerView? _compositionUI;

    private bool _largeIcons = false;
    private bool _autoArrange = true;

    private CustomRootDesignerView CompositionUIControl
    {
        get
        {
            Debug.Assert(_compositionUI is not null, "UI must be created by now.");
            return _compositionUI!;
        }
    }
    internal IRemoteToolboxService ToolboxService
        => GetCachedService(ref _lazyToolboxService);


    public bool TrayAutoArrange
    {
        get => _autoArrange;
        set
        {
            _autoArrange = value;
            CompositionUIControl.AutoArrange = value;
        }
    }

    /// <summary>
    ///  Indicates whether the tray should contain a large icon.
    /// </summary>
    public bool TrayLargeIcon
    {
        get => _largeIcons;
        set
        {
            _largeIcons = value;
            CompositionUIControl.ShowLargeIcons = value;
        }
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ComponentChangeService.ComponentAdded -= OnComponentAdded;
            ComponentChangeService.ComponentRemoved -= OnComponentRemoved;

            _commandSet?.Dispose();
            _commandSet = null;

            DisposeDesignerFrame();

            _compositionUI?.Dispose();
            _compositionUI = null;

            SendObjectNotification(
                name: NotificationNames.ComponentTrays.TrayRemoved,
                obj: this,
                data: TrayRemovedNotification.Data.Empty);

            Host.RemoveService(typeof(IInputDispatchProvider));
            Host.RemoveService(typeof(ComponentTray));
            Host.RemoveService(typeof(Microsoft.DotNet.DesignTools.Commands.CommandSet));

            // Restore the original service
            Host.RemoveService(typeof(ITypeDescriptorFilterService));
            if (_delegateFilterService is not null)
            {
                Host.AddService(typeof(ITypeDescriptorFilterService), _delegateFilterService);
            }
        }

        base.Dispose(disposing);
    }

    public override void Initialize(IComponent component)
    {
        base.Initialize(component);

        // Create the view for this component. We first create the designer frame so we can provide
        // the overlay and split window services, and then later on we initilaize the frame with
        // the designer view.
        _frame = InitializeDesignerFrame();

        _inheritanceService = new InheritanceService();
        Host.AddService(typeof(IInheritanceService), _inheritanceService);

        // Hook up our inheritance service and do a scan for inherited components.
        _inheritanceService.AddInheritedComponents(component, component.Site!.Container!);

        _compositionUI = new CustomRootDesignerView(this, Component.Site!);

        Host.AddService(typeof(IInputDispatchProvider), _compositionUI);
        Host.AddService(typeof(ComponentTray), _compositionUI);

        SendObjectNotification(
            name: NotificationNames.ComponentTrays.TrayAdded,
            obj: this,
            data: new TrayAddedNotification.Data((Control)_compositionUI));

        // And component add and remove events for our tray
        ComponentChangeService.ComponentAdded += OnComponentAdded;
        ComponentChangeService.ComponentRemoved += OnComponentRemoved;

        // Listen for the completed load.  When finished, we need to select the form.  We don't
        // want to do it before we're done, however, or else the dimensions of the selection rectangle
        // could be off because during load, change events are not fired.

        Host.LoadComplete += OnLoadComplete;

        // Setup our menu command structure.

        _commandSet = new ComponentCommandSet(_compositionUI, component.Site);
        Host.AddService(typeof(Microsoft.DotNet.DesignTools.Commands.CommandSet), _commandSet);

        // Hook up to the ITypeDescriptorFilterService so we can hide the location property on all components
        // being added to the designer.
        if (Host.TryGetService(out _delegateFilterService))
        {
            Host.RemoveService(typeof(ITypeDescriptorFilterService));
        }

        Host.AddService(typeof(ITypeDescriptorFilterService), this);

        // Finally hook the designer view into the frame.  We do this last because the frame may
        // cause the control to be created, and if this happens before the inheritance scan we
        // will subclass the inherited controls before we get a chance to attach designers.  

        _frame.Initialize(_compositionUI);

        // And force some shadow properties that we change in the course of initializing the form.
        _compositionUI.Location = Point.Empty;
    }

    private void OnComponentAdded(object? sender, ComponentEventArgs ce)
    {
        if (ce.Component != Component)
        {
            CompositionUIControl.AddComponent(ce.Component!);
        }
    }

    private void OnComponentRemoved(object? sender, ComponentEventArgs ce)
    {
        CompositionUIControl.RemoveComponent(ce.Component!);
    }

    private void OnLoadComplete(object? sender, EventArgs e)
    {
        // Remove the handler; we're done looking at this.
        Host.LoadComplete -= OnLoadComplete;

        // Restore the tray layout.
        _compositionUI?.ResumeLayout();

        // Select this component.
        SelectionService.SetSelectedComponent(Component, SelectionTypes.Replace);
        Debug.Assert(
            SelectionService.PrimarySelection == Component,
            "Form should have primary selection.");
    }

    /// <summary>
    ///  Allows a designer to filter the set of properties the component it is designing will expose through the
    ///  TypeDescriptor object.
    /// </summary>
    protected override void PreFilterProperties(IDictionary properties)
    {
        // This method is filtering properties of the root component. Filtering of child components' properties is
        // handled by ITypeDescriptorFilterService implementations.

        base.PreFilterProperties(properties);

        properties[nameof(TrayLargeIcon)] = TypeDescriptor.CreateProperty(
            typeof(TestComponentDocumentDesigner), nameof(TrayLargeIcon), typeof(bool),
            BrowsableAttribute.No,
            DesignOnlyAttribute.Yes,
            CategoryAttribute.Design);
    }

    // This method returns an instance of the view for this root
    // designer. The "view" is the user interface that is presented
    // in a document window for the user to manipulate. 
    object IRootDesigner.GetView(ViewTechnology technology)
    {
        if (technology != ViewTechnology.Default)
        {
            throw new ArgumentException("Not a supported view technology", nameof(technology));
        }

        return _frame!;
    }

    // IRootDesigner.SupportedTechnologies is a required override for an
    // IRootDesigner. Default is the view technology used by this designer.
    ViewTechnology[] IRootDesigner.SupportedTechnologies
        => new ViewTechnology[] { ViewTechnology.Default };

    bool ITypeDescriptorFilterService.FilterAttributes(IComponent component, IDictionary attributes)
        => _delegateFilterService?.FilterAttributes(component, attributes) ?? true;

    bool ITypeDescriptorFilterService.FilterEvents(IComponent component, IDictionary events)
        => _delegateFilterService?.FilterEvents(component, events) ?? true;

    bool ITypeDescriptorFilterService.FilterProperties(IComponent component, IDictionary properties)
    {
        // This method is filtering properties of the child components. Filtering of root component's properties is
        // handled by PreFilterProperties method.

        _delegateFilterService?.FilterProperties(component, properties);

        // remove the "(Location)" property from the property grid.
        var property = (PropertyDescriptor?)properties[nameof(Control.Location)];
        if (property is not null)
        {
            properties[nameof(Control.Location)] = TypeDescriptor.CreateProperty(
                property.ComponentType, property,
                BrowsableAttribute.No,
                DesignerSerializationVisibilityAttribute.Hidden);
        }

        return true;
    }
}
