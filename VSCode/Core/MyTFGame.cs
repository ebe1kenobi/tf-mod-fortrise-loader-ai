using Microsoft.Xna.Framework;
using TowerFall;

namespace TFModFortRiseLoaderAI
{
  internal class MyTFGame
  {

    internal static void Load()
    {
      On.TowerFall.TFGame.Update += Update_patch;
    }

    internal static void Unload()
    {
      On.TowerFall.TFGame.Update -= Update_patch;
    }

    public static void Update_patch(On.TowerFall.TFGame.orig_Update orig, global::TowerFall.TFGame self, GameTime gameTime)
    {
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
        //AI.CreateAgent();
      }

      orig(self, gameTime);
    }
  }
}
