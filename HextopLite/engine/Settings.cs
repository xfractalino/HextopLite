namespace HextopLite.engine;

public class Settings
{
    public static Settings Default { get; } = new();
    
    public Platform Platform => PlatformInfo.Platform;

    public RendererType RendererType
    {
        get
        {
            if (field != RendererType.Unknown)
                return field;

            // Select the best default renderer based on the platform.
            switch (Platform)
            {
                case Platform.Windows:
                    field = RendererType.D3D11;
                    break;
                case Platform.Linux:
                    field = RendererType.Vulkan;
                    break;
                case Platform.Mac:
                    field = RendererType.Metal;
                    break;
                case Platform.Unknown:
                default:
                    return RendererType.Unknown;
            }

            return RendererType;
        }
        init;
    } = RendererType.Unknown;
}