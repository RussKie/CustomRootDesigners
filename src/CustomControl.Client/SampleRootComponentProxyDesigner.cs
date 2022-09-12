// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// See the LICENSE file in the project root for more information.
// -------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using Microsoft.DotNet.DesignTools.Client.Designers;

namespace CustomControl.Client
{
    // This sample demonstrates how to provide the root designer view, or
    // design mode background view, by overriding IRootDesigner.GetView().

    public class SampleRootComponentProxyDesigner : ParentControlProxyDesigner, IRootDesigner
    {
        private DesignerView _designerView;

        public SampleRootComponentProxyDesigner()
        {
        }

        public override void Initialize(IComponent component)
        {
            base.Initialize(component);

            // Create the view for this component. We first create the designer frame so we can provide
            // the overlay and the split window services, and then later on we initialize the frame with
            // the designer view.
            _designerView = new DesignerView(component.Site);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _designerView?.Dispose();
                _designerView = null;
            }

            base.Dispose(disposing);
        }

        ViewTechnology[] IRootDesigner.SupportedTechnologies
            => new[] { ViewTechnology.Default };

        object IRootDesigner.GetView(ViewTechnology technology)
        {
            if (technology != ViewTechnology.Default)
            {
                throw new ArgumentException(nameof(technology));
            }

            return _designerView;
        }
    }
}
