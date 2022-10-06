// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// See the LICENSE file in the project root for more information.
// -------------------------------------------------------------------

using System;
using System.Composition;
using Microsoft.DotNet.DesignTools.Protocol.DataPipe;
using Microsoft.DotNet.DesignTools.Protocol.Notifications;

namespace CustomControl.Protocol.Notifications;

[Shared]
[ExportNotification]
internal partial class TrayAddedNotification : Notification<TrayAddedNotification.Data>
{
    public override string Name => NotificationNames.ComponentTrays.TrayAdded;

    protected override Data CreateNotificationData(IDataPipeReader reader)
        => new Data(reader);

    internal partial class Data : NotificationData
    {
        public object ComponentTray { get; private set; }
        public Data() { }

        public Data(object componentTray)
        {
            if (componentTray is null)
            {
                throw new ArgumentNullException(nameof(componentTray));
            }

            ComponentTray = componentTray;
        }

        public Data(IDataPipeReader reader) : base(reader) { }

        protected override void ReadProperties(IDataPipeReader reader)
        {
            ComponentTray = reader.ReadObject(nameof(ComponentTray));
        }

        protected override void WriteProperties(IDataPipeWriter writer)
        {
            writer.WriteObject(nameof(ComponentTray), ComponentTray);
        }
    }
}
