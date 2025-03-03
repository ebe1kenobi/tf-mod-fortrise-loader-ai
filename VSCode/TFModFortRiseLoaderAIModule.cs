using System;
using FortRise;
using TowerFall;
using MonoMod.ModInterop;
using System.Collections.Generic;

namespace TFModFortRiseLoaderAI
{
  [Fort("com.ebe1.kenobi.tfmodfortriseloaderai", "TFModFortRiseLoaderAI")]
  public class TFModFortRiseLoaderAIModule : FortModule
  {
    public static TFModFortRiseLoaderAIModule Instance;
    public static bool EightPlayerMod;
    public static bool canAddAgent = false;
    public static Dictionary<int, String> currentPlayerType = new Dictionary<int, String>(8);
    public static Dictionary<int, PlayerInput> savedHumanPlayerInput = new Dictionary<int, PlayerInput>(8);
    public static bool isHumanPlayerTypeSaved = false;
    public static Dictionary<String, Agent[]> listAgentByType = new Dictionary<String, Agent[]>();
    public static int[] nbPlayerType = new int[8];
    public static Dictionary<int, String> listAgentType = new Dictionary<int,String>();
    public const string InputName = "TFModFortRiseLoaderAI.Input";

    public override Type SettingsType => typeof(TFModFortRiseLoaderAISettings);
    public static TFModFortRiseLoaderAISettings Settings => (TFModFortRiseLoaderAISettings)Instance.InternalSettings;

    public TFModFortRiseLoaderAIModule()
    {
      Instance = this;
      Logger.Init("LoaderAILOG");
    }

    public override void LoadContent()
    {

    }

    public override void Load()
    {
      MyTFGame.Load();
      MyRollcallElement.Load();
      MyLevel.Load();
      MyPlayerIndicator.Load();
      MyVersusRoundResults.Load();

      typeof(ModExports).ModInterop();

      EightPlayerMod = IsModExists("WiderSetMod");
    }

    public override void Unload()
    {
      MyTFGame.Unload();
      MyRollcallElement.Unload();
      MyLevel.Unload();
      MyPlayerIndicator.Unload();

      MyVersusRoundResults.Unload();
    }

    public static bool IsAgentPlaying(int playerIndex, Level level)
    {
      return level.GetPlayer(playerIndex) != null; 
      //return true;// level.GetPlayer(playerIndex) != null;  todo training for death
    }

    public static bool CurrentPlayerIs(String type, int playerIndex)
    {
      return currentPlayerType[playerIndex] == type;
    }

    public static string GetPlayerTypePlaying(int playerIndex)
    {
      return currentPlayerType[playerIndex];
    }

    public static string GetPlayerName(int playerIndex)
    {
      String type = GetPlayerTypePlaying(playerIndex);
      if (type == "HUMAN") type = "P";
      if (type == "NONE") type = "P";
      return type + " " + (playerIndex + 1); // space " " important to detect in MyVersusRoundResults.Update_patch()
    }

    public static PlayerInput GetCurrentPlayerInput(int playerIndex)
    {
      if (!currentPlayerType.ContainsKey(playerIndex)) return null;

      if (currentPlayerType[playerIndex] == "HUMAN")
      {
        return savedHumanPlayerInput[playerIndex];
      }
      else
      {
        return listAgentByType[currentPlayerType[playerIndex]][playerIndex].getInput();
      }
    }

    public static bool HumanControlExists(int playerIndex)
    {
      return savedHumanPlayerInput.ContainsKey(playerIndex);
    }

    public static bool IsThereOtherPlayerType(int playerIndex)
    {
      return nbPlayerType[playerIndex] > 1;
    }

    public static String NextPlayerTypeExist(int playerIndex) {
      if (!IsThereOtherPlayerType(playerIndex)) return "NONE";

      if (currentPlayerType[playerIndex] == "HUMAN")
      {
        return listAgentType[0];
      }

      for (var i = 0; i < listAgentType.Count; i++)
      {
        if (currentPlayerType[playerIndex] == listAgentType[i])
        {
          if (i < listAgentType.Count - 1)
          {
            return listAgentType[i + 1];
          }
        }
      }
      return "NONE";
    }

    public static String PreviousPlayerTypeExist(int playerIndex)
    {
      if (!IsThereOtherPlayerType(playerIndex)) return "NONE";

      if (currentPlayerType[playerIndex] == "HUMAN")
      {
        return "NONE";
      }

      for (var i = 0; i < listAgentType.Count; i++)
      {
        if (currentPlayerType[playerIndex] == listAgentType[i])
        {
          if (i == 0 && HumanControlExists(playerIndex)) {
            return "HUMAN";
          }
          if (i > 0)
          {
            return listAgentType[i - 1];
          }
        }
      }
      return "NONE";
    }

