// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// See the LICENSE file in the project root for more information.
// -------------------------------------------------------------------

using System.ComponentModel;
using CustomControl.Protocol.Notifications;
using Microsoft.DotNet.DesignTools.Designers;

namespace CustomControl.Designers;

public partial class TestComponentDocumentDesigner
{
    // RootDesignerView is a simple control that will be displayed 
    // in the designer window.
    private partial class CustomRootDesignerView : ComponentTray, IInputDispatchProvider
    {
        private readonly TestComponentDocumentDesigner _designer;
        private readonly WatermarkLabel _watermark;
        private readonly CustomDispatcher _dispatcher;

        public CustomRootDesignerView(TestComponentDocumentDesigner compositionDesigner, IServiceProvider serviceProvider)
            : base(compositionDesigner, serviceProvider)
        {
            _designer = compositionDesigner;

            DoubleBuffered = true;
        
            Name = nameof(CustomRootDesignerView);
            Text = nameof(CustomRootDesignerView);
            Dock = DockStyle.Fill;
            Padding = new(16);

            _watermark = new(this);
            Controls.Add(_watermark);

            _dispatcher = new CustomDispatcher(this, _watermark);
        }

        public IInputDispatcher InputDispatcher => _dispatcher;

        /// <summary>
        ///  Adds a component to the tray.
        /// </summary>
        public override void AddComponent(IComponent component)
        {
            base.AddComponent(component);

            if (Controls.Count > 1)
            {
                _watermark.Visible = false;
            }
        }

        private void OnLinkClick(object sender, LinkLabelLinkClickedEventArgs e)
        {
            string linkData = (string)e.Link.LinkData;
            _designer.SendObjectNotification(
                _designer,
                new LinkClickedNotification.Data(linkData));
        }

        /// <summary>
        ///  Removes a component from the tray.
        /// </summary>
        public override void RemoveComponent(IComponent component)
        {
            base.RemoveComponent(component);
            if (Controls.Count == 1)
            {
                _watermark.Visible = true;
            }
        }
    }
}
