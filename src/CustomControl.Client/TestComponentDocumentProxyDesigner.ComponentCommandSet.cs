// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// See the LICENSE file in the project root for more information.
// -------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Windows.Forms.Design;

namespace CustomControl.Client;


public partial class TestComponentDocumentProxyDesigner
{
    /// <summary>
    ///  This class implements commands that are specific to the component designer.
    /// </summary>
    private class ComponentCommandSet : Microsoft.DotNet.DesignTools.Client.Commands.CommandSet
    {
        private readonly CommandSetItem[] _commandSet;
        private  readonly IMenuCommandService _menuService;

        public ComponentCommandSet(ISite site) : base(site)
        {
            _menuService = Site.GetRequiredService<IMenuCommandService>();

            // Establish our set of commands
            _commandSet = new[]
            {
                // Keyboard commands
                CreateItem(MenuCommands.KeySelectNext, OnStatusAlways, OnKeySelect),
                CreateItem(MenuCommands.KeySelectPrevious, OnStatusAlways, OnKeySelect),
            };

            for (int i = 0; i < _commandSet.Length; i++)
            {
                _menuService.AddCommand(_commandSet[i]);
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                for (int i = 0; i < _commandSet.Length; i++)
                {
                    _menuService.RemoveCommand(_commandSet[i]);
                    _commandSet[i].Dispose();
                }
            }

            base.Dispose(disposing);
        }

        protected override bool OnKeyCancelCore(MenuCommand command)
        {
            // The base implementation here just checks to see if we are dragging.
            // If we are, then we abort the drag.
            if (base.OnKeyCancelCore(command))
            {
                return false;
            }

            Debug.Assert(
                command.CommandID == MenuCommands.KeyCancel ||
                command.CommandID == MenuCommands.KeyReverseCancel);

            if (command.CommandID == MenuCommands.KeyCancel)
            {
                //CommandEndpoints.SelectParent(Session.Id);
            }
            else
            {
                //CommandEndpoints.SelectFirstChild(Session.Id);
            }

            return true;
        }

        protected override void OnKeyMove(MenuCommand command)
        {
            // We don't support keyboard move commands, so do nothing
        }

        /// <summary>
        /// Called for selection via the TAB or Shift+TAB key.
        /// </summary>
        protected void OnKeySelect(MenuCommand command)
        {
            Debug.Assert(
                command.CommandID == MenuCommands.KeySelectNext ||
                command.CommandID == MenuCommands.KeySelectPrevious);

            bool forward = command.CommandID == MenuCommands.KeySelectNext;

            if (forward)
            {
                //CommandEndpoints.SelectNextSibling(Session.Id);
            }
            else
            {
                //CommandEndpoints.SelectPreviousSibling(Session.Id);
            }
        }

        protected override void OnUpdateCommandStatus()
        {
            if (IsDisposed)
            {
                return;
            }

            // Now go through all of the commands and ask them to update.
            foreach (CommandSetItem command in _commandSet)
            {
                command.UpdateStatus();
            }

            base.OnUpdateCommandStatus();
        }
    }

}
