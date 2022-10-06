// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// See the LICENSE file in the project root for more information.
// -------------------------------------------------------------------

using System.Composition;
using Microsoft.DotNet.DesignTools.Protocol.DataPipe;
using Microsoft.DotNet.DesignTools.Protocol.Notifications;

namespace CustomControl.Protocol.Notifications;

[Shared]
[ExportNotification]
internal partial class LinkClickedNotification : Notification<LinkClickedNotification.Data>
{
    public override string Name => NotificationNames.DesignerVerbs.LinkClicked;

    protected override Data CreateNotificationData(IDataPipeReader reader)
        => new Data(reader);

    internal partial class Data : NotificationData
    {
        public string LinkText { get; private set; }
        public Data() { }

        public Data(string linkText)
        {
            LinkText = linkText; //.OrThrowIfArgumentIsNull();
        }

        public Data(IDataPipeReader reader) : base(reader) { }

        protected override void ReadProperties(IDataPipeReader reader)
        {
            LinkText = reader.ReadString(nameof(LinkText));
        }

        protected override void WriteProperties(IDataPipeWriter writer)
        {
            writer.Write(nameof(LinkText), LinkText);
        }
    }
}
