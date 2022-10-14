// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// See the LICENSE file in the project root for more information.
// -------------------------------------------------------------------

using Microsoft.DotNet.DesignTools.Designers;

namespace CustomControl.Designers;

public partial class TestComponentDocumentDesigner
{
    private partial class CustomRootDesignerView
    {
        private class CustomDispatcher : InputDispatcher
        {
            private readonly IInputDispatcher _parentDispatcher;
            private readonly TestComponentDocumentDesigner _compositionDesigner;
            private readonly WatermarkLabel _watermarkLabel;
            private bool _isToolboxItemSelected;

            public CustomDispatcher(CustomRootDesignerView owner, TestComponentDocumentDesigner compositionDesigner, WatermarkLabel watermarkLabel)
                : base(() => owner)
            {
                _parentDispatcher = owner.GetDispatcher();
                _compositionDesigner = compositionDesigner;
                _watermarkLabel = watermarkLabel;
            }

            public override IInputDispatcher? ParentDispatcher => _parentDispatcher;

            public override InputResponse OnMouseDown(MouseButtons buttons, Point location)
            {
                _isToolboxItemSelected = _compositionDesigner.ToolboxService.IsItemSelected;

                if (_watermarkLabel.Visible)
                {
                    return _watermarkLabel.InputDispatcher.OnMouseDown(buttons, location);
                }

                return _parentDispatcher.OnMouseDown(buttons, location);
            }

            public override InputResponse OnMouseMove(MouseButtons buttons, Point location)
            {
                InputResponse response;
                if (_watermarkLabel.Visible)
                {
                    response = _watermarkLabel.InputDispatcher.OnMouseMove(buttons, location);
                }
                else
                {
                    response = _parentDispatcher.OnMouseMove(buttons, location);
                }

                // HACK: it was observed that something outside the designer can either reset Cursor.Current instance
                // or set it to the WaitCursor. This results in an undesired UX for this designer.
                // Force the cursor to be the default.
                if (response.Cursor is null || response.Cursor == Cursors.WaitCursor)
                {
                    Cursor.Current = Cursors.Default;
                }

                return response;
            }

            public override InputResponse OnMouseUp(MouseButtons buttons, Point location)
            {
                if (_isToolboxItemSelected)
                {
                    _compositionDesigner.SendCreateSelectedToolboxItemNotification(location);
                    _isToolboxItemSelected = false;

                    return InputResponse.Default;
                }

                if (_watermarkLabel.Visible)
                {
                    return _watermarkLabel.InputDispatcher.OnMouseUp(buttons, location);
                }

                return _parentDispatcher.OnMouseUp(buttons, location);
            }
        }
    }
}
