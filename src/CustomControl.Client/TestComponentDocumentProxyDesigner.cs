// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// See the LICENSE file in the project root for more information.
// -------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using CustomControl.Protocol.Notifications;
using Microsoft.DotNet.DesignTools.Client.Designers;

namespace CustomControl.Client;

// This sample demonstrates how to provide the root designer view, or
// design mode background view, by overriding IRootDesigner.GetView().

public partial class TestComponentDocumentProxyDesigner : ComponentProxyDesigner, IToolboxUser
{
    private const string LinkDataCodeView = "CodeView";
    private const string LinkDataToolbox = "Toolbox";

    public override void Initialize(IComponent component)
    {
        base.Initialize(component);

        DesignerProxy.AddHandler<LinkClickedNotification.Data>(OnLinkClickedNotification);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            DesignerProxy.RemoveHandler<LinkClickedNotification.Data>(OnLinkClickedNotification);
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
