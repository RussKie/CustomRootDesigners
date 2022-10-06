// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// See the LICENSE file in the project root for more information.
// -------------------------------------------------------------------

using System.ComponentModel;
using System.ComponentModel.Design;
using Microsoft.DotNet.DesignTools.Designers;

namespace CustomControl.Designers;

public partial class SampleRootComponentDesigner : ComponentDesigner, IRootDesigner
{
    private DesignerFrame? _frame;

    // Member field of custom type RootDesignerView, a control that 
    // will be shown in the Forms designer view. This member is 
    // cached to reduce processing needed to recreate the 
    // view control on each call to GetView().
    private CustomRootDesignerView? _view;

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            DisposeDesignerFrame();

            Host.RemoveService(typeof(IInputDispatchProvider));
        }

        base.Dispose(disposing);
    }


    public override void Initialize(IComponent component)
    {
        base.Initialize(component);

        // Create the view for this component. We first create the designer frame so we can provide
        // the overlay and split window services, and then later on we initilaize the frame with
        // the designer view.
        _frame = InitializeDesignerFrame();

        // Some type of displayable Form or control is required 
        // for a root designer that overrides GetView(). In this 
        // example, a Control of type RootDesignerView is used.
        // Any class that inherits from Control will work.
        _view = new CustomRootDesignerView(this);

        Host.AddService(typeof(IInputDispatchProvider), _view);

        // Finally hook the designer view into the frame.  We do this last because the frame may
        // cause the control to be created, and if this happens before the inheritance scan we
        // will subclass the inherited controls before we get a chance to attach designers.  
        _frame.Initialize(_view);

        // And force some shadow properties that we change in the course of initializing the form.
        _view.Location = Point.Empty;
    }

    // This method returns an instance of the view for this root
    // designer. The "view" is the user interface that is presented
    // in a document window for the user to manipulate. 
    object IRootDesigner.GetView(ViewTechnology technology)
    {
        if (technology != ViewTechnology.Default)
        {
            throw new ArgumentException("Not a supported view technology", nameof(technology));
        }

        return _frame!;
    }

    // IRootDesigner.SupportedTechnologies is a required override for an
    // IRootDesigner. Default is the view technology used by this designer.  
    ViewTechnology[] IRootDesigner.SupportedTechnologies
        => new ViewTechnology[] { ViewTechnology.Default };
}
