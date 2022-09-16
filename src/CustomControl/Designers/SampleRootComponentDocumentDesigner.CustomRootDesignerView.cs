// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// See the LICENSE file in the project root for more information.
// -------------------------------------------------------------------

using Microsoft.DotNet.DesignTools.Designers;

namespace CustomControl.Designers;

public partial class SampleRootComponentDocumentDesigner
{
    // RootDesignerView is a simple control that will be displayed 
    // in the designer window.
    private partial class CustomRootDesignerView : RootDesignerView
    {
        private static Rectangle s_magicArea = new(100, 100, 100, 100);
        private readonly SampleRootComponentDocumentDesigner _designer;
        private readonly IInputDispatcher _parentInputDispatcher;
        private bool _isDoingMagic;
        private readonly Font _magicFont;

        public CustomRootDesignerView(SampleRootComponentDocumentDesigner designer, IInputDispatcher parentInputDispatcher)
            : base(designer)
        {
            _designer = designer;
            _parentInputDispatcher = parentInputDispatcher;
            BackColor = Color.Olive;
            DoubleBuffered = true;
            Font = new Font(Font.FontFamily.Name, 24.0f);
            _magicFont = new Font("Chiller", 24f);
        }

        protected override IInputDispatcher InputDispatcher => new CustomDispatcher(this, _parentInputDispatcher);

        private void DoMagic()
        {
            if (_isDoingMagic)
            {
                return;

            }

            _isDoingMagic = true;
            Invalidate();
        }

        private void UndoMagic()
        {
            if (!_isDoingMagic)
            {
                return;

            }

            _isDoingMagic = false;
            Invalidate();
        }

        protected override void OnPaint(PaintEventArgs pe)
        {
            base.OnPaint(pe);

            if (_isDoingMagic)
            {
                pe.Graphics.FillRectangle(SystemBrushes.Info, s_magicArea);
                pe.Graphics.DrawRectangle(SystemPens.ControlDark, s_magicArea);
                pe.Graphics.DrawString("MAGIC!", _magicFont, Brushes.Red, s_magicArea);
            }

            // Draws the name of the component in large letters.
            pe.Graphics.DrawString(_designer.Component.Site!.Name, Font, Brushes.Yellow, ClientRectangle);
        }
    }
}
