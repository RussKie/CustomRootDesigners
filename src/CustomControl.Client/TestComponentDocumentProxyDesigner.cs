// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// See the LICENSE file in the project root for more information.
// -------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing.Design;
using System.Windows.Forms;
using System.Windows.Forms.Design;
using CustomControl.Protocol.Notifications;
using Microsoft.DotNet.DesignTools.Client.Designers;
using Microsoft.DotNet.DesignTools.Client.Menus;
using Microsoft.DotNet.DesignTools.Client.Proxies;

namespace CustomControl.Client;

// This sample demonstrates how to provide the root designer view, or
// design mode background view, by overriding IRootDesigner.GetView().

public partial class TestComponentDocumentProxyDesigner : ComponentProxyDesigner, IToolboxUser, IContextMenuHandler
{
    private const string LinkDataCodeView = "CodeView";
    private const string LinkDataToolbox = "Toolbox";

    private IMenuCommandService? _menuCommandService;
    private IMenuCommandService? _oldMenuCommandService;
    private ClientComponentTray? _componentTray;
    private ComponentCommandSet? _commandSet;

    public TestComponentDocumentProxyDesigner()
    {
    }

    /// <summary>
    /// Determines if the tab order UI is active. When tab order is active, the tray is locked in a "read only" mode.
    /// </summary>
    private bool TabOrderActive
    {
        get
        {
            if (_menuCommandService.FindCommand(StandardCommands.TabOrder) is MenuCommand command)
            {
                return command.Checked;
            }

            return false;
        }
    }

    public override void Initialize(IComponent component)
    {
        base.Initialize(component);

        IServiceProvider serviceProvider = Component.Site;
        _menuCommandService = serviceProvider.GetRequiredService<IMenuCommandService>();

        DesignerProxy.AddHandler<TrayAddedNotification.Data>(OnTrayAddedNotification);
        DesignerProxy.AddHandler<LinkClickedNotification.Data>(OnLinkClickedNotification);


        // Setup our menu command structure.
        _commandSet = new ComponentCommandSet(component.Site);

        // Replace the IMenuCommandService with our own MenuCommandFilterService. This allows
        // interested parties to filter or provide custom menu commands, much like the behavior
        // service did via Behavior.FindCommand(...).
        _oldMenuCommandService = Host.GetRequiredService<IMenuCommandService>();
        Host.RemoveService(typeof(IMenuCommandService));

        var menuCommandFilterService = new MenuCommandFilterService(_oldMenuCommandService);
        Host.AddService(typeof(IMenuCommandService), menuCommandFilterService);
        Host.AddService(typeof(IMenuCommandFilterService), menuCommandFilterService);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            DesignerProxy.RemoveHandler<TrayAddedNotification.Data>(OnTrayAddedNotification);
            DesignerProxy.RemoveHandler<LinkClickedNotification.Data>(OnLinkClickedNotification);

            _componentTray?.Dispose();
            _commandSet?.Dispose();

            // Put back the original IMenuCommandService
            Host.RemoveService(typeof(IMenuCommandFilterService));
            Host.RemoveService(typeof(IMenuCommandService));
            if (_oldMenuCommandService is not null)
            {
                Host.AddService(typeof(IMenuCommandService), _oldMenuCommandService);
            }
        }

        base.Dispose(disposing);
    }

    private void OnLinkClickedNotification(object sender, string name, LinkClickedNotification.Data data)
    {
        var uiService = GetRequiredService<IUIService>();

        if (data.LinkText == LinkDataToolbox)
        {
            uiService.ShowToolWindow(StandardToolWindows.Toolbox);
        }
        else if (data.LinkText == LinkDataCodeView)
        {
            var eventBindingService = GetRequiredService<IEventBindingService>();
            eventBindingService.ShowCode();
        }
        else
        {
            Debug.Fail("How did we get here?");
        }
    }

    private void OnTrayAddedNotification(object sender, string name, TrayAddedNotification.Data data)
    {
        Debug.Assert(_componentTray is null);

        // This should never happen, but, just in case, dispose the previous instance
        _componentTray?.Dispose();

        _componentTray = new ClientComponentTray(this, Component.Site, (ObjectProxy)data.ComponentTray);
    }

    protected override void ShowContextMenu(int x, int y) => _componentTray?.ShowContextMenu(x, y);

    void IContextMenuHandler.ShowContextMenu(int x, int y) => ShowContextMenu(x, y);

    bool IToolboxUser.GetToolSupported(ToolboxItem tool) => true;

    void IToolboxUser.ToolPicked(ToolboxItem tool)
    {
        //using (DpiAwareness.EnterDpiScope(DpiAwarenessContext.SystemAware))
        {
            Host.Activate();

            // Find the currently selected frame designer and ask it to create the tool.
            try
            {
                base.CreateToolWithMouse(tool, location: null, size: null, toolboxSnapArgs: null);
                ToolboxService.SelectedToolboxItemUsed();
            }
            catch (Exception ex)
            {
                DisplayError(ex);
            }
        }
    }

}
