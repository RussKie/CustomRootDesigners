// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// See the LICENSE file in the project root for more information.
// -------------------------------------------------------------------

using System.ComponentModel.Design;
using System.ComponentModel;

namespace SampleRootDesigner
{
    // The following attribute associates the SampleRootDocumentDesigner designer 
    // with the SampleComponent component.
    [Designer(typeof(SampleRootDocumentDesigner), typeof(IRootDesigner))]
    public class RootDocumentDesignedComponent : Component
    {
        public RootDocumentDesignedComponent()
        {
        }
    }
}
