// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// See the LICENSE file in the project root for more information.
// -------------------------------------------------------------------

namespace SampleRootDesigner
{
    // This sample demonstrates how to provide the root designer view, or
    // design mode background view, by overriding IRootDesigner.GetView().

    // This sample component inherits from RootDocumentDesignedComponent which 
    // uses the SampleRootDocumentDesigner.
    public class RootViewSampleDocumentComponent : RootDocumentDesignedComponent
    {
        public RootViewSampleDocumentComponent()
        {
        }
    }
}
