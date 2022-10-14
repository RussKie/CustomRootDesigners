// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// See the LICENSE file in the project root for more information.
// -------------------------------------------------------------------

using System.ComponentModel;
using System.Diagnostics;
using Microsoft.DotNet.DesignTools.Designers;

namespace CustomControl.Designers;

public partial class TestComponentDocumentDesigner
{
    private class ComponentCommandSet : Microsoft.DotNet.DesignTools.Commands.CommandSet
    {
        private readonly CustomRootDesignerView _compositionUI;

        public ComponentCommandSet(CustomRootDesignerView compositionUI, IServiceProvider serviceProvider)
             : base(serviceProvider)
        {
            _compositionUI = compositionUI;
        }

        protected override IComponent? GetSibling(SelectionTarget target, IComponent? selectedComponent, IComponent[]? trayComponents, IComponent rootComponent)
            => target switch
            {
                SelectionTarget.NextSibling => RotateTabSelection(selectedComponent, rootComponent, backwards: false),
                SelectionTarget.PreviousSibling => RotateTabSelection(selectedComponent, rootComponent, backwards: true),
                _ => base.GetSibling(target, selectedComponent, trayComponents, rootComponent)
            };

        /// <summary>
        ///  Rotates the selection to the element next in the tab index. If backwards is set, this will rotate to the
        ///  previous tab index.
        /// </summary>
        private IComponent? RotateTabSelection(IComponent? selectedComponent, IComponent rootControl, bool backwards)
        {
            IComponent currentComponent = selectedComponent ?? rootControl;
            ComponentTray.TrayControl? nextControl = null;

            // If we have a selected component, get the composition UI for it and find the next control on the UI.
            // Otherwise, we just select the first control on the UI.
            Control? currentControl = ComponentTray.TrayControl.FromComponent(currentComponent);
            if (currentControl is not null)
            {
                Debug.Assert(_compositionUI.Controls[0] is LinkLabel, "The first item in the Composition designer is not a linklabel");

                for (int i = 1; i < _compositionUI.Controls.Count; i++)
                {
                    if (_compositionUI.Controls[i] != currentControl)
                    {
                        continue;
                    }

                    int next;
                    if (!backwards)
                    {
                        next = i + 1;
                        if (next >= _compositionUI.Controls.Count)
                        {
                            next = 1;
                        }
                    }
                    else
                    {
                        next = i - 1;
                        if (next <= 0)
                        {
                            next = _compositionUI.Controls.Count - 1;
                        }
                    }

                    var trayControl = _compositionUI.Controls[next] as ComponentTray.TrayControl;
                    if (trayControl is not null)
                    {
                        nextControl = trayControl;
                        break;
                    }
                    else
                    {
                        continue;
                    }
                }
            }
            else if (_compositionUI.Controls.Count > 1)
            {
                Debug.Assert(_compositionUI.Controls[0] is LinkLabel, "The first item in the Composition designer is not a linklabel");

                var tc = _compositionUI.Controls[1] as ComponentTray.TrayControl;
                if (tc is not null)
                {
                    nextControl = tc;
                }
            }

            return nextControl?.Component;
        }
    }

}
