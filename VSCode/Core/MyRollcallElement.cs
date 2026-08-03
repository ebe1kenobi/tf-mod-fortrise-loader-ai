using System;
using System.Collections.Generic;
using FortRise;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Cil;
using MonoMod.Utils;
using TFModFortRiseCustomName;
using TowerFall;

namespace TFModFortRiseLoaderAI
{
  public class MyRollcallElement : IHookable
  {
    public static Dictionary<int, String> humanPlayerName = new Dictionary<int, String>(8);
    public static Dictionary<int, Image> upArrow = new Dictionary<int, Image>(8);
    public static Dictionary<int, Image> downArrow = new Dictionary<int, Image>(8);

    public static void Load(IHarmony harmony)
    {
      harmony.Patch(
          AccessTools.DeclaredConstructor(typeof(RollcallElement), [
                                                                        typeof(int),
                                                                    ]),
          postfix: new HarmonyMethod(ctor_patch)
      );

      harmony.Patch(
          AccessTools.DeclaredMethod(typeof(RollcallElement), nameof(RollcallElement.Render)),
          prefix: new HarmonyMethod(Render_patch)
      );
      harmony.Patch(
          AccessTools.DeclaredMethod(typeof(RollcallElement), "NotJoinedUpdate"),
          prefix: new HarmonyMethod(NotJoinedUpdate_patch)
      );
      harmony.Patch(
          AccessTools.DeclaredMethod(typeof(RollcallElement), "ForceStart"),
          prefix: new HarmonyMethod(ForceStart_patch)
      );
      harmony.Patch(
          AccessTools.DeclaredMethod(typeof(RollcallElement), "StartVersus"),
          prefix: new HarmonyMethod(StartVersus_patch)
      );
    }

    public static void ctor_patch(RollcallElement __instance, int playerIndex)
    {
      //typeof(EigthPlayerImport).ModInterop();
      //typeof(CustomNameImport).ModInterop();
      //var CustomNameModApi = TFModFortRiseLoaderAIModule.Instance.CustomNameModApi;
      //TFModFortRiseLoaderAIModule.Instance.CustomNameModApi = TFModFortRiseLoaderAIModule.Instance.Context.Interop.GetApi<ICustomNameModApi>("TFModFortRiseCustomName");
      //if (widerSetApi is null)
      //{
      //  return 0;
      //}

      //return widerSetApi.IsWide ? 55 : 0;

      var dynData = DynamicData.For(__instance);

      if (TFModFortRiseLoaderAIModule.savedHumanPlayerInput.ContainsKey(playerIndex))
      {
        TFGame.PlayerInputs[playerIndex] = TFModFortRiseLoaderAIModule.savedHumanPlayerInput[playerIndex];
        dynData.Set("input", TFGame.PlayerInputs[playerIndex]);
      }
      if (!TFModFortRiseLoaderAIModule.GetPlayerTypePlaying(playerIndex).Equals("HUMAN"))
      {
        TFModFortRiseLoaderAIModule.Instance.CustomNameModApi.SetPlayerName(playerIndex, TFModFortRiseLoaderAIModule.GetAIPlayerName(playerIndex));
      }
      //if (TFModFortRiseLoaderAIModule.Instance.CustomNameModApi.GetPlayerName == null) {
      //  throw new Exception("CustomNameModApi.GetPlayerName is null");
      //}
      if (TFModFortRiseLoaderAIModule.GetPlayerTypePlaying(playerIndex).Equals("HUMAN"))
      {
        humanPlayerName[playerIndex] = TFModFortRiseLoaderAIModule.Instance.CustomNameModApi.GetPlayerName(playerIndex);
      }


      Color color = Color.White;
      upArrow[playerIndex] = new Image(TFGame.Atlas["versus/playerIndicator"]);
      upArrow[playerIndex].FlipY = true;
      upArrow[playerIndex].Visible = true;
      upArrow[playerIndex].Color = color;
      __instance.Add((Component)upArrow[playerIndex]);
      upArrow[playerIndex].X = -10;
      upArrow[playerIndex].Y = 0;

      downArrow[playerIndex] = new Image(TFGame.Atlas["versus/playerIndicator"]);
      downArrow[playerIndex].Visible = true;
      __instance.Add((Component)downArrow[playerIndex]);
      downArrow[playerIndex].X = -10;
      downArrow[playerIndex].Y = 0;
      downArrow[playerIndex].Color = color;

      dynData.Dispose();
    }

    public static void SetAllPLayerInput()
    {
      for (var i = 0; i < TFGame.Players.Length; i++)
      {
        if (TFGame.Players[i])
        {
          TFGame.PlayerInputs[i] = TFModFortRiseLoaderAIModule.GetCurrentPlayerInput(i);
        }
      }
    }
    public static void ForceStart_patch(Level __instance)
    {
      SetAllPLayerInput();
    }
    public static void StartVersus_patch(Level __instance)
    {
      SetAllPLayerInput();
    }


