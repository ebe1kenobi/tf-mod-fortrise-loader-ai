using Microsoft.Xna.Framework;
using Monocle;
using MonoMod.Utils;
using System;
using System.Collections.Generic;
using TowerFall;
using MonoMod.ModInterop;

namespace TFModFortRiseLoaderAI
{
  public class MyRollcallElement
  {
    //public static Dictionary<int, Text> playerName = new Dictionary<int, Text>(8);
    public static Dictionary<int, String> humanPlayerName = new Dictionary<int, String>(8);
    public static Dictionary<int, Image> upArrow = new Dictionary<int, Image>(8);
    public static Dictionary<int, Image> downArrow = new Dictionary<int, Image>(8);

    internal static void Load()
    {
      On.TowerFall.RollcallElement.ctor += ctor_patch;
      On.TowerFall.RollcallElement.ForceStart += ForceStart_patch;
      On.TowerFall.RollcallElement.StartVersus += StartVersus_patch;
      On.TowerFall.RollcallElement.Render += Render_patch;
      On.TowerFall.RollcallElement.NotJoinedUpdate += NotJoinedUpdate_patch;
    }

    internal static void Unload()
    {
      On.TowerFall.RollcallElement.ctor -= ctor_patch;
      On.TowerFall.RollcallElement.ForceStart -= ForceStart_patch;
      On.TowerFall.RollcallElement.StartVersus -= StartVersus_patch;
      On.TowerFall.RollcallElement.Render -= Render_patch;
      On.TowerFall.RollcallElement.NotJoinedUpdate -= NotJoinedUpdate_patch;
    }

    public MyRollcallElement() { }

    public static void ctor_patch(On.TowerFall.RollcallElement.orig_ctor orig, global::TowerFall.RollcallElement self, int playerIndex)
    {
      typeof(EigthPlayerImport).ModInterop();
      orig(self, playerIndex);
      var dynData = DynamicData.For(self);

      if (TFModFortRiseLoaderAIModule.savedHumanPlayerInput.ContainsKey(playerIndex))
      {
        TFGame.PlayerInputs[playerIndex] = TFModFortRiseLoaderAIModule.savedHumanPlayerInput[playerIndex];
        dynData.Set("input", TFGame.PlayerInputs[playerIndex]);
      }
      if (!TFModFortRiseLoaderAIModule.GetPlayerTypePlaying(playerIndex).Equals("HUMAN"))
      {
        CustomNameImport.SetPlayerName(playerIndex, TFModFortRiseLoaderAIModule.GetAIPlayerName(playerIndex));
      }
      if (TFModFortRiseLoaderAIModule.GetPlayerTypePlaying(playerIndex).Equals("HUMAN"))
      {
        humanPlayerName[playerIndex] = CustomNameImport.GetPlayerName(playerIndex);
      }


      Color color = Color.White;
      upArrow[playerIndex] = new Image(TFGame.Atlas["versus/playerIndicator"]);
      upArrow[playerIndex].FlipY = true;
      upArrow[playerIndex].Visible = true;
      upArrow[playerIndex].Color = color;
      self.Add((Component)upArrow[playerIndex]);
      upArrow[playerIndex].X = -10;
      upArrow[playerIndex].Y = 0;

      downArrow[playerIndex] = new Image(TFGame.Atlas["versus/playerIndicator"]);
      downArrow[playerIndex].Visible = true;
      self.Add((Component)downArrow[playerIndex]);
      downArrow[playerIndex].X = -10;
      downArrow[playerIndex].Y = 0;
      downArrow[playerIndex].Color = color;

      //String name = "-";
      //playerName[playerIndex] = new Text(TFGame.Font, name, positionText, color, Text.HorizontalAlign.Left, Text.VerticalAlign.Bottom);

      //self.Add((Component)playerName[playerIndex]);

      dynData.Dispose();
    }

    //public static void SetPlayerName(int playerIndex)
    //{
    //  var dynData = DynamicData.For(playerName[playerIndex]);
    //  //String type = TFModFortRiseLoaderAIModule.GetPlayerTypePlaying(playerIndex);
    //  //if (type == "HUMAN") type = "P";
    //  //string name = type + (playerIndex + 1);
    //  string name = TFModFortRiseLoaderAIModule.GetPlayerName(playerIndex);
    //  dynData.Set("text", name);
    //  dynData.Dispose();
    //}

    public static void SetAllPLayerInput()
    {
      for (var i = 0; i < TFGame.Players.Length; i++)
      {
        TFGame.PlayerInputs[i] = TFModFortRiseLoaderAIModule.GetCurrentPlayerInput(i);
      }
    }
    public static void ForceStart_patch(On.TowerFall.RollcallElement.orig_ForceStart orig, global::TowerFall.RollcallElement self)
    {
      SetAllPLayerInput();
      orig(self);
    }
    public static void StartVersus_patch(On.TowerFall.RollcallElement.orig_StartVersus orig, global::TowerFall.RollcallElement self)
    {
      SetAllPLayerInput();
      orig(self);
    }


    public static void Render_patch(On.TowerFall.RollcallElement.orig_Render orig, global::TowerFall.RollcallElement self)
    {
      var dynData = DynamicData.For(self);
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
          if (EigthPlayerImport.LaunchedEightPlayer())
          {
            upY = -53;
            downY = -37;
          }
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

      orig(self);
      dynData.Dispose();

    }
    public static int NotJoinedUpdate_patch(On.TowerFall.RollcallElement.orig_NotJoinedUpdate orig, global::TowerFall.RollcallElement self)
    {
      var dynData = DynamicData.For(self);
      int playerIndex = (int)dynData.Get("playerIndex");
      if (dynData.Get("input") == null)
        return 0;
      var input = DynamicData.For(dynData.Get("input"));
      if (input == null)
        return 0;

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
            humanPlayerName[playerIndex] = CustomNameImport.GetPlayerName(playerIndex);
          }

          TFModFortRiseLoaderAIModule.currentPlayerType[playerIndex] = previousPlayerType;

          if (!previousPlayerType.Equals("HUMAN"))
          {
            CustomNameImport.SetPlayerName(playerIndex, TFModFortRiseLoaderAIModule.GetAIPlayerName(playerIndex));
          }
          if (previousPlayerType.Equals("HUMAN"))
          {
            CustomNameImport.SetPlayerName(playerIndex, humanPlayerName[playerIndex]);
          }
        }

        // Move down
        if (MenuDown
            && !"NONE".Equals(nextPlayerType))
        {
          if (TFModFortRiseLoaderAIModule.currentPlayerType[playerIndex].Equals("HUMAN"))
          {
            humanPlayerName[playerIndex] = CustomNameImport.GetPlayerName(playerIndex);
          }

          TFModFortRiseLoaderAIModule.currentPlayerType[playerIndex] = nextPlayerType;

          if (!nextPlayerType.Equals("HUMAN"))
          {
            CustomNameImport.SetPlayerName(playerIndex, TFModFortRiseLoaderAIModule.GetAIPlayerName(playerIndex));
          }
          if (nextPlayerType.Equals("HUMAN"))
          {
            CustomNameImport.SetPlayerName(playerIndex, humanPlayerName[playerIndex]);
          }
        }
      }
      dynData.Dispose();

      return orig(self);
    }
  }
}
