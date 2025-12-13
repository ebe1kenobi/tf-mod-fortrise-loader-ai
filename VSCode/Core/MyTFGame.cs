using FortRise;
using HarmonyLib;
using Microsoft.Xna.Framework;
using TowerFall;

namespace TFModFortRiseLoaderAI
{
  internal class MyTFGame : IHookable
  {

    public static void Load(IHarmony harmony)
    {
      harmony.Patch(
          AccessTools.DeclaredMethod(typeof(TFGame), "Update"),
          prefix: new HarmonyMethod(Update_patch)
      );
    }

    public static void Update_patch(TFGame __instance)
    {
      //InteropBootstrap.Ensure();
      if (TFGame.GameLoaded && !TFModFortRiseLoaderAIModule.isHumanPlayerTypeSaved)
      {
        for (var i = 0; i < TFGame.Players.Length; i++)
        {
          if (TFGame.PlayerInputs[i] == null)
          {
            TFModFortRiseLoaderAIModule.currentPlayerType[i] = "NONE";
            TFModFortRiseLoaderAIModule.nbPlayerType[i] = 0;
            continue;
          }

          TFModFortRiseLoaderAIModule.nbPlayerType[i] = 1;
          TFModFortRiseLoaderAIModule.currentPlayerType[i] = "HUMAN";
          TFModFortRiseLoaderAIModule.savedHumanPlayerInput[i] = TFGame.PlayerInputs[i];
        }
        TFModFortRiseLoaderAIModule.isHumanPlayerTypeSaved = true;
        TFModFortRiseLoaderAIModule.canAddAgent = true;
      }
    }
  }
}
