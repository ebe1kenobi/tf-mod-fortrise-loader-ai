using System.Collections.Generic;
using TowerFall;

namespace TFModFortRiseLoaderAI;

public sealed class ApiImplementation : ILoaderAIModApi, ILoaderAIAgentsApi
{
  public ApiImplementation() {}

  public bool RegisterAgent(IList<ILoaderAIModApi.IAgentLogic> logic)
  {
    string type = logic[0].Type;
    Logger.Info($"TFModFortRiseLoaderAI RegisterAgent type={type}");

    if (TFModFortRiseLoaderAIModule.listAgentByType.ContainsKey(type))
      return false;

    // Autant d'agents que le jeu a d'emplacements - huit avec WiderSet - dans la
    // limite de ce que le mod d'IA a fourni comme logiques.
    int max = TFModFortRiseLoaderAIModule.PlayerSlots;
    if (logic.Count < max)
      max = logic.Count;

    Agent[] agents = new Agent[max];

    for (int i = 0; i < max; i++)
    {
      TFModFortRiseLoaderAIModule.nbPlayerType[i]++;

      var input = new Input(i);
      agents[i] = new AgentAdapter(logic[i], i, input);

      // Le tableau des entrees peut etre plus court que le nombre d'emplacements :
      // l'agent existe quand meme, il sera branche quand la place s'ouvrira.
      if (TFGame.PlayerInputs == null || i >= TFGame.PlayerInputs.Length)
      {
        continue;
      }

      if (null != TFGame.PlayerInputs[i])
      {
         continue;
      }

      TFGame.PlayerInputs[i] = agents[i].getInput();
      TFModFortRiseLoaderAIModule.currentPlayerType[i] = type;
    }

    TFModFortRiseLoaderAIModule.listAgentByType[type] = agents;
    TFModFortRiseLoaderAIModule.listAgentType.Add(
        TFModFortRiseLoaderAIModule.listAgentType.Count,
        type
    );

    Logger.Info($" TFModFortRiseLoaderAIModule.listAgentType.Count = {TFModFortRiseLoaderAIModule.listAgentType.Count}");
    Logger.Info($" agents.Count = {agents.Length}");


    return true;
  }


  //public bool addAgent(String type, String providerId, int[] playerIndices)
  //{
  //  String newNameType = type;
  //  int index = 1;
  //  while (TFModFortRiseLoaderAIModule.listAgentByType.ContainsKey(newNameType))
  //  {
  //    newNameType = type + "-" + index;
  //    index++;
  //  }
  //  var agents = new Agent[TFGame.Players.Length];
  //  var brain = TFModFortRiseLoaderAIModule.Instance.Context.Interop.GetApi<IAgentBrainModApi>(providerId);
  //  TFModFortRiseLoaderAIModule.listAgentByType[newNameType] = agents;
  //  TFModFortRiseLoaderAIModule.listAgentType.Add(TFModFortRiseLoaderAIModule.listAgentType.Count, newNameType);

  //  for (var p = 0; p < playerIndices.Length; p++)
  //  {
  //    var i = playerIndices[p];
  //    TFModFortRiseLoaderAIModule.nbPlayerType[i]++;
  //    if (TFGame.PlayerInputs[i] == null)
  //    {
  //      TFGame.PlayerInputs[i] = new Input(i);
  //    }
  //    agents[i] = new DelegatedAgent(i, newNameType, TFGame.PlayerInputs[i], brain);
  //    TFModFortRiseLoaderAIModule.currentPlayerType[i] = newNameType;
  //  }
  //  return true;
  //}

  //public bool CurrentPlayerIs(String type, int playerIndex)
  //{
  //  return TFModFortRiseLoaderAIModule.CurrentPlayerIs(type, playerIndex);
  //}

  //public String GetPlayerTypePlaying(int playerIndex)
  //{
  //  return TFModFortRiseLoaderAIModule.GetPlayerTypePlaying(playerIndex);
  //}

  //public String GetPlayerName(int playerIndex)
  //{
  //  //return TFModFortRiseLoaderAIModule.GetPlayerName(playerIndex);
  //  return TFModFortRiseLoaderAIModule.Instance.ProfilesModApi.GetPlayerName(playerIndex);
  //}

  //public bool IsAgentPlaying(int playerIndex, Level level)
  //{
  //  return TFModFortRiseLoaderAIModule.IsAgentPlaying(playerIndex, level);
  //}

  public bool CanAddAgent()
  {
    return TFModFortRiseLoaderAIModule.canAddAgent;
  }

  // ------------------------------------------------------------------
  // ILoaderAIAgentsApi
  // ------------------------------------------------------------------

  /// <summary>
  /// Les types enregistres, dans leur ordre d'enregistrement. listAgentType est une
  /// table indexee par rang plutot qu'une liste : on la parcourt par ses rangs pour
  /// garder cet ordre, que l'ecran de selection suit deja.
  /// </summary>
  public string[] GetAgentTypes()
  {
    var types = new List<string>();

    for (int i = 0; i < TFModFortRiseLoaderAIModule.listAgentType.Count; i++)
    {
      if (TFModFortRiseLoaderAIModule.listAgentType.TryGetValue(i, out string type)
          && !string.IsNullOrEmpty(type))
      {
        types.Add(type);
      }
    }

    return types.ToArray();
  }

  public string GetPlayerType(int playerIndex)
  {
    if (TFModFortRiseLoaderAIModule.currentPlayerType.TryGetValue(playerIndex, out string type)
        && !string.IsNullOrEmpty(type))
    {
      return type;
    }

    return HUMAN;
  }

  private const string HUMAN = "HUMAN";

  /// <summary>
  /// Bascule un emplacement. Ecrit le type ET rebranche l'entree : le premier decide
  /// qui joue, la seconde est ce que le jeu lit reellement. Ne poser que le type
  /// laissait l'agent choisi sans effet jusqu'a ce qu'autre chose rebranche l'entree.
  /// </summary>
  public bool SetPlayerType(int playerIndex, string type)
  {
    if (playerIndex < 0 || playerIndex >= TFModFortRiseLoaderAIModule.PlayerSlots)
    {
      return false;
    }

    if (string.IsNullOrEmpty(type))
    {
      type = HUMAN;
    }

    if (type != HUMAN && !TFModFortRiseLoaderAIModule.listAgentByType.ContainsKey(type))
    {
      // Type inconnu : le mod d'IA n'est pas installe, ou ne s'est pas encore
      // enregistre. On laisse l'emplacement tel quel plutot que de le casser.
      Logger.Info($"SetPlayerType({playerIndex}) : type '{type}' inconnu");
      return false;
    }

    // Revenir a l'humain n'a de sens que si son entree a ete mise de cote au
    // demarrage - sinon l'emplacement n'aurait plus rien pour le piloter.
    if (type == HUMAN && !TFModFortRiseLoaderAIModule.HumanControlExists(playerIndex))
    {
      return false;
    }

    TFModFortRiseLoaderAIModule.currentPlayerType[playerIndex] = type;

    PlayerInput input = TFModFortRiseLoaderAIModule.GetCurrentPlayerInput(playerIndex);

    if (input != null && TFGame.PlayerInputs != null
        && playerIndex < TFGame.PlayerInputs.Length)
    {
      TFGame.PlayerInputs[playerIndex] = input;
    }

    Logger.Info($"SetPlayerType({playerIndex}) -> {type}");
    return true;
  }
}
