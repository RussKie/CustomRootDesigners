// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// See the LICENSE file in the project root for more information.
// -------------------------------------------------------------------

using CustomControl;

namespace NetCoreDesignerTest;

// This sample demonstrates how to provide the root designer view, or
// design mode background view, by overriding IRootDesigner.GetView().

// This sample component inherits from RootDesignedComponent which 
// uses the SampleRootDesigner.
public class SampleRootComponent : SampleRootComponentDesignedComponent
{
    public SampleRootComponent()
    {
    }
}