    public static void Render_patch(Level __instance)
    {
      var dynData = DynamicData.For(__instance);
      int playerIndex = (int)dynData.Get("playerIndex");
      //SetPlayerName(playerIndex);
      if (((Image)dynData.Get("rightArrow")).Visible && TFModFortRiseLoaderAIModule.IsThereOtherPlayerType(playerIndex))
      {
        if ("NONE".Equals(TFModFortRiseLoaderAIModule.PreviousPlayerTypeExist(playerIndex)))
        {
          upArrow[playerIndex].Visible = false;
        }
        else
        {
          upArrow[playerIndex].Visible = true;
        }

        if ("NONE".Equals(TFModFortRiseLoaderAIModule.NextPlayerTypeExist(playerIndex)))
        {
          downArrow[playerIndex].Visible = false;
        }
        else
        {
          downArrow[playerIndex].Visible = true;
        }

        var arrowSine = DynamicData.For(dynData.Get("arrowSine"));
        var rightArrowWiggle = (bool)dynData.Get("rightArrowWiggle");
        var arrowWiggle = DynamicData.For(dynData.Get("arrowWiggle"));
        float arrowSineValue = (float)arrowSine.Get("Value");
        float arrowWiggleValue = (float)arrowWiggle.Get("Value");

        int upY = -73;
        int downY = -57;
        if (TFGame.Players.Length > 4) {
          //if (EigthPlayerImport.LaunchedEightPlayer())
          //{
          //  upY = -53;
          //  downY = -37;
          //}
        } 
        upArrow[playerIndex].Y = (float)(upY + arrowSineValue * 3.0 + 5.0 * (rightArrowWiggle ? arrowWiggleValue : 0.0));
        downArrow[playerIndex].Y = (float)(downY - arrowSineValue * 3.0 + 5.0 * (!rightArrowWiggle ? arrowWiggleValue : 0.0));
        arrowSine.Dispose();
        arrowWiggle.Dispose();
      }
      else
      {
        upArrow[playerIndex].Visible = false;
        downArrow[playerIndex].Visible = false;
      }

      dynData.Dispose();

    }
    public static void NotJoinedUpdate_patch(Level __instance)
    {
      var dynData = DynamicData.For(__instance);
      int playerIndex = (int)dynData.Get("playerIndex");
      if (dynData.Get("input") == null)
        return;
      var input = DynamicData.For(dynData.Get("input"));
      if (input == null)
        return;

      var CustomNameModApi = TFModFortRiseLoaderAIModule.Instance.CustomNameModApi;

      var MenuUp = (bool)input.Get("MenuUp");
      var MenuDown = (bool)input.Get("MenuDown");

      if (TFModFortRiseLoaderAIModule.IsThereOtherPlayerType(playerIndex))
      { //at leat 2 player type
        // Move up 


        String previousPlayerType = TFModFortRiseLoaderAIModule.PreviousPlayerTypeExist(playerIndex);
        String nextPlayerType = TFModFortRiseLoaderAIModule.NextPlayerTypeExist(playerIndex);

        if (MenuUp
            && !"NONE".Equals(previousPlayerType))
        {
          if (TFModFortRiseLoaderAIModule.currentPlayerType[playerIndex].Equals("HUMAN"))
          {
            humanPlayerName[playerIndex] = CustomNameModApi.GetPlayerName(playerIndex);
          }

          TFModFortRiseLoaderAIModule.currentPlayerType[playerIndex] = previousPlayerType;

          if (!previousPlayerType.Equals("HUMAN"))
          {
            CustomNameModApi.SetPlayerName(playerIndex, TFModFortRiseLoaderAIModule.GetAIPlayerName(playerIndex));
          }
          if (previousPlayerType.Equals("HUMAN"))
          {
            CustomNameModApi.SetPlayerName(playerIndex, humanPlayerName[playerIndex]);
          }
        }

        // Move down
        if (MenuDown
            && !"NONE".Equals(nextPlayerType))
        {
          if (TFModFortRiseLoaderAIModule.currentPlayerType[playerIndex].Equals("HUMAN"))
          {
            humanPlayerName[playerIndex] = CustomNameModApi.GetPlayerName(playerIndex);
          }

          TFModFortRiseLoaderAIModule.currentPlayerType[playerIndex] = nextPlayerType;

          if (!nextPlayerType.Equals("HUMAN"))
          {
            CustomNameModApi.SetPlayerName(playerIndex, TFModFortRiseLoaderAIModule.GetAIPlayerName(playerIndex));
          }
          if (nextPlayerType.Equals("HUMAN"))
          {
            CustomNameModApi.SetPlayerName(playerIndex, humanPlayerName[playerIndex]);
          }
        }
      }
      dynData.Dispose();
    }
  }
}
