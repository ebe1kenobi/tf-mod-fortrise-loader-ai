using System.Xml;
using FortRise;
using HarmonyLib;
using TowerFall;
namespace TFModFortRiseLoaderAI
{
  public class MyLevel : IHookable
  {
    public static void Load(IHarmony harmony)
    {
      harmony.Patch(
          AccessTools.DeclaredConstructor(typeof(Level), [
                                                                        typeof(Session),
                                                                        typeof(XmlElement),
                                                                    ]),
          prefix: new HarmonyMethod(ctor_patch)
      );

      harmony.Patch(
          AccessTools.DeclaredMethod(typeof(Level), nameof(Level.Update)),
          prefix: new HarmonyMethod(Update_patch)
      );
    }


    public static void ctor_patch(Level __instance, Session session, XmlElement xml)
    {
      TFModFortRiseLoaderAIModule.SetAgentLevel(__instance); 
    }

    public static void Update_patch(Level __instance)
    {
      if (!(__instance.Ending))
      {
        TFModFortRiseLoaderAIModule.AgentUpdate(__instance);
      }
    }
  }
}
