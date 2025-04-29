namespace Core.Default;
public static class DefaultSetting
{
    private static Engine? _engine;

    internal static void Registry(Engine engine)
    {
        // _engine.dispose();
        _engine = engine;
    }

    internal static Engine GetEngine()
    {
        return _engine ??
            throw new NotImplementedException("Not any engine was registered.");
    }
}
