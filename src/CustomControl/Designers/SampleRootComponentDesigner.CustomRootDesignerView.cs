// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// See the LICENSE file in the project root for more information.
// -------------------------------------------------------------------

using Microsoft.DotNet.DesignTools.Designers;

namespace CustomControl.Designers;

public partial class SampleRootComponentDesigner
{
    // RootDesignerView is a simple control that will be displayed 
    // in the designer window.
    private class CustomRootDesignerView : RootDesignerView
    {
        private static Rectangle s_magicArea = new(100, 100, 100, 100);
        private readonly SampleRootComponentDesigner _designer;
        private bool _isDoingMagic;
        private bool _isMagicOn;
        private readonly Font _magicFont;


        public CustomRootDesignerView(SampleRootComponentDesigner designer)
            : base(designer)
        {
            _designer = designer;

            BackColor = Color.Blue;
            DoubleBuffered = true;
            Font = new Font(Font.FontFamily.Name, 24.0f);
            _magicFont = new Font("Chiller", 24f);
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

        // This is won't going to work!
        protected override void OnMouseMove(MouseEventArgs e)
        {
            _isMagicOn = s_magicArea.Contains(e.Location);
            Cursor = _isMagicOn ? Cursors.SizeAll : Cursors.Default;

            if (_isDoingMagic != _isMagicOn)
            {
                _isDoingMagic = _isMagicOn;
                Invalidate();
            }

            base.OnMouseMove(e);
        }
    }
}
