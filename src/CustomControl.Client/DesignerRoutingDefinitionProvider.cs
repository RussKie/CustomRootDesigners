// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// See the LICENSE file in the project root for more information.
// -------------------------------------------------------------------

using System.Collections.Generic;
using System.Composition;
using Microsoft.DotNet.DesignTools.Client.TypeRouting;

namespace CustomControl.Client
{
    [Shared]
    [ExportTypeRoutingDefinitionProvider]
    internal class DesignerRoutingDefinitionProvider : TypeRoutingDefinitionProvider
    {
        private readonly TypeRoutingDefinition[] _definitions = new[]
        {
            // Container control designers...
            RerouteDesigner<SampleRootComponentProxyDesigner>("CustomControl.Designers.SampleRootComponentDesigner"),
            RerouteDesigner<SampleRootComponentDocumentProxyDesigner>("CustomControl.Designers.SampleRootComponentDocumentDesigner"),
        };

        public TypeRoutingDefinition[] Definitions => _definitions;

        public override IEnumerable<TypeRoutingDefinition> GetDefinitions()
            => Definitions;
    }

}
