﻿namespace NxEditor.Core.Localization;

public class StringResources_Exceptions
{
    private const string GROUP = "Exceptions";

    public readonly PlatformNotSupportedException TargetPlatformNotSupported = new(
        GetStringResource(GROUP, nameof(TargetPlatformNotSupported))
    );
}
