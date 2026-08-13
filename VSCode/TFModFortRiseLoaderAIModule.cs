using System;
using System.Collections.Generic;
using System.Diagnostics;
using FortRise;
using Microsoft.Extensions.Logging;
using TowerFall;
using TFModFortRiseProfiles;

namespace TFModFortRiseLoaderAI
{
  public class TFModFortRiseLoaderAIModule : Mod
  {
    public static TFModFortRiseLoaderAIModule Instance;


    internal Type[] Hookables = [
        typeof(MyLevel),
        typeof(MyRollcallElement),
        typeof(MyTFGame), 
    ];

    public IProfilesModApi ProfilesModApi { get; set; }
    public ApiImplementation LoaderAIModApi { get; private set; }


    /// <summary>
    /// Nombre d'emplacements de joueur du jeu : 4 normalement, 8 quand le mod
    /// WiderSet est actif.
    ///
    /// Lu sur les tableaux du jeu plutot que demande a WiderSet : c'est LUI qui les
    /// agrandit, ils disent donc la verite sans qu'on ait a interroger un mod ni a
    /// deviner s'il est la. L'ancien drapeau EightPlayerMod, herite de l'interop
    /// FortRise 4, n'etait plus affecte par personne depuis la migration : il valait
    /// toujours faux, et les agents s'arretaient donc a quatre.
    ///
    /// Borne a 8 : les tables internes de ce mod (nbPlayerType, currentPlayerType)
    /// sont dimensionnees pour huit.
    /// </summary>
    public static int PlayerSlots
    {
      get
      {
        int slots = 4;

        if (TFGame.Players != null && TFGame.Players.Length > slots)
          slots = TFGame.Players.Length;

        if (TFGame.PlayerInputs != null && TFGame.PlayerInputs.Length > slots)
          slots = TFGame.PlayerInputs.Length;

        return slots > 8 ? 8 : slots;
      }
    }
    public static bool canAddAgent = false;
    public static Dictionary<int, String> currentPlayerType = new Dictionary<int, String>(8);
    public static Dictionary<int, PlayerInput> savedHumanPlayerInput = new Dictionary<int, PlayerInput>(8);
    public static bool isHumanPlayerTypeSaved = false;
    public static Dictionary<String, Agent[]> listAgentByType = new Dictionary<String, Agent[]>();
    public static int[] nbPlayerType = new int[8];
    public static Dictionary<int, String> listAgentType = new Dictionary<int,String>();
    public const string InputName = "TFModFortRiseLoaderAI.Input";

    public TFModFortRiseLoaderAIModule(IModContent content, IModuleContext context, ILogger logger) : base(content, context, logger)
    {
      if (!Debugger.IsAttached)
      {
        //Debugger.Launch(); // Proposera dâ€™attacher Visual Studio
      }
      Instance = this;
      TFModFortRiseLoaderAI.Logger.Init(logger);
      foreach (var hookable in Hookables)
      {
        hookable.GetMethod(nameof(IHookable.Load))!.Invoke(null, [context.Harmony]);
      }

      // Les noms de joueurs viennent du mod Profiles, qui a repris ce role a
      // CustomName. L'interop de FortRise construit son proxy sur la forme des
      // membres : il suffit que IProfilesModApi decrive ce que Profiles expose.
      ProfilesModApi = context.Interop.GetApi<IProfilesModApi>("Archer");
      LoaderAIModApi = new ApiImplementation();
    }

    public override object GetApi()
    {
      return new ApiImplementation();
    }

    public static bool IsAgentPlaying(int playerIndex, Level level)
    {
      return level.GetPlayer(playerIndex) != null;
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

    public static string GetAIPlayerName(int playerIndex)
    {
      String type = GetPlayerTypePlaying(playerIndex);
      if (type == "HUMAN") type = "HUMAN";
      if (type == "NONE") type = "NONE";
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
      {//todo bug when no ai mod (NONE)
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
        if (!TFGame.Players[i]) continue;
        if (null == TFGame.PlayerInputs[i]) continue;
        if (!InputName.Equals(TFGame.PlayerInputs[i].GetType().ToString())) continue;
        listAgentByType[currentPlayerType[i]][i].SetLevel(level);
      }
    }

    public static void AgentUpdate(Level level)
    {
      if (level.LivingPlayers == 0) return;

      for (int i = 0; i < TFGame.Players.Length; i++)
      {
        //TFModFortRiseLoaderAI.Logger.Info($"{i} TFGame.PlayerInputs[i].GetType().ToString() = {TFGame.PlayerInputs[i].GetType().ToString()}");
        if (!TFGame.Players[i])
        {
          continue;
        }

        if (!(InputName.Equals(TFGame.PlayerInputs[i].GetType().ToString())
            && TFModFortRiseLoaderAIModule.IsAgentPlaying(i, level))){
          //TFModFortRiseLoaderAI.Logger.Info($"{i} not playing");
          continue;

        }
        //TFModFortRiseLoaderAI.Logger.Info($"{i} playing");
        if (level.Paused) return;
        if (level.Frozen) return;
        if (level.Ending) return;
        listAgentByType[currentPlayerType[i]][i].Play();
      }
    }
  }
}
