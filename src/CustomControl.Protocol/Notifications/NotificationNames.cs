// -------------------------------------------------------------------
// Copyright (c) Microsoft Corporation. All Rights Reserved.
// See the LICENSE file in the project root for more information.
// -------------------------------------------------------------------

namespace CustomControl.Protocol.Notifications;

internal static partial class NotificationNames
{
    public static class ComponentTrays
    {
        private const string Prefix = "Custom.ComponentTrays/";

        public const string TrayAdded = Prefix + nameof(TrayAdded);
        public const string TrayRemoved = Prefix + nameof(TrayRemoved);
    }

    internal static class DesignerVerbs
    {
        private const string Prefix = "Custom.DesignerVerbs/";

        public const string LinkClicked = Prefix + nameof(LinkClicked);
    }
}
