// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// See the LICENSE file in the project root for more information.
// -------------------------------------------------------------------

using System.ComponentModel;
using System.ComponentModel.Design;
using CustomControl.Designers;

namespace CustomControl;

[Designer(typeof(TestComponentDocumentDesigner), typeof(IRootDesigner))]
public class TestComponentDocumentComponent : Component
{
    public TestComponentDocumentComponent()
    {
    }
}
