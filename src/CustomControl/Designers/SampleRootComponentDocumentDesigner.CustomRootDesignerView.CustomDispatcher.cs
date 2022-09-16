// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// See the LICENSE file in the project root for more information.
// -------------------------------------------------------------------

using Microsoft.DotNet.DesignTools.Designers;

namespace CustomControl.Designers;

public partial class SampleRootComponentDocumentDesigner
{
    private partial class CustomRootDesignerView
    {
        private class CustomDispatcher : InputDispatcher
        {
            private readonly CustomRootDesignerView _designerView;
            private readonly IInputDispatcher _parentInputDispatcher;

            public CustomDispatcher(CustomRootDesignerView designerView, IInputDispatcher parentInputDispatcher)
                : base(() => designerView)
            {
                _designerView = designerView;
                _parentInputDispatcher = parentInputDispatcher;
            }

            public override IInputDispatcher? ParentDispatcher => _parentInputDispatcher;

            public override InputResponse OnMouseMove(MouseButtons buttons, Point location)
            {
                if (s_magicArea.Contains(location))
                {
                    _designerView.DoMagic();
                    return InputResponse.DefaultSizeAll;
                }

                _designerView.UndoMagic();

                // NOTE: this doesn't reset the cursor!
                //return InputResponse.Default;
                return new InputResponse(cursor: Cursors.Default);
            }
        }
    }
}
