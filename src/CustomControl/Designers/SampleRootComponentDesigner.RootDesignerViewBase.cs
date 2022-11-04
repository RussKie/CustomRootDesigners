// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// See the LICENSE file in the project root for more information.
// -------------------------------------------------------------------

using System.ComponentModel.Design;
using System.Diagnostics;
using Microsoft.DotNet.DesignTools.Designers;
using Microsoft.Extensions.Logging;

namespace CustomControl.Designers;

public partial class SampleRootComponentDesigner
{
    protected abstract partial class RootDesignerViewBase<TDesigner> : Control, IInputDispatchProvider
        where TDesigner : ComponentDesigner
    {
        public RootDesignerViewBase(TDesigner designer)
        {
            Designer = designer;

            IDesignerHost host = ((SampleRootComponentDesigner)(ComponentDesigner)designer).HostInternal;

            var loggerFactory = host.GetRequiredService<ILoggerFactory>();
            var _logger = loggerFactory.CreateLogger(GetType().FullName!);

            IInputDispatcher ? parentInputDispatcher = null;
            if (host.TryGetService(out IInputDispatchProvider? inputDispatchProvider))
            {
                parentInputDispatcher = inputDispatchProvider.InputDispatcher;
                host.RemoveService(typeof(IInputDispatchProvider));
            }

            InputDispatcher = new DefaultDispatcher(this, parentInputDispatcher, _logger);

            host.AddService(typeof(IInputDispatchProvider), this);

        }

        public TDesigner Designer { get; }

        public IInputDispatcher InputDispatcher { get; }

        private sealed class DefaultDispatcher : InputDispatcher
        {
            private readonly RootDesignerViewBase<TDesigner> _designerView;
            private readonly IInputDispatcher? _parentInputDispatcher;
            private readonly ILogger _logger;

            public DefaultDispatcher(RootDesignerViewBase<TDesigner> designerView, IInputDispatcher? parentInputDispatcher, ILogger logger)
                : base(() => designerView)
            {
                _designerView = designerView;
                _parentInputDispatcher = parentInputDispatcher;
                _logger = logger;
            }

            public override InputResponse OnMouseMove(MouseButtons buttons, Point location)
            {
                Cursor originalCursor = _designerView.Cursor;
                _designerView.OnMouseMove(new(buttons, clicks: 0, location.X, location.Y, delta: 0));
                Cursor currentCursor = _designerView.Cursor;

                var result = _parentInputDispatcher?.OnMouseMove(buttons, location) ?? InputResponse.Default;

                if (currentCursor != originalCursor)
                {
                    _logger.LogWarning($"original cursor {originalCursor}\r\nnew cursor {currentCursor}\r\n\r\n");
                    return new InputResponse(cursor: currentCursor);
                }

                return result;
            }
        }
    }
}
