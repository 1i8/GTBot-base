using ENet.Managed;

namespace GTProxy.Wrapper;

public  static class ENetWrapper
{
    private static bool initialized = false;

    /// <summary>
    /// Loads the native enet lib
    /// </summary>
    /// <param name="path"></param>
    /// <returns></returns>
    public static int enet_initialize(string path)
    {
        if (initialized) return 0;
        if (!File.Exists(path)) return 1;
        var startupOptions = new ENetStartupOptions { ModulePath = path };
        ManagedENet.Startup(startupOptions);
        ManagedENet.Shutdown(false);
        initialized = true;
        return 0;
    }
}
