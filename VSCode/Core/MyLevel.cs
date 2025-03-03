using System.Xml;
namespace TFModFortRiseLoaderAI
{
  public class MyLevel
  {
    internal static void Load()
    {
      On.TowerFall.Level.Update += Update_patch;
      On.TowerFall.Level.ctor += ctor_patch;
    }

    internal static void Unload()
    {
      On.TowerFall.Level.Update -= Update_patch;
      On.TowerFall.Level.ctor -= ctor_patch;
    }

    public static void ctor_patch(On.TowerFall.Level.orig_ctor orig, global::TowerFall.Level self, global::TowerFall.Session session, XmlElement xml)
    {
      TFModFortRiseLoaderAIModule.SetAgentLevel(self); 
      orig(self, session, xml);
    }

    public static void Update_patch(On.TowerFall.Level.orig_Update orig, global::TowerFall.Level self)
    {
      orig(self);
      if (!(self.Ending))
      {
        TFModFortRiseLoaderAIModule.AgentUpdate(self);
      }
    }
  }
}
