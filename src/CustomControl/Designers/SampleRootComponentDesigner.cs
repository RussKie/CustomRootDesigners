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
    // Member field of custom type RootDesignerView, a control that 
    // will be shown in the Forms designer view. This member is 
    // cached to reduce processing needed to recreate the 
    // view control on each call to GetView().
    private CustomRootDesignerView? _view;

    public override void Initialize(IComponent component)
    {
        base.Initialize(component);

        // Some type of displayable Form or control is required 
        // for a root designer that overrides GetView(). In this 
        // example, a Control of type RootDesignerView is used.
        // Any class that inherits from Control will work.
        _view = new CustomRootDesignerView(this);
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

        return _view!;
    }

    // IRootDesigner.SupportedTechnologies is a required override for an
    // IRootDesigner. Default is the view technology used by this designer.  
    ViewTechnology[] IRootDesigner.SupportedTechnologies
        => new ViewTechnology[] { ViewTechnology.Default };
}
