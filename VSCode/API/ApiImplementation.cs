using System.Collections.Generic;
using TowerFall;

namespace TFModFortRiseLoaderAI;

public sealed class ApiImplementation : ILoaderAIModApi
{
  public ApiImplementation() {}

  public bool RegisterAgent(IList<ILoaderAIModApi.IAgentLogic> logic)
  {
    string type = logic[0].Type;
    Logger.Info($"TFModFortRiseLoaderAI RegisterAgent type={type}");

    if (TFModFortRiseLoaderAIModule.listAgentByType.ContainsKey(type))
      return false;

    int max = TFModFortRiseLoaderAIModule.EightPlayerMod ? 8 : 4;

    Agent[] agents = new Agent[max];

    for (int i = 0; i < max; i++)
    {
      TFModFortRiseLoaderAIModule.nbPlayerType[i]++;

      var input = new Input(i);
      agents[i] = new AgentAdapter(logic[i], i, input);

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
  //  return TFModFortRiseLoaderAIModule.Instance.CustomNameModApi.GetPlayerName(playerIndex);
  //}

  //public bool IsAgentPlaying(int playerIndex, Level level)
  //{
  //  return TFModFortRiseLoaderAIModule.IsAgentPlaying(playerIndex, level);
  //}

  public bool CanAddAgent()
  {
    return TFModFortRiseLoaderAIModule.canAddAgent;
  }
}
