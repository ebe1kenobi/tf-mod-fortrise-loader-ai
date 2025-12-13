using FortRise;

namespace TFModFortRiseLoaderAI;

public interface IHookable
{
    abstract static void Load(IHarmony harmony);
}