    public static void SetAgentLevel(Level level)
    {
      for (var i = 0; i < TFGame.Players.Length; i++)
      {
        //Logger.Info("SetAgentLevel " + i);
        if (!TFGame.Players[i]) continue;
        //Logger.Info("SetAgentLevel1 " + i);
        if (null == TFGame.PlayerInputs[i]) continue;
        //Logger.Info("SetAgentLevel2 " + i);
        if (!InputName.Equals(TFGame.PlayerInputs[i].GetType().ToString())) continue;
        //Logger.Info("SetAgentLevel3 " + i);
        //Logger.Info("currentPlayerType[i] " + currentPlayerType[i]);
        //Logger.Info("listAgentByType " + listAgentByType.Keys.ToString());
        // Not noamel todo, InputName.Equals should continue when human (check with training on)
        if (TFModFortRiseLoaderAIModule.currentPlayerType[i] == "NONE" || TFModFortRiseLoaderAIModule.currentPlayerType[i] == "HUMAN")
        {
          continue;
        }

        listAgentByType[currentPlayerType[i]][i].SetLevel(level);
        //Logger.Info("SetAgentLevel4 " + i);
      }
    }

    public static void AgentUpdate(Level level)
    {
      if (level.LivingPlayers == 0) return;

      for (int i = 0; i < TFGame.Players.Length; i++)
      {
        if (!TFGame.Players[i]) continue;

        if (TFGame.PlayerInputs[i] == null) continue;


        if (!(InputName.Equals(TFGame.PlayerInputs[i].GetType().ToString())
            && TFModFortRiseLoaderAIModule.IsAgentPlaying(i, level)))
          continue;


        // Not noamel todo, InputName.Equals should continue when human (check with training on)
        if (TFModFortRiseLoaderAIModule.currentPlayerType[i] == "NONE" || TFModFortRiseLoaderAIModule.currentPlayerType[i] == "HUMAN")
        {
          continue;
        }

        listAgentByType[currentPlayerType[i]][i].Play();
      }
    }
  }

  [ModExportName("com.fortrise.TFModFortRiseLoaderAI")]
  public static class ModExports
  {
    public static bool addAgent(String type, Agent[] agents, bool forceAgent) {
      String newNameType = type;
      int index = 1;
      while (TFModFortRiseLoaderAIModule.listAgentByType.ContainsKey(newNameType)) {
        newNameType = type + "-" + index;
        index++;
      }
      TFModFortRiseLoaderAIModule.listAgentByType[newNameType] = agents;
      TFModFortRiseLoaderAIModule.listAgentType.Add(TFModFortRiseLoaderAIModule.listAgentType.Count, newNameType);

      for (var i = 0; i < agents.Length; i++) {

        TFModFortRiseLoaderAIModule.nbPlayerType[i]++;
        //Logger.Info("addAgent " + i + " " + TFGame.PlayerInputs[i].GetType().ToString() + " " + TFModFortRiseLoaderAIModule.currentPlayerType[i]);
        if (!forceAgent) {
          if (null != TFGame.PlayerInputs[i])
          {
            continue;
          }
          if (TFModFortRiseLoaderAIModule.currentPlayerType[i] != "NONE")
          {
            continue;
          }
        }


        if (!agents[i].isPlaying())
        {
          //TFGame.Players[i] = false;
          TFGame.PlayerInputs[i] = null;
          TFModFortRiseLoaderAIModule.currentPlayerType[i] = "NONE";
        }
        else
        {
          //TFGame.Players[i] = true;
          TFGame.PlayerInputs[i] = agents[i].getInput();
          TFModFortRiseLoaderAIModule.currentPlayerType[i] = newNameType;
        }
      }
      return true;
    }

    public static bool CurrentPlayerIs(String type, int playerIndex) {
      return TFModFortRiseLoaderAIModule.CurrentPlayerIs(type, playerIndex);
    }

    public static String GetPlayerTypePlaying(int playerIndex)
    {
      return TFModFortRiseLoaderAIModule.GetPlayerTypePlaying(playerIndex);
    }

    public static String GetPlayerName(int playerIndex)
    {
      return TFModFortRiseLoaderAIModule.GetPlayerName(playerIndex);
    }

    public static bool IsAgentPlaying(int playerIndex, Level level)
    {
      return TFModFortRiseLoaderAIModule.IsAgentPlaying(playerIndex, level);
    }

    public static bool CanAddAgent()
    {
      return TFModFortRiseLoaderAIModule.canAddAgent;
    }
  }
}
