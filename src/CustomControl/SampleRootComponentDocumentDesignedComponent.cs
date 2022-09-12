// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// See the LICENSE file in the project root for more information.
// -------------------------------------------------------------------

using System.ComponentModel;
using System.ComponentModel.Design;
using CustomControl.Designers;

namespace CustomControl;

// The following attribute associates the SampleRootComponentDocumentDesigner designer 
// with this component.
[Designer(typeof(SampleRootComponentDocumentDesigner), typeof(IRootDesigner))]
public class SampleRootComponentDocumentDesignedComponent : Component
{
    public SampleRootComponentDocumentDesignedComponent()
    {
    }
}
