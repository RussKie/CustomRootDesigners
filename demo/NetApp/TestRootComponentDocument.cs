// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// See the LICENSE file in the project root for more information.
// -------------------------------------------------------------------

using System.ComponentModel;
using CustomControl;

namespace NetCoreDesignerTest;

public partial class TestRootComponentDocument : TestComponentDocumentComponent
{
    public TestRootComponentDocument()
    {
    }

    public TestRootComponentDocument(IContainer container)
    {
        container.Add(this);

        InitializeComponent();
    }
}
