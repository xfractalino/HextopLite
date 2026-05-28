using System.Runtime.InteropServices;

namespace HextopLite.engine;

public enum Platform
{
    Windows,
    Linux,
    Mac,
    Unknown,
}

public static class PlatformInfo
{
    public static Platform Platform
    {
        get
        {
            if (field != Platform.Unknown)
                return field;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                field = Platform.Windows;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                field = Platform.Linux;
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                field = Platform.Mac;
            }
            else
            {
                return Platform.Unknown;
            }
            
            return Platform;
        }
    } = Platform.Unknown;
}