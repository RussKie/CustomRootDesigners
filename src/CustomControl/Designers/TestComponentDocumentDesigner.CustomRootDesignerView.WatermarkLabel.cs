// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// See the LICENSE file in the project root for more information.
// -------------------------------------------------------------------

using Microsoft.DotNet.DesignTools.Designers;
using Microsoft.DotNet.DesignTools.Services;

namespace CustomControl.Designers;

public partial class TestComponentDocumentDesigner
{
    private partial class CustomRootDesignerView
    {
        private partial class WatermarkLabel : LinkLabel, IInputDispatchProvider
        {
            private const string LinkDataCodeView = "CodeView";
            private const string LinkDataToolbox = "Toolbox";
            private const string CompositionDesignerWaterMark = "Guess what? It's possible to implement your own ComponentDocumentDesigner.\r\n\r\nYou can too drag items from the {0} and use the Properties window to set their properties. To create methods and events for your class, {1}.";
            private const string CompositionDesignerWaterMark_FirstLink = "Toolbox";
            private const string CompositionDesignerWaterMark_SecondLink = "switch to code view";
      
            private readonly Dispatcher _dispatcher;
            private readonly CustomRootDesignerView _owner;

            public WatermarkLabel(CustomRootDesignerView owner)
            {
                _owner = owner;
                _dispatcher = new(this, owner.GetDispatcher());

                Dock = DockStyle.Fill;
                Font = new(Font.FontFamily, 11);
                LinkBehavior = LinkBehavior.HoverUnderline;
                TabStop = false;
                TextAlign = ContentAlignment.MiddleCenter;

                var uiService = owner.GetRequiredService<IUIService>();
                if (uiService.Styles["VsColorPanelHyperLink"] is Color linkColor)
                {
                    LinkColor = linkColor;
                }

                Text = string.Format(CompositionDesignerWaterMark, CompositionDesignerWaterMark_FirstLink, CompositionDesignerWaterMark_SecondLink);

                string link = CompositionDesignerWaterMark_FirstLink;
                Links.Add(Text.IndexOf(link) - 2, link.Length, LinkDataToolbox);

                link = CompositionDesignerWaterMark_SecondLink;
                Links.Add(Text.IndexOf(link) - 2, link.Length, LinkDataCodeView);

                LinkClicked += owner.OnLinkClick;

                // HACK: Set the initial cursor to the default, as often something in the designer infra resets the cursor
                // to WaitCursor after the designer surface was created but before it received the first input message.
                // This allows us to start with the right cursor from get go.
                OverrideCursor = Cursors.Default;
            }

            public IInputDispatcher InputDispatcher => _dispatcher;

            protected override void Dispose(bool disposing)
            {
                if (disposing)
                {
                    LinkClicked -= _owner.OnLinkClick;
                }

                base.Dispose(disposing);
            }

            private class Dispatcher : InputDispatcher
            {
                private readonly WatermarkLabel _watermarkLabel;
                private readonly IInputDispatcher _parentDispatcher;

                public Dispatcher(WatermarkLabel watermarkLabel, IInputDispatcher parentDispatcher)
                    : base(() => watermarkLabel)
                {
                    _watermarkLabel = watermarkLabel;
                    _parentDispatcher = parentDispatcher;
                }

                public override IInputDispatcher? ParentDispatcher => _parentDispatcher;

                public override InputResponse OnMouseDown(MouseButtons buttons, Point location)
                {
                    if (buttons == MouseButtons.Left)
                    {
                        // LinkLabel sets the state required to raise LinkClicked event in OnMouseDown.
                        // The event is then raised in OnMouseUp.
                        MouseEventArgs e = CreateMouseEventArgs(buttons, location);
                        _watermarkLabel.OnMouseDown(e);

                        return InputResponse.Default;
                    }

                    return _parentDispatcher.OnMouseDown(buttons, location);
                }

                public override InputResponse OnMouseMove(MouseButtons buttons, Point location)
                {
                    _watermarkLabel.OnMouseMove(CreateMouseEventArgs(buttons, location));

                    // LinkLabel.OverrideCursor is being set when the mouse hovers the "link" portion,
                    // otherwise we keep the cursor as default
                    if (_watermarkLabel.OverrideCursor is not null)
                    {
                        return new InputResponse(cursor: _watermarkLabel.OverrideCursor);
                    }

                    return _parentDispatcher.OnMouseMove(buttons, location);
                }

                public override InputResponse OnMouseUp(MouseButtons buttons, Point location)
                {
                    if (buttons == MouseButtons.Left)
                    {
                        // LinkLabel sets the state required to raise LinkClicked event in OnMouseDown.
                        // The event is then raised in OnMouseUp.
                        MouseEventArgs e = CreateMouseEventArgs(buttons, location);
                        _watermarkLabel.OnMouseUp(e);

                        return InputResponse.Default;
                    }

                    return _parentDispatcher.OnMouseUp(buttons, location);
                }
            }

        }
    }
}
