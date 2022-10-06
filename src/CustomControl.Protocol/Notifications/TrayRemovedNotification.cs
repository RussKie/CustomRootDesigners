// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// See the LICENSE file in the project root for more information.
// -------------------------------------------------------------------

using System.ComponentModel;
using System.Composition;
using System.Drawing;
using Microsoft.DotNet.DesignTools.Protocol.DataPipe;
using Microsoft.DotNet.DesignTools.Protocol.Notifications;

namespace CustomControl.Protocol.Notifications;

[Shared]
[ExportNotification]
internal partial class TrayRemovedNotification : Notification<TrayRemovedNotification.Data>
{
    public override string Name => NotificationNames.ComponentTrays.TrayRemoved;

    protected override Data CreateNotificationData(IDataPipeReader reader)
        => new Data(reader);

    internal partial class Data : NotificationData
    {
        public static Data Empty { get; } = new Data();

        private Data() { }

        public Data(IDataPipeReader reader) : base(reader) { }

        protected override void ReadProperties(IDataPipeReader reader) { }

        protected override void WriteProperties(IDataPipeWriter writer) { }
    }
}

